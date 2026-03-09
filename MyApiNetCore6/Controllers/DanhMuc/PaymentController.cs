// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.PaymentController
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
public class PaymentController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public PaymentController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetPayment(string LOC_ID)
  {
    try
    {
      List<ct_PhieuChi> lstValue = await this._context.ct_PhieuChi.Where<ct_PhieuChi>((Expression<Func<ct_PhieuChi, bool>>) (e => e.LOC_ID == LOC_ID)).ToListAsync<ct_PhieuChi>();
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
  public async Task<IActionResult> GetPayment(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      List<ct_PhieuChi> lstValue = await this._context.ct_PhieuChi.Where<ct_PhieuChi>((Expression<Func<ct_PhieuChi, bool>>) (e => e.LOC_ID == LOC_ID)).Where<ct_PhieuChi>(KeyWhere, (object) ValuesSearch).ToListAsync<ct_PhieuChi>();
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
  public async Task<IActionResult> GetPayment(string LOC_ID, string ID)
  {
    try
    {
      ct_PhieuChi Payment = await this._context.ct_PhieuChi.FirstOrDefaultAsync<ct_PhieuChi>((Expression<Func<ct_PhieuChi, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) Payment
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
  public async Task<IActionResult> PutPayment(string LOC_ID, string ID, [FromBody] v_ct_PhieuChi Payment)
  {
    try
    {
      if (!this.PaymentExistsID(Payment.LOC_ID, Payment.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {Payment.LOC_ID}-{Payment.ID} dữ liệu!",
          Data = (object) ""
        });
      ct_PhieuChi PhieuThuCheck = await this._context.ct_PhieuChi.AsNoTracking<ct_PhieuChi>().FirstOrDefaultAsync<ct_PhieuChi>((Expression<Func<ct_PhieuChi, bool>>) (e => e.LOC_ID == Payment.LOC_ID && e.ID == Payment.ID));
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        this._context.Entry<v_ct_PhieuChi>(Payment).State = EntityState.Modified;
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        auditLog = (AuditLogController) null;
        transaction.Commit();
        using (IDbContextTransaction transaction1 = this._context.Database.BeginTransaction())
        {
          if (PhieuThuCheck != null)
          {
            if (Payment.CHUNGTUKEMTHEO != PhieuThuCheck.CHUNGTUKEMTHEO && PhieuThuCheck.CHUNGTUKEMTHEO != null && PhieuThuCheck.CHUNGTUKEMTHEO.StartsWith("PX-"))
            {
              ct_PhieuXuat PhieuXuat = await this._context.ct_PhieuXuat.FirstOrDefaultAsync<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == PhieuThuCheck.LOC_ID && e.MAPHIEU == PhieuThuCheck.CHUNGTUKEMTHEO));
              if (PhieuXuat != null)
              {
                ct_PhieuGiaoHang_ChiTiet ct_PhieuGiaoHang_ChiTiet = await this._context.ct_PhieuGiaoHang_ChiTiet.FirstOrDefaultAsync<ct_PhieuGiaoHang_ChiTiet>((Expression<Func<ct_PhieuGiaoHang_ChiTiet, bool>>) (e => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUGIAOHANG == PhieuXuat.ID));
                if (ct_PhieuGiaoHang_ChiTiet != null)
                {
                  ct_PhieuGiaoHang_ChiTiet.ID_PHIEUCHI = "";
                  this._context.Entry<ct_PhieuGiaoHang_ChiTiet>(ct_PhieuGiaoHang_ChiTiet).State = EntityState.Modified;
                }
                ct_PhieuGiaoHang_ChiTiet = (ct_PhieuGiaoHang_ChiTiet) null;
              }
            }
            if (Payment.CHUNGTUKEMTHEO != null && Payment.CHUNGTUKEMTHEO.StartsWith("PX-"))
            {
              ct_PhieuXuat PhieuXuat = await this._context.ct_PhieuXuat.FirstOrDefaultAsync<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == Payment.LOC_ID && e.MAPHIEU == Payment.CHUNGTUKEMTHEO));
              if (PhieuXuat != null)
              {
                ct_PhieuGiaoHang_ChiTiet ct_PhieuGiaoHang_ChiTiet = await this._context.ct_PhieuGiaoHang_ChiTiet.FirstOrDefaultAsync<ct_PhieuGiaoHang_ChiTiet>((Expression<Func<ct_PhieuGiaoHang_ChiTiet, bool>>) (e => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUXUAT == PhieuXuat.ID));
                if (ct_PhieuGiaoHang_ChiTiet != null)
                {
                  ct_PhieuGiaoHang_ChiTiet.ID_PHIEUCHI = Payment.MAPHIEU;
                  this._context.Entry<ct_PhieuGiaoHang_ChiTiet>(ct_PhieuGiaoHang_ChiTiet).State = EntityState.Modified;
                }
                ct_PhieuGiaoHang_ChiTiet = (ct_PhieuGiaoHang_ChiTiet) null;
              }
            }
          }
          transaction1.Commit();
          v_ct_PhieuChi PhieuChi = new v_ct_PhieuChi();
          SP_Parameter SP_Parameter = new SP_Parameter();
          SP_Parameter.ID_PHIEUCHI = Payment.ID;
          ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
          IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuChi(SP_Parameter);
          if (actionResult is OkObjectResult okResult)
          {
            ApiResponse ApiResponse = okResult.Value as ApiResponse;
            if (ApiResponse != null && ApiResponse.Data != null)
            {
              List<v_ct_PhieuChi> lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuChi>;
              if (lstPhieuNhap != null && lstPhieuNhap.Count<v_ct_PhieuChi>() > 0)
                PhieuChi = lstPhieuNhap.FirstOrDefault<v_ct_PhieuChi>() ?? new v_ct_PhieuChi();
              lstPhieuNhap = (List<v_ct_PhieuChi>) null;
            }
            ApiResponse = (ApiResponse) null;
          }
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = true,
            Message = "Success",
            Data = (object) PhieuChi
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
  public async Task<ActionResult<ct_PhieuNhap>> PostPayment([FromBody] v_ct_PhieuChi Payment)
  {
    try
    {
      if (this.PaymentExistsID(Payment.LOC_ID, Payment.ID))
        return (ActionResult<ct_PhieuNhap>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Payment.LOC_ID}-{Payment.ID} trong dữ liệu!",
          Data = (object) "",
          CheckValue = true
        });
      ct_PhieuChi objPhieuNhap = await this._context.ct_PhieuChi.FirstOrDefaultAsync<ct_PhieuChi>((Expression<Func<ct_PhieuChi, bool>>) (e => e.LOC_ID == Payment.LOC_ID && e.MAPHIEU == Payment.MAPHIEU));
      if (objPhieuNhap != null)
        return (ActionResult<ct_PhieuNhap>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Payment.LOC_ID}-{Payment.MAPHIEU} trong dữ liệu!",
          Data = (object) "",
          CheckValue = true
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        this._context.ct_PhieuChi.Add((ct_PhieuChi) Payment);
        AuditLogController auditLog1 = new AuditLogController(this._context, this._configuration);
        auditLog1.InserAuditLog();
        int num1 = await this._context.SaveChangesAsync();
        auditLog1 = (AuditLogController) null;
        transaction.Commit();
        using (IDbContextTransaction transaction1 = this._context.Database.BeginTransaction())
        {
          if (Payment.CHUNGTUKEMTHEO != null && Payment.CHUNGTUKEMTHEO.StartsWith("PX-"))
          {
            ct_PhieuXuat PhieuXuat = await this._context.ct_PhieuXuat.FirstOrDefaultAsync<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == Payment.LOC_ID && e.MAPHIEU == Payment.CHUNGTUKEMTHEO));
            if (PhieuXuat != null)
            {
              ct_PhieuGiaoHang_ChiTiet ct_PhieuGiaoHang_ChiTiet = await this._context.ct_PhieuGiaoHang_ChiTiet.FirstOrDefaultAsync<ct_PhieuGiaoHang_ChiTiet>((Expression<Func<ct_PhieuGiaoHang_ChiTiet, bool>>) (e => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUXUAT == PhieuXuat.ID));
              if (ct_PhieuGiaoHang_ChiTiet != null)
              {
                double SoTien = await this._context.ct_PhieuThu.Where<ct_PhieuThu>((Expression<Func<ct_PhieuThu, bool>>) (e => e.LOC_ID == Payment.LOC_ID && e.CHUNGTUKEMTHEO == Payment.CHUNGTUKEMTHEO)).SumAsync<ct_PhieuThu>((Expression<Func<ct_PhieuThu, double>>) (e => e.SOTIEN));
                ct_PhieuGiaoHang_ChiTiet.ID_PHIEUCHI = Payment.MAPHIEU;
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
          List<ct_PhieuChi> lstPhieuDatHangCheck = await this._context.ct_PhieuChi.Where<ct_PhieuChi>((Expression<Func<ct_PhieuChi, bool>>) (e => e.LOC_ID == Payment.LOC_ID && e.MAPHIEU == Payment.MAPHIEU)).OrderByDescending<ct_PhieuChi, DateTime>((Expression<Func<ct_PhieuChi, DateTime>>) (e => e.NGAYLAP)).ToListAsync<ct_PhieuChi>();
          if (lstPhieuDatHangCheck != null && lstPhieuDatHangCheck.Count<ct_PhieuChi>() > 1 && lstPhieuDatHangCheck.FirstOrDefault<ct_PhieuChi>().ID == Payment.ID)
          {
            int Max_ID = this._context.ct_PhieuChi.Where<ct_PhieuChi>((Expression<Func<ct_PhieuChi, bool>>) (e => e.LOC_ID == Payment.LOC_ID && e.NGAYLAP.Date == Payment.NGAYLAP.Date)).Select<ct_PhieuChi, int>((Expression<Func<ct_PhieuChi, int>>) (e => e.SOPHIEU)).DefaultIfEmpty<int>().Max<int>();
            Payment.SOPHIEU = Max_ID + 1;
            Payment.MAPHIEU = API.GetMaPhieu(nameof (Payment), Payment.NGAYLAP, Payment.SOPHIEU);
            this._context.Entry<v_ct_PhieuChi>(Payment).State = EntityState.Modified;
            int num3 = await this._context.SaveChangesAsync();
          }
          v_ct_PhieuChi PhieuChi = new v_ct_PhieuChi();
          SP_Parameter SP_Parameter = new SP_Parameter();
          SP_Parameter.ID_PHIEUCHI = Payment.ID;
          ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
          IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuChi(SP_Parameter);
          if (actionResult is OkObjectResult okResult)
          {
            ApiResponse ApiResponse = okResult.Value as ApiResponse;
            if (ApiResponse != null && ApiResponse.Data != null)
            {
              List<v_ct_PhieuChi> lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuChi>;
              if (lstPhieuNhap != null && lstPhieuNhap.Count<v_ct_PhieuChi>() > 0)
                PhieuChi = lstPhieuNhap.FirstOrDefault<v_ct_PhieuChi>() ?? new v_ct_PhieuChi();
              lstPhieuNhap = (List<v_ct_PhieuChi>) null;
            }
            ApiResponse = (ApiResponse) null;
          }
          return (ActionResult<ct_PhieuNhap>) (ActionResult) this.Ok((object) new ApiResponse()
          {
            Success = true,
            Message = "Success",
            Data = (object) PhieuChi
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
  public async Task<IActionResult> DeletePayment(string LOC_ID, string ID)
  {
    try
    {
      ct_PhieuChi Payment = await this._context.ct_PhieuChi.FirstOrDefaultAsync<ct_PhieuChi>((Expression<Func<ct_PhieuChi, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Payment == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        if (Payment.CHUNGTUKEMTHEO != null && Payment.CHUNGTUKEMTHEO.StartsWith("PX-"))
        {
          ct_PhieuXuat PhieuXuat = await this._context.ct_PhieuXuat.FirstOrDefaultAsync<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == Payment.LOC_ID && e.MAPHIEU == Payment.CHUNGTUKEMTHEO));
          if (PhieuXuat != null)
          {
            ct_PhieuGiaoHang_ChiTiet ct_PhieuGiaoHang_ChiTiet = await this._context.ct_PhieuGiaoHang_ChiTiet.FirstOrDefaultAsync<ct_PhieuGiaoHang_ChiTiet>((Expression<Func<ct_PhieuGiaoHang_ChiTiet, bool>>) (e => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUXUAT == PhieuXuat.ID));
            if (ct_PhieuGiaoHang_ChiTiet != null)
            {
              double SoTien = await this._context.ct_PhieuThu.Where<ct_PhieuThu>((Expression<Func<ct_PhieuThu, bool>>) (e => e.LOC_ID == Payment.LOC_ID && e.CHUNGTUKEMTHEO == Payment.CHUNGTUKEMTHEO)).SumAsync<ct_PhieuThu>((Expression<Func<ct_PhieuThu, double>>) (e => e.SOTIEN));
              if (ct_PhieuGiaoHang_ChiTiet.ID_PHIEUCHI == Payment.MAPHIEU)
              {
                ct_PhieuGiaoHang_ChiTiet.ID_PHIEUCHI = "";
                this._context.Entry<ct_PhieuGiaoHang_ChiTiet>(ct_PhieuGiaoHang_ChiTiet).State = EntityState.Modified;
              }
            }
            ct_PhieuGiaoHang_ChiTiet = (ct_PhieuGiaoHang_ChiTiet) null;
          }
        }
        this._context.ct_PhieuChi.Remove(Payment);
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

  private bool PaymentExistsID(string LOC_ID, string ID)
  {
    return this._context.ct_PhieuChi.Any<ct_PhieuChi>((Expression<Func<ct_PhieuChi, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }
}
