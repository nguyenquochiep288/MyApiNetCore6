using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApiNetCore6.Data;
using System.Data;
using System.Text.Json;

namespace MyApiNetCore6.Controllers;

[ApiController, Route("api/[controller]"), Authorize(Roles = "User")]
public class GrossProfitController : ControllerBase
{
    private readonly dbTrangHiepPhatContext db;
    private readonly ILogger<GrossProfitController> logger;
    private static readonly Lazy<string> ReturnEstimatesSql = new(() => ReadSql("ReturnCostEstimates"));
    private static readonly Lazy<string> Sql = new(() => LoadReportSql("GrossProfit"));
    private static readonly Lazy<string> AuditSql = new(() => LoadReportSql("CostAudit"));

    private static string LoadReportSql(string name) =>
        ReadSql(name).Replace("-- INCLUDE ReturnCostEstimates.sql", ReturnEstimatesSql.Value, StringComparison.Ordinal);

    private static string ReadSql(string name)
    {
        using var stream = typeof(GrossProfitController).Assembly.GetManifestResourceStream($"Reports.{name}.sql")!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
    public GrossProfitController(dbTrangHiepPhatContext context, ILogger<GrossProfitController> log)
    { db = context; logger = log; }

    private async Task<bool> CanView(string loc, CancellationToken ct)
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(username)) return false;
        var groupId = await db.view_AspNetUsers.Where(x => x.UserName == username)
            .Select(x => x.ID_NHOMQUYEN).FirstOrDefaultAsync(ct);
        if (groupId == "-1") return true;
        var group = await db.web_NhomQuyen.AsNoTracking().FirstOrDefaultAsync(x => x.ID == groupId && x.LOC_ID == loc && x.ISACTIVE, ct);
        if (group == null) return false;
        if (!group.ISPHANQUYEN) return true;
        var rights = db.view_web_PhanQuyen.Where(x => x.LOC_ID == loc && x.ID_NHOMQUYEN == groupId && x.TRANGTHAI);
        return await rights.AnyAsync(x => x.CONTROLLERNAME == "Inventory" && x.MAQUYEN == "View", ct)
            && await rights.AnyAsync(x => x.CONTROLLERNAME == "Output" && x.MAQUYEN == "View", ct);
    }

    [HttpPost("Report")]
    public Task<IActionResult> Report(GrossProfitRequest request, CancellationToken ct) => Load(request, false, ct);

    [HttpPost("Audit")]
    public Task<IActionResult> Audit(GrossProfitRequest request, CancellationToken ct) => Load(request, true, ct);

    private async Task<IActionResult> Load(GrossProfitRequest request, bool audit, CancellationToken ct)
    {
        var today = DateTime.UtcNow.AddHours(7).Date;
        if (string.IsNullOrWhiteSpace(request.LOC_ID) || request.LOC_ID.Length > 50
            || request.FROM.Date < new DateTime(2000, 1, 1) || request.TO.Date > today
            || request.FROM.Date > request.TO.Date || (request.TO.Date-request.FROM.Date).Days > 730
            || request.PAGE < 1 || request.SEARCH?.Length > 150 || request.DEPOT?.Length > 50
            || (request.GROUPS?.Count ?? 0) > 1000 || (request.GROUPS?.Any(x => x == null || x.Length > 50) ?? false)
            || request.DOCUMENT?.Length > 52
            || !(audit ? new[] { "all", "missing", "invalid", "negative", "cost", "blocked", "opening" }
                : new[] { "product", "group", "customer", "employee", "depot", "voucher", "detail" }).Contains(request.GROUPBY))
            return Ok(new { Success = false, Message = "Điều kiện không hợp lệ. Chọn kỳ tối đa 731 ngày, không vượt hôm nay." });
        if (!await CanView(request.LOC_ID, ct)) return Forbid();
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(ct);
        deadline.CancelAfter(TimeSpan.FromSeconds(90));
        var connection = db.Database.GetDbConnection();
        var openedHere = connection.State != ConnectionState.Open;
        try
        {
            if (openedHere) await connection.OpenAsync(deadline.Token);
            using var data = await connection.QueryMultipleAsync(new CommandDefinition(audit ? AuditSql.Value : Sql.Value, new {
                Branch = request.LOC_ID, Depot = request.DEPOT ?? "", Search = (request.SEARCH ?? "").Trim(),
                Groups = JsonSerializer.Serialize((request.GROUPS ?? new()).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct()),
                Start = request.FROM.Date, Until = request.TO.Date.AddDays(1), GroupBy = request.GROUPBY,
                Page = request.PAGE, Export = request.EXPORT, Document = request.DOCUMENT ?? ""
            }, commandTimeout: 80, cancellationToken: deadline.Token));
            var totals = await data.ReadSingleAsync();
            var meta = await data.ReadSingleAsync();
            var rows = (await data.ReadAsync()).ToList();
            if ((bool)meta.EXPORTTOOLARGE)
                return Ok(new { Success = false, Message = "Xuất tối đa 20.000 dòng. Hãy thu hẹp kỳ/kho/nhóm hàng." });
            return Ok(new { Success = true, Data = new { TOTALS = totals, META = meta, ROWS = rows } });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Gross profit report failed for branch {Branch}", request.LOC_ID);
            return Ok(new { Success = false, Message = "Không tải được báo cáo. Kiểm tra kết nối, cột ISGIAVONDATINH và dữ liệu giá vốn; hoặc thu hẹp kỳ báo cáo." });
        }
        finally { if (openedHere) await connection.CloseAsync(); }
    }
}

public sealed class GrossProfitRequest
{
    public string LOC_ID { get; set; } = "";
    public string? DEPOT { get; set; }
    public string? SEARCH { get; set; }
    public List<string>? GROUPS { get; set; }
    public DateTime FROM { get; set; }
    public DateTime TO { get; set; }
    public string GROUPBY { get; set; } = "product";
    public string? DOCUMENT { get; set; }
    public int PAGE { get; set; } = 1;
    public bool EXPORT { get; set; }
}
