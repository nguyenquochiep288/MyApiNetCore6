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
using MyApiNetCore6.Controllers;
using MyApiNetCore6.Data;

namespace API_QuanLyTHP.Controllers.PhanQuyen;

[Route("api/[controller]")]
[ApiController]
public class GroupPermissionsController : ControllerBase
{
	private readonly dbTrangHiepPhatContext _context;

	private readonly IConfiguration _configuration;

	public GroupPermissionsController(dbTrangHiepPhatContext context, IConfiguration configuration)
	{
		_context = context;
		_context = context;
		_configuration = configuration;
	}

	[HttpGet("{LOC_ID}")]
	[Authorize(Roles = "User")]
	public async Task<IActionResult> GetGroupPermissions(string LOC_ID)
	{
		try
		{
			List<web_NhomQuyen> lstValue = await _context.web_NhomQuyen.Where((web_NhomQuyen e) => e.LOC_ID == LOC_ID).ToListAsync();
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
	public async Task<IActionResult> GetGroupPermissions(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
	{
		try
		{
			List<web_NhomQuyen> lstValue = await _context.web_NhomQuyen.Where((web_NhomQuyen e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).ToListAsync();
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
	public async Task<IActionResult> GetGroupPermissions(string LOC_ID, string ID)
	{
		try
		{
			web_NhomQuyen GroupPermissions = await _context.web_NhomQuyen.FirstOrDefaultAsync((web_NhomQuyen e) => e.LOC_ID == LOC_ID && e.ID == ID);
			if (GroupPermissions == null)
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
				Data = GroupPermissions
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
	public async Task<IActionResult> PutGroupPermissions(string LOC_ID, string MA, web_NhomQuyen GroupPermissions)
	{
		try
		{
			if (LOC_ID != GroupPermissions.LOC_ID || GroupPermissions.MA != MA)
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Dữ liệu khóa không giống nhau!",
					Data = ""
				});
			}
			if (!GroupPermissionsExistsID(LOC_ID, GroupPermissions.ID))
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Không tìm thấy " + LOC_ID + "-" + GroupPermissions.ID + " dữ liệu!",
					Data = ""
				});
			}
			_context.Entry(GroupPermissions).State = EntityState.Modified;
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
	public async Task<ActionResult<web_NhomQuyen>> PostGroupPermissions(web_NhomQuyen GroupPermissions)
	{
		try
		{
			if (GroupPermissionsExistsMA(GroupPermissions.LOC_ID, GroupPermissions.MA))
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Đã tồn tại" + GroupPermissions.LOC_ID + "-" + GroupPermissions.MA + " trong dữ liệu!",
					Data = ""
				});
			}
			_context.web_NhomQuyen.Add(GroupPermissions);
			AuditLogController auditLog = new AuditLogController(_context, _configuration);
			auditLog.InserAuditLog();
			await _context.SaveChangesAsync();
			return Ok(new ApiResponse
			{
				Success = true,
				Message = "Success",
				Data = GroupPermissions
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
	public async Task<IActionResult> DeleteGroupPermissions(string LOC_ID, string ID)
	{
		try
		{
			web_NhomQuyen GroupPermissions = await _context.web_NhomQuyen.FirstOrDefaultAsync((web_NhomQuyen e) => e.LOC_ID == LOC_ID && e.ID == ID);
			if (GroupPermissions == null)
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
					Data = ""
				});
			}
			_context.web_NhomQuyen.Remove(GroupPermissions);
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

	private bool GroupPermissionsExistsMA(string LOC_ID, string MA)
	{
		return _context.web_NhomQuyen.Any((web_NhomQuyen e) => e.LOC_ID == LOC_ID && e.MA == MA);
	}

	private bool GroupPermissionsExistsID(string LOC_ID, string ID)
	{
		return _context.web_NhomQuyen.Any((web_NhomQuyen e) => e.LOC_ID == LOC_ID && e.ID == ID);
	}
}
