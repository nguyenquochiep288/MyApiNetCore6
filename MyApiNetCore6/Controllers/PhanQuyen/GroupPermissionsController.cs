// Decompiled with JetBrains decompiler
// Type: API_QuanLyTHP.Controllers.PhanQuyen.GroupPermissionsController
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using DatabaseTHP;
using DatabaseTHP.Class;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyApiNetCore6.Controllers;
using MyApiNetCore6.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace API_QuanLyTHP.Controllers.PhanQuyen;

[Route("api/[controller]")]
[ApiController]
public class GroupPermissionsController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public GroupPermissionsController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetGroupPermissions(string LOC_ID)
  {
    try
    {
      List<web_NhomQuyen> lstValue = await this._context.web_NhomQuyen.Where<web_NhomQuyen>((Expression<Func<web_NhomQuyen, bool>>) (e => e.LOC_ID == LOC_ID)).ToListAsync<web_NhomQuyen>();
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
  public async Task<IActionResult> GetGroupPermissions(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      List<web_NhomQuyen> lstValue = await this._context.web_NhomQuyen.Where<web_NhomQuyen>((Expression<Func<web_NhomQuyen, bool>>) (e => e.LOC_ID == LOC_ID)).Where<web_NhomQuyen>(KeyWhere, (object) ValuesSearch).ToListAsync<web_NhomQuyen>();
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
  public async Task<IActionResult> GetGroupPermissions(string LOC_ID, string ID)
  {
    try
    {
      web_NhomQuyen GroupPermissions = await this._context.web_NhomQuyen.FirstOrDefaultAsync<web_NhomQuyen>((Expression<Func<web_NhomQuyen, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (GroupPermissions == null)
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
        Data = (object) GroupPermissions
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
  public async Task<IActionResult> PutGroupPermissions(
    string LOC_ID,
    string MA,
    web_NhomQuyen GroupPermissions)
  {
    try
    {
      if (LOC_ID != GroupPermissions.LOC_ID || GroupPermissions.MA != MA)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.GroupPermissionsExistsID(LOC_ID, GroupPermissions.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{GroupPermissions.ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<web_NhomQuyen>(GroupPermissions).State = EntityState.Modified;
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
  public async Task<ActionResult<web_NhomQuyen>> PostGroupPermissions(web_NhomQuyen GroupPermissions)
  {
    try
    {
      if (this.GroupPermissionsExistsMA(GroupPermissions.LOC_ID, GroupPermissions.MA))
        return (ActionResult<web_NhomQuyen>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{GroupPermissions.LOC_ID}-{GroupPermissions.MA} trong dữ liệu!",
          Data = (object) ""
        });
      this._context.web_NhomQuyen.Add(GroupPermissions);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      return (ActionResult<web_NhomQuyen>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) GroupPermissions
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<web_NhomQuyen>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteGroupPermissions(string LOC_ID, string ID)
  {
    try
    {
      web_NhomQuyen GroupPermissions = await this._context.web_NhomQuyen.FirstOrDefaultAsync<web_NhomQuyen>((Expression<Func<web_NhomQuyen, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (GroupPermissions == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.web_NhomQuyen.Remove(GroupPermissions);
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

  private bool GroupPermissionsExistsMA(string LOC_ID, string MA)
  {
    return this._context.web_NhomQuyen.Any<web_NhomQuyen>((Expression<Func<web_NhomQuyen, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == MA));
  }

  private bool GroupPermissionsExistsID(string LOC_ID, string ID)
  {
    return this._context.web_NhomQuyen.Any<web_NhomQuyen>((Expression<Func<web_NhomQuyen, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }
}
