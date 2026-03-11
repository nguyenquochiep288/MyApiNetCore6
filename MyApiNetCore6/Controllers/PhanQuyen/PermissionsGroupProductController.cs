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
	public class PermissionsGroupProductController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public PermissionsGroupProductController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetPermissionsGroupProduct(string LOC_ID)
		{
			try
			{
				List<web_PhanQuyenNhomSanPham> lstValue = await _context.web_PhanQuyenNhomSanPham.Where((web_PhanQuyenNhomSanPham e) => e.LOC_ID == LOC_ID).ToListAsync();
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
		public async Task<IActionResult> GetPermissionsGroupProduct(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				List<web_PhanQuyenNhomSanPham> lstValue = await _context.web_PhanQuyenNhomSanPham.Where((web_PhanQuyenNhomSanPham e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).ToListAsync();
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
		public async Task<IActionResult> GetPermissionsGroupProduct(string LOC_ID, string ID)
		{
			try
			{
				web_PhanQuyenNhomSanPham PermissionsGroupProduct = await _context.web_PhanQuyenNhomSanPham.FirstOrDefaultAsync((web_PhanQuyenNhomSanPham e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (PermissionsGroupProduct == null)
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
					Data = PermissionsGroupProduct
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
		public async Task<IActionResult> PutPermissionsGroupProduct(string LOC_ID, string ID, web_PhanQuyenNhomSanPham PermissionsGroupProduct)
		{
			try
			{
				if (LOC_ID != PermissionsGroupProduct.LOC_ID && ID != PermissionsGroupProduct.ID)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (!PermissionsGroupProductExists(PermissionsGroupProduct.LOC_ID, PermissionsGroupProduct.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.Entry(PermissionsGroupProduct).State = EntityState.Modified;
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
		public async Task<ActionResult<web_PhanQuyenNhomSanPham>> PostPermissionsGroupProduct(web_PhanQuyenNhomSanPham PermissionsGroupProduct)
		{
			try
			{
				if (PermissionsGroupProductExists(PermissionsGroupProduct.LOC_ID, PermissionsGroupProduct.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại " + PermissionsGroupProduct.LOC_ID + "-" + PermissionsGroupProduct.ID + " trong dữ liệu!",
						Data = "",
						CheckValue = true
					});
				}
				_context.web_PhanQuyenNhomSanPham.Add(PermissionsGroupProduct);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = PermissionsGroupProduct
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
		public async Task<IActionResult> DeletePermissionsGroupProduct(string LOC_ID, string ID)
		{
			try
			{
				web_PhanQuyenNhomSanPham PermissionsGroupProduct = await _context.web_PhanQuyenNhomSanPham.FirstOrDefaultAsync((web_PhanQuyenNhomSanPham e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (PermissionsGroupProduct == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.web_PhanQuyenNhomSanPham.Remove(PermissionsGroupProduct);
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

		private bool PermissionsGroupProductExists(string LOC_ID, string ID)
		{
			return _context.web_PhanQuyenNhomSanPham.Any((web_PhanQuyenNhomSanPham e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}
	}
}
