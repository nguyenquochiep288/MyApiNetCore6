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
	public class TaxController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public TaxController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetTax(string LOC_ID)
		{
			try
			{
				List<dm_ThueSuat> lstValue = await (from e in _context.dm_ThueSuat
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
		public async Task<IActionResult> GetTax(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<dm_ThueSuat> lstValue = await (from e in _context.dm_ThueSuat.Where((dm_ThueSuat e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch)
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
		public async Task<IActionResult> GetTax(string LOC_ID, string ID)
		{
			try
			{
				dm_ThueSuat Tax = await _context.dm_ThueSuat.FirstOrDefaultAsync((dm_ThueSuat e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Tax == null)
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
					Data = Tax
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
		public async Task<IActionResult> PutTax(string LOC_ID, string MA, dm_ThueSuat Tax)
		{
			try
			{
				if (TaxExists(Tax))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Tax.LOC_ID + "-" + Tax.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				if (LOC_ID != Tax.LOC_ID || Tax.MA != MA)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (!TaxExistsID(LOC_ID, Tax.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + Tax.ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.Entry(Tax).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				dm_TienTe OKTax = await _context.dm_TienTe.FirstOrDefaultAsync((dm_TienTe e) => e.LOC_ID == Tax.LOC_ID && e.ID == Tax.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKTax
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
		public async Task<ActionResult<dm_ThueSuat>> PostTax(dm_ThueSuat Tax)
		{
			try
			{
				if (TaxExistsMA(Tax.LOC_ID, Tax.MA))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Tax.LOC_ID + "-" + Tax.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				_context.dm_ThueSuat.Add(Tax);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				dm_TienTe OKTax = await _context.dm_TienTe.FirstOrDefaultAsync((dm_TienTe e) => e.LOC_ID == Tax.LOC_ID && e.ID == Tax.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKTax
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
		public async Task<IActionResult> DeleteTax(string LOC_ID, string ID)
		{
			try
			{
				dm_ThueSuat Tax = await _context.dm_ThueSuat.FirstOrDefaultAsync((dm_ThueSuat e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Tax == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
				ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete(Tax, Tax.ID, Tax.NAME);
				if (!apiResponse.Success)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = apiResponse.Message,
						Data = ""
					});
				}
				_context.dm_ThueSuat.Remove(Tax);
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

		private bool TaxExistsMA(string LOC_ID, string MA)
		{
			return _context.dm_ThueSuat.Any((dm_ThueSuat e) => e.LOC_ID == LOC_ID && e.MA == MA);
		}

		private bool TaxExistsID(string LOC_ID, string ID)
		{
			return _context.dm_ThueSuat.Any((dm_ThueSuat e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}

		private bool TaxExists(dm_ThueSuat Position)
		{
			return _context.dm_ThueSuat.Any((dm_ThueSuat e) => e.LOC_ID == Position.LOC_ID && e.MA == Position.MA && e.ID != Position.ID);
		}
	}
}
