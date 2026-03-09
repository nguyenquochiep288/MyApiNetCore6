// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.AreaController
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
public class AreaController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public AreaController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetArea(string LOC_ID)
  {
    try
    {
      List<dm_KhuVuc> lstValue = await this._context.dm_KhuVuc.Where<dm_KhuVuc>((Expression<Func<dm_KhuVuc, bool>>) (e => e.LOC_ID == LOC_ID)).OrderBy<dm_KhuVuc, string>((Expression<Func<dm_KhuVuc, string>>) (e => e.MA)).ToListAsync<dm_KhuVuc>();
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
  public async Task<IActionResult> GetArea(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<dm_KhuVuc> lstValue = await this._context.dm_KhuVuc.Where<dm_KhuVuc>((Expression<Func<dm_KhuVuc, bool>>) (e => e.LOC_ID == LOC_ID)).Where<dm_KhuVuc>(KeyWhere, (object) ValuesSearch).OrderBy<dm_KhuVuc, string>((Expression<Func<dm_KhuVuc, string>>) (e => e.MA)).ToListAsync<dm_KhuVuc>();
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
  public async Task<IActionResult> GetArea(string LOC_ID, string ID)
  {
    try
    {
      dm_KhuVuc Area = await this._context.dm_KhuVuc.FirstOrDefaultAsync<dm_KhuVuc>((Expression<Func<dm_KhuVuc, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Area == null)
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
        Data = (object) Area
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
  public async Task<IActionResult> PutArea(string LOC_ID, string MA, dm_KhuVuc Area)
  {
    try
    {
      if (LOC_ID != Area.LOC_ID || Area.MA != MA)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (this.AreaExists(Area))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Area.LOC_ID}-{Area.MA} trong dữ liệu!",
          Data = (object) ""
        });
      if (!this.AreaExistsID(LOC_ID, Area.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{Area.ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<dm_KhuVuc>(Area).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      dm_KhuVuc OKArea = await this._context.dm_KhuVuc.FirstOrDefaultAsync<dm_KhuVuc>((Expression<Func<dm_KhuVuc, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID));
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKArea
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
  public async Task<ActionResult<dm_KhuVuc>> PostArea(dm_KhuVuc Area)
  {
    try
    {
      if (this.AreaExistsMA(Area.LOC_ID, Area.MA))
        return (ActionResult<dm_KhuVuc>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Area.LOC_ID}-{Area.MA} trong dữ liệu!",
          Data = (object) ""
        });
      this._context.dm_KhuVuc.Add(Area);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      dm_KhuVuc OKArea = await this._context.dm_KhuVuc.FirstOrDefaultAsync<dm_KhuVuc>((Expression<Func<dm_KhuVuc, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID));
      return (ActionResult<dm_KhuVuc>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKArea
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_KhuVuc>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteArea(string LOC_ID, string ID)
  {
    try
    {
      dm_KhuVuc Area = await this._context.dm_KhuVuc.FirstOrDefaultAsync<dm_KhuVuc>((Expression<Func<dm_KhuVuc, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Area == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_KhuVuc>(Area, Area.ID, Area.MA);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      List<web_PhanQuyenKhuVuc> lstweb_PhanQuyenSanPham = await this._context.web_PhanQuyenKhuVuc.Where<web_PhanQuyenKhuVuc>((Expression<Func<web_PhanQuyenKhuVuc, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_KHUVUC == ID)).ToListAsync<web_PhanQuyenKhuVuc>();
      if (lstweb_PhanQuyenSanPham != null)
      {
        foreach (web_PhanQuyenKhuVuc itm in lstweb_PhanQuyenSanPham)
          this._context.web_PhanQuyenKhuVuc.Remove(itm);
      }
      this._context.dm_KhuVuc.Remove(Area);
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

  private bool AreaExistsMA(string LOC_ID, string MA)
  {
    return this._context.dm_KhuVuc.Any<dm_KhuVuc>((Expression<Func<dm_KhuVuc, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == MA));
  }

  private bool AreaExistsID(string LOC_ID, string ID)
  {
    return this._context.dm_KhuVuc.Any<dm_KhuVuc>((Expression<Func<dm_KhuVuc, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }

  private bool AreaExists(dm_KhuVuc Area)
  {
    return this._context.dm_KhuVuc.Any<dm_KhuVuc>((Expression<Func<dm_KhuVuc, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.MA == Area.MA && e.ID != Area.ID));
  }
}
