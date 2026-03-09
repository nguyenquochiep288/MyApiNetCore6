// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.Delivery_ImageController
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
public class Delivery_ImageController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public Delivery_ImageController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetInput(string LOC_ID, string ID)
  {
    try
    {
      ParameterExpression parameterExpression1;
      ParameterExpression parameterExpression2;
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      IQueryable<v_ct_PhieuGiaoHang_HinhAnh> Input = this._context.ct_PhieuGiaoHang_HinhAnh.Where<ct_PhieuGiaoHang_HinhAnh>((Expression<Func<ct_PhieuGiaoHang_HinhAnh, bool>>) (itm => itm.LOC_ID == LOC_ID && (itm.ID_PHIEUXUAT == ID || itm.ID_PHIEUGIAOHANG == ID))).Join((IEnumerable<AspNetUsers>) this._context.AspNetUsers, (Expression<Func<ct_PhieuGiaoHang_HinhAnh, string>>) (itm => itm.ID_NGUOITAO), (Expression<Func<AspNetUsers, string>>) (lpn => lpn.ID), (itm, lpn) => new
      {
        itm = itm,
        lpn = lpn
      }).Join((IEnumerable<ct_PhieuXuat>) this._context.ct_PhieuXuat, data => data.itm.ID_PHIEUXUAT, (Expression<Func<ct_PhieuXuat, string>>) (px => px.ID), Expression.Lambda<Func<\u003C\u003Ef__AnonymousType3<ct_PhieuGiaoHang_HinhAnh, AspNetUsers>, ct_PhieuXuat, v_ct_PhieuGiaoHang_HinhAnh>>((Expression) Expression.MemberInit(Expression.New(typeof (v_ct_PhieuGiaoHang_HinhAnh)), (MemberBinding) Expression.Bind((MethodInfo) MethodBase.GetMethodFromHandle(__methodref (ct_PhieuGiaoHang_HinhAnh.set_LOC_ID)), )))); // Unable to render the statement
      if (Input != null)
        ;
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) Input
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

  [HttpPost]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PostInput([FromBody] ct_PhieuGiaoHang_HinhAnh Input)
  {
    try
    {
      ct_PhieuGiaoHang PhieuGiaoHang = await this._context.ct_PhieuGiaoHang.FirstOrDefaultAsync<ct_PhieuGiaoHang>((Expression<Func<ct_PhieuGiaoHang, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.ID == Input.ID_PHIEUGIAOHANG));
      if (PhieuGiaoHang != null && PhieuGiaoHang.ISHOANTAT)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Phiếu giao hàng {PhieuGiaoHang.MAPHIEU} đã được hoàn tất! Nên không thể thay đổi trạng thái!",
          Data = (object) ""
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        this._context.ct_PhieuGiaoHang_HinhAnh.Add(Input);
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

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteArea(string LOC_ID, string ID)
  {
    try
    {
      ct_PhieuGiaoHang_HinhAnh Area = await this._context.ct_PhieuGiaoHang_HinhAnh.FirstOrDefaultAsync<ct_PhieuGiaoHang_HinhAnh>((Expression<Func<ct_PhieuGiaoHang_HinhAnh, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Area == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      this._context.ct_PhieuGiaoHang_HinhAnh.Remove(Area);
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
}
