using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.StoredProcedure;
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
	public class PayrollController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public PayrollController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetArea(string LOC_ID)
		{
			try
			{
				List<view_nv_BangLuong> lstValue = await (from e in _context.view_nv_BangLuong
														  where e.LOC_ID == LOC_ID
														  orderby e.NAMTHANG_ORDERBY
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
		public async Task<IActionResult> GetArea(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<view_nv_BangLuong> lstValue = await (from e in _context.view_nv_BangLuong.Where((view_nv_BangLuong e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch)
														  orderby e.NAMTHANG_ORDERBY
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
		public async Task<IActionResult> GetArea(string LOC_ID, string ID)
		{
			try
			{
				view_nv_BangLuong Area = await _context.view_nv_BangLuong.FirstOrDefaultAsync((view_nv_BangLuong e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Area == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				v_nv_BangLuong ct_PhieuDatHang2 = new v_nv_BangLuong();
				if (Area != null)
				{
					string strDeposit = JsonConvert.SerializeObject(Area);
					ct_PhieuDatHang2 = JsonConvert.DeserializeObject<v_nv_BangLuong>(strDeposit) ?? new v_nv_BangLuong();
				}
				ct_PhieuDatHang2.lstnv_BangLuong_ChiTiet = new List<nv_BangLuong_ChiTiet>();
				List<nv_BangLuong_ChiTiet> lstValue = await _context.nv_BangLuong_ChiTiet.Where((nv_BangLuong_ChiTiet e) => e.LOC_ID == LOC_ID && e.ID_BANGLUONG == ID).ToListAsync();
				if (lstValue != null)
				{
					ct_PhieuDatHang2.lstnv_BangLuong_ChiTiet = lstValue;
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ct_PhieuDatHang2
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
		public async Task<IActionResult> PutArea(v_nv_BangLuong Area)
		{
			try
			{
				if (await _context.view_nv_BangLuong.FirstOrDefaultAsync((view_nv_BangLuong e) => e.LOC_ID == Area.LOC_ID && e.ID_THANGLUONG == Area.ID_THANGLUONG && e.ID != Area.ID) != null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại tháng lương " + Area.LOC_ID + "-" + Area.ID_THANGLUONG + " dữ liệu bảng lương!",
						Data = ""
					});
				}
				List<nv_BangLuong_ChiTiet> lstValueChiTiet = await (from e in _context.nv_BangLuong_ChiTiet.AsNoTracking()
																	where e.LOC_ID == Area.LOC_ID && e.ID_BANGLUONG == Area.ID
																	select e).ToListAsync();
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				foreach (nv_BangLuong_ChiTiet itm in lstValueChiTiet)
				{
					if (Area.lstnv_BangLuong_ChiTiet.Count((nv_BangLuong_ChiTiet s) => s.ID == itm.ID) <= 0)
					{
						_context.nv_BangLuong_ChiTiet.Remove(itm);
					}
				}
				foreach (nv_BangLuong_ChiTiet itm2 in Area.lstnv_BangLuong_ChiTiet)
				{
					if (lstValueChiTiet.Count((nv_BangLuong_ChiTiet s) => s.ID == itm2.ID) <= 0)
					{
						itm2.ID_BANGLUONG = Area.ID;
						_context.nv_BangLuong_ChiTiet.Add(itm2);
					}
					else
					{
						_context.Entry(itm2).State = EntityState.Modified;
					}
				}
				Area.TIENLUONG = Area.lstnv_BangLuong_ChiTiet.Where((nv_BangLuong_ChiTiet s) => Convert.ToInt32(s.TYPE) > 0).Sum((nv_BangLuong_ChiTiet s) => s.SOTIEN);
				Area.TIENGIAM = Area.lstnv_BangLuong_ChiTiet.Where((nv_BangLuong_ChiTiet s) => Convert.ToInt32(s.TYPE) < 0).Sum((nv_BangLuong_ChiTiet s) => s.SOTIEN);
				Area.TIENTHUCNHAN = Area.TIENLUONG - Area.TIENGIAM;
				_context.Entry(Area).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
				view_nv_BangLuong view_nv_BangLuong2 = await _context.view_nv_BangLuong.FirstOrDefaultAsync((view_nv_BangLuong e) => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID);
				if (view_nv_BangLuong2 == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + Area.LOC_ID + "-" + Area.ID + " dữ liệu!",
						Data = ""
					});
				}
				v_nv_BangLuong ct_PhieuDatHang2 = new v_nv_BangLuong();
				if (view_nv_BangLuong2 != null)
				{
					string strDeposit = JsonConvert.SerializeObject(view_nv_BangLuong2);
					ct_PhieuDatHang2 = JsonConvert.DeserializeObject<v_nv_BangLuong>(strDeposit) ?? new v_nv_BangLuong();
				}
				ct_PhieuDatHang2.lstnv_BangLuong_ChiTiet = new List<nv_BangLuong_ChiTiet>();
				List<nv_BangLuong_ChiTiet> lstValue = await _context.nv_BangLuong_ChiTiet.Where((nv_BangLuong_ChiTiet e) => e.LOC_ID == Area.LOC_ID && e.ID_BANGLUONG == Area.ID).ToListAsync();
				if (lstValue != null)
				{
					ct_PhieuDatHang2.lstnv_BangLuong_ChiTiet = lstValue;
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ct_PhieuDatHang2
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
		public async Task<ActionResult<dm_KhuVuc>> PostArea(v_nv_BangLuong Area)
		{
			try
			{
				if (await _context.view_nv_BangLuong.FirstOrDefaultAsync((view_nv_BangLuong e) => e.LOC_ID == Area.LOC_ID && e.ID_THANGLUONG == Area.ID_THANGLUONG) != null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại tháng lương " + Area.LOC_ID + "-" + Area.ID_THANGLUONG + " dữ liệu bảng lương!",
						Data = ""
					});
				}
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				foreach (nv_BangLuong_ChiTiet itm in Area.lstnv_BangLuong_ChiTiet)
				{
					_context.nv_BangLuong_ChiTiet.Add(itm);
				}
				Area.TIENLUONG = Area.lstnv_BangLuong_ChiTiet.Where((nv_BangLuong_ChiTiet s) => Convert.ToInt32(s.TYPE) > 0).Sum((nv_BangLuong_ChiTiet s) => s.SOTIEN);
				Area.TIENGIAM = Area.lstnv_BangLuong_ChiTiet.Where((nv_BangLuong_ChiTiet s) => Convert.ToInt32(s.TYPE) < 0).Sum((nv_BangLuong_ChiTiet s) => s.SOTIEN);
				Area.TIENTHUCNHAN = Area.TIENLUONG - Area.TIENGIAM;
				_context.nv_BangLuong.Add(Area);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
				view_nv_BangLuong view_nv_BangLuong2 = await _context.view_nv_BangLuong.FirstOrDefaultAsync((view_nv_BangLuong e) => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID);
				if (view_nv_BangLuong2 == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + Area.LOC_ID + "-" + Area.ID + " dữ liệu!",
						Data = ""
					});
				}
				v_nv_BangLuong ct_PhieuDatHang2 = new v_nv_BangLuong();
				if (view_nv_BangLuong2 != null)
				{
					string strDeposit = JsonConvert.SerializeObject(view_nv_BangLuong2);
					ct_PhieuDatHang2 = JsonConvert.DeserializeObject<v_nv_BangLuong>(strDeposit) ?? new v_nv_BangLuong();
				}
				ct_PhieuDatHang2.lstnv_BangLuong_ChiTiet = new List<nv_BangLuong_ChiTiet>();
				List<nv_BangLuong_ChiTiet> lstValue = await _context.nv_BangLuong_ChiTiet.Where((nv_BangLuong_ChiTiet e) => e.LOC_ID == Area.LOC_ID && e.ID_BANGLUONG == Area.ID).ToListAsync();
				if (lstValue != null)
				{
					ct_PhieuDatHang2.lstnv_BangLuong_ChiTiet = lstValue;
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ct_PhieuDatHang2
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
		public async Task<IActionResult> DeleteArea(string LOC_ID, string ID)
		{
			try
			{
				nv_BangLuong Area = await _context.nv_BangLuong.FirstOrDefaultAsync((nv_BangLuong e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Area == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				foreach (nv_BangLuong_ChiTiet itm in await (from e in _context.nv_BangLuong_ChiTiet.AsNoTracking()
															where e.LOC_ID == LOC_ID && e.ID_BANGLUONG == ID
															select e).ToListAsync())
				{
					_context.nv_BangLuong_ChiTiet.Remove(itm);
				}
				_context.nv_BangLuong.Remove(Area);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
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

		[HttpPost("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<ActionResult<dm_KhuVuc>> GetLuongThang(v_nv_BangLuong Area, string LOC_ID)
		{
			try
			{
				if (await _context.view_nv_BangLuong.FirstOrDefaultAsync((view_nv_BangLuong e) => e.LOC_ID == Area.LOC_ID && e.ID_THANGLUONG == Area.ID_THANGLUONG) != null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại tháng lương " + Area.LOC_ID + "-" + Area.ID_THANGLUONG + " dữ liệu bảng lương!",
						Data = ""
					});
				}
				view_dm_NhanVien NhanVien = await _context.view_dm_NhanVien.FirstOrDefaultAsync((view_dm_NhanVien e) => e.ID == Area.ID_NHANVIEN);
				if (NhanVien == null || string.IsNullOrEmpty(NhanVien.ID_TAIKHOAN))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy nhân viên: " + Area.ID_NHANVIEN,
						Data = ""
					});
				}
				dm_PhongBan PhongBan = await _context.dm_PhongBan.FirstOrDefaultAsync((dm_PhongBan e) => e.ID == NhanVien.ID_PHONGBAN);
				if (PhongBan == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy phòng ban: " + NhanVien.ID_PHONGBAN,
						Data = ""
					});
				}
				dm_ThangLuong dm_ThangLuong2 = await _context.dm_ThangLuong.FirstOrDefaultAsync((dm_ThangLuong e) => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID_THANGLUONG && string.Concat("," + e.ID_PHONGBAN, ",").Contains(string.Concat("," + PhongBan.MA, ",")) && e.ISACTIVE);
				if (dm_ThangLuong2 == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy danh mục tháng lương: " + Area.ID_THANGLUONG + "-" + PhongBan.MA,
						Data = ""
					});
				}
				if (await _context.AspNetUsers.FirstOrDefaultAsync((AspNetUsers e) => e.ID == NhanVien.ID_TAIKHOAN) == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy tài khoản: " + NhanVien.ID_TAIKHOAN,
						Data = ""
					});
				}
				dm_BangLuong vdm_BangLuong = await _context.dm_BangLuong.FirstOrDefaultAsync((dm_BangLuong e) => e.LOC_ID == Area.LOC_ID && (string.IsNullOrEmpty(e.ID_PHONGBAN) || e.ID_PHONGBAN == NhanVien.ID_PHONGBAN));
				if (vdm_BangLuong == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy danh mục bảng lương cho phòng ban: " + NhanVien.ID_PHONGBAN,
						Data = ""
					});
				}
				List<dm_BangLuong_ChiTiet> lstdm_BangLuong_ChiTiet = await _context.dm_BangLuong_ChiTiet.Where((dm_BangLuong_ChiTiet e) => e.LOC_ID == Area.LOC_ID && e.ID_BANGLUONG == vdm_BangLuong.ID).ToListAsync();
				string GhiChuNghiPhep = "";
				string GhiChuNghiKhongPhep = "";
				string GhiChuVeSom = "";
				double SoNgayCong = 0.0;
				double SoNgayNghiPhep = 0.0;
				double SoNgayLamViec = 0.0;
				double SoNgayNghiKhongPhep = 0.0;
				double SoNgayCoDiLam = 0.0;
				DateTime date = dm_ThangLuong2.NGAYBATDAU;
				while (true)
				{
					if (!(date <= dm_ThangLuong2.NGAYKETTHUC))
					{
						break;
					}
					bool bolDungPhep = false;
					if (!string.IsNullOrEmpty(dm_ThangLuong2.DANHSACHNGAYNGHI) && !("," + dm_ThangLuong2.DANHSACHNGAYNGHI + ",").Contains("," + date.Day + ","))
					{
						nv_NghiPhep nv_NghiPhep2 = await _context.nv_NghiPhep.FirstOrDefaultAsync((nv_NghiPhep e) => e.LOC_ID == Area.LOC_ID && e.THOIGIANRA.Date >= ((DateTime)date).Date && e.THOIGIANVAO.Date <= ((DateTime)date).Date && e.ID_NHANVIEN == NhanVien.ID);
						if (nv_NghiPhep2?.ISNGHIPHEP ?? false)
						{
							if (string.IsNullOrEmpty(GhiChuNghiPhep))
							{
								GhiChuNghiPhep = "Nghỉ phép: ";
							}
							SoNgayCong += 1.0;
							if (nv_NghiPhep2.HINHTHUCNGHIPHEP == 0)
							{
								SoNgayNghiPhep += 1.0;
								GhiChuNghiPhep = GhiChuNghiPhep + date.Day + "(" + (nv_NghiPhep2.ISDUYETPHEP ? "Duyệt" : "Chưa duyệt") + " 1 ngày);";
								goto IL_1b08;
							}
							SoNgayNghiPhep += 0.5;
							GhiChuNghiPhep = GhiChuNghiPhep + date.Day + "(" + (nv_NghiPhep2.ISDUYETPHEP ? "Duyệt" : "Chưa duyệt") + " 0.5 ngày);";
							bolDungPhep = true;
						}
						nv_ChamCong nv_ChamCong2 = await _context.nv_ChamCong.FirstOrDefaultAsync((nv_ChamCong e) => e.LOC_ID == Area.LOC_ID && e.NGAYCONG.Date == ((DateTime)date).Date);
						if (nv_ChamCong2 != null)
						{
							TimeSpan GIOBATDAU = dm_ThangLuong2.GIOBATDAU;
							TimeSpan GIOKETTHUC = dm_ThangLuong2.GIOKETTHUC;
							double SoGioLe = 0.0;
							if (nv_ChamCong2.THOIGIANVAO.HasValue && nv_ChamCong2.THOIGIANRA.HasValue)
							{
								if (nv_ChamCong2.THOIGIANVAO.HasValue && nv_ChamCong2.THOIGIANVAO.Value.TimeOfDay > dm_ThangLuong2.GIOBATDAU)
								{
									GIOBATDAU = nv_ChamCong2.THOIGIANVAO.Value.TimeOfDay;
									if (!bolDungPhep)
									{
										if (string.IsNullOrEmpty(GhiChuVeSom))
										{
											GhiChuVeSom = "Đi trễ, về sớm: ";
										}
										GhiChuVeSom = GhiChuVeSom + "(v1) " + nv_ChamCong2.THOIGIANVAO.Value.ToString("dd HH:mm") + ";";
									}
								}
								if (nv_ChamCong2.THOIGIANRA.HasValue && nv_ChamCong2.THOIGIANRA.Value.TimeOfDay < dm_ThangLuong2.GIOKETTHUC)
								{
									GIOKETTHUC = nv_ChamCong2.THOIGIANRA.Value.TimeOfDay;
									if (!bolDungPhep)
									{
										if (string.IsNullOrEmpty(GhiChuVeSom))
										{
											GhiChuVeSom = "Đi trễ, về sớm: ";
										}
										GhiChuVeSom = GhiChuVeSom + "(v2) " + nv_ChamCong2.THOIGIANRA.Value.ToString("dd HH:mm") + ";";
									}
								}
								double SoGioLamViec = (GIOKETTHUC - GIOBATDAU).TotalHours;
								TimeSpan SoTiengLamTrongNgay = dm_ThangLuong2.GIOKETTHUC - dm_ThangLuong2.GIOBATDAU;
								double SoGioNghiTrua = (dm_ThangLuong2.GIOKETTHUC_NGHITRUA - dm_ThangLuong2.GIOBATDAU_NGHITRUA).TotalHours;
								double SoGioNghiTruaTrongNgay = (dm_ThangLuong2.GIOKETTHUC_NGHITRUA - dm_ThangLuong2.GIOBATDAU_NGHITRUA).TotalHours;
								if (GIOBATDAU < dm_ThangLuong2.GIOBATDAU_NGHITRUA && GIOKETTHUC <= dm_ThangLuong2.GIOKETTHUC_NGHITRUA)
								{
									SoGioNghiTrua = (GIOKETTHUC - dm_ThangLuong2.GIOBATDAU_NGHITRUA).TotalHours;
								}
								if (GIOBATDAU > dm_ThangLuong2.GIOKETTHUC_NGHITRUA && GIOKETTHUC > dm_ThangLuong2.GIOKETTHUC_NGHITRUA)
								{
									SoGioNghiTrua = (dm_ThangLuong2.GIOKETTHUC_NGHITRUA - dm_ThangLuong2.GIOBATDAU_NGHITRUA).TotalHours;
								}
								if (GIOBATDAU < dm_ThangLuong2.GIOBATDAU_NGHITRUA && GIOKETTHUC <= dm_ThangLuong2.GIOBATDAU_NGHITRUA)
								{
									SoGioNghiTrua = 0.0;
								}
								if (GIOBATDAU >= dm_ThangLuong2.GIOKETTHUC_NGHITRUA)
								{
									SoGioNghiTrua = 0.0;
								}
								SoGioLe = (SoGioLamViec - SoGioNghiTrua) / (SoTiengLamTrongNgay.TotalHours - SoGioNghiTruaTrongNgay);
								if (bolDungPhep)
								{
									SoGioLe = 0.5;
								}
							}
							else
							{
								if (string.IsNullOrEmpty(GhiChuVeSom))
								{
									GhiChuVeSom = "Đi trễ, về sớm: ";
								}
								GhiChuVeSom = GhiChuVeSom + "(v1v2) " + date.ToString("dd") + ";";
							}
							SoNgayCong += SoGioLe;
							SoNgayLamViec += SoGioLe;
							SoNgayCoDiLam += 1.0;
						}
						else if (!dm_ThangLuong2.ISCHAMCONG && !bolDungPhep)
						{
							SoNgayCong += 1.0;
							SoNgayLamViec += 1.0;
							SoNgayCoDiLam += 1.0;
						}
						if ((nv_NghiPhep2 == null && nv_ChamCong2 == null && dm_ThangLuong2.ISCHAMCONG) || (nv_NghiPhep2 != null && !nv_NghiPhep2.ISNGHIPHEP))
						{
							SoNgayNghiKhongPhep += 1.0;
							if (string.IsNullOrEmpty(GhiChuNghiKhongPhep))
							{
								GhiChuNghiKhongPhep = "Nghỉ không phép: ";
							}
							GhiChuNghiKhongPhep = GhiChuNghiKhongPhep + "Ngày " + date.Day + ((nv_NghiPhep2 != null && nv_NghiPhep2.ISDUYETPHEP) ? " Đã duyệt" : ((nv_NghiPhep2 != null) ? " Chưa duyệt nghỉ" : " Chưa xin nghỉ")) + ";";
						}
					}
					goto IL_1b08;
				IL_1b08:
					date = date.AddDays(1.0);
				}
				Area.MUCLUONG = NhanVien.LUONGCOBAN;
				Area.SONGAYCONG = dm_ThangLuong2.SONGAYCONG;
				Area.SONGAYLAMVIEC = Math.Round(SoNgayLamViec, 2);
				Area.SONGAYNGHIPHEP = SoNgayNghiPhep;
				Area.SONGAYNGHIKHONGPHEP = SoNgayNghiKhongPhep;
				Area.GHICHU = GhiChuNghiPhep + (string.IsNullOrEmpty(GhiChuNghiKhongPhep) ? "" : (Environment.NewLine + GhiChuNghiKhongPhep)) + (string.IsNullOrEmpty(GhiChuVeSom) ? "" : (Environment.NewLine + GhiChuVeSom));
				if (lstdm_BangLuong_ChiTiet != null)
				{
					Area.lstnv_BangLuong_ChiTiet = new List<nv_BangLuong_ChiTiet>();
					foreach (dm_BangLuong_ChiTiet itm in lstdm_BangLuong_ChiTiet)
					{
						nv_BangLuong_ChiTiet newnv_BangLuong_ChiTiet = new nv_BangLuong_ChiTiet
						{
							ID_LOAILUONG = itm.ID_LOAILUONG,
							ID_BANGLUONG = Area.ID,
							ID = Guid.NewGuid().ToString()
						};
						if (itm.TYPE_QUYTACTINHLUONG == 0)
						{
							if (itm.TYPE_LUONG == 0)
							{
								newnv_BangLuong_ChiTiet.SOTIEN = Math.Ceiling(((itm.SOTIEN > 0.0) ? itm.SOTIEN : NhanVien.LUONGCOBAN) / dm_ThangLuong2.SONGAYCONG * (Area.SONGAYLAMVIEC + SoNgayNghiPhep));
							}
							if (itm.TYPE_LUONG == 1)
							{
								SP_Parameter SP_Parameter = new SP_Parameter();
								KPI_SaleController KPI = new KPI_SaleController(_context, _configuration);
								SP_Parameter.LOC_ID = Area.LOC_ID;
								SP_Parameter.TUNGAY = dm_ThangLuong2.NGAYBATDAU;
								SP_Parameter.DENNGAY = dm_ThangLuong2.NGAYKETTHUC;
								SP_Parameter.ID_NHANVIEN = NhanVien.ID;
								if (await KPI.PutProduct(SP_Parameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse })
								{
									List<v_Tinh_KPI_KinhDoanh> lst_ChiTiet = ApiResponse.Data as List<v_Tinh_KPI_KinhDoanh>;
									newnv_BangLuong_ChiTiet.SOTIEN = Math.Ceiling(((itm.SOTIEN > 0.0) ? itm.SOTIEN : ((lst_ChiTiet != null && lst_ChiTiet.Count > 0) ? lst_ChiTiet[0].SOTIEN_KPI : 0.0)) / dm_ThangLuong2.SONGAYCONG * (Area.SONGAYLAMVIEC + SoNgayNghiPhep));
								}
							}
							if (itm.TYPE_LUONG == 2)
							{
								newnv_BangLuong_ChiTiet.SOTIEN = Math.Ceiling(itm.SOTIEN / dm_ThangLuong2.SONGAYCONG * (Area.SONGAYLAMVIEC + SoNgayNghiPhep));
							}
						}
						else if (itm.TYPE_QUYTACTINHLUONG == 1)
						{
							if (itm.TYPE_LUONG == 0)
							{
								newnv_BangLuong_ChiTiet.SOTIEN = Math.Ceiling(((itm.SOTIEN > 0.0) ? itm.SOTIEN : NhanVien.LUONGCOBAN) / dm_ThangLuong2.SONGAYCONG * SoNgayCoDiLam);
							}
							if (itm.TYPE_LUONG == 1)
							{
								SP_Parameter SP_Parameter2 = new SP_Parameter();
								KPI_SaleController KPI2 = new KPI_SaleController(_context, _configuration);
								SP_Parameter2.LOC_ID = Area.LOC_ID;
								SP_Parameter2.TUNGAY = dm_ThangLuong2.NGAYBATDAU;
								SP_Parameter2.DENNGAY = dm_ThangLuong2.NGAYKETTHUC;
								SP_Parameter2.ID_NHANVIEN = NhanVien.ID;
								if (await KPI2.PutProduct(SP_Parameter2) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse2 })
								{
									List<v_Tinh_KPI_KinhDoanh> lst_ChiTiet2 = ApiResponse2.Data as List<v_Tinh_KPI_KinhDoanh>;
									newnv_BangLuong_ChiTiet.SOTIEN = Math.Ceiling(((itm.SOTIEN > 0.0) ? itm.SOTIEN : ((lst_ChiTiet2 != null && lst_ChiTiet2.Count > 0) ? lst_ChiTiet2[0].SOTIEN_KPI : 0.0)) / dm_ThangLuong2.SONGAYCONG * SoNgayCoDiLam);
								}
							}
							if (itm.TYPE_LUONG == 2)
							{
								newnv_BangLuong_ChiTiet.SOTIEN = Math.Ceiling(itm.SOTIEN / dm_ThangLuong2.SONGAYCONG * SoNgayCoDiLam);
							}
						}
						else if (itm.TYPE_QUYTACTINHLUONG == 2)
						{
							if (itm.TYPE_LUONG == 0)
							{
								newnv_BangLuong_ChiTiet.SOTIEN = Math.Ceiling((itm.SOTIEN > 0.0) ? itm.SOTIEN : NhanVien.LUONGCOBAN);
							}
							if (itm.TYPE_LUONG == 1)
							{
								SP_Parameter SP_Parameter3 = new SP_Parameter();
								KPI_SaleController KPI3 = new KPI_SaleController(_context, _configuration);
								SP_Parameter3.LOC_ID = Area.LOC_ID;
								SP_Parameter3.TUNGAY = dm_ThangLuong2.NGAYBATDAU;
								SP_Parameter3.DENNGAY = dm_ThangLuong2.NGAYKETTHUC;
								SP_Parameter3.ID_NHANVIEN = NhanVien.ID;
								if (await KPI3.PutProduct(SP_Parameter3) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse3 })
								{
									List<v_Tinh_KPI_KinhDoanh> lst_ChiTiet3 = ApiResponse3.Data as List<v_Tinh_KPI_KinhDoanh>;
									newnv_BangLuong_ChiTiet.SOTIEN = Math.Ceiling((itm.SOTIEN > 0.0) ? itm.SOTIEN : ((lst_ChiTiet3 != null && lst_ChiTiet3.Count > 0) ? lst_ChiTiet3[0].SOTIEN_KPI : 0.0));
								}
							}
							if (itm.TYPE_LUONG == 2)
							{
								newnv_BangLuong_ChiTiet.SOTIEN = itm.SOTIEN;
							}
						}
						dm_LoaiLuong dm_LoaiLuong2 = await _context.dm_LoaiLuong.FirstOrDefaultAsync((dm_LoaiLuong e) => e.LOC_ID == Area.LOC_ID && e.ID == itm.ID_LOAILUONG);
						newnv_BangLuong_ChiTiet.TYPE = ((dm_LoaiLuong2 != null) ? dm_LoaiLuong2.TYPE.ToString() : "0");
						Area.lstnv_BangLuong_ChiTiet.Add(newnv_BangLuong_ChiTiet);
					}
				}
				Area.TIENLUONG = Math.Ceiling(Area.lstnv_BangLuong_ChiTiet.Where((nv_BangLuong_ChiTiet s) => Convert.ToInt32(s.TYPE) > 0).Sum((nv_BangLuong_ChiTiet s) => s.SOTIEN));
				Area.TIENGIAM = Math.Ceiling(Area.lstnv_BangLuong_ChiTiet.Where((nv_BangLuong_ChiTiet s) => Convert.ToInt32(s.TYPE) < 0).Sum((nv_BangLuong_ChiTiet s) => s.SOTIEN));
				Area.TIENTHUCNHAN = Area.TIENLUONG - Area.TIENGIAM;
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = Area
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

		[HttpPost("{LOC_ID}/{ID}")]
		[Authorize(Roles = "User")]
		public async Task<ActionResult<dm_KhuVuc>> GetPhieuIn(string LOC_ID, string ID)
		{
			try
			{
				List<view_nv_BangLuong_ChiTiet> lstValue = await _context.view_nv_BangLuong_ChiTiet.Where((view_nv_BangLuong_ChiTiet e) => e.LOC_ID == LOC_ID && e.ID_BANGLUONG == ID).ToListAsync();
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
	}
}
