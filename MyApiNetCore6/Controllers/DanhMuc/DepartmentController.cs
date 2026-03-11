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
	public class DepartmentController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public DepartmentController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetDepartment(string LOC_ID)
		{
			try
			{
				List<dm_PhongBan> lstValue = await (from e in _context.dm_PhongBan
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
		public async Task<IActionResult> GetDepartment(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<dm_PhongBan> lstValue = await (from e in _context.dm_PhongBan.Where((dm_PhongBan e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch)
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
		public async Task<IActionResult> GetDepartment(string LOC_ID, string ID)
		{
			try
			{
				dm_PhongBan Department = await _context.dm_PhongBan.FirstOrDefaultAsync((dm_PhongBan e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Department == null)
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
					Data = Department
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
		public async Task<IActionResult> PutDepartment(string LOC_ID, string MA, dm_PhongBan Department)
		{
			try
			{
				if (LOC_ID != Department.LOC_ID || Department.MA != MA)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (!DepartmentExistsID(LOC_ID, Department.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + Department.ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.Entry(Department).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				dm_TienTe OKDepartment = await _context.dm_TienTe.FirstOrDefaultAsync((dm_TienTe e) => e.LOC_ID == Department.LOC_ID && e.ID == Department.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKDepartment
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
		public async Task<ActionResult<dm_PhongBan>> PostDepartment(dm_PhongBan Department)
		{
			try
			{
				if (DepartmentExistsMA(Department.LOC_ID, Department.MA))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Department.LOC_ID + "-" + Department.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				_context.dm_PhongBan.Add(Department);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				dm_TienTe OKDepartment = await _context.dm_TienTe.FirstOrDefaultAsync((dm_TienTe e) => e.LOC_ID == Department.LOC_ID && e.ID == Department.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKDepartment
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
		public async Task<IActionResult> DeleteDepartment(string LOC_ID, string ID)
		{
			try
			{
				dm_PhongBan Department = await _context.dm_PhongBan.FirstOrDefaultAsync((dm_PhongBan e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Department == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
				ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete(Department, Department.ID, Department.MA);
				if (!apiResponse.Success)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = apiResponse.Message,
						Data = ""
					});
				}
				_context.dm_PhongBan.Remove(Department);
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

		private bool DepartmentExistsMA(string LOC_ID, string MA)
		{
			return _context.dm_PhongBan.Any((dm_PhongBan e) => e.LOC_ID == LOC_ID && e.MA == MA);
		}

		private bool DepartmentExistsID(string LOC_ID, string ID)
		{
			return _context.dm_PhongBan.Any((dm_PhongBan e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}

		private bool DepartmentExists(dm_PhongBan Department)
		{
			return _context.dm_PhongBan.Any((dm_PhongBan e) => e.LOC_ID == Department.LOC_ID && e.MA == Department.MA && e.ID != Department.ID);
		}
	}
}
