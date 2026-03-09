// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.PermissionsGroupProductController
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


namespace MyApiNetCore6.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PermissionsGroupProductController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public PermissionsGroupProductController(
    dbTrangHiepPhatContext context,
    IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetPermissionsGroupProduct(string LOC_ID)
  {
    try
    {
      List<web_PhanQuyenNhomSanPham> lstValue = await this._context.web_PhanQuyenNhomSanPham.Where<web_PhanQuyenNhomSanPham>((Expression<Func<web_PhanQuyenNhomSanPham, bool>>) (e => e.LOC_ID == LOC_ID)).ToListAsync<web_PhanQuyenNhomSanPham>();
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
  public async Task<IActionResult> GetPermissionsGroupProduct(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      List<web_PhanQuyenNhomSanPham> lstValue = await this._context.web_PhanQuyenNhomSanPham.Where<web_PhanQuyenNhomSanPham>((Expression<Func<web_PhanQuyenNhomSanPham, bool>>) (e => e.LOC_ID == LOC_ID)).Where<web_PhanQuyenNhomSanPham>(KeyWhere, (object) ValuesSearch).ToListAsync<web_PhanQuyenNhomSanPham>();
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
  public async Task<IActionResult> GetPermissionsGroupProduct(string LOC_ID, string ID)
  {
    try
    {
      web_PhanQuyenNhomSanPham PermissionsGroupProduct = await this._context.web_PhanQuyenNhomSanPham.FirstOrDefaultAsync<web_PhanQuyenNhomSanPham>((Expression<Func<web_PhanQuyenNhomSanPham, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (PermissionsGroupProduct == null)
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
        Data = (object) PermissionsGroupProduct
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
  public async Task<IActionResult> PutPermissionsGroupProduct(
    string LOC_ID,
    string ID,
    web_PhanQuyenNhomSanPham PermissionsGroupProduct)
  {
    try
    {
      if (LOC_ID != PermissionsGroupProduct.LOC_ID && ID != PermissionsGroupProduct.ID)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.PermissionsGroupProductExists(PermissionsGroupProduct.LOC_ID, PermissionsGroupProduct.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<web_PhanQuyenNhomSanPham>(PermissionsGroupProduct).State = EntityState.Modified;
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
  public async Task<ActionResult<web_PhanQuyenNhomSanPham>> PostPermissionsGroupProduct(
    web_PhanQuyenNhomSanPham PermissionsGroupProduct)
  {
    try
    {
      if (this.PermissionsGroupProductExists(PermissionsGroupProduct.LOC_ID, PermissionsGroupProduct.ID))
        return (ActionResult<web_PhanQuyenNhomSanPham>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại {PermissionsGroupProduct.LOC_ID}-{PermissionsGroupProduct.ID} trong dữ liệu!",
          Data = (object) "",
          CheckValue = true
        });
      this._context.web_PhanQuyenNhomSanPham.Add(PermissionsGroupProduct);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      return (ActionResult<web_PhanQuyenNhomSanPham>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) PermissionsGroupProduct
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<web_PhanQuyenNhomSanPham>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeletePermissionsGroupProduct(string LOC_ID, string ID)
  {
    try
    {
      web_PhanQuyenNhomSanPham PermissionsGroupProduct = await this._context.web_PhanQuyenNhomSanPham.FirstOrDefaultAsync<web_PhanQuyenNhomSanPham>((Expression<Func<web_PhanQuyenNhomSanPham, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (PermissionsGroupProduct == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.web_PhanQuyenNhomSanPham.Remove(PermissionsGroupProduct);
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

  private bool PermissionsGroupProductExists(string LOC_ID, string ID)
  {
    return this._context.web_PhanQuyenNhomSanPham.Any<web_PhanQuyenNhomSanPham>((Expression<Func<web_PhanQuyenNhomSanPham, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }
}
