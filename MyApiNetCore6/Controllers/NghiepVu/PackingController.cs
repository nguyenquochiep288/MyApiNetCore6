using System.Data.Common;
using System.Text.Json;
using Dapper;
using DatabaseTHP;
using DatabaseTHP.Class;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyApiNetCore6.Data;
using MyApiNetCore6.Services;

namespace MyApiNetCore6.Controllers;

[ApiController, Route("api/Packing/{loc}"), Authorize(Roles = "User")]
public class PackingController : ControllerBase
{
    private const string TypeId = "THP-DONG-GOI";
    // Use the saved issue voucher so old orders with standard PX codes get the same DG format.
    private const string OrderCodesSql = @"
        CROSS APPLY (VALUES (CONCAT('DG-', LOWER(REPLACE(CONVERT(varchar(36),p.ID),'-',''))))) legacy(Code)
        CROSS APPLY (VALUES (CASE
            WHEN p.OutputCode LIKE 'PX-[0-9][0-9][0-9][0-9][0-9][0-9]-%'
                AND LEN(p.OutputCode) BETWEEN 15 AND 19
                AND SUBSTRING(p.OutputCode,11,9) NOT LIKE '%[^0-9]%'
            THEN CONCAT('DG',SUBSTRING(p.OutputCode,3,17))
            ELSE legacy.Code END)) orderCode(Code)";
    private readonly dbTrangHiepPhatContext db;
    private readonly IConfiguration configuration;
    private readonly ILogger<PackingController> logger;
    private DbConnection Connection => db.Database.GetDbConnection();
    private DbTransaction? Transaction => db.Database.CurrentTransaction?.GetDbTransaction();

    public PackingController(dbTrangHiepPhatContext db, IConfiguration configuration, ILogger<PackingController> logger)
    {
        this.db = db;
        this.configuration = configuration;
        this.logger = logger;
    }

    private async Task<string?> Operator(string loc, string? catalog = null, string? permission = null)
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(loc) || loc.Length > 50) return null;
        var user = await db.view_AspNetUsers.Where(x => x.UserName == username)
            .Select(x => new { x.ID, x.ID_NHOMQUYEN }).FirstOrDefaultAsync();
        if (user == null) return null;
        if (user.ID_NHOMQUYEN == "-1") return user.ID;
        var group = await db.web_NhomQuyen.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ID == user.ID_NHOMQUYEN && x.LOC_ID == loc && x.ISACTIVE);
        if (group == null) return null;
        if (!group.ISPHANQUYEN) return user.ID;
        var rights = db.view_web_PhanQuyen.Where(x => x.LOC_ID == loc && x.ID_NHOMQUYEN == group.ID && x.TRANGTHAI);
        if (catalog != null)
            return await rights.AnyAsync(x => x.CONTROLLERNAME == catalog && x.MAQUYEN == permission) ? user.ID : null;
        return await rights.AnyAsync(x => x.CONTROLLERNAME == "Inventory" && x.MAQUYEN == "View")
            && await rights.AnyAsync(x => x.CONTROLLERNAME == "Input" && x.MAQUYEN == "Create")
            && await rights.AnyAsync(x => x.CONTROLLERNAME == "Output" && x.MAQUYEN == "Create") ? user.ID : null;
    }

    [HttpGet("type-flag/{direction}")]
    public async Task<IActionResult> TypeFlag(string loc, string direction)
    {
        var catalog = Catalog(direction);
        if (catalog == null) return Invalid("Loại phiếu không hợp lệ.");
        if (await Operator(loc, catalog, "View") == null) return Forbid();
        return Ok(new { enabled = await ReadTypeFlag(loc, direction) });
    }

    [HttpPost("type-flag/{direction}")]
    public async Task<IActionResult> SetTypeFlag(string loc, string direction, PackingFlag request)
    {
        var catalog = Catalog(direction);
        if (catalog == null) return Invalid("Loại phiếu không hợp lệ.");
        var user = await Operator(loc, catalog, "Edit");
        if (user == null) return Forbid();
        try
        {
            using var tx = await db.Database.BeginTransactionAsync();
            await LockTypes(loc);
            var changed = await Connection.ExecuteAsync(direction == "output"
                ? @"UPDATE dbo.dm_LoaiPhieuXuat SET ISXUATNGUYENLIEU=@enabled, THOIGIANSUA=@now, ID_NGUOISUA=@user
                    WHERE LOC_ID=@loc AND ID=@id AND ISACTIVE=1 AND ISSYSTEM=0 AND TYPE=0 AND ISTRAHANG=0"
                : @"UPDATE dbo.dm_LoaiPhieuNhap SET ISNHAPTHANHPHAM=@enabled, THOIGIANSUA=@now, ID_NGUOISUA=@user
                    WHERE LOC_ID=@loc AND ID=@id AND ISACTIVE=1 AND ISSYSTEM=0 AND TYPE=0 AND ISNHAPTRAHANG=0",
                new { loc, id = TypeId, request.Enabled, user, now = DateTime.UtcNow.AddHours(7) }, Transaction);
            if (changed != 1) return Invalid("Loại phiếu đóng gói là loại hệ thống hoặc không còn hợp lệ để sửa.");
            await tx.CommitAsync();
            return Ok(new { request.Enabled });
        }
        catch (Exception ex) { return SaveError(ex, loc); }
    }

    [HttpGet("stocks")]
    public async Task<IActionResult> Stocks(string loc, string depot, string? search)
    {
        if (await Operator(loc) == null) return Forbid();
        if (!ValidId(depot) || search?.Length > 150) return Invalid("Điều kiện tìm hàng không hợp lệ.");
        var query = from stock in db.dm_HangHoa_Kho
                    join product in db.dm_HangHoa on new { stock.LOC_ID, ID = stock.ID_HANGHOA } equals new { product.LOC_ID, product.ID }
                    where stock.LOC_ID == loc && stock.ID_KHO == depot && product.ISACTIVE
                        && product.LOAIHANGHOA != "2" && !product.ISCOMBO
                        && (string.IsNullOrEmpty(search) || product.MA.Contains(search) || product.NAME.Contains(search))
                    orderby product.MA, stock.ID
                    let unit = product.TYLE_QD > 1 && product.ID_DVT_QD != null && product.ID_DVT_QD != "" ? product.ID_DVT_QD : product.ID_DVT
                    select new { id = stock.ID, code = product.MA, name = product.NAME, quantity = stock.QTY,
                        unitId = unit,
                        unit = db.dm_DonViTinh.Where(x => x.LOC_ID == loc && x.ID == unit).Select(x => x.NAME).FirstOrDefault(),
                        largeUnitId = product.ID_DVT,
                        largeUnit = db.dm_DonViTinh.Where(x => x.LOC_ID == loc && x.ID == product.ID_DVT).Select(x => x.NAME).FirstOrDefault(),
                        ratio = product.TYLE_QD };
        return Ok(await query.Take(60).ToListAsync());
    }

    [HttpGet("orders")]
    public async Task<IActionResult> Orders(string loc, string depot, int page = 1)
    {
        if (await Operator(loc) == null) return Forbid();
        if (!ValidId(depot) || page is < 1 or > 100000) return Invalid("Điều kiện tra cứu không hợp lệ.");
        const int size = 30;
        return Ok(await Connection.QueryAsync($@"
            SELECT p.ID,orderCode.Code,p.OutputCode,p.InputCode,p.SourceKg,p.CreatedAt,p.ReceivedAt,
                CONVERT(bit,CASE WHEN p.InputId IS NOT NULL OR p.ClosedWithoutReceipt=1 THEN 1 ELSE 0 END) AS Closed,
                COALESCE((SELECT SUM(d.Kg) FROM dbo.ct_LenhDongGoi_Dot d WHERE d.LOC_ID=p.LOC_ID AND d.OrderId=p.ID AND d.Direction=1),0) AS PartialKg
            FROM dbo.ct_LenhDongGoi p {OrderCodesSql} WHERE p.LOC_ID=@loc AND p.Depot=@depot
            ORDER BY p.CreatedAt DESC,p.ID DESC OFFSET @offset ROWS FETCH NEXT @size ROWS ONLY
            ", new { loc, depot, offset = (page - 1) * size, size }));
    }

    [HttpGet("orders/{id:guid}")]
    public async Task<IActionResult> Detail(string loc, Guid id)
    {
        if (await Operator(loc) == null) return Forbid();
        var order = await Load(loc, id);
        if (order == null) return NotFound(new { message = "Không tìm thấy lệnh đóng gói." });
        return Ok(await Describe(loc, order));
    }

    [HttpGet("trace")]
    public async Task<IActionResult> Trace(string loc, string? depot, string? search, DateTime? from, DateTime? to,
        string status = "all", int page = 1, bool export = false, bool lineDetail = false, int? direction = null)
    {
        if (await Operator(loc) == null) return Forbid();
        if (depot?.Length > 50 || search?.Length > 150 || page is < 1 or > 100000
            || status is not ("all" or "pending" or "completed" or "difference")
            || direction is not (null or 0 or 1)
            || from?.Year < 2000 || to?.Year < 2000 || from?.Date > to?.Date || to?.Date == DateTime.MaxValue.Date)
            return Invalid("Điều kiện tra cứu không hợp lệ.");
        const int size = 30;
        const int exportLimit = 10000;
        using var report = await Connection.QueryMultipleAsync($@"
            SELECT p.ID, orderCode.Code, k.NAME AS DepotName,v.OutputCode,v.InputCode,p.SourceKg,p.CreatedAt,v.LastInputAt AS ReceivedAt,
                c.SourceName, state.CostState, closing.Closed,
                v.VoucherCount,
                CASE WHEN closing.Closed=1 THEN p.SourceKg-p.LossKg ELSE b.ReceivedKg END AS FinishedKg,
                p.LossKg,
                CASE WHEN closing.Closed=1 THEN CONVERT(decimal(18,4),100.0*p.LossKg/NULLIF(p.SourceKg,0)) END AS LossRate,
                CASE WHEN closing.Closed=0 THEN c.CurrentCost ELSE p.MaterialCost END AS MaterialCost,
                CASE WHEN p.InputId IS NULL THEN b.PackageCost ELSE packing.PackageCost+COALESCE(b.PackageCost,0) END AS PackageCost,
                p.Labor, p.LossCost,
                CASE WHEN p.InputId IS NULL THEN b.ReceiptValue ELSE CONVERT(decimal(28,2),n.TONGTHANHTIEN)+COALESCE(b.ReceiptValue,0) END AS ReceiptValue
            INTO #PackingReport
            FROM dbo.ct_LenhDongGoi p
            {OrderCodesSql}
            CROSS APPLY (VALUES (CONVERT(bit,CASE WHEN p.InputId IS NOT NULL OR p.ClosedWithoutReceipt=1 THEN 1 ELSE 0 END))) closing(Closed)
            LEFT JOIN dbo.dm_Kho k ON k.LOC_ID=p.LOC_ID AND k.ID=p.Depot
            LEFT JOIN dbo.ct_PhieuNhap n ON n.LOC_ID=p.LOC_ID AND n.ID=p.InputId
            OUTER APPLY (
                SELECT SUM(d.Kg) AS ReceivedKg, SUM(d.PackageCost) AS PackageCost, SUM(d.ReceiptValue) AS ReceiptValue
                FROM dbo.ct_LenhDongGoi_Dot d WHERE d.LOC_ID=p.LOC_ID AND d.OrderId=p.ID AND d.Direction=1
            ) b
            OUTER APPLY (
                SELECT STRING_AGG(CASE WHEN f.Direction=0 THEN CONVERT(nvarchar(max),f.VoucherCode) END, ', ')
                        WITHIN GROUP (ORDER BY f.PostedAt,f.VoucherCode) AS OutputCode,
                    STRING_AGG(CASE WHEN f.Direction=1 THEN CONVERT(nvarchar(max),f.VoucherCode) END, ', ')
                        WITHIN GROUP (ORDER BY f.PostedAt,f.VoucherCode) AS InputCode,
                    COUNT(*) AS VoucherCount, MAX(CASE WHEN f.Direction=1 THEN f.PostedAt END) AS LastInputAt
                FROM dbo.v_LenhDongGoi_Phieu f WHERE f.LOC_ID=p.LOC_ID AND f.OrderId=p.ID
            ) v
            OUTER APPLY (
                SELECT CASE WHEN COUNT(Cost)=COUNT(*) THEN SUM(Cost) END AS CurrentCost,
                    STRING_AGG(CONVERT(nvarchar(max),CONCAT(Code,' · ',Name)), ' | ') AS SourceName
                FROM dbo.v_LenhDongGoi_NguyenLieu WHERE LOC_ID=p.LOC_ID AND OrderId=p.ID
            ) c
            OUTER APPLY (
                SELECT CASE WHEN COUNT(*)=COUNT(Quantity*PackageCost)
                    THEN SUM(CONVERT(decimal(28,2),ROUND(Quantity*PackageCost,2))) END AS PackageCost
                FROM OPENJSON(CASE WHEN p.InputId IS NOT NULL THEN p.ReceiptJson END)
                    WITH (Quantity decimal(18,6) '$.Quantity', PackageCost decimal(18,6) '$.PackageCost')
            ) packing
            CROSS APPLY (VALUES (CASE WHEN closing.Closed=0 THEN CASE WHEN b.ReceivedKg IS NULL THEN 'pending' ELSE 'partial' END
                WHEN c.CurrentCost IS NULL OR p.MaterialCost IS NULL
                    OR (p.InputId IS NOT NULL AND (packing.PackageCost IS NULL OR n.TONGTHANHTIEN IS NULL))
                    OR p.LossKg IS NULL OR p.LossCost IS NULL THEN 'missing'
                WHEN c.CurrentCost<>p.MaterialCost THEN 'difference' ELSE 'completed' END)) state(CostState)
            WHERE p.LOC_ID=@loc AND (@depot IS NULL OR p.Depot=@depot)
                AND ((@lineDetail=0 AND (@from IS NULL OR p.CreatedAt>=@from) AND (@until IS NULL OR p.CreatedAt<@until))
                    OR (@lineDetail=1 AND EXISTS (
                        SELECT 1 FROM dbo.v_LenhDongGoi_Phieu period
                        WHERE period.LOC_ID=p.LOC_ID AND period.OrderId=p.ID
                            AND (@direction IS NULL OR period.Direction=@direction)
                            AND (@from IS NULL OR period.PostedAt>=@from) AND (@until IS NULL OR period.PostedAt<@until))))
                AND (@status='all' OR (@status='pending' AND closing.Closed=0) OR (@status='completed' AND closing.Closed=1)
                    OR (@status='difference' AND state.CostState IN ('missing','difference')))
                AND (@search='' OR CHARINDEX(@search, v.OutputCode)>0 OR CHARINDEX(@search,v.InputCode)>0
                    OR CHARINDEX(@search,orderCode.Code)>0 OR CHARINDEX(@search,legacy.Code)>0
                    OR CHARINDEX(@search,c.SourceName)>0
                    OR EXISTS (SELECT 1 FROM OPENJSON(p.PlanJson)
                        WITH (Code nvarchar(350) '$.Code', Name nvarchar(350) '$.Name') a
                        WHERE CHARINDEX(@search,a.Code)>0 OR CHARINDEX(@search,a.Name)>0));"
            + (lineDetail ? LineReportSql : @"
            SELECT COUNT(*) AS OrderCount,
                COUNT(CASE WHEN CostState IN ('pending','partial') THEN 1 END) AS PendingCount,
                COUNT(CASE WHEN CostState NOT IN ('pending','partial') THEN 1 END) AS CompletedCount,
                COUNT(CASE WHEN CostState IN ('missing','difference') THEN 1 END) AS ReviewCount,
                COALESCE(SUM(SourceKg),0) AS SourceKg,
                COALESCE(SUM(CASE WHEN CostState IN ('pending','partial') THEN SourceKg-COALESCE(FinishedKg,0) END),0) AS PendingKg,
                CASE WHEN COUNT(FinishedKg)=COUNT(CASE WHEN CostState<>'pending' THEN 1 END) THEN SUM(FinishedKg) END AS FinishedKg,
                CASE WHEN COUNT(LossKg)=COUNT(CASE WHEN CostState NOT IN ('pending','partial') THEN 1 END) THEN SUM(LossKg) END AS LossKg,
                CASE WHEN COUNT(LossKg)=COUNT(CASE WHEN CostState NOT IN ('pending','partial') THEN 1 END)
                    THEN CONVERT(decimal(18,4),100.0*SUM(LossKg)/NULLIF(SUM(CASE WHEN CostState NOT IN ('pending','partial') THEN SourceKg END),0)) END AS LossRate,
                CASE WHEN COUNT(CASE WHEN CostState NOT IN ('pending','partial') THEN MaterialCost END)=COUNT(CASE WHEN CostState NOT IN ('pending','partial') THEN 1 END)
                    THEN SUM(CASE WHEN CostState NOT IN ('pending','partial') THEN MaterialCost END) END AS MaterialCost,
                CASE WHEN COUNT(PackageCost)=COUNT(CASE WHEN CostState<>'pending' THEN 1 END) THEN SUM(PackageCost) END AS PackageCost,
                SUM(CASE WHEN CostState NOT IN ('pending','partial') THEN Labor END) AS Labor,
                CASE WHEN COUNT(LossCost)=COUNT(CASE WHEN CostState NOT IN ('pending','partial') THEN 1 END) THEN SUM(LossCost) END AS LossCost,
                CASE WHEN COUNT(ReceiptValue)=COUNT(CASE WHEN CostState<>'pending' THEN 1 END) THEN SUM(ReceiptValue) END AS ReceiptValue
            FROM #PackingReport;
            SELECT * FROM #PackingReport
            ORDER BY CreatedAt DESC,ID DESC OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY;"),
            new { loc, depot = string.IsNullOrWhiteSpace(depot) ? null : depot, search = search?.Trim() ?? "",
                from = from?.Date, until = to?.Date.AddDays(1), status, direction, lineDetail,
                offset = export ? 0 : (page - 1) * size, limit = export ? exportLimit + 1 : size + 1 });
        var summary = (IDictionary<string, object>)await report.ReadSingleAsync();
        if (export && (int)summary[lineDetail ? "LineCount" : "OrderCount"] > exportLimit)
            return Invalid(lineDetail ? "Kết quả vượt 10.000 dòng. Thu hẹp bộ lọc để xuất Excel."
                : "Kết quả vượt 10.000 lệnh. Thu hẹp bộ lọc để xuất Excel.");
        var rows = (await report.ReadAsync()).ToList();
        return Ok(new { summary, rows = rows.Take(export ? exportLimit : size), more = !export && rows.Count > size });
    }

    private const string LineReportSql = @"
        SELECT * INTO #PackingReportLines FROM (
            SELECT r.ID,r.Code,r.DepotName,r.CostState,f.Direction,f.VoucherCode,f.PostedAt,
                COALESCE(p.MA COLLATE DATABASE_DEFAULT,s.StockId COLLATE DATABASE_DEFAULT) AS ItemCode,
                p.NAME COLLATE DATABASE_DEFAULT AS ItemName,
                COALESCE(u.NAME COLLATE DATABASE_DEFAULT,d.ID_DVT COLLATE DATABASE_DEFAULT) AS Unit,
                CONVERT(decimal(28,6),d.SOLUONG) AS Quantity,s.Kg,
                CASE WHEN d.ISGIAVONDATINH=1 AND d.GIAVON>=0 THEN CONVERT(decimal(28,6),d.GIAVON*d.TYLE_QD) END AS UnitCost,
                CASE WHEN d.ISGIAVONDATINH=1 AND d.GIAVON>=0 THEN CONVERT(decimal(28,2),ROUND(d.SOLUONG*d.TYLE_QD*d.GIAVON,2)) END AS LineValue,
                d.STT AS LineNumber,d.ID COLLATE DATABASE_DEFAULT AS LineKey
            FROM #PackingReport r
            JOIN dbo.ct_LenhDongGoi_NguyenLieu s ON s.LOC_ID=@loc AND s.OrderId=r.ID
            JOIN dbo.ct_PhieuXuat_ChiTiet d ON d.LOC_ID=s.LOC_ID AND d.ID=s.DetailId
            JOIN dbo.v_LenhDongGoi_Phieu f ON f.LOC_ID=s.LOC_ID AND f.OrderId=r.ID AND f.Direction=0 AND f.VoucherId=d.ID_PHIEUXUAT
            LEFT JOIN dbo.dm_HangHoa_Kho k ON k.LOC_ID=s.LOC_ID AND k.ID=s.StockId
            LEFT JOIN dbo.dm_HangHoa p ON p.LOC_ID=k.LOC_ID AND p.ID=k.ID_HANGHOA
            LEFT JOIN dbo.dm_DonViTinh u ON u.LOC_ID=d.LOC_ID AND u.ID=d.ID_DVT
            WHERE (@direction IS NULL OR @direction=0)
                AND (@from IS NULL OR f.PostedAt>=@from) AND (@until IS NULL OR f.PostedAt<@until)
            UNION ALL
            SELECT r.ID,r.Code,r.DepotName,r.CostState,f.Direction,f.VoucherCode,f.PostedAt,
                a.Code COLLATE DATABASE_DEFAULT,a.Name COLLATE DATABASE_DEFAULT,
                COALESCE(a.UnitName,a.UnitId) COLLATE DATABASE_DEFAULT,a.Quantity,
                CONVERT(decimal(18,6),a.Quantity*a.KgPerUnit),
                a.UnitCost,CONVERT(decimal(28,2),ROUND(a.Quantity*a.UnitCost,2)),
                TRY_CONVERT(int,j.[key])+1,j.[key] COLLATE DATABASE_DEFAULT
            FROM #PackingReport r
            JOIN dbo.ct_LenhDongGoi p ON p.LOC_ID=@loc AND p.ID=r.ID
            JOIN dbo.v_LenhDongGoi_Phieu f ON f.LOC_ID=p.LOC_ID AND f.OrderId=p.ID AND f.Direction=1
            LEFT JOIN dbo.ct_LenhDongGoi_Dot b ON b.LOC_ID=f.LOC_ID AND b.OrderId=f.OrderId
                AND b.Direction=1 AND b.VoucherId=f.VoucherId
            CROSS APPLY OPENJSON(CASE WHEN b.VoucherId IS NULL THEN p.ReceiptJson ELSE b.ReceiptJson END) j
            CROSS APPLY OPENJSON(j.value) WITH (
                Code nvarchar(350) '$.Code', Name nvarchar(350) '$.Name', UnitId nvarchar(50) '$.UnitId',
                UnitName nvarchar(350) '$.UnitName', Quantity decimal(18,6) '$.Quantity',
                KgPerUnit decimal(18,6) '$.KgPerUnit', UnitCost decimal(18,6) '$.UnitCost') a
            WHERE (@direction IS NULL OR @direction=1) AND a.Quantity>0
                AND (@from IS NULL OR f.PostedAt>=@from) AND (@until IS NULL OR f.PostedAt<@until)
        ) d;
        SELECT COUNT(*) AS LineCount,COUNT(DISTINCT ID) AS OrderCount,
            COALESCE(SUM(CASE WHEN Direction=0 THEN Kg END),0) AS OutputKg,
            COALESCE(SUM(CASE WHEN Direction=1 THEN Kg END),0) AS InputKg,
            SUM(CASE WHEN Direction=0 THEN LineValue END) AS KnownOutputCost,
            SUM(CASE WHEN Direction=1 THEN LineValue END) AS InputValue,
            COUNT(CASE WHEN LineValue IS NULL THEN 1 END) AS MissingCostCount
        FROM #PackingReportLines;
        SELECT * FROM #PackingReportLines
        ORDER BY PostedAt DESC,ID DESC,Direction,LEN(VoucherCode),VoucherCode,LineNumber,LineKey
        OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY;";

    [HttpPost("orders/{id:guid}/delete")]
    public async Task<IActionResult> Delete(string loc, Guid id, PackingDelete request)
    {
        if (await Operator(loc) == null || await Operator(loc, "Output", "Delete") == null
            || await Operator(loc, "Input", "Delete") == null) return Forbid();

        try
        {
            using var tx = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            await LockOrder(loc, id);
            var order = await Load(loc, id);
            if (order == null) return Ok(new { deleted = true, orderDeleted = true });
            var links = (await Connection.QueryAsync<PackingVoucher>(@"
                SELECT Direction,VoucherId,VoucherCode,PostedAt FROM dbo.v_LenhDongGoi_Phieu
                WHERE LOC_ID=@loc AND OrderId=@id", new { loc, id }, Transaction)).AsList();
            if (request.Received != order.Closed ||
                (request.VoucherCount ?? (request.Received ? 2 : 1)) != links.Count)
                return Conflict(new { message = "Trạng thái lệnh đã thay đổi. Tải lại danh sách trước khi xóa." });

            var allOutputIds = links.Where(x => x.Direction == 0).Select(x => x.VoucherId).ToArray();
            var allInputIds = links.Where(x => x.Direction == 1).Select(x => x.VoucherId).ToArray();
            PackingVoucher? single = null;
            if (request.VoucherId != null)
            {
                single = links.SingleOrDefault(x => SameId(x.VoucherId, request.VoucherId));
                if (single == null) return NotFound(new { message = "Phiếu không còn thuộc lệnh. Tải lại chi tiết." });
                if (single.Direction == 0 && allInputIds.Length != 0)
                    return Invalid("Cần xóa các phiếu nhập của lệnh trước khi xóa phiếu xuất.");
                var latest = links.Where(x => x.Direction == single.Direction).OrderByDescending(x => x.PostedAt)
                    .ThenByDescending(x => x.VoucherCode.Length).ThenByDescending(x => x.VoucherCode, StringComparer.Ordinal).First();
                if (single != latest || (single.Direction == 0 && single.VoucherId == order.OutputId && allOutputIds.Length > 1))
                    return Invalid("Hãy xóa phiếu mới nhất cùng loại trước.");
            }
            var orderDeleted = single == null || links.Count == 1;
            var targetLinks = single == null ? links : new List<PackingVoucher> { single };
            var outputIds = targetLinks.Where(x => x.Direction == 0).Select(x => x.VoucherId).ToArray();
            var inputIds = targetLinks.Where(x => x.Direction == 1).Select(x => x.VoucherId).ToArray();
            var outputs = await db.ct_PhieuXuat.Where(x => x.LOC_ID == loc && outputIds.Contains(x.ID)).ToListAsync();
            var inputs = await db.ct_PhieuNhap.Where(x => x.LOC_ID == loc && inputIds.Contains(x.ID)).ToListAsync();
            var issued = await db.ct_PhieuXuat_ChiTiet.Where(x => x.LOC_ID == loc && outputIds.Contains(x.ID_PHIEUXUAT)).ToListAsync();
            var received = await db.ct_PhieuNhap_ChiTiet.Where(x => x.LOC_ID == loc && inputIds.Contains(x.ID_PHIEUNHAP)).ToListAsync();
            if (outputs.Count != outputIds.Length || inputs.Count != inputIds.Length ||
                outputs.Any(x => x.ID_LOAIPHIEUXUAT != TypeId || !issued.Any(d => d.ID_PHIEUXUAT == x.ID)) ||
                inputs.Any(x => x.ID_LOAIPHIEUNHAP != TypeId || !received.Any(d => d.ID_PHIEUNHAP == x.ID)))
                return Invalid("Thiếu phiếu đóng gói hoặc chi tiết phiếu. Kiểm tra dữ liệu trước khi xóa.");

            var issueDays = outputs.ToDictionary(x => x.ID, x => (x.THOIGIANTHEM ?? order.CreatedAt).Date);
            var receiptDays = inputs.ToDictionary(x => x.ID, x => (x.THOIGIANTHEM ?? order.ReceivedAt ?? order.CreatedAt).Date);
            var changes = issued.Select(x => (Stock: x.ID_HANGHOAKHO, Delta: x.SOLUONG * x.TYLE_QD, Day: issueDays[x.ID_PHIEUXUAT]))
                .Concat(received.Select(x => (Stock: x.ID_HANGHOAKHO, Delta: -x.SOLUONG * x.TYLE_QD, Day: receiptDays[x.ID_PHIEUNHAP])))
                .GroupBy(x => x.Stock, StringComparer.OrdinalIgnoreCase).OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase);
            foreach (var change in changes)
            {
                await StockCostPosting.LockStock(db, loc, change.Key);
                var stock = await db.dm_HangHoa_Kho.FindAsync(change.Key, loc);
                var opening = await db.Set<OpeningStockCost>().FindAsync(loc, change.Key);
                if (stock == null || opening == null || opening.TUNGAY > change.Min(x => x.Day))
                    return Invalid("Không tìm thấy tồn kho hoặc mốc giá vốn hợp lệ để hoàn lệnh.");
                var quantity = stock.QTY + change.Sum(x => x.Delta);
                if (quantity < -0.000001) return Invalid("Tồn kho thành phẩm không đủ để hoàn lệnh.");
                stock.QTY = Math.Max(0, quantity);
            }

            if (await HasDeleteDependencies(loc, id, order, targetLinks, outputIds, inputIds, allOutputIds, allInputIds,
                issued, received, issueDays.Values.DefaultIfEmpty(order.CreatedAt.Date).Min(),
                receiptDays.Values.DefaultIfEmpty(order.CreatedAt.Date).Min()))
                return Invalid("Đã có chứng từ liên quan hoặc hàng đã phát sinh xuất/đóng gói tiếp. Không thể xóa.");

            // Remove the links first so the existing voucher guards still protect every other entry point.
            if (orderDeleted)
                await Connection.ExecuteAsync(@"
                    DELETE dbo.ct_LenhDongGoi_NguyenLieu WHERE LOC_ID=@loc AND OrderId=@id;
                    DELETE dbo.ct_LenhDongGoi_Dot WHERE LOC_ID=@loc AND OrderId=@id;
                    DELETE dbo.ct_LenhDongGoi WHERE LOC_ID=@loc AND ID=@id;", new { loc, id }, Transaction);
            else if (single!.Direction == 0)
                await Connection.ExecuteAsync(@"
                    UPDATE o SET SourceKg=o.SourceKg-d.Kg
                    FROM dbo.ct_LenhDongGoi o JOIN dbo.ct_LenhDongGoi_Dot d ON d.LOC_ID=o.LOC_ID AND d.OrderId=o.ID
                    WHERE o.LOC_ID=@loc AND o.ID=@id AND d.Direction=0 AND d.VoucherId=@voucherId;
                    DELETE dbo.ct_LenhDongGoi_NguyenLieu WHERE LOC_ID=@loc AND OrderId=@id AND DetailId IN @detailIds;
                    DELETE dbo.ct_LenhDongGoi_Dot WHERE LOC_ID=@loc AND OrderId=@id AND Direction=0 AND VoucherId=@voucherId;",
                    new { loc, id, single.VoucherId, detailIds = issued.Select(x => x.ID).ToArray() }, Transaction);
            else
                await Connection.ExecuteAsync(@"
                    DELETE dbo.ct_LenhDongGoi_Dot WHERE LOC_ID=@loc AND OrderId=@id AND Direction=1 AND VoucherId=@voucherId;
                    UPDATE dbo.ct_LenhDongGoi SET InputId=NULL,InputCode=NULL,ClosedWithoutReceipt=0,ReceivedAt=NULL,
                        MaterialCost=NULL,LossKg=NULL,LossCost=NULL,ReceiptJson=NULL
                    WHERE LOC_ID=@loc AND ID=@id AND (InputId=@voucherId OR ClosedWithoutReceipt=1);",
                    new { loc, id, single.VoucherId }, Transaction);
            db.ct_PhieuXuat_ChiTiet.RemoveRange(issued);
            db.ct_PhieuXuat.RemoveRange(outputs);
            db.ct_PhieuNhap_ChiTiet.RemoveRange(received);
            db.ct_PhieuNhap.RemoveRange(inputs);
            new AuditLogController(db, configuration).InserAuditLog();
            await StockCostPosting.SaveMovements(db);
            await tx.CommitAsync();
            return Ok(new { deleted = true, orderDeleted });
        }
        catch (Exception ex) { return SaveError(ex, loc); }
    }

    private Task<bool> HasDeleteDependencies(string loc, Guid id, PackingOrder order, List<PackingVoucher> links, string[] outputIds, string[] inputIds,
        string[] allOutputIds, string[] allInputIds,
        List<ct_PhieuXuat_ChiTiet> issued, List<ct_PhieuNhap_ChiTiet> received, DateTime issueDay, DateTime receiptDay) =>
        Connection.QuerySingleAsync<bool>(@"
            SELECT CONVERT(bit,CASE WHEN
                EXISTS (SELECT 1 FROM dbo.ct_PhieuThu WHERE LOC_ID=@loc AND CHUNGTUKEMTHEO IN @codes)
                OR EXISTS (SELECT 1 FROM dbo.ct_PhieuChi WHERE LOC_ID=@loc AND CHUNGTUKEMTHEO IN @codes)
                OR EXISTS (SELECT 1 FROM dbo.ct_PhieuDatHang WHERE LOC_ID=@loc AND ID_PHIEUXUAT IN @outputIds)
                OR EXISTS (SELECT 1 FROM dbo.ct_PhieuGiaoHang_ChiTiet WHERE LOC_ID=@loc AND ID_PHIEUXUAT IN @outputIds)
                OR EXISTS (SELECT 1 FROM dbo.ct_HoaDon WHERE LOC_ID=@loc AND (ID_PHIEUXUAT IN @outputIds OR CHUNGTUKEMTHEO IN @codes))
                OR EXISTS (SELECT 1 FROM dbo.ct_HoaDon_ChiTiet WHERE LOC_ID=@loc AND ID_PHIEUXUAT_CHITIET IN @details)
                OR EXISTS (SELECT 1 FROM dbo.ct_PhieuChuyen WHERE LOC_ID=@loc AND (ID_PHIEUXUATKHAC IN @outputIds OR ID_PHIEUNHAPKHAC IN @inputIds))
                OR EXISTS (SELECT 1 FROM dbo.ct_PhieuNhap_ChiTiet WHERE LOC_ID=@loc AND ID_PHIEUXUAT_CHITIET IN @details)
                OR EXISTS (SELECT 1 FROM dbo.ct_PhieuXuat WHERE LOC_ID=@loc AND ID NOT IN @allOutputIds AND CHUNGTUKEMTHEO IN @codes)
                OR EXISTS (SELECT 1 FROM dbo.ct_PhieuNhap WHERE LOC_ID=@loc AND ID NOT IN @allInputIds AND CHUNGTUKEMTHEO IN @codes)
                OR EXISTS (SELECT 1 FROM dbo.ct_PhieuXuat_ChiTiet d
                    JOIN dbo.ct_PhieuXuat h ON h.LOC_ID=d.LOC_ID AND h.ID=d.ID_PHIEUXUAT
                    WHERE d.LOC_ID=@loc AND d.ID_HANGHOAKHO IN @finishedStocks AND h.ID NOT IN @outputIds
                        AND COALESCE(h.THOIGIANTHEM,h.NGAYLAP)>=@receiptDay AND d.SOLUONG<>0)
                OR EXISTS (SELECT 1 FROM dbo.ct_LenhDongGoi_NguyenLieu m
                    JOIN dbo.ct_LenhDongGoi p ON p.LOC_ID=m.LOC_ID AND p.ID=m.OrderId
                    JOIN dbo.ct_PhieuXuat_ChiTiet raw ON raw.LOC_ID=m.LOC_ID AND raw.ID=m.DetailId
                    JOIN dbo.ct_PhieuXuat header ON header.LOC_ID=raw.LOC_ID AND header.ID=raw.ID_PHIEUXUAT
                    WHERE m.LOC_ID=@loc AND m.StockId IN @sourceStocks AND p.ID<>@id
                        AND (p.InputId IS NOT NULL OR p.ClosedWithoutReceipt=1
                            OR EXISTS (SELECT 1 FROM dbo.ct_LenhDongGoi_Dot d WHERE d.LOC_ID=p.LOC_ID AND d.OrderId=p.ID AND d.Direction=1))
                        AND COALESCE(header.THOIGIANTHEM,header.NGAYLAP)>=@issueDay)
                THEN 1 ELSE 0 END)", new
            {
                loc, id, outputIds, inputIds, allOutputIds, allInputIds,
                codes = links.Select(x => x.VoucherCode).Where(x => !string.IsNullOrWhiteSpace(x))
                    .Append(order.Code).Append("DG-" + id.ToString("N")).ToArray(),
                details = issued.Select(x => x.ID).ToArray(),
                finishedStocks = received.Select(x => x.ID_HANGHOAKHO).ToArray(),
                sourceStocks = issued.Select(x => x.ID_HANGHOAKHO).ToArray(),
                issueDay, receiptDay
            }, Transaction);

    [HttpGet("trace/{id:guid}")]
    public async Task<IActionResult> TraceDetail(string loc, Guid id)
    {
        if (await Operator(loc) == null) return Forbid();
        var order = await Load(loc, id);
        if (order == null) return NotFound(new { message = "Không tìm thấy lệnh đóng gói." });
        var source = await Connection.QuerySingleAsync(@"
            SELECT k.NAME AS DepotName,COALESCE(CONVERT(decimal(28,2),n.TONGTHANHTIEN),0)+COALESCE(b.ReceiptValue,0) AS ReceiptValue
            FROM dbo.ct_LenhDongGoi o
            LEFT JOIN dbo.dm_Kho k ON k.LOC_ID=o.LOC_ID AND k.ID=o.Depot
            LEFT JOIN dbo.ct_PhieuNhap n ON n.LOC_ID=o.LOC_ID AND n.ID=o.InputId
            OUTER APPLY (SELECT SUM(ReceiptValue) AS ReceiptValue FROM dbo.ct_LenhDongGoi_Dot
                WHERE LOC_ID=o.LOC_ID AND OrderId=o.ID AND Direction=1) b
            WHERE o.LOC_ID=@loc AND o.ID=@id", new { loc, id });
        var materials = await Connection.QueryAsync(@"
            SELECT Code,Name,Quantity,Unit,Ratio,Kg,Cost FROM dbo.v_LenhDongGoi_NguyenLieu
            WHERE LOC_ID=@loc AND OrderId=@id ORDER BY LineNumber,StockId", new { loc, id });
        return Ok(new { order = await Describe(loc, order), source, materials, order.CreatedAt, order.ReceivedAt,
            currentCost = await ReadMaterialCost(loc, order.ID) });
    }

    [HttpPost("issue")]
    public async Task<IActionResult> Issue(string loc, PackingIssue request)
    {
        var user = await Operator(loc);
        if (user == null) return Forbid();
        var orderId = request.OrderId ?? request.Id;
        var sources = request.Sources ?? new List<PackingMaterial> { new(request.Stock ?? "", request.Quantity, request.KgPerUnit, request.UnitId) };
        if (request.Id == Guid.Empty || orderId == Guid.Empty || !ValidId(request.Depot) || !Money(request.Labor)
            || sources.Count is < 1 or > 30 || sources.Any(x => x == null || !ValidId(x.Stock) || !Positive(x.Quantity) || !Positive(x.KgPerUnit))
            || (request.OrderId == null && (request.Lines is not { Count: > 0 and <= 30 }
                || request.Lines.Any(x => x == null || !ValidId(x.Stock) || !Positive(x.Quantity)
                    || !Positive(x.KgPerUnit) || !Money(x.PackageCost)))))
            return Invalid("Kiểm tra hàng, số lượng, khối lượng và chi phí của lệnh.");
        var planned = request.OrderId == null ? request.Lines : new List<PlannedLine>();
        var ids = planned.Select(x => x.Stock).Concat(sources.Select(x => x.Stock)).ToArray();
        if (ids.Distinct(StringComparer.OrdinalIgnoreCase).Count() != ids.Length)
            return Invalid("Mỗi mã hàng chỉ được chọn một lần trong lệnh.");
        var sourceKg = sources.Sum(x => x.Quantity * x.KgPerUnit);
        if (!Positive(sourceKg) || sources.Any(x => !Positive(x.Quantity * x.KgPerUnit))
            || (planned.Count != 0 && !Positive(planned.Sum(x => x.Quantity * x.KgPerUnit))))
            return Invalid("Tổng khối lượng không hợp lệ hoặc có quá 6 số lẻ.");

        try
        {
            using var tx = await db.Database.BeginTransactionAsync();
            await LockOrder(loc, orderId);
            var existing = await Load(loc, orderId);
            if (request.OrderId != null)
            {
                if (existing == null) return NotFound(new { message = "Không tìm thấy lệnh đóng gói." });
                if (await BatchExists(loc, orderId, request.Id, 0)) return Ok(await Describe(loc, existing));
                if (existing.Closed || !SameId(existing.Depot, request.Depot))
                    return Invalid("Lệnh đã chốt hoặc kho xuất không khớp với lệnh.");
                if (request.BeforeFirstReceipt && (await ReadReceiptBatches(loc, orderId)).Count > 0)
                    return Invalid("Lệnh đã có phiếu nhập. Tải lại chi tiết lệnh.");
                if (!Positive(existing.SourceKg + sourceKg) || sources.Any(x => ReadPlan(existing).Any(y => SameId(x.Stock, y.Stock))))
                    return Invalid("Khối lượng lệnh vượt giới hạn hoặc nguyên liệu trùng mã thành phẩm.");
            }
            else if (existing != null) return Ok(await Describe(loc, existing));
            var products = await LoadStocks(loc, request.Depot, ids);
            if (products.Count != ids.Length) return Invalid("Hàng đã chọn không còn hợp lệ tại kho này.");
            var plan = new List<PackingPlan>();
            foreach (var line in planned)
            {
                var product = products.Single(x => SameId(x.Id, line.Stock));
                var unit = product.ResolveUnit(line.UnitId);
                if (unit == null || !Positive(line.Quantity * unit.Ratio))
                    return Invalid("Đơn vị hoặc số lượng quy đổi không hợp lệ của " + product.Code + ".");
                plan.Add(new PackingPlan(product.Id, product.Code, product.Name, line.Quantity, line.KgPerUnit,
                    line.PackageCost, unit.Id, unit.Name, unit.Ratio));
            }

            var now = DateTime.UtcNow.AddHours(7);
            var voucherId = Guid.NewGuid().ToString();
            var materials = new List<(ct_PhieuXuat_ChiTiet Detail, decimal Kg)>();
            foreach (var line in sources.OrderBy(x => x.Stock, StringComparer.OrdinalIgnoreCase))
            {
                var product = products.Single(x => SameId(x.Id, line.Stock));
                var unit = product.ResolveUnit(line.UnitId);
                if (unit == null || !Positive(line.Quantity * unit.Ratio))
                    return Invalid("Đơn vị hoặc số lượng quy đổi không hợp lệ của " + product.Code + ".");
                var stockQuantity = line.Quantity * unit.Ratio;
                await StockCostPosting.LockStock(db, loc, product.Id);
                var stock = await db.dm_HangHoa_Kho.FirstAsync(x => x.LOC_ID == loc && x.ID == product.Id);
                if (stock.QTY < (double)stockQuantity) return Invalid(product.Code + " không đủ tồn kho sau quy đổi.");
                if (!await db.Set<OpeningStockCost>().AnyAsync(x => x.LOC_ID == loc && x.ID_HANGHOAKHO == stock.ID && x.TUNGAY <= now.Date))
                    return Invalid(product.Code + " chưa có giá vốn đầu kỳ tại ngày xuất.");
                stock.QTY -= (double)stockQuantity;
                materials.Add((new ct_PhieuXuat_ChiTiet
                {
                    LOC_ID = loc, ID = Guid.NewGuid().ToString(), ID_PHIEUXUAT = voucherId, ID_HANGHOAKHO = stock.ID,
                    ID_DVT = unit.Id, TYLE_QD = (double)unit.Ratio, SOLUONG = (double)line.Quantity,
                    TONGSOLUONG = (double)stockQuantity, STT = materials.Count + 1
                }, line.Quantity * line.KgPerUnit));
            }

            await EnsureTypes(loc, user, now);
            if (!await ReadTypeFlag(loc, "output") || !await ReadTypeFlag(loc, "input"))
                return Invalid("Cần bật Hoạt động cho hai loại phiếu đóng gói và đánh dấu Xuất nguyên liệu, Nhập thành phẩm trước khi lập lệnh mới.");
            var (sequence, voucherCode) = await NextVoucher(loc, now.Date, isInput: false);
            if (voucherCode == null) return Invalid("Số thứ tự phiếu xuất trong ngày vượt giới hạn.");
            db.ct_PhieuXuat.Add(new ct_PhieuXuat
            {
                LOC_ID = loc, ID = voucherId, MAPHIEU = voucherCode, ID_LOAIPHIEUXUAT = TypeId,
                ID_KHO = request.Depot, NGAYLAP = now.Date, THOIGIANTHEM = now, ID_NGUOITAO = user,
                SOPHIEU = sequence, CHUNGTUKEMTHEO = existing?.Code ?? ("DG" + voucherCode[2..]), GHICHU = "Xuất nguyên liệu đóng gói", ISHOANTAT = true
            });
            db.ct_PhieuXuat_ChiTiet.AddRange(materials.Select(x => x.Detail));
            new AuditLogController(db, configuration).InserAuditLog();
            await db.SaveChangesAsync();
            foreach (var material in materials) await StockCostPosting.Recalculate(db, loc, material.Detail.ID_HANGHOAKHO);
            var first = materials[0].Detail;
            if (existing == null)
                await Connection.ExecuteAsync(@"
                INSERT dbo.ct_LenhDongGoi(LOC_ID,ID,Depot,SourceStock,SourceDetail,SourceKg,Labor,PlanJson,OutputId,OutputCode,CreatedAt)
                VALUES(@loc,@id,@depot,@stock,@detailId,@sourceKg,@labor,@plan,@voucherId,@voucherCode,@now)
                ", new { loc, id = orderId, request.Depot, stock = first.ID_HANGHOAKHO, detailId = first.ID, sourceKg, request.Labor,
                    plan = JsonSerializer.Serialize(plan), voucherId, voucherCode, now }, Transaction);
            else
            {
                await Connection.ExecuteAsync(@"
                    UPDATE dbo.ct_LenhDongGoi SET SourceKg=SourceKg+@sourceKg WHERE LOC_ID=@loc AND ID=@orderId;
                    INSERT dbo.ct_LenhDongGoi_Dot(LOC_ID,OrderId,RequestId,Direction,VoucherId,VoucherCode,PostedAt,Kg)
                    VALUES(@loc,@orderId,@requestId,0,@voucherId,@voucherCode,@now,@sourceKg);",
                    new { loc, orderId, requestId = request.Id, voucherId, voucherCode, now, sourceKg }, Transaction);
            }
            await Connection.ExecuteAsync(@"
                INSERT dbo.ct_LenhDongGoi_NguyenLieu(LOC_ID,OrderId,DetailId,StockId,Kg)
                VALUES(@loc,@id,@detailId,@stock,@kg)",
                materials.Select(x => new { loc, id = orderId, detailId = x.Detail.ID, stock = x.Detail.ID_HANGHOAKHO, x.Kg }), Transaction);
            var cost = await ReadMaterialCost(loc, orderId);
            if (cost == null || !Money(cost.Value) || cost.Value + (existing?.Labor ?? request.Labor) >= 1000000000000m)
                return Invalid("Chưa tính được giá vốn nguyên liệu xuất hoặc chi phí lệnh vượt giới hạn.");
            var order = (await Load(loc, orderId))!;
            var result = await Describe(loc, order);
            await tx.CommitAsync();
            return Ok(result);
        }
        catch (Exception ex) { return SaveError(ex, loc); }
    }

    [HttpPost("orders/{id:guid}/receive")]
    public async Task<IActionResult> Receive(string loc, Guid id, PackingReceipt request)
    {
        var user = await Operator(loc);
        if (user == null) return Forbid();
        if (request.Lines is not { Count: > 0 and <= 30 } || !Money(request.MaterialCost) || !Money(request.LossCost)
            || (!request.FinalizeOrder && (request.RequestId is null || request.RequestId == Guid.Empty || request.LossKg != 0 || request.LossCost != 0))
            || !NonnegativeQuantity(request.LossKg) || (request.LossKg == 0 && request.LossCost != 0)
            || request.Lines.Any(x => x == null || !ValidId(x.Stock) || !NonnegativeQuantity(x.Quantity)
                || x.UnitCost < 0 || x.UnitCost >= 1000000000000m || decimal.Round(x.UnitCost, 6) != x.UnitCost)
            || request.Lines.Select(x => x.Stock).Distinct(StringComparer.OrdinalIgnoreCase).Count() != request.Lines.Count)
            return Invalid("Kiểm tra số lượng, đơn giá và hao hụt thực tế.");
        try
        {
            using var tx = await db.Database.BeginTransactionAsync();
            await LockOrder(loc, id);
            var order = await Load(loc, id);
            if (order == null) return NotFound(new { message = "Không tìm thấy lệnh đóng gói." });
            if (request.RequestId is Guid requestId && await BatchExists(loc, id, requestId, 1)) return Ok(await Describe(loc, order));
            if (order.Closed)
                return request.FinalizeOrder ? Ok(await Describe(loc, order)) : Invalid("Lệnh đã chốt.");
            var plan = ReadPlan(order);
            if (request.Lines.Count != plan.Count || request.Lines.Any(x => !plan.Any(y => SameId(x.Stock, y.Stock))))
                return Invalid("Danh sách mã thành phẩm phải khớp với lệnh đóng gói.");
            var actual = plan.Select(line =>
            {
                var entered = request.Lines.Single(x => SameId(x.Stock, line.Stock));
                return new PackingActual(line.Stock, line.Code, line.Name, entered.Quantity, line.KgPerUnit, line.PackageCost,
                    entered.UnitCost, line.UnitId, line.UnitName, line.Ratio);
            }).Where(x => x.Quantity > 0).ToList();
            var previous = await ReadReceiptBatches(loc, id);
            var receivedKg = previous.Sum(x => x.Kg);
            if (request.ReceivedKg != receivedKg || (request.SourceKg != null && request.SourceKg != order.SourceKg))
                return Conflict(new { message = "Lệnh đã phát sinh phiếu khác. Mở lại lệnh trước khi lưu." });
            var remainingKg = order.SourceKg - receivedKg;
            var actualKg = actual.Sum(x => x.Quantity * x.KgPerUnit);
            var closeWithLossOnly = request.FinalizeOrder && receivedKg > 0 && actual.Count == 0;
            if ((!closeWithLossOnly && !Positive(actualKg)) ||
                (request.FinalizeOrder ? actualKg + request.LossKg != remainingKg : actualKg >= remainingKg))
                return Invalid(request.FinalizeOrder ? "Kg thành phẩm và hao hụt phải bằng kg còn lại của lệnh."
                    : "Nhập từng đợt phải để lại khối lượng cho đợt chốt lệnh.");
            var stockIds = actual.Select(x => x.Stock).ToArray();
            var sourceIds = await Connection.QueryAsync<string>(
                "SELECT StockId FROM dbo.ct_LenhDongGoi_NguyenLieu WHERE LOC_ID=@loc AND OrderId=@id", new { loc, id }, Transaction);
            foreach (var stock in stockIds.Concat(sourceIds).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                await StockCostPosting.LockStock(db, loc, stock);
            var material = await ReadMaterialCost(loc, order.ID);
            if (material == null || material.Value != request.MaterialCost)
                return Conflict(new { message = "Giá vốn nguyên liệu đã thay đổi hoặc chưa đủ dữ liệu. Mở lại form nhập để lấy giá mới." });
            var remainingCommon = material.Value + order.Labor - previous.Sum(x => x.CommonCost);
            var common = request.FinalizeOrder ? remainingCommon : RoundMoney(remainingCommon * actualKg / remainingKg);
            var package = actual.Sum(x => RoundMoney(x.Quantity * x.PackageCost));
            var expected = common + package;
            if (common < 0 || expected >= 1000000000000m || request.LossCost > common
                || actual.Sum(x => RoundMoney(x.Quantity * x.UnitCost)) + request.LossCost != expected)
                return Invalid("Trị giá nhập cộng chi phí hao hụt phải bằng tổng giá vốn của lệnh.");
            if (closeWithLossOnly)
            {
                await Connection.ExecuteAsync(@"
                    UPDATE dbo.ct_LenhDongGoi SET ClosedWithoutReceipt=1,ReceivedAt=@now,MaterialCost=@material,LossKg=@lossKg,LossCost=@lossCost
                    WHERE LOC_ID=@loc AND ID=@id;",
                    new { loc, id, now = DateTime.UtcNow.AddHours(7), material, request.LossKg, request.LossCost }, Transaction);
                var closed = await Describe(loc, (await Load(loc, id))!);
                await tx.CommitAsync();
                return Ok(closed);
            }

            var products = await LoadStocks(loc, order.Depot, stockIds);
            if (products.Count != stockIds.Length) return Invalid("Có thành phẩm không còn hợp lệ tại kho.");
            var units = products.ToDictionary(x => x.Id, x =>
                x.ResolveUnit(actual.Single(line => SameId(line.Stock, x.Id)).UnitId), StringComparer.OrdinalIgnoreCase);
            if (actual.Any(x => units[x.Stock] is not { } unit || unit.Ratio != (x.Ratio ?? 1)
                || !Positive(x.Quantity * unit.Ratio)))
                return Invalid("Đơn vị hoặc tỷ lệ quy đổi thành phẩm đã thay đổi hoặc số lượng quy đổi không hợp lệ. Kiểm tra lại danh mục.");
            var now = DateTime.UtcNow.AddHours(7);
            foreach (var stock in stockIds)
                if (!await EnsureOpening(loc, stock, now.Date))
                    return Invalid("Thành phẩm đã có lịch sử nhưng chưa có đầu kỳ hợp lệ. Hãy khai báo giá vốn đầu kỳ trước.");

            var voucherId = Guid.NewGuid().ToString();
            await LockTypes(loc);
            var (sequence, voucherCode) = await NextVoucher(loc, now.Date, isInput: true);
            if (voucherCode == null) return Invalid("Số thứ tự phiếu nhập trong ngày vượt giới hạn.");
            var total = actual.Sum(x => RoundMoney(x.Quantity * x.UnitCost));
            db.ct_PhieuNhap.Add(new ct_PhieuNhap
            {
                LOC_ID = loc, ID = voucherId, MAPHIEU = voucherCode, ID_LOAIPHIEUNHAP = TypeId, ID_KHO = order.Depot,
                NGAYLAP = now.Date, THOIGIANTHEM = now, ID_NGUOITAO = user, SOPHIEU = sequence,
                CHUNGTUKEMTHEO = order.Code, GHICHU = "Nhập thành phẩm đóng gói", TONGTHANHTIEN = (double)total, TONGTIEN = (double)total
            });
            foreach (var line in actual)
            {
                var unit = units[line.Stock]!;
                var stock = await db.dm_HangHoa_Kho.FirstAsync(x => x.LOC_ID == loc && x.ID == line.Stock);
                var stockQuantity = line.Quantity * unit.Ratio;
                stock.QTY += (double)stockQuantity;
                var value = (double)RoundMoney(line.Quantity * line.UnitCost);
                db.ct_PhieuNhap_ChiTiet.Add(new ct_PhieuNhap_ChiTiet
                {
                    LOC_ID = loc, ID = Guid.NewGuid().ToString(), ID_PHIEUNHAP = voucherId, ID_HANGHOAKHO = stock.ID,
                    ID_DVT = unit.Id, TYLE_QD = (double)unit.Ratio, SOLUONG = (double)line.Quantity, TONGSOLUONG = (double)stockQuantity,
                    DONGIA = (double)line.UnitCost, GIAVON = (double)(line.UnitCost / unit.Ratio), THANHTIEN = value, TONGCONG = value
                });
            }
            new AuditLogController(db, configuration).InserAuditLog();
            await db.SaveChangesAsync();
            foreach (var stock in stockIds) await StockCostPosting.Recalculate(db, loc, stock);
            if (request.FinalizeOrder)
                await Connection.ExecuteAsync(@"
                UPDATE dbo.ct_LenhDongGoi SET InputId=@voucherId,InputCode=@voucherCode,ReceivedAt=@now,
                    MaterialCost=@material,LossKg=@lossKg,LossCost=@lossCost,ReceiptJson=@receipt
                WHERE LOC_ID=@loc AND ID=@id AND InputId IS NULL
                ", new { loc, id, voucherId, voucherCode, now, material, request.LossKg, request.LossCost,
                    receipt = JsonSerializer.Serialize(actual) }, Transaction);
            else
                await Connection.ExecuteAsync(@"
                    INSERT dbo.ct_LenhDongGoi_Dot(LOC_ID,OrderId,RequestId,Direction,VoucherId,VoucherCode,PostedAt,Kg,CommonCost,PackageCost,ReceiptValue,ReceiptJson)
                    VALUES(@loc,@id,@requestId,1,@voucherId,@voucherCode,@now,@actualKg,@common,@package,@total,@receipt);",
                    new { loc, id, request.RequestId, voucherId, voucherCode, now, actualKg, common, package, total,
                        receipt = JsonSerializer.Serialize(actual) }, Transaction);
            var result = await Describe(loc, (await Load(loc, id))!);
            await tx.CommitAsync();
            return Ok(result);
        }
        catch (Exception ex) { return SaveError(ex, loc); }
    }

    private async Task<bool> EnsureOpening(string loc, string stock, DateTime day)
    {
        var opening = await db.Set<OpeningStockCost>().FindAsync(loc, stock);
        if (opening != null) return opening.TUNGAY <= day;
        var hasHistory = await db.ct_PhieuNhap_ChiTiet.AnyAsync(x => x.LOC_ID == loc && x.ID_HANGHOAKHO == stock)
            || await db.ct_PhieuXuat_ChiTiet.AnyAsync(x => x.LOC_ID == loc && x.ID_HANGHOAKHO == stock);
        if (hasHistory || await db.dm_HangHoa_Kho.AnyAsync(x => x.LOC_ID == loc && x.ID == stock && x.QTY != 0)) return false;
        db.Add(new OpeningStockCost { LOC_ID = loc, ID_HANGHOAKHO = stock, TUNGAY = day });
        return true;
    }

    private async Task EnsureTypes(string loc, string user, DateTime now)
    {
        // Serialize first-use setup across orders in the same branch.
        await LockTypes(loc);
        await Connection.ExecuteAsync(@"
            IF NOT EXISTS (SELECT 1 FROM dbo.dm_LoaiPhieuXuat WHERE LOC_ID=@loc AND ID=@id)
                INSERT dbo.dm_LoaiPhieuXuat(LOC_ID,ID,MA,NAME,ISACTIVE,ISDEFAULT,ISSYSTEM,TYPE,ISTRAHANG,ISXUATNGUYENLIEU,THOIGIANTHEM,ID_NGUOITAO)
                VALUES(@loc,@id,N'DGX',N'Xuất sản xuất',1,0,1,0,0,1,@now,@user);
            IF NOT EXISTS (SELECT 1 FROM dbo.dm_LoaiPhieuNhap WHERE LOC_ID=@loc AND ID=@id)
                INSERT dbo.dm_LoaiPhieuNhap(LOC_ID,ID,MA,NAME,ISACTIVE,ISDEFAULT,ISSYSTEM,TYPE,ISNHAPTRAHANG,ISNHAPTHANHPHAM,THOIGIANTHEM,ID_NGUOITAO)
                VALUES(@loc,@id,N'DGN',N'Nhập thành phẩm',1,0,1,0,0,1,@now,@user);
            ", new { loc, id = TypeId, user, now }, Transaction);
    }

    private Task LockTypes(string loc) => Lock("THP-Packing-Types-" + loc.TrimEnd().ToUpperInvariant());

    private async Task<(int Sequence, string? Code)> NextVoucher(string loc, DateTime day, bool isInput)
    {
        // Hold the voucher date range until the packing transaction commits.
        var table = isInput ? "dbo.ct_PhieuNhap" : "dbo.ct_PhieuXuat";
        var type = isInput ? "Input" : "Output";
        var last = await Connection.QuerySingleAsync<int>($@"
            SELECT ISNULL(MAX(SOPHIEU),0) FROM {table} WITH (UPDLOCK,HOLDLOCK)
            WHERE LOC_ID=@loc AND NGAYLAP>=@day AND NGAYLAP<@until",
            new { loc, day, until = day.AddDays(1) }, Transaction);
        for (var sequence = (long)Math.Max(last, 0) + 1; sequence <= 999999999; sequence++)
        {
            var code = API.GetMaPhieu(type, day, (int)sequence);
            var exists = await Connection.QuerySingleAsync<bool>($@"
                SELECT CONVERT(bit,CASE WHEN EXISTS(SELECT 1 FROM {table} WHERE LOC_ID=@loc AND MAPHIEU=@code)
                    THEN 1 ELSE 0 END)", new { loc, code }, Transaction);
            if (!exists) return ((int)sequence, code);
        }
        return (0, null);
    }

    private Task<bool> ReadTypeFlag(string loc, string direction) => Connection.QuerySingleOrDefaultAsync<bool>(
        direction == "output"
            ? "SELECT ISXUATNGUYENLIEU FROM dbo.dm_LoaiPhieuXuat WHERE LOC_ID=@loc AND ID=@id AND ISACTIVE=1 AND TYPE=0 AND ISTRAHANG=0"
            : "SELECT ISNHAPTHANHPHAM FROM dbo.dm_LoaiPhieuNhap WHERE LOC_ID=@loc AND ID=@id AND ISACTIVE=1 AND TYPE=0 AND ISNHAPTRAHANG=0",
        new { loc, id = TypeId }, Transaction);

    private static string? Catalog(string direction) => direction switch
    {
        "input" => "TypeInput",
        "output" => "TypeOutput",
        _ => null
    };

    private async Task<List<PackingStock>> LoadStocks(string loc, string depot, string[] ids) =>
        await (from stock in db.dm_HangHoa_Kho
               join p in db.dm_HangHoa on new { stock.LOC_ID, ID = stock.ID_HANGHOA } equals new { p.LOC_ID, p.ID }
               where stock.LOC_ID == loc && stock.ID_KHO == depot && ids.Contains(stock.ID)
                   && p.ISACTIVE && p.LOAIHANGHOA != "2" && !p.ISCOMBO
               let unit = p.TYLE_QD > 1 && p.ID_DVT_QD != null && p.ID_DVT_QD != "" ? p.ID_DVT_QD : p.ID_DVT
               where unit != null && unit != ""
               select new PackingStock(stock.ID, p.MA, p.NAME, unit,
                   db.dm_DonViTinh.Where(x => x.LOC_ID == loc && x.ID == unit).Select(x => x.NAME).FirstOrDefault(),
                   p.ID_DVT,
                   db.dm_DonViTinh.Where(x => x.LOC_ID == loc && x.ID == p.ID_DVT).Select(x => x.NAME).FirstOrDefault(),
                   p.TYLE_QD)).ToListAsync();

    private Task LockOrder(string loc, Guid id) => Lock($"THP-Packing-{loc.TrimEnd().ToUpperInvariant()}-{id:N}");

    private Task Lock(string key) => Connection.ExecuteAsync(@"
        DECLARE @result int;
        EXEC @result=sys.sp_getapplock @Resource=@key,@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=10000;
        IF @result<0 THROW 51221,N'Lệnh đóng gói đang được xử lý. Vui lòng thử lại.',1;
        ", new { key }, Transaction);

    private Task<PackingOrder?> Load(string loc, Guid id) => Connection.QuerySingleOrDefaultAsync<PackingOrder>(
        $@"SELECT p.*, orderCode.Code FROM dbo.ct_LenhDongGoi p
           {OrderCodesSql}
           WHERE p.LOC_ID=@loc AND p.ID=@id", new { loc, id }, Transaction);

    private Task<decimal?> ReadMaterialCost(string loc, Guid id) => Connection.QuerySingleOrDefaultAsync<decimal?>(@"
        SELECT CASE WHEN COUNT(Cost)=COUNT(*) THEN SUM(Cost) END FROM dbo.v_LenhDongGoi_NguyenLieu
        WHERE LOC_ID=@loc AND OrderId=@id", new { loc, id }, Transaction);

    private Task<bool> BatchExists(string loc, Guid id, Guid requestId, int direction) => Connection.QuerySingleAsync<bool>(@"
        SELECT CONVERT(bit,CASE WHEN EXISTS(SELECT 1 FROM dbo.ct_LenhDongGoi_Dot
            WHERE LOC_ID=@loc AND OrderId=@id AND RequestId=@requestId AND Direction=@direction) THEN 1 ELSE 0 END)",
        new { loc, id, requestId, direction }, Transaction);

    private async Task<List<PackingBatchReceipt>> ReadReceiptBatches(string loc, Guid id) =>
        (await Connection.QueryAsync<PackingBatchReceipt>(@"
            SELECT VoucherCode,PostedAt,Kg,CommonCost,ReceiptValue,ReceiptJson FROM dbo.ct_LenhDongGoi_Dot
            WHERE LOC_ID=@loc AND OrderId=@id AND Direction=1 ORDER BY PostedAt,VoucherCode", new { loc, id }, Transaction)).AsList();

    private async Task<object> Describe(string loc, PackingOrder order)
    {
        var partial = await ReadReceiptBatches(loc, order.ID);
        var vouchers = (await Connection.QueryAsync<PackingVoucher>(@"
            SELECT Direction,VoucherId,VoucherCode,PostedAt FROM dbo.v_LenhDongGoi_Phieu
            WHERE LOC_ID=@loc AND OrderId=@id ORDER BY PostedAt,LEN(VoucherCode),VoucherCode", new { loc, id = order.ID }, Transaction)).AsList();
        var closed = order.Closed;
        var posted = partial.Select(x => new { x.VoucherCode, x.PostedAt, x.Kg, x.ReceiptValue,
            lines = JsonSerializer.Deserialize<List<PackingActual>>(x.ReceiptJson) }).ToList();
        var received = partial.Sum(x => x.Kg);
        var materialCost = closed ? order.MaterialCost : await ReadMaterialCost(loc, order.ID);
        return new
        {
            order.ID, code = order.Code, order.Depot, order.OutputCode, order.InputCode, order.SourceKg, order.Labor, closed,
            materialCost, order.LossKg, order.LossCost, lines = ReadPlan(order),
            receivedKg = closed ? order.SourceKg - order.LossKg : received,
            remainingKg = closed ? 0 : order.SourceKg - received,
            remainingCommon = materialCost + order.Labor - partial.Sum(x => x.CommonCost),
            vouchers, partialReceipts = posted,
            receipt = order.ReceiptJson == null ? null : JsonSerializer.Deserialize<List<PackingActual>>(order.ReceiptJson)
        };
    }

    private IActionResult SaveError(Exception ex, string loc)
    {
        var sql = ex.GetBaseException() as SqlException;
        if (sql?.Number is 51101 or 51102 or 51103 or 51220 or 51221) return Invalid(sql.Message);
        logger.LogError(ex, "Packing posting failed for branch {Branch}", loc);
        return StatusCode(500, new { message = "Không lưu được lệnh đóng gói. Kiểm tra nhật ký API và cấu hình cơ sở dữ liệu." });
    }

    private BadRequestObjectResult Invalid(string message) => BadRequest(new { message });
    private static List<PackingPlan> ReadPlan(PackingOrder order) => JsonSerializer.Deserialize<List<PackingPlan>>(order.PlanJson)!;
    private static bool ValidId(string? id) => !string.IsNullOrWhiteSpace(id) && id.Length <= 50;
    private static bool SameId(string a, string b) => StringComparer.OrdinalIgnoreCase.Equals(a, b);
    private static bool Positive(decimal n) => n > 0 && NonnegativeQuantity(n);
    private static bool NonnegativeQuantity(decimal n) => n >= 0 && n < 1000000m && decimal.Round(n, 6) == n;
    private static bool Money(decimal n) => n >= 0 && n < 1000000000000m && RoundMoney(n) == n;
    private static decimal RoundMoney(decimal n) => decimal.Round(n, 2, MidpointRounding.AwayFromZero);

    public sealed record PackingIssue(Guid Id, string Depot, string? Stock, decimal Quantity, decimal KgPerUnit, decimal Labor,
        List<PlannedLine> Lines, string? UnitId = null, List<PackingMaterial>? Sources = null, Guid? OrderId = null,
        bool BeforeFirstReceipt = false);
    public sealed record PackingMaterial(string Stock, decimal Quantity, decimal KgPerUnit, string? UnitId = null);
    public sealed record PlannedLine(string Stock, decimal Quantity, decimal KgPerUnit, decimal PackageCost, string? UnitId = null);
    public sealed record PackingReceipt(decimal MaterialCost, decimal LossKg, decimal LossCost, List<ReceivedLine> Lines,
        bool FinalizeOrder = true, Guid? RequestId = null, decimal ReceivedKg = 0, decimal? SourceKg = null);
    public sealed record ReceivedLine(string Stock, decimal Quantity, decimal UnitCost);
    public sealed record PackingFlag(bool Enabled);
    public sealed record PackingDelete(bool Received, int? VoucherCount = null, string? VoucherId = null);
    private sealed record PackingUnit(string Id, string? Name, decimal Ratio);
    private sealed record PackingStock(string Id, string Code, string Name, string UnitId, string? UnitName,
        string? LargeUnitId, string? LargeUnitName, double Ratio)
    {
        public PackingUnit? ResolveUnit(string? id)
        {
            if (id == null || SameId(id, UnitId)) return new PackingUnit(UnitId, UnitName, 1);
            if (LargeUnitId == null || !SameId(id, LargeUnitId) || !(Ratio > 1 && Ratio < 1000000)) return null;
            var ratio = (decimal)Ratio;
            return Positive(ratio) ? new PackingUnit(LargeUnitId, LargeUnitName, ratio) : null;
        }
    }
    private sealed record PackingPlan(string Stock, string Code, string Name, decimal Quantity, decimal KgPerUnit, decimal PackageCost,
        string? UnitId = null, string? UnitName = null, decimal? Ratio = null);
    private sealed record PackingActual(string Stock, string Code, string Name, decimal Quantity, decimal KgPerUnit, decimal PackageCost, decimal UnitCost,
        string? UnitId = null, string? UnitName = null, decimal? Ratio = null);

    private sealed class PackingVoucher
    {
        public int Direction { get; set; }
        public string VoucherId { get; set; } = "";
        public string VoucherCode { get; set; } = "";
        public DateTime? PostedAt { get; set; }
    }

    private sealed class PackingBatchReceipt
    {
        public string VoucherCode { get; set; } = "";
        public DateTime PostedAt { get; set; }
        public decimal Kg { get; set; }
        public decimal CommonCost { get; set; }
        public decimal ReceiptValue { get; set; }
        public string ReceiptJson { get; set; } = "";
    }

    private sealed class PackingOrder
    {
        public Guid ID { get; set; }
        public string Code { get; set; } = "";
        public string Depot { get; set; } = "";
        public decimal SourceKg { get; set; }
        public decimal Labor { get; set; }
        public string PlanJson { get; set; } = "";
        public string OutputCode { get; set; } = "";
        public string OutputId { get; set; } = "";
        public string? InputId { get; set; }
        public string? InputCode { get; set; }
        public bool ClosedWithoutReceipt { get; set; }
        public bool Closed => InputId != null || ClosedWithoutReceipt;
        public DateTime CreatedAt { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public decimal? MaterialCost { get; set; }
        public decimal? LossKg { get; set; }
        public decimal? LossCost { get; set; }
        public string? ReceiptJson { get; set; }
    }
}
