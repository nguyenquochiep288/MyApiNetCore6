using System;
using System.Collections.Generic;
using System.Linq;
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
	public class Product_ComboController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public Product_ComboController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}/{ID_HANGHOACOMBO}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetProduct_Combo(string LOC_ID, string ID_HANGHOACOMBO)
		{
			try
			{
				List<view_dm_HangHoa_Combo> lstValue = await _context.view_dm_HangHoa_Combo.Where((view_dm_HangHoa_Combo e) => e.LOC_ID == LOC_ID && e.ID_HANGHOACOMBO == ID_HANGHOACOMBO).ToListAsync();
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

		[HttpPut("{LOC_ID}/{ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> PutProduct_Combo(string LOC_ID, string ID, dm_HangHoa_Combo Product_Combo)
		{
			try
			{
				if (Product_Combo.ID != ID)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (!Product_ComboExists(LOC_ID, Product_Combo.ID_HANGHOA))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.Entry(Product_Combo).State = EntityState.Modified;
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
		public async Task<ActionResult<dm_HangHoa_Combo>> PostProduct_Combo(dm_HangHoa_Combo Product_Combo)
		{
			try
			{
				if (Product_ComboExists(Product_Combo.LOC_ID, Product_Combo.ID_HANGHOA))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Product_Combo.LOC_ID + "-" + Product_Combo.ID_HANGHOA + " trong dữ liệu!",
						Data = ""
					});
				}
				_context.dm_HangHoa_Combo.Add(Product_Combo);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = Product_Combo
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
		public async Task<IActionResult> DeleteProduct_Combo(string LOC_ID, string ID)
		{
			try
			{
				dm_HangHoa_Combo Product_Combo = await _context.dm_HangHoa_Combo.FirstOrDefaultAsync((dm_HangHoa_Combo e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Product_Combo == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.dm_HangHoa_Combo.Remove(Product_Combo);
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

		private bool Product_ComboExists(string LOC_ID, string ID_HANGHOA)
		{
			return _context.dm_HangHoa_Combo.Any((dm_HangHoa_Combo e) => e.LOC_ID == LOC_ID && e.ID_HANGHOA == ID_HANGHOA);
		}
	}
}
