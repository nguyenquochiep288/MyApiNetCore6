using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using DatabaseTHP;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using MyApiNetCore6.Data;
using Newtonsoft.Json.Linq;
using NuGet.Common;

using DatabaseTHP.Class;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using System.Security.Permissions;
using System.Reflection;

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
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetReport()
        {
            try
            {

                var lstValue = await _context.view_web_Report!.ToListAsync();
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = lstValue
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                });
            }

        }

        // GET: api/Report
        [HttpGet("{Type}/{KeyWhere}/{ValuesSearch}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetReport(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
        {
            try
            {
                ValuesSearch = ValuesSearch.Replace("%2f", "/");
                var lstValue = await _context.view_web_Report!.Where(KeyWhere, ValuesSearch).ToListAsync();
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = lstValue
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                });
            }
        }


        //GET: api/Report/5
        [HttpGet("{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetReport(string ID)
        {
            try
            {
                var Report = await _context.view_web_Report!.FirstOrDefaultAsync(e => e.ID == ID);
                if (Report != null)
                {
                    Report.lstweb_Report_Parameter = new List<v_web_Report_Parameter>();

                   var lstweb_Report_Parameter = await _context.view_web_Report_Parameter!.Where(e => e.ID_REPORT == ID).ToListAsync();
                    if(lstweb_Report_Parameter != null)
                    {
                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(lstweb_Report_Parameter);
                        Report.lstweb_Report_Parameter = Newtonsoft.Json.JsonConvert.DeserializeObject<List<v_web_Report_Parameter>>(json);
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
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                });
            }

        }

        // PUT: api/Report/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{MA}")]
        [Authorize(Roles = UserRoles.User)]
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
                using var transaction = _context.Database.BeginTransaction();
                {
                    foreach (var itm in Report.lstweb_Report_Parameter)
                    {
                        web_Report_Parameter newct_PhieuXuat_CT = new web_Report_Parameter();
                        newct_PhieuXuat_CT = ConvertobjectToct_web_Report_Parameter(itm, newct_PhieuXuat_CT);
                        if (!string.IsNullOrEmpty(itm.ID))
                        {
                            var Report_Parameter = await _context.view_web_Report_Parameter!.FirstOrDefaultAsync(e => e.ID == itm.ID && e.ID_REPORT == Report.ID);
                            if (Report_Parameter != null)
                            {
                                _context.Entry(newct_PhieuXuat_CT).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            }
                            else
                            {
                                newct_PhieuXuat_CT.ID = Guid.NewGuid().ToString();
                                newct_PhieuXuat_CT.ID_REPORT = Report.ID;
                                _context.web_Report_Parameter!.Add(newct_PhieuXuat_CT);
                            }
                        }
                        else
                        {
                            newct_PhieuXuat_CT.ID = Guid.NewGuid().ToString();
                            newct_PhieuXuat_CT.ID_REPORT = Report.ID;
                            _context.web_Report_Parameter!.Add(newct_PhieuXuat_CT);
                            
                        }
                    }

                    _context.Entry(Report).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
                transaction.Commit();
                var OKReport = await _context.view_web_Report!.FirstOrDefaultAsync(e => e.ID == Report.ID);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = OKReport
                });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                });
            }
        }

        private static web_Report_Parameter ConvertobjectToct_web_Report_Parameter<T>(T objectFrom, web_Report_Parameter objectTo)
        {
            if (objectFrom != null)
            {
                var properties = objectFrom.GetType().GetProperties();
                foreach (PropertyInfo itmPropertyInfo in properties)
                {
                    if (itmPropertyInfo != null)
                    {
                        var val = itmPropertyInfo.GetValue(objectFrom);
                        if (val != null)
                        {
                            var piShared = objectTo.GetType().GetProperty(itmPropertyInfo.Name);
                            if (piShared != null)
                                piShared.SetValue(objectTo, val);
                        }
                    }
                }
            }

            return objectTo;
        }
        // POST: api/Report
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
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
                using var transaction = _context.Database.BeginTransaction();
                {
                    _context.web_Report!.Add(Report);
                    foreach (var itm in Report.lstweb_Report_Parameter)
                    {
                        web_Report_Parameter newct_PhieuXuat_CT = new web_Report_Parameter();
                        newct_PhieuXuat_CT = ConvertobjectToct_web_Report_Parameter(itm, newct_PhieuXuat_CT);
                        newct_PhieuXuat_CT.ID = Guid.NewGuid().ToString();
                        newct_PhieuXuat_CT.ID_REPORT = Report.ID;
                        _context.web_Report_Parameter!.Add(newct_PhieuXuat_CT);
                    }

                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
                transaction.Commit();
                var OKReport = await _context.view_web_Report!.FirstOrDefaultAsync(e => e.ID == Report.ID);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = OKReport
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                });
            }
        }

        // DELETE: api/Report/5
        [HttpDelete("{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeleteReport(string ID)
        {
            try
            {
               
                var Report = await _context.web_Report!.FirstOrDefaultAsync(e => e.ID == ID);
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
                ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<web_Report>(Report, Report.ID, Report.MA);
                if (!apiResponse.Success)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = apiResponse.Message,
                        Data = ""
                    });
                }

                var lstweb_PhanQuyenSanPham = await _context.web_Report_Parameter!.Where(e => e.ID_REPORT == ID).ToListAsync();
                if (lstweb_PhanQuyenSanPham != null)
                {
                    foreach (var itm in lstweb_PhanQuyenSanPham)
                    {
                        _context.web_Report_Parameter!.Remove(itm);
                    }
                }
                _context.web_Report!.Remove(Report);
                AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = ""
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                });
            }
        }

        private bool ReportExistsMA(string MA)
        {
            return _context.web_Report!.Any(e => e.MA == MA);
        }

        private bool ReportExistsID(string ID)
        {
            return _context.web_Report!.Any(e => e.ID == ID);
        }

    }
}