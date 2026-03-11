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
	public class AccountUnibenController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public AccountUnibenController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetCompany()
		{
			try
			{
				List<dm_TaiKhoan_Uniben> lstValue = await _context.dm_TaiKhoan_Uniben.ToListAsync();
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

		[HttpGet("{Type}/{KeyWhere}/{ValuesSearch}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetCompany(int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<dm_TaiKhoan_Uniben> lstValue = await _context.dm_TaiKhoan_Uniben.Where(KeyWhere, ValuesSearch).ToListAsync();
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

		[HttpGet("{ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetCompany(string ID)
		{
			try
			{
				dm_TaiKhoan_Uniben Company = await _context.dm_TaiKhoan_Uniben.FirstOrDefaultAsync((dm_TaiKhoan_Uniben e) => e.ID == ID);
				if (Company == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + ID + " dữ liệu!",
						Data = ""
					});
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = Company
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

		[HttpPut("{MA}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> PutCompany(string MA, dm_TaiKhoan_Uniben Company)
		{
			try
			{
				if (MA != Company.MASOTHUE)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (!CompanyExistsID(Company.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + Company.ID + " dữ liệu!",
						Data = ""
					});
				}
				dm_TaiKhoan_Uniben TaiKhoan_Misa = await _context.dm_TaiKhoan_Uniben.FirstOrDefaultAsync((dm_TaiKhoan_Uniben e) => e.ID == Company.ID);
				if (TaiKhoan_Misa != null)
				{
					if (TaiKhoan_Misa.USERNAME != Company.USERNAME || TaiKhoan_Misa.PASSWORD != Company.PASSWORD || TaiKhoan_Misa.MASOTHUE != Company.MASOTHUE)
					{
						TaiKhoan_Misa.ACCESSTOKEN = "";
						TaiKhoan_Misa.EXPIRESIN = null;
						TaiKhoan_Misa.USERID = "";
						TaiKhoan_Misa.ORGANIZATIONUNITID = "";
						TaiKhoan_Misa.COMPANYID = null;
						TaiKhoan_Misa.THOIGIANLAYTOKEN = null;
						foreach (dm_LoaiHoaDon itm in await _context.dm_LoaiHoaDon.Where((dm_LoaiHoaDon e) => e.LOC_ID == Company.LOC_ID).ToListAsync())
						{
							itm.IPTEMPLATEID = "";
							itm.INVSERIES = "";
							_context.Entry(itm).State = EntityState.Modified;
						}
					}
					TaiKhoan_Misa.USERNAME = Company.USERNAME;
					TaiKhoan_Misa.PASSWORD = Company.PASSWORD;
					TaiKhoan_Misa.MASOTHUE = Company.MASOTHUE;
					TaiKhoan_Misa.LINK = Company.LINK;
					TaiKhoan_Misa.ISACTIVE = Company.ISACTIVE;
					TaiKhoan_Misa.THOIGIANTHEM = Company.THOIGIANTHEM;
					TaiKhoan_Misa.THOIGIANSUA = Company.THOIGIANSUA;
					TaiKhoan_Misa.ID_NGUOITAO = Company.ID_NGUOITAO;
					TaiKhoan_Misa.ID_NGUOISUA = Company.ID_NGUOISUA;
					_context.Entry(TaiKhoan_Misa).State = EntityState.Modified;
				}
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				dm_TaiKhoan_Uniben OKCompany = await _context.dm_TaiKhoan_Uniben.FirstOrDefaultAsync((dm_TaiKhoan_Uniben e) => e.ID == Company.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKCompany
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
		public async Task<ActionResult<dm_TaiKhoan_Uniben>> PostCompany(dm_TaiKhoan_Uniben Company)
		{
			try
			{
				if (CompanyExistsMA(Company.MASOTHUE))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Company.MASOTHUE + " trong dữ liệu!",
						Data = ""
					});
				}
				_context.dm_TaiKhoan_Uniben.Add(Company);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				dm_TaiKhoan_Uniben OKCompany = await _context.dm_TaiKhoan_Uniben.FirstOrDefaultAsync((dm_TaiKhoan_Uniben e) => e.ID == Company.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKCompany
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

		[HttpDelete("{ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> DeleteCompany(string ID)
		{
			try
			{
				dm_TaiKhoan_Uniben Company = await _context.dm_TaiKhoan_Uniben.FirstOrDefaultAsync((dm_TaiKhoan_Uniben e) => e.ID == ID);
				if (Company == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.dm_TaiKhoan_Uniben.Remove(Company);
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

		private bool CompanyExistsID(string ID)
		{
			return _context.dm_TaiKhoan_Uniben.Any((dm_TaiKhoan_Uniben e) => e.ID == ID);
		}

		private bool CompanyExistsMA(string MA)
		{
			return _context.dm_TaiKhoan_Uniben.Any((dm_TaiKhoan_Uniben e) => e.MASOTHUE == MA);
		}

		private bool CompanyExists(dm_TaiKhoan_Uniben Company)
		{
			return _context.dm_TaiKhoan_Uniben.Any((dm_TaiKhoan_Uniben e) => e.MASOTHUE == Company.MASOTHUE && e.ID != Company.ID);
		}
	}
}
