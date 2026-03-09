// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.MoneyFundController
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
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;


namespace MyApiNetCore6.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MoneyFundController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public MoneyFundController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpPost]
  [Authorize(Roles = "User")]
  public async Task<ActionResult<List<v_ThongKeCongNo_ChiTiet>>> PostDetail(
    [FromBody] v_ThongKeQuyTien KhachHang)
  {
    try
    {
      List<v_ThongKeCongNo_ChiTiet> lstThongKeCongNo_ChiTiet = new List<v_ThongKeCongNo_ChiTiet>();
      ParameterExpression parameterExpression1;
      ParameterExpression parameterExpression2;
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      IQueryable<v_ThongKeCongNo_ChiTiet> rlsPhieuThu = this._context.ct_PhieuThu.Where<ct_PhieuThu>((Expression<Func<ct_PhieuThu, bool>>) (itm => itm.ID_TAIKHOANNGANHANG == KhachHang.ID_TAIKHOANNGANHANG)).Where<ct_PhieuThu>((Expression<Func<ct_PhieuThu, bool>>) (itm => !KhachHang.ISTHEOTHOIGIAN || itm.NGAYLAP.Date >= KhachHang.TUNGAY.Date & itm.NGAYLAP.Date <= KhachHang.DENNGAY.Date)).Join<ct_PhieuThu, dm_LoaiPhieuThu, string, v_ThongKeCongNo_ChiTiet>((IEnumerable<dm_LoaiPhieuThu>) this._context.dm_LoaiPhieuThu, (Expression<Func<ct_PhieuThu, string>>) (itm => itm.ID_LOAIPHIEUTHU), (Expression<Func<dm_LoaiPhieuThu, string>>) (lpn => lpn.ID), Expression.Lambda<Func<ct_PhieuThu, dm_LoaiPhieuThu, v_ThongKeCongNo_ChiTiet>>((Expression) Expression.MemberInit(Expression.New(typeof (v_ThongKeCongNo_ChiTiet)), (MemberBinding) Expression.Bind((MethodInfo) MethodBase.GetMethodFromHandle(__methodref (v_ThongKeCongNo_ChiTiet.set_LOAIPHIEU)), )))); // Unable to render the statement
      ParameterExpression parameterExpression3;
      ParameterExpression parameterExpression4;
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      IQueryable<v_ThongKeCongNo_ChiTiet> rlsPhieuChi = this._context.ct_PhieuChi.Where<ct_PhieuChi>((Expression<Func<ct_PhieuChi, bool>>) (itm => itm.ID_TAIKHOANNGANHANG == KhachHang.ID_TAIKHOANNGANHANG)).Where<ct_PhieuChi>((Expression<Func<ct_PhieuChi, bool>>) (itm => !KhachHang.ISTHEOTHOIGIAN || itm.NGAYLAP.Date >= KhachHang.TUNGAY.Date & itm.NGAYLAP.Date <= KhachHang.DENNGAY.Date)).Join<ct_PhieuChi, dm_LoaiPhieuChi, string, v_ThongKeCongNo_ChiTiet>((IEnumerable<dm_LoaiPhieuChi>) this._context.dm_LoaiPhieuChi, (Expression<Func<ct_PhieuChi, string>>) (itm => itm.ID_LOAIPHIEUCHI), (Expression<Func<dm_LoaiPhieuChi, string>>) (lpn => lpn.ID), Expression.Lambda<Func<ct_PhieuChi, dm_LoaiPhieuChi, v_ThongKeCongNo_ChiTiet>>((Expression) Expression.MemberInit(Expression.New(typeof (v_ThongKeCongNo_ChiTiet)), (MemberBinding) Expression.Bind((MethodInfo) MethodBase.GetMethodFromHandle(__methodref (v_ThongKeCongNo_ChiTiet.set_LOAIPHIEU)), )))); // Unable to render the statement
      lstThongKeCongNo_ChiTiet.AddRange((IEnumerable<v_ThongKeCongNo_ChiTiet>) rlsPhieuChi);
      lstThongKeCongNo_ChiTiet.AddRange((IEnumerable<v_ThongKeCongNo_ChiTiet>) rlsPhieuThu);
      return (ActionResult<List<v_ThongKeCongNo_ChiTiet>>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) lstThongKeCongNo_ChiTiet
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<List<v_ThongKeCongNo_ChiTiet>>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteInput(string LOC_ID, string ID)
  {
    try
    {
      ct_PhieuGiaoHang Input = await this._context.ct_PhieuGiaoHang.FirstOrDefaultAsync<ct_PhieuGiaoHang>((Expression<Func<ct_PhieuGiaoHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Input == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        List<ct_PhieuGiaoHang_ChiTiet> lstPhieuNhap_ChiTiet = await this._context.ct_PhieuGiaoHang_ChiTiet.Where<ct_PhieuGiaoHang_ChiTiet>((Expression<Func<ct_PhieuGiaoHang_ChiTiet, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUGIAOHANG == Input.ID)).ToListAsync<ct_PhieuGiaoHang_ChiTiet>();
        if (lstPhieuNhap_ChiTiet != null)
        {
          foreach (ct_PhieuGiaoHang_ChiTiet itm in lstPhieuNhap_ChiTiet)
            this._context.ct_PhieuGiaoHang_ChiTiet.Remove(itm);
        }
        List<ct_PhieuGiaoHang_NhanVienGiao> lstPhieuNhap_NhanVienGiao = await this._context.ct_PhieuGiaoHang_NhanVienGiao.Where<ct_PhieuGiaoHang_NhanVienGiao>((Expression<Func<ct_PhieuGiaoHang_NhanVienGiao, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUGIAOHANG == Input.ID)).ToListAsync<ct_PhieuGiaoHang_NhanVienGiao>();
        if (lstPhieuNhap_ChiTiet != null)
        {
          foreach (ct_PhieuGiaoHang_NhanVienGiao itm in lstPhieuNhap_NhanVienGiao)
            this._context.ct_PhieuGiaoHang_NhanVienGiao.Remove(itm);
        }
        this._context.ct_PhieuGiaoHang.Remove(Input);
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        lstPhieuNhap_ChiTiet = (List<ct_PhieuGiaoHang_ChiTiet>) null;
        lstPhieuNhap_NhanVienGiao = (List<ct_PhieuGiaoHang_NhanVienGiao>) null;
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

  private bool InputExistsID(string LOC_ID, string ID)
  {
    return this._context.ct_PhieuGiaoHang.Any<ct_PhieuGiaoHang>((Expression<Func<ct_PhieuGiaoHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }
}
