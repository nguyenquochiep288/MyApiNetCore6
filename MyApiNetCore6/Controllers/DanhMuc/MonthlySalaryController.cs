// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.MonthlySalaryController
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
public class MonthlySalaryController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public MonthlySalaryController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetMonthlySalary(string LOC_ID)
  {
    try
    {
      List<dm_ThangLuong> lstValue = await this._context.dm_ThangLuong.Where<dm_ThangLuong>((Expression<Func<dm_ThangLuong, bool>>) (e => e.LOC_ID == LOC_ID)).OrderBy<dm_ThangLuong, string>((Expression<Func<dm_ThangLuong, string>>) (e => e.MA)).ToListAsync<dm_ThangLuong>();
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
  public async Task<IActionResult> GetMonthlySalary(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<dm_ThangLuong> lstValue = await this._context.dm_ThangLuong.Where<dm_ThangLuong>((Expression<Func<dm_ThangLuong, bool>>) (e => e.LOC_ID == LOC_ID)).Where<dm_ThangLuong>(KeyWhere, (object) ValuesSearch).OrderBy<dm_ThangLuong, string>((Expression<Func<dm_ThangLuong, string>>) (e => e.MA)).ToListAsync<dm_ThangLuong>();
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
  public async Task<IActionResult> GetMonthlySalary(string LOC_ID, string ID)
  {
    try
    {
      dm_ThangLuong MonthlySalary = await this._context.dm_ThangLuong.FirstOrDefaultAsync<dm_ThangLuong>((Expression<Func<dm_ThangLuong, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (MonthlySalary == null)
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
        Data = (object) MonthlySalary
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
  public async Task<IActionResult> PutMonthlySalary(
    string LOC_ID,
    string MA,
    dm_ThangLuong MonthlySalary)
  {
    try
    {
      if (LOC_ID != MonthlySalary.LOC_ID || MonthlySalary.MA != MA)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (this.MonthlySalaryExists(MonthlySalary))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{MonthlySalary.LOC_ID}-{MonthlySalary.MA} trong dữ liệu!",
          Data = (object) ""
        });
      if (!this.MonthlySalaryExistsID(LOC_ID, MonthlySalary.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{MonthlySalary.ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<dm_ThangLuong>(MonthlySalary).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      dm_ThangLuong OKMonthlySalary = await this._context.dm_ThangLuong.FirstOrDefaultAsync<dm_ThangLuong>((Expression<Func<dm_ThangLuong, bool>>) (e => e.LOC_ID == MonthlySalary.LOC_ID && e.ID == MonthlySalary.ID));
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKMonthlySalary
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
  public async Task<ActionResult<dm_ThangLuong>> PostMonthlySalary(dm_ThangLuong MonthlySalary)
  {
    try
    {
      if (this.MonthlySalaryExistsMA(MonthlySalary.LOC_ID, MonthlySalary.MA))
        return (ActionResult<dm_ThangLuong>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{MonthlySalary.LOC_ID}-{MonthlySalary.MA} trong dữ liệu!",
          Data = (object) ""
        });
      this._context.dm_ThangLuong.Add(MonthlySalary);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      dm_ThangLuong OKMonthlySalary = await this._context.dm_ThangLuong.FirstOrDefaultAsync<dm_ThangLuong>((Expression<Func<dm_ThangLuong, bool>>) (e => e.LOC_ID == MonthlySalary.LOC_ID && e.ID == MonthlySalary.ID));
      return (ActionResult<dm_ThangLuong>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKMonthlySalary
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_ThangLuong>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteMonthlySalary(string LOC_ID, string ID)
  {
    try
    {
      dm_ThangLuong MonthlySalary = await this._context.dm_ThangLuong.FirstOrDefaultAsync<dm_ThangLuong>((Expression<Func<dm_ThangLuong, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (MonthlySalary == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_ThangLuong>(MonthlySalary, MonthlySalary.ID, MonthlySalary.MA);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      this._context.dm_ThangLuong.Remove(MonthlySalary);
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

  private bool MonthlySalaryExistsMA(string LOC_ID, string MA)
  {
    return this._context.dm_ThangLuong.Any<dm_ThangLuong>((Expression<Func<dm_ThangLuong, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == MA));
  }

  private bool MonthlySalaryExistsID(string LOC_ID, string ID)
  {
    return this._context.dm_ThangLuong.Any<dm_ThangLuong>((Expression<Func<dm_ThangLuong, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }

  private bool MonthlySalaryExists(dm_ThangLuong MonthlySalary)
  {
    return this._context.dm_ThangLuong.Any<dm_ThangLuong>((Expression<Func<dm_ThangLuong, bool>>) (e => e.LOC_ID == MonthlySalary.LOC_ID && e.MA == MonthlySalary.MA && e.ID != MonthlySalary.ID));
  }
}
