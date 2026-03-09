// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.ParameterController
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

#nullable enable
namespace MyApiNetCore6.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ParameterController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public ParameterController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetParameter()
  {
    try
    {
      List<web_Parameter> lstValue = await this._context.web_Parameter.ToListAsync<web_Parameter>();
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
  public async Task<IActionResult> GetParameter(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<web_Parameter> lstValue = await this._context.web_Parameter.Where<web_Parameter>(KeyWhere, (object) ValuesSearch).ToListAsync<web_Parameter>();
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
  public async Task<IActionResult> GetParameter(string ID)
  {
    try
    {
      web_Parameter Parameter = await this._context.web_Parameter.FirstOrDefaultAsync<web_Parameter>((Expression<Func<web_Parameter, bool>>) (e => e.ID == ID));
      if (Parameter == null)
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
        Data = (object) Parameter
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
  public async Task<IActionResult> PutParameter(string MA, web_Parameter Parameter)
  {
    try
    {
      if (Parameter.MA != MA)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.ParameterExistsID(Parameter.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {Parameter.ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<web_Parameter>(Parameter).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      web_Parameter OKParameter = await this._context.web_Parameter.FirstOrDefaultAsync<web_Parameter>((Expression<Func<web_Parameter, bool>>) (e => e.ID == Parameter.ID));
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKParameter
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
  public async Task<ActionResult<web_Parameter>> PostParameter(web_Parameter Parameter)
  {
    try
    {
      if (this.ParameterExistsMA(Parameter.MA))
        return (ActionResult<web_Parameter>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Parameter.MA} trong dữ liệu!",
          Data = (object) ""
        });
      this._context.web_Parameter.Add(Parameter);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      web_Parameter OKParameter = await this._context.web_Parameter.FirstOrDefaultAsync<web_Parameter>((Expression<Func<web_Parameter, bool>>) (e => e.ID == Parameter.ID));
      return (ActionResult<web_Parameter>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKParameter
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<web_Parameter>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteParameter(string ID)
  {
    try
    {
      web_Parameter Parameter = await this._context.web_Parameter.FirstOrDefaultAsync<web_Parameter>((Expression<Func<web_Parameter, bool>>) (e => e.ID == ID));
      if (Parameter == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<web_Parameter>(Parameter, Parameter.ID, Parameter.MA);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      List<web_Report_Parameter> lstweb_PhanQuyenSanPham = await this._context.web_Report_Parameter.Where<web_Report_Parameter>((Expression<Func<web_Report_Parameter, bool>>) (e => e.ID_PARAMETER == ID)).ToListAsync<web_Report_Parameter>();
      if (lstweb_PhanQuyenSanPham != null)
      {
        foreach (web_Report_Parameter itm in lstweb_PhanQuyenSanPham)
          this._context.web_Report_Parameter.Remove(itm);
      }
      this._context.web_Parameter.Remove(Parameter);
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

  private bool ParameterExistsMA(string MA)
  {
    return this._context.web_Parameter.Any<web_Parameter>((Expression<Func<web_Parameter, bool>>) (e => e.MA == MA));
  }

  private bool ParameterExistsID(string ID)
  {
    return this._context.web_Parameter.Any<web_Parameter>((Expression<Func<web_Parameter, bool>>) (e => e.ID == ID));
  }
}
