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
	public class ReportController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public ReportController(dbTrangHiepPhatContext context, IConfiguration configuration)
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
				List<view_web_Report> lstValue = await _context.view_web_Report.ToListAsync();
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
				List<view_web_Report> lstValue = await _context.view_web_Report.Where(KeyWhere, ValuesSearch).ToListAsync();
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
				view_web_Report Report = await _context.view_web_Report.FirstOrDefaultAsync((view_web_Report e) => e.ID == ID);
				if (Report != null)
				{
					Report.lstweb_Report_Parameter = new List<v_web_Report_Parameter>();
					List<view_web_Report_Parameter> lstweb_Report_Parameter = await _context.view_web_Report_Parameter.Where((view_web_Report_Parameter e) => e.ID_REPORT == ID).ToListAsync();
					if (lstweb_Report_Parameter != null)
					{
						string json = JsonConvert.SerializeObject(lstweb_Report_Parameter);
						Report.lstweb_Report_Parameter = JsonConvert.DeserializeObject<List<v_web_Report_Parameter>>(json);
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
		public async Task<IActionResult> PutReport(string MA, v_web_Report Report)
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
				foreach (v_web_Report_Parameter itm in Report.lstweb_Report_Parameter)
				{
					web_Report_Parameter newct_PhieuXuat_CT = ConvertobjectToct_web_Report_Parameter(objectTo: new web_Report_Parameter(), objectFrom: itm);
					if (!string.IsNullOrEmpty(itm.ID))
					{
						if (await _context.view_web_Report_Parameter.FirstOrDefaultAsync((view_web_Report_Parameter e) => e.ID == itm.ID && e.ID_REPORT == Report.ID) != null)
						{
							_context.Entry(newct_PhieuXuat_CT).State = EntityState.Modified;
							continue;
						}
						newct_PhieuXuat_CT.ID = Guid.NewGuid().ToString();
						newct_PhieuXuat_CT.ID_REPORT = Report.ID;
						_context.web_Report_Parameter.Add(newct_PhieuXuat_CT);
					}
					else
					{
						newct_PhieuXuat_CT.ID = Guid.NewGuid().ToString();
						newct_PhieuXuat_CT.ID_REPORT = Report.ID;
						_context.web_Report_Parameter.Add(newct_PhieuXuat_CT);
					}
				}
				_context.Entry(Report).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
				view_web_Report OKReport = await _context.view_web_Report.FirstOrDefaultAsync((view_web_Report e) => e.ID == Report.ID);
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

		private static web_Report_Parameter ConvertobjectToct_web_Report_Parameter<T>(T objectFrom, web_Report_Parameter objectTo)
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
		public async Task<ActionResult<web_Report>> PostReport(v_web_Report Report)
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
				_context.web_Report.Add(Report);
				foreach (v_web_Report_Parameter itm in Report.lstweb_Report_Parameter)
				{
					web_Report_Parameter newct_PhieuXuat_CT = new web_Report_Parameter();
					newct_PhieuXuat_CT = ConvertobjectToct_web_Report_Parameter(itm, newct_PhieuXuat_CT);
					newct_PhieuXuat_CT.ID = Guid.NewGuid().ToString();
					newct_PhieuXuat_CT.ID_REPORT = Report.ID;
					_context.web_Report_Parameter.Add(newct_PhieuXuat_CT);
				}
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
				view_web_Report OKReport = await _context.view_web_Report.FirstOrDefaultAsync((view_web_Report e) => e.ID == Report.ID);
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
				web_Report Report = await _context.web_Report.FirstOrDefaultAsync((web_Report e) => e.ID == ID);
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
				List<web_Report_Parameter> lstweb_PhanQuyenSanPham = await _context.web_Report_Parameter.Where((web_Report_Parameter e) => e.ID_REPORT == ID).ToListAsync();
				if (lstweb_PhanQuyenSanPham != null)
				{
					foreach (web_Report_Parameter itm in lstweb_PhanQuyenSanPham)
					{
						_context.web_Report_Parameter.Remove(itm);
					}
				}
				_context.web_Report.Remove(Report);
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
			return _context.web_Report.Any((web_Report e) => e.MA == MA);
		}

		private bool ReportExistsID(string ID)
		{
			return _context.web_Report.Any((web_Report e) => e.ID == ID);
		}
	}
}
