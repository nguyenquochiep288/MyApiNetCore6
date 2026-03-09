// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.ReportController
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
public class ReportController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public ReportController(dbTrangHiepPhatContext context, IConfiguration configuration)
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
      List<view_web_Report> lstValue = await this._context.view_web_Report.ToListAsync<view_web_Report>();
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
      List<view_web_Report> lstValue = await this._context.view_web_Report.Where<view_web_Report>(KeyWhere, (object) ValuesSearch).ToListAsync<view_web_Report>();
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
      view_web_Report Report = await this._context.view_web_Report.FirstOrDefaultAsync<view_web_Report>((Expression<Func<view_web_Report, bool>>) (e => e.ID == ID));
      if (Report != null)
      {
        Report.lstweb_Report_Parameter = new List<v_web_Report_Parameter>();
        List<view_web_Report_Parameter> lstweb_Report_Parameter = await this._context.view_web_Report_Parameter.Where<view_web_Report_Parameter>((Expression<Func<view_web_Report_Parameter, bool>>) (e => e.ID_REPORT == ID)).ToListAsync<view_web_Report_Parameter>();
        if (lstweb_Report_Parameter != null)
        {
          string json = JsonConvert.SerializeObject((object) lstweb_Report_Parameter);
          Report.lstweb_Report_Parameter = JsonConvert.DeserializeObject<List<v_web_Report_Parameter>>(json);
          json = (string) null;
        }
        lstweb_Report_Parameter = (List<view_web_Report_Parameter>) null;
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
  public async Task<IActionResult> PutReport(string MA, v_web_Report Report)
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
        foreach (v_web_Report_Parameter webReportParameter1 in Report.lstweb_Report_Parameter)
        {
          v_web_Report_Parameter itm = webReportParameter1;
          web_Report_Parameter newct_PhieuXuat_CT = new web_Report_Parameter();
          newct_PhieuXuat_CT = ReportController.ConvertobjectToct_web_Report_Parameter<v_web_Report_Parameter>(itm, newct_PhieuXuat_CT);
          Guid guid;
          if (!string.IsNullOrEmpty(itm.ID))
          {
            view_web_Report_Parameter Report_Parameter = await this._context.view_web_Report_Parameter.FirstOrDefaultAsync<view_web_Report_Parameter>((Expression<Func<view_web_Report_Parameter, bool>>) (e => e.ID == itm.ID && e.ID_REPORT == Report.ID));
            if (Report_Parameter != null)
            {
              this._context.Entry<web_Report_Parameter>(newct_PhieuXuat_CT).State = EntityState.Modified;
            }
            else
            {
              web_Report_Parameter webReportParameter2 = newct_PhieuXuat_CT;
              guid = Guid.NewGuid();
              string str = guid.ToString();
              webReportParameter2.ID = str;
              newct_PhieuXuat_CT.ID_REPORT = Report.ID;
              this._context.web_Report_Parameter.Add(newct_PhieuXuat_CT);
            }
            Report_Parameter = (view_web_Report_Parameter) null;
          }
          else
          {
            web_Report_Parameter webReportParameter3 = newct_PhieuXuat_CT;
            guid = Guid.NewGuid();
            string str = guid.ToString();
            webReportParameter3.ID = str;
            newct_PhieuXuat_CT.ID_REPORT = Report.ID;
            this._context.web_Report_Parameter.Add(newct_PhieuXuat_CT);
          }
          newct_PhieuXuat_CT = (web_Report_Parameter) null;
        }
        this._context.Entry<v_web_Report>(Report).State = EntityState.Modified;
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        auditLog = (AuditLogController) null;
        transaction.Commit();
        view_web_Report OKReport = await this._context.view_web_Report.FirstOrDefaultAsync<view_web_Report>((Expression<Func<view_web_Report, bool>>) (e => e.ID == Report.ID));
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

  private static web_Report_Parameter ConvertobjectToct_web_Report_Parameter<T>(
    T objectFrom,
    web_Report_Parameter objectTo)
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
  public async Task<ActionResult<web_Report>> PostReport(v_web_Report Report)
  {
    try
    {
      if (this.ReportExistsMA(Report.MA))
        return (ActionResult<web_Report>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Report.MA} trong dữ liệu!",
          Data = (object) ""
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        this._context.web_Report.Add((web_Report) Report);
        foreach (v_web_Report_Parameter itm in Report.lstweb_Report_Parameter)
        {
          web_Report_Parameter newct_PhieuXuat_CT = new web_Report_Parameter();
          newct_PhieuXuat_CT = ReportController.ConvertobjectToct_web_Report_Parameter<v_web_Report_Parameter>(itm, newct_PhieuXuat_CT);
          newct_PhieuXuat_CT.ID = Guid.NewGuid().ToString();
          newct_PhieuXuat_CT.ID_REPORT = Report.ID;
          this._context.web_Report_Parameter.Add(newct_PhieuXuat_CT);
          newct_PhieuXuat_CT = (web_Report_Parameter) null;
        }
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        auditLog = (AuditLogController) null;
        transaction.Commit();
        view_web_Report OKReport = await this._context.view_web_Report.FirstOrDefaultAsync<view_web_Report>((Expression<Func<view_web_Report, bool>>) (e => e.ID == Report.ID));
        return (ActionResult<web_Report>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) OKReport
        });
      }
    }
    catch (Exception ex)
    {
      return (ActionResult<web_Report>) (ActionResult) this.Ok((object) new ApiResponse()
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
      web_Report Report = await this._context.web_Report.FirstOrDefaultAsync<web_Report>((Expression<Func<web_Report, bool>>) (e => e.ID == ID));
      if (Report == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<web_Report>(Report, Report.ID, Report.MA);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      List<web_Report_Parameter> lstweb_PhanQuyenSanPham = await this._context.web_Report_Parameter.Where<web_Report_Parameter>((Expression<Func<web_Report_Parameter, bool>>) (e => e.ID_REPORT == ID)).ToListAsync<web_Report_Parameter>();
      if (lstweb_PhanQuyenSanPham != null)
      {
        foreach (web_Report_Parameter itm in lstweb_PhanQuyenSanPham)
          this._context.web_Report_Parameter.Remove(itm);
      }
      this._context.web_Report.Remove(Report);
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
    return this._context.web_Report.Any<web_Report>((Expression<Func<web_Report, bool>>) (e => e.MA == MA));
  }

  private bool ReportExistsID(string ID)
  {
    return this._context.web_Report.Any<web_Report>((Expression<Func<web_Report, bool>>) (e => e.ID == ID));
  }
}
