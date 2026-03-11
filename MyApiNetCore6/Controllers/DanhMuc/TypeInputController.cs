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
	public class TypeInputController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public TypeInputController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetTypeInput(string LOC_ID)
		{
			try
			{
				List<dm_LoaiPhieuNhap> lstValue = await (from e in _context.dm_LoaiPhieuNhap
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
		public async Task<IActionResult> GetTypePayment(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<dm_LoaiPhieuNhap> lstValue = await (from e in _context.dm_LoaiPhieuNhap.Where((dm_LoaiPhieuNhap e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch)
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
		public async Task<IActionResult> GetTypePayment(string LOC_ID, string ID)
		{
			try
			{
				dm_LoaiPhieuNhap TypePayment = await _context.dm_LoaiPhieuNhap.FirstOrDefaultAsync((dm_LoaiPhieuNhap e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (TypePayment == null)
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
					Data = TypePayment
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
		public async Task<IActionResult> PutTypePayment(string LOC_ID, string MA, dm_LoaiPhieuNhap TypePayment)
		{
			try
			{
				if (TypePaymentExists(TypePayment))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + TypePayment.LOC_ID + "-" + TypePayment.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				if (LOC_ID != TypePayment.LOC_ID || TypePayment.MA != MA)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (!TypePaymentExistsID(LOC_ID, TypePayment.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + TypePayment.ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.Entry(TypePayment).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = TypePayment
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
		public async Task<ActionResult<dm_LoaiPhieuNhap>> PostTypePayment(dm_LoaiPhieuNhap TypePayment)
		{
			try
			{
				if (TypePaymentExistsMA(TypePayment.LOC_ID, TypePayment.MA))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + TypePayment.LOC_ID + "-" + TypePayment.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				_context.dm_LoaiPhieuNhap.Add(TypePayment);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = TypePayment
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
		public async Task<IActionResult> DeleteTypePayment(string LOC_ID, string ID)
		{
			try
			{
				dm_LoaiPhieuNhap TypePayment = await _context.dm_LoaiPhieuNhap.FirstOrDefaultAsync((dm_LoaiPhieuNhap e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (TypePayment == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
				ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete(TypePayment, TypePayment.ID, TypePayment.NAME);
				if (!apiResponse.Success)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = apiResponse.Message,
						Data = ""
					});
				}
				_context.dm_LoaiPhieuNhap.Remove(TypePayment);
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

		private bool TypePaymentExistsMA(string LOC_ID, string MA)
		{
			return _context.dm_LoaiPhieuNhap.Any((dm_LoaiPhieuNhap e) => e.LOC_ID == LOC_ID && e.MA == MA);
		}

		private bool TypePaymentExistsID(string LOC_ID, string ID)
		{
			return _context.dm_LoaiPhieuNhap.Any((dm_LoaiPhieuNhap e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}

		private bool TypePaymentExists(dm_LoaiPhieuNhap Position)
		{
			return _context.dm_LoaiPhieuNhap.Any((dm_LoaiPhieuNhap e) => e.LOC_ID == Position.LOC_ID && e.MA == Position.MA && e.ID != Position.ID);
		}
	}
}
