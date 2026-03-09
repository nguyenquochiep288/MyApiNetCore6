// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.GroupCustomerController
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using DatabaseTHP;
using DatabaseTHP.Class;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
public class GroupCustomerController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public GroupCustomerController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetGroupCusVen(string LOC_ID)
  {
    try
    {
      List<dm_NhomKhachHang> lstValue = await this._context.dm_NhomKhachHang.Where<dm_NhomKhachHang>((Expression<Func<dm_NhomKhachHang, bool>>) (e => e.LOC_ID == LOC_ID)).OrderBy<dm_NhomKhachHang, string>((Expression<Func<dm_NhomKhachHang, string>>) (e => e.MA)).ToListAsync<dm_NhomKhachHang>();
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
  public async Task<IActionResult> GetGroupCusVen(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<dm_NhomKhachHang> lstValue = await this._context.dm_NhomKhachHang.Where<dm_NhomKhachHang>((Expression<Func<dm_NhomKhachHang, bool>>) (e => e.LOC_ID == LOC_ID)).Where<dm_NhomKhachHang>(KeyWhere, (object) ValuesSearch).OrderBy<dm_NhomKhachHang, string>((Expression<Func<dm_NhomKhachHang, string>>) (e => e.MA)).ToListAsync<dm_NhomKhachHang>();
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
  public async Task<IActionResult> GetGroupCusVen(string LOC_ID, string ID)
  {
    try
    {
      dm_NhomKhachHang GroupCusVen = await this._context.dm_NhomKhachHang.FirstOrDefaultAsync<dm_NhomKhachHang>((Expression<Func<dm_NhomKhachHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (GroupCusVen == null)
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
        Data = (object) GroupCusVen
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
  public async Task<IActionResult> PutGroupCusVen(
    string LOC_ID,
    string MA,
    dm_NhomKhachHang GroupCusVen)
  {
    try
    {
      if (this.GroupCusVenExists(GroupCusVen))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{GroupCusVen.LOC_ID}-{GroupCusVen.MA} trong dữ liệu!",
          Data = (object) ""
        });
      if (LOC_ID != GroupCusVen.LOC_ID || GroupCusVen.MA != MA)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.GroupCusVenExistsID(LOC_ID, GroupCusVen.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{GroupCusVen.ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<dm_NhomKhachHang>(GroupCusVen).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) GroupCusVen
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
  public async Task<ActionResult<dm_NhomKhachHang>> PostGroupCusVen(dm_NhomKhachHang GroupCusVen)
  {
    try
    {
      if (this.GroupCusVenExistsMA(GroupCusVen.LOC_ID, GroupCusVen.MA))
        return (ActionResult<dm_NhomKhachHang>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{GroupCusVen.LOC_ID}-{GroupCusVen.MA} trong dữ liệu!",
          Data = (object) ""
        });
      this._context.dm_NhomKhachHang.Add(GroupCusVen);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      return (ActionResult<dm_NhomKhachHang>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) GroupCusVen
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_NhomKhachHang>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteGroupCusVen(string LOC_ID, string ID)
  {
    try
    {
      dm_NhomKhachHang GroupCusVen = await this._context.dm_NhomKhachHang.FirstOrDefaultAsync<dm_NhomKhachHang>((Expression<Func<dm_NhomKhachHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (GroupCusVen == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_NhomKhachHang>(GroupCusVen, GroupCusVen.ID, GroupCusVen.MA);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        List<web_PhanQuyenSanPham> lstweb_PhanQuyenSanPham = await this._context.web_PhanQuyenSanPham.Where<web_PhanQuyenSanPham>((Expression<Func<web_PhanQuyenSanPham, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_NHOMQUYEN == ID)).ToListAsync<web_PhanQuyenSanPham>();
        if (lstweb_PhanQuyenSanPham != null)
        {
          foreach (web_PhanQuyenSanPham itm in lstweb_PhanQuyenSanPham)
            this._context.web_PhanQuyenSanPham.Remove(itm);
        }
        List<web_PhanQuyenNhomSanPham> lstweb_PhanQuyenNhomSanPham = await this._context.web_PhanQuyenNhomSanPham.Where<web_PhanQuyenNhomSanPham>((Expression<Func<web_PhanQuyenNhomSanPham, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_NHOMQUYEN == ID)).ToListAsync<web_PhanQuyenNhomSanPham>();
        if (lstweb_PhanQuyenNhomSanPham != null)
        {
          foreach (web_PhanQuyenNhomSanPham itm in lstweb_PhanQuyenNhomSanPham)
            this._context.web_PhanQuyenNhomSanPham.Remove(itm);
        }
        List<web_PhanQuyenKhuVuc> lstweb_PhanQuyenKhuVuc = await this._context.web_PhanQuyenKhuVuc.Where<web_PhanQuyenKhuVuc>((Expression<Func<web_PhanQuyenKhuVuc, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_NHOMQUYEN == ID)).ToListAsync<web_PhanQuyenKhuVuc>();
        if (lstweb_PhanQuyenKhuVuc != null)
        {
          foreach (web_PhanQuyenKhuVuc itm in lstweb_PhanQuyenKhuVuc)
            this._context.web_PhanQuyenKhuVuc.Remove(itm);
        }
        List<web_PhanQuyenKhachHang> lstweb_PhanQuyenKhachHang = await this._context.web_PhanQuyenKhachHang.Where<web_PhanQuyenKhachHang>((Expression<Func<web_PhanQuyenKhachHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_NHOMQUYEN == ID)).ToListAsync<web_PhanQuyenKhachHang>();
        if (lstweb_PhanQuyenKhachHang != null)
        {
          foreach (web_PhanQuyenKhachHang itm in lstweb_PhanQuyenKhachHang)
            this._context.web_PhanQuyenKhachHang.Remove(itm);
        }
        this._context.dm_NhomKhachHang.Remove(GroupCusVen);
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        lstweb_PhanQuyenSanPham = (List<web_PhanQuyenSanPham>) null;
        lstweb_PhanQuyenNhomSanPham = (List<web_PhanQuyenNhomSanPham>) null;
        lstweb_PhanQuyenKhuVuc = (List<web_PhanQuyenKhuVuc>) null;
        lstweb_PhanQuyenKhachHang = (List<web_PhanQuyenKhachHang>) null;
        auditLog = (AuditLogController) null;
        transaction.Commit();
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) ""
        });
      }
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

  private bool GroupCusVenExistsMA(string LOC_ID, string MA)
  {
    return this._context.dm_NhomKhachHang.Any<dm_NhomKhachHang>((Expression<Func<dm_NhomKhachHang, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == MA));
  }

  private bool GroupCusVenExistsID(string LOC_ID, string ID)
  {
    return this._context.dm_NhomKhachHang.Any<dm_NhomKhachHang>((Expression<Func<dm_NhomKhachHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }

  private bool GroupCusVenExists(dm_NhomKhachHang GroupCusVen)
  {
    return this._context.dm_NhomKhachHang.Any<dm_NhomKhachHang>((Expression<Func<dm_NhomKhachHang, bool>>) (e => e.LOC_ID == GroupCusVen.LOC_ID && e.MA == GroupCusVen.MA && e.ID != GroupCusVen.ID));
  }
}
