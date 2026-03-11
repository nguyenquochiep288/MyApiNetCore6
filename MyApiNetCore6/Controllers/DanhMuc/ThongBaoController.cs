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
	public class ThongBaoController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public ThongBaoController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet]
		public async Task<IActionResult> GetThongBao()
		{
			try
			{
				List<web_ThongBao> lstValue = await _context.web_ThongBao.ToListAsync();
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
		public async Task<IActionResult> GetThongBao(int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<web_ThongBao> lstValue = await _context.web_ThongBao.Where(KeyWhere, ValuesSearch).ToListAsync();
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
		public async Task<IActionResult> GetThongBao(string ID)
		{
			try
			{
				web_ThongBao ThongBao = await _context.web_ThongBao.FirstOrDefaultAsync((web_ThongBao e) => e.ID == ID);
				if (ThongBao == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy -" + ID + " dữ liệu!",
						Data = ""
					});
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ThongBao
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
		public async Task<IActionResult> PutThongBao(string ID, web_ThongBao ThongBao)
		{
			try
			{
				if (ThongBao.ID != ID)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (!ThongBaoExistsID(ThongBao.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + ThongBao.ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.Entry(ThongBao).State = EntityState.Modified;
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
		public async Task<ActionResult<web_ThongBao>> PostThongBao(web_ThongBao ThongBao)
		{
			try
			{
				if (ThongBaoExistsDISPLAYNAME(ThongBao.DISPLAYNAME))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + ThongBao.DISPLAYNAME + " trong dữ liệu!",
						Data = ""
					});
				}
				_context.web_ThongBao.Add(ThongBao);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ThongBao
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
		public async Task<IActionResult> DeleteThongBao(string ID)
		{
			try
			{
				web_ThongBao ThongBao = await _context.web_ThongBao.FirstOrDefaultAsync((web_ThongBao e) => e.ID == ID);
				if (ThongBao == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.web_ThongBao.Remove(ThongBao);
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

		private bool ThongBaoExistsID(string ID)
		{
			return _context.web_ThongBao.Any((web_ThongBao e) => e.ID == ID);
		}

		private bool ThongBaoExistsDISPLAYNAME(string DISPLAYNAME)
		{
			return _context.web_ThongBao.Any((web_ThongBao e) => e.DISPLAYNAME == DISPLAYNAME);
		}
	}
}
