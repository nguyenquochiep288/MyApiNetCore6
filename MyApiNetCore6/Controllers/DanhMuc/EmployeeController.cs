// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.EmployeeController
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
public class EmployeeController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public EmployeeController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetEmployee(string LOC_ID)
  {
    try
    {
      List<view_dm_NhanVien> lstValue = await this._context.view_dm_NhanVien.Where<view_dm_NhanVien>((Expression<Func<view_dm_NhanVien, bool>>) (e => e.LOC_ID == LOC_ID)).OrderBy<view_dm_NhanVien, string>((Expression<Func<view_dm_NhanVien, string>>) (e => e.MA)).ToListAsync<view_dm_NhanVien>();
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
  public async Task<IActionResult> GetEmployee(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<view_dm_NhanVien> lstValue = await this._context.view_dm_NhanVien.Where<view_dm_NhanVien>((Expression<Func<view_dm_NhanVien, bool>>) (e => e.LOC_ID == LOC_ID)).Where<view_dm_NhanVien>(KeyWhere, (object) ValuesSearch).OrderBy<view_dm_NhanVien, string>((Expression<Func<view_dm_NhanVien, string>>) (e => e.MA)).ToListAsync<view_dm_NhanVien>();
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
  public async Task<IActionResult> GetEmployee(string LOC_ID, string ID)
  {
    try
    {
      view_dm_NhanVien Employee = await this._context.view_dm_NhanVien.FirstOrDefaultAsync<view_dm_NhanVien>((Expression<Func<view_dm_NhanVien, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Employee == null)
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
        Data = (object) Employee
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
  public async Task<IActionResult> PutEmployee(string LOC_ID, string MA, dm_NhanVien Employee)
  {
    try
    {
      if (this.EmployeeExists(Employee))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Employee.LOC_ID}-{Employee.MA} trong dữ liệu!",
          Data = (object) ""
        });
      if (LOC_ID != Employee.LOC_ID || Employee.MA != MA)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.EmployeeExistsID(LOC_ID, Employee.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{Employee.ID} dữ liệu!",
          Data = (object) ""
        });
      if (!string.IsNullOrEmpty(Employee.ID_TAIKHOAN))
      {
        view_dm_NhanVien check = await this._context.view_dm_NhanVien.FirstOrDefaultAsync<view_dm_NhanVien>((Expression<Func<view_dm_NhanVien, bool>>) (e => e.LOC_ID == Employee.LOC_ID && e.ID != Employee.ID && e.ID_TAIKHOAN == Employee.ID_TAIKHOAN));
        if (check != null)
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = $"Tài khoản cấp cho {Employee.MA} - {Employee.NAME} đã được cấp cho nhân viên{check.MA} - {check.NAME}",
            Data = (object) ""
          });
        check = (view_dm_NhanVien) null;
      }
      this._context.Entry<dm_NhanVien>(Employee).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      view_dm_NhanVien OKEmployee = await this._context.view_dm_NhanVien.FirstOrDefaultAsync<view_dm_NhanVien>((Expression<Func<view_dm_NhanVien, bool>>) (e => e.LOC_ID == Employee.LOC_ID && e.ID == Employee.ID));
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKEmployee
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
  public async Task<ActionResult<dm_NhanVien>> PostEmployee(dm_NhanVien Employee)
  {
    try
    {
      if (this.EmployeeExistsMA(Employee.LOC_ID, Employee.MA))
        return (ActionResult<dm_NhanVien>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Employee.LOC_ID}-{Employee.MA} trong dữ liệu!",
          Data = (object) ""
        });
      if (!string.IsNullOrEmpty(Employee.ID_TAIKHOAN))
      {
        view_dm_NhanVien check = await this._context.view_dm_NhanVien.FirstOrDefaultAsync<view_dm_NhanVien>((Expression<Func<view_dm_NhanVien, bool>>) (e => e.LOC_ID == Employee.LOC_ID && e.ID != Employee.ID && e.ID_TAIKHOAN == Employee.ID_TAIKHOAN));
        if (check != null)
          return (ActionResult<dm_NhanVien>) (ActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = $"Tài khoản cấp cho {Employee.MA} - {Employee.NAME} đã được cấp cho nhân viên{check.MA} - {check.NAME}",
            Data = (object) ""
          });
        check = (view_dm_NhanVien) null;
      }
      this._context.dm_NhanVien.Add(Employee);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      view_dm_NhanVien OKEmployee = await this._context.view_dm_NhanVien.FirstOrDefaultAsync<view_dm_NhanVien>((Expression<Func<view_dm_NhanVien, bool>>) (e => e.LOC_ID == Employee.LOC_ID && e.ID == Employee.ID));
      return (ActionResult<dm_NhanVien>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKEmployee
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_NhanVien>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteEmployee(string LOC_ID, string ID)
  {
    try
    {
      dm_NhanVien Employee = await this._context.dm_NhanVien.FirstOrDefaultAsync<dm_NhanVien>((Expression<Func<dm_NhanVien, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Employee == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_NhanVien>(Employee, Employee.ID, Employee.MA);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      this._context.dm_NhanVien.Remove(Employee);
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

  private bool EmployeeExistsMA(string LOC_ID, string MA)
  {
    return this._context.dm_NhanVien.Any<dm_NhanVien>((Expression<Func<dm_NhanVien, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == MA));
  }

  private bool EmployeeExistsID(string LOC_ID, string ID)
  {
    return this._context.dm_NhanVien.Any<dm_NhanVien>((Expression<Func<dm_NhanVien, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }

  private bool EmployeeExists(dm_NhanVien Employee)
  {
    return this._context.dm_NhanVien.Any<dm_NhanVien>((Expression<Func<dm_NhanVien, bool>>) (e => e.LOC_ID == Employee.LOC_ID && e.MA == Employee.MA && e.ID != Employee.ID));
  }
}
