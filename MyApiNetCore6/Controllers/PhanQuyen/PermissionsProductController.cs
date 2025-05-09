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
    public class PermissionsProductController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public PermissionsProductController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
        }
        [HttpGet("{LOC_ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetPermissionsProduct(string LOC_ID)
        {
            try
            {

                var lstValue = await _context.web_PhanQuyenSanPham!.Where(e => e.LOC_ID == LOC_ID).ToListAsync();
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

        // GET: api/PermissionsProduct
        [HttpGet("{LOC_ID}/{Type}/{KeyWhere}/{ValuesSearch}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetPermissionsProduct(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
        {
            try
            {
                var lstValue = await _context.web_PhanQuyenSanPham!.Where(e => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).ToListAsync();
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = lstValue
                }); 
            } 
            catch(Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                });
            }
        }
        

        //GET: api/PermissionsProduct/5
        [HttpGet("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetPermissionsProduct(string LOC_ID,string ID)
        {
            try
            {
                var PermissionsProduct = await _context.web_PhanQuyenSanPham!.FirstOrDefaultAsync(e =>  e.LOC_ID == LOC_ID&& e.ID == ID);

                if (PermissionsProduct == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID +"-"+ ID +  " dữ liệu!",
                        Data = ""
                    });
                }
				

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = PermissionsProduct
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

        // PUT: api/PermissionsProduct/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{LOC_ID}/{ID}")]
		[Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PutPermissionsProduct(string LOC_ID,string ID, web_PhanQuyenSanPham PermissionsProduct)
        {
            try
            {
                if ( LOC_ID != PermissionsProduct.LOC_ID&& ID != PermissionsProduct.ID)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Dữ liệu khóa không giống nhau!",
                        Data = ""
                    });
                }
                if (!PermissionsProductExists(PermissionsProduct.LOC_ID,PermissionsProduct.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID +"-"+ ID +  " dữ liệu!",
                        Data = ""
                    });
                }
                _context.Entry(PermissionsProduct).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
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

        // POST: api/PermissionsProduct
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<List<Treeview>>> PostPermissionsProduct(List<Treeview> lstTreeview)
        {
            try
            {
                using var transaction = _context.Database.BeginTransaction();
                var FirstOrDefault = lstTreeview.FirstOrDefault();
                if (FirstOrDefault != null)
                {
                    _context.Database.ExecuteSqlRaw("UPDATE web_PhanQuyenNhomSanPham SET ISACTIVE = 0 WHERE LOC_ID ='" + FirstOrDefault.LOC_ID + "' AND ID_NHOMQUYEN = '" + FirstOrDefault.idNhomQuyen + "'");
                    foreach (Treeview itm in lstTreeview.Where(s => s.Name == "TBL_DEPT"))
                    {
                        var checkSP = await _context.web_PhanQuyenNhomSanPham!.FirstOrDefaultAsync(e => e.ID_NHOMQUYEN == itm.idNhomQuyen && e.ID_NHOMSANPHAM == itm.id);
                        if (checkSP == null && lstTreeview.Where(s => s.idNhomSanPham == itm.id && s.Checked == true).Count() > 0)
                        {
                            web_PhanQuyenNhomSanPham web_PhanQuyenNhomSanPham = new web_PhanQuyenNhomSanPham();
                            web_PhanQuyenNhomSanPham.LOC_ID = itm.LOC_ID;
                            web_PhanQuyenNhomSanPham.ID = Guid.NewGuid().ToString();
                            web_PhanQuyenNhomSanPham.ID_NHOMSANPHAM = itm.id;
                            web_PhanQuyenNhomSanPham.ID_NHOMQUYEN = itm.idNhomQuyen;
                            web_PhanQuyenNhomSanPham.ISACTIVE = itm.Checked;
                            web_PhanQuyenNhomSanPham.ISPHANQUYENSANPHAM = lstTreeview.Where(s => s.Name == "TBL_DEPTALL" && s.id == itm.id).Select(s => s.Checked).FirstOrDefault();
                            _context.web_PhanQuyenNhomSanPham!.Add(web_PhanQuyenNhomSanPham);
                        }
                        else
                        {
                            if (checkSP != null)
                            {
                                checkSP.ISACTIVE = itm.Checked;
                                checkSP.ISPHANQUYENSANPHAM = lstTreeview.Where(s => s.Name == "TBL_DEPTALL" && s.id == itm.id).Select(s => s.Checked).FirstOrDefault();
                                _context.Entry(checkSP).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            }    
                        }
                        AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                    }
                    _context.Database.ExecuteSqlRaw("UPDATE web_PhanQuyenSanPham SET ISACTIVE = 0 WHERE LOC_ID ='" + FirstOrDefault.LOC_ID + "' AND ID_NHOMQUYEN = '" + FirstOrDefault.idNhomQuyen + "'");
                    foreach (Treeview itm in lstTreeview.Where(s => s.Name.StartsWith("TBL_ITEM-") && s.Checked))
                    {
                        var checkSP = await _context.web_PhanQuyenSanPham!.FirstOrDefaultAsync(e => e.ID_NHOMQUYEN == itm.idNhomQuyen && e.ID_SANPHAM == itm.id);
                        if (checkSP == null)
                        {
                            web_PhanQuyenSanPham web_PhanQuyenSanPham = new web_PhanQuyenSanPham();
                            web_PhanQuyenSanPham.LOC_ID = itm.LOC_ID;
                            web_PhanQuyenSanPham.ID = Guid.NewGuid().ToString();
                            web_PhanQuyenSanPham.ID_SANPHAM = itm.id;
                            web_PhanQuyenSanPham.ID_NHOMQUYEN = itm.idNhomQuyen;
                            web_PhanQuyenSanPham.ISACTIVE = itm.Checked;
                            _context.web_PhanQuyenSanPham!.Add(web_PhanQuyenSanPham);
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
            catch(Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                });
            }
        }

        // DELETE: api/PermissionsProduct/5
        [HttpDelete("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeletePermissionsProduct(string LOC_ID,string ID)
        {
            try
            {
                var PermissionsProduct = await _context.web_PhanQuyenSanPham!.FirstOrDefaultAsync(e =>  e.LOC_ID == LOC_ID && e.ID == ID);
                if (PermissionsProduct == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID +"-"+ ID +  " dữ liệu!",
                        Data = ""
                    });
                }
                _context.web_PhanQuyenSanPham!.Remove(PermissionsProduct);
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

        private bool PermissionsProductExists(string LOC_ID,string ID)
        {
            return _context.web_PhanQuyenSanPham!.Any(e =>  e.LOC_ID == LOC_ID&& e.ID == ID);
        }

       
    }
}