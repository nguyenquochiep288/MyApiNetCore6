// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.UnitController
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
public class UnitController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public UnitController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetUnit(string LOC_ID)
  {
    try
    {
      List<dm_DonViTinh> lstValue = await this._context.dm_DonViTinh.Where<dm_DonViTinh>((Expression<Func<dm_DonViTinh, bool>>) (e => e.LOC_ID == LOC_ID)).OrderBy<dm_DonViTinh, string>((Expression<Func<dm_DonViTinh, string>>) (e => e.MA)).ToListAsync<dm_DonViTinh>();
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
  public async Task<IActionResult> GetUnit(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<dm_DonViTinh> lstValue = await this._context.dm_DonViTinh.Where<dm_DonViTinh>((Expression<Func<dm_DonViTinh, bool>>) (e => e.LOC_ID == LOC_ID)).Where<dm_DonViTinh>(KeyWhere, (object) ValuesSearch).OrderBy<dm_DonViTinh, string>((Expression<Func<dm_DonViTinh, string>>) (e => e.MA)).ToListAsync<dm_DonViTinh>();
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
  public async Task<IActionResult> GetUnit(string LOC_ID, string ID)
  {
    try
    {
      dm_DonViTinh Unit = await this._context.dm_DonViTinh.FirstOrDefaultAsync<dm_DonViTinh>((Expression<Func<dm_DonViTinh, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Unit == null)
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
        Data = (object) Unit
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
  public async Task<IActionResult> PutUnit(string LOC_ID, string MA, dm_DonViTinh Unit)
  {
    try
    {
      if (this.UnitExists(Unit))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Unit.LOC_ID}-{Unit.MA} trong dữ liệu!",
          Data = (object) ""
        });
      if (LOC_ID != Unit.LOC_ID || Unit.MA != MA)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.UnitExistsID(LOC_ID, Unit.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{Unit.ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<dm_DonViTinh>(Unit).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) Unit
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
  public async Task<ActionResult<dm_DonViTinh>> PostUnit(dm_DonViTinh Unit)
  {
    try
    {
      if (this.UnitExistsMA(Unit.LOC_ID, Unit.MA))
        return (ActionResult<dm_DonViTinh>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Unit.LOC_ID}-{Unit.MA} trong dữ liệu!",
          Data = (object) ""
        });
      this._context.dm_DonViTinh.Add(Unit);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      return (ActionResult<dm_DonViTinh>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) Unit
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_DonViTinh>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteUnit(string LOC_ID, string ID)
  {
    try
    {
      dm_DonViTinh Unit = await this._context.dm_DonViTinh.FirstOrDefaultAsync<dm_DonViTinh>((Expression<Func<dm_DonViTinh, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Unit == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_DonViTinh>(Unit, Unit.ID, Unit.NAME);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      this._context.dm_DonViTinh.Remove(Unit);
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

  private bool UnitExistsMA(string LOC_ID, string MA)
  {
    return this._context.dm_DonViTinh.Any<dm_DonViTinh>((Expression<Func<dm_DonViTinh, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == MA));
  }

  private bool UnitExistsID(string LOC_ID, string ID)
  {
    return this._context.dm_DonViTinh.Any<dm_DonViTinh>((Expression<Func<dm_DonViTinh, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }

  private bool UnitExists(dm_DonViTinh Position)
  {
    return this._context.dm_DonViTinh.Any<dm_DonViTinh>((Expression<Func<dm_DonViTinh, bool>>) (e => e.LOC_ID == Position.LOC_ID && e.MA == Position.MA && e.ID != Position.ID));
  }
}
