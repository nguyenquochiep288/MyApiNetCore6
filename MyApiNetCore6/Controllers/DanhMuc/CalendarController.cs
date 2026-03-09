// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.CalendarController
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
public class CalendarController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public CalendarController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetArea(string LOC_ID)
  {
    try
    {
      List<dm_LichLamViec> lstValue = await this._context.dm_LichLamViec.Where<dm_LichLamViec>((Expression<Func<dm_LichLamViec, bool>>) (e => e.LOC_ID == LOC_ID)).OrderBy<dm_LichLamViec, string>((Expression<Func<dm_LichLamViec, string>>) (e => e.MA)).ToListAsync<dm_LichLamViec>();
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
      List<dm_LichLamViec> lstValue = await this._context.dm_LichLamViec.Where<dm_LichLamViec>((Expression<Func<dm_LichLamViec, bool>>) (e => e.LOC_ID == LOC_ID)).Where<dm_LichLamViec>(KeyWhere, (object) ValuesSearch).OrderBy<dm_LichLamViec, string>((Expression<Func<dm_LichLamViec, string>>) (e => e.MA)).ToListAsync<dm_LichLamViec>();
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
      dm_LichLamViec Area = await this._context.dm_LichLamViec.FirstOrDefaultAsync<dm_LichLamViec>((Expression<Func<dm_LichLamViec, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
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
  public async Task<IActionResult> PutArea(string LOC_ID, string MA, dm_LichLamViec Area)
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
      Area.GHICHU = ",";
      Area.GHICHU += Area.T2 ? "T2," : "";
      Area.GHICHU += Area.T3 ? "T3," : "";
      Area.GHICHU += Area.T4 ? "T4," : "";
      Area.GHICHU += Area.T5 ? "T5," : "";
      Area.GHICHU += Area.T6 ? "T6," : "";
      Area.GHICHU += Area.T7 ? "T7," : "";
      Area.GHICHU += Area.CN ? "CN," : "";
      this._context.Entry<dm_LichLamViec>(Area).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      dm_LichLamViec OKArea = await this._context.dm_LichLamViec.FirstOrDefaultAsync<dm_LichLamViec>((Expression<Func<dm_LichLamViec, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID));
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
  public async Task<ActionResult<dm_LichLamViec>> PostArea(dm_LichLamViec Area)
  {
    try
    {
      if (this.AreaExistsMA(Area.LOC_ID, Area.MA))
        return (ActionResult<dm_LichLamViec>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Area.LOC_ID}-{Area.MA} trong dữ liệu!",
          Data = (object) ""
        });
      Area.GHICHU = ",";
      Area.GHICHU += Area.T2 ? "T2," : "";
      Area.GHICHU += Area.T3 ? "T3," : "";
      Area.GHICHU += Area.T4 ? "T4," : "";
      Area.GHICHU += Area.T5 ? "T5," : "";
      Area.GHICHU += Area.T6 ? "T6," : "";
      Area.GHICHU += Area.T7 ? "T7," : "";
      Area.GHICHU += Area.CN ? "CN," : "";
      this._context.dm_LichLamViec.Add(Area);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      dm_LichLamViec OKArea = await this._context.dm_LichLamViec.FirstOrDefaultAsync<dm_LichLamViec>((Expression<Func<dm_LichLamViec, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID));
      return (ActionResult<dm_LichLamViec>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKArea
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_LichLamViec>) (ActionResult) this.Ok((object) new ApiResponse()
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
      dm_LichLamViec Area = await this._context.dm_LichLamViec.FirstOrDefaultAsync<dm_LichLamViec>((Expression<Func<dm_LichLamViec, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Area == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_LichLamViec>(Area, Area.ID, Area.MA);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      List<web_PhanQuyenKhuVuc> lstweb_PhanQuyenSanPham = await this._context.web_PhanQuyenKhuVuc.Where<web_PhanQuyenKhuVuc>((Expression<Func<web_PhanQuyenKhuVuc, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_LICHLAMVIEC == ID)).ToListAsync<web_PhanQuyenKhuVuc>();
      if (lstweb_PhanQuyenSanPham != null)
      {
        foreach (web_PhanQuyenKhuVuc itm in lstweb_PhanQuyenSanPham)
          this._context.web_PhanQuyenKhuVuc.Remove(itm);
      }
      this._context.dm_LichLamViec.Remove(Area);
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
    return this._context.dm_LichLamViec.Any<dm_LichLamViec>((Expression<Func<dm_LichLamViec, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == MA));
  }

  private bool AreaExistsID(string LOC_ID, string ID)
  {
    return this._context.dm_LichLamViec.Any<dm_LichLamViec>((Expression<Func<dm_LichLamViec, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }

  private bool AreaExists(dm_LichLamViec Area)
  {
    return this._context.dm_LichLamViec.Any<dm_LichLamViec>((Expression<Func<dm_LichLamViec, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.MA == Area.MA && e.ID != Area.ID));
  }
}
