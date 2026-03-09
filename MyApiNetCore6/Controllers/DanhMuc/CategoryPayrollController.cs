// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.CategoryPayrollController
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using DatabaseTHP;
using DatabaseTHP.Class;
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
using System.Reflection;
using System.Threading.Tasks;

#nullable enable
namespace MyApiNetCore6.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryPayrollController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public CategoryPayrollController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetReport()
  {
    try
    {
      List<view_dm_BangLuong> lstValue = await this._context.view_dm_BangLuong.ToListAsync<view_dm_BangLuong>();
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

  [HttpGet("{Type}/{KeyWhere}/{ValuesSearch}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetReport(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<view_dm_BangLuong> lstValue = await this._context.view_dm_BangLuong.Where<view_dm_BangLuong>(KeyWhere, (object) ValuesSearch).ToListAsync<view_dm_BangLuong>();
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

  [HttpGet("{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetReport(string ID)
  {
    try
    {
      view_dm_BangLuong Report = await this._context.view_dm_BangLuong.FirstOrDefaultAsync<view_dm_BangLuong>((Expression<Func<view_dm_BangLuong, bool>>) (e => e.ID == ID));
      if (Report != null)
      {
        Report.lstdm_BangLuong_ChiTiet = new List<v_dm_BangLuong_ChiTiet>();
        List<dm_BangLuong_ChiTiet> lstdm_BangLuong_ChiTiet = await this._context.dm_BangLuong_ChiTiet.Where<dm_BangLuong_ChiTiet>((Expression<Func<dm_BangLuong_ChiTiet, bool>>) (e => e.ID_BANGLUONG == ID)).ToListAsync<dm_BangLuong_ChiTiet>();
        if (lstdm_BangLuong_ChiTiet != null)
        {
          string json = JsonConvert.SerializeObject((object) lstdm_BangLuong_ChiTiet);
          Report.lstdm_BangLuong_ChiTiet = JsonConvert.DeserializeObject<List<v_dm_BangLuong_ChiTiet>>(json);
          json = (string) null;
        }
        lstdm_BangLuong_ChiTiet = (List<dm_BangLuong_ChiTiet>) null;
      }
      if (Report == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {ID} dữ liệu!",
          Data = (object) ""
        });
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) Report
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

  [HttpPut("{MA}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PutReport(string MA, v_dm_BangLuong Report)
  {
    try
    {
      if (Report.MA != MA)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.ReportExistsID(Report.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {Report.ID} dữ liệu!",
          Data = (object) ""
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        List<dm_BangLuong_ChiTiet> lstdm_BangLuong_ChiTiet1 = await this._context.dm_BangLuong_ChiTiet.AsNoTracking<dm_BangLuong_ChiTiet>().Where<dm_BangLuong_ChiTiet>((Expression<Func<dm_BangLuong_ChiTiet, bool>>) (e => e.ID_BANGLUONG == Report.ID)).ToListAsync<dm_BangLuong_ChiTiet>();
        foreach (dm_BangLuong_ChiTiet bangLuongChiTiet in lstdm_BangLuong_ChiTiet1)
        {
          dm_BangLuong_ChiTiet dm_BangLuong_ChiTiet = bangLuongChiTiet;
          if (Report.lstdm_BangLuong_ChiTiet.Where<v_dm_BangLuong_ChiTiet>((Func<v_dm_BangLuong_ChiTiet, bool>) (s => s.ID == dm_BangLuong_ChiTiet.ID)).Count<v_dm_BangLuong_ChiTiet>() == 0)
            this._context.dm_BangLuong_ChiTiet.Remove(dm_BangLuong_ChiTiet);
        }
        foreach (v_dm_BangLuong_ChiTiet bangLuongChiTiet1 in Report.lstdm_BangLuong_ChiTiet)
        {
          v_dm_BangLuong_ChiTiet itm = bangLuongChiTiet1;
          dm_BangLuong_ChiTiet newct_PhieuXuat_CT = new dm_BangLuong_ChiTiet();
          newct_PhieuXuat_CT = CategoryPayrollController.ConvertobjectToct_dm_BangLuong_ChiTiet<v_dm_BangLuong_ChiTiet>(itm, newct_PhieuXuat_CT);
          Guid guid;
          if (!string.IsNullOrEmpty(itm.ID))
          {
            dm_BangLuong_ChiTiet Report_Parameter = await this._context.dm_BangLuong_ChiTiet.AsNoTracking<dm_BangLuong_ChiTiet>().FirstOrDefaultAsync<dm_BangLuong_ChiTiet>((Expression<Func<dm_BangLuong_ChiTiet, bool>>) (e => e.ID == itm.ID && e.ID_BANGLUONG == Report.ID));
            if (Report_Parameter != null)
            {
              this._context.Entry<dm_BangLuong_ChiTiet>(newct_PhieuXuat_CT).State = EntityState.Modified;
            }
            else
            {
              dm_BangLuong_ChiTiet bangLuongChiTiet2 = newct_PhieuXuat_CT;
              guid = Guid.NewGuid();
              string str = guid.ToString();
              bangLuongChiTiet2.ID = str;
              newct_PhieuXuat_CT.ID_BANGLUONG = Report.ID;
              this._context.dm_BangLuong_ChiTiet.Add(newct_PhieuXuat_CT);
            }
            Report_Parameter = (dm_BangLuong_ChiTiet) null;
          }
          else
          {
            dm_BangLuong_ChiTiet bangLuongChiTiet3 = newct_PhieuXuat_CT;
            guid = Guid.NewGuid();
            string str = guid.ToString();
            bangLuongChiTiet3.ID = str;
            newct_PhieuXuat_CT.ID_BANGLUONG = Report.ID;
            this._context.dm_BangLuong_ChiTiet.Add(newct_PhieuXuat_CT);
          }
          newct_PhieuXuat_CT = (dm_BangLuong_ChiTiet) null;
        }
        dm_BangLuong newdm_BangLuong = new dm_BangLuong();
        newdm_BangLuong = CategoryPayrollController.ConvertobjectToct_dm_BangLuong<v_dm_BangLuong>(Report, newdm_BangLuong);
        this._context.dm_BangLuong.Add(newdm_BangLuong);
        this._context.Entry<dm_BangLuong>(newdm_BangLuong).State = EntityState.Modified;
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        lstdm_BangLuong_ChiTiet1 = (List<dm_BangLuong_ChiTiet>) null;
        newdm_BangLuong = (dm_BangLuong) null;
        auditLog = (AuditLogController) null;
        transaction.Commit();
        view_dm_BangLuong OKReport = await this._context.view_dm_BangLuong.FirstOrDefaultAsync<view_dm_BangLuong>((Expression<Func<view_dm_BangLuong, bool>>) (e => e.ID == Report.ID));
        if (OKReport != null)
        {
          OKReport.lstdm_BangLuong_ChiTiet = new List<v_dm_BangLuong_ChiTiet>();
          List<dm_BangLuong_ChiTiet> lstdm_BangLuong_ChiTiet2 = await this._context.dm_BangLuong_ChiTiet.Where<dm_BangLuong_ChiTiet>((Expression<Func<dm_BangLuong_ChiTiet, bool>>) (e => e.ID_BANGLUONG == Report.ID)).ToListAsync<dm_BangLuong_ChiTiet>();
          if (lstdm_BangLuong_ChiTiet2 != null)
          {
            string json = JsonConvert.SerializeObject((object) lstdm_BangLuong_ChiTiet2);
            OKReport.lstdm_BangLuong_ChiTiet = JsonConvert.DeserializeObject<List<v_dm_BangLuong_ChiTiet>>(json);
            json = (string) null;
          }
          lstdm_BangLuong_ChiTiet2 = (List<dm_BangLuong_ChiTiet>) null;
        }
        if (OKReport == null)
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = $"Không tìm thấy {Report.ID} dữ liệu!",
            Data = (object) ""
          });
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) OKReport
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

  private static dm_BangLuong_ChiTiet ConvertobjectToct_dm_BangLuong_ChiTiet<T>(
    T objectFrom,
    dm_BangLuong_ChiTiet objectTo)
  {
    if ((object) objectFrom != null)
    {
      foreach (PropertyInfo property1 in objectFrom.GetType().GetProperties())
      {
        if (property1 != (PropertyInfo) null)
        {
          object obj = property1.GetValue((object) objectFrom);
          if (obj != null)
          {
            PropertyInfo property2 = objectTo.GetType().GetProperty(property1.Name);
            if (property2 != (PropertyInfo) null)
              property2.SetValue((object) objectTo, obj);
          }
        }
      }
    }
    return objectTo;
  }

  private static dm_BangLuong ConvertobjectToct_dm_BangLuong<T>(T objectFrom, dm_BangLuong objectTo)
  {
    if ((object) objectFrom != null)
    {
      foreach (PropertyInfo property1 in objectFrom.GetType().GetProperties())
      {
        if (property1 != (PropertyInfo) null)
        {
          object obj = property1.GetValue((object) objectFrom);
          if (obj != null)
          {
            PropertyInfo property2 = objectTo.GetType().GetProperty(property1.Name);
            if (property2 != (PropertyInfo) null)
              property2.SetValue((object) objectTo, obj);
          }
        }
      }
    }
    return objectTo;
  }

  [HttpPost]
  [Authorize(Roles = "User")]
  public async Task<ActionResult<dm_BangLuong>> PostReport(v_dm_BangLuong Report)
  {
    try
    {
      if (this.ReportExistsMA(Report.MA))
        return (ActionResult<dm_BangLuong>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Report.MA} trong dữ liệu!",
          Data = (object) ""
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        dm_BangLuong newdm_BangLuong = new dm_BangLuong();
        newdm_BangLuong = CategoryPayrollController.ConvertobjectToct_dm_BangLuong<v_dm_BangLuong>(Report, newdm_BangLuong);
        this._context.dm_BangLuong.Add(newdm_BangLuong);
        foreach (v_dm_BangLuong_ChiTiet itm in Report.lstdm_BangLuong_ChiTiet)
        {
          dm_BangLuong_ChiTiet newct_PhieuXuat_CT = new dm_BangLuong_ChiTiet();
          newct_PhieuXuat_CT = CategoryPayrollController.ConvertobjectToct_dm_BangLuong_ChiTiet<v_dm_BangLuong_ChiTiet>(itm, newct_PhieuXuat_CT);
          newct_PhieuXuat_CT.ID = Guid.NewGuid().ToString();
          newct_PhieuXuat_CT.ID_BANGLUONG = Report.ID;
          this._context.dm_BangLuong_ChiTiet.Add(newct_PhieuXuat_CT);
          newct_PhieuXuat_CT = (dm_BangLuong_ChiTiet) null;
        }
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        newdm_BangLuong = (dm_BangLuong) null;
        auditLog = (AuditLogController) null;
        transaction.Commit();
        view_dm_BangLuong OKReport = await this._context.view_dm_BangLuong.FirstOrDefaultAsync<view_dm_BangLuong>((Expression<Func<view_dm_BangLuong, bool>>) (e => e.ID == Report.ID));
        if (OKReport != null)
        {
          OKReport.lstdm_BangLuong_ChiTiet = new List<v_dm_BangLuong_ChiTiet>();
          List<dm_BangLuong_ChiTiet> lstdm_BangLuong_ChiTiet = await this._context.dm_BangLuong_ChiTiet.Where<dm_BangLuong_ChiTiet>((Expression<Func<dm_BangLuong_ChiTiet, bool>>) (e => e.ID_BANGLUONG == Report.ID)).ToListAsync<dm_BangLuong_ChiTiet>();
          if (lstdm_BangLuong_ChiTiet != null)
          {
            string json = JsonConvert.SerializeObject((object) lstdm_BangLuong_ChiTiet);
            OKReport.lstdm_BangLuong_ChiTiet = JsonConvert.DeserializeObject<List<v_dm_BangLuong_ChiTiet>>(json);
            json = (string) null;
          }
          lstdm_BangLuong_ChiTiet = (List<dm_BangLuong_ChiTiet>) null;
        }
        if (OKReport == null)
          return (ActionResult<dm_BangLuong>) (ActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = $"Không tìm thấy {Report.ID} dữ liệu!",
            Data = (object) ""
          });
        return (ActionResult<dm_BangLuong>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) OKReport
        });
      }
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_BangLuong>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteReport(string ID)
  {
    try
    {
      dm_BangLuong Report = await this._context.dm_BangLuong.FirstOrDefaultAsync<dm_BangLuong>((Expression<Func<dm_BangLuong, bool>>) (e => e.ID == ID));
      if (Report == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_BangLuong>(Report, Report.ID, Report.MA);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      List<dm_BangLuong_ChiTiet> lstweb_PhanQuyenSanPham = await this._context.dm_BangLuong_ChiTiet.Where<dm_BangLuong_ChiTiet>((Expression<Func<dm_BangLuong_ChiTiet, bool>>) (e => e.ID_BANGLUONG == ID)).ToListAsync<dm_BangLuong_ChiTiet>();
      if (lstweb_PhanQuyenSanPham != null)
      {
        foreach (dm_BangLuong_ChiTiet itm in lstweb_PhanQuyenSanPham)
          this._context.dm_BangLuong_ChiTiet.Remove(itm);
      }
      this._context.dm_BangLuong.Remove(Report);
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

  private bool ReportExistsMA(string MA)
  {
    return this._context.dm_BangLuong.Any<dm_BangLuong>((Expression<Func<dm_BangLuong, bool>>) (e => e.MA == MA));
  }

  private bool ReportExistsID(string ID)
  {
    return this._context.dm_BangLuong.Any<dm_BangLuong>((Expression<Func<dm_BangLuong, bool>>) (e => e.ID == ID));
  }
}
