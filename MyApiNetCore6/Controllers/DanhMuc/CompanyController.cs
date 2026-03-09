// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.CompanyController
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
public class CompanyController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public CompanyController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetCompany()
  {
    try
    {
      List<dm_CongTy> lstValue = await this._context.dm_CongTy.ToListAsync<dm_CongTy>();
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
  public async Task<IActionResult> GetCompany(int Type, string KeyWhere = "", string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<dm_CongTy> lstValue = await this._context.dm_CongTy.Where<dm_CongTy>(KeyWhere, (object) ValuesSearch).ToListAsync<dm_CongTy>();
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
  public async Task<IActionResult> GetCompany(string ID)
  {
    try
    {
      dm_CongTy Company = await this._context.dm_CongTy.FirstOrDefaultAsync<dm_CongTy>((Expression<Func<dm_CongTy, bool>>) (e => e.ID == ID));
      if (Company == null)
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
        Data = (object) Company
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
  public async Task<IActionResult> PutCompany(string MA, dm_CongTy Company)
  {
    try
    {
      if (MA != Company.MA)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.CompanyExistsID(Company.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {Company.ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<dm_CongTy>(Company).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      dm_CongTy OKCompany = await this._context.dm_CongTy.FirstOrDefaultAsync<dm_CongTy>((Expression<Func<dm_CongTy, bool>>) (e => e.ID == Company.ID));
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKCompany
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
  public async Task<ActionResult<dm_CongTy>> PostCompany(dm_CongTy Company)
  {
    try
    {
      if (this.CompanyExistsMA(Company.MA))
        return (ActionResult<dm_CongTy>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Company.MA} trong dữ liệu!",
          Data = (object) ""
        });
      this._context.dm_CongTy.Add(Company);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      dm_CongTy OKCompany = await this._context.dm_CongTy.FirstOrDefaultAsync<dm_CongTy>((Expression<Func<dm_CongTy, bool>>) (e => e.ID == Company.ID));
      return (ActionResult<dm_CongTy>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKCompany
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_CongTy>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteCompany(string ID)
  {
    try
    {
      dm_CongTy Company = await this._context.dm_CongTy.FirstOrDefaultAsync<dm_CongTy>((Expression<Func<dm_CongTy, bool>>) (e => e.ID == ID));
      if (Company == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_CongTy>(Company, Company.ID, Company.MA);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      this._context.dm_CongTy.Remove(Company);
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

  private bool CompanyExistsID(string ID)
  {
    return this._context.dm_CongTy.Any<dm_CongTy>((Expression<Func<dm_CongTy, bool>>) (e => e.ID == ID));
  }

  private bool CompanyExistsMA(string MA)
  {
    return this._context.dm_CongTy.Any<dm_CongTy>((Expression<Func<dm_CongTy, bool>>) (e => e.MA == MA));
  }

  private bool CompanyExists(dm_CongTy Company)
  {
    return this._context.dm_CongTy.Any<dm_CongTy>((Expression<Func<dm_CongTy, bool>>) (e => e.MA == Company.MA && e.ID != Company.ID));
  }
}
