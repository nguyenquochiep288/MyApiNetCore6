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
    public class AnnualLeaveController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public AnnualLeaveController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
        }
        [HttpGet("{LOC_ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetAnnualLeave(string LOC_ID)
        {
            try
            {

                var lstValue = await _context.view_nv_PhepNam!.Where(e => e.LOC_ID == LOC_ID).OrderBy(e => e.NAM).ToListAsync();
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

        // GET: api/AnnualLeave
        [HttpGet("{LOC_ID}/{Type}/{KeyWhere}/{ValuesSearch}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetAnnualLeave(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
        {
            try
            {
                ValuesSearch = ValuesSearch.Replace("%2f", "/");
                var lstValue = await _context.view_nv_PhepNam!.Where(e => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).OrderBy(e => e.NAM).ToListAsync();
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


        //GET: api/AnnualLeave/5
        [HttpGet("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetAnnualLeave(string LOC_ID, string ID)
        {
            try
            {
                var AnnualLeave = await _context.nv_PhepNam!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);

                if (AnnualLeave == null)
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
                    Data = AnnualLeave
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

        // PUT: api/AnnualLeave/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{LOC_ID}/{ID_NHANVIEN}/{NAM}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PutAnnualLeave(string LOC_ID,string ID_NHANVIEN, string NAM, nv_PhepNam AnnualLeave)
        {
            try
            {
                if (LOC_ID != AnnualLeave.LOC_ID || AnnualLeave.ID_NHANVIEN != ID_NHANVIEN || AnnualLeave.NAM.ToString() != NAM)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Dữ liệu khóa không giống nhau!",
                        Data = ""
                    });
                }
                if (AnnualLeaveExists(AnnualLeave))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + AnnualLeave.LOC_ID + "-" + AnnualLeave.NAM.ToString() + " trong dữ liệu!",
                        Data = ""
                    });
                }
                if (!AnnualLeaveExistsID(LOC_ID, AnnualLeave.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + AnnualLeave.ID + " dữ liệu!",
                        Data = ""
                    });
                }
                _context.Entry(AnnualLeave).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                var OKAnnualLeave = await _context.view_nv_PhepNam!.FirstOrDefaultAsync(e => e.LOC_ID == AnnualLeave.LOC_ID && e.ID == AnnualLeave.ID);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = OKAnnualLeave
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

        // POST: api/AnnualLeave
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<nv_PhepNam>> PostAnnualLeave(v_nv_PhepNam AnnualLeave)
        {
            try
            {
                if (AnnualLeaveExistsMA(AnnualLeave.LOC_ID, AnnualLeave.NAM, AnnualLeave.ID_NHANVIEN))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + AnnualLeave.LOC_ID + "-" + AnnualLeave.NAM + " trong dữ liệu!",
                        Data = ""
                    });
                }
                if(AnnualLeave.ISALL)
                {
                    var lstValue = await _context.AspNetUsers!.Where(e => e.LockoutEnabled).ToListAsync();
                    foreach (var itm in lstValue)
                    {
                        var TaiKhoan = await _context.dm_NhanVien!.FirstOrDefaultAsync(e => e.LOC_ID == AnnualLeave.LOC_ID && e.ID_TAIKHOAN == itm.ID);
                        if (!AnnualLeaveExistsMA(AnnualLeave.LOC_ID, AnnualLeave.NAM, itm.ID) && TaiKhoan != null)
                        {
                             nv_PhepNam b = new nv_PhepNam();
                            b.ID = Guid.NewGuid().ToString();
                            b.LOC_ID = AnnualLeave.LOC_ID;
                            b.NAM = AnnualLeave.NAM;
                            b.ID_NHANVIEN = itm.ID;
                            b.SONGAYPHEP = TaiKhoan.SONGAYPHEP > 0 ? TaiKhoan.SONGAYPHEP : AnnualLeave.SONGAYPHEP;
                            b.SONGAYPHEPDADUNG = AnnualLeave.SONGAYPHEPDADUNG;
                            b.NGAYBATDAU = AnnualLeave.NGAYBATDAU;
                            b.NGAYKETTHUC = AnnualLeave.NGAYKETTHUC;
                            b.ID_NGUOITAO = AnnualLeave.ID_NGUOITAO;
                            b.THOIGIANTHEM = AnnualLeave.THOIGIANTHEM;
                            _context.nv_PhepNam!.Add(b);
                        }
                    }
                }
                else
                    _context.nv_PhepNam!.Add(AnnualLeave);
                AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                var OKAnnualLeave = await _context.view_nv_PhepNam!.FirstOrDefaultAsync(e => e.LOC_ID == AnnualLeave.LOC_ID && e.ID == AnnualLeave.ID);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = OKAnnualLeave
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

        // DELETE: api/AnnualLeave/5
        [HttpDelete("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeleteAnnualLeave(string LOC_ID, string ID)
        {
            try
            {
               
                var AnnualLeave = await _context.nv_PhepNam!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (AnnualLeave == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                _context.nv_PhepNam!.Remove(AnnualLeave);
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

        private bool AnnualLeaveExistsMA(string LOC_ID, double NAM , string ID_NHANVIEN)
        {
            return _context.nv_PhepNam!.Any(e => e.LOC_ID == LOC_ID && e.NAM == NAM && e.ID_NHANVIEN == ID_NHANVIEN);
        }

        private bool AnnualLeaveExistsID(string LOC_ID, string ID)
        {
            return _context.nv_PhepNam!.Any(e => e.LOC_ID == LOC_ID && e.ID == ID);
        }
        private bool AnnualLeaveExists(nv_PhepNam AnnualLeave)
        {
            return _context.nv_PhepNam!.Any(e => e.LOC_ID == AnnualLeave.LOC_ID && e.NAM == AnnualLeave.NAM && e.ID_NHANVIEN == AnnualLeave.ID_NHANVIEN && e.ID != AnnualLeave.ID);
        }
    }
}