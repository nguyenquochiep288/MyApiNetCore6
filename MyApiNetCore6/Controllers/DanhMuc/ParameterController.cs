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
	public class ParameterController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public ParameterController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetParameter()
		{
			try
			{
				List<web_Parameter> lstValue = await _context.web_Parameter.ToListAsync();
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
		public async Task<IActionResult> GetParameter(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<web_Parameter> lstValue = await _context.web_Parameter.Where(KeyWhere, ValuesSearch).ToListAsync();
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
		public async Task<IActionResult> GetParameter(string ID)
		{
			try
			{
				web_Parameter Parameter = await _context.web_Parameter.FirstOrDefaultAsync((web_Parameter e) => e.ID == ID);
				if (Parameter == null)
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
					Data = Parameter
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
		public async Task<IActionResult> PutParameter(string MA, web_Parameter Parameter)
		{
			try
			{
				if (Parameter.MA != MA)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (!ParameterExistsID(Parameter.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + Parameter.ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.Entry(Parameter).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				web_Parameter OKParameter = await _context.web_Parameter.FirstOrDefaultAsync((web_Parameter e) => e.ID == Parameter.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKParameter
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
		public async Task<ActionResult<web_Parameter>> PostParameter(web_Parameter Parameter)
		{
			try
			{
				if (ParameterExistsMA(Parameter.MA))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Parameter.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				_context.web_Parameter.Add(Parameter);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				web_Parameter OKParameter = await _context.web_Parameter.FirstOrDefaultAsync((web_Parameter e) => e.ID == Parameter.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKParameter
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
		public async Task<IActionResult> DeleteParameter(string ID)
		{
			try
			{
				web_Parameter Parameter = await _context.web_Parameter.FirstOrDefaultAsync((web_Parameter e) => e.ID == ID);
				if (Parameter == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + ID + " dữ liệu!",
						Data = ""
					});
				}
				ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
				ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete(Parameter, Parameter.ID, Parameter.MA);
				if (!apiResponse.Success)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = apiResponse.Message,
						Data = ""
					});
				}
				List<web_Report_Parameter> lstweb_PhanQuyenSanPham = await _context.web_Report_Parameter.Where((web_Report_Parameter e) => e.ID_PARAMETER == ID).ToListAsync();
				if (lstweb_PhanQuyenSanPham != null)
				{
					foreach (web_Report_Parameter itm in lstweb_PhanQuyenSanPham)
					{
						_context.web_Report_Parameter.Remove(itm);
					}
				}
				_context.web_Parameter.Remove(Parameter);
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

		private bool ParameterExistsMA(string MA)
		{
			return _context.web_Parameter.Any((web_Parameter e) => e.MA == MA);
		}

		private bool ParameterExistsID(string ID)
		{
			return _context.web_Parameter.Any((web_Parameter e) => e.ID == ID);
		}
	}
}
