// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.CurrencyController
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
public class CurrencyController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public CurrencyController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetCurrency(string LOC_ID)
  {
    try
    {
      List<dm_TienTe> lstValue = await this._context.dm_TienTe.Where<dm_TienTe>((Expression<Func<dm_TienTe, bool>>) (e => e.LOC_ID == LOC_ID)).OrderBy<dm_TienTe, string>((Expression<Func<dm_TienTe, string>>) (e => e.MA)).ToListAsync<dm_TienTe>();
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
  public async Task<IActionResult> GetCurrency(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<dm_TienTe> lstValue = await this._context.dm_TienTe.Where<dm_TienTe>((Expression<Func<dm_TienTe, bool>>) (e => e.LOC_ID == LOC_ID)).Where<dm_TienTe>(KeyWhere, (object) ValuesSearch).OrderBy<dm_TienTe, string>((Expression<Func<dm_TienTe, string>>) (e => e.MA)).ToListAsync<dm_TienTe>();
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
  public async Task<IActionResult> GetCurrency(string LOC_ID, string ID)
  {
    try
    {
      dm_TienTe Currency = await this._context.dm_TienTe.FirstOrDefaultAsync<dm_TienTe>((Expression<Func<dm_TienTe, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Currency == null)
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
        Data = (object) Currency
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
  public async Task<IActionResult> PutCurrency(string LOC_ID, string MA, dm_TienTe Currency)
  {
    try
    {
      if (LOC_ID != Currency.LOC_ID || Currency.MA != MA)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (this.CurrencyExists(Currency))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Currency.LOC_ID}-{Currency.MA} trong dữ liệu!",
          Data = (object) ""
        });
      if (!this.CurrencyExistsID(LOC_ID, Currency.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{Currency.ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<dm_TienTe>(Currency).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      dm_TienTe OKCurrency = await this._context.dm_TienTe.FirstOrDefaultAsync<dm_TienTe>((Expression<Func<dm_TienTe, bool>>) (e => e.LOC_ID == Currency.LOC_ID && e.ID == Currency.ID));
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKCurrency
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
  public async Task<ActionResult<dm_TienTe>> PostCurrency(dm_TienTe Currency)
  {
    try
    {
      if (this.CurrencyExistsMA(Currency.LOC_ID, Currency.MA))
        return (ActionResult<dm_TienTe>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Currency.LOC_ID}-{Currency.MA} trong dữ liệu!",
          Data = (object) ""
        });
      this._context.dm_TienTe.Add(Currency);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      dm_TienTe OKCurrency = await this._context.dm_TienTe.FirstOrDefaultAsync<dm_TienTe>((Expression<Func<dm_TienTe, bool>>) (e => e.LOC_ID == Currency.LOC_ID && e.ID == Currency.ID));
      return (ActionResult<dm_TienTe>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKCurrency
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_TienTe>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteCurrency(string LOC_ID, string ID)
  {
    try
    {
      dm_TienTe Currency = await this._context.dm_TienTe.FirstOrDefaultAsync<dm_TienTe>((Expression<Func<dm_TienTe, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Currency == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_TienTe>(Currency, Currency.ID, Currency.MA);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      this._context.dm_TienTe.Remove(Currency);
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

  private bool CurrencyExistsMA(string LOC_ID, string MA)
  {
    return this._context.dm_TienTe.Any<dm_TienTe>((Expression<Func<dm_TienTe, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == MA));
  }

  private bool CurrencyExistsID(string LOC_ID, string ID)
  {
    return this._context.dm_TienTe.Any<dm_TienTe>((Expression<Func<dm_TienTe, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }

  private bool CurrencyExists(dm_TienTe Currency)
  {
    return this._context.dm_TienTe.Any<dm_TienTe>((Expression<Func<dm_TienTe, bool>>) (e => e.LOC_ID == Currency.LOC_ID && e.MA == Currency.MA && e.ID != Currency.ID));
  }
}
