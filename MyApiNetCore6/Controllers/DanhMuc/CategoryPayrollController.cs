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
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetReport()
        {
            try
            {

                var lstValue = await _context.view_dm_BangLuong!.ToListAsync();
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
                var lstValue = await _context.view_dm_BangLuong!.Where(KeyWhere, ValuesSearch).ToListAsync();
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
                var Report = await _context.view_dm_BangLuong!.FirstOrDefaultAsync(e => e.ID == ID);
                if (Report != null)
                {
                    Report.lstdm_BangLuong_ChiTiet = new List<v_dm_BangLuong_ChiTiet>();

                   var lstdm_BangLuong_ChiTiet = await _context.dm_BangLuong_ChiTiet!.Where(e => e.ID_BANGLUONG == ID).ToListAsync();
                    if(lstdm_BangLuong_ChiTiet != null)
                    {
                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(lstdm_BangLuong_ChiTiet);
                        Report.lstdm_BangLuong_ChiTiet = Newtonsoft.Json.JsonConvert.DeserializeObject<List<v_dm_BangLuong_ChiTiet>>(json);
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
                using var transaction = _context.Database.BeginTransaction();
                {
                    var lstdm_BangLuong_ChiTiet = await _context.dm_BangLuong_ChiTiet!.AsNoTracking().Where(e => e.ID_BANGLUONG == Report.ID).ToListAsync();
                    foreach (var dm_BangLuong_ChiTiet in lstdm_BangLuong_ChiTiet)
                    {
                        if (Report.lstdm_BangLuong_ChiTiet.Where(s => s.ID == dm_BangLuong_ChiTiet.ID).Count() == 0)
                                _context.dm_BangLuong_ChiTiet!.Remove(dm_BangLuong_ChiTiet);
                    }

                    foreach (var itm in Report.lstdm_BangLuong_ChiTiet)
                    {
                        
                        dm_BangLuong_ChiTiet newct_PhieuXuat_CT = new dm_BangLuong_ChiTiet();
                        newct_PhieuXuat_CT = ConvertobjectToct_dm_BangLuong_ChiTiet(itm, newct_PhieuXuat_CT);
                        if (!string.IsNullOrEmpty(itm.ID))
                        {
                            var Report_Parameter = await _context.dm_BangLuong_ChiTiet!.AsNoTracking().FirstOrDefaultAsync(e => e.ID == itm.ID && e.ID_BANGLUONG == Report.ID);
                            if (Report_Parameter != null)
                            {
                                _context.Entry(newct_PhieuXuat_CT).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            }
                            else
                            {
                                newct_PhieuXuat_CT.ID = Guid.NewGuid().ToString();
                                newct_PhieuXuat_CT.ID_BANGLUONG = Report.ID;
                                _context.dm_BangLuong_ChiTiet!.Add(newct_PhieuXuat_CT);
                            }
                        }
                        else
                        {
                            newct_PhieuXuat_CT.ID = Guid.NewGuid().ToString();
                            newct_PhieuXuat_CT.ID_BANGLUONG = Report.ID;
                            _context.dm_BangLuong_ChiTiet!.Add(newct_PhieuXuat_CT);
                        }
                    }
                    dm_BangLuong newdm_BangLuong = new dm_BangLuong();
                    newdm_BangLuong = ConvertobjectToct_dm_BangLuong(Report, newdm_BangLuong);
                    _context.dm_BangLuong!.Add(newdm_BangLuong);
                    _context.Entry(newdm_BangLuong).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
                transaction.Commit();
                var OKReport = await _context.view_dm_BangLuong!.FirstOrDefaultAsync(e => e.ID == Report.ID);
                if (OKReport != null)
                {
                    OKReport.lstdm_BangLuong_ChiTiet = new List<v_dm_BangLuong_ChiTiet>();

                    var lstdm_BangLuong_ChiTiet = await _context.dm_BangLuong_ChiTiet!.Where(e => e.ID_BANGLUONG == Report.ID).ToListAsync();
                    if (lstdm_BangLuong_ChiTiet != null)
                    {
                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(lstdm_BangLuong_ChiTiet);
                        OKReport.lstdm_BangLuong_ChiTiet = Newtonsoft.Json.JsonConvert.DeserializeObject<List<v_dm_BangLuong_ChiTiet>>(json);
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
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                });
            }
        }

        private static dm_BangLuong_ChiTiet ConvertobjectToct_dm_BangLuong_ChiTiet<T>(T objectFrom, dm_BangLuong_ChiTiet objectTo)
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

        private static dm_BangLuong ConvertobjectToct_dm_BangLuong<T>(T objectFrom, dm_BangLuong objectTo)
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
                using var transaction = _context.Database.BeginTransaction();
                {
                    dm_BangLuong newdm_BangLuong = new dm_BangLuong();
                    newdm_BangLuong = ConvertobjectToct_dm_BangLuong(Report, newdm_BangLuong);
                    _context.dm_BangLuong!.Add(newdm_BangLuong);
                    foreach (var itm in Report.lstdm_BangLuong_ChiTiet)
                    {
                        dm_BangLuong_ChiTiet newct_PhieuXuat_CT = new dm_BangLuong_ChiTiet();
                        newct_PhieuXuat_CT = ConvertobjectToct_dm_BangLuong_ChiTiet(itm, newct_PhieuXuat_CT);
                        newct_PhieuXuat_CT.ID = Guid.NewGuid().ToString();
                        newct_PhieuXuat_CT.ID_BANGLUONG = Report.ID;
                        _context.dm_BangLuong_ChiTiet!.Add(newct_PhieuXuat_CT);
                    }

                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
                transaction.Commit();
                var OKReport = await _context.view_dm_BangLuong!.FirstOrDefaultAsync(e => e.ID == Report.ID);
                if (OKReport != null)
                {
                    OKReport.lstdm_BangLuong_ChiTiet = new List<v_dm_BangLuong_ChiTiet>();

                    var lstdm_BangLuong_ChiTiet = await _context.dm_BangLuong_ChiTiet!.Where(e => e.ID_BANGLUONG == Report.ID).ToListAsync();
                    if (lstdm_BangLuong_ChiTiet != null)
                    {
                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(lstdm_BangLuong_ChiTiet);
                        OKReport.lstdm_BangLuong_ChiTiet = Newtonsoft.Json.JsonConvert.DeserializeObject<List<v_dm_BangLuong_ChiTiet>>(json);
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
               
                var Report = await _context.dm_BangLuong!.FirstOrDefaultAsync(e => e.ID == ID);
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
                ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_BangLuong>(Report, Report.ID, Report.MA);
                if (!apiResponse.Success)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = apiResponse.Message,
                        Data = ""
                    });
                }

                var lstweb_PhanQuyenSanPham = await _context.dm_BangLuong_ChiTiet!.Where(e => e.ID_BANGLUONG == ID).ToListAsync();
                if (lstweb_PhanQuyenSanPham != null)
                {
                    foreach (var itm in lstweb_PhanQuyenSanPham)
                    {
                        _context.dm_BangLuong_ChiTiet!.Remove(itm);
                    }
                }
                _context.dm_BangLuong!.Remove(Report);
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
            return _context.dm_BangLuong!.Any(e => e.MA == MA);
        }

        private bool ReportExistsID(string ID)
        {
            return _context.dm_BangLuong!.Any(e => e.ID == ID);
        }

    }
}