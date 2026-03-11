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
	public class CompanyController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public CompanyController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetCompany()
		{
			try
			{
				List<dm_CongTy> lstValue = await _context.dm_CongTy.ToListAsync();
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

		[HttpGet("{Type}/{KeyWhere}/{ValuesSearch}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetCompany(int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<dm_CongTy> lstValue = await _context.dm_CongTy.Where(KeyWhere, ValuesSearch).ToListAsync();
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

		[HttpGet("{ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetCompany(string ID)
		{
			try
			{
				dm_CongTy Company = await _context.dm_CongTy.FirstOrDefaultAsync((dm_CongTy e) => e.ID == ID);
				if (Company == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + ID + " dữ liệu!",
						Data = ""
					});
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = Company
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

		[HttpPut("{MA}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> PutCompany(string MA, dm_CongTy Company)
		{
			try
			{
				if (MA != Company.MA)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (!CompanyExistsID(Company.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + Company.ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.Entry(Company).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				dm_CongTy OKCompany = await _context.dm_CongTy.FirstOrDefaultAsync((dm_CongTy e) => e.ID == Company.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKCompany
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
		public async Task<ActionResult<dm_CongTy>> PostCompany(dm_CongTy Company)
		{
			try
			{
				if (CompanyExistsMA(Company.MA))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Company.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				_context.dm_CongTy.Add(Company);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				dm_CongTy OKCompany = await _context.dm_CongTy.FirstOrDefaultAsync((dm_CongTy e) => e.ID == Company.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKCompany
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

		[HttpDelete("{ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> DeleteCompany(string ID)
		{
			try
			{
				dm_CongTy Company = await _context.dm_CongTy.FirstOrDefaultAsync((dm_CongTy e) => e.ID == ID);
				if (Company == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + ID + " dữ liệu!",
						Data = ""
					});
				}
				ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
				ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete(Company, Company.ID, Company.MA);
				if (!apiResponse.Success)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = apiResponse.Message,
						Data = ""
					});
				}
				_context.dm_CongTy.Remove(Company);
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

		private bool CompanyExistsID(string ID)
		{
			return _context.dm_CongTy.Any((dm_CongTy e) => e.ID == ID);
		}

		private bool CompanyExistsMA(string MA)
		{
			return _context.dm_CongTy.Any((dm_CongTy e) => e.MA == MA);
		}

		private bool CompanyExists(dm_CongTy Company)
		{
			return _context.dm_CongTy.Any((dm_CongTy e) => e.MA == Company.MA && e.ID != Company.ID);
		}
	}
}
