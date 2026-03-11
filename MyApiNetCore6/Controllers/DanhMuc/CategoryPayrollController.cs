using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
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
	public class CategoryPayrollController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public CategoryPayrollController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetReport()
		{
			try
			{
				List<view_dm_BangLuong> lstValue = await _context.view_dm_BangLuong.ToListAsync();
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
		public async Task<IActionResult> GetReport(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<view_dm_BangLuong> lstValue = await _context.view_dm_BangLuong.Where(KeyWhere, ValuesSearch).ToListAsync();
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
		public async Task<IActionResult> GetReport(string ID)
		{
			try
			{
				view_dm_BangLuong Report = await _context.view_dm_BangLuong.FirstOrDefaultAsync((view_dm_BangLuong e) => e.ID == ID);
				if (Report != null)
				{
					Report.lstdm_BangLuong_ChiTiet = new List<v_dm_BangLuong_ChiTiet>();
					List<dm_BangLuong_ChiTiet> lstdm_BangLuong_ChiTiet = await _context.dm_BangLuong_ChiTiet.Where((dm_BangLuong_ChiTiet e) => e.ID_BANGLUONG == ID).ToListAsync();
					if (lstdm_BangLuong_ChiTiet != null)
					{
						string json = JsonConvert.SerializeObject(lstdm_BangLuong_ChiTiet);
						Report.lstdm_BangLuong_ChiTiet = JsonConvert.DeserializeObject<List<v_dm_BangLuong_ChiTiet>>(json);
					}
				}
				if (Report == null)
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
					Data = Report
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
		public async Task<IActionResult> PutReport(string MA, v_dm_BangLuong Report)
		{
			try
			{
				if (Report.MA != MA)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (!ReportExistsID(Report.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + Report.ID + " dữ liệu!",
						Data = ""
					});
				}
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				foreach (dm_BangLuong_ChiTiet dm_BangLuong_ChiTiet2 in await (from e in _context.dm_BangLuong_ChiTiet.AsNoTracking()
																			  where e.ID_BANGLUONG == Report.ID
																			  select e).ToListAsync())
				{
					if (Report.lstdm_BangLuong_ChiTiet.Where((v_dm_BangLuong_ChiTiet s) => s.ID == dm_BangLuong_ChiTiet2.ID).Count() == 0)
					{
						_context.dm_BangLuong_ChiTiet.Remove(dm_BangLuong_ChiTiet2);
					}
				}
				foreach (v_dm_BangLuong_ChiTiet itm in Report.lstdm_BangLuong_ChiTiet)
				{
					dm_BangLuong_ChiTiet newct_PhieuXuat_CT = ConvertobjectToct_dm_BangLuong_ChiTiet(objectTo: new dm_BangLuong_ChiTiet(), objectFrom: itm);
					if (!string.IsNullOrEmpty(itm.ID))
					{
						if (await _context.dm_BangLuong_ChiTiet.AsNoTracking().FirstOrDefaultAsync((dm_BangLuong_ChiTiet e) => e.ID == itm.ID && e.ID_BANGLUONG == Report.ID) != null)
						{
							_context.Entry(newct_PhieuXuat_CT).State = EntityState.Modified;
							continue;
						}
						newct_PhieuXuat_CT.ID = Guid.NewGuid().ToString();
						newct_PhieuXuat_CT.ID_BANGLUONG = Report.ID;
						_context.dm_BangLuong_ChiTiet.Add(newct_PhieuXuat_CT);
					}
					else
					{
						newct_PhieuXuat_CT.ID = Guid.NewGuid().ToString();
						newct_PhieuXuat_CT.ID_BANGLUONG = Report.ID;
						_context.dm_BangLuong_ChiTiet.Add(newct_PhieuXuat_CT);
					}
				}
				dm_BangLuong newdm_BangLuong = ConvertobjectToct_dm_BangLuong(objectTo: new dm_BangLuong(), objectFrom: Report);
				_context.dm_BangLuong.Add(newdm_BangLuong);
				_context.Entry(newdm_BangLuong).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
				view_dm_BangLuong OKReport = await _context.view_dm_BangLuong.FirstOrDefaultAsync((view_dm_BangLuong e) => e.ID == Report.ID);
				if (OKReport != null)
				{
					OKReport.lstdm_BangLuong_ChiTiet = new List<v_dm_BangLuong_ChiTiet>();
					List<dm_BangLuong_ChiTiet> lstdm_BangLuong_ChiTiet = await _context.dm_BangLuong_ChiTiet.Where((dm_BangLuong_ChiTiet e) => e.ID_BANGLUONG == Report.ID).ToListAsync();
					if (lstdm_BangLuong_ChiTiet != null)
					{
						string json = JsonConvert.SerializeObject(lstdm_BangLuong_ChiTiet);
						OKReport.lstdm_BangLuong_ChiTiet = JsonConvert.DeserializeObject<List<v_dm_BangLuong_ChiTiet>>(json);
					}
				}
				if (OKReport == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + Report.ID + " dữ liệu!",
						Data = ""
					});
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKReport
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

		private static dm_BangLuong_ChiTiet ConvertobjectToct_dm_BangLuong_ChiTiet<T>(T objectFrom, dm_BangLuong_ChiTiet objectTo)
		{
			if (objectFrom != null)
			{
				PropertyInfo[] properties = objectFrom.GetType().GetProperties();
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					if (!(propertyInfo != null))
					{
						continue;
					}
					object value = propertyInfo.GetValue(objectFrom);
					if (value != null)
					{
						PropertyInfo property = objectTo.GetType().GetProperty(propertyInfo.Name);
						if (property != null)
						{
							property.SetValue(objectTo, value);
						}
					}
				}
			}
			return objectTo;
		}

		private static dm_BangLuong ConvertobjectToct_dm_BangLuong<T>(T objectFrom, dm_BangLuong objectTo)
		{
			if (objectFrom != null)
			{
				PropertyInfo[] properties = objectFrom.GetType().GetProperties();
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					if (!(propertyInfo != null))
					{
						continue;
					}
					object value = propertyInfo.GetValue(objectFrom);
					if (value != null)
					{
						PropertyInfo property = objectTo.GetType().GetProperty(propertyInfo.Name);
						if (property != null)
						{
							property.SetValue(objectTo, value);
						}
					}
				}
			}
			return objectTo;
		}

		[HttpPost]
		[Authorize(Roles = "User")]
		public async Task<ActionResult<dm_BangLuong>> PostReport(v_dm_BangLuong Report)
		{
			try
			{
				if (ReportExistsMA(Report.MA))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Report.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				dm_BangLuong newdm_BangLuong = ConvertobjectToct_dm_BangLuong(objectTo: new dm_BangLuong(), objectFrom: Report);
				_context.dm_BangLuong.Add(newdm_BangLuong);
				foreach (v_dm_BangLuong_ChiTiet itm in Report.lstdm_BangLuong_ChiTiet)
				{
					dm_BangLuong_ChiTiet newct_PhieuXuat_CT = new dm_BangLuong_ChiTiet();
					newct_PhieuXuat_CT = ConvertobjectToct_dm_BangLuong_ChiTiet(itm, newct_PhieuXuat_CT);
					newct_PhieuXuat_CT.ID = Guid.NewGuid().ToString();
					newct_PhieuXuat_CT.ID_BANGLUONG = Report.ID;
					_context.dm_BangLuong_ChiTiet.Add(newct_PhieuXuat_CT);
				}
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
				view_dm_BangLuong OKReport = await _context.view_dm_BangLuong.FirstOrDefaultAsync((view_dm_BangLuong e) => e.ID == Report.ID);
				if (OKReport != null)
				{
					OKReport.lstdm_BangLuong_ChiTiet = new List<v_dm_BangLuong_ChiTiet>();
					List<dm_BangLuong_ChiTiet> lstdm_BangLuong_ChiTiet = await _context.dm_BangLuong_ChiTiet.Where((dm_BangLuong_ChiTiet e) => e.ID_BANGLUONG == Report.ID).ToListAsync();
					if (lstdm_BangLuong_ChiTiet != null)
					{
						string json = JsonConvert.SerializeObject(lstdm_BangLuong_ChiTiet);
						OKReport.lstdm_BangLuong_ChiTiet = JsonConvert.DeserializeObject<List<v_dm_BangLuong_ChiTiet>>(json);
					}
				}
				if (OKReport == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + Report.ID + " dữ liệu!",
						Data = ""
					});
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKReport
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
		public async Task<IActionResult> DeleteReport(string ID)
		{
			try
			{
				dm_BangLuong Report = await _context.dm_BangLuong.FirstOrDefaultAsync((dm_BangLuong e) => e.ID == ID);
				if (Report == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + ID + " dữ liệu!",
						Data = ""
					});
				}
				ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
				ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete(Report, Report.ID, Report.MA);
				if (!apiResponse.Success)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = apiResponse.Message,
						Data = ""
					});
				}
				List<dm_BangLuong_ChiTiet> lstweb_PhanQuyenSanPham = await _context.dm_BangLuong_ChiTiet.Where((dm_BangLuong_ChiTiet e) => e.ID_BANGLUONG == ID).ToListAsync();
				if (lstweb_PhanQuyenSanPham != null)
				{
					foreach (dm_BangLuong_ChiTiet itm in lstweb_PhanQuyenSanPham)
					{
						_context.dm_BangLuong_ChiTiet.Remove(itm);
					}
				}
				_context.dm_BangLuong.Remove(Report);
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

		private bool ReportExistsMA(string MA)
		{
			return _context.dm_BangLuong.Any((dm_BangLuong e) => e.MA == MA);
		}

		private bool ReportExistsID(string ID)
		{
			return _context.dm_BangLuong.Any((dm_BangLuong e) => e.ID == ID);
		}
	}
}
