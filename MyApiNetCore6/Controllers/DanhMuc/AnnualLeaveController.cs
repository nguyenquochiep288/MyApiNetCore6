// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.AnnualLeaveController
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using DatabaseTHP;
using DatabaseTHP.Class;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyApiNetCore6.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace MyApiNetCore6.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnnualLeaveController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public AnnualLeaveController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetAnnualLeave(string LOC_ID)
  {
    try
    {
      List<view_nv_PhepNam> lstValue = await this._context.view_nv_PhepNam.Where<view_nv_PhepNam>((Expression<Func<view_nv_PhepNam, bool>>) (e => e.LOC_ID == LOC_ID)).OrderBy<view_nv_PhepNam, double>((Expression<Func<view_nv_PhepNam, double>>) (e => e.NAM)).ToListAsync<view_nv_PhepNam>();
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
  public async Task<IActionResult> GetAnnualLeave(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<view_nv_PhepNam> lstValue = await this._context.view_nv_PhepNam.Where<view_nv_PhepNam>((Expression<Func<view_nv_PhepNam, bool>>) (e => e.LOC_ID == LOC_ID)).Where<view_nv_PhepNam>(KeyWhere, (object) ValuesSearch).OrderBy<view_nv_PhepNam, double>((Expression<Func<view_nv_PhepNam, double>>) (e => e.NAM)).ToListAsync<view_nv_PhepNam>();
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
  public async Task<IActionResult> GetAnnualLeave(string LOC_ID, string ID)
  {
    try
    {
      nv_PhepNam AnnualLeave = await this._context.nv_PhepNam.FirstOrDefaultAsync<nv_PhepNam>((Expression<Func<nv_PhepNam, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (AnnualLeave == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) AnnualLeave
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

  [HttpPut("{LOC_ID}/{ID_NHANVIEN}/{NAM}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PutAnnualLeave(
    string LOC_ID,
    string ID_NHANVIEN,
    string NAM,
    nv_PhepNam AnnualLeave)
  {
    try
    {
      double nam;
      int num1;
      if (!(LOC_ID != AnnualLeave.LOC_ID) && !(AnnualLeave.ID_NHANVIEN != ID_NHANVIEN))
      {
        nam = AnnualLeave.NAM;
        num1 = nam.ToString() != NAM ? 1 : 0;
      }
      else
        num1 = 1;
      if (num1 != 0)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (this.AnnualLeaveExists(AnnualLeave))
      {
        ApiResponse apiResponse1 = new ApiResponse();
        apiResponse1.Success = false;
        ApiResponse apiResponse2 = apiResponse1;
        string[] strArray = new string[5]
        {
          "Đã tồn tại",
          AnnualLeave.LOC_ID,
          "-",
          null,
          null
        };
        nam = AnnualLeave.NAM;
        strArray[3] = nam.ToString();
        strArray[4] = " trong dữ liệu!";
        string str = string.Concat(strArray);
        apiResponse2.Message = str;
        apiResponse1.Data = (object) "";
        return (IActionResult) this.Ok((object) apiResponse1);
      }
      if (!this.AnnualLeaveExistsID(LOC_ID, AnnualLeave.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{AnnualLeave.ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<nv_PhepNam>(AnnualLeave).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num2 = await this._context.SaveChangesAsync();
      view_nv_PhepNam OKAnnualLeave = await this._context.view_nv_PhepNam.FirstOrDefaultAsync<view_nv_PhepNam>((Expression<Func<view_nv_PhepNam, bool>>) (e => e.LOC_ID == AnnualLeave.LOC_ID && e.ID == AnnualLeave.ID));
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKAnnualLeave
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

  [HttpPost]
  [Authorize(Roles = "User")]
  public async Task<ActionResult<nv_PhepNam>> PostAnnualLeave(v_nv_PhepNam AnnualLeave)
  {
    try
    {
      if (this.AnnualLeaveExistsMA(AnnualLeave.LOC_ID, AnnualLeave.NAM, AnnualLeave.ID_NHANVIEN))
        return (ActionResult<nv_PhepNam>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{AnnualLeave.LOC_ID}-{AnnualLeave.NAM.ToString()} trong dữ liệu!",
          Data = (object) ""
        });
      if (AnnualLeave.ISALL)
      {
        List<AspNetUsers> lstValue = await this._context.AspNetUsers.Where<AspNetUsers>((Expression<Func<AspNetUsers, bool>>) (e => e.LockoutEnabled)).ToListAsync<AspNetUsers>();
        foreach (AspNetUsers aspNetUsers in lstValue)
        {
          AspNetUsers itm = aspNetUsers;
          dm_NhanVien TaiKhoan = await this._context.dm_NhanVien.FirstOrDefaultAsync<dm_NhanVien>((Expression<Func<dm_NhanVien, bool>>) (e => e.LOC_ID == AnnualLeave.LOC_ID && e.ID_TAIKHOAN == itm.ID));
          if (!this.AnnualLeaveExistsMA(AnnualLeave.LOC_ID, AnnualLeave.NAM, itm.ID) && TaiKhoan != null)
          {
            nv_PhepNam b = new nv_PhepNam();
            b.ID = Guid.NewGuid().ToString();
            b.LOC_ID = AnnualLeave.LOC_ID;
            b.NAM = AnnualLeave.NAM;
            b.ID_NHANVIEN = itm.ID;
            b.SONGAYPHEP = TaiKhoan.SONGAYPHEP > 0.0 ? TaiKhoan.SONGAYPHEP : AnnualLeave.SONGAYPHEP;
            b.SONGAYPHEPDADUNG = AnnualLeave.SONGAYPHEPDADUNG;
            b.NGAYBATDAU = AnnualLeave.NGAYBATDAU;
            b.NGAYKETTHUC = AnnualLeave.NGAYKETTHUC;
            b.ID_NGUOITAO = AnnualLeave.ID_NGUOITAO;
            b.THOIGIANTHEM = AnnualLeave.THOIGIANTHEM;
            this._context.nv_PhepNam.Add(b);
            b = (nv_PhepNam) null;
          }
          TaiKhoan = (dm_NhanVien) null;
        }
        lstValue = (List<AspNetUsers>) null;
      }
      else
        this._context.nv_PhepNam.Add((nv_PhepNam) AnnualLeave);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      view_nv_PhepNam OKAnnualLeave = await this._context.view_nv_PhepNam.FirstOrDefaultAsync<view_nv_PhepNam>((Expression<Func<view_nv_PhepNam, bool>>) (e => e.LOC_ID == AnnualLeave.LOC_ID && e.ID == AnnualLeave.ID));
      return (ActionResult<nv_PhepNam>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKAnnualLeave
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<nv_PhepNam>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteAnnualLeave(string LOC_ID, string ID)
  {
    try
    {
      nv_PhepNam AnnualLeave = await this._context.nv_PhepNam.FirstOrDefaultAsync<nv_PhepNam>((Expression<Func<nv_PhepNam, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (AnnualLeave == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.nv_PhepNam.Remove(AnnualLeave);
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

  private bool AnnualLeaveExistsMA(string LOC_ID, double NAM, string ID_NHANVIEN)
  {
    return this._context.nv_PhepNam.Any<nv_PhepNam>((Expression<Func<nv_PhepNam, bool>>) (e => e.LOC_ID == LOC_ID && e.NAM == NAM && e.ID_NHANVIEN == ID_NHANVIEN));
  }

  private bool AnnualLeaveExistsID(string LOC_ID, string ID)
  {
    return this._context.nv_PhepNam.Any<nv_PhepNam>((Expression<Func<nv_PhepNam, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }

  private bool AnnualLeaveExists(nv_PhepNam AnnualLeave)
  {
    return this._context.nv_PhepNam.Any<nv_PhepNam>((Expression<Func<nv_PhepNam, bool>>) (e => e.LOC_ID == AnnualLeave.LOC_ID && e.NAM == AnnualLeave.NAM && e.ID_NHANVIEN == AnnualLeave.ID_NHANVIEN && e.ID != AnnualLeave.ID));
  }
}
