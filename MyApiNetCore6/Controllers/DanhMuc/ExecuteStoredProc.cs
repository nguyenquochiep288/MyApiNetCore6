// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.ExecuteStoredProc
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.StoredProcedure;
using DatabaseTHP.StoredProcedure.Parameter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyApiNetCore6.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;


namespace MyApiNetCore6.Controllers;

[Route("api/")]
[ApiController]
public class ExecuteStoredProc : Controller
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public ExecuteStoredProc(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._configuration = configuration;
    this._context = context;
  }

  [HttpPost("web_Sp_Get_DSKhachHang")]
  public async Task<IActionResult> web_Sp_Get_DSKhachHang(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_dm_KhachHang>(nameof (web_Sp_Get_DSKhachHang), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("web_Sp_Get_DSNhomSanPham")]
  public async Task<IActionResult> web_Sp_Get_DSNhomSanPham(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<web_Sp_Get_DSNhomSanPham_Result>(nameof (web_Sp_Get_DSNhomSanPham), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("web_Sp_Get_DSSanPham")]
  public async Task<IActionResult> web_Sp_Get_DSSanPham(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<web_Sp_Get_DSSanPham_Result>(nameof (web_Sp_Get_DSSanPham), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("web_Sp_Get_SanPham")]
  public async Task<IActionResult> web_Sp_Get_SanPham(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<web_Sp_Get_SanPham_Result>(nameof (web_Sp_Get_SanPham), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("web_Sp_Get_SanPham_Combo")]
  public async Task<IActionResult> web_Sp_Get_DSSanPham_Combo(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<web_Sp_Get_SanPham_Combo_Result>("web_Sp_Get_SanPham_Combo", procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("web_Sp_Get_SanPham_Group")]
  public async Task<IActionResult> web_Sp_Get_DSSanPham_Group(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<web_Sp_Get_SanPham_Group_Result>("web_Sp_Get_SanPham_Group", procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("web_Sp_Get_DanhSachKhuyenMai")]
  public async Task<IActionResult> web_Sp_Get_DanhSachKhuyenMai(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<web_Sp_Get_DanhSachKhuyenMai_Result>(nameof (web_Sp_Get_DanhSachKhuyenMai), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("web_Sp_Get_DanhSachChietKhauHoaDon")]
  public async Task<IActionResult> web_Sp_Get_DanhSachChietKhauHoaDon(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<web_Sp_Get_DanhSachChietKhauHoaDon_Result>(nameof (web_Sp_Get_DanhSachChietKhauHoaDon), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("web_Sp_Get_DSKhuVuc")]
  public async Task<IActionResult> web_Sp_Get_DSKhuVuc(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<web_Sp_Get_DSKhuVuc_Result>(nameof (web_Sp_Get_DSKhuVuc), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_BaoCaoTheoNhanVien")]
  public async Task<IActionResult> Sp_Get_BaoCaoTheoNhanVien(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<Sp_Get_BaoCaoTheoNhanVien_Result>(nameof (Sp_Get_BaoCaoTheoNhanVien), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_BaoCaoTaiChinh_New")]
  public async Task<IActionResult> Sp_Get_BaoCaoTaiChinh_New(SP_Parameter_Report sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter_Report(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<Sp_Get_BaoCaoTaiChinh_New_Result>(nameof (Sp_Get_BaoCaoTaiChinh_New), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachSanPhamKho")]
  public async Task<IActionResult> Sp_Get_DanhSachSanPhamKho(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<Product_Detail>(nameof (Sp_Get_DanhSachSanPhamKho), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachSanPhamKho_Combo")]
  public async Task<IActionResult> Sp_Get_DanhSachSanPhamKho_Combo(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<Product_Detail>(nameof (Sp_Get_DanhSachSanPhamKho_Combo), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_BaoCaoGiaoHang")]
  public async Task<IActionResult> Sp_Get_BaoCaoGiaoHang(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<Sp_Get_BaoCaoGiaoHang_Result>(nameof (Sp_Get_BaoCaoGiaoHang), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_BaoCaoGiaoHang_ChiTiet")]
  public async Task<IActionResult> Sp_Get_BaoCaoGiaoHang_ChiTiet(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ThongKeCongNo_ChiTiet>(nameof (Sp_Get_BaoCaoGiaoHang_ChiTiet), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_BaoCaoTaiChinh")]
  public async Task<IActionResult> Sp_Get_BaoCaoTaiChinh(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<Sp_Get_BaoCaoTaiChinh_Result>(nameof (Sp_Get_BaoCaoTaiChinh), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuNhap")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuNhap(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ct_PhieuNhap>(nameof (Sp_Get_DanhSachPhieuNhap), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuNhap_PhieuGiaoHang")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuNhap_PhieuGiaoHang(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ct_PhieuNhap>(nameof (Sp_Get_DanhSachPhieuNhap_PhieuGiaoHang), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuNhap_Chitiet")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuNhap_Chitiet(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ct_PhieuNhap_ChiTiet>(nameof (Sp_Get_DanhSachPhieuNhap_Chitiet), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachHoaDon")]
  public async Task<IActionResult> Sp_Get_DanhSachHoaDon(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ct_HoaDon>(nameof (Sp_Get_DanhSachHoaDon), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachHoaDon_Chitiet")]
  public async Task<IActionResult> Sp_Get_DanhSachHoaDon_Chitiet(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ct_HoaDon_ChiTiet>(nameof (Sp_Get_DanhSachHoaDon_Chitiet), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuDatHangNCC")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuDatHangNCC(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ct_PhieuDatHangNCC>(nameof (Sp_Get_DanhSachPhieuDatHangNCC), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuDatHangNCC_Chitiet")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuDatHangNCC_Chitiet(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ct_PhieuDatHangNCC_ChiTiet>(nameof (Sp_Get_DanhSachPhieuDatHangNCC_Chitiet), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachChamCong")]
  public async Task<IActionResult> Sp_Get_DanhSachChamCong(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_nv_ChamCong>(nameof (Sp_Get_DanhSachChamCong), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachNghiPhep")]
  public async Task<IActionResult> Sp_Get_DanhSachNghiPhep(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_nv_NghiPhep>(nameof (Sp_Get_DanhSachNghiPhep), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuDatHang")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuDatHang(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ct_PhieuDatHang>(nameof (Sp_Get_DanhSachPhieuDatHang), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuDatHang_ChiTiet")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuDatHang_ChiTiet(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ct_PhieuDatHang_ChiTiet>(nameof (Sp_Get_DanhSachPhieuDatHang_ChiTiet), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_BaoCaoPhieuDatHang")]
  public async Task<IActionResult> Sp_Get_BaoCaoPhieuDatHang(SP_Parameter_Report sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter_Report(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ct_PhieuDatHang_ChiTiet_BaoCao>("Sp_Get_DanhSachPhieuDatHang_ChiTiet_BaoCao", procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuNhap_ChiTiet_BaoCao")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuNhap_ChiTiet_BaoCao(
    SP_Parameter_Report sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter_Report(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ct_PhieuDatHang_ChiTiet_BaoCao>(nameof (Sp_Get_DanhSachPhieuNhap_ChiTiet_BaoCao), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuNhapTraHang_ChiTiet_BaoCao")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuNhapTraHang_ChiTiet_BaoCao(
    SP_Parameter_Report sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter_Report(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ct_PhieuDatHang_ChiTiet_BaoCao>(nameof (Sp_Get_DanhSachPhieuNhapTraHang_ChiTiet_BaoCao), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuGiaoHang_ChiTiet_BaoCao")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuGiaoHang_ChiTiet_BaoCao(
    SP_Parameter_Report sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter_Report(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ct_PhieuDatHang_ChiTiet_BaoCao>(nameof (Sp_Get_DanhSachPhieuGiaoHang_ChiTiet_BaoCao), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuXuat_PhieuGiaoHang")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuXuat_PhieuGiaoHang(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ct_PhieuXuat>(nameof (Sp_Get_DanhSachPhieuXuat_PhieuGiaoHang), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuXuat")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuXuat(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ct_PhieuXuat>(nameof (Sp_Get_DanhSachPhieuXuat), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuXuat_TimKiem")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuXuat_TimKiem(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ct_PhieuXuat>(nameof (Sp_Get_DanhSachPhieuXuat_TimKiem), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuXuat_ChiTiet")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuXuat_Chitiet(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ct_PhieuXuat_ChiTiet>("Sp_Get_DanhSachPhieuXuat_ChiTiet", procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuGiaoHang_In")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuXuat_In(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<Sp_Get_DanhSachPhieuGiaoHang_In>("Sp_Get_DanhSachPhieuGiaoHang_In", procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuGiaoHang_InPhieuGiao")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuGiaoHang_InPhieuGiao(
    SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<Sp_Get_DanhSachPhieuGiaoHang_In>(nameof (Sp_Get_DanhSachPhieuGiaoHang_InPhieuGiao), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuGiaoHang")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuGiaoHang(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ct_PhieuGiaoHang>(nameof (Sp_Get_DanhSachPhieuGiaoHang), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuGiaoHang_ChiTiet")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuGiaoHang_ChiTiet(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ct_PhieuGiaoHang_ChiTiet>(nameof (Sp_Get_DanhSachPhieuGiaoHang_ChiTiet), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuGiaoHang_NhanVienGiao")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuGiaoHang_NhanVienGiao(
    SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ct_PhieuGiaoHang_NhanVienGiao>(nameof (Sp_Get_DanhSachPhieuGiaoHang_NhanVienGiao), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuGiaoHang_PhieuXuat")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuGiaoHang_PhieuXuat(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ct_PhieuGiaoHang>(nameof (Sp_Get_DanhSachPhieuGiaoHang_PhieuXuat), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuThu")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuThu(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ct_PhieuThu>(nameof (Sp_Get_DanhSachPhieuThu), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuThu_PhieuGiaoHang")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuThu_PhieuGiaoHang(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ct_PhieuThu>(nameof (Sp_Get_DanhSachPhieuThu_PhieuGiaoHang), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuChi")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuChi(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ct_PhieuChi>(nameof (Sp_Get_DanhSachPhieuChi), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuChi_PhieuGiaoHang")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuChi_PhieuGiaoHang(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ct_PhieuChi>(nameof (Sp_Get_DanhSachPhieuChi_PhieuGiaoHang), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_ChuongTrinhKhuyenMai")]
  public async Task<IActionResult> Sp_Get_ChuongTrinhKhuyenMai(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_dm_ChuongTrinhKhuyenMai>(nameof (Sp_Get_ChuongTrinhKhuyenMai), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuXuatHang_KhuyenMai")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuXuatHang_KhuyenMai(
    SP_Parameter_Report sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter_Report(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<DatabaseTHP.StoredProcedure.Sp_Get_DanhSachPhieuXuatHang_KhuyenMai>(nameof (Sp_Get_DanhSachPhieuXuatHang_KhuyenMai), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuGiaoHang_KPI")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuGiaoHang_KPI(SP_Parameter_Report sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter_Report(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<Sp_Get_DanhSachPhieuGiaoHang_KPI_Result>(nameof (Sp_Get_DanhSachPhieuGiaoHang_KPI), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("SP_GetReport")]
  public async Task<IActionResult> Sp_GetReport(SP_Parameter_Report sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter_Report(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc_DataTable<DataTable>(sp_Parameter.NAME_SP, procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_ThongKeCongNoKhachHang")]
  public async Task<IActionResult> Sp_Get_ThongKeCongNoKhachHang(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ThongKeCongNoKhachHang>(nameof (Sp_Get_ThongKeCongNoKhachHang), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_ThongKeCongNoNhaCungCap")]
  public async Task<IActionResult> Sp_Get_ThongKeCongNoNhaCungCap(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ThongKeCongNoNhaCungCap>(nameof (Sp_Get_ThongKeCongNoNhaCungCap), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_ThongKeCongNoNhanVien")]
  public async Task<IActionResult> Sp_Get_ThongKeCongNoNhanVien(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ThongKeCongNoNhanVien>(nameof (Sp_Get_ThongKeCongNoNhanVien), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_ThongKeTonKhoHangHoa")]
  public async Task<IActionResult> Sp_Get_ThongKeTonKhoHangHoa(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ThongKeTonKhoHangHoa>(nameof (Sp_Get_ThongKeTonKhoHangHoa), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_ThongKeThuChi_GroupBy")]
  public async Task<IActionResult> Sp_Get_ThongKeThuChi_GroupBy(SP_Parameter_Report sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter_Report(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<Sp_Get_ThongKeThuChi_Result>(nameof (Sp_Get_ThongKeThuChi_GroupBy), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_ThongKeThuChi")]
  public async Task<IActionResult> Sp_Get_ThongKeThuChi(SP_Parameter_Report sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter_Report(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<Sp_Get_ThongKeThuChi_Result>(nameof (Sp_Get_ThongKeThuChi), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuDatHang_ChiTiet_KPI")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuDatHang_ChiTiet_KPI(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<DanhSachPhieuDatHang_ChiTiet_KPI>(nameof (Sp_Get_DanhSachPhieuDatHang_ChiTiet_KPI), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachPhieuTraHang_ChiTiet_KPI")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuTraHang_ChiTiet_KPI(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<DanhSachPhieuTraHang_ChiTiet_KPI>(nameof (Sp_Get_DanhSachPhieuTraHang_ChiTiet_KPI), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachHangHoa")]
  public async Task<IActionResult> Sp_Get_DanhSachHangHoa(SP_Parameter_Report sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter_Report(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<DatabaseTHP.StoredProcedure.Sp_Get_DanhSachHangHoa>(nameof (Sp_Get_DanhSachHangHoa), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachHangHoa_NhanVien")]
  public async Task<IActionResult> Sp_Get_DanhSachHangHoa_NhanVien(SP_Parameter_Report sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter_Report(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<Sp_Get_DanhSachHangHoa_Result>(nameof (Sp_Get_DanhSachHangHoa_NhanVien), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachHangHoa_KhachHang")]
  public async Task<IActionResult> Sp_Get_DanhSachHangHoa_KhachHang(SP_Parameter_Report sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter_Report(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<Sp_Get_DanhSachHangHoa_Result>(nameof (Sp_Get_DanhSachHangHoa_KhachHang), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachHangHoa_BanChay")]
  public async Task<IActionResult> Sp_Get_DanhSachHangHoa_BanChay(SP_Parameter_Report sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter_Report(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<Sp_Get_DanhSachHangHoa_Result>(nameof (Sp_Get_DanhSachHangHoa_BanChay), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_ThongKeQuyTien")]
  public async Task<IActionResult> Sp_Get_ThongKeQuyTien(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<v_ThongKeQuyTien>(nameof (Sp_Get_ThongKeQuyTien), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  [HttpPost("Sp_Get_DanhSachBangLuong")]
  public async Task<IActionResult> Sp_Get_DanhSachBangLuong(SP_Parameter sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<Sp_Get_DanhSachBangLuong_Result>(nameof (Sp_Get_DanhSachBangLuong), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  public async Task<ApiResponse> Execute_StoredProc<T>(
    string storedProcName,
    Dictionary<string, object> procParams)
    where T : class
  {
    ApiResponse apiResponse = new ApiResponse();
    List<T> objList = new List<T>();
    DbConnection conn = this._context.Database.GetDbConnection();
    try
    {
      if (conn.State != ConnectionState.Open)
        conn.Open();
      await using (DbCommand command = conn.CreateCommand())
      {
        command.CommandTimeout = 120;
        command.CommandText = storedProcName;
        command.CommandType = CommandType.StoredProcedure;
        foreach (KeyValuePair<string, object> procParam1 in procParams)
        {
          KeyValuePair<string, object> procParam = procParam1;
          DbParameter param = command.CreateParameter();
          param.ParameterName = procParam.Key;
          param.Value = procParam.Value;
          command.Parameters.Add((object) param);
          param = (DbParameter) null;
          procParam = new KeyValuePair<string, object>();
        }
        DbDataReader reader = await command.ExecuteReaderAsync();
        IEnumerable<PropertyInfo> props = typeof (T).GetRuntimeProperties();
        Dictionary<string, DbColumn> colMapping = reader.GetColumnSchema().Where<DbColumn>((Func<DbColumn, bool>) (x => props.Any<PropertyInfo>((Func<PropertyInfo, bool>) (y => y.Name.ToLower() == x.ColumnName.ToLower())))).ToDictionary<DbColumn, string>((Func<DbColumn, string>) (key => key.ColumnName.ToLower()));
        if (reader.HasRows)
        {
          while (true)
          {
            if (await reader.ReadAsync())
            {
              if (colMapping != null && colMapping.Count > 0)
              {
                T obj = Activator.CreateInstance<T>();
                foreach (PropertyInfo prop in props)
                {
                  if (colMapping.ContainsKey(prop.Name.ToLower()))
                  {
                    object val = reader.GetValue(colMapping[prop.Name.ToLower()].ColumnOrdinal.Value);
                    if (val != null)
                    {
                      if (prop.PropertyType.Name.ToUpper().Contains("STRING"))
                        prop.SetValue((object) obj, val == DBNull.Value ? (object) "" : val);
                      else
                        prop.SetValue((object) obj, val == DBNull.Value ? (object) null : val);
                    }
                    else if (prop.PropertyType.Name.ToUpper().Contains("STRING"))
                      prop.SetValue((object) obj, (object) "");
                    val = (object) null;
                  }
                  else if (prop.PropertyType.Name.ToUpper().Contains("STRING"))
                    prop.SetValue((object) obj, (object) "");
                }
                objList.Add(obj);
                obj = default (T);
              }
            }
            else
              break;
          }
        }
        reader.Dispose();
        reader = (DbDataReader) null;
        colMapping = (Dictionary<string, DbColumn>) null;
      }
      apiResponse = new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) objList
      };
    }
    catch (Exception ex)
    {
      objList = new List<T>();
      apiResponse = new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) objList
      };
    }
    finally
    {
      conn.Close();
    }
    ApiResponse apiResponse1 = apiResponse;
    apiResponse = (ApiResponse) null;
    objList = (List<T>) null;
    conn = (DbConnection) null;
    return apiResponse1;
  }

  public async Task<ApiResponse> Execute_StoredProc_DataTable<T>(
    string storedProcName,
    Dictionary<string, object> procParams)
    where T : class
  {
    ApiResponse apiResponse = new ApiResponse();
    DbConnection conn = this._context.Database.GetDbConnection();
    DataTable data = new DataTable();
    bool CheckValue = false;
    try
    {
      if (conn.State != ConnectionState.Open)
        conn.Open();
      await using (DbCommand command = conn.CreateCommand())
      {
        command.CommandText = storedProcName;
        command.CommandType = CommandType.StoredProcedure;
        foreach (KeyValuePair<string, object> procParam1 in procParams)
        {
          KeyValuePair<string, object> procParam = procParam1;
          DbParameter param = command.CreateParameter();
          param.ParameterName = procParam.Key;
          param.Value = procParam.Value;
          command.Parameters.Add((object) param);
          param = (DbParameter) null;
          procParam = new KeyValuePair<string, object>();
        }
        DbDataReader reader = await command.ExecuteReaderAsync();
        data.Load((IDataReader) reader);
        if (data != null && data.Rows.Count == 0)
        {
          List<string> lstDataColumn = new List<string>();
          foreach (DataColumn cl in (InternalDataCollectionBase) data.Columns)
            lstDataColumn.Add(cl.ColumnName);
          data.Columns.Clear();
          foreach (string cl in lstDataColumn)
            data.Columns.Add(cl, typeof (string));
          DataRow dr = data.NewRow();
          data.Rows.Add(dr);
          CheckValue = true;
          lstDataColumn = (List<string>) null;
          dr = (DataRow) null;
        }
        reader.Dispose();
        reader = (DbDataReader) null;
      }
      apiResponse = new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) data,
        CheckValue = CheckValue
      };
    }
    catch (Exception ex)
    {
      data = new DataTable();
      apiResponse = new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) data
      };
    }
    finally
    {
      conn.Close();
    }
    ApiResponse apiResponse1 = apiResponse;
    apiResponse = (ApiResponse) null;
    conn = (DbConnection) null;
    data = (DataTable) null;
    return apiResponse1;
  }

  private bool IsNumericType(Type type)
  {
    switch (Type.GetTypeCode(type))
    {
      case TypeCode.SByte:
      case TypeCode.Byte:
      case TypeCode.Int16:
      case TypeCode.UInt16:
      case TypeCode.Int32:
      case TypeCode.UInt32:
      case TypeCode.Int64:
      case TypeCode.UInt64:
      case TypeCode.Single:
      case TypeCode.Double:
      case TypeCode.Decimal:
        return true;
      default:
        return false;
    }
  }

  public async Task<ApiResponse> CheckDelete<T>(T Data, string ID, string MA) where T : class
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    ExecuteStoredProc.\u003C\u003Ec__DisplayClass69_0<T> cDisplayClass690 = new ExecuteStoredProc.\u003C\u003Ec__DisplayClass69_0<T>();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass690.Data = Data;
    string sqlQuery = "";
    string Error = "";
    ApiResponse apiResponse = new ApiResponse();
    DbConnection conn = this._context.Database.GetDbConnection();
    try
    {
      ParameterExpression parameterExpression;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method reference
      // ISSUE: method reference
      List<web_NoteClass> lstNoteClass = await this._context.web_NoteClass.Where<web_NoteClass>(Expression.Lambda<Func<web_NoteClass, bool>>((Expression) Expression.Equal(e.FOREIGNKEY, (Expression) Expression.Property((Expression) Expression.Call(cDisplayClass690.Data, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (object.GetType)), Array.Empty<Expression>()), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (MemberInfo.get_Name)))), parameterExpression)).ToListAsync<web_NoteClass>();
      if (lstNoteClass != null && lstNoteClass.Count > 0)
      {
        foreach (web_NoteClass webNoteClass in lstNoteClass)
        {
          // ISSUE: object of a compiler-generated type is created
          // ISSUE: variable of a compiler-generated type
          ExecuteStoredProc.\u003C\u003Ec__DisplayClass69_1<T> cDisplayClass691 = new ExecuteStoredProc.\u003C\u003Ec__DisplayClass69_1<T>();
          // ISSUE: reference to a compiler-generated field
          cDisplayClass691.note = webNoteClass;
          try
          {
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            if (!(cDisplayClass691.note.NAMECLASS != null ? cDisplayClass691.note.NAMECLASS : "").ToUpper().Contains("PHANQUYEN"))
            {
              int totalRow = 0;
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              sqlQuery = $"SELECT COUNT(ID) FROM {cDisplayClass691.note.NAMECLASS} WHERE {cDisplayClass691.note.NAMECOLUMN} = '{ID}'";
              if (conn.State != ConnectionState.Open)
                conn.Open();
              await using (DbCommand command = conn.CreateCommand())
              {
                command.CommandText = sqlQuery;
                command.CommandType = CommandType.Text;
                DbDataReader wantedRow = command.ExecuteReader();
                while (wantedRow.Read())
                  totalRow = Convert.ToInt32(wantedRow[0].ToString());
                wantedRow = (DbDataReader) null;
              }
              conn.Close();
              if (totalRow > 0)
              {
                // ISSUE: reference to a compiler-generated field
                web_NoteTable web_NoteTable = await this._context.web_NoteTable.FirstOrDefaultAsync<web_NoteTable>((Expression<Func<web_NoteTable, bool>>) (e => e.NAMECLASS == cDisplayClass691.note.NAMECLASS));
                // ISSUE: reference to a compiler-generated field
                Error = $"{Error}Không thể xóa '{MA}' do dữ liệu đang liên kết tới '{(web_NoteTable == null || string.IsNullOrEmpty(web_NoteTable.NOTE) ? cDisplayClass691.note.NAMECLASS : web_NoteTable.NOTE)}'!{Environment.NewLine}";
                web_NoteTable = (web_NoteTable) null;
              }
            }
            else
              continue;
          }
          catch
          {
            if (conn.State == ConnectionState.Open)
              conn.Close();
          }
          cDisplayClass691 = (ExecuteStoredProc.\u003C\u003Ec__DisplayClass69_1<T>) null;
        }
      }
      lstNoteClass = (List<web_NoteClass>) null;
    }
    catch (Exception ex)
    {
      apiResponse = new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      };
    }
    finally
    {
      apiResponse = new ApiResponse()
      {
        Success = string.IsNullOrEmpty(Error),
        Message = Error,
        Data = (object) ""
      };
    }
    ApiResponse apiResponse1 = apiResponse;
    cDisplayClass690 = (ExecuteStoredProc.\u003C\u003Ec__DisplayClass69_0<T>) null;
    sqlQuery = (string) null;
    Error = (string) null;
    apiResponse = (ApiResponse) null;
    conn = (DbConnection) null;
    return apiResponse1;
  }

  public static Dictionary<string, object> GetSP_Parameter_Report(SP_Parameter_Report sp_Parameter)
  {
    Dictionary<string, object> spParameterReport = new Dictionary<string, object>();
    foreach (PropertyInfo property in sp_Parameter.GetType().GetProperties())
    {
      if (property.Name != "NAME_SP" && property.Name != "ID_REPORT" && property.Name != "HINHTHUC" && property.Name != "HINHTHUC_BAOCAOTAICHINH" && property.Name != "HINHTHUC_PHIEUXUATHANG_KHUYENMAI" && property.Name != "ISDETAIL")
      {
        object obj = property.GetValue((object) sp_Parameter) ?? (object) DBNull.Value;
        if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
          spParameterReport.Add(property.Name, obj);
      }
    }
    return spParameterReport;
  }

  public static Dictionary<string, object> GetSP_Parameter(SP_Parameter sp_Parameter)
  {
    Dictionary<string, object> spParameter = new Dictionary<string, object>();
    if (sp_Parameter.LOC_ID != null)
      spParameter.Add("LOC_ID", (object) sp_Parameter.LOC_ID);
    if (sp_Parameter.ID_NHANVIEN != null)
      spParameter.Add("ID_NHANVIEN", (object) sp_Parameter.ID_NHANVIEN);
    if (sp_Parameter.ID_NHOMQUYEN != null)
      spParameter.Add("ID_NHOMQUYEN", (object) sp_Parameter.ID_NHOMQUYEN);
    if (sp_Parameter.ID_NHOMHANGHOA != null)
      spParameter.Add("ID_NHOMHANGHOA", (object) sp_Parameter.ID_NHOMHANGHOA);
    if (sp_Parameter.KEY != null)
      spParameter.Add("KEY", (object) sp_Parameter.KEY);
    if (sp_Parameter.ID_HANGHOA != null)
      spParameter.Add("ID_HANGHOA", (object) sp_Parameter.ID_HANGHOA);
    if (sp_Parameter.ID_KHUVUC != null)
      spParameter.Add("ID_KHUVUC", (object) sp_Parameter.ID_KHUVUC);
    if (sp_Parameter.ID_KHO != null)
      spParameter.Add("ID_KHO", (object) sp_Parameter.ID_KHO);
    if (sp_Parameter.ID_HANGHOAKHO != null)
      spParameter.Add("ID_HANGHOAKHO", (object) sp_Parameter.ID_HANGHOAKHO);
    if (sp_Parameter.TUNGAY.HasValue)
      spParameter.Add("TUNGAY", (object) sp_Parameter.TUNGAY);
    if (sp_Parameter.DENNGAY.HasValue)
      spParameter.Add("DENNGAY", (object) sp_Parameter.DENNGAY);
    if (sp_Parameter.ID_PHIEUNHAP != null)
      spParameter.Add("ID_PHIEUNHAP", (object) sp_Parameter.ID_PHIEUNHAP);
    if (sp_Parameter.ID_PHIEUXUAT != null)
      spParameter.Add("ID_PHIEUXUAT", (object) sp_Parameter.ID_PHIEUXUAT);
    if (sp_Parameter.ID_PHIEUCHI != null)
      spParameter.Add("ID_PHIEUCHI", (object) sp_Parameter.ID_PHIEUCHI);
    if (sp_Parameter.ID_PHIEUTHU != null)
      spParameter.Add("ID_PHIEUTHU", (object) sp_Parameter.ID_PHIEUTHU);
    if (sp_Parameter.ID_PHIEUDATHANG != null)
      spParameter.Add("ID_PHIEUDATHANG", (object) sp_Parameter.ID_PHIEUDATHANG);
    if (sp_Parameter.ID_PHIEUGIAOHANG != null)
      spParameter.Add("ID_PHIEUGIAOHANG", (object) sp_Parameter.ID_PHIEUGIAOHANG);
    if (sp_Parameter.ID_COMBO != null)
      spParameter.Add("ID_COMBO", (object) sp_Parameter.ID_COMBO);
    if (sp_Parameter.BOLTONKHO.HasValue)
      spParameter.Add("BOLTONKHO", (object) sp_Parameter.BOLTONKHO);
    if (sp_Parameter.ID_KHACHHANG != null)
      spParameter.Add("ID_KHACHHANG", (object) sp_Parameter.ID_KHACHHANG);
    if (sp_Parameter.ISCHITIET.HasValue)
      spParameter.Add("ISCHITIET", (object) sp_Parameter.ISCHITIET);
    if (sp_Parameter.ID_NHACUNGCAP != null)
      spParameter.Add("ID_NHACUNGCAP", (object) sp_Parameter.ID_NHACUNGCAP);
    if (sp_Parameter.ID_NHOMKHACHHANG != null)
      spParameter.Add("ID_NHOMKHACHHANG", (object) sp_Parameter.ID_NHOMKHACHHANG);
    if (sp_Parameter.ID_NHOMNCC != null)
      spParameter.Add("ID_NHOMNCC", (object) sp_Parameter.ID_NHOMNCC);
    if (sp_Parameter.ISTHEOTHOIGIAN.HasValue)
      spParameter.Add("ISTHEOTHOIGIAN", (object) sp_Parameter.ISTHEOTHOIGIAN);
    if (sp_Parameter.ISPHATSINHCONGNO.HasValue)
      spParameter.Add("ISPHATSINHCONGNO", (object) sp_Parameter.ISPHATSINHCONGNO);
    if (sp_Parameter.ISPHATSINHCONGNOTRONGKY.HasValue)
      spParameter.Add("ISPHATSINHCONGNOTRONGKY", (object) sp_Parameter.ISPHATSINHCONGNOTRONGKY);
    if (sp_Parameter.ISCONCONGNO.HasValue)
      spParameter.Add("ISCONCONGNO", (object) sp_Parameter.ISCONCONGNO);
    if (sp_Parameter.LOAITIMKIEM != null)
      spParameter.Add("LOAITIMKIEM", (object) sp_Parameter.LOAITIMKIEM);
    if (sp_Parameter.THU != null)
      spParameter.Add("THU", (object) ExecuteStoredProc.GetThu(sp_Parameter.THU));
    if (sp_Parameter.ID_PHONGBAN != null)
      spParameter.Add("ID_PHONGBAN", (object) sp_Parameter.ID_PHONGBAN);
    if (sp_Parameter.SOLAN.HasValue)
      spParameter.Add("SOLAN", (object) sp_Parameter.SOLAN);
    if (sp_Parameter.ID_XE != null)
      spParameter.Add("ID_XE", (object) sp_Parameter.ID_XE);
    if (sp_Parameter.ID_TAIKHOANNGANHANG != null)
      spParameter.Add("ID_TAIKHOANNGANHANG", (object) sp_Parameter.ID_TAIKHOANNGANHANG);
    if (sp_Parameter.NGAYCONG.HasValue)
      spParameter.Add("NGAYCONG", (object) sp_Parameter.NGAYCONG);
    if (sp_Parameter.ID_HOADON != null)
      spParameter.Add("ID_HOADON", (object) sp_Parameter.ID_HOADON);
    return spParameter;
  }

  [HttpPost("Sp_Get_DanhSachPhieuXuat_ChiTiet_BC")]
  public async Task<IActionResult> Sp_Get_DanhSachPhieuXuat_ChiTiet_BC(
    SP_Parameter_Report sp_Parameter)
  {
    try
    {
      Dictionary<string, object> procParams = ExecuteStoredProc.GetSP_Parameter_Report(sp_Parameter);
      ApiResponse apiResponse = await this.Execute_StoredProc<Sp_Get_DanhSachPhieuXuat_ChiTiet_Result>(nameof (Sp_Get_DanhSachPhieuXuat_ChiTiet_BC), procParams);
      return (IActionResult) this.Ok((object) apiResponse);
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

  private static string GetThu(string THU = "")
  {
    if (THU == "-1")
      return THU;
    switch (DateTime.Now.DayOfWeek)
    {
      case DayOfWeek.Sunday:
        THU = "CN";
        break;
      case DayOfWeek.Monday:
        THU = "T2";
        break;
      case DayOfWeek.Tuesday:
        THU = "T3";
        break;
      case DayOfWeek.Wednesday:
        THU = "T4";
        break;
      case DayOfWeek.Thursday:
        THU = "T5";
        break;
      case DayOfWeek.Friday:
        THU = "T6";
        break;
      case DayOfWeek.Saturday:
        THU = "T7";
        break;
    }
    return THU;
  }

  [HttpPost("Insert_Customer_Map")]
  public async Task<ApiResponse> Insert_Customer_Map(view_dm_KhachHang KhachHang)
  {
    ApiResponse apiResponse = new ApiResponse();
    DbConnection conn = this._context.Database.GetDbConnection();
    try
    {
      if (conn.State != ConnectionState.Open)
        conn.Open();
      await using (DbCommand command = conn.CreateCommand())
      {
        command.CommandText = nameof (Insert_Customer_Map);
        command.CommandType = CommandType.StoredProcedure;
        DbParameter param = command.CreateParameter();
        param.ParameterName = "ID";
        param.Value = (object) KhachHang.ID;
        command.Parameters.Add((object) param);
        param = command.CreateParameter();
        param.ParameterName = "CONTENT_MAP";
        param.Value = (object) KhachHang.CONTENT_MAP;
        command.Parameters.Add((object) param);
        if (KhachHang.LATITUDE.HasValue)
        {
          param = command.CreateParameter();
          param.ParameterName = "LATITUDE";
          param.Value = (object) KhachHang.LATITUDE;
          command.Parameters.Add((object) param);
        }
        if (KhachHang.LONGITUDE.HasValue)
        {
          param = command.CreateParameter();
          param.ParameterName = "LONGITUDE";
          param.Value = (object) KhachHang.LONGITUDE;
          command.Parameters.Add((object) param);
        }
        int reader = await command.ExecuteNonQueryAsync();
        param = (DbParameter) null;
      }
      apiResponse = new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) null
      };
    }
    catch (Exception ex)
    {
      apiResponse = new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) null
      };
    }
    finally
    {
      conn.Close();
    }
    ApiResponse apiResponse1 = apiResponse;
    apiResponse = (ApiResponse) null;
    conn = (DbConnection) null;
    return apiResponse1;
  }
}
