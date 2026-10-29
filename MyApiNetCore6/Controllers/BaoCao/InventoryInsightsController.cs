using MyApiNetCore6.Services;
using Microsoft.Data.SqlClient;
using DatabaseTHP;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApiNetCore6.Data;

namespace MyApiNetCore6.Controllers;

[ApiController, Route("api/[controller]"), Authorize(Roles = "User")]
public class InventoryInsightsController : ControllerBase
{
    private readonly dbTrangHiepPhatContext db;
    private readonly ILogger<InventoryInsightsController> logger;
    public InventoryInsightsController(dbTrangHiepPhatContext context, ILogger<InventoryInsightsController> log)
    { db = context; logger = log; }

    [HttpPost("Report")]
    public async Task<IActionResult> Report(InventoryInsightRequest request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.AddHours(7).Date;
        var start = request.FROM.Date;
        var end = request.TO.Date;
        if (string.IsNullOrWhiteSpace(request.LOC_ID) || start > end || end > today || (end - start).TotalDays > 730
            || start < new DateTime(2000, 1, 1) || request.IDLE_DAYS < 1 || request.IDLE_DAYS > 3650
            || request.LEAD_DAYS < 0 || request.LEAD_DAYS > 365 || request.COVER_DAYS < 1 || request.COVER_DAYS > 365
            || !double.IsFinite(request.MIN_STOCK) || request.MIN_STOCK < 0 || request.MIN_STOCK > 1000000000
            || !new[] { "slow", "reorder", "sellers" }.Contains(request.MODE)
            || !new[] { "fast", "slow", "none" }.Contains(request.RANK))
            return Ok(new ApiResponse { Success = false, Message = "Điều kiện báo cáo không hợp lệ. Chọn kỳ tối đa 731 ngày, không vượt hôm nay và các ngưỡng hợp lệ." });
        var groups = (request.GROUPS ?? new()).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
        if (groups.Count > 1000) return Ok(new ApiResponse { Success = false, Message = "Chọn tối đa 1.000 nhóm." });
        var search = (request.SEARCH ?? "").Trim();
        var until = end.AddDays(1);
        var tomorrow = today.AddDays(1);
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        deadline.CancelAfter(TimeSpan.FromSeconds(90));
        cancellationToken = deadline.Token;
        try
        {
            // QTY and TONGSOLUONG use the inventory unit (smallest unit), as in OutputController.
            var scope = from stock in db.dm_HangHoa_Kho.AsNoTracking()
                        join product in db.dm_HangHoa.AsNoTracking() on new { stock.LOC_ID, ID = stock.ID_HANGHOA } equals new { product.LOC_ID, product.ID }
                        where stock.LOC_ID == request.LOC_ID && !product.ISCOMBO && !product.ISKHONGHIENTHITONKHO
                            && (request.INCLUDE_INACTIVE || product.ISACTIVE)
                            && (request.DEPOT == "" || request.DEPOT == null || stock.ID_KHO == request.DEPOT)
                            && (groups.Count == 0 || groups.Contains(product.ID_NHOMHANGHOA))
                            && (search == "" || product.MA.Contains(search) || product.NAME.Contains(search))
                        select new { stock.ID, stock.ID_KHO, stock.QTY, product.MA, product.NAME, product.ID_NHOMHANGHOA,
                            product.ID_DVT, product.ID_DVT_QD, product.TYLE_QD };
            var stockRows = await scope.ToListAsync(cancellationToken);
            var groupNames = await db.dm_NhomHangHoa.AsNoTracking().Where(x => x.LOC_ID == request.LOC_ID).ToDictionaryAsync(x => x.ID, x => x.NAME, cancellationToken);
            var depots = await db.dm_Kho.AsNoTracking().Where(x => x.LOC_ID == request.LOC_ID).ToDictionaryAsync(x => x.ID, x => x.NAME, cancellationToken);
            var units = await db.dm_DonViTinh.AsNoTracking().Where(x => x.LOC_ID == request.LOC_ID).ToDictionaryAsync(x => x.ID, x => x.NAME, cancellationToken);
            var totals = stockRows.Count == 0 ? new InventoryInsightTotals()
                : await InventoryInsightTotals.LoadAsync(db, request, groups, search, start, until, tomorrow, cancellationToken);
            var saleTotals = totals.Sales;
            var receiptTotals = totals.Receipts;
            var supplierReturns = totals.SupplierReturns;
            var days = (end - start).Days + 1;
            var rows = stockRows.Select(stock =>
            {
                saleTotals.TryGetValue(stock.ID, out var sold);
                receiptTotals.TryGetValue(stock.ID, out var received);
                supplierReturns.TryGetValue(stock.ID, out var supplierReturn);
                var converted = !string.IsNullOrEmpty(stock.ID_DVT_QD) && stock.TYLE_QD > 1;
                var net = (sold?.Qty ?? 0) - (received?.Returns ?? 0);
                var average = Math.Max(0, net) / days;
                DateTime? lastSale = sold?.Last;
                DateTime? idleSince = lastSale ?? received?.First;
                int? idleDays = idleSince.HasValue ? Math.Max(0, (today - idleSince.Value.Date).Days) : null;
                double? cover = average > 0 ? Math.Max(0, stock.QTY) / average : null;
                var idleTooLong = idleDays.HasValue && idleDays.Value >= request.IDLE_DAYS;
                var excessiveCover = cover.HasValue && cover.Value >= request.IDLE_DAYS;
                var threshold = average * request.LEAD_DAYS + request.MIN_STOCK;
                var target = average * (request.LEAD_DAYS + request.COVER_DAYS) + request.MIN_STOCK;
                var suggested = Math.Ceiling(Math.Max(0, target - stock.QTY));
                return new InventoryInsightRow
                {
                    ID = stock.ID, CODE = stock.MA, NAME = stock.NAME,
                    GROUP = Name(groupNames, stock.ID_NHOMHANGHOA), DEPOT = Name(depots, stock.ID_KHO),
                    UNIT = Name(units, converted ? stock.ID_DVT_QD : stock.ID_DVT),
                    BASE_UNIT = Name(units, stock.ID_DVT), RATIO = converted ? stock.TYLE_QD : 1,
                    STOCK = stock.QTY, SOLD = sold?.Qty ?? 0, RETURNED = received?.Returns ?? 0, NET_SOLD = net,
                    NET_SOLD_BASE = net / (converted ? stock.TYLE_QD : 1),
                    SUPPLIER_RETURNED = supplierReturn?.Qty ?? 0,
                    REVENUE = (sold?.Revenue ?? 0) - (received?.ReturnedRevenue ?? 0),
                    LAST_SALE = lastSale, LAST_RECEIPT = received?.Last, IDLE_DAYS = idleDays, DAILY_SALES = average,
                    COVER_DAYS = cover, REORDER_POINT = threshold, SUGGESTED = suggested,
                    SUGGESTED_BASE = Math.Ceiling(suggested / (converted ? stock.TYLE_QD : 1)),
                    SLOW = stock.QTY > 0 && (idleTooLong || excessiveCover),
                    NOTE = idleTooLong && excessiveCover ? "Lâu không bán và tồn đủ bán vượt ngưỡng"
                        : idleTooLong ? (lastSale.HasValue ? "Số ngày không bán đạt ngưỡng" : "Chưa bán; số ngày từ lần nhập đầu đạt ngưỡng")
                        : excessiveCover ? "Vẫn có bán; tồn đủ bán vượt ngưỡng"
                        : !idleDays.HasValue ? "Thiếu lịch sử để đánh giá" : "Chưa đạt ngưỡng chậm luân chuyển",
                    LOW = stock.QTY <= threshold,
                    STATUS = average <= 0 ? "Chưa đủ sức bán để dự báo" : stock.QTY <= 0 ? "Hết hàng" : stock.QTY <= threshold ? "Cần nhập thêm" : "Đủ hàng"
                };
            }).ToList();
            IEnumerable<InventoryInsightRow> filtered = rows;
            if (request.MODE == "slow")
                filtered = rows.Where(x => x.SLOW).OrderByDescending(x => x.IDLE_DAYS ?? int.MaxValue).ThenByDescending(x => x.STOCK);
            else if (request.MODE == "reorder")
                filtered = rows.Where(x => x.LOW).OrderBy(x => x.COVER_DAYS ?? double.MaxValue).ThenByDescending(x => x.SUGGESTED);
            else if (request.RANK == "none") filtered = rows.Where(x => x.SOLD == 0).OrderBy(x => x.CODE);
            // Sort numerically in the same base unit used by the displayed pack quantities.
            // Keep fractional packs and negative returns; never sort the formatted text.
            else if (request.RANK == "slow") filtered = rows.OrderBy(x => x.NET_SOLD_BASE).ThenBy(x => x.REVENUE).ThenBy(x => x.CODE);
            else filtered = rows.OrderByDescending(x => x.NET_SOLD_BASE).ThenByDescending(x => x.REVENUE).ThenBy(x => x.CODE);
            var result = filtered.ThenStable().ToList();
            if (request.MODE == "sellers")
                for (var i = 0; i < result.Count; i++) result[i].RANK_POSITION = i + 1;
            if (request.EXPORT && result.Count > 20000)
                return Ok(new ApiResponse { Success = false, Message = "Xuất tối đa 20.000 dòng/lần. Hãy chọn kho hoặc nhóm để thu hẹp báo cáo." });
            var pages = Math.Max(1, (int)Math.Ceiling(result.Count / 100d));
            var page = Math.Clamp(request.PAGE, 1, pages);
            return Ok(new ApiResponse { Success = true, Message = "Success", Data = new
            {
                ROWS = request.EXPORT ? result : result.Skip((page - 1) * 100).Take(100).ToList(),
                TOTAL = result.Count, PAGE = page, PAGES = pages, SNAPSHOT = DateTime.UtcNow.AddHours(7),
                SLOW_COUNT = rows.Count(x => x.SLOW), LOW_COUNT = rows.Count(x => x.LOW), NO_SALES_COUNT = rows.Count(x => x.SOLD == 0),
                MISSING_HISTORY_COUNT = rows.Count(x => x.STOCK > 0 && !x.IDLE_DAYS.HasValue)
            } });
        }
        catch (OperationCanceledException) when (deadline.IsCancellationRequested)
        {
            return Ok(new ApiResponse { Success = false, Message = "Đã dừng tải báo cáo hoặc truy vấn vượt 90 giây. Hãy chọn kho/nhóm hàng để thu hẹp phạm vi rồi thử lại." });
        }
        catch (SqlException ex) when (ex.Number == -2)
        {
            logger.LogWarning(ex, "Inventory report query timed out for branch {Branch}", request.LOC_ID);
            return Ok(new ApiResponse { Success = false, Message = "Truy vấn báo cáo quá thời gian chờ. Hãy chọn kho/nhóm hàng để thu hẹp phạm vi rồi thử lại; đây không phải kết quả không có dữ liệu." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Inventory report failed for branch {Branch}", request.LOC_ID);
            return Ok(new ApiResponse { Success = false, Message = "Không tải được báo cáo tồn kho. Vui lòng kiểm tra kết nối API và dữ liệu phiếu nhập/xuất." });
        }
    }

    private static string Name(Dictionary<string, string> source, string? id) => id != null && source.TryGetValue(id, out var name) ? name : "—";
}

public class InventoryInsightRequest
{
    public string LOC_ID { get; set; } = "";
    public string MODE { get; set; } = "slow";
    public string RANK { get; set; } = "fast";
    public string? DEPOT { get; set; }
    public List<string>? GROUPS { get; set; }
    public string? SEARCH { get; set; }
    public DateTime FROM { get; set; }
    public DateTime TO { get; set; }
    public int IDLE_DAYS { get; set; } = 90;
    public int LEAD_DAYS { get; set; } = 7;
    public int COVER_DAYS { get; set; } = 30;
    public double MIN_STOCK { get; set; }
    public bool INCLUDE_INACTIVE { get; set; }
    public int PAGE { get; set; } = 1;
    public bool EXPORT { get; set; }
}

public class InventoryInsightRow
{
    public string ID { get; set; } = "";
    public string CODE { get; set; } = "";
    public string NAME { get; set; } = "";
    public string GROUP { get; set; } = "";
    public string DEPOT { get; set; } = "";
    public string UNIT { get; set; } = "";
    public string BASE_UNIT { get; set; } = "";
    public double RATIO { get; set; }
    public double STOCK { get; set; }
    public double SOLD { get; set; }
    public double RETURNED { get; set; }
    public double SUPPLIER_RETURNED { get; set; }
    public double NET_SOLD { get; set; }
    public double NET_SOLD_BASE { get; set; }
    public int? RANK_POSITION { get; set; }
    public double REVENUE { get; set; }
    public DateTime? LAST_SALE { get; set; }
    public DateTime? LAST_RECEIPT { get; set; }
    public int? IDLE_DAYS { get; set; }
    public double DAILY_SALES { get; set; }
    public double? COVER_DAYS { get; set; }
    public double REORDER_POINT { get; set; }
    public double SUGGESTED { get; set; }
    public double SUGGESTED_BASE { get; set; }
    public bool SLOW { get; set; }
    public string NOTE { get; set; } = "";
    public bool LOW { get; set; }
    public string STATUS { get; set; } = "";
}

internal static class InventoryInsightOrdering
{
    public static IEnumerable<InventoryInsightRow> ThenStable(this IEnumerable<InventoryInsightRow> rows)
        => rows is IOrderedEnumerable<InventoryInsightRow> ordered ? ordered.ThenBy(x => x.ID) : rows.OrderBy(x => x.ID);
}
