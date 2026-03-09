// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.BankAccountController
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
public class BankAccountController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public BankAccountController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetBankAccount(string LOC_ID)
  {
    try
    {
      List<dm_TaiKhoanNganHang> lstValue = await this._context.dm_TaiKhoanNganHang.Where<dm_TaiKhoanNganHang>((Expression<Func<dm_TaiKhoanNganHang, bool>>) (e => e.LOC_ID == LOC_ID)).OrderBy<dm_TaiKhoanNganHang, string>((Expression<Func<dm_TaiKhoanNganHang, string>>) (e => e.MA)).ToListAsync<dm_TaiKhoanNganHang>();
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
  public async Task<IActionResult> GetBankAccount(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<dm_TaiKhoanNganHang> lstValue = await this._context.dm_TaiKhoanNganHang.Where<dm_TaiKhoanNganHang>((Expression<Func<dm_TaiKhoanNganHang, bool>>) (e => e.LOC_ID == LOC_ID)).Where<dm_TaiKhoanNganHang>(KeyWhere, (object) ValuesSearch).OrderBy<dm_TaiKhoanNganHang, string>((Expression<Func<dm_TaiKhoanNganHang, string>>) (e => e.MA)).ToListAsync<dm_TaiKhoanNganHang>();
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
  public async Task<IActionResult> GetBankAccount(string LOC_ID, string ID)
  {
    try
    {
      dm_TaiKhoanNganHang BankAccount = await this._context.dm_TaiKhoanNganHang.FirstOrDefaultAsync<dm_TaiKhoanNganHang>((Expression<Func<dm_TaiKhoanNganHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (BankAccount == null)
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
        Data = (object) BankAccount
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
  public async Task<IActionResult> PutBankAccount(
    string LOC_ID,
    string MA,
    dm_TaiKhoanNganHang BankAccount)
  {
    try
    {
      if (LOC_ID != BankAccount.LOC_ID || BankAccount.MA != MA)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (this.BankAccountExists(BankAccount))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{BankAccount.LOC_ID}-{BankAccount.MA} trong dữ liệu!",
          Data = (object) ""
        });
      if (!this.BankAccountExistsID(LOC_ID, BankAccount.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{BankAccount.ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<dm_TaiKhoanNganHang>(BankAccount).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      dm_TaiKhoanNganHang OKBankAccount = await this._context.dm_TaiKhoanNganHang.FirstOrDefaultAsync<dm_TaiKhoanNganHang>((Expression<Func<dm_TaiKhoanNganHang, bool>>) (e => e.LOC_ID == BankAccount.LOC_ID && e.ID == BankAccount.ID));
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKBankAccount
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
  public async Task<ActionResult<dm_TaiKhoanNganHang>> PostBankAccount(
    dm_TaiKhoanNganHang BankAccount)
  {
    try
    {
      if (this.BankAccountExistsMA(BankAccount.LOC_ID, BankAccount.MA))
        return (ActionResult<dm_TaiKhoanNganHang>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{BankAccount.LOC_ID}-{BankAccount.MA} trong dữ liệu!",
          Data = (object) ""
        });
      this._context.dm_TaiKhoanNganHang.Add(BankAccount);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      dm_TaiKhoanNganHang OKBankAccount = await this._context.dm_TaiKhoanNganHang.FirstOrDefaultAsync<dm_TaiKhoanNganHang>((Expression<Func<dm_TaiKhoanNganHang, bool>>) (e => e.LOC_ID == BankAccount.LOC_ID && e.ID == BankAccount.ID));
      return (ActionResult<dm_TaiKhoanNganHang>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKBankAccount
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_TaiKhoanNganHang>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteBankAccount(string LOC_ID, string ID)
  {
    try
    {
      dm_TaiKhoanNganHang BankAccount = await this._context.dm_TaiKhoanNganHang.FirstOrDefaultAsync<dm_TaiKhoanNganHang>((Expression<Func<dm_TaiKhoanNganHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (BankAccount == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_TaiKhoanNganHang>(BankAccount, BankAccount.ID, BankAccount.MA);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      this._context.dm_TaiKhoanNganHang.Remove(BankAccount);
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

  private bool BankAccountExistsMA(string LOC_ID, string MA)
  {
    return this._context.dm_TaiKhoanNganHang.Any<dm_TaiKhoanNganHang>((Expression<Func<dm_TaiKhoanNganHang, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == MA));
  }

  private bool BankAccountExistsID(string LOC_ID, string ID)
  {
    return this._context.dm_TaiKhoanNganHang.Any<dm_TaiKhoanNganHang>((Expression<Func<dm_TaiKhoanNganHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }

  private bool BankAccountExists(dm_TaiKhoanNganHang BankAccount)
  {
    return this._context.dm_TaiKhoanNganHang.Any<dm_TaiKhoanNganHang>((Expression<Func<dm_TaiKhoanNganHang, bool>>) (e => e.LOC_ID == BankAccount.LOC_ID && e.MA == BankAccount.MA && e.ID != BankAccount.ID));
  }
}
