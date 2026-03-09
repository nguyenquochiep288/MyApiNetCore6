// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.GroupProviderController
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
public class GroupProviderController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public GroupProviderController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetGroupProvider(string LOC_ID)
  {
    try
    {
      List<dm_NhomNhaCungCap> lstValue = await this._context.dm_NhomNhaCungCap.Where<dm_NhomNhaCungCap>((Expression<Func<dm_NhomNhaCungCap, bool>>) (e => e.LOC_ID == LOC_ID)).OrderBy<dm_NhomNhaCungCap, string>((Expression<Func<dm_NhomNhaCungCap, string>>) (e => e.MA)).ToListAsync<dm_NhomNhaCungCap>();
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
  public async Task<IActionResult> GetGroupCusVen(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<dm_NhomNhaCungCap> lstValue = await this._context.dm_NhomNhaCungCap.Where<dm_NhomNhaCungCap>((Expression<Func<dm_NhomNhaCungCap, bool>>) (e => e.LOC_ID == LOC_ID)).Where<dm_NhomNhaCungCap>(KeyWhere, (object) ValuesSearch).OrderBy<dm_NhomNhaCungCap, string>((Expression<Func<dm_NhomNhaCungCap, string>>) (e => e.MA)).ToListAsync<dm_NhomNhaCungCap>();
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
  public async Task<IActionResult> GetGroupCusVen(string LOC_ID, string ID)
  {
    try
    {
      dm_NhomNhaCungCap GroupCusVen = await this._context.dm_NhomNhaCungCap.FirstOrDefaultAsync<dm_NhomNhaCungCap>((Expression<Func<dm_NhomNhaCungCap, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (GroupCusVen == null)
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
        Data = (object) GroupCusVen
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
  public async Task<IActionResult> PutGroupCusVen(
    string LOC_ID,
    string MA,
    dm_NhomNhaCungCap GroupCusVen)
  {
    try
    {
      if (this.GroupCusVenExists(GroupCusVen))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{GroupCusVen.LOC_ID}-{GroupCusVen.MA} trong dữ liệu!",
          Data = (object) ""
        });
      if (LOC_ID != GroupCusVen.LOC_ID || GroupCusVen.MA != MA)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.GroupCusVenExistsID(LOC_ID, GroupCusVen.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{GroupCusVen.ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<dm_NhomNhaCungCap>(GroupCusVen).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) GroupCusVen
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
  public async Task<ActionResult<dm_NhomNhaCungCap>> PostGroupCusVen(dm_NhomNhaCungCap GroupCusVen)
  {
    try
    {
      if (this.GroupCusVenExistsMA(GroupCusVen.LOC_ID, GroupCusVen.MA))
        return (ActionResult<dm_NhomNhaCungCap>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{GroupCusVen.LOC_ID}-{GroupCusVen.MA} trong dữ liệu!",
          Data = (object) ""
        });
      this._context.dm_NhomNhaCungCap.Add(GroupCusVen);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      return (ActionResult<dm_NhomNhaCungCap>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) GroupCusVen
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_NhomNhaCungCap>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteGroupCusVen(string LOC_ID, string ID)
  {
    try
    {
      dm_NhomNhaCungCap GroupCusVen = await this._context.dm_NhomNhaCungCap.FirstOrDefaultAsync<dm_NhomNhaCungCap>((Expression<Func<dm_NhomNhaCungCap, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (GroupCusVen == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_NhomNhaCungCap>(GroupCusVen, GroupCusVen.ID, GroupCusVen.MA);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      this._context.dm_NhomNhaCungCap.Remove(GroupCusVen);
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

  private bool GroupCusVenExistsMA(string LOC_ID, string MA)
  {
    return this._context.dm_NhomNhaCungCap.Any<dm_NhomNhaCungCap>((Expression<Func<dm_NhomNhaCungCap, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == MA));
  }

  private bool GroupCusVenExistsID(string LOC_ID, string ID)
  {
    return this._context.dm_NhomNhaCungCap.Any<dm_NhomNhaCungCap>((Expression<Func<dm_NhomNhaCungCap, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }

  private bool GroupCusVenExists(dm_NhomNhaCungCap GroupCusVen)
  {
    return this._context.dm_NhomNhaCungCap.Any<dm_NhomNhaCungCap>((Expression<Func<dm_NhomNhaCungCap, bool>>) (e => e.LOC_ID == GroupCusVen.LOC_ID && e.MA == GroupCusVen.MA && e.ID != GroupCusVen.ID));
  }
}
