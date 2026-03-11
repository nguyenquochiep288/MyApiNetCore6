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
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using MyApiNetCore6.Data;
using Newtonsoft.Json;

namespace MyApiNetCore6.Controllers
{

	[Route("api/[controller]")]
	[ApiController]
	public class PromotionController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public PromotionController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetProduct(string LOC_ID)
		{
			try
			{
				List<view_dm_ChuongTrinhKhuyenMai> lstValue = await (from e in _context.view_dm_ChuongTrinhKhuyenMai
																	 where e.LOC_ID == LOC_ID
																	 orderby e.DENNGAY descending
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
		public async Task<IActionResult> GetProduct(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<view_dm_ChuongTrinhKhuyenMai> lstValue = await (from e in _context.view_dm_ChuongTrinhKhuyenMai.Where((view_dm_ChuongTrinhKhuyenMai e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch)
																	 orderby e.DENNGAY descending
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
		public async Task<IActionResult> GetProduct(string LOC_ID, string ID)
		{
			try
			{
				view_dm_ChuongTrinhKhuyenMai Product = await _context.view_dm_ChuongTrinhKhuyenMai.FirstOrDefaultAsync((view_dm_ChuongTrinhKhuyenMai e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Product == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				v_view_dm_ChuongTrinhKhuyenMai dm_ChuongTrinhKhuyenMai2 = new v_view_dm_ChuongTrinhKhuyenMai
				{
					lstdm_ChuongTrinhKhuyenMai_Tang = new List<v_dm_ChuongTrinhKhuyenMai_Tang>(),
					lstdm_ChuongTrinhKhuyenMai_YeuCau = new List<v_dm_ChuongTrinhKhuyenMai_YeuCau>()
				};
				if (Product != null)
				{
					dm_ChuongTrinhKhuyenMai2 = JsonConvert.DeserializeObject<v_view_dm_ChuongTrinhKhuyenMai>(JsonConvert.SerializeObject(Product)) ?? new v_view_dm_ChuongTrinhKhuyenMai();
					dm_ChuongTrinhKhuyenMai2.lstdm_ChuongTrinhKhuyenMai_Tang = JsonConvert.DeserializeObject<List<v_dm_ChuongTrinhKhuyenMai_Tang>>(JsonConvert.SerializeObject(await _context.view_dm_ChuongTrinhKhuyenMai_Tang.Where((view_dm_ChuongTrinhKhuyenMai_Tang e) => e.LOC_ID == LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == Product.ID).ToListAsync()));
					dm_ChuongTrinhKhuyenMai2.lstdm_ChuongTrinhKhuyenMai_YeuCau = JsonConvert.DeserializeObject<List<v_dm_ChuongTrinhKhuyenMai_YeuCau>>(JsonConvert.SerializeObject(await _context.view_dm_ChuongTrinhKhuyenMai_YeuCau.Where((view_dm_ChuongTrinhKhuyenMai_YeuCau e) => e.LOC_ID == LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == Product.ID).ToListAsync()));
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = dm_ChuongTrinhKhuyenMai2
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
		public async Task<IActionResult> PutProduct(string LOC_ID, string MA, [FromBody] v_dm_ChuongTrinhKhuyenMai ChuongTrinhKhuyenMai)
		{
			try
			{
				if (ProductExists(ChuongTrinhKhuyenMai))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + ChuongTrinhKhuyenMai.LOC_ID + "-" + ChuongTrinhKhuyenMai.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				if (LOC_ID != ChuongTrinhKhuyenMai.LOC_ID || ChuongTrinhKhuyenMai.MA != MA)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (!ProductExistsID(ChuongTrinhKhuyenMai.LOC_ID, ChuongTrinhKhuyenMai.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + ChuongTrinhKhuyenMai.LOC_ID + "-" + ChuongTrinhKhuyenMai.ID + " dữ liệu!",
						Data = ""
					});
				}
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				foreach (dm_ChuongTrinhKhuyenMai_Tang itm in await _context.dm_ChuongTrinhKhuyenMai_Tang.Where((dm_ChuongTrinhKhuyenMai_Tang e) => e.LOC_ID == LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == ChuongTrinhKhuyenMai.ID).ToListAsync())
				{
					_context.dm_ChuongTrinhKhuyenMai_Tang.Remove(itm);
				}
				foreach (dm_ChuongTrinhKhuyenMai_YeuCau itm2 in await _context.dm_ChuongTrinhKhuyenMai_YeuCau.Where((dm_ChuongTrinhKhuyenMai_YeuCau e) => e.LOC_ID == LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == ChuongTrinhKhuyenMai.ID).ToListAsync())
				{
					_context.dm_ChuongTrinhKhuyenMai_YeuCau.Remove(itm2);
				}
				foreach (v_dm_ChuongTrinhKhuyenMai_Tang itm3 in ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_Tang)
				{
					itm3.ID_CHUONGTRINHKHUYENMAI = ChuongTrinhKhuyenMai.ID;
					_context.dm_ChuongTrinhKhuyenMai_Tang.Add(itm3);
				}
				foreach (v_dm_ChuongTrinhKhuyenMai_YeuCau itm4 in ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_YeuCau)
				{
					itm4.ID_CHUONGTRINHKHUYENMAI = ChuongTrinhKhuyenMai.ID;
					_context.dm_ChuongTrinhKhuyenMai_YeuCau.Add(itm4);
				}
				_context.Entry(ChuongTrinhKhuyenMai).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
				view_dm_ChuongTrinhKhuyenMai OKProduct = await _context.view_dm_ChuongTrinhKhuyenMai.FirstOrDefaultAsync((view_dm_ChuongTrinhKhuyenMai e) => e.LOC_ID == ChuongTrinhKhuyenMai.LOC_ID && e.ID == ChuongTrinhKhuyenMai.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKProduct
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
		public async Task<ActionResult<v_dm_ChuongTrinhKhuyenMai>> PostProduct([FromBody] v_dm_ChuongTrinhKhuyenMai ChuongTrinhKhuyenMai)
		{
			try
			{
				if (ProductExistsMA(ChuongTrinhKhuyenMai.LOC_ID, ChuongTrinhKhuyenMai.MA))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + ChuongTrinhKhuyenMai.LOC_ID + "-" + ChuongTrinhKhuyenMai.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				foreach (v_dm_ChuongTrinhKhuyenMai_Tang itm in ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_Tang)
				{
					itm.ID_CHUONGTRINHKHUYENMAI = ChuongTrinhKhuyenMai.ID;
					_context.dm_ChuongTrinhKhuyenMai_Tang.Add(itm);
				}
				foreach (v_dm_ChuongTrinhKhuyenMai_YeuCau itm2 in ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_YeuCau)
				{
					itm2.ID_CHUONGTRINHKHUYENMAI = ChuongTrinhKhuyenMai.ID;
					_context.dm_ChuongTrinhKhuyenMai_YeuCau.Add(itm2);
				}
				_context.dm_ChuongTrinhKhuyenMai.Add(ChuongTrinhKhuyenMai);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
				view_dm_ChuongTrinhKhuyenMai OKProduct = await _context.view_dm_ChuongTrinhKhuyenMai.FirstOrDefaultAsync((view_dm_ChuongTrinhKhuyenMai e) => e.LOC_ID == ChuongTrinhKhuyenMai.LOC_ID && e.ID == ChuongTrinhKhuyenMai.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKProduct
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
		public async Task<IActionResult> DeleteProduct(string LOC_ID, string ID)
		{
			try
			{
				dm_ChuongTrinhKhuyenMai ChuongTrinhKhuyenMai = await _context.dm_ChuongTrinhKhuyenMai.AsNoTracking().FirstOrDefaultAsync((dm_ChuongTrinhKhuyenMai e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (ChuongTrinhKhuyenMai == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
				ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete(ChuongTrinhKhuyenMai, ChuongTrinhKhuyenMai.ID, ChuongTrinhKhuyenMai.NAME);
				if (!apiResponse.Success)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = apiResponse.Message,
						Data = ""
					});
				}
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				foreach (dm_ChuongTrinhKhuyenMai_Tang itm in await _context.dm_ChuongTrinhKhuyenMai_Tang.Where((dm_ChuongTrinhKhuyenMai_Tang e) => e.LOC_ID == LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == ID).ToListAsync())
				{
					_context.dm_ChuongTrinhKhuyenMai_Tang.Remove(itm);
				}
				foreach (dm_ChuongTrinhKhuyenMai_YeuCau itm2 in await _context.dm_ChuongTrinhKhuyenMai_YeuCau.Where((dm_ChuongTrinhKhuyenMai_YeuCau e) => e.LOC_ID == LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == ID).ToListAsync())
				{
					_context.dm_ChuongTrinhKhuyenMai_YeuCau.Remove(itm2);
				}
				_context.dm_ChuongTrinhKhuyenMai.Remove(ChuongTrinhKhuyenMai);
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

		private bool ProductExistsMA(string LOC_ID, string MA)
		{
			return _context.dm_ChuongTrinhKhuyenMai.Any((dm_ChuongTrinhKhuyenMai e) => e.LOC_ID == LOC_ID && e.MA == MA);
		}

		private bool ProductExistsID(string LOC_ID, string ID)
		{
			return _context.dm_ChuongTrinhKhuyenMai.Any((dm_ChuongTrinhKhuyenMai e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}

		private bool ProductExists(dm_ChuongTrinhKhuyenMai Position)
		{
			return _context.dm_ChuongTrinhKhuyenMai.Any((dm_ChuongTrinhKhuyenMai e) => e.LOC_ID == Position.LOC_ID && e.MA == Position.MA && e.ID != Position.ID);
		}
	}
}
