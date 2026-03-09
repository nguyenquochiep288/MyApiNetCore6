// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.DebtEmployeeController
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using DatabaseTHP;
using DatabaseTHP.Class;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
public class DebtEmployeeController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public DebtEmployeeController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpPost]
  [Authorize(Roles = "User")]
  public async Task<ActionResult<List<v_ThongKeCongNo_ChiTiet>>> PostDetail(
    [FromBody] v_ThongKeCongNoNhanVien NhaCungCap)
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
      IQueryable<v_ThongKeCongNo_ChiTiet> rlsPhieuNhap = this._context.ct_PhieuNhap.Where<ct_PhieuNhap>((Expression<Func<ct_PhieuNhap, bool>>) (itm => itm.ID_NHANVIEN == NhaCungCap.ID)).Where<ct_PhieuNhap>((Expression<Func<ct_PhieuNhap, bool>>) (itm => !NhaCungCap.ISTHEOTHOIGIAN || itm.NGAYLAP.Date >= NhaCungCap.TUNGAY.Date & itm.NGAYLAP.Date <= NhaCungCap.DENNGAY.Date)).Join<ct_PhieuNhap, dm_LoaiPhieuNhap, string, v_ThongKeCongNo_ChiTiet>((IEnumerable<dm_LoaiPhieuNhap>) this._context.dm_LoaiPhieuNhap, (Expression<Func<ct_PhieuNhap, string>>) (itm => itm.ID_LOAIPHIEUNHAP), (Expression<Func<dm_LoaiPhieuNhap, string>>) (lpn => lpn.ID), Expression.Lambda<Func<ct_PhieuNhap, dm_LoaiPhieuNhap, v_ThongKeCongNo_ChiTiet>>((Expression) Expression.MemberInit(Expression.New(typeof (v_ThongKeCongNo_ChiTiet)), (MemberBinding) Expression.Bind((MethodInfo) MethodBase.GetMethodFromHandle(__methodref (v_ThongKeCongNo_ChiTiet.set_LOAIPHIEU)), )))); // Unable to render the statement
      ParameterExpression parameterExpression3;
      ParameterExpression parameterExpression4;
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      IQueryable<v_ThongKeCongNo_ChiTiet> rlsBangLuong = this._context.nv_BangLuong.Where<nv_BangLuong>((Expression<Func<nv_BangLuong, bool>>) (itm => itm.ID_NHANVIEN == NhaCungCap.ID)).Where<nv_BangLuong>((Expression<Func<nv_BangLuong, bool>>) (itm => itm.ISTINHLUONG == true)).Where<nv_BangLuong>((Expression<Func<nv_BangLuong, bool>>) (itm => !NhaCungCap.ISTHEOTHOIGIAN || itm.NGAYLAP.Date >= NhaCungCap.TUNGAY.Date & itm.NGAYLAP.Date <= NhaCungCap.DENNGAY.Date)).Join<nv_BangLuong, dm_ThangLuong, string, v_ThongKeCongNo_ChiTiet>((IEnumerable<dm_ThangLuong>) this._context.dm_ThangLuong, (Expression<Func<nv_BangLuong, string>>) (itm => itm.ID_THANGLUONG), (Expression<Func<dm_ThangLuong, string>>) (lpn => lpn.ID), Expression.Lambda<Func<nv_BangLuong, dm_ThangLuong, v_ThongKeCongNo_ChiTiet>>((Expression) Expression.MemberInit(Expression.New(typeof (v_ThongKeCongNo_ChiTiet)), (MemberBinding) Expression.Bind((MethodInfo) MethodBase.GetMethodFromHandle(__methodref (v_ThongKeCongNo_ChiTiet.set_LOAIPHIEU)), )))); // Unable to render the statement
      ParameterExpression parameterExpression5;
      ParameterExpression parameterExpression6;
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      IQueryable<v_ThongKeCongNo_ChiTiet> rlsPhieuXuat = this._context.ct_PhieuXuat.Where<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (itm => itm.ID_NHANVIEN == NhaCungCap.ID)).Where<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (itm => !NhaCungCap.ISTHEOTHOIGIAN || itm.NGAYLAP.Date >= NhaCungCap.TUNGAY.Date & itm.NGAYLAP.Date <= NhaCungCap.DENNGAY.Date)).Join<ct_PhieuXuat, dm_LoaiPhieuXuat, string, v_ThongKeCongNo_ChiTiet>((IEnumerable<dm_LoaiPhieuXuat>) this._context.dm_LoaiPhieuXuat, (Expression<Func<ct_PhieuXuat, string>>) (itm => itm.ID_LOAIPHIEUXUAT), (Expression<Func<dm_LoaiPhieuXuat, string>>) (lpn => lpn.ID), Expression.Lambda<Func<ct_PhieuXuat, dm_LoaiPhieuXuat, v_ThongKeCongNo_ChiTiet>>((Expression) Expression.MemberInit(Expression.New(typeof (v_ThongKeCongNo_ChiTiet)), (MemberBinding) Expression.Bind((MethodInfo) MethodBase.GetMethodFromHandle(__methodref (v_ThongKeCongNo_ChiTiet.set_LOAIPHIEU)), )))); // Unable to render the statement
      ParameterExpression parameterExpression7;
      ParameterExpression parameterExpression8;
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      IQueryable<v_ThongKeCongNo_ChiTiet> rlsPhieuThu = this._context.ct_PhieuThu.Where<ct_PhieuThu>((Expression<Func<ct_PhieuThu, bool>>) (itm => itm.ID_NHANVIEN == NhaCungCap.ID)).Where<ct_PhieuThu>((Expression<Func<ct_PhieuThu, bool>>) (itm => !NhaCungCap.ISTHEOTHOIGIAN || itm.NGAYLAP.Date >= NhaCungCap.TUNGAY.Date & itm.NGAYLAP.Date <= NhaCungCap.DENNGAY.Date)).Join<ct_PhieuThu, dm_LoaiPhieuThu, string, v_ThongKeCongNo_ChiTiet>((IEnumerable<dm_LoaiPhieuThu>) this._context.dm_LoaiPhieuThu, (Expression<Func<ct_PhieuThu, string>>) (itm => itm.ID_LOAIPHIEUTHU), (Expression<Func<dm_LoaiPhieuThu, string>>) (lpn => lpn.ID), Expression.Lambda<Func<ct_PhieuThu, dm_LoaiPhieuThu, v_ThongKeCongNo_ChiTiet>>((Expression) Expression.MemberInit(Expression.New(typeof (v_ThongKeCongNo_ChiTiet)), (MemberBinding) Expression.Bind((MethodInfo) MethodBase.GetMethodFromHandle(__methodref (v_ThongKeCongNo_ChiTiet.set_LOAIPHIEU)), )))); // Unable to render the statement
      ParameterExpression parameterExpression9;
      ParameterExpression parameterExpression10;
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      IQueryable<v_ThongKeCongNo_ChiTiet> rlsPhieuChi = this._context.ct_PhieuChi.Where<ct_PhieuChi>((Expression<Func<ct_PhieuChi, bool>>) (itm => itm.ID_NHANVIEN == NhaCungCap.ID)).Where<ct_PhieuChi>((Expression<Func<ct_PhieuChi, bool>>) (itm => !NhaCungCap.ISTHEOTHOIGIAN || itm.NGAYLAP.Date >= NhaCungCap.TUNGAY.Date & itm.NGAYLAP.Date <= NhaCungCap.DENNGAY.Date)).Join<ct_PhieuChi, dm_LoaiPhieuChi, string, v_ThongKeCongNo_ChiTiet>((IEnumerable<dm_LoaiPhieuChi>) this._context.dm_LoaiPhieuChi, (Expression<Func<ct_PhieuChi, string>>) (itm => itm.ID_LOAIPHIEUCHI), (Expression<Func<dm_LoaiPhieuChi, string>>) (lpn => lpn.ID), Expression.Lambda<Func<ct_PhieuChi, dm_LoaiPhieuChi, v_ThongKeCongNo_ChiTiet>>((Expression) Expression.MemberInit(Expression.New(typeof (v_ThongKeCongNo_ChiTiet)), (MemberBinding) Expression.Bind((MethodInfo) MethodBase.GetMethodFromHandle(__methodref (v_ThongKeCongNo_ChiTiet.set_LOAIPHIEU)), )))); // Unable to render the statement
      lstThongKeCongNo_ChiTiet.AddRange((IEnumerable<v_ThongKeCongNo_ChiTiet>) rlsPhieuNhap);
      lstThongKeCongNo_ChiTiet.AddRange((IEnumerable<v_ThongKeCongNo_ChiTiet>) rlsPhieuXuat);
      lstThongKeCongNo_ChiTiet.AddRange((IEnumerable<v_ThongKeCongNo_ChiTiet>) rlsPhieuChi);
      lstThongKeCongNo_ChiTiet.AddRange((IEnumerable<v_ThongKeCongNo_ChiTiet>) rlsPhieuThu);
      lstThongKeCongNo_ChiTiet.AddRange((IEnumerable<v_ThongKeCongNo_ChiTiet>) rlsBangLuong);
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
}
