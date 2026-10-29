using Dapper;
using Microsoft.EntityFrameworkCore;
using MyApiNetCore6.Controllers;
using MyApiNetCore6.Data;
using System.Data;

namespace MyApiNetCore6.Services;

internal sealed class InventoryInsightTotals
{
    public Dictionary<string, SaleTotal> Sales { get; private set; } = new();
    public Dictionary<string, ReceiptTotal> Receipts { get; private set; } = new();
    public Dictionary<string, SupplierReturnTotal> SupplierReturns { get; private set; } = new();

    public static async Task<InventoryInsightTotals> LoadAsync(dbTrangHiepPhatContext db,
        InventoryInsightRequest request, List<string> groups, string search,
        DateTime start, DateTime until, DateTime tomorrow, CancellationToken cancellationToken)
    {
        // Group directly by the detail's stock key. EF's grouping over the joined scope
        // generated separate correlated MAX/SUM subqueries for every stock row.
        // Only fixed SQL fragments are concatenated; all filter values are parameters.
        var scope = @";WITH ScopedStock AS (
SELECT s.ID
FROM dbo.dm_HangHoa_Kho s
JOIN dbo.dm_HangHoa p ON p.LOC_ID = s.LOC_ID AND p.ID = s.ID_HANGHOA
WHERE s.LOC_ID = @Branch AND p.ISCOMBO = 0 AND p.ISKHONGHIENTHITONKHO = 0
  AND (@IncludeInactive = 1 OR p.ISACTIVE = 1)
  AND (@Depot = '' OR s.ID_KHO = @Depot)
  AND (@Search = '' OR CHARINDEX(@Search, p.MA) > 0 OR CHARINDEX(@Search, p.NAME) > 0)
" + (groups.Count > 0 ? " AND p.ID_NHOMHANGHOA IN @Groups\n" : "") + ")\n";

        var sql = scope + @"
SELECT d.ID_HANGHOAKHO AS ID, MAX(h.NGAYLAP) AS [Last],
       SUM(CASE WHEN h.NGAYLAP >= @Start AND h.NGAYLAP < @Until THEN d.TONGSOLUONG ELSE 0 END) AS Qty,
       SUM(CASE WHEN h.NGAYLAP >= @Start AND h.NGAYLAP < @Until THEN d.TONGCONG - d.TONGTIENVAT ELSE 0 END) AS Revenue
FROM dbo.ct_PhieuXuat_ChiTiet d
JOIN dbo.ct_PhieuXuat h ON h.LOC_ID = d.LOC_ID AND h.ID = d.ID_PHIEUXUAT
JOIN dbo.dm_LoaiPhieuXuat t ON t.LOC_ID = h.LOC_ID AND t.ID = h.ID_LOAIPHIEUXUAT
JOIN ScopedStock s ON s.ID = d.ID_HANGHOAKHO
WHERE d.LOC_ID = @Branch AND t.TYPE = 2 AND t.ISTRAHANG = 0
  AND d.ISKHUYENMAI = 0 AND d.SOLUONG > 0 AND d.DONGIA > 0 AND h.NGAYLAP < @Tomorrow
  AND NOT EXISTS (SELECT 1 FROM dbo.ct_PhieuChuyen c WHERE c.LOC_ID = @Branch AND c.ID_PHIEUXUATKHAC = h.ID)
GROUP BY d.ID_HANGHOAKHO OPTION (RECOMPILE);
" + scope + @"
SELECT d.ID_HANGHOAKHO AS ID, SUM(d.TONGSOLUONG) AS Qty
FROM dbo.ct_PhieuXuat_ChiTiet d
JOIN dbo.ct_PhieuXuat h ON h.LOC_ID = d.LOC_ID AND h.ID = d.ID_PHIEUXUAT
JOIN dbo.dm_LoaiPhieuXuat t ON t.LOC_ID = h.LOC_ID AND t.ID = h.ID_LOAIPHIEUXUAT
JOIN ScopedStock s ON s.ID = d.ID_HANGHOAKHO
WHERE d.LOC_ID = @Branch AND t.ISTRAHANG = 1 AND d.SOLUONG > 0
  AND h.NGAYLAP >= @Start AND h.NGAYLAP < @Until
  AND NOT EXISTS (SELECT 1 FROM dbo.ct_PhieuChuyen c WHERE c.LOC_ID = @Branch AND c.ID_PHIEUXUATKHAC = h.ID)
GROUP BY d.ID_HANGHOAKHO OPTION (RECOMPILE);
" + scope + @"
SELECT d.ID_HANGHOAKHO AS ID, MIN(h.NGAYLAP) AS [First], MAX(h.NGAYLAP) AS [Last],
       SUM(CASE WHEN t.ISNHAPTRAHANG = 1 AND d.DONGIA > 0 AND h.NGAYLAP >= @Start AND h.NGAYLAP < @Until
                THEN d.TONGSOLUONG ELSE 0 END) AS [Returns],
       SUM(CASE WHEN t.ISNHAPTRAHANG = 1 AND d.DONGIA > 0 AND h.NGAYLAP >= @Start AND h.NGAYLAP < @Until
                THEN d.TONGCONG - d.TONGTIENVAT ELSE 0 END) AS ReturnedRevenue
FROM dbo.ct_PhieuNhap_ChiTiet d
JOIN dbo.ct_PhieuNhap h ON h.LOC_ID = d.LOC_ID AND h.ID = d.ID_PHIEUNHAP
JOIN dbo.dm_LoaiPhieuNhap t ON t.LOC_ID = h.LOC_ID AND t.ID = h.ID_LOAIPHIEUNHAP
JOIN ScopedStock s ON s.ID = d.ID_HANGHOAKHO
WHERE d.LOC_ID = @Branch AND d.SOLUONG > 0 AND h.NGAYLAP < @Tomorrow
  AND NOT EXISTS (SELECT 1 FROM dbo.ct_PhieuChuyen c WHERE c.LOC_ID = @Branch AND c.ID_PHIEUNHAPKHAC = h.ID)
GROUP BY d.ID_HANGHOAKHO OPTION (RECOMPILE);";

        var connection = db.Database.GetDbConnection();
        var openedHere = connection.State != ConnectionState.Open;
        try
        {
            if (openedHere) await connection.OpenAsync(cancellationToken);
            using var result = await connection.QueryMultipleAsync(new CommandDefinition(sql, new
            {
                Branch = request.LOC_ID, IncludeInactive = request.INCLUDE_INACTIVE,
                Depot = request.DEPOT ?? "", Search = search, Groups = groups,
                Start = start, Until = until, Tomorrow = tomorrow
            }, commandTimeout: 80, cancellationToken: cancellationToken));
            var totals = new InventoryInsightTotals();
            totals.Sales = (await result.ReadAsync<SaleTotal>()).ToDictionary(x => x.ID);
            totals.SupplierReturns = (await result.ReadAsync<SupplierReturnTotal>()).ToDictionary(x => x.ID);
            totals.Receipts = (await result.ReadAsync<ReceiptTotal>()).ToDictionary(x => x.ID);
            return totals;
        }
        finally
        {
            if (openedHere) await connection.CloseAsync();
        }
    }

    internal sealed class SaleTotal
    {
        public string ID { get; set; } = "";
        public DateTime? Last { get; set; }
        public double Qty { get; set; }
        public double Revenue { get; set; }
    }
    internal sealed class SupplierReturnTotal
    {
        public string ID { get; set; } = "";
        public double Qty { get; set; }
    }
    internal sealed class ReceiptTotal
    {
        public string ID { get; set; } = "";
        public DateTime? First { get; set; }
        public DateTime? Last { get; set; }
        public double Returns { get; set; }
        public double ReturnedRevenue { get; set; }
    }
}
