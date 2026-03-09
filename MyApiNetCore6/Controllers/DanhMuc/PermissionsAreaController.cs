// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.PermissionsAreaController
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
public class PermissionsAreaController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public PermissionsAreaController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetPermissionsArea(string LOC_ID)
  {
    try
    {
      List<web_PhanQuyenKhuVuc> lstValue = await this._context.web_PhanQuyenKhuVuc.Where<web_PhanQuyenKhuVuc>((Expression<Func<web_PhanQuyenKhuVuc, bool>>) (e => e.LOC_ID == LOC_ID)).ToListAsync<web_PhanQuyenKhuVuc>();
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
  public async Task<IActionResult> GetPermissionsArea(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      List<web_PhanQuyenKhuVuc> lstValue = await this._context.web_PhanQuyenKhuVuc.Where<web_PhanQuyenKhuVuc>((Expression<Func<web_PhanQuyenKhuVuc, bool>>) (e => e.LOC_ID == LOC_ID)).Where<web_PhanQuyenKhuVuc>(KeyWhere, (object) ValuesSearch).ToListAsync<web_PhanQuyenKhuVuc>();
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
  public async Task<IActionResult> GetPermissionsArea(string LOC_ID, string ID)
  {
    try
    {
      web_PhanQuyenKhuVuc PermissionsArea = await this._context.web_PhanQuyenKhuVuc.FirstOrDefaultAsync<web_PhanQuyenKhuVuc>((Expression<Func<web_PhanQuyenKhuVuc, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (PermissionsArea == null)
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
        Data = (object) PermissionsArea
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
  public async Task<IActionResult> PutPermissionsArea(
    string LOC_ID,
    string ID,
    web_PhanQuyenKhuVuc PermissionsArea)
  {
    try
    {
      if (LOC_ID != PermissionsArea.LOC_ID && ID != PermissionsArea.ID)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.PermissionsAreaExists(PermissionsArea.LOC_ID, PermissionsArea.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<web_PhanQuyenKhuVuc>(PermissionsArea).State = EntityState.Modified;
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
  public async Task<ActionResult<List<DatabaseTHP.Treeview.Treeview>>> PostPermissionsArea(
    List<DatabaseTHP.Treeview.Treeview> lstTreeview)
  {
    try
    {
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        DatabaseTHP.Treeview.Treeview FirstOrDefault = lstTreeview.FirstOrDefault<DatabaseTHP.Treeview.Treeview>();
        if (FirstOrDefault != null)
        {
          this._context.Database.ExecuteSqlRaw($"UPDATE web_PhanQuyenKhuVuc SET ISACTIVE = 0 WHERE LOC_ID ='{FirstOrDefault.LOC_ID}' AND ID_NHOMQUYEN = '{FirstOrDefault.idNhomQuyen}'");
          foreach (DatabaseTHP.Treeview.Treeview treeview in lstTreeview.Where<DatabaseTHP.Treeview.Treeview>((Func<DatabaseTHP.Treeview.Treeview, bool>) (s => s.Name == "TBL_DEPT")))
          {
            DatabaseTHP.Treeview.Treeview itm = treeview;
            web_PhanQuyenKhuVuc checkSP = await this._context.web_PhanQuyenKhuVuc.FirstOrDefaultAsync<web_PhanQuyenKhuVuc>((Expression<Func<web_PhanQuyenKhuVuc, bool>>) (e => e.ID_NHOMQUYEN == itm.idNhomQuyen && e.ID_KHUVUC == itm.id));
            if (checkSP == null && lstTreeview.Where<DatabaseTHP.Treeview.Treeview>((Func<DatabaseTHP.Treeview.Treeview, bool>) (s => s.idNhomSanPham == itm.id && s.Checked)).Count<DatabaseTHP.Treeview.Treeview>() > 0)
            {
              web_PhanQuyenKhuVuc web_PhanQuyenKhuVuc = new web_PhanQuyenKhuVuc();
              web_PhanQuyenKhuVuc.LOC_ID = itm.LOC_ID;
              web_PhanQuyenKhuVuc.ID = Guid.NewGuid().ToString();
              web_PhanQuyenKhuVuc.ID_KHUVUC = itm.id;
              web_PhanQuyenKhuVuc.ID_NHOMQUYEN = itm.idNhomQuyen;
              web_PhanQuyenKhuVuc.ISACTIVE = itm.Checked;
              this._context.web_PhanQuyenKhuVuc.Add(web_PhanQuyenKhuVuc);
              web_PhanQuyenKhuVuc = (web_PhanQuyenKhuVuc) null;
            }
            else if (checkSP != null)
            {
              checkSP.ISACTIVE = itm.Checked;
              this._context.Entry<web_PhanQuyenKhuVuc>(checkSP).State = EntityState.Modified;
            }
            AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
            auditLog.InserAuditLog();
            int num = await this._context.SaveChangesAsync();
            checkSP = (web_PhanQuyenKhuVuc) null;
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
  public async Task<IActionResult> DeletePermissionsArea(string LOC_ID, string ID)
  {
    try
    {
      web_PhanQuyenKhuVuc PermissionsArea = await this._context.web_PhanQuyenKhuVuc.FirstOrDefaultAsync<web_PhanQuyenKhuVuc>((Expression<Func<web_PhanQuyenKhuVuc, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (PermissionsArea == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.web_PhanQuyenKhuVuc.Remove(PermissionsArea);
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

  private bool PermissionsAreaExists(string LOC_ID, string ID)
  {
    return this._context.web_PhanQuyenKhuVuc.Any<web_PhanQuyenKhuVuc>((Expression<Func<web_PhanQuyenKhuVuc, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }
}
