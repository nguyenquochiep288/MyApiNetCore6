// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.AccountMisaController
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
public class AccountMisaController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public AccountMisaController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetCompany()
  {
    try
    {
      List<dm_TaiKhoan_Misa> lstValue = await this._context.dm_TaiKhoan_Misa.ToListAsync<dm_TaiKhoan_Misa>();
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
  public async Task<IActionResult> GetCompany(int Type, string KeyWhere = "", string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<dm_TaiKhoan_Misa> lstValue = await this._context.dm_TaiKhoan_Misa.Where<dm_TaiKhoan_Misa>(KeyWhere, (object) ValuesSearch).ToListAsync<dm_TaiKhoan_Misa>();
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
  public async Task<IActionResult> GetCompany(string ID)
  {
    try
    {
      dm_TaiKhoan_Misa Company = await this._context.dm_TaiKhoan_Misa.FirstOrDefaultAsync<dm_TaiKhoan_Misa>((Expression<Func<dm_TaiKhoan_Misa, bool>>) (e => e.ID == ID));
      if (Company == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {ID} dữ liệu!",
          Data = (object) ""
        });
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) Company
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

  [HttpPut("{MA}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PutCompany(string MA, dm_TaiKhoan_Misa Company)
  {
    try
    {
      if (MA != Company.MASOTHUE)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.CompanyExistsID(Company.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {Company.ID} dữ liệu!",
          Data = (object) ""
        });
      dm_TaiKhoan_Misa TaiKhoan_Misa = await this._context.dm_TaiKhoan_Misa.FirstOrDefaultAsync<dm_TaiKhoan_Misa>((Expression<Func<dm_TaiKhoan_Misa, bool>>) (e => e.ID == Company.ID));
      if (TaiKhoan_Misa != null)
      {
        if (TaiKhoan_Misa.USERNAME != Company.USERNAME || TaiKhoan_Misa.PASSWORD != Company.PASSWORD || TaiKhoan_Misa.MASOTHUE != Company.MASOTHUE)
        {
          TaiKhoan_Misa.ACCESSTOKEN = "";
          TaiKhoan_Misa.EXPIRESIN = new int?();
          TaiKhoan_Misa.USERID = "";
          TaiKhoan_Misa.ORGANIZATIONUNITID = "";
          TaiKhoan_Misa.COMPANYID = new int?();
          TaiKhoan_Misa.THOIGIANLAYTOKEN = new DateTime?();
          List<dm_LoaiHoaDon> lstValue = await this._context.dm_LoaiHoaDon.Where<dm_LoaiHoaDon>((Expression<Func<dm_LoaiHoaDon, bool>>) (e => e.LOC_ID == Company.LOC_ID)).ToListAsync<dm_LoaiHoaDon>();
          foreach (dm_LoaiHoaDon itm in lstValue)
          {
            itm.IPTEMPLATEID = "";
            itm.INVSERIES = "";
            this._context.Entry<dm_LoaiHoaDon>(itm).State = EntityState.Modified;
          }
          lstValue = (List<dm_LoaiHoaDon>) null;
        }
        TaiKhoan_Misa.USERNAME = Company.USERNAME;
        TaiKhoan_Misa.PASSWORD = Company.PASSWORD;
        TaiKhoan_Misa.MASOTHUE = Company.MASOTHUE;
        TaiKhoan_Misa.LINK = Company.LINK;
        TaiKhoan_Misa.ISACTIVE = Company.ISACTIVE;
        TaiKhoan_Misa.THOIGIANTHEM = Company.THOIGIANTHEM;
        TaiKhoan_Misa.THOIGIANSUA = Company.THOIGIANSUA;
        TaiKhoan_Misa.ID_NGUOITAO = Company.ID_NGUOITAO;
        TaiKhoan_Misa.ID_NGUOISUA = Company.ID_NGUOISUA;
        this._context.Entry<dm_TaiKhoan_Misa>(TaiKhoan_Misa).State = EntityState.Modified;
      }
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      dm_TaiKhoan_Misa OKCompany = await this._context.dm_TaiKhoan_Misa.FirstOrDefaultAsync<dm_TaiKhoan_Misa>((Expression<Func<dm_TaiKhoan_Misa, bool>>) (e => e.ID == Company.ID));
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKCompany
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
  public async Task<ActionResult<dm_TaiKhoan_Misa>> PostCompany(dm_TaiKhoan_Misa Company)
  {
    try
    {
      if (this.CompanyExistsMA(Company.MASOTHUE))
        return (ActionResult<dm_TaiKhoan_Misa>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Company.MASOTHUE} trong dữ liệu!",
          Data = (object) ""
        });
      this._context.dm_TaiKhoan_Misa.Add(Company);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      dm_TaiKhoan_Misa OKCompany = await this._context.dm_TaiKhoan_Misa.FirstOrDefaultAsync<dm_TaiKhoan_Misa>((Expression<Func<dm_TaiKhoan_Misa, bool>>) (e => e.ID == Company.ID));
      return (ActionResult<dm_TaiKhoan_Misa>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKCompany
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_TaiKhoan_Misa>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteCompany(string ID)
  {
    try
    {
      dm_TaiKhoan_Misa Company = await this._context.dm_TaiKhoan_Misa.FirstOrDefaultAsync<dm_TaiKhoan_Misa>((Expression<Func<dm_TaiKhoan_Misa, bool>>) (e => e.ID == ID));
      if (Company == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.dm_TaiKhoan_Misa.Remove(Company);
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

  private bool CompanyExistsID(string ID)
  {
    return this._context.dm_TaiKhoan_Misa.Any<dm_TaiKhoan_Misa>((Expression<Func<dm_TaiKhoan_Misa, bool>>) (e => e.ID == ID));
  }

  private bool CompanyExistsMA(string MA)
  {
    return this._context.dm_TaiKhoan_Misa.Any<dm_TaiKhoan_Misa>((Expression<Func<dm_TaiKhoan_Misa, bool>>) (e => e.MASOTHUE == MA));
  }

  private bool CompanyExists(dm_TaiKhoan_Misa Company)
  {
    return this._context.dm_TaiKhoan_Misa.Any<dm_TaiKhoan_Misa>((Expression<Func<dm_TaiKhoan_Misa, bool>>) (e => e.MASOTHUE == Company.MASOTHUE && e.ID != Company.ID));
  }
}
