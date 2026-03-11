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
	public class UserController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public UserController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetUser()
		{
			try
			{
				List<view_AspNetUsers> lstValue = await _context.view_AspNetUsers.ToListAsync();
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
		public async Task<IActionResult> GetUser(int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<view_AspNetUsers> lstValue = await (from e in _context.view_AspNetUsers.Where(KeyWhere, ValuesSearch)
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

		[HttpGet("{ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetUser(string ID)
		{
			try
			{
				view_AspNetUsers User = await _context.view_AspNetUsers.FirstOrDefaultAsync((view_AspNetUsers e) => e.ID == ID);
				if (User == null)
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
					Data = User
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

		[HttpPut("{ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> PutUser(string ID, AspNetUsers User)
		{
			try
			{
				if (ID != User.ID)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (!UserExists(User.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.Entry(User).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				view_AspNetUsers OKUser = await _context.view_AspNetUsers.FirstOrDefaultAsync((view_AspNetUsers e) => e.ID == User.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKUser
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
		public async Task<ActionResult<AspNetUsers>> PostUser(AspNetUsers User)
		{
			try
			{
				if (UserExists(User.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + User.ID + " trong dữ liệu!",
						Data = ""
					});
				}
				_context.AspNetUsers.Add(User);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				view_AspNetUsers OKUser = await _context.view_AspNetUsers.FirstOrDefaultAsync((view_AspNetUsers e) => e.ID == User.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKUser
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
		public async Task<IActionResult> DeleteUser(string ID)
		{
			try
			{
				AspNetUsers User = await _context.AspNetUsers.FirstOrDefaultAsync((AspNetUsers e) => e.ID == ID);
				if (User == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + ID + " dữ liệu!",
						Data = ""
					});
				}
				ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
				ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete(User, User.ID, User.UserName);
				if (!apiResponse.Success)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = apiResponse.Message,
						Data = ""
					});
				}
				_context.AspNetUsers.Remove(User);
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

		private bool UserExists(string ID)
		{
			return _context.AspNetUsers.Any((AspNetUsers e) => e.ID == ID);
		}
	}
}
