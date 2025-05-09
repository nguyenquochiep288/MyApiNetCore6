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

namespace MyApiNetCore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimekeepingController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public TimekeepingController(dbTrangHiepPhatContext context, IConfiguration configuration)
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

                var lstValue = await _context.nv_ChamCong!.Where(e => e.LOC_ID == LOC_ID).OrderBy(e => e.NGAYCONG).ToListAsync();
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
                var lstValue = await _context.nv_ChamCong!.Where(e => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).OrderBy(e => e.NGAYCONG).ToListAsync();
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
                var Area = await _context.nv_ChamCong!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);

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
        public async Task<IActionResult> PutArea(string LOC_ID, string ID, nv_ChamCong Area)
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
                _context.Entry(Area).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                var OKArea = await _context.nv_ChamCong!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID);
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

        // POST: api/Area/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostCheckIn")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PostCheckIn(nv_ChamCong Area)
        {
            try
            {
                var nv_ChamCong = await _context.nv_ChamCong!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.NGAYCONG.Date == Area.NGAYCONG.Date && e.ID_NHANVIEN == Area.ID_NHANVIEN);
                if (nv_ChamCong != null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại " + nv_ChamCong.LOC_ID + "-" + nv_ChamCong.ID + " trong dữ liệu!(Đã có dữ liệu chấm công ngày " + Area.NGAYCONG.ToString("dd/MM/yyyy") + " )",
                        Data = ""
                    });
                }

                _context.nv_ChamCong!.Add(Area);
                AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                var OKArea = await _context.nv_ChamCong!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID);
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

        // Post: api/Area/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostCheckOut")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PostCheckOut(nv_ChamCong Area)
        {
            try
            {
                if (!AreaExistsID(Area.LOC_ID, Area.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + Area.LOC_ID + "-" + Area.ID + " dữ liệu!",
                        Data = ""
                    });
                }
                
                var nv_ChamCong = await _context.nv_ChamCong!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID);
                if(nv_ChamCong != null)
                {
                    if (nv_ChamCong.ID_NHANVIEN != Area.ID_NHANVIEN)
                    {
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Dữ liệu chấm công khác nhau!",
                            Data = ""
                        });
                    }
                    nv_ChamCong.ID_NGUOISUA = Area.ID_NGUOISUA;
                    nv_ChamCong.THOIGIANSUA = Area.THOIGIANSUA;
                    nv_ChamCong.THOIGIANRA = Area.THOIGIANRA;
                    nv_ChamCong.IP_CHAMCONGRA = Area.IP_CHAMCONGRA;
                    string GHICHU = nv_ChamCong.GHICHU + Area.GHICHU;
                    if(GHICHU.Length > 250)
                        nv_ChamCong.GHICHU = GHICHU.Substring(0, 249);
                    else
                        nv_ChamCong.GHICHU = GHICHU;
                }
                _context.Entry(nv_ChamCong).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                var OKArea = await _context.nv_ChamCong!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID);
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
        public async Task<ActionResult<nv_ChamCong>> PostArea(nv_ChamCong Area)
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
                var nv_ChamCong = await _context.nv_ChamCong!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.NGAYCONG.Date == Area.NGAYCONG.Date && e.ID_NHANVIEN == Area.ID_NHANVIEN);
                if (nv_ChamCong != null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại " + nv_ChamCong.LOC_ID + "-" + nv_ChamCong.ID + " trong dữ liệu!(Đã có dữ liệu chấm công ngày " + Area.NGAYCONG.ToString("dd/MM/yyyy") + " )",
                        Data = ""
                    });
                }
                _context.nv_ChamCong!.Add(Area);
                AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                var OKArea = await _context.nv_ChamCong!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID);
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
               
                var Area = await _context.nv_ChamCong!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (Area == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                _context.nv_ChamCong!.Remove(Area);
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

        private bool AreaExistsID(string LOC_ID, string ID)
        {
            return _context.nv_ChamCong!.Any(e => e.LOC_ID == LOC_ID && e.ID == ID);
        }
    }
}