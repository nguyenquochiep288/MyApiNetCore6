// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.Order_ProviderController
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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace MyApiNetCore6.Controllers;

[Route("api/[controller]")]
[ApiController]
public class Order_ProviderController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public Order_ProviderController(dbTrangHiepPhatContext context, IConfiguration configuration)
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
      List<ct_PhieuDatHangNCC> lstValue = await this._context.ct_PhieuDatHangNCC.Where<ct_PhieuDatHangNCC>((Expression<Func<ct_PhieuDatHangNCC, bool>>) (e => e.LOC_ID == LOC_ID)).ToListAsync<ct_PhieuDatHangNCC>();
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
      List<ct_PhieuDatHangNCC> lstValue = await this._context.ct_PhieuDatHangNCC.Where<ct_PhieuDatHangNCC>((Expression<Func<ct_PhieuDatHangNCC, bool>>) (e => e.LOC_ID == LOC_ID)).Where<ct_PhieuDatHangNCC>(KeyWhere, (object) ValuesSearch).ToListAsync<ct_PhieuDatHangNCC>();
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
      ct_PhieuDatHangNCC Input = await this._context.ct_PhieuDatHangNCC.FirstOrDefaultAsync<ct_PhieuDatHangNCC>((Expression<Func<ct_PhieuDatHangNCC, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Input == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC = new v_ct_PhieuDatHangNCC();
      if (Input != null)
      {
        string strInput = JsonConvert.SerializeObject((object) Input);
        ct_PhieuDatHangNCC = JsonConvert.DeserializeObject<v_ct_PhieuDatHangNCC>(strInput) ?? new v_ct_PhieuDatHangNCC();
        strInput = (string) null;
      }
      ct_PhieuDatHangNCC.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuDatHangNCC_ChiTiet>();
      SP_Parameter SP_Parameter = new SP_Parameter();
      SP_Parameter.ID_PHIEUNHAP = ID;
      ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
      IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuDatHangNCC_Chitiet(SP_Parameter);
      if (actionResult is OkObjectResult okResult)
      {
        ApiResponse ApiResponse = okResult.Value as ApiResponse;
        if (ApiResponse != null && ApiResponse.Data != null)
        {
          if (ApiResponse.Data is List<v_ct_PhieuDatHangNCC_ChiTiet> lst_ChiTiet)
            ct_PhieuDatHangNCC.lstct_PhieuNhap_ChiTiet.AddRange((IEnumerable<v_ct_PhieuDatHangNCC_ChiTiet>) lst_ChiTiet);
          lst_ChiTiet = (List<v_ct_PhieuDatHangNCC_ChiTiet>) null;
        }
        ApiResponse = (ApiResponse) null;
      }
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) ct_PhieuDatHangNCC
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
  public async Task<IActionResult> PutInput(string LOC_ID, string ID, [FromBody] v_ct_PhieuDatHangNCC Input)
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
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        List<ct_PhieuDatHangNCC_ChiTiet> lstPhieuNhap_ChiTiet = await this._context.ct_PhieuDatHangNCC_ChiTiet.Where<ct_PhieuDatHangNCC_ChiTiet>((Expression<Func<ct_PhieuDatHangNCC_ChiTiet, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUDATHANGNCC == Input.ID)).ToListAsync<ct_PhieuDatHangNCC_ChiTiet>();
        if (lstPhieuNhap_ChiTiet != null)
        {
          foreach (ct_PhieuDatHangNCC_ChiTiet datHangNccChiTiet in lstPhieuNhap_ChiTiet)
          {
            ct_PhieuDatHangNCC_ChiTiet itm = datHangNccChiTiet;
            dm_HangHoa_Kho objdm_HangHoa_Kho = await this._context.dm_HangHoa_Kho.FirstOrDefaultAsync<dm_HangHoa_Kho>((Expression<Func<dm_HangHoa_Kho, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO));
            if (objdm_HangHoa_Kho != null)
            {
              itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
              v_ct_PhieuDatHangNCC_ChiTiet chkPhieuNhap_ChiTiet = Input.lstct_PhieuNhap_ChiTiet.Where<v_ct_PhieuDatHangNCC_ChiTiet>((Func<v_ct_PhieuDatHangNCC_ChiTiet, bool>) (e => e.ID == itm.ID)).FirstOrDefault<v_ct_PhieuDatHangNCC_ChiTiet>();
              if (chkPhieuNhap_ChiTiet != null)
              {
                chkPhieuNhap_ChiTiet.ISEDIT = true;
                chkPhieuNhap_ChiTiet.ID_PHIEUDATHANGNCC = Input.ID;
                this._context.Entry<dm_HangHoa_Kho>(objdm_HangHoa_Kho).State = EntityState.Modified;
              }
              else
                this._context.ct_PhieuDatHangNCC_ChiTiet.Remove(itm);
              objdm_HangHoa_Kho = (dm_HangHoa_Kho) null;
              chkPhieuNhap_ChiTiet = (v_ct_PhieuDatHangNCC_ChiTiet) null;
            }
            else
              return (IActionResult) this.Ok((object) new ApiResponse()
              {
                Success = false,
                Message = ("Không tìm thấy sản phẩm kho!" + itm.ID_HANGHOAKHO),
                Data = (object) ""
              });
          }
        }
        if (Input.lstct_PhieuNhap_ChiTiet != null)
        {
          foreach (v_ct_PhieuDatHangNCC_ChiTiet datHangNccChiTiet in Input.lstct_PhieuNhap_ChiTiet)
          {
            v_ct_PhieuDatHangNCC_ChiTiet itm = datHangNccChiTiet;
            itm.ID_PHIEUDATHANGNCC = Input.ID;
            dm_HangHoa_Kho objdm_HangHoa_Kho = this._context.dm_HangHoa_Kho.FirstOrDefault<dm_HangHoa_Kho>((Expression<Func<dm_HangHoa_Kho, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO));
            if (objdm_HangHoa_Kho != null)
            {
              itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
              if (!itm.ISEDIT)
                this._context.ct_PhieuDatHangNCC_ChiTiet.Add((ct_PhieuDatHangNCC_ChiTiet) itm);
              objdm_HangHoa_Kho = (dm_HangHoa_Kho) null;
            }
            else
              return (IActionResult) this.Ok((object) new ApiResponse()
              {
                Success = false,
                Message = ("Không tìm thấy sản phẩm kho!" + itm.ID_HANGHOAKHO),
                Data = (object) ""
              });
          }
        }
        this._context.Entry<v_ct_PhieuDatHangNCC>(Input).State = EntityState.Modified;
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        lstPhieuNhap_ChiTiet = (List<ct_PhieuDatHangNCC_ChiTiet>) null;
        auditLog = (AuditLogController) null;
        transaction.Commit();
        v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC = new v_ct_PhieuDatHangNCC();
        ct_PhieuDatHangNCC.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuDatHangNCC_ChiTiet>();
        SP_Parameter SP_Parameter = new SP_Parameter();
        SP_Parameter.ID_PHIEUNHAP = Input.ID;
        ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
        IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuDatHangNCC(SP_Parameter);
        if (actionResult is OkObjectResult okResult)
        {
          ApiResponse ApiResponse = okResult.Value as ApiResponse;
          if (ApiResponse != null && ApiResponse.Data != null)
          {
            List<v_ct_PhieuDatHangNCC> lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuDatHangNCC>;
            if (lstPhieuNhap != null && lstPhieuNhap.Count<v_ct_PhieuDatHangNCC>() > 0)
              ct_PhieuDatHangNCC = lstPhieuNhap.FirstOrDefault<v_ct_PhieuDatHangNCC>() ?? new v_ct_PhieuDatHangNCC();
            lstPhieuNhap = (List<v_ct_PhieuDatHangNCC>) null;
          }
          ApiResponse = (ApiResponse) null;
        }
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) ct_PhieuDatHangNCC
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

  [HttpPut("{LOC_ID}/{ID}/{TRANGTHAI}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PutInput(string LOC_ID, string ID, string TRANGTHAI)
  {
    try
    {
      if (!this.InputExistsID(LOC_ID, ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        ct_PhieuDatHangNCC objct_PhieuDatHangNCC = await this._context.ct_PhieuDatHangNCC.FirstOrDefaultAsync<ct_PhieuDatHangNCC>((Expression<Func<ct_PhieuDatHangNCC, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
        if (objct_PhieuDatHangNCC != null)
        {
          objct_PhieuDatHangNCC.ISHOANTAT = TRANGTHAI == "1";
          this._context.Entry<ct_PhieuDatHangNCC>(objct_PhieuDatHangNCC).State = EntityState.Modified;
          AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
          auditLog.InserAuditLog();
          int num = await this._context.SaveChangesAsync();
          auditLog = (AuditLogController) null;
        }
        objct_PhieuDatHangNCC = (ct_PhieuDatHangNCC) null;
        transaction.Commit();
        v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC = new v_ct_PhieuDatHangNCC();
        ct_PhieuDatHangNCC.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuDatHangNCC_ChiTiet>();
        SP_Parameter SP_Parameter = new SP_Parameter();
        SP_Parameter.ID_PHIEUNHAP = ID;
        ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
        IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuDatHangNCC(SP_Parameter);
        if (actionResult is OkObjectResult okResult)
        {
          ApiResponse ApiResponse = okResult.Value as ApiResponse;
          if (ApiResponse != null && ApiResponse.Data != null)
          {
            List<v_ct_PhieuDatHangNCC> lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuDatHangNCC>;
            if (lstPhieuNhap != null && lstPhieuNhap.Count<v_ct_PhieuDatHangNCC>() > 0)
              ct_PhieuDatHangNCC = lstPhieuNhap.FirstOrDefault<v_ct_PhieuDatHangNCC>() ?? new v_ct_PhieuDatHangNCC();
            lstPhieuNhap = (List<v_ct_PhieuDatHangNCC>) null;
          }
          ApiResponse = (ApiResponse) null;
        }
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) ct_PhieuDatHangNCC
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

  [HttpPost]
  [Authorize(Roles = "User")]
  public async Task<ActionResult<ct_PhieuDatHangNCC>> PostInput([FromBody] v_ct_PhieuDatHangNCC Input)
  {
    try
    {
      if (this.InputExistsID(Input.LOC_ID, Input.ID))
        return (ActionResult<ct_PhieuDatHangNCC>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Input.LOC_ID}-{Input.ID} trong dữ liệu!",
          Data = (object) "",
          CheckValue = true
        });
      ct_PhieuDatHangNCC objPhieuNhap = await this._context.ct_PhieuDatHangNCC.FirstOrDefaultAsync<ct_PhieuDatHangNCC>((Expression<Func<ct_PhieuDatHangNCC, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.MAPHIEU));
      if (objPhieuNhap != null)
        return (ActionResult<ct_PhieuDatHangNCC>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Input.LOC_ID}-{Input.MAPHIEU} trong dữ liệu!",
          Data = (object) "",
          CheckValue = true
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        if (Input.lstct_PhieuNhap_ChiTiet != null)
        {
          foreach (v_ct_PhieuDatHangNCC_ChiTiet datHangNccChiTiet in Input.lstct_PhieuNhap_ChiTiet)
          {
            v_ct_PhieuDatHangNCC_ChiTiet itm = datHangNccChiTiet;
            dm_HangHoa_Kho objdm_HangHoa_Kho = await this._context.dm_HangHoa_Kho.FirstOrDefaultAsync<dm_HangHoa_Kho>((Expression<Func<dm_HangHoa_Kho, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO));
            if (objdm_HangHoa_Kho != null)
            {
              itm.TONGSOLUONG = itm.SOLUONG * itm.TYLE_QD;
              itm.LOC_ID = Input.LOC_ID;
              itm.ID_PHIEUDATHANGNCC = Input.ID;
              this._context.ct_PhieuDatHangNCC_ChiTiet.Add((ct_PhieuDatHangNCC_ChiTiet) itm);
              objdm_HangHoa_Kho = (dm_HangHoa_Kho) null;
            }
            else
              return (ActionResult<ct_PhieuDatHangNCC>) (ActionResult) this.Ok((object) new ApiResponse()
              {
                Success = false,
                Message = ("Không tìm thấy sản phẩm kho!" + itm.ID_HANGHOAKHO),
                Data = (object) ""
              });
          }
        }
        this._context.ct_PhieuDatHangNCC.Add((ct_PhieuDatHangNCC) Input);
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num1 = await this._context.SaveChangesAsync();
        auditLog = (AuditLogController) null;
        transaction.Commit();
        List<ct_PhieuDatHangNCC> lstPhieuDatHangCheck = await this._context.ct_PhieuDatHangNCC.Where<ct_PhieuDatHangNCC>((Expression<Func<ct_PhieuDatHangNCC, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.MAPHIEU)).OrderByDescending<ct_PhieuDatHangNCC, DateTime>((Expression<Func<ct_PhieuDatHangNCC, DateTime>>) (e => e.NGAYLAP)).ToListAsync<ct_PhieuDatHangNCC>();
        if (lstPhieuDatHangCheck != null && lstPhieuDatHangCheck.Count<ct_PhieuDatHangNCC>() > 1 && lstPhieuDatHangCheck.FirstOrDefault<ct_PhieuDatHangNCC>().ID == Input.ID)
        {
          int Max_ID = this._context.ct_PhieuDatHangNCC.Where<ct_PhieuDatHangNCC>((Expression<Func<ct_PhieuDatHangNCC, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.NGAYLAP.Date == Input.NGAYLAP.Date)).Select<ct_PhieuDatHangNCC, int>((Expression<Func<ct_PhieuDatHangNCC, int>>) (e => e.SOPHIEU)).DefaultIfEmpty<int>().Max<int>();
          Input.SOPHIEU = Max_ID + 1;
          Input.MAPHIEU = API.GetMaPhieu("Order_Provider", Input.NGAYLAP, Input.SOPHIEU);
          this._context.Entry<v_ct_PhieuDatHangNCC>(Input).State = EntityState.Modified;
          int num2 = await this._context.SaveChangesAsync();
        }
        v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC = new v_ct_PhieuDatHangNCC();
        ct_PhieuDatHangNCC.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuDatHangNCC_ChiTiet>();
        SP_Parameter SP_Parameter = new SP_Parameter();
        SP_Parameter.ID_PHIEUNHAP = Input.ID;
        ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
        IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuDatHangNCC(SP_Parameter);
        if (actionResult is OkObjectResult okResult)
        {
          ApiResponse ApiResponse = okResult.Value as ApiResponse;
          if (ApiResponse != null && ApiResponse.Data != null)
          {
            List<v_ct_PhieuDatHangNCC> lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuDatHangNCC>;
            if (lstPhieuNhap != null && lstPhieuNhap.Count<v_ct_PhieuDatHangNCC>() > 0)
              ct_PhieuDatHangNCC = lstPhieuNhap.FirstOrDefault<v_ct_PhieuDatHangNCC>() ?? new v_ct_PhieuDatHangNCC();
            lstPhieuNhap = (List<v_ct_PhieuDatHangNCC>) null;
          }
          ApiResponse = (ApiResponse) null;
        }
        return (ActionResult<ct_PhieuDatHangNCC>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) ct_PhieuDatHangNCC
        });
      }
    }
    catch (Exception ex)
    {
      return (ActionResult<ct_PhieuDatHangNCC>) (ActionResult) this.Ok((object) new ApiResponse()
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
      ct_PhieuDatHangNCC Input = await this._context.ct_PhieuDatHangNCC.FirstOrDefaultAsync<ct_PhieuDatHangNCC>((Expression<Func<ct_PhieuDatHangNCC, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Input == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      if (Input.ISHOANTAT)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Phiếu {Input.MAPHIEU} đã hoàn thành! Vui lòng kiểm tra lại!",
          Data = (object) ""
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        List<ct_PhieuDatHangNCC_ChiTiet> lstPhieuNhap_ChiTiet = await this._context.ct_PhieuDatHangNCC_ChiTiet.Where<ct_PhieuDatHangNCC_ChiTiet>((Expression<Func<ct_PhieuDatHangNCC_ChiTiet, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUDATHANGNCC == Input.ID)).ToListAsync<ct_PhieuDatHangNCC_ChiTiet>();
        if (lstPhieuNhap_ChiTiet != null)
        {
          foreach (ct_PhieuDatHangNCC_ChiTiet datHangNccChiTiet in lstPhieuNhap_ChiTiet)
          {
            ct_PhieuDatHangNCC_ChiTiet itm = datHangNccChiTiet;
            dm_HangHoa_Kho objdm_HangHoa_Kho = await this._context.dm_HangHoa_Kho.FirstOrDefaultAsync<dm_HangHoa_Kho>((Expression<Func<dm_HangHoa_Kho, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO));
            if (objdm_HangHoa_Kho != null)
            {
              itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
              this._context.ct_PhieuDatHangNCC_ChiTiet.Remove(itm);
              objdm_HangHoa_Kho = (dm_HangHoa_Kho) null;
            }
            else
              return (IActionResult) this.Ok((object) new ApiResponse()
              {
                Success = false,
                Message = ("Không tìm thấy sản phẩm kho!" + itm.ID_HANGHOAKHO),
                Data = (object) ""
              });
          }
        }
        this._context.ct_PhieuDatHangNCC.Remove(Input);
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        lstPhieuNhap_ChiTiet = (List<ct_PhieuDatHangNCC_ChiTiet>) null;
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
    return this._context.ct_PhieuDatHangNCC.Any<ct_PhieuDatHangNCC>((Expression<Func<ct_PhieuDatHangNCC, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }
}
