// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.TimekeepingController
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
public class TimekeepingController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public TimekeepingController(dbTrangHiepPhatContext context, IConfiguration configuration)
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
      List<nv_ChamCong> lstValue = await this._context.nv_ChamCong.Where<nv_ChamCong>((Expression<Func<nv_ChamCong, bool>>) (e => e.LOC_ID == LOC_ID)).OrderBy<nv_ChamCong, DateTime>((Expression<Func<nv_ChamCong, DateTime>>) (e => e.NGAYCONG)).ToListAsync<nv_ChamCong>();
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
      List<nv_ChamCong> lstValue = await this._context.nv_ChamCong.Where<nv_ChamCong>((Expression<Func<nv_ChamCong, bool>>) (e => e.LOC_ID == LOC_ID)).Where<nv_ChamCong>(KeyWhere, (object) ValuesSearch).OrderBy<nv_ChamCong, DateTime>((Expression<Func<nv_ChamCong, DateTime>>) (e => e.NGAYCONG)).ToListAsync<nv_ChamCong>();
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
      nv_ChamCong Area = await this._context.nv_ChamCong.FirstOrDefaultAsync<nv_ChamCong>((Expression<Func<nv_ChamCong, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
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

  [HttpPut("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PutArea(string LOC_ID, string ID, nv_ChamCong Area)
  {
    try
    {
      if (!this.AreaExistsID(LOC_ID, Area.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{Area.ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<nv_ChamCong>(Area).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      nv_ChamCong OKArea = await this._context.nv_ChamCong.FirstOrDefaultAsync<nv_ChamCong>((Expression<Func<nv_ChamCong, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID));
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

  [HttpPost("PostCheckIn")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PostCheckIn(nv_ChamCong Area)
  {
    try
    {
      nv_ChamCong nv_ChamCong = await this._context.nv_ChamCong.FirstOrDefaultAsync<nv_ChamCong>((Expression<Func<nv_ChamCong, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.NGAYCONG.Date == Area.NGAYCONG.Date && e.ID_NHANVIEN == Area.ID_NHANVIEN));
      if (nv_ChamCong != null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại {nv_ChamCong.LOC_ID}-{nv_ChamCong.ID} trong dữ liệu!(Đã có dữ liệu chấm công ngày {Area.NGAYCONG.ToString("dd/MM/yyyy")} )",
          Data = (object) ""
        });
      this._context.nv_ChamCong.Add(Area);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      nv_ChamCong OKArea = await this._context.nv_ChamCong.FirstOrDefaultAsync<nv_ChamCong>((Expression<Func<nv_ChamCong, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID));
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

  [HttpPost("PostCheckOut")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PostCheckOut(nv_ChamCong Area)
  {
    try
    {
      if (!this.AreaExistsID(Area.LOC_ID, Area.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {Area.LOC_ID}-{Area.ID} dữ liệu!",
          Data = (object) ""
        });
      nv_ChamCong nv_ChamCong = await this._context.nv_ChamCong.FirstOrDefaultAsync<nv_ChamCong>((Expression<Func<nv_ChamCong, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID));
      if (nv_ChamCong != null)
      {
        if (nv_ChamCong.ID_NHANVIEN != Area.ID_NHANVIEN)
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = "Dữ liệu chấm công khác nhau!",
            Data = (object) ""
          });
        nv_ChamCong.ID_NGUOISUA = Area.ID_NGUOISUA;
        nv_ChamCong.THOIGIANSUA = Area.THOIGIANSUA;
        nv_ChamCong.THOIGIANRA = Area.THOIGIANRA;
        nv_ChamCong.IP_CHAMCONGRA = Area.IP_CHAMCONGRA;
        string GHICHU = nv_ChamCong.GHICHU + Area.GHICHU;
        nv_ChamCong.GHICHU = GHICHU.Length <= 250 ? GHICHU : GHICHU.Substring(0, 249);
        GHICHU = (string) null;
      }
      this._context.Entry<nv_ChamCong>(nv_ChamCong).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      nv_ChamCong OKArea = await this._context.nv_ChamCong.FirstOrDefaultAsync<nv_ChamCong>((Expression<Func<nv_ChamCong, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID));
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
  public async Task<ActionResult<nv_ChamCong>> PostArea(nv_ChamCong Area)
  {
    try
    {
      if (this.AreaExistsID(Area.LOC_ID, Area.ID))
        return (ActionResult<nv_ChamCong>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại {Area.LOC_ID}-{Area.ID} trong dữ liệu!",
          Data = (object) ""
        });
      nv_ChamCong nv_ChamCong = await this._context.nv_ChamCong.FirstOrDefaultAsync<nv_ChamCong>((Expression<Func<nv_ChamCong, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.NGAYCONG.Date == Area.NGAYCONG.Date && e.ID_NHANVIEN == Area.ID_NHANVIEN));
      if (nv_ChamCong != null)
        return (ActionResult<nv_ChamCong>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại {nv_ChamCong.LOC_ID}-{nv_ChamCong.ID} trong dữ liệu!(Đã có dữ liệu chấm công ngày {Area.NGAYCONG.ToString("dd/MM/yyyy")} )",
          Data = (object) ""
        });
      this._context.nv_ChamCong.Add(Area);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      nv_ChamCong OKArea = await this._context.nv_ChamCong.FirstOrDefaultAsync<nv_ChamCong>((Expression<Func<nv_ChamCong, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID));
      return (ActionResult<nv_ChamCong>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKArea
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<nv_ChamCong>) (ActionResult) this.Ok((object) new ApiResponse()
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
      nv_ChamCong Area = await this._context.nv_ChamCong.FirstOrDefaultAsync<nv_ChamCong>((Expression<Func<nv_ChamCong, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Area == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.nv_ChamCong.Remove(Area);
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

  private bool AreaExistsID(string LOC_ID, string ID)
  {
    return this._context.nv_ChamCong.Any<nv_ChamCong>((Expression<Func<nv_ChamCong, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }
}
