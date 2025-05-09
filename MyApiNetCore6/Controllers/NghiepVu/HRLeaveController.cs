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
using MyApiNetCore6.Models;
using Microsoft.VisualBasic;

namespace MyApiNetCore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HRLeaveController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public HRLeaveController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
        }
        [HttpGet("{LOC_ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetArea(string LOC_ID)
        {
            try
            {

                var lstValue = await _context.nv_NghiPhep!.Where(e => e.LOC_ID == LOC_ID).OrderBy(e => e.THOIGIANVAO).ToListAsync();
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

        // GET: api/Area
        [HttpGet("{LOC_ID}/{Type}/{KeyWhere}/{ValuesSearch}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetArea(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
        {
            try
            {
                ValuesSearch = ValuesSearch.Replace("%2f", "/");
                var lstValue = await _context.nv_NghiPhep!.Where(e => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).OrderBy(e => e.THOIGIANVAO).ToListAsync();
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


        //GET: api/Area/5
        [HttpGet("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetArea(string LOC_ID, string ID)
        {
            try
            {
                var Area = await _context.nv_NghiPhep!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);

                if (Area == null)
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
                    Data = Area
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

        // PUT: api/Area/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PutArea(string LOC_ID, string ID, nv_NghiPhep Area)
        {
            try
            {
                if (!AreaExistsID(LOC_ID, Area.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + Area.ID + " dữ liệu!",
                        Data = ""
                    });
                }
                var nv_NghiPhepOld = await _context.nv_NghiPhep!.AsNoTracking().FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID);
                using var transaction = _context.Database.BeginTransaction();
                {
                    if(nv_NghiPhepOld != null)
                    {
                        if (nv_NghiPhepOld.ID_PHEPNAM != Area.ID_PHEPNAM)
                        {
                            var nv_PhepNamOld = await _context.nv_PhepNam!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.ID == nv_NghiPhepOld.ID_PHEPNAM);
                            if (nv_PhepNamOld == null)
                            {
                                return Ok(new ApiResponse
                                {
                                    Success = false,
                                    Message = "Không tìm thấy " + nv_NghiPhepOld.LOC_ID + "-" + nv_NghiPhepOld.ID + " dữ liệu phép năm!",
                                    Data = ""
                                });
                            }
                            else
                            {
                                nv_PhepNamOld.SONGAYPHEPDADUNG += nv_NghiPhepOld.SOLUONG;
                                _context.Entry(nv_PhepNamOld).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            }
                            var nv_PhepNam = await _context.nv_PhepNam!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID_PHEPNAM);
                            if (nv_PhepNam == null)
                            {
                                return Ok(new ApiResponse
                                {
                                    Success = false,
                                    Message = "Không tìm thấy " + Area.LOC_ID + "-" + Area.ID_PHEPNAM + " dữ liệu phép năm!",
                                    Data = ""
                                });
                            }
                            else
                            {
                                if (Area.ISNGHIPHEP && (nv_PhepNam.SONGAYPHEP - nv_PhepNam.SONGAYPHEPDADUNG < Area.SOLUONG))
                                {
                                    return Ok(new ApiResponse
                                    {
                                        Success = false,
                                        Message = "Số lượng ngày còn lại phép không đủ!" + (nv_PhepNam.SONGAYPHEP - nv_PhepNam.SONGAYPHEPDADUNG).ToString() + " ngày",
                                        Data = ""
                                    });
                                }
                                if (Area.ISNGHIPHEP && (nv_PhepNam.NGAYBATDAU >= Area.THOIGIANVAO && nv_PhepNam.NGAYKETTHUC >= Area.THOIGIANRA))
                                {
                                    return Ok(new ApiResponse
                                    {
                                        Success = false,
                                        Message = "Ngày bắt đầu và ngày kết thúc không nằm trong khoảng thời gian cho tạo phép!",
                                        Data = ""
                                    });
                                }
                                nv_PhepNam.SONGAYPHEPDADUNG -= nv_NghiPhepOld.SOLUONG;
                                _context.Entry(nv_PhepNam).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            }
                        }
                        else
                        {
                            var nv_PhepNam = await _context.nv_PhepNam!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID_PHEPNAM);
                            if (nv_PhepNam == null)
                            {
                                return Ok(new ApiResponse
                                {
                                    Success = false,
                                    Message = "Không tìm thấy " + Area.LOC_ID + "-" + Area.ID_PHEPNAM + " dữ liệu phép năm!",
                                    Data = ""
                                });
                            }
                            else
                            {
                                nv_PhepNam.SONGAYPHEPDADUNG -= nv_NghiPhepOld.SOLUONG;
                                if (Area.ISNGHIPHEP && (nv_PhepNam.SONGAYPHEP - nv_PhepNam.SONGAYPHEPDADUNG < Area.SOLUONG))
                                {
                                    return Ok(new ApiResponse
                                    {
                                        Success = false,
                                        Message = "Số lượng ngày còn lại phép không đủ!" + (nv_PhepNam.SONGAYPHEP - nv_PhepNam.SONGAYPHEPDADUNG).ToString() + " ngày",
                                        Data = ""
                                    });
                                }
                                if (Area.ISNGHIPHEP && (nv_PhepNam.NGAYBATDAU >= Area.THOIGIANVAO && nv_PhepNam.NGAYKETTHUC >= Area.THOIGIANRA))
                                {
                                    return Ok(new ApiResponse
                                    {
                                        Success = false,
                                        Message = "Ngày bắt đầu và ngày kết thúc không nằm trong khoảng thời gian cho tạo phép!",
                                        Data = ""
                                    });
                                }
                                
                                nv_PhepNam.SONGAYPHEPDADUNG += Area.SOLUONG;
                                _context.Entry(nv_PhepNam).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            }
                        }
                    }    
                    

                    _context.Entry(Area).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
                transaction.Commit();
                var OKArea = await _context.nv_NghiPhep!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = OKArea
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

        // POST: api/Area
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<nv_NghiPhep>> PostArea(nv_NghiPhep Area)
        {
            try
            {
                if (AreaExistsID(Area.LOC_ID, Area.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại " + Area.LOC_ID + "-" + Area.ID + " trong dữ liệu!",
                        Data = ""
                    });
                }
                int DayInterval = 0;
                DateTime StartDate = Area.THOIGIANVAO;
                while (Area.THOIGIANVAO.AddDays(DayInterval) <= Area.THOIGIANRA)
                {
                    StartDate = StartDate.AddDays(DayInterval);
                    var nv_NghiPhep = await _context.nv_NghiPhep!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.THOIGIANVAO.Date >= StartDate.Date && e.THOIGIANRA.Date <= StartDate.Date && e.ID_NHANVIEN == Area.ID_NHANVIEN);
                    if (nv_NghiPhep != null)
                    {
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Đã tồn tại " + nv_NghiPhep.LOC_ID + "-" + nv_NghiPhep.ID + " trong dữ liệu!(Đã có đơn nghĩ phép ngày " + StartDate.ToString("dd/MM/yyyy") + " )",
                            Data = ""
                        });
                    }
                    DayInterval += 1;
                }
                using var transaction = _context.Database.BeginTransaction();
                {
                    var nv_PhepNam = await _context.nv_PhepNam!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID_PHEPNAM);
                    if (nv_PhepNam != null)
                    {
                        if (Area.ISNGHIPHEP && (nv_PhepNam.SONGAYPHEP - nv_PhepNam.SONGAYPHEPDADUNG < Area.SOLUONG))
                        {
                            return Ok(new ApiResponse
                            {
                                Success = false,
                                Message = "Số lượng ngày còn lại phép không đủ!" + (nv_PhepNam.SONGAYPHEP - nv_PhepNam.SONGAYPHEPDADUNG).ToString() + " ngày",
                                Data = ""
                            });
                        }
                        if (Area.ISNGHIPHEP && (nv_PhepNam.NGAYBATDAU <= Area.THOIGIANVAO && nv_PhepNam.NGAYKETTHUC <= Area.THOIGIANRA))
                        {
                            return Ok(new ApiResponse
                            {
                                Success = false,
                                Message = "Ngày bắt đầu và ngày kết thúc không nằm trong khoảng thời gian cho tạo phép!",
                                Data = ""
                            });
                        }
                        if (Area.ISNGHIPHEP)
                            nv_PhepNam.SONGAYPHEPDADUNG += Area.SOLUONG;
                        _context.Entry(nv_PhepNam).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    }
                    else
                    {
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Không tìm thấy " + Area.LOC_ID + "-" + Area.ID + " dữ liệu phép năm!",
                            Data = ""
                        });
                    }
                    _context.nv_NghiPhep!.Add(Area);
                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
                transaction.Commit();
                var OKArea = await _context.nv_NghiPhep!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = OKArea
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

        // DELETE: api/Area/5
        [HttpDelete("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeleteArea(string LOC_ID, string ID)
        {
            try
            {
               
                var Area = await _context.nv_NghiPhep!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (Area == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                using var transaction = _context.Database.BeginTransaction();
                {
                    var nv_PhepNam = await _context.nv_PhepNam!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID_PHEPNAM);
                    if (nv_PhepNam != null)
                    {
                        if (Area.ISNGHIPHEP)
                            nv_PhepNam.SONGAYPHEPDADUNG -= Area.SOLUONG;
                        _context.Entry(nv_PhepNam).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    }
                    else
                    {
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu phép năm!",
                            Data = ""
                        });
                    }
                    _context.nv_NghiPhep!.Remove(Area);
                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
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
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                });
            }
        }

        private bool AreaExistsID(string LOC_ID, string ID)
        {
            return _context.nv_NghiPhep!.Any(e => e.LOC_ID == LOC_ID && e.ID == ID);
        }
    }
}