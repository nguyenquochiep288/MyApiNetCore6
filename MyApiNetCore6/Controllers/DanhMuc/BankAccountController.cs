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
	public class BankAccountController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public BankAccountController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetBankAccount(string LOC_ID)
		{
			try
			{
				List<dm_TaiKhoanNganHang> lstValue = await (from e in _context.dm_TaiKhoanNganHang
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
		public async Task<IActionResult> GetBankAccount(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<dm_TaiKhoanNganHang> lstValue = await (from e in _context.dm_TaiKhoanNganHang.Where((dm_TaiKhoanNganHang e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch)
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
		public async Task<IActionResult> GetBankAccount(string LOC_ID, string ID)
		{
			try
			{
				dm_TaiKhoanNganHang BankAccount = await _context.dm_TaiKhoanNganHang.FirstOrDefaultAsync((dm_TaiKhoanNganHang e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (BankAccount == null)
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
					Data = BankAccount
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
		public async Task<IActionResult> PutBankAccount(string LOC_ID, string MA, dm_TaiKhoanNganHang BankAccount)
		{
			try
			{
				if (LOC_ID != BankAccount.LOC_ID || BankAccount.MA != MA)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (BankAccountExists(BankAccount))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + BankAccount.LOC_ID + "-" + BankAccount.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				if (!BankAccountExistsID(LOC_ID, BankAccount.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + BankAccount.ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.Entry(BankAccount).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				dm_TaiKhoanNganHang OKBankAccount = await _context.dm_TaiKhoanNganHang.FirstOrDefaultAsync((dm_TaiKhoanNganHang e) => e.LOC_ID == BankAccount.LOC_ID && e.ID == BankAccount.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKBankAccount
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
		public async Task<ActionResult<dm_TaiKhoanNganHang>> PostBankAccount(dm_TaiKhoanNganHang BankAccount)
		{
			try
			{
				if (BankAccountExistsMA(BankAccount.LOC_ID, BankAccount.MA))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + BankAccount.LOC_ID + "-" + BankAccount.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				_context.dm_TaiKhoanNganHang.Add(BankAccount);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				dm_TaiKhoanNganHang OKBankAccount = await _context.dm_TaiKhoanNganHang.FirstOrDefaultAsync((dm_TaiKhoanNganHang e) => e.LOC_ID == BankAccount.LOC_ID && e.ID == BankAccount.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKBankAccount
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
		public async Task<IActionResult> DeleteBankAccount(string LOC_ID, string ID)
		{
			try
			{
				dm_TaiKhoanNganHang BankAccount = await _context.dm_TaiKhoanNganHang.FirstOrDefaultAsync((dm_TaiKhoanNganHang e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (BankAccount == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
				ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete(BankAccount, BankAccount.ID, BankAccount.MA);
				if (!apiResponse.Success)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = apiResponse.Message,
						Data = ""
					});
				}
				_context.dm_TaiKhoanNganHang.Remove(BankAccount);
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

		private bool BankAccountExistsMA(string LOC_ID, string MA)
		{
			return _context.dm_TaiKhoanNganHang.Any((dm_TaiKhoanNganHang e) => e.LOC_ID == LOC_ID && e.MA == MA);
		}

		private bool BankAccountExistsID(string LOC_ID, string ID)
		{
			return _context.dm_TaiKhoanNganHang.Any((dm_TaiKhoanNganHang e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}

		private bool BankAccountExists(dm_TaiKhoanNganHang BankAccount)
		{
			return _context.dm_TaiKhoanNganHang.Any((dm_TaiKhoanNganHang e) => e.LOC_ID == BankAccount.LOC_ID && e.MA == BankAccount.MA && e.ID != BankAccount.ID);
		}
	}
}
