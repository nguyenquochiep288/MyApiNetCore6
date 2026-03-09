// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.MenuController
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
public class MenuController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public MenuController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetMenu()
  {
    try
    {
      List<view_web_Menu> lstValue = await this._context.view_web_Menu.ToListAsync<view_web_Menu>();
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

  [HttpGet("{Type}/{KeyWhere}/{ValuesSearch}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetMenu(int Type, string KeyWhere = "", string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<view_web_Menu> lstValue = await this._context.view_web_Menu.Where<view_web_Menu>(KeyWhere, (object) ValuesSearch).ToListAsync<view_web_Menu>();
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

  [HttpGet("{id}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetMenu(string id)
  {
    try
    {
      view_web_Menu Menu = await this._context.view_web_Menu.FirstOrDefaultAsync<view_web_Menu>((Expression<Func<view_web_Menu, bool>>) (e => e.ID == id));
      if (Menu == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {id} dữ liệu!",
          Data = (object) ""
        });
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) Menu
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

  [HttpPut("{id}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PutMenu(string id, web_Menu Menu)
  {
    try
    {
      if (id != Menu.ID)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.MenuExists(Menu.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {id} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<web_Menu>(Menu).State = EntityState.Modified;
      List<view_web_Menu> lstMenu = await this._context.view_web_Menu.Where<view_web_Menu>((Expression<Func<view_web_Menu, bool>>) (e => e.ID == Menu.ID)).ToListAsync<view_web_Menu>();
      foreach (view_web_Menu viewWebMenu in lstMenu)
      {
        view_web_Menu itm = viewWebMenu;
        List<web_Quyen> lstQuyen = await this._context.web_Quyen.Where<web_Quyen>((Expression<Func<web_Quyen, bool>>) (e => e.ID_MENU == itm.ID)).ToListAsync<web_Quyen>();
        if (lstQuyen == null || lstQuyen.Count<web_Quyen>() <= 0)
        {
          if (itm.ACTIONNAME == "Index" && itm.CONTROLLERNAME != "ViewReport")
          {
            web_Quyen newweb_Quyen = new web_Quyen();
            web_Quyen Quyen = await this._context.web_Quyen.FirstOrDefaultAsync<web_Quyen>((Expression<Func<web_Quyen, bool>>) (e => e.ID_MENU == itm.ID && e.MAQUYEN == "View"));
            if (Quyen == null)
            {
              newweb_Quyen = new web_Quyen();
              newweb_Quyen.ID = Guid.NewGuid().ToString();
              newweb_Quyen.LOC_ID = "02";
              newweb_Quyen.MAQUYEN = "View";
              newweb_Quyen.TENQUYEN = "Xem";
              newweb_Quyen.ID_MENU = itm.ID;
              this._context.web_Quyen.Add(newweb_Quyen);
            }
            Quyen = await this._context.web_Quyen.FirstOrDefaultAsync<web_Quyen>((Expression<Func<web_Quyen, bool>>) (e => e.ID_MENU == itm.ID && e.MAQUYEN == "Edit"));
            if (Quyen == null)
            {
              newweb_Quyen = new web_Quyen();
              newweb_Quyen.ID = Guid.NewGuid().ToString();
              newweb_Quyen.LOC_ID = "02";
              newweb_Quyen.MAQUYEN = "Edit";
              newweb_Quyen.TENQUYEN = "Sửa";
              newweb_Quyen.ID_MENU = itm.ID;
              this._context.web_Quyen.Add(newweb_Quyen);
            }
            Quyen = await this._context.web_Quyen.FirstOrDefaultAsync<web_Quyen>((Expression<Func<web_Quyen, bool>>) (e => e.ID_MENU == itm.ID && e.MAQUYEN == "Delete"));
            if (Quyen == null)
            {
              newweb_Quyen = new web_Quyen();
              newweb_Quyen.ID = Guid.NewGuid().ToString();
              newweb_Quyen.LOC_ID = "02";
              newweb_Quyen.MAQUYEN = "Delete";
              newweb_Quyen.TENQUYEN = "Xóa";
              newweb_Quyen.ID_MENU = itm.ID;
              this._context.web_Quyen.Add(newweb_Quyen);
            }
            newweb_Quyen = (web_Quyen) null;
            Quyen = (web_Quyen) null;
          }
          else if (string.IsNullOrEmpty(itm.ACTIONNAME) && !string.IsNullOrEmpty(itm.CONTROLLERNAME))
          {
            web_Menu MenuBaoCao = await this._context.web_Menu.FirstOrDefaultAsync<web_Menu>((Expression<Func<web_Menu, bool>>) (e => e.ID == itm.ID_QUYENCHA));
            if (MenuBaoCao != null && MenuBaoCao.CONTROLLERNAME == "ViewReport")
            {
              web_Quyen newweb_Quyen = new web_Quyen();
              web_Quyen Quyen = await this._context.web_Quyen.FirstOrDefaultAsync<web_Quyen>((Expression<Func<web_Quyen, bool>>) (e => e.ID_MENU == itm.ID && e.MAQUYEN == "View"));
              if (Quyen == null)
              {
                newweb_Quyen = new web_Quyen();
                newweb_Quyen.ID = Guid.NewGuid().ToString();
                newweb_Quyen.LOC_ID = "02";
                newweb_Quyen.MAQUYEN = "View";
                newweb_Quyen.TENQUYEN = "Xem";
                newweb_Quyen.ID_MENU = itm.ID;
                this._context.web_Quyen.Add(newweb_Quyen);
              }
              newweb_Quyen = (web_Quyen) null;
              Quyen = (web_Quyen) null;
            }
            else
            {
              while (MenuBaoCao != null && MenuBaoCao.CONTROLLERNAME == "ViewReport")
              {
                MenuBaoCao = await this._context.web_Menu.FirstOrDefaultAsync<web_Menu>((Expression<Func<web_Menu, bool>>) (e => e.ID == MenuBaoCao.ID_QUYENCHA));
                if (MenuBaoCao != null && MenuBaoCao.CONTROLLERNAME == "ViewReport")
                {
                  web_Quyen newweb_Quyen = new web_Quyen();
                  web_Quyen Quyen = await this._context.web_Quyen.FirstOrDefaultAsync<web_Quyen>((Expression<Func<web_Quyen, bool>>) (e => e.ID_MENU == itm.ID && e.MAQUYEN == "View"));
                  if (Quyen == null)
                  {
                    newweb_Quyen = new web_Quyen();
                    newweb_Quyen.ID = Guid.NewGuid().ToString();
                    newweb_Quyen.LOC_ID = "02";
                    newweb_Quyen.MAQUYEN = "View";
                    newweb_Quyen.TENQUYEN = "Xem";
                    newweb_Quyen.ID_MENU = itm.ID;
                    this._context.web_Quyen.Add(newweb_Quyen);
                  }
                  newweb_Quyen = (web_Quyen) null;
                  Quyen = (web_Quyen) null;
                }
              }
            }
          }
          lstQuyen = (List<web_Quyen>) null;
        }
      }
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      view_web_Menu OkMenu = await this._context.view_web_Menu.FirstOrDefaultAsync<view_web_Menu>((Expression<Func<view_web_Menu, bool>>) (e => e.ID == id));
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OkMenu
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
  public async Task<ActionResult<web_Menu>> PostMenu(web_Menu Menu)
  {
    try
    {
      if (this.MenuExists(Menu.ID))
        return (ActionResult<web_Menu>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại {Menu.ID} trong dữ liệu!",
          Data = (object) "",
          CheckValue = true
        });
      this._context.web_Menu.Add(Menu);
      List<view_web_Menu> lstMenu = await this._context.view_web_Menu.Where<view_web_Menu>((Expression<Func<view_web_Menu, bool>>) (e => e.ID == Menu.ID)).ToListAsync<view_web_Menu>();
      foreach (view_web_Menu viewWebMenu in lstMenu)
      {
        view_web_Menu itm = viewWebMenu;
        List<web_Quyen> lstQuyen = await this._context.web_Quyen.Where<web_Quyen>((Expression<Func<web_Quyen, bool>>) (e => e.ID_MENU == itm.ID)).ToListAsync<web_Quyen>();
        if (lstQuyen == null || lstQuyen.Count<web_Quyen>() <= 0)
        {
          Guid guid;
          if (itm.ACTIONNAME == "Index" && itm.CONTROLLERNAME != "ViewReport")
          {
            web_Quyen newweb_Quyen = new web_Quyen();
            web_Quyen Quyen = await this._context.web_Quyen.FirstOrDefaultAsync<web_Quyen>((Expression<Func<web_Quyen, bool>>) (e => e.ID_MENU == itm.ID && e.MAQUYEN == "View"));
            if (Quyen == null)
            {
              web_Quyen webQuyen = newweb_Quyen;
              guid = Guid.NewGuid();
              string str = guid.ToString();
              webQuyen.ID = str;
              newweb_Quyen.LOC_ID = "02";
              newweb_Quyen.MAQUYEN = "View";
              newweb_Quyen.TENQUYEN = "Xem";
              newweb_Quyen.ID_MENU = itm.ID;
              this._context.web_Quyen.Add(newweb_Quyen);
            }
            Quyen = await this._context.web_Quyen.FirstOrDefaultAsync<web_Quyen>((Expression<Func<web_Quyen, bool>>) (e => e.ID_MENU == itm.ID && e.MAQUYEN == "Edit"));
            if (Quyen == null)
            {
              web_Quyen webQuyen = newweb_Quyen;
              guid = Guid.NewGuid();
              string str = guid.ToString();
              webQuyen.ID = str;
              newweb_Quyen.LOC_ID = "02";
              newweb_Quyen.MAQUYEN = "Edit";
              newweb_Quyen.TENQUYEN = "Sửa";
              newweb_Quyen.ID_MENU = itm.ID;
              this._context.web_Quyen.Add(newweb_Quyen);
            }
            Quyen = await this._context.web_Quyen.FirstOrDefaultAsync<web_Quyen>((Expression<Func<web_Quyen, bool>>) (e => e.ID_MENU == itm.ID && e.MAQUYEN == "Delete"));
            if (Quyen == null)
            {
              web_Quyen webQuyen = newweb_Quyen;
              guid = Guid.NewGuid();
              string str = guid.ToString();
              webQuyen.ID = str;
              newweb_Quyen.LOC_ID = "02";
              newweb_Quyen.MAQUYEN = "Delete";
              newweb_Quyen.TENQUYEN = "Xóa";
              newweb_Quyen.ID_MENU = itm.ID;
              this._context.web_Quyen.Add(newweb_Quyen);
            }
            Quyen = await this._context.web_Quyen.FirstOrDefaultAsync<web_Quyen>((Expression<Func<web_Quyen, bool>>) (e => e.ID_MENU == itm.ID && e.MAQUYEN == "Create"));
            if (Quyen == null)
            {
              web_Quyen webQuyen = newweb_Quyen;
              guid = Guid.NewGuid();
              string str = guid.ToString();
              webQuyen.ID = str;
              newweb_Quyen.LOC_ID = "02";
              newweb_Quyen.MAQUYEN = "Create";
              newweb_Quyen.TENQUYEN = "Thêm";
              newweb_Quyen.ID_MENU = itm.ID;
              this._context.web_Quyen.Add(newweb_Quyen);
            }
            newweb_Quyen = (web_Quyen) null;
            Quyen = (web_Quyen) null;
          }
          else if (string.IsNullOrEmpty(itm.ACTIONNAME) && !string.IsNullOrEmpty(itm.CONTROLLERNAME))
          {
            web_Menu MenuBaoCao = await this._context.web_Menu.FirstOrDefaultAsync<web_Menu>((Expression<Func<web_Menu, bool>>) (e => e.ID == itm.ID_QUYENCHA));
            if (MenuBaoCao != null && MenuBaoCao.CONTROLLERNAME == "ViewReport")
            {
              web_Quyen newweb_Quyen = new web_Quyen();
              web_Quyen Quyen = await this._context.web_Quyen.FirstOrDefaultAsync<web_Quyen>((Expression<Func<web_Quyen, bool>>) (e => e.ID_MENU == itm.ID && e.MAQUYEN == "View"));
              if (Quyen == null)
              {
                web_Quyen webQuyen = newweb_Quyen;
                guid = Guid.NewGuid();
                string str = guid.ToString();
                webQuyen.ID = str;
                newweb_Quyen.LOC_ID = "02";
                newweb_Quyen.MAQUYEN = "View";
                newweb_Quyen.TENQUYEN = "Xem";
                newweb_Quyen.ID_MENU = itm.ID;
                this._context.web_Quyen.Add(newweb_Quyen);
              }
              newweb_Quyen = (web_Quyen) null;
              Quyen = (web_Quyen) null;
            }
            else
            {
              while (MenuBaoCao != null && MenuBaoCao.CONTROLLERNAME == "ViewReport")
              {
                MenuBaoCao = await this._context.web_Menu.FirstOrDefaultAsync<web_Menu>((Expression<Func<web_Menu, bool>>) (e => e.ID == MenuBaoCao.ID_QUYENCHA));
                if (MenuBaoCao != null && MenuBaoCao.CONTROLLERNAME == "ViewReport")
                {
                  web_Quyen newweb_Quyen = new web_Quyen();
                  web_Quyen Quyen = await this._context.web_Quyen.FirstOrDefaultAsync<web_Quyen>((Expression<Func<web_Quyen, bool>>) (e => e.ID_MENU == itm.ID && e.MAQUYEN == "View"));
                  if (Quyen == null)
                  {
                    web_Quyen webQuyen = newweb_Quyen;
                    guid = Guid.NewGuid();
                    string str = guid.ToString();
                    webQuyen.ID = str;
                    newweb_Quyen.LOC_ID = "02";
                    newweb_Quyen.MAQUYEN = "View";
                    newweb_Quyen.TENQUYEN = "Xem";
                    newweb_Quyen.ID_MENU = itm.ID;
                    this._context.web_Quyen.Add(newweb_Quyen);
                  }
                  newweb_Quyen = (web_Quyen) null;
                  Quyen = (web_Quyen) null;
                }
              }
            }
          }
          lstQuyen = (List<web_Quyen>) null;
        }
      }
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      view_web_Menu OkMenu = await this._context.view_web_Menu.FirstOrDefaultAsync<view_web_Menu>((Expression<Func<view_web_Menu, bool>>) (e => e.ID == Menu.ID));
      return (ActionResult<web_Menu>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OkMenu
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<web_Menu>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{id}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteMenu(string id)
  {
    try
    {
      web_Menu Menu = await this._context.web_Menu.FirstOrDefaultAsync<web_Menu>((Expression<Func<web_Menu, bool>>) (e => e.ID == id));
      if (Menu == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {id} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<web_Menu>(Menu, Menu.ID, Menu.NAME);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      List<web_Quyen> lstweb_Quyen = await this._context.web_Quyen.Where<web_Quyen>((Expression<Func<web_Quyen, bool>>) (e => e.ID_MENU == Menu.ID)).ToListAsync<web_Quyen>();
      if (lstweb_Quyen != null)
      {
        foreach (web_Quyen webQuyen in lstweb_Quyen)
        {
          web_Quyen itm = webQuyen;
          List<web_PhanQuyen> lstweb_PhanQuyen = await this._context.web_PhanQuyen.Where<web_PhanQuyen>((Expression<Func<web_PhanQuyen, bool>>) (e => e.ID_QUYEN == itm.ID)).ToListAsync<web_PhanQuyen>();
          if (lstweb_PhanQuyen != null)
          {
            foreach (web_PhanQuyen itm1 in lstweb_PhanQuyen)
              this._context.web_PhanQuyen.Remove(itm1);
          }
          this._context.web_Quyen.Remove(itm);
          lstweb_PhanQuyen = (List<web_PhanQuyen>) null;
        }
      }
      this._context.web_Menu.Remove(Menu);
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

  private bool MenuExists(string id)
  {
    return this._context.web_Menu.Any<web_Menu>((Expression<Func<web_Menu, bool>>) (e => e.ID == id));
  }
}
