using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.Class.Misa;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyApiNetCore6.Controllers;
using MyApiNetCore6.Data;

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
		_context = context;
		_configuration = configuration;
		_httpClient = new HttpClient();
	}

	[HttpPut("{LOC_ID}/{ID}")]
	[Authorize(Roles = "User")]
	public async Task<IActionResult> PutInput(string LOC_ID, string ID)
	{
		try
		{
			dm_TaiKhoan_Misa TaiKhoan = await _context.dm_TaiKhoan_Misa.Where((dm_TaiKhoan_Misa e) => e.LOC_ID == LOC_ID && e.ID == ID).FirstAsync();
			if (TaiKhoan == null)
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Không tìm thấy " + LOC_ID + " dữ liệu!",
					Data = ""
				});
			}
			if (!(await CheckToken(TaiKhoan, bolLayTokenMoi: true)))
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Không lấy được Token",
					Data = ""
				});
			}
			return Ok(new ApiResponse
			{
				Success = true,
				Message = "Success",
				Data = TaiKhoan
			});
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			return Ok(new ApiResponse
			{
				Success = false,
				Message = ex2.Message,
				Data = ""
			});
		}
	}

	[HttpPost("{LOC_ID}")]
	public async Task<IActionResult> Invoice(string LOC_ID, [FromBody] List<Deposit> lstHoaDon)
	{
		try
		{
			dm_TaiKhoan_Misa TaiKhoan = await _context.dm_TaiKhoan_Misa.Where((dm_TaiKhoan_Misa e) => e.LOC_ID == LOC_ID && e.ISACTIVE).FirstAsync();
			if (TaiKhoan == null)
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Không tìm thấy " + LOC_ID + " dữ liệu!",
					Data = ""
				});
			}
			InvoiceService _invoiceService = new InvoiceService();
			if (!(await CheckToken(TaiKhoan)))
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Không lấy được Token",
					Data = ""
				});
			}
			dm_LoaiHoaDon dm_LoaiHoaDon2 = new dm_LoaiHoaDon();
			string Message = "";
			foreach (Deposit hoadon in lstHoaDon)
			{
				List<MisaInvoiceMaster> invoices = new List<MisaInvoiceMaster>();
				ct_HoaDon hoaDon = await _context.ct_HoaDon.Where((ct_HoaDon e) => e.ID == hoadon.ID && e.LOC_ID == hoadon.LOC_ID).FirstOrDefaultAsync();
				if (hoaDon == null || hoaDon.ISXUATHOADON)
				{
					continue;
				}
				if (dm_LoaiHoaDon2.ID != hoaDon.ID_LOAIHOADON)
				{
					dm_LoaiHoaDon2 = await _context.dm_LoaiHoaDon.Where((dm_LoaiHoaDon e) => e.LOC_ID == LOC_ID && e.ISACTIVE).FirstAsync();
				}
				List<ct_HoaDon_ChiTiet> chiTietHoaDon = await _context.ct_HoaDon_ChiTiet.Where((ct_HoaDon_ChiTiet e) => e.ID_HOADON == hoadon.ID && e.LOC_ID == hoadon.LOC_ID).ToListAsync();
				new MisaInvoiceMaster();
				MisaInvoiceMaster invoice = await GetInvoiceMaster(hoaDon, chiTietHoaDon, TaiKhoan, dm_LoaiHoaDon2);
				if (invoice == null)
				{
					continue;
				}
				invoices.Add(invoice);
				if (invoice == null)
				{
					continue;
				}
				MisaApiResponseInvoiced response = await _invoiceService.InsertInvoiceAsync(invoices, TaiKhoan.ACCESSTOKEN, TaiKhoan.MASOTHUE, TaiKhoan.LINK + linkInvoiced);
				if (response?.success ?? false)
				{
					if (!string.IsNullOrEmpty(response.error))
					{
						List<MisaApiResponseInvoiced_Error> apiResponse = JsonSerializer.Deserialize<List<MisaApiResponseInvoiced_Error>>(response.error);
						if (apiResponse != null && apiResponse.Count() > 0 && !string.IsNullOrEmpty(apiResponse[0].ErrorMessage))
						{
							string allSeries = string.Join(" - ", apiResponse.Select((MisaApiResponseInvoiced_Error x) => x.ErrorMessage));
							hoaDon.ERROR = allSeries;
							_context.Entry(hoaDon).State = EntityState.Modified;
							await _context.SaveChangesAsync();
							Message = Message + "(" + hoaDon.MAPHIEU + ") " + hoaDon.ERROR + "\n";
						}
					}
					else
					{
						hoaDon.ERROR = "";
						hoaDon.ISXUATHOADON = true;
						hoaDon.NGAYDAYHOADON = DateTime.Now;
						_context.Entry(hoaDon).State = EntityState.Modified;
						await _context.SaveChangesAsync();
					}
				}
				else
				{
					hoaDon.ERROR = ((response != null) ? (response.error + response.error_description) : "Lỗi không định!");
					_context.Entry(hoaDon).State = EntityState.Modified;
					Message = Message + "(" + hoaDon.MAPHIEU + ") " + hoaDon.ERROR + "\n";
				}
			}
			if (!string.IsNullOrEmpty(Message))
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = Message,
					Data = null
				});
			}
			return Ok(new ApiResponse
			{
				Success = true,
				Message = "Success",
				Data = null
			});
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			return Ok(new ApiResponse
			{
				Success = false,
				Message = ex2.Message,
				Data = ""
			});
		}
	}

	private async Task<MisaInvoiceMaster?> GetInvoiceMaster(ct_HoaDon HoaDon, List<ct_HoaDon_ChiTiet> lstChiTietHoaDon, dm_TaiKhoan_Misa TaiKhoan, dm_LoaiHoaDon dm_LoaiHoaDon)
	{
		try
		{
			decimal exchangeRate = (HoaDon.TYGIA.HasValue ? Convert.ToDecimal(HoaDon.TYGIA.Value) : 1m);
			MisaInvoiceMaster invoice = new MisaInvoiceMaster
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
			foreach (ct_HoaDon_ChiTiet product in lstChiTietHoaDon.OrderBy((ct_HoaDon_ChiTiet s) => s.STT))
			{
				dm_ThueSuat ThueSuat = await _context.dm_ThueSuat.FirstOrDefaultAsync((dm_ThueSuat e) => e.ID == product.ID_THUESUAT && e.LOC_ID == product.LOC_ID);
				MisaInvoiceDetails productDetail = new MisaInvoiceDetails
				{
					InventoryItemType = GetTinhChat(product.TINHCHAT),
					InventoryItemCode = product.MAHANGHOA,
					Description = product.TENHANGHOA,
					SortOrderView = product.STT,
					SortOrder = product.STT,
					IsPromotion = (product.TINHCHAT == 2),
					UnitName = product.DVT,
					Quantity = Convert.ToDecimal(product.SOLUONG),
					UnitPrice = Convert.ToDecimal(product.DONGIA),
					AmountOC = Math.Round(Convert.ToDecimal(product.SOLUONG * product.DONGIA), 0),
					Amount = Math.Round(Convert.ToDecimal(product.SOLUONG * product.DONGIA), 0) * exchangeRate,
					DiscountRate = Convert.ToDecimal(product.CHIETKHAU),
					DiscountAmountOC = ((product.TINHCHAT == 3) ? 0m : Convert.ToDecimal(product.TONGTIENGIAMGIA)),
					DiscountAmount = ((product.TINHCHAT == 3) ? 0m : Convert.ToDecimal(product.TONGTIENGIAMGIA)) * exchangeRate,
					AmountWithoutVATOC = Convert.ToDecimal(product.THANHTIEN),
					AmountWithoutVAT = Convert.ToDecimal(product.THANHTIEN) * exchangeRate,
					VATRate = ((ThueSuat != null) ? new int?(Convert.ToInt16(ThueSuat.MA)) : ((int?)null)),
					VATAmountOC = Convert.ToDecimal(product.TONGTIENVAT),
					VATAmount = Convert.ToDecimal(product.TONGTIENVAT) * exchangeRate,
					InWards = Convert.ToDecimal(product.SOLUONG)
				};
				invoice.InvoiceDetails.Add(productDetail);
			}
			invoice.TotalSaleAmountOC = invoice.InvoiceDetails.Where((MisaInvoiceDetails x) => x.InventoryItemType == 0).Sum((MisaInvoiceDetails s) => s.AmountOC) - invoice.InvoiceDetails.Where((MisaInvoiceDetails x) => x.InventoryItemType == 4).Sum((MisaInvoiceDetails s) => s.AmountOC);
			invoice.TotalSaleAmount = invoice.TotalSaleAmountOC * exchangeRate;
			invoice.TotalDiscountAmountOC = invoice.InvoiceDetails.Where((MisaInvoiceDetails x) => x.InventoryItemType == 0).Sum((MisaInvoiceDetails s) => s.DiscountAmountOC).GetValueOrDefault();
			invoice.TotalDiscountAmount = invoice.TotalDiscountAmountOC * exchangeRate;
			return invoice;
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			Console.WriteLine("Lỗi khi tạo InvoiceMaster: " + ex2.Message);
			return null;
		}
	}

	private int GetTinhChat(int tinhChat)
	{
		if (!Enum.IsDefined(typeof(DatabaseTHP.Class.Misa.TinhChatHangHoa), tinhChat))
		{
			return 1;
		}
		string name = Enum.GetName(typeof(DatabaseTHP.Class.Misa.TinhChatHangHoa), tinhChat);
		if (string.IsNullOrEmpty(name))
		{
			return 1;
		}
		if (Enum.TryParse<TinhChatHangHoa_Misa>(name, out var result))
		{
			return (int)result;
		}
		return 1;
	}

	[HttpGet("{LOC_ID}")]
	public async Task<IActionResult> Template(string LOC_ID)
	{
		try
		{
			dm_TaiKhoan_Misa TaiKhoan = await _context.dm_TaiKhoan_Misa.Where((dm_TaiKhoan_Misa e) => e.LOC_ID == LOC_ID && e.ISACTIVE).FirstAsync();
			if (TaiKhoan == null)
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Không tìm thấy " + LOC_ID + " dữ liệu!",
					Data = ""
				});
			}
			InvoiceService _invoiceService = new InvoiceService();
			MisaTokenRequest request = new MisaTokenRequest
			{
				TaxCode = TaiKhoan.MASOTHUE,
				Username = TaiKhoan.USERNAME,
				Password = TaiKhoan.PASSWORD
			};
			MisaApiResponseInvoiced response = await _invoiceService.GetTemplateAsync(request, TaiKhoan.LINK + linkTemplate);
			return Ok(new ApiResponse
			{
				Success = true,
				Message = "Success",
				Data = ((response != null && response.data != null) ? JsonSerializer.Deserialize<List<MisaInvoiceTemplate>>(response.data) : null)
			});
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			return Ok(new ApiResponse
			{
				Success = false,
				Message = ex2.Message,
				Data = ""
			});
		}
	}

	private async Task<bool> CheckToken(dm_TaiKhoan_Misa TaiKhoan, bool bolLayTokenMoi = false)
	{
		try
		{
			InvoiceService _invoiceService = new InvoiceService();
			MisaTokenRequest request = new MisaTokenRequest
			{
				TaxCode = TaiKhoan.MASOTHUE,
				Username = TaiKhoan.USERNAME,
				Password = TaiKhoan.PASSWORD
			};
			if (string.IsNullOrEmpty(TaiKhoan.ACCESSTOKEN) || string.IsNullOrEmpty(TaiKhoan.USERID) || !TaiKhoan.COMPANYID.HasValue || !TaiKhoan.THOIGIANLAYTOKEN.HasValue || (TaiKhoan.THOIGIANLAYTOKEN.HasValue && TaiKhoan.THOIGIANLAYTOKEN.Value.AddDays(1.0) < DateTime.Now) || bolLayTokenMoi)
			{
				MisaTokenData token = await _invoiceService.GetTokenAsync(request, TaiKhoan.LINK + linkToken);
				if (token == null)
				{
					return false;
				}
				TaiKhoan.ACCESSTOKEN = token.access_token;
				TaiKhoan.EXPIRESIN = token.expires_in;
				TaiKhoan.USERID = token.UserID;
				TaiKhoan.ORGANIZATIONUNITID = token.OrganizationUnitID;
				TaiKhoan.COMPANYID = token.CompanyID;
				TaiKhoan.THOIGIANLAYTOKEN = DateTime.Now.AddMinutes(-1.0);
				_context.Entry(TaiKhoan).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
			}
			return true;
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			Console.WriteLine("Lỗi khi tạo InvoiceMaster: " + ex2.Message);
			return false;
		}
	}

	[HttpPut("{LOC_ID}")]
	[Authorize(Roles = "User")]
	public async Task<IActionResult> PutArea(string LOC_ID, [FromBody] List<Deposit> lstHoaDon)
	{
		try
		{
			bool bolTatca = lstHoaDon.Where((Deposit x) => x.ID == "-1").Count() > 0;
			InvoiceService _invoiceService = new InvoiceService();
			dm_TaiKhoan_Misa TaiKhoan = _context.dm_TaiKhoan_Misa.Where((dm_TaiKhoan_Misa e) => e.LOC_ID == LOC_ID && e.ISACTIVE).FirstOrDefault();
			int KetQua = 0;
			if (TaiKhoan != null)
			{
				if (!(await CheckToken(TaiKhoan)))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không lấy được Token",
						Data = ""
					});
				}
				List<string> lstID_ = lstHoaDon.Select((Deposit x) => x.ID).ToList();
				List<ct_HoaDon> lstValue = _context.ct_HoaDon.Where((ct_HoaDon e) => (bolTatca && e.ISXUATHOADON && string.IsNullOrEmpty(e.MATRACUU_MISA) && string.IsNullOrEmpty(e.ERROR)) || (!bolTatca && lstID_.Contains(e.ID))).ToList();
				List<string> lstID = new List<string>();
				foreach (ct_HoaDon itm in lstValue)
				{
					lstID.Add(itm.ID);
				}
				if (lstID.Count > 0)
				{
					MisaApiResponseInvoiced response = await _invoiceService.GetListInvoiced(lstID, TaiKhoan.ACCESSTOKEN, TaiKhoan.MASOTHUE, TaiKhoan.LINK + linkGetlist);
					if ((response?.success ?? false) && !string.IsNullOrEmpty(response.data))
					{
						List<MisaInvoiceInfo> apiResponse = JsonSerializer.Deserialize<List<MisaInvoiceInfo>>(response.data);
						if (apiResponse != null && apiResponse.Count() > 0)
						{
							foreach (MisaInvoiceInfo item in apiResponse)
							{
								if (!string.IsNullOrEmpty(item.TransactionID))
								{
									ct_HoaDon objUpdate = lstValue.Where((ct_HoaDon e) => e.ID == item.RefID).FirstOrDefault();
									if (objUpdate != null)
									{
										objUpdate.NGAYHOADON = item.InvDate;
										objUpdate.MATRACUU_MISA = item.TransactionID;
										objUpdate.MACQT = item.InvoiceCode;
										objUpdate.SOHOADON = item.InvNo;
										objUpdate.KYHIEUHOADON = item.InvSeries;
										objUpdate.NGAYLAYKETQUA = DateTime.Now;
										_context.ct_HoaDon.Update(objUpdate);
										KetQua++;
									}
								}
							}
							await _context.SaveChangesAsync();
						}
						else if (lstID.Count == 1)
						{
							ct_HoaDon objUpdate2 = lstValue.FirstOrDefault();
							if (objUpdate2 != null)
							{
								objUpdate2.NGAYDAYHOADON = null;
								objUpdate2.ISXUATHOADON = false;
								objUpdate2.MATRACUU_MISA = "";
								objUpdate2.MACQT = "";
								objUpdate2.SOHOADON = "";
								objUpdate2.NGAYLAYKETQUA = DateTime.Now;
								_context.ct_HoaDon.Update(objUpdate2);
								KetQua++;
								await _context.SaveChangesAsync();
							}
						}
					}
				}
			}
			return Ok(new ApiResponse
			{
				Success = true,
				Message = "Success",
				Data = KetQua
			});
		}
		catch (DbUpdateConcurrencyException ex)
		{
			DbUpdateConcurrencyException ex2 = ex;
			return Ok(new ApiResponse
			{
				Success = false,
				Message = ex2.Message,
				Data = ""
			});
		}
	}
}
