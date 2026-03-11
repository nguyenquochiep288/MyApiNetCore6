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
using Microsoft.Extensions.Configuration;
using MyApiNetCore6.Data;

namespace MyApiNetCore6.Controllers
{

	[Route("api/[controller]")]
	[ApiController]
	public class TimekeepingController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public TimekeepingController(dbTrangHiepPhatContext context, IConfiguration configuration)
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
				List<nv_ChamCong> lstValue = await (from e in _context.nv_ChamCong
													where e.LOC_ID == LOC_ID
													orderby e.NGAYCONG
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
				List<nv_ChamCong> lstValue = await (from e in _context.nv_ChamCong.Where((nv_ChamCong e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch)
													orderby e.NGAYCONG
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
				nv_ChamCong Area = await _context.nv_ChamCong.FirstOrDefaultAsync((nv_ChamCong e) => e.LOC_ID == LOC_ID && e.ID == ID);
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
		public async Task<IActionResult> PutArea(string LOC_ID, string ID, nv_ChamCong Area)
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
				_context.Entry(Area).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				nv_ChamCong OKArea = await _context.nv_ChamCong.FirstOrDefaultAsync((nv_ChamCong e) => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID);
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

		[HttpPost("PostCheckIn")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> PostCheckIn(nv_ChamCong Area)
		{
			try
			{
				nv_ChamCong nv_ChamCong2 = await _context.nv_ChamCong.FirstOrDefaultAsync((nv_ChamCong e) => e.LOC_ID == Area.LOC_ID && e.NGAYCONG.Date == Area.NGAYCONG.Date && e.ID_NHANVIEN == Area.ID_NHANVIEN);
				if (nv_ChamCong2 != null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại " + nv_ChamCong2.LOC_ID + "-" + nv_ChamCong2.ID + " trong dữ liệu!(Đã có dữ liệu chấm công ngày " + Area.NGAYCONG.ToString("dd/MM/yyyy") + " )",
						Data = ""
					});
				}
				_context.nv_ChamCong.Add(Area);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				nv_ChamCong OKArea = await _context.nv_ChamCong.FirstOrDefaultAsync((nv_ChamCong e) => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID);
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

		[HttpPost("PostCheckOut")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> PostCheckOut(nv_ChamCong Area)
		{
			try
			{
				if (!AreaExistsID(Area.LOC_ID, Area.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + Area.LOC_ID + "-" + Area.ID + " dữ liệu!",
						Data = ""
					});
				}
				nv_ChamCong nv_ChamCong2 = await _context.nv_ChamCong.FirstOrDefaultAsync((nv_ChamCong e) => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID);
				if (nv_ChamCong2 != null)
				{
					if (nv_ChamCong2.ID_NHANVIEN != Area.ID_NHANVIEN)
					{
						return Ok(new ApiResponse
						{
							Success = false,
							Message = "Dữ liệu chấm công khác nhau!",
							Data = ""
						});
					}
					nv_ChamCong2.ID_NGUOISUA = Area.ID_NGUOISUA;
					nv_ChamCong2.THOIGIANSUA = Area.THOIGIANSUA;
					nv_ChamCong2.THOIGIANRA = Area.THOIGIANRA;
					nv_ChamCong2.IP_CHAMCONGRA = Area.IP_CHAMCONGRA;
					string GHICHU = nv_ChamCong2.GHICHU + Area.GHICHU;
					if (GHICHU.Length > 250)
					{
						nv_ChamCong2.GHICHU = GHICHU.Substring(0, 249);
					}
					else
					{
						nv_ChamCong2.GHICHU = GHICHU;
					}
				}
				_context.Entry(nv_ChamCong2).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				nv_ChamCong OKArea = await _context.nv_ChamCong.FirstOrDefaultAsync((nv_ChamCong e) => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID);
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
		public async Task<ActionResult<nv_ChamCong>> PostArea(nv_ChamCong Area)
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
				nv_ChamCong nv_ChamCong2 = await _context.nv_ChamCong.FirstOrDefaultAsync((nv_ChamCong e) => e.LOC_ID == Area.LOC_ID && e.NGAYCONG.Date == Area.NGAYCONG.Date && e.ID_NHANVIEN == Area.ID_NHANVIEN);
				if (nv_ChamCong2 != null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại " + nv_ChamCong2.LOC_ID + "-" + nv_ChamCong2.ID + " trong dữ liệu!(Đã có dữ liệu chấm công ngày " + Area.NGAYCONG.ToString("dd/MM/yyyy") + " )",
						Data = ""
					});
				}
				_context.nv_ChamCong.Add(Area);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				nv_ChamCong OKArea = await _context.nv_ChamCong.FirstOrDefaultAsync((nv_ChamCong e) => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKArea
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
				nv_ChamCong Area = await _context.nv_ChamCong.FirstOrDefaultAsync((nv_ChamCong e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Area == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.nv_ChamCong.Remove(Area);
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

		private bool AreaExistsID(string LOC_ID, string ID)
		{
			return _context.nv_ChamCong.Any((nv_ChamCong e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}
	}
}
