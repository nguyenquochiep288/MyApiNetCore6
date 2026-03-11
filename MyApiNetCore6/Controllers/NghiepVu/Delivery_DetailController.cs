using System;
using System.Threading.Tasks;
using DatabaseTHP;
using DatabaseTHP.Class;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using MyApiNetCore6.Data;

namespace MyApiNetCore6.Controllers
{

	[Route("api/[controller]")]
	[ApiController]
	public class Delivery_DetailController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public Delivery_DetailController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}/{ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetInput(string LOC_ID, string ID)
		{
			try
			{
				ct_PhieuGiaoHang_ChiTiet Input = await _context.ct_PhieuGiaoHang_ChiTiet.FirstOrDefaultAsync((ct_PhieuGiaoHang_ChiTiet e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Input == null)
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
					Data = Input
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
		public async Task<IActionResult> PutInput(string LOC_ID, string ID, [FromBody] v_ct_PhieuGiaoHang_ChiTiet Input)
		{
			try
			{
				ct_PhieuGiaoHang PhieuGiaoHang = await _context.ct_PhieuGiaoHang.FirstOrDefaultAsync((ct_PhieuGiaoHang e) => e.LOC_ID == Input.LOC_ID && e.ID == Input.ID_PHIEUGIAOHANG);
				if (PhieuGiaoHang?.ISHOANTAT ?? false)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Phiếu giao hàng " + PhieuGiaoHang.MAPHIEU + " đã được hoàn tất! Nên không thể thay đổi trạng thái!",
						Data = ""
					});
				}
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				_context.Entry(Input).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
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
	}
}
