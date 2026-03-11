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
	public class KPI_SaleController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public KPI_SaleController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetProduct(string LOC_ID)
		{
			try
			{
				List<view_dm_KPI_KinhDoanh> lstValue = await (from e in _context.view_dm_KPI_KinhDoanh
															  where e.LOC_ID == LOC_ID
															  orderby e.MA
															  select e).ToListAsync();
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

		[HttpGet("{LOC_ID}/{Type}/{KeyWhere}/{ValuesSearch}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetProduct(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<view_dm_KPI_KinhDoanh> lstValue = await (from e in _context.view_dm_KPI_KinhDoanh.Where((view_dm_KPI_KinhDoanh e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch)
															  orderby e.MA
															  select e).ToListAsync();
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
		public async Task<IActionResult> GetProduct(string LOC_ID, string ID)
		{
			try
			{
				view_dm_KPI_KinhDoanh Product = await _context.view_dm_KPI_KinhDoanh.FirstOrDefaultAsync((view_dm_KPI_KinhDoanh e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Product == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				v_view_dm_KPI_KinhDoanh dm_KPI_KinhDoanh2 = new v_view_dm_KPI_KinhDoanh
				{
					lstdm_KPI_KinhDoanh_YeuCau = new List<v_dm_KPI_KinhDoanh_YeuCau>()
				};
				if (Product != null)
				{
					dm_KPI_KinhDoanh2 = JsonConvert.DeserializeObject<v_view_dm_KPI_KinhDoanh>(JsonConvert.SerializeObject(Product)) ?? new v_view_dm_KPI_KinhDoanh();
					dm_KPI_KinhDoanh2.lstdm_KPI_KinhDoanh_YeuCau = JsonConvert.DeserializeObject<List<v_dm_KPI_KinhDoanh_YeuCau>>(JsonConvert.SerializeObject(await _context.view_dm_KPI_KinhDoanh_YeuCau.Where((view_dm_KPI_KinhDoanh_YeuCau e) => e.LOC_ID == LOC_ID && e.ID_KPI_KINHDOANH == Product.ID).ToListAsync()));
					dm_KPI_KinhDoanh2.lstdm_KPI_KinhDoanh_NhanVien = JsonConvert.DeserializeObject<List<v_dm_KPI_KinhDoanh_NhanVien>>(JsonConvert.SerializeObject(await _context.view_dm_KPI_KinhDoanh_NhanVien.Where((view_dm_KPI_KinhDoanh_NhanVien e) => e.LOC_ID == LOC_ID && e.ID_KPI_KINHDOANH == Product.ID).ToListAsync()));
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = dm_KPI_KinhDoanh2
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

		[HttpPut("{LOC_ID}/{MA}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> PutProduct(string LOC_ID, string MA, [FromBody] v_dm_KPI_KinhDoanh ChuongTrinhKhuyenMai)
		{
			try
			{
				if (ProductExists(ChuongTrinhKhuyenMai))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + ChuongTrinhKhuyenMai.LOC_ID + "-" + ChuongTrinhKhuyenMai.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				if (LOC_ID != ChuongTrinhKhuyenMai.LOC_ID || ChuongTrinhKhuyenMai.MA != MA)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (!ProductExistsID(ChuongTrinhKhuyenMai.LOC_ID, ChuongTrinhKhuyenMai.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + ChuongTrinhKhuyenMai.LOC_ID + "-" + ChuongTrinhKhuyenMai.ID + " dữ liệu!",
						Data = ""
					});
				}
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				foreach (dm_KPI_KinhDoanh_YeuCau itm in await _context.dm_KPI_KinhDoanh_YeuCau.Where((dm_KPI_KinhDoanh_YeuCau e) => e.LOC_ID == LOC_ID && e.ID_KPI_KINHDOANH == ChuongTrinhKhuyenMai.ID).ToListAsync())
				{
					_context.dm_KPI_KinhDoanh_YeuCau.Remove(itm);
				}
				foreach (v_dm_KPI_KinhDoanh_YeuCau itm2 in ChuongTrinhKhuyenMai.lstdm_KPI_KinhDoanh_YeuCau)
				{
					itm2.ID_KPI_KINHDOANH = ChuongTrinhKhuyenMai.ID;
					_context.dm_KPI_KinhDoanh_YeuCau.Add(itm2);
				}
				foreach (dm_KPI_KinhDoanh_NhanVien itm3 in await _context.dm_KPI_KinhDoanh_NhanVien.Where((dm_KPI_KinhDoanh_NhanVien e) => e.LOC_ID == LOC_ID && e.ID_KPI_KINHDOANH == ChuongTrinhKhuyenMai.ID).ToListAsync())
				{
					_context.dm_KPI_KinhDoanh_NhanVien.Remove(itm3);
				}
				foreach (v_dm_KPI_KinhDoanh_NhanVien itm4 in ChuongTrinhKhuyenMai.lstdm_KPI_KinhDoanh_NhanVien)
				{
					itm4.ID_KPI_KINHDOANH = ChuongTrinhKhuyenMai.ID;
					_context.dm_KPI_KinhDoanh_NhanVien.Add(itm4);
				}
				_context.Entry(ChuongTrinhKhuyenMai).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
				view_dm_KPI_KinhDoanh OKProduct = await _context.view_dm_KPI_KinhDoanh.FirstOrDefaultAsync((view_dm_KPI_KinhDoanh e) => e.LOC_ID == ChuongTrinhKhuyenMai.LOC_ID && e.ID == ChuongTrinhKhuyenMai.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKProduct
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

		[HttpPost]
		[Authorize(Roles = "User")]
		public async Task<ActionResult<v_dm_KPI_KinhDoanh>> PostProduct([FromBody] v_dm_KPI_KinhDoanh ChuongTrinhKhuyenMai)
		{
			try
			{
				if (ProductExistsMA(ChuongTrinhKhuyenMai.LOC_ID, ChuongTrinhKhuyenMai.MA))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + ChuongTrinhKhuyenMai.LOC_ID + "-" + ChuongTrinhKhuyenMai.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				foreach (v_dm_KPI_KinhDoanh_YeuCau itm in ChuongTrinhKhuyenMai.lstdm_KPI_KinhDoanh_YeuCau)
				{
					itm.ID_KPI_KINHDOANH = ChuongTrinhKhuyenMai.ID;
					_context.dm_KPI_KinhDoanh_YeuCau.Add(itm);
				}
				foreach (v_dm_KPI_KinhDoanh_NhanVien itm2 in ChuongTrinhKhuyenMai.lstdm_KPI_KinhDoanh_NhanVien)
				{
					itm2.ID_KPI_KINHDOANH = ChuongTrinhKhuyenMai.ID;
					_context.dm_KPI_KinhDoanh_NhanVien.Add(itm2);
				}
				_context.dm_KPI_KinhDoanh.Add(ChuongTrinhKhuyenMai);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
				view_dm_KPI_KinhDoanh OKProduct = await _context.view_dm_KPI_KinhDoanh.FirstOrDefaultAsync((view_dm_KPI_KinhDoanh e) => e.LOC_ID == ChuongTrinhKhuyenMai.LOC_ID && e.ID == ChuongTrinhKhuyenMai.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKProduct
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
		public async Task<IActionResult> DeleteProduct(string LOC_ID, string ID)
		{
			try
			{
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				dm_KPI_KinhDoanh ChuongTrinhKhuyenMai = await _context.dm_KPI_KinhDoanh.FirstOrDefaultAsync((dm_KPI_KinhDoanh e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (ChuongTrinhKhuyenMai == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
				ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete(ChuongTrinhKhuyenMai, ChuongTrinhKhuyenMai.ID, ChuongTrinhKhuyenMai.NAME);
				if (!apiResponse.Success)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = apiResponse.Message,
						Data = ""
					});
				}
				foreach (dm_KPI_KinhDoanh_YeuCau itm in await _context.dm_KPI_KinhDoanh_YeuCau.Where((dm_KPI_KinhDoanh_YeuCau e) => e.LOC_ID == LOC_ID && e.ID_KPI_KINHDOANH == ID).ToListAsync())
				{
					_context.dm_KPI_KinhDoanh_YeuCau.Remove(itm);
				}
				foreach (dm_KPI_KinhDoanh_NhanVien itm2 in await _context.dm_KPI_KinhDoanh_NhanVien.Where((dm_KPI_KinhDoanh_NhanVien e) => e.LOC_ID == LOC_ID && e.ID_KPI_KINHDOANH == ID).ToListAsync())
				{
					_context.dm_KPI_KinhDoanh_NhanVien.Remove(itm2);
				}
				_context.dm_KPI_KinhDoanh.Remove(ChuongTrinhKhuyenMai);
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

		private bool ProductExistsMA(string LOC_ID, string MA)
		{
			return _context.dm_KPI_KinhDoanh.Any((dm_KPI_KinhDoanh e) => e.LOC_ID == LOC_ID && e.MA == MA);
		}

		private bool ProductExistsID(string LOC_ID, string ID)
		{
			return _context.dm_KPI_KinhDoanh.Any((dm_KPI_KinhDoanh e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}

		private bool ProductExists(dm_KPI_KinhDoanh Product)
		{
			return _context.dm_KPI_KinhDoanh.Any((dm_KPI_KinhDoanh e) => e.LOC_ID == Product.LOC_ID && e.MA == Product.MA && e.ID != Product.ID);
		}

		[HttpPut]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> PutProduct([FromBody] SP_Parameter SP_Parameter)
		{
			try
			{
				new List<DanhSachPhieuDatHang_ChiTiet_KPI>();
				List<DanhSachPhieuTraHang_ChiTiet_KPI> lst_ChiTiet_TraHang = new List<DanhSachPhieuTraHang_ChiTiet_KPI>();
				List<v_Tinh_KPI_KinhDoanh> lstTinh_KPI_KinhDoanh = new List<v_Tinh_KPI_KinhDoanh>();
				List<view_dm_KPI_KinhDoanh> lstValue = await (from e in _context.view_dm_KPI_KinhDoanh
															  where e.LOC_ID == SP_Parameter.LOC_ID && e.TUNGAY <= SP_Parameter.TUNGAY && e.DENNGAY >= SP_Parameter.DENNGAY && e.ISACTIVE
															  orderby e.CAPDO
															  select e).ToListAsync();
				List<view_dm_HangHoa_KhungGia_HangHoa> lstValueKhungGia = await _context.view_dm_HangHoa_KhungGia_HangHoa.Where((view_dm_HangHoa_KhungGia_HangHoa e) => e.LOC_ID == SP_Parameter.LOC_ID && e.ISACTIVE).ToListAsync();
				if (lstValue != null || (lstValueKhungGia != null && lstValueKhungGia.Count > 0))
				{
					ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
					if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuTraHang_ChiTiet_KPI(SP_Parameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse })
					{
						lst_ChiTiet_TraHang = ApiResponse.Data as List<DanhSachPhieuTraHang_ChiTiet_KPI>;
					}
					if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuDatHang_ChiTiet_KPI(SP_Parameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse2 })
					{
						List<DanhSachPhieuDatHang_ChiTiet_KPI> lst_ChiTiet = ApiResponse2.Data as List<DanhSachPhieuDatHang_ChiTiet_KPI>;
						if (lst_ChiTiet != null && lst_ChiTiet_TraHang != null)
						{
							if (lstValue != null)
							{
								foreach (view_dm_KPI_KinhDoanh itm in lstValue)
								{
									List<view_dm_KPI_KinhDoanh_NhanVien> lstValue_NhanVien = await _context.view_dm_KPI_KinhDoanh_NhanVien.Where((view_dm_KPI_KinhDoanh_NhanVien e) => e.ID_KPI_KINHDOANH == itm.ID).ToListAsync();
									List<view_dm_KPI_KinhDoanh_YeuCau> lstValue_YeuCau = await _context.view_dm_KPI_KinhDoanh_YeuCau.Where((view_dm_KPI_KinhDoanh_YeuCau e) => e.ID_KPI_KINHDOANH == itm.ID).ToListAsync();
									foreach (view_dm_KPI_KinhDoanh_NhanVien NhanVien in lstValue_NhanVien)
									{
										if (NhanVien.HINHTHUC == 0 && (string.IsNullOrEmpty(SP_Parameter.ID_NHANVIEN) || NhanVien.ID_NHANVIEN == SP_Parameter.ID_NHANVIEN))
										{
											if (!string.IsNullOrEmpty(SP_Parameter.ID_NHOMQUYEN))
											{
												view_dm_NhanVien objNhanVien = await _context.view_dm_NhanVien.Where((view_dm_NhanVien e) => e.ID == NhanVien.ID_NHANVIEN).FirstOrDefaultAsync();
												if (objNhanVien != null && objNhanVien.ID_NHOMQUYEN != SP_Parameter.ID_NHOMQUYEN)
												{
													continue;
												}
											}
											v_Tinh_KPI_KinhDoanh NhanVienKinhDoanh = lstTinh_KPI_KinhDoanh.FirstOrDefault((v_Tinh_KPI_KinhDoanh e) => e.ID_NHANVIEN == NhanVien.ID_NHANVIEN);
											if (NhanVienKinhDoanh == null)
											{
												NhanVienKinhDoanh = new v_Tinh_KPI_KinhDoanh
												{
													lstTinh_KPI_KinhDoanh_ChiTiet = new List<v_Tinh_KPI_KinhDoanh_ChiTiet>(),
													ID_NHANVIEN = NhanVien.ID_NHANVIEN,
													NAME_NHANVIEN = NhanVien.MA + " - " + NhanVien.NAME
												};
												await _context.dm_NhanVien.FirstOrDefaultAsync((dm_NhanVien e) => e.ID == NhanVien.ID_NHANVIEN);
												NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange(Get_ChiTiet(itm, NhanVien, lstValue_YeuCau, lst_ChiTiet, NhanVienKinhDoanh, lst_ChiTiet_TraHang));
												NhanVienKinhDoanh.SOTIEN_KPI = NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Sum((v_Tinh_KPI_KinhDoanh_ChiTiet e) => e.SOTIEN_KPI);
												if (NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet != null && NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Count > 0)
												{
													lstTinh_KPI_KinhDoanh.Add(NhanVienKinhDoanh);
												}
											}
											else
											{
												if (NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.FirstOrDefault((v_Tinh_KPI_KinhDoanh_ChiTiet e) => e.ID_KPI_KINHDOANH == itm.ID) == null)
												{
													NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange(Get_ChiTiet(itm, NhanVien, lstValue_YeuCau, lst_ChiTiet, NhanVienKinhDoanh, lst_ChiTiet_TraHang));
												}
												NhanVienKinhDoanh.SOTIEN_KPI = NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Sum((v_Tinh_KPI_KinhDoanh_ChiTiet e) => e.SOTIEN_KPI);
											}
										}
										if (NhanVien.HINHTHUC != 1 || (!string.IsNullOrEmpty(SP_Parameter.ID_NHOMQUYEN) && !(NhanVien.ID_NHANVIEN == SP_Parameter.ID_NHOMQUYEN)))
										{
											continue;
										}
										List<view_dm_NhanVien> lstNhanVien = await _context.view_dm_NhanVien.Where((view_dm_NhanVien e) => e.ID_NHOMQUYEN == NhanVien.ID_NHANVIEN && e.ISACTIVE).ToListAsync();
										if (lstNhanVien == null)
										{
											continue;
										}
										foreach (view_dm_NhanVien item in lstNhanVien)
										{
											if (item == null || (!string.IsNullOrEmpty(SP_Parameter.ID_NHOMQUYEN) && item != null && item.ID_NHOMQUYEN != SP_Parameter.ID_NHOMQUYEN))
											{
												continue;
											}
											v_Tinh_KPI_KinhDoanh NhanVienKinhDoanh2 = lstTinh_KPI_KinhDoanh.FirstOrDefault((v_Tinh_KPI_KinhDoanh e) => e.ID_NHANVIEN == item.ID);
											if (NhanVienKinhDoanh2 == null)
											{
												NhanVienKinhDoanh2 = new v_Tinh_KPI_KinhDoanh
												{
													lstTinh_KPI_KinhDoanh_ChiTiet = new List<v_Tinh_KPI_KinhDoanh_ChiTiet>(),
													ID_NHANVIEN = item.ID,
													NAME_NHANVIEN = item.MA + " - " + item.NAME
												};
												await _context.dm_NhanVien.FirstOrDefaultAsync((dm_NhanVien e) => e.ID == item.ID);
												NhanVienKinhDoanh2.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange(Get_ChiTiet(itm, NhanVien, lstValue_YeuCau, lst_ChiTiet, NhanVienKinhDoanh2, lst_ChiTiet_TraHang));
												NhanVienKinhDoanh2.SOTIEN_KPI = NhanVienKinhDoanh2.lstTinh_KPI_KinhDoanh_ChiTiet.Sum((v_Tinh_KPI_KinhDoanh_ChiTiet e) => e.SOTIEN_KPI);
												if (NhanVienKinhDoanh2.lstTinh_KPI_KinhDoanh_ChiTiet != null && NhanVienKinhDoanh2.lstTinh_KPI_KinhDoanh_ChiTiet.Count > 0)
												{
													lstTinh_KPI_KinhDoanh.Add(NhanVienKinhDoanh2);
												}
											}
											else if (NhanVienKinhDoanh2.lstTinh_KPI_KinhDoanh_ChiTiet.FirstOrDefault((v_Tinh_KPI_KinhDoanh_ChiTiet e) => e.ID_KPI_KINHDOANH == itm.ID) == null)
											{
												NhanVienKinhDoanh2.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange(Get_ChiTiet(itm, NhanVien, lstValue_YeuCau, lst_ChiTiet, NhanVienKinhDoanh2, lst_ChiTiet_TraHang));
												NhanVienKinhDoanh2.SOTIEN_KPI = NhanVienKinhDoanh2.lstTinh_KPI_KinhDoanh_ChiTiet.Sum((v_Tinh_KPI_KinhDoanh_ChiTiet e) => e.SOTIEN_KPI);
											}
										}
									}
								}
							}
							if (lstValueKhungGia != null && lstValueKhungGia.Count > 0)
							{
								lst_ChiTiet = lst_ChiTiet.Where((DanhSachPhieuDatHang_ChiTiet_KPI s) => lstValueKhungGia.Select((view_dm_HangHoa_KhungGia_HangHoa e) => e.ID_HANGHOA).Contains(s.ID_HANGHOA)).ToList();
								lst_ChiTiet_TraHang = lst_ChiTiet_TraHang.Where((DanhSachPhieuTraHang_ChiTiet_KPI s) => lstValueKhungGia.Select((view_dm_HangHoa_KhungGia_HangHoa e) => e.ID_HANGHOA).Contains(s.ID_HANGHOA)).ToList();
								if (lst_ChiTiet != null && lst_ChiTiet.Count > 0 && lst_ChiTiet_TraHang != null)
								{
                                    List<KhungGiaMaster> lstMater = (from e in lstValueKhungGia
													group e by new { e.ID_HANGHOA_KHUNGGIA_MASTER } into g
													select new KhungGiaMaster
                                                    {
                                                        ID_HANGHOA_KHUNGGIA_MASTER = g.Key.ID_HANGHOA_KHUNGGIA_MASTER
                                                    }).ToList();
									var lstTaiKhoan = (from e in lst_ChiTiet
													   group e by new { e.ID_TAIKHOAN } into g
													   select new { g.Key.ID_TAIKHOAN }).ToList();
                                    KhungGiaMaster item2;
									foreach (var item3 in lstMater)
									{
										item2 = item3;
										dm_HangHoa_KhungGia_Master view_dm_HangHoa_KhungGia_Master = await _context.dm_HangHoa_KhungGia_Master.FirstOrDefaultAsync((dm_HangHoa_KhungGia_Master e) => e.ID == item2.ID_HANGHOA_KHUNGGIA_MASTER);
										if (view_dm_HangHoa_KhungGia_Master == null)
										{
											continue;
										}
										List<view_dm_HangHoa_KhungGia_HangHoa> lstHangHoa = await _context.view_dm_HangHoa_KhungGia_HangHoa.Where((view_dm_HangHoa_KhungGia_HangHoa e) => e.ID_HANGHOA_KHUNGGIA_MASTER == item2.ID_HANGHOA_KHUNGGIA_MASTER).ToListAsync();
										if (lstHangHoa == null)
										{
											continue;
										}
										foreach (var tk in lstTaiKhoan)
										{
											List<DanhSachPhieuDatHang_ChiTiet_KPI> lstChiTiet = lst_ChiTiet.Where((DanhSachPhieuDatHang_ChiTiet_KPI s) => lstHangHoa.Select((view_dm_HangHoa_KhungGia_HangHoa e) => e.ID_HANGHOA).Contains(s.ID_HANGHOA) && s.ID_TAIKHOAN == tk.ID_TAIKHOAN).ToList();
											lst_ChiTiet_TraHang.Where((DanhSachPhieuTraHang_ChiTiet_KPI s) => lstHangHoa.Select((view_dm_HangHoa_KhungGia_HangHoa e) => e.ID_HANGHOA).Contains(s.ID_HANGHOA) && s.ID_TAIKHOAN == tk.ID_TAIKHOAN).ToList();
											dm_NhanVien NhaVien = await _context.dm_NhanVien.FirstOrDefaultAsync((dm_NhanVien e) => e.ID_TAIKHOAN == tk.ID_TAIKHOAN);
											if (NhaVien == null)
											{
												continue;
											}
											List<view_dm_HangHoa_KhungGia> lstview_dm_HangHoa_KhungGia = await _context.view_dm_HangHoa_KhungGia.Where((view_dm_HangHoa_KhungGia e) => e.ID_HANGHOA_KHUNGGIA_MASTER == item2.ID_HANGHOA_KHUNGGIA_MASTER).ToListAsync();
											v_Tinh_KPI_KinhDoanh NhanVienKinhDoanh3 = lstTinh_KPI_KinhDoanh.FirstOrDefault((v_Tinh_KPI_KinhDoanh e) => e.ID_NHANVIEN == NhaVien.ID);
											if (NhanVienKinhDoanh3 == null)
											{
												NhanVienKinhDoanh3 = new v_Tinh_KPI_KinhDoanh();
												NhanVienKinhDoanh3.lstTinh_KPI_KinhDoanh_ChiTiet = new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
												NhanVienKinhDoanh3.ID_NHANVIEN = tk.ID_TAIKHOAN;
												NhanVienKinhDoanh3.NAME_NHANVIEN = NhaVien.MA + " - " + NhaVien.NAME;
												NhanVienKinhDoanh3.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange(Get_ChiTiet_KhungGia(view_dm_HangHoa_KhungGia_Master, lstChiTiet, lst_ChiTiet_TraHang, lstview_dm_HangHoa_KhungGia));
												NhanVienKinhDoanh3.SOTIEN_KPI = NhanVienKinhDoanh3.lstTinh_KPI_KinhDoanh_ChiTiet.Sum((v_Tinh_KPI_KinhDoanh_ChiTiet e) => e.SOTIEN_KPI);
												if (NhanVienKinhDoanh3.lstTinh_KPI_KinhDoanh_ChiTiet != null && NhanVienKinhDoanh3.lstTinh_KPI_KinhDoanh_ChiTiet.Count > 0)
												{
													lstTinh_KPI_KinhDoanh.Add(NhanVienKinhDoanh3);
												}
											}
											else
											{
												NhanVienKinhDoanh3.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange(Get_ChiTiet_KhungGia(view_dm_HangHoa_KhungGia_Master, lstChiTiet, lst_ChiTiet_TraHang, lstview_dm_HangHoa_KhungGia));
												NhanVienKinhDoanh3.SOTIEN_KPI = NhanVienKinhDoanh3.lstTinh_KPI_KinhDoanh_ChiTiet.Sum((v_Tinh_KPI_KinhDoanh_ChiTiet e) => e.SOTIEN_KPI);
											}
										}
									}
								}
							}
						}
					}
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = lstTinh_KPI_KinhDoanh
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

		private List<v_Tinh_KPI_KinhDoanh_ChiTiet> Get_ChiTiet(view_dm_KPI_KinhDoanh KPI_KinhDoanh, view_dm_KPI_KinhDoanh_NhanVien KPI_KinhDoanh_NhanVien, List<view_dm_KPI_KinhDoanh_YeuCau> lstValue_YeuCau, List<DanhSachPhieuDatHang_ChiTiet_KPI> lstChiTietDatHang, v_Tinh_KPI_KinhDoanh NhanVien_KinhDoanh, List<DanhSachPhieuTraHang_ChiTiet_KPI> lstChiTietTraHang)
		{
			try
			{
				List<v_Tinh_KPI_KinhDoanh_ChiTiet> list = new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
				bool flag = true;
				double num = 0.0;
				double num2 = 0.0;
				double num3 = 0.0;
				foreach (view_dm_KPI_KinhDoanh_YeuCau itm in lstValue_YeuCau)
				{
					double num4 = lstChiTietDatHang.Where((DanhSachPhieuDatHang_ChiTiet_KPI e) => !e.bolDaTinhKpi && ((itm.HINHTHUC == 0) ? (e.ID_HANGHOA == itm.ID_HANGHOA) : (e.ID_NHOMHANGHOA == itm.ID_HANGHOA)) && ((KPI_KinhDoanh_NhanVien.HINHTHUC == 0) ? (e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN) : (e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN))).Sum((DanhSachPhieuDatHang_ChiTiet_KPI e) => e.TONGCONG);
					double num5 = lstChiTietTraHang.Where((DanhSachPhieuTraHang_ChiTiet_KPI e) => !e.bolDaTinhKpi && ((itm.HINHTHUC == 0) ? (e.ID_HANGHOA == itm.ID_HANGHOA) : (e.ID_NHOMHANGHOA == itm.ID_HANGHOA)) && ((KPI_KinhDoanh_NhanVien.HINHTHUC == 0) ? (e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN) : (e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN))).Sum((DanhSachPhieuTraHang_ChiTiet_KPI e) => e.TONGCONG);
					double num6 = lstChiTietDatHang.Where(delegate (DanhSachPhieuDatHang_ChiTiet_KPI e)
					{
						bool num13;
						if (!e.bolDaTinhKpi)
						{
							if (itm.HINHTHUC != 0)
							{
								if (e.ID_NHOMHANGHOA == itm.ID_HANGHOA)
								{
									num13 = e.ID_DVT == itm.ID_DVT;
									goto IL_007c;
								}
							}
							else if (e.ID_HANGHOA == itm.ID_HANGHOA)
							{
								num13 = e.ID_DVT == itm.ID_DVT;
								goto IL_007c;
							}
						}
						goto IL_00ca;
					IL_00ca:
						int result = 0;
						goto IL_00cb;
					IL_007c:
						if (!num13)
						{
							goto IL_00ca;
						}
						result = (((KPI_KinhDoanh_NhanVien.HINHTHUC == 0) ? (e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN) : (e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)) ? 1 : 0);
						goto IL_00cb;
					IL_00cb:
						return (byte)result != 0;
					}).Sum((DanhSachPhieuDatHang_ChiTiet_KPI e) => e.TONGSOLUONG / e.TYLE_QD);
					int num7 = Convert.ToInt32(lstChiTietDatHang.Where(delegate (DanhSachPhieuDatHang_ChiTiet_KPI e)
					{
						bool num13;
						if (!e.bolDaTinhKpi)
						{
							if (itm.HINHTHUC != 0)
							{
								if (e.ID_NHOMHANGHOA == itm.ID_HANGHOA)
								{
									num13 = e.ID_DVT != itm.ID_DVT;
									goto IL_007c;
								}
							}
							else if (e.ID_HANGHOA == itm.ID_HANGHOA)
							{
								num13 = e.ID_DVT != itm.ID_DVT;
								goto IL_007c;
							}
						}
						goto IL_00ca;
					IL_00ca:
						int result = 0;
						goto IL_00cb;
					IL_007c:
						if (!num13)
						{
							goto IL_00ca;
						}
						result = (((KPI_KinhDoanh_NhanVien.HINHTHUC == 0) ? (e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN) : (e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)) ? 1 : 0);
						goto IL_00cb;
					IL_00cb:
						return (byte)result != 0;
					}).Sum((DanhSachPhieuDatHang_ChiTiet_KPI e) => e.TONGSOLUONG / ((e.TYLE_QD_HH == 0.0) ? 1.0 : e.TYLE_QD_HH)));
					num6 += (double)num7;
					double num8 = lstChiTietTraHang.Where(delegate (DanhSachPhieuTraHang_ChiTiet_KPI e)
					{
						bool num13;
						if (!e.bolDaTinhKpi)
						{
							if (itm.HINHTHUC != 0)
							{
								if (e.ID_NHOMHANGHOA == itm.ID_HANGHOA)
								{
									num13 = e.ID_DVT == itm.ID_DVT;
									goto IL_007c;
								}
							}
							else if (e.ID_HANGHOA == itm.ID_HANGHOA)
							{
								num13 = e.ID_DVT == itm.ID_DVT;
								goto IL_007c;
							}
						}
						goto IL_00ca;
					IL_00ca:
						int result = 0;
						goto IL_00cb;
					IL_007c:
						if (!num13)
						{
							goto IL_00ca;
						}
						result = (((KPI_KinhDoanh_NhanVien.HINHTHUC == 0) ? (e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN) : (e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)) ? 1 : 0);
						goto IL_00cb;
					IL_00cb:
						return (byte)result != 0;
					}).Sum((DanhSachPhieuTraHang_ChiTiet_KPI e) => e.TONGSOLUONG / e.TYLE_QD);
					int num9 = Convert.ToInt32(lstChiTietTraHang.Where(delegate (DanhSachPhieuTraHang_ChiTiet_KPI e)
					{
						bool num13;
						if (!e.bolDaTinhKpi)
						{
							if (itm.HINHTHUC != 0)
							{
								if (e.ID_NHOMHANGHOA == itm.ID_HANGHOA)
								{
									num13 = e.ID_DVT != itm.ID_DVT;
									goto IL_007c;
								}
							}
							else if (e.ID_HANGHOA == itm.ID_HANGHOA)
							{
								num13 = e.ID_DVT != itm.ID_DVT;
								goto IL_007c;
							}
						}
						goto IL_00ca;
					IL_00ca:
						int result = 0;
						goto IL_00cb;
					IL_007c:
						if (!num13)
						{
							goto IL_00ca;
						}
						result = (((KPI_KinhDoanh_NhanVien.HINHTHUC == 0) ? (e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN) : (e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)) ? 1 : 0);
						goto IL_00cb;
					IL_00cb:
						return (byte)result != 0;
					}).Sum((DanhSachPhieuTraHang_ChiTiet_KPI e) => e.TONGSOLUONG / ((e.TYLE_QD_HH == 0.0) ? 1.0 : e.TYLE_QD_HH)));
					num8 += (double)num9;
					if ((itm.SOTIEN > 0.0 || itm.SOLUONG > 0.0 || itm.CHIETKHAU > 0.0 || itm.TIENGIAM > 0.0) && itm.SOTIEN <= num4 - num5 && itm.SOLUONG <= num6 - num8)
					{
						dm_DonViTinh dm_DonViTinh2 = _context.dm_DonViTinh.Where((dm_DonViTinh e) => e.ID == itm.ID_DVT).FirstOrDefault();
						v_Tinh_KPI_KinhDoanh_ChiTiet v_Tinh_KPI_KinhDoanh_ChiTiet2 = new v_Tinh_KPI_KinhDoanh_ChiTiet();
						v_Tinh_KPI_KinhDoanh_ChiTiet2 = new v_Tinh_KPI_KinhDoanh_ChiTiet();
						v_Tinh_KPI_KinhDoanh_ChiTiet2.HINHTHUC = itm.HINHTHUC;
						v_Tinh_KPI_KinhDoanh_ChiTiet2.NAME_HINHTHUC = itm.NAME_HINHTHUC;
						v_Tinh_KPI_KinhDoanh_ChiTiet2.ID_KPI_KINHDOANH = KPI_KinhDoanh.ID;
						v_Tinh_KPI_KinhDoanh_ChiTiet2.NAME_KPI_KINHDOANH = KPI_KinhDoanh.NAME;
						v_Tinh_KPI_KinhDoanh_ChiTiet2.ID_HANGHOA = itm.ID_HANGHOA;
						v_Tinh_KPI_KinhDoanh_ChiTiet2.NAME_HANGHOA = itm.NAME;
						if (itm.CHIETKHAU > 0.0 || itm.TIENGIAM > 0.0)
						{
							v_Tinh_KPI_KinhDoanh_ChiTiet2.TONGTIEN = num4;
							v_Tinh_KPI_KinhDoanh_ChiTiet2.TONGTIEN_TRAHANG = num5;
							v_Tinh_KPI_KinhDoanh_ChiTiet2.TONGTIEN_KPI = itm.SOTIEN;
						}
						int num10 = 0;
						double num11 = 1.0;
						num10 = Convert.ToInt32(num6) / Convert.ToInt32(num11);
						v_Tinh_KPI_KinhDoanh_ChiTiet2.NAME_DVT = itm.NAME_DVT;
						v_Tinh_KPI_KinhDoanh_ChiTiet2.NAME_DVT_QD = ((dm_DonViTinh2 != null) ? dm_DonViTinh2.NAME : "");
						v_Tinh_KPI_KinhDoanh_ChiTiet2.TYLE_QD = num11;
						if (itm.HINHTHUC_TINHKPI == 1 && (itm.CHIETKHAU > 0.0 || itm.TIENGIAM > 0.0))
						{
							v_Tinh_KPI_KinhDoanh_ChiTiet2.TONGSOLUONG = Convert.ToInt32(num6) / Convert.ToInt32(num11);
							v_Tinh_KPI_KinhDoanh_ChiTiet2.TONGSOLUONG_TRAHANG = Convert.ToInt32(num8) / Convert.ToInt32(num11);
							v_Tinh_KPI_KinhDoanh_ChiTiet2.TONGSOLUONG_KPI = itm.SOLUONG.ToString("N0").Replace(',', '.') + " " + itm.NAME_DVT;
						}
						if (itm.HINHTHUC_TINHKPI == 3 && (itm.CHIETKHAU > 0.0 || itm.TIENGIAM > 0.0))
						{
							v_Tinh_KPI_KinhDoanh_ChiTiet2.TONGSOLUONG = Convert.ToInt32(num6) / Convert.ToInt32(num11);
							v_Tinh_KPI_KinhDoanh_ChiTiet2.TONGSOLUONG_TRAHANG = Convert.ToInt32(num8) / Convert.ToInt32(num11);
							v_Tinh_KPI_KinhDoanh_ChiTiet2.TONGSOLUONG_KPI = itm.SOLUONG.ToString("N0").Replace(',', '.') + " " + itm.NAME_DVT;
						}
						v_Tinh_KPI_KinhDoanh_ChiTiet2.CHIETKHAU = itm.CHIETKHAU;
						v_Tinh_KPI_KinhDoanh_ChiTiet2.TIENTHUONG = itm.TIENGIAM;
						num10 = Convert.ToInt32(num6 - num8) / Convert.ToInt32(num11);
						if (itm.HINHTHUC_TINHKPI == 1)
						{
							v_Tinh_KPI_KinhDoanh_ChiTiet2.SOTIEN_KPI = ((v_Tinh_KPI_KinhDoanh_ChiTiet2.CHIETKHAU > 0.0) ? (v_Tinh_KPI_KinhDoanh_ChiTiet2.CHIETKHAU * (num4 - num5) / 100.0) : (itm.TIENGIAM * ((double)num10 - itm.SOLUONG)));
						}
						else if (itm.HINHTHUC_TINHKPI == 3)
						{
							v_Tinh_KPI_KinhDoanh_ChiTiet2.SOTIEN_KPI = ((v_Tinh_KPI_KinhDoanh_ChiTiet2.CHIETKHAU > 0.0) ? (v_Tinh_KPI_KinhDoanh_ChiTiet2.CHIETKHAU * (num4 - num5) / 100.0) : (itm.TIENGIAM * (double)num10));
						}
						else if (itm.HINHTHUC_TINHKPI == 2)
						{
							v_Tinh_KPI_KinhDoanh_ChiTiet2.SOTIEN_KPI = ((v_Tinh_KPI_KinhDoanh_ChiTiet2.CHIETKHAU > 0.0) ? (v_Tinh_KPI_KinhDoanh_ChiTiet2.CHIETKHAU * (num4 - num5 - itm.SOTIEN) / 100.0) : itm.TIENGIAM);
						}
						else
						{
							v_Tinh_KPI_KinhDoanh_ChiTiet2.SOTIEN_KPI = ((v_Tinh_KPI_KinhDoanh_ChiTiet2.CHIETKHAU > 0.0) ? (v_Tinh_KPI_KinhDoanh_ChiTiet2.CHIETKHAU * (num4 - num5) / 100.0) : itm.TIENGIAM);
						}
						list.Add(v_Tinh_KPI_KinhDoanh_ChiTiet2);
						foreach (DanhSachPhieuDatHang_ChiTiet_KPI item in lstChiTietDatHang.Where((DanhSachPhieuDatHang_ChiTiet_KPI e) => !e.bolDaTinhKpi && ((itm.HINHTHUC == 0) ? (e.ID_HANGHOA == itm.ID_HANGHOA) : (e.ID_NHOMHANGHOA == itm.ID_HANGHOA)) && ((KPI_KinhDoanh_NhanVien.HINHTHUC == 0) ? (e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN) : (e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN))))
						{
						}
						foreach (DanhSachPhieuTraHang_ChiTiet_KPI item2 in lstChiTietTraHang.Where((DanhSachPhieuTraHang_ChiTiet_KPI e) => !e.bolDaTinhKpi && ((itm.HINHTHUC == 0) ? (e.ID_HANGHOA == itm.ID_HANGHOA) : (e.ID_NHOMHANGHOA == itm.ID_HANGHOA)) && ((KPI_KinhDoanh_NhanVien.HINHTHUC == 0) ? (e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN) : (e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN))))
						{
						}
						foreach (DanhSachPhieuDatHang_ChiTiet_KPI item3 in lstChiTietDatHang.Where(delegate (DanhSachPhieuDatHang_ChiTiet_KPI e)
						{
							bool num13;
							if (!e.bolDaTinhKpi)
							{
								if (itm.HINHTHUC != 0)
								{
									if (e.ID_NHOMHANGHOA == itm.ID_HANGHOA)
									{
										num13 = e.ID_DVT == itm.ID_DVT;
										goto IL_007c;
									}
								}
								else if (e.ID_HANGHOA == itm.ID_HANGHOA)
								{
									num13 = e.ID_DVT == itm.ID_DVT;
									goto IL_007c;
								}
							}
							goto IL_00ca;
						IL_00ca:
							int result = 0;
							goto IL_00cb;
						IL_007c:
							if (!num13)
							{
								goto IL_00ca;
							}
							result = (((KPI_KinhDoanh_NhanVien.HINHTHUC == 0) ? (e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN) : (e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)) ? 1 : 0);
							goto IL_00cb;
						IL_00cb:
							return (byte)result != 0;
						}))
						{
						}
						foreach (DanhSachPhieuTraHang_ChiTiet_KPI item4 in lstChiTietTraHang.Where(delegate (DanhSachPhieuTraHang_ChiTiet_KPI e)
						{
							bool num13;
							if (!e.bolDaTinhKpi)
							{
								if (itm.HINHTHUC != 0)
								{
									if (e.ID_NHOMHANGHOA == itm.ID_HANGHOA)
									{
										num13 = e.ID_DVT == itm.ID_DVT;
										goto IL_007c;
									}
								}
								else if (e.ID_HANGHOA == itm.ID_HANGHOA)
								{
									num13 = e.ID_DVT == itm.ID_DVT;
									goto IL_007c;
								}
							}
							goto IL_00ca;
						IL_00ca:
							int result = 0;
							goto IL_00cb;
						IL_007c:
							if (!num13)
							{
								goto IL_00ca;
							}
							result = (((KPI_KinhDoanh_NhanVien.HINHTHUC == 0) ? (e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN) : (e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)) ? 1 : 0);
							goto IL_00cb;
						IL_00cb:
							return (byte)result != 0;
						}))
						{
						}
						num += num4 - num5;
					}
					else
					{
						flag = false;
					}
					num2 += num4;
					num3 += num5;
				}
				double num12 = lstChiTietDatHang.Where((DanhSachPhieuDatHang_ChiTiet_KPI e) => e.ID_DVT == KPI_KinhDoanh.ID_DVT_DATKM).Sum((DanhSachPhieuDatHang_ChiTiet_KPI e) => e.TONGCONG);
				double tONGSOLUONG_TRAHANG = lstChiTietTraHang.Where((DanhSachPhieuTraHang_ChiTiet_KPI e) => e.ID_DVT == KPI_KinhDoanh.ID_DVT_DATKM).Sum((DanhSachPhieuTraHang_ChiTiet_KPI e) => e.TONGCONG);
				if (KPI_KinhDoanh.IS_YEUCAUCHITIET)
				{
					if (flag && (KPI_KinhDoanh.CHIETKHAU > 0.0 || KPI_KinhDoanh.TIENGIAM > 0.0))
					{
						v_Tinh_KPI_KinhDoanh_ChiTiet v_Tinh_KPI_KinhDoanh_ChiTiet3 = new v_Tinh_KPI_KinhDoanh_ChiTiet();
						v_Tinh_KPI_KinhDoanh_ChiTiet3.HINHTHUC = -1;
						v_Tinh_KPI_KinhDoanh_ChiTiet3.NAME_HINHTHUC = "Tổng";
						v_Tinh_KPI_KinhDoanh_ChiTiet3.ID_KPI_KINHDOANH = KPI_KinhDoanh.ID;
						v_Tinh_KPI_KinhDoanh_ChiTiet3.NAME_KPI_KINHDOANH = KPI_KinhDoanh.NAME;
						v_Tinh_KPI_KinhDoanh_ChiTiet3.NAME_HANGHOA = v_Tinh_KPI_KinhDoanh_ChiTiet3.NAME_HINHTHUC;
						v_Tinh_KPI_KinhDoanh_ChiTiet3.TONGTIEN = num2;
						v_Tinh_KPI_KinhDoanh_ChiTiet3.TONGTIEN_TRAHANG = num3;
						v_Tinh_KPI_KinhDoanh_ChiTiet3.TONGTIEN_KPI = KPI_KinhDoanh.TONGTIEN_DATKM;
						if (!string.IsNullOrEmpty(KPI_KinhDoanh.NAME_DVT))
						{
							v_Tinh_KPI_KinhDoanh_ChiTiet3.TONGSOLUONG = num12;
							v_Tinh_KPI_KinhDoanh_ChiTiet3.NAME_DVT = KPI_KinhDoanh.NAME_DVT;
							v_Tinh_KPI_KinhDoanh_ChiTiet3.TONGSOLUONG_TRAHANG = tONGSOLUONG_TRAHANG;
							v_Tinh_KPI_KinhDoanh_ChiTiet3.TONGSOLUONG_KPI = KPI_KinhDoanh.SOLUONG_DATKM + " " + KPI_KinhDoanh.NAME_DVT;
						}
						v_Tinh_KPI_KinhDoanh_ChiTiet3.CHIETKHAU = KPI_KinhDoanh.CHIETKHAU;
						v_Tinh_KPI_KinhDoanh_ChiTiet3.TIENTHUONG = KPI_KinhDoanh.TIENGIAM;
						v_Tinh_KPI_KinhDoanh_ChiTiet3.SOTIEN_KPI = ((v_Tinh_KPI_KinhDoanh_ChiTiet3.CHIETKHAU > 0.0) ? (v_Tinh_KPI_KinhDoanh_ChiTiet3.CHIETKHAU * num / 100.0) : KPI_KinhDoanh.TIENGIAM);
						list.Add(v_Tinh_KPI_KinhDoanh_ChiTiet3);
					}
				}
				else if ((KPI_KinhDoanh.CHIETKHAU > 0.0 || KPI_KinhDoanh.TIENGIAM > 0.0) && KPI_KinhDoanh.SOLUONG_DATKM <= num12 && KPI_KinhDoanh.TONGTIEN_DATKM <= num2)
				{
					v_Tinh_KPI_KinhDoanh_ChiTiet v_Tinh_KPI_KinhDoanh_ChiTiet4 = new v_Tinh_KPI_KinhDoanh_ChiTiet();
					v_Tinh_KPI_KinhDoanh_ChiTiet4.HINHTHUC = -1;
					v_Tinh_KPI_KinhDoanh_ChiTiet4.NAME_HINHTHUC = "Tổng";
					v_Tinh_KPI_KinhDoanh_ChiTiet4.ID_KPI_KINHDOANH = KPI_KinhDoanh.ID;
					v_Tinh_KPI_KinhDoanh_ChiTiet4.NAME_KPI_KINHDOANH = KPI_KinhDoanh.NAME;
					v_Tinh_KPI_KinhDoanh_ChiTiet4.NAME_HANGHOA = v_Tinh_KPI_KinhDoanh_ChiTiet4.NAME_HINHTHUC;
					v_Tinh_KPI_KinhDoanh_ChiTiet4.TONGTIEN = num2;
					v_Tinh_KPI_KinhDoanh_ChiTiet4.TONGTIEN_TRAHANG = num3;
					v_Tinh_KPI_KinhDoanh_ChiTiet4.TONGTIEN_KPI = KPI_KinhDoanh.TONGTIEN_DATKM;
					if (!string.IsNullOrEmpty(KPI_KinhDoanh.NAME_DVT))
					{
						v_Tinh_KPI_KinhDoanh_ChiTiet4.TONGSOLUONG = num12;
						v_Tinh_KPI_KinhDoanh_ChiTiet4.NAME_DVT = KPI_KinhDoanh.NAME_DVT;
						v_Tinh_KPI_KinhDoanh_ChiTiet4.TONGSOLUONG_TRAHANG = tONGSOLUONG_TRAHANG;
						v_Tinh_KPI_KinhDoanh_ChiTiet4.TONGSOLUONG_KPI = KPI_KinhDoanh.SOLUONG_DATKM + " " + KPI_KinhDoanh.NAME_DVT;
					}
					v_Tinh_KPI_KinhDoanh_ChiTiet4.CHIETKHAU = KPI_KinhDoanh.CHIETKHAU;
					v_Tinh_KPI_KinhDoanh_ChiTiet4.TIENTHUONG = KPI_KinhDoanh.TIENGIAM;
					v_Tinh_KPI_KinhDoanh_ChiTiet4.SOTIEN_KPI = ((v_Tinh_KPI_KinhDoanh_ChiTiet4.CHIETKHAU > 0.0) ? (v_Tinh_KPI_KinhDoanh_ChiTiet4.CHIETKHAU * (v_Tinh_KPI_KinhDoanh_ChiTiet4.TONGTIEN - v_Tinh_KPI_KinhDoanh_ChiTiet4.TONGTIEN_TRAHANG) / 100.0) : KPI_KinhDoanh.TIENGIAM);
					list.Add(v_Tinh_KPI_KinhDoanh_ChiTiet4);
				}
				return list;
			}
			catch
			{
				return new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
			}
		}

		private List<v_Tinh_KPI_KinhDoanh_ChiTiet> Get_ChiTiet_KhungGia(dm_HangHoa_KhungGia_Master dm_HangHoa_KhungGia_Master, List<DanhSachPhieuDatHang_ChiTiet_KPI> lstChiTietDatHang, List<DanhSachPhieuTraHang_ChiTiet_KPI> lstChiTietTraHang, List<view_dm_HangHoa_KhungGia> lstKhungGia)
		{
			if (lstKhungGia == null || lstKhungGia.Count == 0)
			{
				return new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
			}
			List<v_Tinh_KPI_KinhDoanh_ChiTiet> list = new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
			foreach (view_dm_HangHoa_KhungGia khunggia in lstKhungGia)
			{
				v_Tinh_KPI_KinhDoanh_ChiTiet newv_Tinh_KPI_KinhDoanh_ChiTiet = new v_Tinh_KPI_KinhDoanh_ChiTiet();
				newv_Tinh_KPI_KinhDoanh_ChiTiet.HINHTHUC = -1;
				newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_HINHTHUC = dm_HangHoa_KhungGia_Master.NAME;
				newv_Tinh_KPI_KinhDoanh_ChiTiet.ID_KPI_KINHDOANH = dm_HangHoa_KhungGia_Master.ID;
				newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_KPI_KINHDOANH = dm_HangHoa_KhungGia_Master.NAME;
				newv_Tinh_KPI_KinhDoanh_ChiTiet.ID_HANGHOA = khunggia.ID;
				newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_HANGHOA = dm_HangHoa_KhungGia_Master.NAME + " (" + khunggia.DONGIA.ToString("N0") + ")";
				newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN = (from s in lstChiTietDatHang
															where s.ID_DVT == khunggia.ID_DVT && s.DONGIA == khunggia.DONGIA
															group s by s.ID_PHIEUDATHANG into g
															select new
															{
																ID_PHIEUDATHANG = g.Key,
																TongSoLuong = g.Sum((DanhSachPhieuDatHang_ChiTiet_KPI x) => x.SOLUONG),
																TongTien = g.Sum((DanhSachPhieuDatHang_ChiTiet_KPI x) => x.TONGCONG)
															} into s
															where s.TongSoLuong >= khunggia.TU && s.TongSoLuong <= khunggia.DEN
															select s).Sum(s => s.TongTien);
				newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN_TRAHANG = lstChiTietTraHang.Where((DanhSachPhieuTraHang_ChiTiet_KPI s) => s.ID_DVT == khunggia.ID_DVT && s.DONGIA == khunggia.DONGIA).Sum((DanhSachPhieuTraHang_ChiTiet_KPI s) => s.TONGCONG);
				newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN_KPI = 0.0;
				newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU = khunggia.CK_KPI;
				newv_Tinh_KPI_KinhDoanh_ChiTiet.TIENTHUONG = khunggia.TIEN_KPI;
				if (newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN == 0.0 && newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN_TRAHANG == 0.0 && newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU == 0.0 && newv_Tinh_KPI_KinhDoanh_ChiTiet.TIENTHUONG == 0.0)
				{
					continue;
				}
				if (!string.IsNullOrEmpty(khunggia.NAME_DVT))
				{
					newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG = (from s in lstChiTietDatHang
																   where s.ID_DVT == khunggia.ID_DVT && s.DONGIA == khunggia.DONGIA
																   group s by s.ID_PHIEUDATHANG into g
																   select new
																   {
																	   ID_PHIEUDATHANG = g.Key,
																	   TongSoLuong = g.Sum((DanhSachPhieuDatHang_ChiTiet_KPI x) => x.SOLUONG),
																	   TongTien = g.Sum((DanhSachPhieuDatHang_ChiTiet_KPI x) => x.TONGCONG)
																   } into s
																   where s.TongSoLuong >= khunggia.TU && s.TongSoLuong <= khunggia.DEN
																   select s).Sum(s => s.TongSoLuong);
					newv_Tinh_KPI_KinhDoanh_ChiTiet.TYLE_QD = 1.0;
					newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_DVT = khunggia.NAME_DVT;
					newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG_TRAHANG = lstChiTietTraHang.Where((DanhSachPhieuTraHang_ChiTiet_KPI s) => s.ID_DVT == khunggia.ID_DVT && s.DONGIA == khunggia.DONGIA).Sum((DanhSachPhieuTraHang_ChiTiet_KPI s) => s.SOLUONG);
					newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG_KPI = khunggia.TU + " - " + khunggia.DEN + " " + khunggia.NAME_DVT;
				}
				if (khunggia.HINHTHUC_TINHKPI == 1)
				{
					var source = from e in lstChiTietDatHang
								 where e.ID_DVT == khunggia.ID_DVT && e.DONGIA == khunggia.DONGIA
								 group e by e.ID_PHIEUDATHANG into g
								 select new
								 {
									 ID_PHIEUDATHANG = g.Key,
									 TongSoLuong = g.Sum((DanhSachPhieuDatHang_ChiTiet_KPI x) => x.SOLUONG),
									 TongTien = g.Sum((DanhSachPhieuDatHang_ChiTiet_KPI x) => x.TONGCONG)
								 } into s
								 where s.TongSoLuong >= khunggia.TU && s.TongSoLuong <= khunggia.DEN
								 select s;
					newv_Tinh_KPI_KinhDoanh_ChiTiet.SOTIEN_KPI = source.Sum(x => (newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU > 0.0) ? (newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU * x.TongSoLuong / 100.0) : (x.TongSoLuong * newv_Tinh_KPI_KinhDoanh_ChiTiet.TIENTHUONG));
				}
				else
				{
					var source2 = from e in lstChiTietDatHang
								  where e.ID_DVT == khunggia.ID_DVT && e.DONGIA == khunggia.DONGIA
								  group e by e.ID_PHIEUDATHANG into g
								  select new
								  {
									  ID_PHIEUDATHANG = g.Key,
									  TongSoLuong = g.Sum((DanhSachPhieuDatHang_ChiTiet_KPI x) => x.SOLUONG),
									  TongTien = g.Sum((DanhSachPhieuDatHang_ChiTiet_KPI x) => x.TONGCONG)
								  } into s
								  where s.TongSoLuong >= khunggia.TU && s.TongSoLuong <= khunggia.DEN
								  select s;
					newv_Tinh_KPI_KinhDoanh_ChiTiet.SOTIEN_KPI = source2.Sum(x => (newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU > 0.0) ? (newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU * x.TongSoLuong / 100.0) : newv_Tinh_KPI_KinhDoanh_ChiTiet.TIENTHUONG);
				}
				list.Add(newv_Tinh_KPI_KinhDoanh_ChiTiet);
			}
			return list;
		}

		[HttpPost("PostCreateKPI_Sale")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> PostKPI_Sale([FromBody] List<Deposit> lstDeposit)
		{
			try
			{
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				foreach (Deposit item in lstDeposit)
				{
					dm_KPI_KinhDoanh KPI_KinhDoanh = (from e in _context.dm_KPI_KinhDoanh.AsNoTracking()
													  where e.LOC_ID == item.LOC_ID && e.ID == item.ID
													  select e).FirstOrDefault();
					if (KPI_KinhDoanh == null)
					{
						continue;
					}
					KPI_KinhDoanh.ID_NGUOITAO = item.ID_NGUOITAO;
					KPI_KinhDoanh.THOIGIANTHEM = DateTime.Now;
					KPI_KinhDoanh.ID_NGUOISUA = null;
					KPI_KinhDoanh.THOIGIANSUA = null;
					KPI_KinhDoanh.ID = Guid.NewGuid().ToString();
					KPI_KinhDoanh.TUNGAY = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
					KPI_KinhDoanh.DENNGAY = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
					KPI_KinhDoanh.MA = KPI_KinhDoanh.MA.Replace("(T" + DateTime.Now.AddMonths(-1).Month.ToString("00") + ")", "") + "(T" + DateTime.Now.Month.ToString("00") + ")";
					dm_KPI_KinhDoanh KPI_KinhDoanhCheck = (from e in _context.dm_KPI_KinhDoanh.AsNoTracking()
														   where e.LOC_ID == item.LOC_ID && e.MA == KPI_KinhDoanh.MA
														   select e).FirstOrDefault();
					if (KPI_KinhDoanhCheck != null)
					{
						KPI_KinhDoanh.MA += "_Copy";
					}
					KPI_KinhDoanh.NAME = KPI_KinhDoanh.NAME.Replace("(T" + DateTime.Now.AddMonths(-1).Month.ToString("00") + ")", "") + "(T" + DateTime.Now.Month.ToString("00") + ")";
					List<dm_KPI_KinhDoanh_NhanVien> lstdm_KPI_KinhDoanh_NhanVien = (from e in _context.dm_KPI_KinhDoanh_NhanVien.AsNoTracking()
																					where e.LOC_ID == item.LOC_ID && e.ID_KPI_KINHDOANH == item.ID
																					select e).ToList();
					List<dm_KPI_KinhDoanh_YeuCau> lstdm_KPI_KinhDoanh_YeuCau = (from e in _context.dm_KPI_KinhDoanh_YeuCau.AsNoTracking()
																				where e.LOC_ID == item.LOC_ID && e.ID_KPI_KINHDOANH == item.ID
																				select e).ToList();
					if (lstdm_KPI_KinhDoanh_NhanVien != null)
					{
						foreach (dm_KPI_KinhDoanh_NhanVien itm in lstdm_KPI_KinhDoanh_NhanVien)
						{
							itm.ID = Guid.NewGuid().ToString();
							itm.ID_KPI_KINHDOANH = KPI_KinhDoanh.ID;
							_context.dm_KPI_KinhDoanh_NhanVien.Add(itm);
						}
					}
					if (lstdm_KPI_KinhDoanh_YeuCau != null)
					{
						foreach (dm_KPI_KinhDoanh_YeuCau itm2 in lstdm_KPI_KinhDoanh_YeuCau)
						{
							itm2.ID = Guid.NewGuid().ToString();
							itm2.ID_KPI_KINHDOANH = KPI_KinhDoanh.ID;
							_context.dm_KPI_KinhDoanh_YeuCau.Add(itm2);
						}
					}
					_context.dm_KPI_KinhDoanh.Add(KPI_KinhDoanh);
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

		[HttpPut("PutKPI_Tam")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> PutKPI_Tam([FromBody] SP_Parameter SP_Parameter)
		{
			try
			{
				new List<DanhSachPhieuDatHang_ChiTiet_KPI>();
				List<DanhSachPhieuTraHang_ChiTiet_KPI> lst_ChiTiet_TraHang = new List<DanhSachPhieuTraHang_ChiTiet_KPI>();
				List<v_Tinh_KPI_KinhDoanh> lstTinh_KPI_KinhDoanh = new List<v_Tinh_KPI_KinhDoanh>();
				List<view_dm_KPI_KinhDoanh> lstValue = await (from e in _context.view_dm_KPI_KinhDoanh
															  where e.LOC_ID == SP_Parameter.LOC_ID && e.TUNGAY <= SP_Parameter.TUNGAY && e.DENNGAY >= SP_Parameter.DENNGAY && e.ISACTIVE
															  orderby e.CAPDO
															  select e).ToListAsync();
				if (lstValue != null)
				{
					ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
					if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuTraHang_ChiTiet_KPI(SP_Parameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse })
					{
						lst_ChiTiet_TraHang = ApiResponse.Data as List<DanhSachPhieuTraHang_ChiTiet_KPI>;
					}
					if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuDatHang_ChiTiet_KPI(SP_Parameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse2 })
					{
						List<DanhSachPhieuDatHang_ChiTiet_KPI> lst_ChiTiet = ApiResponse2.Data as List<DanhSachPhieuDatHang_ChiTiet_KPI>;
						if (lst_ChiTiet != null && lst_ChiTiet_TraHang != null)
						{
							foreach (view_dm_KPI_KinhDoanh itm in lstValue)
							{
								List<view_dm_KPI_KinhDoanh_NhanVien> lstValue_NhanVien = await _context.view_dm_KPI_KinhDoanh_NhanVien.Where((view_dm_KPI_KinhDoanh_NhanVien e) => e.ID_KPI_KINHDOANH == itm.ID).ToListAsync();
								List<view_dm_KPI_KinhDoanh_YeuCau> lstValue_YeuCau = await _context.view_dm_KPI_KinhDoanh_YeuCau.Where((view_dm_KPI_KinhDoanh_YeuCau e) => e.ID_KPI_KINHDOANH == itm.ID).ToListAsync();
								foreach (view_dm_KPI_KinhDoanh_NhanVien NhanVien in lstValue_NhanVien)
								{
									if (NhanVien.HINHTHUC == 0 && (string.IsNullOrEmpty(SP_Parameter.ID_NHANVIEN) || NhanVien.ID_NHANVIEN == SP_Parameter.ID_NHANVIEN))
									{
										if (!string.IsNullOrEmpty(SP_Parameter.ID_NHOMQUYEN))
										{
											view_dm_NhanVien objNhanVien = await _context.view_dm_NhanVien.Where((view_dm_NhanVien e) => e.ID == NhanVien.ID_NHANVIEN).FirstOrDefaultAsync();
											if (objNhanVien != null && objNhanVien.ID_NHOMQUYEN != SP_Parameter.ID_NHOMQUYEN)
											{
												continue;
											}
										}
										v_Tinh_KPI_KinhDoanh NhanVienKinhDoanh = lstTinh_KPI_KinhDoanh.FirstOrDefault((v_Tinh_KPI_KinhDoanh e) => e.ID_NHANVIEN == NhanVien.ID_NHANVIEN);
										if (NhanVienKinhDoanh == null)
										{
											NhanVienKinhDoanh = new v_Tinh_KPI_KinhDoanh
											{
												lstTinh_KPI_KinhDoanh_ChiTiet = new List<v_Tinh_KPI_KinhDoanh_ChiTiet>(),
												ID_NHANVIEN = NhanVien.ID_NHANVIEN,
												NAME_NHANVIEN = NhanVien.MA + " - " + NhanVien.NAME
											};
											await _context.dm_NhanVien.FirstOrDefaultAsync((dm_NhanVien e) => e.ID == NhanVien.ID_NHANVIEN);
											NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange(Get_ChiTietTam(itm, NhanVien, lstValue_YeuCau, lst_ChiTiet, NhanVienKinhDoanh, lst_ChiTiet_TraHang));
											NhanVienKinhDoanh.SOTIEN_KPI = NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Sum((v_Tinh_KPI_KinhDoanh_ChiTiet e) => e.SOTIEN_KPI);
											if (NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet != null && NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Count > 0)
											{
												lstTinh_KPI_KinhDoanh.Add(NhanVienKinhDoanh);
											}
										}
										else
										{
											if (NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.FirstOrDefault((v_Tinh_KPI_KinhDoanh_ChiTiet e) => e.ID_KPI_KINHDOANH == itm.ID) == null)
											{
												NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange(Get_ChiTietTam(itm, NhanVien, lstValue_YeuCau, lst_ChiTiet, NhanVienKinhDoanh, lst_ChiTiet_TraHang));
											}
											NhanVienKinhDoanh.SOTIEN_KPI = NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Sum((v_Tinh_KPI_KinhDoanh_ChiTiet e) => e.SOTIEN_KPI);
										}
									}
									if (NhanVien.HINHTHUC != 1 || (!string.IsNullOrEmpty(SP_Parameter.ID_NHOMQUYEN) && !(NhanVien.ID_NHANVIEN == SP_Parameter.ID_NHOMQUYEN)))
									{
										continue;
									}
									List<view_dm_NhanVien> lstNhanVien = await _context.view_dm_NhanVien.Where((view_dm_NhanVien e) => e.ID_NHOMQUYEN == NhanVien.ID_NHANVIEN && e.ISACTIVE).ToListAsync();
									if (lstNhanVien == null)
									{
										continue;
									}
									foreach (view_dm_NhanVien item in lstNhanVien)
									{
										if (item == null || (!string.IsNullOrEmpty(SP_Parameter.ID_NHOMQUYEN) && item != null && item.ID_NHOMQUYEN != SP_Parameter.ID_NHOMQUYEN))
										{
											continue;
										}
										v_Tinh_KPI_KinhDoanh NhanVienKinhDoanh2 = lstTinh_KPI_KinhDoanh.FirstOrDefault((v_Tinh_KPI_KinhDoanh e) => e.ID_NHANVIEN == item.ID);
										if (NhanVienKinhDoanh2 == null)
										{
											NhanVienKinhDoanh2 = new v_Tinh_KPI_KinhDoanh
											{
												lstTinh_KPI_KinhDoanh_ChiTiet = new List<v_Tinh_KPI_KinhDoanh_ChiTiet>(),
												ID_NHANVIEN = item.ID,
												NAME_NHANVIEN = item.MA + " - " + item.NAME
											};
											await _context.dm_NhanVien.FirstOrDefaultAsync((dm_NhanVien e) => e.ID == item.ID);
											NhanVienKinhDoanh2.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange(Get_ChiTietTam(itm, NhanVien, lstValue_YeuCau, lst_ChiTiet, NhanVienKinhDoanh2, lst_ChiTiet_TraHang));
											NhanVienKinhDoanh2.SOTIEN_KPI = NhanVienKinhDoanh2.lstTinh_KPI_KinhDoanh_ChiTiet.Sum((v_Tinh_KPI_KinhDoanh_ChiTiet e) => e.SOTIEN_KPI);
											if (NhanVienKinhDoanh2.lstTinh_KPI_KinhDoanh_ChiTiet != null && NhanVienKinhDoanh2.lstTinh_KPI_KinhDoanh_ChiTiet.Count > 0)
											{
												lstTinh_KPI_KinhDoanh.Add(NhanVienKinhDoanh2);
											}
										}
										else if (NhanVienKinhDoanh2.lstTinh_KPI_KinhDoanh_ChiTiet.FirstOrDefault((v_Tinh_KPI_KinhDoanh_ChiTiet e) => e.ID_KPI_KINHDOANH == itm.ID) == null)
										{
											NhanVienKinhDoanh2.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange(Get_ChiTietTam(itm, NhanVien, lstValue_YeuCau, lst_ChiTiet, NhanVienKinhDoanh2, lst_ChiTiet_TraHang));
										}
									}
								}
							}
						}
					}
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = lstTinh_KPI_KinhDoanh
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

		private List<v_Tinh_KPI_KinhDoanh_ChiTiet> Get_ChiTietTam(view_dm_KPI_KinhDoanh KPI_KinhDoanh, view_dm_KPI_KinhDoanh_NhanVien KPI_KinhDoanh_NhanVien, List<view_dm_KPI_KinhDoanh_YeuCau> lstValue_YeuCau, List<DanhSachPhieuDatHang_ChiTiet_KPI> lstChiTietDatHang, v_Tinh_KPI_KinhDoanh NhanVien_KinhDoanh, List<DanhSachPhieuTraHang_ChiTiet_KPI> lstChiTietTraHang)
		{
			try
			{
				List<v_Tinh_KPI_KinhDoanh_ChiTiet> list = new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
				bool flag = true;
				double num = 0.0;
				double num2 = 0.0;
				double num3 = 0.0;
				foreach (view_dm_KPI_KinhDoanh_YeuCau itm in lstValue_YeuCau)
				{
					double num4 = lstChiTietDatHang.Where((DanhSachPhieuDatHang_ChiTiet_KPI e) => !e.bolDaTinhKpi && ((itm.HINHTHUC == 0) ? (e.ID_HANGHOA == itm.ID_HANGHOA) : (e.ID_NHOMHANGHOA == itm.ID_HANGHOA)) && ((KPI_KinhDoanh_NhanVien.HINHTHUC == 0) ? (e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN) : (e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN))).Sum((DanhSachPhieuDatHang_ChiTiet_KPI e) => e.TONGCONG);
					double num5 = lstChiTietTraHang.Where((DanhSachPhieuTraHang_ChiTiet_KPI e) => !e.bolDaTinhKpi && ((itm.HINHTHUC == 0) ? (e.ID_HANGHOA == itm.ID_HANGHOA) : (e.ID_NHOMHANGHOA == itm.ID_HANGHOA)) && ((KPI_KinhDoanh_NhanVien.HINHTHUC == 0) ? (e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN) : (e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN))).Sum((DanhSachPhieuTraHang_ChiTiet_KPI e) => e.TONGCONG);
					double num6 = lstChiTietDatHang.Where(delegate (DanhSachPhieuDatHang_ChiTiet_KPI e)
					{
						bool num13;
						if (!e.bolDaTinhKpi)
						{
							if (itm.HINHTHUC != 0)
							{
								if (e.ID_NHOMHANGHOA == itm.ID_HANGHOA)
								{
									num13 = e.ID_DVT == itm.ID_DVT;
									goto IL_007c;
								}
							}
							else if (e.ID_HANGHOA == itm.ID_HANGHOA)
							{
								num13 = e.ID_DVT == itm.ID_DVT;
								goto IL_007c;
							}
						}
						goto IL_00ca;
					IL_00ca:
						int result = 0;
						goto IL_00cb;
					IL_007c:
						if (!num13)
						{
							goto IL_00ca;
						}
						result = (((KPI_KinhDoanh_NhanVien.HINHTHUC == 0) ? (e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN) : (e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)) ? 1 : 0);
						goto IL_00cb;
					IL_00cb:
						return (byte)result != 0;
					}).Sum((DanhSachPhieuDatHang_ChiTiet_KPI e) => e.TONGSOLUONG / e.TYLE_QD);
					int num7 = Convert.ToInt32(lstChiTietDatHang.Where(delegate (DanhSachPhieuDatHang_ChiTiet_KPI e)
					{
						bool num13;
						if (!e.bolDaTinhKpi)
						{
							if (itm.HINHTHUC != 0)
							{
								if (e.ID_NHOMHANGHOA == itm.ID_HANGHOA)
								{
									num13 = e.ID_DVT != itm.ID_DVT;
									goto IL_007c;
								}
							}
							else if (e.ID_HANGHOA == itm.ID_HANGHOA)
							{
								num13 = e.ID_DVT != itm.ID_DVT;
								goto IL_007c;
							}
						}
						goto IL_00ca;
					IL_00ca:
						int result = 0;
						goto IL_00cb;
					IL_007c:
						if (!num13)
						{
							goto IL_00ca;
						}
						result = (((KPI_KinhDoanh_NhanVien.HINHTHUC == 0) ? (e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN) : (e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)) ? 1 : 0);
						goto IL_00cb;
					IL_00cb:
						return (byte)result != 0;
					}).Sum((DanhSachPhieuDatHang_ChiTiet_KPI e) => e.TONGSOLUONG / ((e.TYLE_QD_HH == 0.0) ? 1.0 : e.TYLE_QD_HH)));
					num6 += (double)num7;
					double num8 = lstChiTietTraHang.Where(delegate (DanhSachPhieuTraHang_ChiTiet_KPI e)
					{
						bool num13;
						if (!e.bolDaTinhKpi)
						{
							if (itm.HINHTHUC != 0)
							{
								if (e.ID_NHOMHANGHOA == itm.ID_HANGHOA)
								{
									num13 = e.ID_DVT == itm.ID_DVT;
									goto IL_007c;
								}
							}
							else if (e.ID_HANGHOA == itm.ID_HANGHOA)
							{
								num13 = e.ID_DVT == itm.ID_DVT;
								goto IL_007c;
							}
						}
						goto IL_00ca;
					IL_00ca:
						int result = 0;
						goto IL_00cb;
					IL_007c:
						if (!num13)
						{
							goto IL_00ca;
						}
						result = (((KPI_KinhDoanh_NhanVien.HINHTHUC == 0) ? (e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN) : (e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)) ? 1 : 0);
						goto IL_00cb;
					IL_00cb:
						return (byte)result != 0;
					}).Sum((DanhSachPhieuTraHang_ChiTiet_KPI e) => e.TONGSOLUONG / e.TYLE_QD);
					int num9 = Convert.ToInt32(lstChiTietTraHang.Where(delegate (DanhSachPhieuTraHang_ChiTiet_KPI e)
					{
						bool num13;
						if (!e.bolDaTinhKpi)
						{
							if (itm.HINHTHUC != 0)
							{
								if (e.ID_NHOMHANGHOA == itm.ID_HANGHOA)
								{
									num13 = e.ID_DVT != itm.ID_DVT;
									goto IL_007c;
								}
							}
							else if (e.ID_HANGHOA == itm.ID_HANGHOA)
							{
								num13 = e.ID_DVT != itm.ID_DVT;
								goto IL_007c;
							}
						}
						goto IL_00ca;
					IL_00ca:
						int result = 0;
						goto IL_00cb;
					IL_007c:
						if (!num13)
						{
							goto IL_00ca;
						}
						result = (((KPI_KinhDoanh_NhanVien.HINHTHUC == 0) ? (e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN) : (e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)) ? 1 : 0);
						goto IL_00cb;
					IL_00cb:
						return (byte)result != 0;
					}).Sum((DanhSachPhieuTraHang_ChiTiet_KPI e) => e.TONGSOLUONG / ((e.TYLE_QD_HH == 0.0) ? 1.0 : e.TYLE_QD_HH)));
					num8 += (double)num9;
					if ((itm.SOTIEN > 0.0 || itm.SOLUONG > 0.0 || itm.CHIETKHAU > 0.0 || itm.TIENGIAM > 0.0) && 0.0 < num4 - num5 && 0.0 <= num6 - num8)
					{
						dm_DonViTinh dm_DonViTinh2 = _context.dm_DonViTinh.Where((dm_DonViTinh e) => e.ID == itm.ID_DVT).FirstOrDefault();
						v_Tinh_KPI_KinhDoanh_ChiTiet v_Tinh_KPI_KinhDoanh_ChiTiet2 = new v_Tinh_KPI_KinhDoanh_ChiTiet();
						v_Tinh_KPI_KinhDoanh_ChiTiet2 = new v_Tinh_KPI_KinhDoanh_ChiTiet();
						v_Tinh_KPI_KinhDoanh_ChiTiet2.HINHTHUC = itm.HINHTHUC;
						v_Tinh_KPI_KinhDoanh_ChiTiet2.NAME_HINHTHUC = itm.NAME_HINHTHUC;
						v_Tinh_KPI_KinhDoanh_ChiTiet2.ID_KPI_KINHDOANH = KPI_KinhDoanh.ID;
						v_Tinh_KPI_KinhDoanh_ChiTiet2.NAME_KPI_KINHDOANH = KPI_KinhDoanh.NAME;
						v_Tinh_KPI_KinhDoanh_ChiTiet2.ID_HANGHOA = itm.ID_HANGHOA;
						v_Tinh_KPI_KinhDoanh_ChiTiet2.NAME_HANGHOA = itm.NAME;
						if (itm.CHIETKHAU > 0.0 || itm.TIENGIAM > 0.0)
						{
							v_Tinh_KPI_KinhDoanh_ChiTiet2.TONGTIEN = num4;
							v_Tinh_KPI_KinhDoanh_ChiTiet2.TONGTIEN_TRAHANG = num5;
							v_Tinh_KPI_KinhDoanh_ChiTiet2.TONGTIEN_KPI = itm.SOTIEN;
						}
						int num10 = 0;
						double num11 = 1.0;
						num10 = Convert.ToInt32(num6) / Convert.ToInt32(num11);
						v_Tinh_KPI_KinhDoanh_ChiTiet2.NAME_DVT = itm.NAME_DVT;
						v_Tinh_KPI_KinhDoanh_ChiTiet2.NAME_DVT_QD = ((dm_DonViTinh2 != null) ? dm_DonViTinh2.NAME : "");
						v_Tinh_KPI_KinhDoanh_ChiTiet2.TYLE_QD = num11;
						if (itm.HINHTHUC_TINHKPI == 1 && (itm.CHIETKHAU > 0.0 || itm.TIENGIAM > 0.0))
						{
							v_Tinh_KPI_KinhDoanh_ChiTiet2.TONGSOLUONG = Convert.ToInt32(num6) / Convert.ToInt32(num11);
							v_Tinh_KPI_KinhDoanh_ChiTiet2.TONGSOLUONG_TRAHANG = Convert.ToInt32(num8) / Convert.ToInt32(num11);
							v_Tinh_KPI_KinhDoanh_ChiTiet2.TONGSOLUONG_KPI = itm.SOLUONG.ToString("N0").Replace(',', '.') + " " + itm.NAME_DVT;
						}
						if (itm.HINHTHUC_TINHKPI == 3 && (itm.CHIETKHAU > 0.0 || itm.TIENGIAM > 0.0))
						{
							v_Tinh_KPI_KinhDoanh_ChiTiet2.TONGSOLUONG = Convert.ToInt32(num6) / Convert.ToInt32(num11);
							v_Tinh_KPI_KinhDoanh_ChiTiet2.TONGSOLUONG_TRAHANG = Convert.ToInt32(num8) / Convert.ToInt32(num11);
							v_Tinh_KPI_KinhDoanh_ChiTiet2.TONGSOLUONG_KPI = itm.SOLUONG.ToString("N0").Replace(',', '.') + " " + itm.NAME_DVT;
						}
						v_Tinh_KPI_KinhDoanh_ChiTiet2.CHIETKHAU = itm.CHIETKHAU;
						v_Tinh_KPI_KinhDoanh_ChiTiet2.TIENTHUONG = itm.TIENGIAM;
						num10 = Convert.ToInt32(num6 - num8) / Convert.ToInt32(num11);
						if (itm.SOTIEN <= num4 - num5 && itm.SOLUONG <= num6 - num8)
						{
							if (itm.HINHTHUC_TINHKPI == 1)
							{
								v_Tinh_KPI_KinhDoanh_ChiTiet2.SOTIEN_KPI = ((v_Tinh_KPI_KinhDoanh_ChiTiet2.CHIETKHAU > 0.0) ? (v_Tinh_KPI_KinhDoanh_ChiTiet2.CHIETKHAU * (num4 - num5) / 100.0) : (itm.TIENGIAM * ((double)num10 - itm.SOLUONG)));
							}
							else if (itm.HINHTHUC_TINHKPI == 3)
							{
								v_Tinh_KPI_KinhDoanh_ChiTiet2.SOTIEN_KPI = ((v_Tinh_KPI_KinhDoanh_ChiTiet2.CHIETKHAU > 0.0) ? (v_Tinh_KPI_KinhDoanh_ChiTiet2.CHIETKHAU * (num4 - num5) / 100.0) : (itm.TIENGIAM * (double)num10));
							}
							else if (itm.HINHTHUC_TINHKPI == 2)
							{
								v_Tinh_KPI_KinhDoanh_ChiTiet2.SOTIEN_KPI = ((v_Tinh_KPI_KinhDoanh_ChiTiet2.CHIETKHAU > 0.0) ? (v_Tinh_KPI_KinhDoanh_ChiTiet2.CHIETKHAU * (num4 - num5 - itm.SOTIEN) / 100.0) : itm.TIENGIAM);
							}
							else
							{
								v_Tinh_KPI_KinhDoanh_ChiTiet2.SOTIEN_KPI = ((v_Tinh_KPI_KinhDoanh_ChiTiet2.CHIETKHAU > 0.0) ? (v_Tinh_KPI_KinhDoanh_ChiTiet2.CHIETKHAU * (num4 - num5) / 100.0) : itm.TIENGIAM);
							}
						}
						list.Add(v_Tinh_KPI_KinhDoanh_ChiTiet2);
						foreach (DanhSachPhieuDatHang_ChiTiet_KPI item in lstChiTietDatHang.Where((DanhSachPhieuDatHang_ChiTiet_KPI e) => !e.bolDaTinhKpi && ((itm.HINHTHUC == 0) ? (e.ID_HANGHOA == itm.ID_HANGHOA) : (e.ID_NHOMHANGHOA == itm.ID_HANGHOA)) && ((KPI_KinhDoanh_NhanVien.HINHTHUC == 0) ? (e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN) : (e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN))))
						{
						}
						foreach (DanhSachPhieuTraHang_ChiTiet_KPI item2 in lstChiTietTraHang.Where((DanhSachPhieuTraHang_ChiTiet_KPI e) => !e.bolDaTinhKpi && ((itm.HINHTHUC == 0) ? (e.ID_HANGHOA == itm.ID_HANGHOA) : (e.ID_NHOMHANGHOA == itm.ID_HANGHOA)) && ((KPI_KinhDoanh_NhanVien.HINHTHUC == 0) ? (e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN) : (e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN))))
						{
						}
						foreach (DanhSachPhieuDatHang_ChiTiet_KPI item3 in lstChiTietDatHang.Where(delegate (DanhSachPhieuDatHang_ChiTiet_KPI e)
						{
							bool num13;
							if (!e.bolDaTinhKpi)
							{
								if (itm.HINHTHUC != 0)
								{
									if (e.ID_NHOMHANGHOA == itm.ID_HANGHOA)
									{
										num13 = e.ID_DVT == itm.ID_DVT;
										goto IL_007c;
									}
								}
								else if (e.ID_HANGHOA == itm.ID_HANGHOA)
								{
									num13 = e.ID_DVT == itm.ID_DVT;
									goto IL_007c;
								}
							}
							goto IL_00ca;
						IL_00ca:
							int result = 0;
							goto IL_00cb;
						IL_007c:
							if (!num13)
							{
								goto IL_00ca;
							}
							result = (((KPI_KinhDoanh_NhanVien.HINHTHUC == 0) ? (e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN) : (e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)) ? 1 : 0);
							goto IL_00cb;
						IL_00cb:
							return (byte)result != 0;
						}))
						{
						}
						foreach (DanhSachPhieuTraHang_ChiTiet_KPI item4 in lstChiTietTraHang.Where(delegate (DanhSachPhieuTraHang_ChiTiet_KPI e)
						{
							bool num13;
							if (!e.bolDaTinhKpi)
							{
								if (itm.HINHTHUC != 0)
								{
									if (e.ID_NHOMHANGHOA == itm.ID_HANGHOA)
									{
										num13 = e.ID_DVT == itm.ID_DVT;
										goto IL_007c;
									}
								}
								else if (e.ID_HANGHOA == itm.ID_HANGHOA)
								{
									num13 = e.ID_DVT == itm.ID_DVT;
									goto IL_007c;
								}
							}
							goto IL_00ca;
						IL_00ca:
							int result = 0;
							goto IL_00cb;
						IL_007c:
							if (!num13)
							{
								goto IL_00ca;
							}
							result = (((KPI_KinhDoanh_NhanVien.HINHTHUC == 0) ? (e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN) : (e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)) ? 1 : 0);
							goto IL_00cb;
						IL_00cb:
							return (byte)result != 0;
						}))
						{
						}
						num += num4 - num5;
					}
					else
					{
						flag = false;
					}
					num2 += num4;
					num3 += num5;
				}
				double num12 = lstChiTietDatHang.Where((DanhSachPhieuDatHang_ChiTiet_KPI e) => e.ID_DVT == KPI_KinhDoanh.ID_DVT_DATKM).Sum((DanhSachPhieuDatHang_ChiTiet_KPI e) => e.TONGCONG);
				double tONGSOLUONG_TRAHANG = lstChiTietTraHang.Where((DanhSachPhieuTraHang_ChiTiet_KPI e) => e.ID_DVT == KPI_KinhDoanh.ID_DVT_DATKM).Sum((DanhSachPhieuTraHang_ChiTiet_KPI e) => e.TONGCONG);
				if (KPI_KinhDoanh.IS_YEUCAUCHITIET)
				{
					if (flag && (KPI_KinhDoanh.CHIETKHAU > 0.0 || KPI_KinhDoanh.TIENGIAM > 0.0))
					{
						v_Tinh_KPI_KinhDoanh_ChiTiet v_Tinh_KPI_KinhDoanh_ChiTiet3 = new v_Tinh_KPI_KinhDoanh_ChiTiet();
						v_Tinh_KPI_KinhDoanh_ChiTiet3.HINHTHUC = -1;
						v_Tinh_KPI_KinhDoanh_ChiTiet3.NAME_HINHTHUC = "Tổng";
						v_Tinh_KPI_KinhDoanh_ChiTiet3.ID_KPI_KINHDOANH = KPI_KinhDoanh.ID;
						v_Tinh_KPI_KinhDoanh_ChiTiet3.NAME_KPI_KINHDOANH = KPI_KinhDoanh.NAME;
						v_Tinh_KPI_KinhDoanh_ChiTiet3.NAME_HANGHOA = v_Tinh_KPI_KinhDoanh_ChiTiet3.NAME_HINHTHUC;
						v_Tinh_KPI_KinhDoanh_ChiTiet3.TONGTIEN = num2;
						v_Tinh_KPI_KinhDoanh_ChiTiet3.TONGTIEN_TRAHANG = num3;
						v_Tinh_KPI_KinhDoanh_ChiTiet3.TONGTIEN_KPI = KPI_KinhDoanh.TONGTIEN_DATKM;
						if (!string.IsNullOrEmpty(KPI_KinhDoanh.NAME_DVT))
						{
							v_Tinh_KPI_KinhDoanh_ChiTiet3.TONGSOLUONG = num12;
							v_Tinh_KPI_KinhDoanh_ChiTiet3.NAME_DVT = KPI_KinhDoanh.NAME_DVT;
							v_Tinh_KPI_KinhDoanh_ChiTiet3.TONGSOLUONG_TRAHANG = tONGSOLUONG_TRAHANG;
							v_Tinh_KPI_KinhDoanh_ChiTiet3.TONGSOLUONG_KPI = KPI_KinhDoanh.SOLUONG_DATKM + " " + KPI_KinhDoanh.NAME_DVT;
						}
						v_Tinh_KPI_KinhDoanh_ChiTiet3.CHIETKHAU = KPI_KinhDoanh.CHIETKHAU;
						v_Tinh_KPI_KinhDoanh_ChiTiet3.TIENTHUONG = KPI_KinhDoanh.TIENGIAM;
						v_Tinh_KPI_KinhDoanh_ChiTiet3.SOTIEN_KPI = ((v_Tinh_KPI_KinhDoanh_ChiTiet3.CHIETKHAU > 0.0) ? (v_Tinh_KPI_KinhDoanh_ChiTiet3.CHIETKHAU * num / 100.0) : KPI_KinhDoanh.TIENGIAM);
						list.Add(v_Tinh_KPI_KinhDoanh_ChiTiet3);
					}
				}
				else if ((KPI_KinhDoanh.CHIETKHAU > 0.0 || KPI_KinhDoanh.TIENGIAM > 0.0) && 0.0 < num12 && 0.0 < num2)
				{
					v_Tinh_KPI_KinhDoanh_ChiTiet v_Tinh_KPI_KinhDoanh_ChiTiet4 = new v_Tinh_KPI_KinhDoanh_ChiTiet();
					v_Tinh_KPI_KinhDoanh_ChiTiet4.HINHTHUC = -1;
					v_Tinh_KPI_KinhDoanh_ChiTiet4.NAME_HINHTHUC = "Tổng";
					v_Tinh_KPI_KinhDoanh_ChiTiet4.ID_KPI_KINHDOANH = KPI_KinhDoanh.ID;
					v_Tinh_KPI_KinhDoanh_ChiTiet4.NAME_KPI_KINHDOANH = KPI_KinhDoanh.NAME;
					v_Tinh_KPI_KinhDoanh_ChiTiet4.NAME_HANGHOA = v_Tinh_KPI_KinhDoanh_ChiTiet4.NAME_HINHTHUC;
					v_Tinh_KPI_KinhDoanh_ChiTiet4.TONGTIEN = num2;
					v_Tinh_KPI_KinhDoanh_ChiTiet4.TONGTIEN_TRAHANG = num3;
					v_Tinh_KPI_KinhDoanh_ChiTiet4.TONGTIEN_KPI = KPI_KinhDoanh.TONGTIEN_DATKM;
					if (!string.IsNullOrEmpty(KPI_KinhDoanh.NAME_DVT))
					{
						v_Tinh_KPI_KinhDoanh_ChiTiet4.TONGSOLUONG = num12;
						v_Tinh_KPI_KinhDoanh_ChiTiet4.NAME_DVT = KPI_KinhDoanh.NAME_DVT;
						v_Tinh_KPI_KinhDoanh_ChiTiet4.TONGSOLUONG_TRAHANG = tONGSOLUONG_TRAHANG;
						v_Tinh_KPI_KinhDoanh_ChiTiet4.TONGSOLUONG_KPI = KPI_KinhDoanh.SOLUONG_DATKM + " " + KPI_KinhDoanh.NAME_DVT;
					}
					v_Tinh_KPI_KinhDoanh_ChiTiet4.CHIETKHAU = KPI_KinhDoanh.CHIETKHAU;
					v_Tinh_KPI_KinhDoanh_ChiTiet4.TIENTHUONG = KPI_KinhDoanh.TIENGIAM;
					if (KPI_KinhDoanh.SOLUONG_DATKM <= num12 && KPI_KinhDoanh.TONGTIEN_DATKM <= num2)
					{
						v_Tinh_KPI_KinhDoanh_ChiTiet4.SOTIEN_KPI = ((v_Tinh_KPI_KinhDoanh_ChiTiet4.CHIETKHAU > 0.0) ? (v_Tinh_KPI_KinhDoanh_ChiTiet4.CHIETKHAU * (v_Tinh_KPI_KinhDoanh_ChiTiet4.TONGTIEN - v_Tinh_KPI_KinhDoanh_ChiTiet4.TONGTIEN_TRAHANG) / 100.0) : KPI_KinhDoanh.TIENGIAM);
					}
					list.Add(v_Tinh_KPI_KinhDoanh_ChiTiet4);
				}
				return list;
			}
			catch
			{
				return new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
			}
		}
	}
}
public class KhungGiaMaster
{
    public string ID_HANGHOA_KHUNGGIA_MASTER { get; set; }
}
