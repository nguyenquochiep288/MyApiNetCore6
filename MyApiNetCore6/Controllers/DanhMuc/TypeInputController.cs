// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.TypeInputController
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
public class TypeInputController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public TypeInputController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetTypeInput(string LOC_ID)
  {
    try
    {
      List<dm_LoaiPhieuNhap> lstValue = await this._context.dm_LoaiPhieuNhap.Where<dm_LoaiPhieuNhap>((Expression<Func<dm_LoaiPhieuNhap, bool>>) (e => e.LOC_ID == LOC_ID)).OrderBy<dm_LoaiPhieuNhap, string>((Expression<Func<dm_LoaiPhieuNhap, string>>) (e => e.MA)).ToListAsync<dm_LoaiPhieuNhap>();
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
  public async Task<IActionResult> GetTypePayment(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<dm_LoaiPhieuNhap> lstValue = await this._context.dm_LoaiPhieuNhap.Where<dm_LoaiPhieuNhap>((Expression<Func<dm_LoaiPhieuNhap, bool>>) (e => e.LOC_ID == LOC_ID)).Where<dm_LoaiPhieuNhap>(KeyWhere, (object) ValuesSearch).OrderBy<dm_LoaiPhieuNhap, string>((Expression<Func<dm_LoaiPhieuNhap, string>>) (e => e.MA)).ToListAsync<dm_LoaiPhieuNhap>();
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
  public async Task<IActionResult> GetTypePayment(string LOC_ID, string ID)
  {
    try
    {
      dm_LoaiPhieuNhap TypePayment = await this._context.dm_LoaiPhieuNhap.FirstOrDefaultAsync<dm_LoaiPhieuNhap>((Expression<Func<dm_LoaiPhieuNhap, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (TypePayment == null)
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
        Data = (object) TypePayment
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
  public async Task<IActionResult> PutTypePayment(
    string LOC_ID,
    string MA,
    dm_LoaiPhieuNhap TypePayment)
  {
    try
    {
      if (this.TypePaymentExists(TypePayment))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{TypePayment.LOC_ID}-{TypePayment.MA} trong dữ liệu!",
          Data = (object) ""
        });
      if (LOC_ID != TypePayment.LOC_ID || TypePayment.MA != MA)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.TypePaymentExistsID(LOC_ID, TypePayment.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{TypePayment.ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<dm_LoaiPhieuNhap>(TypePayment).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) TypePayment
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
  public async Task<ActionResult<dm_LoaiPhieuNhap>> PostTypePayment(dm_LoaiPhieuNhap TypePayment)
  {
    try
    {
      if (this.TypePaymentExistsMA(TypePayment.LOC_ID, TypePayment.MA))
        return (ActionResult<dm_LoaiPhieuNhap>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{TypePayment.LOC_ID}-{TypePayment.MA} trong dữ liệu!",
          Data = (object) ""
        });
      this._context.dm_LoaiPhieuNhap.Add(TypePayment);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      return (ActionResult<dm_LoaiPhieuNhap>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) TypePayment
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_LoaiPhieuNhap>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteTypePayment(string LOC_ID, string ID)
  {
    try
    {
      dm_LoaiPhieuNhap TypePayment = await this._context.dm_LoaiPhieuNhap.FirstOrDefaultAsync<dm_LoaiPhieuNhap>((Expression<Func<dm_LoaiPhieuNhap, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (TypePayment == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_LoaiPhieuNhap>(TypePayment, TypePayment.ID, TypePayment.NAME);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      this._context.dm_LoaiPhieuNhap.Remove(TypePayment);
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

  private bool TypePaymentExistsMA(string LOC_ID, string MA)
  {
    return this._context.dm_LoaiPhieuNhap.Any<dm_LoaiPhieuNhap>((Expression<Func<dm_LoaiPhieuNhap, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == MA));
  }

  private bool TypePaymentExistsID(string LOC_ID, string ID)
  {
    return this._context.dm_LoaiPhieuNhap.Any<dm_LoaiPhieuNhap>((Expression<Func<dm_LoaiPhieuNhap, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }

  private bool TypePaymentExists(dm_LoaiPhieuNhap Position)
  {
    return this._context.dm_LoaiPhieuNhap.Any<dm_LoaiPhieuNhap>((Expression<Func<dm_LoaiPhieuNhap, bool>>) (e => e.LOC_ID == Position.LOC_ID && e.MA == Position.MA && e.ID != Position.ID));
  }
}
