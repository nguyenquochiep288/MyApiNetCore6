// Decompiled with JetBrains decompiler
// Type: API_QuanLyTHP.Controllers.Misa.Invoiced_MisaController
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.Class.Misa;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyApiNetCore6.Controllers;
using MyApiNetCore6.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;


namespace API_QuanLyTHP.Controllers.Misa;

[Route("api/[controller]")]
[ApiController]
public class Invoiced_MisaController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;
  private readonly HttpClient _httpClient;
  private string linkToken = "/token";
  private string linkInvoiced = "/insert";
  private string linkGetlist = "/getlist?InvoiceWithCode=true";
  private string linkTemplate = "/templates?invoiceWithCode=true";

  public Invoiced_MisaController(dbTrangHiepPhatContext context, IConfiguration configuration)
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
      dm_TaiKhoan_Misa TaiKhoan = await this._context.dm_TaiKhoan_Misa.Where<dm_TaiKhoan_Misa>((Expression<Func<dm_TaiKhoan_Misa, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID)).FirstAsync<dm_TaiKhoan_Misa>();
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

  [HttpPost("{LOC_ID}")]
  public async Task<IActionResult> Invoice(string LOC_ID, [FromBody] List<Deposit> lstHoaDon)
  {
    try
    {
      dm_TaiKhoan_Misa TaiKhoan = await this._context.dm_TaiKhoan_Misa.Where<dm_TaiKhoan_Misa>((Expression<Func<dm_TaiKhoan_Misa, bool>>) (e => e.LOC_ID == LOC_ID && e.ISACTIVE)).FirstAsync<dm_TaiKhoan_Misa>();
      if (TaiKhoan == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID} dữ liệu!",
          Data = (object) ""
        });
      InvoiceService _invoiceService = new InvoiceService();
      bool bolCheck = await this.CheckToken(TaiKhoan);
      if (!bolCheck)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Không lấy được Token",
          Data = (object) ""
        });
      dm_LoaiHoaDon dm_LoaiHoaDon = new dm_LoaiHoaDon();
      string Message = "";
      foreach (Deposit deposit in lstHoaDon)
      {
        Deposit hoadon = deposit;
        List<MisaInvoiceMaster> invoices = new List<MisaInvoiceMaster>();
        ct_HoaDon hoaDon = await this._context.ct_HoaDon.Where<ct_HoaDon>((Expression<Func<ct_HoaDon, bool>>) (e => e.ID == hoadon.ID && e.LOC_ID == hoadon.LOC_ID)).FirstOrDefaultAsync<ct_HoaDon>();
        if (hoaDon != null && !hoaDon.ISXUATHOADON)
        {
          if (dm_LoaiHoaDon.ID != hoaDon.ID_LOAIHOADON)
            dm_LoaiHoaDon = await this._context.dm_LoaiHoaDon.Where<dm_LoaiHoaDon>((Expression<Func<dm_LoaiHoaDon, bool>>) (e => e.LOC_ID == LOC_ID && e.ISACTIVE)).FirstAsync<dm_LoaiHoaDon>();
          List<ct_HoaDon_ChiTiet> chiTietHoaDon = await this._context.ct_HoaDon_ChiTiet.Where<ct_HoaDon_ChiTiet>((Expression<Func<ct_HoaDon_ChiTiet, bool>>) (e => e.ID_HOADON == hoadon.ID && e.LOC_ID == hoadon.LOC_ID)).ToListAsync<ct_HoaDon_ChiTiet>();
          MisaInvoiceMaster invoice = new MisaInvoiceMaster();
          invoice = await this.GetInvoiceMaster(hoaDon, chiTietHoaDon, TaiKhoan, dm_LoaiHoaDon);
          if (invoice != null)
          {
            invoices.Add(invoice);
            if (invoice != null)
            {
              MisaApiResponseInvoiced response = await _invoiceService.InsertInvoiceAsync(invoices, TaiKhoan.ACCESSTOKEN, TaiKhoan.MASOTHUE, TaiKhoan.LINK + this.linkInvoiced);
              if (response != null && response.success)
              {
                if (!string.IsNullOrEmpty(response.error))
                {
                  List<MisaApiResponseInvoiced_Error> apiResponse = JsonSerializer.Deserialize<List<MisaApiResponseInvoiced_Error>>(response.error);
                  if (apiResponse != null && apiResponse.Count<MisaApiResponseInvoiced_Error>() > 0 && !string.IsNullOrEmpty(apiResponse[0].ErrorMessage))
                  {
                    string allSeries = string.Join(" - ", apiResponse.Select<MisaApiResponseInvoiced_Error, string>((Func<MisaApiResponseInvoiced_Error, string>) (x => x.ErrorMessage)));
                    hoaDon.ERROR = allSeries;
                    this._context.Entry<ct_HoaDon>(hoaDon).State = EntityState.Modified;
                    int num = await this._context.SaveChangesAsync();
                    Message = $"{Message}({hoaDon.MAPHIEU}) {hoaDon.ERROR}\n";
                    allSeries = (string) null;
                  }
                  apiResponse = (List<MisaApiResponseInvoiced_Error>) null;
                }
                else
                {
                  hoaDon.ERROR = "";
                  hoaDon.ISXUATHOADON = true;
                  hoaDon.NGAYDAYHOADON = new DateTime?(DateTime.Now);
                  this._context.Entry<ct_HoaDon>(hoaDon).State = EntityState.Modified;
                  int num = await this._context.SaveChangesAsync();
                }
              }
              else
              {
                hoaDon.ERROR = response != null ? response.error + response.error_description : "Lỗi không định!";
                this._context.Entry<ct_HoaDon>(hoaDon).State = EntityState.Modified;
                Message = $"{Message}({hoaDon.MAPHIEU}) {hoaDon.ERROR}\n";
              }
              response = (MisaApiResponseInvoiced) null;
            }
            chiTietHoaDon = (List<ct_HoaDon_ChiTiet>) null;
            invoice = (MisaInvoiceMaster) null;
          }
          else
            continue;
        }
        invoices = (List<MisaInvoiceMaster>) null;
        hoaDon = (ct_HoaDon) null;
      }
      if (!string.IsNullOrEmpty(Message))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = Message,
          Data = (object) null
        });
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

  private async Task<MisaInvoiceMaster?> GetInvoiceMaster(
    ct_HoaDon HoaDon,
    List<ct_HoaDon_ChiTiet> lstChiTietHoaDon,
    dm_TaiKhoan_Misa TaiKhoan,
    dm_LoaiHoaDon dm_LoaiHoaDon)
  {
    try
    {
      double? tygia = HoaDon.TYGIA;
      Decimal num;
      if (!tygia.HasValue)
      {
        num = 1M;
      }
      else
      {
        tygia = HoaDon.TYGIA;
        num = Convert.ToDecimal(tygia.Value);
      }
      Decimal exchangeRate = num;
      MisaInvoiceMaster invoice = new MisaInvoiceMaster()
      {
        RefID = HoaDon.ID,
        InvoiceTemplateID = dm_LoaiHoaDon.IPTEMPLATEID,
        InvSeries = dm_LoaiHoaDon.INVSERIES,
        InvDate = HoaDon.NGAYLAP.ToString("yyyy-MM-dd"),
        AccountObjectTaxCode = HoaDon.MASOTHUE?.Trim(),
        AccountObjectName = HoaDon.TENDONVI,
        AccountObjectAddress = HoaDon.DIACHI,
        AccountObjectBankAccount = HoaDon.SOTAIKHOAN,
        AccountObjectBankName = HoaDon.TENNGANHANG,
        CitizenIDNumber = HoaDon.CCCD?.Trim(),
        PassportNumber = HoaDon.SOHOCHIEU?.Trim(),
        RelatedUnitCode = HoaDon.MASODONVINGANSACH,
        ContactName = HoaDon.TENKHACHHANG,
        ReceiverEmail = HoaDon.EMAIL?.Trim(),
        ReceiverMobile = HoaDon.DIENTHOAI?.Trim(),
        PaymentMethod = HoaDon.HTTT,
        CurrencyCode = HoaDon.LOAITIEN,
        ExchangeRate = exchangeRate,
        TotalVATAmountOC = Convert.ToDecimal(HoaDon.TONGTIENVAT),
        TotalVATAmount = Convert.ToDecimal(HoaDon.TONGTIENVAT) * exchangeRate,
        TotalAmountOC = Convert.ToDecimal(HoaDon.TONGTIEN),
        TotalAmount = Convert.ToDecimal(HoaDon.TONGTIEN) * exchangeRate,
        CreatedDate = DateTime.Now.ToString("yyyy-MM-dd"),
        ModifiedDate = DateTime.Now.ToString("yyyy-MM-dd"),
        CreatedBy = TaiKhoan.USERID
      };
      invoice.InvoiceDetails = new List<MisaInvoiceDetails>();
      foreach (ct_HoaDon_ChiTiet ctHoaDonChiTiet in (IEnumerable<ct_HoaDon_ChiTiet>) lstChiTietHoaDon.OrderBy<ct_HoaDon_ChiTiet, int>((Func<ct_HoaDon_ChiTiet, int>) (s => s.STT)))
      {
        ct_HoaDon_ChiTiet product = ctHoaDonChiTiet;
        dm_ThueSuat ThueSuat = await this._context.dm_ThueSuat.FirstOrDefaultAsync<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.ID == product.ID_THUESUAT && e.LOC_ID == product.LOC_ID));
        MisaInvoiceDetails productDetail = new MisaInvoiceDetails()
        {
          InventoryItemType = this.GetTinhChat(product.TINHCHAT),
          InventoryItemCode = product.MAHANGHOA,
          Description = product.TENHANGHOA,
          SortOrderView = new int?(product.STT),
          SortOrder = product.STT,
          IsPromotion = product.TINHCHAT == 2,
          UnitName = product.DVT,
          Quantity = Convert.ToDecimal(product.SOLUONG),
          UnitPrice = Convert.ToDecimal(product.DONGIA),
          AmountOC = Math.Round(Convert.ToDecimal(product.SOLUONG * product.DONGIA), 0),
          Amount = Math.Round(Convert.ToDecimal(product.SOLUONG * product.DONGIA), 0) * exchangeRate,
          DiscountRate = new Decimal?(Convert.ToDecimal(product.CHIETKHAU)),
          DiscountAmountOC = new Decimal?(product.TINHCHAT == 3 ? 0M : Convert.ToDecimal(product.TONGTIENGIAMGIA)),
          DiscountAmount = new Decimal?((product.TINHCHAT == 3 ? 0M : Convert.ToDecimal(product.TONGTIENGIAMGIA)) * exchangeRate),
          AmountWithoutVATOC = new Decimal?(Convert.ToDecimal(product.THANHTIEN)),
          AmountWithoutVAT = new Decimal?(Convert.ToDecimal(product.THANHTIEN) * exchangeRate),
          VATRate = ThueSuat != null ? new int?((int) Convert.ToInt16(ThueSuat.MA)) : new int?(),
          VATAmountOC = Convert.ToDecimal(product.TONGTIENVAT),
          VATAmount = Convert.ToDecimal(product.TONGTIENVAT) * exchangeRate,
          InWards = Convert.ToDecimal(product.SOLUONG)
        };
        invoice.InvoiceDetails.Add(productDetail);
        ThueSuat = (dm_ThueSuat) null;
        productDetail = (MisaInvoiceDetails) null;
      }
      invoice.TotalSaleAmountOC = invoice.InvoiceDetails.Where<MisaInvoiceDetails>((Func<MisaInvoiceDetails, bool>) (x => x.InventoryItemType == 0)).Sum<MisaInvoiceDetails>((Func<MisaInvoiceDetails, Decimal>) (s => s.AmountOC)) - invoice.InvoiceDetails.Where<MisaInvoiceDetails>((Func<MisaInvoiceDetails, bool>) (x => x.InventoryItemType == 4)).Sum<MisaInvoiceDetails>((Func<MisaInvoiceDetails, Decimal>) (s => s.AmountOC));
      invoice.TotalSaleAmount = invoice.TotalSaleAmountOC * exchangeRate;
      invoice.TotalDiscountAmountOC = invoice.InvoiceDetails.Where<MisaInvoiceDetails>((Func<MisaInvoiceDetails, bool>) (x => x.InventoryItemType == 0)).Sum<MisaInvoiceDetails>((Func<MisaInvoiceDetails, Decimal?>) (s => s.DiscountAmountOC)).GetValueOrDefault();
      invoice.TotalDiscountAmount = invoice.TotalDiscountAmountOC * exchangeRate;
      return invoice;
    }
    catch (Exception ex)
    {
      Console.WriteLine("Lỗi khi tạo InvoiceMaster: " + ex.Message);
      return (MisaInvoiceMaster) null;
    }
  }

  private int GetTinhChat(int tinhChat)
  {
    if (!Enum.IsDefined(typeof (TinhChatHangHoa), (object) tinhChat))
      return 1;
    string name = Enum.GetName(typeof (TinhChatHangHoa), (object) tinhChat);
    TinhChatHangHoa_Misa result;
    return string.IsNullOrEmpty(name) || !Enum.TryParse<TinhChatHangHoa_Misa>(name, out result) ? 1 : (int) result;
  }

  [HttpGet("{LOC_ID}")]
  public async Task<IActionResult> Template(string LOC_ID)
  {
    try
    {
      dm_TaiKhoan_Misa TaiKhoan = await this._context.dm_TaiKhoan_Misa.Where<dm_TaiKhoan_Misa>((Expression<Func<dm_TaiKhoan_Misa, bool>>) (e => e.LOC_ID == LOC_ID && e.ISACTIVE)).FirstAsync<dm_TaiKhoan_Misa>();
      if (TaiKhoan == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID} dữ liệu!",
          Data = (object) ""
        });
      InvoiceService _invoiceService = new InvoiceService();
      MisaTokenRequest request = new MisaTokenRequest()
      {
        TaxCode = TaiKhoan.MASOTHUE,
        Username = TaiKhoan.USERNAME,
        Password = TaiKhoan.PASSWORD
      };
      MisaApiResponseInvoiced response = await _invoiceService.GetTemplateAsync(request, TaiKhoan.LINK + this.linkTemplate);
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (response == null || response.data == null ? (object) (List<MisaInvoiceTemplate>) null : (object) JsonSerializer.Deserialize<List<MisaInvoiceTemplate>>(response.data))
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

  private async Task<bool> CheckToken(dm_TaiKhoan_Misa TaiKhoan, bool bolLayTokenMoi = false)
  {
    try
    {
      InvoiceService _invoiceService = new InvoiceService();
      MisaTokenRequest request = new MisaTokenRequest()
      {
        TaxCode = TaiKhoan.MASOTHUE,
        Username = TaiKhoan.USERNAME,
        Password = TaiKhoan.PASSWORD
      };
      DateTime now;
      int num1;
      if (!string.IsNullOrEmpty(TaiKhoan.ACCESSTOKEN) && !string.IsNullOrEmpty(TaiKhoan.USERID) && TaiKhoan.COMPANYID.HasValue && TaiKhoan.THOIGIANLAYTOKEN.HasValue)
      {
        if (TaiKhoan.THOIGIANLAYTOKEN.HasValue)
        {
          now = TaiKhoan.THOIGIANLAYTOKEN.Value;
          num1 = now.AddDays(1.0) < DateTime.Now ? 1 : 0;
        }
        else
          num1 = 0;
      }
      else
        num1 = 1;
      int num2 = bolLayTokenMoi ? 1 : 0;
      if ((num1 | num2) != 0)
      {
        MisaTokenData token = await _invoiceService.GetTokenAsync(request, TaiKhoan.LINK + this.linkToken);
        if (token == null)
          return false;
        TaiKhoan.ACCESSTOKEN = token.access_token;
        TaiKhoan.EXPIRESIN = new int?(token.expires_in);
        TaiKhoan.USERID = token.UserID;
        TaiKhoan.ORGANIZATIONUNITID = token.OrganizationUnitID;
        TaiKhoan.COMPANYID = new int?(token.CompanyID);
        dm_TaiKhoan_Misa dmTaiKhoanMisa = TaiKhoan;
        now = DateTime.Now;
        DateTime? nullable = new DateTime?(now.AddMinutes(-1.0));
        dmTaiKhoanMisa.THOIGIANLAYTOKEN = nullable;
        this._context.Entry<dm_TaiKhoan_Misa>(TaiKhoan).State = EntityState.Modified;
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num3 = await this._context.SaveChangesAsync();
        auditLog = (AuditLogController) null;
        token = (MisaTokenData) null;
      }
      return true;
    }
    catch (Exception ex)
    {
      Console.WriteLine("Lỗi khi tạo InvoiceMaster: " + ex.Message);
      return false;
    }
  }

  [HttpPut("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PutArea(string LOC_ID, [FromBody] List<Deposit> lstHoaDon)
  {
    try
    {
      bool bolTatca = lstHoaDon.Where<Deposit>((Func<Deposit, bool>) (x => x.ID == "-1")).Count<Deposit>() > 0;
      InvoiceService _invoiceService = new InvoiceService();
      dm_TaiKhoan_Misa TaiKhoan = this._context.dm_TaiKhoan_Misa.Where<dm_TaiKhoan_Misa>((Expression<Func<dm_TaiKhoan_Misa, bool>>) (e => e.LOC_ID == LOC_ID && e.ISACTIVE)).FirstOrDefault<dm_TaiKhoan_Misa>();
      int KetQua = 0;
      if (TaiKhoan != null)
      {
        bool bolCheck = await this.CheckToken(TaiKhoan);
        if (!bolCheck)
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = "Không lấy được Token",
            Data = (object) ""
          });
        List<string> lstID_ = lstHoaDon.Select<Deposit, string>((Func<Deposit, string>) (x => x.ID)).ToList<string>();
        List<ct_HoaDon> lstValue = this._context.ct_HoaDon.Where<ct_HoaDon>((Expression<Func<ct_HoaDon, bool>>) (e => bolTatca && e.ISXUATHOADON && string.IsNullOrEmpty(e.MATRACUU_MISA) && string.IsNullOrEmpty(e.ERROR) || !bolTatca && lstID_.Contains(e.ID))).ToList<ct_HoaDon>();
        List<string> lstID = new List<string>();
        foreach (ct_HoaDon itm in lstValue)
          lstID.Add(itm.ID);
        if (lstID.Count > 0)
        {
          MisaApiResponseInvoiced response = await _invoiceService.GetListInvoiced(lstID, TaiKhoan.ACCESSTOKEN, TaiKhoan.MASOTHUE, TaiKhoan.LINK + this.linkGetlist);
          if (response != null && response.success && !string.IsNullOrEmpty(response.data))
          {
            List<MisaInvoiceInfo> apiResponse = JsonSerializer.Deserialize<List<MisaInvoiceInfo>>(response.data);
            if (apiResponse != null && apiResponse.Count<MisaInvoiceInfo>() > 0)
            {
              foreach (MisaInvoiceInfo misaInvoiceInfo in apiResponse)
              {
                MisaInvoiceInfo item = misaInvoiceInfo;
                if (!string.IsNullOrEmpty(item.TransactionID))
                {
                  ct_HoaDon objUpdate = lstValue.Where<ct_HoaDon>((Func<ct_HoaDon, bool>) (e => e.ID == item.RefID)).FirstOrDefault<ct_HoaDon>();
                  if (objUpdate != null)
                  {
                    objUpdate.NGAYHOADON = item.InvDate;
                    objUpdate.MATRACUU_MISA = item.TransactionID;
                    objUpdate.MACQT = item.InvoiceCode;
                    objUpdate.SOHOADON = item.InvNo;
                    objUpdate.KYHIEUHOADON = item.InvSeries;
                    objUpdate.NGAYLAYKETQUA = new DateTime?(DateTime.Now);
                    this._context.ct_HoaDon.Update(objUpdate);
                    ++KetQua;
                  }
                  objUpdate = (ct_HoaDon) null;
                }
              }
              int num = await this._context.SaveChangesAsync();
            }
            else if (lstID.Count == 1)
            {
              ct_HoaDon objUpdate = lstValue.FirstOrDefault<ct_HoaDon>();
              if (objUpdate != null)
              {
                objUpdate.NGAYDAYHOADON = new DateTime?();
                objUpdate.ISXUATHOADON = false;
                objUpdate.MATRACUU_MISA = "";
                objUpdate.MACQT = "";
                objUpdate.SOHOADON = "";
                objUpdate.NGAYLAYKETQUA = new DateTime?(DateTime.Now);
                this._context.ct_HoaDon.Update(objUpdate);
                ++KetQua;
                int num = await this._context.SaveChangesAsync();
              }
              objUpdate = (ct_HoaDon) null;
            }
            apiResponse = (List<MisaInvoiceInfo>) null;
          }
          response = (MisaApiResponseInvoiced) null;
        }
        lstValue = (List<ct_HoaDon>) null;
        lstID = (List<string>) null;
      }
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) KetQua
      });
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
}
