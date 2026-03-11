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
	public class GroupProviderController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public GroupProviderController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetGroupProvider(string LOC_ID)
		{
			try
			{
				List<dm_NhomNhaCungCap> lstValue = await (from e in _context.dm_NhomNhaCungCap
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
		public async Task<IActionResult> GetGroupCusVen(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<dm_NhomNhaCungCap> lstValue = await (from e in _context.dm_NhomNhaCungCap.Where((dm_NhomNhaCungCap e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch)
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
		public async Task<IActionResult> GetGroupCusVen(string LOC_ID, string ID)
		{
			try
			{
				dm_NhomNhaCungCap GroupCusVen = await _context.dm_NhomNhaCungCap.FirstOrDefaultAsync((dm_NhomNhaCungCap e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (GroupCusVen == null)
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
					Data = GroupCusVen
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
		public async Task<IActionResult> PutGroupCusVen(string LOC_ID, string MA, dm_NhomNhaCungCap GroupCusVen)
		{
			try
			{
				if (GroupCusVenExists(GroupCusVen))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + GroupCusVen.LOC_ID + "-" + GroupCusVen.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				if (LOC_ID != GroupCusVen.LOC_ID || GroupCusVen.MA != MA)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (!GroupCusVenExistsID(LOC_ID, GroupCusVen.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + GroupCusVen.ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.Entry(GroupCusVen).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = GroupCusVen
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
		public async Task<ActionResult<dm_NhomNhaCungCap>> PostGroupCusVen(dm_NhomNhaCungCap GroupCusVen)
		{
			try
			{
				if (GroupCusVenExistsMA(GroupCusVen.LOC_ID, GroupCusVen.MA))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + GroupCusVen.LOC_ID + "-" + GroupCusVen.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				_context.dm_NhomNhaCungCap.Add(GroupCusVen);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = GroupCusVen
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
		public async Task<IActionResult> DeleteGroupCusVen(string LOC_ID, string ID)
		{
			try
			{
				dm_NhomNhaCungCap GroupCusVen = await _context.dm_NhomNhaCungCap.FirstOrDefaultAsync((dm_NhomNhaCungCap e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (GroupCusVen == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
				ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete(GroupCusVen, GroupCusVen.ID, GroupCusVen.MA);
				if (!apiResponse.Success)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = apiResponse.Message,
						Data = ""
					});
				}
				_context.dm_NhomNhaCungCap.Remove(GroupCusVen);
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

		private bool GroupCusVenExistsMA(string LOC_ID, string MA)
		{
			return _context.dm_NhomNhaCungCap.Any((dm_NhomNhaCungCap e) => e.LOC_ID == LOC_ID && e.MA == MA);
		}

		private bool GroupCusVenExistsID(string LOC_ID, string ID)
		{
			return _context.dm_NhomNhaCungCap.Any((dm_NhomNhaCungCap e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}

		private bool GroupCusVenExists(dm_NhomNhaCungCap GroupCusVen)
		{
			return _context.dm_NhomNhaCungCap.Any((dm_NhomNhaCungCap e) => e.LOC_ID == GroupCusVen.LOC_ID && e.MA == GroupCusVen.MA && e.ID != GroupCusVen.ID);
		}
	}
}
