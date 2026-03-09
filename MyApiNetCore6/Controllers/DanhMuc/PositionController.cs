// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.PositionController
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
public class PositionController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public PositionController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetPosition(string LOC_ID)
  {
    try
    {
      List<dm_ChucVu> lstValue = await this._context.dm_ChucVu.Where<dm_ChucVu>((Expression<Func<dm_ChucVu, bool>>) (e => e.LOC_ID == LOC_ID)).OrderBy<dm_ChucVu, string>((Expression<Func<dm_ChucVu, string>>) (e => e.MA)).ToListAsync<dm_ChucVu>();
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
  public async Task<IActionResult> GetPosition(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<dm_ChucVu> lstValue = await this._context.dm_ChucVu.Where<dm_ChucVu>((Expression<Func<dm_ChucVu, bool>>) (e => e.LOC_ID == LOC_ID)).Where<dm_ChucVu>(KeyWhere, (object) ValuesSearch).OrderBy<dm_ChucVu, string>((Expression<Func<dm_ChucVu, string>>) (e => e.MA)).ToListAsync<dm_ChucVu>();
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
  public async Task<IActionResult> GetPosition(string LOC_ID, string ID)
  {
    try
    {
      dm_ChucVu Position = await this._context.dm_ChucVu.FirstOrDefaultAsync<dm_ChucVu>((Expression<Func<dm_ChucVu, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Position == null)
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
        Data = (object) Position
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
  public async Task<IActionResult> PutPosition(string LOC_ID, string MA, dm_ChucVu Position)
  {
    try
    {
      if (this.PositionExists(Position))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Position.LOC_ID}-{Position.MA} trong dữ liệu!",
          Data = (object) ""
        });
      if (LOC_ID != Position.LOC_ID || Position.MA != MA)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.PositionExistsID(LOC_ID, Position.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{Position.ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<dm_ChucVu>(Position).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      dm_TienTe OKPosition = await this._context.dm_TienTe.FirstOrDefaultAsync<dm_TienTe>((Expression<Func<dm_TienTe, bool>>) (e => e.LOC_ID == Position.LOC_ID && e.ID == Position.ID));
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKPosition
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
  public async Task<ActionResult<dm_ChucVu>> PostPosition(dm_ChucVu Position)
  {
    try
    {
      if (this.PositionExistsMA(Position.LOC_ID, Position.MA))
        return (ActionResult<dm_ChucVu>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Position.LOC_ID}-{Position.MA} trong dữ liệu!",
          Data = (object) ""
        });
      this._context.dm_ChucVu.Add(Position);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      dm_TienTe OKPosition = await this._context.dm_TienTe.FirstOrDefaultAsync<dm_TienTe>((Expression<Func<dm_TienTe, bool>>) (e => e.LOC_ID == Position.LOC_ID && e.ID == Position.ID));
      return (ActionResult<dm_ChucVu>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKPosition
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_ChucVu>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeletePosition(string LOC_ID, string ID)
  {
    try
    {
      dm_ChucVu Position = await this._context.dm_ChucVu.FirstOrDefaultAsync<dm_ChucVu>((Expression<Func<dm_ChucVu, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Position == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_ChucVu>(Position, Position.ID, Position.NAME);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      this._context.dm_ChucVu.Remove(Position);
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

  private bool PositionExistsMA(string LOC_ID, string MA)
  {
    return this._context.dm_ChucVu.Any<dm_ChucVu>((Expression<Func<dm_ChucVu, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == MA));
  }

  private bool PositionExistsID(string LOC_ID, string ID)
  {
    return this._context.dm_ChucVu.Any<dm_ChucVu>((Expression<Func<dm_ChucVu, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }

  private bool PositionExists(dm_ChucVu Position)
  {
    return this._context.dm_ChucVu.Any<dm_ChucVu>((Expression<Func<dm_ChucVu, bool>>) (e => e.LOC_ID == Position.LOC_ID && e.MA == Position.MA && e.ID != Position.ID));
  }
}
