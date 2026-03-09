// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.DepositController
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

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
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;


namespace MyApiNetCore6.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DepositController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;
  private static readonly Queue<v_ct_PhieuDatHang> requestQueue = new Queue<v_ct_PhieuDatHang>();
  private static bool isProcessing = false;
  private string strTable = "ct_PhieuDatHang";

  public DepositController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetDeposit(string LOC_ID)
  {
    try
    {
      List<ct_PhieuDatHang> lstValue = await this._context.ct_PhieuDatHang.Where<ct_PhieuDatHang>((Expression<Func<ct_PhieuDatHang, bool>>) (e => e.LOC_ID == LOC_ID)).ToListAsync<ct_PhieuDatHang>();
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
      List<ct_PhieuDatHang> lstValue = await this._context.ct_PhieuDatHang.Where<ct_PhieuDatHang>((Expression<Func<ct_PhieuDatHang, bool>>) (e => e.LOC_ID == LOC_ID)).Where<ct_PhieuDatHang>(KeyWhere, (object) ValuesSearch).OrderBy<ct_PhieuDatHang, string>((Expression<Func<ct_PhieuDatHang, string>>) (e => e.MAPHIEU)).ToListAsync<ct_PhieuDatHang>();
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

  [HttpGet("{LOC_ID}/{ID_KHO}/{FROMDATE}/{TODATE}/{Type}/{KeyWhere}/{ValuesSearch}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetDeposit(
    string LOC_ID,
    string ID_KHO,
    DateTime FROMDATE,
    DateTime TODATE,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      List<ct_PhieuDatHang> lstValue = await this._context.ct_PhieuDatHang.Where<ct_PhieuDatHang>((Expression<Func<ct_PhieuDatHang, bool>>) (e => e.LOC_ID == LOC_ID && e.NGAYLAP.Date >= FROMDATE.Date && e.NGAYLAP.Date <= TODATE.Date)).Where<ct_PhieuDatHang>(KeyWhere, (object) ValuesSearch).ToListAsync<ct_PhieuDatHang>();
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
  public async Task<IActionResult> GetDeposit(string LOC_ID, string ID)
  {
    try
    {
      ct_PhieuDatHang Deposit = await this._context.ct_PhieuDatHang.FirstOrDefaultAsync<ct_PhieuDatHang>((Expression<Func<ct_PhieuDatHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Deposit == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      v_ct_PhieuDatHang ct_PhieuDatHang = new v_ct_PhieuDatHang();
      if (Deposit != null)
      {
        string strDeposit = JsonConvert.SerializeObject((object) Deposit);
        ct_PhieuDatHang = JsonConvert.DeserializeObject<v_ct_PhieuDatHang>(strDeposit) ?? new v_ct_PhieuDatHang();
        strDeposit = (string) null;
      }
      ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet = new List<v_ct_PhieuDatHang_ChiTiet>();
      SP_Parameter SP_Parameter = new SP_Parameter();
      SP_Parameter.ID_PHIEUDATHANG = ID;
      ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
      IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuDatHang_ChiTiet(SP_Parameter);
      if (actionResult is OkObjectResult okResult)
      {
        ApiResponse ApiResponse = okResult.Value as ApiResponse;
        if (ApiResponse != null && ApiResponse.Data != null)
        {
          if (ApiResponse.Data is List<v_ct_PhieuDatHang_ChiTiet> lst_ChiTiet)
            ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet.AddRange((IEnumerable<v_ct_PhieuDatHang_ChiTiet>) lst_ChiTiet);
          lst_ChiTiet = (List<v_ct_PhieuDatHang_ChiTiet>) null;
        }
        ApiResponse = (ApiResponse) null;
      }
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) ct_PhieuDatHang
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

  [HttpPut("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PutDeposit(string LOC_ID, [FromBody] List<Product_Detail> lstProduct_Detail)
  {
    try
    {
      IActionResult chuongTrinhKhuyenMai = await this.Get_ChuongTrinhKhuyenMai(lstProduct_Detail, LOC_ID);
      return chuongTrinhKhuyenMai;
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

  [HttpPut("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PutDeposit(string LOC_ID, string ID, [FromBody] v_ct_PhieuDatHang Deposit)
  {
    try
    {
      if (!this.DepositExistsID(Deposit.LOC_ID, Deposit.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {Deposit.LOC_ID}-{Deposit.ID} dữ liệu!",
          Data = (object) ""
        });
      if (!string.IsNullOrEmpty(Deposit.ID_PHIEUXUAT))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Phiếu đặt hàng {Deposit.MAPHIEU} đã được tạo phiếu xuất!",
          Data = (object) ""
        });
      string StrHetSoLuong = "";
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        List<ct_PhieuDatHang_ChiTiet> lstPhieuNhap_ChiTiet = await this._context.ct_PhieuDatHang_ChiTiet.Where<ct_PhieuDatHang_ChiTiet>((Expression<Func<ct_PhieuDatHang_ChiTiet, bool>>) (e => e.LOC_ID == Deposit.LOC_ID && e.ID_PHIEUDATHANG == Deposit.ID)).ToListAsync<ct_PhieuDatHang_ChiTiet>();
        if (lstPhieuNhap_ChiTiet != null)
        {
          foreach (ct_PhieuDatHang_ChiTiet phieuDatHangChiTiet in lstPhieuNhap_ChiTiet)
          {
            ct_PhieuDatHang_ChiTiet itm = phieuDatHangChiTiet;
            if (!string.IsNullOrEmpty(itm.ID_THUESUAT))
            {
              dm_ThueSuat objVAT = this._context.dm_ThueSuat.FirstOrDefault<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO && e.ID == itm.ID_THUESUAT));
              itm.THUESUAT = objVAT != null ? objVAT.THUESUAT : itm.THUESUAT;
              objVAT = (dm_ThueSuat) null;
            }
            dm_HangHoa_Kho objdm_HangHoa_Kho = this._context.dm_HangHoa_Kho.FromSqlRaw<dm_HangHoa_Kho>("\r\n                                    SELECT *\r\n                                    FROM dm_HangHoa_Kho WITH (UPDLOCK, ROWLOCK)\r\n                                    WHERE LOC_ID = @loc\r\n                                      AND ID = @id\r\n                                ", (object) new SqlParameter("@loc", (object) itm.LOC_ID), (object) new SqlParameter("@id", (object) itm.ID_HANGHOAKHO)).AsTracking<dm_HangHoa_Kho>().FirstOrDefault<dm_HangHoa_Kho>();
            if (objdm_HangHoa_Kho != null)
            {
              itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
              objdm_HangHoa_Kho.QTY += itm.TONGSOLUONG;
              this._context.Entry<dm_HangHoa_Kho>(objdm_HangHoa_Kho).State = EntityState.Modified;
              v_ct_PhieuDatHang_ChiTiet chkPhieuNhap_ChiTiet = Deposit.lstct_PhieuDatHang_ChiTiet.Where<v_ct_PhieuDatHang_ChiTiet>((Func<v_ct_PhieuDatHang_ChiTiet, bool>) (e => e.ID == itm.ID)).FirstOrDefault<v_ct_PhieuDatHang_ChiTiet>();
              if (chkPhieuNhap_ChiTiet != null)
              {
                chkPhieuNhap_ChiTiet.ISEDIT = true;
                chkPhieuNhap_ChiTiet.ID_PHIEUDATHANG = Deposit.ID;
                ct_PhieuDatHang_ChiTiet newct_PhieuDatHang_ChiTiet = new ct_PhieuDatHang_ChiTiet();
                newct_PhieuDatHang_ChiTiet = DepositController.ConvertobjectToct_PhieuDatHang_ChiTiet<v_ct_PhieuDatHang_ChiTiet>(chkPhieuNhap_ChiTiet, itm);
                newct_PhieuDatHang_ChiTiet.TONGSOLUONG = newct_PhieuDatHang_ChiTiet.TYLE_QD * newct_PhieuDatHang_ChiTiet.SOLUONG;
                this._context.Entry<ct_PhieuDatHang_ChiTiet>(newct_PhieuDatHang_ChiTiet).State = EntityState.Modified;
                newct_PhieuDatHang_ChiTiet = (ct_PhieuDatHang_ChiTiet) null;
              }
              else
                this._context.ct_PhieuDatHang_ChiTiet.Remove(itm);
              objdm_HangHoa_Kho = (dm_HangHoa_Kho) null;
              chkPhieuNhap_ChiTiet = (v_ct_PhieuDatHang_ChiTiet) null;
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
        if (Deposit.lstct_PhieuDatHang_ChiTiet != null)
        {
          foreach (v_ct_PhieuDatHang_ChiTiet phieuDatHangChiTiet in Deposit.lstct_PhieuDatHang_ChiTiet)
          {
            v_ct_PhieuDatHang_ChiTiet itm = phieuDatHangChiTiet;
            if (!string.IsNullOrEmpty(itm.ID_THUESUAT))
            {
              dm_ThueSuat objVAT = this._context.dm_ThueSuat.FirstOrDefault<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO && e.ID == itm.ID_THUESUAT));
              itm.THUESUAT = objVAT != null ? objVAT.THUESUAT : itm.THUESUAT;
              objVAT = (dm_ThueSuat) null;
            }
            itm.THANHTIEN = itm.SOLUONG * itm.DONGIA - itm.TONGTIENGIAMGIA;
            itm.TONGCONG = itm.THANHTIEN + itm.TONGTIENVAT;
            itm.ID_PHIEUDATHANG = Deposit.ID;
            dm_HangHoa_Kho objdm_HangHoa_Kho = this._context.dm_HangHoa_Kho.FromSqlRaw<dm_HangHoa_Kho>("\r\n                                    SELECT *\r\n                                    FROM dm_HangHoa_Kho WITH (UPDLOCK, ROWLOCK)\r\n                                    WHERE LOC_ID = @loc\r\n                                      AND ID = @id\r\n                                ", (object) new SqlParameter("@loc", (object) itm.LOC_ID), (object) new SqlParameter("@id", (object) itm.ID_HANGHOAKHO)).AsTracking<dm_HangHoa_Kho>().FirstOrDefault<dm_HangHoa_Kho>();
            if (objdm_HangHoa_Kho != null)
            {
              view_dm_HangHoa objdm_HangHoa = this._context.view_dm_HangHoa.FirstOrDefault<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOA));
              itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
              if (objdm_HangHoa_Kho.QTY >= itm.TONGSOLUONG)
              {
                objdm_HangHoa_Kho.QTY -= itm.TONGSOLUONG;
                this._context.Entry<dm_HangHoa_Kho>(objdm_HangHoa_Kho).State = EntityState.Modified;
              }
              else
              {
                string Strsoluong = "";
                if (objdm_HangHoa != null && itm.TYLE_QD >= 1.0)
                {
                  int soluong = 0;
                  double num;
                  if (itm.TYLE_QD > 1.0)
                  {
                    soluong = Convert.ToInt32(objdm_HangHoa_Kho.QTY) / Convert.ToInt32(itm.TYLE_QD);
                    if (soluong > 0)
                      Strsoluong = $"{soluong.ToString("N0")} {objdm_HangHoa.NAME_DVT}";
                    if (objdm_HangHoa_Kho.QTY - (double) soluong * itm.TYLE_QD > 0.0)
                    {
                      if (!string.IsNullOrEmpty(Strsoluong))
                      {
                        string[] strArray = new string[6]
                        {
                          Strsoluong,
                          " ",
                          null,
                          null,
                          null,
                          null
                        };
                        num = objdm_HangHoa_Kho.QTY - (double) soluong * itm.TYLE_QD;
                        strArray[2] = num.ToString("N0");
                        strArray[3] = " ";
                        strArray[4] = objdm_HangHoa.NAME_DVT_QD;
                        strArray[5] = Environment.NewLine;
                        Strsoluong = string.Concat(strArray);
                      }
                      else
                      {
                        string str1 = Strsoluong;
                        num = objdm_HangHoa_Kho.QTY - (double) soluong * itm.TYLE_QD;
                        string str2 = num.ToString("N0");
                        string nameDvtQd = objdm_HangHoa.NAME_DVT_QD;
                        Strsoluong = $"{str1}{str2} {nameDvtQd}";
                      }
                    }
                    StrHetSoLuong = $"{StrHetSoLuong}Sản phẩm {itm.NAME} không đủ tồn kho!{Strsoluong}{Environment.NewLine}";
                  }
                  else
                  {
                    num = objdm_HangHoa_Kho.QTY;
                    Strsoluong = $"{num.ToString("N0")} {objdm_HangHoa.NAME_DVT}";
                    StrHetSoLuong = $"{StrHetSoLuong}Sản phẩm {itm.NAME} không đủ tồn kho!{Strsoluong}{Environment.NewLine}";
                  }
                }
                Strsoluong = (string) null;
              }
              objdm_HangHoa = (view_dm_HangHoa) null;
              if (!itm.ISEDIT)
                this._context.ct_PhieuDatHang_ChiTiet.Add((ct_PhieuDatHang_ChiTiet) itm);
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
          if (!string.IsNullOrEmpty(StrHetSoLuong))
            return (IActionResult) this.Ok((object) new ApiResponse()
            {
              Success = false,
              Message = StrHetSoLuong,
              Data = (object) ""
            });
          Deposit.TONGTHANHTIEN = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum<v_ct_PhieuDatHang_ChiTiet>((Func<v_ct_PhieuDatHang_ChiTiet, double>) (s => s.THANHTIEN)), 0);
          Deposit.TONGTIENGIAMGIA = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum<v_ct_PhieuDatHang_ChiTiet>((Func<v_ct_PhieuDatHang_ChiTiet, double>) (s => s.TONGTIENGIAMGIA)), 0);
          Deposit.TONGTIENVAT = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum<v_ct_PhieuDatHang_ChiTiet>((Func<v_ct_PhieuDatHang_ChiTiet, double>) (s => s.TONGTIENVAT)), 0);
          Deposit.TONGTIEN = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum<v_ct_PhieuDatHang_ChiTiet>((Func<v_ct_PhieuDatHang_ChiTiet, double>) (s => s.TONGCONG)), 0);
        }
        this._context.Entry<v_ct_PhieuDatHang>(Deposit).State = EntityState.Modified;
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num1 = await this._context.SaveChangesAsync();
        lstPhieuNhap_ChiTiet = (List<ct_PhieuDatHang_ChiTiet>) null;
        auditLog = (AuditLogController) null;
        transaction.Commit();
        v_ct_PhieuDatHang ct_PhieuDatHang = new v_ct_PhieuDatHang();
        ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet = new List<v_ct_PhieuDatHang_ChiTiet>();
        SP_Parameter SP_Parameter = new SP_Parameter();
        SP_Parameter.ID_PHIEUNHAP = Deposit.ID;
        ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
        IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuNhap(SP_Parameter);
        if (actionResult is OkObjectResult okResult)
        {
          ApiResponse ApiResponse = okResult.Value as ApiResponse;
          if (ApiResponse != null && ApiResponse.Data != null)
          {
            List<v_ct_PhieuDatHang> lstPhieuDatHang = ApiResponse.Data as List<v_ct_PhieuDatHang>;
            if (lstPhieuDatHang != null && lstPhieuDatHang.Count<v_ct_PhieuDatHang>() > 0)
              ct_PhieuDatHang = lstPhieuDatHang.FirstOrDefault<v_ct_PhieuDatHang>() ?? new v_ct_PhieuDatHang();
            lstPhieuDatHang = (List<v_ct_PhieuDatHang>) null;
          }
          ApiResponse = (ApiResponse) null;
        }
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) ct_PhieuDatHang
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
    finally
    {
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.DeleteRequest(this.strTable);
      auditLog = (AuditLogController) null;
    }
  }

  [HttpPost]
  [Authorize(Roles = "User")]
  public async Task<ActionResult<ct_PhieuDatHang>> PostDeposit([FromBody] v_ct_PhieuDatHang Deposit)
  {
    try
    {
      if (this.DepositExistsID(Deposit.LOC_ID, Deposit.ID))
        return (ActionResult<ct_PhieuDatHang>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Deposit.LOC_ID}-{Deposit.ID} trong dữ liệu!",
          Data = (object) "",
          CheckValue = true
        });
      dm_NhanVien objdm_NhanVien = await this._context.dm_NhanVien.FirstOrDefaultAsync<dm_NhanVien>((Expression<Func<dm_NhanVien, bool>>) (e => e.LOC_ID == Deposit.LOC_ID && e.ID_TAIKHOAN == Deposit.ID_NHANVIEN));
      if (objdm_NhanVien != null)
      {
        ct_PhieuDatHang objPhieuNhap = await this._context.ct_PhieuDatHang.FirstOrDefaultAsync<ct_PhieuDatHang>((Expression<Func<ct_PhieuDatHang, bool>>) (e => e.LOC_ID == Deposit.LOC_ID && e.MAPHIEU == Deposit.MAPHIEU));
        if (objPhieuNhap != null)
          return (ActionResult<ct_PhieuDatHang>) (ActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = $"Đã tồn tại{Deposit.LOC_ID}-{Deposit.MAPHIEU} trong dữ liệu!",
            Data = (object) "",
            CheckValue = true
          });
        using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
        {
          if (Deposit.lstct_PhieuDatHang_ChiTiet != null)
          {
            string StrHetSoLuong = "";
            foreach (v_ct_PhieuDatHang_ChiTiet phieuDatHangChiTiet1 in Deposit.lstct_PhieuDatHang_ChiTiet)
            {
              v_ct_PhieuDatHang_ChiTiet itm = phieuDatHangChiTiet1;
              if (!string.IsNullOrEmpty(itm.ID_THUESUAT))
              {
                dm_ThueSuat objVAT = this._context.dm_ThueSuat.FirstOrDefault<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO && e.ID == itm.ID_THUESUAT));
                itm.THUESUAT = objVAT != null ? objVAT.THUESUAT : itm.THUESUAT;
                objVAT = (dm_ThueSuat) null;
              }
              itm.THANHTIEN = itm.SOLUONG * itm.DONGIA - itm.TONGTIENGIAMGIA;
              itm.TONGCONG = itm.THANHTIEN + itm.TONGTIENVAT;
              dm_HangHoa_Kho objdm_HangHoa_Kho = this._context.dm_HangHoa_Kho.FromSqlRaw<dm_HangHoa_Kho>("\r\n                                    SELECT *\r\n                                    FROM dm_HangHoa_Kho WITH (UPDLOCK, ROWLOCK)\r\n                                    WHERE LOC_ID = @loc\r\n                                      AND ID = @id\r\n                                ", (object) new SqlParameter("@loc", (object) itm.LOC_ID), (object) new SqlParameter("@id", (object) itm.ID_HANGHOAKHO)).AsTracking<dm_HangHoa_Kho>().FirstOrDefault<dm_HangHoa_Kho>();
              if (objdm_HangHoa_Kho != null)
              {
                view_dm_HangHoa objdm_HangHoa = this._context.view_dm_HangHoa.FirstOrDefault<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOA));
                itm.TONGSOLUONG = itm.SOLUONG * itm.TYLE_QD;
                double num;
                if (objdm_HangHoa_Kho.QTY >= itm.TONGSOLUONG)
                {
                  v_ct_PhieuDatHang_ChiTiet phieuDatHangChiTiet2 = itm;
                  num = objdm_HangHoa_Kho.QTY;
                  string str1 = num.ToString() + ";";
                  phieuDatHangChiTiet2.GHICHU = str1;
                  objdm_HangHoa_Kho.QTY -= itm.TONGSOLUONG;
                  v_ct_PhieuDatHang_ChiTiet phieuDatHangChiTiet3 = itm;
                  string ghichu = phieuDatHangChiTiet3.GHICHU;
                  num = objdm_HangHoa_Kho.QTY;
                  string str2 = num.ToString();
                  phieuDatHangChiTiet3.GHICHU = $"{ghichu}{str2};";
                  this._context.Entry<dm_HangHoa_Kho>(objdm_HangHoa_Kho).State = EntityState.Modified;
                }
                else
                {
                  string Strsoluong = "";
                  if (objdm_HangHoa != null && itm.TYLE_QD >= 1.0)
                  {
                    int soluong = 0;
                    if (itm.TYLE_QD > 1.0)
                    {
                      soluong = Convert.ToInt32(objdm_HangHoa_Kho.QTY) / Convert.ToInt32(itm.TYLE_QD);
                      if (soluong > 0)
                        Strsoluong = $"{soluong.ToString("N0")} {objdm_HangHoa.NAME_DVT}";
                      if (objdm_HangHoa_Kho.QTY - (double) soluong * itm.TYLE_QD > 0.0)
                      {
                        if (!string.IsNullOrEmpty(Strsoluong))
                        {
                          string[] strArray = new string[6]
                          {
                            Strsoluong,
                            " ",
                            null,
                            null,
                            null,
                            null
                          };
                          num = objdm_HangHoa_Kho.QTY - (double) soluong * itm.TYLE_QD;
                          strArray[2] = num.ToString("N0");
                          strArray[3] = " ";
                          strArray[4] = objdm_HangHoa.NAME_DVT_QD;
                          strArray[5] = Environment.NewLine;
                          Strsoluong = string.Concat(strArray);
                        }
                        else
                        {
                          string str3 = Strsoluong;
                          num = objdm_HangHoa_Kho.QTY - (double) soluong * itm.TYLE_QD;
                          string str4 = num.ToString("N0");
                          string nameDvtQd = objdm_HangHoa.NAME_DVT_QD;
                          Strsoluong = $"{str3}{str4} {nameDvtQd}";
                        }
                      }
                      StrHetSoLuong = $"{StrHetSoLuong}Sản phẩm {itm.NAME} không đủ tồn kho!{Strsoluong}{Environment.NewLine}";
                    }
                    else
                    {
                      num = objdm_HangHoa_Kho.QTY;
                      Strsoluong = $"{num.ToString("N0")} {objdm_HangHoa.NAME_DVT}";
                      StrHetSoLuong = $"{StrHetSoLuong}Sản phẩm {itm.NAME} không đủ tồn kho!{Strsoluong}{Environment.NewLine}";
                    }
                  }
                  Strsoluong = (string) null;
                }
                objdm_HangHoa = (view_dm_HangHoa) null;
                ct_PhieuDatHang_ChiTiet objct_PhieuDatHang_ChiTiet = await this._context.ct_PhieuDatHang_ChiTiet.FirstOrDefaultAsync<ct_PhieuDatHang_ChiTiet>((Expression<Func<ct_PhieuDatHang_ChiTiet, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID));
                if (objct_PhieuDatHang_ChiTiet != null)
                  itm.ID = Guid.NewGuid().ToString();
                itm.LOC_ID = Deposit.LOC_ID;
                itm.ID_PHIEUDATHANG = Deposit.ID;
                this._context.ct_PhieuDatHang_ChiTiet.Add((ct_PhieuDatHang_ChiTiet) itm);
                objdm_HangHoa_Kho = (dm_HangHoa_Kho) null;
                objct_PhieuDatHang_ChiTiet = (ct_PhieuDatHang_ChiTiet) null;
              }
              else
                return (ActionResult<ct_PhieuDatHang>) (ActionResult) this.Ok((object) new ApiResponse()
                {
                  Success = false,
                  Message = ("Không tìm thấy sản phẩm kho!" + itm.ID_HANGHOAKHO),
                  Data = (object) ""
                });
            }
            if (!string.IsNullOrEmpty(StrHetSoLuong))
              return (ActionResult<ct_PhieuDatHang>) (ActionResult) this.Ok((object) new ApiResponse()
              {
                Success = false,
                Message = StrHetSoLuong,
                Data = (object) ""
              });
            Deposit.TONGTHANHTIEN = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum<v_ct_PhieuDatHang_ChiTiet>((Func<v_ct_PhieuDatHang_ChiTiet, double>) (s => s.THANHTIEN)), 0);
            Deposit.TONGTIENGIAMGIA = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum<v_ct_PhieuDatHang_ChiTiet>((Func<v_ct_PhieuDatHang_ChiTiet, double>) (s => s.TONGTIENGIAMGIA)), 0);
            Deposit.TONGTIENVAT = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum<v_ct_PhieuDatHang_ChiTiet>((Func<v_ct_PhieuDatHang_ChiTiet, double>) (s => s.TONGTIENVAT)), 0);
            Deposit.TONGTIEN = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum<v_ct_PhieuDatHang_ChiTiet>((Func<v_ct_PhieuDatHang_ChiTiet, double>) (s => s.TONGCONG)), 0);
            StrHetSoLuong = (string) null;
          }
          this._context.ct_PhieuDatHang.Add((ct_PhieuDatHang) Deposit);
          AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
          auditLog.InserAuditLog();
          int num1 = await this._context.SaveChangesAsync();
          auditLog = (AuditLogController) null;
          transaction.Commit();
          List<ct_PhieuDatHang> lstPhieuDatHangCheck = await this._context.ct_PhieuDatHang.Where<ct_PhieuDatHang>((Expression<Func<ct_PhieuDatHang, bool>>) (e => e.LOC_ID == Deposit.LOC_ID && e.MAPHIEU == Deposit.MAPHIEU)).OrderByDescending<ct_PhieuDatHang, DateTime>((Expression<Func<ct_PhieuDatHang, DateTime>>) (e => e.NGAYLAP)).ToListAsync<ct_PhieuDatHang>();
          if (lstPhieuDatHangCheck != null && lstPhieuDatHangCheck.Count<ct_PhieuDatHang>() > 1 && lstPhieuDatHangCheck.FirstOrDefault<ct_PhieuDatHang>().ID == Deposit.ID)
          {
            int Max_ID = this._context.ct_PhieuDatHang.Where<ct_PhieuDatHang>((Expression<Func<ct_PhieuDatHang, bool>>) (e => e.LOC_ID == Deposit.LOC_ID && e.NGAYLAP.Date == Deposit.NGAYLAP.Date)).Select<ct_PhieuDatHang, int>((Expression<Func<ct_PhieuDatHang, int>>) (e => e.SOPHIEU)).DefaultIfEmpty<int>().Max<int>();
            Deposit.SOPHIEU = Max_ID + 1;
            Deposit.MAPHIEU = API.GetMaPhieu(nameof (Deposit), Deposit.NGAYLAP, Deposit.SOPHIEU);
            this._context.Entry<v_ct_PhieuDatHang>(Deposit).State = EntityState.Modified;
            int num2 = await this._context.SaveChangesAsync();
          }
          v_ct_PhieuDatHang ct_PhieuDatHang = new v_ct_PhieuDatHang();
          ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet = new List<v_ct_PhieuDatHang_ChiTiet>();
          SP_Parameter SP_Parameter = new SP_Parameter();
          SP_Parameter.ID_PHIEUNHAP = Deposit.ID;
          ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
          IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuNhap(SP_Parameter);
          if (actionResult is OkObjectResult okResult)
          {
            ApiResponse ApiResponse = okResult.Value as ApiResponse;
            if (ApiResponse != null && ApiResponse.Data != null)
            {
              List<v_ct_PhieuDatHang> lstPhieuDatHang = ApiResponse.Data as List<v_ct_PhieuDatHang>;
              if (lstPhieuDatHang != null && lstPhieuDatHang.Count<v_ct_PhieuDatHang>() > 0)
                ct_PhieuDatHang = lstPhieuDatHang.FirstOrDefault<v_ct_PhieuDatHang>() ?? new v_ct_PhieuDatHang();
              lstPhieuDatHang = (List<v_ct_PhieuDatHang>) null;
            }
            ApiResponse = (ApiResponse) null;
          }
          return (ActionResult<ct_PhieuDatHang>) (ActionResult) this.Ok((object) new ApiResponse()
          {
            Success = true,
            Message = "Success",
            Data = (object) ct_PhieuDatHang
          });
        }
      }
      return (ActionResult<ct_PhieuDatHang>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = "Tài khoản chưa được gắn với nhân viên trong dữ liệu!",
        Data = (object) ""
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<ct_PhieuDatHang>) (ActionResult) this.Ok((object) new ApiResponse()
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

  [HttpPost("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PostDeposit(
    string LOC_ID,
    [FromBody] List<Product_Detail> lstProduct_Detail)
  {
    try
    {
      IActionResult chuongTrinhKhuyenMai = await this.Get_ChuongTrinhKhuyenMai(lstProduct_Detail, LOC_ID);
      return chuongTrinhKhuyenMai;
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

  [HttpPost("PostCreateOutput")]
  [Authorize(Roles = "User")]
  public async Task<ActionResult<ct_PhieuDatHang>> PostDeposit([FromBody] List<Deposit> lstDeposit)
  {
    try
    {
      string LOC_ID = "";
      string ID_KHO = "";
      string ID_NGUOITAO = "";
      DateTime NGAYLAP = new DateTime();
      DateTime now = DateTime.Now;
      NGAYLAP = now.Date;
      if (lstDeposit != null && lstDeposit.Count > 0)
      {
        Deposit Deposit = lstDeposit.FirstOrDefault<Deposit>() ?? new Deposit();
        LOC_ID = Deposit != null ? Deposit.LOC_ID : "";
        ID_NGUOITAO = Deposit != null ? Deposit.ID_NGUOITAO : "";
        DateTime dateTime;
        if (Deposit == null)
        {
          now = DateTime.Now;
          dateTime = now.Date;
        }
        else
          dateTime = Deposit.NGAYLAP;
        NGAYLAP = dateTime;
        Deposit = (Deposit) null;
        Dictionary<string, ct_PhieuXuat> lstPhieuXuatCheck = new Dictionary<string, ct_PhieuXuat>();
        List<ct_PhieuDatHang_ChiTiet> lstPhieuDatHang_ChiTiet = new List<ct_PhieuDatHang_ChiTiet>();
        Dictionary<string, string> lstPhieuDatHang = new Dictionary<string, string>();
        using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
        {
          int Max_ID1 = this._context.ct_PhieuXuat.Where<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == LOC_ID && e.NGAYLAP.Date == NGAYLAP.Date)).Select<ct_PhieuXuat, int>((Expression<Func<ct_PhieuXuat, int>>) (e => e.SOPHIEU)).DefaultIfEmpty<int>().Max<int>();
          foreach (Deposit deposit in lstDeposit)
          {
            Deposit itm = deposit;
            ct_PhieuDatHang PhieuDatHang = await this._context.ct_PhieuDatHang.FirstOrDefaultAsync<ct_PhieuDatHang>((Expression<Func<ct_PhieuDatHang, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID));
            if (PhieuDatHang == null)
              return (ActionResult<ct_PhieuDatHang>) (ActionResult) this.Ok((object) new ApiResponse()
              {
                Success = false,
                Message = $"Không tìm thấy {LOC_ID}-{itm.ID} dữ liệu!",
                Data = (object) ""
              });
            if (!string.IsNullOrEmpty(PhieuDatHang.ID_PHIEUXUAT))
              return (ActionResult<ct_PhieuDatHang>) (ActionResult) this.Ok((object) new ApiResponse()
              {
                Success = false,
                Message = $"Phiếu đặt hàng {PhieuDatHang.MAPHIEU} đã được tạo phiếu xuất!",
                Data = (object) ""
              });
            if (!string.IsNullOrEmpty(ID_KHO) && ID_KHO != PhieuDatHang.ID_KHO)
              return (ActionResult<ct_PhieuDatHang>) (ActionResult) this.Ok((object) new ApiResponse()
              {
                Success = false,
                Message = $"Phiếu đặt hàng {PhieuDatHang.MAPHIEU} khác kho với các phiếu khác!",
                Data = (object) ""
              });
            ID_KHO = PhieuDatHang.ID_KHO;
            lstPhieuDatHang.Add(PhieuDatHang.ID, PhieuDatHang.ID_KHACHHANG);
            ct_PhieuDatHang_ChiTiet[] lstChiTietPhieuDatHang_CT = await this._context.ct_PhieuDatHang_ChiTiet.Where<ct_PhieuDatHang_ChiTiet>((Expression<Func<ct_PhieuDatHang_ChiTiet, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID_PHIEUDATHANG == itm.ID)).ToArrayAsync<ct_PhieuDatHang_ChiTiet>();
            if (lstChiTietPhieuDatHang_CT == null || ((IEnumerable<ct_PhieuDatHang_ChiTiet>) lstChiTietPhieuDatHang_CT).Count<ct_PhieuDatHang_ChiTiet>() == 0)
              return (ActionResult<ct_PhieuDatHang>) (ActionResult) this.Ok((object) new ApiResponse()
              {
                Success = false,
                Message = $"Không tìm thấy chi tiết phiếu đặt hàng {LOC_ID}-{itm.ID} dữ liệu!",
                Data = (object) ""
              });
            lstPhieuDatHang_ChiTiet.AddRange((IEnumerable<ct_PhieuDatHang_ChiTiet>) lstChiTietPhieuDatHang_CT);
            PhieuDatHang = (ct_PhieuDatHang) null;
            lstChiTietPhieuDatHang_CT = (ct_PhieuDatHang_ChiTiet[]) null;
          }
          dm_LoaiPhieuXuat dm_LoaiPhieuXuat = await this._context.dm_LoaiPhieuXuat.FirstOrDefaultAsync<dm_LoaiPhieuXuat>((Expression<Func<dm_LoaiPhieuXuat, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == API.XHKH));
          foreach (IGrouping<string, string> grouping in lstPhieuDatHang.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (e => e.Value)).GroupBy<string, string>((Func<string, string>) (e => e.ToString())))
          {
            IGrouping<string, string> itm = grouping;
            IEnumerable<string> lstPhieuDatHang_KH = lstPhieuDatHang.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (e => e.Value == itm.Key.ToString())).Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (e => e.Key));
            List<ct_PhieuDatHang_ChiTiet> lstPhieuDatHang_ChiTiet_KH = lstPhieuDatHang_ChiTiet.Where<ct_PhieuDatHang_ChiTiet>((Func<ct_PhieuDatHang_ChiTiet, bool>) (e => lstPhieuDatHang_KH.Contains<string>(e.ID_PHIEUDATHANG))).ToList<ct_PhieuDatHang_ChiTiet>();
            ct_PhieuXuat newct_PhieuXuat = new ct_PhieuXuat();
            ++Max_ID1;
            newct_PhieuXuat.ID = Guid.NewGuid().ToString();
            newct_PhieuXuat.LOC_ID = LOC_ID;
            newct_PhieuXuat.ID_LOAIPHIEUXUAT = dm_LoaiPhieuXuat != null ? dm_LoaiPhieuXuat.ID : "";
            newct_PhieuXuat.NGAYLAP = NGAYLAP;
            newct_PhieuXuat.SOPHIEU = Max_ID1;
            newct_PhieuXuat.ID_KHACHHANG = itm.Key;
            newct_PhieuXuat.ID_KHO = ID_KHO;
            newct_PhieuXuat.TONGTIENGIAMGIA = Math.Round(lstPhieuDatHang_ChiTiet_KH.Sum<ct_PhieuDatHang_ChiTiet>((Func<ct_PhieuDatHang_ChiTiet, double>) (s => s.TONGTIENGIAMGIA)), 0);
            newct_PhieuXuat.TONGTHANHTIEN = Math.Round(lstPhieuDatHang_ChiTiet_KH.Sum<ct_PhieuDatHang_ChiTiet>((Func<ct_PhieuDatHang_ChiTiet, double>) (s => s.THANHTIEN)), 0);
            newct_PhieuXuat.TONGTIENVAT = Math.Round(lstPhieuDatHang_ChiTiet_KH.Sum<ct_PhieuDatHang_ChiTiet>((Func<ct_PhieuDatHang_ChiTiet, double>) (s => s.TONGTIENVAT)), 0);
            newct_PhieuXuat.TONGTIEN = Math.Round(lstPhieuDatHang_ChiTiet_KH.Sum<ct_PhieuDatHang_ChiTiet>((Func<ct_PhieuDatHang_ChiTiet, double>) (s => s.TONGCONG)), 0);
            newct_PhieuXuat.ID_NGUOITAO = ID_NGUOITAO;
            newct_PhieuXuat.THOIGIANTHEM = new DateTime?(DateTime.Now);
            newct_PhieuXuat.ISKHUYENMAI = true;
            newct_PhieuXuat.ISPHIEUDIEUHANG = true;
            lstPhieuDatHang_ChiTiet_KH = lstPhieuDatHang_ChiTiet_KH.OrderBy<ct_PhieuDatHang_ChiTiet, string>((Func<ct_PhieuDatHang_ChiTiet, string>) (s => s.ID_PHIEUDATHANG)).ThenBy<ct_PhieuDatHang_ChiTiet, int>((Func<ct_PhieuDatHang_ChiTiet, int>) (s => s.STT)).ThenBy<ct_PhieuDatHang_ChiTiet, bool>((Func<ct_PhieuDatHang_ChiTiet, bool>) (s => s.ISKHUYENMAI)).ToList<ct_PhieuDatHang_ChiTiet>();
            int STT = 0;
            string ID_PHIEUDATHANG = "";
            int STT_PHIEUDATHANG = 0;
            foreach (ct_PhieuDatHang_ChiTiet ct in lstPhieuDatHang_ChiTiet_KH)
            {
              ct_PhieuXuat_ChiTiet newct_PhieuXuat_CT = new ct_PhieuXuat_ChiTiet();
              newct_PhieuXuat_CT = DepositController.ConvertobjectToct_PhieuXuat_ChiTiet<ct_PhieuDatHang_ChiTiet>(ct, newct_PhieuXuat_CT);
              newct_PhieuXuat_CT.ID_PHIEUXUAT = newct_PhieuXuat.ID;
              newct_PhieuXuat_CT.ID_PHIEUDIEUHANG_CHITIET = ct.ID;
              if (string.IsNullOrEmpty(ID_PHIEUDATHANG) || ct.ID_PHIEUDATHANG != ID_PHIEUDATHANG || ct.ID_PHIEUDATHANG == ID_PHIEUDATHANG && ct.STT != STT_PHIEUDATHANG)
              {
                ++STT;
                STT_PHIEUDATHANG = ct.STT;
                ID_PHIEUDATHANG = ct.ID_PHIEUDATHANG;
              }
              newct_PhieuXuat_CT.STT = STT;
              this._context.ct_PhieuXuat_ChiTiet.Add(newct_PhieuXuat_CT);
              newct_PhieuXuat_CT = (ct_PhieuXuat_ChiTiet) null;
            }
            foreach (string str in lstPhieuDatHang_KH)
            {
              string value = str;
              ct_PhieuDatHang PhieuDatHang = await this._context.ct_PhieuDatHang.FirstOrDefaultAsync<ct_PhieuDatHang>((Expression<Func<ct_PhieuDatHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == value));
              if (PhieuDatHang != null)
              {
                newct_PhieuXuat.GHICHU = (string.IsNullOrEmpty(newct_PhieuXuat.GHICHU) ? "" : newct_PhieuXuat.GHICHU + ",") + PhieuDatHang.MAPHIEU;
                PhieuDatHang.ID_PHIEUXUAT = newct_PhieuXuat.ID;
                this._context.Entry<ct_PhieuDatHang>(PhieuDatHang).State = EntityState.Modified;
              }
              PhieuDatHang = (ct_PhieuDatHang) null;
            }
            bool bolCheckMA = false;
            while (!bolCheckMA)
            {
              newct_PhieuXuat.MAPHIEU = API.GetMaPhieu("Output", newct_PhieuXuat.NGAYLAP, newct_PhieuXuat.SOPHIEU);
              ct_PhieuXuat check = this._context.ct_PhieuXuat.Where<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == LOC_ID && e.MAPHIEU == newct_PhieuXuat.MAPHIEU)).FirstOrDefault<ct_PhieuXuat>();
              if (check != null)
              {
                ++Max_ID1;
                newct_PhieuXuat.SOPHIEU = Max_ID1;
              }
              else
                bolCheckMA = true;
              check = (ct_PhieuXuat) null;
            }
            this._context.ct_PhieuXuat.Add(newct_PhieuXuat);
            lstPhieuXuatCheck.Add(newct_PhieuXuat.ID, newct_PhieuXuat);
            lstPhieuDatHang_ChiTiet_KH = (List<ct_PhieuDatHang_ChiTiet>) null;
            ID_PHIEUDATHANG = (string) null;
          }
          AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
          auditLog.InserAuditLog();
          int num1 = await this._context.SaveChangesAsync();
          dm_LoaiPhieuXuat = (dm_LoaiPhieuXuat) null;
          auditLog = (AuditLogController) null;
          transaction.Commit();
          foreach (KeyValuePair<string, ct_PhieuXuat> keyValuePair in lstPhieuXuatCheck)
          {
            KeyValuePair<string, ct_PhieuXuat> itm = keyValuePair;
            List<ct_PhieuXuat> lstPhieuDatHangCheck = await this._context.ct_PhieuXuat.Where<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == itm.Value.LOC_ID && e.MAPHIEU == itm.Value.MAPHIEU)).OrderByDescending<ct_PhieuXuat, DateTime>((Expression<Func<ct_PhieuXuat, DateTime>>) (e => e.NGAYLAP)).ToListAsync<ct_PhieuXuat>();
            if (lstPhieuDatHangCheck != null && lstPhieuDatHangCheck.Count<ct_PhieuXuat>() > 1 && lstPhieuDatHangCheck.FirstOrDefault<ct_PhieuXuat>().ID == itm.Value.ID)
            {
              int Max_ID2 = this._context.ct_PhieuXuat.Where<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == itm.Value.LOC_ID && e.NGAYLAP.Date == itm.Value.NGAYLAP.Date)).Select<ct_PhieuXuat, int>((Expression<Func<ct_PhieuXuat, int>>) (e => e.SOPHIEU)).DefaultIfEmpty<int>().Max<int>();
              itm.Value.SOPHIEU = Max_ID2 + 1;
              itm.Value.MAPHIEU = API.GetMaPhieu("Output", itm.Value.NGAYLAP, itm.Value.SOPHIEU);
              this._context.Entry<ct_PhieuXuat>(itm.Value).State = EntityState.Modified;
              int num2 = await this._context.SaveChangesAsync();
            }
            lstPhieuDatHangCheck = (List<ct_PhieuXuat>) null;
          }
          return (ActionResult<ct_PhieuDatHang>) (ActionResult) this.Ok((object) new ApiResponse()
          {
            Success = true,
            Message = "Success",
            Data = (object) ""
          });
        }
      }
      return (ActionResult<ct_PhieuDatHang>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = "Không tìm thấy dữ liệu!",
        Data = (object) ""
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<ct_PhieuDatHang>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  private static ct_PhieuXuat_ChiTiet ConvertobjectToct_PhieuXuat_ChiTiet<T>(
    T objectFrom,
    ct_PhieuXuat_ChiTiet objectTo)
  {
    if ((object) objectFrom != null)
    {
      foreach (PropertyInfo property1 in objectFrom.GetType().GetProperties())
      {
        if (property1 != (PropertyInfo) null)
        {
          object obj = property1.GetValue((object) objectFrom);
          if (obj != null)
          {
            PropertyInfo property2 = objectTo.GetType().GetProperty(property1.Name);
            if (property2 != (PropertyInfo) null)
              property2.SetValue((object) objectTo, obj);
          }
        }
      }
    }
    return objectTo;
  }

  private static ct_PhieuDatHang_ChiTiet ConvertobjectToct_PhieuDatHang_ChiTiet<T>(
    T objectFrom,
    ct_PhieuDatHang_ChiTiet objectTo)
  {
    if ((object) objectFrom != null)
    {
      foreach (PropertyInfo property1 in objectFrom.GetType().GetProperties())
      {
        if (property1 != (PropertyInfo) null)
        {
          object obj = property1.GetValue((object) objectFrom);
          if (obj != null)
          {
            PropertyInfo property2 = objectTo.GetType().GetProperty(property1.Name);
            if (property2 != (PropertyInfo) null)
              property2.SetValue((object) objectTo, obj);
          }
        }
      }
    }
    return objectTo;
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteDeposit(string LOC_ID, string ID)
  {
    try
    {
      ct_PhieuDatHang Deposit = await this._context.ct_PhieuDatHang.FirstOrDefaultAsync<ct_PhieuDatHang>((Expression<Func<ct_PhieuDatHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Deposit == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      if (!string.IsNullOrEmpty(Deposit.ID_PHIEUXUAT))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Phiếu đặt hàng {Deposit.MAPHIEU} đã được tạo phiếu xuất!",
          Data = (object) ""
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        List<ct_PhieuDatHang_ChiTiet> lstPhieuNhap_ChiTiet = await this._context.ct_PhieuDatHang_ChiTiet.Where<ct_PhieuDatHang_ChiTiet>((Expression<Func<ct_PhieuDatHang_ChiTiet, bool>>) (e => e.LOC_ID == Deposit.LOC_ID && e.ID_PHIEUDATHANG == Deposit.ID)).ToListAsync<ct_PhieuDatHang_ChiTiet>();
        if (lstPhieuNhap_ChiTiet != null)
        {
          foreach (ct_PhieuDatHang_ChiTiet phieuDatHangChiTiet in lstPhieuNhap_ChiTiet)
          {
            ct_PhieuDatHang_ChiTiet itm = phieuDatHangChiTiet;
            dm_HangHoa_Kho objdm_HangHoa_Kho = this._context.dm_HangHoa_Kho.FromSqlRaw<dm_HangHoa_Kho>("\r\n                                    SELECT *\r\n                                    FROM dm_HangHoa_Kho WITH (UPDLOCK, ROWLOCK)\r\n                                    WHERE LOC_ID = @loc\r\n                                      AND ID = @id\r\n                                ", (object) new SqlParameter("@loc", (object) itm.LOC_ID), (object) new SqlParameter("@id", (object) itm.ID_HANGHOAKHO)).AsTracking<dm_HangHoa_Kho>().FirstOrDefault<dm_HangHoa_Kho>();
            if (objdm_HangHoa_Kho != null)
            {
              itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
              objdm_HangHoa_Kho.QTY += itm.TONGSOLUONG;
              this._context.Entry<dm_HangHoa_Kho>(objdm_HangHoa_Kho).State = EntityState.Modified;
              this._context.ct_PhieuDatHang_ChiTiet.Remove(itm);
              objdm_HangHoa_Kho = (dm_HangHoa_Kho) null;
              itm = (ct_PhieuDatHang_ChiTiet) null;
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
        this._context.ct_PhieuDatHang.Remove(Deposit);
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        lstPhieuNhap_ChiTiet = (List<ct_PhieuDatHang_ChiTiet>) null;
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

  private bool DepositExistsID(string LOC_ID, string ID)
  {
    return this._context.ct_PhieuDatHang.Any<ct_PhieuDatHang>((Expression<Func<ct_PhieuDatHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }

  private async Task<IActionResult> Get_ChuongTrinhKhuyenMai(
    List<Product_Detail> lstProduct_Detail,
    string LOC_ID)
  {
    try
    {
      List<v_dm_ChuongTrinhKhuyenMai> lstdm_ChuongTrinhKhuyenMai = new List<v_dm_ChuongTrinhKhuyenMai>();
      SP_Parameter SP_Parameter = new SP_Parameter();
      SP_Parameter.LOC_ID = LOC_ID;
      SP_Parameter.TUNGAY = new DateTime?(DateTime.Now.Date);
      SP_Parameter.DENNGAY = new DateTime?(DateTime.Now.Date);
      ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
      IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_ChuongTrinhKhuyenMai(SP_Parameter);
      if (actionResult is OkObjectResult okResult3)
      {
        ApiResponse ApiResponse = okResult3.Value as ApiResponse;
        if (ApiResponse != null && ApiResponse.Data != null)
        {
          if (ApiResponse.Data is List<v_dm_ChuongTrinhKhuyenMai> lst_ChiTiet)
            lstdm_ChuongTrinhKhuyenMai.AddRange((IEnumerable<v_dm_ChuongTrinhKhuyenMai>) lst_ChiTiet);
          lst_ChiTiet = (List<v_dm_ChuongTrinhKhuyenMai>) null;
        }
        ApiResponse = (ApiResponse) null;
      }
      List<Product_Detail> lstKhuyenMai = lstProduct_Detail.Where<Product_Detail>((Func<Product_Detail, bool>) (e => e.ISKHUYENMAI)).ToList<Product_Detail>();
      if (lstKhuyenMai != null && lstKhuyenMai.Count<Product_Detail>() > 0)
      {
        foreach (Product_Detail itm in lstKhuyenMai)
          lstProduct_Detail.Remove(itm);
      }
      IEnumerable<Product_Detail> lstKhuyenMai1 = lstProduct_Detail.Where<Product_Detail>((Func<Product_Detail, bool>) (e => !string.IsNullOrEmpty(e.ID_KHUYENMAI)));
      if (lstKhuyenMai1 != null && lstKhuyenMai1.Count<Product_Detail>() > 0)
      {
        foreach (Product_Detail productDetail in lstKhuyenMai1)
        {
          Product_Detail itm = productDetail;
          dm_ThueSuat clsdm_ThueSuat = new dm_ThueSuat();
          if (!string.IsNullOrEmpty(itm.ID_THUESUAT))
          {
            dm_ThueSuat dmThueSuat = await this._context.dm_ThueSuat.FirstOrDefaultAsync<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_THUESUAT));
            clsdm_ThueSuat = dmThueSuat ?? new dm_ThueSuat();
            dmThueSuat = (dm_ThueSuat) null;
          }
          itm.CHIETKHAU = 0.0;
          itm.TYPE = "CHIETKHAU";
          API.TinhTong(itm, "", lstProduct_Detail, clsdm_ThueSuat);
          itm.TONGTIENGIAMGIA = 0.0;
          itm.TYPE = "TONGTIENGIAMGIA";
          API.TinhTong(itm, "", lstProduct_Detail, clsdm_ThueSuat);
          itm.ISDALAYKHUYENMAI = false;
          itm.ID_KHUYENMAI = "";
          clsdm_ThueSuat = (dm_ThueSuat) null;
        }
      }
      double SOTIENTHUE_KM = 0.0;
      List<string> lstDanhSachDaLayKhuyenMai = new List<string>();
      List<Product_Detail> lstProduct_Detail_Tam = new List<Product_Detail>();
      foreach (v_dm_ChuongTrinhKhuyenMai chuongTrinhKhuyenMai in lstdm_ChuongTrinhKhuyenMai)
      {
        v_dm_ChuongTrinhKhuyenMai itm = chuongTrinhKhuyenMai;
        if (!itm.ID.StartsWith("4419b3ea-5fbc-4871-9c66-d29ce1d2134c") && !itm.ID.StartsWith("b442b878-78e3-48fa-9f61-86e6958ca858"))
          ;
        bool bolConSoLuong = false;
        string input = itm.MA;
        int lastIndex = input.LastIndexOf('_');
        if (lastIndex != -1)
        {
          string result = input.Substring(0, lastIndex);
          if (lstDanhSachDaLayKhuyenMai.Where<string>((Func<string, bool>) (e => e.StartsWith(result))).Count<string>() > 0)
          {
            if (lstProduct_Detail_Tam.Where<Product_Detail>((Func<Product_Detail, bool>) (s => s.SOLUONG - s.SOLUONGDALAY_KM > 0.0)).Count<Product_Detail>() > 0)
              bolConSoLuong = true;
            else
              continue;
          }
          else
          {
            lstDanhSachDaLayKhuyenMai = new List<string>();
            lstProduct_Detail_Tam = lstProduct_Detail.ToList<Product_Detail>();
            bolConSoLuong = true;
          }
        }
        else
        {
          lstDanhSachDaLayKhuyenMai = new List<string>();
          lstProduct_Detail_Tam = lstProduct_Detail.ToList<Product_Detail>();
        }
        int intCoLayKhuyenMai = 0;
        List<dm_ChuongTrinhKhuyenMai_YeuCau> lstChuongTrinhKhuyenMai_YeuCau = await this._context.dm_ChuongTrinhKhuyenMai_YeuCau.Where<dm_ChuongTrinhKhuyenMai_YeuCau>((Expression<Func<dm_ChuongTrinhKhuyenMai_YeuCau, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == itm.ID)).ToListAsync<dm_ChuongTrinhKhuyenMai_YeuCau>();
        if (itm.IS_YEUCAUCHITIET)
        {
          if (lstChuongTrinhKhuyenMai_YeuCau != null && lstChuongTrinhKhuyenMai_YeuCau.Count > 0)
          {
            int PhanNguyen = 0;
            bool bolThoatWhile = false;
            List<Product_Detail> lstSelectProduct_Detail = new List<Product_Detail>();
            List<Product_Detail> lstSelectProduct_Detail_HT = new List<Product_Detail>();
            List<Product_Detail> lstSelectProduct_Detail_Old = new List<Product_Detail>();
            while (!bolThoatWhile)
            {
              ++PhanNguyen;
              foreach (dm_ChuongTrinhKhuyenMai_YeuCau trinhKhuyenMaiYeuCau in lstChuongTrinhKhuyenMai_YeuCau)
              {
                dm_ChuongTrinhKhuyenMai_YeuCau ChiTiet = trinhKhuyenMaiYeuCau;
                List<Product_Detail> getlst = lstProduct_Detail.Where<Product_Detail>((Func<Product_Detail, bool>) (e => (!itm.ISTONGHOADON || itm.ISTONGHOADON && !e.ISDALAYKHUYENMAI) && (e.ID_HANGHOA == ChiTiet.ID_HANGHOA && ChiTiet.HINHTHUC == 0 || e.ID_NHOMHANGHOA == ChiTiet.ID_HANGHOA && ChiTiet.HINHTHUC == 1) && e.ID_DVT == ChiTiet.ID_DVT)).ToList<Product_Detail>();
                double CTKM_YC = getlst.Sum<Product_Detail>((Func<Product_Detail, double>) (e =>
                {
                  if (ChiTiet.SOLUONG <= 0.0)
                    return e.THANHTIEN;
                  return !bolConSoLuong ? e.SOLUONG : e.SOLUONG - e.SOLUONGDALAY_KM;
                }));
                if (CTKM_YC >= (ChiTiet.SOLUONG > 0.0 ? ChiTiet.SOLUONG : ChiTiet.SOTIEN) * (double) PhanNguyen)
                {
                  if (PhanNguyen == 1)
                    lstSelectProduct_Detail_HT.AddRange((IEnumerable<Product_Detail>) getlst);
                  if (itm.IS_YEUCAUCHITIET && ChiTiet.SOLUONG == 0.0 && ChiTiet.SOTIEN == 0.0)
                  {
                    bolThoatWhile = true;
                    PhanNguyen = 0;
                  }
                }
                else
                {
                  --PhanNguyen;
                  bolThoatWhile = true;
                  lstSelectProduct_Detail = lstSelectProduct_Detail_Old.ToList<Product_Detail>();
                }
                getlst = (List<Product_Detail>) null;
              }
              if (!itm.ISTINHLUYTUYEN)
                bolThoatWhile = true;
              if (!bolThoatWhile)
                lstSelectProduct_Detail_Old = lstSelectProduct_Detail_HT.ToList<Product_Detail>();
              else
                lstSelectProduct_Detail = lstSelectProduct_Detail_HT.ToList<Product_Detail>();
            }
            if (PhanNguyen > 0)
            {
              foreach (dm_ChuongTrinhKhuyenMai_YeuCau trinhKhuyenMaiYeuCau in lstChuongTrinhKhuyenMai_YeuCau)
              {
                dm_ChuongTrinhKhuyenMai_YeuCau ChiTiet = trinhKhuyenMaiYeuCau;
                double TONGTIENGIAMGIA = 0.0;
                List<Product_Detail> getlst = lstProduct_Detail.Where<Product_Detail>((Func<Product_Detail, bool>) (e => (e.ID_HANGHOA == ChiTiet.ID_HANGHOA && ChiTiet.HINHTHUC == 0 || e.ID_NHOMHANGHOA == ChiTiet.ID_HANGHOA && ChiTiet.HINHTHUC == 1) && e.ID_DVT == ChiTiet.ID_DVT)).ToList<Product_Detail>();
                foreach (Product_Detail productDetail in getlst)
                {
                  Product_Detail ChiTietHoaDon = productDetail;
                  dm_ThueSuat clsdm_ThueSuat = new dm_ThueSuat();
                  if (!string.IsNullOrEmpty(ChiTietHoaDon.ID_THUESUAT))
                  {
                    dm_ThueSuat dmThueSuat = await this._context.dm_ThueSuat.FirstOrDefaultAsync<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == ChiTietHoaDon.LOC_ID && e.ID == ChiTietHoaDon.ID_THUESUAT));
                    clsdm_ThueSuat = dmThueSuat ?? new dm_ThueSuat();
                    dmThueSuat = (dm_ThueSuat) null;
                  }
                  if (ChiTietHoaDon.CHIETKHAU < ChiTiet.CHIETKHAU)
                  {
                    TONGTIENGIAMGIA = ChiTietHoaDon.SOLUONG * ChiTietHoaDon.DONGIA * ChiTiet.CHIETKHAU / 100.0;
                    if (TONGTIENGIAMGIA > ChiTietHoaDon.TONGTIENGIAMGIA)
                    {
                      ChiTietHoaDon.CHIETKHAU = ChiTiet.CHIETKHAU;
                      ChiTietHoaDon.TYPE = "CHIETKHAU";
                      API.TinhTong(ChiTietHoaDon, "", lstProduct_Detail, clsdm_ThueSuat);
                    }
                  }
                  TONGTIENGIAMGIA = ChiTiet.TIENGIAM * (itm.ISTINHLUYTUYEN ? (double) PhanNguyen : 1.0);
                  if (ChiTietHoaDon.TONGTIENGIAMGIA < TONGTIENGIAMGIA)
                  {
                    ChiTietHoaDon.TONGTIENGIAMGIA = TONGTIENGIAMGIA;
                    ChiTietHoaDon.TYPE = "TONGTIENGIAMGIA";
                    API.TinhTong(ChiTietHoaDon, "", lstProduct_Detail, clsdm_ThueSuat);
                  }
                  ChiTietHoaDon.ISDALAYKHUYENMAI = false;
                  ChiTietHoaDon.ID_KHUYENMAI = ChiTiet.ID_CHUONGTRINHKHUYENMAI;
                  view_dm_HangHoa objHangHoa = await this._context.view_dm_HangHoa.FirstOrDefaultAsync<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ChiTietHoaDon.ID_HANGHOA));
                  if (objHangHoa != null && objHangHoa.MUCTHUE != 0.0)
                    SOTIENTHUE_KM += ChiTietHoaDon.TONGTIENGIAMGIA * objHangHoa.MUCTHUE / 100.0;
                  clsdm_ThueSuat = (dm_ThueSuat) null;
                  objHangHoa = (view_dm_HangHoa) null;
                }
                ++intCoLayKhuyenMai;
                getlst = (List<Product_Detail>) null;
              }
            }
            List<dm_ChuongTrinhKhuyenMai_Tang> lstdm_ChuongTrinhKhuyenMai_Tang = await this._context.dm_ChuongTrinhKhuyenMai_Tang.Where<dm_ChuongTrinhKhuyenMai_Tang>((Expression<Func<dm_ChuongTrinhKhuyenMai_Tang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == itm.ID)).ToListAsync<dm_ChuongTrinhKhuyenMai_Tang>();
            if (PhanNguyen > 0 && lstdm_ChuongTrinhKhuyenMai_Tang != null && lstdm_ChuongTrinhKhuyenMai_Tang.Count > 0)
            {
              string ID_KHO = lstProduct_Detail.Select<Product_Detail, string>((Func<Product_Detail, string>) (e => e.ID_KHO)).FirstOrDefault<string>();
              if (lstdm_ChuongTrinhKhuyenMai_Tang != null)
              {
                foreach (dm_ChuongTrinhKhuyenMai_Tang trinhKhuyenMaiTang in lstdm_ChuongTrinhKhuyenMai_Tang)
                {
                  dm_ChuongTrinhKhuyenMai_Tang CTKM_Tang = trinhKhuyenMaiTang;
                  dm_HangHoa_Kho HangHoaKho = await this._context.dm_HangHoa_Kho.FirstOrDefaultAsync<dm_HangHoa_Kho>((Expression<Func<dm_HangHoa_Kho, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID_HANGHOA == CTKM_Tang.ID_HANGHOA && e.ID_KHO == ID_KHO));
                  view_dm_HangHoa HangHoa = await this._context.view_dm_HangHoa.FirstOrDefaultAsync<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == CTKM_Tang.ID_HANGHOA));
                  if (HangHoaKho != null && HangHoa != null)
                  {
                    Product_Detail newProduct_Detail = new Product_Detail();
                    newProduct_Detail.STT = lstSelectProduct_Detail.Max<Product_Detail>((Func<Product_Detail, int>) (e => e.STT));
                    newProduct_Detail.ID = Guid.NewGuid().ToString();
                    newProduct_Detail.NAME = HangHoa.NAME;
                    newProduct_Detail.MA = HangHoa.MA;
                    newProduct_Detail.ID_HANGHOA = CTKM_Tang.ID_HANGHOA;
                    newProduct_Detail.ID_HANGHOAKHO = HangHoaKho.ID;
                    newProduct_Detail.DONGIA = 0.0;
                    newProduct_Detail.ID_DVT = CTKM_Tang.ID_DVT;
                    newProduct_Detail.SOLUONG = (double) PhanNguyen * CTKM_Tang.SOLUONG;
                    newProduct_Detail.CHIETKHAU = 0.0;
                    newProduct_Detail.TONGTIENGIAMGIA = 0.0;
                    newProduct_Detail.THANHTIEN = 0.0;
                    newProduct_Detail.THUESUAT = 0.0;
                    newProduct_Detail.TONGTIENVAT = 0.0;
                    newProduct_Detail.TONGCONG = 0.0;
                    if (CTKM_Tang.SOTIEN > 0.0)
                    {
                      newProduct_Detail.TONGTIENGIAMGIA = CTKM_Tang.SOTIEN * (double) PhanNguyen;
                      newProduct_Detail.TYPE = "TONGTIENGIAMGIA";
                      dm_ThueSuat clsdm_ThueSuat = new dm_ThueSuat();
                      if (!string.IsNullOrEmpty(HangHoa.ID_THUESUAT))
                      {
                        dm_ThueSuat dmThueSuat = await this._context.dm_ThueSuat.FirstOrDefaultAsync<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == HangHoa.LOC_ID && e.ID == HangHoa.ID_THUESUAT));
                        clsdm_ThueSuat = dmThueSuat ?? new dm_ThueSuat();
                        dmThueSuat = (dm_ThueSuat) null;
                      }
                      API.TinhTong(newProduct_Detail, "", lstProduct_Detail, clsdm_ThueSuat);
                      if (HangHoa != null && HangHoa.MUCTHUE != 0.0)
                        SOTIENTHUE_KM += newProduct_Detail.TONGTIENGIAMGIA * HangHoa.MUCTHUE / 100.0;
                      clsdm_ThueSuat = (dm_ThueSuat) null;
                    }
                    newProduct_Detail.ID_KHO = ID_KHO;
                    newProduct_Detail.ISKHUYENMAI = true;
                    newProduct_Detail.ID_KHUYENMAI = itm.ID;
                    if (HangHoa != null && HangHoa.ID_DVT == newProduct_Detail.ID_DVT)
                    {
                      newProduct_Detail.NAME_DVT = HangHoa.NAME_DVT;
                      if (!string.IsNullOrEmpty(HangHoa.ID_DVT_QD))
                        newProduct_Detail.TYLE_QD = HangHoa.TYLE_QD;
                      else if (HangHoa.LOAIHANGHOA == 2.ToString())
                        newProduct_Detail.TYLE_QD = 0.0;
                      else
                        newProduct_Detail.TYLE_QD = 1.0;
                      if (newProduct_Detail.SOLUONG != 0.0 && HangHoa != null && HangHoa.MUCTHUE != 0.0)
                        SOTIENTHUE_KM += newProduct_Detail.SOLUONG * newProduct_Detail.TYLE_QD * HangHoa.GIA01 * HangHoa.MUCTHUE / 100.0;
                    }
                    else if (HangHoa != null && HangHoa.ID_DVT_QD == newProduct_Detail.ID_DVT)
                    {
                      if (!string.IsNullOrEmpty(HangHoa.ID_DVT_QD))
                      {
                        newProduct_Detail.NAME_DVT = HangHoa.NAME_DVT_QD;
                        newProduct_Detail.TYLE_QD = 1.0;
                      }
                      if (newProduct_Detail.SOLUONG != 0.0 && HangHoa != null && HangHoa.MUCTHUE != 0.0)
                        SOTIENTHUE_KM += newProduct_Detail.SOLUONG * newProduct_Detail.TYLE_QD * HangHoa.GIA01_QD * HangHoa.MUCTHUE / 100.0;
                    }
                    else
                      return (IActionResult) this.Ok((object) new ApiResponse()
                      {
                        Success = false,
                        Message = $"Không tìm thấy thông tin sản phẩm với đơn vị tính {newProduct_Detail.ID_DVT} Kiểm tra CTKM{itm.NAME}",
                        Data = (object) null
                      });
                    newProduct_Detail.TONGSOLUONG = newProduct_Detail.TYLE_QD * newProduct_Detail.SOLUONG;
                    if (HangHoa != null && HangHoa.LOAIHANGHOA == 1.ToString())
                    {
                      newProduct_Detail.ID_KHUYENMAI = newProduct_Detail.ID_HANGHOA;
                      SP_Parameter objParameter = new SP_Parameter();
                      objParameter.LOC_ID = itm.LOC_ID;
                      objParameter.ID_KHO = newProduct_Detail.ID_KHO;
                      objParameter.ID_COMBO = newProduct_Detail.ID_HANGHOA;
                      ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
                      actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachSanPhamKho_Combo(objParameter);
                      if (actionResult is OkObjectResult okResult3)
                      {
                        ApiResponse ApiResponse = okResult3.Value as ApiResponse;
                        if (ApiResponse != null && ApiResponse.Data != null)
                        {
                          if (ApiResponse.Data is List<Product_Detail> lst_ChiTiet)
                          {
                            foreach (Product_Detail ChiTiet in lst_ChiTiet)
                            {
                              ChiTiet.STT = newProduct_Detail.STT;
                              ChiTiet.ID = Guid.NewGuid().ToString();
                              ChiTiet.ID_DVT = ChiTiet.ID_DVT_COMBO;
                              ChiTiet.SOLUONG = newProduct_Detail.SOLUONG * ChiTiet.QTY_COMBO;
                              ChiTiet.TYLE_QD = ChiTiet.TYLE_QD_COMBO;
                              ChiTiet.TONGSOLUONG = newProduct_Detail.SOLUONG * ChiTiet.QTY_TOTAL_COMBO;
                              ChiTiet.DONGIA = 0.0;
                              ChiTiet.CHIETKHAU = 0.0;
                              ChiTiet.TONGTIENGIAMGIA = 0.0;
                              ChiTiet.THANHTIEN = 0.0;
                              ChiTiet.THUESUAT = 0.0;
                              ChiTiet.TONGTIENVAT = 0.0;
                              ChiTiet.TONGCONG = 0.0;
                              ChiTiet.ISKHUYENMAI = true;
                              ChiTiet.ID_KHUYENMAI = itm.ID;
                              ChiTiet.ISCOMBO = true;
                              ChiTiet.ID_COMBO = newProduct_Detail.ID_HANGHOA;
                              lstProduct_Detail.Add(ChiTiet);
                            }
                          }
                          lst_ChiTiet = (List<Product_Detail>) null;
                        }
                        ApiResponse = (ApiResponse) null;
                      }
                      objParameter = (SP_Parameter) null;
                    }
                    lstProduct_Detail.Add(newProduct_Detail);
                    newProduct_Detail = (Product_Detail) null;
                  }
                  HangHoaKho = (dm_HangHoa_Kho) null;
                }
                ++intCoLayKhuyenMai;
              }
            }
            if (PhanNguyen > 0 && (itm.CHIETKHAU > 0.0 || itm.TIENGIAM > 0.0))
            {
              lstSelectProduct_Detail = lstSelectProduct_Detail.Where<Product_Detail>((Func<Product_Detail, bool>) (e => !e.ISKHUYENMAI)).ToList<Product_Detail>();
              Dictionary<string, int> lstCTKM_YC = new Dictionary<string, int>();
              if (lstChuongTrinhKhuyenMai_YeuCau != null && lstChuongTrinhKhuyenMai_YeuCau.Count > 0)
              {
                foreach (dm_ChuongTrinhKhuyenMai_YeuCau CTKM_YC in lstChuongTrinhKhuyenMai_YeuCau)
                  lstCTKM_YC.Add(CTKM_YC.ID_HANGHOA, CTKM_YC.HINHTHUC);
              }
              if (lstSelectProduct_Detail != null && lstSelectProduct_Detail.Count > 0)
              {
                double SumTien = 0.0;
                double SumSoLuong = 0.0;
                if (lstChuongTrinhKhuyenMai_YeuCau != null && lstChuongTrinhKhuyenMai_YeuCau.Count > 0)
                {
                  SumTien = lstSelectProduct_Detail.Where<Product_Detail>((Func<Product_Detail, bool>) (e => !e.ISDALAYKHUYENMAI && lstCTKM_YC.Count<KeyValuePair<string, int>>((Func<KeyValuePair<string, int>, bool>) (s => s.Key.Contains(e.ID_HANGHOA) && s.Value == 0)) > 0 || lstCTKM_YC.Count<KeyValuePair<string, int>>((Func<KeyValuePair<string, int>, bool>) (s => s.Key.Contains(e.ID_NHOMHANGHOA) && s.Value == 1)) > 0)).Sum<Product_Detail>((Func<Product_Detail, double>) (s => Convert.ToDouble(s.THANHTIEN)));
                  if (!string.IsNullOrEmpty(itm.ID_DVT_DATKM))
                    SumSoLuong = lstSelectProduct_Detail.Where<Product_Detail>((Func<Product_Detail, bool>) (e => !e.ISDALAYKHUYENMAI && (lstCTKM_YC.Count<KeyValuePair<string, int>>((Func<KeyValuePair<string, int>, bool>) (s => s.Key.Contains(e.ID_HANGHOA) && s.Value == 0)) > 0 || lstCTKM_YC.Count<KeyValuePair<string, int>>((Func<KeyValuePair<string, int>, bool>) (s => s.Key.Contains(e.ID_NHOMHANGHOA) && s.Value == 1)) > 0) && e.ID_DVT == itm.ID_DVT_DATKM)).Sum<Product_Detail>((Func<Product_Detail, double>) (s => Convert.ToDouble(s.SOLUONG)));
                }
                else
                {
                  SumTien = lstSelectProduct_Detail.Where<Product_Detail>((Func<Product_Detail, bool>) (e => !e.ISDALAYKHUYENMAI && !e.ISKHUYENMAI)).Sum<Product_Detail>((Func<Product_Detail, double>) (s => Convert.ToDouble(s.TONGCONG)));
                  if (!string.IsNullOrEmpty(itm.ID_DVT_DATKM))
                    SumSoLuong = lstSelectProduct_Detail.Where<Product_Detail>((Func<Product_Detail, bool>) (e => !e.ISDALAYKHUYENMAI && e.ID_DVT == itm.ID_DVT_DATKM)).Sum<Product_Detail>((Func<Product_Detail, double>) (s => Convert.ToDouble(s.TONGSOLUONG)));
                }
                if ((itm.SOLUONG_DATKM != 0.0 || itm.SOLUONG_DATKM_DEN != 0.0) && SumSoLuong != 0.0 && itm.SOLUONG_DATKM <= SumSoLuong && (itm.SOLUONG_DATKM_DEN == 0.0 || itm.SOLUONG_DATKM_DEN >= SumSoLuong) || (itm.TONGTIEN_DATKM != 0.0 || itm.TONGTIEN_DATKM_DEN != 0.0) && SumTien != 0.0 && itm.TONGTIEN_DATKM <= SumTien && (itm.TONGTIEN_DATKM_DEN == 0.0 || itm.TONGTIEN_DATKM_DEN >= SumTien))
                {
                  if (!itm.ISTONGHOADON)
                  {
                    foreach (Product_Detail productDetail in lstSelectProduct_Detail)
                    {
                      Product_Detail ChiTiet = productDetail;
                      dm_ThueSuat clsdm_ThueSuat = new dm_ThueSuat();
                      if (!string.IsNullOrEmpty(ChiTiet.ID_THUESUAT))
                      {
                        dm_ThueSuat dmThueSuat = await this._context.dm_ThueSuat.FirstOrDefaultAsync<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == ChiTiet.LOC_ID && e.ID == ChiTiet.ID_THUESUAT));
                        clsdm_ThueSuat = dmThueSuat ?? new dm_ThueSuat();
                        dmThueSuat = (dm_ThueSuat) null;
                      }
                      if (itm.CHIETKHAU > 0.0)
                      {
                        ChiTiet.CHIETKHAU = itm.CHIETKHAU;
                        ChiTiet.TYPE = "CHIETKHAU";
                        API.TinhTong(ChiTiet, "", lstProduct_Detail, clsdm_ThueSuat);
                        ChiTiet.ID_KHUYENMAI = itm.ID;
                        view_dm_HangHoa objHangHoa = await this._context.view_dm_HangHoa.FirstOrDefaultAsync<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == ChiTiet.LOC_ID && e.ID == ChiTiet.ID_HANGHOA));
                        if (objHangHoa != null && objHangHoa.MUCTHUE != 0.0)
                          SOTIENTHUE_KM += ChiTiet.TONGTIENGIAMGIA * objHangHoa.MUCTHUE / 100.0;
                        objHangHoa = (view_dm_HangHoa) null;
                      }
                      else if (itm.TIENGIAM > 0.0)
                      {
                        ChiTiet.TONGTIENGIAMGIA = itm.TIENGIAM;
                        ChiTiet.TYPE = "TONGTIENGIAMGIA";
                        API.TinhTong(ChiTiet, "", lstProduct_Detail, clsdm_ThueSuat);
                        ChiTiet.ID_KHUYENMAI = itm.ID;
                        view_dm_HangHoa objHangHoa = await this._context.view_dm_HangHoa.FirstOrDefaultAsync<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == ChiTiet.LOC_ID && e.ID == ChiTiet.ID_HANGHOA));
                        if (objHangHoa != null && objHangHoa.MUCTHUE != 0.0)
                          SOTIENTHUE_KM += ChiTiet.TONGTIENGIAMGIA * objHangHoa.MUCTHUE / 100.0;
                        objHangHoa = (view_dm_HangHoa) null;
                      }
                      clsdm_ThueSuat = (dm_ThueSuat) null;
                    }
                  }
                  else
                  {
                    string ID_KHO = lstProduct_Detail.Select<Product_Detail, string>((Func<Product_Detail, string>) (e => e.ID_KHO)).FirstOrDefault<string>();
                    view_dm_HangHoa HangHoa = await this._context.view_dm_HangHoa.FirstOrDefaultAsync<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.MA == API.GTBH));
                    if (HangHoa != null)
                    {
                      dm_HangHoa_Kho HangHoaKho = await this._context.dm_HangHoa_Kho.FirstOrDefaultAsync<dm_HangHoa_Kho>((Expression<Func<dm_HangHoa_Kho, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID_HANGHOA == HangHoa.ID && e.ID_KHO == ID_KHO));
                      if (HangHoaKho != null)
                      {
                        Product_Detail newProduct_Detail = new Product_Detail();
                        newProduct_Detail.STT = lstSelectProduct_Detail.Max<Product_Detail>((Func<Product_Detail, int>) (e => e.STT));
                        newProduct_Detail.ID = Guid.NewGuid().ToString();
                        newProduct_Detail.NAME = HangHoa.NAME;
                        newProduct_Detail.MA = HangHoa.MA;
                        newProduct_Detail.ID_HANGHOA = HangHoaKho.ID_HANGHOA;
                        newProduct_Detail.ID_HANGHOAKHO = HangHoaKho.ID;
                        newProduct_Detail.DONGIA = 0.0;
                        newProduct_Detail.ID_DVT = HangHoa.ID_DVT;
                        newProduct_Detail.NAME_DVT = HangHoa.NAME_DVT;
                        newProduct_Detail.SOLUONG = 0.0;
                        dm_ThueSuat clsdm_ThueSuat = new dm_ThueSuat();
                        if (!string.IsNullOrEmpty(HangHoa.ID_THUESUAT))
                        {
                          dm_ThueSuat dmThueSuat = await this._context.dm_ThueSuat.FirstOrDefaultAsync<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == HangHoa.ID_THUESUAT));
                          clsdm_ThueSuat = dmThueSuat ?? new dm_ThueSuat();
                          dmThueSuat = (dm_ThueSuat) null;
                        }
                        if (itm.CHIETKHAU > 0.0)
                        {
                          newProduct_Detail.CHIETKHAU = itm.CHIETKHAU;
                          newProduct_Detail.TONGTIENGIAMGIA = lstSelectProduct_Detail.Where<Product_Detail>((Func<Product_Detail, bool>) (e => !e.ISKHUYENMAI)).Sum<Product_Detail>((Func<Product_Detail, double>) (e => e.THANHTIEN)) * newProduct_Detail.CHIETKHAU / 100.0;
                          newProduct_Detail.TYPE = "TONGTIENGIAMGIA";
                          API.TinhTong(newProduct_Detail, "", lstProduct_Detail, clsdm_ThueSuat);
                          newProduct_Detail.ISDALAYKHUYENMAI = true;
                          newProduct_Detail.SOLUONGDALAYKHUYENMAI = newProduct_Detail.TONGSOLUONG;
                          newProduct_Detail.ID_KHUYENMAI = itm.ID;
                          foreach (Product_Detail productDetail in lstSelectProduct_Detail)
                          {
                            Product_Detail s = productDetail;
                            view_dm_HangHoa objHangHoa = await this._context.view_dm_HangHoa.FirstOrDefaultAsync<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == itm.LOC_ID && (e.ID == s.ID || e.ID_NHOMHANGHOA == s.ID)));
                            if (objHangHoa != null && objHangHoa.MUCTHUE != 0.0)
                            {
                              SOTIENTHUE_KM += newProduct_Detail.TONGTIENGIAMGIA * objHangHoa.MUCTHUE / 100.0;
                              break;
                            }
                            objHangHoa = (view_dm_HangHoa) null;
                          }
                        }
                        else if (itm.TIENGIAM > 0.0)
                        {
                          newProduct_Detail.TONGTIENGIAMGIA = itm.TIENGIAM * (double) PhanNguyen;
                          newProduct_Detail.TYPE = "TONGTIENGIAMGIA";
                          API.TinhTong(newProduct_Detail, "", lstProduct_Detail, clsdm_ThueSuat);
                          newProduct_Detail.ISDALAYKHUYENMAI = true;
                          newProduct_Detail.SOLUONGDALAYKHUYENMAI = newProduct_Detail.TONGSOLUONG;
                          newProduct_Detail.ID_KHUYENMAI = itm.ID;
                          foreach (Product_Detail productDetail in lstSelectProduct_Detail)
                          {
                            Product_Detail s = productDetail;
                            view_dm_HangHoa objHangHoa = await this._context.view_dm_HangHoa.FirstOrDefaultAsync<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == itm.LOC_ID && (e.ID == s.ID || e.ID_NHOMHANGHOA == s.ID)));
                            if (objHangHoa != null && objHangHoa.MUCTHUE != 0.0)
                            {
                              SOTIENTHUE_KM += newProduct_Detail.TONGTIENGIAMGIA * objHangHoa.MUCTHUE / 100.0;
                              break;
                            }
                            objHangHoa = (view_dm_HangHoa) null;
                          }
                        }
                        newProduct_Detail.ID_KHO = ID_KHO;
                        newProduct_Detail.ISDALAYKHUYENMAI = true;
                        newProduct_Detail.ISKHUYENMAI = true;
                        newProduct_Detail.ID_KHUYENMAI = itm.ID;
                        lstProduct_Detail.Add(newProduct_Detail);
                        foreach (Product_Detail ChiTiet in lstSelectProduct_Detail)
                          ChiTiet.ISDALAYKHUYENMAI = true;
                        newProduct_Detail = (Product_Detail) null;
                        clsdm_ThueSuat = (dm_ThueSuat) null;
                      }
                      HangHoaKho = (dm_HangHoa_Kho) null;
                    }
                  }
                  ++intCoLayKhuyenMai;
                }
              }
            }
            lstSelectProduct_Detail = (List<Product_Detail>) null;
            lstSelectProduct_Detail_HT = (List<Product_Detail>) null;
            lstSelectProduct_Detail_Old = (List<Product_Detail>) null;
            lstdm_ChuongTrinhKhuyenMai_Tang = (List<dm_ChuongTrinhKhuyenMai_Tang>) null;
          }
        }
        else
        {
          List<Product_Detail> lstSelectProduct_Detail = new List<Product_Detail>();
          Dictionary<string, int> lstCTKM_YC = new Dictionary<string, int>();
          double MUCTHUE = 0.0;
          int PhanNguyen = 0;
          bool bolBatBuoc = false;
          if (lstChuongTrinhKhuyenMai_YeuCau != null && lstChuongTrinhKhuyenMai_YeuCau.Count > 0)
          {
            bool isOk = true;
            foreach (dm_ChuongTrinhKhuyenMai_YeuCau trinhKhuyenMaiYeuCau in lstChuongTrinhKhuyenMai_YeuCau)
            {
              dm_ChuongTrinhKhuyenMai_YeuCau CTKM_YC = trinhKhuyenMaiYeuCau;
              lstCTKM_YC.Add(CTKM_YC.ID_HANGHOA, CTKM_YC.HINHTHUC);
              view_dm_HangHoa objHangHoa = await this._context.view_dm_HangHoa.FirstOrDefaultAsync<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == itm.LOC_ID && (e.ID == CTKM_YC.ID_HANGHOA || e.ID_NHOMHANGHOA == CTKM_YC.ID_HANGHOA)));
              if (MUCTHUE == 0.0 && objHangHoa != null && objHangHoa.MUCTHUE != 0.0)
                MUCTHUE = objHangHoa.MUCTHUE;
              if (CTKM_YC.ISBATBUOC)
              {
                double SumSoLuong = 0.0;
                SumSoLuong = !bolConSoLuong ? lstProduct_Detail.Where<Product_Detail>((Func<Product_Detail, bool>) (e => (!itm.ISTONGHOADON || itm.ISTONGHOADON && !e.ISDALAYKHUYENMAI) && (e.ID_HANGHOA == CTKM_YC.ID_HANGHOA && CTKM_YC.HINHTHUC == 0 || e.ID_NHOMHANGHOA == CTKM_YC.ID_HANGHOA && CTKM_YC.HINHTHUC == 1) && e.ID_DVT == CTKM_YC.ID_DVT)).Sum<Product_Detail>((Func<Product_Detail, double>) (s => Convert.ToDouble(s.SOLUONG))) : lstProduct_Detail_Tam.Where<Product_Detail>((Func<Product_Detail, bool>) (e => (!itm.ISTONGHOADON || itm.ISTONGHOADON && !e.ISDALAYKHUYENMAI) && (e.ID_HANGHOA == CTKM_YC.ID_HANGHOA && CTKM_YC.HINHTHUC == 0 || e.ID_NHOMHANGHOA == CTKM_YC.ID_HANGHOA && CTKM_YC.HINHTHUC == 1) && e.ID_DVT == CTKM_YC.ID_DVT)).Sum<Product_Detail>((Func<Product_Detail, double>) (s => Convert.ToDouble(s.SOLUONG - s.SOLUONGDALAY_KM)));
                if (SumSoLuong < CTKM_YC.SOLUONG_BATBUOC)
                {
                  isOk = false;
                  break;
                }
              }
              objHangHoa = (view_dm_HangHoa) null;
            }
            if (isOk)
            {
              if (itm.ISTINHLUYTUYEN)
              {
                bool bolThoatwhile = false;
                IEnumerable<dm_ChuongTrinhKhuyenMai_YeuCau> lstbatBuoc = lstChuongTrinhKhuyenMai_YeuCau.Where<dm_ChuongTrinhKhuyenMai_YeuCau>((Func<dm_ChuongTrinhKhuyenMai_YeuCau, bool>) (e => e.ISBATBUOC));
                if (lstbatBuoc != null && lstbatBuoc.Count<dm_ChuongTrinhKhuyenMai_YeuCau>() > 0)
                {
                  while (!bolThoatwhile)
                  {
                    ++PhanNguyen;
                    foreach (dm_ChuongTrinhKhuyenMai_YeuCau trinhKhuyenMaiYeuCau in lstbatBuoc)
                    {
                      dm_ChuongTrinhKhuyenMai_YeuCau CTKM_YC = trinhKhuyenMaiYeuCau;
                      double SumSoLuong = 0.0;
                      SumSoLuong = !bolConSoLuong ? lstProduct_Detail.Where<Product_Detail>((Func<Product_Detail, bool>) (e => (!itm.ISTONGHOADON || itm.ISTONGHOADON && !e.ISDALAYKHUYENMAI) && (e.ID_HANGHOA == CTKM_YC.ID_HANGHOA && CTKM_YC.HINHTHUC == 0 || e.ID_NHOMHANGHOA == CTKM_YC.ID_HANGHOA && CTKM_YC.HINHTHUC == 1) && e.ID_DVT == CTKM_YC.ID_DVT)).Sum<Product_Detail>((Func<Product_Detail, double>) (s => Convert.ToDouble(s.SOLUONG))) : lstProduct_Detail_Tam.Where<Product_Detail>((Func<Product_Detail, bool>) (e => (!itm.ISTONGHOADON || itm.ISTONGHOADON && !e.ISDALAYKHUYENMAI) && (e.ID_HANGHOA == CTKM_YC.ID_HANGHOA && CTKM_YC.HINHTHUC == 0 || e.ID_NHOMHANGHOA == CTKM_YC.ID_HANGHOA && CTKM_YC.HINHTHUC == 1) && e.ID_DVT == CTKM_YC.ID_DVT)).Sum<Product_Detail>((Func<Product_Detail, double>) (s => Convert.ToDouble(s.SOLUONG - s.SOLUONGDALAY_KM)));
                      if (SumSoLuong < CTKM_YC.SOLUONG_BATBUOC * (double) PhanNguyen)
                      {
                        --PhanNguyen;
                        bolThoatwhile = true;
                        break;
                      }
                      bolBatBuoc = true;
                    }
                  }
                }
                lstbatBuoc = (IEnumerable<dm_ChuongTrinhKhuyenMai_YeuCau>) null;
              }
              List<Product_Detail> Tam = (List<Product_Detail>) null;
              Tam = !bolConSoLuong ? lstProduct_Detail.Where<Product_Detail>((Func<Product_Detail, bool>) (e => !string.IsNullOrEmpty(e.ID_HANGHOA) && !string.IsNullOrEmpty(e.ID_NHOMHANGHOA) && !e.ISKHUYENMAI)).ToList<Product_Detail>() : lstProduct_Detail_Tam.Where<Product_Detail>((Func<Product_Detail, bool>) (e => !string.IsNullOrEmpty(e.ID_HANGHOA) && !string.IsNullOrEmpty(e.ID_NHOMHANGHOA) && !e.ISKHUYENMAI && e.SOLUONG - e.SOLUONGDALAY_KM > 0.0)).ToList<Product_Detail>();
              if (Tam != null)
                lstSelectProduct_Detail = Tam.Where<Product_Detail>((Func<Product_Detail, bool>) (e => lstCTKM_YC.Count<KeyValuePair<string, int>>((Func<KeyValuePair<string, int>, bool>) (s => s.Key.Contains(e.ID_HANGHOA) && s.Value == 0)) > 0 || lstCTKM_YC.Count<KeyValuePair<string, int>>((Func<KeyValuePair<string, int>, bool>) (s => s.Key.Contains(e.ID_NHOMHANGHOA) && s.Value == 1)) > 0)).ToList<Product_Detail>();
              Tam = (List<Product_Detail>) null;
            }
            else
              continue;
          }
          else
            lstSelectProduct_Detail = !bolConSoLuong ? lstProduct_Detail.Where<Product_Detail>((Func<Product_Detail, bool>) (e => !e.ISKHUYENMAI)).ToList<Product_Detail>() : lstProduct_Detail.Where<Product_Detail>((Func<Product_Detail, bool>) (e => !e.ISKHUYENMAI && e.SOLUONG - e.SOLUONGDALAY_KM > 0.0)).ToList<Product_Detail>();
          if (lstSelectProduct_Detail != null && lstSelectProduct_Detail.Count > 0)
          {
            double SumTien = 0.0;
            double SumSoLuong = 0.0;
            if (lstChuongTrinhKhuyenMai_YeuCau != null && lstChuongTrinhKhuyenMai_YeuCau.Count > 0)
            {
              SumTien = lstSelectProduct_Detail.Where<Product_Detail>((Func<Product_Detail, bool>) (e => !e.ISKHUYENMAI && lstCTKM_YC.Count<KeyValuePair<string, int>>((Func<KeyValuePair<string, int>, bool>) (s => s.Key.Contains(e.ID_HANGHOA) && s.Value == 0)) > 0 || lstCTKM_YC.Count<KeyValuePair<string, int>>((Func<KeyValuePair<string, int>, bool>) (s => s.Key.Contains(e.ID_NHOMHANGHOA) && s.Value == 1)) > 0)).Sum<Product_Detail>((Func<Product_Detail, double>) (s => Convert.ToDouble(s.THANHTIEN)));
              if (!string.IsNullOrEmpty(itm.ID_DVT_DATKM))
                SumSoLuong = !bolConSoLuong ? lstSelectProduct_Detail.Where<Product_Detail>((Func<Product_Detail, bool>) (e => !e.ISKHUYENMAI && (lstCTKM_YC.Count<KeyValuePair<string, int>>((Func<KeyValuePair<string, int>, bool>) (s => s.Key.Contains(e.ID_HANGHOA) && s.Value == 0)) > 0 || lstCTKM_YC.Count<KeyValuePair<string, int>>((Func<KeyValuePair<string, int>, bool>) (s => s.Key.Contains(e.ID_NHOMHANGHOA) && s.Value == 1)) > 0) && e.ID_DVT == itm.ID_DVT_DATKM)).Sum<Product_Detail>((Func<Product_Detail, double>) (s => Convert.ToDouble(s.SOLUONG))) : lstSelectProduct_Detail.Where<Product_Detail>((Func<Product_Detail, bool>) (e => !e.ISKHUYENMAI && (lstCTKM_YC.Count<KeyValuePair<string, int>>((Func<KeyValuePair<string, int>, bool>) (s => s.Key.Contains(e.ID_HANGHOA) && s.Value == 0)) > 0 || lstCTKM_YC.Count<KeyValuePair<string, int>>((Func<KeyValuePair<string, int>, bool>) (s => s.Key.Contains(e.ID_NHOMHANGHOA) && s.Value == 1)) > 0) && e.ID_DVT == itm.ID_DVT_DATKM)).Sum<Product_Detail>((Func<Product_Detail, double>) (s => Convert.ToDouble(s.SOLUONG - s.SOLUONGDALAY_KM)));
            }
            else
            {
              SumTien = lstSelectProduct_Detail.Where<Product_Detail>((Func<Product_Detail, bool>) (e => !e.ISKHUYENMAI)).Sum<Product_Detail>((Func<Product_Detail, double>) (s => Convert.ToDouble(s.TONGCONG)));
              if (!string.IsNullOrEmpty(itm.ID_DVT_DATKM))
                SumSoLuong = !bolConSoLuong ? lstSelectProduct_Detail.Where<Product_Detail>((Func<Product_Detail, bool>) (e => !e.ISKHUYENMAI && e.ID_DVT == itm.ID_DVT_DATKM)).Sum<Product_Detail>((Func<Product_Detail, double>) (s => Convert.ToDouble(s.SOLUONG))) : lstSelectProduct_Detail.Where<Product_Detail>((Func<Product_Detail, bool>) (e => !e.ISKHUYENMAI && e.ID_DVT == itm.ID_DVT_DATKM)).Sum<Product_Detail>((Func<Product_Detail, double>) (s => Convert.ToDouble(s.SOLUONG - s.SOLUONGDALAY_KM)));
            }
            if ((itm.SOLUONG_DATKM != 0.0 || itm.SOLUONG_DATKM_DEN != 0.0) && SumSoLuong != 0.0 && itm.SOLUONG_DATKM <= SumSoLuong && (itm.SOLUONG_DATKM_DEN == 0.0 || itm.SOLUONG_DATKM_DEN >= SumSoLuong) || (itm.TONGTIEN_DATKM != 0.0 || itm.TONGTIEN_DATKM != 0.0) && SumTien != 0.0 && itm.TONGTIEN_DATKM <= SumTien && (itm.TONGTIEN_DATKM_DEN == 0.0 || itm.TONGTIEN_DATKM_DEN >= SumTien))
            {
              List<dm_ChuongTrinhKhuyenMai_Tang> lstdm_ChuongTrinhKhuyenMai_Tang = await this._context.dm_ChuongTrinhKhuyenMai_Tang.Where<dm_ChuongTrinhKhuyenMai_Tang>((Expression<Func<dm_ChuongTrinhKhuyenMai_Tang, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == itm.ID)).ToListAsync<dm_ChuongTrinhKhuyenMai_Tang>();
              if (lstdm_ChuongTrinhKhuyenMai_Tang != null && lstdm_ChuongTrinhKhuyenMai_Tang.Count > 0)
              {
                string ID_KHO = lstProduct_Detail.Select<Product_Detail, string>((Func<Product_Detail, string>) (e => e.ID_KHO)).FirstOrDefault<string>();
                if (lstdm_ChuongTrinhKhuyenMai_Tang != null)
                {
                  int SLKM_SL = itm.SOLUONG_DATKM_DEN != 0.0 ? Convert.ToInt32(SumSoLuong) / Convert.ToInt32(itm.SOLUONG_DATKM_DEN) : (itm.SOLUONG_DATKM != 0.0 ? Convert.ToInt32(SumSoLuong) / Convert.ToInt32(itm.SOLUONG_DATKM) : 0);
                  int SLKM_TIEN = itm.TONGTIEN_DATKM_DEN != 0.0 ? Convert.ToInt32(SumTien) / Convert.ToInt32(itm.TONGTIEN_DATKM_DEN) : (itm.TONGTIEN_DATKM != 0.0 ? Convert.ToInt32(SumTien) / Convert.ToInt32(itm.TONGTIEN_DATKM) : 0);
                  if (bolBatBuoc)
                    SLKM_SL = SLKM_SL > PhanNguyen ? PhanNguyen : SLKM_SL;
                  foreach (dm_ChuongTrinhKhuyenMai_Tang trinhKhuyenMaiTang in lstdm_ChuongTrinhKhuyenMai_Tang)
                  {
                    dm_ChuongTrinhKhuyenMai_Tang CTKM_Tang = trinhKhuyenMaiTang;
                    dm_HangHoa_Kho HangHoaKho = await this._context.dm_HangHoa_Kho.FirstOrDefaultAsync<dm_HangHoa_Kho>((Expression<Func<dm_HangHoa_Kho, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID_HANGHOA == CTKM_Tang.ID_HANGHOA && e.ID_KHO == ID_KHO));
                    view_dm_HangHoa HangHoa = await this._context.view_dm_HangHoa.FirstOrDefaultAsync<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == CTKM_Tang.ID_HANGHOA));
                    if (HangHoaKho != null && HangHoa != null)
                    {
                      Product_Detail newProduct_Detail = new Product_Detail();
                      newProduct_Detail.STT = lstSelectProduct_Detail.Max<Product_Detail>((Func<Product_Detail, int>) (e => e.STT));
                      newProduct_Detail.ID = Guid.NewGuid().ToString();
                      newProduct_Detail.NAME = HangHoa.NAME;
                      newProduct_Detail.MA = HangHoa.MA;
                      newProduct_Detail.ID_HANGHOA = CTKM_Tang.ID_HANGHOA;
                      newProduct_Detail.ID_HANGHOAKHO = HangHoaKho.ID;
                      newProduct_Detail.DONGIA = 0.0;
                      newProduct_Detail.ID_DVT = CTKM_Tang.ID_DVT;
                      newProduct_Detail.SOLUONG = SLKM_SL <= SLKM_TIEN ? (itm.ISTINHLUYTUYEN ? (double) SLKM_TIEN : 1.0) * CTKM_Tang.SOLUONG : (itm.ISTINHLUYTUYEN ? (double) SLKM_SL : 1.0) * CTKM_Tang.SOLUONG;
                      newProduct_Detail.CHIETKHAU = 0.0;
                      newProduct_Detail.TONGTIENGIAMGIA = 0.0;
                      newProduct_Detail.THANHTIEN = 0.0;
                      newProduct_Detail.THUESUAT = 0.0;
                      newProduct_Detail.TONGTIENVAT = 0.0;
                      newProduct_Detail.TONGCONG = 0.0;
                      if (CTKM_Tang.SOTIEN > 0.0)
                      {
                        newProduct_Detail.TONGTIENGIAMGIA = CTKM_Tang.SOTIEN;
                        newProduct_Detail.TYPE = "TONGTIENGIAMGIA";
                        dm_ThueSuat clsdm_ThueSuat = new dm_ThueSuat();
                        if (!string.IsNullOrEmpty(HangHoa.ID_THUESUAT))
                        {
                          dm_ThueSuat dmThueSuat = await this._context.dm_ThueSuat.FirstOrDefaultAsync<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == HangHoa.LOC_ID && e.ID == HangHoa.ID_THUESUAT));
                          clsdm_ThueSuat = dmThueSuat ?? new dm_ThueSuat();
                          dmThueSuat = (dm_ThueSuat) null;
                        }
                        API.TinhTong(newProduct_Detail, "", lstProduct_Detail, clsdm_ThueSuat);
                        if (MUCTHUE != 0.0)
                          SOTIENTHUE_KM += newProduct_Detail.TONGTIENGIAMGIA * MUCTHUE / 100.0;
                        clsdm_ThueSuat = (dm_ThueSuat) null;
                      }
                      newProduct_Detail.ID_KHO = ID_KHO;
                      newProduct_Detail.ISDALAYKHUYENMAI = true;
                      newProduct_Detail.ISKHUYENMAI = true;
                      newProduct_Detail.ID_KHUYENMAI = itm.ID;
                      if (HangHoa != null && HangHoa.ID_DVT == newProduct_Detail.ID_DVT)
                      {
                        newProduct_Detail.NAME_DVT = HangHoa.NAME_DVT;
                        if (!string.IsNullOrEmpty(HangHoa.ID_DVT_QD))
                          newProduct_Detail.TYLE_QD = HangHoa.TYLE_QD;
                        else if (HangHoa.LOAIHANGHOA == 2.ToString())
                          newProduct_Detail.TYLE_QD = 0.0;
                        else
                          newProduct_Detail.TYLE_QD = 1.0;
                        if (newProduct_Detail.SOLUONG != 0.0 && HangHoa != null && HangHoa.MUCTHUE != 0.0)
                          SOTIENTHUE_KM += newProduct_Detail.SOLUONG * HangHoa.GIA01 * HangHoa.MUCTHUE / 100.0;
                      }
                      else if (HangHoa != null && HangHoa.ID_DVT_QD == newProduct_Detail.ID_DVT)
                      {
                        if (!string.IsNullOrEmpty(HangHoa.ID_DVT_QD))
                        {
                          newProduct_Detail.NAME_DVT = HangHoa.NAME_DVT_QD;
                          newProduct_Detail.TYLE_QD = 1.0;
                        }
                        if (newProduct_Detail.SOLUONG != 0.0 && HangHoa != null && HangHoa.MUCTHUE != 0.0)
                          SOTIENTHUE_KM += newProduct_Detail.SOLUONG * newProduct_Detail.TYLE_QD * HangHoa.GIA01_QD * HangHoa.MUCTHUE / 100.0;
                      }
                      else
                        return (IActionResult) this.Ok((object) new ApiResponse()
                        {
                          Success = false,
                          Message = $"Không tìm thấy thông tin sản phẩm với đơn vị tính {newProduct_Detail.ID_DVT} Kiểm tra CTKM{itm.NAME}",
                          Data = (object) null
                        });
                      newProduct_Detail.TONGSOLUONG = newProduct_Detail.TYLE_QD * newProduct_Detail.SOLUONG;
                      if (HangHoa != null && HangHoa.LOAIHANGHOA == 1.ToString())
                      {
                        newProduct_Detail.ID_KHUYENMAI = newProduct_Detail.ID_HANGHOA;
                        SP_Parameter objParameter = new SP_Parameter();
                        objParameter.LOC_ID = itm.LOC_ID;
                        objParameter.ID_KHO = newProduct_Detail.ID_KHO;
                        objParameter.ID_COMBO = newProduct_Detail.ID_HANGHOA;
                        ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
                        actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachSanPhamKho_Combo(objParameter);
                        if (actionResult is OkObjectResult okResult3)
                        {
                          ApiResponse ApiResponse = okResult3.Value as ApiResponse;
                          if (ApiResponse != null && ApiResponse.Data != null)
                          {
                            if (ApiResponse.Data is List<Product_Detail> lst_ChiTiet)
                            {
                              foreach (Product_Detail ChiTiet in lst_ChiTiet)
                              {
                                ChiTiet.STT = newProduct_Detail.STT;
                                ChiTiet.ID = Guid.NewGuid().ToString();
                                ChiTiet.ID_DVT = ChiTiet.ID_DVT_COMBO;
                                ChiTiet.SOLUONG = newProduct_Detail.SOLUONG * ChiTiet.QTY_COMBO;
                                ChiTiet.TYLE_QD = ChiTiet.TYLE_QD_COMBO;
                                ChiTiet.TONGSOLUONG = newProduct_Detail.SOLUONG * ChiTiet.QTY_TOTAL_COMBO;
                                ChiTiet.DONGIA = 0.0;
                                ChiTiet.CHIETKHAU = 0.0;
                                ChiTiet.TONGTIENGIAMGIA = 0.0;
                                ChiTiet.THANHTIEN = 0.0;
                                ChiTiet.THUESUAT = 0.0;
                                ChiTiet.TONGTIENVAT = 0.0;
                                ChiTiet.TONGCONG = 0.0;
                                ChiTiet.ISKHUYENMAI = true;
                                ChiTiet.ID_KHUYENMAI = itm.ID;
                                ChiTiet.ISCOMBO = true;
                                ChiTiet.ID_COMBO = newProduct_Detail.ID_HANGHOA;
                                lstProduct_Detail.Add(ChiTiet);
                              }
                            }
                            lst_ChiTiet = (List<Product_Detail>) null;
                          }
                          ApiResponse = (ApiResponse) null;
                        }
                        objParameter = (SP_Parameter) null;
                      }
                      lstProduct_Detail.Add(newProduct_Detail);
                      newProduct_Detail = (Product_Detail) null;
                    }
                    HangHoaKho = (dm_HangHoa_Kho) null;
                  }
                  if (lstChuongTrinhKhuyenMai_YeuCau != null && lstChuongTrinhKhuyenMai_YeuCau.Count > 0)
                  {
                    double SoLuongYeuCau = (itm.SOLUONG_DATKM_DEN != 0.0 ? itm.SOLUONG_DATKM_DEN : itm.SOLUONG_DATKM) * (itm.ISTINHLUYTUYEN ? (double) SLKM_SL : 1.0);
                    foreach (dm_ChuongTrinhKhuyenMai_YeuCau trinhKhuyenMaiYeuCau in (IEnumerable<dm_ChuongTrinhKhuyenMai_YeuCau>) lstChuongTrinhKhuyenMai_YeuCau.OrderByDescending<dm_ChuongTrinhKhuyenMai_YeuCau, bool>((Func<dm_ChuongTrinhKhuyenMai_YeuCau, bool>) (e => e.ISBATBUOC)))
                    {
                      dm_ChuongTrinhKhuyenMai_YeuCau CTKM_YC = trinhKhuyenMaiYeuCau;
                      foreach (Product_Detail productDetail in lstProduct_Detail_Tam.Where<Product_Detail>((Func<Product_Detail, bool>) (e => (!itm.ISTONGHOADON || itm.ISTONGHOADON && !e.ISDALAYKHUYENMAI) && (e.ID_HANGHOA == CTKM_YC.ID_HANGHOA && CTKM_YC.HINHTHUC == 0 || e.ID_NHOMHANGHOA == CTKM_YC.ID_HANGHOA && CTKM_YC.HINHTHUC == 1) && e.ID_DVT == CTKM_YC.ID_DVT && e.SOLUONG - e.SOLUONGDALAY_KM > 0.0)).ToList<Product_Detail>())
                      {
                        Product_Detail ChiTiet = productDetail;
                        if (SoLuongYeuCau != 0.0)
                        {
                          if (SoLuongYeuCau - (ChiTiet.SOLUONG - ChiTiet.SOLUONGDALAY_KM) > 0.0)
                          {
                            ChiTiet.SOLUONGDALAY_KM += ChiTiet.SOLUONG - ChiTiet.SOLUONGDALAY_KM;
                            SoLuongYeuCau -= ChiTiet.SOLUONGDALAY_KM;
                          }
                          else
                          {
                            ChiTiet.SOLUONGDALAY_KM += SoLuongYeuCau;
                            SoLuongYeuCau = 0.0;
                          }
                          ChiTiet = (Product_Detail) null;
                        }
                        else
                          break;
                      }
                    }
                  }
                  ++intCoLayKhuyenMai;
                }
              }
              if (itm.CHIETKHAU > 0.0 || itm.TIENGIAM > 0.0)
              {
                if (!itm.ISTONGHOADON)
                {
                  foreach (Product_Detail productDetail in lstProduct_Detail.Where<Product_Detail>((Func<Product_Detail, bool>) (s => lstSelectProduct_Detail.Where<Product_Detail>((Func<Product_Detail, bool>) (e => e.ID == s.ID)).Count<Product_Detail>() > 0)))
                  {
                    Product_Detail ChiTiet = productDetail;
                    double SLKM_SL = itm.SOLUONG_DATKM_DEN != 0.0 ? ChiTiet.SOLUONG / itm.SOLUONG_DATKM_DEN : (itm.SOLUONG_DATKM != 0.0 ? ChiTiet.SOLUONG / itm.SOLUONG_DATKM : 0.0);
                    dm_ThueSuat clsdm_ThueSuat = new dm_ThueSuat();
                    if (!string.IsNullOrEmpty(ChiTiet.ID_THUESUAT))
                    {
                      dm_ThueSuat dmThueSuat = await this._context.dm_ThueSuat.FirstOrDefaultAsync<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == ChiTiet.LOC_ID && e.ID == ChiTiet.ID_THUESUAT));
                      clsdm_ThueSuat = dmThueSuat ?? new dm_ThueSuat();
                      dmThueSuat = (dm_ThueSuat) null;
                    }
                    if (itm.CHIETKHAU > 0.0)
                    {
                      ChiTiet.CHIETKHAU = itm.CHIETKHAU;
                      ChiTiet.TYPE = "CHIETKHAU";
                      API.TinhTong(ChiTiet, "", lstProduct_Detail, clsdm_ThueSuat);
                      ChiTiet.SOLUONGDALAYKHUYENMAI = ChiTiet.TONGSOLUONG;
                      ChiTiet.ID_KHUYENMAI = itm.ID;
                      view_dm_HangHoa objHangHoa = await this._context.view_dm_HangHoa.FirstOrDefaultAsync<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == ChiTiet.LOC_ID && e.ID == ChiTiet.ID_HANGHOA));
                      if (objHangHoa != null && objHangHoa.MUCTHUE != 0.0)
                        SOTIENTHUE_KM += ChiTiet.TONGTIENGIAMGIA * objHangHoa.MUCTHUE / 100.0;
                      objHangHoa = (view_dm_HangHoa) null;
                    }
                    else if (itm.TIENGIAM > 0.0)
                    {
                      ChiTiet.TONGTIENGIAMGIA = itm.TIENGIAM * (itm.ISTINHLUYTUYEN ? SLKM_SL : 1.0);
                      ChiTiet.TYPE = "TONGTIENGIAMGIA";
                      API.TinhTong(ChiTiet, "", lstProduct_Detail, clsdm_ThueSuat);
                      ChiTiet.SOLUONGDALAYKHUYENMAI = ChiTiet.TONGSOLUONG;
                      ChiTiet.ID_KHUYENMAI = itm.ID;
                      view_dm_HangHoa objHangHoa = await this._context.view_dm_HangHoa.FirstOrDefaultAsync<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == ChiTiet.LOC_ID && e.ID == ChiTiet.ID_HANGHOA));
                      if (objHangHoa != null && objHangHoa.MUCTHUE != 0.0)
                        SOTIENTHUE_KM += ChiTiet.TONGTIENGIAMGIA * objHangHoa.MUCTHUE / 100.0;
                      objHangHoa = (view_dm_HangHoa) null;
                    }
                    clsdm_ThueSuat = (dm_ThueSuat) null;
                  }
                }
                else
                {
                  string ID_KHO = lstProduct_Detail.Select<Product_Detail, string>((Func<Product_Detail, string>) (e => e.ID_KHO)).FirstOrDefault<string>();
                  view_dm_HangHoa HangHoa = await this._context.view_dm_HangHoa.FirstOrDefaultAsync<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.MA == API.GTBH));
                  if (HangHoa != null)
                  {
                    dm_HangHoa_Kho HangHoaKho = await this._context.dm_HangHoa_Kho.FirstOrDefaultAsync<dm_HangHoa_Kho>((Expression<Func<dm_HangHoa_Kho, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID_HANGHOA == HangHoa.ID && e.ID_KHO == ID_KHO));
                    if (HangHoaKho != null)
                    {
                      Product_Detail newProduct_Detail = new Product_Detail();
                      newProduct_Detail.STT = lstSelectProduct_Detail.Max<Product_Detail>((Func<Product_Detail, int>) (e => e.STT)) + 1;
                      newProduct_Detail.ID = Guid.NewGuid().ToString();
                      newProduct_Detail.NAME = HangHoa.NAME;
                      newProduct_Detail.MA = HangHoa.MA;
                      newProduct_Detail.ID_HANGHOA = HangHoaKho.ID_HANGHOA;
                      newProduct_Detail.ID_HANGHOAKHO = HangHoaKho.ID;
                      newProduct_Detail.DONGIA = 0.0;
                      newProduct_Detail.ID_DVT = HangHoa.ID_DVT;
                      newProduct_Detail.NAME_DVT = HangHoa.NAME_DVT;
                      newProduct_Detail.SOLUONG = 0.0;
                      dm_ThueSuat clsdm_ThueSuat = new dm_ThueSuat();
                      if (!string.IsNullOrEmpty(HangHoa.ID_THUESUAT))
                      {
                        dm_ThueSuat dmThueSuat = await this._context.dm_ThueSuat.FirstOrDefaultAsync<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == HangHoa.ID_THUESUAT));
                        clsdm_ThueSuat = dmThueSuat ?? new dm_ThueSuat();
                        dmThueSuat = (dm_ThueSuat) null;
                      }
                      if (itm.CHIETKHAU > 0.0)
                      {
                        newProduct_Detail.CHIETKHAU = itm.CHIETKHAU;
                        newProduct_Detail.TONGTIENGIAMGIA = SumTien * newProduct_Detail.CHIETKHAU / 100.0;
                        newProduct_Detail.TYPE = "TONGTIENGIAMGIA";
                        API.TinhTong(newProduct_Detail, "", lstProduct_Detail, clsdm_ThueSuat);
                        newProduct_Detail.ISDALAYKHUYENMAI = true;
                        newProduct_Detail.SOLUONGDALAYKHUYENMAI = newProduct_Detail.TONGSOLUONG;
                        newProduct_Detail.ID_KHUYENMAI = itm.ID;
                        if (MUCTHUE != 0.0)
                          SOTIENTHUE_KM += newProduct_Detail.TONGTIENGIAMGIA * MUCTHUE / 100.0;
                      }
                      else if (itm.TIENGIAM > 0.0)
                      {
                        newProduct_Detail.TONGTIENGIAMGIA = itm.TIENGIAM * (itm.ISTINHLUYTUYEN ? (double) PhanNguyen : 1.0);
                        newProduct_Detail.TYPE = "TONGTIENGIAMGIA";
                        API.TinhTong(newProduct_Detail, "", lstProduct_Detail, clsdm_ThueSuat);
                        newProduct_Detail.ISDALAYKHUYENMAI = true;
                        newProduct_Detail.SOLUONGDALAYKHUYENMAI = newProduct_Detail.TONGSOLUONG;
                        newProduct_Detail.ID_KHUYENMAI = itm.ID;
                        if (MUCTHUE != 0.0)
                          SOTIENTHUE_KM += newProduct_Detail.TONGTIENGIAMGIA * MUCTHUE / 100.0;
                      }
                      newProduct_Detail.ID_KHO = ID_KHO;
                      newProduct_Detail.ISDALAYKHUYENMAI = true;
                      newProduct_Detail.ISKHUYENMAI = true;
                      newProduct_Detail.ID_KHUYENMAI = itm.ID;
                      lstProduct_Detail.Add(newProduct_Detail);
                      foreach (Product_Detail ChiTiet in lstProduct_Detail)
                        ChiTiet.ISDALAYKHUYENMAI = true;
                      newProduct_Detail = (Product_Detail) null;
                      clsdm_ThueSuat = (dm_ThueSuat) null;
                    }
                    HangHoaKho = (dm_HangHoa_Kho) null;
                  }
                }
                ++intCoLayKhuyenMai;
              }
              lstdm_ChuongTrinhKhuyenMai_Tang = (List<dm_ChuongTrinhKhuyenMai_Tang>) null;
            }
          }
        }
        if (intCoLayKhuyenMai > 0)
          lstDanhSachDaLayKhuyenMai.Add(itm.MA);
        input = (string) null;
        lstChuongTrinhKhuyenMai_YeuCau = (List<dm_ChuongTrinhKhuyenMai_YeuCau>) null;
      }
      if (SOTIENTHUE_KM != 0.0)
      {
        string ID_KHO = lstProduct_Detail.Select<Product_Detail, string>((Func<Product_Detail, string>) (e => e.ID_KHO)).FirstOrDefault<string>();
        view_dm_HangHoa HangHoa = await this._context.view_dm_HangHoa.FirstOrDefaultAsync<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == API.TINHTHUE_KM));
        if (HangHoa != null)
        {
          dm_HangHoa_Kho HangHoaKho = await this._context.dm_HangHoa_Kho.FirstOrDefaultAsync<dm_HangHoa_Kho>((Expression<Func<dm_HangHoa_Kho, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_HANGHOA == HangHoa.ID));
          if (HangHoaKho != null)
          {
            Product_Detail newProduct_Detail = new Product_Detail();
            newProduct_Detail.STT = lstProduct_Detail.Max<Product_Detail>((Func<Product_Detail, int>) (e => e.STT));
            newProduct_Detail.ID = Guid.NewGuid().ToString();
            newProduct_Detail.NAME = HangHoa.NAME;
            newProduct_Detail.MA = HangHoa.MA;
            newProduct_Detail.ID_HANGHOA = HangHoaKho.ID_HANGHOA;
            newProduct_Detail.ID_HANGHOAKHO = HangHoaKho.ID;
            newProduct_Detail.DONGIA = 0.0;
            newProduct_Detail.ID_DVT = HangHoa.ID_DVT;
            newProduct_Detail.NAME_DVT = HangHoa.NAME_DVT;
            newProduct_Detail.SOLUONG = 0.0;
            dm_ThueSuat clsdm_ThueSuat = new dm_ThueSuat();
            if (!string.IsNullOrEmpty(HangHoa.ID_THUESUAT))
            {
              dm_ThueSuat dmThueSuat = await this._context.dm_ThueSuat.FirstOrDefaultAsync<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == HangHoa.ID_THUESUAT));
              clsdm_ThueSuat = dmThueSuat ?? new dm_ThueSuat();
              dmThueSuat = (dm_ThueSuat) null;
            }
            newProduct_Detail.TONGTIENGIAMGIA = -1.0 * Math.Ceiling(SOTIENTHUE_KM);
            newProduct_Detail.TYPE = "TONGTIENGIAMGIA";
            API.TinhTong(newProduct_Detail, "", lstProduct_Detail, clsdm_ThueSuat);
            newProduct_Detail.ISKHUYENMAI = true;
            newProduct_Detail.GHICHU = "";
            lstProduct_Detail.Add(newProduct_Detail);
            newProduct_Detail = (Product_Detail) null;
            clsdm_ThueSuat = (dm_ThueSuat) null;
          }
          HangHoaKho = (dm_HangHoa_Kho) null;
        }
        ID_KHO = (string) null;
      }
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) lstProduct_Detail.OrderBy<Product_Detail, int>((Func<Product_Detail, int>) (x => x.STT)).ThenBy<Product_Detail, bool>((Func<Product_Detail, bool>) (x => x.ISKHUYENMAI))
      });
    }
    catch (Exception ex)
    {
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) lstProduct_Detail
      });
    }
  }
}
