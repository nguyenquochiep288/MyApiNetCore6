// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.DepotController
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
public class DepotController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public DepotController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetDepot(string LOC_ID)
  {
    try
    {
      List<dm_Kho> lstValue = await this._context.dm_Kho.Where<dm_Kho>((Expression<Func<dm_Kho, bool>>) (e => e.LOC_ID == LOC_ID)).OrderBy<dm_Kho, string>((Expression<Func<dm_Kho, string>>) (e => e.MA)).ToListAsync<dm_Kho>();
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
  public async Task<IActionResult> GetDepot(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<dm_Kho> lstValue = await this._context.dm_Kho.Where<dm_Kho>((Expression<Func<dm_Kho, bool>>) (e => e.LOC_ID == LOC_ID)).Where<dm_Kho>(KeyWhere, (object) ValuesSearch).OrderBy<dm_Kho, string>((Expression<Func<dm_Kho, string>>) (e => e.MA)).ToListAsync<dm_Kho>();
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
  public async Task<IActionResult> GetDepot(string LOC_ID, string ID)
  {
    try
    {
      dm_Kho Depot = await this._context.dm_Kho.FirstOrDefaultAsync<dm_Kho>((Expression<Func<dm_Kho, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Depot == null)
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
        Data = (object) Depot
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
  public async Task<IActionResult> PutDepot(string LOC_ID, string MA, dm_Kho Depot)
  {
    try
    {
      if (this.DepotExists(Depot))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Depot.LOC_ID}-{Depot.MA} trong dữ liệu!",
          Data = (object) ""
        });
      if (LOC_ID != Depot.LOC_ID || Depot.MA != MA)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.DepotExistsID(LOC_ID, Depot.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{Depot.ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<dm_Kho>(Depot).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      dm_TienTe OKDepot = await this._context.dm_TienTe.FirstOrDefaultAsync<dm_TienTe>((Expression<Func<dm_TienTe, bool>>) (e => e.LOC_ID == Depot.LOC_ID && e.ID == Depot.ID));
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) OKDepot
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
  public async Task<ActionResult<dm_Kho>> PostDepot(dm_Kho Depot)
  {
    try
    {
      if (this.DepotExistsMA(Depot.LOC_ID, Depot.MA))
        return (ActionResult<dm_Kho>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Depot.LOC_ID}-{Depot.MA} trong dữ liệu!",
          Data = (object) ""
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        this._context.dm_Kho.Add(Depot);
        List<dm_HangHoa> lstdm_HangHoa = await this._context.dm_HangHoa.Where<dm_HangHoa>((Expression<Func<dm_HangHoa, bool>>) (e => e.LOC_ID == Depot.LOC_ID)).ToListAsync<dm_HangHoa>();
        foreach (dm_HangHoa itm in lstdm_HangHoa)
        {
          dm_HangHoa_Kho newdm_HangHoa_Kho = new dm_HangHoa_Kho();
          newdm_HangHoa_Kho.ID = Guid.NewGuid().ToString();
          newdm_HangHoa_Kho.LOC_ID = Depot.LOC_ID;
          newdm_HangHoa_Kho.ID_KHO = Depot.ID;
          newdm_HangHoa_Kho.ID_HANGHOA = itm.ID;
          newdm_HangHoa_Kho.QTY = 0.0;
          this._context.dm_HangHoa_Kho.Add(newdm_HangHoa_Kho);
          newdm_HangHoa_Kho = (dm_HangHoa_Kho) null;
        }
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        lstdm_HangHoa = (List<dm_HangHoa>) null;
        auditLog = (AuditLogController) null;
        transaction.Commit();
        dm_TienTe OKDepot = await this._context.dm_TienTe.FirstOrDefaultAsync<dm_TienTe>((Expression<Func<dm_TienTe, bool>>) (e => e.LOC_ID == Depot.LOC_ID && e.ID == Depot.ID));
        return (ActionResult<dm_Kho>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) OKDepot
        });
      }
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_Kho>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteDepot(string LOC_ID, string ID)
  {
    try
    {
      dm_Kho Depot = await this._context.dm_Kho.FirstOrDefaultAsync<dm_Kho>((Expression<Func<dm_Kho, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Depot == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_Kho>(Depot, Depot.ID, Depot.MA);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      this._context.dm_Kho.Remove(Depot);
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

  private bool DepotExistsMA(string LOC_ID, string MA)
  {
    return this._context.dm_Kho.Any<dm_Kho>((Expression<Func<dm_Kho, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == MA));
  }

  private bool DepotExistsID(string LOC_ID, string ID)
  {
    return this._context.dm_Kho.Any<dm_Kho>((Expression<Func<dm_Kho, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }

  private bool DepotExists(dm_Kho Depot)
  {
    return this._context.dm_Kho.Any<dm_Kho>((Expression<Func<dm_Kho, bool>>) (e => e.LOC_ID == Depot.LOC_ID && e.MA == Depot.MA && e.ID != Depot.ID));
  }
}
