// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.ProviderController
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
public class ProviderController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public ProviderController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetProvider(string LOC_ID)
  {
    try
    {
      List<view_dm_NhaCungCap> lstValue = await this._context.view_dm_NhaCungCap.Where<view_dm_NhaCungCap>((Expression<Func<view_dm_NhaCungCap, bool>>) (e => e.LOC_ID == LOC_ID)).OrderBy<view_dm_NhaCungCap, string>((Expression<Func<view_dm_NhaCungCap, string>>) (e => e.MA)).ToListAsync<view_dm_NhaCungCap>();
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
  public async Task<IActionResult> GetProvider(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<view_dm_NhaCungCap> lstValue = await this._context.view_dm_NhaCungCap.Where<view_dm_NhaCungCap>((Expression<Func<view_dm_NhaCungCap, bool>>) (e => e.LOC_ID == LOC_ID)).Where<view_dm_NhaCungCap>(KeyWhere, (object) ValuesSearch).OrderBy<view_dm_NhaCungCap, string>((Expression<Func<view_dm_NhaCungCap, string>>) (e => e.MA)).ToListAsync<view_dm_NhaCungCap>();
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
  public async Task<IActionResult> GetProvider(string LOC_ID, string ID)
  {
    try
    {
      view_dm_NhaCungCap Provider = await this._context.view_dm_NhaCungCap.FirstOrDefaultAsync<view_dm_NhaCungCap>((Expression<Func<view_dm_NhaCungCap, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Provider == null)
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
        Data = (object) Provider
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
  public async Task<IActionResult> PutProvider(string LOC_ID, string MA, dm_NhaCungCap Provider)
  {
    try
    {
      if (this.ProviderExists(Provider))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Provider.LOC_ID}-{Provider.MA} trong dữ liệu!",
          Data = (object) ""
        });
      if (LOC_ID != Provider.LOC_ID || Provider.MA != MA)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.ProviderExistsID(LOC_ID, Provider.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{Provider.ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<dm_NhaCungCap>(Provider).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      view_dm_NhaCungCap OKProvider = await this._context.view_dm_NhaCungCap.FirstOrDefaultAsync<view_dm_NhaCungCap>((Expression<Func<view_dm_NhaCungCap, bool>>) (e => e.LOC_ID == Provider.LOC_ID && e.ID == Provider.ID));
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKProvider
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
  public async Task<ActionResult<dm_NhaCungCap>> PostProvider(dm_NhaCungCap Provider)
  {
    try
    {
      if (this.ProviderExistsMA(Provider.LOC_ID, Provider.MA))
        return (ActionResult<dm_NhaCungCap>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Provider.LOC_ID}-{Provider.MA} trong dữ liệu!",
          Data = (object) ""
        });
      this._context.dm_NhaCungCap.Add(Provider);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      view_dm_NhaCungCap OKProvider = await this._context.view_dm_NhaCungCap.FirstOrDefaultAsync<view_dm_NhaCungCap>((Expression<Func<view_dm_NhaCungCap, bool>>) (e => e.LOC_ID == Provider.LOC_ID && e.ID == Provider.ID));
      return (ActionResult<dm_NhaCungCap>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKProvider
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_NhaCungCap>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteProvider(string LOC_ID, string ID)
  {
    try
    {
      dm_NhaCungCap Provider = await this._context.dm_NhaCungCap.FirstOrDefaultAsync<dm_NhaCungCap>((Expression<Func<dm_NhaCungCap, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Provider == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_NhaCungCap>(Provider, Provider.ID, Provider.NAME);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      this._context.dm_NhaCungCap.Remove(Provider);
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

  private bool ProviderExistsMA(string LOC_ID, string MA)
  {
    return this._context.dm_NhaCungCap.Any<dm_NhaCungCap>((Expression<Func<dm_NhaCungCap, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == MA));
  }

  private bool ProviderExistsID(string LOC_ID, string ID)
  {
    return this._context.dm_NhaCungCap.Any<dm_NhaCungCap>((Expression<Func<dm_NhaCungCap, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }

  private bool ProviderExists(dm_NhaCungCap Position)
  {
    return this._context.dm_NhaCungCap.Any<dm_NhaCungCap>((Expression<Func<dm_NhaCungCap, bool>>) (e => e.LOC_ID == Position.LOC_ID && e.MA == Position.MA && e.ID != Position.ID));
  }
}
