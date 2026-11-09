using DatabaseTHP;
using Microsoft.EntityFrameworkCore;
using MyApiNetCore6.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiNetCore6.Services;

[Table("ct_GiaVonDauKy")]
public class OpeningStockCost
{
    public string LOC_ID { get; set; } = "";
    public string ID_HANGHOAKHO { get; set; } = "";
    [Column(TypeName = "date")]
    public DateTime TUNGAY { get; set; }
    [Precision(18, 6)]
    public decimal SOLUONG { get; set; }
    [Precision(18, 6)]
    public decimal DONGIA { get; set; }
}

public static class StockCostPosting
{
    public static Task LockStock(dbTrangHiepPhatContext db, string loc, string stock) =>
        db.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT ID FROM dbo.dm_HangHoa_Kho WITH (UPDLOCK,HOLDLOCK) WHERE LOC_ID={loc} AND ID={stock}");

    public static Task Recalculate(dbTrangHiepPhatContext db, string loc, string stock) =>
        db.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.TinhGiaVonHangKho {loc}, {stock}");

    // Called inside the voucher's transaction. Include removed lines and both sides of stock edits.
    public static async Task SaveMovements(dbTrangHiepPhatContext db)
    {
        var stocks = new HashSet<(string loc, string stock)>();
        foreach (var entry in db.ChangeTracker.Entries())
        {
            if (entry.Entity is dm_HangHoa_Kho stock && entry.State == EntityState.Modified)
                stocks.Add((stock.LOC_ID, stock.ID));
            if (entry.Entity is not (ct_PhieuNhap_ChiTiet or ct_PhieuXuat_ChiTiet)) continue;
            if (entry.State != EntityState.Deleted)
                stocks.Add(((string)entry.CurrentValues["LOC_ID"]!, (string)entry.CurrentValues["ID_HANGHOAKHO"]!));
            if (entry.State != EntityState.Added)
                stocks.Add(((string)entry.OriginalValues["LOC_ID"]!, (string)entry.OriginalValues["ID_HANGHOAKHO"]!));
        }
        var ordered = stocks.OrderBy(x => x.loc, StringComparer.Ordinal).ThenBy(x => x.stock, StringComparer.Ordinal).ToArray();
        foreach (var stock in ordered) await LockStock(db, stock.loc, stock.stock);
        await db.SaveChangesAsync();
        foreach (var stock in ordered) await Recalculate(db, stock.loc, stock.stock);
    }
}
