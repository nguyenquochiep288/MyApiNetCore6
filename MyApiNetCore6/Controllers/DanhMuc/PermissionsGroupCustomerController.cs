// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.PermissionsGroupCustomerController
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
public class PermissionsGroupCustomerController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public PermissionsGroupCustomerController(
    dbTrangHiepPhatContext context,
    IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetPermissionsGroupCustomer(string LOC_ID)
  {
    try
    {
      List<web_PhanQuyenNhomKhachHang> lstValue = await this._context.web_PhanQuyenNhomKhachHang.Where<web_PhanQuyenNhomKhachHang>((Expression<Func<web_PhanQuyenNhomKhachHang, bool>>) (e => e.LOC_ID == LOC_ID)).ToListAsync<web_PhanQuyenNhomKhachHang>();
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
  public async Task<IActionResult> GetPermissionsGroupCustomer(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      List<web_PhanQuyenNhomKhachHang> lstValue = await this._context.web_PhanQuyenNhomKhachHang.Where<web_PhanQuyenNhomKhachHang>((Expression<Func<web_PhanQuyenNhomKhachHang, bool>>) (e => e.LOC_ID == LOC_ID)).Where<web_PhanQuyenNhomKhachHang>(KeyWhere, (object) ValuesSearch).ToListAsync<web_PhanQuyenNhomKhachHang>();
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

  [HttpGet("{LOC_ID}/{id}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetPermissionsGroupCustomer(string LOC_ID, string ID)
  {
    try
    {
      web_PhanQuyenNhomKhachHang PermissionsGroupCustomer = await this._context.web_PhanQuyenNhomKhachHang.FirstOrDefaultAsync<web_PhanQuyenNhomKhachHang>((Expression<Func<web_PhanQuyenNhomKhachHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (PermissionsGroupCustomer == null)
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
        Data = (object) PermissionsGroupCustomer
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
  public async Task<IActionResult> PutPermissionsGroupCustomer(
    string LOC_ID,
    string ID,
    web_PhanQuyenNhomKhachHang PermissionsGroupCustomer)
  {
    try
    {
      if (LOC_ID != PermissionsGroupCustomer.LOC_ID && ID != PermissionsGroupCustomer.ID)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.PermissionsGroupCustomerExists(PermissionsGroupCustomer.LOC_ID, PermissionsGroupCustomer.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<web_PhanQuyenNhomKhachHang>(PermissionsGroupCustomer).State = EntityState.Modified;
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
  public async Task<ActionResult<web_PhanQuyenNhomKhachHang>> PostPermissionsGroupCustomer(
    web_PhanQuyenNhomKhachHang PermissionsGroupCustomer)
  {
    try
    {
      if (this.PermissionsGroupCustomerExists(PermissionsGroupCustomer.LOC_ID, PermissionsGroupCustomer.ID))
        return (ActionResult<web_PhanQuyenNhomKhachHang>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại {PermissionsGroupCustomer.LOC_ID}-{PermissionsGroupCustomer.ID} trong dữ liệu!",
          Data = (object) "",
          CheckValue = true
        });
      this._context.web_PhanQuyenNhomKhachHang.Add(PermissionsGroupCustomer);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      return (ActionResult<web_PhanQuyenNhomKhachHang>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) PermissionsGroupCustomer
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<web_PhanQuyenNhomKhachHang>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeletePermissionsGroupCustomer(string LOC_ID, string ID)
  {
    try
    {
      web_PhanQuyenNhomKhachHang PermissionsGroupCustomer = await this._context.web_PhanQuyenNhomKhachHang.FirstOrDefaultAsync<web_PhanQuyenNhomKhachHang>((Expression<Func<web_PhanQuyenNhomKhachHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (PermissionsGroupCustomer == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.web_PhanQuyenNhomKhachHang.Remove(PermissionsGroupCustomer);
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

  private bool PermissionsGroupCustomerExists(string LOC_ID, string ID)
  {
    return this._context.web_PhanQuyenNhomKhachHang.Any<web_PhanQuyenNhomKhachHang>((Expression<Func<web_PhanQuyenNhomKhachHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }
}
