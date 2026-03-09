// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.PermissionsCustomerController
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


namespace MyApiNetCore6.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PermissionsCustomerController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public PermissionsCustomerController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetPermissionsCustomer(string LOC_ID)
  {
    try
    {
      List<web_PhanQuyenKhachHang> lstValue = await this._context.web_PhanQuyenKhachHang.Where<web_PhanQuyenKhachHang>((Expression<Func<web_PhanQuyenKhachHang, bool>>) (e => e.LOC_ID == LOC_ID)).ToListAsync<web_PhanQuyenKhachHang>();
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
  public async Task<IActionResult> GetPermissionsCustomer(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      List<web_PhanQuyenKhachHang> lstValue = await this._context.web_PhanQuyenKhachHang.Where<web_PhanQuyenKhachHang>((Expression<Func<web_PhanQuyenKhachHang, bool>>) (e => e.LOC_ID == LOC_ID)).Where<web_PhanQuyenKhachHang>(KeyWhere, (object) ValuesSearch).ToListAsync<web_PhanQuyenKhachHang>();
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
  public async Task<IActionResult> GetPermissionsCustomer(string LOC_ID, string ID)
  {
    try
    {
      web_PhanQuyenKhachHang PermissionsCustomer = await this._context.web_PhanQuyenKhachHang.FirstOrDefaultAsync<web_PhanQuyenKhachHang>((Expression<Func<web_PhanQuyenKhachHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (PermissionsCustomer == null)
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
        Data = (object) PermissionsCustomer
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

  [HttpPut("{LOC_ID}/{id}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PutPermissionsCustomer(
    string LOC_ID,
    string id,
    web_PhanQuyenKhachHang PermissionsCustomer)
  {
    try
    {
      if (LOC_ID != PermissionsCustomer.LOC_ID && id != PermissionsCustomer.ID)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.PermissionsCustomerExists(PermissionsCustomer.LOC_ID, PermissionsCustomer.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{id} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<web_PhanQuyenKhachHang>(PermissionsCustomer).State = EntityState.Modified;
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
  public async Task<ActionResult<List<DatabaseTHP.Treeview.Treeview>>> PostPermissionsCustomer(
    List<DatabaseTHP.Treeview.Treeview> lstTreeview)
  {
    try
    {
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        DatabaseTHP.Treeview.Treeview FirstOrDefault = lstTreeview.FirstOrDefault<DatabaseTHP.Treeview.Treeview>();
        if (FirstOrDefault != null)
        {
          this._context.Database.ExecuteSqlRaw($"UPDATE web_PhanQuyenKhuVuc SET ISACTIVE = 0, ISPHANQUYENKHUVUC = 0 WHERE LOC_ID ='{FirstOrDefault.LOC_ID}' AND ID_NHOMQUYEN = '{FirstOrDefault.idNhomQuyen}' AND ID_LICHLAMVIEC = '{FirstOrDefault.idLichLamViec}'");
          foreach (DatabaseTHP.Treeview.Treeview treeview in lstTreeview.Where<DatabaseTHP.Treeview.Treeview>((Func<DatabaseTHP.Treeview.Treeview, bool>) (s => s.Name == "TBL_DEPT" && s.Checked)))
          {
            DatabaseTHP.Treeview.Treeview itm = treeview;
            web_PhanQuyenKhuVuc checkSP = await this._context.web_PhanQuyenKhuVuc.FirstOrDefaultAsync<web_PhanQuyenKhuVuc>((Expression<Func<web_PhanQuyenKhuVuc, bool>>) (e => e.ID_NHOMQUYEN == itm.idNhomQuyen && e.ID_KHUVUC == itm.id && e.ID_LICHLAMVIEC == itm.idLichLamViec));
            if (checkSP == null && lstTreeview.Where<DatabaseTHP.Treeview.Treeview>((Func<DatabaseTHP.Treeview.Treeview, bool>) (s => s.idNhomSanPham == itm.id && s.Checked)).Count<DatabaseTHP.Treeview.Treeview>() > 0)
            {
              web_PhanQuyenKhuVuc web_PhanQuyenKhuVuc = new web_PhanQuyenKhuVuc();
              web_PhanQuyenKhuVuc.LOC_ID = itm.LOC_ID;
              web_PhanQuyenKhuVuc.ID = Guid.NewGuid().ToString();
              web_PhanQuyenKhuVuc.ID_KHUVUC = itm.id;
              web_PhanQuyenKhuVuc.ID_NHOMQUYEN = itm.idNhomQuyen;
              web_PhanQuyenKhuVuc.ID_LICHLAMVIEC = itm.idLichLamViec;
              web_PhanQuyenKhuVuc.ISACTIVE = itm.Checked;
              web_PhanQuyenKhuVuc.ISPHANQUYENKHUVUC = lstTreeview.Where<DatabaseTHP.Treeview.Treeview>((Func<DatabaseTHP.Treeview.Treeview, bool>) (s => s.Name == "TBL_DEPTALL" && s.id == itm.id)).Select<DatabaseTHP.Treeview.Treeview, bool>((Func<DatabaseTHP.Treeview.Treeview, bool>) (s => s.Checked)).FirstOrDefault<bool>();
              this._context.web_PhanQuyenKhuVuc.Add(web_PhanQuyenKhuVuc);
              web_PhanQuyenKhuVuc = (web_PhanQuyenKhuVuc) null;
            }
            else if (checkSP != null)
            {
              checkSP.ISACTIVE = itm.Checked;
              checkSP.ISPHANQUYENKHUVUC = lstTreeview.Where<DatabaseTHP.Treeview.Treeview>((Func<DatabaseTHP.Treeview.Treeview, bool>) (s => s.Name == "TBL_DEPTALL" && s.id == itm.id)).Select<DatabaseTHP.Treeview.Treeview, bool>((Func<DatabaseTHP.Treeview.Treeview, bool>) (s => s.Checked)).FirstOrDefault<bool>();
              this._context.Entry<web_PhanQuyenKhuVuc>(checkSP).State = EntityState.Modified;
            }
            AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
            auditLog.InserAuditLog();
            int num = await this._context.SaveChangesAsync();
            checkSP = (web_PhanQuyenKhuVuc) null;
            auditLog = (AuditLogController) null;
          }
          this._context.Database.ExecuteSqlRaw($"UPDATE web_PhanQuyenKhachHang SET ISACTIVE = 0 WHERE LOC_ID ='{FirstOrDefault.LOC_ID}' AND ID_NHOMQUYEN = '{FirstOrDefault.idNhomQuyen}' AND ID_LICHLAMVIEC = '{FirstOrDefault.idLichLamViec}'");
          foreach (DatabaseTHP.Treeview.Treeview treeview in lstTreeview.Where<DatabaseTHP.Treeview.Treeview>((Func<DatabaseTHP.Treeview.Treeview, bool>) (s => s.Name.StartsWith("TBL_ITEM-") && s.Checked)))
          {
            DatabaseTHP.Treeview.Treeview itm = treeview;
            web_PhanQuyenKhachHang checkSP = await this._context.web_PhanQuyenKhachHang.FirstOrDefaultAsync<web_PhanQuyenKhachHang>((Expression<Func<web_PhanQuyenKhachHang, bool>>) (e => e.ID_NHOMQUYEN == itm.idNhomQuyen && e.ID_KHACHHANG == itm.id && e.ID_LICHLAMVIEC == itm.idLichLamViec));
            if (checkSP == null)
            {
              web_PhanQuyenKhachHang web_PhanQuyenKhachHang = new web_PhanQuyenKhachHang();
              web_PhanQuyenKhachHang.LOC_ID = itm.LOC_ID;
              web_PhanQuyenKhachHang.ID = Guid.NewGuid().ToString();
              web_PhanQuyenKhachHang.ID_KHACHHANG = itm.id;
              web_PhanQuyenKhachHang.ID_NHOMQUYEN = itm.idNhomQuyen;
              web_PhanQuyenKhachHang.ISACTIVE = itm.Checked;
              web_PhanQuyenKhachHang.ID_LICHLAMVIEC = itm.idLichLamViec;
              this._context.web_PhanQuyenKhachHang.Add(web_PhanQuyenKhachHang);
              web_PhanQuyenKhachHang = (web_PhanQuyenKhachHang) null;
            }
            else
            {
              checkSP.ISACTIVE = itm.Checked;
              this._context.Entry<web_PhanQuyenKhachHang>(checkSP).State = EntityState.Modified;
            }
            AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
            auditLog.InserAuditLog();
            int num = await this._context.SaveChangesAsync();
            checkSP = (web_PhanQuyenKhachHang) null;
            auditLog = (AuditLogController) null;
          }
        }
        transaction.Commit();
        return (ActionResult<List<DatabaseTHP.Treeview.Treeview>>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) lstTreeview
        });
      }
    }
    catch (Exception ex)
    {
      return (ActionResult<List<DatabaseTHP.Treeview.Treeview>>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeletePermissionsCustomer(string LOC_ID, string ID)
  {
    try
    {
      web_PhanQuyenKhachHang PermissionsCustomer = await this._context.web_PhanQuyenKhachHang.FirstOrDefaultAsync<web_PhanQuyenKhachHang>((Expression<Func<web_PhanQuyenKhachHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (PermissionsCustomer == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.web_PhanQuyenKhachHang.Remove(PermissionsCustomer);
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

  private bool PermissionsCustomerExists(string LOC_ID, string ID)
  {
    return this._context.web_PhanQuyenKhachHang.Any<web_PhanQuyenKhachHang>((Expression<Func<web_PhanQuyenKhachHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }
}
