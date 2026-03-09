// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.CustomerController
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
public class CustomerController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public CustomerController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetCustomer(string LOC_ID)
  {
    try
    {
      List<view_dm_KhachHang> lstValue = await this._context.view_dm_KhachHang.Where<view_dm_KhachHang>((Expression<Func<view_dm_KhachHang, bool>>) (e => e.LOC_ID == LOC_ID)).OrderBy<view_dm_KhachHang, string>((Expression<Func<view_dm_KhachHang, string>>) (e => e.MA)).ToListAsync<view_dm_KhachHang>();
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
  public async Task<IActionResult> GetCustomer(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<view_dm_KhachHang> lstValue = await this._context.view_dm_KhachHang.Where<view_dm_KhachHang>((Expression<Func<view_dm_KhachHang, bool>>) (e => e.LOC_ID == LOC_ID)).Where<view_dm_KhachHang>(KeyWhere, (object) ValuesSearch).OrderBy<view_dm_KhachHang, string>((Expression<Func<view_dm_KhachHang, string>>) (e => e.MA)).ToListAsync<view_dm_KhachHang>();
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
  public async Task<IActionResult> GetCustomer(string LOC_ID, string ID)
  {
    try
    {
      view_dm_KhachHang Customer = await this._context.view_dm_KhachHang.FirstOrDefaultAsync<view_dm_KhachHang>((Expression<Func<view_dm_KhachHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Customer == null)
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
        Data = (object) Customer
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
  public async Task<IActionResult> PutCustomer(string LOC_ID, string MA, dm_KhachHang Customer)
  {
    try
    {
      if (LOC_ID != Customer.LOC_ID || Customer.MA != MA)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (this.CustomerExists(Customer))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Customer.LOC_ID}-{Customer.MA} trong dữ liệu!",
          Data = (object) ""
        });
      if (!this.CustomerExistsID(LOC_ID, Customer.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{Customer.ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<dm_KhachHang>(Customer).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      view_dm_KhachHang OKCustomer = await this._context.view_dm_KhachHang.FirstOrDefaultAsync<view_dm_KhachHang>((Expression<Func<view_dm_KhachHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == Customer.ID));
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKCustomer
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
  public async Task<ActionResult<dm_KhachHang>> PostCustomer(dm_KhachHang Customer)
  {
    try
    {
      if (this.CustomerExistsMA(Customer.LOC_ID, Customer.MA))
        return (ActionResult<dm_KhachHang>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Customer.LOC_ID}-{Customer.MA} trong dữ liệu!",
          Data = (object) ""
        });
      this._context.dm_KhachHang.Add(Customer);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      view_dm_KhachHang OKCustomer = await this._context.view_dm_KhachHang.FirstOrDefaultAsync<view_dm_KhachHang>((Expression<Func<view_dm_KhachHang, bool>>) (e => e.LOC_ID == Customer.LOC_ID && e.ID == Customer.ID));
      return (ActionResult<dm_KhachHang>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKCustomer
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_KhachHang>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteCustomer(string LOC_ID, string ID)
  {
    try
    {
      dm_KhachHang Customer = await this._context.dm_KhachHang.FirstOrDefaultAsync<dm_KhachHang>((Expression<Func<dm_KhachHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Customer == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_KhachHang>(Customer, Customer.ID, Customer.MA);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      List<web_PhanQuyenKhachHang> lstweb_PhanQuyenSanPham = await this._context.web_PhanQuyenKhachHang.Where<web_PhanQuyenKhachHang>((Expression<Func<web_PhanQuyenKhachHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_KHACHHANG == ID)).ToListAsync<web_PhanQuyenKhachHang>();
      if (lstweb_PhanQuyenSanPham != null)
      {
        foreach (web_PhanQuyenKhachHang itm in lstweb_PhanQuyenSanPham)
          this._context.web_PhanQuyenKhachHang.Remove(itm);
      }
      this._context.dm_KhachHang.Remove(Customer);
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

  private bool CustomerExistsMA(string LOC_ID, string MA)
  {
    return this._context.dm_KhachHang.Any<dm_KhachHang>((Expression<Func<dm_KhachHang, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == MA));
  }

  private bool CustomerExistsID(string LOC_ID, string ID)
  {
    return this._context.dm_KhachHang.Any<dm_KhachHang>((Expression<Func<dm_KhachHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }

  private bool CustomerExists(dm_KhachHang Customer)
  {
    return this._context.dm_KhachHang.Any<dm_KhachHang>((Expression<Func<dm_KhachHang, bool>>) (e => e.LOC_ID == Customer.LOC_ID && e.MA == Customer.MA && e.ID != Customer.ID));
  }
}
