// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.ThongBaoController
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
public class ThongBaoController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public ThongBaoController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet]
  public async Task<IActionResult> GetThongBao()
  {
    try
    {
      List<web_ThongBao> lstValue = await this._context.web_ThongBao.ToListAsync<web_ThongBao>();
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
  public async Task<IActionResult> GetThongBao(int Type, string KeyWhere = "", string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<web_ThongBao> lstValue = await this._context.web_ThongBao.Where<web_ThongBao>(KeyWhere, (object) ValuesSearch).ToListAsync<web_ThongBao>();
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

  [HttpGet("{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetThongBao(string ID)
  {
    try
    {
      web_ThongBao ThongBao = await this._context.web_ThongBao.FirstOrDefaultAsync<web_ThongBao>((Expression<Func<web_ThongBao, bool>>) (e => e.ID == ID));
      if (ThongBao == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy -{ID} dữ liệu!",
          Data = (object) ""
        });
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) ThongBao
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

  [HttpPut("{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PutThongBao(string ID, web_ThongBao ThongBao)
  {
    try
    {
      if (ThongBao.ID != ID)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.ThongBaoExistsID(ThongBao.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {ThongBao.ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<web_ThongBao>(ThongBao).State = EntityState.Modified;
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
  public async Task<ActionResult<web_ThongBao>> PostThongBao(web_ThongBao ThongBao)
  {
    try
    {
      if (this.ThongBaoExistsDISPLAYNAME(ThongBao.DISPLAYNAME))
        return (ActionResult<web_ThongBao>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{ThongBao.DISPLAYNAME} trong dữ liệu!",
          Data = (object) ""
        });
      this._context.web_ThongBao.Add(ThongBao);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      return (ActionResult<web_ThongBao>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) ThongBao
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<web_ThongBao>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteThongBao(string ID)
  {
    try
    {
      web_ThongBao ThongBao = await this._context.web_ThongBao.FirstOrDefaultAsync<web_ThongBao>((Expression<Func<web_ThongBao, bool>>) (e => e.ID == ID));
      if (ThongBao == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.web_ThongBao.Remove(ThongBao);
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

  private bool ThongBaoExistsID(string ID)
  {
    return this._context.web_ThongBao.Any<web_ThongBao>((Expression<Func<web_ThongBao, bool>>) (e => e.ID == ID));
  }

  private bool ThongBaoExistsDISPLAYNAME(string DISPLAYNAME)
  {
    return this._context.web_ThongBao.Any<web_ThongBao>((Expression<Func<web_ThongBao, bool>>) (e => e.DISPLAYNAME == DISPLAYNAME));
  }
}
