// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.ReceiptController
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.StoredProcedure.Parameter;
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
public class ReceiptController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public ReceiptController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetReceipt(string LOC_ID)
  {
    try
    {
      List<ct_PhieuThu> lstValue = await this._context.ct_PhieuThu.Where<ct_PhieuThu>((Expression<Func<ct_PhieuThu, bool>>) (e => e.LOC_ID == LOC_ID)).ToListAsync<ct_PhieuThu>();
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
  public async Task<IActionResult> GetReceipt(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      List<ct_PhieuThu> lstValue = await this._context.ct_PhieuThu.Where<ct_PhieuThu>((Expression<Func<ct_PhieuThu, bool>>) (e => e.LOC_ID == LOC_ID)).Where<ct_PhieuThu>(KeyWhere, (object) ValuesSearch).ToListAsync<ct_PhieuThu>();
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
  public async Task<IActionResult> GetReceipt(string LOC_ID, string ID)
  {
    try
    {
      ct_PhieuThu Receipt = await this._context.ct_PhieuThu.FirstOrDefaultAsync<ct_PhieuThu>((Expression<Func<ct_PhieuThu, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) Receipt
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
  public async Task<IActionResult> PutReceipt(string LOC_ID, string ID, [FromBody] v_ct_PhieuThu Receipt)
  {
    try
    {
      if (!this.ReceiptExistsID(Receipt.LOC_ID, Receipt.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {Receipt.LOC_ID}-{Receipt.ID} dữ liệu!",
          Data = (object) ""
        });
      if (Receipt.ISCHUYENCONGNOCHONHANVIEN)
      {
        dm_LoaiPhieuThu LoaiPhieuThu = await this._context.dm_LoaiPhieuThu.AsNoTracking<dm_LoaiPhieuThu>().FirstOrDefaultAsync<dm_LoaiPhieuThu>((Expression<Func<dm_LoaiPhieuThu, bool>>) (e => e.LOC_ID == Receipt.LOC_ID && e.MA == API.PTGCNKHCNV));
        if (LoaiPhieuThu != null)
          Receipt.ID_LOAIPHIEUTHU = LoaiPhieuThu.ID;
        LoaiPhieuThu = (dm_LoaiPhieuThu) null;
      }
      ct_PhieuThu PhieuThuCheck = await this._context.ct_PhieuThu.AsNoTracking<ct_PhieuThu>().FirstOrDefaultAsync<ct_PhieuThu>((Expression<Func<ct_PhieuThu, bool>>) (e => e.LOC_ID == Receipt.LOC_ID && e.ID == Receipt.ID));
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        this._context.Entry<v_ct_PhieuThu>(Receipt).State = EntityState.Modified;
        AuditLogController auditLog1 = new AuditLogController(this._context, this._configuration);
        auditLog1.InserAuditLog();
        int num1 = await this._context.SaveChangesAsync();
        auditLog1 = (AuditLogController) null;
        transaction.Commit();
        using (IDbContextTransaction transaction1 = this._context.Database.BeginTransaction())
        {
          if (PhieuThuCheck != null && PhieuThuCheck.CHUNGTUKEMTHEO != Receipt.CHUNGTUKEMTHEO)
          {
            if (PhieuThuCheck.CHUNGTUKEMTHEO != null && PhieuThuCheck.CHUNGTUKEMTHEO.StartsWith("PX-"))
            {
              ct_PhieuXuat PhieuXuat = await this._context.ct_PhieuXuat.FirstOrDefaultAsync<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == PhieuThuCheck.LOC_ID && e.MAPHIEU == PhieuThuCheck.CHUNGTUKEMTHEO));
              if (PhieuXuat != null)
              {
                ct_PhieuGiaoHang_ChiTiet ct_PhieuGiaoHang_ChiTiet = await this._context.ct_PhieuGiaoHang_ChiTiet.FirstOrDefaultAsync<ct_PhieuGiaoHang_ChiTiet>((Expression<Func<ct_PhieuGiaoHang_ChiTiet, bool>>) (e => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUGIAOHANG == PhieuXuat.ID));
                if (ct_PhieuGiaoHang_ChiTiet != null)
                {
                  double SoTien = await this._context.ct_PhieuThu.Where<ct_PhieuThu>((Expression<Func<ct_PhieuThu, bool>>) (e => e.LOC_ID == PhieuThuCheck.LOC_ID && e.CHUNGTUKEMTHEO == PhieuThuCheck.CHUNGTUKEMTHEO)).SumAsync<ct_PhieuThu>((Expression<Func<ct_PhieuThu, double>>) (e => e.SOTIEN));
                  ct_PhieuGiaoHang_ChiTiet.SOTIENDATHU = SoTien;
                  this._context.Entry<ct_PhieuGiaoHang_ChiTiet>(ct_PhieuGiaoHang_ChiTiet).State = EntityState.Modified;
                }
                ct_PhieuGiaoHang_ChiTiet = (ct_PhieuGiaoHang_ChiTiet) null;
              }
            }
            if (Receipt.CHUNGTUKEMTHEO != null && Receipt.CHUNGTUKEMTHEO.StartsWith("PX-"))
            {
              ct_PhieuXuat PhieuXuat = await this._context.ct_PhieuXuat.FirstOrDefaultAsync<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == Receipt.LOC_ID && e.MAPHIEU == Receipt.CHUNGTUKEMTHEO));
              if (PhieuXuat != null)
              {
                ct_PhieuGiaoHang_ChiTiet ct_PhieuGiaoHang_ChiTiet = await this._context.ct_PhieuGiaoHang_ChiTiet.FirstOrDefaultAsync<ct_PhieuGiaoHang_ChiTiet>((Expression<Func<ct_PhieuGiaoHang_ChiTiet, bool>>) (e => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUXUAT == PhieuXuat.ID));
                if (ct_PhieuGiaoHang_ChiTiet != null)
                {
                  double SoTien = await this._context.ct_PhieuThu.Where<ct_PhieuThu>((Expression<Func<ct_PhieuThu, bool>>) (e => e.LOC_ID == Receipt.LOC_ID && e.CHUNGTUKEMTHEO == Receipt.CHUNGTUKEMTHEO)).SumAsync<ct_PhieuThu>((Expression<Func<ct_PhieuThu, double>>) (e => e.SOTIEN));
                  ct_PhieuGiaoHang_ChiTiet.SOTIENDATHU = SoTien;
                  ct_PhieuGiaoHang_ChiTiet.ISCHUYENCONGNOCHONHANVIEN = Receipt.ISCHUYENCONGNOCHONHANVIEN;
                  this._context.Entry<ct_PhieuGiaoHang_ChiTiet>(ct_PhieuGiaoHang_ChiTiet).State = EntityState.Modified;
                }
                ct_PhieuGiaoHang_ChiTiet = (ct_PhieuGiaoHang_ChiTiet) null;
              }
            }
            AuditLogController auditLog2 = new AuditLogController(this._context, this._configuration);
            auditLog2.InserAuditLog();
            int num2 = await this._context.SaveChangesAsync();
            auditLog2 = (AuditLogController) null;
          }
          transaction1.Commit();
          v_ct_PhieuThu PhieuThu = new v_ct_PhieuThu();
          SP_Parameter SP_Parameter = new SP_Parameter();
          SP_Parameter.ID_PHIEUTHU = Receipt.ID;
          ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
          IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuThu(SP_Parameter);
          if (actionResult is OkObjectResult okResult)
          {
            ApiResponse ApiResponse = okResult.Value as ApiResponse;
            if (ApiResponse != null && ApiResponse.Data != null)
            {
              List<v_ct_PhieuThu> lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuThu>;
              if (lstPhieuNhap != null && lstPhieuNhap.Count<v_ct_PhieuThu>() > 0)
                PhieuThu = lstPhieuNhap.FirstOrDefault<v_ct_PhieuThu>() ?? new v_ct_PhieuThu();
              lstPhieuNhap = (List<v_ct_PhieuThu>) null;
            }
            ApiResponse = (ApiResponse) null;
          }
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = true,
            Message = "Success",
            Data = (object) PhieuThu
          });
        }
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
  public async Task<ActionResult<ct_PhieuNhap>> PostReceipt([FromBody] v_ct_PhieuThu Receipt)
  {
    try
    {
      if (this.ReceiptExistsID(Receipt.LOC_ID, Receipt.ID))
        return (ActionResult<ct_PhieuNhap>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Receipt.LOC_ID}-{Receipt.ID} trong dữ liệu!",
          Data = (object) "",
          CheckValue = true
        });
      if (Receipt.ISCHUYENCONGNOCHONHANVIEN)
      {
        dm_LoaiPhieuThu LoaiPhieuThu = await this._context.dm_LoaiPhieuThu.AsNoTracking<dm_LoaiPhieuThu>().FirstOrDefaultAsync<dm_LoaiPhieuThu>((Expression<Func<dm_LoaiPhieuThu, bool>>) (e => e.LOC_ID == Receipt.LOC_ID && e.MA == API.PTGCNKHCNV));
        if (LoaiPhieuThu != null)
          Receipt.ID_LOAIPHIEUTHU = LoaiPhieuThu.ID;
        LoaiPhieuThu = (dm_LoaiPhieuThu) null;
      }
      ct_PhieuThu objPhieuNhap = await this._context.ct_PhieuThu.FirstOrDefaultAsync<ct_PhieuThu>((Expression<Func<ct_PhieuThu, bool>>) (e => e.LOC_ID == Receipt.LOC_ID && e.MAPHIEU == Receipt.MAPHIEU));
      if (objPhieuNhap != null)
        return (ActionResult<ct_PhieuNhap>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Receipt.LOC_ID}-{Receipt.MAPHIEU} trong dữ liệu!",
          Data = (object) "",
          CheckValue = true
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        this._context.ct_PhieuThu.Add((ct_PhieuThu) Receipt);
        AuditLogController auditLog1 = new AuditLogController(this._context, this._configuration);
        auditLog1.InserAuditLog();
        int num1 = await this._context.SaveChangesAsync();
        auditLog1 = (AuditLogController) null;
        transaction.Commit();
        using (IDbContextTransaction transaction1 = this._context.Database.BeginTransaction())
        {
          if (Receipt.CHUNGTUKEMTHEO != null && Receipt.CHUNGTUKEMTHEO.StartsWith("PX-"))
          {
            ct_PhieuXuat PhieuXuat = await this._context.ct_PhieuXuat.FirstOrDefaultAsync<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == Receipt.LOC_ID && e.MAPHIEU == Receipt.CHUNGTUKEMTHEO));
            if (PhieuXuat != null)
            {
              ct_PhieuGiaoHang_ChiTiet ct_PhieuGiaoHang_ChiTiet = await this._context.ct_PhieuGiaoHang_ChiTiet.FirstOrDefaultAsync<ct_PhieuGiaoHang_ChiTiet>((Expression<Func<ct_PhieuGiaoHang_ChiTiet, bool>>) (e => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUXUAT == PhieuXuat.ID));
              if (ct_PhieuGiaoHang_ChiTiet != null)
              {
                double SoTien = await this._context.ct_PhieuThu.Where<ct_PhieuThu>((Expression<Func<ct_PhieuThu, bool>>) (e => e.LOC_ID == Receipt.LOC_ID && e.CHUNGTUKEMTHEO == Receipt.CHUNGTUKEMTHEO)).SumAsync<ct_PhieuThu>((Expression<Func<ct_PhieuThu, double>>) (e => e.SOTIEN));
                ct_PhieuGiaoHang_ChiTiet.SOTIENDATHU = SoTien;
                ct_PhieuGiaoHang_ChiTiet.ISCHUYENCONGNOCHONHANVIEN = Receipt.ISCHUYENCONGNOCHONHANVIEN;
                this._context.Entry<ct_PhieuGiaoHang_ChiTiet>(ct_PhieuGiaoHang_ChiTiet).State = EntityState.Modified;
              }
              ct_PhieuGiaoHang_ChiTiet = (ct_PhieuGiaoHang_ChiTiet) null;
            }
          }
          AuditLogController auditLog2 = new AuditLogController(this._context, this._configuration);
          auditLog2.InserAuditLog();
          int num2 = await this._context.SaveChangesAsync();
          auditLog2 = (AuditLogController) null;
          transaction1.Commit();
          List<ct_PhieuThu> lstPhieuDatHangCheck = await this._context.ct_PhieuThu.Where<ct_PhieuThu>((Expression<Func<ct_PhieuThu, bool>>) (e => e.LOC_ID == Receipt.LOC_ID && e.MAPHIEU == Receipt.MAPHIEU)).OrderByDescending<ct_PhieuThu, DateTime>((Expression<Func<ct_PhieuThu, DateTime>>) (e => e.NGAYLAP)).ToListAsync<ct_PhieuThu>();
          if (lstPhieuDatHangCheck != null && lstPhieuDatHangCheck.Count<ct_PhieuThu>() > 1 && lstPhieuDatHangCheck.FirstOrDefault<ct_PhieuThu>().ID == Receipt.ID)
          {
            int Max_ID = this._context.ct_PhieuThu.Where<ct_PhieuThu>((Expression<Func<ct_PhieuThu, bool>>) (e => e.LOC_ID == Receipt.LOC_ID && e.NGAYLAP.Date == Receipt.NGAYLAP.Date)).Select<ct_PhieuThu, int>((Expression<Func<ct_PhieuThu, int>>) (e => e.SOPHIEU)).DefaultIfEmpty<int>().Max<int>();
            Receipt.SOPHIEU = Max_ID + 1;
            Receipt.MAPHIEU = API.GetMaPhieu(nameof (Receipt), Receipt.NGAYLAP, Receipt.SOPHIEU);
            this._context.Entry<v_ct_PhieuThu>(Receipt).State = EntityState.Modified;
            int num3 = await this._context.SaveChangesAsync();
          }
          v_ct_PhieuThu PhieuThu = new v_ct_PhieuThu();
          SP_Parameter SP_Parameter = new SP_Parameter();
          SP_Parameter.ID_PHIEUTHU = Receipt.ID;
          ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
          IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuThu(SP_Parameter);
          if (actionResult is OkObjectResult okResult)
          {
            ApiResponse ApiResponse = okResult.Value as ApiResponse;
            if (ApiResponse != null && ApiResponse.Data != null)
            {
              List<v_ct_PhieuThu> lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuThu>;
              if (lstPhieuNhap != null && lstPhieuNhap.Count<v_ct_PhieuThu>() > 0)
                PhieuThu = lstPhieuNhap.FirstOrDefault<v_ct_PhieuThu>() ?? new v_ct_PhieuThu();
              lstPhieuNhap = (List<v_ct_PhieuThu>) null;
            }
            ApiResponse = (ApiResponse) null;
          }
          return (ActionResult<ct_PhieuNhap>) (ActionResult) this.Ok((object) new ApiResponse()
          {
            Success = true,
            Message = "Success",
            Data = (object) PhieuThu
          });
        }
      }
    }
    catch (Exception ex)
    {
      return (ActionResult<ct_PhieuNhap>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteReceipt(string LOC_ID, string ID)
  {
    try
    {
      ct_PhieuThu Receipt = await this._context.ct_PhieuThu.FirstOrDefaultAsync<ct_PhieuThu>((Expression<Func<ct_PhieuThu, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Receipt == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        if (Receipt.CHUNGTUKEMTHEO != null && Receipt.CHUNGTUKEMTHEO.StartsWith("PX-"))
        {
          ct_PhieuXuat PhieuXuat = await this._context.ct_PhieuXuat.FirstOrDefaultAsync<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == Receipt.LOC_ID && e.MAPHIEU == Receipt.CHUNGTUKEMTHEO));
          if (PhieuXuat != null)
          {
            ct_PhieuGiaoHang_ChiTiet ct_PhieuGiaoHang_ChiTiet = await this._context.ct_PhieuGiaoHang_ChiTiet.FirstOrDefaultAsync<ct_PhieuGiaoHang_ChiTiet>((Expression<Func<ct_PhieuGiaoHang_ChiTiet, bool>>) (e => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUXUAT == PhieuXuat.ID));
            if (ct_PhieuGiaoHang_ChiTiet != null)
            {
              double SoTien = await this._context.ct_PhieuThu.Where<ct_PhieuThu>((Expression<Func<ct_PhieuThu, bool>>) (e => e.LOC_ID == Receipt.LOC_ID && e.CHUNGTUKEMTHEO == Receipt.CHUNGTUKEMTHEO)).SumAsync<ct_PhieuThu>((Expression<Func<ct_PhieuThu, double>>) (e => e.SOTIEN));
              ct_PhieuGiaoHang_ChiTiet.SOTIENDATHU = SoTien - Receipt.SOTIEN;
              ct_PhieuThu ReceiptCheck = await this._context.ct_PhieuThu.FirstOrDefaultAsync<ct_PhieuThu>((Expression<Func<ct_PhieuThu, bool>>) (e => e.LOC_ID == LOC_ID && e.MAPHIEU == Receipt.CHUNGTUKEMTHEO && e.ISCHUYENCONGNOCHONHANVIEN));
              if (ReceiptCheck == null)
                ct_PhieuGiaoHang_ChiTiet.ISCHUYENCONGNOCHONHANVIEN = false;
              this._context.Entry<ct_PhieuGiaoHang_ChiTiet>(ct_PhieuGiaoHang_ChiTiet).State = EntityState.Modified;
              ReceiptCheck = (ct_PhieuThu) null;
            }
            ct_PhieuGiaoHang_ChiTiet = (ct_PhieuGiaoHang_ChiTiet) null;
          }
        }
        this._context.ct_PhieuThu.Remove(Receipt);
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
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

  private bool ReceiptExistsID(string LOC_ID, string ID)
  {
    return this._context.ct_PhieuThu.Any<ct_PhieuThu>((Expression<Func<ct_PhieuThu, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }
}
