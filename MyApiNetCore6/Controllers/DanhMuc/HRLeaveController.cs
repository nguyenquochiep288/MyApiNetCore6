// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.HRLeaveController
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using DatabaseTHP;
using DatabaseTHP.Class;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
public class HRLeaveController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public HRLeaveController(dbTrangHiepPhatContext context, IConfiguration configuration)
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
      List<nv_NghiPhep> lstValue = await this._context.nv_NghiPhep.Where<nv_NghiPhep>((Expression<Func<nv_NghiPhep, bool>>) (e => e.LOC_ID == LOC_ID)).OrderBy<nv_NghiPhep, DateTime>((Expression<Func<nv_NghiPhep, DateTime>>) (e => e.THOIGIANVAO)).ToListAsync<nv_NghiPhep>();
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
      List<nv_NghiPhep> lstValue = await this._context.nv_NghiPhep.Where<nv_NghiPhep>((Expression<Func<nv_NghiPhep, bool>>) (e => e.LOC_ID == LOC_ID)).Where<nv_NghiPhep>(KeyWhere, (object) ValuesSearch).OrderBy<nv_NghiPhep, DateTime>((Expression<Func<nv_NghiPhep, DateTime>>) (e => e.THOIGIANVAO)).ToListAsync<nv_NghiPhep>();
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
      nv_NghiPhep Area = await this._context.nv_NghiPhep.FirstOrDefaultAsync<nv_NghiPhep>((Expression<Func<nv_NghiPhep, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
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
  public async Task<IActionResult> PutArea(string LOC_ID, string ID, nv_NghiPhep Area)
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
      nv_NghiPhep nv_NghiPhepOld = await this._context.nv_NghiPhep.AsNoTracking<nv_NghiPhep>().FirstOrDefaultAsync<nv_NghiPhep>((Expression<Func<nv_NghiPhep, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID));
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        if (nv_NghiPhepOld != null)
        {
          if (nv_NghiPhepOld.ID_PHEPNAM != Area.ID_PHEPNAM)
          {
            nv_PhepNam nv_PhepNamOld = await this._context.nv_PhepNam.FirstOrDefaultAsync<nv_PhepNam>((Expression<Func<nv_PhepNam, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID == nv_NghiPhepOld.ID_PHEPNAM));
            if (nv_PhepNamOld == null)
              return (IActionResult) this.Ok((object) new ApiResponse()
              {
                Success = false,
                Message = $"Không tìm thấy {nv_NghiPhepOld.LOC_ID}-{nv_NghiPhepOld.ID} dữ liệu phép năm!",
                Data = (object) ""
              });
            nv_PhepNamOld.SONGAYPHEPDADUNG += nv_NghiPhepOld.SOLUONG;
            this._context.Entry<nv_PhepNam>(nv_PhepNamOld).State = EntityState.Modified;
            nv_PhepNam nv_PhepNam = await this._context.nv_PhepNam.FirstOrDefaultAsync<nv_PhepNam>((Expression<Func<nv_PhepNam, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID_PHEPNAM));
            if (nv_PhepNam == null)
              return (IActionResult) this.Ok((object) new ApiResponse()
              {
                Success = false,
                Message = $"Không tìm thấy {Area.LOC_ID}-{Area.ID_PHEPNAM} dữ liệu phép năm!",
                Data = (object) ""
              });
            if (Area.ISNGHIPHEP && nv_PhepNam.SONGAYPHEP - nv_PhepNam.SONGAYPHEPDADUNG < Area.SOLUONG)
              return (IActionResult) this.Ok((object) new ApiResponse()
              {
                Success = false,
                Message = $"Số lượng ngày còn lại phép không đủ!{(nv_PhepNam.SONGAYPHEP - nv_PhepNam.SONGAYPHEPDADUNG).ToString()} ngày",
                Data = (object) ""
              });
            if (Area.ISNGHIPHEP && nv_PhepNam.NGAYBATDAU >= Area.THOIGIANVAO && nv_PhepNam.NGAYKETTHUC >= Area.THOIGIANRA)
              return (IActionResult) this.Ok((object) new ApiResponse()
              {
                Success = false,
                Message = "Ngày bắt đầu và ngày kết thúc không nằm trong khoảng thời gian cho tạo phép!",
                Data = (object) ""
              });
            nv_PhepNam.SONGAYPHEPDADUNG -= nv_NghiPhepOld.SOLUONG;
            this._context.Entry<nv_PhepNam>(nv_PhepNam).State = EntityState.Modified;
            nv_PhepNamOld = (nv_PhepNam) null;
            nv_PhepNam = (nv_PhepNam) null;
          }
          else
          {
            nv_PhepNam nv_PhepNam = await this._context.nv_PhepNam.FirstOrDefaultAsync<nv_PhepNam>((Expression<Func<nv_PhepNam, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID_PHEPNAM));
            if (nv_PhepNam == null)
              return (IActionResult) this.Ok((object) new ApiResponse()
              {
                Success = false,
                Message = $"Không tìm thấy {Area.LOC_ID}-{Area.ID_PHEPNAM} dữ liệu phép năm!",
                Data = (object) ""
              });
            nv_PhepNam.SONGAYPHEPDADUNG -= nv_NghiPhepOld.SOLUONG;
            if (Area.ISNGHIPHEP && nv_PhepNam.SONGAYPHEP - nv_PhepNam.SONGAYPHEPDADUNG < Area.SOLUONG)
              return (IActionResult) this.Ok((object) new ApiResponse()
              {
                Success = false,
                Message = $"Số lượng ngày còn lại phép không đủ!{(nv_PhepNam.SONGAYPHEP - nv_PhepNam.SONGAYPHEPDADUNG).ToString()} ngày",
                Data = (object) ""
              });
            if (Area.ISNGHIPHEP && nv_PhepNam.NGAYBATDAU >= Area.THOIGIANVAO && nv_PhepNam.NGAYKETTHUC >= Area.THOIGIANRA)
              return (IActionResult) this.Ok((object) new ApiResponse()
              {
                Success = false,
                Message = "Ngày bắt đầu và ngày kết thúc không nằm trong khoảng thời gian cho tạo phép!",
                Data = (object) ""
              });
            nv_PhepNam.SONGAYPHEPDADUNG += Area.SOLUONG;
            this._context.Entry<nv_PhepNam>(nv_PhepNam).State = EntityState.Modified;
            nv_PhepNam = (nv_PhepNam) null;
          }
        }
        this._context.Entry<nv_NghiPhep>(Area).State = EntityState.Modified;
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        auditLog = (AuditLogController) null;
        transaction.Commit();
        nv_NghiPhep OKArea = await this._context.nv_NghiPhep.FirstOrDefaultAsync<nv_NghiPhep>((Expression<Func<nv_NghiPhep, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID));
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) OKArea
        });
      }
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
  public async Task<ActionResult<nv_NghiPhep>> PostArea(nv_NghiPhep Area)
  {
    try
    {
      if (this.AreaExistsID(Area.LOC_ID, Area.ID))
        return (ActionResult<nv_NghiPhep>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại {Area.LOC_ID}-{Area.ID} trong dữ liệu!",
          Data = (object) ""
        });
      int DayInterval = 0;
      DateTime StartDate = Area.THOIGIANVAO;
      while (Area.THOIGIANVAO.AddDays((double) DayInterval) <= Area.THOIGIANRA)
      {
        StartDate = StartDate.AddDays((double) DayInterval);
        nv_NghiPhep nv_NghiPhep = await this._context.nv_NghiPhep.FirstOrDefaultAsync<nv_NghiPhep>((Expression<Func<nv_NghiPhep, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.THOIGIANVAO.Date >= StartDate.Date && e.THOIGIANRA.Date <= StartDate.Date && e.ID_NHANVIEN == Area.ID_NHANVIEN));
        if (nv_NghiPhep != null)
          return (ActionResult<nv_NghiPhep>) (ActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = $"Đã tồn tại {nv_NghiPhep.LOC_ID}-{nv_NghiPhep.ID} trong dữ liệu!(Đã có đơn nghĩ phép ngày {StartDate.ToString("dd/MM/yyyy")} )",
            Data = (object) ""
          });
        ++DayInterval;
        nv_NghiPhep = (nv_NghiPhep) null;
      }
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        nv_PhepNam nv_PhepNam = await this._context.nv_PhepNam.FirstOrDefaultAsync<nv_PhepNam>((Expression<Func<nv_PhepNam, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID_PHEPNAM));
        if (nv_PhepNam != null)
        {
          if (Area.ISNGHIPHEP && nv_PhepNam.SONGAYPHEP - nv_PhepNam.SONGAYPHEPDADUNG < Area.SOLUONG)
            return (ActionResult<nv_NghiPhep>) (ActionResult) this.Ok((object) new ApiResponse()
            {
              Success = false,
              Message = $"Số lượng ngày còn lại phép không đủ!{(nv_PhepNam.SONGAYPHEP - nv_PhepNam.SONGAYPHEPDADUNG).ToString()} ngày",
              Data = (object) ""
            });
          if (Area.ISNGHIPHEP && nv_PhepNam.NGAYBATDAU <= Area.THOIGIANVAO && nv_PhepNam.NGAYKETTHUC <= Area.THOIGIANRA)
            return (ActionResult<nv_NghiPhep>) (ActionResult) this.Ok((object) new ApiResponse()
            {
              Success = false,
              Message = "Ngày bắt đầu và ngày kết thúc không nằm trong khoảng thời gian cho tạo phép!",
              Data = (object) ""
            });
          if (Area.ISNGHIPHEP)
            nv_PhepNam.SONGAYPHEPDADUNG += Area.SOLUONG;
          this._context.Entry<nv_PhepNam>(nv_PhepNam).State = EntityState.Modified;
          this._context.nv_NghiPhep.Add(Area);
          AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
          auditLog.InserAuditLog();
          int num = await this._context.SaveChangesAsync();
          nv_PhepNam = (nv_PhepNam) null;
          auditLog = (AuditLogController) null;
          transaction.Commit();
          nv_NghiPhep OKArea = await this._context.nv_NghiPhep.FirstOrDefaultAsync<nv_NghiPhep>((Expression<Func<nv_NghiPhep, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID));
          return (ActionResult<nv_NghiPhep>) (ActionResult) this.Ok((object) new ApiResponse()
          {
            Success = true,
            Message = "Success",
            Data = (object) OKArea
          });
        }
        return (ActionResult<nv_NghiPhep>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {Area.LOC_ID}-{Area.ID} dữ liệu phép năm!",
          Data = (object) ""
        });
      }
    }
    catch (Exception ex)
    {
      return (ActionResult<nv_NghiPhep>) (ActionResult) this.Ok((object) new ApiResponse()
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
      nv_NghiPhep Area = await this._context.nv_NghiPhep.FirstOrDefaultAsync<nv_NghiPhep>((Expression<Func<nv_NghiPhep, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Area == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        nv_PhepNam nv_PhepNam = await this._context.nv_PhepNam.FirstOrDefaultAsync<nv_PhepNam>((Expression<Func<nv_PhepNam, bool>>) (e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID_PHEPNAM));
        if (nv_PhepNam != null)
        {
          if (Area.ISNGHIPHEP)
            nv_PhepNam.SONGAYPHEPDADUNG -= Area.SOLUONG;
          this._context.Entry<nv_PhepNam>(nv_PhepNam).State = EntityState.Modified;
          this._context.nv_NghiPhep.Remove(Area);
          AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
          auditLog.InserAuditLog();
          int num = await this._context.SaveChangesAsync();
          nv_PhepNam = (nv_PhepNam) null;
          auditLog = (AuditLogController) null;
          transaction.Commit();
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = true,
            Message = "Success",
            Data = (object) ""
          });
        }
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu phép năm!",
          Data = (object) ""
        });
      }
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
    return this._context.nv_NghiPhep.Any<nv_NghiPhep>((Expression<Func<nv_NghiPhep, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }
}
