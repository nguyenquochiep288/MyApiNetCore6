using System.Text.RegularExpressions;
using DatabaseTHP;
using DatabaseTHP.Class;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApiNetCore6.Data;

namespace MyApiNetCore6.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "User")]
public class PriceFormulaController : ControllerBase
{
    private static readonly string[] BuiltInVariables = { "GIAMUA", "TIEN_XANG", "PHI_VAN_CHUYEN", "LOI_NHUAN", "THUE_SUAT" };
    private static readonly string[] OutputFields = { "GIAMUA", "GIA01", "GIA02", "GIA03" };
    private readonly dbTrangHiepPhatContext _context;

    public PriceFormulaController(dbTrangHiepPhatContext context) => _context = context;

    [HttpGet("{LOC_ID}")]
    public async Task<IActionResult> GetList(string LOC_ID)
    {
        var formulas = await _context.dm_CongThucGia
            .AsNoTracking()
            .Include(x => x.Components)
            .Include(x => x.Groups)
            .Where(x => x.LOC_ID == LOC_ID)
            .OrderBy(x => x.NAME)
            .ToListAsync();
        var groups = await _context.dm_NhomHangHoa.Where(x => x.LOC_ID == LOC_ID).ToDictionaryAsync(x => x.ID, x => x.NAME);
        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Success",
            Data = formulas.Select(x => ToResponse(x, groups)).ToList()
        });
    }

    [HttpGet("{LOC_ID}/{ID}")]
    public async Task<IActionResult> GetDetail(string LOC_ID, string ID)
    {
        var formula = await _context.dm_CongThucGia.AsNoTracking().Include(x => x.Components).Include(x => x.Groups)
            .FirstOrDefaultAsync(x => x.LOC_ID == LOC_ID && x.ID == ID);
        if (formula == null) return Ok(Failure("Không tìm thấy công thức giá."));
        var groups = await _context.dm_NhomHangHoa.Where(x => x.LOC_ID == LOC_ID).ToDictionaryAsync(x => x.ID, x => x.NAME);
        return Ok(new ApiResponse { Success = true, Message = "Success", Data = ToResponse(formula, groups) });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PriceFormulaRequest request)
    {
        var validation = await ValidateRequest(request, request.LOC_ID);
        if (validation != null) return Ok(Failure(validation));
        if (await _context.dm_CongThucGia.AnyAsync(x => x.LOC_ID == request.LOC_ID && x.MA == request.MA))
            return Ok(Failure("Mã công thức đã tồn tại trong chi nhánh."));
        if (HasActiveOverlap(request, request.LOC_ID, null)) return Ok(Failure("Một nhóm hàng đã chọn có công thức cùng loại giá bị trùng khoảng hiệu lực."));

        string formulaId = Guid.NewGuid().ToString();
        var entity = BuildEntity(request, formulaId, request.LOC_ID, User.Identity?.Name);
        _context.dm_CongThucGia.Add(entity);
        await _context.SaveChangesAsync();
        var groupNames = await _context.dm_NhomHangHoa.Where(x => x.LOC_ID == request.LOC_ID).ToDictionaryAsync(x => x.ID, x => x.NAME);
        return Ok(new ApiResponse { Success = true, Message = "Đã tạo công thức giá.", Data = ToResponse(entity, groupNames) });
    }

    [HttpPut("{LOC_ID}/{ID}")]
    public async Task<IActionResult> Update(string LOC_ID, string ID, [FromBody] PriceFormulaRequest request)
    {
        if (!string.Equals(LOC_ID, request.LOC_ID, StringComparison.Ordinal)) return Ok(Failure("Chi nhánh không khớp."));
        var validation = await ValidateRequest(request, LOC_ID);
        if (validation != null) return Ok(Failure(validation));
        var entity = await _context.dm_CongThucGia.Include(x => x.Components).Include(x => x.Groups)
            .FirstOrDefaultAsync(x => x.LOC_ID == LOC_ID && x.ID == ID);
        if (entity == null) return Ok(Failure("Không tìm thấy công thức giá."));
        if (await _context.dm_CongThucGia.AnyAsync(x => x.LOC_ID == LOC_ID && x.ID != ID && x.MA == request.MA))
            return Ok(Failure("Mã công thức đã tồn tại trong chi nhánh."));
        if (HasActiveOverlap(request, LOC_ID, ID)) return Ok(Failure("Một nhóm hàng đã chọn có công thức cùng loại giá bị trùng khoảng hiệu lực."));

        entity.MA = request.MA.Trim();
        entity.NAME = request.NAME.Trim();
        entity.LOAIGIA = request.LOAIGIA;
        entity.CONGTHUC = request.CONGTHUC.Trim();
        entity.TINHTHUE = request.TINHTHUE;
        entity.TUNGAY = request.TUNGAY.Date;
        entity.DENNGAY = request.DENNGAY?.Date;
        entity.ISACTIVE = request.ISACTIVE;
        entity.THOIGIANSUA = DateTime.Now;
        entity.ID_NGUOISUA = User.Identity?.Name;
        _context.dm_CongThucGia_ThanhPhan.RemoveRange(entity.Components);
        entity.Components = BuildComponents(request, LOC_ID, ID);
        _context.dm_CongThucGia_NhomHangHoa.RemoveRange(entity.Groups);
        entity.Groups = BuildGroups(request, LOC_ID, ID);
        await _context.SaveChangesAsync();
        var groupNames = await _context.dm_NhomHangHoa.Where(x => x.LOC_ID == LOC_ID).ToDictionaryAsync(x => x.ID, x => x.NAME);
        return Ok(new ApiResponse { Success = true, Message = "Đã cập nhật công thức giá.", Data = ToResponse(entity, groupNames) });
    }

    [HttpDelete("{LOC_ID}/{ID}")]
    public async Task<IActionResult> Delete(string LOC_ID, string ID)
    {
        var entity = await _context.dm_CongThucGia.FirstOrDefaultAsync(x => x.LOC_ID == LOC_ID && x.ID == ID);
        if (entity == null) return Ok(Failure("Không tìm thấy công thức giá."));
        entity.ISACTIVE = false;
        entity.THOIGIANSUA = DateTime.Now;
        entity.ID_NGUOISUA = User.Identity?.Name;
        await _context.SaveChangesAsync();
        return Ok(new ApiResponse { Success = true, Message = "Đã ngừng áp dụng công thức giá.", Data = entity });
    }

    [HttpPost("Calculate")]
    public IActionResult Calculate([FromBody] PriceFormulaCalculateRequest request)
    {
        try
        {
            decimal beforeTax = PriceFormulaCalculator.Evaluate(request.CONGTHUC, request.GIA_TRI);
            decimal afterTax = request.TINHTHUE ? beforeTax * (1 + request.THUE_SUAT / 100m) : beforeTax;
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Công thức hợp lệ.",
                Data = new PriceFormulaCalculateResponse
                {
                    GIA_TRUOC_THUE = decimal.Round(beforeTax, 2, MidpointRounding.AwayFromZero),
                    GIA_SAU_THUE = decimal.Round(afterTax, 2, MidpointRounding.AwayFromZero)
                }
            });
        }
        catch (Exception ex) when (ex is FormatException || ex is DivideByZeroException || ex is OverflowException)
        {
            return Ok(Failure(ex.Message));
        }
    }

    [HttpPost("PreviewBatch")]
    public async Task<IActionResult> PreviewBatch([FromBody] PriceCalculationBatchRequest request)
    {
        var validation = ValidateBatchRequest(request);
        if (validation != null) return Ok(Failure(validation));
        try
        {
            if (request.DRAFT_FORMULA != null)
            {
                request.DRAFT_FORMULA.LOC_ID = request.LOC_ID;
                var draftError = await ValidateRequest(request.DRAFT_FORMULA, request.LOC_ID);
                if (draftError != null) return Ok(Failure(draftError));
                request.LOAIGIA = request.DRAFT_FORMULA.LOAIGIA;
                request.ID_NHOMHANGHOAS = request.DRAFT_FORMULA.ID_NHOMHANGHOAS;
            }
            var rows = await BuildBatchPreviewAsync(request);
            return Ok(new ApiResponse { Success = true, Message = "Đã tính xong danh sách xem trước.", Data = rows });
        }
        catch (Exception ex)
        {
            return Ok(Failure("Không thể tính danh sách giá: " + ex.Message));
        }
    }

    [HttpPost("CalculationProducts")]
    public async Task<IActionResult> CalculationProducts([FromBody] PriceCalculationBatchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.LOC_ID) || !OutputFields.Contains(request.LOAIGIA, StringComparer.OrdinalIgnoreCase))
            return Ok(Failure("Chi nhánh hoặc loại giá không hợp lệ."));
        var groups = request.ID_NHOMHANGHOAS ?? new List<string>();
        var search = request.SEARCH?.Trim() ?? "";
        var query = _context.dm_HangHoa.AsNoTracking().Where(x => x.LOC_ID == request.LOC_ID);
        if (groups.Count > 0) query = query.Where(x => groups.Contains(x.ID_NHOMHANGHOA));
        if (search.Length > 0) query = query.Where(x => x.MA.Contains(search) || x.NAME.Contains(search));
        var products = await query.OrderBy(x => x.MA).Take(1001).ToListAsync();
        if (products.Count > 1000) return Ok(Failure("Phạm vi có hơn 1.000 hàng hóa. Hãy chọn thêm nhóm hoặc nhập mã/tên hàng để thu hẹp danh sách."));
        return Ok(new ApiResponse
        {
            Success = true, Message = $"Đã nạp {products.Count} hàng hóa.",
            Data = products.Select(x => new PriceCalculationBatchItem { MAHANG = x.MA, TEN_HANG = x.NAME, GIAHIENTAI = (decimal)GetProductPrice(x, request.LOAIGIA) }).ToList()
        });
    }

    [HttpPost("ComparePrices")]
    public async Task<IActionResult> ComparePrices([FromBody] PriceComparisonRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.LOC_ID) || !OutputFields.Contains(request.LOAIGIA, StringComparer.OrdinalIgnoreCase))
            return Ok(Failure("Chi nhánh hoặc loại giá bán không hợp lệ."));
        var groups = (request.ID_NHOMHANGHOAS ?? new List<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var search = request.SEARCH?.Trim() ?? "";
        var productQuery = _context.dm_HangHoa.AsNoTracking().Where(x => x.LOC_ID == request.LOC_ID);
        if (groups.Count > 0) productQuery = productQuery.Where(x => groups.Contains(x.ID_NHOMHANGHOA));
        if (search.Length > 0) productQuery = productQuery.Where(x => x.MA.Contains(search) || x.NAME.Contains(search));
        var priceType = request.LOAIGIA.ToUpperInvariant();
        var totalProducts = await productQuery.CountAsync();
        // Filter against the latest eligible receipt in SQL before paging products.
        var comparedQuery = productQuery.Select(product => new
        {
            Product = product,
            SalePrice = priceType == "GIA01" ? product.GIA01 : priceType == "GIA02" ? product.GIA02 : priceType == "GIA03" ? product.GIA03 : product.GIAMUA,
            PurchasePrice = (
                from detail in _context.ct_PhieuNhap_ChiTiet
                join receipt in _context.ct_PhieuNhap
                    on new { detail.LOC_ID, ID = detail.ID_PHIEUNHAP } equals new { receipt.LOC_ID, ID = receipt.ID }
                join stockProduct in _context.dm_HangHoa_Kho
                    on new { detail.LOC_ID, ID = detail.ID_HANGHOAKHO } equals new { stockProduct.LOC_ID, stockProduct.ID }
                where detail.LOC_ID == request.LOC_ID && stockProduct.ID_HANGHOA == product.ID
                    && receipt.NGAYLAP.Date <= request.NGAYTINH.Date && !detail.ISCOMBO && detail.DONGIA > 0
                    && (detail.ID_DVT == product.ID_DVT || detail.ID_DVT == product.ID_DVT_QD)
                orderby receipt.NGAYLAP descending, receipt.THOIGIANSUA descending, receipt.THOIGIANTHEM descending, receipt.ID descending, detail.ID descending
                select (double?)(detail.ID_DVT == product.ID_DVT_QD && detail.ID_DVT != product.ID_DVT && product.TYLE_QD > 1d
                    ? detail.DONGIA * product.TYLE_QD : detail.DONGIA)
            ).FirstOrDefault()
        });
        var belowQuery = comparedQuery.Where(x => x.PurchasePrice.HasValue && x.SalePrice < x.PurchasePrice.Value);
        var belowCount = await belowQuery.CountAsync();
        var total = request.ONLY_BELOW ? belowCount : totalProducts;
        const int pageSize = 100;
        var pages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
        var page = Math.Clamp(request.PAGE, 1, pages);
        var filteredProducts = request.ONLY_BELOW ? belowQuery.Select(x => x.Product) : productQuery;
        var products = await filteredProducts.OrderBy(x => x.MA).ThenBy(x => x.ID)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var productIds = products.Select(x => x.ID).ToList();
        var purchaseRows = await (
            from detail in _context.ct_PhieuNhap_ChiTiet.AsNoTracking()
            join receipt in _context.ct_PhieuNhap.AsNoTracking()
                on new { detail.LOC_ID, ID = detail.ID_PHIEUNHAP } equals new { receipt.LOC_ID, ID = receipt.ID }
            join stockProduct in _context.dm_HangHoa_Kho.AsNoTracking()
                on new { detail.LOC_ID, ID = detail.ID_HANGHOAKHO } equals new { stockProduct.LOC_ID, stockProduct.ID }
            join product in _context.dm_HangHoa.AsNoTracking()
                on new { detail.LOC_ID, ID = stockProduct.ID_HANGHOA } equals new { product.LOC_ID, ID = product.ID }
            where detail.LOC_ID == request.LOC_ID && productIds.Contains(stockProduct.ID_HANGHOA)
                && receipt.NGAYLAP.Date <= request.NGAYTINH.Date && !detail.ISCOMBO
                && (detail.ID_DVT == product.ID_DVT || detail.ID_DVT == product.ID_DVT_QD)
                && detail.DONGIA > 0
            orderby receipt.NGAYLAP descending, receipt.THOIGIANSUA descending, receipt.THOIGIANTHEM descending, receipt.ID descending, detail.ID descending
            select new
            {
                ProductId = stockProduct.ID_HANGHOA,
                PurchasePrice = detail.ID_DVT == product.ID_DVT_QD
                    && detail.ID_DVT != product.ID_DVT
                    && product.TYLE_QD > 1d
                    ? detail.DONGIA * product.TYLE_QD
                    : detail.DONGIA,
                receipt.NGAYLAP,
                receipt.MAPHIEU,
                receipt.THOIGIANSUA,
                receipt.THOIGIANTHEM
            }).ToListAsync();
        var purchases = purchaseRows.GroupBy(x => x.ProductId, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.NGAYLAP).ThenByDescending(x => x.THOIGIANSUA).ThenByDescending(x => x.THOIGIANTHEM).First(), StringComparer.OrdinalIgnoreCase);
        var groupIds = products.Select(x => x.ID_NHOMHANGHOA).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
        var groupNames = await _context.dm_NhomHangHoa.AsNoTracking().Where(x => x.LOC_ID == request.LOC_ID && groupIds.Contains(x.ID))
            .ToDictionaryAsync(x => x.ID, x => x.NAME, StringComparer.OrdinalIgnoreCase);
        var unitIds = products.SelectMany(x => new[] { x.ID_DVT, x.ID_DVT_QD }).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
        var unitNames = await _context.dm_DonViTinh.AsNoTracking().Where(x => x.LOC_ID == request.LOC_ID && unitIds.Contains(x.ID))
            .ToDictionaryAsync(x => x.ID, x => x.NAME, StringComparer.OrdinalIgnoreCase);

        var rows = products.Select(product =>
        {
            var salePrice = (decimal)GetProductPrice(product, request.LOAIGIA);
            var hasConversion = HasConvertedUnit(product);
            var hasPurchase = purchases.TryGetValue(product.ID, out var purchase);
            decimal? purchasePrice = hasPurchase ? (decimal)purchase!.PurchasePrice : null;
            var row = new PriceComparisonRow
            {
                MAHANG = product.MA,
                TEN_HANG = product.NAME,
                TEN_NHOMHANG = groupNames.TryGetValue(product.ID_NHOMHANGHOA ?? "", out var groupName) ? groupName : "",
                GIA_BAN_HIENTAI = salePrice,
                BAN_THAP_HON_GIA_NHAP = purchasePrice.HasValue && salePrice < purchasePrice.Value,
                TRANGTHAI = !purchasePrice.HasValue ? "Chưa có phiếu nhập hợp lệ" : salePrice < purchasePrice.Value ? "Giá bán thấp hơn giá nhập" : "Giá bán từ bằng giá nhập"
            };
            if (hasConversion)
            {
                row.TYLE_QD = (decimal)product.TYLE_QD;
                var baseUnit = unitNames.TryGetValue(product.ID_DVT ?? "", out var baseName) ? baseName : product.ID_DVT ?? "ĐVT";
                var convertedUnit = unitNames.TryGetValue(product.ID_DVT_QD ?? "", out var convertedName) ? convertedName : product.ID_DVT_QD ?? "ĐVT quy đổi";
                row.TYLE_QD_TEXT = $"1 {baseUnit} / {product.TYLE_QD.ToString("N0", System.Globalization.CultureInfo.InvariantCulture)} {convertedUnit}";
                row.GIA_BAN_QD_HIENTAI = (decimal)GetProductConvertedPrice(product, request.LOAIGIA);
                if (purchasePrice.HasValue) row.GIA_NHAP_QD_GANNHAT = decimal.Round(purchasePrice.Value / (decimal)product.TYLE_QD, 2, MidpointRounding.AwayFromZero);
            }
            if (hasPurchase)
            {
                row.GIA_NHAP_GANNHAT = purchasePrice;
                row.NGAY_NHAP_GANNHAT = purchase!.NGAYLAP;
                row.MAPHIEU_NHAP_GANNHAT = purchase.MAPHIEU ?? "";
            }
            return row;
        }).ToList();
        return Ok(new ApiResponse { Success = true, Message = $"Đã đối chiếu toàn bộ {totalProducts} sản phẩm phù hợp.",
            Data = new { ROWS = rows, TOTAL = total, TOTAL_PRODUCTS = totalProducts, BELOW_COUNT = belowCount, PAGE = page, PAGES = pages, PAGE_SIZE = pageSize } });
    }

    [HttpPost("ApplyBatch")]
    public async Task<IActionResult> ApplyBatch([FromBody] PriceCalculationApplyRequest request)
    {
        if (request.DRAFT_FORMULA != null)
            return Ok(Failure("Hãy lưu công thức và tính xem trước lại trước khi áp dụng giá."));
        var validation = ValidateBatchRequest(request);
        if (validation != null) return Ok(Failure(validation));
        var selectedCodes = (request.MAHANG_APDUNG ?? new List<string>()).Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (selectedCodes.Count == 0) return Ok(Failure("Vui lòng chọn ít nhất một hàng hóa để áp dụng giá."));

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var preview = await BuildBatchPreviewAsync(request);
            var selectedRows = preview.Where(x => selectedCodes.Contains(x.MAHANG)).ToList();
            var readyRows = selectedRows.Where(x => x.SAN_SANG_AP_DUNG).ToList();
            if (readyRows.Count == 0)
                return Ok(Failure("Không có hàng hóa được chọn đủ điều kiện cập nhật. Hãy kiểm tra trạng thái từng dòng."));

            var adjustedPrices = request.GIA_MOI_APDUNG ?? new Dictionary<string, decimal>();
            var adjustedConvertedPrices = request.GIA_QD_MOI_APDUNG ?? new Dictionary<string, decimal>();
            foreach (var row in readyRows)
            {
                if (!adjustedPrices.TryGetValue(row.MAHANG, out var adjustedPrice)) continue;
                if (adjustedPrice < 0) return Ok(Failure($"Giá mới của mã {row.MAHANG} không được nhỏ hơn 0."));
                row.GIA_MOI_TINH = decimal.Round(adjustedPrice, 0, MidpointRounding.AwayFromZero);
                row.GIA_QD_MOI = CalculateConvertedPrice(row.GIA_MOI_TINH.Value, row.TYLE_QD);
                if (row.GIA_QD_MOI.HasValue && adjustedConvertedPrices.TryGetValue(row.MAHANG, out var adjustedConvertedPrice))
                {
                    if (adjustedConvertedPrice < 0) return Ok(Failure($"Giá quy đổi mới của mã {row.MAHANG} không được nhỏ hơn 0."));
                    row.GIA_QD_MOI = decimal.Round(adjustedConvertedPrice, 0, MidpointRounding.AwayFromZero);
                }
                row.CHENHLECH = row.GIA_MOI_TINH.Value - row.GIAHIENTAI;
            }

            var productIds = readyRows.Select(x => x.PRODUCT_ID).Distinct().ToList();
            var products = await _context.dm_HangHoa.Where(x => x.LOC_ID == request.LOC_ID && productIds.Contains(x.ID)).ToListAsync();
            var productMap = products.ToDictionary(x => x.ID, StringComparer.OrdinalIgnoreCase);
            var now = DateTime.Now;
            var userId = User.Identity?.Name ?? "";
            var historyIds = new List<string>();
            foreach (var row in readyRows)
            {
                if (!productMap.TryGetValue(row.PRODUCT_ID, out var product) || !row.GIA_MOI_TINH.HasValue || !row.GIA_NHAP_GANNHAT.HasValue)
                    continue;
                var oldPrice = GetProductPrice(product, request.LOAIGIA);
                var oldConvertedPrice = HasConvertedUnit(product) ? (decimal?)GetProductConvertedPrice(product, request.LOAIGIA) : null;
                SetProductPrice(product, request.LOAIGIA, (double)row.GIA_MOI_TINH.Value);
                if (row.GIA_QD_MOI.HasValue)
                    SetProductConvertedPrice(product, request.LOAIGIA, (double)row.GIA_QD_MOI.Value);
                product.THOIGIANSUA = now;
                product.ID_NGUOISUA = userId;
                var historyId = Guid.NewGuid().ToString();
                historyIds.Add(historyId);
                _context.PriceCalculationHistories.Add(new PriceCalculationHistory
                {
                    LOC_ID = request.LOC_ID,
                    ID = historyId,
                    ID_HANGHOA = product.ID,
                    MAHANG = product.MA,
                    TENHANG = product.NAME,
                    LOAIGIA = request.LOAIGIA,
                    GIACU = (decimal)oldPrice,
                    TYLE_QD = row.TYLE_QD,
                    GIA_QD_CU = oldConvertedPrice,
                    GIAMUA_GANNHAT = row.GIA_NHAP_GANNHAT.Value,
                    NGAYNHAP_GANNHAT = row.NGAY_NHAP_GANNHAT,
                    NGUON_GIA_NHAP = row.NGUON_GIA_NHAP,
                    GIA_MOI = row.GIA_MOI_TINH.Value,
                    GIA_QD_MOI = row.GIA_QD_MOI,
                    ID_CONGTHUCGIA = row.ID_CONGTHUCGIA,
                    MA_CONGTHUC = row.MA_CONGTHUC,
                    ID_NGUOITAO = userId,
                    DA_KHOIPHUC = false,
                    THOIGIANTHEM = now
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return Ok(new ApiResponse
            {
                Success = true,
                Message = $"Đã cập nhật {historyIds.Count} hàng hóa.",
                Data = new { UPDATED = historyIds.Count, SKIPPED = selectedRows.Count - historyIds.Count, ROWS = selectedRows, HISTORY_IDS = historyIds }
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Ok(Failure("Không thể áp dụng giá: " + ex.Message));
        }
    }

    [HttpPost("HistoryReport")]
    public async Task<IActionResult> HistoryReport([FromBody] PriceHistoryRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.LOC_ID)) return Ok(Failure("Thiếu chi nhánh."));
        if (request.FROM == default || request.TO == default || request.FROM.Date > request.TO.Date || request.TO.Date >= DateTime.MaxValue.Date)
            return Ok(Failure("Khoảng ngày không hợp lệ."));
        if (!string.IsNullOrEmpty(request.LOAIGIA) && !OutputFields.Contains(request.LOAIGIA))
            return Ok(Failure("Loại giá không hợp lệ."));
        if (!string.IsNullOrEmpty(request.STATUS) && !new[] { "restored", "not-restored", "unknown" }.Contains(request.STATUS))
            return Ok(Failure("Trạng thái không hợp lệ."));
        try
        {
            var from = request.FROM.Date;
            var until = request.TO.Date.AddDays(1);
            var query = _context.PriceCalculationHistories.AsNoTracking()
                .Where(x => x.LOC_ID == request.LOC_ID && x.THOIGIANTHEM >= from && x.THOIGIANTHEM < until);
            if (!string.IsNullOrEmpty(request.LOAIGIA)) query = query.Where(x => x.LOAIGIA == request.LOAIGIA);
            if (!string.IsNullOrWhiteSpace(request.SEARCH))
            {
                var search = request.SEARCH.Trim();
                query = query.Where(x => x.MAHANG.Contains(search) || x.TENHANG.Contains(search));
            }
            if (!string.IsNullOrWhiteSpace(request.FORMULA))
            {
                var code = request.FORMULA.Trim();
                query = query.Where(x => x.MA_CONGTHUC.Contains(code));
            }
            if (!string.IsNullOrWhiteSpace(request.USER))
            {
                var user = request.USER.Trim();
                query = query.Where(x => x.ID_NGUOITAO.Contains(user));
            }
            if (request.STATUS == "restored") query = query.Where(x => x.DA_KHOIPHUC == true);
            if (request.STATUS == "not-restored") query = query.Where(x => x.DA_KHOIPHUC == false);
            if (request.STATUS == "unknown") query = query.Where(x => x.DA_KHOIPHUC == null);
            var total = await query.CountAsync();
            if (request.EXPORT && total > 10000) return Ok(Failure("Có hơn 10.000 dòng. Hãy thu hẹp điều kiện lọc trước khi xuất Excel."));
            const int size = 100;
            var pages = Math.Max(1, (int)Math.Ceiling(total / (double)size));
            var page = Math.Clamp(request.PAGE, 1, pages);
            var rows = await query.OrderByDescending(x => x.THOIGIANTHEM).ThenBy(x => x.ID)
                .Skip(request.EXPORT ? 0 : (page - 1) * size).Take(request.EXPORT ? 10000 : size).ToListAsync();
            var increased = await query.CountAsync(x => x.GIA_MOI > x.GIACU);
            var decreased = await query.CountAsync(x => x.GIA_MOI < x.GIACU);
            var restored = await query.CountAsync(x => x.DA_KHOIPHUC == true);
            return Ok(new ApiResponse { Success = true, Data = new { ROWS = rows, TOTAL = total, PAGE = page, PAGES = pages, INCREASED = increased, DECREASED = decreased, RESTORED = restored } });
        }
        catch (Exception ex)
        {
            // Preserve operational details in server logs, not in the report UI.
            System.Diagnostics.Trace.TraceError("Price history report: " + ex);
            return Ok(Failure("Không tải được lịch sử. Kiểm tra kết nối SQL và chạy script 20261020_PriceFormulaHistoryReport.sql sau script 20261015_PriceFormula.sql."));
        }
    }

    [HttpGet("LatestAppliedBatch/{LOC_ID}")]
    public async Task<IActionResult> LatestAppliedBatch(string LOC_ID)
    {
        var latestTime = await _context.PriceCalculationHistories.AsNoTracking()
            .Where(x => x.LOC_ID == LOC_ID)
            .MaxAsync(x => (DateTime?)x.THOIGIANTHEM);
        if (!latestTime.HasValue)
            return Ok(new ApiResponse { Success = true, Message = "Chưa có lần áp dụng giá nào.", Data = null });

        var histories = await _context.PriceCalculationHistories.AsNoTracking()
            .Where(x => x.LOC_ID == LOC_ID && x.THOIGIANTHEM == latestTime.Value)
            .OrderBy(x => x.MAHANG)
            .ToListAsync();
        var productIds = histories.Select(x => x.ID_HANGHOA).Distinct().ToList();
        var products = await _context.dm_HangHoa.AsNoTracking()
            .Where(x => x.LOC_ID == LOC_ID && productIds.Contains(x.ID))
            .ToDictionaryAsync(x => x.ID, StringComparer.OrdinalIgnoreCase);
        var restorableIds = histories.Where(history => history.DA_KHOIPHUC != true &&
                products.TryGetValue(history.ID_HANGHOA, out var product) &&
                decimal.Round((decimal)GetProductPrice(product, history.LOAIGIA), 4) == decimal.Round(history.GIA_MOI, 4) &&
                ConvertedPriceStillMatches(product, history))
            .Select(x => x.ID)
            .ToList();

        return Ok(new ApiResponse
        {
            Success = true,
            Message = restorableIds.Count > 0 ? "Có thể khôi phục lần áp dụng giá gần nhất." : "Lần áp dụng gần nhất đã được khôi phục hoặc giá đã thay đổi.",
            Data = new
            {
                APPLIED_AT = latestTime.Value,
                APPLIED_BY = histories.Select(x => x.ID_NGUOITAO).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? "",
                TOTAL = histories.Count,
                RESTORABLE = restorableIds.Count,
                HISTORY_IDS = restorableIds
            }
        });
    }

    [HttpPost("RestoreBatch")]
    public async Task<IActionResult> RestoreBatch([FromBody] PriceCalculationRestoreRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.LOC_ID))
            return Ok(Failure("Thiếu thông tin chi nhánh."));
        var historyIds = (request.HISTORY_IDS ?? new List<string>())
            .Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        if (historyIds.Count == 0)
            return Ok(Failure("Không có lịch sử giá để khôi phục."));
        if (historyIds.Count > 1000)
            return Ok(Failure("Mỗi lần chỉ khôi phục tối đa 1.000 hàng hóa."));

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var histories = await _context.PriceCalculationHistories
                .Where(x => x.LOC_ID == request.LOC_ID && historyIds.Contains(x.ID))
                .ToListAsync();
            if (histories.Count == 0)
                return Ok(Failure("Không tìm thấy lịch sử giá cần khôi phục."));
            if (histories.Select(x => x.THOIGIANTHEM).Distinct().Count() != 1)
                return Ok(Failure("Các dòng khôi phục không thuộc cùng một lần áp dụng giá."));

            var productIds = histories.Select(x => x.ID_HANGHOA).Distinct().ToList();
            var products = await _context.dm_HangHoa
                .Where(x => x.LOC_ID == request.LOC_ID && productIds.Contains(x.ID))
                .ToDictionaryAsync(x => x.ID, StringComparer.OrdinalIgnoreCase);
            var now = DateTime.Now;
            var userId = User.Identity?.Name ?? "";
            var restored = new List<object>();
            var skipped = new List<object>();

            foreach (var history in histories)
            {
                if (history.DA_KHOIPHUC == true)
                {
                    skipped.Add(new { history.MAHANG, REASON = "Lịch sử này đã được khôi phục." });
                    continue;
                }
                if (!products.TryGetValue(history.ID_HANGHOA, out var product))
                {
                    skipped.Add(new { history.MAHANG, REASON = "Không tìm thấy hàng hóa." });
                    continue;
                }

                var currentPrice = decimal.Round((decimal)GetProductPrice(product, history.LOAIGIA), 4);
                if (currentPrice != decimal.Round(history.GIA_MOI, 4))
                {
                    skipped.Add(new { history.MAHANG, REASON = "Giá hiện tại đã thay đổi sau lần áp dụng này." });
                    continue;
                }

                if (!ConvertedPriceStillMatches(product, history))
                {
                    skipped.Add(new { history.MAHANG, REASON = "Giá quy đổi hiện tại đã thay đổi sau lần áp dụng này." });
                    continue;
                }

                SetProductPrice(product, history.LOAIGIA, (double)history.GIACU);
                if (history.GIA_QD_CU.HasValue)
                    SetProductConvertedPrice(product, history.LOAIGIA, (double)history.GIA_QD_CU.Value);
                product.THOIGIANSUA = now;
                product.ID_NGUOISUA = userId;
                history.DA_KHOIPHUC = true;
                history.THOIGIAN_KHOIPHUC = now;
                history.ID_NGUOI_KHOIPHUC = userId;
                restored.Add(new { history.MAHANG, history.TENHANG, history.LOAIGIA, GIA_TRUOC_KHOI_PHUC = currentPrice, GIA_DA_KHOI_PHUC = history.GIACU });
            }

            if (restored.Count == 0)
            {
                await transaction.RollbackAsync();
                return Ok(Failure("Không có giá nào được khôi phục vì giá hiện tại đã thay đổi hoặc hàng hóa không còn tồn tại."));
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return Ok(new ApiResponse
            {
                Success = true,
                Message = $"Đã khôi phục giá cũ cho {restored.Count} hàng hóa.",
                Data = new { RESTORED = restored.Count, SKIPPED = skipped.Count, ROWS = restored, SKIPPED_ROWS = skipped }
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Ok(Failure("Không thể khôi phục giá cũ: " + ex.Message));
        }
    }

    private static string? ValidateBatchRequest(PriceCalculationBatchRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.LOC_ID)) return "Thiếu thông tin chi nhánh.";
        if (!OutputFields.Contains(request.LOAIGIA, StringComparer.OrdinalIgnoreCase)) return "Loại giá cần cập nhật không hợp lệ.";
        if (request.NGAYTINH == default) return "Ngày áp dụng không hợp lệ.";
        if (request.ITEMS == null || request.ITEMS.Count == 0) return "File chưa có hàng hóa để tính.";
        if (request.ITEMS.Count > 1000) return "Mỗi lần chỉ xử lý tối đa 1.000 hàng hóa.";
        if (request.ITEMS.Any(x => string.IsNullOrWhiteSpace(x.MAHANG))) return "Có dòng thiếu mã hàng.";
        if (request.ITEMS.GroupBy(x => x.MAHANG.Trim(), StringComparer.OrdinalIgnoreCase).Any(g => g.Count() > 1)) return "File có mã hàng bị trùng. Hãy giữ lại một dòng cho mỗi mã.";
        return null;
    }

    private async Task<List<PriceCalculationBatchRow>> BuildBatchPreviewAsync(PriceCalculationBatchRequest request)
    {
        var codes = request.ITEMS.Select(x => x.MAHANG.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var products = await _context.dm_HangHoa.AsNoTracking()
            .Where(x => x.LOC_ID == request.LOC_ID && codes.Contains(x.MA))
            .ToListAsync();
        var productsByCode = products.GroupBy(x => x.MA, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);
        var productIds = products.Select(x => x.ID).Distinct().ToList();
        var selectedGroups = (request.ID_NHOMHANGHOAS ?? new List<string>()).Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var purchaseRows = await (
            from detail in _context.ct_PhieuNhap_ChiTiet.AsNoTracking()
            join receipt in _context.ct_PhieuNhap.AsNoTracking()
                on new { detail.LOC_ID, ID = detail.ID_PHIEUNHAP } equals new { receipt.LOC_ID, ID = receipt.ID }
            join stockProduct in _context.dm_HangHoa_Kho.AsNoTracking()
                on new { detail.LOC_ID, ID = detail.ID_HANGHOAKHO } equals new { stockProduct.LOC_ID, stockProduct.ID }
            join product in _context.dm_HangHoa.AsNoTracking()
                on new { detail.LOC_ID, ID = stockProduct.ID_HANGHOA } equals new { product.LOC_ID, ID = product.ID }
            where !request.USE_EXCEL_PURCHASE_PRICE && detail.LOC_ID == request.LOC_ID && productIds.Contains(stockProduct.ID_HANGHOA)
                && receipt.NGAYLAP.Date <= request.NGAYTINH.Date && !detail.ISCOMBO
                && (detail.ID_DVT == product.ID_DVT || detail.ID_DVT == product.ID_DVT_QD)
                && detail.DONGIA > 0
            orderby receipt.NGAYLAP descending, receipt.THOIGIANSUA descending, receipt.THOIGIANTHEM descending
            select new
            {
                ProductId = stockProduct.ID_HANGHOA,
                PurchasePrice = detail.ID_DVT == product.ID_DVT_QD
                    && detail.ID_DVT != product.ID_DVT
                    && product.TYLE_QD > 1d
                    ? detail.DONGIA * product.TYLE_QD
                    : detail.DONGIA,
                receipt.NGAYLAP,
                receipt.MAPHIEU,
                receipt.THOIGIANSUA,
                receipt.THOIGIANTHEM
            }).ToListAsync();
        var latestPurchaseByProduct = purchaseRows.GroupBy(x => x.ProductId, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.NGAYLAP).ThenByDescending(x => x.THOIGIANSUA).ThenByDescending(x => x.THOIGIANTHEM).First(), StringComparer.OrdinalIgnoreCase);

        var taxIds = products.Where(x => !string.IsNullOrWhiteSpace(x.ID_THUESUAT)).Select(x => x.ID_THUESUAT).Distinct().ToList();
        var taxes = await _context.dm_ThueSuat.AsNoTracking().Where(x => x.LOC_ID == request.LOC_ID && taxIds.Contains(x.ID))
            .ToDictionaryAsync(x => x.ID, x => (decimal)x.THUESUAT);
        var unitIds = products.SelectMany(x => new[] { x.ID_DVT, x.ID_DVT_QD })
            .Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
        var unitNames = await _context.dm_DonViTinh.AsNoTracking()
            .Where(x => x.LOC_ID == request.LOC_ID && unitIds.Contains(x.ID))
            .ToDictionaryAsync(x => x.ID, x => x.NAME, StringComparer.OrdinalIgnoreCase);
        var formulas = request.DRAFT_FORMULA != null
            ? new List<PriceFormulaRule> { BuildEntity(request.DRAFT_FORMULA, "draft", request.LOC_ID, null) }
            : await _context.dm_CongThucGia.AsNoTracking().Include(x => x.Groups).Include(x => x.Components)
            .Where(x => x.LOC_ID == request.LOC_ID && x.ISACTIVE && x.LOAIGIA == request.LOAIGIA
                && (request.ID_CONGTHUCGIA == null || x.ID == request.ID_CONGTHUCGIA)
                && x.TUNGAY <= request.NGAYTINH.Date && (x.DENNGAY == null || x.DENNGAY >= request.NGAYTINH.Date))
            .ToListAsync();

        var result = new List<PriceCalculationBatchRow>();
        foreach (var input in request.ITEMS)
        {
            var row = new PriceCalculationBatchRow { MAHANG = input.MAHANG.Trim(), TEN_HANG = input.TEN_HANG?.Trim() ?? "", NGUON_GIA_NHAP = request.USE_EXCEL_PURCHASE_PRICE ? "EXCEL" : "PHIEUNHAP" };
            result.Add(row);
            if (!productsByCode.TryGetValue(row.MAHANG, out var matches)) { row.TRANGTHAI = "Không tìm thấy mã hàng"; continue; }
            if (matches.Count != 1) { row.TRANGTHAI = "Mã hàng bị trùng trong danh mục"; continue; }
            var product = matches[0];
            row.GIAHIENTAI = (decimal)GetProductPrice(product, request.LOAIGIA);
            row.PRODUCT_ID = product.ID;
            row.ID_NHOMHANGHOA = product.ID_NHOMHANGHOA;
            if (HasConvertedUnit(product))
            {
                row.TYLE_QD = (decimal)product.TYLE_QD;
                var baseUnitName = unitNames.TryGetValue(product.ID_DVT ?? "", out var baseName) ? baseName : product.ID_DVT ?? "ĐVT";
                var convertedUnitName = unitNames.TryGetValue(product.ID_DVT_QD ?? "", out var convertedName) ? convertedName : product.ID_DVT_QD ?? "ĐVT quy đổi";
                row.TYLE_QD_TEXT = $"1 {baseUnitName} / {product.TYLE_QD.ToString("N0", System.Globalization.CultureInfo.InvariantCulture)} {convertedUnitName}";
                row.GIA_QD_HIENTAI = (decimal)GetProductConvertedPrice(product, request.LOAIGIA);
            }
            var importedNameMismatch = !string.IsNullOrWhiteSpace(row.TEN_HANG) && !String.Equals(row.TEN_HANG, product.NAME, StringComparison.OrdinalIgnoreCase);
            row.TEN_HANG = product.NAME;
            if (selectedGroups.Count > 0 && !selectedGroups.Contains(product.ID_NHOMHANGHOA)) { row.TRANGTHAI = "Ngoài nhóm hàng đã chọn"; continue; }
            var hasPurchase = latestPurchaseByProduct.TryGetValue(product.ID, out var purchase);
            if (request.USE_EXCEL_PURCHASE_PRICE)
            {
                if (input.GIAHIENTAI <= 0) { row.TRANGTHAI = "Giá nhập gốc trong cột GIAHIENTAI phải lớn hơn 0"; continue; }
                row.GIA_NHAP_GANNHAT = input.GIAHIENTAI;
                hasPurchase = true;
            }
            else if (hasPurchase)
            {
                row.GIA_NHAP_GANNHAT = (decimal)purchase!.PurchasePrice;
                row.NGAY_NHAP_GANNHAT = purchase.NGAYLAP;
                row.MAPHIEU_NHAP_GANNHAT = purchase.MAPHIEU ?? "";
            }
            if (hasPurchase && HasConvertedUnit(product))
                row.GIA_NHAP_QD_GANNHAT = decimal.Round(row.GIA_NHAP_GANNHAT.Value / (decimal)product.TYLE_QD, 2, MidpointRounding.AwayFromZero);
            var matchingFormulas = formulas.Where(f => f.Groups.Count == 0 || f.Groups.Any(g => String.Equals(g.ID_NHOMHANGHOA, product.ID_NHOMHANGHOA, StringComparison.OrdinalIgnoreCase)))
                .OrderByDescending(f => f.TUNGAY).ToList();
            if (matchingFormulas.Count == 0) { row.TRANGTHAI = "Không có công thức hiệu lực"; continue; }
            if (matchingFormulas.Count > 1) { row.TRANGTHAI = "Có nhiều công thức cùng hiệu lực"; continue; }

            var formula = matchingFormulas[0];
            row.ID_CONGTHUCGIA = formula.ID;
            row.MA_CONGTHUC = formula.MA;
            row.TEN_CONGTHUC = formula.NAME;
            row.CONGTHUC = formula.CONGTHUC;
            row.CONGTHUC_TUNGAY = formula.TUNGAY;
            row.CONGTHUC_DENNGAY = formula.DENNGAY;
            if (!hasPurchase) { row.TRANGTHAI = "Chưa có phiếu nhập"; continue; }
            var values = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
            {
                ["GIAMUA"] = row.GIA_NHAP_GANNHAT.Value,
                ["TIEN_XANG"] = 0,
                ["PHI_VAN_CHUYEN"] = 0,
                ["LOI_NHUAN"] = 0,
                ["THUE_SUAT"] = !string.IsNullOrWhiteSpace(product.ID_THUESUAT) && taxes.TryGetValue(product.ID_THUESUAT, out var rate) ? rate : 0
            };
            foreach (var component in formula.Components) values[component.MA] = component.GIATRI_MACDINH;
            try
            {
                var beforeTax = PriceFormulaCalculator.Evaluate(formula.CONGTHUC, values);
                var afterTax = formula.TINHTHUE ? beforeTax * (1 + values["THUE_SUAT"] / 100m) : beforeTax;
                row.GIA_TRUOC_THUE = decimal.Round(beforeTax, 2, MidpointRounding.AwayFromZero);
                row.GIA_MOI_TINH = decimal.Round(afterTax, 0, MidpointRounding.AwayFromZero);
                row.GIA_QD_MOI = CalculateConvertedPrice(row.GIA_MOI_TINH.Value, row.TYLE_QD);
                row.CHENHLECH = row.GIA_MOI_TINH.Value - row.GIAHIENTAI;
                row.TRANGTHAI = importedNameMismatch ? "Sẵn sàng · tên file khác danh mục" : "Sẵn sàng";
                row.SAN_SANG_AP_DUNG = true;
            }
            catch (Exception ex) when (ex is FormatException || ex is DivideByZeroException || ex is OverflowException)
            {
                row.TRANGTHAI = "Lỗi công thức: " + ex.Message;
            }
        }
        return result;
    }

    private static double GetProductPrice(dm_HangHoa product, string priceType) => priceType.ToUpperInvariant() switch
    {
        "GIAMUA" => product.GIAMUA,
        "GIA01" => product.GIA01,
        "GIA02" => product.GIA02,
        "GIA03" => product.GIA03,
        _ => 0
    };

    private static void SetProductPrice(dm_HangHoa product, string priceType, double value)
    {
        switch (priceType.ToUpperInvariant())
        {
            case "GIAMUA": product.GIAMUA = value; break;
            case "GIA01": product.GIA01 = value; break;
            case "GIA02": product.GIA02 = value; break;
            case "GIA03": product.GIA03 = value; break;
        }
    }

    private static double GetProductConvertedPrice(dm_HangHoa product, string priceType) => priceType.ToUpperInvariant() switch
    {
        "GIAMUA" => product.GIAMUA_QD,
        "GIA01" => product.GIA01_QD,
        "GIA02" => product.GIA02_QD,
        "GIA03" => product.GIA03_QD,
        _ => 0
    };

    private static void SetProductConvertedPrice(dm_HangHoa product, string priceType, double value)
    {
        switch (priceType.ToUpperInvariant())
        {
            case "GIAMUA": product.GIAMUA_QD = value; break;
            case "GIA01": product.GIA01_QD = value; break;
            case "GIA02": product.GIA02_QD = value; break;
            case "GIA03": product.GIA03_QD = value; break;
        }
    }

    private static bool HasConvertedUnit(dm_HangHoa product) => product.TYLE_QD > 1d && !string.IsNullOrWhiteSpace(product.ID_DVT_QD);

    private static decimal? CalculateConvertedPrice(decimal basePrice, decimal? conversionRate)
    {
        if (!conversionRate.HasValue || conversionRate.Value <= 1m) return null;
        return decimal.Round(basePrice / conversionRate.Value, 0, MidpointRounding.AwayFromZero);
    }

    private static bool ConvertedPriceStillMatches(dm_HangHoa product, PriceCalculationHistory history)
    {
        if (!history.GIA_QD_MOI.HasValue) return true;
        return decimal.Round((decimal)GetProductConvertedPrice(product, history.LOAIGIA), 4) ==
               decimal.Round(history.GIA_QD_MOI.Value, 4);
    }

    [HttpPost("CalculateForGroup")]
    public async Task<IActionResult> CalculateForGroup([FromBody] PriceFormulaGroupCalculateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.LOC_ID) || string.IsNullOrWhiteSpace(request.ID_NHOMHANGHOA))
            return Ok(Failure("Cần truyền chi nhánh và nhóm hàng hóa."));
        if (!OutputFields.Contains(request.LOAIGIA, StringComparer.OrdinalIgnoreCase))
            return Ok(Failure("Loại giá đầu ra không hợp lệ."));

        DateTime date = request.NGAYTINH.Date;
        var formula = await _context.dm_CongThucGia.AsNoTracking().Include(x => x.Components)
            .Where(x => x.LOC_ID == request.LOC_ID && (!x.Groups.Any() || x.Groups.Any(g => g.ID_NHOMHANGHOA == request.ID_NHOMHANGHOA))
                && x.LOAIGIA == request.LOAIGIA && x.ISACTIVE && x.TUNGAY <= date
                && (x.DENNGAY == null || x.DENNGAY >= date))
            .OrderByDescending(x => x.TUNGAY).FirstOrDefaultAsync();
        if (formula == null) return Ok(Failure("Nhóm hàng chưa có công thức giá đang hiệu lực cho ngày được chọn."));

        var values = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
        {
            ["GIAMUA"] = 0, ["TIEN_XANG"] = 0, ["PHI_VAN_CHUYEN"] = 0,
            ["LOI_NHUAN"] = 0, ["THUE_SUAT"] = request.THUE_SUAT
        };
        foreach (var component in formula.Components) values[component.MA] = component.GIATRI_MACDINH;
        foreach (var value in request.GIA_TRI ?? new Dictionary<string, decimal>()) values[value.Key] = value.Value;

        try
        {
            decimal beforeTax = PriceFormulaCalculator.Evaluate(formula.CONGTHUC, values);
            decimal afterTax = formula.TINHTHUE ? beforeTax * (1 + request.THUE_SUAT / 100m) : beforeTax;
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Đã tính theo công thức đang hiệu lực.",
                Data = new
                {
                    formula.ID, formula.MA, formula.NAME, formula.LOAIGIA, formula.CONGTHUC,
                    GIA_TRUOC_THUE = decimal.Round(beforeTax, 2, MidpointRounding.AwayFromZero),
                    GIA_SAU_THUE = decimal.Round(afterTax, 2, MidpointRounding.AwayFromZero)
                }
            });
        }
        catch (Exception ex) when (ex is FormatException || ex is DivideByZeroException || ex is OverflowException)
        {
            return Ok(Failure(ex.Message));
        }
    }

    private async Task<string?> ValidateRequest(PriceFormulaRequest request, string LOC_ID)
    {
        request.MA = request.MA?.Trim() ?? "";
        request.NAME = request.NAME?.Trim() ?? "";
        request.CONGTHUC = request.CONGTHUC?.Trim() ?? "";
        var groupIds = GetRequestedGroupIds(request);
        request.ID_NHOMHANGHOAS = groupIds;
        request.ID_NHOMHANGHOA = groupIds.FirstOrDefault() ?? "";
        if (string.IsNullOrWhiteSpace(LOC_ID) || string.IsNullOrWhiteSpace(request.MA) || string.IsNullOrWhiteSpace(request.NAME))
            return "Vui lòng nhập chi nhánh, mã và tên công thức.";
        if (request.MA.Length > 50 || request.NAME.Length > 200 || request.CONGTHUC.Length > 1000)
            return "Mã, tên hoặc công thức vượt quá độ dài cho phép.";
        if (!OutputFields.Contains(request.LOAIGIA, StringComparer.OrdinalIgnoreCase)) return "Loại giá đầu ra không hợp lệ.";
        if (request.TUNGAY == default || (request.DENNGAY.HasValue && request.DENNGAY.Value.Date < request.TUNGAY.Date))
            return "Khoảng ngày hiệu lực không hợp lệ.";
        if (groupIds.Count > 0)
        {
            var existingGroups = await _context.dm_NhomHangHoa.CountAsync(x => x.LOC_ID == LOC_ID && groupIds.Contains(x.ID));
            if (existingGroups != groupIds.Count) return "Có nhóm hàng hóa không tồn tại trong chi nhánh.";
        }
        var components = request.Components ?? new List<PriceFormulaComponentRequest>();
        if (components.Count > 30) return "Mỗi công thức chỉ được có tối đa 30 thành phần tùy chỉnh.";
        var codes = new HashSet<string>(BuiltInVariables, StringComparer.OrdinalIgnoreCase);
        foreach (var component in components)
        {
            component.MA = component.MA?.Trim().ToUpperInvariant() ?? "";
            component.TEN = component.TEN?.Trim() ?? "";
            component.KIEUGIATRI = component.KIEUGIATRI?.Trim().ToUpperInvariant() ?? "TIEN";
            if (!Regex.IsMatch(component.MA, "^[A-Z_][A-Z0-9_]{0,39}$")) return "Mã thành phần chỉ được dùng chữ in hoa, số và dấu gạch dưới.";
            if (!codes.Add(component.MA)) return "Mã thành phần bị trùng với biến có sẵn hoặc thành phần khác: " + component.MA;
            if (string.IsNullOrWhiteSpace(component.TEN) || component.TEN.Length > 100) return "Tên thành phần không hợp lệ.";
            if (component.KIEUGIATRI != "TIEN" && component.KIEUGIATRI != "PHANTRAM") return "Kiểu thành phần phải là tiền hoặc phần trăm.";
        }
        try
        {
            var sample = codes.ToDictionary(x => x, _ => 1m, StringComparer.OrdinalIgnoreCase);
            PriceFormulaCalculator.Evaluate(request.CONGTHUC, sample);
        }
        catch (Exception ex) when (ex is FormatException || ex is DivideByZeroException || ex is OverflowException)
        {
            return "Công thức chưa hợp lệ: " + ex.Message;
        }
        return null;
    }

    private bool HasActiveOverlap(PriceFormulaRequest request, string LOC_ID, string? exceptId)
    {
        var groupIds = GetRequestedGroupIds(request);
        DateTime requestEnd = request.DENNGAY?.Date ?? DateTime.MaxValue.Date;
        return _context.dm_CongThucGia.Any(x => x.LOC_ID == LOC_ID && x.ISACTIVE && x.ID != exceptId
            && (groupIds.Count == 0 || !x.Groups.Any() || x.Groups.Any(g => groupIds.Contains(g.ID_NHOMHANGHOA)))
            && x.LOAIGIA == request.LOAIGIA
            && x.TUNGAY.Date <= requestEnd && (x.DENNGAY == null || x.DENNGAY.Value.Date >= request.TUNGAY.Date));
    }

    private static PriceFormulaRule BuildEntity(PriceFormulaRequest request, string id, string locId, string? userId)
    {
        return new PriceFormulaRule
        {
            LOC_ID = locId,
            ID = id,
            MA = request.MA.Trim(),
            NAME = request.NAME.Trim(),
            LOAIGIA = request.LOAIGIA,
            CONGTHUC = request.CONGTHUC.Trim(),
            TINHTHUE = request.TINHTHUE,
            TUNGAY = request.TUNGAY.Date,
            DENNGAY = request.DENNGAY?.Date,
            ISACTIVE = request.ISACTIVE,
            THOIGIANTHEM = DateTime.Now,
            ID_NGUOITAO = userId,
            Components = BuildComponents(request, locId, id),
            Groups = BuildGroups(request, locId, id)
        };
    }

    private static List<string> GetRequestedGroupIds(PriceFormulaRequest request)
    {
        var groupIds = request.ID_NHOMHANGHOAS ?? new List<string>();
        if (groupIds.Count == 0 && !string.IsNullOrWhiteSpace(request.ID_NHOMHANGHOA)) groupIds.Add(request.ID_NHOMHANGHOA);
        return groupIds.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    private static List<PriceFormulaGroupLink> BuildGroups(PriceFormulaRequest request, string locId, string formulaId)
    {
        return GetRequestedGroupIds(request).Select(groupId => new PriceFormulaGroupLink
        {
            LOC_ID = locId,
            ID_CONGTHUCGIA = formulaId,
            ID_NHOMHANGHOA = groupId
        }).ToList();
    }

    private static List<PriceFormulaComponent> BuildComponents(PriceFormulaRequest request, string locId, string formulaId)
    {
        return (request.Components ?? new List<PriceFormulaComponentRequest>()).Select(x => new PriceFormulaComponent
        {
            LOC_ID = locId,
            ID = Guid.NewGuid().ToString(),
            ID_CONGTHUCGIA = formulaId,
            MA = x.MA.Trim().ToUpperInvariant(),
            TEN = x.TEN.Trim(),
            KIEUGIATRI = x.KIEUGIATRI.Trim().ToUpperInvariant(),
            GIATRI_MACDINH = x.GIATRI_MACDINH,
            STT = x.STT
        }).ToList();
    }

    private static object ToResponse(PriceFormulaRule formula, Dictionary<string, string> groupNames) => new
    {
        formula.LOC_ID,
        formula.ID,
        formula.MA,
        formula.NAME,
        ID_NHOMHANGHOA = formula.Groups.Select(x => x.ID_NHOMHANGHOA).FirstOrDefault(),
        ID_NHOMHANGHOAS = formula.Groups.Select(x => x.ID_NHOMHANGHOA).ToList(),
        TEN_NHOMHANGHOAS = string.Join(", ", formula.Groups.Select(x => groupNames.TryGetValue(x.ID_NHOMHANGHOA, out string? groupName) ? groupName : x.ID_NHOMHANGHOA)),
        formula.LOAIGIA,
        formula.CONGTHUC,
        formula.TINHTHUE,
        formula.TUNGAY,
        formula.DENNGAY,
        formula.ISACTIVE,
        formula.THOIGIANTHEM,
        formula.THOIGIANSUA,
        Components = formula.Components.OrderBy(x => x.STT).Select(x => new
        {
            x.LOC_ID, x.ID, x.ID_CONGTHUCGIA, x.MA, x.TEN, x.KIEUGIATRI, x.GIATRI_MACDINH, x.STT
        }).ToList()
    };

    private static ApiResponse Failure(string message) => new() { Success = false, Message = message, Data = "" };
}
