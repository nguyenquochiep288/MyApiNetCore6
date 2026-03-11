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
	public class GroupProductController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public GroupProductController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetGroupProduct(string LOC_ID)
		{
			try
			{
				List<dm_NhomHangHoa> lstValue = await (from e in _context.dm_NhomHangHoa
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
		public async Task<IActionResult> GetGroupProduct(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<dm_NhomHangHoa> lstValue = await (from e in _context.dm_NhomHangHoa.Where((dm_NhomHangHoa e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch)
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
		public async Task<IActionResult> GetGroupProduct(string LOC_ID, string ID)
		{
			try
			{
				dm_NhomHangHoa GroupProduct = await _context.dm_NhomHangHoa.FirstOrDefaultAsync((dm_NhomHangHoa e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (GroupProduct == null)
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
					Data = GroupProduct
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
		public async Task<IActionResult> PutGroupProduct(string LOC_ID, string MA, dm_NhomHangHoa GroupProduct)
		{
			try
			{
				if (GroupProductExists(GroupProduct))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + GroupProduct.LOC_ID + "-" + GroupProduct.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				if (LOC_ID != GroupProduct.LOC_ID || GroupProduct.MA != MA)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (!GroupProductExistsID(LOC_ID, GroupProduct.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + GroupProduct.ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.Entry(GroupProduct).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = GroupProduct
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
		public async Task<ActionResult<dm_NhomHangHoa>> PostGroupProduct(dm_NhomHangHoa GroupProduct)
		{
			try
			{
				if (GroupProductExistsMA(GroupProduct.LOC_ID, GroupProduct.MA))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + GroupProduct.LOC_ID + "-" + GroupProduct.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				_context.dm_NhomHangHoa.Add(GroupProduct);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = GroupProduct
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
		public async Task<IActionResult> DeleteGroupProduct(string LOC_ID, string ID)
		{
			try
			{
				dm_NhomHangHoa GroupProduct = await _context.dm_NhomHangHoa.FirstOrDefaultAsync((dm_NhomHangHoa e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (GroupProduct == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
				ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete(GroupProduct, GroupProduct.ID, GroupProduct.MA);
				if (!apiResponse.Success)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = apiResponse.Message,
						Data = ""
					});
				}
				List<web_PhanQuyenNhomSanPham> lstweb_PhanQuyenSanPham = await _context.web_PhanQuyenNhomSanPham.Where((web_PhanQuyenNhomSanPham e) => e.LOC_ID == LOC_ID && e.ID_NHOMSANPHAM == ID).ToListAsync();
				if (lstweb_PhanQuyenSanPham != null)
				{
					foreach (web_PhanQuyenNhomSanPham itm in lstweb_PhanQuyenSanPham)
					{
						_context.web_PhanQuyenNhomSanPham.Remove(itm);
					}
				}
				_context.dm_NhomHangHoa.Remove(GroupProduct);
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

		private bool GroupProductExistsMA(string LOC_ID, string MA)
		{
			return _context.dm_NhomHangHoa.Any((dm_NhomHangHoa e) => e.LOC_ID == LOC_ID && e.MA == MA);
		}

		private bool GroupProductExistsID(string LOC_ID, string ID)
		{
			return _context.dm_NhomHangHoa.Any((dm_NhomHangHoa e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}

		private bool GroupProductExists(dm_NhomHangHoa GroupProduct)
		{
			return _context.dm_NhomHangHoa.Any((dm_NhomHangHoa e) => e.LOC_ID == GroupProduct.LOC_ID && e.MA == GroupProduct.MA && e.ID != GroupProduct.ID);
		}
	}
}
