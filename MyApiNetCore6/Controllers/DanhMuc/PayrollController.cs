// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.PayrollController
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.StoredProcedure;
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


namespace MyApiNetCore6.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PayrollController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public PayrollController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetArea(string LOC_ID)
  {
    try
    {
      List<view_nv_BangLuong> lstValue = await this._context.view_nv_BangLuong.Where<view_nv_BangLuong>((Expression<Func<view_nv_BangLuong, bool>>) (e => e.LOC_ID == LOC_ID)).OrderBy<view_nv_BangLuong, double>((Expression<Func<view_nv_BangLuong, double>>) (e => e.NAMTHANG_ORDERBY)).ToListAsync<view_nv_BangLuong>();
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
  public async Task<IActionResult> GetArea(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<view_nv_BangLuong> lstValue = await this._context.view_nv_BangLuong.Where<view_nv_BangLuong>((Expression<Func<view_nv_BangLuong, bool>>) (e => e.LOC_ID == LOC_ID)).Where<view_nv_BangLuong>(KeyWhere, (object) ValuesSearch).OrderBy<view_nv_BangLuong, double>((Expression<Func<view_nv_BangLuong, double>>) (e => e.NAMTHANG_ORDERBY)).ToListAsync<view_nv_BangLuong>();
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
  public async Task<IActionResult> GetArea(string LOC_ID, string ID)
  {
    try
    {
      view_nv_BangLuong Area = await this._context.view_nv_BangLuong.FirstOrDefaultAsync<view_nv_BangLuong>((Expression<Func<view_nv_BangLuong, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Area == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      v_nv_BangLuong ct_PhieuDatHang = new v_nv_BangLuong();
      if (Area != null)
      {
        string strDeposit = JsonConvert.SerializeObject((object) Area);
        ct_PhieuDatHang = JsonConvert.DeserializeObject<v_nv_BangLuong>(strDeposit) ?? new v_nv_BangLuong();
        strDeposit = (string) null;
      }
      ct_PhieuDatHang.lstnv_BangLuong_ChiTiet = new List<nv_BangLuong_ChiTiet>();
      List<nv_BangLuong_ChiTiet> lstValue = await this._context.nv_BangLuong_ChiTiet.Where<nv_BangLuong_ChiTiet>((Expression<Func<nv_BangLuong_ChiTiet, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_BANGLUONG == ID)).ToListAsync<nv_BangLuong_ChiTiet>();
      if (lstValue != null)
        ct_PhieuDatHang.lstnv_BangLuong_ChiTiet = lstValue;
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) ct_PhieuDatHang
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

  [HttpPut("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PutArea(v_nv_BangLuong Area)
  {
    try
    {
      view_nv_BangLuong Area1 = await this._context.view_nv_BangLuong.FirstOrDefaultAsync<view_nv_BangLuong>((Expression<Func<view_nv_BangLuong, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID_THANGLUONG == Area.ID_THANGLUONG && e.ID != Area.ID));
      if (Area1 != null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại tháng lương {Area.LOC_ID}-{Area.ID_THANGLUONG} dữ liệu bảng lương!",
          Data = (object) ""
        });
      List<nv_BangLuong_ChiTiet> lstValueChiTiet = await this._context.nv_BangLuong_ChiTiet.AsNoTracking<nv_BangLuong_ChiTiet>().Where<nv_BangLuong_ChiTiet>((Expression<Func<nv_BangLuong_ChiTiet, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID_BANGLUONG == Area.ID)).ToListAsync<nv_BangLuong_ChiTiet>();
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        foreach (nv_BangLuong_ChiTiet bangLuongChiTiet in lstValueChiTiet)
        {
          nv_BangLuong_ChiTiet itm = bangLuongChiTiet;
          if (Area.lstnv_BangLuong_ChiTiet.Count<nv_BangLuong_ChiTiet>((Func<nv_BangLuong_ChiTiet, bool>) (s => s.ID == itm.ID)) <= 0)
            this._context.nv_BangLuong_ChiTiet.Remove(itm);
        }
        foreach (nv_BangLuong_ChiTiet bangLuongChiTiet in Area.lstnv_BangLuong_ChiTiet)
        {
          nv_BangLuong_ChiTiet itm = bangLuongChiTiet;
          if (lstValueChiTiet.Count<nv_BangLuong_ChiTiet>((Func<nv_BangLuong_ChiTiet, bool>) (s => s.ID == itm.ID)) <= 0)
          {
            itm.ID_BANGLUONG = Area.ID;
            this._context.nv_BangLuong_ChiTiet.Add(itm);
          }
          else
            this._context.Entry<nv_BangLuong_ChiTiet>(itm).State = EntityState.Modified;
        }
        Area.TIENLUONG = Area.lstnv_BangLuong_ChiTiet.Where<nv_BangLuong_ChiTiet>((Func<nv_BangLuong_ChiTiet, bool>) (s => Convert.ToInt32(s.TYPE) > 0)).Sum<nv_BangLuong_ChiTiet>((Func<nv_BangLuong_ChiTiet, double>) (s => s.SOTIEN));
        Area.TIENGIAM = Area.lstnv_BangLuong_ChiTiet.Where<nv_BangLuong_ChiTiet>((Func<nv_BangLuong_ChiTiet, bool>) (s => Convert.ToInt32(s.TYPE) < 0)).Sum<nv_BangLuong_ChiTiet>((Func<nv_BangLuong_ChiTiet, double>) (s => s.SOTIEN));
        Area.TIENTHUCNHAN = Area.TIENLUONG - Area.TIENGIAM;
        this._context.Entry<v_nv_BangLuong>(Area).State = EntityState.Modified;
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        auditLog = (AuditLogController) null;
        transaction.Commit();
        view_nv_BangLuong view_nv_BangLuong = await this._context.view_nv_BangLuong.FirstOrDefaultAsync<view_nv_BangLuong>((Expression<Func<view_nv_BangLuong, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID));
        if (view_nv_BangLuong == null)
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = $"Không tìm thấy {Area.LOC_ID}-{Area.ID} dữ liệu!",
            Data = (object) ""
          });
        v_nv_BangLuong ct_PhieuDatHang = new v_nv_BangLuong();
        if (view_nv_BangLuong != null)
        {
          string strDeposit = JsonConvert.SerializeObject((object) view_nv_BangLuong);
          ct_PhieuDatHang = JsonConvert.DeserializeObject<v_nv_BangLuong>(strDeposit) ?? new v_nv_BangLuong();
          strDeposit = (string) null;
        }
        ct_PhieuDatHang.lstnv_BangLuong_ChiTiet = new List<nv_BangLuong_ChiTiet>();
        List<nv_BangLuong_ChiTiet> lstValue = await this._context.nv_BangLuong_ChiTiet.Where<nv_BangLuong_ChiTiet>((Expression<Func<nv_BangLuong_ChiTiet, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID_BANGLUONG == Area.ID)).ToListAsync<nv_BangLuong_ChiTiet>();
        if (lstValue != null)
          ct_PhieuDatHang.lstnv_BangLuong_ChiTiet = lstValue;
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) ct_PhieuDatHang
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
  public async Task<ActionResult<dm_KhuVuc>> PostArea(v_nv_BangLuong Area)
  {
    try
    {
      view_nv_BangLuong Area1 = await this._context.view_nv_BangLuong.FirstOrDefaultAsync<view_nv_BangLuong>((Expression<Func<view_nv_BangLuong, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID_THANGLUONG == Area.ID_THANGLUONG));
      if (Area1 != null)
        return (ActionResult<dm_KhuVuc>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại tháng lương {Area.LOC_ID}-{Area.ID_THANGLUONG} dữ liệu bảng lương!",
          Data = (object) ""
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        foreach (nv_BangLuong_ChiTiet itm in Area.lstnv_BangLuong_ChiTiet)
          this._context.nv_BangLuong_ChiTiet.Add(itm);
        Area.TIENLUONG = Area.lstnv_BangLuong_ChiTiet.Where<nv_BangLuong_ChiTiet>((Func<nv_BangLuong_ChiTiet, bool>) (s => Convert.ToInt32(s.TYPE) > 0)).Sum<nv_BangLuong_ChiTiet>((Func<nv_BangLuong_ChiTiet, double>) (s => s.SOTIEN));
        Area.TIENGIAM = Area.lstnv_BangLuong_ChiTiet.Where<nv_BangLuong_ChiTiet>((Func<nv_BangLuong_ChiTiet, bool>) (s => Convert.ToInt32(s.TYPE) < 0)).Sum<nv_BangLuong_ChiTiet>((Func<nv_BangLuong_ChiTiet, double>) (s => s.SOTIEN));
        Area.TIENTHUCNHAN = Area.TIENLUONG - Area.TIENGIAM;
        this._context.nv_BangLuong.Add((nv_BangLuong) Area);
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        auditLog = (AuditLogController) null;
        transaction.Commit();
        view_nv_BangLuong view_nv_BangLuong = await this._context.view_nv_BangLuong.FirstOrDefaultAsync<view_nv_BangLuong>((Expression<Func<view_nv_BangLuong, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID));
        if (view_nv_BangLuong == null)
          return (ActionResult<dm_KhuVuc>) (ActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = $"Không tìm thấy {Area.LOC_ID}-{Area.ID} dữ liệu!",
            Data = (object) ""
          });
        v_nv_BangLuong ct_PhieuDatHang = new v_nv_BangLuong();
        if (view_nv_BangLuong != null)
        {
          string strDeposit = JsonConvert.SerializeObject((object) view_nv_BangLuong);
          ct_PhieuDatHang = JsonConvert.DeserializeObject<v_nv_BangLuong>(strDeposit) ?? new v_nv_BangLuong();
          strDeposit = (string) null;
        }
        ct_PhieuDatHang.lstnv_BangLuong_ChiTiet = new List<nv_BangLuong_ChiTiet>();
        List<nv_BangLuong_ChiTiet> lstValue = await this._context.nv_BangLuong_ChiTiet.Where<nv_BangLuong_ChiTiet>((Expression<Func<nv_BangLuong_ChiTiet, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID_BANGLUONG == Area.ID)).ToListAsync<nv_BangLuong_ChiTiet>();
        if (lstValue != null)
          ct_PhieuDatHang.lstnv_BangLuong_ChiTiet = lstValue;
        return (ActionResult<dm_KhuVuc>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) ct_PhieuDatHang
        });
      }
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_KhuVuc>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteArea(string LOC_ID, string ID)
  {
    try
    {
      nv_BangLuong Area = await this._context.nv_BangLuong.FirstOrDefaultAsync<nv_BangLuong>((Expression<Func<nv_BangLuong, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Area == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      List<nv_BangLuong_ChiTiet> lstValueChiTiet = await this._context.nv_BangLuong_ChiTiet.AsNoTracking<nv_BangLuong_ChiTiet>().Where<nv_BangLuong_ChiTiet>((Expression<Func<nv_BangLuong_ChiTiet, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_BANGLUONG == ID)).ToListAsync<nv_BangLuong_ChiTiet>();
      foreach (nv_BangLuong_ChiTiet itm in lstValueChiTiet)
        this._context.nv_BangLuong_ChiTiet.Remove(itm);
      this._context.nv_BangLuong.Remove(Area);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) ""
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

  [HttpPost("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<ActionResult<dm_KhuVuc>> GetLuongThang(v_nv_BangLuong Area, string LOC_ID)
  {
    try
    {
      view_nv_BangLuong Area1 = await this._context.view_nv_BangLuong.FirstOrDefaultAsync<view_nv_BangLuong>((Expression<Func<view_nv_BangLuong, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID_THANGLUONG == Area.ID_THANGLUONG));
      if (Area1 != null)
        return (ActionResult<dm_KhuVuc>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại tháng lương {Area.LOC_ID}-{Area.ID_THANGLUONG} dữ liệu bảng lương!",
          Data = (object) ""
        });
      view_dm_NhanVien NhanVien = await this._context.view_dm_NhanVien.FirstOrDefaultAsync<view_dm_NhanVien>((Expression<Func<view_dm_NhanVien, bool>>) (e => e.ID == Area.ID_NHANVIEN));
      if (NhanVien == null || string.IsNullOrEmpty(NhanVien.ID_TAIKHOAN))
        return (ActionResult<dm_KhuVuc>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = ("Không tìm thấy nhân viên: " + Area.ID_NHANVIEN),
          Data = (object) ""
        });
      dm_PhongBan PhongBan = await this._context.dm_PhongBan.FirstOrDefaultAsync<dm_PhongBan>((Expression<Func<dm_PhongBan, bool>>) (e => e.ID == NhanVien.ID_PHONGBAN));
      if (PhongBan == null)
        return (ActionResult<dm_KhuVuc>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = ("Không tìm thấy phòng ban: " + NhanVien.ID_PHONGBAN),
          Data = (object) ""
        });
      dm_ThangLuong dm_ThangLuong = await this._context.dm_ThangLuong.FirstOrDefaultAsync<dm_ThangLuong>((Expression<Func<dm_ThangLuong, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID_THANGLUONG && ("," + e.ID_PHONGBAN + ",").Contains("," + PhongBan.MA + ",") && e.ISACTIVE));
      if (dm_ThangLuong == null)
        return (ActionResult<dm_KhuVuc>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy danh mục tháng lương: {Area.ID_THANGLUONG}-{PhongBan.MA}",
          Data = (object) ""
        });
      AspNetUsers TaiKhoan = await this._context.AspNetUsers.FirstOrDefaultAsync<AspNetUsers>((Expression<Func<AspNetUsers, bool>>) (e => e.ID == NhanVien.ID_TAIKHOAN));
      if (TaiKhoan == null)
        return (ActionResult<dm_KhuVuc>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = ("Không tìm thấy tài khoản: " + NhanVien.ID_TAIKHOAN),
          Data = (object) ""
        });
      dm_BangLuong vdm_BangLuong = await this._context.dm_BangLuong.FirstOrDefaultAsync<dm_BangLuong>((Expression<Func<dm_BangLuong, bool>>) (e => e.LOC_ID == Area.LOC_ID && (string.IsNullOrEmpty(e.ID_PHONGBAN) || e.ID_PHONGBAN == NhanVien.ID_PHONGBAN)));
      if (vdm_BangLuong == null)
        return (ActionResult<dm_KhuVuc>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = ("Không tìm thấy danh mục bảng lương cho phòng ban: " + NhanVien.ID_PHONGBAN),
          Data = (object) ""
        });
      List<dm_BangLuong_ChiTiet> lstdm_BangLuong_ChiTiet = await this._context.dm_BangLuong_ChiTiet.Where<dm_BangLuong_ChiTiet>((Expression<Func<dm_BangLuong_ChiTiet, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID_BANGLUONG == vdm_BangLuong.ID)).ToListAsync<dm_BangLuong_ChiTiet>();
      string GhiChuNghiPhep = "";
      string GhiChuNghiKhongPhep = "";
      string GhiChuVeSom = "";
      double SoNgayCong = 0.0;
      double SoNgayNghiPhep = 0.0;
      double SoNgayLamViec = 0.0;
      double SoNgayNghiKhongPhep = 0.0;
      double SoNgayCoDiLam = 0.0;
      int num1;
      for (DateTime date = dm_ThangLuong.NGAYBATDAU; date <= dm_ThangLuong.NGAYKETTHUC; date = date.AddDays(1.0))
      {
        bool bolDungPhep = false;
        int num2;
        if (!string.IsNullOrEmpty(dm_ThangLuong.DANHSACHNGAYNGHI))
        {
          string str1 = $",{dm_ThangLuong.DANHSACHNGAYNGHI},";
          num1 = date.Day;
          string str2 = $",{num1.ToString()},";
          num2 = !str1.Contains(str2) ? 1 : 0;
        }
        else
          num2 = 0;
        if (num2 != 0)
        {
          nv_NghiPhep nv_NghiPhep = await this._context.nv_NghiPhep.FirstOrDefaultAsync<nv_NghiPhep>((Expression<Func<nv_NghiPhep, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.THOIGIANRA.Date >= date.Date && e.THOIGIANVAO.Date <= date.Date && e.ID_NHANVIEN == NhanVien.ID));
          if (nv_NghiPhep != null && nv_NghiPhep.ISNGHIPHEP)
          {
            if (string.IsNullOrEmpty(GhiChuNghiPhep))
              GhiChuNghiPhep = "Nghỉ phép: ";
            ++SoNgayCong;
            if (nv_NghiPhep.HINHTHUCNGHIPHEP == 0)
            {
              ++SoNgayNghiPhep;
              string[] strArray = new string[5]
              {
                GhiChuNghiPhep,
                null,
                null,
                null,
                null
              };
              num1 = date.Day;
              strArray[1] = num1.ToString();
              strArray[2] = "(";
              strArray[3] = nv_NghiPhep.ISDUYETPHEP ? "Duyệt" : "Chưa duyệt";
              strArray[4] = " 1 ngày);";
              GhiChuNghiPhep = string.Concat(strArray);
              continue;
            }
            SoNgayNghiPhep += 0.5;
            string[] strArray1 = new string[5]
            {
              GhiChuNghiPhep,
              null,
              null,
              null,
              null
            };
            num1 = date.Day;
            strArray1[1] = num1.ToString();
            strArray1[2] = "(";
            strArray1[3] = nv_NghiPhep.ISDUYETPHEP ? "Duyệt" : "Chưa duyệt";
            strArray1[4] = " 0.5 ngày);";
            GhiChuNghiPhep = string.Concat(strArray1);
            bolDungPhep = true;
          }
          nv_ChamCong nv_ChamCong = await this._context.nv_ChamCong.FirstOrDefaultAsync<nv_ChamCong>((Expression<Func<nv_ChamCong, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.NGAYCONG.Date == date.Date));
          if (nv_ChamCong != null)
          {
            TimeSpan GIOBATDAU = dm_ThangLuong.GIOBATDAU;
            TimeSpan GIOKETTHUC = dm_ThangLuong.GIOKETTHUC;
            double SoGioLe = 0.0;
            DateTime? nullable = nv_ChamCong.THOIGIANVAO;
            int num3;
            if (nullable.HasValue)
            {
              nullable = nv_ChamCong.THOIGIANRA;
              num3 = nullable.HasValue ? 1 : 0;
            }
            else
              num3 = 0;
            if (num3 != 0)
            {
              nullable = nv_ChamCong.THOIGIANVAO;
              if (nullable.HasValue)
              {
                nullable = nv_ChamCong.THOIGIANVAO;
                if (nullable.Value.TimeOfDay > dm_ThangLuong.GIOBATDAU)
                {
                  nullable = nv_ChamCong.THOIGIANVAO;
                  GIOBATDAU = nullable.Value.TimeOfDay;
                  if (!bolDungPhep)
                  {
                    if (string.IsNullOrEmpty(GhiChuVeSom))
                      GhiChuVeSom = "Đi trễ, về sớm: ";
                    string str3 = GhiChuVeSom;
                    nullable = nv_ChamCong.THOIGIANVAO;
                    string str4 = nullable.Value.ToString("dd HH:mm");
                    GhiChuVeSom = $"{str3}(v1) {str4};";
                  }
                }
                else
                  SoGioLe = 1.0;
              }
              nullable = nv_ChamCong.THOIGIANRA;
              if (nullable.HasValue)
              {
                nullable = nv_ChamCong.THOIGIANRA;
                if (nullable.Value.TimeOfDay < dm_ThangLuong.GIOKETTHUC)
                {
                  nullable = nv_ChamCong.THOIGIANRA;
                  GIOKETTHUC = nullable.Value.TimeOfDay;
                  if (!bolDungPhep)
                  {
                    if (string.IsNullOrEmpty(GhiChuVeSom))
                      GhiChuVeSom = "Đi trễ, về sớm: ";
                    string str5 = GhiChuVeSom;
                    nullable = nv_ChamCong.THOIGIANRA;
                    string str6 = nullable.Value.ToString("dd HH:mm");
                    GhiChuVeSom = $"{str5}(v2) {str6};";
                  }
                }
                else
                  SoGioLe = 1.0;
              }
              TimeSpan timeSpan = GIOKETTHUC - GIOBATDAU;
              double SoGioLamViec = timeSpan.TotalHours;
              TimeSpan SoTiengLamTrongNgay = dm_ThangLuong.GIOKETTHUC - dm_ThangLuong.GIOBATDAU;
              timeSpan = dm_ThangLuong.GIOKETTHUC_NGHITRUA - dm_ThangLuong.GIOBATDAU_NGHITRUA;
              double SoGioNghiTrua = timeSpan.TotalHours;
              timeSpan = dm_ThangLuong.GIOKETTHUC_NGHITRUA - dm_ThangLuong.GIOBATDAU_NGHITRUA;
              double SoGioNghiTruaTrongNgay = timeSpan.TotalHours;
              if (GIOBATDAU < dm_ThangLuong.GIOBATDAU_NGHITRUA && GIOKETTHUC <= dm_ThangLuong.GIOKETTHUC_NGHITRUA)
              {
                timeSpan = GIOKETTHUC - dm_ThangLuong.GIOBATDAU_NGHITRUA;
                SoGioNghiTrua = timeSpan.TotalHours;
              }
              if (GIOBATDAU > dm_ThangLuong.GIOKETTHUC_NGHITRUA && GIOKETTHUC > dm_ThangLuong.GIOKETTHUC_NGHITRUA)
              {
                timeSpan = dm_ThangLuong.GIOKETTHUC_NGHITRUA - dm_ThangLuong.GIOBATDAU_NGHITRUA;
                SoGioNghiTrua = timeSpan.TotalHours;
              }
              if (GIOBATDAU < dm_ThangLuong.GIOBATDAU_NGHITRUA && GIOKETTHUC <= dm_ThangLuong.GIOBATDAU_NGHITRUA)
                SoGioNghiTrua = 0.0;
              if (GIOBATDAU >= dm_ThangLuong.GIOKETTHUC_NGHITRUA)
                SoGioNghiTrua = 0.0;
              SoGioLe = (SoGioLamViec - SoGioNghiTrua) / (SoTiengLamTrongNgay.TotalHours - SoGioNghiTruaTrongNgay);
              if (bolDungPhep)
                SoGioLe = 0.5;
            }
            else
            {
              if (string.IsNullOrEmpty(GhiChuVeSom))
                GhiChuVeSom = "Đi trễ, về sớm: ";
              GhiChuVeSom = $"{GhiChuVeSom}(v1v2) {date.ToString("dd")};";
            }
            SoNgayCong += SoGioLe;
            SoNgayLamViec += SoGioLe;
            ++SoNgayCoDiLam;
          }
          else if (!dm_ThangLuong.ISCHAMCONG && !bolDungPhep)
          {
            ++SoNgayCong;
            ++SoNgayLamViec;
            ++SoNgayCoDiLam;
          }
          if (nv_NghiPhep == null && nv_ChamCong == null && dm_ThangLuong.ISCHAMCONG || nv_NghiPhep != null && !nv_NghiPhep.ISNGHIPHEP)
          {
            ++SoNgayNghiKhongPhep;
            if (string.IsNullOrEmpty(GhiChuNghiKhongPhep))
              GhiChuNghiKhongPhep = "Nghỉ không phép: ";
            string[] strArray = new string[5]
            {
              GhiChuNghiKhongPhep,
              "Ngày ",
              null,
              null,
              null
            };
            num1 = date.Day;
            strArray[2] = num1.ToString();
            strArray[3] = nv_NghiPhep == null || !nv_NghiPhep.ISDUYETPHEP ? (nv_NghiPhep != null ? " Chưa duyệt nghỉ" : " Chưa xin nghỉ") : " Đã duyệt";
            strArray[4] = ";";
            GhiChuNghiKhongPhep = string.Concat(strArray);
          }
          nv_NghiPhep = (nv_NghiPhep) null;
          nv_ChamCong = (nv_ChamCong) null;
        }
      }
      Area.MUCLUONG = NhanVien.LUONGCOBAN;
      Area.SONGAYCONG = dm_ThangLuong.SONGAYCONG;
      Area.SONGAYLAMVIEC = Math.Round(SoNgayLamViec, 2);
      Area.SONGAYNGHIPHEP = SoNgayNghiPhep;
      Area.SONGAYNGHIKHONGPHEP = SoNgayNghiKhongPhep;
      Area.GHICHU = GhiChuNghiPhep + (string.IsNullOrEmpty(GhiChuNghiKhongPhep) ? "" : Environment.NewLine + GhiChuNghiKhongPhep) + (string.IsNullOrEmpty(GhiChuVeSom) ? "" : Environment.NewLine + GhiChuVeSom);
      if (lstdm_BangLuong_ChiTiet != null)
      {
        Area.lstnv_BangLuong_ChiTiet = new List<nv_BangLuong_ChiTiet>();
        foreach (dm_BangLuong_ChiTiet bangLuongChiTiet1 in lstdm_BangLuong_ChiTiet)
        {
          dm_BangLuong_ChiTiet itm = bangLuongChiTiet1;
          nv_BangLuong_ChiTiet newnv_BangLuong_ChiTiet = new nv_BangLuong_ChiTiet();
          newnv_BangLuong_ChiTiet.ID_LOAILUONG = itm.ID_LOAILUONG;
          newnv_BangLuong_ChiTiet.ID_BANGLUONG = Area.ID;
          newnv_BangLuong_ChiTiet.ID = Guid.NewGuid().ToString();
          if (itm.TYPE_QUYTACTINHLUONG == 0)
          {
            if (itm.TYPE_LUONG == 0)
              newnv_BangLuong_ChiTiet.SOTIEN = Math.Ceiling((itm.SOTIEN > 0.0 ? itm.SOTIEN : NhanVien.LUONGCOBAN) / dm_ThangLuong.SONGAYCONG * (Area.SONGAYLAMVIEC + SoNgayNghiPhep));
            if (itm.TYPE_LUONG == 1)
            {
              SP_Parameter SP_Parameter = new SP_Parameter();
              KPI_SaleController KPI = new KPI_SaleController(this._context, this._configuration);
              SP_Parameter.LOC_ID = Area.LOC_ID;
              SP_Parameter.TUNGAY = new DateTime?(dm_ThangLuong.NGAYBATDAU);
              SP_Parameter.DENNGAY = new DateTime?(dm_ThangLuong.NGAYKETTHUC);
              SP_Parameter.ID_NHANVIEN = NhanVien.ID;
              IActionResult KetQua = await KPI.PutProduct(SP_Parameter);
              if (KetQua is OkObjectResult okResult)
              {
                ApiResponse ApiResponse = okResult.Value as ApiResponse;
                if (ApiResponse != null && ApiResponse.Data != null)
                {
                  List<v_Tinh_KPI_KinhDoanh> lst_ChiTiet = ApiResponse.Data as List<v_Tinh_KPI_KinhDoanh>;
                  newnv_BangLuong_ChiTiet.SOTIEN = Math.Ceiling((itm.SOTIEN > 0.0 ? itm.SOTIEN : (lst_ChiTiet == null || lst_ChiTiet.Count <= 0 ? 0.0 : lst_ChiTiet[0].SOTIEN_KPI)) / dm_ThangLuong.SONGAYCONG * (Area.SONGAYLAMVIEC + SoNgayNghiPhep));
                  lst_ChiTiet = (List<v_Tinh_KPI_KinhDoanh>) null;
                }
                ApiResponse = (ApiResponse) null;
              }
              SP_Parameter = (SP_Parameter) null;
              KPI = (KPI_SaleController) null;
              KetQua = (IActionResult) null;
              okResult = (OkObjectResult) null;
            }
            if (itm.TYPE_LUONG == 2)
              newnv_BangLuong_ChiTiet.SOTIEN = Math.Ceiling(itm.SOTIEN / dm_ThangLuong.SONGAYCONG * (Area.SONGAYLAMVIEC + SoNgayNghiPhep));
          }
          else if (itm.TYPE_QUYTACTINHLUONG == 1)
          {
            if (itm.TYPE_LUONG == 0)
              newnv_BangLuong_ChiTiet.SOTIEN = Math.Ceiling((itm.SOTIEN > 0.0 ? itm.SOTIEN : NhanVien.LUONGCOBAN) / dm_ThangLuong.SONGAYCONG * SoNgayCoDiLam);
            if (itm.TYPE_LUONG == 1)
            {
              SP_Parameter SP_Parameter = new SP_Parameter();
              KPI_SaleController KPI = new KPI_SaleController(this._context, this._configuration);
              SP_Parameter.LOC_ID = Area.LOC_ID;
              SP_Parameter.TUNGAY = new DateTime?(dm_ThangLuong.NGAYBATDAU);
              SP_Parameter.DENNGAY = new DateTime?(dm_ThangLuong.NGAYKETTHUC);
              SP_Parameter.ID_NHANVIEN = NhanVien.ID;
              IActionResult KetQua = await KPI.PutProduct(SP_Parameter);
              if (KetQua is OkObjectResult okResult)
              {
                ApiResponse ApiResponse = okResult.Value as ApiResponse;
                if (ApiResponse != null && ApiResponse.Data != null)
                {
                  List<v_Tinh_KPI_KinhDoanh> lst_ChiTiet = ApiResponse.Data as List<v_Tinh_KPI_KinhDoanh>;
                  newnv_BangLuong_ChiTiet.SOTIEN = Math.Ceiling((itm.SOTIEN > 0.0 ? itm.SOTIEN : (lst_ChiTiet == null || lst_ChiTiet.Count <= 0 ? 0.0 : lst_ChiTiet[0].SOTIEN_KPI)) / dm_ThangLuong.SONGAYCONG * SoNgayCoDiLam);
                  lst_ChiTiet = (List<v_Tinh_KPI_KinhDoanh>) null;
                }
                ApiResponse = (ApiResponse) null;
              }
              SP_Parameter = (SP_Parameter) null;
              KPI = (KPI_SaleController) null;
              KetQua = (IActionResult) null;
              okResult = (OkObjectResult) null;
            }
            if (itm.TYPE_LUONG == 2)
              newnv_BangLuong_ChiTiet.SOTIEN = Math.Ceiling(itm.SOTIEN / dm_ThangLuong.SONGAYCONG * SoNgayCoDiLam);
          }
          else if (itm.TYPE_QUYTACTINHLUONG == 2)
          {
            if (itm.TYPE_LUONG == 0)
              newnv_BangLuong_ChiTiet.SOTIEN = Math.Ceiling(itm.SOTIEN > 0.0 ? itm.SOTIEN : NhanVien.LUONGCOBAN);
            if (itm.TYPE_LUONG == 1)
            {
              SP_Parameter SP_Parameter = new SP_Parameter();
              KPI_SaleController KPI = new KPI_SaleController(this._context, this._configuration);
              SP_Parameter.LOC_ID = Area.LOC_ID;
              SP_Parameter.TUNGAY = new DateTime?(dm_ThangLuong.NGAYBATDAU);
              SP_Parameter.DENNGAY = new DateTime?(dm_ThangLuong.NGAYKETTHUC);
              SP_Parameter.ID_NHANVIEN = NhanVien.ID;
              IActionResult KetQua = await KPI.PutProduct(SP_Parameter);
              if (KetQua is OkObjectResult okResult)
              {
                ApiResponse ApiResponse = okResult.Value as ApiResponse;
                if (ApiResponse != null && ApiResponse.Data != null)
                {
                  List<v_Tinh_KPI_KinhDoanh> lst_ChiTiet = ApiResponse.Data as List<v_Tinh_KPI_KinhDoanh>;
                  newnv_BangLuong_ChiTiet.SOTIEN = Math.Ceiling(itm.SOTIEN > 0.0 ? itm.SOTIEN : (lst_ChiTiet == null || lst_ChiTiet.Count <= 0 ? 0.0 : lst_ChiTiet[0].SOTIEN_KPI));
                  lst_ChiTiet = (List<v_Tinh_KPI_KinhDoanh>) null;
                }
                ApiResponse = (ApiResponse) null;
              }
              SP_Parameter = (SP_Parameter) null;
              KPI = (KPI_SaleController) null;
              KetQua = (IActionResult) null;
              okResult = (OkObjectResult) null;
            }
            if (itm.TYPE_LUONG == 2)
              newnv_BangLuong_ChiTiet.SOTIEN = itm.SOTIEN;
          }
          dm_LoaiLuong dm_LoaiLuong = await this._context.dm_LoaiLuong.FirstOrDefaultAsync<dm_LoaiLuong>((Expression<Func<dm_LoaiLuong, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID == itm.ID_LOAILUONG));
          nv_BangLuong_ChiTiet bangLuongChiTiet2 = newnv_BangLuong_ChiTiet;
          string str;
          if (dm_LoaiLuong == null)
          {
            str = "0";
          }
          else
          {
            num1 = dm_LoaiLuong.TYPE;
            str = num1.ToString();
          }
          bangLuongChiTiet2.TYPE = str;
          Area.lstnv_BangLuong_ChiTiet.Add(newnv_BangLuong_ChiTiet);
          newnv_BangLuong_ChiTiet = (nv_BangLuong_ChiTiet) null;
          dm_LoaiLuong = (dm_LoaiLuong) null;
        }
      }
      Area.TIENLUONG = Math.Ceiling(Area.lstnv_BangLuong_ChiTiet.Where<nv_BangLuong_ChiTiet>((Func<nv_BangLuong_ChiTiet, bool>) (s => Convert.ToInt32(s.TYPE) > 0)).Sum<nv_BangLuong_ChiTiet>((Func<nv_BangLuong_ChiTiet, double>) (s => s.SOTIEN)));
      Area.TIENGIAM = Math.Ceiling(Area.lstnv_BangLuong_ChiTiet.Where<nv_BangLuong_ChiTiet>((Func<nv_BangLuong_ChiTiet, bool>) (s => Convert.ToInt32(s.TYPE) < 0)).Sum<nv_BangLuong_ChiTiet>((Func<nv_BangLuong_ChiTiet, double>) (s => s.SOTIEN)));
      Area.TIENTHUCNHAN = Area.TIENLUONG - Area.TIENGIAM;
      return (ActionResult<dm_KhuVuc>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) Area
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_KhuVuc>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpPost("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<ActionResult<dm_KhuVuc>> GetPhieuIn(string LOC_ID, string ID)
  {
    try
    {
      List<view_nv_BangLuong_ChiTiet> lstValue = await this._context.view_nv_BangLuong_ChiTiet.Where<view_nv_BangLuong_ChiTiet>((Expression<Func<view_nv_BangLuong_ChiTiet, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_BANGLUONG == ID)).ToListAsync<view_nv_BangLuong_ChiTiet>();
      return (ActionResult<dm_KhuVuc>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) lstValue
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_KhuVuc>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }
}
