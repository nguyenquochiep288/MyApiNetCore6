// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.OutputController
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace MyApiNetCore6.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OutputController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;
  private string strTable = "ct_PhieuXuat";

  public OutputController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetOutput(string LOC_ID)
  {
    try
    {
      List<ct_PhieuXuat> lstValue = await this._context.ct_PhieuXuat.Where<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == LOC_ID)).ToListAsync<ct_PhieuXuat>();
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
  public async Task<IActionResult> GetOutput(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      List<ct_PhieuXuat> lstValue = await this._context.ct_PhieuXuat.Where<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == LOC_ID)).Where<ct_PhieuXuat>(KeyWhere, (object) ValuesSearch).ToListAsync<ct_PhieuXuat>();
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
  public async Task<IActionResult> GetOutput(
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
      List<ct_PhieuXuat> lstValue = await this._context.ct_PhieuXuat.Where<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == LOC_ID && e.NGAYLAP.Date >= FROMDATE.Date && e.NGAYLAP.Date <= TODATE.Date)).Where<ct_PhieuXuat>(KeyWhere, (object) ValuesSearch).ToListAsync<ct_PhieuXuat>();
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
  public async Task<IActionResult> GetOutput(string LOC_ID, string ID)
  {
    try
    {
      ct_PhieuXuat Output = await this._context.ct_PhieuXuat.FirstOrDefaultAsync<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Output == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      v_ct_PhieuXuat ct_PhieuXuat = new v_ct_PhieuXuat();
      SP_Parameter SP_Parameter = new SP_Parameter();
      SP_Parameter.LOC_ID = LOC_ID;
      SP_Parameter.ID_PHIEUXUAT = ID;
      ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
      IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuXuat(SP_Parameter);
      if (actionResult is OkObjectResult okResult2)
      {
        ApiResponse ApiResponse = okResult2.Value as ApiResponse;
        if (ApiResponse != null && ApiResponse.Data != null)
          ct_PhieuXuat = (ApiResponse.Data is List<v_ct_PhieuXuat> data ? data.FirstOrDefault<v_ct_PhieuXuat>() : (v_ct_PhieuXuat) null) ?? new v_ct_PhieuXuat();
        ApiResponse = (ApiResponse) null;
      }
      SP_Parameter = new SP_Parameter();
      SP_Parameter.ID_PHIEUXUAT = ID;
      ct_PhieuXuat.lstct_PhieuXuat_ChiTiet = new List<v_ct_PhieuXuat_ChiTiet>();
      ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
      actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuXuat_Chitiet(SP_Parameter);
      if (actionResult is OkObjectResult okResult2)
      {
        ApiResponse ApiResponse = okResult2.Value as ApiResponse;
        if (ApiResponse != null && ApiResponse.Data != null)
        {
          if (ApiResponse.Data is List<v_ct_PhieuXuat_ChiTiet> lst_ChiTiet)
            ct_PhieuXuat.lstct_PhieuXuat_ChiTiet.AddRange((IEnumerable<v_ct_PhieuXuat_ChiTiet>) lst_ChiTiet);
          lst_ChiTiet = (List<v_ct_PhieuXuat_ChiTiet>) null;
        }
        ApiResponse = (ApiResponse) null;
      }
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) ct_PhieuXuat
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
  public async Task<IActionResult> PutOutput(string LOC_ID, string ID, [FromBody] v_ct_PhieuXuat Output)
  {
    try
    {
      if (!this.OutputExistsID(Output.LOC_ID, Output.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {Output.LOC_ID}-{Output.ID} dữ liệu!",
          Data = (object) ""
        });
      string StrHetSoLuong = "";
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        List<ct_PhieuXuat_ChiTiet> lstPhieuNhap_ChiTiet = await this._context.ct_PhieuXuat_ChiTiet.Where<ct_PhieuXuat_ChiTiet>((Expression<Func<ct_PhieuXuat_ChiTiet, bool>>) (e => e.LOC_ID == Output.LOC_ID && e.ID_PHIEUXUAT == Output.ID)).ToListAsync<ct_PhieuXuat_ChiTiet>();
        if (lstPhieuNhap_ChiTiet != null)
        {
          foreach (ct_PhieuXuat_ChiTiet phieuXuatChiTiet in lstPhieuNhap_ChiTiet)
          {
            ct_PhieuXuat_ChiTiet itm = phieuXuatChiTiet;
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
              objdm_HangHoa_Kho.QTY += itm.TONGSOLUONG;
              this._context.Entry<dm_HangHoa_Kho>(objdm_HangHoa_Kho).State = EntityState.Modified;
              v_ct_PhieuXuat_ChiTiet chkPhieuNhap_ChiTiet = Output.lstct_PhieuXuat_ChiTiet.Where<v_ct_PhieuXuat_ChiTiet>((Func<v_ct_PhieuXuat_ChiTiet, bool>) (e => e.ID == itm.ID)).FirstOrDefault<v_ct_PhieuXuat_ChiTiet>();
              if (chkPhieuNhap_ChiTiet != null)
              {
                chkPhieuNhap_ChiTiet.ISEDIT = true;
                chkPhieuNhap_ChiTiet.ID_PHIEUXUAT = Output.ID;
                this._context.Entry<dm_HangHoa_Kho>(objdm_HangHoa_Kho).State = EntityState.Modified;
              }
              else
                this._context.ct_PhieuXuat_ChiTiet.Remove(itm);
              objdm_HangHoa_Kho = (dm_HangHoa_Kho) null;
              chkPhieuNhap_ChiTiet = (v_ct_PhieuXuat_ChiTiet) null;
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
        if (Output.lstct_PhieuXuat_ChiTiet != null)
        {
          foreach (v_ct_PhieuXuat_ChiTiet phieuXuatChiTiet in Output.lstct_PhieuXuat_ChiTiet)
          {
            v_ct_PhieuXuat_ChiTiet itm = phieuXuatChiTiet;
            if (!string.IsNullOrEmpty(itm.ID_THUESUAT))
            {
              dm_ThueSuat objVAT = this._context.dm_ThueSuat.FirstOrDefault<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO && e.ID == itm.ID_THUESUAT));
              itm.THUESUAT = objVAT != null ? objVAT.THUESUAT : itm.THUESUAT;
              objVAT = (dm_ThueSuat) null;
            }
            itm.THANHTIEN = itm.SOLUONG * itm.DONGIA - itm.TONGTIENGIAMGIA;
            itm.TONGCONG = itm.THANHTIEN + itm.TONGTIENVAT;
            itm.ID_PHIEUXUAT = Output.ID;
            dm_HangHoa_Kho objdm_HangHoa_Kho = this._context.dm_HangHoa_Kho.FromSqlRaw<dm_HangHoa_Kho>("\r\n                                        SELECT *\r\n                                        FROM dm_HangHoa_Kho WITH (UPDLOCK, ROWLOCK)\r\n                                        WHERE LOC_ID = @loc\r\n                                          AND ID = @id\r\n                                          AND ID_KHO = @kho\r\n                                    ", (object) new SqlParameter("@loc", (object) itm.LOC_ID), (object) new SqlParameter("@id", (object) itm.ID_HANGHOAKHO), (object) new SqlParameter("@kho", (object) Output.ID_KHO)).AsTracking<dm_HangHoa_Kho>().FirstOrDefault<dm_HangHoa_Kho>();
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
                this._context.ct_PhieuXuat_ChiTiet.Add((ct_PhieuXuat_ChiTiet) itm);
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
          Output.TONGTHANHTIEN = Math.Round(Output.lstct_PhieuXuat_ChiTiet.Sum<v_ct_PhieuXuat_ChiTiet>((Func<v_ct_PhieuXuat_ChiTiet, double>) (s => s.THANHTIEN)), 0);
          Output.TONGTIENGIAMGIA = Math.Round(Output.lstct_PhieuXuat_ChiTiet.Sum<v_ct_PhieuXuat_ChiTiet>((Func<v_ct_PhieuXuat_ChiTiet, double>) (s => s.TONGTIENGIAMGIA)), 0);
          Output.TONGTIENVAT = Math.Round(Output.lstct_PhieuXuat_ChiTiet.Sum<v_ct_PhieuXuat_ChiTiet>((Func<v_ct_PhieuXuat_ChiTiet, double>) (s => s.TONGTIENVAT)), 0);
          Output.TONGTIEN = Math.Round(Output.lstct_PhieuXuat_ChiTiet.Sum<v_ct_PhieuXuat_ChiTiet>((Func<v_ct_PhieuXuat_ChiTiet, double>) (s => s.TONGCONG)), 0);
          if (!string.IsNullOrEmpty(StrHetSoLuong))
            return (IActionResult) this.Ok((object) new ApiResponse()
            {
              Success = false,
              Message = StrHetSoLuong,
              Data = (object) ""
            });
        }
        this._context.Entry<v_ct_PhieuXuat>(Output).State = EntityState.Modified;
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num1 = await this._context.SaveChangesAsync();
        lstPhieuNhap_ChiTiet = (List<ct_PhieuXuat_ChiTiet>) null;
        auditLog = (AuditLogController) null;
        transaction.Commit();
        v_ct_PhieuXuat ct_PhieuXuat = new v_ct_PhieuXuat();
        ct_PhieuXuat.lstct_PhieuXuat_ChiTiet = new List<v_ct_PhieuXuat_ChiTiet>();
        SP_Parameter SP_Parameter = new SP_Parameter();
        SP_Parameter.ID_PHIEUXUAT = Output.ID;
        ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
        IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuXuat(SP_Parameter);
        if (actionResult is OkObjectResult okResult)
        {
          ApiResponse ApiResponse = okResult.Value as ApiResponse;
          if (ApiResponse != null && ApiResponse.Data != null)
          {
            List<v_ct_PhieuXuat> lstPhieuDatHang = ApiResponse.Data as List<v_ct_PhieuXuat>;
            if (lstPhieuDatHang != null && lstPhieuDatHang.Count<v_ct_PhieuXuat>() > 0)
              ct_PhieuXuat = lstPhieuDatHang.FirstOrDefault<v_ct_PhieuXuat>() ?? new v_ct_PhieuXuat();
            lstPhieuDatHang = (List<v_ct_PhieuXuat>) null;
          }
          ApiResponse = (ApiResponse) null;
        }
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) ct_PhieuXuat
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
  public async Task<ActionResult<ct_PhieuXuat>> PostOutput([FromBody] v_ct_PhieuXuat Output)
  {
    try
    {
      if (this.OutputExistsID(Output.LOC_ID, Output.ID))
        return (ActionResult<ct_PhieuXuat>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Output.LOC_ID}-{Output.ID} trong dữ liệu!",
          Data = (object) "",
          CheckValue = true
        });
      ct_PhieuXuat objPhieuNhap = await this._context.ct_PhieuXuat.FirstOrDefaultAsync<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == Output.LOC_ID && e.MAPHIEU == Output.MAPHIEU));
      if (objPhieuNhap != null)
        return (ActionResult<ct_PhieuXuat>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Output.LOC_ID}-{Output.MAPHIEU} trong dữ liệu!",
          Data = (object) "",
          CheckValue = true
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        if (Output.lstct_PhieuXuat_ChiTiet != null)
        {
          string StrHetSoLuong = "";
          foreach (v_ct_PhieuXuat_ChiTiet phieuXuatChiTiet in Output.lstct_PhieuXuat_ChiTiet)
          {
            v_ct_PhieuXuat_ChiTiet itm = phieuXuatChiTiet;
            if (!string.IsNullOrEmpty(itm.ID_THUESUAT))
            {
              dm_ThueSuat objVAT = this._context.dm_ThueSuat.FirstOrDefault<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO && e.ID == itm.ID_THUESUAT));
              itm.THUESUAT = objVAT != null ? objVAT.THUESUAT : itm.THUESUAT;
              objVAT = (dm_ThueSuat) null;
            }
            itm.THANHTIEN = itm.SOLUONG * itm.DONGIA - itm.TONGTIENGIAMGIA;
            itm.TONGCONG = itm.THANHTIEN + itm.TONGTIENVAT;
            dm_HangHoa_Kho objdm_HangHoa_Kho = await this._context.dm_HangHoa_Kho.FirstOrDefaultAsync<dm_HangHoa_Kho>((Expression<Func<dm_HangHoa_Kho, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO && e.ID_KHO == Output.ID_KHO));
            if (objdm_HangHoa_Kho != null)
            {
              view_dm_HangHoa objdm_HangHoa = this._context.view_dm_HangHoa.FirstOrDefault<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOA));
              itm.TONGSOLUONG = itm.SOLUONG * itm.TYLE_QD;
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
              ct_PhieuXuat_ChiTiet objct_PhieuXuat_ChiTiet = await this._context.ct_PhieuXuat_ChiTiet.FirstOrDefaultAsync<ct_PhieuXuat_ChiTiet>((Expression<Func<ct_PhieuXuat_ChiTiet, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID));
              if (objct_PhieuXuat_ChiTiet != null)
                itm.ID = Guid.NewGuid().ToString();
              itm.LOC_ID = Output.LOC_ID;
              itm.ID_PHIEUXUAT = Output.ID;
              this._context.ct_PhieuXuat_ChiTiet.Add((ct_PhieuXuat_ChiTiet) itm);
              objdm_HangHoa_Kho = (dm_HangHoa_Kho) null;
              objct_PhieuXuat_ChiTiet = (ct_PhieuXuat_ChiTiet) null;
            }
            else
              return (ActionResult<ct_PhieuXuat>) (ActionResult) this.Ok((object) new ApiResponse()
              {
                Success = false,
                Message = ("Không tìm thấy sản phẩm kho!" + itm.ID_HANGHOAKHO),
                Data = (object) ""
              });
          }
          if (!string.IsNullOrEmpty(StrHetSoLuong))
            return (ActionResult<ct_PhieuXuat>) (ActionResult) this.Ok((object) new ApiResponse()
            {
              Success = false,
              Message = StrHetSoLuong,
              Data = (object) ""
            });
          Output.TONGTHANHTIEN = Math.Round(Output.lstct_PhieuXuat_ChiTiet.Sum<v_ct_PhieuXuat_ChiTiet>((Func<v_ct_PhieuXuat_ChiTiet, double>) (s => s.THANHTIEN)), 0);
          Output.TONGTIENGIAMGIA = Math.Round(Output.lstct_PhieuXuat_ChiTiet.Sum<v_ct_PhieuXuat_ChiTiet>((Func<v_ct_PhieuXuat_ChiTiet, double>) (s => s.TONGTIENGIAMGIA)), 0);
          Output.TONGTIENVAT = Math.Round(Output.lstct_PhieuXuat_ChiTiet.Sum<v_ct_PhieuXuat_ChiTiet>((Func<v_ct_PhieuXuat_ChiTiet, double>) (s => s.TONGTIENVAT)), 0);
          Output.TONGTIEN = Math.Round(Output.lstct_PhieuXuat_ChiTiet.Sum<v_ct_PhieuXuat_ChiTiet>((Func<v_ct_PhieuXuat_ChiTiet, double>) (s => s.TONGCONG)), 0);
          StrHetSoLuong = (string) null;
        }
        bool bolCheckMA = false;
        while (!bolCheckMA)
        {
          ct_PhieuXuat check = this._context.ct_PhieuXuat.Where<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == Output.LOC_ID && e.MAPHIEU == Output.MAPHIEU)).FirstOrDefault<ct_PhieuXuat>();
          if (check != null)
            Output.MAPHIEU = API.GetMaPhieu(nameof (Output), Output.NGAYLAP, Output.SOPHIEU);
          else
            bolCheckMA = true;
          check = (ct_PhieuXuat) null;
        }
        this._context.ct_PhieuXuat.Add((ct_PhieuXuat) Output);
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num1 = await this._context.SaveChangesAsync();
        auditLog = (AuditLogController) null;
        transaction.Commit();
        List<ct_PhieuXuat> lstPhieuDatHangCheck = await this._context.ct_PhieuXuat.Where<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == Output.LOC_ID && e.MAPHIEU == Output.MAPHIEU)).OrderByDescending<ct_PhieuXuat, DateTime>((Expression<Func<ct_PhieuXuat, DateTime>>) (e => e.NGAYLAP)).ToListAsync<ct_PhieuXuat>();
        if (lstPhieuDatHangCheck != null && lstPhieuDatHangCheck.Count<ct_PhieuXuat>() > 1 && lstPhieuDatHangCheck.FirstOrDefault<ct_PhieuXuat>().ID == Output.ID)
        {
          int Max_ID = this._context.ct_PhieuXuat.Where<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == Output.LOC_ID && e.NGAYLAP.Date == Output.NGAYLAP.Date)).Select<ct_PhieuXuat, int>((Expression<Func<ct_PhieuXuat, int>>) (e => e.SOPHIEU)).DefaultIfEmpty<int>().Max<int>();
          Output.SOPHIEU = Max_ID + 1;
          Output.MAPHIEU = API.GetMaPhieu(nameof (Output), Output.NGAYLAP, Output.SOPHIEU);
          this._context.Entry<v_ct_PhieuXuat>(Output).State = EntityState.Modified;
          int num2 = await this._context.SaveChangesAsync();
        }
        v_ct_PhieuXuat ct_PhieuXuat = new v_ct_PhieuXuat();
        ct_PhieuXuat.lstct_PhieuXuat_ChiTiet = new List<v_ct_PhieuXuat_ChiTiet>();
        SP_Parameter SP_Parameter = new SP_Parameter();
        SP_Parameter.ID_PHIEUXUAT = Output.ID;
        ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
        IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuXuat(SP_Parameter);
        if (actionResult is OkObjectResult okResult)
        {
          ApiResponse ApiResponse = okResult.Value as ApiResponse;
          if (ApiResponse != null && ApiResponse.Data != null)
          {
            List<v_ct_PhieuXuat> lstPhieuDatHang = ApiResponse.Data as List<v_ct_PhieuXuat>;
            if (lstPhieuDatHang != null && lstPhieuDatHang.Count<v_ct_PhieuXuat>() > 0)
              ct_PhieuXuat = lstPhieuDatHang.FirstOrDefault<v_ct_PhieuXuat>() ?? new v_ct_PhieuXuat();
            lstPhieuDatHang = (List<v_ct_PhieuXuat>) null;
          }
          ApiResponse = (ApiResponse) null;
        }
        return (ActionResult<ct_PhieuXuat>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) ct_PhieuXuat
        });
      }
    }
    catch (Exception ex)
    {
      return (ActionResult<ct_PhieuXuat>) (ActionResult) this.Ok((object) new ApiResponse()
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

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteOutput(string LOC_ID, string ID)
  {
    try
    {
      ct_PhieuXuat Output = await this._context.ct_PhieuXuat.FirstOrDefaultAsync<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Output == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        List<ct_PhieuGiaoHang_ChiTiet> lstPhieuGiaoHang_ChiTiet = await this._context.ct_PhieuGiaoHang_ChiTiet.Where<ct_PhieuGiaoHang_ChiTiet>((Expression<Func<ct_PhieuGiaoHang_ChiTiet, bool>>) (e => e.LOC_ID == Output.LOC_ID && e.ID_PHIEUXUAT == Output.ID)).ToListAsync<ct_PhieuGiaoHang_ChiTiet>();
        if (lstPhieuGiaoHang_ChiTiet != null && lstPhieuGiaoHang_ChiTiet.Count<ct_PhieuGiaoHang_ChiTiet>() > 0)
        {
          string ChungTu = $"({string.Join(";", lstPhieuGiaoHang_ChiTiet.Select<ct_PhieuGiaoHang_ChiTiet, string>((Func<ct_PhieuGiaoHang_ChiTiet, string>) (e => e.ID_PHIEUGIAOHANG)))})";
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = $"Có 'Phiếu Giao Hàng {ChungTu}' liên quan tới {Output.MAPHIEU}!",
            Data = (object) ""
          });
        }
        List<ct_PhieuChi> lstPhieuChi = await this._context.ct_PhieuChi.Where<ct_PhieuChi>((Expression<Func<ct_PhieuChi, bool>>) (e => e.LOC_ID == Output.LOC_ID && e.CHUNGTUKEMTHEO == Output.MAPHIEU)).ToListAsync<ct_PhieuChi>();
        if (lstPhieuChi != null && lstPhieuChi.Count<ct_PhieuChi>() > 0)
        {
          string ChungTu = $"({string.Join(";", lstPhieuChi.Select<ct_PhieuChi, string>((Func<ct_PhieuChi, string>) (e => e.MAPHIEU)))})";
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = $"Có 'Phiếu Chi {ChungTu}' liên quan tới {Output.MAPHIEU}!",
            Data = (object) ""
          });
        }
        List<ct_PhieuThu> lstPhieuThu = await this._context.ct_PhieuThu.Where<ct_PhieuThu>((Expression<Func<ct_PhieuThu, bool>>) (e => e.LOC_ID == Output.LOC_ID && e.CHUNGTUKEMTHEO == Output.MAPHIEU)).ToListAsync<ct_PhieuThu>();
        if (lstPhieuThu != null && lstPhieuThu.Count<ct_PhieuThu>() > 0)
        {
          string ChungTu = $"({string.Join(";", lstPhieuThu.Select<ct_PhieuThu, string>((Func<ct_PhieuThu, string>) (e => e.MAPHIEU)))})";
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = $"Có 'Phiếu Thu {ChungTu}' liên quan tới {Output.MAPHIEU}!",
            Data = (object) ""
          });
        }
        List<ct_PhieuNhap> lstPhieuNhap = await this._context.ct_PhieuNhap.Where<ct_PhieuNhap>((Expression<Func<ct_PhieuNhap, bool>>) (e => e.LOC_ID == Output.LOC_ID && e.CHUNGTUKEMTHEO == Output.MAPHIEU)).ToListAsync<ct_PhieuNhap>();
        if (lstPhieuNhap != null && lstPhieuNhap.Count<ct_PhieuNhap>() > 0)
        {
          string ChungTu = $"({string.Join(";", lstPhieuNhap.Select<ct_PhieuNhap, string>((Func<ct_PhieuNhap, string>) (e => e.MAPHIEU)))})";
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = $"Có 'Phiếu Nhập {ChungTu}' liên quan tới {Output.MAPHIEU}!",
            Data = (object) ""
          });
        }
        List<ct_PhieuXuat_ChiTiet> lstPhieuNhap_ChiTiet = await this._context.ct_PhieuXuat_ChiTiet.Where<ct_PhieuXuat_ChiTiet>((Expression<Func<ct_PhieuXuat_ChiTiet, bool>>) (e => e.LOC_ID == Output.LOC_ID && e.ID_PHIEUXUAT == Output.ID)).ToListAsync<ct_PhieuXuat_ChiTiet>();
        if (lstPhieuNhap_ChiTiet != null)
        {
          foreach (ct_PhieuXuat_ChiTiet itm in lstPhieuNhap_ChiTiet)
          {
            if (!Output.ISPHIEUDIEUHANG)
            {
              dm_HangHoa_Kho objdm_HangHoa_Kho = this._context.dm_HangHoa_Kho.FromSqlRaw<dm_HangHoa_Kho>("\r\n                                        SELECT *\r\n                                        FROM dm_HangHoa_Kho WITH (UPDLOCK, ROWLOCK)\r\n                                        WHERE LOC_ID = @loc\r\n                                          AND ID = @id\r\n                                    ", (object) new SqlParameter("@loc", (object) itm.LOC_ID), (object) new SqlParameter("@id", (object) itm.ID_HANGHOAKHO)).AsTracking<dm_HangHoa_Kho>().FirstOrDefault<dm_HangHoa_Kho>();
              if (objdm_HangHoa_Kho != null)
              {
                itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
                objdm_HangHoa_Kho.QTY += itm.TONGSOLUONG;
                this._context.Entry<dm_HangHoa_Kho>(objdm_HangHoa_Kho).State = EntityState.Modified;
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
            this._context.ct_PhieuXuat_ChiTiet.Remove(itm);
          }
        }
        List<ct_PhieuDatHang> lstPhieuDatHang = await this._context.ct_PhieuDatHang.Where<ct_PhieuDatHang>((Expression<Func<ct_PhieuDatHang, bool>>) (e => e.LOC_ID == Output.LOC_ID && e.ID_PHIEUXUAT == Output.ID)).ToListAsync<ct_PhieuDatHang>();
        foreach (ct_PhieuDatHang itm in lstPhieuDatHang)
          itm.ID_PHIEUXUAT = "";
        this._context.ct_PhieuXuat.Remove(Output);
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        lstPhieuGiaoHang_ChiTiet = (List<ct_PhieuGiaoHang_ChiTiet>) null;
        lstPhieuChi = (List<ct_PhieuChi>) null;
        lstPhieuThu = (List<ct_PhieuThu>) null;
        lstPhieuNhap = (List<ct_PhieuNhap>) null;
        lstPhieuNhap_ChiTiet = (List<ct_PhieuXuat_ChiTiet>) null;
        lstPhieuDatHang = (List<ct_PhieuDatHang>) null;
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

  private bool OutputExistsID(string LOC_ID, string ID)
  {
    return this._context.ct_PhieuXuat.Any<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }
}
