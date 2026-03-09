// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.GroupProductController
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
public class GroupProductController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public GroupProductController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetGroupProduct(string LOC_ID)
  {
    try
    {
      List<dm_NhomHangHoa> lstValue = await this._context.dm_NhomHangHoa.Where<dm_NhomHangHoa>((Expression<Func<dm_NhomHangHoa, bool>>) (e => e.LOC_ID == LOC_ID)).OrderBy<dm_NhomHangHoa, string>((Expression<Func<dm_NhomHangHoa, string>>) (e => e.MA)).ToListAsync<dm_NhomHangHoa>();
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
  public async Task<IActionResult> GetGroupProduct(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<dm_NhomHangHoa> lstValue = await this._context.dm_NhomHangHoa.Where<dm_NhomHangHoa>((Expression<Func<dm_NhomHangHoa, bool>>) (e => e.LOC_ID == LOC_ID)).Where<dm_NhomHangHoa>(KeyWhere, (object) ValuesSearch).OrderBy<dm_NhomHangHoa, string>((Expression<Func<dm_NhomHangHoa, string>>) (e => e.MA)).ToListAsync<dm_NhomHangHoa>();
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
  public async Task<IActionResult> GetGroupProduct(string LOC_ID, string ID)
  {
    try
    {
      dm_NhomHangHoa GroupProduct = await this._context.dm_NhomHangHoa.FirstOrDefaultAsync<dm_NhomHangHoa>((Expression<Func<dm_NhomHangHoa, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (GroupProduct == null)
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
        Data = (object) GroupProduct
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
  public async Task<IActionResult> PutGroupProduct(
    string LOC_ID,
    string MA,
    dm_NhomHangHoa GroupProduct)
  {
    try
    {
      if (this.GroupProductExists(GroupProduct))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{GroupProduct.LOC_ID}-{GroupProduct.MA} trong dữ liệu!",
          Data = (object) ""
        });
      if (LOC_ID != GroupProduct.LOC_ID || GroupProduct.MA != MA)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.GroupProductExistsID(LOC_ID, GroupProduct.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{GroupProduct.ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.Entry<dm_NhomHangHoa>(GroupProduct).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) GroupProduct
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
  public async Task<ActionResult<dm_NhomHangHoa>> PostGroupProduct(dm_NhomHangHoa GroupProduct)
  {
    try
    {
      if (this.GroupProductExistsMA(GroupProduct.LOC_ID, GroupProduct.MA))
        return (ActionResult<dm_NhomHangHoa>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{GroupProduct.LOC_ID}-{GroupProduct.MA} trong dữ liệu!",
          Data = (object) ""
        });
      this._context.dm_NhomHangHoa.Add(GroupProduct);
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      return (ActionResult<dm_NhomHangHoa>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) GroupProduct
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_NhomHangHoa>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteGroupProduct(string LOC_ID, string ID)
  {
    try
    {
      dm_NhomHangHoa GroupProduct = await this._context.dm_NhomHangHoa.FirstOrDefaultAsync<dm_NhomHangHoa>((Expression<Func<dm_NhomHangHoa, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (GroupProduct == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_NhomHangHoa>(GroupProduct, GroupProduct.ID, GroupProduct.MA);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      List<web_PhanQuyenNhomSanPham> lstweb_PhanQuyenSanPham = await this._context.web_PhanQuyenNhomSanPham.Where<web_PhanQuyenNhomSanPham>((Expression<Func<web_PhanQuyenNhomSanPham, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_NHOMSANPHAM == ID)).ToListAsync<web_PhanQuyenNhomSanPham>();
      if (lstweb_PhanQuyenSanPham != null)
      {
        foreach (web_PhanQuyenNhomSanPham itm in lstweb_PhanQuyenSanPham)
          this._context.web_PhanQuyenNhomSanPham.Remove(itm);
      }
      this._context.dm_NhomHangHoa.Remove(GroupProduct);
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

  private bool GroupProductExistsMA(string LOC_ID, string MA)
  {
    return this._context.dm_NhomHangHoa.Any<dm_NhomHangHoa>((Expression<Func<dm_NhomHangHoa, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == MA));
  }

  private bool GroupProductExistsID(string LOC_ID, string ID)
  {
    return this._context.dm_NhomHangHoa.Any<dm_NhomHangHoa>((Expression<Func<dm_NhomHangHoa, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }

  private bool GroupProductExists(dm_NhomHangHoa GroupProduct)
  {
    return this._context.dm_NhomHangHoa.Any<dm_NhomHangHoa>((Expression<Func<dm_NhomHangHoa, bool>>) (e => e.LOC_ID == GroupProduct.LOC_ID && e.MA == GroupProduct.MA && e.ID != GroupProduct.ID));
  }
}
