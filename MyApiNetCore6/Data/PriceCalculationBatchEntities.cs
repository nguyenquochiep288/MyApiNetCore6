namespace MyApiNetCore6.Data;

public class PriceCalculationBatchRequest
{
    public bool USE_EXCEL_PURCHASE_PRICE { get; set; }
    public string LOC_ID { get; set; } = "";
    public string LOAIGIA { get; set; } = "GIA01";
    public DateTime NGAYTINH { get; set; } = DateTime.Today;
    public List<string> ID_NHOMHANGHOAS { get; set; } = new();
    public List<PriceCalculationBatchItem> ITEMS { get; set; } = new();
    public PriceFormulaRequest? DRAFT_FORMULA { get; set; }
    public string? ID_CONGTHUCGIA { get; set; }
    public string? SEARCH { get; set; }
}

public class PriceCalculationBatchItem
{
    public string MAHANG { get; set; } = "";
    public string TEN_HANG { get; set; } = "";
    public decimal GIAHIENTAI { get; set; }
}

public class PriceComparisonRequest
{
    public int PAGE { get; set; } = 1;
    public bool ONLY_BELOW { get; set; }
    public string LOC_ID { get; set; } = "";
    public string LOAIGIA { get; set; } = "GIA01";
    public DateTime NGAYTINH { get; set; } = DateTime.Today;
    public List<string> ID_NHOMHANGHOAS { get; set; } = new();
    public string? SEARCH { get; set; }
}

public class PriceComparisonRow
{
    public string MAHANG { get; set; } = "";
    public string TEN_HANG { get; set; } = "";
    public string TEN_NHOMHANG { get; set; } = "";
    public decimal GIA_BAN_HIENTAI { get; set; }
    public decimal? TYLE_QD { get; set; }
    public string TYLE_QD_TEXT { get; set; } = "";
    public decimal? GIA_BAN_QD_HIENTAI { get; set; }
    public decimal? GIA_NHAP_GANNHAT { get; set; }
    public decimal? GIA_NHAP_QD_GANNHAT { get; set; }
    public DateTime? NGAY_NHAP_GANNHAT { get; set; }
    public string MAPHIEU_NHAP_GANNHAT { get; set; } = "";
    public bool BAN_THAP_HON_GIA_NHAP { get; set; }
    public string TRANGTHAI { get; set; } = "";
}

public class PriceCalculationBatchRow
{
    public string NGUON_GIA_NHAP { get; set; } = "PHIEUNHAP";
    public string MAHANG { get; set; } = "";
    public string TEN_HANG { get; set; } = "";
    public string PRODUCT_ID { get; set; } = "";
    public string ID_NHOMHANGHOA { get; set; } = "";
    public decimal GIAHIENTAI { get; set; }
    public decimal? TYLE_QD { get; set; }
    public string TYLE_QD_TEXT { get; set; } = "";
    public decimal? GIA_QD_HIENTAI { get; set; }
    public decimal? GIA_NHAP_GANNHAT { get; set; }
    public decimal? GIA_NHAP_QD_GANNHAT { get; set; }
    public DateTime? NGAY_NHAP_GANNHAT { get; set; }
    public string MAPHIEU_NHAP_GANNHAT { get; set; } = "";
    public decimal? GIA_TRUOC_THUE { get; set; }
    public decimal? GIA_MOI_TINH { get; set; }
    public decimal? GIA_QD_MOI { get; set; }
    public decimal? CHENHLECH { get; set; }
    public string ID_CONGTHUCGIA { get; set; } = "";
    public string MA_CONGTHUC { get; set; } = "";
    public string TEN_CONGTHUC { get; set; } = "";
    public string CONGTHUC { get; set; } = "";
    public DateTime? CONGTHUC_TUNGAY { get; set; }
    public DateTime? CONGTHUC_DENNGAY { get; set; }
    public string TRANGTHAI { get; set; } = "";
    public bool SAN_SANG_AP_DUNG { get; set; }
}

public class PriceCalculationApplyRequest : PriceCalculationBatchRequest
{
    public List<string> MAHANG_APDUNG { get; set; } = new();
    public Dictionary<string, decimal> GIA_MOI_APDUNG { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, decimal> GIA_QD_MOI_APDUNG { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public class PriceCalculationRestoreRequest
{
    public string LOC_ID { get; set; } = "";
    public List<string> HISTORY_IDS { get; set; } = new();
}

public class PriceCalculationHistory
{
    public bool? DA_KHOIPHUC { get; set; }
    public DateTime? THOIGIAN_KHOIPHUC { get; set; }
    public string? ID_NGUOI_KHOIPHUC { get; set; }
    public string LOC_ID { get; set; } = "";
    public string ID { get; set; } = "";
    public string ID_HANGHOA { get; set; } = "";
    public string MAHANG { get; set; } = "";
    public string TENHANG { get; set; } = "";
    public string LOAIGIA { get; set; } = "";
    public decimal GIACU { get; set; }
    public decimal? TYLE_QD { get; set; }
    public decimal? GIA_QD_CU { get; set; }
    public decimal GIAMUA_GANNHAT { get; set; }
    public DateTime? NGAYNHAP_GANNHAT { get; set; }
    public string NGUON_GIA_NHAP { get; set; } = "PHIEUNHAP";
    public decimal GIA_MOI { get; set; }
    public decimal? GIA_QD_MOI { get; set; }
    public string ID_CONGTHUCGIA { get; set; } = "";
    public string MA_CONGTHUC { get; set; } = "";
    public string ID_NGUOITAO { get; set; } = "";
    public DateTime THOIGIANTHEM { get; set; }
}

public class PriceHistoryRequest
{
    public string LOC_ID { get; set; } = "";
    public DateTime FROM { get; set; }
    public DateTime TO { get; set; }
    public string? LOAIGIA { get; set; }
    public string? SEARCH { get; set; }
    public string? FORMULA { get; set; }
    public string? USER { get; set; }
    public string? STATUS { get; set; }
    public int PAGE { get; set; } = 1;
    public bool EXPORT { get; set; }
}
