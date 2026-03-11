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
	public class LocationController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public LocationController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetCar(string LOC_ID)
		{
			try
			{
				List<dm_DiaDiemChamCong> lstValue = await (from e in _context.dm_DiaDiemChamCong
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
		public async Task<IActionResult> GetCar(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<dm_DiaDiemChamCong> lstValue = await (from e in _context.dm_DiaDiemChamCong.Where((dm_DiaDiemChamCong e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch)
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
		public async Task<IActionResult> GetCar(string LOC_ID, string ID)
		{
			try
			{
				dm_DiaDiemChamCong Car = await _context.dm_DiaDiemChamCong.FirstOrDefaultAsync((dm_DiaDiemChamCong e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Car == null)
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
					Data = Car
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
		public async Task<IActionResult> PutCar(string LOC_ID, string MA, dm_DiaDiemChamCong Car)
		{
			try
			{
				if (LOC_ID != Car.LOC_ID || Car.MA != MA)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (CarExists(Car))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Car.LOC_ID + "-" + Car.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				if (!CarExistsID(LOC_ID, Car.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + Car.ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.Entry(Car).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				dm_DiaDiemChamCong OKCar = await _context.dm_DiaDiemChamCong.FirstOrDefaultAsync((dm_DiaDiemChamCong e) => e.LOC_ID == Car.LOC_ID && e.ID == Car.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKCar
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
		public async Task<ActionResult<dm_DiaDiemChamCong>> PostCar(dm_DiaDiemChamCong Car)
		{
			try
			{
				if (CarExistsMA(Car.LOC_ID, Car.MA))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Car.LOC_ID + "-" + Car.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				_context.dm_DiaDiemChamCong.Add(Car);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				dm_DiaDiemChamCong OKCar = await _context.dm_DiaDiemChamCong.FirstOrDefaultAsync((dm_DiaDiemChamCong e) => e.LOC_ID == Car.LOC_ID && e.ID == Car.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKCar
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
		public async Task<IActionResult> DeleteCar(string LOC_ID, string ID)
		{
			try
			{
				dm_DiaDiemChamCong Car = await _context.dm_DiaDiemChamCong.FirstOrDefaultAsync((dm_DiaDiemChamCong e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Car == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
				ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete(Car, Car.ID, Car.MA);
				if (!apiResponse.Success)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = apiResponse.Message,
						Data = ""
					});
				}
				List<web_PhanQuyenKhuVuc> lstweb_PhanQuyenSanPham = await _context.web_PhanQuyenKhuVuc.Where((web_PhanQuyenKhuVuc e) => e.LOC_ID == LOC_ID && e.ID_KHUVUC == ID).ToListAsync();
				if (lstweb_PhanQuyenSanPham != null)
				{
					foreach (web_PhanQuyenKhuVuc itm in lstweb_PhanQuyenSanPham)
					{
						_context.web_PhanQuyenKhuVuc.Remove(itm);
					}
				}
				_context.dm_DiaDiemChamCong.Remove(Car);
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

		private bool CarExistsMA(string LOC_ID, string MA)
		{
			return _context.dm_DiaDiemChamCong.Any((dm_DiaDiemChamCong e) => e.LOC_ID == LOC_ID && e.MA == MA);
		}

		private bool CarExistsID(string LOC_ID, string ID)
		{
			return _context.dm_DiaDiemChamCong.Any((dm_DiaDiemChamCong e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}

		private bool CarExists(dm_DiaDiemChamCong Car)
		{
			return _context.dm_DiaDiemChamCong.Any((dm_DiaDiemChamCong e) => e.LOC_ID == Car.LOC_ID && e.MA == Car.MA && e.ID != Car.ID);
		}
	}
}
