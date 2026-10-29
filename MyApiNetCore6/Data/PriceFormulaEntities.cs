using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiNetCore6.Data;

[Table("dm_CongThucGia")]
public class PriceFormulaRule
{
    [Required, StringLength(50)] public string LOC_ID { get; set; } = "";
    [Required, StringLength(50)] public string ID { get; set; } = "";
    [Required, StringLength(50)] public string MA { get; set; } = "";
    [Required, StringLength(200)] public string NAME { get; set; } = "";
    [Required, StringLength(20)] public string LOAIGIA { get; set; } = "GIA01";
    [Required, StringLength(1000)] public string CONGTHUC { get; set; } = "";
    public bool TINHTHUE { get; set; }
    public DateTime TUNGAY { get; set; }
    public DateTime? DENNGAY { get; set; }
    public bool ISACTIVE { get; set; } = true;
    public DateTime? THOIGIANSUA { get; set; }
    [StringLength(50)] public string? ID_NGUOISUA { get; set; }
    public DateTime THOIGIANTHEM { get; set; }
    [StringLength(50)] public string? ID_NGUOITAO { get; set; }
    public List<PriceFormulaComponent> Components { get; set; } = new();
    public List<PriceFormulaGroupLink> Groups { get; set; } = new();
}

[Table("dm_CongThucGia_NhomHangHoa")]
public class PriceFormulaGroupLink
{
    [Required, StringLength(50)] public string LOC_ID { get; set; } = "";
    [Required, StringLength(50)] public string ID_CONGTHUCGIA { get; set; } = "";
    [Required, StringLength(50)] public string ID_NHOMHANGHOA { get; set; } = "";
}

[Table("dm_CongThucGia_ThanhPhan")]
public class PriceFormulaComponent
{
    [Required, StringLength(50)] public string LOC_ID { get; set; } = "";
    [Required, StringLength(50)] public string ID { get; set; } = "";
    [Required, StringLength(50)] public string ID_CONGTHUCGIA { get; set; } = "";
    [Required, StringLength(40)] public string MA { get; set; } = "";
    [Required, StringLength(100)] public string TEN { get; set; } = "";
    [Required, StringLength(20)] public string KIEUGIATRI { get; set; } = "TIEN";
    [Column(TypeName = "decimal(18,4)")] public decimal GIATRI_MACDINH { get; set; }
    public int STT { get; set; }
}

public class PriceFormulaRequest
{
    public string LOC_ID { get; set; } = "";
    public string? ID { get; set; }
    public string MA { get; set; } = "";
    public string NAME { get; set; } = "";
    public string? ID_NHOMHANGHOA { get; set; }
    public List<string> ID_NHOMHANGHOAS { get; set; } = new();
    public string LOAIGIA { get; set; } = "GIA01";
    public string CONGTHUC { get; set; } = "";
    public bool TINHTHUE { get; set; }
    public DateTime TUNGAY { get; set; }
    public DateTime? DENNGAY { get; set; }
    public bool ISACTIVE { get; set; } = true;
    public List<PriceFormulaComponentRequest> Components { get; set; } = new();
}

public class PriceFormulaComponentRequest
{
    public string MA { get; set; } = "";
    public string TEN { get; set; } = "";
    public string KIEUGIATRI { get; set; } = "TIEN";
    public decimal GIATRI_MACDINH { get; set; }
    public int STT { get; set; }
}

public class PriceFormulaCalculateRequest
{
    public string CONGTHUC { get; set; } = "";
    public Dictionary<string, decimal> GIA_TRI { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public bool TINHTHUE { get; set; }
    public decimal THUE_SUAT { get; set; }
}

public class PriceFormulaCalculateResponse
{
    public decimal GIA_TRUOC_THUE { get; set; }
    public decimal GIA_SAU_THUE { get; set; }
}

public class PriceFormulaGroupCalculateRequest
{
    public string LOC_ID { get; set; } = "";
    public string ID_NHOMHANGHOA { get; set; } = "";
    public string LOAIGIA { get; set; } = "GIA01";
    public DateTime NGAYTINH { get; set; } = DateTime.Today;
    public Dictionary<string, decimal> GIA_TRI { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public decimal THUE_SUAT { get; set; }
}
