using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using DatabaseTHP;
using DatabaseTHP.Class;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using MyApiNetCore6.Data;

namespace MyApiNetCore6.Controllers
{

	[Route("api/[controller]")]
	[ApiController]
	public class HRLeaveController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public HRLeaveController(dbTrangHiepPhatContext context, IConfiguration configuration)
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
				List<nv_NghiPhep> lstValue = await (from e in _context.nv_NghiPhep
													where e.LOC_ID == LOC_ID
													orderby e.THOIGIANVAO
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
				List<nv_NghiPhep> lstValue = await (from e in _context.nv_NghiPhep.Where((nv_NghiPhep e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch)
													orderby e.THOIGIANVAO
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
				nv_NghiPhep Area = await _context.nv_NghiPhep.FirstOrDefaultAsync((nv_NghiPhep e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Area == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
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

		[HttpPut("{LOC_ID}/{ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> PutArea(string LOC_ID, string ID, nv_NghiPhep Area)
		{
			try
			{
				if (!AreaExistsID(LOC_ID, Area.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + Area.ID + " dữ liệu!",
						Data = ""
					});
				}
				nv_NghiPhep nv_NghiPhepOld = await _context.nv_NghiPhep.AsNoTracking().FirstOrDefaultAsync((nv_NghiPhep e) => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID);
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				if (nv_NghiPhepOld != null)
				{
					if (nv_NghiPhepOld.ID_PHEPNAM != Area.ID_PHEPNAM)
					{
						nv_PhepNam nv_PhepNamOld = await _context.nv_PhepNam.FirstOrDefaultAsync((nv_PhepNam e) => e.LOC_ID == Area.LOC_ID && e.ID == nv_NghiPhepOld.ID_PHEPNAM);
						if (nv_PhepNamOld == null)
						{
							return Ok(new ApiResponse
							{
								Success = false,
								Message = "Không tìm thấy " + nv_NghiPhepOld.LOC_ID + "-" + nv_NghiPhepOld.ID + " dữ liệu phép năm!",
								Data = ""
							});
						}
						nv_PhepNamOld.SONGAYPHEPDADUNG += nv_NghiPhepOld.SOLUONG;
						_context.Entry(nv_PhepNamOld).State = EntityState.Modified;
						nv_PhepNam nv_PhepNam2 = await _context.nv_PhepNam.FirstOrDefaultAsync((nv_PhepNam e) => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID_PHEPNAM);
						if (nv_PhepNam2 == null)
						{
							return Ok(new ApiResponse
							{
								Success = false,
								Message = "Không tìm thấy " + Area.LOC_ID + "-" + Area.ID_PHEPNAM + " dữ liệu phép năm!",
								Data = ""
							});
						}
						if (Area.ISNGHIPHEP && nv_PhepNam2.SONGAYPHEP - nv_PhepNam2.SONGAYPHEPDADUNG < Area.SOLUONG)
						{
							return Ok(new ApiResponse
							{
								Success = false,
								Message = "Số lượng ngày còn lại phép không đủ!" + (nv_PhepNam2.SONGAYPHEP - nv_PhepNam2.SONGAYPHEPDADUNG) + " ngày",
								Data = ""
							});
						}
						if (Area.ISNGHIPHEP && nv_PhepNam2.NGAYBATDAU >= Area.THOIGIANVAO && nv_PhepNam2.NGAYKETTHUC >= Area.THOIGIANRA)
						{
							return Ok(new ApiResponse
							{
								Success = false,
								Message = "Ngày bắt đầu và ngày kết thúc không nằm trong khoảng thời gian cho tạo phép!",
								Data = ""
							});
						}
						nv_PhepNam2.SONGAYPHEPDADUNG -= nv_NghiPhepOld.SOLUONG;
						_context.Entry(nv_PhepNam2).State = EntityState.Modified;
					}
					else
					{
						nv_PhepNam nv_PhepNam3 = await _context.nv_PhepNam.FirstOrDefaultAsync((nv_PhepNam e) => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID_PHEPNAM);
						if (nv_PhepNam3 == null)
						{
							return Ok(new ApiResponse
							{
								Success = false,
								Message = "Không tìm thấy " + Area.LOC_ID + "-" + Area.ID_PHEPNAM + " dữ liệu phép năm!",
								Data = ""
							});
						}
						nv_PhepNam3.SONGAYPHEPDADUNG -= nv_NghiPhepOld.SOLUONG;
						if (Area.ISNGHIPHEP && nv_PhepNam3.SONGAYPHEP - nv_PhepNam3.SONGAYPHEPDADUNG < Area.SOLUONG)
						{
							return Ok(new ApiResponse
							{
								Success = false,
								Message = "Số lượng ngày còn lại phép không đủ!" + (nv_PhepNam3.SONGAYPHEP - nv_PhepNam3.SONGAYPHEPDADUNG) + " ngày",
								Data = ""
							});
						}
						if (Area.ISNGHIPHEP && nv_PhepNam3.NGAYBATDAU >= Area.THOIGIANVAO && nv_PhepNam3.NGAYKETTHUC >= Area.THOIGIANRA)
						{
							return Ok(new ApiResponse
							{
								Success = false,
								Message = "Ngày bắt đầu và ngày kết thúc không nằm trong khoảng thời gian cho tạo phép!",
								Data = ""
							});
						}
						nv_PhepNam3.SONGAYPHEPDADUNG += Area.SOLUONG;
						_context.Entry(nv_PhepNam3).State = EntityState.Modified;
					}
				}
				_context.Entry(Area).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
				nv_NghiPhep OKArea = await _context.nv_NghiPhep.FirstOrDefaultAsync((nv_NghiPhep e) => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKArea
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
		public async Task<ActionResult<nv_NghiPhep>> PostArea(nv_NghiPhep Area)
		{
			try
			{
				if (AreaExistsID(Area.LOC_ID, Area.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại " + Area.LOC_ID + "-" + Area.ID + " trong dữ liệu!",
						Data = ""
					});
				}
				int DayInterval = 0;
				DateTime StartDate = Area.THOIGIANVAO;
				for (; Area.THOIGIANVAO.AddDays(DayInterval) <= Area.THOIGIANRA; DayInterval++)
				{
					StartDate = StartDate.AddDays(DayInterval);
					nv_NghiPhep nv_NghiPhep2 = await _context.nv_NghiPhep.FirstOrDefaultAsync((nv_NghiPhep e) => e.LOC_ID == Area.LOC_ID && e.THOIGIANVAO.Date >= ((DateTime)StartDate).Date && e.THOIGIANRA.Date <= ((DateTime)StartDate).Date && e.ID_NHANVIEN == Area.ID_NHANVIEN);
					if (nv_NghiPhep2 != null)
					{
						return Ok(new ApiResponse
						{
							Success = false,
							Message = "Đã tồn tại " + nv_NghiPhep2.LOC_ID + "-" + nv_NghiPhep2.ID + " trong dữ liệu!(Đã có đơn nghĩ phép ngày " + StartDate.ToString("dd/MM/yyyy") + " )",
							Data = ""
						});
					}
				}
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				nv_PhepNam nv_PhepNam2 = await _context.nv_PhepNam.FirstOrDefaultAsync((nv_PhepNam e) => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID_PHEPNAM);
				if (nv_PhepNam2 != null)
				{
					if (Area.ISNGHIPHEP && nv_PhepNam2.SONGAYPHEP - nv_PhepNam2.SONGAYPHEPDADUNG < Area.SOLUONG)
					{
						return Ok(new ApiResponse
						{
							Success = false,
							Message = "Số lượng ngày còn lại phép không đủ!" + (nv_PhepNam2.SONGAYPHEP - nv_PhepNam2.SONGAYPHEPDADUNG) + " ngày",
							Data = ""
						});
					}
					if (Area.ISNGHIPHEP && nv_PhepNam2.NGAYBATDAU <= Area.THOIGIANVAO && nv_PhepNam2.NGAYKETTHUC <= Area.THOIGIANRA)
					{
						return Ok(new ApiResponse
						{
							Success = false,
							Message = "Ngày bắt đầu và ngày kết thúc không nằm trong khoảng thời gian cho tạo phép!",
							Data = ""
						});
					}
					if (Area.ISNGHIPHEP)
					{
						nv_PhepNam2.SONGAYPHEPDADUNG += Area.SOLUONG;
					}
					_context.Entry(nv_PhepNam2).State = EntityState.Modified;
					_context.nv_NghiPhep.Add(Area);
					AuditLogController auditLog = new AuditLogController(_context, _configuration);
					auditLog.InserAuditLog();
					await _context.SaveChangesAsync();
					transaction.Commit();
					nv_NghiPhep OKArea = await _context.nv_NghiPhep.FirstOrDefaultAsync((nv_NghiPhep e) => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID);
					return Ok(new ApiResponse
					{
						Success = true,
						Message = "Success",
						Data = OKArea
					});
				}
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Không tìm thấy " + Area.LOC_ID + "-" + Area.ID + " dữ liệu phép năm!",
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
		public async Task<IActionResult> DeleteArea(string LOC_ID, string ID)
		{
			try
			{
				nv_NghiPhep Area = await _context.nv_NghiPhep.FirstOrDefaultAsync((nv_NghiPhep e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Area == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				nv_PhepNam nv_PhepNam2 = await _context.nv_PhepNam.FirstOrDefaultAsync((nv_PhepNam e) => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID_PHEPNAM);
				if (nv_PhepNam2 != null)
				{
					if (Area.ISNGHIPHEP)
					{
						nv_PhepNam2.SONGAYPHEPDADUNG -= Area.SOLUONG;
					}
					_context.Entry(nv_PhepNam2).State = EntityState.Modified;
					_context.nv_NghiPhep.Remove(Area);
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
					Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu phép năm!",
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

		private bool AreaExistsID(string LOC_ID, string ID)
		{
			return _context.nv_NghiPhep.Any((nv_NghiPhep e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}
	}
}
