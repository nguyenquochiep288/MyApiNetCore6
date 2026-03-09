// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.ProductPriceRangeController
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
using System.Linq.Expressions;
using System.Threading.Tasks;

#nullable enable
namespace MyApiNetCore6.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductPriceRangeController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public ProductPriceRangeController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetProductPriceRange(string LOC_ID)
  {
    try
    {
      List<dm_HangHoa_KhungGia_Master> lstValue = await this._context.dm_HangHoa_KhungGia_Master.Where<dm_HangHoa_KhungGia_Master>((Expression<Func<dm_HangHoa_KhungGia_Master, bool>>) (e => e.LOC_ID == LOC_ID)).ToListAsync<dm_HangHoa_KhungGia_Master>();
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
  public async Task<IActionResult> GetProductPriceRange(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<dm_HangHoa_KhungGia_Master> lstValue = await this._context.dm_HangHoa_KhungGia_Master.Where<dm_HangHoa_KhungGia_Master>((Expression<Func<dm_HangHoa_KhungGia_Master, bool>>) (e => e.LOC_ID == LOC_ID && (e.MA.Contains(ValuesSearch) || e.NAME.Contains(ValuesSearch)))).OrderBy<dm_HangHoa_KhungGia_Master, string>((Expression<Func<dm_HangHoa_KhungGia_Master, string>>) (e => e.MA)).ToListAsync<dm_HangHoa_KhungGia_Master>();
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

  [HttpPost("{LOC_ID}/{ID_HANGHOA}/{ID_DVT}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PostProductPriceRange(
    string LOC_ID,
    string ID_HANGHOA,
    string ID_DVT,
    [FromBody] List<v_ChiTietHoaDon> lstChiTietHoaDon)
  {
    try
    {
      List<dm_HangHoa_KhungGia> lstValue = new List<dm_HangHoa_KhungGia>();
      view_dm_HangHoa_KhungGia_HangHoa view_dm_HangHoa_KhungGia_HangHoa = await this._context.view_dm_HangHoa_KhungGia_HangHoa.FirstOrDefaultAsync<view_dm_HangHoa_KhungGia_HangHoa>((Expression<Func<view_dm_HangHoa_KhungGia_HangHoa, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_HANGHOA == ID_HANGHOA && e.ISACTIVE));
      if (view_dm_HangHoa_KhungGia_HangHoa != null)
      {
        List<view_dm_HangHoa_KhungGia_HangHoa> lstValue_HangHoa = await this._context.view_dm_HangHoa_KhungGia_HangHoa.Where<view_dm_HangHoa_KhungGia_HangHoa>((Expression<Func<view_dm_HangHoa_KhungGia_HangHoa, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_HANGHOA_KHUNGGIA_MASTER == view_dm_HangHoa_KhungGia_HangHoa.ID_HANGHOA_KHUNGGIA_MASTER && e.ISACTIVE)).ToListAsync<view_dm_HangHoa_KhungGia_HangHoa>();
        if (lstValue_HangHoa != null)
        {
          List<v_ChiTietHoaDon> lstDonGia = lstChiTietHoaDon.Where<v_ChiTietHoaDon>((Func<v_ChiTietHoaDon, bool>) (s => lstValue_HangHoa.Select<view_dm_HangHoa_KhungGia_HangHoa, string>((Func<view_dm_HangHoa_KhungGia_HangHoa, string>) (s => s.ID_HANGHOA)).Contains<string>(s.ID_HANGHOA))).ToList<v_ChiTietHoaDon>();
          double TongSoLuong = lstChiTietHoaDon.Where<v_ChiTietHoaDon>((Func<v_ChiTietHoaDon, bool>) (s => lstValue_HangHoa.Select<view_dm_HangHoa_KhungGia_HangHoa, string>((Func<view_dm_HangHoa_KhungGia_HangHoa, string>) (s => s.ID_HANGHOA)).Contains<string>(s.ID_HANGHOA))).Sum<v_ChiTietHoaDon>((Func<v_ChiTietHoaDon, double>) (s => s.SOLUONG));
          lstValue = await this._context.dm_HangHoa_KhungGia.Where<dm_HangHoa_KhungGia>((Expression<Func<dm_HangHoa_KhungGia, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_HANGHOA_KHUNGGIA_MASTER == view_dm_HangHoa_KhungGia_HangHoa.ID_HANGHOA_KHUNGGIA_MASTER && e.ISACTIVE && e.ID_DVT == ID_DVT && e.TU <= TongSoLuong && e.DEN >= TongSoLuong)).ToListAsync<dm_HangHoa_KhungGia>();
          if (lstValue != null && lstDonGia != null)
          {
            foreach (v_ChiTietHoaDon itm in lstDonGia)
              itm.DONGIAMOI = lstValue.FirstOrDefault<dm_HangHoa_KhungGia>() != null ? lstValue.FirstOrDefault<dm_HangHoa_KhungGia>().DONGIA : itm.DONGIA;
          }
          else
            lstChiTietHoaDon.Clear();
          lstDonGia = (List<v_ChiTietHoaDon>) null;
        }
        else
          lstChiTietHoaDon.Clear();
      }
      else
        lstChiTietHoaDon.Clear();
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) lstChiTietHoaDon
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
  public async Task<IActionResult> GetProductPriceRange(string LOC_ID, string ID)
  {
    try
    {
      dm_HangHoa_KhungGia_Master ProductPriceRange = await this._context.dm_HangHoa_KhungGia_Master.FirstOrDefaultAsync<dm_HangHoa_KhungGia_Master>((Expression<Func<dm_HangHoa_KhungGia_Master, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (ProductPriceRange == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      List<dm_HangHoa_KhungGia> OKProductPriceRange = await this._context.dm_HangHoa_KhungGia.Where<dm_HangHoa_KhungGia>((Expression<Func<dm_HangHoa_KhungGia, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_HANGHOA_KHUNGGIA_MASTER == ID)).ToListAsync<dm_HangHoa_KhungGia>();
      List<view_dm_HangHoa_KhungGia_HangHoa> OKProductPriceRangeHangHoa = await this._context.view_dm_HangHoa_KhungGia_HangHoa.Where<view_dm_HangHoa_KhungGia_HangHoa>((Expression<Func<view_dm_HangHoa_KhungGia_HangHoa, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_HANGHOA_KHUNGGIA_MASTER == ID)).ToListAsync<view_dm_HangHoa_KhungGia_HangHoa>();
      v_dm_HangHoa_KhungGia_Master Mater = new v_dm_HangHoa_KhungGia_Master();
      Mater.LOC_ID = ProductPriceRange.LOC_ID;
      Mater.ID = ProductPriceRange.ID;
      Mater.MA = ProductPriceRange.MA;
      Mater.NAME = ProductPriceRange.NAME;
      Mater.ISACTIVE = ProductPriceRange.ISACTIVE;
      Mater.lstdm_HangHoa_KhungGia = new List<dm_HangHoa_KhungGia>();
      Mater.lstdm_HangHoa_KhungGia = OKProductPriceRange;
      Mater.lstdm_HangHoa_KhungGia_HangHoa = new List<v_dm_HangHoa_KhungGia_HangHoa>();
      foreach (view_dm_HangHoa_KhungGia_HangHoa itm in OKProductPriceRangeHangHoa)
      {
        v_dm_HangHoa_KhungGia_HangHoa hanghoa = new v_dm_HangHoa_KhungGia_HangHoa();
        hanghoa.ID = itm.ID;
        hanghoa.LOC_ID = itm.LOC_ID;
        hanghoa.ID_HANGHOA = itm.ID_HANGHOA;
        hanghoa.NAME = itm.NAME;
        hanghoa.MA = itm.MA;
        hanghoa.ID_HANGHOA_KHUNGGIA_MASTER = itm.ID_HANGHOA_KHUNGGIA_MASTER;
        Mater.lstdm_HangHoa_KhungGia_HangHoa.Add(hanghoa);
        hanghoa = (v_dm_HangHoa_KhungGia_HangHoa) null;
      }
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) Mater
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
  public async Task<IActionResult> PutProductPriceRange(
    string LOC_ID,
    string ID,
    [FromBody] v_dm_HangHoa_KhungGia_Master ProductPriceRange)
  {
    try
    {
      List<dm_HangHoa_KhungGia> lstProductPriceRange = await this._context.dm_HangHoa_KhungGia.Where<dm_HangHoa_KhungGia>((Expression<Func<dm_HangHoa_KhungGia, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_HANGHOA_KHUNGGIA_MASTER == ID)).ToListAsync<dm_HangHoa_KhungGia>();
      if (lstProductPriceRange == null)
        lstProductPriceRange = new List<dm_HangHoa_KhungGia>();
      foreach (dm_HangHoa_KhungGia dmHangHoaKhungGia in ProductPriceRange.lstdm_HangHoa_KhungGia)
      {
        dm_HangHoa_KhungGia itm = dmHangHoaKhungGia;
        dm_HangHoa_KhungGia objProductPriceRange = lstProductPriceRange.Where<dm_HangHoa_KhungGia>((Func<dm_HangHoa_KhungGia, bool>) (s => s.ID == itm.ID)).FirstOrDefault<dm_HangHoa_KhungGia>();
        if (objProductPriceRange != null)
        {
          itm.ID = objProductPriceRange.ID;
          this._context.Entry<dm_HangHoa_KhungGia>(objProductPriceRange).CurrentValues.SetValues((object) itm);
          lstProductPriceRange.Remove(objProductPriceRange);
        }
        else
        {
          dm_HangHoa_KhungGia newdm_HangHoa_KhungGia = new dm_HangHoa_KhungGia()
          {
            LOC_ID = itm.LOC_ID,
            ID = itm.ID,
            TU = itm.TU,
            DEN = itm.DEN,
            ID_DVT = itm.ID_DVT,
            DONGIA = itm.DONGIA,
            ISACTIVE = itm.ISACTIVE,
            TIEN_KPI = itm.TIEN_KPI,
            HINHTHUC_TINHKPI = itm.HINHTHUC_TINHKPI,
            CK_KPI = itm.CK_KPI
          };
          newdm_HangHoa_KhungGia.TIEN_KPI = itm.TIEN_KPI;
          this._context.dm_HangHoa_KhungGia.Add(newdm_HangHoa_KhungGia);
          newdm_HangHoa_KhungGia = (dm_HangHoa_KhungGia) null;
          objProductPriceRange = (dm_HangHoa_KhungGia) null;
        }
      }
      foreach (dm_HangHoa_KhungGia itm in lstProductPriceRange)
        this._context.dm_HangHoa_KhungGia.Remove(itm);
      List<dm_HangHoa_KhungGia_HangHoa> lstProductPriceRangeHangHoa = await this._context.dm_HangHoa_KhungGia_HangHoa.Where<dm_HangHoa_KhungGia_HangHoa>((Expression<Func<dm_HangHoa_KhungGia_HangHoa, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_HANGHOA_KHUNGGIA_MASTER == ID)).ToListAsync<dm_HangHoa_KhungGia_HangHoa>();
      if (lstProductPriceRangeHangHoa == null)
        lstProductPriceRangeHangHoa = new List<dm_HangHoa_KhungGia_HangHoa>();
      foreach (v_dm_HangHoa_KhungGia_HangHoa hoaKhungGiaHangHoa in ProductPriceRange.lstdm_HangHoa_KhungGia_HangHoa)
      {
        v_dm_HangHoa_KhungGia_HangHoa itm = hoaKhungGiaHangHoa;
        dm_HangHoa_KhungGia_HangHoa objProductPriceRange = lstProductPriceRangeHangHoa.Where<dm_HangHoa_KhungGia_HangHoa>((Func<dm_HangHoa_KhungGia_HangHoa, bool>) (s => s.ID == itm.ID)).FirstOrDefault<dm_HangHoa_KhungGia_HangHoa>();
        if (objProductPriceRange != null)
        {
          itm.ID = objProductPriceRange.ID;
          this._context.Entry<dm_HangHoa_KhungGia_HangHoa>(objProductPriceRange).CurrentValues.SetValues((object) itm);
          lstProductPriceRangeHangHoa.Remove(objProductPriceRange);
        }
        else
        {
          dm_HangHoa_KhungGia_HangHoa newdm_HangHoa_KhungGia = new dm_HangHoa_KhungGia_HangHoa();
          newdm_HangHoa_KhungGia.LOC_ID = itm.LOC_ID;
          newdm_HangHoa_KhungGia.ID = itm.ID;
          newdm_HangHoa_KhungGia.ID_HANGHOA = itm.ID_HANGHOA;
          newdm_HangHoa_KhungGia.ID_HANGHOA_KHUNGGIA_MASTER = ID;
          this._context.dm_HangHoa_KhungGia_HangHoa.Add(newdm_HangHoa_KhungGia);
          newdm_HangHoa_KhungGia = (dm_HangHoa_KhungGia_HangHoa) null;
          objProductPriceRange = (dm_HangHoa_KhungGia_HangHoa) null;
        }
      }
      foreach (dm_HangHoa_KhungGia_HangHoa itm in lstProductPriceRangeHangHoa)
        this._context.dm_HangHoa_KhungGia_HangHoa.Remove(itm);
      this._context.Entry<v_dm_HangHoa_KhungGia_Master>(ProductPriceRange).State = EntityState.Modified;
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      List<dm_HangHoa_KhungGia> OKProductPriceRange = await this._context.dm_HangHoa_KhungGia.Where<dm_HangHoa_KhungGia>((Expression<Func<dm_HangHoa_KhungGia, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_HANGHOA_KHUNGGIA_MASTER == ID)).ToListAsync<dm_HangHoa_KhungGia>();
      List<view_dm_HangHoa_KhungGia_HangHoa> OKProductPriceRangeHangHoa = await this._context.view_dm_HangHoa_KhungGia_HangHoa.Where<view_dm_HangHoa_KhungGia_HangHoa>((Expression<Func<view_dm_HangHoa_KhungGia_HangHoa, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_HANGHOA_KHUNGGIA_MASTER == ID)).ToListAsync<view_dm_HangHoa_KhungGia_HangHoa>();
      v_dm_HangHoa_KhungGia_Master Mater = new v_dm_HangHoa_KhungGia_Master();
      Mater.LOC_ID = ProductPriceRange.LOC_ID;
      Mater.ID = ProductPriceRange.ID;
      Mater.MA = ProductPriceRange.MA;
      Mater.NAME = ProductPriceRange.NAME;
      Mater.ISACTIVE = ProductPriceRange.ISACTIVE;
      Mater.lstdm_HangHoa_KhungGia = new List<dm_HangHoa_KhungGia>();
      Mater.lstdm_HangHoa_KhungGia = OKProductPriceRange;
      Mater.lstdm_HangHoa_KhungGia_HangHoa = new List<v_dm_HangHoa_KhungGia_HangHoa>();
      foreach (view_dm_HangHoa_KhungGia_HangHoa itm in OKProductPriceRangeHangHoa)
      {
        v_dm_HangHoa_KhungGia_HangHoa hanghoa = new v_dm_HangHoa_KhungGia_HangHoa();
        hanghoa.ID = itm.ID;
        hanghoa.LOC_ID = itm.LOC_ID;
        hanghoa.ID_HANGHOA = itm.ID_HANGHOA;
        hanghoa.NAME = itm.NAME;
        hanghoa.MA = itm.MA;
        hanghoa.ID_HANGHOA_KHUNGGIA_MASTER = itm.ID_HANGHOA_KHUNGGIA_MASTER;
        Mater.lstdm_HangHoa_KhungGia_HangHoa.Add(hanghoa);
        hanghoa = (v_dm_HangHoa_KhungGia_HangHoa) null;
      }
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) Mater
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
  public async Task<ActionResult<dm_KhuVuc>> PostProductPriceRange(
    [FromBody] v_dm_HangHoa_KhungGia_Master ProductPriceRange)
  {
    try
    {
      if (this.ProductPriceRangeExistsMA(ProductPriceRange.LOC_ID, ProductPriceRange.MA))
        return (ActionResult<dm_KhuVuc>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{ProductPriceRange.LOC_ID}-{ProductPriceRange.MA} trong dữ liệu!",
          Data = (object) ""
        });
      this._context.dm_HangHoa_KhungGia_Master.Add((dm_HangHoa_KhungGia_Master) ProductPriceRange);
      foreach (dm_HangHoa_KhungGia itm in ProductPriceRange.lstdm_HangHoa_KhungGia)
      {
        itm.ID_HANGHOA_KHUNGGIA_MASTER = ProductPriceRange.ID;
        this._context.dm_HangHoa_KhungGia.Add(itm);
      }
      foreach (v_dm_HangHoa_KhungGia_HangHoa itm in ProductPriceRange.lstdm_HangHoa_KhungGia_HangHoa)
      {
        itm.ID_HANGHOA_KHUNGGIA_MASTER = ProductPriceRange.ID;
        this._context.dm_HangHoa_KhungGia_HangHoa.Add((dm_HangHoa_KhungGia_HangHoa) itm);
      }
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.InserAuditLog();
      int num = await this._context.SaveChangesAsync();
      List<dm_HangHoa_KhungGia> OKProductPriceRange = await this._context.dm_HangHoa_KhungGia.Where<dm_HangHoa_KhungGia>((Expression<Func<dm_HangHoa_KhungGia, bool>>) (e => e.LOC_ID == ProductPriceRange.LOC_ID && e.ID_HANGHOA_KHUNGGIA_MASTER == ProductPriceRange.ID)).ToListAsync<dm_HangHoa_KhungGia>();
      List<view_dm_HangHoa_KhungGia_HangHoa> OKProductPriceRangeHangHoa = await this._context.view_dm_HangHoa_KhungGia_HangHoa.Where<view_dm_HangHoa_KhungGia_HangHoa>((Expression<Func<view_dm_HangHoa_KhungGia_HangHoa, bool>>) (e => e.LOC_ID == ProductPriceRange.LOC_ID && e.ID_HANGHOA_KHUNGGIA_MASTER == ProductPriceRange.ID)).ToListAsync<view_dm_HangHoa_KhungGia_HangHoa>();
      v_dm_HangHoa_KhungGia_Master Mater = new v_dm_HangHoa_KhungGia_Master();
      Mater.LOC_ID = ProductPriceRange.LOC_ID;
      Mater.ID = ProductPriceRange.ID;
      Mater.MA = ProductPriceRange.MA;
      Mater.NAME = ProductPriceRange.NAME;
      Mater.ISACTIVE = ProductPriceRange.ISACTIVE;
      Mater.lstdm_HangHoa_KhungGia = new List<dm_HangHoa_KhungGia>();
      Mater.lstdm_HangHoa_KhungGia = OKProductPriceRange;
      Mater.lstdm_HangHoa_KhungGia_HangHoa = new List<v_dm_HangHoa_KhungGia_HangHoa>();
      foreach (view_dm_HangHoa_KhungGia_HangHoa itm in OKProductPriceRangeHangHoa)
      {
        v_dm_HangHoa_KhungGia_HangHoa hanghoa = new v_dm_HangHoa_KhungGia_HangHoa();
        hanghoa.ID = itm.ID;
        hanghoa.LOC_ID = itm.LOC_ID;
        hanghoa.ID_HANGHOA = itm.ID_HANGHOA;
        hanghoa.NAME = itm.NAME;
        hanghoa.MA = itm.MA;
        hanghoa.ID_HANGHOA_KHUNGGIA_MASTER = itm.ID_HANGHOA_KHUNGGIA_MASTER;
        Mater.lstdm_HangHoa_KhungGia_HangHoa.Add(hanghoa);
        hanghoa = (v_dm_HangHoa_KhungGia_HangHoa) null;
      }
      return (ActionResult<dm_KhuVuc>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) Mater
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<dm_KhuVuc>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteProductPriceRange(string LOC_ID, string ID)
  {
    try
    {
      dm_HangHoa_KhungGia_Master ProductPriceRange = await this._context.dm_HangHoa_KhungGia_Master.FirstOrDefaultAsync<dm_HangHoa_KhungGia_Master>((Expression<Func<dm_HangHoa_KhungGia_Master, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (ProductPriceRange == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      List<dm_HangHoa_KhungGia> OKProductPriceRange = await this._context.dm_HangHoa_KhungGia.Where<dm_HangHoa_KhungGia>((Expression<Func<dm_HangHoa_KhungGia, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_HANGHOA_KHUNGGIA_MASTER == ID)).ToListAsync<dm_HangHoa_KhungGia>();
      List<dm_HangHoa_KhungGia_HangHoa> OKProductPriceRangeHangHoa = await this._context.dm_HangHoa_KhungGia_HangHoa.Where<dm_HangHoa_KhungGia_HangHoa>((Expression<Func<dm_HangHoa_KhungGia_HangHoa, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_HANGHOA_KHUNGGIA_MASTER == ID)).ToListAsync<dm_HangHoa_KhungGia_HangHoa>();
      foreach (dm_HangHoa_KhungGia itm in OKProductPriceRange)
        this._context.dm_HangHoa_KhungGia.Remove(itm);
      foreach (dm_HangHoa_KhungGia_HangHoa itm in OKProductPriceRangeHangHoa)
        this._context.dm_HangHoa_KhungGia_HangHoa.Remove(itm);
      this._context.dm_HangHoa_KhungGia_Master.Remove(ProductPriceRange);
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

  private bool ProductPriceRangeExistsMA(string LOC_ID, string MA)
  {
    return this._context.dm_HangHoa_KhungGia_Master.Any<dm_HangHoa_KhungGia_Master>((Expression<Func<dm_HangHoa_KhungGia_Master, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == MA));
  }

  private bool ProductPriceRangeExistsID(string LOC_ID, string ID)
  {
    return this._context.dm_HangHoa_KhungGia_Master.Any<dm_HangHoa_KhungGia_Master>((Expression<Func<dm_HangHoa_KhungGia_Master, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }
}
