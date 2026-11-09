using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyApiNetCore6.Data;
using MyApiNetCore6.Services;

namespace MyApiNetCore6.Controllers;

[ApiController, Route("api/OpeningCost"), Authorize(Roles = "User")]
public class OpeningCostController : ControllerBase
{
    private readonly dbTrangHiepPhatContext db;
    public OpeningCostController(dbTrangHiepPhatContext db) => this.db = db;

    private async Task<bool> CanManage(string loc)
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(username)) return false;
        var groupId = await db.view_AspNetUsers.Where(x => x.UserName == username)
            .Select(x => x.ID_NHOMQUYEN).FirstOrDefaultAsync();
        if (groupId == "-1") return true;
        var group = await db.web_NhomQuyen.AsNoTracking().FirstOrDefaultAsync(x => x.ID == groupId && x.LOC_ID == loc && x.ISACTIVE);
        if (group == null) return false;
        if (!group.ISPHANQUYEN) return true;
        var rights = db.view_web_PhanQuyen.Where(x => x.LOC_ID == loc && x.ID_NHOMQUYEN == groupId && x.TRANGTHAI);
        return await rights.AnyAsync(x => x.CONTROLLERNAME == "Product" && x.MAQUYEN == "Edit")
            && await rights.AnyAsync(x => x.CONTROLLERNAME == "Inventory" && x.MAQUYEN == "View");
    }

    [HttpGet("{loc}/products")]
    public async Task<IActionResult> Products(string loc, string depot, string? search)
    {
        return await LoadProducts(loc, depot, search, Array.Empty<string>(), 1);
    }

    public class OpeningProductsQuery
    {
        public string Depot { get; set; } = "";
        public string? Search { get; set; }
        public string[] Groups { get; set; } = Array.Empty<string>();
        public int Page { get; set; } = 1;
    }

    [HttpPost("{loc}/products-grid")]
    public Task<IActionResult> ProductsGrid(string loc, OpeningProductsQuery request)
    {
        return LoadProducts(loc, request.Depot, request.Search, request.Groups ?? Array.Empty<string>(), request.Page);
    }

    private async Task<IActionResult> LoadProducts(string loc, string depot, string? search, string[] groups, int page)
    {
        if (!await CanManage(loc)) return Forbid();
        if (page < 1 || page > 100000 || groups.Length > 1000 || search?.Length > 200)
            return BadRequest(new { message = "Điều kiện lọc không hợp lệ." });
        var query = from s in db.dm_HangHoa_Kho
                    join p in db.dm_HangHoa on new { s.LOC_ID, ID = s.ID_HANGHOA } equals new { p.LOC_ID, p.ID }
                    join o in db.Set<OpeningStockCost>() on new { s.LOC_ID, ID = s.ID } equals new { o.LOC_ID, ID = o.ID_HANGHOAKHO } into openings
                    from o in openings.DefaultIfEmpty()
                    let unitId = p.TYLE_QD > 1 && p.ID_DVT_QD != null && p.ID_DVT_QD != "" ? p.ID_DVT_QD : p.ID_DVT
                    where s.LOC_ID == loc && s.ID_KHO == depot && (string.IsNullOrEmpty(search) || p.MA.Contains(search) || p.NAME.Contains(search))
                        && (groups.Length == 0 || groups.Contains(p.ID_NHOMHANGHOA))
                    orderby p.MA, s.ID
                    select new { id = s.ID, code = p.MA, name = p.NAME,
                        unit = db.dm_DonViTinh.Where(u => u.LOC_ID == loc && u.ID == unitId).Select(u => u.NAME).FirstOrDefault(),
                        date = (DateTime?)o.TUNGAY, qty = (decimal?)o.SOLUONG, price = (decimal?)o.DONGIA };
        const int pageSize = 50;
        var total = await query.CountAsync();
        page = Math.Min(page, Math.Max(1, (int)Math.Ceiling(total / (double)pageSize)));
        var rows = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return Ok(new { rows, more = page * pageSize < total, total, page, pageSize });
    }

    [HttpPost("{loc}/save")]
    public async Task<IActionResult> Save(string loc, OpeningStockCost input)
    {
        if (!await CanManage(loc)) return Forbid();
        if (string.IsNullOrWhiteSpace(input.ID_HANGHOAKHO) || input.TUNGAY.Date < new DateTime(2000,1,1)
            || input.TUNGAY.Date > DateTime.UtcNow.AddHours(7).Date || input.SOLUONG < 0 || input.DONGIA < 0
            || (input.SOLUONG > 0 && input.DONGIA == 0) || input.SOLUONG >= 1000000000000m || input.DONGIA >= 1000000000000m
            || decimal.Round(input.SOLUONG,6) != input.SOLUONG || decimal.Round(input.DONGIA,6) != input.DONGIA)
            return BadRequest(new { message = "Kiểm tra ngày bắt đầu, số lượng và đơn giá vốn đầu kỳ (tối đa 6 số lẻ)." });
        using var tx = await db.Database.BeginTransactionAsync();
        await StockCostPosting.LockStock(db, loc, input.ID_HANGHOAKHO);
        if (!await db.dm_HangHoa_Kho.AnyAsync(s => s.LOC_ID == loc && s.ID == input.ID_HANGHOAKHO))
            return NotFound(new { message = "Không tìm thấy hàng tại kho." });
        var opening = await db.Set<OpeningStockCost>().FindAsync(loc, input.ID_HANGHOAKHO);
        if (opening == null)
        {
            opening = new OpeningStockCost { LOC_ID = loc, ID_HANGHOAKHO = input.ID_HANGHOAKHO };
            db.Add(opening);
        }
        opening.TUNGAY = input.TUNGAY.Date;
        opening.SOLUONG = input.SOLUONG;
        opening.DONGIA = input.DONGIA;
        await db.SaveChangesAsync();
        try { await StockCostPosting.Recalculate(db, loc, input.ID_HANGHOAKHO); }
        catch (SqlException ex) when (ex.Number >= 51101 && ex.Number <= 51103)
        { return BadRequest(new { message = ex.Message }); }
        await tx.CommitAsync();
        return Ok(new { message = "Đã lưu đầu kỳ và tính lại giá vốn chi tiết phiếu xuất." });
    }

    [HttpGet("{loc}/seeded")]
    public async Task<IActionResult> Seeded(string loc, string depot)
    {
        if (!await CanManage(loc)) return Forbid();
        var rows = await (from s in db.dm_HangHoa_Kho
            join p in db.dm_HangHoa on new { s.LOC_ID, ID = s.ID_HANGHOA } equals new { p.LOC_ID, p.ID }
            join o in db.Set<OpeningStockCost>() on new { s.LOC_ID, ID = s.ID } equals new { o.LOC_ID, ID = o.ID_HANGHOAKHO }
            where s.LOC_ID == loc && s.ID_KHO == depot
            orderby p.MA, s.ID
            select new { id = s.ID, code = p.MA, name = p.NAME, start = o.TUNGAY }).ToListAsync();
        return Ok(new { rows });
    }

    [HttpPost("{loc}/recalculate")]
    public async Task<IActionResult> Recalculate(string loc, string depot, string stock)
    {
        if (!await CanManage(loc)) return Forbid();
        using var tx = await db.Database.BeginTransactionAsync();
        await StockCostPosting.LockStock(db, loc, stock);
        var start = await (from s in db.dm_HangHoa_Kho
            join o in db.Set<OpeningStockCost>() on new { s.LOC_ID, ID = s.ID } equals new { o.LOC_ID, ID = o.ID_HANGHOAKHO }
            where s.LOC_ID == loc && s.ID_KHO == depot && s.ID == stock
            select (DateTime?)o.TUNGAY).SingleOrDefaultAsync();
        if (!start.HasValue) return Ok(new { issue = "Hàng tại kho này chưa có vốn đầu kỳ." });
        try
        {
            await StockCostPosting.Recalculate(db, loc, stock);
            var lines = await (from d in db.ct_PhieuXuat_ChiTiet
                join h in db.ct_PhieuXuat on new { d.LOC_ID, ID = d.ID_PHIEUXUAT } equals new { h.LOC_ID, h.ID }
                where d.LOC_ID == loc && d.ID_HANGHOAKHO == stock && d.SOLUONG != 0 && h.NGAYLAP >= start.Value
                select d.ID).CountAsync();
            await tx.CommitAsync();
            return Ok(new { lines });
        }
        catch (SqlException ex) when (ex.Number >= 51101 && ex.Number <= 51103)
        { return Ok(new { issue = ex.Message }); }
    }
}
