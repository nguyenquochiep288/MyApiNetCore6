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
	public class ProviderController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public ProviderController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetProvider(string LOC_ID)
		{
			try
			{
				List<view_dm_NhaCungCap> lstValue = await (from e in _context.view_dm_NhaCungCap
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
		public async Task<IActionResult> GetProvider(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<view_dm_NhaCungCap> lstValue = await (from e in _context.view_dm_NhaCungCap.Where((view_dm_NhaCungCap e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch)
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
		public async Task<IActionResult> GetProvider(string LOC_ID, string ID)
		{
			try
			{
				view_dm_NhaCungCap Provider = await _context.view_dm_NhaCungCap.FirstOrDefaultAsync((view_dm_NhaCungCap e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Provider == null)
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
					Data = Provider
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
		public async Task<IActionResult> PutProvider(string LOC_ID, string MA, dm_NhaCungCap Provider)
		{
			try
			{
				if (ProviderExists(Provider))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Provider.LOC_ID + "-" + Provider.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				if (LOC_ID != Provider.LOC_ID || Provider.MA != MA)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (!ProviderExistsID(LOC_ID, Provider.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + Provider.ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.Entry(Provider).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				view_dm_NhaCungCap OKProvider = await _context.view_dm_NhaCungCap.FirstOrDefaultAsync((view_dm_NhaCungCap e) => e.LOC_ID == Provider.LOC_ID && e.ID == Provider.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKProvider
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
		public async Task<ActionResult<dm_NhaCungCap>> PostProvider(dm_NhaCungCap Provider)
		{
			try
			{
				if (ProviderExistsMA(Provider.LOC_ID, Provider.MA))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Provider.LOC_ID + "-" + Provider.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				_context.dm_NhaCungCap.Add(Provider);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				view_dm_NhaCungCap OKProvider = await _context.view_dm_NhaCungCap.FirstOrDefaultAsync((view_dm_NhaCungCap e) => e.LOC_ID == Provider.LOC_ID && e.ID == Provider.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKProvider
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
		public async Task<IActionResult> DeleteProvider(string LOC_ID, string ID)
		{
			try
			{
				dm_NhaCungCap Provider = await _context.dm_NhaCungCap.FirstOrDefaultAsync((dm_NhaCungCap e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Provider == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
				ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete(Provider, Provider.ID, Provider.NAME);
				if (!apiResponse.Success)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = apiResponse.Message,
						Data = ""
					});
				}
				_context.dm_NhaCungCap.Remove(Provider);
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

		private bool ProviderExistsMA(string LOC_ID, string MA)
		{
			return _context.dm_NhaCungCap.Any((dm_NhaCungCap e) => e.LOC_ID == LOC_ID && e.MA == MA);
		}

		private bool ProviderExistsID(string LOC_ID, string ID)
		{
			return _context.dm_NhaCungCap.Any((dm_NhaCungCap e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}

		private bool ProviderExists(dm_NhaCungCap Position)
		{
			return _context.dm_NhaCungCap.Any((dm_NhaCungCap e) => e.LOC_ID == Position.LOC_ID && e.MA == Position.MA && e.ID != Position.ID);
		}
	}
}
