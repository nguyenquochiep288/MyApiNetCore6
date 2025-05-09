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

namespace MyApiNetCore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public CalendarController(dbTrangHiepPhatContext context, IConfiguration configuration)
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

                var lstValue = await _context.dm_LichLamViec!.Where(e => e.LOC_ID == LOC_ID).OrderBy(e => e.MA).ToListAsync();
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
                var lstValue = await _context.dm_LichLamViec!.Where(e => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).OrderBy(e => e.MA).ToListAsync();
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
                var Area = await _context.dm_LichLamViec!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);

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
        [HttpPut("{LOC_ID}/{MA}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PutArea(string LOC_ID, string MA, dm_LichLamViec Area)
        {
            try
            {
               

                if (LOC_ID != Area.LOC_ID || Area.MA != MA)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Dữ liệu khóa không giống nhau!",
                        Data = ""
                    });
                }
                if (AreaExists(Area))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + Area.LOC_ID + "-" + Area.MA + " trong dữ liệu!",
                        Data = ""
                    });
                }
                if (!AreaExistsID(LOC_ID, Area.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + Area.ID + " dữ liệu!",
                        Data = ""
                    });
                }
                Area.GHICHU = ",";
                Area.GHICHU += Area.T2 ? "T2," : "";
                Area.GHICHU += Area.T3 ? "T3," : "";
                Area.GHICHU += Area.T4 ? "T4," : "";
                Area.GHICHU += Area.T5 ? "T5," : "";
                Area.GHICHU += Area.T6 ? "T6," : "";
                Area.GHICHU += Area.T7 ? "T7," : "";
                Area.GHICHU += Area.CN ? "CN," : "";
                _context.Entry(Area).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                var OKArea = await _context.dm_LichLamViec!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID);
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
        public async Task<ActionResult<dm_LichLamViec>> PostArea(dm_LichLamViec Area)
        {
            try
            {
                if (AreaExistsMA(Area.LOC_ID, Area.MA))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + Area.LOC_ID + "-" + Area.MA + " trong dữ liệu!",
                        Data = ""
                    });
                }
                Area.GHICHU = ",";
                Area.GHICHU += Area.T2 ? "T2," : "";
                Area.GHICHU += Area.T3 ? "T3," : "";
                Area.GHICHU += Area.T4 ? "T4," : "";
                Area.GHICHU += Area.T5 ? "T5," : "";
                Area.GHICHU += Area.T6 ? "T6," : "";
                Area.GHICHU += Area.T7 ? "T7," : "";
                Area.GHICHU += Area.CN ? "CN," : "";
                _context.dm_LichLamViec!.Add(Area);
                AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                var OKArea = await _context.dm_LichLamViec!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID);
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
               
                var Area = await _context.dm_LichLamViec!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (Area == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
                ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_LichLamViec>(Area, Area.ID, Area.MA);
                if (!apiResponse.Success)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = apiResponse.Message,
                        Data = ""
                    });
                }
                var lstweb_PhanQuyenSanPham = await _context.web_PhanQuyenKhuVuc!.Where(e => e.LOC_ID == LOC_ID && e.ID_LICHLAMVIEC == ID).ToListAsync();
                if (lstweb_PhanQuyenSanPham != null)
                {
                    foreach (var itm in lstweb_PhanQuyenSanPham)
                    {
                        _context.web_PhanQuyenKhuVuc!.Remove(itm);
                    }
                }
                _context.dm_LichLamViec!.Remove(Area);
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

        private bool AreaExistsMA(string LOC_ID, string MA)
        {
            return _context.dm_LichLamViec!.Any(e => e.LOC_ID == LOC_ID && e.MA == MA);
        }

        private bool AreaExistsID(string LOC_ID, string ID)
        {
            return _context.dm_LichLamViec!.Any(e => e.LOC_ID == LOC_ID && e.ID == ID);
        }
        private bool AreaExists(dm_LichLamViec Area)
        {
            return _context.dm_LichLamViec!.Any(e => e.LOC_ID == Area.LOC_ID && e.MA == Area.MA && e.ID != Area.ID);
        }
    }
}