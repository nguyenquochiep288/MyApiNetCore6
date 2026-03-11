using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.Class.Uniben;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using MyApiNetCore6.Controllers;
using MyApiNetCore6.Data;

namespace API_QuanLyTHP.Controllers.Uniben;

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
			dm_TaiKhoan_Uniben TaiKhoan = await _context.dm_TaiKhoan_Uniben.Where((dm_TaiKhoan_Uniben e) => e.LOC_ID == LOC_ID && e.ID == ID).FirstAsync();
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

	private async Task<bool> CheckToken(dm_TaiKhoan_Uniben TaiKhoan, bool bolLayTokenMoi = false)
	{
		try
		{
			UnibenService _invoiceService = new UnibenService();
			DatabaseTHP.Class.Uniben.Uniben.UnibenTokenRequest request = new DatabaseTHP.Class.Uniben.Uniben.UnibenTokenRequest
			{
				username = TaiKhoan.USERNAME,
				password = TaiKhoan.PASSWORD
			};
			if (string.IsNullOrEmpty(TaiKhoan.ACCESSTOKEN) || !TaiKhoan.THOIGIANLAYTOKEN.HasValue || string.IsNullOrEmpty(TaiKhoan.ORGANIZATIONUNITID) || (TaiKhoan.THOIGIANLAYTOKEN.HasValue && TaiKhoan.THOIGIANLAYTOKEN.Value.Date != DateTime.Now.Date) || bolLayTokenMoi)
			{
				DatabaseTHP.Class.Uniben.Uniben.UnibenToken token = await _invoiceService.GetTokenAsync(request, TaiKhoan.LINK + linkToken);
				if (token == null)
				{
					return false;
				}
				TaiKhoan.ACCESSTOKEN = token.id_token;
				TaiKhoan.ORGANIZATIONUNITID = token.distributorCode;
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

	[HttpGet("{LOC_ID}/{toOrderDate}/{fromOrderDate}/{SearchString}")]
	public async Task<IActionResult> GetListSales(string LOC_ID, string toOrderDate, string fromOrderDate, string SearchString)
	{
		try
		{
			dm_TaiKhoan_Uniben TaiKhoan = await _context.dm_TaiKhoan_Uniben.Where((dm_TaiKhoan_Uniben e) => e.LOC_ID == LOC_ID && e.ISACTIVE).FirstAsync();
			if (TaiKhoan == null)
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Không tìm thấy " + LOC_ID + " dữ liệu!",
					Data = ""
				});
			}
			UnibenService invoiceService = new UnibenService();
			if (!(await CheckToken(TaiKhoan)))
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Không lấy được Token",
					Data = ""
				});
			}
			if (SearchString == "%")
			{
				SearchString = "";
			}
			List<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData> lstUnibenOrderData = new List<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData>();
			int pageSize = 100;
			int currentPage = 1;
			int lastPage = 1;
			int totalItems = 0;
			do
			{
				DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse response = await invoiceService.GetListSales(TaiKhoan.ACCESSTOKEN, TaiKhoan.LINK + GetLink(TaiKhoan.ORGANIZATIONUNITID, currentPage, pageSize, fromOrderDate, toOrderDate, totalItems, SearchString));
				if (response != null && response.status == 404)
				{
					await CheckToken(TaiKhoan);
					break;
				}
				if (response == null || response.payload == null || currentPage > response.metaData.lastPage)
				{
					break;
				}
				string json = response.payload.ToString();
				if (!string.IsNullOrWhiteSpace(json) && json.Trim() == "[]")
				{
					break;
				}
				if (response.payload != null && !string.IsNullOrWhiteSpace(json))
				{
					List<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData> data = JsonSerializer.Deserialize<List<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData>>(json);
					if (data != null)
					{
						foreach (DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData item in data)
						{
							List<ct_PhieuDatHang> existingOrderCodes = await _context.ct_PhieuDatHang.Where((ct_PhieuDatHang e) => e.LOC_ID == LOC_ID && e.GHICHU.Trim() == item.orderCode.Trim()).ToListAsync();
							if (existingOrderCodes != null && existingOrderCodes.Count > 0)
							{
								item.DATONTAI = true;
								item.MAPHIEUDATHANG += string.Join(",", existingOrderCodes.Select((ct_PhieuDatHang e) => e.MAPHIEU).ToList());
							}
						}
						lstUnibenOrderData.AddRange(data);
					}
				}
				if (response.metaData != null)
				{
					lastPage = response.metaData.lastPage;
					totalItems = response.metaData.totalItems;
				}
				currentPage++;
			}
			while (currentPage <= lastPage);
			return Ok(new ApiResponse
			{
				Success = true,
				Message = "Success",
				Data = lstUnibenOrderData
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

	[HttpGet("{LOC_ID}/{Type}/{SearchString}")]
	public async Task<IActionResult> GetLienKet(string LOC_ID, string Type, string SearchString = "")
	{
		try
		{
			DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse unibenOrderListResponse = new DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse
			{
				lstDonHang = new List<DatabaseTHP.Class.Uniben.Uniben.DonHang>(),
				HangHoa = new List<v_uniben_dm_LienKet_HangHoa>(),
				KhachHang = new List<v_uniben_dm_LienKet_KhachHang>(),
				NhanVien = new List<v_uniben_dm_LienKet_NhanVien>()
			};
			if (SearchString == "%")
			{
				SearchString = "";
			}
			switch (Type)
			{
			case "Product":
				foreach (uniben_dm_LienKet_HangHoa item3 in await (from e in _context.uniben_dm_LienKet_HangHoa
					where e.LOC_ID == LOC_ID && e.ISACTIVE && (string.IsNullOrEmpty(SearchString) || e.TENHANGHOA.Contains(SearchString) || e.MAHANGHOA.Contains(SearchString) || e.ID_UNIBEN.Contains(SearchString))
					orderby e.TENHANGHOA
					select e).ToListAsync())
				{
					unibenOrderListResponse.HangHoa.Add(ConvertobjectTodm_LienKet_HangHoa(item3, new v_uniben_dm_LienKet_HangHoa()));
				}
				break;
			case "Customer":
				foreach (uniben_dm_LienKet_KhachHang item2 in await (from e in _context.uniben_dm_LienKet_KhachHang
					where e.LOC_ID == LOC_ID && e.ISACTIVE && (string.IsNullOrEmpty(SearchString) || e.TENKHACHHANG.Contains(SearchString) || e.MASOTHUE.Contains(SearchString) || e.MAKHACHHANG.Contains(SearchString) || e.ID_UNIBEN.Contains(SearchString))
					orderby e.TENKHACHHANG
					select e).ToListAsync())
				{
					unibenOrderListResponse.KhachHang.Add(ConvertobjectTodm_LienKet_KhachHang(item2, new v_uniben_dm_LienKet_KhachHang()));
				}
				break;
			case "Employee":
				foreach (uniben_dm_LienKet_NhanVien item in await (from e in _context.uniben_dm_LienKet_NhanVien
					where e.LOC_ID == LOC_ID && e.ISACTIVE && (string.IsNullOrEmpty(SearchString) || e.TENNHANVIEN.Contains(SearchString) || e.MANHANVIEN.Contains(SearchString) || e.ID_UNIBEN.Contains(SearchString))
					orderby e.TENNHANVIEN
					select e).ToListAsync())
				{
					unibenOrderListResponse.NhanVien.Add(ConvertobjectTodm_LienKet_NhanVien(item, new v_uniben_dm_LienKet_NhanVien()));
				}
				break;
			}
			List<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse> list = new List<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse> { unibenOrderListResponse };
			return Ok(new ApiResponse
			{
				Success = true,
				Message = "Success",
				Data = list
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

	private static v_uniben_dm_LienKet_HangHoa ConvertobjectTodm_LienKet_HangHoa<T>(T objectFrom, v_uniben_dm_LienKet_HangHoa objectTo)
	{
		if (objectFrom != null)
		{
			PropertyInfo[] properties = objectFrom.GetType().GetProperties();
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				if (!(propertyInfo != null))
				{
					continue;
				}
				object value = propertyInfo.GetValue(objectFrom);
				if (value != null)
				{
					PropertyInfo property = objectTo.GetType().GetProperty(propertyInfo.Name);
					if (property != null)
					{
						property.SetValue(objectTo, value);
					}
				}
			}
		}
		return objectTo;
	}

	private static v_uniben_dm_LienKet_KhachHang ConvertobjectTodm_LienKet_KhachHang<T>(T objectFrom, v_uniben_dm_LienKet_KhachHang objectTo)
	{
		if (objectFrom != null)
		{
			PropertyInfo[] properties = objectFrom.GetType().GetProperties();
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				if (!(propertyInfo != null))
				{
					continue;
				}
				object value = propertyInfo.GetValue(objectFrom);
				if (value != null)
				{
					PropertyInfo property = objectTo.GetType().GetProperty(propertyInfo.Name);
					if (property != null)
					{
						property.SetValue(objectTo, value);
					}
				}
			}
		}
		return objectTo;
	}

	private static v_uniben_dm_LienKet_NhanVien ConvertobjectTodm_LienKet_NhanVien<T>(T objectFrom, v_uniben_dm_LienKet_NhanVien objectTo)
	{
		if (objectFrom != null)
		{
			PropertyInfo[] properties = objectFrom.GetType().GetProperties();
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				if (!(propertyInfo != null))
				{
					continue;
				}
				object value = propertyInfo.GetValue(objectFrom);
				if (value != null)
				{
					PropertyInfo property = objectTo.GetType().GetProperty(propertyInfo.Name);
					if (property != null)
					{
						property.SetValue(objectTo, value);
					}
				}
			}
		}
		return objectTo;
	}

	[HttpPost("{LOC_ID}")]
	public async Task<IActionResult> GetInput(string LOC_ID, [FromBody] List<Deposit> Input)
	{
		try
		{
			dm_TaiKhoan_Uniben TaiKhoan = await _context.dm_TaiKhoan_Uniben.Where((dm_TaiKhoan_Uniben e) => e.LOC_ID == LOC_ID && e.ISACTIVE).FirstAsync();
			if (TaiKhoan == null)
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Không tìm thấy " + LOC_ID + " dữ liệu!",
					Data = ""
				});
			}
			UnibenService invoiceService = new UnibenService();
			if (!(await CheckToken(TaiKhoan)))
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Không lấy được Token",
					Data = ""
				});
			}
			DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse unibenOrderListResponse = new DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse();
			List<v_uniben_dm_LienKet_HangHoa> response_hh = new List<v_uniben_dm_LienKet_HangHoa>();
			List<v_uniben_dm_LienKet_KhachHang> response_kh = new List<v_uniben_dm_LienKet_KhachHang>();
			List<v_uniben_dm_LienKet_NhanVien> response_nv = new List<v_uniben_dm_LienKet_NhanVien>();
			List<DatabaseTHP.Class.Uniben.Uniben.DonHang> lstDonHang = new List<DatabaseTHP.Class.Uniben.Uniben.DonHang>();
			DatabaseTHP.Class.Uniben.Uniben.UnibenOrder data;
			foreach (Deposit order in Input)
			{
				DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse response = await invoiceService.GetOrderDetail(TaiKhoan.ACCESSTOKEN, TaiKhoan.LINK + GetLink(TaiKhoan.ORGANIZATIONUNITID, order.ID));
				if (response != null && response.status == 404)
				{
					await CheckToken(TaiKhoan);
					break;
				}
				v_uniben_dm_LienKet_KhachHang khachHang = new v_uniben_dm_LienKet_KhachHang();
				v_uniben_dm_LienKet_HangHoa hangHoa = new v_uniben_dm_LienKet_HangHoa();
				v_uniben_dm_LienKet_NhanVien nhanVien = new v_uniben_dm_LienKet_NhanVien();
				DatabaseTHP.Class.Uniben.Uniben.DonHang donHang = new DatabaseTHP.Class.Uniben.Uniben.DonHang();
				if (response == null || response.payload == null)
				{
					continue;
				}
				bool bolAddOrder = true;
				string json = response.payload.ToString();
				if (string.IsNullOrWhiteSpace(json))
				{
					continue;
				}
				data = JsonSerializer.Deserialize<DatabaseTHP.Class.Uniben.Uniben.UnibenOrder>(json);
				if (data == null)
				{
					continue;
				}
				khachHang = new v_uniben_dm_LienKet_KhachHang();
				khachHang.ID_UNIBEN = data.customerId.ToString();
				khachHang.MAKHACHHANG = data.customerCode;
				khachHang.TENKHACHHANG = data.customerName;
				khachHang.DIACHI = data.deliveryAddress;
				khachHang.MASOTHUE = data.taxNo;
				khachHang.LOC_ID = LOC_ID;
				uniben_dm_LienKet_KhachHang khachHangExist = await _context.uniben_dm_LienKet_KhachHang.Where((uniben_dm_LienKet_KhachHang e) => e.MAKHACHHANG == khachHang.MAKHACHHANG && e.ID_UNIBEN == khachHang.ID_UNIBEN && e.MASOTHUE == khachHang.MASOTHUE && e.LOC_ID == LOC_ID).FirstOrDefaultAsync();
				if (khachHangExist == null || string.IsNullOrEmpty(khachHangExist.ID_KHACHHANG))
				{
					dm_KhachHang khachHangMSTExist = await _context.dm_KhachHang.Where((dm_KhachHang e) => e.MASOTHUE == khachHang.MASOTHUE && e.LOC_ID == LOC_ID).FirstOrDefaultAsync();
					if (khachHangMSTExist != null)
					{
						khachHang.ID_KHACHHANG = khachHangMSTExist.ID;
					}
					v_uniben_dm_LienKet_KhachHang khachHangKHExist = response_kh.Where((v_uniben_dm_LienKet_KhachHang s) => s.MASOTHUE == khachHang.MASOTHUE && !string.IsNullOrEmpty(khachHang.MASOTHUE)).FirstOrDefault();
					if (khachHangKHExist == null)
					{
						bolAddOrder = false;
						response_kh.Add(khachHang);
					}
				}
				else
				{
					khachHang.ID_KHACHHANG = khachHangExist.ID_KHACHHANG;
				}
				nhanVien = new v_uniben_dm_LienKet_NhanVien();
				nhanVien.ID_UNIBEN = data.routeCode;
				nhanVien.MANHANVIEN = data.routeCode;
				nhanVien.LOC_ID = LOC_ID;
				uniben_dm_LienKet_NhanVien nhanVienExist = await _context.uniben_dm_LienKet_NhanVien.Where((uniben_dm_LienKet_NhanVien e) => e.ID_UNIBEN == nhanVien.ID_UNIBEN && e.MANHANVIEN == nhanVien.MANHANVIEN && e.LOC_ID == LOC_ID).FirstOrDefaultAsync();
				if (nhanVienExist == null || string.IsNullOrEmpty(nhanVienExist.ID_NHANVIEN))
				{
					DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse responseSales = await invoiceService.GetListSales(TaiKhoan.ACCESSTOKEN, TaiKhoan.LINK + GetLinkSales(TaiKhoan.ORGANIZATIONUNITID, order.ID));
					if (responseSales != null && responseSales?.payload != null)
					{
						json = responseSales.payload.ToString();
						if (!string.IsNullOrWhiteSpace(json))
						{
							List<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData> salesData = JsonSerializer.Deserialize<List<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderData>>(json);
							if (salesData != null && salesData.Count > 0)
							{
								string salesUserName = salesData[0].salesUserName;
								nhanVien.TENNHANVIEN = salesUserName;
								v_uniben_dm_LienKet_NhanVien khachHangKHExist2 = response_nv.Where((v_uniben_dm_LienKet_NhanVien s) => s.MANHANVIEN == nhanVien.MANHANVIEN && !string.IsNullOrEmpty(nhanVien.MANHANVIEN)).FirstOrDefault();
								if (khachHangKHExist2 == null)
								{
									response_nv.Add(nhanVien);
									bolAddOrder = false;
								}
							}
						}
					}
				}
				else
				{
					nhanVien.ID_NHANVIEN = nhanVienExist.ID_NHANVIEN;
				}
				if (data.details != null && data.details.Count > 0)
				{
					foreach (DatabaseTHP.Class.Uniben.Uniben.UnibenOrderDetail item in data.details)
					{
						hangHoa = new v_uniben_dm_LienKet_HangHoa();
						hangHoa.MAHANGHOA = item.productCode;
						hangHoa.TENHANGHOA = item.productAbbr;
						hangHoa.ID_UNIBEN = item.productCode;
						hangHoa.ISKHUYENMAI = !string.IsNullOrEmpty(item.promotionCode);
						if (hangHoa.MAHANGHOA == null)
						{
							continue;
						}
						uniben_dm_LienKet_HangHoa hangHoaExist = await _context.uniben_dm_LienKet_HangHoa.Where((uniben_dm_LienKet_HangHoa e) => e.MAHANGHOA == hangHoa.MAHANGHOA && e.ID_UNIBEN == hangHoa.ID_UNIBEN && e.ISKHUYENMAI == hangHoa.ISKHUYENMAI && e.LOC_ID == LOC_ID).FirstOrDefaultAsync();
						if (hangHoaExist == null || string.IsNullOrEmpty(hangHoaExist.ID_HANGHOA))
						{
							v_uniben_dm_LienKet_HangHoa hangHoaHHExist = response_hh.Where((v_uniben_dm_LienKet_HangHoa e) => e.MAHANGHOA == hangHoa.MAHANGHOA && e.ISKHUYENMAI == hangHoa.ISKHUYENMAI).FirstOrDefault();
							if (hangHoaHHExist == null)
							{
								bolAddOrder = false;
								response_hh.Add(hangHoa);
							}
						}
					}
				}
				if (!bolAddOrder)
				{
					continue;
				}
				var (PhieuDatHang, error) = await ConvertOrderToPhieuDatHang(data, khachHang, nhanVien, LOC_ID);
				if (PhieuDatHang != null)
				{
					AspNetUsers aspNetUsers = await _context.AspNetUsers.Where((AspNetUsers e) => e.ID == nhanVien.ID_NHANVIEN).FirstOrDefaultAsync();
					dm_KhachHang KhachHang = await _context.dm_KhachHang.Where((dm_KhachHang e) => e.ID == khachHang.ID_KHACHHANG).FirstOrDefaultAsync();
					donHang.DonDatHang = new v_ct_PhieuDatHang();
					donHang.DonDatHang = PhieuDatHang;
					donHang.DonDatHang.NAME_NHANVIEN = ((aspNetUsers != null) ? aspNetUsers.FullName : "");
					donHang.DonDatHang.MA_KHACHHANG = ((KhachHang != null) ? KhachHang.MA : "");
					donHang.DonDatHang.NAME_KHACHHANG = ((KhachHang != null) ? KhachHang.NAME : "");
					donHang.DonDatHang.DIACHI_KHACHHANG = ((KhachHang != null) ? KhachHang.ADDRESS : "");
					donHang.unibenOrder = data;
					List<ct_PhieuDatHang> existingOrderCodes = await _context.ct_PhieuDatHang.Where((ct_PhieuDatHang e) => e.LOC_ID == LOC_ID && e.GHICHU.Trim() == data.orderCode.Trim()).ToListAsync();
					if (existingOrderCodes != null && existingOrderCodes.Count > 0)
					{
						donHang.DATONTAI = true;
						donHang.MAPHIEUDATHANG += string.Join(",", existingOrderCodes.Select((ct_PhieuDatHang e) => e.MAPHIEU).ToList());
					}
					lstDonHang.Add(donHang);
					continue;
				}
				return Ok(new ApiResponse
				{
					Success = false,
					Message = order.ID + " - " + error,
					Data = ""
				});
			}
			unibenOrderListResponse.KhachHang = new List<v_uniben_dm_LienKet_KhachHang>();
			unibenOrderListResponse.KhachHang.AddRange(response_kh);
			unibenOrderListResponse.HangHoa = new List<v_uniben_dm_LienKet_HangHoa>();
			unibenOrderListResponse.HangHoa.AddRange(response_hh);
			unibenOrderListResponse.NhanVien = new List<v_uniben_dm_LienKet_NhanVien>();
			unibenOrderListResponse.NhanVien.AddRange(response_nv);
			unibenOrderListResponse.lstDonHang = new List<DatabaseTHP.Class.Uniben.Uniben.DonHang>();
			unibenOrderListResponse.lstDonHang.AddRange(lstDonHang);
			return Ok(new ApiResponse
			{
				Success = true,
				Message = "Success",
				Data = unibenOrderListResponse
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

	private async Task<(v_ct_PhieuDatHang? Data, string Error)> ConvertOrderToPhieuDatHang(DatabaseTHP.Class.Uniben.Uniben.UnibenOrder order, v_uniben_dm_LienKet_KhachHang khachHang, v_uniben_dm_LienKet_NhanVien nhanVien, string LOC_ID)
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
				uniben_dm_LienKet_KhachHang khachHangExist = await _context.uniben_dm_LienKet_KhachHang.Where((uniben_dm_LienKet_KhachHang e) => e.MAKHACHHANG == khachHang.MAKHACHHANG && e.ID_UNIBEN == khachHang.ID_UNIBEN && e.MASOTHUE == khachHang.MASOTHUE && e.LOC_ID == LOC_ID).FirstOrDefaultAsync();
				if (khachHangExist == null || string.IsNullOrEmpty(khachHangExist.ID_KHACHHANG))
				{
					return (Data: null, Error: "Không tìm thấy khách hàng: " + khachHang.MAKHACHHANG + "-" + khachHang.MASOTHUE);
				}
				khachHang.ID_KHACHHANG = khachHangExist.ID_KHACHHANG;
			}
			if (string.IsNullOrEmpty(nhanVien.ID_NHANVIEN))
			{
				nhanVien.ID_UNIBEN = order.routeCode;
				nhanVien.MANHANVIEN = order.routeCode;
				nhanVien.LOC_ID = LOC_ID;
				uniben_dm_LienKet_NhanVien nhanVienExist = await _context.uniben_dm_LienKet_NhanVien.Where((uniben_dm_LienKet_NhanVien e) => e.ID_UNIBEN == nhanVien.ID_UNIBEN && e.MANHANVIEN == nhanVien.MANHANVIEN && e.LOC_ID == LOC_ID).FirstOrDefaultAsync();
				if (nhanVienExist == null || string.IsNullOrEmpty(nhanVienExist.ID_NHANVIEN))
				{
					return (Data: null, Error: "Không tìm thấy nhân viên: " + nhanVien.MANHANVIEN);
				}
				nhanVien.ID_NHANVIEN = nhanVienExist.ID_NHANVIEN;
			}
			v_ct_PhieuDatHang phieuDatHang = new v_ct_PhieuDatHang();
			phieuDatHang.lstct_PhieuDatHang_ChiTiet = new List<v_ct_PhieuDatHang_ChiTiet>();
			phieuDatHang.ID_NHANVIEN = nhanVien.ID_NHANVIEN;
			phieuDatHang.ID_KHO = "bfbb9565-b14d-429d-9400-4f371d5b82de";
			phieuDatHang.LOC_ID = khachHang.LOC_ID;
			phieuDatHang.ID_KHACHHANG = khachHang.ID_KHACHHANG;
			phieuDatHang.NGAYLAP = DateTime.Now;
			phieuDatHang.GHICHU = order.orderCode;
			dm_KhachHang KhachHang = await _context.dm_KhachHang.Where((dm_KhachHang e) => e.ID == phieuDatHang.ID_KHACHHANG && e.LOC_ID == khachHang.LOC_ID).FirstOrDefaultAsync();
			if (KhachHang == null)
			{
				return (Data: null, Error: "Không tìm thấy khách hàng: " + phieuDatHang.ID_KHACHHANG);
			}
			phieuDatHang.ADDRESS = KhachHang.ADDRESS;
			phieuDatHang.TEL = KhachHang.TEL;
			int STT = 1;
			foreach (DatabaseTHP.Class.Uniben.Uniben.UnibenOrderDetail item in order.details)
			{
				new v_ct_PhieuDatHang_ChiTiet();
				if (!string.IsNullOrEmpty(item.productCode))
				{
					uniben_dm_LienKet_HangHoa lienKetHangHoa = await _context.uniben_dm_LienKet_HangHoa.Where((uniben_dm_LienKet_HangHoa e) => e.MAHANGHOA == item.productCode && e.LOC_ID == khachHang.LOC_ID && e.ISKHUYENMAI == !string.IsNullOrEmpty(item.promotionCode)).FirstOrDefaultAsync();
					if (lienKetHangHoa == null)
					{
						return (Data: null, Error: "Không tìm thấy sản phẩm liên kết: " + item.productCode);
					}
					if (string.IsNullOrEmpty(lienKetHangHoa.ID_HANGHOA))
					{
						return (Data: null, Error: "Sản phẩm liên kết chưa có hàng hóa: " + item.productCode);
					}
					dm_HangHoa hangHoa = await _context.dm_HangHoa.Where((dm_HangHoa e) => e.ID == lienKetHangHoa.ID_HANGHOA && e.LOC_ID == khachHang.LOC_ID).FirstOrDefaultAsync();
					if (hangHoa == null)
					{
						return (Data: null, Error: "Không tìm thấy sản phẩm: " + lienKetHangHoa.ID_HANGHOA);
					}
					dm_HangHoa_Kho hangHoaKho = await _context.dm_HangHoa_Kho.Where((dm_HangHoa_Kho e) => e.ID_HANGHOA == hangHoa.ID && e.ID_KHO == phieuDatHang.ID_KHO && e.LOC_ID == khachHang.LOC_ID).FirstOrDefaultAsync();
					if (hangHoaKho == null)
					{
						return (Data: null, Error: "Không tìm thấy sản phẩm kho: " + hangHoa.ID + "-" + phieuDatHang.ID_KHO);
					}
					if (item.qty > 0)
					{
						v_ct_PhieuDatHang_ChiTiet chiTiet = new v_ct_PhieuDatHang_ChiTiet();
						dm_DonViTinh donViTinh = await _context.dm_DonViTinh.Where((dm_DonViTinh e) => e.ID == ((item.qty > 0 || string.IsNullOrEmpty(hangHoa.ID_DVT_QD)) ? hangHoa.ID_DVT : hangHoa.ID_DVT_QD) && e.LOC_ID == khachHang.LOC_ID).FirstOrDefaultAsync();
						if (donViTinh == null)
						{
							return (Data: null, Error: "Không tìm thấy đơn vị tính: " + ((item.qty > 0 || string.IsNullOrEmpty(hangHoa.ID_DVT_QD)) ? hangHoa.ID_DVT : hangHoa.ID_DVT_QD));
						}
						chiTiet.STT = STT;
						chiTiet.ID_HANGHOAKHO = hangHoaKho.ID;
						chiTiet.ID_HANGHOA = hangHoa.ID;
						chiTiet.ID_DVT = donViTinh.ID;
						chiTiet.NAME = hangHoa.NAME;
						chiTiet.MA = hangHoa.MA;
						chiTiet.NAME_DVT = donViTinh.NAME;
						chiTiet.SOLUONG = item.qty;
						chiTiet.TYLE_QD = item.quantityConversion;
						chiTiet.TONGSOLUONG = item.qty * item.quantityConversion;
						chiTiet.DONGIA = ((item.totalAmount > 0m) ? Convert.ToDouble(item.salesOutPrice) : 0.0);
						chiTiet.CHIETKHAU = ((item.totalAmount > 0m) ? Convert.ToDouble(item.discountRate) : 0.0);
						chiTiet.TONGTIENGIAMGIA = ((item.totalAmount > 0m) ? Convert.ToDouble(item.discountAmount) : 0.0);
						chiTiet.THANHTIEN = ((item.totalAmount > 0m) ? Convert.ToDouble(item.netAmount) : 0.0);
						chiTiet.TONGCONG = Convert.ToDouble(item.totalAmount);
						phieuDatHang.lstct_PhieuDatHang_ChiTiet.Add(chiTiet);
						STT++;
					}
					if (item.secondQty > 0)
					{
						v_ct_PhieuDatHang_ChiTiet chiTiet = new v_ct_PhieuDatHang_ChiTiet();
						dm_DonViTinh donViTinh2 = await _context.dm_DonViTinh.Where((dm_DonViTinh e) => e.ID == ((item.secondQty > 0 && !string.IsNullOrEmpty(hangHoa.ID_DVT_QD)) ? hangHoa.ID_DVT_QD : hangHoa.ID_DVT) && e.LOC_ID == khachHang.LOC_ID).FirstOrDefaultAsync();
						if (donViTinh2 == null)
						{
							return (Data: null, Error: "Không tìm thấy đơn vị tính: " + ((item.secondQty > 0 && !string.IsNullOrEmpty(hangHoa.ID_DVT_QD)) ? hangHoa.ID_DVT_QD : hangHoa.ID_DVT));
						}
						chiTiet.STT = STT;
						chiTiet.ID_HANGHOAKHO = hangHoaKho.ID;
						chiTiet.ID_HANGHOA = hangHoa.ID;
						chiTiet.ID_DVT = donViTinh2.ID;
						chiTiet.NAME = hangHoa.NAME;
						chiTiet.MA = hangHoa.MA;
						chiTiet.NAME_DVT = donViTinh2.NAME;
						chiTiet.SOLUONG = item.secondQty;
						chiTiet.TYLE_QD = 1.0;
						chiTiet.TONGSOLUONG = item.secondQty;
						chiTiet.DONGIA = ((item.totalAmount > 0m) ? Convert.ToDouble(item.secondSalesOutPrice) : 0.0);
						chiTiet.CHIETKHAU = ((item.totalAmount > 0m) ? Convert.ToDouble(item.discountRate) : 0.0);
						chiTiet.TONGTIENGIAMGIA = ((item.totalAmount > 0m) ? Convert.ToDouble(item.discountAmount) : 0.0);
						chiTiet.THANHTIEN = ((item.totalAmount > 0m) ? Convert.ToDouble(item.netAmount) : 0.0);
						chiTiet.TONGCONG = Convert.ToDouble(item.totalAmount);
						phieuDatHang.lstct_PhieuDatHang_ChiTiet.Add(chiTiet);
						STT++;
					}
				}
				else
				{
					v_ct_PhieuDatHang_ChiTiet chiTiet = new v_ct_PhieuDatHang_ChiTiet();
					view_dm_HangHoa hangHoa2 = await _context.view_dm_HangHoa.FirstOrDefaultAsync((view_dm_HangHoa e) => e.LOC_ID == khachHang.LOC_ID && e.MA == API.GTBH);
					if (hangHoa2 == null)
					{
						return (Data: null, Error: "Không tìm thấy sản phẩm: " + API.GTBH);
					}
					dm_HangHoa_Kho hangHoaKho2 = await _context.dm_HangHoa_Kho.Where((dm_HangHoa_Kho e) => e.ID_HANGHOA == hangHoa2.ID && e.ID_KHO == phieuDatHang.ID_KHO && e.LOC_ID == khachHang.LOC_ID).FirstOrDefaultAsync();
					if (hangHoaKho2 == null)
					{
						return (Data: null, Error: "Không tìm thấy sản phẩm kho: " + hangHoa2.ID + "-" + phieuDatHang.ID_KHO);
					}
					dm_DonViTinh donViTinh3 = await _context.dm_DonViTinh.Where((dm_DonViTinh e) => e.ID == hangHoa2.ID_DVT && e.LOC_ID == khachHang.LOC_ID).FirstOrDefaultAsync();
					if (donViTinh3 == null)
					{
						return (Data: null, Error: "Không tìm thấy đơn vị tính: " + hangHoa2.ID_DVT);
					}
					chiTiet.STT = STT;
					chiTiet.ID_HANGHOAKHO = hangHoaKho2.ID;
					chiTiet.ID_HANGHOA = hangHoa2.ID;
					chiTiet.ID_DVT = hangHoa2.ID_DVT;
					chiTiet.NAME = hangHoa2.NAME;
					chiTiet.MA = hangHoa2.MA;
					chiTiet.NAME_DVT = donViTinh3.NAME;
					chiTiet.SOLUONG = 1.0;
					chiTiet.TYLE_QD = 1.0;
					chiTiet.TONGSOLUONG = 1.0;
					chiTiet.DONGIA = 0.0;
					chiTiet.CHIETKHAU = 0.0;
					chiTiet.TONGTIENGIAMGIA = Convert.ToDouble(item.discountAmount);
					chiTiet.THANHTIEN = -1.0 * Convert.ToDouble(item.discountAmount);
					chiTiet.TONGCONG = chiTiet.THANHTIEN;
					phieuDatHang.lstct_PhieuDatHang_ChiTiet.Add(chiTiet);
					STT++;
				}
			}
			if (order.personalIncomeAmount > 0m)
			{
				v_ct_PhieuDatHang_ChiTiet chiTiet2 = new v_ct_PhieuDatHang_ChiTiet();
				view_dm_HangHoa hangHoa3 = await _context.view_dm_HangHoa.FirstOrDefaultAsync((view_dm_HangHoa e) => e.LOC_ID == khachHang.LOC_ID && e.MA == API.TINHTHUE_KM);
				if (hangHoa3 == null)
				{
					return (Data: null, Error: "Không tìm thấy sản phẩm: " + API.TINHTHUE_KM);
				}
				dm_HangHoa_Kho hangHoaKho3 = await _context.dm_HangHoa_Kho.Where((dm_HangHoa_Kho e) => e.ID_HANGHOA == hangHoa3.ID && e.ID_KHO == phieuDatHang.ID_KHO && e.LOC_ID == khachHang.LOC_ID).FirstOrDefaultAsync();
				if (hangHoaKho3 == null)
				{
					return (Data: null, Error: "Không tìm thấy sản phẩm kho: " + hangHoa3.ID + "-" + phieuDatHang.ID_KHO);
				}
				dm_DonViTinh donViTinh4 = await _context.dm_DonViTinh.Where((dm_DonViTinh e) => e.ID == hangHoa3.ID_DVT && e.LOC_ID == khachHang.LOC_ID).FirstOrDefaultAsync();
				if (donViTinh4 == null)
				{
					return (Data: null, Error: "Không tìm thấy đơn vị tính: " + hangHoa3.ID_DVT);
				}
				chiTiet2.STT = STT;
				chiTiet2.ID_HANGHOAKHO = hangHoaKho3.ID;
				chiTiet2.ID_HANGHOA = hangHoa3.ID;
				chiTiet2.ID_DVT = hangHoa3.ID_DVT;
				chiTiet2.NAME = hangHoa3.NAME;
				chiTiet2.MA = hangHoa3.MA;
				chiTiet2.NAME_DVT = donViTinh4.NAME;
				chiTiet2.SOLUONG = 1.0;
				chiTiet2.TYLE_QD = 1.0;
				chiTiet2.TONGSOLUONG = 1.0;
				chiTiet2.DONGIA = Convert.ToDouble(order.personalIncomeAmount);
				chiTiet2.CHIETKHAU = 0.0;
				chiTiet2.TONGTIENGIAMGIA = 0.0;
				chiTiet2.THANHTIEN = chiTiet2.DONGIA * chiTiet2.SOLUONG;
				chiTiet2.TONGCONG = chiTiet2.THANHTIEN;
				phieuDatHang.lstct_PhieuDatHang_ChiTiet.Add(chiTiet2);
			}
			phieuDatHang.TONGTIENGIAMGIA = phieuDatHang.lstct_PhieuDatHang_ChiTiet.Sum((v_ct_PhieuDatHang_ChiTiet e) => e.TONGTIENGIAMGIA);
			phieuDatHang.TONGTHANHTIEN = phieuDatHang.lstct_PhieuDatHang_ChiTiet.Sum((v_ct_PhieuDatHang_ChiTiet e) => e.THANHTIEN);
			phieuDatHang.TONGTIEN = phieuDatHang.lstct_PhieuDatHang_ChiTiet.Sum((v_ct_PhieuDatHang_ChiTiet e) => e.TONGCONG);
			phieuDatHang.TONGTIENVAT = phieuDatHang.lstct_PhieuDatHang_ChiTiet.Sum((v_ct_PhieuDatHang_ChiTiet e) => e.TONGTIENVAT);
			return (Data: phieuDatHang, Error: "");
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			return (Data: null, Error: ex2.Message);
		}
	}

	private string GetLink(string distributorCode, int page, int pageSize, string fromOrderDate, string toOrderDate, int totalItems, string SearchString)
	{
		return $"/admin/sales-outs?page={page}&pageSize={pageSize}&query={SearchString}&sort=orderDate&direction=asc&distributorCode={distributorCode}&salesUserCode=&routeCodes=&customerCodes=&fromOrderDate={fromOrderDate}&toOrderDate={toOrderDate}&status=&totalItems={totalItems}&hasMore=false";
	}

	private string GetLink(string distributorCode, string orderCode)
	{
		return "/admin/sales-outs/" + orderCode + "?distributorCode=" + distributorCode;
	}

	private string GetLinkSales(string distributorCode, string orderCode)
	{
		return "/admin/sales-outs?query=" + orderCode + "&distributorCode=" + distributorCode;
	}

	[HttpPost("Customer/{LOC_ID}")]
	public async Task<IActionResult> PutCustomer(string LOC_ID, [FromBody] List<uniben_dm_LienKet_KhachHang> Input)
	{
		try
		{
			if (Input == null || Input.Count == 0)
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Dữ liệu truyền vào không hợp lệ!",
					Data = ""
				});
			}
			foreach (uniben_dm_LienKet_KhachHang item in Input)
			{
				uniben_dm_LienKet_KhachHang existingEntity = await _context.uniben_dm_LienKet_KhachHang.Where((uniben_dm_LienKet_KhachHang e) => e.LOC_ID == LOC_ID && e.MAKHACHHANG == item.MAKHACHHANG && e.ID_UNIBEN == item.ID_UNIBEN).FirstOrDefaultAsync();
				if (existingEntity != null)
				{
					existingEntity.ID_KHACHHANG = item.ID_KHACHHANG;
					existingEntity.MAKHACHHANG = item.MAKHACHHANG;
					existingEntity.TENKHACHHANG = item.TENKHACHHANG;
					existingEntity.MASOTHUE = item.MASOTHUE;
					existingEntity.ISACTIVE = true;
					_context.Entry(existingEntity).State = EntityState.Modified;
				}
				else
				{
					item.LOC_ID = LOC_ID;
					item.ID = Guid.NewGuid().ToString();
					item.ISACTIVE = true;
					_context.uniben_dm_LienKet_KhachHang.Add(item);
				}
			}
			await _context.SaveChangesAsync();
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

	[HttpPost("Product/{LOC_ID}")]
	public async Task<IActionResult> PutProduct(string LOC_ID, [FromBody] List<uniben_dm_LienKet_HangHoa> Input)
	{
		try
		{
			if (Input == null || Input.Count == 0)
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Dữ liệu truyền vào không hợp lệ!",
					Data = ""
				});
			}
			foreach (uniben_dm_LienKet_HangHoa item in Input)
			{
				uniben_dm_LienKet_HangHoa existingEntity = await _context.uniben_dm_LienKet_HangHoa.Where((uniben_dm_LienKet_HangHoa e) => e.LOC_ID == LOC_ID && e.MAHANGHOA == item.MAHANGHOA && e.ISKHUYENMAI == item.ISKHUYENMAI && e.ID_UNIBEN == item.ID_UNIBEN).FirstOrDefaultAsync();
				if (existingEntity != null)
				{
					existingEntity.ID_HANGHOA = item.ID_HANGHOA;
					existingEntity.MAHANGHOA = item.MAHANGHOA;
					existingEntity.TENHANGHOA = item.TENHANGHOA;
					existingEntity.ISACTIVE = true;
					_context.Entry(existingEntity).State = EntityState.Modified;
				}
				else
				{
					item.ISACTIVE = true;
					item.LOC_ID = LOC_ID;
					item.ID = Guid.NewGuid().ToString();
					_context.uniben_dm_LienKet_HangHoa.Add(item);
				}
			}
			await _context.SaveChangesAsync();
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

	[HttpPost("Employee/{LOC_ID}")]
	public async Task<IActionResult> PutEmployee(string LOC_ID, [FromBody] List<uniben_dm_LienKet_NhanVien> Input)
	{
		try
		{
			if (Input == null || Input.Count == 0)
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Dữ liệu truyền vào không hợp lệ!",
					Data = ""
				});
			}
			foreach (uniben_dm_LienKet_NhanVien item in Input)
			{
				uniben_dm_LienKet_NhanVien existingEntity = await _context.uniben_dm_LienKet_NhanVien.Where((uniben_dm_LienKet_NhanVien e) => e.LOC_ID == LOC_ID && e.MANHANVIEN == item.MANHANVIEN && e.ID_UNIBEN == item.ID_UNIBEN).FirstOrDefaultAsync();
				if (existingEntity != null)
				{
					existingEntity.ID_NHANVIEN = item.ID_NHANVIEN;
					existingEntity.MANHANVIEN = item.MANHANVIEN;
					existingEntity.TENNHANVIEN = item.TENNHANVIEN;
					existingEntity.ISACTIVE = true;
					_context.Entry(existingEntity).State = EntityState.Modified;
				}
				else
				{
					item.ISACTIVE = true;
					item.LOC_ID = LOC_ID;
					item.ID = Guid.NewGuid().ToString();
					_context.uniben_dm_LienKet_NhanVien.Add(item);
				}
			}
			await _context.SaveChangesAsync();
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

	[HttpPost("UnibenSalesOrder/{LOC_ID}")]
	public async Task<IActionResult> PutUnibenSalesOrder(string LOC_ID, [FromBody] List<Deposit> Input)
	{
		try
		{
			dm_TaiKhoan_Uniben TaiKhoan = await _context.dm_TaiKhoan_Uniben.Where((dm_TaiKhoan_Uniben e) => e.LOC_ID == LOC_ID && e.ISACTIVE).FirstAsync();
			if (TaiKhoan == null)
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Không tìm thấy " + LOC_ID + " dữ liệu!",
					Data = ""
				});
			}
			if (Input == null || Input.Count == 0)
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Dữ liệu truyền vào không hợp lệ!",
					Data = ""
				});
			}
			UnibenService invoiceService = new UnibenService();
			if (!(await CheckToken(TaiKhoan)))
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Không lấy được Token",
					Data = ""
				});
			}
			int Max_ID = (from e in _context.ct_PhieuDatHang
				where e.LOC_ID == LOC_ID && e.NGAYLAP.Date == DateTime.Now.Date
				select e.SOPHIEU).DefaultIfEmpty().Max();
			v_ct_PhieuDatHang Deposit;
			foreach (Deposit item in Input)
			{
				DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse response = await invoiceService.GetOrderDetail(TaiKhoan.ACCESSTOKEN, TaiKhoan.LINK + GetLink(TaiKhoan.ORGANIZATIONUNITID, item.ID));
				if (response != null && response.status == 404)
				{
					await CheckToken(TaiKhoan);
					break;
				}
				if (response == null || response.payload == null)
				{
					continue;
				}
				string json = response.payload.ToString();
				if (string.IsNullOrWhiteSpace(json))
				{
					continue;
				}
				DatabaseTHP.Class.Uniben.Uniben.UnibenOrder data = JsonSerializer.Deserialize<DatabaseTHP.Class.Uniben.Uniben.UnibenOrder>(json);
				if (data == null)
				{
					continue;
				}
				string error;
				(Deposit, error) = await ConvertOrderToPhieuDatHang(data, new v_uniben_dm_LienKet_KhachHang(), new v_uniben_dm_LienKet_NhanVien(), LOC_ID);
				if (Deposit != null)
				{
					Max_ID++;
					using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
					{
						Deposit.ID = Guid.NewGuid().ToString();
						Deposit.GHICHU = data.orderCode;
						Deposit.SOPHIEU = Max_ID;
						Deposit.ID_NGUOITAO = item.ID_NGUOITAO;
						Deposit.THOIGIANTHEM = DateTime.Now;
						bool bolCheckMA = false;
						while (!bolCheckMA)
						{
							Deposit.MAPHIEU = API.GetMaPhieu("Deposit", Deposit.NGAYLAP, Deposit.SOPHIEU);
							ct_PhieuDatHang check = _context.ct_PhieuDatHang.Where((ct_PhieuDatHang e) => e.LOC_ID == LOC_ID && e.MAPHIEU == Deposit.MAPHIEU).FirstOrDefault();
							if (check != null)
							{
								Max_ID++;
								Deposit.SOPHIEU = Max_ID;
							}
							else
							{
								bolCheckMA = true;
							}
						}
						if (Deposit.lstct_PhieuDatHang_ChiTiet != null)
						{
							string StrHetSoLuong = "";
							foreach (v_ct_PhieuDatHang_ChiTiet itm in Deposit.lstct_PhieuDatHang_ChiTiet)
							{
								if (!string.IsNullOrEmpty(itm.ID_THUESUAT))
								{
									dm_ThueSuat objVAT = _context.dm_ThueSuat.FirstOrDefault((dm_ThueSuat e) => e.LOC_ID == LOC_ID && e.ID == itm.ID_HANGHOAKHO && e.ID == itm.ID_THUESUAT);
									itm.THUESUAT = objVAT?.THUESUAT ?? itm.THUESUAT;
								}
								itm.THANHTIEN = itm.SOLUONG * itm.DONGIA - itm.TONGTIENGIAMGIA;
								itm.TONGCONG = itm.THANHTIEN + itm.TONGTIENVAT;
								dm_HangHoa_Kho objdm_HangHoa_Kho = await _context.dm_HangHoa_Kho.FirstOrDefaultAsync((dm_HangHoa_Kho e) => e.LOC_ID == LOC_ID && e.ID == itm.ID_HANGHOAKHO && e.ID_KHO == Deposit.ID_KHO);
								if (objdm_HangHoa_Kho != null)
								{
									view_dm_HangHoa objdm_HangHoa = _context.view_dm_HangHoa.FirstOrDefault((view_dm_HangHoa e) => e.LOC_ID == LOC_ID && e.ID == itm.ID_HANGHOA);
									if (objdm_HangHoa != null && objdm_HangHoa.LOAIHANGHOA == 2.ToString())
									{
										itm.TYLE_QD = 0.0;
									}
									itm.TONGSOLUONG = itm.SOLUONG * itm.TYLE_QD;
									if (objdm_HangHoa_Kho.QTY >= itm.TONGSOLUONG)
									{
										itm.GHICHU = objdm_HangHoa_Kho.QTY + ";";
										objdm_HangHoa_Kho.QTY -= itm.TONGSOLUONG;
										v_ct_PhieuDatHang_ChiTiet obj = itm;
										obj.GHICHU = obj.GHICHU + objdm_HangHoa_Kho.QTY + ";";
										_context.Entry(objdm_HangHoa_Kho).State = EntityState.Modified;
									}
									else
									{
										string Strsoluong = "";
										if (objdm_HangHoa != null && itm.TYLE_QD >= 1.0)
										{
											if (itm.TYLE_QD > 1.0)
											{
												int soluong = Convert.ToInt32(objdm_HangHoa_Kho.QTY) / Convert.ToInt32(itm.TYLE_QD);
												if (soluong > 0)
												{
													Strsoluong = soluong.ToString("N0") + " " + objdm_HangHoa.NAME_DVT;
												}
												if (objdm_HangHoa_Kho.QTY - (double)soluong * itm.TYLE_QD > 0.0)
												{
													Strsoluong = (string.IsNullOrEmpty(Strsoluong) ? (Strsoluong + (objdm_HangHoa_Kho.QTY - (double)soluong * itm.TYLE_QD).ToString("N0") + " " + objdm_HangHoa.NAME_DVT_QD) : (Strsoluong + " " + (objdm_HangHoa_Kho.QTY - (double)soluong * itm.TYLE_QD).ToString("N0") + " " + objdm_HangHoa.NAME_DVT_QD + Environment.NewLine));
												}
												StrHetSoLuong = StrHetSoLuong + "Sản phẩm " + itm.NAME + " không đủ tồn kho!" + Strsoluong + Environment.NewLine;
											}
											else
											{
												Strsoluong = objdm_HangHoa_Kho.QTY.ToString("N0") + " " + objdm_HangHoa.NAME_DVT;
												StrHetSoLuong = StrHetSoLuong + "Sản phẩm " + itm.NAME + " không đủ tồn kho!" + Strsoluong + Environment.NewLine;
											}
										}
									}
									itm.ID = Guid.NewGuid().ToString();
									itm.LOC_ID = Deposit.LOC_ID;
									itm.ID_PHIEUDATHANG = Deposit.ID;
									_context.ct_PhieuDatHang_ChiTiet.Add(itm);
									continue;
								}
								return Ok(new ApiResponse
								{
									Success = false,
									Message = "Không tìm thấy sản phẩm kho!" + itm.ID_HANGHOAKHO,
									Data = ""
								});
							}
							if (!string.IsNullOrEmpty(StrHetSoLuong))
							{
								return Ok(new ApiResponse
								{
									Success = false,
									Message = StrHetSoLuong,
									Data = ""
								});
							}
							Deposit.TONGTHANHTIEN = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum((v_ct_PhieuDatHang_ChiTiet s) => s.THANHTIEN), 0);
							Deposit.TONGTIENGIAMGIA = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum((v_ct_PhieuDatHang_ChiTiet s) => s.TONGTIENGIAMGIA), 0);
							Deposit.TONGTIENVAT = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum((v_ct_PhieuDatHang_ChiTiet s) => s.TONGTIENVAT), 0);
							Deposit.TONGTIEN = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum((v_ct_PhieuDatHang_ChiTiet s) => s.TONGCONG), 0);
						}
						_context.ct_PhieuDatHang.Add(Deposit);
						AuditLogController auditLog = new AuditLogController(_context, _configuration);
						auditLog.InserAuditLog();
						await _context.SaveChangesAsync();
						transaction.Commit();
					}
					continue;
				}
				return Ok(new ApiResponse
				{
					Success = false,
					Message = error,
					Data = ""
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

	[HttpGet("{LOC_ID}/{Type}")]
	public async Task<IActionResult> PostGetUniben(string LOC_ID, string Type)
	{
		try
		{
			DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse unibenOrderListResponse = new DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse();
			new List<v_uniben_dm_LienKet_HangHoa>();
			new List<v_uniben_dm_LienKet_KhachHang>();
			new List<v_uniben_dm_LienKet_NhanVien>();
			unibenOrderListResponse.HangHoa = new List<v_uniben_dm_LienKet_HangHoa>();
			unibenOrderListResponse.KhachHang = new List<v_uniben_dm_LienKet_KhachHang>();
			unibenOrderListResponse.NhanVien = new List<v_uniben_dm_LienKet_NhanVien>();
			switch (Type)
			{
			case "Product":
			{
				DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse unibenOrderListResponse4 = unibenOrderListResponse;
				unibenOrderListResponse4.HangHoa = await GetProductUniben(LOC_ID);
				break;
			}
			case "Customer":
			{
				DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse unibenOrderListResponse3 = unibenOrderListResponse;
				unibenOrderListResponse3.KhachHang = await GetCustomerUniben(LOC_ID);
				break;
			}
			case "Employee":
			{
				DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse unibenOrderListResponse2 = unibenOrderListResponse;
				unibenOrderListResponse2.NhanVien = await GetEmployeeUniben(LOC_ID);
				break;
			}
			}
			List<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse> list = new List<DatabaseTHP.Class.Uniben.Uniben.UnibenOrderListResponse> { unibenOrderListResponse };
			return Ok(new ApiResponse
			{
				Success = true,
				Message = "Success",
				Data = list
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

	private async Task<List<v_uniben_dm_LienKet_KhachHang>> GetCustomerUniben(string LOC_ID)
	{
		List<v_uniben_dm_LienKet_KhachHang> khachHang = new List<v_uniben_dm_LienKet_KhachHang>();
		dm_TaiKhoan_Uniben TaiKhoan = await _context.dm_TaiKhoan_Uniben.Where((dm_TaiKhoan_Uniben e) => e.LOC_ID == LOC_ID && e.ISACTIVE).FirstAsync();
		if (TaiKhoan == null)
		{
			return khachHang;
		}
		UnibenService invoiceService = new UnibenService();
		int pageSize = 100;
		int currentPage = 1;
		int lastPage = 1;
		int totalItems = 0;
		do
		{
			DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse response = await invoiceService.GetListSales(TaiKhoan.ACCESSTOKEN, TaiKhoan.LINK + GetLinkCustomer(TaiKhoan.ORGANIZATIONUNITID, currentPage, pageSize, totalItems));
			if (response != null && response.status == 404)
			{
				await CheckToken(TaiKhoan);
				break;
			}
			if (response == null || response.payload == null || currentPage > response.metaData.lastPage)
			{
				break;
			}
			string json = response.payload.ToString();
			if (!string.IsNullOrWhiteSpace(json) && json.Trim() == "[]")
			{
				break;
			}
			if (response.payload != null && !string.IsNullOrWhiteSpace(json))
			{
				List<DatabaseTHP.Class.Uniben.Uniben.UnibenCustomer> data = JsonSerializer.Deserialize<List<DatabaseTHP.Class.Uniben.Uniben.UnibenCustomer>>(json);
				if (data != null)
				{
					foreach (DatabaseTHP.Class.Uniben.Uniben.UnibenCustomer item in data)
					{
						v_uniben_dm_LienKet_KhachHang existingKH = khachHang.Where((v_uniben_dm_LienKet_KhachHang e) => e.MAKHACHHANG == item.customerCode).FirstOrDefault();
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
							if (await _context.uniben_dm_LienKet_KhachHang.Where((uniben_dm_LienKet_KhachHang e) => e.LOC_ID == LOC_ID && e.ID_UNIBEN == khach.ID_UNIBEN && e.MAKHACHHANG == khach.MAKHACHHANG).FirstOrDefaultAsync() == null)
							{
								khachHang.Add(khach);
							}
						}
					}
				}
			}
			if (response.metaData != null)
			{
				lastPage = response.metaData.lastPage;
				totalItems = response.metaData.totalItems;
			}
			currentPage++;
		}
		while (currentPage <= lastPage);
		return khachHang.OrderBy((v_uniben_dm_LienKet_KhachHang e) => e.TENKHACHHANG).ToList();
	}

	private string GetLinkCustomer(string distributorCode, int page, int pageSize, int totalItems)
	{
		return $"/admin/customers?sort=&direction=&page={page}&pageSize={pageSize}&query=&distributorCode={distributorCode}&customerCode=&customerName=&routeCode=&userCode=&status=APPROVAL&activeFlag=true&includingRoute=true&totalItems={totalItems}";
	}

	private async Task<List<v_uniben_dm_LienKet_HangHoa>> GetProductUniben(string LOC_ID)
	{
		List<v_uniben_dm_LienKet_HangHoa> khachHang = new List<v_uniben_dm_LienKet_HangHoa>();
		dm_TaiKhoan_Uniben TaiKhoan = await _context.dm_TaiKhoan_Uniben.Where((dm_TaiKhoan_Uniben e) => e.LOC_ID == LOC_ID && e.ISACTIVE).FirstAsync();
		if (TaiKhoan == null)
		{
			return khachHang;
		}
		UnibenService invoiceService = new UnibenService();
		int pageSize = 100;
		int currentPage = 1;
		int lastPage = 1;
		int totalItems = 0;
		do
		{
			DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse response = await invoiceService.GetListSales(TaiKhoan.ACCESSTOKEN, TaiKhoan.LINK + GetLinkProduct(TaiKhoan.ORGANIZATIONUNITID, currentPage, pageSize, totalItems));
			if (response != null && response.status == 404)
			{
				await CheckToken(TaiKhoan);
				break;
			}
			if (response == null || response.payload == null || currentPage > response.metaData.lastPage)
			{
				break;
			}
			string json = response.payload.ToString();
			if (!string.IsNullOrWhiteSpace(json) && json.Trim() == "[]")
			{
				break;
			}
			if (response.payload != null && !string.IsNullOrWhiteSpace(json))
			{
				List<DatabaseTHP.Class.Uniben.Uniben.UnibenProduct> data = JsonSerializer.Deserialize<List<DatabaseTHP.Class.Uniben.Uniben.UnibenProduct>>(json);
				if (data != null)
				{
					foreach (DatabaseTHP.Class.Uniben.Uniben.UnibenProduct item in data)
					{
						v_uniben_dm_LienKet_HangHoa existingKH = khachHang.Where((v_uniben_dm_LienKet_HangHoa e) => e.MAHANGHOA == item.productCode).FirstOrDefault();
						if (existingKH == null)
						{
							v_uniben_dm_LienKet_HangHoa khach = new v_uniben_dm_LienKet_HangHoa();
							khach.ID_UNIBEN = item.productCode;
							khach.MAHANGHOA = item.productCode;
							khach.TENHANGHOA = item.productName;
							khach.ISACTIVE = true;
							khach.LOC_ID = LOC_ID;
							if (await _context.uniben_dm_LienKet_HangHoa.Where((uniben_dm_LienKet_HangHoa e) => e.LOC_ID == LOC_ID && e.MAHANGHOA == khach.MAHANGHOA && e.ID_UNIBEN == khach.ID_UNIBEN).FirstOrDefaultAsync() == null)
							{
								khachHang.Add(khach);
							}
						}
					}
				}
			}
			if (response.metaData != null)
			{
				lastPage = response.metaData.lastPage;
				totalItems = response.metaData.totalItems;
			}
			currentPage++;
		}
		while (currentPage <= lastPage);
		return khachHang.OrderBy((v_uniben_dm_LienKet_HangHoa e) => e.TENHANGHOA).ToList();
	}

	private string GetLinkProduct(string distributorCode, int page, int pageSize, int totalItems)
	{
		return $"/admin/products?page={page}&pageSize={pageSize}&query=&sort=&direction=&productCode=&productName=&distributorCode={distributorCode}&totalItems={totalItems}&includingStock=true&includingPrice=true&promotionFlag=false&posmFlag=false&activeFlag=true";
	}

	private async Task<List<v_uniben_dm_LienKet_NhanVien>> GetEmployeeUniben(string LOC_ID)
	{
		List<v_uniben_dm_LienKet_NhanVien> khachHang = new List<v_uniben_dm_LienKet_NhanVien>();
		dm_TaiKhoan_Uniben TaiKhoan = await _context.dm_TaiKhoan_Uniben.Where((dm_TaiKhoan_Uniben e) => e.LOC_ID == LOC_ID && e.ISACTIVE).FirstAsync();
		if (TaiKhoan == null)
		{
			return khachHang;
		}
		UnibenService invoiceService = new UnibenService();
		int pageSize = 100;
		int currentPage = 1;
		int lastPage = 1;
		int totalItems = 0;
		do
		{
			DatabaseTHP.Class.Uniben.Uniben.UnibenApiResponse response = await invoiceService.GetListSales(TaiKhoan.ACCESSTOKEN, TaiKhoan.LINK + GetLinkEmployee(TaiKhoan.ORGANIZATIONUNITID, currentPage, pageSize, totalItems));
			if (response != null && response.status == 404)
			{
				await CheckToken(TaiKhoan);
				break;
			}
			if (response == null || response.payload == null || currentPage > response.metaData.lastPage)
			{
				break;
			}
			string json = response.payload.ToString();
			if (!string.IsNullOrWhiteSpace(json) && json.Trim() == "[]")
			{
				break;
			}
			if (response.payload != null && !string.IsNullOrWhiteSpace(json))
			{
				List<DatabaseTHP.Class.Uniben.Uniben.UnibenRoute> data = JsonSerializer.Deserialize<List<DatabaseTHP.Class.Uniben.Uniben.UnibenRoute>>(json);
				if (data != null)
				{
					foreach (DatabaseTHP.Class.Uniben.Uniben.UnibenRoute item in data)
					{
						v_uniben_dm_LienKet_NhanVien existingKH = khachHang.Where((v_uniben_dm_LienKet_NhanVien e) => e.MANHANVIEN == item.routeCode).FirstOrDefault();
						if (existingKH == null)
						{
							v_uniben_dm_LienKet_NhanVien khach = new v_uniben_dm_LienKet_NhanVien();
							khach.ID_UNIBEN = item.routeCode;
							khach.MANHANVIEN = item.routeCode;
							khach.TENNHANVIEN = (string.IsNullOrEmpty(item.salesUserName) ? item.usmName : item.salesUserName);
							khach.ISACTIVE = true;
							khach.LOC_ID = LOC_ID;
							if (await _context.uniben_dm_LienKet_NhanVien.Where((uniben_dm_LienKet_NhanVien e) => e.LOC_ID == LOC_ID && e.MANHANVIEN == khach.MANHANVIEN && e.ID_UNIBEN == khach.ID_UNIBEN).FirstOrDefaultAsync() == null)
							{
								khachHang.Add(khach);
							}
						}
					}
				}
			}
			if (response.metaData != null)
			{
				lastPage = response.metaData.lastPage;
				totalItems = response.metaData.totalItems;
			}
			currentPage++;
		}
		while (currentPage <= lastPage);
		return khachHang.OrderBy((v_uniben_dm_LienKet_NhanVien e) => e.TENNHANVIEN).ToList();
	}

	private string GetLinkEmployee(string distributorCode, int page, int pageSize, int totalItems)
	{
		return $"/admin/sales-routes?page={page}&pageSize={pageSize}&query=&sort=&direction=&distributorCode={distributorCode}&salesRouteCodes=&salesRouteCode=&salesRouteName=&salesTeamCodes=&activeFlag=true&notEmptyFlag=false&vansalesFlag=false";
	}
}
