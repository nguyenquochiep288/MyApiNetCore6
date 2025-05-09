using System;
using System.Collections.Generic;
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
using DatabaseTHP.Treeview;
using System.Xml.Linq;

namespace MyApiNetCore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsAreaController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public PermissionsAreaController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
        }
        [HttpGet("{LOC_ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetPermissionsArea(string LOC_ID)
        {
            try
            {

                var lstValue = await _context.web_PhanQuyenKhuVuc!.Where(e => e.LOC_ID == LOC_ID).ToListAsync();
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

        // GET: api/PermissionsArea
        [HttpGet("{LOC_ID}/{Type}/{KeyWhere}/{ValuesSearch}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetPermissionsArea(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
        {
            try
            {
                var lstValue = await _context.web_PhanQuyenKhuVuc!.Where(e => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).ToListAsync();
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


        //GET: api/PermissionsArea/5
        [HttpGet("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetPermissionsArea(string LOC_ID, string ID)
        {
            try
            {
                var PermissionsArea = await _context.web_PhanQuyenKhuVuc!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);

                if (PermissionsArea == null)
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
                    Data = PermissionsArea
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

        // PUT: api/PermissionsArea/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkID=2123754
        [HttpPut("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PutPermissionsArea(string LOC_ID, string ID, web_PhanQuyenKhuVuc PermissionsArea)
        {
            try
            {
                if (LOC_ID != PermissionsArea.LOC_ID && ID != PermissionsArea.ID)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Dữ liệu khóa không giống nhau!",
                        Data = ""
                    });
                }
                if (!PermissionsAreaExists(PermissionsArea.LOC_ID, PermissionsArea.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                _context.Entry(PermissionsArea).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = ""
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

        // POST: api/PermissionsArea
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkID=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<List<Treeview>>> PostPermissionsArea(List<Treeview> lstTreeview)
        {
            try
            {
                using var transaction = _context.Database.BeginTransaction();
                var FirstOrDefault = lstTreeview.FirstOrDefault();
                if (FirstOrDefault != null)
                {
                    _context.Database.ExecuteSqlRaw("UPDATE web_PhanQuyenKhuVuc SET ISACTIVE = 0 WHERE LOC_ID ='" + FirstOrDefault.LOC_ID + "' AND ID_NHOMQUYEN = '" + FirstOrDefault.idNhomQuyen + "'");
                    foreach (Treeview itm in lstTreeview.Where(s => s.Name == "TBL_DEPT"))
                    {
                        var checkSP = await _context.web_PhanQuyenKhuVuc!.FirstOrDefaultAsync(e => e.ID_NHOMQUYEN == itm.idNhomQuyen && e.ID_KHUVUC == itm.id);
                        if (checkSP == null && lstTreeview.Where(s => s.idNhomSanPham == itm.id && s.Checked == true).Count() > 0)
                        {
                            web_PhanQuyenKhuVuc web_PhanQuyenKhuVuc = new web_PhanQuyenKhuVuc();
                            web_PhanQuyenKhuVuc.LOC_ID = itm.LOC_ID;
                            web_PhanQuyenKhuVuc.ID = Guid.NewGuid().ToString();
                            web_PhanQuyenKhuVuc.ID_KHUVUC = itm.id;
                            web_PhanQuyenKhuVuc.ID_NHOMQUYEN = itm.idNhomQuyen;
                            web_PhanQuyenKhuVuc.ISACTIVE   = itm.Checked;
                            _context.web_PhanQuyenKhuVuc!.Add(web_PhanQuyenKhuVuc);
                        }
                        else
                        {
                            if (checkSP != null)
                            {
                                checkSP.ISACTIVE = itm.Checked;
                                _context.Entry(checkSP).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            }
                        }
                        AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                    }
                }

                transaction.Commit();
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = lstTreeview
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

        // DELETE: api/PermissionsArea/5
        [HttpDelete("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeletePermissionsArea(string LOC_ID, string ID)
        {
            try
            {
                var PermissionsArea = await _context.web_PhanQuyenKhuVuc!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (PermissionsArea == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                _context.web_PhanQuyenKhuVuc!.Remove(PermissionsArea);
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

        private bool PermissionsAreaExists(string LOC_ID, string ID)
        {
            return _context.web_PhanQuyenKhuVuc!.Any(e => e.LOC_ID == LOC_ID && e.ID == ID);
        }


    }
}