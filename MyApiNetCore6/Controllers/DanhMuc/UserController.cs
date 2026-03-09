// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.UserController
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
public class UserController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public UserController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetUser()
  {
    try
    {
      List<view_AspNetUsers> lstValue = await this._context.view_AspNetUsers.ToListAsync<view_AspNetUsers>();
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

  [HttpGet("{Type}/{KeyWhere}/{ValuesSearch}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetUser(int Type, string KeyWhere = "", string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<view_AspNetUsers> lstValue = await this._context.view_AspNetUsers.Where<view_AspNetUsers>(KeyWhere, (object) ValuesSearch).OrderBy<view_AspNetUsers, string>((Expression<Func<view_AspNetUsers, string>>) (e => e.MA)).ToListAsync<view_AspNetUsers>();
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

  [HttpGet("{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetUser(string ID)
  {
    try
    {
      view_AspNetUsers User = await this._context.view_AspNetUsers.FirstOrDefaultAsync<view_AspNetUsers>((Expression<Func<view_AspNetUsers, bool>>) (e => e.ID == ID));
      if (User == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {ID} dữ liệu!",
          Data = (object) ""
        });
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) User
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

  [HttpPut("{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PutUser(string ID, AspNetUsers User)
  {
    try
    {
      if (ID != User.ID)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.UserExists(User.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<AspNetUsers>(User).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      view_AspNetUsers OKUser = await this._context.view_AspNetUsers.FirstOrDefaultAsync<view_AspNetUsers>((Expression<Func<view_AspNetUsers, bool>>) (e => e.ID == User.ID));
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKUser
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
  public async Task<ActionResult<AspNetUsers>> PostUser(AspNetUsers User)
  {
    try
    {
      if (this.UserExists(User.ID))
        return (ActionResult<AspNetUsers>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{User.ID} trong dữ liệu!",
          Data = (object) ""
        });
      this._context.AspNetUsers.Add(User);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      view_AspNetUsers OKUser = await this._context.view_AspNetUsers.FirstOrDefaultAsync<view_AspNetUsers>((Expression<Func<view_AspNetUsers, bool>>) (e => e.ID == User.ID));
      return (ActionResult<AspNetUsers>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKUser
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<AspNetUsers>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteUser(string ID)
  {
    try
    {
      AspNetUsers User = await this._context.AspNetUsers.FirstOrDefaultAsync<AspNetUsers>((Expression<Func<AspNetUsers, bool>>) (e => e.ID == ID));
      if (User == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<AspNetUsers>(User, User.ID, User.UserName);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      this._context.AspNetUsers.Remove(User);
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

  private bool UserExists(string ID)
  {
    return this._context.AspNetUsers.Any<AspNetUsers>((Expression<Func<AspNetUsers, bool>>) (e => e.ID == ID));
  }
}
