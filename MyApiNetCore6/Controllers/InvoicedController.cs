using API_QuanLyTHP.Controllers;
using DatabaseTHP;
using DatabaseTHP.Class;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using MyApiNetCore6.Data;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyApiNetCore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicedController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private string link = "https://testapi.meinvoice.vn/api/integration/webapp";
        private string linkToken =  "/token";
        private string linkInvoiced = "/insert";
        private string linkTemplate = "/templates?invoiceWithCode=true";
        public InvoicedController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        [HttpPost("{LOC_ID}")]
        public async Task<IActionResult> Invoice(string LOC_ID, [FromBody] List<Deposit> lstHoaDon)
        {
            try
            {
                var TaiKhoan = await _context.dm_TaiKhoan_Misa!.Where(e => e.LOC_ID == LOC_ID && e.ISACTIVE).FirstAsync();
                if (TaiKhoan == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + " dữ liệu!",
                        Data = ""
                    });
                }
                link = TaiKhoan.LINK;
                InvoiceService _invoiceService = new InvoiceService();
                bool bolCheck = await CheckToken(TaiKhoan);
                if (!bolCheck)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không lấy được Token",
                        Data = ""
                    });
                }
                var invoices = new List<InvoiceMaster>();
                foreach (Deposit hoadon in lstHoaDon)
                {
                    var hoaDon = await _context.ct_HoaDon!.Where(e => e.ID == hoadon.ID && e.LOC_ID == hoadon.LOC_ID).FirstOrDefaultAsync();
                    if (hoaDon != null)
                    {
                        var chiTietHoaDon = await _context.ct_HoaDon_ChiTiet!.Where(e => e.ID_HOADON == hoadon.ID && e.LOC_ID == hoadon.LOC_ID).ToListAsync();
                        InvoiceMaster? invoice = new InvoiceMaster();
                        invoice = await GetInvoiceMaster(hoaDon, chiTietHoaDon, TaiKhoan);
                        if (invoice != null)
                            invoices.Add(invoice);
                    }
                }
                var response = await _invoiceService.SubmitInvoiceAsync(invoices, TaiKhoan.ACCESSTOKEN, TaiKhoan.MASOTHUE, link + linkInvoiced);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                });
            }
        }
        #region Chuyển dữ liệu hóa đơn sang định dạng Invoiced misa
        private async Task<InvoiceMaster?> GetInvoiceMaster(ct_HoaDon HoaDon, List<ct_HoaDon_ChiTiet> lstChiTietHoaDon, dm_TaiKhoan_Misa TaiKhoan)
        {
            try
            {
                decimal exchangeRate = HoaDon.TYGIA.HasValue ? Convert.ToDecimal(HoaDon.TYGIA.Value) : 1;
                var invoice = new InvoiceMaster
                {
                   RefID = HoaDon.ID,
                     InvoiceTemplateID = "74c16c5d-0532-40ba-b763-58ca5d58f129",
                    InvSeries = "1C25TTH",
                    InvDate = HoaDon.NGAYLAP.ToString(),
                    AccountObjectTaxCode = HoaDon.MASOTHUE,
                    AccountObjectName = HoaDon.TENDONVI,
                    //AccountObjectCode
                    AccountObjectAddress = HoaDon.DIACHI,
                    AccountObjectBankAccount = HoaDon.SOTAIKHOAN,
                    AccountObjectBankName = HoaDon.TENNGANHANG,
                    CitizenIDNumber = HoaDon.CCCD,
                    PassportNumber = HoaDon.SOHOCHIEU,
                    RelatedUnitCode = HoaDon.MASODONVINGANSACH,
                    //SellerShopCode
                    //SellerShopName
                    ContactName = HoaDon.TENKHACHHANG,
                    ReceiverEmail = HoaDon.EMAIL,
                    //ReceiverName
                    ReceiverMobile = HoaDon.DIENTHOAI,
                    PaymentMethod = HoaDon.HTTT,
                    CurrencyCode = HoaDon.LOAITIEN,
                    //DiscountRate
                    ExchangeRate  = exchangeRate,
                    TotalSaleAmountOC = Convert.ToDecimal(HoaDon.TONGTHANHTIEN),
                    TotalSaleAmount = Convert.ToDecimal(HoaDon.TONGTHANHTIEN) * exchangeRate,
                    TotalDiscountAmountOC = Convert.ToDecimal(HoaDon.TONGTIENGIAMGIA),
                    TotalDiscountAmount = Convert.ToDecimal(HoaDon.TONGTIENGIAMGIA) * exchangeRate,
                    TotalVATAmountOC = Convert.ToDecimal(HoaDon.TONGTIENVAT),
                    TotalVATAmount = Convert.ToDecimal(HoaDon.TONGTIENVAT) * exchangeRate,
                    TotalAmountOC = Convert.ToDecimal(HoaDon.TONGTIEN),
                    TotalAmount = Convert.ToDecimal(HoaDon.TONGTIEN) * exchangeRate,
                    CreatedDate = DateTime.Now,
                    CreatedBy = TaiKhoan.USERID,
                };

                foreach (var product in lstChiTietHoaDon)
                {
                    var ThueSuat = await _context.dm_ThueSuat!.FirstOrDefaultAsync(e => e.ID == product.ID_THUESUAT && e.LOC_ID == product.LOC_ID);
                    var productDetail = new InvoiceDetails
                    {
                        InventoryItemType = product.TINHCHAT,
                        InventoryItemCode = product.MAHANGHOA,
                        Description = product.TENHANGHOA,
                        SortOrderView = product.STT,
                        SortOrder = product.STT,
                        IsPromotion = product.TINHCHAT == (int)TinhChatHangHoa.KhuyenMai,
                        UnitName = product.DVT,
                        Quantity = Convert.ToDecimal(product.SOLUONG),
                        UnitPrice = Convert.ToDecimal(product.DONGIA),
                        AmountOC = Convert.ToDecimal(product.THANHTIEN),
                        Amount = Convert.ToDecimal(product.THANHTIEN) * exchangeRate,
                        DiscountRate = Convert.ToDecimal(product.CHIETKHAU),
                        DiscountAmountOC = Convert.ToDecimal(product.TONGTIENGIAMGIA),
                        DiscountAmount = Convert.ToDecimal(product.TONGTIENGIAMGIA) * exchangeRate,
                        VATRate = ThueSuat != null ? Convert.ToInt16(ThueSuat.NAME) : null,
                        VATAmountOC = Convert.ToDecimal(product.TONGTIENVAT),
                        VATAmount = Convert.ToDecimal(product.TONGTIENVAT) * exchangeRate,
                        InWards = Convert.ToDecimal(product.SOLUONG),
                    };
                    invoice.InvoiceDetails.Add(productDetail);
                }
                return invoice;
            }
            catch (Exception ex)
            {
                // Ghi log nếu cần
                Console.WriteLine($"Lỗi khi tạo InvoiceMaster: {ex.Message}");
                return null;
            }
        }
        #endregion

        [HttpGet("{LOC_ID}")]
        public async Task<IActionResult> Template(string LOC_ID)
        {
            try
            {
                var TaiKhoan = await _context.dm_TaiKhoan_Misa!.Where(e => e.LOC_ID == LOC_ID && e.ISACTIVE).FirstAsync();
                if (TaiKhoan == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + " dữ liệu!",
                        Data = ""
                    });
                }
                link = TaiKhoan.LINK;
                InvoiceService _invoiceService = new InvoiceService();
                TokenRequest request = new TokenRequest { TaxCode = TaiKhoan.MASOTHUE, Username = TaiKhoan.USERNAME, Password = TaiKhoan.PASSWORD };
                bool bolCheck = await CheckToken(TaiKhoan);
                if (!bolCheck)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không lấy được Token",
                        Data = ""
                    });
                }
                var response = await _invoiceService.SubmitTemplateAsync(request, link + linkTemplate);

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = (response != null && response.Data != null ? JsonSerializer.Deserialize<List<InvoiceTemplate>>(response.Data) : null)
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                });
            }
        }

        private async Task<Boolean> CheckToken(dm_TaiKhoan_Misa TaiKhoan)
        {
            InvoiceService _invoiceService = new InvoiceService();
            TokenRequest request = new TokenRequest { TaxCode = TaiKhoan.MASOTHUE, Username = TaiKhoan.USERNAME, Password = TaiKhoan.PASSWORD };
            if (string.IsNullOrEmpty(TaiKhoan.ACCESSTOKEN)
              || string.IsNullOrEmpty(TaiKhoan.USERID)
              || TaiKhoan.COMPANYID == null
              || TaiKhoan.THOIGIANLAYTOKEN == null
              || (TaiKhoan.THOIGIANLAYTOKEN != null && TaiKhoan.THOIGIANLAYTOKEN.Value.AddDays(1) > DateTime.Now))
            {
                var token = await _invoiceService.GetTokenAsync(request, link + linkToken);
                if (token != null)
                {
                    TaiKhoan.ACCESSTOKEN = token.AccessToken;
                    TaiKhoan.EXPIRESIN = token.ExpiresIn;
                    TaiKhoan.USERID = token.UserID;
                    TaiKhoan.ORGANIZATIONUNITID = token.OrganizationUnitID;
                    TaiKhoan.COMPANYID = token.ExpiresIn;
                    TaiKhoan.THOIGIANLAYTOKEN = DateTime.Now.AddMinutes(-1);
                    _context.Entry(TaiKhoan).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    AuditLogController auditLog = new AuditLogController(_context, _configuration); auditLog.InserAuditLog(); await _context.SaveChangesAsync();
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}