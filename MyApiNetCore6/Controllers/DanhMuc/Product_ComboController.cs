// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.Product_ComboController
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
using System.Linq.Expressions;
using System.Threading.Tasks;

#nullable enable
namespace MyApiNetCore6.Controllers;

[Route("api/[controller]")]
[ApiController]
public class Product_ComboController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public Product_ComboController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}/{ID_HANGHOACOMBO}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetProduct_Combo(string LOC_ID, string ID_HANGHOACOMBO)
  {
    try
    {
      List<view_dm_HangHoa_Combo> lstValue = await this._context.view_dm_HangHoa_Combo.Where<view_dm_HangHoa_Combo>((Expression<Func<view_dm_HangHoa_Combo, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_HANGHOACOMBO == ID_HANGHOACOMBO)).ToListAsync<view_dm_HangHoa_Combo>();
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

  [HttpPut("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PutProduct_Combo(
    string LOC_ID,
    string ID,
    dm_HangHoa_Combo Product_Combo)
  {
    try
    {
      if (Product_Combo.ID != ID)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.Product_ComboExists(LOC_ID, Product_Combo.ID_HANGHOA))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<dm_HangHoa_Combo>(Product_Combo).State = EntityState.Modified;
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
  public async Task<ActionResult<dm_HangHoa_Combo>> PostProduct_Combo(dm_HangHoa_Combo Product_Combo)
  {
    try
    {
      if (this.Product_ComboExists(Product_Combo.LOC_ID, Product_Combo.ID_HANGHOA))
        return (ActionResult<dm_HangHoa_Combo>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Product_Combo.LOC_ID}-{Product_Combo.ID_HANGHOA} trong dữ liệu!",
          Data = (object) ""
        });
      this._context.dm_HangHoa_Combo.Add(Product_Combo);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      return (ActionResult<dm_HangHoa_Combo>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) Product_Combo
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_HangHoa_Combo>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteProduct_Combo(string LOC_ID, string ID)
  {
    try
    {
      dm_HangHoa_Combo Product_Combo = await this._context.dm_HangHoa_Combo.FirstOrDefaultAsync<dm_HangHoa_Combo>((Expression<Func<dm_HangHoa_Combo, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Product_Combo == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.dm_HangHoa_Combo.Remove(Product_Combo);
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

  private bool Product_ComboExists(string LOC_ID, string ID_HANGHOA)
  {
    return this._context.dm_HangHoa_Combo.Any<dm_HangHoa_Combo>((Expression<Func<dm_HangHoa_Combo, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_HANGHOA == ID_HANGHOA));
  }
}
