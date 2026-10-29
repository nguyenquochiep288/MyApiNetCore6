using System.Security.Cryptography;
using System.Text;
using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.StoredProcedure.Parameter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using MyApiNetCore6.Data;
using MyApiNetCore6.Models;

namespace MyApiNetCore6.Controllers;

[ApiController]
[Route("api/CustomerPortal")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class CustomerPortalController : ControllerBase
{
    // Legacy tables can have different collations. Normalize UNION projections only;
    // keep branch/customer filters on their native indexed columns.
    private const string PortalCollation = "DATABASE_DEFAULT";
    private readonly dbTrangHiepPhatContext db;
    private readonly IConfiguration config;
    private readonly ILogger<CustomerPortalController> logger;
    public CustomerPortalController(dbTrangHiepPhatContext db, IConfiguration config, ILogger<CustomerPortalController> logger)
    { this.db = db; this.config = config; this.logger = logger; }

    // Management requires both customer-edit permission and view permission for every exposed module.
    private async Task<bool> CanManage(string loc, string customerId)
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username)) return false;
        var user = await db.view_AspNetUsers.AsNoTracking().Where(x => x.UserName == username)
            .Select(x => new { x.ID_NHOMQUYEN }).FirstOrDefaultAsync();
        if (user == null) return false;
        if (user.ID_NHOMQUYEN == "-1") return true;
        var group = await db.web_NhomQuyen.AsNoTracking().Where(x => x.ID == user.ID_NHOMQUYEN && x.ISACTIVE)
            .Select(x => new { x.ID, x.LOC_ID, x.ISPHANQUYEN }).FirstOrDefaultAsync();
        if (group == null) return false;
        // AccountsController maps unrestricted groups to the MVC administrator sentinel -1.
        // The database keeps the group's real ID, so checking that ID alone denies administrators.
        if (!group.ISPHANQUYEN) return true;
        if (group.LOC_ID != loc) return false;
        var rights = await db.view_web_PhanQuyen.AsNoTracking().Where(x => x.LOC_ID == loc && x.ID_NHOMQUYEN == group.ID && x.TRANGTHAI).ToListAsync();
        bool Has(string controller, string right) => rights.Any(x => string.Equals(x.CONTROLLERNAME, controller, StringComparison.OrdinalIgnoreCase) && string.Equals(x.MAQUYEN, right, StringComparison.OrdinalIgnoreCase));
        if (!Has("Customer", "Edit") || !new[] { "Deposit", "Output", "Invoiced", "DebtCustomer" }.All(x => Has(x, "View"))) return false;
        var customer = await db.dm_KhachHang.AsNoTracking().FirstOrDefaultAsync(x => x.LOC_ID == loc && x.ID == customerId);
        if (customer == null) return false;
        var customerGroup = await db.web_PhanQuyenNhomKhachHang.AsNoTracking().FirstOrDefaultAsync(x => x.LOC_ID == loc && x.ID_NHOMQUYEN == group.ID && x.ID_NHOMKHACHHANG == customer.ID_NHOMKHACHHANG && x.ISACTIVE);
        return customerGroup != null && (!customerGroup.ISPHANQUYENKHACHHANG || await db.web_PhanQuyenKhachHang.AnyAsync(x => x.LOC_ID == loc && x.ID_NHOMQUYEN == group.ID && x.ID_KHACHHANG == customerId && x.ISACTIVE));
    }

    [HttpGet("links/{loc}/{customerId}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> LinkStatus(string loc, string customerId)
    {
        if (!await CanManage(loc, customerId)) return Forbid();
        var customer = await db.dm_KhachHang.AsNoTracking().Where(x => x.LOC_ID == loc && x.ID == customerId).Select(x => new { x.MA, x.NAME, x.ISACTIVE }).FirstOrDefaultAsync();
        if (customer == null) return NotFound();
        var link = await db.CustomerPortalLinks.AsNoTracking().FirstOrDefaultAsync(x => x.LocId == loc && x.CustomerId == customerId);
        var active = customer.ISACTIVE && link != null && link.IsActive && (!link.ExpiresUtc.HasValue || link.ExpiresUtc > DateTime.UtcNow);
        var token = active ? PortalLinkTokenStore.Reveal(link!, config["JWT:Secret"]) : null;
        return Ok(new { Customer = customer, Active = active, Token = token, RequiresReissue = active && token == null, link?.CreatedUtc, link?.ExpiresUtc });
    }

    [HttpGet("links/{loc}/{customerId}/history")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> History(string loc, string customerId, int page = 1)
    {
        if (!await CanManage(loc, customerId)) return Forbid();
        if (page < 1 || page > 100000) return BadRequest();
        var query = db.CustomerPortalLinkAudits.AsNoTracking().Where(x => x.LocId == loc && x.CustomerId == customerId);
        return Ok(new { Total = await query.CountAsync(), Page = page, PageSize = 25,
            Rows = await query.OrderByDescending(x => x.OccurredUtc).ThenByDescending(x => x.Id).Skip((page - 1) * 25).Take(25)
                .Select(x => new { x.Action, x.OccurredUtc, x.ActorUsername, x.ActorDisplayName, x.CustomerCode, x.CustomerName, x.ExpiresUtc, x.PreviousExpiresUtc }).ToListAsync() });
    }

    private async Task AppendAudit(CustomerPortalLink link, string action, DateTime? previousExpires)
    {
        // Actor information comes only from the authenticated principal and database.
        // SaveChanges writes the link mutation and this audit event in one EF transaction.
        var username = User.Identity?.Name ?? "";
        var actor = await db.view_AspNetUsers.AsNoTracking().Where(x => x.UserName == username).Select(x => new { x.ID, x.FullName }).FirstOrDefaultAsync();
        var customer = await db.dm_KhachHang.AsNoTracking().Where(x => x.LOC_ID == link.LocId && x.ID == link.CustomerId).Select(x => new { x.MA, x.NAME }).FirstOrDefaultAsync();
        db.CustomerPortalLinkAudits.Add(new CustomerPortalLinkAudit {
            LocId = link.LocId, CustomerId = link.CustomerId, CustomerCode = customer?.MA ?? "", CustomerName = customer?.NAME ?? "",
            Action = action, OccurredUtc = DateTime.UtcNow, ActorUserId = actor?.ID ?? "", ActorUsername = username, ActorDisplayName = actor?.FullName ?? username,
            ExpiresUtc = link.ExpiresUtc, PreviousExpiresUtc = previousExpires
        });
    }

    [HttpPost("links/{loc}/{customerId}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> Issue(string loc, string customerId, PortalLinkRequest request)
    {
        if (!await CanManage(loc, customerId)) return Forbid();
        if (request.ValidDays.HasValue && (request.ValidDays < 1 || request.ValidDays > 3650)) return BadRequest(new { Message = "Thời hạn phải từ 1 đến 3650 ngày." });
        if (!await db.dm_KhachHang.AnyAsync(x => x.LOC_ID == loc && x.ID == customerId && x.ISACTIVE)) return NotFound();
        string token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        var link = await db.CustomerPortalLinks.FirstOrDefaultAsync(x => x.LocId == loc && x.CustomerId == customerId);
        var action = link == null ? "created" : "reissued";
        var previousExpires = link?.ExpiresUtc;
        if (link == null) { link = new CustomerPortalLink { LocId = loc, CustomerId = customerId }; db.CustomerPortalLinks.Add(link); }
        link.TokenHash = Hash(token);
        link.ProtectedToken = PortalLinkTokenStore.Protect(token, config["JWT:Secret"], loc, customerId);
        link.IsActive = true;
        link.CreatedUtc = DateTime.UtcNow;
        link.ExpiresUtc = request.ValidDays.HasValue ? DateTime.UtcNow.AddDays(request.ValidDays.Value) : null;
        link.RevokedUtc = null;
        link.CreatedBy = User.Identity?.Name ?? "";
        await AppendAudit(link, action, previousExpires);
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException ex) when (ex is DbUpdateConcurrencyException || ex.InnerException is SqlException { Number: 2601 or 2627 })
        { return Conflict(new { Message = "Link vừa được thay đổi. Vui lòng tải lại trang." }); }
        return Ok(new { Token = token, link.ExpiresUtc });
    }

    [HttpDelete("links/{loc}/{customerId}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> Revoke(string loc, string customerId)
    {
        if (!await CanManage(loc, customerId)) return Forbid();
        var link = await db.CustomerPortalLinks.FirstOrDefaultAsync(x => x.LocId == loc && x.CustomerId == customerId);
        if (link != null && link.IsActive)
        {
            link.IsActive = false; link.RevokedUtc = DateTime.UtcNow;
            link.ProtectedToken = null;
            await AppendAudit(link, "revoked", link.ExpiresUtc);
            try { await db.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException) { return Conflict(new { Message = "Link vừa được thay đổi. Vui lòng tải lại trang." }); }
        }
        return Ok(new { Success = true });
    }

    internal static string Hash(string token) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    internal static bool ValidToken(string? token) => token != null && token.Length == 43 && token.All(c => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '-' || c == '_');

    [AllowAnonymous]
    [HttpPost("read")]
    public async Task<IActionResult> Read(PortalReadRequest request)
    {
        // No customer/branch selector is accepted from the public caller. Resolve both from the token.
        string? token = Request.Headers["X-Customer-Portal-Token"].FirstOrDefault();
        if (!ValidToken(token)) return NotFound(new { Message = "Link không hợp lệ hoặc đã hết hiệu lực." });
        try
        {
            string hash = Hash(token!);
            var link = await db.CustomerPortalLinks.AsNoTracking().FirstOrDefaultAsync(x => x.TokenHash == hash && x.IsActive && (!x.ExpiresUtc.HasValue || x.ExpiresUtc > DateTime.UtcNow));
            if (link == null) return NotFound(new { Message = "Link không hợp lệ hoặc đã hết hiệu lực." });
            var customer = await db.dm_KhachHang.AsNoTracking().FirstOrDefaultAsync(x => x.LOC_ID == link.LocId && x.ID == link.CustomerId && x.ISACTIVE);
            if (customer == null) return NotFound(new { Message = "Link không hợp lệ hoặc đã hết hiệu lực." });
            if (request.Section == "payment") return await Payment(link, customer, request.Amount);
            var today = DateTime.UtcNow.AddHours(7).Date;
            var monthStart = new DateTime(today.Year, today.Month, 1);
            DateTime from = (request.From ?? monthStart).Date, to = (request.To ?? monthStart.AddMonths(1).AddDays(-1)).Date;
            if (from < new DateTime(1900, 1, 1) || to < from || to >= new DateTime(9999, 12, 31) || (to - from).TotalDays > 1096)
                return BadRequest(new { Message = "Chọn khoảng thời gian hợp lệ, tối đa 3 năm." });
            if (request.Page < 1 || request.Page > 100000 || (request.Search?.Length ?? 0) > 100 || (request.Id?.Length ?? 0) > 50)
                return BadRequest(new { Message = "Tham số tra cứu không hợp lệ." });
            var documents = Documents(link);
            if (!string.IsNullOrEmpty(request.Id))
            {
                var doc = await documents.FirstOrDefaultAsync(x => x.Kind == request.Section && x.Id == request.Id);
                if (doc == null) return NotFound(new { Message = "Không tìm thấy chứng từ." });
                return Ok(new { Document = doc, Lines = await Lines(link, doc) });
            }
            if (request.Section == "overview") return await Overview(link, customer, from, to);
            if (request.Section == "debt") return await Debt(link, customer, from, to, request);
            if (request.Section == "debt-export") return await Statement(link, customer, from, to);
            if (!new[] { "orders", "outputs", "invoices", "receipts" }.Contains(request.Section)) return BadRequest();
            var filtered = documents.Where(x => x.Kind == request.Section && x.Date >= from && x.Date < to.AddDays(1));
            if (!string.IsNullOrWhiteSpace(request.Search)) filtered = filtered.Where(x => x.Code.Contains(request.Search) || (x.InvoiceNumber != null && x.InvoiceNumber.Contains(request.Search)));
            var total = await filtered.CountAsync();
            var sum = await filtered.SumAsync(x => (double?)x.Total) ?? 0;
            var rows = await filtered.OrderByDescending(x => x.Date).ThenBy(x => x.Id).Skip((request.Page - 1) * 25).Take(25).ToListAsync();
            return Ok(new { Rows = rows, Total = total, Sum = sum, Page = request.Page, PageSize = 25 });
        }
        catch (Exception ex)
        {
            // Do not log request bodies, tokens or request URLs.
            logger.LogError("Customer portal read failed ({ErrorType}, SQL {SqlNumber}).", ex.GetType().Name, (ex as SqlException)?.Number);
            return StatusCode(503, new { Message = "Chưa thể tải dữ liệu. Vui lòng thử lại sau." });
        }
    }

    private IQueryable<PortalDocument> Documents(CustomerPortalLink link)
    {
        var orders = db.ct_PhieuDatHang.AsNoTracking().Where(x => x.LOC_ID == link.LocId && x.ID_KHACHHANG == link.CustomerId)
            .Select(x => new PortalDocument { Id = EF.Functions.Collate(x.ID, PortalCollation), Kind = "orders", Code = EF.Functions.Collate(x.MAPHIEU, PortalCollation), Date = x.NGAYLAP, Subtotal = x.TONGTHANHTIEN, Discount = x.TONGTIENGIAMGIA, Tax = x.TONGTIENVAT, Total = x.TONGTIEN, Status = x.ID_PHIEUXUAT != null && x.ID_PHIEUXUAT != "" ? "Đã lập phiếu xuất" : "Chưa lập phiếu xuất", InvoiceNumber = null, InvoiceSeries = null, TaxAuthorityCode = null, LookupCode = null });
        // Use the latest delivery attempt for this output; reopening a delivery must
        // remove Completed, and an old delivered attempt must not mask a new pending one.
        var outputs = from x in db.ct_PhieuXuat.AsNoTracking().Where(x => x.LOC_ID == link.LocId && x.ID_KHACHHANG == link.CustomerId)
                      from delivery in (from detail in db.ct_PhieuGiaoHang_ChiTiet.AsNoTracking()
                                        join header in db.ct_PhieuGiaoHang.AsNoTracking()
                                            on new { detail.LOC_ID, ID = detail.ID_PHIEUGIAOHANG } equals new { header.LOC_ID, header.ID }
                                        where detail.LOC_ID == link.LocId && detail.ID_PHIEUXUAT == x.ID
                                        orderby detail.SOLAN descending, header.NGAYLAP descending, detail.ID descending
                                        select new { Delivered = (bool?)detail.ISDAGIAOHANG, Completed = (bool?)header.ISHOANTAT }).Take(1).DefaultIfEmpty()
                      select new PortalDocument { Id = EF.Functions.Collate(x.ID, PortalCollation), Kind = "outputs", Code = EF.Functions.Collate(x.MAPHIEU, PortalCollation), Date = x.NGAYLAP, Subtotal = x.TONGTHANHTIEN, Discount = x.TONGTIENGIAMGIA, Tax = x.TONGTIENVAT, Total = x.TONGTIEN,
                          Status = delivery.Completed == true ? "\u0048oàn tất" : delivery.Delivered == true ? "Đã giao hàng" : "Chưa giao hàng",
                          InvoiceNumber = null, InvoiceSeries = null, TaxAuthorityCode = null, LookupCode = null };
        var invoices = db.ct_HoaDon.AsNoTracking().Where(x => x.LOC_ID == link.LocId && x.ID_KHACHHANG == link.CustomerId)
            .Select(x => new PortalDocument { Id = EF.Functions.Collate(x.ID, PortalCollation), Kind = "invoices", Code = EF.Functions.Collate(x.MAPHIEU, PortalCollation), Date = x.NGAYHOADON, Subtotal = x.TONGTHANHTIEN, Discount = x.TONGTIENGIAMGIA, Tax = x.TONGTIENVAT, Total = x.TONGTIEN, Status = x.MACQT != null && x.MACQT != "" ? "Đã cấp mã" : x.ISXUATHOADON ? "Chờ cấp mã" : "Chưa xuất MISA", InvoiceNumber = EF.Functions.Collate(x.SOHOADON, PortalCollation), InvoiceSeries = EF.Functions.Collate(x.KYHIEUHOADON, PortalCollation), TaxAuthorityCode = EF.Functions.Collate(x.MACQT, PortalCollation), LookupCode = EF.Functions.Collate(x.MATRACUU_MISA, PortalCollation) });
        var receipts = db.ct_PhieuThu.AsNoTracking().Where(x => x.LOC_ID == link.LocId && x.ID_KHACHHANG == link.CustomerId)
            .Select(x => new PortalDocument { Id = EF.Functions.Collate(x.ID, PortalCollation), Kind = "receipts", Code = EF.Functions.Collate(x.MAPHIEU, PortalCollation), Date = x.NGAYLAP, Subtotal = x.SOTIEN, Discount = 0, Tax = 0, Total = x.SOTIEN, Status = "Đã ghi nhận", InvoiceNumber = null, InvoiceSeries = null, TaxAuthorityCode = null, LookupCode = null });
        return orders.Concat(outputs).Concat(invoices).Concat(receipts);
    }

    private async Task<List<PortalLine>> Lines(CustomerPortalLink link, PortalDocument doc)
    {
        if (doc.Kind == "receipts") return new List<PortalLine>();
        if (doc.Kind == "invoices")
            return await db.ct_HoaDon_ChiTiet.AsNoTracking().Where(x => x.LOC_ID == link.LocId && x.ID_HOADON == doc.Id)
                .OrderBy(x => x.STT).ThenBy(x => x.ID).Select(x => new PortalLine { Position = x.STT, Code = x.MAHANGHOA, Name = x.TENHANGHOA, Unit = x.DVT, Quantity = x.SOLUONG, Price = x.DONGIA, Discount = x.TONGTIENGIAMGIA, Tax = x.TINHCHAT == 3 ? -x.TONGTIENVAT : x.TONGTIENVAT, Total = x.TINHCHAT == 3 ? -x.TONGCONG : x.TONGCONG, Nature = x.TINHCHAT }).ToListAsync();
        // Left joins keep historical document lines visible when catalog data has been removed.
        var lines = doc.Kind == "orders"
            ? db.ct_PhieuDatHang_ChiTiet.AsNoTracking().Where(x => x.LOC_ID == link.LocId && x.ID_PHIEUDATHANG == doc.Id).Select(x => new { x.ID, x.STT, x.ID_HANGHOAKHO, x.ID_DVT, x.SOLUONG, x.DONGIA, x.TONGTIENGIAMGIA, x.TONGTIENVAT, x.TONGCONG })
            : db.ct_PhieuXuat_ChiTiet.AsNoTracking().Where(x => x.LOC_ID == link.LocId && x.ID_PHIEUXUAT == doc.Id).Select(x => new { x.ID, x.STT, x.ID_HANGHOAKHO, x.ID_DVT, x.SOLUONG, x.DONGIA, x.TONGTIENGIAMGIA, x.TONGTIENVAT, x.TONGCONG });
        return await (from line in lines
                      join stock in db.dm_HangHoa_Kho.Where(x => x.LOC_ID == link.LocId) on line.ID_HANGHOAKHO equals stock.ID into stocks
                      from stock in stocks.DefaultIfEmpty()
                      join product in db.dm_HangHoa.Where(x => x.LOC_ID == link.LocId) on stock.ID_HANGHOA equals product.ID into products
                      from product in products.DefaultIfEmpty()
                      join unit in db.dm_DonViTinh.Where(x => x.LOC_ID == link.LocId) on line.ID_DVT equals unit.ID into units
                      from unit in units.DefaultIfEmpty()
                      orderby line.STT, line.ID
                      select new PortalLine { Position = line.STT, Code = product.MA, Name = product.NAME ?? "Hàng hóa không còn trong danh mục", Unit = unit.NAME, Quantity = line.SOLUONG, Price = line.DONGIA, Discount = line.TONGTIENGIAMGIA, Tax = line.TONGTIENVAT, Total = line.TONGCONG }).ToListAsync();
    }

    private async Task<v_ThongKeCongNoKhachHang> DebtSummary(CustomerPortalLink link, DateTime from, DateTime to)
    {
        var parameter = new SP_Parameter { LOC_ID = link.LocId, ID_KHACHHANG = link.CustomerId, ID_NHOMKHACHHANG = "", ID_KHUVUC = "", ISTHEOTHOIGIAN = true, ISPHATSINHCONGNO = true, ISPHATSINHCONGNOTRONGKY = false, ISCONCONGNO = false, TUNGAY = from, DENNGAY = to };
        var result = await new ExecuteStoredProc(db, config).Execute_StoredProc<v_ThongKeCongNoKhachHang>("Sp_Get_ThongKeCongNoKhachHang", ExecuteStoredProc.GetSP_Parameter(parameter));
        if (!result.Success) throw new InvalidOperationException("Debt report unavailable");
        // A report can omit a zero-balance customer; never use a row belonging to another customer.
        if (result.Data is not List<v_ThongKeCongNoKhachHang> rows) throw new InvalidOperationException("Invalid debt report response");
        var row = rows.FirstOrDefault(x => x.ID == link.CustomerId);
        // Some versions of the stored procedure omit LOC_ID. Its input is already fixed to
        // this link's branch; reject a conflicting branch if the procedure returns one.
        if (row != null && !string.IsNullOrEmpty(row.LOC_ID) && row.LOC_ID != link.LocId) throw new InvalidOperationException("Invalid debt report scope");
        return row ?? new v_ThongKeCongNoKhachHang();
    }

    private async Task<IActionResult> Overview(CustomerPortalLink link, dm_KhachHang customer, DateTime from, DateTime to)
    {
        var end = to.AddDays(1);
        // Group on the server. The overview does not fetch every document into memory.
        var query = Documents(link).Where(x => x.Date >= from && x.Date < end);
        var monthly = await query.GroupBy(x => new { x.Kind, x.Date.Year, x.Date.Month }).Select(g => new { g.Key.Kind, g.Key.Year, g.Key.Month, Total = g.Sum(x => x.Total), Count = g.Count() }).ToListAsync();
        var states = await query.GroupBy(x => new { x.Kind, x.Status }).Select(g => new { g.Key.Kind, Name = g.Key.Status, Count = g.Count() }).ToListAsync();
        var groups = await (from line in db.ct_PhieuXuat_ChiTiet.AsNoTracking()
                            join output in db.ct_PhieuXuat.Where(x => x.LOC_ID == link.LocId && x.ID_KHACHHANG == link.CustomerId && x.NGAYLAP >= @from && x.NGAYLAP < end) on new { line.LOC_ID, ID = line.ID_PHIEUXUAT } equals new { output.LOC_ID, output.ID }
                            join stockRow in db.dm_HangHoa_Kho on new { line.LOC_ID, ID = line.ID_HANGHOAKHO } equals new { stockRow.LOC_ID, stockRow.ID } into stocks
                            from stock in stocks.DefaultIfEmpty()
                            join productRow in db.dm_HangHoa on new { stock.LOC_ID, ID = stock.ID_HANGHOA } equals new { productRow.LOC_ID, productRow.ID } into products
                            from product in products.DefaultIfEmpty()
                            join categoryRow in db.dm_NhomHangHoa on new { product.LOC_ID, ID = product.ID_NHOMHANGHOA } equals new { categoryRow.LOC_ID, categoryRow.ID } into categories
                            from groupRow in categories.DefaultIfEmpty()
                            group line by (groupRow.NAME ?? "Chưa phân nhóm") into g
                            select new { Name = g.Key, Total = g.Sum(x => x.TONGCONG) }).ToListAsync();
        var debt = await DebtSummary(link, from, to);
        return Ok(new {
            Customer = new { Code = customer.MA, Name = customer.NAME, Address = customer.ADDRESS }, From = from, To = to,
            Monthly = monthly, States = states, Groups = groups,
            Debt = PublicDebt(debt),
            Recent = await query.OrderByDescending(x => x.Date).ThenBy(x => x.Kind).ThenBy(x => x.Id).Take(8).ToListAsync()
        });
    }

    private static object PublicDebt(v_ThongKeCongNoKhachHang row) => new { Opening = row.TONGTIENCONGNODAUKY, Outputs = row.TONGTIENPHIEUXUAT, Returns = row.TONGTIENPHIEUNHAP, Receipts = row.TONGTIENPHIEUTHU, Payments = row.TONGTIENPHIEUCHI, Closing = row.TONGTIENCONGNOCUOIKY };

    private IQueryable<PortalLedgerEntry> Ledger(CustomerPortalLink link, DateTime from, DateTime to)
    {
        var end = to.AddDays(1);
        return db.ct_PhieuXuat.AsNoTracking().Where(x => x.LOC_ID == link.LocId && x.ID_KHACHHANG == link.CustomerId && x.NGAYLAP >= from && x.NGAYLAP < end).Select(x => new PortalLedgerEntry { Id = EF.Functions.Collate(x.ID, PortalCollation), Kind = "outputs", Code = EF.Functions.Collate(x.MAPHIEU, PortalCollation), Date = x.NGAYLAP, Increase = x.TONGTIEN, Decrease = 0 })
            .Concat(db.ct_PhieuNhap.AsNoTracking().Where(x => x.LOC_ID == link.LocId && x.ID_KHACHHANG == link.CustomerId && x.NGAYLAP >= from && x.NGAYLAP < end).Select(x => new PortalLedgerEntry { Id = EF.Functions.Collate(x.ID, PortalCollation), Kind = "returns", Code = EF.Functions.Collate(x.MAPHIEU, PortalCollation), Date = x.NGAYLAP, Increase = 0, Decrease = x.TONGTIEN }))
            .Concat(db.ct_PhieuThu.AsNoTracking().Where(x => x.LOC_ID == link.LocId && x.ID_KHACHHANG == link.CustomerId && x.NGAYLAP >= from && x.NGAYLAP < end).Select(x => new PortalLedgerEntry { Id = EF.Functions.Collate(x.ID, PortalCollation), Kind = "receipts", Code = EF.Functions.Collate(x.MAPHIEU, PortalCollation), Date = x.NGAYLAP, Increase = 0, Decrease = x.SOTIEN }))
            .Concat(db.ct_PhieuChi.AsNoTracking().Where(x => x.LOC_ID == link.LocId && x.ID_KHACHHANG == link.CustomerId && x.NGAYLAP >= from && x.NGAYLAP < end).Select(x => new PortalLedgerEntry { Id = EF.Functions.Collate(x.ID, PortalCollation), Kind = "payments", Code = EF.Functions.Collate(x.MAPHIEU, PortalCollation), Date = x.NGAYLAP, Increase = x.SOTIEN, Decrease = 0 }));
    }

    private async Task<IActionResult> Debt(CustomerPortalLink link, dm_KhachHang customer, DateTime from, DateTime to, PortalReadRequest request)
    {
        var ledger = Ledger(link, from, to);
        var summary = await DebtSummary(link, from, to);
        if (!string.IsNullOrWhiteSpace(request.Search)) ledger = ledger.Where(x => x.Code != null && x.Code.Contains(request.Search));
        return Ok(new { Summary = PublicDebt(summary), Total = await ledger.CountAsync(), Rows = await ledger.OrderByDescending(x => x.Date).ThenBy(x => x.Kind).ThenBy(x => x.Id).Skip((request.Page - 1) * 25).Take(25).ToListAsync(), Page = request.Page, PageSize = 25 });
    }

    private async Task<IActionResult> Statement(CustomerPortalLink link, dm_KhachHang customer, DateTime from, DateTime to)
    {
        var ledger = Ledger(link, from, to);
        if (await ledger.CountAsync() > 50000) return BadRequest(new { Message = "Kỳ báo cáo vượt 50.000 phát sinh. Vui lòng chọn khoảng thời gian ngắn hơn." });
        var summary = await DebtSummary(link, from, to);
        // The limit is checked again after materialization, so concurrent inserts cannot silently truncate a statement.
        var entries = await ledger.OrderBy(x => x.Date).ThenBy(x => x.Kind).ThenBy(x => x.Id).Take(50001).ToListAsync();
        if (entries.Count > 50000) return BadRequest(new { Message = "Kỳ báo cáo vượt 50.000 phát sinh. Vui lòng chọn khoảng thời gian ngắn hơn." });
        var rows = PortalStatementBuilder.Build(entries, Convert.ToDecimal(summary.TONGTIENCONGNODAUKY));
        var calculated = rows.Count == 0 ? Convert.ToDecimal(summary.TONGTIENCONGNODAUKY) : rows[^1].Balance;
        return Ok(new { Customer = new { Code = customer.MA, Name = customer.NAME, Address = customer.ADDRESS }, From = from, To = to, GeneratedUtc = DateTime.UtcNow,
            Summary = PublicDebt(summary), Rows = rows, Increase = rows.Sum(x => x.Increase), Decrease = rows.Sum(x => x.Decrease), CalculatedClosing = calculated,
            Difference = Convert.ToDecimal(summary.TONGTIENCONGNOCUOIKY) - calculated });
    }

    private async Task<IActionResult> Payment(CustomerPortalLink link, dm_KhachHang customer, decimal? amount)
    {
        if (amount.HasValue && (amount <= 0 || amount > 9999999999999m || decimal.Truncate(amount.Value) != amount.Value))
            return BadRequest(new { Message = "Số tiền phải là số nguyên dương, tối đa 13 chữ số." });
        // Publish only an explicitly selected default receiving account; never expose other internal accounts.
        var banks = await db.dm_TaiKhoanNganHang.AsNoTracking().Where(x => x.LOC_ID == link.LocId && x.ISACTIVE && x.ISDEFAULT)
            .Select(x => new { BankName = x.TENNGANHANG, Bin = x.MANGANHANG, AccountNumber = x.SOTAIKHOAN, AccountName = x.CHUTAIKHOAN }).Take(2).ToListAsync();
        if (banks.Count != 1 || string.IsNullOrWhiteSpace(banks[0].AccountNumber) || string.IsNullOrWhiteSpace(banks[0].AccountName))
            return Ok(new { Available = false, Message = "Chưa có tài khoản nhận tiền được xác nhận. Vui lòng liên hệ Trang Hiệp Phát." });
        var bank = banks[0];
        var reference = PortalVietQr.Reference(customer.MA);
        var payload = PortalVietQr.Build(bank.Bin, bank.AccountNumber, reference, amount);
        return Ok(new { Available = true, Bank = bank, Reference = reference, Amount = amount, Payload = payload,
            Message = payload == null ? "Chưa thể tạo QR cho thông tin ngân hàng này. Bạn có thể chuyển khoản theo thông tin bên trên." : "" });
    }
}
