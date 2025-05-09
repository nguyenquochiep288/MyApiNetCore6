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
using DatabaseTHP.Treeview;
using System.Xml.Linq;

namespace MyApiNetCore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsCustomerController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public PermissionsCustomerController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
        }
        [HttpGet("{LOC_ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetPermissionsCustomer(string LOC_ID)
        {
            try
            {

                var lstValue = await _context.web_PhanQuyenKhachHang!.Where(e => e.LOC_ID == LOC_ID).ToListAsync();
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

        // GET: api/PermissionsCustomer
        [HttpGet("{LOC_ID}/{Type}/{KeyWhere}/{ValuesSearch}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetPermissionsCustomer(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
        {
            try
            {
                var lstValue = await _context.web_PhanQuyenKhachHang!.Where(e => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).ToListAsync();
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


        //GET: api/PermissionsCustomer/5
        [HttpGet("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetPermissionsCustomer(string LOC_ID, string ID)
        {
            try
            {
                var PermissionsCustomer = await _context.web_PhanQuyenKhachHang!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);

                if (PermissionsCustomer == null)
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
                    Data = PermissionsCustomer
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

        // PUT: api/PermissionsCustomer/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{LOC_ID}/{id}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PutPermissionsCustomer(string LOC_ID, string id, web_PhanQuyenKhachHang PermissionsCustomer)
        {
            try
            {
                if (LOC_ID != PermissionsCustomer.LOC_ID && id != PermissionsCustomer.ID)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Dữ liệu khóa không giống nhau!",
                        Data = ""
                    });
                }
                if (!PermissionsCustomerExists(PermissionsCustomer.LOC_ID, PermissionsCustomer.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + id + " dữ liệu!",
                        Data = ""
                    });
                }
                _context.Entry(PermissionsCustomer).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
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

        // POST: api/PermissionsCustomer
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<List<Treeview>>> PostPermissionsCustomer(List<Treeview> lstTreeview)
        {
            try
            {
                using var transaction = _context.Database.BeginTransaction();
                var FirstOrDefault = lstTreeview.FirstOrDefault();
                if (FirstOrDefault != null)
                {
                    _context.Database.ExecuteSqlRaw("UPDATE web_PhanQuyenKhuVuc SET ISACTIVE = 0, ISPHANQUYENKHUVUC = 0 WHERE LOC_ID ='" + FirstOrDefault.LOC_ID + "' AND ID_NHOMQUYEN = '" + FirstOrDefault.idNhomQuyen + "' AND ID_LICHLAMVIEC = '" + FirstOrDefault.idLichLamViec + "'");
                    foreach (Treeview itm in lstTreeview.Where(s => s.Name == "TBL_DEPT" && s.Checked))
                    {
                        var checkSP = await _context.web_PhanQuyenKhuVuc!.FirstOrDefaultAsync(e => e.ID_NHOMQUYEN == itm.idNhomQuyen && e.ID_KHUVUC == itm.id && e.ID_LICHLAMVIEC == itm.idLichLamViec);
                        if (checkSP == null && lstTreeview.Where(s => s.idNhomSanPham == itm.id && s.Checked == true).Count() > 0)
                        {
                            web_PhanQuyenKhuVuc web_PhanQuyenKhuVuc = new web_PhanQuyenKhuVuc();
                            web_PhanQuyenKhuVuc.LOC_ID = itm.LOC_ID;
                            web_PhanQuyenKhuVuc.ID = Guid.NewGuid().ToString();
                            web_PhanQuyenKhuVuc.ID_KHUVUC = itm.id;
                            web_PhanQuyenKhuVuc.ID_NHOMQUYEN = itm.idNhomQuyen;
                            web_PhanQuyenKhuVuc.ID_LICHLAMVIEC = itm.idLichLamViec;
                            web_PhanQuyenKhuVuc.ISACTIVE = itm.Checked;// lstTreeview.Where(s => s.idNhomSanPham == itm.id && s.Checked == true).Select(s => s.Checked).FirstOrDefault();
                            web_PhanQuyenKhuVuc.ISPHANQUYENKHUVUC = lstTreeview.Where(s => s.Name == "TBL_DEPTALL" && s.id == itm.id).Select(s => s.Checked).FirstOrDefault();
                            _context.web_PhanQuyenKhuVuc!.Add(web_PhanQuyenKhuVuc);
                        }
                        else
                        {
                            if (checkSP != null)
                            {
                                checkSP.ISACTIVE = itm.Checked;//lstTreeview.Where(s => s.idNhomSanPham == itm.id && s.Checked == true).Select(s => s.Checked).FirstOrDefault();
                                checkSP.ISPHANQUYENKHUVUC = lstTreeview.Where(s => s.Name == "TBL_DEPTALL" && s.id == itm.id).Select(s => s.Checked).FirstOrDefault();
                                _context.Entry(checkSP).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            }
                        }
                        AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                    }
                    _context.Database.ExecuteSqlRaw("UPDATE web_PhanQuyenKhachHang SET ISACTIVE = 0 WHERE LOC_ID ='" + FirstOrDefault.LOC_ID + "' AND ID_NHOMQUYEN = '" + FirstOrDefault.idNhomQuyen + "' AND ID_LICHLAMVIEC = '" + FirstOrDefault.idLichLamViec + "'");
                    foreach (Treeview itm in lstTreeview.Where(s => s.Name.StartsWith("TBL_ITEM-") && s.Checked))
                    {
                        var checkSP = await _context.web_PhanQuyenKhachHang!.FirstOrDefaultAsync(e => e.ID_NHOMQUYEN == itm.idNhomQuyen && e.ID_KHACHHANG == itm.id && e.ID_LICHLAMVIEC == itm.idLichLamViec);
                        if (checkSP == null)
                        {
                            web_PhanQuyenKhachHang web_PhanQuyenKhachHang = new web_PhanQuyenKhachHang();
                            web_PhanQuyenKhachHang.LOC_ID = itm.LOC_ID;
                            web_PhanQuyenKhachHang.ID = Guid.NewGuid().ToString();
                            web_PhanQuyenKhachHang.ID_KHACHHANG = itm.id;
                            web_PhanQuyenKhachHang.ID_NHOMQUYEN = itm.idNhomQuyen;
                            web_PhanQuyenKhachHang.ISACTIVE = itm.Checked;
                            web_PhanQuyenKhachHang.ID_LICHLAMVIEC = itm.idLichLamViec;
                            _context.web_PhanQuyenKhachHang!.Add(web_PhanQuyenKhachHang);
                        }
                        else
                        {
                            checkSP.ISACTIVE = itm.Checked;
                            _context.Entry(checkSP).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
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

        // DELETE: api/PermissionsCustomer/5
        [HttpDelete("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeletePermissionsCustomer(string LOC_ID, string ID)
        {
            try
            {
                var PermissionsCustomer = await _context.web_PhanQuyenKhachHang!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (PermissionsCustomer == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                _context.web_PhanQuyenKhachHang!.Remove(PermissionsCustomer);
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

        private bool PermissionsCustomerExists(string LOC_ID, string ID)
        {
            return _context.web_PhanQuyenKhachHang!.Any(e => e.LOC_ID == LOC_ID && e.ID == ID);
        }


    }
}