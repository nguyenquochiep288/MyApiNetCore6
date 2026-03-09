// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.LocationController
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
public class LocationController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public LocationController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetCar(string LOC_ID)
  {
    try
    {
      List<dm_DiaDiemChamCong> lstValue = await this._context.dm_DiaDiemChamCong.Where<dm_DiaDiemChamCong>((Expression<Func<dm_DiaDiemChamCong, bool>>) (e => e.LOC_ID == LOC_ID)).OrderBy<dm_DiaDiemChamCong, string>((Expression<Func<dm_DiaDiemChamCong, string>>) (e => e.MA)).ToListAsync<dm_DiaDiemChamCong>();
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
  public async Task<IActionResult> GetCar(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<dm_DiaDiemChamCong> lstValue = await this._context.dm_DiaDiemChamCong.Where<dm_DiaDiemChamCong>((Expression<Func<dm_DiaDiemChamCong, bool>>) (e => e.LOC_ID == LOC_ID)).Where<dm_DiaDiemChamCong>(KeyWhere, (object) ValuesSearch).OrderBy<dm_DiaDiemChamCong, string>((Expression<Func<dm_DiaDiemChamCong, string>>) (e => e.MA)).ToListAsync<dm_DiaDiemChamCong>();
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
  public async Task<IActionResult> GetCar(string LOC_ID, string ID)
  {
    try
    {
      dm_DiaDiemChamCong Car = await this._context.dm_DiaDiemChamCong.FirstOrDefaultAsync<dm_DiaDiemChamCong>((Expression<Func<dm_DiaDiemChamCong, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Car == null)
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
        Data = (object) Car
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
  public async Task<IActionResult> PutCar(string LOC_ID, string MA, dm_DiaDiemChamCong Car)
  {
    try
    {
      if (LOC_ID != Car.LOC_ID || Car.MA != MA)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (this.CarExists(Car))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Car.LOC_ID}-{Car.MA} trong dữ liệu!",
          Data = (object) ""
        });
      if (!this.CarExistsID(LOC_ID, Car.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{Car.ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<dm_DiaDiemChamCong>(Car).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      dm_DiaDiemChamCong OKCar = await this._context.dm_DiaDiemChamCong.FirstOrDefaultAsync<dm_DiaDiemChamCong>((Expression<Func<dm_DiaDiemChamCong, bool>>) (e => e.LOC_ID == Car.LOC_ID && e.ID == Car.ID));
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKCar
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
  public async Task<ActionResult<dm_DiaDiemChamCong>> PostCar(dm_DiaDiemChamCong Car)
  {
    try
    {
      if (this.CarExistsMA(Car.LOC_ID, Car.MA))
        return (ActionResult<dm_DiaDiemChamCong>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Car.LOC_ID}-{Car.MA} trong dữ liệu!",
          Data = (object) ""
        });
      this._context.dm_DiaDiemChamCong.Add(Car);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      dm_DiaDiemChamCong OKCar = await this._context.dm_DiaDiemChamCong.FirstOrDefaultAsync<dm_DiaDiemChamCong>((Expression<Func<dm_DiaDiemChamCong, bool>>) (e => e.LOC_ID == Car.LOC_ID && e.ID == Car.ID));
      return (ActionResult<dm_DiaDiemChamCong>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKCar
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_DiaDiemChamCong>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteCar(string LOC_ID, string ID)
  {
    try
    {
      dm_DiaDiemChamCong Car = await this._context.dm_DiaDiemChamCong.FirstOrDefaultAsync<dm_DiaDiemChamCong>((Expression<Func<dm_DiaDiemChamCong, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Car == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_DiaDiemChamCong>(Car, Car.ID, Car.MA);
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
      this._context.dm_DiaDiemChamCong.Remove(Car);
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

  private bool CarExistsMA(string LOC_ID, string MA)
  {
    return this._context.dm_DiaDiemChamCong.Any<dm_DiaDiemChamCong>((Expression<Func<dm_DiaDiemChamCong, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == MA));
  }

  private bool CarExistsID(string LOC_ID, string ID)
  {
    return this._context.dm_DiaDiemChamCong.Any<dm_DiaDiemChamCong>((Expression<Func<dm_DiaDiemChamCong, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }

  private bool CarExists(dm_DiaDiemChamCong Car)
  {
    return this._context.dm_DiaDiemChamCong.Any<dm_DiaDiemChamCong>((Expression<Func<dm_DiaDiemChamCong, bool>>) (e => e.LOC_ID == Car.LOC_ID && e.MA == Car.MA && e.ID != Car.ID));
  }
}
