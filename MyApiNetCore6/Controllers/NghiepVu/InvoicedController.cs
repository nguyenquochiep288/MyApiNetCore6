using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
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

namespace MyApiNetCore6.Controllers
{

	[Route("api/[controller]")]
	[ApiController]
	public class InvoicedController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		private readonly string _connectionString;

		private string strTable = "ct_HoaDon";

		public InvoicedController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
			_connectionString = configuration.GetConnectionString("TrangHiepPhat");
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetInput(string LOC_ID)
		{
			try
			{
				List<ct_HoaDon> lstValue = await _context.ct_HoaDon.Where((ct_HoaDon e) => e.LOC_ID == LOC_ID).ToListAsync();
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = lstValue
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

		[HttpGet("{LOC_ID}/{ID_KHACHHANG}/{CHUNGTUKEMTHEO}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetInput(string LOC_ID, string ID_KHACHHANG, string CHUNGTUKEMTHEO)
		{
			try
			{
				new v_ct_HoaDon();
				v_ct_HoaDon ct_HoaDon2 = await GetHoaDon(LOC_ID, ID_KHACHHANG, CHUNGTUKEMTHEO);
				ct_HoaDon2.lstct_HoaDon_ChiTiet = new List<v_ct_HoaDon_ChiTiet>();
				ct_HoaDon2.TONGTHANHTIEN = Math.Round(ct_HoaDon2.lstct_HoaDon_ChiTiet_TraVe.Sum((Product_Detail e) => (e.TINHCHAT == 3) ? (-1.0 * e.THANHTIEN) : e.THANHTIEN), 0);
				ct_HoaDon2.TONGTIENVAT = Math.Round(ct_HoaDon2.lstct_HoaDon_ChiTiet_TraVe.Sum((Product_Detail e) => (e.TINHCHAT == 3) ? (e.TONGTIENVAT * -1.0) : e.TONGTIENVAT), 0);
				ct_HoaDon2.TONGTIENGIAMGIA = Math.Round(ct_HoaDon2.lstct_HoaDon_ChiTiet_TraVe.Sum((Product_Detail e) => e.TONGTIENGIAMGIA), 0);
				ct_HoaDon2.TONGTIEN = Math.Round(ct_HoaDon2.lstct_HoaDon_ChiTiet_TraVe.Sum((Product_Detail e) => (e.TINHCHAT == 3) ? (e.TONGCONG * -1.0) : e.TONGCONG), 0);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ct_HoaDon2
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

		private async Task<v_ct_HoaDon?> GetHoaDon(string LOC_ID, string ID_KHACHHANG, string CHUNGTUKEMTHEO, bool bolTaoHangLoat = false)
		{
			v_ct_HoaDon ct_HoaDon2 = new v_ct_HoaDon();
			if (!string.IsNullOrEmpty(ID_KHACHHANG))
			{
				dm_KhachHang KhachHang = await _context.dm_KhachHang.FirstOrDefaultAsync((dm_KhachHang e) => e.LOC_ID == LOC_ID && e.ID == ID_KHACHHANG);
				if (KhachHang != null)
				{
					ct_HoaDon2.ID_KHACHHANG = KhachHang.ID;
					ct_HoaDon2.TENKHACHHANG = KhachHang.TENKHACHHANG;
					ct_HoaDon2.TENDONVI = KhachHang.TENDONVI;
					ct_HoaDon2.DIACHI = KhachHang.DIACHI;
					ct_HoaDon2.MASOTHUE = (string.IsNullOrEmpty(KhachHang.TENDONVI) ? "" : KhachHang.MASOTHUE);
					ct_HoaDon2.DIENTHOAI = KhachHang.TEL;
					ct_HoaDon2.EMAIL = KhachHang.EMAIL;
					ct_HoaDon2.CCCD = KhachHang.CCCD;
					if (string.IsNullOrEmpty(KhachHang.TENKHACHHANG) && string.IsNullOrEmpty(KhachHang.TENDONVI))
					{
						ct_HoaDon2.TENKHACHHANG = "Khách hàng không lấy hóa đơn";
					}
				}
				else
				{
					ct_HoaDon2.ID_KHACHHANG = null;
				}
				ct_PhieuXuat Input = await _context.ct_PhieuXuat.Where((ct_PhieuXuat e) => e.LOC_ID == LOC_ID && (e.MAPHIEU == CHUNGTUKEMTHEO || e.ID == CHUNGTUKEMTHEO)).FirstOrDefaultAsync();
				if (Input != null)
				{
					view_dm_HangHoa HangHoa_GTBH = await _context.view_dm_HangHoa.FirstOrDefaultAsync((view_dm_HangHoa e) => e.LOC_ID == LOC_ID && e.MA == API.GTBH);
					view_dm_HangHoa HangHoa_TINHTHUE_KM = await _context.view_dm_HangHoa.FirstOrDefaultAsync((view_dm_HangHoa e) => e.LOC_ID == LOC_ID && e.MA == API.TINHTHUE_KM);
					List<ct_PhieuXuat_ChiTiet> lstInput_ChiTiet = await (from e in _context.ct_PhieuXuat_ChiTiet.AsNoTracking()
																		 where e.LOC_ID == LOC_ID && e.ID_PHIEUXUAT == Input.ID
																		 select e).ToListAsync();
					if (lstInput_ChiTiet != null && lstInput_ChiTiet.Count() > 0)
					{
						ct_HoaDon2.lstct_HoaDon_ChiTiet_TraVe = new List<Product_Detail>();
						ct_HoaDon2.lstct_HoaDon_ChiTiet = new List<v_ct_HoaDon_ChiTiet>();
						List<ct_PhieuXuat_ChiTiet> result = (from hd in lstInput_ChiTiet
															 orderby hd.STT, hd.ISKHUYENMAI
															 select hd).ToList();
						int STT = 1;
						dm_ThueSuat VAT10 = await _context.dm_ThueSuat.FirstOrDefaultAsync((dm_ThueSuat e) => e.LOC_ID == LOC_ID && e.MA == "10");
						dm_ThueSuat VAT11 = await _context.dm_ThueSuat.FirstOrDefaultAsync((dm_ThueSuat e) => e.LOC_ID == LOC_ID && e.MA == "8");
						await _context.dm_ThueSuat.FirstOrDefaultAsync((dm_ThueSuat e) => e.LOC_ID == LOC_ID && e.MA == "0");
						dm_ThueSuat VAT12 = await _context.dm_ThueSuat.FirstOrDefaultAsync((dm_ThueSuat e) => e.LOC_ID == LOC_ID && e.MA == "5");
						string[] listMaHang10 = new string[0];
						string[] listMaHang11 = new string[0];
						if (VAT10 != null && !string.IsNullOrEmpty(VAT10.GHICHU))
						{
							listMaHang10 = (from x in VAT10.GHICHU.Replace("\r", "").Replace("\n", "").Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries)
											select x.Trim() into x
											where !string.IsNullOrEmpty(x)
											select x).ToArray();
						}
						if (VAT12 != null && !string.IsNullOrEmpty(VAT12.GHICHU))
						{
							listMaHang11 = (from x in VAT12.GHICHU.Replace("\r", "").Replace("\n", "").Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries)
											select x.Trim() into x
											where !string.IsNullOrEmpty(x)
											select x).ToArray();
						}
						foreach (ct_PhieuXuat_ChiTiet itm in result)
						{
							bool bolBaoKhuyenMaiVaChietKhau = false;
							dm_HangHoa_Kho HangHoaKho = await _context.dm_HangHoa_Kho.FirstOrDefaultAsync((dm_HangHoa_Kho e) => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO);
							if (HangHoaKho == null || (HangHoaKho != null && HangHoa_TINHTHUE_KM != null && HangHoaKho.ID_HANGHOA == HangHoa_TINHTHUE_KM.ID))
							{
								continue;
							}
							view_dm_HangHoa HangHoa_ = await _context.view_dm_HangHoa.FirstOrDefaultAsync((view_dm_HangHoa e) => e.LOC_ID == LOC_ID && e.ID == HangHoaKho.ID_HANGHOA);
							if (HangHoa_ == null)
							{
								continue;
							}
							Product_Detail Product_Detail = new Product_Detail();
							v_ct_HoaDon_ChiTiet ct_HoaDon_ChiTiet2 = new v_ct_HoaDon_ChiTiet();
							string KM = "";
							int tINHCHAT;
							if (itm.ISKHUYENMAI || (itm.DONGIA == 0.0 && itm.THANHTIEN == 0.0))
							{
								Product_Detail product_Detail = Product_Detail;
								tINHCHAT = (ct_HoaDon_ChiTiet2.TINHCHAT = 1);
								product_Detail.TINHCHAT = tINHCHAT;
								KM = " (Hàng khuyến mãi không thu tiền)";
							}
							else
							{
								Product_Detail product_Detail2 = Product_Detail;
								tINHCHAT = (ct_HoaDon_ChiTiet2.TINHCHAT = 1);
								product_Detail2.TINHCHAT = tINHCHAT;
							}
							if (HangHoaKho != null && HangHoa_GTBH != null && HangHoa_.ID == HangHoa_GTBH.ID)
							{
								Product_Detail product_Detail3 = Product_Detail;
								tINHCHAT = (ct_HoaDon_ChiTiet2.TINHCHAT = 3);
								product_Detail3.TINHCHAT = tINHCHAT;
								KM = "";
							}
							if (itm.TONGCONG < 0.0 && itm.DONGIA == 0.0)
							{
								if (itm.SOLUONG > 0.0)
								{
									bolBaoKhuyenMaiVaChietKhau = true;
								}
								else
								{
									Product_Detail product_Detail4 = Product_Detail;
									tINHCHAT = (ct_HoaDon_ChiTiet2.TINHCHAT = 3);
									product_Detail4.TINHCHAT = tINHCHAT;
								}
							}
							if (HangHoaKho == null)
							{
								continue;
							}
							view_dm_HangHoa HangHoa = await _context.view_dm_HangHoa.FirstOrDefaultAsync((view_dm_HangHoa e) => e.LOC_ID == itm.LOC_ID && e.ID == HangHoaKho.ID_HANGHOA);
							if (HangHoa != null && HangHoa.ISXUATHOADON)
							{
								continue;
							}
							if (bolTaoHangLoat && HangHoa != null && !string.IsNullOrEmpty(HangHoa.VAT) && HangHoa.VAT != "8")
							{
								return null;
							}
							if (HangHoa == null)
							{
								continue;
							}
							double THUESUAT = ((listMaHang11.Any((string x) => HangHoa.MA.StartsWith(x)) && VAT12 != null) ? VAT12.THUESUAT : ((listMaHang10.Any((string x) => HangHoa.MA.StartsWith(x)) && VAT10 != null) ? VAT10.THUESUAT : (VAT11?.THUESUAT ?? 0.0)));
							string ID_THUESUAT = ((listMaHang11.Any((string x) => HangHoa.MA.StartsWith(x)) && VAT12 != null) ? VAT12.ID : ((listMaHang10.Any((string x) => HangHoa.MA.StartsWith(x)) && VAT10 != null) ? VAT10.ID : ((VAT11 != null) ? VAT11.ID : "")));
							dm_DonViTinh DVT = await _context.dm_DonViTinh.FirstOrDefaultAsync((dm_DonViTinh e) => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_DVT);
							Product_Detail product_Detail5 = Product_Detail;
							string iD = (ct_HoaDon_ChiTiet2.ID = Guid.NewGuid().ToString());
							product_Detail5.ID = iD;
							Product_Detail product_Detail6 = Product_Detail;
							iD = (ct_HoaDon_ChiTiet2.MAHANGHOA = HangHoa.MA);
							product_Detail6.MAHANGHOA = iD;
							Product_Detail product_Detail7 = Product_Detail;
							iD = (ct_HoaDon_ChiTiet2.TENHANGHOA = HangHoa.NAME + KM);
							product_Detail7.TENHANGHOA = iD;
							Product_Detail product_Detail8 = Product_Detail;
							iD = (ct_HoaDon_ChiTiet2.ID_DVT = DVT?.ID);
							product_Detail8.ID_DVT = iD;
							Product_Detail product_Detail9 = Product_Detail;
							iD = (ct_HoaDon_ChiTiet2.DVT = DVT?.NAME);
							product_Detail9.DVT = iD;
							Product_Detail product_Detail10 = Product_Detail;
							double sOLUONG = (ct_HoaDon_ChiTiet2.SOLUONG = itm.SOLUONG);
							product_Detail10.SOLUONG = sOLUONG;
							Product_Detail product_Detail11 = Product_Detail;
							sOLUONG = (ct_HoaDon_ChiTiet2.DONGIA = itm.DONGIA);
							product_Detail11.DONGIA = sOLUONG;
							Product_Detail product_Detail12 = Product_Detail;
							sOLUONG = (ct_HoaDon_ChiTiet2.CHIETKHAU = itm.CHIETKHAU);
							product_Detail12.CHIETKHAU = sOLUONG;
							Product_Detail product_Detail13 = Product_Detail;
							sOLUONG = (ct_HoaDon_ChiTiet2.TONGTIENGIAMGIA = itm.TONGTIENGIAMGIA);
							product_Detail13.TONGTIENGIAMGIA = sOLUONG;
							Product_Detail product_Detail14 = Product_Detail;
							sOLUONG = (ct_HoaDon_ChiTiet2.THANHTIEN = itm.THANHTIEN);
							product_Detail14.THANHTIEN = sOLUONG;
							Product_Detail product_Detail15 = Product_Detail;
							iD = (ct_HoaDon_ChiTiet2.ID_THUESUAT = itm.ID_THUESUAT);
							product_Detail15.ID_THUESUAT = iD;
							Product_Detail product_Detail16 = Product_Detail;
							sOLUONG = (ct_HoaDon_ChiTiet2.THUESUAT = itm.THUESUAT);
							product_Detail16.THUESUAT = sOLUONG;
							Product_Detail product_Detail17 = Product_Detail;
							sOLUONG = (ct_HoaDon_ChiTiet2.TONGTIENVAT = itm.TONGTIENVAT);
							product_Detail17.TONGTIENVAT = sOLUONG;
							Product_Detail product_Detail18 = Product_Detail;
							sOLUONG = (ct_HoaDon_ChiTiet2.TONGCONG = Math.Round(itm.TONGCONG, 0));
							product_Detail18.TONGCONG = sOLUONG;
							Product_Detail product_Detail19 = Product_Detail;
							iD = (ct_HoaDon_ChiTiet2.ID_PHIEUXUAT_CHITIET = itm.ID);
							product_Detail19.ID_PHIEUXUAT_CHITIET = iD;
							Product_Detail.ISKHUYENMAI = itm.ISKHUYENMAI;
							Product_Detail product_Detail20 = Product_Detail;
							tINHCHAT = (ct_HoaDon_ChiTiet2.STT = STT);
							product_Detail20.STT = tINHCHAT;
							if (!string.IsNullOrEmpty(KM))
							{
								Product_Detail product_Detail21 = Product_Detail;
								v_ct_HoaDon_ChiTiet obj = ct_HoaDon_ChiTiet2;
								string text5 = (itm.ID_THUESUAT = ID_THUESUAT);
								iD = (obj.ID_THUESUAT = text5);
								product_Detail21.ID_THUESUAT = iD;
								Product_Detail.THUESUAT = THUESUAT;
								Product_Detail product_Detail22 = Product_Detail;
								sOLUONG = (ct_HoaDon_ChiTiet2.CHIETKHAU = 0.0);
								product_Detail22.CHIETKHAU = sOLUONG;
								Product_Detail product_Detail23 = Product_Detail;
								sOLUONG = (ct_HoaDon_ChiTiet2.TONGTIENGIAMGIA = 0.0);
								product_Detail23.TONGTIENGIAMGIA = sOLUONG;
								Product_Detail product_Detail24 = Product_Detail;
								sOLUONG = (ct_HoaDon_ChiTiet2.THANHTIEN = 0.0);
								product_Detail24.THANHTIEN = sOLUONG;
								Product_Detail product_Detail25 = Product_Detail;
								sOLUONG = (ct_HoaDon_ChiTiet2.TONGTIENVAT = 0.0);
								product_Detail25.TONGTIENVAT = sOLUONG;
								Product_Detail product_Detail26 = Product_Detail;
								sOLUONG = (ct_HoaDon_ChiTiet2.TONGCONG = 0.0);
								product_Detail26.TONGCONG = sOLUONG;
							}
							else if (ct_HoaDon_ChiTiet2.TINHCHAT == 3)
							{
								Product_Detail product_Detail27 = Product_Detail;
								sOLUONG = (ct_HoaDon_ChiTiet2.SOLUONG = 1.0);
								product_Detail27.SOLUONG = sOLUONG;
								if (itm.THANHTIEN < 0.0)
								{
									itm.THANHTIEN *= -1.0;
								}
								Product_Detail product_Detail28 = Product_Detail;
								v_ct_HoaDon_ChiTiet obj2 = ct_HoaDon_ChiTiet2;
								double num13 = (itm.THUESUAT = THUESUAT);
								sOLUONG = (obj2.THUESUAT = num13);
								product_Detail28.THUESUAT = sOLUONG;
								Product_Detail product_Detail29 = Product_Detail;
								v_ct_HoaDon_ChiTiet obj3 = ct_HoaDon_ChiTiet2;
								string text5 = (itm.ID_THUESUAT = ID_THUESUAT);
								iD = (obj3.ID_THUESUAT = text5);
								product_Detail29.ID_THUESUAT = iD;
								double GiaChuaVAT = itm.THANHTIEN / (1.0 + Product_Detail.THUESUAT / 100.0);
								Product_Detail product_Detail30 = Product_Detail;
								sOLUONG = (ct_HoaDon_ChiTiet2.DONGIA = Math.Round(GiaChuaVAT, 2));
								product_Detail30.DONGIA = sOLUONG;
								Product_Detail product_Detail31 = Product_Detail;
								sOLUONG = (ct_HoaDon_ChiTiet2.CHIETKHAU = 0.0);
								product_Detail31.CHIETKHAU = sOLUONG;
								Product_Detail product_Detail32 = Product_Detail;
								sOLUONG = (ct_HoaDon_ChiTiet2.TONGTIENGIAMGIA = 0.0);
								product_Detail32.TONGTIENGIAMGIA = sOLUONG;
								Product_Detail product_Detail33 = Product_Detail;
								sOLUONG = (ct_HoaDon_ChiTiet2.THANHTIEN = Math.Round(GiaChuaVAT * Product_Detail.SOLUONG, 0));
								product_Detail33.THANHTIEN = sOLUONG;
								Product_Detail product_Detail34 = Product_Detail;
								sOLUONG = (ct_HoaDon_ChiTiet2.TONGCONG = Math.Round(itm.THANHTIEN, 0));
								product_Detail34.TONGCONG = sOLUONG;
								Product_Detail product_Detail35 = Product_Detail;
								sOLUONG = (ct_HoaDon_ChiTiet2.TONGTIENVAT = Math.Round(Product_Detail.TONGCONG - Product_Detail.THANHTIEN, 0));
								product_Detail35.TONGTIENVAT = sOLUONG;
							}
							else if (string.IsNullOrEmpty(itm.ID_THUESUAT))
							{
								Product_Detail product_Detail36 = Product_Detail;
								v_ct_HoaDon_ChiTiet obj4 = ct_HoaDon_ChiTiet2;
								double num13 = (itm.THUESUAT = THUESUAT);
								sOLUONG = (obj4.THUESUAT = num13);
								product_Detail36.THUESUAT = sOLUONG;
								Product_Detail product_Detail37 = Product_Detail;
								v_ct_HoaDon_ChiTiet obj5 = ct_HoaDon_ChiTiet2;
								string text5 = (itm.ID_THUESUAT = ID_THUESUAT);
								iD = (obj5.ID_THUESUAT = text5);
								product_Detail37.ID_THUESUAT = iD;
								double GiaChuaVAT2 = Product_Detail.DONGIA / (1.0 + itm.THUESUAT / 100.0);
								Product_Detail product_Detail38 = Product_Detail;
								sOLUONG = (ct_HoaDon_ChiTiet2.DONGIA = Math.Round(GiaChuaVAT2, 2));
								product_Detail38.DONGIA = sOLUONG;
								Product_Detail product_Detail39 = Product_Detail;
								v_ct_HoaDon_ChiTiet obj6 = ct_HoaDon_ChiTiet2;
								num13 = (itm.TONGTIENGIAMGIA = Math.Round(itm.TONGTIENGIAMGIA / (1.0 + Product_Detail.THUESUAT / 100.0), 0));
								sOLUONG = (obj6.TONGTIENGIAMGIA = num13);
								product_Detail39.TONGTIENGIAMGIA = sOLUONG;
								if (itm.TONGTIENGIAMGIA > 0.0)
								{
									Product_Detail product_Detail40 = Product_Detail;
									sOLUONG = (ct_HoaDon_ChiTiet2.CHIETKHAU = ((GiaChuaVAT2 != 0.0 && itm.SOLUONG != 0.0) ? Math.Round(itm.TONGTIENGIAMGIA / (Product_Detail.DONGIA * itm.SOLUONG) * 100.0, 2) : 0.0));
									product_Detail40.CHIETKHAU = sOLUONG;
								}
								else
								{
									Product_Detail product_Detail41 = Product_Detail;
									sOLUONG = (ct_HoaDon_ChiTiet2.CHIETKHAU = itm.CHIETKHAU);
									product_Detail41.CHIETKHAU = sOLUONG;
								}
								Product_Detail product_Detail42 = Product_Detail;
								sOLUONG = (ct_HoaDon_ChiTiet2.THANHTIEN = Math.Round(Product_Detail.DONGIA * Product_Detail.SOLUONG - Product_Detail.TONGTIENGIAMGIA, 0));
								product_Detail42.THANHTIEN = sOLUONG;
								double VAT13 = Math.Round(Product_Detail.THANHTIEN * (Product_Detail.THUESUAT / 100.0), 0);
								Product_Detail product_Detail43 = Product_Detail;
								sOLUONG = (ct_HoaDon_ChiTiet2.TONGTIENVAT = Math.Round(Product_Detail.TONGCONG - Product_Detail.TONGTIENVAT - Product_Detail.THANHTIEN, 0));
								product_Detail43.TONGTIENVAT = sOLUONG;
								if (VAT13 - Product_Detail.TONGTIENVAT > 1.0)
								{
									Product_Detail.THANHTIEN -= 1.0;
									Product_Detail.TONGTIENVAT += 1.0;
								}
								Product_Detail product_Detail44 = Product_Detail;
								sOLUONG = (ct_HoaDon_ChiTiet2.TONGCONG = Product_Detail.TONGTIENVAT + Product_Detail.THANHTIEN);
								product_Detail44.TONGCONG = sOLUONG;
							}
							if (bolBaoKhuyenMaiVaChietKhau && HangHoa_GTBH != null)
							{
								Product_Detail = new Product_Detail();
								ct_HoaDon_ChiTiet2 = new v_ct_HoaDon_ChiTiet();
								Product_Detail product_Detail45 = Product_Detail;
								tINHCHAT = (ct_HoaDon_ChiTiet2.TINHCHAT = 3);
								product_Detail45.TINHCHAT = tINHCHAT;
								Product_Detail product_Detail46 = Product_Detail;
								iD = (ct_HoaDon_ChiTiet2.ID = Guid.NewGuid().ToString());
								product_Detail46.ID = iD;
								Product_Detail product_Detail47 = Product_Detail;
								iD = (ct_HoaDon_ChiTiet2.MAHANGHOA = HangHoa_GTBH.MA);
								product_Detail47.MAHANGHOA = iD;
								Product_Detail product_Detail48 = Product_Detail;
								iD = (ct_HoaDon_ChiTiet2.TENHANGHOA = HangHoa_GTBH.NAME);
								product_Detail48.TENHANGHOA = iD;
								Product_Detail product_Detail49 = Product_Detail;
								iD = (ct_HoaDon_ChiTiet2.ID_DVT = null);
								product_Detail49.ID_DVT = iD;
								Product_Detail product_Detail50 = Product_Detail;
								iD = (ct_HoaDon_ChiTiet2.DVT = "Tiền");
								product_Detail50.DVT = iD;
								Product_Detail product_Detail51 = Product_Detail;
								sOLUONG = (ct_HoaDon_ChiTiet2.SOLUONG = 1.0);
								product_Detail51.SOLUONG = sOLUONG;
								if (itm.THANHTIEN < 0.0)
								{
									itm.THANHTIEN *= -1.0;
								}
								Product_Detail product_Detail52 = Product_Detail;
								v_ct_HoaDon_ChiTiet obj7 = ct_HoaDon_ChiTiet2;
								double num13 = (itm.THUESUAT = THUESUAT);
								sOLUONG = (obj7.THUESUAT = num13);
								product_Detail52.THUESUAT = sOLUONG;
								Product_Detail product_Detail53 = Product_Detail;
								v_ct_HoaDon_ChiTiet obj8 = ct_HoaDon_ChiTiet2;
								string text5 = (itm.ID_THUESUAT = ID_THUESUAT);
								iD = (obj8.ID_THUESUAT = text5);
								product_Detail53.ID_THUESUAT = iD;
								double GiaChuaVAT3 = itm.THANHTIEN / (1.0 + Product_Detail.THUESUAT / 100.0);
								Product_Detail product_Detail54 = Product_Detail;
								sOLUONG = (ct_HoaDon_ChiTiet2.DONGIA = Math.Round(GiaChuaVAT3, 2));
								product_Detail54.DONGIA = sOLUONG;
								Product_Detail product_Detail55 = Product_Detail;
								sOLUONG = (ct_HoaDon_ChiTiet2.CHIETKHAU = 0.0);
								product_Detail55.CHIETKHAU = sOLUONG;
								Product_Detail product_Detail56 = Product_Detail;
								sOLUONG = (ct_HoaDon_ChiTiet2.TONGTIENGIAMGIA = 0.0);
								product_Detail56.TONGTIENGIAMGIA = sOLUONG;
								Product_Detail product_Detail57 = Product_Detail;
								sOLUONG = (ct_HoaDon_ChiTiet2.THANHTIEN = Math.Round(GiaChuaVAT3 * Product_Detail.SOLUONG, 0));
								product_Detail57.THANHTIEN = sOLUONG;
								Product_Detail product_Detail58 = Product_Detail;
								sOLUONG = (ct_HoaDon_ChiTiet2.TONGCONG = Math.Round(itm.THANHTIEN, 0));
								product_Detail58.TONGCONG = sOLUONG;
								Product_Detail product_Detail59 = Product_Detail;
								sOLUONG = (ct_HoaDon_ChiTiet2.TONGTIENVAT = Math.Round(Product_Detail.TONGCONG - Product_Detail.THANHTIEN, 0));
								product_Detail59.TONGTIENVAT = sOLUONG;
                                //double VAT13 = Math.Round(Product_Detail.THANHTIEN * (Product_Detail.THUESUAT / 100.0), 0);
                                //if (VAT13 - Product_Detail.TONGTIENVAT > 1.0)
                                //{
                                //    Product_Detail.THANHTIEN -= 1.0;
                                //    Product_Detail.TONGTIENVAT += 1.0;
                                //}
                                Product_Detail product_Detail60 = Product_Detail;
								iD = (ct_HoaDon_ChiTiet2.ID_PHIEUXUAT_CHITIET = itm.ID);
								product_Detail60.ID_PHIEUXUAT_CHITIET = iD;
								Product_Detail.ISKHUYENMAI = itm.ISKHUYENMAI;
								Product_Detail product_Detail61 = Product_Detail;
								tINHCHAT = (ct_HoaDon_ChiTiet2.STT = STT);
								product_Detail61.STT = tINHCHAT;
								ct_HoaDon2.lstct_HoaDon_ChiTiet_TraVe.Add(Product_Detail);
								ct_HoaDon2.lstct_HoaDon_ChiTiet.Add(ct_HoaDon_ChiTiet2);
								STT++;
							}
							else
							{
								ct_HoaDon2.lstct_HoaDon_ChiTiet_TraVe.Add(Product_Detail);
								ct_HoaDon2.lstct_HoaDon_ChiTiet.Add(ct_HoaDon_ChiTiet2);
								STT++;
							}
						}
					}
				}
			}
			return ct_HoaDon2;
		}

		[HttpGet("{LOC_ID}/{Type}/{KeyWhere}/{ValuesSearch}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetInput(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				List<ct_HoaDon> lstValue = await _context.ct_HoaDon.Where((ct_HoaDon e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).ToListAsync();
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = lstValue
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

		[HttpGet("{LOC_ID}/{ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetInput(string LOC_ID, string ID)
		{
			try
			{
				ct_HoaDon Input = await _context.ct_HoaDon.FirstOrDefaultAsync((ct_HoaDon e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Input == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				v_ct_HoaDon ct_HoaDon2 = new v_ct_HoaDon();
				if (Input != null)
				{
					string strInput = JsonConvert.SerializeObject(Input);
					ct_HoaDon2 = JsonConvert.DeserializeObject<v_ct_HoaDon>(strInput) ?? new v_ct_HoaDon();
				}
				ct_HoaDon2.lstct_HoaDon_ChiTiet = new List<v_ct_HoaDon_ChiTiet>();
				SP_Parameter SP_Parameter = new SP_Parameter
				{
					ID_HOADON = ID
				};
				ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
				if (await ExecuteStoredProc1.Sp_Get_DanhSachHoaDon_Chitiet(SP_Parameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse })
				{
					List<v_ct_HoaDon_ChiTiet> lst_ChiTiet = ApiResponse.Data as List<v_ct_HoaDon_ChiTiet>;
					if (lst_ChiTiet != null)
					{
						ct_HoaDon2.lstct_HoaDon_ChiTiet.AddRange(lst_ChiTiet);
					}
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ct_HoaDon2
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

		[HttpPut("{LOC_ID}/{ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> PutInput(string LOC_ID, string ID, [FromBody] v_ct_HoaDon Input)
		{
			try
			{
				if (!InputExistsID(Input.LOC_ID, Input.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + Input.LOC_ID + "-" + Input.ID + " dữ liệu!",
						Data = ""
					});
				}
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				ct_HoaDon objHoaDon = await _context.ct_HoaDon.AsNoTracking().FirstOrDefaultAsync((ct_HoaDon e) => e.LOC_ID == Input.LOC_ID && e.ID == ID);
				if (objHoaDon != null && objHoaDon.ISXUATHOADON)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Chứng từ " + objHoaDon.MAPHIEU + " đã được lập hóa đơn!",
						Data = "",
						CheckValue = true
					});
				}
				if (objHoaDon != null && objHoaDon.CHUNGTUKEMTHEO != Input.CHUNGTUKEMTHEO)
				{
					if (!string.IsNullOrEmpty(Input.CHUNGTUKEMTHEO))
					{
						ct_PhieuXuat objOutput = await _context.ct_PhieuXuat.FirstOrDefaultAsync((ct_PhieuXuat e) => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO);
						if (objOutput == null)
						{
							return Ok(new ApiResponse
							{
								Success = false,
								Message = "Không tồn tại" + Input.LOC_ID + "-" + Input.CHUNGTUKEMTHEO + " trong dữ liệu!",
								Data = "",
								CheckValue = true
							});
						}
						if (!string.IsNullOrEmpty(objOutput.ID_HOADON))
						{
							return Ok(new ApiResponse
							{
								Success = false,
								Message = "Chứng từ " + Input.CHUNGTUKEMTHEO + " đã được lập hóa đơn!",
								Data = "",
								CheckValue = true
							});
						}
					}
					ct_PhieuXuat objOutput_Old = await _context.ct_PhieuXuat.FirstOrDefaultAsync((ct_PhieuXuat e) => e.LOC_ID == objHoaDon.LOC_ID && e.MAPHIEU == objHoaDon.CHUNGTUKEMTHEO);
					if (objOutput_Old != null)
					{
						objOutput_Old.ID_HOADON = null;
						_context.Entry(objOutput_Old).State = EntityState.Modified;
					}
				}
				List<ct_HoaDon_ChiTiet> lstHoaDon_ChiTiet = await _context.ct_HoaDon_ChiTiet.Where((ct_HoaDon_ChiTiet e) => e.LOC_ID == Input.LOC_ID && e.ID_HOADON == Input.ID).ToListAsync();
				if (lstHoaDon_ChiTiet != null)
				{
					foreach (ct_HoaDon_ChiTiet itm in lstHoaDon_ChiTiet)
					{
						v_ct_HoaDon_ChiTiet chkHoaDon_ChiTiet = Input.lstct_HoaDon_ChiTiet.Where((v_ct_HoaDon_ChiTiet e) => e.ID == itm.ID).FirstOrDefault();
						if (chkHoaDon_ChiTiet != null)
						{
							itm.TINHCHAT = chkHoaDon_ChiTiet.TINHCHAT;
							itm.ID_HANGHOAKHO = chkHoaDon_ChiTiet.ID_HANGHOAKHO;
							itm.MAHANGHOA = chkHoaDon_ChiTiet.MAHANGHOA;
							itm.TENHANGHOA = chkHoaDon_ChiTiet.TENHANGHOA;
							itm.ID_DVT = chkHoaDon_ChiTiet.ID_DVT;
							itm.DVT = chkHoaDon_ChiTiet.DVT;
							itm.DONGIA = chkHoaDon_ChiTiet.DONGIA;
							itm.CHIETKHAU = chkHoaDon_ChiTiet.CHIETKHAU;
							itm.TONGTIENGIAMGIA = chkHoaDon_ChiTiet.TONGTIENGIAMGIA;
							itm.THANHTIEN = chkHoaDon_ChiTiet.THANHTIEN;
							itm.ID_THUESUAT = chkHoaDon_ChiTiet.ID_THUESUAT;
							dm_ThueSuat VAT = await _context.dm_ThueSuat.FirstOrDefaultAsync((dm_ThueSuat e) => e.LOC_ID == objHoaDon.LOC_ID && e.ID == chkHoaDon_ChiTiet.ID_THUESUAT);
							itm.THUESUAT = VAT?.THUESUAT ?? chkHoaDon_ChiTiet.THUESUAT;
							itm.TONGTIENVAT = chkHoaDon_ChiTiet.TONGTIENVAT;
							itm.TONGCONG = chkHoaDon_ChiTiet.TONGCONG;
							itm.STT = chkHoaDon_ChiTiet.STT;
							itm.LOC_ID = chkHoaDon_ChiTiet.LOC_ID;
							itm.SOLUONG = chkHoaDon_ChiTiet.SOLUONG;
							chkHoaDon_ChiTiet.ISEDIT = true;
							chkHoaDon_ChiTiet.ID_HOADON = Input.ID;
							_context.Entry(itm).State = EntityState.Modified;
						}
						else
						{
							_context.ct_HoaDon_ChiTiet.Remove(itm);
						}
					}
				}
				if (Input.lstct_HoaDon_ChiTiet != null)
				{
					foreach (v_ct_HoaDon_ChiTiet itm2 in Input.lstct_HoaDon_ChiTiet)
					{
						if (!itm2.ISEDIT)
						{
							itm2.ID_HOADON = Input.ID;
							_context.ct_HoaDon_ChiTiet.Add(itm2);
						}
					}
				}
				_context.Entry(Input).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
				v_ct_HoaDon ct_HoaDon2 = new v_ct_HoaDon
				{
					lstct_HoaDon_ChiTiet = new List<v_ct_HoaDon_ChiTiet>()
				};
				SP_Parameter SP_Parameter = new SP_Parameter
				{
					ID_HOADON = Input.ID
				};
				ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
				if (await ExecuteStoredProc1.Sp_Get_DanhSachHoaDon(SP_Parameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse })
				{
					List<v_ct_HoaDon> lstHoaDon = ApiResponse.Data as List<v_ct_HoaDon>;
					if (lstHoaDon != null && lstHoaDon.Count() > 0)
					{
						ct_HoaDon2 = lstHoaDon.FirstOrDefault() ?? new v_ct_HoaDon();
					}
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ct_HoaDon2
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
			finally
			{
				AuditLogController auditLog2 = new AuditLogController(_context, _configuration);
				auditLog2.DeleteRequest(strTable);
			}
		}

		[HttpPost]
		[Authorize(Roles = "User")]
		public async Task<ActionResult<ct_HoaDon>> PostInput([FromBody] v_ct_HoaDon Input)
		{
			try
			{
				if (InputExistsID(Input.LOC_ID, Input.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Input.LOC_ID + "-" + Input.ID + " trong dữ liệu!",
						Data = "",
						CheckValue = true
					});
				}
				ct_PhieuXuat objOutput = new ct_PhieuXuat();
				if (!string.IsNullOrEmpty(Input.CHUNGTUKEMTHEO))
				{
					ct_PhieuXuat result = await _context.ct_PhieuXuat.FirstOrDefaultAsync((ct_PhieuXuat e) => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO);
					if (result == null)
					{
						return Ok(new ApiResponse
						{
							Success = false,
							Message = "Không tồn tại" + Input.LOC_ID + "-" + Input.CHUNGTUKEMTHEO + " trong dữ liệu!",
							Data = "",
							CheckValue = true
						});
					}
					objOutput = result;
					if (!string.IsNullOrEmpty(result.ID_HOADON))
					{
						return Ok(new ApiResponse
						{
							Success = false,
							Message = "Chứng từ " + Input.CHUNGTUKEMTHEO + " đã được lập hóa đơn!",
							Data = "",
							CheckValue = true
						});
					}
					if (await _context.ct_HoaDon.FirstOrDefaultAsync((ct_HoaDon e) => e.LOC_ID == Input.LOC_ID && e.CHUNGTUKEMTHEO == Input.CHUNGTUKEMTHEO) != null)
					{
						return Ok(new ApiResponse
						{
							Success = false,
							Message = "Đã tồn tại" + Input.LOC_ID + "-" + Input.CHUNGTUKEMTHEO + " trong dữ liệu!",
							Data = "",
							CheckValue = true
						});
					}
				}
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				if (Input.lstct_HoaDon_ChiTiet != null)
				{
					foreach (v_ct_HoaDon_ChiTiet itm in Input.lstct_HoaDon_ChiTiet)
					{
						itm.LOC_ID = Input.LOC_ID;
						itm.ID_HOADON = Input.ID;
						dm_ThueSuat VAT = await _context.dm_ThueSuat.FirstOrDefaultAsync((dm_ThueSuat e) => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_THUESUAT);
						itm.THUESUAT = VAT?.THUESUAT ?? itm.THUESUAT;
						_context.ct_HoaDon_ChiTiet.Add(itm);
					}
				}
				Input.ID_PHIEUXUAT = objOutput?.ID;
				_context.ct_HoaDon.Add(Input);
				if (objOutput != null && !string.IsNullOrEmpty(objOutput.ID))
				{
					objOutput.ID_HOADON = Input.ID;
					_context.Entry(objOutput).State = EntityState.Modified;
				}
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
				List<ct_HoaDon> lstPhieuDatHangCheck = await (from e in _context.ct_HoaDon
															  where e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.MAPHIEU
															  orderby e.NGAYLAP descending
															  select e).ToListAsync();
				if (lstPhieuDatHangCheck != null && lstPhieuDatHangCheck.Count() > 1 && lstPhieuDatHangCheck.FirstOrDefault().ID == Input.ID)
				{
					int Max_ID = (from e in _context.ct_HoaDon
								  where e.LOC_ID == Input.LOC_ID && e.NGAYLAP.Date == Input.NGAYLAP.Date
								  select e.SOPHIEU).DefaultIfEmpty().Max();
					Input.SOPHIEU = Max_ID + 1;
					Input.MAPHIEU = API.GetMaPhieu("Invoiced", Input.NGAYLAP, Input.SOPHIEU);
					_context.Entry(Input).State = EntityState.Modified;
					await _context.SaveChangesAsync();
				}
				v_ct_HoaDon ct_HoaDon2 = new v_ct_HoaDon
				{
					lstct_HoaDon_ChiTiet = new List<v_ct_HoaDon_ChiTiet>()
				};
				SP_Parameter SP_Parameter = new SP_Parameter
				{
					ID_HOADON = Input.ID
				};
				ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
				if (await ExecuteStoredProc1.Sp_Get_DanhSachHoaDon(SP_Parameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse })
				{
					List<v_ct_HoaDon> lstHoaDon = ApiResponse.Data as List<v_ct_HoaDon>;
					if (lstHoaDon != null && lstHoaDon.Count() > 0)
					{
						ct_HoaDon2 = lstHoaDon.FirstOrDefault() ?? new v_ct_HoaDon();
					}
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ct_HoaDon2
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
			finally
			{
				AuditLogController auditLog2 = new AuditLogController(_context, _configuration);
				auditLog2.DeleteRequest(strTable);
			}
		}

		[HttpPost("PostCreateOutput")]
		[Authorize(Roles = "User")]
		public async Task<ActionResult<ct_PhieuDatHang>> PostDeposit([FromBody] List<Deposit> lstDeposit)
		{
			try
			{
				string LOC_ID = "";
				DateTime NGAYLAP = default(DateTime);
				NGAYLAP = DateTime.Now.Date;
				if (lstDeposit != null && lstDeposit.Count > 0)
				{
					Deposit Deposit = lstDeposit.FirstOrDefault() ?? new Deposit();
					LOC_ID = ((Deposit != null) ? Deposit.LOC_ID : "");
					string ID_NGUOITAO = ((Deposit != null) ? Deposit.ID_NGUOITAO : "");
					NGAYLAP = Deposit?.NGAYLAP ?? DateTime.Now.Date;
					string ID_LOAIHOADON = ((Deposit != null) ? Deposit.ID_LOAIHOADON : "");
					new Dictionary<string, string>();
					using IDbContextTransaction transaction = _context.Database.BeginTransaction();
					int Max_ID = (from e in _context.ct_HoaDon
								  where e.LOC_ID == LOC_ID && e.NGAYLAP.Date == ((DateTime)NGAYLAP).Date
								  select e.SOPHIEU).DefaultIfEmpty().Max();
					foreach (Deposit itm in lstDeposit)
					{
						ct_PhieuXuat PhieuDatHang = await _context.ct_PhieuXuat.FirstOrDefaultAsync((ct_PhieuXuat e) => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID);
						if (PhieuDatHang == null)
						{
							return Ok(new ApiResponse
							{
								Success = false,
								Message = "Không tìm thấy " + LOC_ID + "-" + itm.ID + " dữ liệu phiếu xuất!",
								Data = ""
							});
						}
						if (!string.IsNullOrEmpty(PhieuDatHang.ID_HOADON))
						{
							return Ok(new ApiResponse
							{
								Success = false,
								Message = "Phiếu xuất " + PhieuDatHang.MAPHIEU + " đã được tạo hóa đơn!",
								Data = ""
							});
						}
						Max_ID++;
						new v_ct_HoaDon();
						v_ct_HoaDon ct_HoaDon2 = await GetHoaDon(LOC_ID, PhieuDatHang.ID_KHACHHANG, PhieuDatHang.ID, bolTaoHangLoat: true);
						if (ct_HoaDon2 == null)
						{
							continue;
						}
						ct_HoaDon2.ID = Guid.NewGuid().ToString();
						if (ct_HoaDon2.lstct_HoaDon_ChiTiet != null)
						{
							foreach (v_ct_HoaDon_ChiTiet s in ct_HoaDon2.lstct_HoaDon_ChiTiet)
							{
								s.LOC_ID = LOC_ID;
								s.ID_HOADON = ct_HoaDon2.ID;
								_context.ct_HoaDon_ChiTiet.Add(s);
							}
							ct_HoaDon2.TONGTHANHTIEN = Math.Round(ct_HoaDon2.lstct_HoaDon_ChiTiet_TraVe.Sum((Product_Detail e) => (e.TINHCHAT == 3) ? (-1.0 * e.THANHTIEN) : e.THANHTIEN), 0);
							ct_HoaDon2.TONGTIENVAT = Math.Round(ct_HoaDon2.lstct_HoaDon_ChiTiet_TraVe.Sum((Product_Detail e) => (e.TINHCHAT == 3) ? (e.TONGTIENVAT * -1.0) : e.TONGTIENVAT), 0);
							ct_HoaDon2.TONGTIENGIAMGIA = Math.Round(ct_HoaDon2.lstct_HoaDon_ChiTiet_TraVe.Sum((Product_Detail e) => e.TONGTIENGIAMGIA), 0);
							ct_HoaDon2.TONGTIEN = Math.Round(ct_HoaDon2.lstct_HoaDon_ChiTiet_TraVe.Sum((Product_Detail e) => (e.TINHCHAT == 3) ? (e.TONGCONG * -1.0) : e.TONGCONG), 0);
						}
						ct_HoaDon2.HTTT = "TM/CK";
						ct_HoaDon2.LOAITIEN = "VND";
						ct_HoaDon2.TYGIA = 1.0;
						DateTime nGAYHOADON = (ct_HoaDon2.NGAYLAP = NGAYLAP);
						ct_HoaDon2.NGAYHOADON = nGAYHOADON;
						ct_HoaDon2.ID_NGUOITAO = ID_NGUOITAO;
						ct_HoaDon2.LOC_ID = LOC_ID;
						ct_HoaDon2.ID_LOAIHOADON = ID_LOAIHOADON;
						ct_HoaDon2.ID_PHIEUXUAT = PhieuDatHang?.ID;
						ct_HoaDon2.CHUNGTUKEMTHEO = PhieuDatHang?.MAPHIEU;
						ct_HoaDon2.SOPHIEU = Max_ID;
						ct_HoaDon2.MAPHIEU = API.GetMaPhieu("Invoiced", ct_HoaDon2.NGAYLAP, ct_HoaDon2.SOPHIEU);
						_context.ct_HoaDon.Add(ct_HoaDon2);
						if (ct_HoaDon2 != null && !string.IsNullOrEmpty(ct_HoaDon2.ID) && PhieuDatHang != null && !string.IsNullOrEmpty(PhieuDatHang.ID))
						{
							PhieuDatHang.ID_HOADON = ct_HoaDon2.ID;
							_context.Entry(PhieuDatHang).State = EntityState.Modified;
						}
					}
					AuditLogController auditLog = new AuditLogController(_context, _configuration);
					auditLog.InserAuditLog();
					await _context.SaveChangesAsync();
					transaction.Commit();
					return Ok(new ApiResponse
					{
						Success = true,
						Message = "Success",
						Data = ""
					});
				}
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Không tìm thấy dữ liệu!",
					Data = ""
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

		[HttpDelete("{LOC_ID}/{ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> DeleteInput(string LOC_ID, string ID)
		{
			try
			{
				ct_HoaDon Input = await _context.ct_HoaDon.FirstOrDefaultAsync((ct_HoaDon e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Input == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				if (Input.ISXUATHOADON)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Chứng từ " + Input.MAPHIEU + " đã được tạo hóa đơn!",
						Data = ""
					});
				}
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				ct_PhieuXuat objOutput = await _context.ct_PhieuXuat.FirstOrDefaultAsync((ct_PhieuXuat e) => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO);
				if (objOutput != null)
				{
					objOutput.ID_HOADON = null;
					_context.Entry(objOutput).State = EntityState.Modified;
				}
				List<ct_HoaDon_ChiTiet> lstHoaDon_ChiTiet = await _context.ct_HoaDon_ChiTiet.Where((ct_HoaDon_ChiTiet e) => e.LOC_ID == Input.LOC_ID && e.ID_HOADON == Input.ID).ToListAsync();
				if (lstHoaDon_ChiTiet != null)
				{
					foreach (ct_HoaDon_ChiTiet itm in lstHoaDon_ChiTiet)
					{
						_context.ct_HoaDon_ChiTiet.Remove(itm);
					}
				}
				_context.ct_HoaDon.Remove(Input);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ""
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

		[HttpPut("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> Delete(string LOC_ID, [FromBody] List<Deposit> lstDeposit)
		{
			try
			{
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				foreach (Deposit s in lstDeposit)
				{
					ct_HoaDon Input = await _context.ct_HoaDon.FirstOrDefaultAsync((ct_HoaDon e) => e.LOC_ID == LOC_ID && e.ID == s.ID);
					if (Input == null)
					{
						return Ok(new ApiResponse
						{
							Success = false,
							Message = "Không tìm thấy " + LOC_ID + "-" + s.ID + " dữ liệu!",
							Data = ""
						});
					}
					if (Input.ISXUATHOADON)
					{
						return Ok(new ApiResponse
						{
							Success = false,
							Message = "Chứng từ " + Input.MAPHIEU + " đã được tạo hóa đơn!",
							Data = ""
						});
					}
					ct_PhieuXuat objOutput = await _context.ct_PhieuXuat.FirstOrDefaultAsync((ct_PhieuXuat e) => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO);
					if (objOutput != null)
					{
						objOutput.ID_HOADON = null;
						_context.Entry(objOutput).State = EntityState.Modified;
					}
					List<ct_HoaDon_ChiTiet> lstHoaDon_ChiTiet = await _context.ct_HoaDon_ChiTiet.Where((ct_HoaDon_ChiTiet e) => e.LOC_ID == Input.LOC_ID && e.ID_HOADON == Input.ID).ToListAsync();
					if (lstHoaDon_ChiTiet != null)
					{
						foreach (ct_HoaDon_ChiTiet itm in lstHoaDon_ChiTiet)
						{
							_context.ct_HoaDon_ChiTiet.Remove(itm);
						}
					}
					_context.ct_HoaDon.Remove(Input);
				}
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ""
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

		private bool InputExistsID(string LOC_ID, string ID)
		{
			return _context.ct_HoaDon.Any((ct_HoaDon e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}
	}
}
