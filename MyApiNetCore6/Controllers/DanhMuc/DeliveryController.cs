// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.DeliveryController
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
using System.Reflection;
using System.Threading.Tasks;


namespace MyApiNetCore6.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DeliveryController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public DeliveryController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetInput(string LOC_ID)
  {
    try
    {
      List<ct_PhieuGiaoHang> lstValue = await this._context.ct_PhieuGiaoHang.Where<ct_PhieuGiaoHang>((Expression<Func<ct_PhieuGiaoHang, bool>>) (e => e.LOC_ID == LOC_ID)).ToListAsync<ct_PhieuGiaoHang>();
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
  public async Task<IActionResult> GetInput(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      List<ct_PhieuGiaoHang> lstValue = await this._context.ct_PhieuGiaoHang.Where<ct_PhieuGiaoHang>((Expression<Func<ct_PhieuGiaoHang, bool>>) (e => e.LOC_ID == LOC_ID)).Where<ct_PhieuGiaoHang>(KeyWhere, (object) ValuesSearch).ToListAsync<ct_PhieuGiaoHang>();
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
  public async Task<IActionResult> GetInput(string LOC_ID, string ID)
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
      v_ct_PhieuGiaoHang ct_PhieuGiaoHang = new v_ct_PhieuGiaoHang();
      SP_Parameter SP_Parameter = new SP_Parameter();
      SP_Parameter.ID_PHIEUGIAOHANG = ID;
      ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
      IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang(SP_Parameter);
      if (actionResult is OkObjectResult okResult3)
      {
        if (okResult3.Value is ApiResponse ApiResponse)
        {
          if (!ApiResponse.Success || ApiResponse.Data == null)
            return (IActionResult) this.Ok((object) ApiResponse);
          List<v_ct_PhieuGiaoHang> lst = ApiResponse.Data as List<v_ct_PhieuGiaoHang>;
          if (lst != null && lst.Count > 0)
            ct_PhieuGiaoHang = lst.FirstOrDefault<v_ct_PhieuGiaoHang>() ?? new v_ct_PhieuGiaoHang();
          lst = (List<v_ct_PhieuGiaoHang>) null;
        }
        ApiResponse = (ApiResponse) null;
      }
      ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>();
      ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
      SP_Parameter = new SP_Parameter();
      SP_Parameter.ID_PHIEUGIAOHANG = ID;
      ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
      actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang_ChiTiet(SP_Parameter);
      if (actionResult is OkObjectResult okResult3)
      {
        if (okResult3.Value is ApiResponse ApiResponse)
        {
          if (!ApiResponse.Success || ApiResponse.Data == null)
            return (IActionResult) this.Ok((object) ApiResponse);
          if (ApiResponse.Data is List<v_ct_PhieuGiaoHang_ChiTiet> lst_ChiTiet)
            ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet.AddRange((IEnumerable<v_ct_PhieuGiaoHang_ChiTiet>) lst_ChiTiet);
          lst_ChiTiet = (List<v_ct_PhieuGiaoHang_ChiTiet>) null;
        }
        ApiResponse = (ApiResponse) null;
      }
      actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang_NhanVienGiao(SP_Parameter);
      if (actionResult is OkObjectResult okResult3)
      {
        if (okResult3.Value is ApiResponse ApiResponse)
        {
          if (!ApiResponse.Success || ApiResponse.Data == null)
            return (IActionResult) this.Ok((object) ApiResponse);
          if (ApiResponse.Data is List<v_ct_PhieuGiaoHang_NhanVienGiao> lst_ChiTiet)
            ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao.AddRange((IEnumerable<v_ct_PhieuGiaoHang_NhanVienGiao>) lst_ChiTiet);
          lst_ChiTiet = (List<v_ct_PhieuGiaoHang_NhanVienGiao>) null;
        }
        ApiResponse = (ApiResponse) null;
      }
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) ct_PhieuGiaoHang
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
  public async Task<IActionResult> PutInput(string LOC_ID, string ID, [FromBody] v_ct_PhieuGiaoHang Input)
  {
    try
    {
      if (!this.InputExistsID(Input.LOC_ID, Input.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {Input.LOC_ID}-{Input.ID} dữ liệu!",
          Data = (object) ""
        });
      ct_PhieuGiaoHang PhieuNhap = await this._context.ct_PhieuGiaoHang.FirstOrDefaultAsync<ct_PhieuGiaoHang>((Expression<Func<ct_PhieuGiaoHang, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.ID == Input.ID));
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        List<ct_PhieuGiaoHang_ChiTiet> lstPhieuNhap_ChiTiet = await this._context.ct_PhieuGiaoHang_ChiTiet.Where<ct_PhieuGiaoHang_ChiTiet>((Expression<Func<ct_PhieuGiaoHang_ChiTiet, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUGIAOHANG == Input.ID)).ToListAsync<ct_PhieuGiaoHang_ChiTiet>();
        if (lstPhieuNhap_ChiTiet != null)
        {
          foreach (ct_PhieuGiaoHang_ChiTiet phieuGiaoHangChiTiet in lstPhieuNhap_ChiTiet)
          {
            ct_PhieuGiaoHang_ChiTiet itm = phieuGiaoHangChiTiet;
            v_ct_PhieuGiaoHang_ChiTiet chkPhieuNhap_ChiTiet = Input.lstct_PhieuGiaoHang_ChiTiet.Where<v_ct_PhieuGiaoHang_ChiTiet>((Func<v_ct_PhieuGiaoHang_ChiTiet, bool>) (e => e.ID == itm.ID)).FirstOrDefault<v_ct_PhieuGiaoHang_ChiTiet>();
            if (chkPhieuNhap_ChiTiet != null)
            {
              if (PhieuNhap != null && PhieuNhap.ISHOANTAT != Input.ISHOANTAT && Input.ISHOANTAT)
                chkPhieuNhap_ChiTiet.ISDAGIAOHANG = true;
              chkPhieuNhap_ChiTiet.ISEDIT = true;
              chkPhieuNhap_ChiTiet.ID_PHIEUGIAOHANG = Input.ID;
              this._context.Entry<ct_PhieuGiaoHang_ChiTiet>(itm).State = EntityState.Modified;
            }
            else
              this._context.ct_PhieuGiaoHang_ChiTiet.Remove(itm);
            chkPhieuNhap_ChiTiet = (v_ct_PhieuGiaoHang_ChiTiet) null;
          }
        }
        if (Input.lstct_PhieuGiaoHang_ChiTiet != null)
        {
          foreach (v_ct_PhieuGiaoHang_ChiTiet itm in Input.lstct_PhieuGiaoHang_ChiTiet)
          {
            if (PhieuNhap != null && PhieuNhap.ISHOANTAT != Input.ISHOANTAT && Input.ISHOANTAT)
              itm.ISDAGIAOHANG = true;
            itm.ID_PHIEUGIAOHANG = Input.ID;
            if (!itm.ISEDIT)
            {
              itm.SOLAN = lstPhieuNhap_ChiTiet != null ? lstPhieuNhap_ChiTiet.Max<ct_PhieuGiaoHang_ChiTiet>((Func<ct_PhieuGiaoHang_ChiTiet, int>) (s => s.SOLAN)) + 1 : 1;
              this._context.ct_PhieuGiaoHang_ChiTiet.Add((ct_PhieuGiaoHang_ChiTiet) itm);
            }
          }
          Input.SOLUONG_DONHANG = (double) Input.lstct_PhieuGiaoHang_ChiTiet.Count<v_ct_PhieuGiaoHang_ChiTiet>();
          Input.SOTIENGIAOHANG = Input.lstct_PhieuGiaoHang_ChiTiet.Sum<v_ct_PhieuGiaoHang_ChiTiet>((Func<v_ct_PhieuGiaoHang_ChiTiet, double>) (e => e.SOTIENGIAOHANG));
        }
        List<ct_PhieuGiaoHang_NhanVienGiao> lstPhieuNhap_NhanVienGiao = await this._context.ct_PhieuGiaoHang_NhanVienGiao.Where<ct_PhieuGiaoHang_NhanVienGiao>((Expression<Func<ct_PhieuGiaoHang_NhanVienGiao, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUGIAOHANG == Input.ID)).ToListAsync<ct_PhieuGiaoHang_NhanVienGiao>();
        if (lstPhieuNhap_NhanVienGiao != null)
        {
          foreach (ct_PhieuGiaoHang_NhanVienGiao hangNhanVienGiao in lstPhieuNhap_NhanVienGiao)
          {
            ct_PhieuGiaoHang_NhanVienGiao itm = hangNhanVienGiao;
            v_ct_PhieuGiaoHang_NhanVienGiao chkPhieuNhap_NhanVienGiao = Input.lstct_PhieuGiaoHang_NhanVienGiao.Where<v_ct_PhieuGiaoHang_NhanVienGiao>((Func<v_ct_PhieuGiaoHang_NhanVienGiao, bool>) (e => e.ID == itm.ID)).FirstOrDefault<v_ct_PhieuGiaoHang_NhanVienGiao>();
            if (chkPhieuNhap_NhanVienGiao != null)
            {
              chkPhieuNhap_NhanVienGiao.ISEDIT = true;
              chkPhieuNhap_NhanVienGiao.ID_PHIEUGIAOHANG = Input.ID;
              this._context.Entry<ct_PhieuGiaoHang_NhanVienGiao>(itm).State = EntityState.Modified;
            }
            else
              this._context.ct_PhieuGiaoHang_NhanVienGiao.Remove(itm);
            chkPhieuNhap_NhanVienGiao = (v_ct_PhieuGiaoHang_NhanVienGiao) null;
          }
        }
        if (Input.lstct_PhieuGiaoHang_NhanVienGiao != null)
        {
          foreach (v_ct_PhieuGiaoHang_NhanVienGiao itm in Input.lstct_PhieuGiaoHang_NhanVienGiao)
          {
            itm.ID_PHIEUGIAOHANG = Input.ID;
            if (!itm.ISEDIT)
            {
              itm.ID = Guid.NewGuid().ToString();
              this._context.ct_PhieuGiaoHang_NhanVienGiao.Add((ct_PhieuGiaoHang_NhanVienGiao) itm);
            }
          }
        }
        if (PhieuNhap != null)
          this._context.Entry<ct_PhieuGiaoHang>(PhieuNhap).State = EntityState.Detached;
        this._context.Entry<v_ct_PhieuGiaoHang>(Input).State = EntityState.Modified;
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        lstPhieuNhap_ChiTiet = (List<ct_PhieuGiaoHang_ChiTiet>) null;
        lstPhieuNhap_NhanVienGiao = (List<ct_PhieuGiaoHang_NhanVienGiao>) null;
        auditLog = (AuditLogController) null;
        transaction.Commit();
        v_ct_PhieuGiaoHang ct_PhieuGiaoHang = new v_ct_PhieuGiaoHang();
        ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>();
        ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
        SP_Parameter SP_Parameter = new SP_Parameter();
        SP_Parameter.LOC_ID = Input.LOC_ID;
        SP_Parameter.ID_PHIEUGIAOHANG = Input.ID;
        ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
        IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang(SP_Parameter);
        if (actionResult is OkObjectResult okResult3)
        {
          if (okResult3.Value is ApiResponse ApiResponse)
          {
            if (!ApiResponse.Success || ApiResponse.Data == null)
              return (IActionResult) this.Ok((object) ApiResponse);
            List<v_ct_PhieuGiaoHang> lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuGiaoHang>;
            if (lstPhieuNhap != null && lstPhieuNhap.Count<v_ct_PhieuGiaoHang>() > 0)
              ct_PhieuGiaoHang = lstPhieuNhap.FirstOrDefault<v_ct_PhieuGiaoHang>() ?? new v_ct_PhieuGiaoHang();
            lstPhieuNhap = (List<v_ct_PhieuGiaoHang>) null;
          }
          ApiResponse = (ApiResponse) null;
        }
        ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>();
        ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
        SP_Parameter = new SP_Parameter();
        SP_Parameter.ID_PHIEUGIAOHANG = Input.ID;
        actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang_ChiTiet(SP_Parameter);
        if (actionResult is OkObjectResult okResult3)
        {
          if (okResult3.Value is ApiResponse ApiResponse)
          {
            if (!ApiResponse.Success || ApiResponse.Data == null)
              return (IActionResult) this.Ok((object) ApiResponse);
            if (ApiResponse.Data is List<v_ct_PhieuGiaoHang_ChiTiet> lst_ChiTiet)
              ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet.AddRange((IEnumerable<v_ct_PhieuGiaoHang_ChiTiet>) lst_ChiTiet);
            lst_ChiTiet = (List<v_ct_PhieuGiaoHang_ChiTiet>) null;
          }
          ApiResponse = (ApiResponse) null;
        }
        actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang_NhanVienGiao(SP_Parameter);
        if (actionResult is OkObjectResult okResult3)
        {
          if (okResult3.Value is ApiResponse ApiResponse)
          {
            if (!ApiResponse.Success || ApiResponse.Data == null)
              return (IActionResult) this.Ok((object) ApiResponse);
            if (ApiResponse.Data is List<v_ct_PhieuGiaoHang_NhanVienGiao> lst_ChiTiet)
              ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao.AddRange((IEnumerable<v_ct_PhieuGiaoHang_NhanVienGiao>) lst_ChiTiet);
            lst_ChiTiet = (List<v_ct_PhieuGiaoHang_NhanVienGiao>) null;
          }
          ApiResponse = (ApiResponse) null;
        }
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) ct_PhieuGiaoHang
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

  private static ct_PhieuGiaoHang ConvertobjectToct_PhieuGiaoHang<T>(T objectFrom)
  {
    ct_PhieuGiaoHang ctPhieuGiaoHang = new ct_PhieuGiaoHang();
    if ((object) objectFrom != null)
    {
      foreach (PropertyInfo property1 in objectFrom.GetType().GetProperties())
      {
        if (property1 != (PropertyInfo) null)
        {
          object obj = property1.GetValue((object) objectFrom);
          if (obj != null)
          {
            PropertyInfo property2 = ctPhieuGiaoHang.GetType().GetProperty(property1.Name);
            if (property2 != (PropertyInfo) null)
              property2.SetValue((object) ctPhieuGiaoHang, obj);
          }
        }
      }
    }
    return ctPhieuGiaoHang;
  }

  [HttpPost]
  [Authorize(Roles = "User")]
  public async Task<ActionResult<ct_PhieuGiaoHang>> PostInput([FromBody] v_ct_PhieuGiaoHang Input)
  {
    try
    {
      if (this.InputExistsID(Input.LOC_ID, Input.ID))
        return (ActionResult<ct_PhieuGiaoHang>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Input.LOC_ID}-{Input.ID} trong dữ liệu!",
          Data = (object) "",
          CheckValue = true
        });
      ct_PhieuGiaoHang objPhieuNhap = await this._context.ct_PhieuGiaoHang.FirstOrDefaultAsync<ct_PhieuGiaoHang>((Expression<Func<ct_PhieuGiaoHang, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.MAPHIEU));
      if (objPhieuNhap != null)
        return (ActionResult<ct_PhieuGiaoHang>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Input.LOC_ID}-{Input.MAPHIEU} trong dữ liệu!",
          Data = (object) "",
          CheckValue = true
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        if (Input.lstct_PhieuGiaoHang_ChiTiet != null)
        {
          foreach (v_ct_PhieuGiaoHang_ChiTiet phieuGiaoHangChiTiet in Input.lstct_PhieuGiaoHang_ChiTiet)
          {
            v_ct_PhieuGiaoHang_ChiTiet itm = phieuGiaoHangChiTiet;
            ct_PhieuXuat objct_PhieuXuat = await this._context.ct_PhieuXuat.FirstOrDefaultAsync<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.ID == itm.ID_PHIEUXUAT));
            if (objct_PhieuXuat != null)
            {
              ct_PhieuGiaoHang_ChiTiet objct_PhieuGiaoHang_ChiTiet = await this._context.ct_PhieuGiaoHang_ChiTiet.FirstOrDefaultAsync<ct_PhieuGiaoHang_ChiTiet>((Expression<Func<ct_PhieuGiaoHang_ChiTiet, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUXUAT == itm.ID_PHIEUXUAT));
              if (objct_PhieuGiaoHang_ChiTiet != null)
                return (ActionResult<ct_PhieuGiaoHang>) (ActionResult) this.Ok((object) new ApiResponse()
                {
                  Success = false,
                  Message = $"Phiếu xuất {objct_PhieuXuat.LOC_ID}-{objct_PhieuXuat.MAPHIEU} đã được tạo phiếu giao hàng!",
                  Data = (object) "",
                  CheckValue = true
                });
              if (objct_PhieuXuat.ISHOANTAT)
                return (ActionResult<ct_PhieuGiaoHang>) (ActionResult) this.Ok((object) new ApiResponse()
                {
                  Success = false,
                  Message = $"Đã hoàn tất {objct_PhieuXuat.LOC_ID}-{objct_PhieuXuat.MAPHIEU} trong dữ liệu!",
                  Data = (object) "",
                  CheckValue = true
                });
              itm.ID = Guid.NewGuid().ToString();
              itm.LOC_ID = Input.LOC_ID;
              itm.ID_PHIEUGIAOHANG = Input.ID;
              itm.SOLAN = 1;
              this._context.ct_PhieuGiaoHang_ChiTiet.Add((ct_PhieuGiaoHang_ChiTiet) itm);
              objct_PhieuGiaoHang_ChiTiet = (ct_PhieuGiaoHang_ChiTiet) null;
              objct_PhieuXuat = (ct_PhieuXuat) null;
            }
            else
              return (ActionResult<ct_PhieuGiaoHang>) (ActionResult) this.Ok((object) new ApiResponse()
              {
                Success = false,
                Message = $"Không tìm thấy phiếu xuất {Input.LOC_ID}-{itm.ID_PHIEUXUAT} trong dữ liệu!",
                Data = (object) "",
                CheckValue = true
              });
          }
          Input.SOLUONG_DONHANG = (double) Input.lstct_PhieuGiaoHang_ChiTiet.Count<v_ct_PhieuGiaoHang_ChiTiet>();
          Input.SOTIENGIAOHANG = Input.lstct_PhieuGiaoHang_ChiTiet.Sum<v_ct_PhieuGiaoHang_ChiTiet>((Func<v_ct_PhieuGiaoHang_ChiTiet, double>) (e => e.SOTIENGIAOHANG));
        }
        if (Input.lstct_PhieuGiaoHang_NhanVienGiao != null)
        {
          foreach (v_ct_PhieuGiaoHang_NhanVienGiao itm in Input.lstct_PhieuGiaoHang_NhanVienGiao)
          {
            itm.ID = Guid.NewGuid().ToString();
            itm.LOC_ID = Input.LOC_ID;
            itm.ID_PHIEUGIAOHANG = Input.ID;
            this._context.ct_PhieuGiaoHang_NhanVienGiao.Add((ct_PhieuGiaoHang_NhanVienGiao) itm);
          }
        }
        bool bolCheckMA = false;
        while (!bolCheckMA)
        {
          ct_PhieuGiaoHang check = this._context.ct_PhieuGiaoHang.Where<ct_PhieuGiaoHang>((Expression<Func<ct_PhieuGiaoHang, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.MAPHIEU)).FirstOrDefault<ct_PhieuGiaoHang>();
          if (check != null)
            Input.MAPHIEU = API.GetMaPhieu("Delivery", Input.NGAYLAP, Input.SOPHIEU);
          else
            bolCheckMA = true;
          check = (ct_PhieuGiaoHang) null;
        }
        this._context.ct_PhieuGiaoHang.Add((ct_PhieuGiaoHang) Input);
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num1 = await this._context.SaveChangesAsync();
        auditLog = (AuditLogController) null;
        transaction.Commit();
        List<ct_PhieuGiaoHang> lstPhieuDatHangCheck = await this._context.ct_PhieuGiaoHang.Where<ct_PhieuGiaoHang>((Expression<Func<ct_PhieuGiaoHang, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.MAPHIEU)).OrderByDescending<ct_PhieuGiaoHang, DateTime>((Expression<Func<ct_PhieuGiaoHang, DateTime>>) (e => e.NGAYLAP)).ToListAsync<ct_PhieuGiaoHang>();
        if (lstPhieuDatHangCheck != null && lstPhieuDatHangCheck.Count<ct_PhieuGiaoHang>() > 1 && lstPhieuDatHangCheck.FirstOrDefault<ct_PhieuGiaoHang>().ID == Input.ID)
        {
          int Max_ID = this._context.ct_PhieuGiaoHang.Where<ct_PhieuGiaoHang>((Expression<Func<ct_PhieuGiaoHang, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.NGAYLAP.Date == Input.NGAYLAP.Date)).Select<ct_PhieuGiaoHang, int>((Expression<Func<ct_PhieuGiaoHang, int>>) (e => e.SOPHIEU)).DefaultIfEmpty<int>().Max<int>();
          Input.SOPHIEU = Max_ID + 1;
          Input.MAPHIEU = API.GetMaPhieu("Delivery", Input.NGAYLAP, Input.SOPHIEU);
          this._context.Entry<v_ct_PhieuGiaoHang>(Input).State = EntityState.Modified;
          int num2 = await this._context.SaveChangesAsync();
        }
        v_ct_PhieuGiaoHang ct_PhieuGiaoHang = new v_ct_PhieuGiaoHang();
        ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>();
        ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
        SP_Parameter SP_Parameter = new SP_Parameter();
        SP_Parameter.LOC_ID = Input.LOC_ID;
        SP_Parameter.ID_PHIEUGIAOHANG = Input.ID;
        ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
        IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang(SP_Parameter);
        if (actionResult is OkObjectResult okResult3)
        {
          if (okResult3.Value is ApiResponse ApiResponse)
          {
            if (!ApiResponse.Success || ApiResponse.Data == null)
              return (ActionResult<ct_PhieuGiaoHang>) (ActionResult) this.Ok((object) ApiResponse);
            List<v_ct_PhieuGiaoHang> lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuGiaoHang>;
            if (lstPhieuNhap != null && lstPhieuNhap.Count<v_ct_PhieuGiaoHang>() > 0)
              ct_PhieuGiaoHang = lstPhieuNhap.FirstOrDefault<v_ct_PhieuGiaoHang>() ?? new v_ct_PhieuGiaoHang();
            lstPhieuNhap = (List<v_ct_PhieuGiaoHang>) null;
          }
          ApiResponse = (ApiResponse) null;
        }
        ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>();
        ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
        SP_Parameter = new SP_Parameter();
        SP_Parameter.ID_PHIEUGIAOHANG = Input.ID;
        actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang_ChiTiet(SP_Parameter);
        if (actionResult is OkObjectResult okResult3)
        {
          if (okResult3.Value is ApiResponse ApiResponse)
          {
            if (!ApiResponse.Success || ApiResponse.Data == null)
              return (ActionResult<ct_PhieuGiaoHang>) (ActionResult) this.Ok((object) ApiResponse);
            if (ApiResponse.Data is List<v_ct_PhieuGiaoHang_ChiTiet> lst_ChiTiet)
              ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet.AddRange((IEnumerable<v_ct_PhieuGiaoHang_ChiTiet>) lst_ChiTiet);
            lst_ChiTiet = (List<v_ct_PhieuGiaoHang_ChiTiet>) null;
          }
          ApiResponse = (ApiResponse) null;
        }
        actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang_NhanVienGiao(SP_Parameter);
        if (actionResult is OkObjectResult okResult3)
        {
          if (okResult3.Value is ApiResponse ApiResponse)
          {
            if (!ApiResponse.Success || ApiResponse.Data == null)
              return (ActionResult<ct_PhieuGiaoHang>) (ActionResult) this.Ok((object) ApiResponse);
            if (ApiResponse.Data is List<v_ct_PhieuGiaoHang_NhanVienGiao> lst_ChiTiet)
              ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao.AddRange((IEnumerable<v_ct_PhieuGiaoHang_NhanVienGiao>) lst_ChiTiet);
            lst_ChiTiet = (List<v_ct_PhieuGiaoHang_NhanVienGiao>) null;
          }
          ApiResponse = (ApiResponse) null;
        }
        return (ActionResult<ct_PhieuGiaoHang>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) ct_PhieuGiaoHang
        });
      }
    }
    catch (Exception ex)
    {
      return (ActionResult<ct_PhieuGiaoHang>) (ActionResult) this.Ok((object) new ApiResponse()
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
        List<ct_PhieuThu> lstPhieuThu = await this._context.ct_PhieuThu.Where<ct_PhieuThu>((Expression<Func<ct_PhieuThu, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.CHUNGTUKEMTHEO == Input.MAPHIEU)).ToListAsync<ct_PhieuThu>();
        if (lstPhieuThu != null && lstPhieuThu.Count<ct_PhieuThu>() > 0)
        {
          string ChungTu = $"({string.Join(";", lstPhieuThu.Select<ct_PhieuThu, string>((Func<ct_PhieuThu, string>) (e => e.MAPHIEU)))})";
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = $"Có 'Phiếu Thu {ChungTu}' liên quan tới {Input.MAPHIEU}!",
            Data = (object) ""
          });
        }
        List<ct_PhieuChi> lstPhieuChi = await this._context.ct_PhieuChi.Where<ct_PhieuChi>((Expression<Func<ct_PhieuChi, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.CHUNGTUKEMTHEO == Input.MAPHIEU)).ToListAsync<ct_PhieuChi>();
        if (lstPhieuChi != null && lstPhieuChi.Count<ct_PhieuChi>() > 0)
        {
          string ChungTu = $"({string.Join(";", lstPhieuChi.Select<ct_PhieuChi, string>((Func<ct_PhieuChi, string>) (e => e.MAPHIEU)))})";
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = $"Có 'Phiếu Chi {ChungTu}' liên quan tới {Input.MAPHIEU}!",
            Data = (object) ""
          });
        }
        List<ct_PhieuNhap> lstPhieuNhap = await this._context.ct_PhieuNhap.Where<ct_PhieuNhap>((Expression<Func<ct_PhieuNhap, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.CHUNGTUKEMTHEO == Input.MAPHIEU)).ToListAsync<ct_PhieuNhap>();
        if (lstPhieuNhap != null && lstPhieuNhap.Count<ct_PhieuNhap>() > 0)
        {
          string ChungTu = $"({string.Join(";", lstPhieuNhap.Select<ct_PhieuNhap, string>((Func<ct_PhieuNhap, string>) (e => e.MAPHIEU)))})";
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = $"Có 'Phiếu Nhập {ChungTu}' liên quan tới {Input.MAPHIEU}!",
            Data = (object) ""
          });
        }
        List<ct_PhieuXuat> lstPhieuXuat = await this._context.ct_PhieuXuat.Where<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.CHUNGTUKEMTHEO == Input.MAPHIEU)).ToListAsync<ct_PhieuXuat>();
        if (lstPhieuXuat != null && lstPhieuXuat.Count<ct_PhieuXuat>() > 0)
        {
          string ChungTu = $"({string.Join(";", lstPhieuXuat.Select<ct_PhieuXuat, string>((Func<ct_PhieuXuat, string>) (e => e.MAPHIEU)))})";
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = $"Có 'Phiếu Xuất {ChungTu}' liên quan tới {Input.MAPHIEU}!",
            Data = (object) ""
          });
        }
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
        lstPhieuThu = (List<ct_PhieuThu>) null;
        lstPhieuChi = (List<ct_PhieuChi>) null;
        lstPhieuNhap = (List<ct_PhieuNhap>) null;
        lstPhieuXuat = (List<ct_PhieuXuat>) null;
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
