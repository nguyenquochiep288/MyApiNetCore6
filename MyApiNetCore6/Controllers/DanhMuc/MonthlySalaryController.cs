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
	public class MonthlySalaryController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public MonthlySalaryController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetMonthlySalary(string LOC_ID)
		{
			try
			{
				List<dm_ThangLuong> lstValue = await (from e in _context.dm_ThangLuong
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
		public async Task<IActionResult> GetMonthlySalary(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<dm_ThangLuong> lstValue = await (from e in _context.dm_ThangLuong.Where((dm_ThangLuong e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch)
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
		public async Task<IActionResult> GetMonthlySalary(string LOC_ID, string ID)
		{
			try
			{
				dm_ThangLuong MonthlySalary = await _context.dm_ThangLuong.FirstOrDefaultAsync((dm_ThangLuong e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (MonthlySalary == null)
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
					Data = MonthlySalary
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
		public async Task<IActionResult> PutMonthlySalary(string LOC_ID, string MA, dm_ThangLuong MonthlySalary)
		{
			try
			{
				if (LOC_ID != MonthlySalary.LOC_ID || MonthlySalary.MA != MA)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (MonthlySalaryExists(MonthlySalary))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + MonthlySalary.LOC_ID + "-" + MonthlySalary.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				if (!MonthlySalaryExistsID(LOC_ID, MonthlySalary.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + MonthlySalary.ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.Entry(MonthlySalary).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				dm_ThangLuong OKMonthlySalary = await _context.dm_ThangLuong.FirstOrDefaultAsync((dm_ThangLuong e) => e.LOC_ID == MonthlySalary.LOC_ID && e.ID == MonthlySalary.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKMonthlySalary
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
		public async Task<ActionResult<dm_ThangLuong>> PostMonthlySalary(dm_ThangLuong MonthlySalary)
		{
			try
			{
				if (MonthlySalaryExistsMA(MonthlySalary.LOC_ID, MonthlySalary.MA))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + MonthlySalary.LOC_ID + "-" + MonthlySalary.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				_context.dm_ThangLuong.Add(MonthlySalary);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				dm_ThangLuong OKMonthlySalary = await _context.dm_ThangLuong.FirstOrDefaultAsync((dm_ThangLuong e) => e.LOC_ID == MonthlySalary.LOC_ID && e.ID == MonthlySalary.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKMonthlySalary
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
		public async Task<IActionResult> DeleteMonthlySalary(string LOC_ID, string ID)
		{
			try
			{
				dm_ThangLuong MonthlySalary = await _context.dm_ThangLuong.FirstOrDefaultAsync((dm_ThangLuong e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (MonthlySalary == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
				ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete(MonthlySalary, MonthlySalary.ID, MonthlySalary.MA);
				if (!apiResponse.Success)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = apiResponse.Message,
						Data = ""
					});
				}
				_context.dm_ThangLuong.Remove(MonthlySalary);
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

		private bool MonthlySalaryExistsMA(string LOC_ID, string MA)
		{
			return _context.dm_ThangLuong.Any((dm_ThangLuong e) => e.LOC_ID == LOC_ID && e.MA == MA);
		}

		private bool MonthlySalaryExistsID(string LOC_ID, string ID)
		{
			return _context.dm_ThangLuong.Any((dm_ThangLuong e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}

		private bool MonthlySalaryExists(dm_ThangLuong MonthlySalary)
		{
			return _context.dm_ThangLuong.Any((dm_ThangLuong e) => e.LOC_ID == MonthlySalary.LOC_ID && e.MA == MonthlySalary.MA && e.ID != MonthlySalary.ID);
		}
	}
}
