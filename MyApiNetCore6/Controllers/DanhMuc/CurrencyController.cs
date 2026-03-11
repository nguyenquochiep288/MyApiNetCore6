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
	public class CurrencyController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public CurrencyController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetCurrency(string LOC_ID)
		{
			try
			{
				List<dm_TienTe> lstValue = await (from e in _context.dm_TienTe
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
		public async Task<IActionResult> GetCurrency(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<dm_TienTe> lstValue = await (from e in _context.dm_TienTe.Where((dm_TienTe e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch)
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
		public async Task<IActionResult> GetCurrency(string LOC_ID, string ID)
		{
			try
			{
				dm_TienTe Currency = await _context.dm_TienTe.FirstOrDefaultAsync((dm_TienTe e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Currency == null)
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
					Data = Currency
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
		public async Task<IActionResult> PutCurrency(string LOC_ID, string MA, dm_TienTe Currency)
		{
			try
			{
				if (LOC_ID != Currency.LOC_ID || Currency.MA != MA)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (CurrencyExists(Currency))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Currency.LOC_ID + "-" + Currency.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				if (!CurrencyExistsID(LOC_ID, Currency.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + Currency.ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.Entry(Currency).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				dm_TienTe OKCurrency = await _context.dm_TienTe.FirstOrDefaultAsync((dm_TienTe e) => e.LOC_ID == Currency.LOC_ID && e.ID == Currency.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKCurrency
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
		public async Task<ActionResult<dm_TienTe>> PostCurrency(dm_TienTe Currency)
		{
			try
			{
				if (CurrencyExistsMA(Currency.LOC_ID, Currency.MA))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Currency.LOC_ID + "-" + Currency.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				_context.dm_TienTe.Add(Currency);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				dm_TienTe OKCurrency = await _context.dm_TienTe.FirstOrDefaultAsync((dm_TienTe e) => e.LOC_ID == Currency.LOC_ID && e.ID == Currency.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKCurrency
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
		public async Task<IActionResult> DeleteCurrency(string LOC_ID, string ID)
		{
			try
			{
				dm_TienTe Currency = await _context.dm_TienTe.FirstOrDefaultAsync((dm_TienTe e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Currency == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
				ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete(Currency, Currency.ID, Currency.MA);
				if (!apiResponse.Success)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = apiResponse.Message,
						Data = ""
					});
				}
				_context.dm_TienTe.Remove(Currency);
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

		private bool CurrencyExistsMA(string LOC_ID, string MA)
		{
			return _context.dm_TienTe.Any((dm_TienTe e) => e.LOC_ID == LOC_ID && e.MA == MA);
		}

		private bool CurrencyExistsID(string LOC_ID, string ID)
		{
			return _context.dm_TienTe.Any((dm_TienTe e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}

		private bool CurrencyExists(dm_TienTe Currency)
		{
			return _context.dm_TienTe.Any((dm_TienTe e) => e.LOC_ID == Currency.LOC_ID && e.MA == Currency.MA && e.ID != Currency.ID);
		}
	}
}
