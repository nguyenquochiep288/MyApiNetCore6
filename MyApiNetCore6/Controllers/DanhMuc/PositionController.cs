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
	public class PositionController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public PositionController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetPosition(string LOC_ID)
		{
			try
			{
				List<dm_ChucVu> lstValue = await (from e in _context.dm_ChucVu
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
		public async Task<IActionResult> GetPosition(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<dm_ChucVu> lstValue = await (from e in _context.dm_ChucVu.Where((dm_ChucVu e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch)
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
		public async Task<IActionResult> GetPosition(string LOC_ID, string ID)
		{
			try
			{
				dm_ChucVu Position = await _context.dm_ChucVu.FirstOrDefaultAsync((dm_ChucVu e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Position == null)
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
					Data = Position
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
		public async Task<IActionResult> PutPosition(string LOC_ID, string MA, dm_ChucVu Position)
		{
			try
			{
				if (PositionExists(Position))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Position.LOC_ID + "-" + Position.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				if (LOC_ID != Position.LOC_ID || Position.MA != MA)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (!PositionExistsID(LOC_ID, Position.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + Position.ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.Entry(Position).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				dm_TienTe OKPosition = await _context.dm_TienTe.FirstOrDefaultAsync((dm_TienTe e) => e.LOC_ID == Position.LOC_ID && e.ID == Position.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKPosition
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
		public async Task<ActionResult<dm_ChucVu>> PostPosition(dm_ChucVu Position)
		{
			try
			{
				if (PositionExistsMA(Position.LOC_ID, Position.MA))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Position.LOC_ID + "-" + Position.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				_context.dm_ChucVu.Add(Position);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				dm_TienTe OKPosition = await _context.dm_TienTe.FirstOrDefaultAsync((dm_TienTe e) => e.LOC_ID == Position.LOC_ID && e.ID == Position.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKPosition
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
		public async Task<IActionResult> DeletePosition(string LOC_ID, string ID)
		{
			try
			{
				dm_ChucVu Position = await _context.dm_ChucVu.FirstOrDefaultAsync((dm_ChucVu e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Position == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
				ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete(Position, Position.ID, Position.NAME);
				if (!apiResponse.Success)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = apiResponse.Message,
						Data = ""
					});
				}
				_context.dm_ChucVu.Remove(Position);
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

		private bool PositionExistsMA(string LOC_ID, string MA)
		{
			return _context.dm_ChucVu.Any((dm_ChucVu e) => e.LOC_ID == LOC_ID && e.MA == MA);
		}

		private bool PositionExistsID(string LOC_ID, string ID)
		{
			return _context.dm_ChucVu.Any((dm_ChucVu e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}

		private bool PositionExists(dm_ChucVu Position)
		{
			return _context.dm_ChucVu.Any((dm_ChucVu e) => e.LOC_ID == Position.LOC_ID && e.MA == Position.MA && e.ID != Position.ID);
		}
	}
}
