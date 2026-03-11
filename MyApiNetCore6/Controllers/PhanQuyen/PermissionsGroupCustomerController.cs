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
	public class PermissionsGroupCustomerController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public PermissionsGroupCustomerController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetPermissionsGroupCustomer(string LOC_ID)
		{
			try
			{
				List<web_PhanQuyenNhomKhachHang> lstValue = await _context.web_PhanQuyenNhomKhachHang.Where((web_PhanQuyenNhomKhachHang e) => e.LOC_ID == LOC_ID).ToListAsync();
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
		public async Task<IActionResult> GetPermissionsGroupCustomer(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				List<web_PhanQuyenNhomKhachHang> lstValue = await _context.web_PhanQuyenNhomKhachHang.Where((web_PhanQuyenNhomKhachHang e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).ToListAsync();
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

		[HttpGet("{LOC_ID}/{id}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetPermissionsGroupCustomer(string LOC_ID, string ID)
		{
			try
			{
				web_PhanQuyenNhomKhachHang PermissionsGroupCustomer = await _context.web_PhanQuyenNhomKhachHang.FirstOrDefaultAsync((web_PhanQuyenNhomKhachHang e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (PermissionsGroupCustomer == null)
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
					Data = PermissionsGroupCustomer
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

		[HttpPut("{LOC_ID}/{ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> PutPermissionsGroupCustomer(string LOC_ID, string ID, web_PhanQuyenNhomKhachHang PermissionsGroupCustomer)
		{
			try
			{
				if (LOC_ID != PermissionsGroupCustomer.LOC_ID && ID != PermissionsGroupCustomer.ID)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (!PermissionsGroupCustomerExists(PermissionsGroupCustomer.LOC_ID, PermissionsGroupCustomer.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.Entry(PermissionsGroupCustomer).State = EntityState.Modified;
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
		public async Task<ActionResult<web_PhanQuyenNhomKhachHang>> PostPermissionsGroupCustomer(web_PhanQuyenNhomKhachHang PermissionsGroupCustomer)
		{
			try
			{
				if (PermissionsGroupCustomerExists(PermissionsGroupCustomer.LOC_ID, PermissionsGroupCustomer.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại " + PermissionsGroupCustomer.LOC_ID + "-" + PermissionsGroupCustomer.ID + " trong dữ liệu!",
						Data = "",
						CheckValue = true
					});
				}
				_context.web_PhanQuyenNhomKhachHang.Add(PermissionsGroupCustomer);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = PermissionsGroupCustomer
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
		public async Task<IActionResult> DeletePermissionsGroupCustomer(string LOC_ID, string ID)
		{
			try
			{
				web_PhanQuyenNhomKhachHang PermissionsGroupCustomer = await _context.web_PhanQuyenNhomKhachHang.FirstOrDefaultAsync((web_PhanQuyenNhomKhachHang e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (PermissionsGroupCustomer == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.web_PhanQuyenNhomKhachHang.Remove(PermissionsGroupCustomer);
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

		private bool PermissionsGroupCustomerExists(string LOC_ID, string ID)
		{
			return _context.web_PhanQuyenNhomKhachHang.Any((web_PhanQuyenNhomKhachHang e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}
	}
}
