// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.PermissionsProductController
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
public class PermissionsProductController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public PermissionsProductController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetPermissionsProduct(string LOC_ID)
  {
    try
    {
      List<web_PhanQuyenSanPham> lstValue = await this._context.web_PhanQuyenSanPham.Where<web_PhanQuyenSanPham>((Expression<Func<web_PhanQuyenSanPham, bool>>) (e => e.LOC_ID == LOC_ID)).ToListAsync<web_PhanQuyenSanPham>();
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
  public async Task<IActionResult> GetPermissionsProduct(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      List<web_PhanQuyenSanPham> lstValue = await this._context.web_PhanQuyenSanPham.Where<web_PhanQuyenSanPham>((Expression<Func<web_PhanQuyenSanPham, bool>>) (e => e.LOC_ID == LOC_ID)).Where<web_PhanQuyenSanPham>(KeyWhere, (object) ValuesSearch).ToListAsync<web_PhanQuyenSanPham>();
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
  public async Task<IActionResult> GetPermissionsProduct(string LOC_ID, string ID)
  {
    try
    {
      web_PhanQuyenSanPham PermissionsProduct = await this._context.web_PhanQuyenSanPham.FirstOrDefaultAsync<web_PhanQuyenSanPham>((Expression<Func<web_PhanQuyenSanPham, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (PermissionsProduct == null)
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
        Data = (object) PermissionsProduct
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
  public async Task<IActionResult> PutPermissionsProduct(
    string LOC_ID,
    string ID,
    web_PhanQuyenSanPham PermissionsProduct)
  {
    try
    {
      if (LOC_ID != PermissionsProduct.LOC_ID && ID != PermissionsProduct.ID)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.PermissionsProductExists(PermissionsProduct.LOC_ID, PermissionsProduct.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<web_PhanQuyenSanPham>(PermissionsProduct).State = EntityState.Modified;
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
  public async Task<ActionResult<List<DatabaseTHP.Treeview.Treeview>>> PostPermissionsProduct(
    List<DatabaseTHP.Treeview.Treeview> lstTreeview)
  {
    try
    {
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        DatabaseTHP.Treeview.Treeview FirstOrDefault = lstTreeview.FirstOrDefault<DatabaseTHP.Treeview.Treeview>();
        if (FirstOrDefault != null)
        {
          this._context.Database.ExecuteSqlRaw($"UPDATE web_PhanQuyenNhomSanPham SET ISACTIVE = 0 WHERE LOC_ID ='{FirstOrDefault.LOC_ID}' AND ID_NHOMQUYEN = '{FirstOrDefault.idNhomQuyen}'");
          foreach (DatabaseTHP.Treeview.Treeview treeview in lstTreeview.Where<DatabaseTHP.Treeview.Treeview>((Func<DatabaseTHP.Treeview.Treeview, bool>) (s => s.Name == "TBL_DEPT")))
          {
            DatabaseTHP.Treeview.Treeview itm = treeview;
            web_PhanQuyenNhomSanPham checkSP = await this._context.web_PhanQuyenNhomSanPham.FirstOrDefaultAsync<web_PhanQuyenNhomSanPham>((Expression<Func<web_PhanQuyenNhomSanPham, bool>>) (e => e.ID_NHOMQUYEN == itm.idNhomQuyen && e.ID_NHOMSANPHAM == itm.id));
            if (checkSP == null && lstTreeview.Where<DatabaseTHP.Treeview.Treeview>((Func<DatabaseTHP.Treeview.Treeview, bool>) (s => s.idNhomSanPham == itm.id && s.Checked)).Count<DatabaseTHP.Treeview.Treeview>() > 0)
            {
              web_PhanQuyenNhomSanPham web_PhanQuyenNhomSanPham = new web_PhanQuyenNhomSanPham();
              web_PhanQuyenNhomSanPham.LOC_ID = itm.LOC_ID;
              web_PhanQuyenNhomSanPham.ID = Guid.NewGuid().ToString();
              web_PhanQuyenNhomSanPham.ID_NHOMSANPHAM = itm.id;
              web_PhanQuyenNhomSanPham.ID_NHOMQUYEN = itm.idNhomQuyen;
              web_PhanQuyenNhomSanPham.ISACTIVE = itm.Checked;
              web_PhanQuyenNhomSanPham.ISPHANQUYENSANPHAM = lstTreeview.Where<DatabaseTHP.Treeview.Treeview>((Func<DatabaseTHP.Treeview.Treeview, bool>) (s => s.Name == "TBL_DEPTALL" && s.id == itm.id)).Select<DatabaseTHP.Treeview.Treeview, bool>((Func<DatabaseTHP.Treeview.Treeview, bool>) (s => s.Checked)).FirstOrDefault<bool>();
              this._context.web_PhanQuyenNhomSanPham.Add(web_PhanQuyenNhomSanPham);
              web_PhanQuyenNhomSanPham = (web_PhanQuyenNhomSanPham) null;
            }
            else if (checkSP != null)
            {
              checkSP.ISACTIVE = itm.Checked;
              checkSP.ISPHANQUYENSANPHAM = lstTreeview.Where<DatabaseTHP.Treeview.Treeview>((Func<DatabaseTHP.Treeview.Treeview, bool>) (s => s.Name == "TBL_DEPTALL" && s.id == itm.id)).Select<DatabaseTHP.Treeview.Treeview, bool>((Func<DatabaseTHP.Treeview.Treeview, bool>) (s => s.Checked)).FirstOrDefault<bool>();
              this._context.Entry<web_PhanQuyenNhomSanPham>(checkSP).State = EntityState.Modified;
            }
            AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
            auditLog.InserAuditLog();
            int num = await this._context.SaveChangesAsync();
            checkSP = (web_PhanQuyenNhomSanPham) null;
            auditLog = (AuditLogController) null;
          }
          this._context.Database.ExecuteSqlRaw($"UPDATE web_PhanQuyenSanPham SET ISACTIVE = 0 WHERE LOC_ID ='{FirstOrDefault.LOC_ID}' AND ID_NHOMQUYEN = '{FirstOrDefault.idNhomQuyen}'");
          foreach (DatabaseTHP.Treeview.Treeview treeview in lstTreeview.Where<DatabaseTHP.Treeview.Treeview>((Func<DatabaseTHP.Treeview.Treeview, bool>) (s => s.Name.StartsWith("TBL_ITEM-") && s.Checked)))
          {
            DatabaseTHP.Treeview.Treeview itm = treeview;
            web_PhanQuyenSanPham checkSP = await this._context.web_PhanQuyenSanPham.FirstOrDefaultAsync<web_PhanQuyenSanPham>((Expression<Func<web_PhanQuyenSanPham, bool>>) (e => e.ID_NHOMQUYEN == itm.idNhomQuyen && e.ID_SANPHAM == itm.id));
            if (checkSP == null)
            {
              web_PhanQuyenSanPham web_PhanQuyenSanPham = new web_PhanQuyenSanPham();
              web_PhanQuyenSanPham.LOC_ID = itm.LOC_ID;
              web_PhanQuyenSanPham.ID = Guid.NewGuid().ToString();
              web_PhanQuyenSanPham.ID_SANPHAM = itm.id;
              web_PhanQuyenSanPham.ID_NHOMQUYEN = itm.idNhomQuyen;
              web_PhanQuyenSanPham.ISACTIVE = itm.Checked;
              this._context.web_PhanQuyenSanPham.Add(web_PhanQuyenSanPham);
              web_PhanQuyenSanPham = (web_PhanQuyenSanPham) null;
            }
            else
            {
              checkSP.ISACTIVE = itm.Checked;
              this._context.Entry<web_PhanQuyenSanPham>(checkSP).State = EntityState.Modified;
            }
            AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
            auditLog.InserAuditLog();
            int num = await this._context.SaveChangesAsync();
            checkSP = (web_PhanQuyenSanPham) null;
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
  public async Task<IActionResult> DeletePermissionsProduct(string LOC_ID, string ID)
  {
    try
    {
      web_PhanQuyenSanPham PermissionsProduct = await this._context.web_PhanQuyenSanPham.FirstOrDefaultAsync<web_PhanQuyenSanPham>((Expression<Func<web_PhanQuyenSanPham, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (PermissionsProduct == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.web_PhanQuyenSanPham.Remove(PermissionsProduct);
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

  private bool PermissionsProductExists(string LOC_ID, string ID)
  {
    return this._context.web_PhanQuyenSanPham.Any<web_PhanQuyenSanPham>((Expression<Func<web_PhanQuyenSanPham, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }
}
