// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.PermissionsController
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
public class PermissionsController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public PermissionsController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetPermissions(string LOC_ID)
  {
    try
    {
      List<web_PhanQuyen> lstValue = await this._context.web_PhanQuyen.Where<web_PhanQuyen>((Expression<Func<web_PhanQuyen, bool>>) (e => e.LOC_ID == LOC_ID)).ToListAsync<web_PhanQuyen>();
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
  public async Task<IActionResult> GetPermissions(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      List<web_PhanQuyen> lstValue = await this._context.web_PhanQuyen.Where<web_PhanQuyen>((Expression<Func<web_PhanQuyen, bool>>) (e => e.LOC_ID == LOC_ID)).Where<web_PhanQuyen>(KeyWhere, (object) ValuesSearch).ToListAsync<web_PhanQuyen>();
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
  public async Task<IActionResult> GetPermissions(string LOC_ID, string id)
  {
    try
    {
      web_PhanQuyen Permissions = await this._context.web_PhanQuyen.FirstOrDefaultAsync<web_PhanQuyen>((Expression<Func<web_PhanQuyen, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == id));
      if (Permissions == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{id} dữ liệu!",
          Data = (object) ""
        });
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) Permissions
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
  public async Task<IActionResult> PutPermissions(
    string LOC_ID,
    string id,
    web_PhanQuyen Permissions)
  {
    try
    {
      if (LOC_ID != Permissions.LOC_ID && id != Permissions.ID)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.PermissionsExists(Permissions.LOC_ID, Permissions.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{id} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<web_PhanQuyen>(Permissions).State = EntityState.Modified;
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
          this._context.Database.ExecuteSqlRaw($"UPDATE web_PhanQuyen SET ISACTIVE = 0 WHERE LOC_ID ='{FirstOrDefault.LOC_ID}' AND ID_NHOMQUYEN = '{FirstOrDefault.idNhomQuyen}'");
          foreach (DatabaseTHP.Treeview.Treeview treeview in lstTreeview.Where<DatabaseTHP.Treeview.Treeview>((Func<DatabaseTHP.Treeview.Treeview, bool>) (s => s.Name.StartsWith("TBL_ITEM-") && s.Checked)))
          {
            DatabaseTHP.Treeview.Treeview itm = treeview;
            web_PhanQuyen checkSP = await this._context.web_PhanQuyen.FirstOrDefaultAsync<web_PhanQuyen>((Expression<Func<web_PhanQuyen, bool>>) (e => e.ID_NHOMQUYEN == itm.idNhomQuyen && e.ID_QUYEN == itm.id));
            if (checkSP == null)
            {
              web_PhanQuyen newweb_PhanQuyen = new web_PhanQuyen();
              newweb_PhanQuyen.LOC_ID = itm.LOC_ID;
              newweb_PhanQuyen.ID = Guid.NewGuid().ToString();
              newweb_PhanQuyen.ID_QUYEN = itm.id;
              newweb_PhanQuyen.ID_NHOMQUYEN = itm.idNhomQuyen;
              newweb_PhanQuyen.ISACTIVE = itm.Checked;
              this._context.web_PhanQuyen.Add(newweb_PhanQuyen);
              newweb_PhanQuyen = (web_PhanQuyen) null;
            }
            else
            {
              checkSP.ISACTIVE = itm.Checked;
              this._context.Entry<web_PhanQuyen>(checkSP).State = EntityState.Modified;
            }
            AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
            auditLog.InserAuditLog();
            int num = await this._context.SaveChangesAsync();
            checkSP = (web_PhanQuyen) null;
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

  [HttpDelete("{LOC_ID}/{id}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeletePermissions(string LOC_ID, string id)
  {
    try
    {
      web_PhanQuyen Permissions = await this._context.web_PhanQuyen.FirstOrDefaultAsync<web_PhanQuyen>((Expression<Func<web_PhanQuyen, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == id));
      if (Permissions == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{id} dữ liệu!",
          Data = (object) ""
        });
      this._context.web_PhanQuyen.Remove(Permissions);
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

  private bool PermissionsExists(string LOC_ID, string id)
  {
    return this._context.web_PhanQuyen.Any<web_PhanQuyen>((Expression<Func<web_PhanQuyen, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == id));
  }
}
