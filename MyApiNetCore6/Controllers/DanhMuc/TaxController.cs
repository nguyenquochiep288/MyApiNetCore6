// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.TaxController
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
public class TaxController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public TaxController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetTax(string LOC_ID)
  {
    try
    {
      List<dm_ThueSuat> lstValue = await this._context.dm_ThueSuat.Where<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == LOC_ID)).OrderBy<dm_ThueSuat, string>((Expression<Func<dm_ThueSuat, string>>) (e => e.MA)).ToListAsync<dm_ThueSuat>();
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
  public async Task<IActionResult> GetTax(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<dm_ThueSuat> lstValue = await this._context.dm_ThueSuat.Where<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == LOC_ID)).Where<dm_ThueSuat>(KeyWhere, (object) ValuesSearch).OrderBy<dm_ThueSuat, string>((Expression<Func<dm_ThueSuat, string>>) (e => e.MA)).ToListAsync<dm_ThueSuat>();
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
  public async Task<IActionResult> GetTax(string LOC_ID, string ID)
  {
    try
    {
      dm_ThueSuat Tax = await this._context.dm_ThueSuat.FirstOrDefaultAsync<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Tax == null)
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
        Data = (object) Tax
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
  public async Task<IActionResult> PutTax(string LOC_ID, string MA, dm_ThueSuat Tax)
  {
    try
    {
      if (this.TaxExists(Tax))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Tax.LOC_ID}-{Tax.MA} trong dữ liệu!",
          Data = (object) ""
        });
      if (LOC_ID != Tax.LOC_ID || Tax.MA != MA)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.TaxExistsID(LOC_ID, Tax.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{Tax.ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<dm_ThueSuat>(Tax).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      dm_TienTe OKTax = await this._context.dm_TienTe.FirstOrDefaultAsync<dm_TienTe>((Expression<Func<dm_TienTe, bool>>) (e => e.LOC_ID == Tax.LOC_ID && e.ID == Tax.ID));
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKTax
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
  public async Task<ActionResult<dm_ThueSuat>> PostTax(dm_ThueSuat Tax)
  {
    try
    {
      if (this.TaxExistsMA(Tax.LOC_ID, Tax.MA))
        return (ActionResult<dm_ThueSuat>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Tax.LOC_ID}-{Tax.MA} trong dữ liệu!",
          Data = (object) ""
        });
      this._context.dm_ThueSuat.Add(Tax);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      dm_TienTe OKTax = await this._context.dm_TienTe.FirstOrDefaultAsync<dm_TienTe>((Expression<Func<dm_TienTe, bool>>) (e => e.LOC_ID == Tax.LOC_ID && e.ID == Tax.ID));
      return (ActionResult<dm_ThueSuat>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKTax
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_ThueSuat>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteTax(string LOC_ID, string ID)
  {
    try
    {
      dm_ThueSuat Tax = await this._context.dm_ThueSuat.FirstOrDefaultAsync<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Tax == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_ThueSuat>(Tax, Tax.ID, Tax.NAME);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      this._context.dm_ThueSuat.Remove(Tax);
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

  private bool TaxExistsMA(string LOC_ID, string MA)
  {
    return this._context.dm_ThueSuat.Any<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == MA));
  }

  private bool TaxExistsID(string LOC_ID, string ID)
  {
    return this._context.dm_ThueSuat.Any<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }

  private bool TaxExists(dm_ThueSuat Position)
  {
    return this._context.dm_ThueSuat.Any<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == Position.LOC_ID && e.MA == Position.MA && e.ID != Position.ID));
  }
}
