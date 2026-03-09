// Decompiled with JetBrains decompiler
// Type: API_QuanLyTHP.Controllers.Uniben.UnibenController
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
using MyApiNetCore6.Controllers;
using MyApiNetCore6.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;


namespace API_QuanLyTHP.Controllers.Uniben
{

[Route("api/[controller]")]
[ApiController]
public class UnibenController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;
  private readonly HttpClient _httpClient;
  private string linkToken = "/authenticate";

  public UnibenController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._configuration = configuration;
    this._httpClient = new HttpClient();
  }

  [HttpPut("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PutInput(string LOC_ID, string ID)
  {
    try
    {
      dm_TaiKhoan_Uniben TaiKhoan = await this._context.dm_TaiKhoan_Uniben.Where<dm_TaiKhoan_Uniben>((Expression<Func<dm_TaiKhoan_Uniben, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID)).FirstAsync<dm_TaiKhoan_Uniben>();
      if (TaiKhoan == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID} dữ liệu!",
          Data = (object) ""
        });
      bool bolCheck = await this.CheckToken(TaiKhoan, true);
      if (!bolCheck)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Không lấy được Token",
          Data = (object) ""
        });
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) TaiKhoan
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

  private async Task<bool> CheckToken(dm_TaiKhoan_Uniben TaiKhoan, bool bolLayTokenMoi = false)
  {
    try
    {
      UnibenService _invoiceService = new UnibenService();
      DatabaseTHP.Class.Uniben.Uniben.UnibenTokenRequest request = new DatabaseTHP.Class.Uniben.Uniben.UnibenTokenRequest()
      {
        username = TaiKhoan.USERNAME,
        password = TaiKhoan.PASSWORD
      };
      DateTime now;
      int num1;
      if (!string.IsNullOrEmpty(TaiKhoan.ACCESSTOKEN) && TaiKhoan.THOIGIANLAYTOKEN.HasValue && !string.IsNullOrEmpty(TaiKhoan.ORGANIZATIONUNITID))
      {
        if (TaiKhoan.THOIGIANLAYTOKEN.HasValue)
        {
          now = TaiKhoan.THOIGIANLAYTOKEN.Value;
          DateTime date1 = now.Date;
          now = DateTime.Now;
          DateTime date2 = now.Date;
          num1 = date1 != date2 ? 1 : 0;
        }
        else
          num1 = 0;
      }
      else
        num1 = 1;
      int num2 = bolLayTokenMoi ? 1 : 0;
      if ((num1 | num2) != 0)
      {
        DatabaseTHP.Class.Uniben.Uniben.UnibenToken token = await _invoiceService.GetTokenAsync(request, TaiKhoan.LINK + this.linkToken);
        if (token == null)
          return false;
        TaiKhoan.ACCESSTOKEN = token.id_token;
        TaiKhoan.ORGANIZATIONUNITID = token.distributorCode;
        dm_TaiKhoan_Uniben dmTaiKhoanUniben = TaiKhoan;
        now = DateTime.Now;
        DateTime? nullable = new DateTime?(now.AddMinutes(-1.0));
        dmTaiKhoanUniben.THOIGIANLAYTOKEN = nullable;
        this._context.Entry<dm_TaiKhoan_Uniben>(TaiKhoan).State = EntityState.Modified;
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num3 = await this._context.SaveChangesAsync();
        auditLog = (AuditLogController) null;
        token = (DatabaseTHP.Class.Uniben.Uniben.UnibenToken) null;
      }
      return true;
    }
    catch (Exception ex)
    {
      Console.WriteLine("Lỗi khi tạo InvoiceMaster: " + ex.Message);
      return false;
    }
  }

  [HttpGet("{LOC_ID}/{toOrderDate}/{fromOrderDate}/{SearchString}")]
  public async Task<IActionResult> GetListSales(
    string LOC_ID,
    string toOrderDate,
    string fromOrderDate,
    string SearchString)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    UnibenController cDisplayClass70 = new UnibenController();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass70.LOC_ID = LOC_ID;
    try
    {
      // ISSUE: reference to a compiler-generated field
      dm_TaiKhoan_Uniben TaiKhoan = await this._context.dm_TaiKhoan_Uniben.Where<dm_TaiKhoan_Uniben>((Expression<Func<dm_TaiKhoan_Uniben, bool>>) (e => e.LOC_ID == cDisplayClass70.LOC_ID && e.ISACTIVE)).FirstAsync<dm_TaiKhoan_Uniben>();
      if (TaiKhoan == null)
      {
        // ISSUE: reference to a compiler-generated field
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {cDisplayClass70.LOC_ID} dữ liệu!",
          Data = (object) ""
        });
      }
      UnibenService invoiceService = new UnibenService();
      bool bolCheck = await this.CheckToken(TaiKhoan);
      if (!bolCheck)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Không lấy được Token",
          Data = (object) ""
        });
      if (SearchString == "%")
        SearchString = "";
      List<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData> lstUnibenOrderData = new List<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData>();
      int pageSize = 100;
      int currentPage = 1;
      int lastPage = 1;
      int totalItems = 0;
      do
      {
        DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse response = await invoiceService.GetListSales(TaiKhoan.ACCESSTOKEN, TaiKhoan.LINK + this.GetLink(TaiKhoan.ORGANIZATIONUNITID, currentPage, pageSize, fromOrderDate, toOrderDate, totalItems, SearchString));
        if (response != null && response.status == 404)
        {
          bolCheck = await this.CheckToken(TaiKhoan);
          break;
        }
        if (response != null && response.payload != null && currentPage <= response.metaData.lastPage)
        {
          string json = response.payload.ToString();
          if (string.IsNullOrWhiteSpace(json) || !(json.Trim() == "[]"))
          {
            if (response.payload != null && !string.IsNullOrWhiteSpace(json))
            {
              List<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData> data = JsonSerializer.Deserialize<List<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData>>(json);
              if (data != null)
              {
                foreach (DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData unibenOrderData in data)
                {
                  // ISSUE: object of a compiler-generated type is created
                  // ISSUE: variable of a compiler-generated type
                  UnibenController cDisplayClass71 = new UnibenController();
                  // ISSUE: reference to a compiler-generated field
                  cDisplayClass71 = cDisplayClass70;
                  // ISSUE: reference to a compiler-generated field
                  cDisplayClass71.item = unibenOrderData;
                  ParameterExpression parameterExpression;
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: method reference
                  // ISSUE: field reference
                  // ISSUE: method reference
                  // ISSUE: method reference
                  List<ct_PhieuDatHang> existingOrderCodes = await this._context.ct_PhieuDatHang.Where<ct_PhieuDatHang>(Expression.Lambda<Func<ct_PhieuDatHang, bool>>((Expression) Expression.AndAlso(e.LOC_ID == cDisplayClass71.CS\u0024\u003C\u003E8__locals1.LOC_ID, (Expression) Expression.Equal((Expression) Expression.Call(e.GHICHU, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.Trim)), Array.Empty<Expression>()), (Expression) Expression.Call((Expression) Expression.Property((Expression) Expression.Field((Expression) Expression.Constant((object) cDisplayClass71, typeof (UnibenController.\u003C\u003Ec__DisplayClass7_1)), FieldInfo.GetFieldFromHandle(__fieldref (UnibenController.\u003C\u003Ec__DisplayClass7_1.item))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData.get_orderCode))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.Trim)), Array.Empty<Expression>()))), parameterExpression)).ToListAsync<ct_PhieuDatHang>();
                  if (existingOrderCodes != null && existingOrderCodes.Count > 0)
                  {
                    // ISSUE: reference to a compiler-generated field
                    cDisplayClass71.item.DATONTAI = true;
                    // ISSUE: reference to a compiler-generated field
                    cDisplayClass71.item.MAPHIEUDATHANG += string.Join(",", (IEnumerable<string>) existingOrderCodes.Select<ct_PhieuDatHang, string>((Func<ct_PhieuDatHang, string>) (e => e.MAPHIEU)).ToList<string>());
                  }
                  existingOrderCodes = (List<ct_PhieuDatHang>) null;
                  cDisplayClass71 = (UnibenController.\u003C\u003Ec__DisplayClass7_1) null;
                }
                lstUnibenOrderData.AddRange((IEnumerable<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData>) data);
              }
              data = (List<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData>) null;
            }
            if (response.metaData != null)
            {
              lastPage = response.metaData.lastPage;
              totalItems = response.metaData.totalItems;
            }
            ++currentPage;
            response = (DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse) null;
            json = (string) null;
          }
          else
            break;
        }
        else
          break;
      }
      while (currentPage <= lastPage);
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) lstUnibenOrderData
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

  [HttpGet("{LOC_ID}/{Type}/{SearchString}")]
  public async Task<IActionResult> GetLienKet(string LOC_ID, string Type, string SearchString = "")
  {
    try
    {
      DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse unibenOrderListResponse = new DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse();
      unibenOrderListResponse.lstDonHang = new List<DatabaseTHP.Class.Uniben.Uniben.DonHang>();
      unibenOrderListResponse.HangHoa = new List<v_uniben_dm_LienKet_HangHoa>();
      unibenOrderListResponse.KhachHang = new List<v_uniben_dm_LienKet_KhachHang>();
      unibenOrderListResponse.NhanVien = new List<v_uniben_dm_LienKet_NhanVien>();
      if (SearchString == "%")
        SearchString = "";
      switch (Type)
      {
        case "Product":
          List<uniben_dm_LienKet_HangHoa> response1 = await this._context.uniben_dm_LienKet_HangHoa.Where<uniben_dm_LienKet_HangHoa>((Expression<Func<uniben_dm_LienKet_HangHoa, bool>>) (e => e.LOC_ID == LOC_ID && e.ISACTIVE && (string.IsNullOrEmpty(SearchString) || e.TENHANGHOA.Contains(SearchString) || e.MAHANGHOA.Contains(SearchString) || e.ID_UNIBEN.Contains(SearchString)))).OrderBy<uniben_dm_LienKet_HangHoa, string>((Expression<Func<uniben_dm_LienKet_HangHoa, string>>) (e => e.TENHANGHOA)).ToListAsync<uniben_dm_LienKet_HangHoa>();
          foreach (uniben_dm_LienKet_HangHoa item in response1)
            unibenOrderListResponse.HangHoa.Add(UnibenController.ConvertobjectTodm_LienKet_HangHoa<uniben_dm_LienKet_HangHoa>(item, new v_uniben_dm_LienKet_HangHoa()));
          response1 = (List<uniben_dm_LienKet_HangHoa>) null;
          break;
        case "Customer":
          List<uniben_dm_LienKet_KhachHang> response2 = await this._context.uniben_dm_LienKet_KhachHang.Where<uniben_dm_LienKet_KhachHang>((Expression<Func<uniben_dm_LienKet_KhachHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ISACTIVE && (string.IsNullOrEmpty(SearchString) || e.TENKHACHHANG.Contains(SearchString) || e.MASOTHUE.Contains(SearchString) || e.MAKHACHHANG.Contains(SearchString) || e.ID_UNIBEN.Contains(SearchString)))).OrderBy<uniben_dm_LienKet_KhachHang, string>((Expression<Func<uniben_dm_LienKet_KhachHang, string>>) (e => e.TENKHACHHANG)).ToListAsync<uniben_dm_LienKet_KhachHang>();
          foreach (uniben_dm_LienKet_KhachHang item in response2)
            unibenOrderListResponse.KhachHang.Add(UnibenController.ConvertobjectTodm_LienKet_KhachHang<uniben_dm_LienKet_KhachHang>(item, new v_uniben_dm_LienKet_KhachHang()));
          response2 = (List<uniben_dm_LienKet_KhachHang>) null;
          break;
        case "Employee":
          List<uniben_dm_LienKet_NhanVien> response3 = await this._context.uniben_dm_LienKet_NhanVien.Where<uniben_dm_LienKet_NhanVien>((Expression<Func<uniben_dm_LienKet_NhanVien, bool>>) (e => e.LOC_ID == LOC_ID && e.ISACTIVE && (string.IsNullOrEmpty(SearchString) || e.TENNHANVIEN.Contains(SearchString) || e.MANHANVIEN.Contains(SearchString) || e.ID_UNIBEN.Contains(SearchString)))).OrderBy<uniben_dm_LienKet_NhanVien, string>((Expression<Func<uniben_dm_LienKet_NhanVien, string>>) (e => e.TENNHANVIEN)).ToListAsync<uniben_dm_LienKet_NhanVien>();
          foreach (uniben_dm_LienKet_NhanVien item in response3)
            unibenOrderListResponse.NhanVien.Add(UnibenController.ConvertobjectTodm_LienKet_NhanVien<uniben_dm_LienKet_NhanVien>(item, new v_uniben_dm_LienKet_NhanVien()));
          response3 = (List<uniben_dm_LienKet_NhanVien>) null;
          break;
      }
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) new List<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse>()
        {
          unibenOrderListResponse
        }
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

  private static v_uniben_dm_LienKet_HangHoa ConvertobjectTodm_LienKet_HangHoa<T>(
    T objectFrom,
    v_uniben_dm_LienKet_HangHoa objectTo)
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

  private static v_uniben_dm_LienKet_KhachHang ConvertobjectTodm_LienKet_KhachHang<T>(
    T objectFrom,
    v_uniben_dm_LienKet_KhachHang objectTo)
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

  private static v_uniben_dm_LienKet_NhanVien ConvertobjectTodm_LienKet_NhanVien<T>(
    T objectFrom,
    v_uniben_dm_LienKet_NhanVien objectTo)
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

  [HttpPost("{LOC_ID}")]
  public async Task<IActionResult> GetInput(string LOC_ID, [FromBody] List<Deposit> Input)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    UnibenController.\u003C\u003Ec__DisplayClass12_0 cDisplayClass120 = new UnibenController.\u003C\u003Ec__DisplayClass12_0();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass120.LOC_ID = LOC_ID;
    try
    {
      // ISSUE: reference to a compiler-generated field
      dm_TaiKhoan_Uniben TaiKhoan = await this._context.dm_TaiKhoan_Uniben.Where<dm_TaiKhoan_Uniben>((Expression<Func<dm_TaiKhoan_Uniben, bool>>) (e => e.LOC_ID == cDisplayClass120.LOC_ID && e.ISACTIVE)).FirstAsync<dm_TaiKhoan_Uniben>();
      if (TaiKhoan == null)
      {
        // ISSUE: reference to a compiler-generated field
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {cDisplayClass120.LOC_ID} dữ liệu!",
          Data = (object) ""
        });
      }
      UnibenService invoiceService = new UnibenService();
      bool bolCheck = await this.CheckToken(TaiKhoan);
      if (!bolCheck)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Không lấy được Token",
          Data = (object) ""
        });
      DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse unibenOrderListResponse = new DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse();
      List<v_uniben_dm_LienKet_HangHoa> response_hh = new List<v_uniben_dm_LienKet_HangHoa>();
      List<v_uniben_dm_LienKet_KhachHang> response_kh = new List<v_uniben_dm_LienKet_KhachHang>();
      List<v_uniben_dm_LienKet_NhanVien> response_nv = new List<v_uniben_dm_LienKet_NhanVien>();
      List<DatabaseTHP.Class.Uniben.Uniben.DonHang> lstDonHang = new List<DatabaseTHP.Class.Uniben.Uniben.DonHang>();
      foreach (Deposit deposit in Input)
      {
        Deposit order = deposit;
        // ISSUE: object of a compiler-generated type is created
        // ISSUE: variable of a compiler-generated type
        UnibenController.\u003C\u003Ec__DisplayClass12_1 cDisplayClass121 = new UnibenController.\u003C\u003Ec__DisplayClass12_1();
        // ISSUE: reference to a compiler-generated field
        cDisplayClass121.CS\u0024\u003C\u003E8__locals1 = cDisplayClass120;
        DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse response = await invoiceService.GetOrderDetail(TaiKhoan.ACCESSTOKEN, TaiKhoan.LINK + this.GetLink(TaiKhoan.ORGANIZATIONUNITID, order.ID));
        if (response != null && response.status == 404)
        {
          bolCheck = await this.CheckToken(TaiKhoan);
          break;
        }
        // ISSUE: reference to a compiler-generated field
        cDisplayClass121.khachHang = new v_uniben_dm_LienKet_KhachHang();
        // ISSUE: reference to a compiler-generated field
        cDisplayClass121.hangHoa = new v_uniben_dm_LienKet_HangHoa();
        // ISSUE: reference to a compiler-generated field
        cDisplayClass121.nhanVien = new v_uniben_dm_LienKet_NhanVien();
        DatabaseTHP.Class.Uniben.Uniben.DonHang donHang = new DatabaseTHP.Class.Uniben.Uniben.DonHang();
        if (response != null && response.payload != null)
        {
          bool bolAddOrder = true;
          string json = response.payload.ToString();
          if (!string.IsNullOrWhiteSpace(json))
          {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            UnibenController.\u003C\u003Ec__DisplayClass12_2 cDisplayClass122 = new UnibenController.\u003C\u003Ec__DisplayClass12_2();
            // ISSUE: reference to a compiler-generated field
            cDisplayClass122.CS\u0024\u003C\u003E8__locals2 = cDisplayClass121;
            // ISSUE: reference to a compiler-generated field
            cDisplayClass122.data = JsonSerializer.Deserialize<DatabaseTHP.Class.Uniben.Uniben.UnibenOrder>(json);
            // ISSUE: reference to a compiler-generated field
            if (cDisplayClass122.data != null)
            {
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              cDisplayClass122.CS\u0024\u003C\u003E8__locals2.khachHang = new v_uniben_dm_LienKet_KhachHang();
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              cDisplayClass122.CS\u0024\u003C\u003E8__locals2.khachHang.ID_UNIBEN = cDisplayClass122.data.customerId.ToString();
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              cDisplayClass122.CS\u0024\u003C\u003E8__locals2.khachHang.MAKHACHHANG = cDisplayClass122.data.customerCode;
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              cDisplayClass122.CS\u0024\u003C\u003E8__locals2.khachHang.TENKHACHHANG = cDisplayClass122.data.customerName;
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              cDisplayClass122.CS\u0024\u003C\u003E8__locals2.khachHang.DIACHI = cDisplayClass122.data.deliveryAddress;
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              cDisplayClass122.CS\u0024\u003C\u003E8__locals2.khachHang.MASOTHUE = cDisplayClass122.data.taxNo;
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              cDisplayClass122.CS\u0024\u003C\u003E8__locals2.khachHang.LOC_ID = cDisplayClass122.CS\u0024\u003C\u003E8__locals2.CS\u0024\u003C\u003E8__locals1.LOC_ID;
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              uniben_dm_LienKet_KhachHang khachHangExist = await this._context.uniben_dm_LienKet_KhachHang.Where<uniben_dm_LienKet_KhachHang>((Expression<Func<uniben_dm_LienKet_KhachHang, bool>>) (e => e.MAKHACHHANG == cDisplayClass122.CS\u0024\u003C\u003E8__locals2.khachHang.MAKHACHHANG && e.ID_UNIBEN == cDisplayClass122.CS\u0024\u003C\u003E8__locals2.khachHang.ID_UNIBEN && e.MASOTHUE == cDisplayClass122.CS\u0024\u003C\u003E8__locals2.khachHang.MASOTHUE && e.LOC_ID == cDisplayClass122.CS\u0024\u003C\u003E8__locals2.CS\u0024\u003C\u003E8__locals1.LOC_ID)).FirstOrDefaultAsync<uniben_dm_LienKet_KhachHang>();
              if (khachHangExist == null || string.IsNullOrEmpty(khachHangExist.ID_KHACHHANG))
              {
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                dm_KhachHang khachHangMSTExist = await this._context.dm_KhachHang.Where<dm_KhachHang>((Expression<Func<dm_KhachHang, bool>>) (e => e.MASOTHUE == cDisplayClass122.CS\u0024\u003C\u003E8__locals2.khachHang.MASOTHUE && e.LOC_ID == cDisplayClass122.CS\u0024\u003C\u003E8__locals2.CS\u0024\u003C\u003E8__locals1.LOC_ID)).FirstOrDefaultAsync<dm_KhachHang>();
                if (khachHangMSTExist != null)
                {
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  cDisplayClass122.CS\u0024\u003C\u003E8__locals2.khachHang.ID_KHACHHANG = khachHangMSTExist.ID;
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated method
                v_uniben_dm_LienKet_KhachHang khachHangKHExist = response_kh.Where<v_uniben_dm_LienKet_KhachHang>(new Func<v_uniben_dm_LienKet_KhachHang, bool>(cDisplayClass122.CS\u0024\u003C\u003E8__locals2.\u003CGetInput\u003Eb__2)).FirstOrDefault<v_uniben_dm_LienKet_KhachHang>();
                if (khachHangKHExist == null)
                {
                  bolAddOrder = false;
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  response_kh.Add(cDisplayClass122.CS\u0024\u003C\u003E8__locals2.khachHang);
                }
                khachHangMSTExist = (dm_KhachHang) null;
                khachHangKHExist = (v_uniben_dm_LienKet_KhachHang) null;
              }
              else
              {
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                cDisplayClass122.CS\u0024\u003C\u003E8__locals2.khachHang.ID_KHACHHANG = khachHangExist.ID_KHACHHANG;
              }
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              cDisplayClass122.CS\u0024\u003C\u003E8__locals2.nhanVien = new v_uniben_dm_LienKet_NhanVien();
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              cDisplayClass122.CS\u0024\u003C\u003E8__locals2.nhanVien.ID_UNIBEN = cDisplayClass122.data.routeCode;
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              cDisplayClass122.CS\u0024\u003C\u003E8__locals2.nhanVien.MANHANVIEN = cDisplayClass122.data.routeCode;
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              cDisplayClass122.CS\u0024\u003C\u003E8__locals2.nhanVien.LOC_ID = cDisplayClass122.CS\u0024\u003C\u003E8__locals2.CS\u0024\u003C\u003E8__locals1.LOC_ID;
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              uniben_dm_LienKet_NhanVien nhanVienExist = await this._context.uniben_dm_LienKet_NhanVien.Where<uniben_dm_LienKet_NhanVien>((Expression<Func<uniben_dm_LienKet_NhanVien, bool>>) (e => e.ID_UNIBEN == cDisplayClass122.CS\u0024\u003C\u003E8__locals2.nhanVien.ID_UNIBEN && e.MANHANVIEN == cDisplayClass122.CS\u0024\u003C\u003E8__locals2.nhanVien.MANHANVIEN && e.LOC_ID == cDisplayClass122.CS\u0024\u003C\u003E8__locals2.CS\u0024\u003C\u003E8__locals1.LOC_ID)).FirstOrDefaultAsync<uniben_dm_LienKet_NhanVien>();
              if (nhanVienExist == null || string.IsNullOrEmpty(nhanVienExist.ID_NHANVIEN))
              {
                DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse responseSales = await invoiceService.GetListSales(TaiKhoan.ACCESSTOKEN, TaiKhoan.LINK + this.GetLinkSales(TaiKhoan.ORGANIZATIONUNITID, order.ID));
                if (responseSales != null && responseSales?.payload != null)
                {
                  json = responseSales.payload.ToString();
                  if (!string.IsNullOrWhiteSpace(json))
                  {
                    List<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData> salesData = JsonSerializer.Deserialize<List<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData>>(json);
                    if (salesData != null && salesData.Count > 0)
                    {
                      string salesUserName = salesData[0].salesUserName;
                      // ISSUE: reference to a compiler-generated field
                      // ISSUE: reference to a compiler-generated field
                      cDisplayClass122.CS\u0024\u003C\u003E8__locals2.nhanVien.TENNHANVIEN = salesUserName;
                      // ISSUE: reference to a compiler-generated field
                      // ISSUE: reference to a compiler-generated method
                      v_uniben_dm_LienKet_NhanVien khachHangKHExist = response_nv.Where<v_uniben_dm_LienKet_NhanVien>(new Func<v_uniben_dm_LienKet_NhanVien, bool>(cDisplayClass122.CS\u0024\u003C\u003E8__locals2.\u003CGetInput\u003Eb__5)).FirstOrDefault<v_uniben_dm_LienKet_NhanVien>();
                      if (khachHangKHExist == null)
                      {
                        // ISSUE: reference to a compiler-generated field
                        // ISSUE: reference to a compiler-generated field
                        response_nv.Add(cDisplayClass122.CS\u0024\u003C\u003E8__locals2.nhanVien);
                        bolAddOrder = false;
                      }
                      salesUserName = (string) null;
                      khachHangKHExist = (v_uniben_dm_LienKet_NhanVien) null;
                    }
                    salesData = (List<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData>) null;
                  }
                }
                responseSales = (DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse) null;
              }
              else
              {
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                cDisplayClass122.CS\u0024\u003C\u003E8__locals2.nhanVien.ID_NHANVIEN = nhanVienExist.ID_NHANVIEN;
              }
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              if (cDisplayClass122.data.details != null && cDisplayClass122.data.details.Count > 0)
              {
                // ISSUE: reference to a compiler-generated field
                foreach (DatabaseTHP.Class.Uniben.Uniben.UnibenOrderDetail detail in cDisplayClass122.data.details)
                {
                  DatabaseTHP.Class.Uniben.Uniben.UnibenOrderDetail item = detail;
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  cDisplayClass122.CS\u0024\u003C\u003E8__locals2.hangHoa = new v_uniben_dm_LienKet_HangHoa();
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  cDisplayClass122.CS\u0024\u003C\u003E8__locals2.hangHoa.MAHANGHOA = item.productCode;
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  cDisplayClass122.CS\u0024\u003C\u003E8__locals2.hangHoa.TENHANGHOA = item.productAbbr;
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  cDisplayClass122.CS\u0024\u003C\u003E8__locals2.hangHoa.ID_UNIBEN = item.productCode;
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  cDisplayClass122.CS\u0024\u003C\u003E8__locals2.hangHoa.ISKHUYENMAI = !string.IsNullOrEmpty(item.promotionCode);
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  if (cDisplayClass122.CS\u0024\u003C\u003E8__locals2.hangHoa.MAHANGHOA != null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    uniben_dm_LienKet_HangHoa hangHoaExist = await this._context.uniben_dm_LienKet_HangHoa.Where<uniben_dm_LienKet_HangHoa>((Expression<Func<uniben_dm_LienKet_HangHoa, bool>>) (e => e.MAHANGHOA == cDisplayClass122.CS\u0024\u003C\u003E8__locals2.hangHoa.MAHANGHOA && e.ID_UNIBEN == cDisplayClass122.CS\u0024\u003C\u003E8__locals2.hangHoa.ID_UNIBEN && e.ISKHUYENMAI == cDisplayClass122.CS\u0024\u003C\u003E8__locals2.hangHoa.ISKHUYENMAI && e.LOC_ID == cDisplayClass122.CS\u0024\u003C\u003E8__locals2.CS\u0024\u003C\u003E8__locals1.LOC_ID)).FirstOrDefaultAsync<uniben_dm_LienKet_HangHoa>();
                    if (hangHoaExist == null || string.IsNullOrEmpty(hangHoaExist.ID_HANGHOA))
                    {
                      // ISSUE: reference to a compiler-generated field
                      // ISSUE: reference to a compiler-generated field
                      // ISSUE: reference to a compiler-generated field
                      // ISSUE: reference to a compiler-generated field
                      // ISSUE: reference to a compiler-generated field
                      // ISSUE: reference to a compiler-generated method
                      v_uniben_dm_LienKet_HangHoa hangHoaHHExist = response_hh.Where<v_uniben_dm_LienKet_HangHoa>(cDisplayClass122.CS\u0024\u003C\u003E8__locals2.\u003C\u003E9__7 ?? (cDisplayClass122.CS\u0024\u003C\u003E8__locals2.\u003C\u003E9__7 = new Func<v_uniben_dm_LienKet_HangHoa, bool>(cDisplayClass122.CS\u0024\u003C\u003E8__locals2.\u003CGetInput\u003Eb__7))).FirstOrDefault<v_uniben_dm_LienKet_HangHoa>();
                      if (hangHoaHHExist == null)
                      {
                        bolAddOrder = false;
                        // ISSUE: reference to a compiler-generated field
                        // ISSUE: reference to a compiler-generated field
                        response_hh.Add(cDisplayClass122.CS\u0024\u003C\u003E8__locals2.hangHoa);
                      }
                      hangHoaHHExist = (v_uniben_dm_LienKet_HangHoa) null;
                    }
                    hangHoaExist = (uniben_dm_LienKet_HangHoa) null;
                    item = (DatabaseTHP.Class.Uniben.Uniben.UnibenOrderDetail) null;
                  }
                }
              }
              if (bolAddOrder)
              {
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                (v_ct_PhieuDatHang, string) valueTuple1 = await this.ConvertOrderToPhieuDatHang(cDisplayClass122.data, cDisplayClass122.CS\u0024\u003C\u003E8__locals2.khachHang, cDisplayClass122.CS\u0024\u003C\u003E8__locals2.nhanVien, cDisplayClass122.CS\u0024\u003C\u003E8__locals2.CS\u0024\u003C\u003E8__locals1.LOC_ID);
                (v_ct_PhieuDatHang, string) valueTuple2 = valueTuple1;
                v_ct_PhieuDatHang PhieuDatHang = valueTuple2.Item1;
                string error = valueTuple2.Item2;
                valueTuple1 = ();
                valueTuple2 = ();
                if (PhieuDatHang != null)
                {
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  AspNetUsers aspNetUsers = await this._context.AspNetUsers.Where<AspNetUsers>((Expression<Func<AspNetUsers, bool>>) (e => e.ID == cDisplayClass122.CS\u0024\u003C\u003E8__locals2.nhanVien.ID_NHANVIEN)).FirstOrDefaultAsync<AspNetUsers>();
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  dm_KhachHang KhachHang = await this._context.dm_KhachHang.Where<dm_KhachHang>((Expression<Func<dm_KhachHang, bool>>) (e => e.ID == cDisplayClass122.CS\u0024\u003C\u003E8__locals2.khachHang.ID_KHACHHANG)).FirstOrDefaultAsync<dm_KhachHang>();
                  donHang.DonDatHang = new v_ct_PhieuDatHang();
                  donHang.DonDatHang = PhieuDatHang;
                  donHang.DonDatHang.NAME_NHANVIEN = aspNetUsers != null ? aspNetUsers.FullName : "";
                  donHang.DonDatHang.MA_KHACHHANG = KhachHang != null ? KhachHang.MA : "";
                  donHang.DonDatHang.NAME_KHACHHANG = KhachHang != null ? KhachHang.NAME : "";
                  donHang.DonDatHang.DIACHI_KHACHHANG = KhachHang != null ? KhachHang.ADDRESS : "";
                  // ISSUE: reference to a compiler-generated field
                  donHang.unibenOrder = cDisplayClass122.data;
                  ParameterExpression parameterExpression;
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: method reference
                  // ISSUE: field reference
                  // ISSUE: method reference
                  // ISSUE: method reference
                  List<ct_PhieuDatHang> existingOrderCodes = await this._context.ct_PhieuDatHang.Where<ct_PhieuDatHang>(Expression.Lambda<Func<ct_PhieuDatHang, bool>>((Expression) Expression.AndAlso(e.LOC_ID == cDisplayClass122.CS\u0024\u003C\u003E8__locals2.CS\u0024\u003C\u003E8__locals1.LOC_ID, (Expression) Expression.Equal((Expression) Expression.Call(e.GHICHU, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.Trim)), Array.Empty<Expression>()), (Expression) Expression.Call((Expression) Expression.Property((Expression) Expression.Field((Expression) Expression.Constant((object) cDisplayClass122, typeof (UnibenController.\u003C\u003Ec__DisplayClass12_2)), FieldInfo.GetFieldFromHandle(__fieldref (UnibenController.\u003C\u003Ec__DisplayClass12_2.data))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DatabaseTHP.Class.Uniben.Uniben.UnibenOrder.get_orderCode))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.Trim)), Array.Empty<Expression>()))), parameterExpression)).ToListAsync<ct_PhieuDatHang>();
                  if (existingOrderCodes != null && existingOrderCodes.Count > 0)
                  {
                    donHang.DATONTAI = true;
                    donHang.MAPHIEUDATHANG += string.Join(",", (IEnumerable<string>) existingOrderCodes.Select<ct_PhieuDatHang, string>((Func<ct_PhieuDatHang, string>) (e => e.MAPHIEU)).ToList<string>());
                  }
                  lstDonHang.Add(donHang);
                  aspNetUsers = (AspNetUsers) null;
                  KhachHang = (dm_KhachHang) null;
                  existingOrderCodes = (List<ct_PhieuDatHang>) null;
                  PhieuDatHang = (v_ct_PhieuDatHang) null;
                  error = (string) null;
                }
                else
                  return (IActionResult) this.Ok((object) new ApiResponse()
                  {
                    Success = false,
                    Message = $"{order.ID} - {error}",
                    Data = (object) ""
                  });
              }
              khachHangExist = (uniben_dm_LienKet_KhachHang) null;
              nhanVienExist = (uniben_dm_LienKet_NhanVien) null;
            }
            cDisplayClass122 = (UnibenController.\u003C\u003Ec__DisplayClass12_2) null;
          }
          json = (string) null;
        }
        cDisplayClass121 = (UnibenController.\u003C\u003Ec__DisplayClass12_1) null;
        response = (DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse) null;
        donHang = (DatabaseTHP.Class.Uniben.Uniben.DonHang) null;
        order = (Deposit) null;
      }
      unibenOrderListResponse.KhachHang = new List<v_uniben_dm_LienKet_KhachHang>();
      unibenOrderListResponse.KhachHang.AddRange((IEnumerable<v_uniben_dm_LienKet_KhachHang>) response_kh);
      unibenOrderListResponse.HangHoa = new List<v_uniben_dm_LienKet_HangHoa>();
      unibenOrderListResponse.HangHoa.AddRange((IEnumerable<v_uniben_dm_LienKet_HangHoa>) response_hh);
      unibenOrderListResponse.NhanVien = new List<v_uniben_dm_LienKet_NhanVien>();
      unibenOrderListResponse.NhanVien.AddRange((IEnumerable<v_uniben_dm_LienKet_NhanVien>) response_nv);
      unibenOrderListResponse.lstDonHang = new List<DatabaseTHP.Class.Uniben.Uniben.DonHang>();
      unibenOrderListResponse.lstDonHang.AddRange((IEnumerable<DatabaseTHP.Class.Uniben.Uniben.DonHang>) lstDonHang);
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) unibenOrderListResponse
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

  private async Task<(v_ct_PhieuDatHang? Data, string Error)> ConvertOrderToPhieuDatHang(
    DatabaseTHP.Class.Uniben.Uniben.UnibenOrder order,
    v_uniben_dm_LienKet_KhachHang khachHang,
    v_uniben_dm_LienKet_NhanVien nhanVien,
    string LOC_ID)
  {
    try
    {
      if (string.IsNullOrEmpty(khachHang.ID_KHACHHANG))
      {
        khachHang.ID_UNIBEN = order.customerId.ToString();
        khachHang.MAKHACHHANG = order.customerCode;
        khachHang.TENKHACHHANG = order.customerName;
        khachHang.DIACHI = order.deliveryAddress;
        khachHang.MASOTHUE = order.taxNo;
        khachHang.LOC_ID = LOC_ID;
        uniben_dm_LienKet_KhachHang khachHangExist = await this._context.uniben_dm_LienKet_KhachHang.Where<uniben_dm_LienKet_KhachHang>((Expression<Func<uniben_dm_LienKet_KhachHang, bool>>) (e => e.MAKHACHHANG == khachHang.MAKHACHHANG && e.ID_UNIBEN == khachHang.ID_UNIBEN && e.MASOTHUE == khachHang.MASOTHUE && e.LOC_ID == LOC_ID)).FirstOrDefaultAsync<uniben_dm_LienKet_KhachHang>();
        if (khachHangExist == null || string.IsNullOrEmpty(khachHangExist.ID_KHACHHANG))
          return ((v_ct_PhieuDatHang) null, $"Không tìm thấy khách hàng: {khachHang.MAKHACHHANG}-{khachHang.MASOTHUE}");
        khachHang.ID_KHACHHANG = khachHangExist.ID_KHACHHANG;
        khachHangExist = (uniben_dm_LienKet_KhachHang) null;
      }
      if (string.IsNullOrEmpty(nhanVien.ID_NHANVIEN))
      {
        nhanVien.ID_UNIBEN = order.routeCode;
        nhanVien.MANHANVIEN = order.routeCode;
        nhanVien.LOC_ID = LOC_ID;
        uniben_dm_LienKet_NhanVien nhanVienExist = await this._context.uniben_dm_LienKet_NhanVien.Where<uniben_dm_LienKet_NhanVien>((Expression<Func<uniben_dm_LienKet_NhanVien, bool>>) (e => e.ID_UNIBEN == nhanVien.ID_UNIBEN && e.MANHANVIEN == nhanVien.MANHANVIEN && e.LOC_ID == LOC_ID)).FirstOrDefaultAsync<uniben_dm_LienKet_NhanVien>();
        if (nhanVienExist == null || string.IsNullOrEmpty(nhanVienExist.ID_NHANVIEN))
          return ((v_ct_PhieuDatHang) null, "Không tìm thấy nhân viên: " + nhanVien.MANHANVIEN);
        nhanVien.ID_NHANVIEN = nhanVienExist.ID_NHANVIEN;
        nhanVienExist = (uniben_dm_LienKet_NhanVien) null;
      }
      v_ct_PhieuDatHang phieuDatHang = new v_ct_PhieuDatHang();
      phieuDatHang.lstct_PhieuDatHang_ChiTiet = new List<v_ct_PhieuDatHang_ChiTiet>();
      phieuDatHang.ID_NHANVIEN = nhanVien.ID_NHANVIEN;
      phieuDatHang.ID_KHO = "bfbb9565-b14d-429d-9400-4f371d5b82de";
      phieuDatHang.LOC_ID = khachHang.LOC_ID;
      phieuDatHang.ID_KHACHHANG = khachHang.ID_KHACHHANG;
      phieuDatHang.NGAYLAP = DateTime.Now;
      phieuDatHang.GHICHU = order.orderCode;
      dm_KhachHang KhachHang = await this._context.dm_KhachHang.Where<dm_KhachHang>((Expression<Func<dm_KhachHang, bool>>) (e => e.ID == phieuDatHang.ID_KHACHHANG && e.LOC_ID == khachHang.LOC_ID)).FirstOrDefaultAsync<dm_KhachHang>();
      if (KhachHang == null)
        return ((v_ct_PhieuDatHang) null, "Không tìm thấy khách hàng: " + phieuDatHang.ID_KHACHHANG);
      phieuDatHang.ADDRESS = KhachHang.ADDRESS;
      phieuDatHang.TEL = KhachHang.TEL;
      int STT = 1;
      foreach (DatabaseTHP.Class.Uniben.Uniben.UnibenOrderDetail detail in order.details)
      {
        DatabaseTHP.Class.Uniben.Uniben.UnibenOrderDetail item = detail;
        v_ct_PhieuDatHang_ChiTiet chiTiet = new v_ct_PhieuDatHang_ChiTiet();
        if (!string.IsNullOrEmpty(item.productCode))
        {
          uniben_dm_LienKet_HangHoa lienKetHangHoa = await this._context.uniben_dm_LienKet_HangHoa.Where<uniben_dm_LienKet_HangHoa>((Expression<Func<uniben_dm_LienKet_HangHoa, bool>>) (e => e.MAHANGHOA == item.productCode && e.LOC_ID == khachHang.LOC_ID && e.ISKHUYENMAI == !string.IsNullOrEmpty(item.promotionCode))).FirstOrDefaultAsync<uniben_dm_LienKet_HangHoa>();
          if (lienKetHangHoa == null)
            return ((v_ct_PhieuDatHang) null, "Không tìm thấy sản phẩm liên kết: " + item.productCode);
          if (string.IsNullOrEmpty(lienKetHangHoa.ID_HANGHOA))
            return ((v_ct_PhieuDatHang) null, "Sản phẩm liên kết chưa có hàng hóa: " + item.productCode);
          dm_HangHoa hangHoa = await this._context.dm_HangHoa.Where<dm_HangHoa>((Expression<Func<dm_HangHoa, bool>>) (e => e.ID == lienKetHangHoa.ID_HANGHOA && e.LOC_ID == khachHang.LOC_ID)).FirstOrDefaultAsync<dm_HangHoa>();
          if (hangHoa == null)
            return ((v_ct_PhieuDatHang) null, "Không tìm thấy sản phẩm: " + lienKetHangHoa.ID_HANGHOA);
          dm_HangHoa_Kho hangHoaKho = await this._context.dm_HangHoa_Kho.Where<dm_HangHoa_Kho>((Expression<Func<dm_HangHoa_Kho, bool>>) (e => e.ID_HANGHOA == hangHoa.ID && e.ID_KHO == phieuDatHang.ID_KHO && e.LOC_ID == khachHang.LOC_ID)).FirstOrDefaultAsync<dm_HangHoa_Kho>();
          if (hangHoaKho == null)
            return ((v_ct_PhieuDatHang) null, $"Không tìm thấy sản phẩm kho: {hangHoa.ID}-{phieuDatHang.ID_KHO}");
          if (item.qty > 0)
          {
            chiTiet = new v_ct_PhieuDatHang_ChiTiet();
            dm_DonViTinh donViTinh = await this._context.dm_DonViTinh.Where<dm_DonViTinh>((Expression<Func<dm_DonViTinh, bool>>) (e => e.ID == (item.qty > 0 || string.IsNullOrEmpty(hangHoa.ID_DVT_QD) ? hangHoa.ID_DVT : hangHoa.ID_DVT_QD) && e.LOC_ID == khachHang.LOC_ID)).FirstOrDefaultAsync<dm_DonViTinh>();
            if (donViTinh == null)
              return ((v_ct_PhieuDatHang) null, "Không tìm thấy đơn vị tính: " + (item.qty > 0 || string.IsNullOrEmpty(hangHoa.ID_DVT_QD) ? hangHoa.ID_DVT : hangHoa.ID_DVT_QD));
            chiTiet.STT = STT;
            chiTiet.ID_HANGHOAKHO = hangHoaKho.ID;
            chiTiet.ID_HANGHOA = hangHoa.ID;
            chiTiet.ID_DVT = donViTinh.ID;
            chiTiet.NAME = hangHoa.NAME;
            chiTiet.MA = hangHoa.MA;
            chiTiet.NAME_DVT = donViTinh.NAME;
            chiTiet.SOLUONG = (double) item.qty;
            chiTiet.TYLE_QD = (double) item.quantityConversion;
            chiTiet.TONGSOLUONG = (double) (item.qty * item.quantityConversion);
            chiTiet.DONGIA = item.totalAmount > 0M ? Convert.ToDouble(item.salesOutPrice) : 0.0;
            chiTiet.CHIETKHAU = item.totalAmount > 0M ? Convert.ToDouble(item.discountRate) : 0.0;
            chiTiet.TONGTIENGIAMGIA = item.totalAmount > 0M ? Convert.ToDouble(item.discountAmount) : 0.0;
            chiTiet.THANHTIEN = item.totalAmount > 0M ? Convert.ToDouble(item.netAmount) : 0.0;
            chiTiet.TONGCONG = Convert.ToDouble(item.totalAmount);
            phieuDatHang.lstct_PhieuDatHang_ChiTiet.Add(chiTiet);
            ++STT;
            donViTinh = (dm_DonViTinh) null;
          }
          if (item.secondQty > 0)
          {
            chiTiet = new v_ct_PhieuDatHang_ChiTiet();
            dm_DonViTinh donViTinh = await this._context.dm_DonViTinh.Where<dm_DonViTinh>((Expression<Func<dm_DonViTinh, bool>>) (e => e.ID == (item.secondQty > 0 && !string.IsNullOrEmpty(hangHoa.ID_DVT_QD) ? hangHoa.ID_DVT_QD : hangHoa.ID_DVT) && e.LOC_ID == khachHang.LOC_ID)).FirstOrDefaultAsync<dm_DonViTinh>();
            if (donViTinh == null)
              return ((v_ct_PhieuDatHang) null, "Không tìm thấy đơn vị tính: " + (item.secondQty <= 0 || string.IsNullOrEmpty(hangHoa.ID_DVT_QD) ? hangHoa.ID_DVT : hangHoa.ID_DVT_QD));
            chiTiet.STT = STT;
            chiTiet.ID_HANGHOAKHO = hangHoaKho.ID;
            chiTiet.ID_HANGHOA = hangHoa.ID;
            chiTiet.ID_DVT = donViTinh.ID;
            chiTiet.NAME = hangHoa.NAME;
            chiTiet.MA = hangHoa.MA;
            chiTiet.NAME_DVT = donViTinh.NAME;
            chiTiet.SOLUONG = (double) item.secondQty;
            chiTiet.TYLE_QD = 1.0;
            chiTiet.TONGSOLUONG = (double) item.secondQty;
            chiTiet.DONGIA = item.totalAmount > 0M ? Convert.ToDouble(item.secondSalesOutPrice) : 0.0;
            chiTiet.CHIETKHAU = item.totalAmount > 0M ? Convert.ToDouble(item.discountRate) : 0.0;
            chiTiet.TONGTIENGIAMGIA = item.totalAmount > 0M ? Convert.ToDouble(item.discountAmount) : 0.0;
            chiTiet.THANHTIEN = item.totalAmount > 0M ? Convert.ToDouble(item.netAmount) : 0.0;
            chiTiet.TONGCONG = Convert.ToDouble(item.totalAmount);
            phieuDatHang.lstct_PhieuDatHang_ChiTiet.Add(chiTiet);
            ++STT;
            donViTinh = (dm_DonViTinh) null;
          }
          hangHoaKho = (dm_HangHoa_Kho) null;
        }
        else
        {
          chiTiet = new v_ct_PhieuDatHang_ChiTiet();
          view_dm_HangHoa hangHoa = await this._context.view_dm_HangHoa.FirstOrDefaultAsync<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == khachHang.LOC_ID && e.MA == API.GTBH));
          if (hangHoa == null)
            return ((v_ct_PhieuDatHang) null, "Không tìm thấy sản phẩm: " + API.GTBH);
          dm_HangHoa_Kho hangHoaKho = await this._context.dm_HangHoa_Kho.Where<dm_HangHoa_Kho>((Expression<Func<dm_HangHoa_Kho, bool>>) (e => e.ID_HANGHOA == hangHoa.ID && e.ID_KHO == phieuDatHang.ID_KHO && e.LOC_ID == khachHang.LOC_ID)).FirstOrDefaultAsync<dm_HangHoa_Kho>();
          if (hangHoaKho == null)
            return ((v_ct_PhieuDatHang) null, $"Không tìm thấy sản phẩm kho: {hangHoa.ID}-{phieuDatHang.ID_KHO}");
          dm_DonViTinh donViTinh = await this._context.dm_DonViTinh.Where<dm_DonViTinh>((Expression<Func<dm_DonViTinh, bool>>) (e => e.ID == hangHoa.ID_DVT && e.LOC_ID == khachHang.LOC_ID)).FirstOrDefaultAsync<dm_DonViTinh>();
          if (donViTinh == null)
            return ((v_ct_PhieuDatHang) null, "Không tìm thấy đơn vị tính: " + hangHoa.ID_DVT);
          chiTiet.STT = STT;
          chiTiet.ID_HANGHOAKHO = hangHoaKho.ID;
          chiTiet.ID_HANGHOA = hangHoa.ID;
          chiTiet.ID_DVT = hangHoa.ID_DVT;
          chiTiet.NAME = hangHoa.NAME;
          chiTiet.MA = hangHoa.MA;
          chiTiet.NAME_DVT = donViTinh.NAME;
          chiTiet.SOLUONG = 1.0;
          chiTiet.TYLE_QD = 1.0;
          chiTiet.TONGSOLUONG = 1.0;
          chiTiet.DONGIA = 0.0;
          chiTiet.CHIETKHAU = 0.0;
          chiTiet.TONGTIENGIAMGIA = Convert.ToDouble(item.discountAmount);
          chiTiet.THANHTIEN = -1.0 * Convert.ToDouble(item.discountAmount);
          chiTiet.TONGCONG = chiTiet.THANHTIEN;
          phieuDatHang.lstct_PhieuDatHang_ChiTiet.Add(chiTiet);
          ++STT;
          hangHoaKho = (dm_HangHoa_Kho) null;
          donViTinh = (dm_DonViTinh) null;
        }
        chiTiet = (v_ct_PhieuDatHang_ChiTiet) null;
      }
      if (order.personalIncomeAmount > 0M)
      {
        v_ct_PhieuDatHang_ChiTiet chiTiet = new v_ct_PhieuDatHang_ChiTiet();
        view_dm_HangHoa hangHoa = await this._context.view_dm_HangHoa.FirstOrDefaultAsync<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == khachHang.LOC_ID && e.MA == API.TINHTHUE_KM));
        if (hangHoa == null)
          return ((v_ct_PhieuDatHang) null, "Không tìm thấy sản phẩm: " + API.TINHTHUE_KM);
        dm_HangHoa_Kho hangHoaKho = await this._context.dm_HangHoa_Kho.Where<dm_HangHoa_Kho>((Expression<Func<dm_HangHoa_Kho, bool>>) (e => e.ID_HANGHOA == hangHoa.ID && e.ID_KHO == phieuDatHang.ID_KHO && e.LOC_ID == khachHang.LOC_ID)).FirstOrDefaultAsync<dm_HangHoa_Kho>();
        if (hangHoaKho == null)
          return ((v_ct_PhieuDatHang) null, $"Không tìm thấy sản phẩm kho: {hangHoa.ID}-{phieuDatHang.ID_KHO}");
        dm_DonViTinh donViTinh = await this._context.dm_DonViTinh.Where<dm_DonViTinh>((Expression<Func<dm_DonViTinh, bool>>) (e => e.ID == hangHoa.ID_DVT && e.LOC_ID == khachHang.LOC_ID)).FirstOrDefaultAsync<dm_DonViTinh>();
        if (donViTinh == null)
          return ((v_ct_PhieuDatHang) null, "Không tìm thấy đơn vị tính: " + hangHoa.ID_DVT);
        chiTiet.STT = STT;
        chiTiet.ID_HANGHOAKHO = hangHoaKho.ID;
        chiTiet.ID_HANGHOA = hangHoa.ID;
        chiTiet.ID_DVT = hangHoa.ID_DVT;
        chiTiet.NAME = hangHoa.NAME;
        chiTiet.MA = hangHoa.MA;
        chiTiet.NAME_DVT = donViTinh.NAME;
        chiTiet.SOLUONG = 1.0;
        chiTiet.TYLE_QD = 1.0;
        chiTiet.TONGSOLUONG = 1.0;
        chiTiet.DONGIA = Convert.ToDouble(order.personalIncomeAmount);
        chiTiet.CHIETKHAU = 0.0;
        chiTiet.TONGTIENGIAMGIA = 0.0;
        chiTiet.THANHTIEN = chiTiet.DONGIA * chiTiet.SOLUONG;
        chiTiet.TONGCONG = chiTiet.THANHTIEN;
        phieuDatHang.lstct_PhieuDatHang_ChiTiet.Add(chiTiet);
        chiTiet = (v_ct_PhieuDatHang_ChiTiet) null;
        hangHoaKho = (dm_HangHoa_Kho) null;
        donViTinh = (dm_DonViTinh) null;
      }
      phieuDatHang.TONGTIENGIAMGIA = phieuDatHang.lstct_PhieuDatHang_ChiTiet.Sum<v_ct_PhieuDatHang_ChiTiet>((Func<v_ct_PhieuDatHang_ChiTiet, double>) (e => e.TONGTIENGIAMGIA));
      phieuDatHang.TONGTHANHTIEN = phieuDatHang.lstct_PhieuDatHang_ChiTiet.Sum<v_ct_PhieuDatHang_ChiTiet>((Func<v_ct_PhieuDatHang_ChiTiet, double>) (e => e.THANHTIEN));
      phieuDatHang.TONGTIEN = phieuDatHang.lstct_PhieuDatHang_ChiTiet.Sum<v_ct_PhieuDatHang_ChiTiet>((Func<v_ct_PhieuDatHang_ChiTiet, double>) (e => e.TONGCONG));
      phieuDatHang.TONGTIENVAT = phieuDatHang.lstct_PhieuDatHang_ChiTiet.Sum<v_ct_PhieuDatHang_ChiTiet>((Func<v_ct_PhieuDatHang_ChiTiet, double>) (e => e.TONGTIENVAT));
      return (phieuDatHang, "");
    }
    catch (Exception ex)
    {
      return ((v_ct_PhieuDatHang) null, ex.Message);
    }
  }

  private string GetLink(
    string distributorCode,
    int page,
    int pageSize,
    string fromOrderDate,
    string toOrderDate,
    int totalItems,
    string SearchString)
  {
    return $"/admin/sales-outs?page={page}&pageSize={pageSize}&query={SearchString}&sort=orderDate&direction=asc&distributorCode={distributorCode}&salesUserCode=&routeCodes=&customerCodes=&fromOrderDate={fromOrderDate}&toOrderDate={toOrderDate}&status=&totalItems={totalItems}&hasMore=false";
  }

  private string GetLink(string distributorCode, string orderCode)
  {
    return $"/admin/sales-outs/{orderCode}?distributorCode={distributorCode}";
  }

  private string GetLinkSales(string distributorCode, string orderCode)
  {
    return $"/admin/sales-outs?query={orderCode}&distributorCode={distributorCode}";
  }

  [HttpPost("Customer/{LOC_ID}")]
  public async Task<IActionResult> PutCustomer(
    string LOC_ID,
    [FromBody] List<uniben_dm_LienKet_KhachHang> Input)
  {
    try
    {
      if (Input == null || Input.Count == 0)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu truyền vào không hợp lệ!",
          Data = (object) ""
        });
      foreach (uniben_dm_LienKet_KhachHang lienKetKhachHang in Input)
      {
        uniben_dm_LienKet_KhachHang item = lienKetKhachHang;
        uniben_dm_LienKet_KhachHang existingEntity = await this._context.uniben_dm_LienKet_KhachHang.Where<uniben_dm_LienKet_KhachHang>((Expression<Func<uniben_dm_LienKet_KhachHang, bool>>) (e => e.LOC_ID == LOC_ID && e.MAKHACHHANG == item.MAKHACHHANG && e.ID_UNIBEN == item.ID_UNIBEN)).FirstOrDefaultAsync<uniben_dm_LienKet_KhachHang>();
        if (existingEntity != null)
        {
          existingEntity.ID_KHACHHANG = item.ID_KHACHHANG;
          existingEntity.MAKHACHHANG = item.MAKHACHHANG;
          existingEntity.TENKHACHHANG = item.TENKHACHHANG;
          existingEntity.MASOTHUE = item.MASOTHUE;
          existingEntity.ISACTIVE = true;
          this._context.Entry<uniben_dm_LienKet_KhachHang>(existingEntity).State = EntityState.Modified;
        }
        else
        {
          item.LOC_ID = LOC_ID;
          item.ID = Guid.NewGuid().ToString();
          item.ISACTIVE = true;
          this._context.uniben_dm_LienKet_KhachHang.Add(item);
        }
        existingEntity = (uniben_dm_LienKet_KhachHang) null;
      }
      int num = await this._context.SaveChangesAsync();
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) null
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

  [HttpPost("Product/{LOC_ID}")]
  public async Task<IActionResult> PutProduct(string LOC_ID, [FromBody] List<uniben_dm_LienKet_HangHoa> Input)
  {
    try
    {
      if (Input == null || Input.Count == 0)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu truyền vào không hợp lệ!",
          Data = (object) ""
        });
      foreach (uniben_dm_LienKet_HangHoa dmLienKetHangHoa in Input)
      {
        uniben_dm_LienKet_HangHoa item = dmLienKetHangHoa;
        uniben_dm_LienKet_HangHoa existingEntity = await this._context.uniben_dm_LienKet_HangHoa.Where<uniben_dm_LienKet_HangHoa>((Expression<Func<uniben_dm_LienKet_HangHoa, bool>>) (e => e.LOC_ID == LOC_ID && e.MAHANGHOA == item.MAHANGHOA && e.ISKHUYENMAI == item.ISKHUYENMAI && e.ID_UNIBEN == item.ID_UNIBEN)).FirstOrDefaultAsync<uniben_dm_LienKet_HangHoa>();
        if (existingEntity != null)
        {
          existingEntity.ID_HANGHOA = item.ID_HANGHOA;
          existingEntity.MAHANGHOA = item.MAHANGHOA;
          existingEntity.TENHANGHOA = item.TENHANGHOA;
          existingEntity.ISACTIVE = true;
          this._context.Entry<uniben_dm_LienKet_HangHoa>(existingEntity).State = EntityState.Modified;
        }
        else
        {
          item.ISACTIVE = true;
          item.LOC_ID = LOC_ID;
          item.ID = Guid.NewGuid().ToString();
          this._context.uniben_dm_LienKet_HangHoa.Add(item);
        }
        existingEntity = (uniben_dm_LienKet_HangHoa) null;
      }
      int num = await this._context.SaveChangesAsync();
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) null
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

  [HttpPost("Employee/{LOC_ID}")]
  public async Task<IActionResult> PutEmployee(
    string LOC_ID,
    [FromBody] List<uniben_dm_LienKet_NhanVien> Input)
  {
    try
    {
      if (Input == null || Input.Count == 0)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu truyền vào không hợp lệ!",
          Data = (object) ""
        });
      foreach (uniben_dm_LienKet_NhanVien dmLienKetNhanVien in Input)
      {
        uniben_dm_LienKet_NhanVien item = dmLienKetNhanVien;
        uniben_dm_LienKet_NhanVien existingEntity = await this._context.uniben_dm_LienKet_NhanVien.Where<uniben_dm_LienKet_NhanVien>((Expression<Func<uniben_dm_LienKet_NhanVien, bool>>) (e => e.LOC_ID == LOC_ID && e.MANHANVIEN == item.MANHANVIEN && e.ID_UNIBEN == item.ID_UNIBEN)).FirstOrDefaultAsync<uniben_dm_LienKet_NhanVien>();
        if (existingEntity != null)
        {
          existingEntity.ID_NHANVIEN = item.ID_NHANVIEN;
          existingEntity.MANHANVIEN = item.MANHANVIEN;
          existingEntity.TENNHANVIEN = item.TENNHANVIEN;
          existingEntity.ISACTIVE = true;
          this._context.Entry<uniben_dm_LienKet_NhanVien>(existingEntity).State = EntityState.Modified;
        }
        else
        {
          item.ISACTIVE = true;
          item.LOC_ID = LOC_ID;
          item.ID = Guid.NewGuid().ToString();
          this._context.uniben_dm_LienKet_NhanVien.Add(item);
        }
        existingEntity = (uniben_dm_LienKet_NhanVien) null;
      }
      int num = await this._context.SaveChangesAsync();
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) null
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

  [HttpPost("UnibenSalesOrder/{LOC_ID}")]
  public async Task<IActionResult> PutUnibenSalesOrder(string LOC_ID, [FromBody] List<Deposit> Input)
  {
    try
    {
      dm_TaiKhoan_Uniben TaiKhoan = await this._context.dm_TaiKhoan_Uniben.Where<dm_TaiKhoan_Uniben>((Expression<Func<dm_TaiKhoan_Uniben, bool>>) (e => e.LOC_ID == LOC_ID && e.ISACTIVE)).FirstAsync<dm_TaiKhoan_Uniben>();
      if (TaiKhoan == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID} dữ liệu!",
          Data = (object) ""
        });
      if (Input == null || Input.Count == 0)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu truyền vào không hợp lệ!",
          Data = (object) ""
        });
      UnibenService invoiceService = new UnibenService();
      bool bolCheck = await this.CheckToken(TaiKhoan);
      if (!bolCheck)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Không lấy được Token",
          Data = (object) ""
        });
      int Max_ID = this._context.ct_PhieuDatHang.Where<ct_PhieuDatHang>((Expression<Func<ct_PhieuDatHang, bool>>) (e => e.LOC_ID == LOC_ID && e.NGAYLAP.Date == DateTime.Now.Date)).Select<ct_PhieuDatHang, int>((Expression<Func<ct_PhieuDatHang, int>>) (e => e.SOPHIEU)).DefaultIfEmpty<int>().Max<int>();
      foreach (Deposit deposit in Input)
      {
        Deposit item = deposit;
        DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse response = await invoiceService.GetOrderDetail(TaiKhoan.ACCESSTOKEN, TaiKhoan.LINK + this.GetLink(TaiKhoan.ORGANIZATIONUNITID, item.ID));
        if (response != null && response.status == 404)
        {
          bolCheck = await this.CheckToken(TaiKhoan);
          break;
        }
        if (response != null && response.payload != null)
        {
          string json = response.payload.ToString();
          if (!string.IsNullOrWhiteSpace(json))
          {
            DatabaseTHP.Class.Uniben.Uniben.UnibenOrder data = JsonSerializer.Deserialize<DatabaseTHP.Class.Uniben.Uniben.UnibenOrder>(json);
            if (data != null)
            {
              (v_ct_PhieuDatHang, string) valueTuple1 = await this.ConvertOrderToPhieuDatHang(data, new v_uniben_dm_LienKet_KhachHang(), new v_uniben_dm_LienKet_NhanVien(), LOC_ID);
              (v_ct_PhieuDatHang, string) valueTuple2 = valueTuple1;
              v_ct_PhieuDatHang Deposit = valueTuple2.Item1;
              string error = valueTuple2.Item2;
              valueTuple1 = ();
              valueTuple2 = ();
              if (Deposit != null)
              {
                ++Max_ID;
                using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
                {
                  Deposit.ID = Guid.NewGuid().ToString();
                  Deposit.GHICHU = data.orderCode;
                  Deposit.SOPHIEU = Max_ID;
                  Deposit.ID_NGUOITAO = item.ID_NGUOITAO;
                  Deposit.THOIGIANTHEM = new DateTime?(DateTime.Now);
                  bool bolCheckMA = false;
                  while (!bolCheckMA)
                  {
                    Deposit.MAPHIEU = API.GetMaPhieu("Deposit", Deposit.NGAYLAP, Deposit.SOPHIEU);
                    ct_PhieuDatHang check = this._context.ct_PhieuDatHang.Where<ct_PhieuDatHang>((Expression<Func<ct_PhieuDatHang, bool>>) (e => e.LOC_ID == LOC_ID && e.MAPHIEU == Deposit.MAPHIEU)).FirstOrDefault<ct_PhieuDatHang>();
                    if (check != null)
                    {
                      ++Max_ID;
                      Deposit.SOPHIEU = Max_ID;
                    }
                    else
                      bolCheckMA = true;
                    check = (ct_PhieuDatHang) null;
                  }
                  if (Deposit.lstct_PhieuDatHang_ChiTiet != null)
                  {
                    string StrHetSoLuong = "";
                    foreach (v_ct_PhieuDatHang_ChiTiet phieuDatHangChiTiet1 in Deposit.lstct_PhieuDatHang_ChiTiet)
                    {
                      v_ct_PhieuDatHang_ChiTiet itm = phieuDatHangChiTiet1;
                      if (!string.IsNullOrEmpty(itm.ID_THUESUAT))
                      {
                        dm_ThueSuat objVAT = this._context.dm_ThueSuat.FirstOrDefault<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == itm.ID_HANGHOAKHO && e.ID == itm.ID_THUESUAT));
                        itm.THUESUAT = objVAT != null ? objVAT.THUESUAT : itm.THUESUAT;
                        objVAT = (dm_ThueSuat) null;
                      }
                      itm.THANHTIEN = itm.SOLUONG * itm.DONGIA - itm.TONGTIENGIAMGIA;
                      itm.TONGCONG = itm.THANHTIEN + itm.TONGTIENVAT;
                      dm_HangHoa_Kho objdm_HangHoa_Kho = await this._context.dm_HangHoa_Kho.FirstOrDefaultAsync<dm_HangHoa_Kho>((Expression<Func<dm_HangHoa_Kho, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == itm.ID_HANGHOAKHO && e.ID_KHO == Deposit.ID_KHO));
                      if (objdm_HangHoa_Kho != null)
                      {
                        view_dm_HangHoa objdm_HangHoa = this._context.view_dm_HangHoa.FirstOrDefault<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == itm.ID_HANGHOA));
                        if (objdm_HangHoa != null && objdm_HangHoa.LOAIHANGHOA == 2.ToString())
                          itm.TYLE_QD = 0.0;
                        itm.TONGSOLUONG = itm.SOLUONG * itm.TYLE_QD;
                        if (objdm_HangHoa_Kho.QTY >= itm.TONGSOLUONG)
                        {
                          itm.GHICHU = objdm_HangHoa_Kho.QTY.ToString() + ";";
                          objdm_HangHoa_Kho.QTY -= itm.TONGSOLUONG;
                          v_ct_PhieuDatHang_ChiTiet phieuDatHangChiTiet2 = itm;
                          phieuDatHangChiTiet2.GHICHU = $"{phieuDatHangChiTiet2.GHICHU}{objdm_HangHoa_Kho.QTY.ToString()};";
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
                                  Strsoluong = $"{Strsoluong} {(objdm_HangHoa_Kho.QTY - (double) soluong * itm.TYLE_QD).ToString("N0")} {objdm_HangHoa.NAME_DVT_QD}{Environment.NewLine}";
                                else
                                  Strsoluong = $"{Strsoluong}{(objdm_HangHoa_Kho.QTY - (double) soluong * itm.TYLE_QD).ToString("N0")} {objdm_HangHoa.NAME_DVT_QD}";
                              }
                              StrHetSoLuong = $"{StrHetSoLuong}Sản phẩm {itm.NAME} không đủ tồn kho!{Strsoluong}{Environment.NewLine}";
                            }
                            else
                            {
                              Strsoluong = $"{objdm_HangHoa_Kho.QTY.ToString("N0")} {objdm_HangHoa.NAME_DVT}";
                              StrHetSoLuong = $"{StrHetSoLuong}Sản phẩm {itm.NAME} không đủ tồn kho!{Strsoluong}{Environment.NewLine}";
                            }
                          }
                          Strsoluong = (string) null;
                        }
                        objdm_HangHoa = (view_dm_HangHoa) null;
                        itm.ID = Guid.NewGuid().ToString();
                        itm.LOC_ID = Deposit.LOC_ID;
                        itm.ID_PHIEUDATHANG = Deposit.ID;
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
                    StrHetSoLuong = (string) null;
                  }
                  this._context.ct_PhieuDatHang.Add((ct_PhieuDatHang) Deposit);
                  AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
                  auditLog.InserAuditLog();
                  int num = await this._context.SaveChangesAsync();
                  auditLog = (AuditLogController) null;
                  transaction.Commit();
                }
                error = (string) null;
              }
              else
                return (IActionResult) this.Ok((object) new ApiResponse()
                {
                  Success = false,
                  Message = error,
                  Data = (object) ""
                });
            }
            data = (DatabaseTHP.Class.Uniben.Uniben.UnibenOrder) null;
          }
          json = (string) null;
        }
        response = (DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse) null;
        item = (Deposit) null;
      }
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) null
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

  [HttpGet("{LOC_ID}/{Type}")]
  public async Task<IActionResult> PostGetUniben(string LOC_ID, string Type)
  {
    try
    {
      DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse unibenOrderListResponse = new DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse();
      List<v_uniben_dm_LienKet_HangHoa> response_hh = new List<v_uniben_dm_LienKet_HangHoa>();
      List<v_uniben_dm_LienKet_KhachHang> response_kh = new List<v_uniben_dm_LienKet_KhachHang>();
      List<v_uniben_dm_LienKet_NhanVien> response_nv = new List<v_uniben_dm_LienKet_NhanVien>();
      unibenOrderListResponse.HangHoa = new List<v_uniben_dm_LienKet_HangHoa>();
      unibenOrderListResponse.KhachHang = new List<v_uniben_dm_LienKet_KhachHang>();
      unibenOrderListResponse.NhanVien = new List<v_uniben_dm_LienKet_NhanVien>();
      switch (Type)
      {
        case "Product":
          DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse orderListResponse1 = unibenOrderListResponse;
          List<v_uniben_dm_LienKet_HangHoa> dmLienKetHangHoaList = await this.GetProductUniben(LOC_ID);
          orderListResponse1.HangHoa = dmLienKetHangHoaList;
          orderListResponse1 = (DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse) null;
          dmLienKetHangHoaList = (List<v_uniben_dm_LienKet_HangHoa>) null;
          break;
        case "Customer":
          DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse orderListResponse2 = unibenOrderListResponse;
          List<v_uniben_dm_LienKet_KhachHang> lienKetKhachHangList = await this.GetCustomerUniben(LOC_ID);
          orderListResponse2.KhachHang = lienKetKhachHangList;
          orderListResponse2 = (DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse) null;
          lienKetKhachHangList = (List<v_uniben_dm_LienKet_KhachHang>) null;
          break;
        case "Employee":
          DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse orderListResponse3 = unibenOrderListResponse;
          List<v_uniben_dm_LienKet_NhanVien> dmLienKetNhanVienList = await this.GetEmployeeUniben(LOC_ID);
          orderListResponse3.NhanVien = dmLienKetNhanVienList;
          orderListResponse3 = (DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse) null;
          dmLienKetNhanVienList = (List<v_uniben_dm_LienKet_NhanVien>) null;
          break;
      }
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) new List<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse>()
        {
          unibenOrderListResponse
        }
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

  private async Task<List<v_uniben_dm_LienKet_KhachHang>> GetCustomerUniben(string LOC_ID)
  {
    List<v_uniben_dm_LienKet_KhachHang> khachHang = new List<v_uniben_dm_LienKet_KhachHang>();
    dm_TaiKhoan_Uniben TaiKhoan = await this._context.dm_TaiKhoan_Uniben.Where<dm_TaiKhoan_Uniben>((Expression<Func<dm_TaiKhoan_Uniben, bool>>) (e => e.LOC_ID == LOC_ID && e.ISACTIVE)).FirstAsync<dm_TaiKhoan_Uniben>();
    if (TaiKhoan == null)
      return khachHang;
    UnibenService invoiceService = new UnibenService();
    int pageSize = 100;
    int currentPage = 1;
    int lastPage = 1;
    int totalItems = 0;
    do
    {
      DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse response = await invoiceService.GetListSales(TaiKhoan.ACCESSTOKEN, TaiKhoan.LINK + this.GetLinkCustomer(TaiKhoan.ORGANIZATIONUNITID, currentPage, pageSize, totalItems));
      if (response != null && response.status == 404)
      {
        bool bolCheck = await this.CheckToken(TaiKhoan);
        break;
      }
      if (response != null && response.payload != null && currentPage <= response.metaData.lastPage)
      {
        string json = response.payload.ToString();
        if (string.IsNullOrWhiteSpace(json) || !(json.Trim() == "[]"))
        {
          if (response.payload != null && !string.IsNullOrWhiteSpace(json))
          {
            List<DatabaseTHP.Class.Uniben.Uniben.UnibenCustomer> data = JsonSerializer.Deserialize<List<DatabaseTHP.Class.Uniben.Uniben.UnibenCustomer>>(json);
            if (data != null)
            {
              foreach (DatabaseTHP.Class.Uniben.Uniben.UnibenCustomer unibenCustomer in data)
              {
                DatabaseTHP.Class.Uniben.Uniben.UnibenCustomer item = unibenCustomer;
                v_uniben_dm_LienKet_KhachHang existingKH = khachHang.Where<v_uniben_dm_LienKet_KhachHang>((Func<v_uniben_dm_LienKet_KhachHang, bool>) (e => e.MAKHACHHANG == item.customerCode)).FirstOrDefault<v_uniben_dm_LienKet_KhachHang>();
                if (existingKH == null)
                {
                  v_uniben_dm_LienKet_KhachHang khach = new v_uniben_dm_LienKet_KhachHang();
                  khach.ID_UNIBEN = item.customerId.ToString();
                  khach.MAKHACHHANG = item.customerCode;
                  khach.TENKHACHHANG = item.customerName;
                  khach.MASOTHUE = item.taxNo;
                  khach.DIACHI = item.address;
                  khach.ISACTIVE = true;
                  khach.LOC_ID = LOC_ID;
                  uniben_dm_LienKet_KhachHang existingEntity = await this._context.uniben_dm_LienKet_KhachHang.Where<uniben_dm_LienKet_KhachHang>((Expression<Func<uniben_dm_LienKet_KhachHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_UNIBEN == khach.ID_UNIBEN && e.MAKHACHHANG == khach.MAKHACHHANG)).FirstOrDefaultAsync<uniben_dm_LienKet_KhachHang>();
                  if (existingEntity == null)
                    khachHang.Add(khach);
                  existingKH = (v_uniben_dm_LienKet_KhachHang) null;
                  existingEntity = (uniben_dm_LienKet_KhachHang) null;
                }
              }
            }
            data = (List<DatabaseTHP.Class.Uniben.Uniben.UnibenCustomer>) null;
          }
          if (response.metaData != null)
          {
            lastPage = response.metaData.lastPage;
            totalItems = response.metaData.totalItems;
          }
          ++currentPage;
          response = (DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse) null;
          json = (string) null;
        }
        else
          break;
      }
      else
        break;
    }
    while (currentPage <= lastPage);
    return khachHang.OrderBy<v_uniben_dm_LienKet_KhachHang, string>((Func<v_uniben_dm_LienKet_KhachHang, string>) (e => e.TENKHACHHANG)).ToList<v_uniben_dm_LienKet_KhachHang>();
  }

  private string GetLinkCustomer(string distributorCode, int page, int pageSize, int totalItems)
  {
    return $"/admin/customers?sort=&direction=&page={page}&pageSize={pageSize}&query=&distributorCode={distributorCode}&customerCode=&customerName=&routeCode=&userCode=&status=APPROVAL&activeFlag=true&includingRoute=true&totalItems={totalItems}";
  }

  private async Task<List<v_uniben_dm_LienKet_HangHoa>> GetProductUniben(string LOC_ID)
  {
    List<v_uniben_dm_LienKet_HangHoa> khachHang = new List<v_uniben_dm_LienKet_HangHoa>();
    dm_TaiKhoan_Uniben TaiKhoan = await this._context.dm_TaiKhoan_Uniben.Where<dm_TaiKhoan_Uniben>((Expression<Func<dm_TaiKhoan_Uniben, bool>>) (e => e.LOC_ID == LOC_ID && e.ISACTIVE)).FirstAsync<dm_TaiKhoan_Uniben>();
    if (TaiKhoan == null)
      return khachHang;
    UnibenService invoiceService = new UnibenService();
    int pageSize = 100;
    int currentPage = 1;
    int lastPage = 1;
    int totalItems = 0;
    do
    {
      DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse response = await invoiceService.GetListSales(TaiKhoan.ACCESSTOKEN, TaiKhoan.LINK + this.GetLinkProduct(TaiKhoan.ORGANIZATIONUNITID, currentPage, pageSize, totalItems));
      if (response != null && response.status == 404)
      {
        bool bolCheck = await this.CheckToken(TaiKhoan);
        break;
      }
      if (response != null && response.payload != null && currentPage <= response.metaData.lastPage)
      {
        string json = response.payload.ToString();
        if (string.IsNullOrWhiteSpace(json) || !(json.Trim() == "[]"))
        {
          if (response.payload != null && !string.IsNullOrWhiteSpace(json))
          {
            List<DatabaseTHP.Class.Uniben.Uniben.UnibenProduct> data = JsonSerializer.Deserialize<List<DatabaseTHP.Class.Uniben.Uniben.UnibenProduct>>(json);
            if (data != null)
            {
              foreach (DatabaseTHP.Class.Uniben.Uniben.UnibenProduct unibenProduct in data)
              {
                DatabaseTHP.Class.Uniben.Uniben.UnibenProduct item = unibenProduct;
                v_uniben_dm_LienKet_HangHoa existingKH = khachHang.Where<v_uniben_dm_LienKet_HangHoa>((Func<v_uniben_dm_LienKet_HangHoa, bool>) (e => e.MAHANGHOA == item.productCode)).FirstOrDefault<v_uniben_dm_LienKet_HangHoa>();
                if (existingKH == null)
                {
                  v_uniben_dm_LienKet_HangHoa khach = new v_uniben_dm_LienKet_HangHoa();
                  khach.ID_UNIBEN = item.productCode;
                  khach.MAHANGHOA = item.productCode;
                  khach.TENHANGHOA = item.productName;
                  khach.ISACTIVE = true;
                  khach.LOC_ID = LOC_ID;
                  uniben_dm_LienKet_HangHoa existingEntity = await this._context.uniben_dm_LienKet_HangHoa.Where<uniben_dm_LienKet_HangHoa>((Expression<Func<uniben_dm_LienKet_HangHoa, bool>>) (e => e.LOC_ID == LOC_ID && e.MAHANGHOA == khach.MAHANGHOA && e.ID_UNIBEN == khach.ID_UNIBEN)).FirstOrDefaultAsync<uniben_dm_LienKet_HangHoa>();
                  if (existingEntity == null)
                    khachHang.Add(khach);
                  existingKH = (v_uniben_dm_LienKet_HangHoa) null;
                  existingEntity = (uniben_dm_LienKet_HangHoa) null;
                }
              }
            }
            data = (List<DatabaseTHP.Class.Uniben.Uniben.UnibenProduct>) null;
          }
          if (response.metaData != null)
          {
            lastPage = response.metaData.lastPage;
            totalItems = response.metaData.totalItems;
          }
          ++currentPage;
          response = (DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse) null;
          json = (string) null;
        }
        else
          break;
      }
      else
        break;
    }
    while (currentPage <= lastPage);
    return khachHang.OrderBy<v_uniben_dm_LienKet_HangHoa, string>((Func<v_uniben_dm_LienKet_HangHoa, string>) (e => e.TENHANGHOA)).ToList<v_uniben_dm_LienKet_HangHoa>();
  }

  private string GetLinkProduct(string distributorCode, int page, int pageSize, int totalItems)
  {
    return $"/admin/products?page={page}&pageSize={pageSize}&query=&sort=&direction=&productCode=&productName=&distributorCode={distributorCode}&totalItems={totalItems}&includingStock=true&includingPrice=true&promotionFlag=false&posmFlag=false&activeFlag=true";
  }

  private async Task<List<v_uniben_dm_LienKet_NhanVien>> GetEmployeeUniben(string LOC_ID)
  {
    List<v_uniben_dm_LienKet_NhanVien> khachHang = new List<v_uniben_dm_LienKet_NhanVien>();
    dm_TaiKhoan_Uniben TaiKhoan = await this._context.dm_TaiKhoan_Uniben.Where<dm_TaiKhoan_Uniben>((Expression<Func<dm_TaiKhoan_Uniben, bool>>) (e => e.LOC_ID == LOC_ID && e.ISACTIVE)).FirstAsync<dm_TaiKhoan_Uniben>();
    if (TaiKhoan == null)
      return khachHang;
    UnibenService invoiceService = new UnibenService();
    int pageSize = 100;
    int currentPage = 1;
    int lastPage = 1;
    int totalItems = 0;
    do
    {
      DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse response = await invoiceService.GetListSales(TaiKhoan.ACCESSTOKEN, TaiKhoan.LINK + this.GetLinkEmployee(TaiKhoan.ORGANIZATIONUNITID, currentPage, pageSize, totalItems));
      if (response != null && response.status == 404)
      {
        bool bolCheck = await this.CheckToken(TaiKhoan);
        break;
      }
      if (response != null && response.payload != null && currentPage <= response.metaData.lastPage)
      {
        string json = response.payload.ToString();
        if (string.IsNullOrWhiteSpace(json) || !(json.Trim() == "[]"))
        {
          if (response.payload != null && !string.IsNullOrWhiteSpace(json))
          {
            List<DatabaseTHP.Class.Uniben.Uniben.UnibenRoute> data = JsonSerializer.Deserialize<List<DatabaseTHP.Class.Uniben.Uniben.UnibenRoute>>(json);
            if (data != null)
            {
              foreach (DatabaseTHP.Class.Uniben.Uniben.UnibenRoute unibenRoute in data)
              {
                DatabaseTHP.Class.Uniben.Uniben.UnibenRoute item = unibenRoute;
                v_uniben_dm_LienKet_NhanVien existingKH = khachHang.Where<v_uniben_dm_LienKet_NhanVien>((Func<v_uniben_dm_LienKet_NhanVien, bool>) (e => e.MANHANVIEN == item.routeCode)).FirstOrDefault<v_uniben_dm_LienKet_NhanVien>();
                if (existingKH == null)
                {
                  v_uniben_dm_LienKet_NhanVien khach = new v_uniben_dm_LienKet_NhanVien();
                  khach.ID_UNIBEN = item.routeCode;
                  khach.MANHANVIEN = item.routeCode;
                  khach.TENNHANVIEN = string.IsNullOrEmpty(item.salesUserName) ? item.usmName : item.salesUserName;
                  khach.ISACTIVE = true;
                  khach.LOC_ID = LOC_ID;
                  uniben_dm_LienKet_NhanVien existingEntity = await this._context.uniben_dm_LienKet_NhanVien.Where<uniben_dm_LienKet_NhanVien>((Expression<Func<uniben_dm_LienKet_NhanVien, bool>>) (e => e.LOC_ID == LOC_ID && e.MANHANVIEN == khach.MANHANVIEN && e.ID_UNIBEN == khach.ID_UNIBEN)).FirstOrDefaultAsync<uniben_dm_LienKet_NhanVien>();
                  if (existingEntity == null)
                    khachHang.Add(khach);
                  existingKH = (v_uniben_dm_LienKet_NhanVien) null;
                  existingEntity = (uniben_dm_LienKet_NhanVien) null;
                }
              }
            }
            data = (List<DatabaseTHP.Class.Uniben.Uniben.UnibenRoute>) null;
          }
          if (response.metaData != null)
          {
            lastPage = response.metaData.lastPage;
            totalItems = response.metaData.totalItems;
          }
          ++currentPage;
          response = (DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse) null;
          json = (string) null;
        }
        else
          break;
      }
      else
        break;
    }
    while (currentPage <= lastPage);
    return khachHang.OrderBy<v_uniben_dm_LienKet_NhanVien, string>((Func<v_uniben_dm_LienKet_NhanVien, string>) (e => e.TENNHANVIEN)).ToList<v_uniben_dm_LienKet_NhanVien>();
  }

  private string GetLinkEmployee(string distributorCode, int page, int pageSize, int totalItems)
  {
    return $"/admin/sales-routes?page={page}&pageSize={pageSize}&query=&sort=&direction=&distributorCode={distributorCode}&salesRouteCodes=&salesRouteCode=&salesRouteName=&salesTeamCodes=&activeFlag=true&notEmptyFlag=false&vansalesFlag=false";
  }
}
}