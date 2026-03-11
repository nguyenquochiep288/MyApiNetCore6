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
	public class TypeReceiptController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public TypeReceiptController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetTypeReceipt(string LOC_ID)
		{
			try
			{
				List<dm_LoaiPhieuThu> lstValue = await (from e in _context.dm_LoaiPhieuThu
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
		public async Task<IActionResult> GetTypeReceipt(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<dm_LoaiPhieuThu> lstValue = await (from e in _context.dm_LoaiPhieuThu.Where((dm_LoaiPhieuThu e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch)
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
		public async Task<IActionResult> GetTypeReceipt(string LOC_ID, string ID)
		{
			try
			{
				dm_LoaiPhieuThu TypeReceipt = await _context.dm_LoaiPhieuThu.FirstOrDefaultAsync((dm_LoaiPhieuThu e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (TypeReceipt == null)
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
					Data = TypeReceipt
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
		public async Task<IActionResult> PutTypeReceipt(string LOC_ID, string MA, dm_LoaiPhieuThu TypeReceipt)
		{
			try
			{
				if (TypeReceiptExists(TypeReceipt))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + TypeReceipt.LOC_ID + "-" + TypeReceipt.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				if (LOC_ID != TypeReceipt.LOC_ID || TypeReceipt.MA != MA)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (!TypeReceiptExistsID(LOC_ID, TypeReceipt.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + TypeReceipt.ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.Entry(TypeReceipt).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = TypeReceipt
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
		public async Task<ActionResult<dm_LoaiPhieuThu>> PostTypeReceipt(dm_LoaiPhieuThu TypeReceipt)
		{
			try
			{
				if (TypeReceiptExistsMA(TypeReceipt.LOC_ID, TypeReceipt.MA))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + TypeReceipt.LOC_ID + "-" + TypeReceipt.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				_context.dm_LoaiPhieuThu.Add(TypeReceipt);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = TypeReceipt
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
		public async Task<IActionResult> DeleteTypeReceipt(string LOC_ID, string ID)
		{
			try
			{
				dm_LoaiPhieuThu TypeReceipt = await _context.dm_LoaiPhieuThu.FirstOrDefaultAsync((dm_LoaiPhieuThu e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (TypeReceipt == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
				ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete(TypeReceipt, TypeReceipt.ID, TypeReceipt.NAME);
				if (!apiResponse.Success)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = apiResponse.Message,
						Data = ""
					});
				}
				_context.dm_LoaiPhieuThu.Remove(TypeReceipt);
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

		private bool TypeReceiptExistsMA(string LOC_ID, string MA)
		{
			return _context.dm_LoaiPhieuThu.Any((dm_LoaiPhieuThu e) => e.LOC_ID == LOC_ID && e.MA == MA);
		}

		private bool TypeReceiptExistsID(string LOC_ID, string ID)
		{
			return _context.dm_LoaiPhieuThu.Any((dm_LoaiPhieuThu e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}

		private bool TypeReceiptExists(dm_LoaiPhieuThu Position)
		{
			return _context.dm_LoaiPhieuThu.Any((dm_LoaiPhieuThu e) => e.LOC_ID == Position.LOC_ID && e.MA == Position.MA && e.ID != Position.ID);
		}
	}
}
