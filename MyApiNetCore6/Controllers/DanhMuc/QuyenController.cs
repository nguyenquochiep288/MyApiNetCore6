// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.QuyenController
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


namespace MyApiNetCore6.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuyenController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public QuyenController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetQuyen(string LOC_ID)
  {
    try
    {
      List<view_web_Quyen> lstValue = await this._context.view_web_Quyen.Where<view_web_Quyen>((Expression<Func<view_web_Quyen, bool>>) (e => e.LOC_ID == LOC_ID)).ToListAsync<view_web_Quyen>();
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
  public async Task<IActionResult> GetQuyen(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      List<view_web_Quyen> lstValue = await this._context.view_web_Quyen.Where<view_web_Quyen>((Expression<Func<view_web_Quyen, bool>>) (e => e.LOC_ID == LOC_ID)).Where<view_web_Quyen>(KeyWhere, (object) ValuesSearch).ToListAsync<view_web_Quyen>();
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
  public async Task<IActionResult> GetQuyen(string LOC_ID, string ID)
  {
    try
    {
      view_web_Quyen Quyen = await this._context.view_web_Quyen.FirstOrDefaultAsync<view_web_Quyen>((Expression<Func<view_web_Quyen, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Quyen == null)
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
        Data = (object) Quyen
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

  [HttpPut("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PutQuyen(string LOC_ID, string ID, web_Quyen Quyen)
  {
    try
    {
      if (LOC_ID != Quyen.LOC_ID && ID != Quyen.ID)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.QuyenExists(Quyen.LOC_ID, Quyen.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<web_Quyen>(Quyen).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      view_web_Quyen OKQuyen = await this._context.view_web_Quyen.FirstOrDefaultAsync<view_web_Quyen>((Expression<Func<view_web_Quyen, bool>>) (e => e.LOC_ID == Quyen.LOC_ID && e.ID == Quyen.ID));
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKQuyen
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
  public async Task<ActionResult<web_Quyen>> PostQuyen(web_Quyen Quyen)
  {
    try
    {
      if (this.QuyenExists(Quyen.LOC_ID, Quyen.ID))
        return (ActionResult<web_Quyen>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Quyen.LOC_ID}-{Quyen.ID} trong dữ liệu!",
          Data = (object) ""
        });
      this._context.web_Quyen.Add(Quyen);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      view_web_Quyen OKQuyen = await this._context.view_web_Quyen.FirstOrDefaultAsync<view_web_Quyen>((Expression<Func<view_web_Quyen, bool>>) (e => e.LOC_ID == Quyen.LOC_ID && e.ID == Quyen.ID));
      return (ActionResult<web_Quyen>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKQuyen
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<web_Quyen>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteQuyen(string LOC_ID, string ID)
  {
    try
    {
      web_Quyen Quyen = await this._context.web_Quyen.FirstOrDefaultAsync<web_Quyen>((Expression<Func<web_Quyen, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      List<web_PhanQuyen> lstPhanQuyen = await this._context.web_PhanQuyen.Where<web_PhanQuyen>((Expression<Func<web_PhanQuyen, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_QUYEN == ID)).ToListAsync<web_PhanQuyen>();
      if (Quyen == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      if (lstPhanQuyen != null)
      {
        foreach (web_PhanQuyen web_PhanQuyen in lstPhanQuyen)
          this._context.web_PhanQuyen.Remove(web_PhanQuyen);
      }
      this._context.web_Quyen.Remove(Quyen);
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

  private bool QuyenExists(string LOC_ID, string ID)
  {
    return this._context.web_Quyen.Any<web_Quyen>((Expression<Func<web_Quyen, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }
}
