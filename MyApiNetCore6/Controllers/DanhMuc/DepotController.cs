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

namespace MyApiNetCore6.Controllers
{

	[Route("api/[controller]")]
	[ApiController]
	public class DepotController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public DepotController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetDepot(string LOC_ID)
		{
			try
			{
				List<dm_Kho> lstValue = await (from e in _context.dm_Kho
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
		public async Task<IActionResult> GetDepot(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<dm_Kho> lstValue = await (from e in _context.dm_Kho.Where((dm_Kho e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch)
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
		public async Task<IActionResult> GetDepot(string LOC_ID, string ID)
		{
			try
			{
				dm_Kho Depot = await _context.dm_Kho.FirstOrDefaultAsync((dm_Kho e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Depot == null)
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
					Data = Depot
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
		public async Task<IActionResult> PutDepot(string LOC_ID, string MA, dm_Kho Depot)
		{
			try
			{
				if (DepotExists(Depot))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Depot.LOC_ID + "-" + Depot.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				if (LOC_ID != Depot.LOC_ID || Depot.MA != MA)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (!DepotExistsID(LOC_ID, Depot.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + Depot.ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.Entry(Depot).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				dm_TienTe OKDepot = await _context.dm_TienTe.FirstOrDefaultAsync((dm_TienTe e) => e.LOC_ID == Depot.LOC_ID && e.ID == Depot.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKDepot
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
		public async Task<ActionResult<dm_Kho>> PostDepot(dm_Kho Depot)
		{
			try
			{
				if (DepotExistsMA(Depot.LOC_ID, Depot.MA))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Depot.LOC_ID + "-" + Depot.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				_context.dm_Kho.Add(Depot);
				foreach (dm_HangHoa itm in await _context.dm_HangHoa.Where((dm_HangHoa e) => e.LOC_ID == Depot.LOC_ID).ToListAsync())
				{
					dm_HangHoa_Kho newdm_HangHoa_Kho = new dm_HangHoa_Kho
					{
						ID = Guid.NewGuid().ToString(),
						LOC_ID = Depot.LOC_ID,
						ID_KHO = Depot.ID,
						ID_HANGHOA = itm.ID,
						QTY = 0.0
					};
					_context.dm_HangHoa_Kho.Add(newdm_HangHoa_Kho);
				}
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
				dm_TienTe OKDepot = await _context.dm_TienTe.FirstOrDefaultAsync((dm_TienTe e) => e.LOC_ID == Depot.LOC_ID && e.ID == Depot.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKDepot
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
		public async Task<IActionResult> DeleteDepot(string LOC_ID, string ID)
		{
			try
			{
				dm_Kho Depot = await _context.dm_Kho.FirstOrDefaultAsync((dm_Kho e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Depot == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
				ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete(Depot, Depot.ID, Depot.MA);
				if (!apiResponse.Success)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = apiResponse.Message,
						Data = ""
					});
				}
				_context.dm_Kho.Remove(Depot);
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

		private bool DepotExistsMA(string LOC_ID, string MA)
		{
			return _context.dm_Kho.Any((dm_Kho e) => e.LOC_ID == LOC_ID && e.MA == MA);
		}

		private bool DepotExistsID(string LOC_ID, string ID)
		{
			return _context.dm_Kho.Any((dm_Kho e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}

		private bool DepotExists(dm_Kho Depot)
		{
			return _context.dm_Kho.Any((dm_Kho e) => e.LOC_ID == Depot.LOC_ID && e.MA == Depot.MA && e.ID != Depot.ID);
		}
	}
}
