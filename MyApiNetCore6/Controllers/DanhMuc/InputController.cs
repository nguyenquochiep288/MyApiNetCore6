// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.InputController
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using API_QuanLyTHP.Class;
using Dapper;
using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.StoredProcedure.Parameter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using MyApiNetCore6.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace MyApiNetCore6.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InputController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;
  private readonly string _connectionString;
  private string strTable = "ct_PhieuNhap";

  public InputController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
    this._connectionString = configuration.GetConnectionString("TrangHiepPhat");
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetInput(string LOC_ID)
  {
    try
    {
      List<ct_PhieuNhap> lstValue = await this._context.ct_PhieuNhap.Where<ct_PhieuNhap>((Expression<Func<ct_PhieuNhap, bool>>) (e => e.LOC_ID == LOC_ID)).ToListAsync<ct_PhieuNhap>();
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
      List<ct_PhieuNhap> lstValue = await this._context.ct_PhieuNhap.Where<ct_PhieuNhap>((Expression<Func<ct_PhieuNhap, bool>>) (e => e.LOC_ID == LOC_ID)).Where<ct_PhieuNhap>(KeyWhere, (object) ValuesSearch).ToListAsync<ct_PhieuNhap>();
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
      ct_PhieuNhap Input = await this._context.ct_PhieuNhap.FirstOrDefaultAsync<ct_PhieuNhap>((Expression<Func<ct_PhieuNhap, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Input == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      v_ct_PhieuNhap ct_PhieuNhap = new v_ct_PhieuNhap();
      if (Input != null)
      {
        string strInput = JsonConvert.SerializeObject((object) Input);
        ct_PhieuNhap = JsonConvert.DeserializeObject<v_ct_PhieuNhap>(strInput) ?? new v_ct_PhieuNhap();
        strInput = (string) null;
      }
      ct_PhieuNhap.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuNhap_ChiTiet>();
      SP_Parameter SP_Parameter = new SP_Parameter();
      SP_Parameter.ID_PHIEUNHAP = ID;
      ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
      IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuNhap_Chitiet(SP_Parameter);
      if (actionResult is OkObjectResult okResult)
      {
        ApiResponse ApiResponse = okResult.Value as ApiResponse;
        if (ApiResponse != null && ApiResponse.Data != null)
        {
          if (ApiResponse.Data is List<v_ct_PhieuNhap_ChiTiet> lst_ChiTiet)
            ct_PhieuNhap.lstct_PhieuNhap_ChiTiet.AddRange((IEnumerable<v_ct_PhieuNhap_ChiTiet>) lst_ChiTiet);
          lst_ChiTiet = (List<v_ct_PhieuNhap_ChiTiet>) null;
        }
        ApiResponse = (ApiResponse) null;
      }
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) ct_PhieuNhap
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
  public async Task<IActionResult> PutInput(string LOC_ID, string ID, [FromBody] v_ct_PhieuNhap Input)
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
        List<ct_PhieuNhap_ChiTiet> lstPhieuNhap_ChiTiet = await this._context.ct_PhieuNhap_ChiTiet.Where<ct_PhieuNhap_ChiTiet>((Expression<Func<ct_PhieuNhap_ChiTiet, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUNHAP == Input.ID)).ToListAsync<ct_PhieuNhap_ChiTiet>();
        if (lstPhieuNhap_ChiTiet != null)
        {
          foreach (ct_PhieuNhap_ChiTiet phieuNhapChiTiet in lstPhieuNhap_ChiTiet)
          {
            ct_PhieuNhap_ChiTiet itm = phieuNhapChiTiet;
            if (!string.IsNullOrEmpty(itm.ID_THUESUAT))
            {
              dm_ThueSuat objVAT = this._context.dm_ThueSuat.FirstOrDefault<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO && e.ID == itm.ID_THUESUAT));
              itm.THUESUAT = objVAT != null ? objVAT.THUESUAT : itm.THUESUAT;
              objVAT = (dm_ThueSuat) null;
            }
            dm_HangHoa_Kho objdm_HangHoa_Kho = this._context.dm_HangHoa_Kho.FromSqlRaw<dm_HangHoa_Kho>("\r\n                                        SELECT *\r\n                                        FROM dm_HangHoa_Kho WITH (UPDLOCK, ROWLOCK)\r\n                                        WHERE LOC_ID = @loc\r\n                                          AND ID = @id\r\n                                    ", (object) new SqlParameter("@loc", (object) itm.LOC_ID), (object) new SqlParameter("@id", (object) itm.ID_HANGHOAKHO)).AsTracking<dm_HangHoa_Kho>().FirstOrDefault<dm_HangHoa_Kho>();
            if (objdm_HangHoa_Kho != null)
            {
              itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
              objdm_HangHoa_Kho.QTY -= itm.TONGSOLUONG;
              this._context.Entry<dm_HangHoa_Kho>(objdm_HangHoa_Kho).State = EntityState.Modified;
              v_ct_PhieuNhap_ChiTiet chkPhieuNhap_ChiTiet = Input.lstct_PhieuNhap_ChiTiet.Where<v_ct_PhieuNhap_ChiTiet>((Func<v_ct_PhieuNhap_ChiTiet, bool>) (e => e.ID == itm.ID)).FirstOrDefault<v_ct_PhieuNhap_ChiTiet>();
              if (chkPhieuNhap_ChiTiet != null)
              {
                chkPhieuNhap_ChiTiet.ISEDIT = true;
                chkPhieuNhap_ChiTiet.ID_PHIEUNHAP = Input.ID;
                this._context.Entry<dm_HangHoa_Kho>(objdm_HangHoa_Kho).State = EntityState.Modified;
              }
              else
                this._context.ct_PhieuNhap_ChiTiet.Remove(itm);
              objdm_HangHoa_Kho = (dm_HangHoa_Kho) null;
              chkPhieuNhap_ChiTiet = (v_ct_PhieuNhap_ChiTiet) null;
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
          foreach (v_ct_PhieuNhap_ChiTiet phieuNhapChiTiet in Input.lstct_PhieuNhap_ChiTiet)
          {
            v_ct_PhieuNhap_ChiTiet itm = phieuNhapChiTiet;
            if (!string.IsNullOrEmpty(itm.ID_THUESUAT))
            {
              dm_ThueSuat objVAT = this._context.dm_ThueSuat.FirstOrDefault<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO && e.ID == itm.ID_THUESUAT));
              itm.THUESUAT = objVAT != null ? objVAT.THUESUAT : itm.THUESUAT;
              objVAT = (dm_ThueSuat) null;
            }
            itm.THANHTIEN = itm.SOLUONG * itm.DONGIA - itm.TONGTIENGIAMGIA;
            itm.TONGCONG = itm.THANHTIEN + itm.TONGTIENVAT;
            itm.ID_PHIEUNHAP = Input.ID;
            dm_HangHoa_Kho objdm_HangHoa_Kho = this._context.dm_HangHoa_Kho.FromSqlRaw<dm_HangHoa_Kho>("\r\n                                        SELECT *\r\n                                        FROM dm_HangHoa_Kho WITH (UPDLOCK, ROWLOCK)\r\n                                        WHERE LOC_ID = @loc\r\n                                          AND ID = @id\r\n                                    ", (object) new SqlParameter("@loc", (object) itm.LOC_ID), (object) new SqlParameter("@id", (object) itm.ID_HANGHOAKHO)).AsTracking<dm_HangHoa_Kho>().FirstOrDefault<dm_HangHoa_Kho>();
            if (objdm_HangHoa_Kho != null)
            {
              itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
              objdm_HangHoa_Kho.QTY += itm.TONGSOLUONG;
              this._context.Entry<dm_HangHoa_Kho>(objdm_HangHoa_Kho).State = EntityState.Modified;
              if (!itm.ISEDIT)
                this._context.ct_PhieuNhap_ChiTiet.Add((ct_PhieuNhap_ChiTiet) itm);
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
          Input.TONGTHANHTIEN = Math.Round(Input.lstct_PhieuNhap_ChiTiet.Sum<v_ct_PhieuNhap_ChiTiet>((Func<v_ct_PhieuNhap_ChiTiet, double>) (s => s.THANHTIEN)), 0);
          Input.TONGTIENGIAMGIA = Math.Round(Input.lstct_PhieuNhap_ChiTiet.Sum<v_ct_PhieuNhap_ChiTiet>((Func<v_ct_PhieuNhap_ChiTiet, double>) (s => s.TONGTIENGIAMGIA)), 0);
          Input.TONGTIENVAT = Math.Round(Input.lstct_PhieuNhap_ChiTiet.Sum<v_ct_PhieuNhap_ChiTiet>((Func<v_ct_PhieuNhap_ChiTiet, double>) (s => s.TONGTIENVAT)), 0);
          Input.TONGTIEN = Math.Round(Input.lstct_PhieuNhap_ChiTiet.Sum<v_ct_PhieuNhap_ChiTiet>((Func<v_ct_PhieuNhap_ChiTiet, double>) (s => s.TONGCONG)), 0);
        }
        this._context.Entry<v_ct_PhieuNhap>(Input).State = EntityState.Modified;
        AuditLogController auditLog1 = new AuditLogController(this._context, this._configuration);
        auditLog1.InserAuditLog();
        int num1 = await this._context.SaveChangesAsync();
        lstPhieuNhap_ChiTiet = (List<ct_PhieuNhap_ChiTiet>) null;
        auditLog1 = (AuditLogController) null;
        transaction.Commit();
        ct_PhieuNhap InputCheck = await this._context.ct_PhieuNhap.FirstOrDefaultAsync<ct_PhieuNhap>((Expression<Func<ct_PhieuNhap, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.ID == Input.ID));
        using (IDbContextTransaction transaction1 = this._context.Database.BeginTransaction())
        {
          if (InputCheck != null && InputCheck.CHUNGTUKEMTHEO != Input.CHUNGTUKEMTHEO)
          {
            if (InputCheck.CHUNGTUKEMTHEO != null && InputCheck.CHUNGTUKEMTHEO.StartsWith("PX-"))
            {
              ct_PhieuXuat PhieuXuat = await this._context.ct_PhieuXuat.FirstOrDefaultAsync<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == InputCheck.LOC_ID && e.MAPHIEU == InputCheck.CHUNGTUKEMTHEO));
              if (PhieuXuat != null)
              {
                ct_PhieuGiaoHang_ChiTiet ct_PhieuGiaoHang_ChiTiet = await this._context.ct_PhieuGiaoHang_ChiTiet.FirstOrDefaultAsync<ct_PhieuGiaoHang_ChiTiet>((Expression<Func<ct_PhieuGiaoHang_ChiTiet, bool>>) (e => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUXUAT == PhieuXuat.ID));
                if (ct_PhieuGiaoHang_ChiTiet != null)
                {
                  double SoTien = await this._context.ct_PhieuThu.Where<ct_PhieuThu>((Expression<Func<ct_PhieuThu, bool>>) (e => e.LOC_ID == InputCheck.LOC_ID && e.CHUNGTUKEMTHEO == InputCheck.CHUNGTUKEMTHEO)).SumAsync<ct_PhieuThu>((Expression<Func<ct_PhieuThu, double>>) (e => e.SOTIEN));
                  ct_PhieuGiaoHang_ChiTiet.ISTRAHANG = false;
                  this._context.Entry<ct_PhieuGiaoHang_ChiTiet>(ct_PhieuGiaoHang_ChiTiet).State = EntityState.Modified;
                }
                ct_PhieuGiaoHang_ChiTiet = (ct_PhieuGiaoHang_ChiTiet) null;
              }
            }
            if (Input.CHUNGTUKEMTHEO != null && Input.CHUNGTUKEMTHEO.StartsWith("PX-"))
            {
              ct_PhieuXuat PhieuXuat = await this._context.ct_PhieuXuat.FirstOrDefaultAsync<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO));
              if (PhieuXuat != null)
              {
                ct_PhieuGiaoHang_ChiTiet ct_PhieuGiaoHang_ChiTiet = await this._context.ct_PhieuGiaoHang_ChiTiet.FirstOrDefaultAsync<ct_PhieuGiaoHang_ChiTiet>((Expression<Func<ct_PhieuGiaoHang_ChiTiet, bool>>) (e => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUXUAT == PhieuXuat.ID));
                if (ct_PhieuGiaoHang_ChiTiet != null)
                {
                  ct_PhieuGiaoHang_ChiTiet.ISTRAHANG = true;
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
          v_ct_PhieuNhap ct_PhieuNhap = new v_ct_PhieuNhap();
          ct_PhieuNhap.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuNhap_ChiTiet>();
          SP_Parameter SP_Parameter = new SP_Parameter();
          SP_Parameter.ID_PHIEUNHAP = Input.ID;
          ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
          IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuNhap(SP_Parameter);
          if (actionResult is OkObjectResult okResult)
          {
            ApiResponse ApiResponse = okResult.Value as ApiResponse;
            if (ApiResponse != null && ApiResponse.Data != null)
            {
              List<v_ct_PhieuNhap> lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuNhap>;
              if (lstPhieuNhap != null && lstPhieuNhap.Count<v_ct_PhieuNhap>() > 0)
                ct_PhieuNhap = lstPhieuNhap.FirstOrDefault<v_ct_PhieuNhap>() ?? new v_ct_PhieuNhap();
              lstPhieuNhap = (List<v_ct_PhieuNhap>) null;
            }
            ApiResponse = (ApiResponse) null;
          }
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = true,
            Message = "Success",
            Data = (object) ct_PhieuNhap
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
    finally
    {
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.DeleteRequest(this.strTable);
      auditLog = (AuditLogController) null;
    }
  }

  [HttpPost]
  [Authorize(Roles = "User")]
  public async Task<ActionResult<ct_PhieuNhap>> PostInput([FromBody] v_ct_PhieuNhap Input)
  {
    try
    {
      if (this.InputExistsID(Input.LOC_ID, Input.ID))
        return (ActionResult<ct_PhieuNhap>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Input.LOC_ID}-{Input.ID} trong dữ liệu!",
          Data = (object) "",
          CheckValue = true
        });
      ct_PhieuNhap objPhieuNhap = await this._context.ct_PhieuNhap.FirstOrDefaultAsync<ct_PhieuNhap>((Expression<Func<ct_PhieuNhap, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.MAPHIEU));
      if (objPhieuNhap != null)
        return (ActionResult<ct_PhieuNhap>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Input.LOC_ID}-{Input.MAPHIEU} trong dữ liệu!",
          Data = (object) "",
          CheckValue = true
        });
      if (Input.CHUNGTUKEMTHEO != null && Input.CHUNGTUKEMTHEO.StartsWith("NCC-"))
      {
        ct_PhieuDatHangNCC ct_PhieuDatHangNCC = await this._context.ct_PhieuDatHangNCC.FirstOrDefaultAsync<ct_PhieuDatHangNCC>((Expression<Func<ct_PhieuDatHangNCC, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO));
        if (ct_PhieuDatHangNCC != null && ct_PhieuDatHangNCC.ISHOANTAT)
          return (ActionResult<ct_PhieuNhap>) (ActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = $"Phiếu {ct_PhieuDatHangNCC.MAPHIEU} đã hoàn thành! Vui lòng kiểm tra lại!",
            Data = (object) "",
            CheckValue = true
          });
        ct_PhieuDatHangNCC = (ct_PhieuDatHangNCC) null;
      }
      ct_PhieuDatHangNCC objct_PhieuDatHangNCC = new ct_PhieuDatHangNCC();
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        if (Input.lstct_PhieuNhap_ChiTiet != null)
        {
          foreach (v_ct_PhieuNhap_ChiTiet phieuNhapChiTiet in Input.lstct_PhieuNhap_ChiTiet)
          {
            v_ct_PhieuNhap_ChiTiet itm = phieuNhapChiTiet;
            itm.THANHTIEN = itm.SOLUONG * itm.DONGIA - itm.TONGTIENGIAMGIA;
            itm.TONGCONG = itm.THANHTIEN + itm.TONGTIENVAT;
            dm_HangHoa_Kho objdm_HangHoa_Kho = this._context.dm_HangHoa_Kho.FromSqlRaw<dm_HangHoa_Kho>("\r\n                                        SELECT *\r\n                                        FROM dm_HangHoa_Kho WITH (UPDLOCK, ROWLOCK)\r\n                                        WHERE LOC_ID = @loc\r\n                                          AND ID = @id\r\n                                    ", (object) new SqlParameter("@loc", (object) itm.LOC_ID), (object) new SqlParameter("@id", (object) itm.ID_HANGHOAKHO)).AsTracking<dm_HangHoa_Kho>().FirstOrDefault<dm_HangHoa_Kho>();
            if (objdm_HangHoa_Kho != null)
            {
              itm.TONGSOLUONG = itm.SOLUONG * itm.TYLE_QD;
              objdm_HangHoa_Kho.QTY += itm.TONGSOLUONG;
              this._context.Entry<dm_HangHoa_Kho>(objdm_HangHoa_Kho).State = EntityState.Modified;
              itm.LOC_ID = Input.LOC_ID;
              itm.ID_PHIEUNHAP = Input.ID;
              this._context.ct_PhieuNhap_ChiTiet.Add((ct_PhieuNhap_ChiTiet) itm);
              objdm_HangHoa_Kho = (dm_HangHoa_Kho) null;
              itm = (v_ct_PhieuNhap_ChiTiet) null;
            }
            else
              return (ActionResult<ct_PhieuNhap>) (ActionResult) this.Ok((object) new ApiResponse()
              {
                Success = false,
                Message = ("Không tìm thấy sản phẩm kho!" + itm.ID_HANGHOAKHO),
                Data = (object) ""
              });
          }
          Input.TONGTHANHTIEN = Math.Round(Input.lstct_PhieuNhap_ChiTiet.Sum<v_ct_PhieuNhap_ChiTiet>((Func<v_ct_PhieuNhap_ChiTiet, double>) (s => s.THANHTIEN)), 0);
          Input.TONGTIENGIAMGIA = Math.Round(Input.lstct_PhieuNhap_ChiTiet.Sum<v_ct_PhieuNhap_ChiTiet>((Func<v_ct_PhieuNhap_ChiTiet, double>) (s => s.TONGTIENGIAMGIA)), 0);
          Input.TONGTIENVAT = Math.Round(Input.lstct_PhieuNhap_ChiTiet.Sum<v_ct_PhieuNhap_ChiTiet>((Func<v_ct_PhieuNhap_ChiTiet, double>) (s => s.TONGTIENVAT)), 0);
          Input.TONGTIEN = Math.Round(Input.lstct_PhieuNhap_ChiTiet.Sum<v_ct_PhieuNhap_ChiTiet>((Func<v_ct_PhieuNhap_ChiTiet, double>) (s => s.TONGCONG)), 0);
        }
        bool bolCheckMA = false;
        while (!bolCheckMA)
        {
          ct_PhieuNhap check = this._context.ct_PhieuNhap.Where<ct_PhieuNhap>((Expression<Func<ct_PhieuNhap, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.MAPHIEU)).FirstOrDefault<ct_PhieuNhap>();
          if (check != null)
            Input.MAPHIEU = API.GetMaPhieu(nameof (Input), Input.NGAYLAP, Input.SOPHIEU);
          else
            bolCheckMA = true;
          check = (ct_PhieuNhap) null;
        }
        this._context.ct_PhieuNhap.Add((ct_PhieuNhap) Input);
        if (Input.CHUNGTUKEMTHEO != null && Input.CHUNGTUKEMTHEO.StartsWith("PX-"))
        {
          ct_PhieuXuat PhieuXuat = await this._context.ct_PhieuXuat.FirstOrDefaultAsync<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO));
          if (PhieuXuat != null)
          {
            ct_PhieuGiaoHang_ChiTiet ct_PhieuGiaoHang_ChiTiet = await this._context.ct_PhieuGiaoHang_ChiTiet.FirstOrDefaultAsync<ct_PhieuGiaoHang_ChiTiet>((Expression<Func<ct_PhieuGiaoHang_ChiTiet, bool>>) (e => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUXUAT == PhieuXuat.ID));
            if (ct_PhieuGiaoHang_ChiTiet != null)
            {
              ct_PhieuGiaoHang_ChiTiet.ISTRAHANG = true;
              this._context.Entry<ct_PhieuGiaoHang_ChiTiet>(ct_PhieuGiaoHang_ChiTiet).State = EntityState.Modified;
            }
            ct_PhieuGiaoHang_ChiTiet = (ct_PhieuGiaoHang_ChiTiet) null;
          }
        }
        if (Input.CHUNGTUKEMTHEO != null && Input.CHUNGTUKEMTHEO.StartsWith("NCC-"))
        {
          objct_PhieuDatHangNCC = await this._context.ct_PhieuDatHangNCC.FirstOrDefaultAsync<ct_PhieuDatHangNCC>((Expression<Func<ct_PhieuDatHangNCC, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO));
          if (objct_PhieuDatHangNCC != null)
          {
            List<ct_PhieuNhap> lstPhieuNhap = await this._context.ct_PhieuNhap.Where<ct_PhieuNhap>((Expression<Func<ct_PhieuNhap, bool>>) (e => e.LOC_ID == objct_PhieuDatHangNCC.LOC_ID && e.CHUNGTUKEMTHEO == objct_PhieuDatHangNCC.MAPHIEU)).ToListAsync<ct_PhieuNhap>();
            string ChungTu = "";
            if (lstPhieuNhap != null && lstPhieuNhap.Count<ct_PhieuNhap>() > 0)
              ChungTu = string.Join(";", lstPhieuNhap.Select<ct_PhieuNhap, string>((Func<ct_PhieuNhap, string>) (e => e.MAPHIEU)));
            objct_PhieuDatHangNCC.CHUNGTUKEMTHEO = $"({Input.MAPHIEU}{(string.IsNullOrEmpty(ChungTu) ? "" : ";")}{ChungTu})";
            this._context.Entry<ct_PhieuDatHangNCC>(objct_PhieuDatHangNCC).State = EntityState.Modified;
            lstPhieuNhap = (List<ct_PhieuNhap>) null;
            ChungTu = (string) null;
          }
        }
        AuditLogController auditLog1 = new AuditLogController(this._context, this._configuration);
        auditLog1.InserAuditLog();
        int num1 = await this._context.SaveChangesAsync();
        auditLog1 = (AuditLogController) null;
        transaction.Commit();
        List<ct_PhieuNhap> lstPhieuDatHangCheck = await this._context.ct_PhieuNhap.Where<ct_PhieuNhap>((Expression<Func<ct_PhieuNhap, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.MAPHIEU)).OrderByDescending<ct_PhieuNhap, DateTime>((Expression<Func<ct_PhieuNhap, DateTime>>) (e => e.NGAYLAP)).ToListAsync<ct_PhieuNhap>();
        if (lstPhieuDatHangCheck != null && lstPhieuDatHangCheck.Count<ct_PhieuNhap>() > 1 && lstPhieuDatHangCheck.FirstOrDefault<ct_PhieuNhap>().ID == Input.ID)
        {
          int Max_ID = this._context.ct_PhieuNhap.Where<ct_PhieuNhap>((Expression<Func<ct_PhieuNhap, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.NGAYLAP.Date == Input.NGAYLAP.Date)).Select<ct_PhieuNhap, int>((Expression<Func<ct_PhieuNhap, int>>) (e => e.SOPHIEU)).DefaultIfEmpty<int>().Max<int>();
          Input.SOPHIEU = Max_ID + 1;
          Input.MAPHIEU = API.GetMaPhieu(nameof (Input), Input.NGAYLAP, Input.SOPHIEU);
          this._context.Entry<v_ct_PhieuNhap>(Input).State = EntityState.Modified;
          int num2 = await this._context.SaveChangesAsync();
        }
        if (Input.CHUNGTUKEMTHEO != null && Input.CHUNGTUKEMTHEO.StartsWith("NCC-") && objct_PhieuDatHangNCC != null && !string.IsNullOrEmpty(objct_PhieuDatHangNCC.ID))
        {
          IEnumerable<QuantityCheckResult> result = this.GetQuantityCheck(Input.CHUNGTUKEMTHEO);
          using (IDbContextTransaction transaction1 = this._context.Database.BeginTransaction())
          {
            if (result != null)
            {
              int TongSo = result.Sum<QuantityCheckResult>((Func<QuantityCheckResult, int>) (s => s.Status));
              ct_PhieuDatHangNCC OKCar = await this._context.ct_PhieuDatHangNCC.FirstOrDefaultAsync<ct_PhieuDatHangNCC>((Expression<Func<ct_PhieuDatHangNCC, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO));
              if (OKCar != null)
              {
                OKCar.ISHOANTAT = TongSo <= 0;
                this._context.Entry<ct_PhieuDatHangNCC>(OKCar).State = EntityState.Modified;
                AuditLogController auditLog2 = new AuditLogController(this._context, this._configuration);
                auditLog2.InserAuditLog();
                int num3 = await this._context.SaveChangesAsync();
                auditLog2 = (AuditLogController) null;
              }
              OKCar = (ct_PhieuDatHangNCC) null;
            }
            transaction1.Commit();
            v_ct_PhieuDatHangNCC ct_PhieuNhap = new v_ct_PhieuDatHangNCC();
            ct_PhieuNhap.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuDatHangNCC_ChiTiet>();
            SP_Parameter SP_Parameter = new SP_Parameter();
            SP_Parameter.ID_PHIEUNHAP = objct_PhieuDatHangNCC.ID;
            ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
            IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuDatHangNCC(SP_Parameter);
            if (actionResult is OkObjectResult okResult)
            {
              ApiResponse ApiResponse = okResult.Value as ApiResponse;
              if (ApiResponse != null && ApiResponse.Data != null)
              {
                List<v_ct_PhieuDatHangNCC> lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuDatHangNCC>;
                if (lstPhieuNhap != null && lstPhieuNhap.Count<v_ct_PhieuDatHangNCC>() > 0)
                  ct_PhieuNhap = lstPhieuNhap.FirstOrDefault<v_ct_PhieuDatHangNCC>() ?? new v_ct_PhieuDatHangNCC();
                lstPhieuNhap = (List<v_ct_PhieuDatHangNCC>) null;
              }
              ApiResponse = (ApiResponse) null;
            }
            return (ActionResult<ct_PhieuNhap>) (ActionResult) this.Ok((object) new ApiResponse()
            {
              Success = true,
              Message = "Success",
              Data = (object) ct_PhieuNhap
            });
          }
        }
        v_ct_PhieuNhap ct_PhieuNhap1 = new v_ct_PhieuNhap();
        ct_PhieuNhap1.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuNhap_ChiTiet>();
        SP_Parameter SP_Parameter1 = new SP_Parameter();
        SP_Parameter1.ID_PHIEUNHAP = Input.ID;
        ExecuteStoredProc ExecuteStoredProc1_1 = new ExecuteStoredProc(this._context, this._configuration);
        IActionResult actionResult1 = await ExecuteStoredProc1_1.Sp_Get_DanhSachPhieuNhap(SP_Parameter1);
        if (actionResult1 is OkObjectResult okResult1)
        {
          ApiResponse ApiResponse = okResult1.Value as ApiResponse;
          if (ApiResponse != null && ApiResponse.Data != null)
          {
            List<v_ct_PhieuNhap> lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuNhap>;
            if (lstPhieuNhap != null && lstPhieuNhap.Count<v_ct_PhieuNhap>() > 0)
              ct_PhieuNhap1 = lstPhieuNhap.FirstOrDefault<v_ct_PhieuNhap>() ?? new v_ct_PhieuNhap();
            lstPhieuNhap = (List<v_ct_PhieuNhap>) null;
          }
          ApiResponse = (ApiResponse) null;
        }
        return (ActionResult<ct_PhieuNhap>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) ct_PhieuNhap1
        });
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
    finally
    {
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.DeleteRequest(this.strTable);
      auditLog = (AuditLogController) null;
    }
  }

  [HttpPut("{MAPHIEU}")]
  public IEnumerable<QuantityCheckResult> GetQuantityCheck(string MAPHIEU)
  {
    using (SqlConnection sqlConnection = new SqlConnection(this._connectionString))
    {
      SqlConnection cnn = sqlConnection;
      var data = new{ MAPHIEU = MAPHIEU };
      CommandType? nullable = new CommandType?(CommandType.StoredProcedure);
      int? commandTimeout = new int?();
      CommandType? commandType = nullable;
      return cnn.Query<QuantityCheckResult>("CheckPhieuDatHang", (object) data, commandTimeout: commandTimeout, commandType: commandType);
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteInput(string LOC_ID, string ID)
  {
    try
    {
      ct_PhieuNhap Input = await this._context.ct_PhieuNhap.FirstOrDefaultAsync<ct_PhieuNhap>((Expression<Func<ct_PhieuNhap, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
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
        List<ct_PhieuNhap_ChiTiet> lstPhieuNhap_ChiTiet = await this._context.ct_PhieuNhap_ChiTiet.Where<ct_PhieuNhap_ChiTiet>((Expression<Func<ct_PhieuNhap_ChiTiet, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUNHAP == Input.ID)).ToListAsync<ct_PhieuNhap_ChiTiet>();
        if (lstPhieuNhap_ChiTiet != null)
        {
          foreach (ct_PhieuNhap_ChiTiet phieuNhapChiTiet in lstPhieuNhap_ChiTiet)
          {
            ct_PhieuNhap_ChiTiet itm = phieuNhapChiTiet;
            dm_HangHoa_Kho objdm_HangHoa_Kho = this._context.dm_HangHoa_Kho.FromSqlRaw<dm_HangHoa_Kho>("\r\n                                        SELECT *\r\n                                        FROM dm_HangHoa_Kho WITH (UPDLOCK, ROWLOCK)\r\n                                        WHERE LOC_ID = @loc\r\n                                          AND ID = @id\r\n                                    ", (object) new SqlParameter("@loc", (object) itm.LOC_ID), (object) new SqlParameter("@id", (object) itm.ID_HANGHOAKHO)).AsTracking<dm_HangHoa_Kho>().FirstOrDefault<dm_HangHoa_Kho>();
            if (objdm_HangHoa_Kho != null)
            {
              itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
              objdm_HangHoa_Kho.QTY -= itm.TONGSOLUONG;
              this._context.Entry<dm_HangHoa_Kho>(objdm_HangHoa_Kho).State = EntityState.Modified;
              this._context.ct_PhieuNhap_ChiTiet.Remove(itm);
              objdm_HangHoa_Kho = (dm_HangHoa_Kho) null;
              itm = (ct_PhieuNhap_ChiTiet) null;
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
        this._context.ct_PhieuNhap.Remove(Input);
        if (Input.CHUNGTUKEMTHEO != null && Input.CHUNGTUKEMTHEO.StartsWith("PX-"))
        {
          List<ct_PhieuXuat> PhieuXuat = await this._context.ct_PhieuXuat.Where<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO)).ToListAsync<ct_PhieuXuat>();
          if (PhieuXuat != null && PhieuXuat.Count == 1)
          {
            ct_PhieuGiaoHang_ChiTiet ct_PhieuGiaoHang_ChiTiet = await this._context.ct_PhieuGiaoHang_ChiTiet.FirstOrDefaultAsync<ct_PhieuGiaoHang_ChiTiet>((Expression<Func<ct_PhieuGiaoHang_ChiTiet, bool>>) (e => e.LOC_ID == PhieuXuat[0].LOC_ID && e.ID_PHIEUXUAT == PhieuXuat[0].ID));
            if (ct_PhieuGiaoHang_ChiTiet != null)
            {
              ct_PhieuGiaoHang_ChiTiet.ISTRAHANG = false;
              this._context.Entry<ct_PhieuGiaoHang_ChiTiet>(ct_PhieuGiaoHang_ChiTiet).State = EntityState.Modified;
            }
            ct_PhieuGiaoHang_ChiTiet = (ct_PhieuGiaoHang_ChiTiet) null;
          }
        }
        if (Input.CHUNGTUKEMTHEO != null && Input.CHUNGTUKEMTHEO.StartsWith("NCC-"))
        {
          List<ct_PhieuDatHangNCC> PhieuXuat = await this._context.ct_PhieuDatHangNCC.Where<ct_PhieuDatHangNCC>((Expression<Func<ct_PhieuDatHangNCC, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO)).ToListAsync<ct_PhieuDatHangNCC>();
          if (PhieuXuat != null && PhieuXuat.Count > 0)
          {
            foreach (ct_PhieuDatHangNCC itm in PhieuXuat)
            {
              List<ct_PhieuNhap> lstPhieuNhap = await this._context.ct_PhieuNhap.Where<ct_PhieuNhap>((Expression<Func<ct_PhieuNhap, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.CHUNGTUKEMTHEO == Input.CHUNGTUKEMTHEO && e.ID != Input.ID)).ToListAsync<ct_PhieuNhap>();
              string ChungTu = "";
              itm.CHUNGTUKEMTHEO = "";
              itm.ISHOANTAT = false;
              if (lstPhieuNhap != null && lstPhieuNhap.Count<ct_PhieuNhap>() > 0)
              {
                ChungTu = string.Join(";", lstPhieuNhap.Select<ct_PhieuNhap, string>((Func<ct_PhieuNhap, string>) (e => e.MAPHIEU)));
                itm.CHUNGTUKEMTHEO = $"({(string.IsNullOrEmpty(ChungTu) ? "" : ";")}{ChungTu})";
              }
              this._context.Entry<ct_PhieuDatHangNCC>(itm).State = EntityState.Modified;
              lstPhieuNhap = (List<ct_PhieuNhap>) null;
              ChungTu = (string) null;
            }
          }
          PhieuXuat = (List<ct_PhieuDatHangNCC>) null;
        }
        AuditLogController auditLog1 = new AuditLogController(this._context, this._configuration);
        auditLog1.InserAuditLog();
        int num1 = await this._context.SaveChangesAsync();
        lstPhieuThu = (List<ct_PhieuThu>) null;
        lstPhieuChi = (List<ct_PhieuChi>) null;
        lstPhieuXuat = (List<ct_PhieuXuat>) null;
        lstPhieuNhap_ChiTiet = (List<ct_PhieuNhap_ChiTiet>) null;
        auditLog1 = (AuditLogController) null;
        transaction.Commit();
        if (Input != null && Input.CHUNGTUKEMTHEO != null && Input.CHUNGTUKEMTHEO.StartsWith("NCC-"))
        {
          IEnumerable<QuantityCheckResult> result = this.GetQuantityCheck(Input.CHUNGTUKEMTHEO);
          if (result != null)
          {
            int TongSo = result.Sum<QuantityCheckResult>((Func<QuantityCheckResult, int>) (s => s.Status));
            ct_PhieuDatHangNCC OKCar = await this._context.ct_PhieuDatHangNCC.FirstOrDefaultAsync<ct_PhieuDatHangNCC>((Expression<Func<ct_PhieuDatHangNCC, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO));
            if (OKCar != null)
            {
              OKCar.ISHOANTAT = TongSo <= 0;
              this._context.Entry<ct_PhieuDatHangNCC>(OKCar).State = EntityState.Modified;
              AuditLogController auditLog2 = new AuditLogController(this._context, this._configuration);
              auditLog2.InserAuditLog();
              int num2 = await this._context.SaveChangesAsync();
              auditLog2 = (AuditLogController) null;
            }
            OKCar = (ct_PhieuDatHangNCC) null;
          }
          result = (IEnumerable<QuantityCheckResult>) null;
        }
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
    return this._context.ct_PhieuNhap.Any<ct_PhieuNhap>((Expression<Func<ct_PhieuNhap, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }
}
