// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.PromotionController
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
using Newtonsoft.Json;
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
public class PromotionController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public PromotionController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetProduct(string LOC_ID)
  {
    try
    {
      List<view_dm_ChuongTrinhKhuyenMai> lstValue = await this._context.view_dm_ChuongTrinhKhuyenMai.Where<view_dm_ChuongTrinhKhuyenMai>((Expression<Func<view_dm_ChuongTrinhKhuyenMai, bool>>) (e => e.LOC_ID == LOC_ID)).OrderByDescending<view_dm_ChuongTrinhKhuyenMai, DateTime>((Expression<Func<view_dm_ChuongTrinhKhuyenMai, DateTime>>) (e => e.DENNGAY)).ToListAsync<view_dm_ChuongTrinhKhuyenMai>();
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
  public async Task<IActionResult> GetProduct(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<view_dm_ChuongTrinhKhuyenMai> lstValue = await this._context.view_dm_ChuongTrinhKhuyenMai.Where<view_dm_ChuongTrinhKhuyenMai>((Expression<Func<view_dm_ChuongTrinhKhuyenMai, bool>>) (e => e.LOC_ID == LOC_ID)).Where<view_dm_ChuongTrinhKhuyenMai>(KeyWhere, (object) ValuesSearch).OrderByDescending<view_dm_ChuongTrinhKhuyenMai, DateTime>((Expression<Func<view_dm_ChuongTrinhKhuyenMai, DateTime>>) (e => e.DENNGAY)).ToListAsync<view_dm_ChuongTrinhKhuyenMai>();
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
  public async Task<IActionResult> GetProduct(string LOC_ID, string ID)
  {
    try
    {
      view_dm_ChuongTrinhKhuyenMai Product = await this._context.view_dm_ChuongTrinhKhuyenMai.FirstOrDefaultAsync<view_dm_ChuongTrinhKhuyenMai>((Expression<Func<view_dm_ChuongTrinhKhuyenMai, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Product == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      v_view_dm_ChuongTrinhKhuyenMai dm_ChuongTrinhKhuyenMai = new v_view_dm_ChuongTrinhKhuyenMai();
      dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_Tang = new List<v_dm_ChuongTrinhKhuyenMai_Tang>();
      dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_YeuCau = new List<v_dm_ChuongTrinhKhuyenMai_YeuCau>();
      if (Product != null)
      {
        dm_ChuongTrinhKhuyenMai = JsonConvert.DeserializeObject<v_view_dm_ChuongTrinhKhuyenMai>(JsonConvert.SerializeObject((object) Product)) ?? new v_view_dm_ChuongTrinhKhuyenMai();
        List<view_dm_ChuongTrinhKhuyenMai_Tang> lstValue_Tang = await this._context.view_dm_ChuongTrinhKhuyenMai_Tang.Where<view_dm_ChuongTrinhKhuyenMai_Tang>((Expression<Func<view_dm_ChuongTrinhKhuyenMai_Tang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == Product.ID)).ToListAsync<view_dm_ChuongTrinhKhuyenMai_Tang>();
        dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_Tang = JsonConvert.DeserializeObject<List<v_dm_ChuongTrinhKhuyenMai_Tang>>(JsonConvert.SerializeObject((object) lstValue_Tang));
        List<view_dm_ChuongTrinhKhuyenMai_YeuCau> lstValue_yeuCau = await this._context.view_dm_ChuongTrinhKhuyenMai_YeuCau.Where<view_dm_ChuongTrinhKhuyenMai_YeuCau>((Expression<Func<view_dm_ChuongTrinhKhuyenMai_YeuCau, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == Product.ID)).ToListAsync<view_dm_ChuongTrinhKhuyenMai_YeuCau>();
        dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_YeuCau = JsonConvert.DeserializeObject<List<v_dm_ChuongTrinhKhuyenMai_YeuCau>>(JsonConvert.SerializeObject((object) lstValue_yeuCau));
        lstValue_Tang = (List<view_dm_ChuongTrinhKhuyenMai_Tang>) null;
        lstValue_yeuCau = (List<view_dm_ChuongTrinhKhuyenMai_YeuCau>) null;
      }
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) dm_ChuongTrinhKhuyenMai
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
  public async Task<IActionResult> PutProduct(
    string LOC_ID,
    string MA,
    [FromBody] v_dm_ChuongTrinhKhuyenMai ChuongTrinhKhuyenMai)
  {
    try
    {
      if (this.ProductExists((dm_ChuongTrinhKhuyenMai) ChuongTrinhKhuyenMai))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{ChuongTrinhKhuyenMai.LOC_ID}-{ChuongTrinhKhuyenMai.MA} trong dữ liệu!",
          Data = (object) ""
        });
      if (LOC_ID != ChuongTrinhKhuyenMai.LOC_ID || ChuongTrinhKhuyenMai.MA != MA)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.ProductExistsID(ChuongTrinhKhuyenMai.LOC_ID, ChuongTrinhKhuyenMai.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {ChuongTrinhKhuyenMai.LOC_ID}-{ChuongTrinhKhuyenMai.ID} dữ liệu!",
          Data = (object) ""
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        List<dm_ChuongTrinhKhuyenMai_Tang> ChuongTrinhKhuyenMai_Tang = await this._context.dm_ChuongTrinhKhuyenMai_Tang.Where<dm_ChuongTrinhKhuyenMai_Tang>((Expression<Func<dm_ChuongTrinhKhuyenMai_Tang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == ChuongTrinhKhuyenMai.ID)).ToListAsync<dm_ChuongTrinhKhuyenMai_Tang>();
        foreach (dm_ChuongTrinhKhuyenMai_Tang itm in ChuongTrinhKhuyenMai_Tang)
          this._context.dm_ChuongTrinhKhuyenMai_Tang.Remove(itm);
        List<dm_ChuongTrinhKhuyenMai_YeuCau> ChuongTrinhKhuyenMai_YeuCau = await this._context.dm_ChuongTrinhKhuyenMai_YeuCau.Where<dm_ChuongTrinhKhuyenMai_YeuCau>((Expression<Func<dm_ChuongTrinhKhuyenMai_YeuCau, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == ChuongTrinhKhuyenMai.ID)).ToListAsync<dm_ChuongTrinhKhuyenMai_YeuCau>();
        foreach (dm_ChuongTrinhKhuyenMai_YeuCau itm in ChuongTrinhKhuyenMai_YeuCau)
          this._context.dm_ChuongTrinhKhuyenMai_YeuCau.Remove(itm);
        foreach (v_dm_ChuongTrinhKhuyenMai_Tang itm in ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_Tang)
        {
          itm.ID_CHUONGTRINHKHUYENMAI = ChuongTrinhKhuyenMai.ID;
          this._context.dm_ChuongTrinhKhuyenMai_Tang.Add((dm_ChuongTrinhKhuyenMai_Tang) itm);
        }
        foreach (v_dm_ChuongTrinhKhuyenMai_YeuCau itm in ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_YeuCau)
        {
          itm.ID_CHUONGTRINHKHUYENMAI = ChuongTrinhKhuyenMai.ID;
          this._context.dm_ChuongTrinhKhuyenMai_YeuCau.Add((dm_ChuongTrinhKhuyenMai_YeuCau) itm);
        }
        this._context.Entry<v_dm_ChuongTrinhKhuyenMai>(ChuongTrinhKhuyenMai).State = EntityState.Modified;
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        ChuongTrinhKhuyenMai_Tang = (List<dm_ChuongTrinhKhuyenMai_Tang>) null;
        ChuongTrinhKhuyenMai_YeuCau = (List<dm_ChuongTrinhKhuyenMai_YeuCau>) null;
        auditLog = (AuditLogController) null;
        transaction.Commit();
        view_dm_ChuongTrinhKhuyenMai OKProduct = await this._context.view_dm_ChuongTrinhKhuyenMai.FirstOrDefaultAsync<view_dm_ChuongTrinhKhuyenMai>((Expression<Func<view_dm_ChuongTrinhKhuyenMai, bool>>) (e => e.LOC_ID == ChuongTrinhKhuyenMai.LOC_ID && e.ID == ChuongTrinhKhuyenMai.ID));
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) OKProduct
        });
      }
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
  public async Task<ActionResult<v_dm_ChuongTrinhKhuyenMai>> PostProduct(
    [FromBody] v_dm_ChuongTrinhKhuyenMai ChuongTrinhKhuyenMai)
  {
    try
    {
      if (this.ProductExistsMA(ChuongTrinhKhuyenMai.LOC_ID, ChuongTrinhKhuyenMai.MA))
        return (ActionResult<v_dm_ChuongTrinhKhuyenMai>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{ChuongTrinhKhuyenMai.LOC_ID}-{ChuongTrinhKhuyenMai.MA} trong dữ liệu!",
          Data = (object) ""
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        foreach (v_dm_ChuongTrinhKhuyenMai_Tang itm in ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_Tang)
        {
          itm.ID_CHUONGTRINHKHUYENMAI = ChuongTrinhKhuyenMai.ID;
          this._context.dm_ChuongTrinhKhuyenMai_Tang.Add((dm_ChuongTrinhKhuyenMai_Tang) itm);
        }
        foreach (v_dm_ChuongTrinhKhuyenMai_YeuCau itm in ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_YeuCau)
        {
          itm.ID_CHUONGTRINHKHUYENMAI = ChuongTrinhKhuyenMai.ID;
          this._context.dm_ChuongTrinhKhuyenMai_YeuCau.Add((dm_ChuongTrinhKhuyenMai_YeuCau) itm);
        }
        this._context.dm_ChuongTrinhKhuyenMai.Add((dm_ChuongTrinhKhuyenMai) ChuongTrinhKhuyenMai);
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        auditLog = (AuditLogController) null;
        transaction.Commit();
        view_dm_ChuongTrinhKhuyenMai OKProduct = await this._context.view_dm_ChuongTrinhKhuyenMai.FirstOrDefaultAsync<view_dm_ChuongTrinhKhuyenMai>((Expression<Func<view_dm_ChuongTrinhKhuyenMai, bool>>) (e => e.LOC_ID == ChuongTrinhKhuyenMai.LOC_ID && e.ID == ChuongTrinhKhuyenMai.ID));
        return (ActionResult<v_dm_ChuongTrinhKhuyenMai>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) OKProduct
        });
      }
    }
    catch (Exception ex)
    {
      return (ActionResult<v_dm_ChuongTrinhKhuyenMai>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteProduct(string LOC_ID, string ID)
  {
    try
    {
      dm_ChuongTrinhKhuyenMai ChuongTrinhKhuyenMai = await this._context.dm_ChuongTrinhKhuyenMai.AsNoTracking<dm_ChuongTrinhKhuyenMai>().FirstOrDefaultAsync<dm_ChuongTrinhKhuyenMai>((Expression<Func<dm_ChuongTrinhKhuyenMai, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (ChuongTrinhKhuyenMai == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_ChuongTrinhKhuyenMai>(ChuongTrinhKhuyenMai, ChuongTrinhKhuyenMai.ID, ChuongTrinhKhuyenMai.NAME);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        List<dm_ChuongTrinhKhuyenMai_Tang> ChuongTrinhKhuyenMai_Tang = await this._context.dm_ChuongTrinhKhuyenMai_Tang.Where<dm_ChuongTrinhKhuyenMai_Tang>((Expression<Func<dm_ChuongTrinhKhuyenMai_Tang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == ID)).ToListAsync<dm_ChuongTrinhKhuyenMai_Tang>();
        foreach (dm_ChuongTrinhKhuyenMai_Tang itm in ChuongTrinhKhuyenMai_Tang)
          this._context.dm_ChuongTrinhKhuyenMai_Tang.Remove(itm);
        List<dm_ChuongTrinhKhuyenMai_YeuCau> ChuongTrinhKhuyenMai_YeuCau = await this._context.dm_ChuongTrinhKhuyenMai_YeuCau.Where<dm_ChuongTrinhKhuyenMai_YeuCau>((Expression<Func<dm_ChuongTrinhKhuyenMai_YeuCau, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == ID)).ToListAsync<dm_ChuongTrinhKhuyenMai_YeuCau>();
        foreach (dm_ChuongTrinhKhuyenMai_YeuCau itm in ChuongTrinhKhuyenMai_YeuCau)
          this._context.dm_ChuongTrinhKhuyenMai_YeuCau.Remove(itm);
        this._context.dm_ChuongTrinhKhuyenMai.Remove(ChuongTrinhKhuyenMai);
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        ChuongTrinhKhuyenMai_Tang = (List<dm_ChuongTrinhKhuyenMai_Tang>) null;
        ChuongTrinhKhuyenMai_YeuCau = (List<dm_ChuongTrinhKhuyenMai_YeuCau>) null;
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

  private bool ProductExistsMA(string LOC_ID, string MA)
  {
    return this._context.dm_ChuongTrinhKhuyenMai.Any<dm_ChuongTrinhKhuyenMai>((Expression<Func<dm_ChuongTrinhKhuyenMai, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == MA));
  }

  private bool ProductExistsID(string LOC_ID, string ID)
  {
    return this._context.dm_ChuongTrinhKhuyenMai.Any<dm_ChuongTrinhKhuyenMai>((Expression<Func<dm_ChuongTrinhKhuyenMai, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }

  private bool ProductExists(dm_ChuongTrinhKhuyenMai Position)
  {
    return this._context.dm_ChuongTrinhKhuyenMai.Any<dm_ChuongTrinhKhuyenMai>((Expression<Func<dm_ChuongTrinhKhuyenMai, bool>>) (e => e.LOC_ID == Position.LOC_ID && e.MA == Position.MA && e.ID != Position.ID));
  }
}
