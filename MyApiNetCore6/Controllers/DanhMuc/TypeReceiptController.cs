// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.TypeReceiptController
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
public class TypeReceiptController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public TypeReceiptController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetTypeReceipt(string LOC_ID)
  {
    try
    {
      List<dm_LoaiPhieuThu> lstValue = await this._context.dm_LoaiPhieuThu.Where<dm_LoaiPhieuThu>((Expression<Func<dm_LoaiPhieuThu, bool>>) (e => e.LOC_ID == LOC_ID)).OrderBy<dm_LoaiPhieuThu, string>((Expression<Func<dm_LoaiPhieuThu, string>>) (e => e.MA)).ToListAsync<dm_LoaiPhieuThu>();
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
  public async Task<IActionResult> GetTypeReceipt(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<dm_LoaiPhieuThu> lstValue = await this._context.dm_LoaiPhieuThu.Where<dm_LoaiPhieuThu>((Expression<Func<dm_LoaiPhieuThu, bool>>) (e => e.LOC_ID == LOC_ID)).Where<dm_LoaiPhieuThu>(KeyWhere, (object) ValuesSearch).OrderBy<dm_LoaiPhieuThu, string>((Expression<Func<dm_LoaiPhieuThu, string>>) (e => e.MA)).ToListAsync<dm_LoaiPhieuThu>();
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
  public async Task<IActionResult> GetTypeReceipt(string LOC_ID, string ID)
  {
    try
    {
      dm_LoaiPhieuThu TypeReceipt = await this._context.dm_LoaiPhieuThu.FirstOrDefaultAsync<dm_LoaiPhieuThu>((Expression<Func<dm_LoaiPhieuThu, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (TypeReceipt == null)
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
        Data = (object) TypeReceipt
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
  public async Task<IActionResult> PutTypeReceipt(
    string LOC_ID,
    string MA,
    dm_LoaiPhieuThu TypeReceipt)
  {
    try
    {
      if (this.TypeReceiptExists(TypeReceipt))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{TypeReceipt.LOC_ID}-{TypeReceipt.MA} trong dữ liệu!",
          Data = (object) ""
        });
      if (LOC_ID != TypeReceipt.LOC_ID || TypeReceipt.MA != MA)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.TypeReceiptExistsID(LOC_ID, TypeReceipt.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{TypeReceipt.ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<dm_LoaiPhieuThu>(TypeReceipt).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) TypeReceipt
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
  public async Task<ActionResult<dm_LoaiPhieuThu>> PostTypeReceipt(dm_LoaiPhieuThu TypeReceipt)
  {
    try
    {
      if (this.TypeReceiptExistsMA(TypeReceipt.LOC_ID, TypeReceipt.MA))
        return (ActionResult<dm_LoaiPhieuThu>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{TypeReceipt.LOC_ID}-{TypeReceipt.MA} trong dữ liệu!",
          Data = (object) ""
        });
      this._context.dm_LoaiPhieuThu.Add(TypeReceipt);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      return (ActionResult<dm_LoaiPhieuThu>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) TypeReceipt
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_LoaiPhieuThu>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteTypeReceipt(string LOC_ID, string ID)
  {
    try
    {
      dm_LoaiPhieuThu TypeReceipt = await this._context.dm_LoaiPhieuThu.FirstOrDefaultAsync<dm_LoaiPhieuThu>((Expression<Func<dm_LoaiPhieuThu, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (TypeReceipt == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_LoaiPhieuThu>(TypeReceipt, TypeReceipt.ID, TypeReceipt.NAME);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      this._context.dm_LoaiPhieuThu.Remove(TypeReceipt);
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

  private bool TypeReceiptExistsMA(string LOC_ID, string MA)
  {
    return this._context.dm_LoaiPhieuThu.Any<dm_LoaiPhieuThu>((Expression<Func<dm_LoaiPhieuThu, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == MA));
  }

  private bool TypeReceiptExistsID(string LOC_ID, string ID)
  {
    return this._context.dm_LoaiPhieuThu.Any<dm_LoaiPhieuThu>((Expression<Func<dm_LoaiPhieuThu, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }

  private bool TypeReceiptExists(dm_LoaiPhieuThu Position)
  {
    return this._context.dm_LoaiPhieuThu.Any<dm_LoaiPhieuThu>((Expression<Func<dm_LoaiPhieuThu, bool>>) (e => e.LOC_ID == Position.LOC_ID && e.MA == Position.MA && e.ID != Position.ID));
  }
}
