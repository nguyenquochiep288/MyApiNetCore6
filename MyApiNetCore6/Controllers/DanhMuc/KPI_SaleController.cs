// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.KPI_SaleController
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.StoredProcedure.Parameter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using MyApiNetCore6.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;

#nullable enable
namespace MyApiNetCore6.Controllers;

[Route("api/[controller]")]
[ApiController]
public class KPI_SaleController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public KPI_SaleController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetProduct(string LOC_ID)
  {
    try
    {
      List<view_dm_KPI_KinhDoanh> lstValue = await this._context.view_dm_KPI_KinhDoanh.Where<view_dm_KPI_KinhDoanh>((Expression<Func<view_dm_KPI_KinhDoanh, bool>>) (e => e.LOC_ID == LOC_ID)).OrderBy<view_dm_KPI_KinhDoanh, string>((Expression<Func<view_dm_KPI_KinhDoanh, string>>) (e => e.MA)).ToListAsync<view_dm_KPI_KinhDoanh>();
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) lstValue
      });
    }
    catch (Exception ex)
    {
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpGet("{LOC_ID}/{Type}/{KeyWhere}/{ValuesSearch}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetProduct(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<view_dm_KPI_KinhDoanh> lstValue = await this._context.view_dm_KPI_KinhDoanh.Where<view_dm_KPI_KinhDoanh>((Expression<Func<view_dm_KPI_KinhDoanh, bool>>) (e => e.LOC_ID == LOC_ID)).Where<view_dm_KPI_KinhDoanh>(KeyWhere, (object) ValuesSearch).OrderBy<view_dm_KPI_KinhDoanh, string>((Expression<Func<view_dm_KPI_KinhDoanh, string>>) (e => e.MA)).ToListAsync<view_dm_KPI_KinhDoanh>();
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) lstValue
      });
    }
    catch (Exception ex)
    {
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpGet("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetProduct(string LOC_ID, string ID)
  {
    try
    {
      view_dm_KPI_KinhDoanh Product = await this._context.view_dm_KPI_KinhDoanh.FirstOrDefaultAsync<view_dm_KPI_KinhDoanh>((Expression<Func<view_dm_KPI_KinhDoanh, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Product == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      v_view_dm_KPI_KinhDoanh dm_KPI_KinhDoanh = new v_view_dm_KPI_KinhDoanh();
      dm_KPI_KinhDoanh.lstdm_KPI_KinhDoanh_YeuCau = new List<v_dm_KPI_KinhDoanh_YeuCau>();
      if (Product != null)
      {
        dm_KPI_KinhDoanh = JsonConvert.DeserializeObject<v_view_dm_KPI_KinhDoanh>(JsonConvert.SerializeObject((object) Product)) ?? new v_view_dm_KPI_KinhDoanh();
        List<view_dm_KPI_KinhDoanh_YeuCau> lstValue_yeuCau = await this._context.view_dm_KPI_KinhDoanh_YeuCau.Where<view_dm_KPI_KinhDoanh_YeuCau>((Expression<Func<view_dm_KPI_KinhDoanh_YeuCau, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_KPI_KINHDOANH == Product.ID)).ToListAsync<view_dm_KPI_KinhDoanh_YeuCau>();
        dm_KPI_KinhDoanh.lstdm_KPI_KinhDoanh_YeuCau = JsonConvert.DeserializeObject<List<v_dm_KPI_KinhDoanh_YeuCau>>(JsonConvert.SerializeObject((object) lstValue_yeuCau));
        List<view_dm_KPI_KinhDoanh_NhanVien> lstValue_NhanVien = await this._context.view_dm_KPI_KinhDoanh_NhanVien.Where<view_dm_KPI_KinhDoanh_NhanVien>((Expression<Func<view_dm_KPI_KinhDoanh_NhanVien, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_KPI_KINHDOANH == Product.ID)).ToListAsync<view_dm_KPI_KinhDoanh_NhanVien>();
        dm_KPI_KinhDoanh.lstdm_KPI_KinhDoanh_NhanVien = JsonConvert.DeserializeObject<List<v_dm_KPI_KinhDoanh_NhanVien>>(JsonConvert.SerializeObject((object) lstValue_NhanVien));
        lstValue_yeuCau = (List<view_dm_KPI_KinhDoanh_YeuCau>) null;
        lstValue_NhanVien = (List<view_dm_KPI_KinhDoanh_NhanVien>) null;
      }
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) dm_KPI_KinhDoanh
      });
    }
    catch (Exception ex)
    {
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpPut("{LOC_ID}/{MA}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PutProduct(
    string LOC_ID,
    string MA,
    [FromBody] v_dm_KPI_KinhDoanh ChuongTrinhKhuyenMai)
  {
    try
    {
      if (this.ProductExists((dm_KPI_KinhDoanh) ChuongTrinhKhuyenMai))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{ChuongTrinhKhuyenMai.LOC_ID}-{ChuongTrinhKhuyenMai.MA} trong dữ liệu!",
          Data = (object) ""
        });
      if (LOC_ID != ChuongTrinhKhuyenMai.LOC_ID || ChuongTrinhKhuyenMai.MA != MA)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.ProductExistsID(ChuongTrinhKhuyenMai.LOC_ID, ChuongTrinhKhuyenMai.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {ChuongTrinhKhuyenMai.LOC_ID}-{ChuongTrinhKhuyenMai.ID} dữ liệu!",
          Data = (object) ""
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        List<dm_KPI_KinhDoanh_YeuCau> ChuongTrinhKhuyenMai_YeuCau = await this._context.dm_KPI_KinhDoanh_YeuCau.Where<dm_KPI_KinhDoanh_YeuCau>((Expression<Func<dm_KPI_KinhDoanh_YeuCau, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_KPI_KINHDOANH == ChuongTrinhKhuyenMai.ID)).ToListAsync<dm_KPI_KinhDoanh_YeuCau>();
        foreach (dm_KPI_KinhDoanh_YeuCau itm in ChuongTrinhKhuyenMai_YeuCau)
          this._context.dm_KPI_KinhDoanh_YeuCau.Remove(itm);
        foreach (v_dm_KPI_KinhDoanh_YeuCau itm in ChuongTrinhKhuyenMai.lstdm_KPI_KinhDoanh_YeuCau)
        {
          itm.ID_KPI_KINHDOANH = ChuongTrinhKhuyenMai.ID;
          this._context.dm_KPI_KinhDoanh_YeuCau.Add((dm_KPI_KinhDoanh_YeuCau) itm);
        }
        List<dm_KPI_KinhDoanh_NhanVien> ChuongTrinhKhuyenMai_NhanVien = await this._context.dm_KPI_KinhDoanh_NhanVien.Where<dm_KPI_KinhDoanh_NhanVien>((Expression<Func<dm_KPI_KinhDoanh_NhanVien, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_KPI_KINHDOANH == ChuongTrinhKhuyenMai.ID)).ToListAsync<dm_KPI_KinhDoanh_NhanVien>();
        foreach (dm_KPI_KinhDoanh_NhanVien itm in ChuongTrinhKhuyenMai_NhanVien)
          this._context.dm_KPI_KinhDoanh_NhanVien.Remove(itm);
        foreach (v_dm_KPI_KinhDoanh_NhanVien itm in ChuongTrinhKhuyenMai.lstdm_KPI_KinhDoanh_NhanVien)
        {
          itm.ID_KPI_KINHDOANH = ChuongTrinhKhuyenMai.ID;
          this._context.dm_KPI_KinhDoanh_NhanVien.Add((dm_KPI_KinhDoanh_NhanVien) itm);
        }
        this._context.Entry<v_dm_KPI_KinhDoanh>(ChuongTrinhKhuyenMai).State = EntityState.Modified;
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        ChuongTrinhKhuyenMai_YeuCau = (List<dm_KPI_KinhDoanh_YeuCau>) null;
        ChuongTrinhKhuyenMai_NhanVien = (List<dm_KPI_KinhDoanh_NhanVien>) null;
        auditLog = (AuditLogController) null;
        transaction.Commit();
        view_dm_KPI_KinhDoanh OKProduct = await this._context.view_dm_KPI_KinhDoanh.FirstOrDefaultAsync<view_dm_KPI_KinhDoanh>((Expression<Func<view_dm_KPI_KinhDoanh, bool>>) (e => e.LOC_ID == ChuongTrinhKhuyenMai.LOC_ID && e.ID == ChuongTrinhKhuyenMai.ID));
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) OKProduct
        });
      }
    }
    catch (DbUpdateConcurrencyException ex)
    {
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpPost]
  [Authorize(Roles = "User")]
  public async Task<ActionResult<v_dm_KPI_KinhDoanh>> PostProduct(
    [FromBody] v_dm_KPI_KinhDoanh ChuongTrinhKhuyenMai)
  {
    try
    {
      if (this.ProductExistsMA(ChuongTrinhKhuyenMai.LOC_ID, ChuongTrinhKhuyenMai.MA))
        return (ActionResult<v_dm_KPI_KinhDoanh>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{ChuongTrinhKhuyenMai.LOC_ID}-{ChuongTrinhKhuyenMai.MA} trong dữ liệu!",
          Data = (object) ""
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        foreach (v_dm_KPI_KinhDoanh_YeuCau itm in ChuongTrinhKhuyenMai.lstdm_KPI_KinhDoanh_YeuCau)
        {
          itm.ID_KPI_KINHDOANH = ChuongTrinhKhuyenMai.ID;
          this._context.dm_KPI_KinhDoanh_YeuCau.Add((dm_KPI_KinhDoanh_YeuCau) itm);
        }
        foreach (v_dm_KPI_KinhDoanh_NhanVien itm in ChuongTrinhKhuyenMai.lstdm_KPI_KinhDoanh_NhanVien)
        {
          itm.ID_KPI_KINHDOANH = ChuongTrinhKhuyenMai.ID;
          this._context.dm_KPI_KinhDoanh_NhanVien.Add((dm_KPI_KinhDoanh_NhanVien) itm);
        }
        this._context.dm_KPI_KinhDoanh.Add((dm_KPI_KinhDoanh) ChuongTrinhKhuyenMai);
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        auditLog = (AuditLogController) null;
        transaction.Commit();
        view_dm_KPI_KinhDoanh OKProduct = await this._context.view_dm_KPI_KinhDoanh.FirstOrDefaultAsync<view_dm_KPI_KinhDoanh>((Expression<Func<view_dm_KPI_KinhDoanh, bool>>) (e => e.LOC_ID == ChuongTrinhKhuyenMai.LOC_ID && e.ID == ChuongTrinhKhuyenMai.ID));
        return (ActionResult<v_dm_KPI_KinhDoanh>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) OKProduct
        });
      }
    }
    catch (Exception ex)
    {
      return (ActionResult<v_dm_KPI_KinhDoanh>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteProduct(string LOC_ID, string ID)
  {
    try
    {
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        dm_KPI_KinhDoanh ChuongTrinhKhuyenMai = await this._context.dm_KPI_KinhDoanh.FirstOrDefaultAsync<dm_KPI_KinhDoanh>((Expression<Func<dm_KPI_KinhDoanh, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
        if (ChuongTrinhKhuyenMai == null)
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
            Data = (object) ""
          });
        ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
        ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_KPI_KinhDoanh>(ChuongTrinhKhuyenMai, ChuongTrinhKhuyenMai.ID, ChuongTrinhKhuyenMai.NAME);
        if (!apiResponse.Success)
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = apiResponse.Message,
            Data = (object) ""
          });
        List<dm_KPI_KinhDoanh_YeuCau> ChuongTrinhKhuyenMai_YeuCau = await this._context.dm_KPI_KinhDoanh_YeuCau.Where<dm_KPI_KinhDoanh_YeuCau>((Expression<Func<dm_KPI_KinhDoanh_YeuCau, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_KPI_KINHDOANH == ID)).ToListAsync<dm_KPI_KinhDoanh_YeuCau>();
        foreach (dm_KPI_KinhDoanh_YeuCau itm in ChuongTrinhKhuyenMai_YeuCau)
          this._context.dm_KPI_KinhDoanh_YeuCau.Remove(itm);
        List<dm_KPI_KinhDoanh_NhanVien> ChuongTrinhKhuyenMai_NhanVien = await this._context.dm_KPI_KinhDoanh_NhanVien.Where<dm_KPI_KinhDoanh_NhanVien>((Expression<Func<dm_KPI_KinhDoanh_NhanVien, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_KPI_KINHDOANH == ID)).ToListAsync<dm_KPI_KinhDoanh_NhanVien>();
        foreach (dm_KPI_KinhDoanh_NhanVien itm in ChuongTrinhKhuyenMai_NhanVien)
          this._context.dm_KPI_KinhDoanh_NhanVien.Remove(itm);
        this._context.dm_KPI_KinhDoanh.Remove(ChuongTrinhKhuyenMai);
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        ChuongTrinhKhuyenMai = (dm_KPI_KinhDoanh) null;
        ExecuteStoredProc = (ExecuteStoredProc) null;
        apiResponse = (ApiResponse) null;
        ChuongTrinhKhuyenMai_YeuCau = (List<dm_KPI_KinhDoanh_YeuCau>) null;
        ChuongTrinhKhuyenMai_NhanVien = (List<dm_KPI_KinhDoanh_NhanVien>) null;
        auditLog = (AuditLogController) null;
        transaction.Commit();
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) ""
        });
      }
    }
    catch (Exception ex)
    {
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  private bool ProductExistsMA(string LOC_ID, string MA)
  {
    return this._context.dm_KPI_KinhDoanh.Any<dm_KPI_KinhDoanh>((Expression<Func<dm_KPI_KinhDoanh, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == MA));
  }

  private bool ProductExistsID(string LOC_ID, string ID)
  {
    return this._context.dm_KPI_KinhDoanh.Any<dm_KPI_KinhDoanh>((Expression<Func<dm_KPI_KinhDoanh, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }

  private bool ProductExists(dm_KPI_KinhDoanh Product)
  {
    return this._context.dm_KPI_KinhDoanh.Any<dm_KPI_KinhDoanh>((Expression<Func<dm_KPI_KinhDoanh, bool>>) (e => e.LOC_ID == Product.LOC_ID && e.MA == Product.MA && e.ID != Product.ID));
  }

  [HttpPut]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PutProduct([FromBody] SP_Parameter SP_Parameter)
  {
    try
    {
      List<DanhSachPhieuDatHang_ChiTiet_KPI> lst_ChiTiet = new List<DanhSachPhieuDatHang_ChiTiet_KPI>();
      List<DanhSachPhieuTraHang_ChiTiet_KPI> lst_ChiTiet_TraHang = new List<DanhSachPhieuTraHang_ChiTiet_KPI>();
      List<v_Tinh_KPI_KinhDoanh> lstTinh_KPI_KinhDoanh = new List<v_Tinh_KPI_KinhDoanh>();
      List<view_dm_KPI_KinhDoanh> lstValue = await this._context.view_dm_KPI_KinhDoanh.Where<view_dm_KPI_KinhDoanh>((Expression<Func<view_dm_KPI_KinhDoanh, bool>>) (e => e.LOC_ID == SP_Parameter.LOC_ID && (DateTime?) e.TUNGAY <= SP_Parameter.TUNGAY && (DateTime?) e.DENNGAY >= SP_Parameter.DENNGAY && e.ISACTIVE)).OrderBy<view_dm_KPI_KinhDoanh, int>((Expression<Func<view_dm_KPI_KinhDoanh, int>>) (e => e.CAPDO)).ToListAsync<view_dm_KPI_KinhDoanh>();
      List<view_dm_HangHoa_KhungGia_HangHoa> lstValueKhungGia = await this._context.view_dm_HangHoa_KhungGia_HangHoa.Where<view_dm_HangHoa_KhungGia_HangHoa>((Expression<Func<view_dm_HangHoa_KhungGia_HangHoa, bool>>) (e => e.LOC_ID == SP_Parameter.LOC_ID && e.ISACTIVE)).ToListAsync<view_dm_HangHoa_KhungGia_HangHoa>();
      if (lstValue != null || lstValueKhungGia != null && lstValueKhungGia.Count > 0)
      {
        ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
        IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuTraHang_ChiTiet_KPI(SP_Parameter);
        if (actionResult is OkObjectResult okResult2)
        {
          ApiResponse ApiResponse = okResult2.Value as ApiResponse;
          if (ApiResponse != null && ApiResponse.Data != null)
            lst_ChiTiet_TraHang = ApiResponse.Data as List<DanhSachPhieuTraHang_ChiTiet_KPI>;
          ApiResponse = (ApiResponse) null;
        }
        actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuDatHang_ChiTiet_KPI(SP_Parameter);
        if (actionResult is OkObjectResult okResult2)
        {
          ApiResponse ApiResponse = okResult2.Value as ApiResponse;
          if (ApiResponse != null && ApiResponse.Data != null)
          {
            lst_ChiTiet = ApiResponse.Data as List<DanhSachPhieuDatHang_ChiTiet_KPI>;
            if (lst_ChiTiet != null && lst_ChiTiet_TraHang != null)
            {
              if (lstValue != null)
              {
                foreach (view_dm_KPI_KinhDoanh viewDmKpiKinhDoanh in lstValue)
                {
                  view_dm_KPI_KinhDoanh itm = viewDmKpiKinhDoanh;
                  List<view_dm_KPI_KinhDoanh_NhanVien> lstValue_NhanVien = await this._context.view_dm_KPI_KinhDoanh_NhanVien.Where<view_dm_KPI_KinhDoanh_NhanVien>((Expression<Func<view_dm_KPI_KinhDoanh_NhanVien, bool>>) (e => e.ID_KPI_KINHDOANH == itm.ID)).ToListAsync<view_dm_KPI_KinhDoanh_NhanVien>();
                  List<view_dm_KPI_KinhDoanh_YeuCau> lstValue_YeuCau = await this._context.view_dm_KPI_KinhDoanh_YeuCau.Where<view_dm_KPI_KinhDoanh_YeuCau>((Expression<Func<view_dm_KPI_KinhDoanh_YeuCau, bool>>) (e => e.ID_KPI_KINHDOANH == itm.ID)).ToListAsync<view_dm_KPI_KinhDoanh_YeuCau>();
                  foreach (view_dm_KPI_KinhDoanh_NhanVien kinhDoanhNhanVien in lstValue_NhanVien)
                  {
                    view_dm_KPI_KinhDoanh_NhanVien NhanVien = kinhDoanhNhanVien;
                    if (NhanVien.HINHTHUC == 0 && (string.IsNullOrEmpty(SP_Parameter.ID_NHANVIEN) || NhanVien.ID_NHANVIEN == SP_Parameter.ID_NHANVIEN))
                    {
                      if (!string.IsNullOrEmpty(SP_Parameter.ID_NHOMQUYEN))
                      {
                        view_dm_NhanVien objNhanVien = await this._context.view_dm_NhanVien.Where<view_dm_NhanVien>((Expression<Func<view_dm_NhanVien, bool>>) (e => e.ID == NhanVien.ID_NHANVIEN)).FirstOrDefaultAsync<view_dm_NhanVien>();
                        if (objNhanVien == null || !(objNhanVien.ID_NHOMQUYEN != SP_Parameter.ID_NHOMQUYEN))
                          objNhanVien = (view_dm_NhanVien) null;
                        else
                          continue;
                      }
                      v_Tinh_KPI_KinhDoanh NhanVienKinhDoanh = lstTinh_KPI_KinhDoanh.FirstOrDefault<v_Tinh_KPI_KinhDoanh>((Func<v_Tinh_KPI_KinhDoanh, bool>) (e => e.ID_NHANVIEN == NhanVien.ID_NHANVIEN));
                      if (NhanVienKinhDoanh == null)
                      {
                        NhanVienKinhDoanh = new v_Tinh_KPI_KinhDoanh();
                        NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet = new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
                        NhanVienKinhDoanh.ID_NHANVIEN = NhanVien.ID_NHANVIEN;
                        NhanVienKinhDoanh.NAME_NHANVIEN = $"{NhanVien.MA} - {NhanVien.NAME}";
                        dm_NhanVien objNhanVien = await this._context.dm_NhanVien.FirstOrDefaultAsync<dm_NhanVien>((Expression<Func<dm_NhanVien, bool>>) (e => e.ID == NhanVien.ID_NHANVIEN));
                        NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange((IEnumerable<v_Tinh_KPI_KinhDoanh_ChiTiet>) this.Get_ChiTiet(itm, NhanVien, lstValue_YeuCau, lst_ChiTiet, NhanVienKinhDoanh, lst_ChiTiet_TraHang));
                        NhanVienKinhDoanh.SOTIEN_KPI = NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Sum<v_Tinh_KPI_KinhDoanh_ChiTiet>((Func<v_Tinh_KPI_KinhDoanh_ChiTiet, double>) (e => e.SOTIEN_KPI));
                        if (NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet != null && NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Count > 0)
                          lstTinh_KPI_KinhDoanh.Add(NhanVienKinhDoanh);
                        objNhanVien = (dm_NhanVien) null;
                      }
                      else
                      {
                        if (NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.FirstOrDefault<v_Tinh_KPI_KinhDoanh_ChiTiet>((Func<v_Tinh_KPI_KinhDoanh_ChiTiet, bool>) (e => e.ID_KPI_KINHDOANH == itm.ID)) == null)
                          NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange((IEnumerable<v_Tinh_KPI_KinhDoanh_ChiTiet>) this.Get_ChiTiet(itm, NhanVien, lstValue_YeuCau, lst_ChiTiet, NhanVienKinhDoanh, lst_ChiTiet_TraHang));
                        NhanVienKinhDoanh.SOTIEN_KPI = NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Sum<v_Tinh_KPI_KinhDoanh_ChiTiet>((Func<v_Tinh_KPI_KinhDoanh_ChiTiet, double>) (e => e.SOTIEN_KPI));
                      }
                      NhanVienKinhDoanh = (v_Tinh_KPI_KinhDoanh) null;
                    }
                    if (NhanVien.HINHTHUC == 1 && (string.IsNullOrEmpty(SP_Parameter.ID_NHOMQUYEN) || NhanVien.ID_NHANVIEN == SP_Parameter.ID_NHOMQUYEN))
                    {
                      List<view_dm_NhanVien> lstNhanVien = await this._context.view_dm_NhanVien.Where<view_dm_NhanVien>((Expression<Func<view_dm_NhanVien, bool>>) (e => e.ID_NHOMQUYEN == NhanVien.ID_NHANVIEN && e.ISACTIVE)).ToListAsync<view_dm_NhanVien>();
                      if (lstNhanVien != null)
                      {
                        foreach (view_dm_NhanVien viewDmNhanVien in lstNhanVien)
                        {
                          view_dm_NhanVien item = viewDmNhanVien;
                          if (item != null)
                          {
                            if (string.IsNullOrEmpty(SP_Parameter.ID_NHOMQUYEN) || item == null || !(item.ID_NHOMQUYEN != SP_Parameter.ID_NHOMQUYEN))
                            {
                              v_Tinh_KPI_KinhDoanh NhanVienKinhDoanh = lstTinh_KPI_KinhDoanh.FirstOrDefault<v_Tinh_KPI_KinhDoanh>((Func<v_Tinh_KPI_KinhDoanh, bool>) (e => e.ID_NHANVIEN == item.ID));
                              if (NhanVienKinhDoanh == null)
                              {
                                NhanVienKinhDoanh = new v_Tinh_KPI_KinhDoanh();
                                NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet = new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
                                NhanVienKinhDoanh.ID_NHANVIEN = item.ID;
                                NhanVienKinhDoanh.NAME_NHANVIEN = $"{item.MA} - {item.NAME}";
                                dm_NhanVien objNhanVien = await this._context.dm_NhanVien.FirstOrDefaultAsync<dm_NhanVien>((Expression<Func<dm_NhanVien, bool>>) (e => e.ID == item.ID));
                                NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange((IEnumerable<v_Tinh_KPI_KinhDoanh_ChiTiet>) this.Get_ChiTiet(itm, NhanVien, lstValue_YeuCau, lst_ChiTiet, NhanVienKinhDoanh, lst_ChiTiet_TraHang));
                                NhanVienKinhDoanh.SOTIEN_KPI = NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Sum<v_Tinh_KPI_KinhDoanh_ChiTiet>((Func<v_Tinh_KPI_KinhDoanh_ChiTiet, double>) (e => e.SOTIEN_KPI));
                                if (NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet != null && NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Count > 0)
                                  lstTinh_KPI_KinhDoanh.Add(NhanVienKinhDoanh);
                                objNhanVien = (dm_NhanVien) null;
                              }
                              else if (NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.FirstOrDefault<v_Tinh_KPI_KinhDoanh_ChiTiet>((Func<v_Tinh_KPI_KinhDoanh_ChiTiet, bool>) (e => e.ID_KPI_KINHDOANH == itm.ID)) == null)
                              {
                                NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange((IEnumerable<v_Tinh_KPI_KinhDoanh_ChiTiet>) this.Get_ChiTiet(itm, NhanVien, lstValue_YeuCau, lst_ChiTiet, NhanVienKinhDoanh, lst_ChiTiet_TraHang));
                                NhanVienKinhDoanh.SOTIEN_KPI = NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Sum<v_Tinh_KPI_KinhDoanh_ChiTiet>((Func<v_Tinh_KPI_KinhDoanh_ChiTiet, double>) (e => e.SOTIEN_KPI));
                              }
                              NhanVienKinhDoanh = (v_Tinh_KPI_KinhDoanh) null;
                            }
                            else
                              continue;
                          }
                        }
                      }
                      lstNhanVien = (List<view_dm_NhanVien>) null;
                    }
                  }
                  lstValue_NhanVien = (List<view_dm_KPI_KinhDoanh_NhanVien>) null;
                  lstValue_YeuCau = (List<view_dm_KPI_KinhDoanh_YeuCau>) null;
                }
              }
              if (lstValueKhungGia != null && lstValueKhungGia.Count > 0)
              {
                lst_ChiTiet = lst_ChiTiet.Where<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, bool>) (s => lstValueKhungGia.Select<view_dm_HangHoa_KhungGia_HangHoa, string>((Func<view_dm_HangHoa_KhungGia_HangHoa, string>) (e => e.ID_HANGHOA)).Contains<string>(s.ID_HANGHOA))).ToList<DanhSachPhieuDatHang_ChiTiet_KPI>();
                lst_ChiTiet_TraHang = lst_ChiTiet_TraHang.Where<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, bool>) (s => lstValueKhungGia.Select<view_dm_HangHoa_KhungGia_HangHoa, string>((Func<view_dm_HangHoa_KhungGia_HangHoa, string>) (e => e.ID_HANGHOA)).Contains<string>(s.ID_HANGHOA))).ToList<DanhSachPhieuTraHang_ChiTiet_KPI>();
                if (lst_ChiTiet != null && lst_ChiTiet.Count > 0 && lst_ChiTiet_TraHang != null)
                {
                  List<\u003C\u003Ef__AnonymousType0<string>> lstMater = lstValueKhungGia.GroupBy(e => new
                  {
                    ID_HANGHOA_KHUNGGIA_MASTER = e.ID_HANGHOA_KHUNGGIA_MASTER
                  }).Select(g => new
                  {
                    ID_HANGHOA_KHUNGGIA_MASTER = g.Key.ID_HANGHOA_KHUNGGIA_MASTER
                  }).ToList();
                  List<\u003C\u003Ef__AnonymousType1<string>> lstTaiKhoan = lst_ChiTiet.GroupBy(e => new
                  {
                    ID_TAIKHOAN = e.ID_TAIKHOAN
                  }).Select(g => new
                  {
                    ID_TAIKHOAN = g.Key.ID_TAIKHOAN
                  }).ToList();
                  foreach (var data1 in lstMater)
                  {
                    var item = data1;
                    dm_HangHoa_KhungGia_Master view_dm_HangHoa_KhungGia_Master = await this._context.dm_HangHoa_KhungGia_Master.FirstOrDefaultAsync<dm_HangHoa_KhungGia_Master>((Expression<Func<dm_HangHoa_KhungGia_Master, bool>>) (e => e.ID == item.ID_HANGHOA_KHUNGGIA_MASTER));
                    if (view_dm_HangHoa_KhungGia_Master != null)
                    {
                      List<view_dm_HangHoa_KhungGia_HangHoa> lstHangHoa = await this._context.view_dm_HangHoa_KhungGia_HangHoa.Where<view_dm_HangHoa_KhungGia_HangHoa>((Expression<Func<view_dm_HangHoa_KhungGia_HangHoa, bool>>) (e => e.ID_HANGHOA_KHUNGGIA_MASTER == item.ID_HANGHOA_KHUNGGIA_MASTER)).ToListAsync<view_dm_HangHoa_KhungGia_HangHoa>();
                      if (lstHangHoa != null)
                      {
                        foreach (var data2 in lstTaiKhoan)
                        {
                          var tk = data2;
                          List<DanhSachPhieuDatHang_ChiTiet_KPI> lstChiTiet = lst_ChiTiet.Where<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, bool>) (s => lstHangHoa.Select<view_dm_HangHoa_KhungGia_HangHoa, string>((Func<view_dm_HangHoa_KhungGia_HangHoa, string>) (e => e.ID_HANGHOA)).Contains<string>(s.ID_HANGHOA) && s.ID_TAIKHOAN == tk.ID_TAIKHOAN)).ToList<DanhSachPhieuDatHang_ChiTiet_KPI>();
                          List<DanhSachPhieuTraHang_ChiTiet_KPI> lstChiTiet_TraHang = lst_ChiTiet_TraHang.Where<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, bool>) (s => lstHangHoa.Select<view_dm_HangHoa_KhungGia_HangHoa, string>((Func<view_dm_HangHoa_KhungGia_HangHoa, string>) (e => e.ID_HANGHOA)).Contains<string>(s.ID_HANGHOA) && s.ID_TAIKHOAN == tk.ID_TAIKHOAN)).ToList<DanhSachPhieuTraHang_ChiTiet_KPI>();
                          dm_NhanVien NhaVien = await this._context.dm_NhanVien.FirstOrDefaultAsync<dm_NhanVien>((Expression<Func<dm_NhanVien, bool>>) (e => e.ID_TAIKHOAN == tk.ID_TAIKHOAN));
                          if (NhaVien != null)
                          {
                            List<view_dm_HangHoa_KhungGia> lstview_dm_HangHoa_KhungGia = await this._context.view_dm_HangHoa_KhungGia.Where<view_dm_HangHoa_KhungGia>((Expression<Func<view_dm_HangHoa_KhungGia, bool>>) (e => e.ID_HANGHOA_KHUNGGIA_MASTER == item.ID_HANGHOA_KHUNGGIA_MASTER)).ToListAsync<view_dm_HangHoa_KhungGia>();
                            v_Tinh_KPI_KinhDoanh NhanVienKinhDoanh = lstTinh_KPI_KinhDoanh.FirstOrDefault<v_Tinh_KPI_KinhDoanh>((Func<v_Tinh_KPI_KinhDoanh, bool>) (e => e.ID_NHANVIEN == NhaVien.ID));
                            if (NhanVienKinhDoanh == null)
                            {
                              NhanVienKinhDoanh = new v_Tinh_KPI_KinhDoanh();
                              NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet = new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
                              NhanVienKinhDoanh.ID_NHANVIEN = tk.ID_TAIKHOAN;
                              NhanVienKinhDoanh.NAME_NHANVIEN = $"{NhaVien.MA} - {NhaVien.NAME}";
                              NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange((IEnumerable<v_Tinh_KPI_KinhDoanh_ChiTiet>) this.Get_ChiTiet_KhungGia(view_dm_HangHoa_KhungGia_Master, lstChiTiet, lst_ChiTiet_TraHang, lstview_dm_HangHoa_KhungGia));
                              NhanVienKinhDoanh.SOTIEN_KPI = NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Sum<v_Tinh_KPI_KinhDoanh_ChiTiet>((Func<v_Tinh_KPI_KinhDoanh_ChiTiet, double>) (e => e.SOTIEN_KPI));
                              if (NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet != null && NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Count > 0)
                                lstTinh_KPI_KinhDoanh.Add(NhanVienKinhDoanh);
                            }
                            else
                            {
                              NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange((IEnumerable<v_Tinh_KPI_KinhDoanh_ChiTiet>) this.Get_ChiTiet_KhungGia(view_dm_HangHoa_KhungGia_Master, lstChiTiet, lst_ChiTiet_TraHang, lstview_dm_HangHoa_KhungGia));
                              NhanVienKinhDoanh.SOTIEN_KPI = NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Sum<v_Tinh_KPI_KinhDoanh_ChiTiet>((Func<v_Tinh_KPI_KinhDoanh_ChiTiet, double>) (e => e.SOTIEN_KPI));
                            }
                            lstChiTiet = (List<DanhSachPhieuDatHang_ChiTiet_KPI>) null;
                            lstChiTiet_TraHang = (List<DanhSachPhieuTraHang_ChiTiet_KPI>) null;
                            lstview_dm_HangHoa_KhungGia = (List<view_dm_HangHoa_KhungGia>) null;
                            NhanVienKinhDoanh = (v_Tinh_KPI_KinhDoanh) null;
                          }
                        }
                      }
                      view_dm_HangHoa_KhungGia_Master = (dm_HangHoa_KhungGia_Master) null;
                    }
                  }
                  lstMater = null;
                  lstTaiKhoan = null;
                }
              }
            }
          }
          ApiResponse = (ApiResponse) null;
        }
        ExecuteStoredProc1 = (ExecuteStoredProc) null;
        actionResult = (IActionResult) null;
        okResult2 = (OkObjectResult) null;
      }
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) lstTinh_KPI_KinhDoanh
      });
    }
    catch (DbUpdateConcurrencyException ex)
    {
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  private List<v_Tinh_KPI_KinhDoanh_ChiTiet> Get_ChiTiet(
    view_dm_KPI_KinhDoanh KPI_KinhDoanh,
    view_dm_KPI_KinhDoanh_NhanVien KPI_KinhDoanh_NhanVien,
    List<view_dm_KPI_KinhDoanh_YeuCau> lstValue_YeuCau,
    List<DanhSachPhieuDatHang_ChiTiet_KPI> lstChiTietDatHang,
    v_Tinh_KPI_KinhDoanh NhanVien_KinhDoanh,
    List<DanhSachPhieuTraHang_ChiTiet_KPI> lstChiTietTraHang)
  {
    try
    {
      List<v_Tinh_KPI_KinhDoanh_ChiTiet> chiTiet = new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
      bool flag = true;
      double num1 = 0.0;
      double num2 = 0.0;
      double num3 = 0.0;
      foreach (view_dm_KPI_KinhDoanh_YeuCau kpiKinhDoanhYeuCau in lstValue_YeuCau)
      {
        view_dm_KPI_KinhDoanh_YeuCau itm = kpiKinhDoanhYeuCau;
        double num4 = lstChiTietDatHang.Where<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, bool>) (e =>
        {
          if (e.bolDaTinhKpi || (itm.HINHTHUC == 0 ? (e.ID_HANGHOA == itm.ID_HANGHOA ? 1 : 0) : (e.ID_NHOMHANGHOA == itm.ID_HANGHOA ? 1 : 0)) == 0)
            return false;
          return KPI_KinhDoanh_NhanVien.HINHTHUC != 0 ? e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN : e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN;
        })).Sum<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, double>) (e => e.TONGCONG));
        double num5 = lstChiTietTraHang.Where<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, bool>) (e =>
        {
          if (e.bolDaTinhKpi || (itm.HINHTHUC == 0 ? (e.ID_HANGHOA == itm.ID_HANGHOA ? 1 : 0) : (e.ID_NHOMHANGHOA == itm.ID_HANGHOA ? 1 : 0)) == 0)
            return false;
          return KPI_KinhDoanh_NhanVien.HINHTHUC != 0 ? e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN : e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN;
        })).Sum<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, double>) (e => e.TONGCONG));
        double num6 = lstChiTietDatHang.Where<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, bool>) (e =>
        {
          if (e.bolDaTinhKpi || (itm.HINHTHUC == 0 ? (!(e.ID_HANGHOA == itm.ID_HANGHOA) ? 0 : (e.ID_DVT == itm.ID_DVT ? 1 : 0)) : (!(e.ID_NHOMHANGHOA == itm.ID_HANGHOA) ? 0 : (e.ID_DVT == itm.ID_DVT ? 1 : 0))) == 0)
            return false;
          return KPI_KinhDoanh_NhanVien.HINHTHUC != 0 ? e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN : e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN;
        })).Sum<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, double>) (e => e.TONGSOLUONG / e.TYLE_QD)) + (double) Convert.ToInt32(lstChiTietDatHang.Where<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, bool>) (e =>
        {
          if (e.bolDaTinhKpi || (itm.HINHTHUC == 0 ? (!(e.ID_HANGHOA == itm.ID_HANGHOA) ? 0 : (e.ID_DVT != itm.ID_DVT ? 1 : 0)) : (!(e.ID_NHOMHANGHOA == itm.ID_HANGHOA) ? 0 : (e.ID_DVT != itm.ID_DVT ? 1 : 0))) == 0)
            return false;
          return KPI_KinhDoanh_NhanVien.HINHTHUC != 0 ? e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN : e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN;
        })).Sum<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, double>) (e => e.TONGSOLUONG / (e.TYLE_QD_HH == 0.0 ? 1.0 : e.TYLE_QD_HH))));
        double num7 = lstChiTietTraHang.Where<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, bool>) (e =>
        {
          if (e.bolDaTinhKpi || (itm.HINHTHUC == 0 ? (!(e.ID_HANGHOA == itm.ID_HANGHOA) ? 0 : (e.ID_DVT == itm.ID_DVT ? 1 : 0)) : (!(e.ID_NHOMHANGHOA == itm.ID_HANGHOA) ? 0 : (e.ID_DVT == itm.ID_DVT ? 1 : 0))) == 0)
            return false;
          return KPI_KinhDoanh_NhanVien.HINHTHUC != 0 ? e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN : e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN;
        })).Sum<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, double>) (e => e.TONGSOLUONG / e.TYLE_QD)) + (double) Convert.ToInt32(lstChiTietTraHang.Where<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, bool>) (e =>
        {
          if (e.bolDaTinhKpi || (itm.HINHTHUC == 0 ? (!(e.ID_HANGHOA == itm.ID_HANGHOA) ? 0 : (e.ID_DVT != itm.ID_DVT ? 1 : 0)) : (!(e.ID_NHOMHANGHOA == itm.ID_HANGHOA) ? 0 : (e.ID_DVT != itm.ID_DVT ? 1 : 0))) == 0)
            return false;
          return KPI_KinhDoanh_NhanVien.HINHTHUC != 0 ? e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN : e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN;
        })).Sum<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, double>) (e => e.TONGSOLUONG / (e.TYLE_QD_HH == 0.0 ? 1.0 : e.TYLE_QD_HH))));
        if ((itm.SOTIEN > 0.0 || itm.SOLUONG > 0.0 || itm.CHIETKHAU > 0.0 || itm.TIENGIAM > 0.0) && itm.SOTIEN <= num4 - num5 && itm.SOLUONG <= num6 - num7)
        {
          dm_DonViTinh dmDonViTinh = this._context.dm_DonViTinh.Where<dm_DonViTinh>((Expression<Func<dm_DonViTinh, bool>>) (e => e.ID == itm.ID_DVT)).FirstOrDefault<dm_DonViTinh>();
          v_Tinh_KPI_KinhDoanh_ChiTiet kinhDoanhChiTiet1 = new v_Tinh_KPI_KinhDoanh_ChiTiet();
          v_Tinh_KPI_KinhDoanh_ChiTiet kinhDoanhChiTiet2 = new v_Tinh_KPI_KinhDoanh_ChiTiet();
          kinhDoanhChiTiet2.HINHTHUC = itm.HINHTHUC;
          kinhDoanhChiTiet2.NAME_HINHTHUC = itm.NAME_HINHTHUC;
          kinhDoanhChiTiet2.ID_KPI_KINHDOANH = KPI_KinhDoanh.ID;
          kinhDoanhChiTiet2.NAME_KPI_KINHDOANH = KPI_KinhDoanh.NAME;
          kinhDoanhChiTiet2.ID_HANGHOA = itm.ID_HANGHOA;
          kinhDoanhChiTiet2.NAME_HANGHOA = itm.NAME;
          if (itm.CHIETKHAU > 0.0 || itm.TIENGIAM > 0.0)
          {
            kinhDoanhChiTiet2.TONGTIEN = num4;
            kinhDoanhChiTiet2.TONGTIEN_TRAHANG = num5;
            kinhDoanhChiTiet2.TONGTIEN_KPI = itm.SOTIEN;
          }
          int num8 = 0;
          double num9 = 1.0;
          num8 = Convert.ToInt32(num6) / Convert.ToInt32(num9);
          kinhDoanhChiTiet2.NAME_DVT = itm.NAME_DVT;
          kinhDoanhChiTiet2.NAME_DVT_QD = dmDonViTinh != null ? dmDonViTinh.NAME : "";
          kinhDoanhChiTiet2.TYLE_QD = num9;
          if (itm.HINHTHUC_TINHKPI == 1 && (itm.CHIETKHAU > 0.0 || itm.TIENGIAM > 0.0))
          {
            kinhDoanhChiTiet2.TONGSOLUONG = (double) (Convert.ToInt32(num6) / Convert.ToInt32(num9));
            kinhDoanhChiTiet2.TONGSOLUONG_TRAHANG = (double) (Convert.ToInt32(num7) / Convert.ToInt32(num9));
            kinhDoanhChiTiet2.TONGSOLUONG_KPI = $"{itm.SOLUONG.ToString("N0").Replace(',', '.')} {itm.NAME_DVT}";
          }
          if (itm.HINHTHUC_TINHKPI == 3 && (itm.CHIETKHAU > 0.0 || itm.TIENGIAM > 0.0))
          {
            kinhDoanhChiTiet2.TONGSOLUONG = (double) (Convert.ToInt32(num6) / Convert.ToInt32(num9));
            kinhDoanhChiTiet2.TONGSOLUONG_TRAHANG = (double) (Convert.ToInt32(num7) / Convert.ToInt32(num9));
            kinhDoanhChiTiet2.TONGSOLUONG_KPI = $"{itm.SOLUONG.ToString("N0").Replace(',', '.')} {itm.NAME_DVT}";
          }
          kinhDoanhChiTiet2.CHIETKHAU = itm.CHIETKHAU;
          kinhDoanhChiTiet2.TIENTHUONG = itm.TIENGIAM;
          int num10 = Convert.ToInt32(num6 - num7) / Convert.ToInt32(num9);
          kinhDoanhChiTiet2.SOTIEN_KPI = itm.HINHTHUC_TINHKPI != 1 ? (itm.HINHTHUC_TINHKPI != 3 ? (itm.HINHTHUC_TINHKPI != 2 ? (kinhDoanhChiTiet2.CHIETKHAU > 0.0 ? kinhDoanhChiTiet2.CHIETKHAU * (num4 - num5) / 100.0 : itm.TIENGIAM) : (kinhDoanhChiTiet2.CHIETKHAU > 0.0 ? kinhDoanhChiTiet2.CHIETKHAU * (num4 - num5 - itm.SOTIEN) / 100.0 : itm.TIENGIAM)) : (kinhDoanhChiTiet2.CHIETKHAU > 0.0 ? kinhDoanhChiTiet2.CHIETKHAU * (num4 - num5) / 100.0 : itm.TIENGIAM * (double) num10)) : (kinhDoanhChiTiet2.CHIETKHAU > 0.0 ? kinhDoanhChiTiet2.CHIETKHAU * (num4 - num5) / 100.0 : itm.TIENGIAM * ((double) num10 - itm.SOLUONG));
          chiTiet.Add(kinhDoanhChiTiet2);
          foreach (DanhSachPhieuDatHang_ChiTiet_KPI datHangChiTietKpi in lstChiTietDatHang.Where<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, bool>) (e =>
          {
            if (e.bolDaTinhKpi || (itm.HINHTHUC == 0 ? (e.ID_HANGHOA == itm.ID_HANGHOA ? 1 : 0) : (e.ID_NHOMHANGHOA == itm.ID_HANGHOA ? 1 : 0)) == 0)
              return false;
            return KPI_KinhDoanh_NhanVien.HINHTHUC != 0 ? e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN : e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN;
          })))
            ;
          foreach (DanhSachPhieuTraHang_ChiTiet_KPI traHangChiTietKpi in lstChiTietTraHang.Where<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, bool>) (e =>
          {
            if (e.bolDaTinhKpi || (itm.HINHTHUC == 0 ? (e.ID_HANGHOA == itm.ID_HANGHOA ? 1 : 0) : (e.ID_NHOMHANGHOA == itm.ID_HANGHOA ? 1 : 0)) == 0)
              return false;
            return KPI_KinhDoanh_NhanVien.HINHTHUC != 0 ? e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN : e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN;
          })))
            ;
          foreach (DanhSachPhieuDatHang_ChiTiet_KPI datHangChiTietKpi in lstChiTietDatHang.Where<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, bool>) (e =>
          {
            if (e.bolDaTinhKpi || (itm.HINHTHUC == 0 ? (!(e.ID_HANGHOA == itm.ID_HANGHOA) ? 0 : (e.ID_DVT == itm.ID_DVT ? 1 : 0)) : (!(e.ID_NHOMHANGHOA == itm.ID_HANGHOA) ? 0 : (e.ID_DVT == itm.ID_DVT ? 1 : 0))) == 0)
              return false;
            return KPI_KinhDoanh_NhanVien.HINHTHUC != 0 ? e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN : e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN;
          })))
            ;
          foreach (DanhSachPhieuTraHang_ChiTiet_KPI traHangChiTietKpi in lstChiTietTraHang.Where<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, bool>) (e =>
          {
            if (e.bolDaTinhKpi || (itm.HINHTHUC == 0 ? (!(e.ID_HANGHOA == itm.ID_HANGHOA) ? 0 : (e.ID_DVT == itm.ID_DVT ? 1 : 0)) : (!(e.ID_NHOMHANGHOA == itm.ID_HANGHOA) ? 0 : (e.ID_DVT == itm.ID_DVT ? 1 : 0))) == 0)
              return false;
            return KPI_KinhDoanh_NhanVien.HINHTHUC != 0 ? e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN : e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN;
          })))
            ;
          num1 += num4 - num5;
        }
        else
          flag = false;
        num2 += num4;
        num3 += num5;
      }
      double num11 = lstChiTietDatHang.Where<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, bool>) (e => e.ID_DVT == KPI_KinhDoanh.ID_DVT_DATKM)).Sum<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, double>) (e => e.TONGCONG));
      double num12 = lstChiTietTraHang.Where<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, bool>) (e => e.ID_DVT == KPI_KinhDoanh.ID_DVT_DATKM)).Sum<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, double>) (e => e.TONGCONG));
      if (KPI_KinhDoanh.IS_YEUCAUCHITIET)
      {
        if (flag && (KPI_KinhDoanh.CHIETKHAU > 0.0 || KPI_KinhDoanh.TIENGIAM > 0.0))
        {
          v_Tinh_KPI_KinhDoanh_ChiTiet kinhDoanhChiTiet = new v_Tinh_KPI_KinhDoanh_ChiTiet()
          {
            HINHTHUC = -1,
            NAME_HINHTHUC = "Tổng",
            ID_KPI_KINHDOANH = KPI_KinhDoanh.ID,
            NAME_KPI_KINHDOANH = KPI_KinhDoanh.NAME
          };
          kinhDoanhChiTiet.NAME_HANGHOA = kinhDoanhChiTiet.NAME_HINHTHUC;
          kinhDoanhChiTiet.TONGTIEN = num2;
          kinhDoanhChiTiet.TONGTIEN_TRAHANG = num3;
          kinhDoanhChiTiet.TONGTIEN_KPI = KPI_KinhDoanh.TONGTIEN_DATKM;
          if (!string.IsNullOrEmpty(KPI_KinhDoanh.NAME_DVT))
          {
            kinhDoanhChiTiet.TONGSOLUONG = num11;
            kinhDoanhChiTiet.NAME_DVT = KPI_KinhDoanh.NAME_DVT;
            kinhDoanhChiTiet.TONGSOLUONG_TRAHANG = num12;
            kinhDoanhChiTiet.TONGSOLUONG_KPI = $"{KPI_KinhDoanh.SOLUONG_DATKM.ToString()} {KPI_KinhDoanh.NAME_DVT}";
          }
          kinhDoanhChiTiet.CHIETKHAU = KPI_KinhDoanh.CHIETKHAU;
          kinhDoanhChiTiet.TIENTHUONG = KPI_KinhDoanh.TIENGIAM;
          kinhDoanhChiTiet.SOTIEN_KPI = kinhDoanhChiTiet.CHIETKHAU > 0.0 ? kinhDoanhChiTiet.CHIETKHAU * num1 / 100.0 : KPI_KinhDoanh.TIENGIAM;
          chiTiet.Add(kinhDoanhChiTiet);
        }
      }
      else if ((KPI_KinhDoanh.CHIETKHAU > 0.0 || KPI_KinhDoanh.TIENGIAM > 0.0) && KPI_KinhDoanh.SOLUONG_DATKM <= num11 && KPI_KinhDoanh.TONGTIEN_DATKM <= num2)
      {
        v_Tinh_KPI_KinhDoanh_ChiTiet kinhDoanhChiTiet = new v_Tinh_KPI_KinhDoanh_ChiTiet()
        {
          HINHTHUC = -1,
          NAME_HINHTHUC = "Tổng",
          ID_KPI_KINHDOANH = KPI_KinhDoanh.ID,
          NAME_KPI_KINHDOANH = KPI_KinhDoanh.NAME
        };
        kinhDoanhChiTiet.NAME_HANGHOA = kinhDoanhChiTiet.NAME_HINHTHUC;
        kinhDoanhChiTiet.TONGTIEN = num2;
        kinhDoanhChiTiet.TONGTIEN_TRAHANG = num3;
        kinhDoanhChiTiet.TONGTIEN_KPI = KPI_KinhDoanh.TONGTIEN_DATKM;
        if (!string.IsNullOrEmpty(KPI_KinhDoanh.NAME_DVT))
        {
          kinhDoanhChiTiet.TONGSOLUONG = num11;
          kinhDoanhChiTiet.NAME_DVT = KPI_KinhDoanh.NAME_DVT;
          kinhDoanhChiTiet.TONGSOLUONG_TRAHANG = num12;
          kinhDoanhChiTiet.TONGSOLUONG_KPI = $"{KPI_KinhDoanh.SOLUONG_DATKM.ToString()} {KPI_KinhDoanh.NAME_DVT}";
        }
        kinhDoanhChiTiet.CHIETKHAU = KPI_KinhDoanh.CHIETKHAU;
        kinhDoanhChiTiet.TIENTHUONG = KPI_KinhDoanh.TIENGIAM;
        kinhDoanhChiTiet.SOTIEN_KPI = kinhDoanhChiTiet.CHIETKHAU > 0.0 ? kinhDoanhChiTiet.CHIETKHAU * (kinhDoanhChiTiet.TONGTIEN - kinhDoanhChiTiet.TONGTIEN_TRAHANG) / 100.0 : KPI_KinhDoanh.TIENGIAM;
        chiTiet.Add(kinhDoanhChiTiet);
      }
      return chiTiet;
    }
    catch
    {
      return new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
    }
  }

  private List<v_Tinh_KPI_KinhDoanh_ChiTiet> Get_ChiTiet_KhungGia(
    dm_HangHoa_KhungGia_Master dm_HangHoa_KhungGia_Master,
    List<DanhSachPhieuDatHang_ChiTiet_KPI> lstChiTietDatHang,
    List<DanhSachPhieuTraHang_ChiTiet_KPI> lstChiTietTraHang,
    List<view_dm_HangHoa_KhungGia> lstKhungGia)
  {
    if (lstKhungGia == null || lstKhungGia.Count == 0)
      return new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
    List<v_Tinh_KPI_KinhDoanh_ChiTiet> chiTietKhungGia = new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
    foreach (view_dm_HangHoa_KhungGia dmHangHoaKhungGia in lstKhungGia)
    {
      view_dm_HangHoa_KhungGia khunggia = dmHangHoaKhungGia;
      v_Tinh_KPI_KinhDoanh_ChiTiet newv_Tinh_KPI_KinhDoanh_ChiTiet = new v_Tinh_KPI_KinhDoanh_ChiTiet();
      newv_Tinh_KPI_KinhDoanh_ChiTiet.HINHTHUC = -1;
      newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_HINHTHUC = dm_HangHoa_KhungGia_Master.NAME;
      newv_Tinh_KPI_KinhDoanh_ChiTiet.ID_KPI_KINHDOANH = dm_HangHoa_KhungGia_Master.ID;
      newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_KPI_KINHDOANH = dm_HangHoa_KhungGia_Master.NAME;
      newv_Tinh_KPI_KinhDoanh_ChiTiet.ID_HANGHOA = khunggia.ID;
      v_Tinh_KPI_KinhDoanh_ChiTiet kinhDoanhChiTiet1 = newv_Tinh_KPI_KinhDoanh_ChiTiet;
      string name = dm_HangHoa_KhungGia_Master.NAME;
      double num = khunggia.DONGIA;
      string str1 = num.ToString("N0");
      string str2 = $"{name} ({str1})";
      kinhDoanhChiTiet1.NAME_HANGHOA = str2;
      newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN = lstChiTietDatHang.Where<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, bool>) (s => s.ID_DVT == khunggia.ID_DVT && s.DONGIA == khunggia.DONGIA)).GroupBy<DanhSachPhieuDatHang_ChiTiet_KPI, string>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, string>) (s => s.ID_PHIEUDATHANG)).Select(g => new
      {
        ID_PHIEUDATHANG = g.Key,
        TongSoLuong = g.Sum<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, double>) (x => x.SOLUONG)),
        TongTien = g.Sum<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, double>) (x => x.TONGCONG))
      }).Where(s => s.TongSoLuong >= khunggia.TU && s.TongSoLuong <= khunggia.DEN).Sum(s => s.TongTien);
      newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN_TRAHANG = lstChiTietTraHang.Where<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, bool>) (s => s.ID_DVT == khunggia.ID_DVT && s.DONGIA == khunggia.DONGIA)).Sum<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, double>) (s => s.TONGCONG));
      newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN_KPI = 0.0;
      newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU = khunggia.CK_KPI;
      newv_Tinh_KPI_KinhDoanh_ChiTiet.TIENTHUONG = khunggia.TIEN_KPI;
      if (newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN != 0.0 || newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN_TRAHANG != 0.0 || newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU != 0.0 || newv_Tinh_KPI_KinhDoanh_ChiTiet.TIENTHUONG != 0.0)
      {
        if (!string.IsNullOrEmpty(khunggia.NAME_DVT))
        {
          newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG = lstChiTietDatHang.Where<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, bool>) (s => s.ID_DVT == khunggia.ID_DVT && s.DONGIA == khunggia.DONGIA)).GroupBy<DanhSachPhieuDatHang_ChiTiet_KPI, string>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, string>) (s => s.ID_PHIEUDATHANG)).Select(g => new
          {
            ID_PHIEUDATHANG = g.Key,
            TongSoLuong = g.Sum<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, double>) (x => x.SOLUONG)),
            TongTien = g.Sum<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, double>) (x => x.TONGCONG))
          }).Where(s => s.TongSoLuong >= khunggia.TU && s.TongSoLuong <= khunggia.DEN).Sum(s => s.TongSoLuong);
          newv_Tinh_KPI_KinhDoanh_ChiTiet.TYLE_QD = 1.0;
          newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_DVT = khunggia.NAME_DVT;
          newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG_TRAHANG = lstChiTietTraHang.Where<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, bool>) (s => s.ID_DVT == khunggia.ID_DVT && s.DONGIA == khunggia.DONGIA)).Sum<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, double>) (s => s.SOLUONG));
          v_Tinh_KPI_KinhDoanh_ChiTiet kinhDoanhChiTiet2 = newv_Tinh_KPI_KinhDoanh_ChiTiet;
          string[] strArray = new string[5];
          num = khunggia.TU;
          strArray[0] = num.ToString();
          strArray[1] = " - ";
          num = khunggia.DEN;
          strArray[2] = num.ToString();
          strArray[3] = " ";
          strArray[4] = khunggia.NAME_DVT;
          string str3 = string.Concat(strArray);
          kinhDoanhChiTiet2.TONGSOLUONG_KPI = str3;
        }
        if (khunggia.HINHTHUC_TINHKPI == 1)
        {
          IEnumerable<\u003C\u003Ef__AnonymousType2<string, double, double>> source = lstChiTietDatHang.Where<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, bool>) (s => s.ID_DVT == khunggia.ID_DVT && s.DONGIA == khunggia.DONGIA)).GroupBy<DanhSachPhieuDatHang_ChiTiet_KPI, string>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, string>) (e => e.ID_PHIEUDATHANG)).Select(g => new
          {
            ID_PHIEUDATHANG = g.Key,
            TongSoLuong = g.Sum<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, double>) (x => x.SOLUONG)),
            TongTien = g.Sum<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, double>) (x => x.TONGCONG))
          }).Where(s => s.TongSoLuong >= khunggia.TU && s.TongSoLuong <= khunggia.DEN);
          newv_Tinh_KPI_KinhDoanh_ChiTiet.SOTIEN_KPI = source.Sum(x => newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU <= 0.0 ? x.TongSoLuong * newv_Tinh_KPI_KinhDoanh_ChiTiet.TIENTHUONG : newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU * x.TongSoLuong / 100.0);
        }
        else
        {
          IEnumerable<\u003C\u003Ef__AnonymousType2<string, double, double>> source = lstChiTietDatHang.Where<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, bool>) (s => s.ID_DVT == khunggia.ID_DVT && s.DONGIA == khunggia.DONGIA)).GroupBy<DanhSachPhieuDatHang_ChiTiet_KPI, string>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, string>) (e => e.ID_PHIEUDATHANG)).Select(g => new
          {
            ID_PHIEUDATHANG = g.Key,
            TongSoLuong = g.Sum<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, double>) (x => x.SOLUONG)),
            TongTien = g.Sum<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, double>) (x => x.TONGCONG))
          }).Where(s => s.TongSoLuong >= khunggia.TU && s.TongSoLuong <= khunggia.DEN);
          newv_Tinh_KPI_KinhDoanh_ChiTiet.SOTIEN_KPI = source.Sum(x => newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU <= 0.0 ? newv_Tinh_KPI_KinhDoanh_ChiTiet.TIENTHUONG : newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU * x.TongSoLuong / 100.0);
        }
        chiTietKhungGia.Add(newv_Tinh_KPI_KinhDoanh_ChiTiet);
      }
    }
    return chiTietKhungGia;
  }

  [HttpPost("PostCreateKPI_Sale")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PostKPI_Sale([FromBody] List<Deposit> lstDeposit)
  {
    try
    {
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        foreach (Deposit deposit in lstDeposit)
        {
          Deposit item = deposit;
          dm_KPI_KinhDoanh KPI_KinhDoanh = this._context.dm_KPI_KinhDoanh.AsNoTracking<dm_KPI_KinhDoanh>().Where<dm_KPI_KinhDoanh>((Expression<Func<dm_KPI_KinhDoanh, bool>>) (e => e.LOC_ID == item.LOC_ID && e.ID == item.ID)).FirstOrDefault<dm_KPI_KinhDoanh>();
          if (KPI_KinhDoanh != null)
          {
            KPI_KinhDoanh.ID_NGUOITAO = item.ID_NGUOITAO;
            KPI_KinhDoanh.THOIGIANTHEM = new DateTime?(DateTime.Now);
            KPI_KinhDoanh.ID_NGUOISUA = (string) null;
            KPI_KinhDoanh.THOIGIANSUA = new DateTime?();
            KPI_KinhDoanh.ID = Guid.NewGuid().ToString();
            dm_KPI_KinhDoanh dmKpiKinhDoanh1 = KPI_KinhDoanh;
            DateTime dateTime1 = DateTime.Now;
            int year1 = dateTime1.Year;
            dateTime1 = DateTime.Now;
            int month1 = dateTime1.Month;
            DateTime dateTime2 = new DateTime(year1, month1, 1);
            dmKpiKinhDoanh1.TUNGAY = dateTime2;
            dm_KPI_KinhDoanh dmKpiKinhDoanh2 = KPI_KinhDoanh;
            dateTime1 = DateTime.Now;
            int year2 = dateTime1.Year;
            dateTime1 = DateTime.Now;
            int month2 = dateTime1.Month;
            dateTime1 = DateTime.Now;
            int year3 = dateTime1.Year;
            dateTime1 = DateTime.Now;
            int month3 = dateTime1.Month;
            int day = DateTime.DaysInMonth(year3, month3);
            DateTime dateTime3 = new DateTime(year2, month2, day);
            dmKpiKinhDoanh2.DENNGAY = dateTime3;
            dm_KPI_KinhDoanh dmKpiKinhDoanh3 = KPI_KinhDoanh;
            string ma = KPI_KinhDoanh.MA;
            dateTime1 = DateTime.Now;
            dateTime1 = dateTime1.AddMonths(-1);
            string oldValue1 = $"(T{dateTime1.Month.ToString("00")})";
            string str1 = ma.Replace(oldValue1, "");
            dateTime1 = DateTime.Now;
            string str2 = dateTime1.Month.ToString("00");
            string str3 = $"{str1}(T{str2})";
            dmKpiKinhDoanh3.MA = str3;
            dm_KPI_KinhDoanh KPI_KinhDoanhCheck = this._context.dm_KPI_KinhDoanh.AsNoTracking<dm_KPI_KinhDoanh>().Where<dm_KPI_KinhDoanh>((Expression<Func<dm_KPI_KinhDoanh, bool>>) (e => e.LOC_ID == item.LOC_ID && e.MA == KPI_KinhDoanh.MA)).FirstOrDefault<dm_KPI_KinhDoanh>();
            if (KPI_KinhDoanhCheck != null)
              KPI_KinhDoanh.MA += "_Copy";
            dm_KPI_KinhDoanh dmKpiKinhDoanh4 = KPI_KinhDoanh;
            string name = KPI_KinhDoanh.NAME;
            dateTime1 = DateTime.Now;
            dateTime1 = dateTime1.AddMonths(-1);
            string oldValue2 = $"(T{dateTime1.Month.ToString("00")})";
            string str4 = name.Replace(oldValue2, "");
            dateTime1 = DateTime.Now;
            string str5 = dateTime1.Month.ToString("00");
            string str6 = $"{str4}(T{str5})";
            dmKpiKinhDoanh4.NAME = str6;
            List<dm_KPI_KinhDoanh_NhanVien> lstdm_KPI_KinhDoanh_NhanVien = this._context.dm_KPI_KinhDoanh_NhanVien.AsNoTracking<dm_KPI_KinhDoanh_NhanVien>().Where<dm_KPI_KinhDoanh_NhanVien>((Expression<Func<dm_KPI_KinhDoanh_NhanVien, bool>>) (e => e.LOC_ID == item.LOC_ID && e.ID_KPI_KINHDOANH == item.ID)).ToList<dm_KPI_KinhDoanh_NhanVien>();
            List<dm_KPI_KinhDoanh_YeuCau> lstdm_KPI_KinhDoanh_YeuCau = this._context.dm_KPI_KinhDoanh_YeuCau.AsNoTracking<dm_KPI_KinhDoanh_YeuCau>().Where<dm_KPI_KinhDoanh_YeuCau>((Expression<Func<dm_KPI_KinhDoanh_YeuCau, bool>>) (e => e.LOC_ID == item.LOC_ID && e.ID_KPI_KINHDOANH == item.ID)).ToList<dm_KPI_KinhDoanh_YeuCau>();
            if (lstdm_KPI_KinhDoanh_NhanVien != null)
            {
              foreach (dm_KPI_KinhDoanh_NhanVien itm in lstdm_KPI_KinhDoanh_NhanVien)
              {
                itm.ID = Guid.NewGuid().ToString();
                itm.ID_KPI_KINHDOANH = KPI_KinhDoanh.ID;
                this._context.dm_KPI_KinhDoanh_NhanVien.Add(itm);
              }
            }
            if (lstdm_KPI_KinhDoanh_YeuCau != null)
            {
              foreach (dm_KPI_KinhDoanh_YeuCau itm in lstdm_KPI_KinhDoanh_YeuCau)
              {
                itm.ID = Guid.NewGuid().ToString();
                itm.ID_KPI_KINHDOANH = KPI_KinhDoanh.ID;
                this._context.dm_KPI_KinhDoanh_YeuCau.Add(itm);
              }
            }
            this._context.dm_KPI_KinhDoanh.Add(KPI_KinhDoanh);
            KPI_KinhDoanhCheck = (dm_KPI_KinhDoanh) null;
            lstdm_KPI_KinhDoanh_NhanVien = (List<dm_KPI_KinhDoanh_NhanVien>) null;
            lstdm_KPI_KinhDoanh_YeuCau = (List<dm_KPI_KinhDoanh_YeuCau>) null;
          }
        }
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        auditLog = (AuditLogController) null;
        transaction.Commit();
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) ""
        });
      }
    }
    catch (Exception ex)
    {
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpPut("PutKPI_Tam")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PutKPI_Tam([FromBody] SP_Parameter SP_Parameter)
  {
    try
    {
      List<DanhSachPhieuDatHang_ChiTiet_KPI> lst_ChiTiet = new List<DanhSachPhieuDatHang_ChiTiet_KPI>();
      List<DanhSachPhieuTraHang_ChiTiet_KPI> lst_ChiTiet_TraHang = new List<DanhSachPhieuTraHang_ChiTiet_KPI>();
      List<v_Tinh_KPI_KinhDoanh> lstTinh_KPI_KinhDoanh = new List<v_Tinh_KPI_KinhDoanh>();
      List<view_dm_KPI_KinhDoanh> lstValue = await this._context.view_dm_KPI_KinhDoanh.Where<view_dm_KPI_KinhDoanh>((Expression<Func<view_dm_KPI_KinhDoanh, bool>>) (e => e.LOC_ID == SP_Parameter.LOC_ID && (DateTime?) e.TUNGAY <= SP_Parameter.TUNGAY && (DateTime?) e.DENNGAY >= SP_Parameter.DENNGAY && e.ISACTIVE)).OrderBy<view_dm_KPI_KinhDoanh, int>((Expression<Func<view_dm_KPI_KinhDoanh, int>>) (e => e.CAPDO)).ToListAsync<view_dm_KPI_KinhDoanh>();
      if (lstValue != null)
      {
        ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
        IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuTraHang_ChiTiet_KPI(SP_Parameter);
        if (actionResult is OkObjectResult okResult2)
        {
          ApiResponse ApiResponse = okResult2.Value as ApiResponse;
          if (ApiResponse != null && ApiResponse.Data != null)
            lst_ChiTiet_TraHang = ApiResponse.Data as List<DanhSachPhieuTraHang_ChiTiet_KPI>;
          ApiResponse = (ApiResponse) null;
        }
        actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuDatHang_ChiTiet_KPI(SP_Parameter);
        if (actionResult is OkObjectResult okResult2)
        {
          ApiResponse ApiResponse = okResult2.Value as ApiResponse;
          if (ApiResponse != null && ApiResponse.Data != null)
          {
            lst_ChiTiet = ApiResponse.Data as List<DanhSachPhieuDatHang_ChiTiet_KPI>;
            if (lst_ChiTiet != null && lst_ChiTiet_TraHang != null)
            {
              foreach (view_dm_KPI_KinhDoanh viewDmKpiKinhDoanh in lstValue)
              {
                view_dm_KPI_KinhDoanh itm = viewDmKpiKinhDoanh;
                List<view_dm_KPI_KinhDoanh_NhanVien> lstValue_NhanVien = await this._context.view_dm_KPI_KinhDoanh_NhanVien.Where<view_dm_KPI_KinhDoanh_NhanVien>((Expression<Func<view_dm_KPI_KinhDoanh_NhanVien, bool>>) (e => e.ID_KPI_KINHDOANH == itm.ID)).ToListAsync<view_dm_KPI_KinhDoanh_NhanVien>();
                List<view_dm_KPI_KinhDoanh_YeuCau> lstValue_YeuCau = await this._context.view_dm_KPI_KinhDoanh_YeuCau.Where<view_dm_KPI_KinhDoanh_YeuCau>((Expression<Func<view_dm_KPI_KinhDoanh_YeuCau, bool>>) (e => e.ID_KPI_KINHDOANH == itm.ID)).ToListAsync<view_dm_KPI_KinhDoanh_YeuCau>();
                foreach (view_dm_KPI_KinhDoanh_NhanVien kinhDoanhNhanVien in lstValue_NhanVien)
                {
                  view_dm_KPI_KinhDoanh_NhanVien NhanVien = kinhDoanhNhanVien;
                  if (NhanVien.HINHTHUC == 0 && (string.IsNullOrEmpty(SP_Parameter.ID_NHANVIEN) || NhanVien.ID_NHANVIEN == SP_Parameter.ID_NHANVIEN))
                  {
                    if (!string.IsNullOrEmpty(SP_Parameter.ID_NHOMQUYEN))
                    {
                      view_dm_NhanVien objNhanVien = await this._context.view_dm_NhanVien.Where<view_dm_NhanVien>((Expression<Func<view_dm_NhanVien, bool>>) (e => e.ID == NhanVien.ID_NHANVIEN)).FirstOrDefaultAsync<view_dm_NhanVien>();
                      if (objNhanVien == null || !(objNhanVien.ID_NHOMQUYEN != SP_Parameter.ID_NHOMQUYEN))
                        objNhanVien = (view_dm_NhanVien) null;
                      else
                        continue;
                    }
                    v_Tinh_KPI_KinhDoanh NhanVienKinhDoanh = lstTinh_KPI_KinhDoanh.FirstOrDefault<v_Tinh_KPI_KinhDoanh>((Func<v_Tinh_KPI_KinhDoanh, bool>) (e => e.ID_NHANVIEN == NhanVien.ID_NHANVIEN));
                    if (NhanVienKinhDoanh == null)
                    {
                      NhanVienKinhDoanh = new v_Tinh_KPI_KinhDoanh();
                      NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet = new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
                      NhanVienKinhDoanh.ID_NHANVIEN = NhanVien.ID_NHANVIEN;
                      NhanVienKinhDoanh.NAME_NHANVIEN = $"{NhanVien.MA} - {NhanVien.NAME}";
                      dm_NhanVien objNhanVien = await this._context.dm_NhanVien.FirstOrDefaultAsync<dm_NhanVien>((Expression<Func<dm_NhanVien, bool>>) (e => e.ID == NhanVien.ID_NHANVIEN));
                      NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange((IEnumerable<v_Tinh_KPI_KinhDoanh_ChiTiet>) this.Get_ChiTietTam(itm, NhanVien, lstValue_YeuCau, lst_ChiTiet, NhanVienKinhDoanh, lst_ChiTiet_TraHang));
                      NhanVienKinhDoanh.SOTIEN_KPI = NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Sum<v_Tinh_KPI_KinhDoanh_ChiTiet>((Func<v_Tinh_KPI_KinhDoanh_ChiTiet, double>) (e => e.SOTIEN_KPI));
                      if (NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet != null && NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Count > 0)
                        lstTinh_KPI_KinhDoanh.Add(NhanVienKinhDoanh);
                      objNhanVien = (dm_NhanVien) null;
                    }
                    else
                    {
                      if (NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.FirstOrDefault<v_Tinh_KPI_KinhDoanh_ChiTiet>((Func<v_Tinh_KPI_KinhDoanh_ChiTiet, bool>) (e => e.ID_KPI_KINHDOANH == itm.ID)) == null)
                        NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange((IEnumerable<v_Tinh_KPI_KinhDoanh_ChiTiet>) this.Get_ChiTietTam(itm, NhanVien, lstValue_YeuCau, lst_ChiTiet, NhanVienKinhDoanh, lst_ChiTiet_TraHang));
                      NhanVienKinhDoanh.SOTIEN_KPI = NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Sum<v_Tinh_KPI_KinhDoanh_ChiTiet>((Func<v_Tinh_KPI_KinhDoanh_ChiTiet, double>) (e => e.SOTIEN_KPI));
                    }
                    NhanVienKinhDoanh = (v_Tinh_KPI_KinhDoanh) null;
                  }
                  if (NhanVien.HINHTHUC == 1 && (string.IsNullOrEmpty(SP_Parameter.ID_NHOMQUYEN) || NhanVien.ID_NHANVIEN == SP_Parameter.ID_NHOMQUYEN))
                  {
                    List<view_dm_NhanVien> lstNhanVien = await this._context.view_dm_NhanVien.Where<view_dm_NhanVien>((Expression<Func<view_dm_NhanVien, bool>>) (e => e.ID_NHOMQUYEN == NhanVien.ID_NHANVIEN && e.ISACTIVE)).ToListAsync<view_dm_NhanVien>();
                    if (lstNhanVien != null)
                    {
                      foreach (view_dm_NhanVien viewDmNhanVien in lstNhanVien)
                      {
                        view_dm_NhanVien item = viewDmNhanVien;
                        if (item != null)
                        {
                          if (string.IsNullOrEmpty(SP_Parameter.ID_NHOMQUYEN) || item == null || !(item.ID_NHOMQUYEN != SP_Parameter.ID_NHOMQUYEN))
                          {
                            v_Tinh_KPI_KinhDoanh NhanVienKinhDoanh = lstTinh_KPI_KinhDoanh.FirstOrDefault<v_Tinh_KPI_KinhDoanh>((Func<v_Tinh_KPI_KinhDoanh, bool>) (e => e.ID_NHANVIEN == item.ID));
                            if (NhanVienKinhDoanh == null)
                            {
                              NhanVienKinhDoanh = new v_Tinh_KPI_KinhDoanh();
                              NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet = new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
                              NhanVienKinhDoanh.ID_NHANVIEN = item.ID;
                              NhanVienKinhDoanh.NAME_NHANVIEN = $"{item.MA} - {item.NAME}";
                              dm_NhanVien objNhanVien = await this._context.dm_NhanVien.FirstOrDefaultAsync<dm_NhanVien>((Expression<Func<dm_NhanVien, bool>>) (e => e.ID == item.ID));
                              NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange((IEnumerable<v_Tinh_KPI_KinhDoanh_ChiTiet>) this.Get_ChiTietTam(itm, NhanVien, lstValue_YeuCau, lst_ChiTiet, NhanVienKinhDoanh, lst_ChiTiet_TraHang));
                              NhanVienKinhDoanh.SOTIEN_KPI = NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Sum<v_Tinh_KPI_KinhDoanh_ChiTiet>((Func<v_Tinh_KPI_KinhDoanh_ChiTiet, double>) (e => e.SOTIEN_KPI));
                              if (NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet != null && NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Count > 0)
                                lstTinh_KPI_KinhDoanh.Add(NhanVienKinhDoanh);
                              objNhanVien = (dm_NhanVien) null;
                            }
                            else if (NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.FirstOrDefault<v_Tinh_KPI_KinhDoanh_ChiTiet>((Func<v_Tinh_KPI_KinhDoanh_ChiTiet, bool>) (e => e.ID_KPI_KINHDOANH == itm.ID)) == null)
                              NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange((IEnumerable<v_Tinh_KPI_KinhDoanh_ChiTiet>) this.Get_ChiTietTam(itm, NhanVien, lstValue_YeuCau, lst_ChiTiet, NhanVienKinhDoanh, lst_ChiTiet_TraHang));
                            NhanVienKinhDoanh = (v_Tinh_KPI_KinhDoanh) null;
                          }
                          else
                            continue;
                        }
                      }
                    }
                    lstNhanVien = (List<view_dm_NhanVien>) null;
                  }
                }
                lstValue_NhanVien = (List<view_dm_KPI_KinhDoanh_NhanVien>) null;
                lstValue_YeuCau = (List<view_dm_KPI_KinhDoanh_YeuCau>) null;
              }
            }
          }
          ApiResponse = (ApiResponse) null;
        }
        ExecuteStoredProc1 = (ExecuteStoredProc) null;
        actionResult = (IActionResult) null;
        okResult2 = (OkObjectResult) null;
      }
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) lstTinh_KPI_KinhDoanh
      });
    }
    catch (DbUpdateConcurrencyException ex)
    {
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  private List<v_Tinh_KPI_KinhDoanh_ChiTiet> Get_ChiTietTam(
    view_dm_KPI_KinhDoanh KPI_KinhDoanh,
    view_dm_KPI_KinhDoanh_NhanVien KPI_KinhDoanh_NhanVien,
    List<view_dm_KPI_KinhDoanh_YeuCau> lstValue_YeuCau,
    List<DanhSachPhieuDatHang_ChiTiet_KPI> lstChiTietDatHang,
    v_Tinh_KPI_KinhDoanh NhanVien_KinhDoanh,
    List<DanhSachPhieuTraHang_ChiTiet_KPI> lstChiTietTraHang)
  {
    try
    {
      List<v_Tinh_KPI_KinhDoanh_ChiTiet> chiTietTam = new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
      bool flag = true;
      double num1 = 0.0;
      double num2 = 0.0;
      double num3 = 0.0;
      foreach (view_dm_KPI_KinhDoanh_YeuCau kpiKinhDoanhYeuCau in lstValue_YeuCau)
      {
        view_dm_KPI_KinhDoanh_YeuCau itm = kpiKinhDoanhYeuCau;
        double num4 = lstChiTietDatHang.Where<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, bool>) (e =>
        {
          if (e.bolDaTinhKpi || (itm.HINHTHUC == 0 ? (e.ID_HANGHOA == itm.ID_HANGHOA ? 1 : 0) : (e.ID_NHOMHANGHOA == itm.ID_HANGHOA ? 1 : 0)) == 0)
            return false;
          return KPI_KinhDoanh_NhanVien.HINHTHUC != 0 ? e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN : e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN;
        })).Sum<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, double>) (e => e.TONGCONG));
        double num5 = lstChiTietTraHang.Where<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, bool>) (e =>
        {
          if (e.bolDaTinhKpi || (itm.HINHTHUC == 0 ? (e.ID_HANGHOA == itm.ID_HANGHOA ? 1 : 0) : (e.ID_NHOMHANGHOA == itm.ID_HANGHOA ? 1 : 0)) == 0)
            return false;
          return KPI_KinhDoanh_NhanVien.HINHTHUC != 0 ? e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN : e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN;
        })).Sum<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, double>) (e => e.TONGCONG));
        double num6 = lstChiTietDatHang.Where<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, bool>) (e =>
        {
          if (e.bolDaTinhKpi || (itm.HINHTHUC == 0 ? (!(e.ID_HANGHOA == itm.ID_HANGHOA) ? 0 : (e.ID_DVT == itm.ID_DVT ? 1 : 0)) : (!(e.ID_NHOMHANGHOA == itm.ID_HANGHOA) ? 0 : (e.ID_DVT == itm.ID_DVT ? 1 : 0))) == 0)
            return false;
          return KPI_KinhDoanh_NhanVien.HINHTHUC != 0 ? e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN : e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN;
        })).Sum<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, double>) (e => e.TONGSOLUONG / e.TYLE_QD)) + (double) Convert.ToInt32(lstChiTietDatHang.Where<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, bool>) (e =>
        {
          if (e.bolDaTinhKpi || (itm.HINHTHUC == 0 ? (!(e.ID_HANGHOA == itm.ID_HANGHOA) ? 0 : (e.ID_DVT != itm.ID_DVT ? 1 : 0)) : (!(e.ID_NHOMHANGHOA == itm.ID_HANGHOA) ? 0 : (e.ID_DVT != itm.ID_DVT ? 1 : 0))) == 0)
            return false;
          return KPI_KinhDoanh_NhanVien.HINHTHUC != 0 ? e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN : e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN;
        })).Sum<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, double>) (e => e.TONGSOLUONG / (e.TYLE_QD_HH == 0.0 ? 1.0 : e.TYLE_QD_HH))));
        double num7 = lstChiTietTraHang.Where<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, bool>) (e =>
        {
          if (e.bolDaTinhKpi || (itm.HINHTHUC == 0 ? (!(e.ID_HANGHOA == itm.ID_HANGHOA) ? 0 : (e.ID_DVT == itm.ID_DVT ? 1 : 0)) : (!(e.ID_NHOMHANGHOA == itm.ID_HANGHOA) ? 0 : (e.ID_DVT == itm.ID_DVT ? 1 : 0))) == 0)
            return false;
          return KPI_KinhDoanh_NhanVien.HINHTHUC != 0 ? e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN : e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN;
        })).Sum<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, double>) (e => e.TONGSOLUONG / e.TYLE_QD)) + (double) Convert.ToInt32(lstChiTietTraHang.Where<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, bool>) (e =>
        {
          if (e.bolDaTinhKpi || (itm.HINHTHUC == 0 ? (!(e.ID_HANGHOA == itm.ID_HANGHOA) ? 0 : (e.ID_DVT != itm.ID_DVT ? 1 : 0)) : (!(e.ID_NHOMHANGHOA == itm.ID_HANGHOA) ? 0 : (e.ID_DVT != itm.ID_DVT ? 1 : 0))) == 0)
            return false;
          return KPI_KinhDoanh_NhanVien.HINHTHUC != 0 ? e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN : e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN;
        })).Sum<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, double>) (e => e.TONGSOLUONG / (e.TYLE_QD_HH == 0.0 ? 1.0 : e.TYLE_QD_HH))));
        if ((itm.SOTIEN > 0.0 || itm.SOLUONG > 0.0 || itm.CHIETKHAU > 0.0 || itm.TIENGIAM > 0.0) && 0.0 < num4 - num5 && 0.0 <= num6 - num7)
        {
          dm_DonViTinh dmDonViTinh = this._context.dm_DonViTinh.Where<dm_DonViTinh>((Expression<Func<dm_DonViTinh, bool>>) (e => e.ID == itm.ID_DVT)).FirstOrDefault<dm_DonViTinh>();
          v_Tinh_KPI_KinhDoanh_ChiTiet kinhDoanhChiTiet1 = new v_Tinh_KPI_KinhDoanh_ChiTiet();
          v_Tinh_KPI_KinhDoanh_ChiTiet kinhDoanhChiTiet2 = new v_Tinh_KPI_KinhDoanh_ChiTiet();
          kinhDoanhChiTiet2.HINHTHUC = itm.HINHTHUC;
          kinhDoanhChiTiet2.NAME_HINHTHUC = itm.NAME_HINHTHUC;
          kinhDoanhChiTiet2.ID_KPI_KINHDOANH = KPI_KinhDoanh.ID;
          kinhDoanhChiTiet2.NAME_KPI_KINHDOANH = KPI_KinhDoanh.NAME;
          kinhDoanhChiTiet2.ID_HANGHOA = itm.ID_HANGHOA;
          kinhDoanhChiTiet2.NAME_HANGHOA = itm.NAME;
          if (itm.CHIETKHAU > 0.0 || itm.TIENGIAM > 0.0)
          {
            kinhDoanhChiTiet2.TONGTIEN = num4;
            kinhDoanhChiTiet2.TONGTIEN_TRAHANG = num5;
            kinhDoanhChiTiet2.TONGTIEN_KPI = itm.SOTIEN;
          }
          int num8 = 0;
          double num9 = 1.0;
          num8 = Convert.ToInt32(num6) / Convert.ToInt32(num9);
          kinhDoanhChiTiet2.NAME_DVT = itm.NAME_DVT;
          kinhDoanhChiTiet2.NAME_DVT_QD = dmDonViTinh != null ? dmDonViTinh.NAME : "";
          kinhDoanhChiTiet2.TYLE_QD = num9;
          if (itm.HINHTHUC_TINHKPI == 1 && (itm.CHIETKHAU > 0.0 || itm.TIENGIAM > 0.0))
          {
            kinhDoanhChiTiet2.TONGSOLUONG = (double) (Convert.ToInt32(num6) / Convert.ToInt32(num9));
            kinhDoanhChiTiet2.TONGSOLUONG_TRAHANG = (double) (Convert.ToInt32(num7) / Convert.ToInt32(num9));
            kinhDoanhChiTiet2.TONGSOLUONG_KPI = $"{itm.SOLUONG.ToString("N0").Replace(',', '.')} {itm.NAME_DVT}";
          }
          if (itm.HINHTHUC_TINHKPI == 3 && (itm.CHIETKHAU > 0.0 || itm.TIENGIAM > 0.0))
          {
            kinhDoanhChiTiet2.TONGSOLUONG = (double) (Convert.ToInt32(num6) / Convert.ToInt32(num9));
            kinhDoanhChiTiet2.TONGSOLUONG_TRAHANG = (double) (Convert.ToInt32(num7) / Convert.ToInt32(num9));
            kinhDoanhChiTiet2.TONGSOLUONG_KPI = $"{itm.SOLUONG.ToString("N0").Replace(',', '.')} {itm.NAME_DVT}";
          }
          kinhDoanhChiTiet2.CHIETKHAU = itm.CHIETKHAU;
          kinhDoanhChiTiet2.TIENTHUONG = itm.TIENGIAM;
          int num10 = Convert.ToInt32(num6 - num7) / Convert.ToInt32(num9);
          if (itm.SOTIEN <= num4 - num5 && itm.SOLUONG <= num6 - num7)
            kinhDoanhChiTiet2.SOTIEN_KPI = itm.HINHTHUC_TINHKPI != 1 ? (itm.HINHTHUC_TINHKPI != 3 ? (itm.HINHTHUC_TINHKPI != 2 ? (kinhDoanhChiTiet2.CHIETKHAU > 0.0 ? kinhDoanhChiTiet2.CHIETKHAU * (num4 - num5) / 100.0 : itm.TIENGIAM) : (kinhDoanhChiTiet2.CHIETKHAU > 0.0 ? kinhDoanhChiTiet2.CHIETKHAU * (num4 - num5 - itm.SOTIEN) / 100.0 : itm.TIENGIAM)) : (kinhDoanhChiTiet2.CHIETKHAU > 0.0 ? kinhDoanhChiTiet2.CHIETKHAU * (num4 - num5) / 100.0 : itm.TIENGIAM * (double) num10)) : (kinhDoanhChiTiet2.CHIETKHAU > 0.0 ? kinhDoanhChiTiet2.CHIETKHAU * (num4 - num5) / 100.0 : itm.TIENGIAM * ((double) num10 - itm.SOLUONG));
          chiTietTam.Add(kinhDoanhChiTiet2);
          foreach (DanhSachPhieuDatHang_ChiTiet_KPI datHangChiTietKpi in lstChiTietDatHang.Where<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, bool>) (e =>
          {
            if (e.bolDaTinhKpi || (itm.HINHTHUC == 0 ? (e.ID_HANGHOA == itm.ID_HANGHOA ? 1 : 0) : (e.ID_NHOMHANGHOA == itm.ID_HANGHOA ? 1 : 0)) == 0)
              return false;
            return KPI_KinhDoanh_NhanVien.HINHTHUC != 0 ? e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN : e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN;
          })))
            ;
          foreach (DanhSachPhieuTraHang_ChiTiet_KPI traHangChiTietKpi in lstChiTietTraHang.Where<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, bool>) (e =>
          {
            if (e.bolDaTinhKpi || (itm.HINHTHUC == 0 ? (e.ID_HANGHOA == itm.ID_HANGHOA ? 1 : 0) : (e.ID_NHOMHANGHOA == itm.ID_HANGHOA ? 1 : 0)) == 0)
              return false;
            return KPI_KinhDoanh_NhanVien.HINHTHUC != 0 ? e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN : e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN;
          })))
            ;
          foreach (DanhSachPhieuDatHang_ChiTiet_KPI datHangChiTietKpi in lstChiTietDatHang.Where<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, bool>) (e =>
          {
            if (e.bolDaTinhKpi || (itm.HINHTHUC == 0 ? (!(e.ID_HANGHOA == itm.ID_HANGHOA) ? 0 : (e.ID_DVT == itm.ID_DVT ? 1 : 0)) : (!(e.ID_NHOMHANGHOA == itm.ID_HANGHOA) ? 0 : (e.ID_DVT == itm.ID_DVT ? 1 : 0))) == 0)
              return false;
            return KPI_KinhDoanh_NhanVien.HINHTHUC != 0 ? e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN : e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN;
          })))
            ;
          foreach (DanhSachPhieuTraHang_ChiTiet_KPI traHangChiTietKpi in lstChiTietTraHang.Where<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, bool>) (e =>
          {
            if (e.bolDaTinhKpi || (itm.HINHTHUC == 0 ? (!(e.ID_HANGHOA == itm.ID_HANGHOA) ? 0 : (e.ID_DVT == itm.ID_DVT ? 1 : 0)) : (!(e.ID_NHOMHANGHOA == itm.ID_HANGHOA) ? 0 : (e.ID_DVT == itm.ID_DVT ? 1 : 0))) == 0)
              return false;
            return KPI_KinhDoanh_NhanVien.HINHTHUC != 0 ? e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN : e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN;
          })))
            ;
          num1 += num4 - num5;
        }
        else
          flag = false;
        num2 += num4;
        num3 += num5;
      }
      double num11 = lstChiTietDatHang.Where<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, bool>) (e => e.ID_DVT == KPI_KinhDoanh.ID_DVT_DATKM)).Sum<DanhSachPhieuDatHang_ChiTiet_KPI>((Func<DanhSachPhieuDatHang_ChiTiet_KPI, double>) (e => e.TONGCONG));
      double num12 = lstChiTietTraHang.Where<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, bool>) (e => e.ID_DVT == KPI_KinhDoanh.ID_DVT_DATKM)).Sum<DanhSachPhieuTraHang_ChiTiet_KPI>((Func<DanhSachPhieuTraHang_ChiTiet_KPI, double>) (e => e.TONGCONG));
      if (KPI_KinhDoanh.IS_YEUCAUCHITIET)
      {
        if (flag && (KPI_KinhDoanh.CHIETKHAU > 0.0 || KPI_KinhDoanh.TIENGIAM > 0.0))
        {
          v_Tinh_KPI_KinhDoanh_ChiTiet kinhDoanhChiTiet = new v_Tinh_KPI_KinhDoanh_ChiTiet()
          {
            HINHTHUC = -1,
            NAME_HINHTHUC = "Tổng",
            ID_KPI_KINHDOANH = KPI_KinhDoanh.ID,
            NAME_KPI_KINHDOANH = KPI_KinhDoanh.NAME
          };
          kinhDoanhChiTiet.NAME_HANGHOA = kinhDoanhChiTiet.NAME_HINHTHUC;
          kinhDoanhChiTiet.TONGTIEN = num2;
          kinhDoanhChiTiet.TONGTIEN_TRAHANG = num3;
          kinhDoanhChiTiet.TONGTIEN_KPI = KPI_KinhDoanh.TONGTIEN_DATKM;
          if (!string.IsNullOrEmpty(KPI_KinhDoanh.NAME_DVT))
          {
            kinhDoanhChiTiet.TONGSOLUONG = num11;
            kinhDoanhChiTiet.NAME_DVT = KPI_KinhDoanh.NAME_DVT;
            kinhDoanhChiTiet.TONGSOLUONG_TRAHANG = num12;
            kinhDoanhChiTiet.TONGSOLUONG_KPI = $"{KPI_KinhDoanh.SOLUONG_DATKM.ToString()} {KPI_KinhDoanh.NAME_DVT}";
          }
          kinhDoanhChiTiet.CHIETKHAU = KPI_KinhDoanh.CHIETKHAU;
          kinhDoanhChiTiet.TIENTHUONG = KPI_KinhDoanh.TIENGIAM;
          kinhDoanhChiTiet.SOTIEN_KPI = kinhDoanhChiTiet.CHIETKHAU > 0.0 ? kinhDoanhChiTiet.CHIETKHAU * num1 / 100.0 : KPI_KinhDoanh.TIENGIAM;
          chiTietTam.Add(kinhDoanhChiTiet);
        }
      }
      else if ((KPI_KinhDoanh.CHIETKHAU > 0.0 || KPI_KinhDoanh.TIENGIAM > 0.0) && 0.0 < num11 && 0.0 < num2)
      {
        v_Tinh_KPI_KinhDoanh_ChiTiet kinhDoanhChiTiet = new v_Tinh_KPI_KinhDoanh_ChiTiet()
        {
          HINHTHUC = -1,
          NAME_HINHTHUC = "Tổng",
          ID_KPI_KINHDOANH = KPI_KinhDoanh.ID,
          NAME_KPI_KINHDOANH = KPI_KinhDoanh.NAME
        };
        kinhDoanhChiTiet.NAME_HANGHOA = kinhDoanhChiTiet.NAME_HINHTHUC;
        kinhDoanhChiTiet.TONGTIEN = num2;
        kinhDoanhChiTiet.TONGTIEN_TRAHANG = num3;
        kinhDoanhChiTiet.TONGTIEN_KPI = KPI_KinhDoanh.TONGTIEN_DATKM;
        if (!string.IsNullOrEmpty(KPI_KinhDoanh.NAME_DVT))
        {
          kinhDoanhChiTiet.TONGSOLUONG = num11;
          kinhDoanhChiTiet.NAME_DVT = KPI_KinhDoanh.NAME_DVT;
          kinhDoanhChiTiet.TONGSOLUONG_TRAHANG = num12;
          kinhDoanhChiTiet.TONGSOLUONG_KPI = $"{KPI_KinhDoanh.SOLUONG_DATKM.ToString()} {KPI_KinhDoanh.NAME_DVT}";
        }
        kinhDoanhChiTiet.CHIETKHAU = KPI_KinhDoanh.CHIETKHAU;
        kinhDoanhChiTiet.TIENTHUONG = KPI_KinhDoanh.TIENGIAM;
        if (KPI_KinhDoanh.SOLUONG_DATKM <= num11 && KPI_KinhDoanh.TONGTIEN_DATKM <= num2)
          kinhDoanhChiTiet.SOTIEN_KPI = kinhDoanhChiTiet.CHIETKHAU > 0.0 ? kinhDoanhChiTiet.CHIETKHAU * (kinhDoanhChiTiet.TONGTIEN - kinhDoanhChiTiet.TONGTIEN_TRAHANG) / 100.0 : KPI_KinhDoanh.TIENGIAM;
        chiTietTam.Add(kinhDoanhChiTiet);
      }
      return chiTietTam;
    }
    catch
    {
      return new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
    }
  }
}
