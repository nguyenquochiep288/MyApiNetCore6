// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.DepartmentController
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
public class DepartmentController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public DepartmentController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetDepartment(string LOC_ID)
  {
    try
    {
      List<dm_PhongBan> lstValue = await this._context.dm_PhongBan.Where<dm_PhongBan>((Expression<Func<dm_PhongBan, bool>>) (e => e.LOC_ID == LOC_ID)).OrderBy<dm_PhongBan, string>((Expression<Func<dm_PhongBan, string>>) (e => e.MA)).ToListAsync<dm_PhongBan>();
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
  public async Task<IActionResult> GetDepartment(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<dm_PhongBan> lstValue = await this._context.dm_PhongBan.Where<dm_PhongBan>((Expression<Func<dm_PhongBan, bool>>) (e => e.LOC_ID == LOC_ID)).Where<dm_PhongBan>(KeyWhere, (object) ValuesSearch).OrderBy<dm_PhongBan, string>((Expression<Func<dm_PhongBan, string>>) (e => e.MA)).ToListAsync<dm_PhongBan>();
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
  public async Task<IActionResult> GetDepartment(string LOC_ID, string ID)
  {
    try
    {
      dm_PhongBan Department = await this._context.dm_PhongBan.FirstOrDefaultAsync<dm_PhongBan>((Expression<Func<dm_PhongBan, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Department == null)
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
        Data = (object) Department
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
  public async Task<IActionResult> PutDepartment(string LOC_ID, string MA, dm_PhongBan Department)
  {
    try
    {
      if (LOC_ID != Department.LOC_ID || Department.MA != MA)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.DepartmentExistsID(LOC_ID, Department.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{Department.ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<dm_PhongBan>(Department).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      dm_TienTe OKDepartment = await this._context.dm_TienTe.FirstOrDefaultAsync<dm_TienTe>((Expression<Func<dm_TienTe, bool>>) (e => e.LOC_ID == Department.LOC_ID && e.ID == Department.ID));
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKDepartment
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
  public async Task<ActionResult<dm_PhongBan>> PostDepartment(dm_PhongBan Department)
  {
    try
    {
      if (this.DepartmentExistsMA(Department.LOC_ID, Department.MA))
        return (ActionResult<dm_PhongBan>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Department.LOC_ID}-{Department.MA} trong dữ liệu!",
          Data = (object) ""
        });
      this._context.dm_PhongBan.Add(Department);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      dm_TienTe OKDepartment = await this._context.dm_TienTe.FirstOrDefaultAsync<dm_TienTe>((Expression<Func<dm_TienTe, bool>>) (e => e.LOC_ID == Department.LOC_ID && e.ID == Department.ID));
      return (ActionResult<dm_PhongBan>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKDepartment
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_PhongBan>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteDepartment(string LOC_ID, string ID)
  {
    try
    {
      dm_PhongBan Department = await this._context.dm_PhongBan.FirstOrDefaultAsync<dm_PhongBan>((Expression<Func<dm_PhongBan, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Department == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_PhongBan>(Department, Department.ID, Department.MA);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      this._context.dm_PhongBan.Remove(Department);
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

  private bool DepartmentExistsMA(string LOC_ID, string MA)
  {
    return this._context.dm_PhongBan.Any<dm_PhongBan>((Expression<Func<dm_PhongBan, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == MA));
  }

  private bool DepartmentExistsID(string LOC_ID, string ID)
  {
    return this._context.dm_PhongBan.Any<dm_PhongBan>((Expression<Func<dm_PhongBan, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }

  private bool DepartmentExists(dm_PhongBan Department)
  {
    return this._context.dm_PhongBan.Any<dm_PhongBan>((Expression<Func<dm_PhongBan, bool>>) (e => e.LOC_ID == Department.LOC_ID && e.MA == Department.MA && e.ID != Department.ID));
  }
}
