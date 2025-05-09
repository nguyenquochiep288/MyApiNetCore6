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

namespace MyApiNetCore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupCustomerController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public GroupCustomerController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
        }
        [HttpGet("{LOC_ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetGroupCusVen(string LOC_ID)
        {
            try
            {

                var lstValue = await _context.dm_NhomKhachHang!.Where(e => e.LOC_ID == LOC_ID).OrderBy(e => e.MA).ToListAsync();
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

        // GET: api/GroupCusVen
        [HttpGet("{LOC_ID}/{Type}/{KeyWhere}/{ValuesSearch}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetGroupCusVen(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
        {
            try
            {
                ValuesSearch = ValuesSearch.Replace("%2f", "/");
                var lstValue = await _context.dm_NhomKhachHang!.Where(e => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).OrderBy(e => e.MA).ToListAsync();
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


        //GET: api/GroupCusVen/5
        [HttpGet("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetGroupCusVen(string LOC_ID, string ID)
        {
            try
            {
                var GroupCusVen = await _context.dm_NhomKhachHang!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);

                if (GroupCusVen == null)
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
                    Data = GroupCusVen
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

        // PUT: api/GroupCusVen/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{LOC_ID}/{MA}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PutGroupCusVen(string LOC_ID, string MA, dm_NhomKhachHang GroupCusVen)
        {
            try
            {
                if (GroupCusVenExists(GroupCusVen))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + GroupCusVen.LOC_ID + "-" + GroupCusVen.MA + " trong dữ liệu!",
                        Data = ""
                    });
                }

                if (LOC_ID != GroupCusVen.LOC_ID || GroupCusVen.MA != MA)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Dữ liệu khóa không giống nhau!",
                        Data = ""
                    });
                }
                if (!GroupCusVenExistsID(LOC_ID, GroupCusVen.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + GroupCusVen.ID + " dữ liệu!",
                        Data = ""
                    });
                }
                _context.Entry(GroupCusVen).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = GroupCusVen
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

        // POST: api/GroupCusVen
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<dm_NhomKhachHang>> PostGroupCusVen(dm_NhomKhachHang GroupCusVen)
        {
            try
            {
                if (GroupCusVenExistsMA(GroupCusVen.LOC_ID, GroupCusVen.MA))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + GroupCusVen.LOC_ID + "-" + GroupCusVen.MA + " trong dữ liệu!",
                        Data = ""
                    });
                }
                _context.dm_NhomKhachHang!.Add(GroupCusVen);
                AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = GroupCusVen
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

        // DELETE: api/GroupCusVen/5
        [HttpDelete("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeleteGroupCusVen(string LOC_ID, string ID)
        {
            try
            {
                var GroupCusVen = await _context.dm_NhomKhachHang!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (GroupCusVen == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
                ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_NhomKhachHang>(GroupCusVen, GroupCusVen.ID, GroupCusVen.MA);
                if (!apiResponse.Success)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = apiResponse.Message,
                        Data = ""
                    });
                }
                using var transaction = _context.Database.BeginTransaction();
                {
                    var lstweb_PhanQuyenSanPham = await _context.web_PhanQuyenSanPham!.Where(e => e.LOC_ID == LOC_ID && e.ID_NHOMQUYEN == ID).ToListAsync();
                    if (lstweb_PhanQuyenSanPham != null)
                    {
                        foreach (var itm in lstweb_PhanQuyenSanPham)
                        {
                            _context.web_PhanQuyenSanPham!.Remove(itm);
                        }
                    }

                    var lstweb_PhanQuyenNhomSanPham = await _context.web_PhanQuyenNhomSanPham!.Where(e => e.LOC_ID == LOC_ID && e.ID_NHOMQUYEN == ID).ToListAsync();
                    if (lstweb_PhanQuyenNhomSanPham != null)
                    {
                        foreach (var itm in lstweb_PhanQuyenNhomSanPham)
                        {
                            _context.web_PhanQuyenNhomSanPham!.Remove(itm);
                        }
                    }

                    var lstweb_PhanQuyenKhuVuc = await _context.web_PhanQuyenKhuVuc!.Where(e => e.LOC_ID == LOC_ID && e.ID_NHOMQUYEN == ID).ToListAsync();
                    if (lstweb_PhanQuyenKhuVuc != null)
                    {
                        foreach (var itm in lstweb_PhanQuyenKhuVuc)
                        {
                            _context.web_PhanQuyenKhuVuc!.Remove(itm);
                        }
                    }

                    var lstweb_PhanQuyenKhachHang = await _context.web_PhanQuyenKhachHang!.Where(e => e.LOC_ID == LOC_ID && e.ID_NHOMQUYEN == ID).ToListAsync();
                    if (lstweb_PhanQuyenKhachHang != null)
                    {
                        foreach (var itm in lstweb_PhanQuyenKhachHang)
                        {
                            _context.web_PhanQuyenKhachHang!.Remove(itm);
                        }
                    }
                    _context.dm_NhomKhachHang!.Remove(GroupCusVen);
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

        private bool GroupCusVenExistsMA(string LOC_ID, string MA)
        {
            return _context.dm_NhomKhachHang!.Any(e => e.LOC_ID == LOC_ID && e.MA == MA);
        }

        private bool GroupCusVenExistsID(string LOC_ID, string ID)
        {
            return _context.dm_NhomKhachHang!.Any(e => e.LOC_ID == LOC_ID && e.ID == ID);
        }

        private bool GroupCusVenExists(dm_NhomKhachHang GroupCusVen)
        {
            return _context.dm_NhomKhachHang!.Any(e => e.LOC_ID == GroupCusVen.LOC_ID && e.MA == GroupCusVen.MA && e.ID != GroupCusVen.ID);
        }
    }
}