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
    public class PermissionsGroupCustomerController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public PermissionsGroupCustomerController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
        }
        [HttpGet("{LOC_ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetPermissionsGroupCustomer(string LOC_ID)
        {
            try
            {

                var lstValue = await _context.web_PhanQuyenNhomKhachHang!.Where(e => e.LOC_ID == LOC_ID).ToListAsync();
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

        // GET: api/PermissionsGroupCustomer
        [HttpGet("{LOC_ID}/{Type}/{KeyWhere}/{ValuesSearch}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetPermissionsGroupCustomer(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
        {
            try
            {
                var lstValue = await _context.web_PhanQuyenNhomKhachHang!.Where(e => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).ToListAsync();
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
        

        //GET: api/PermissionsGroupCustomer/5
        [HttpGet("{LOC_ID}/{id}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetPermissionsGroupCustomer(string LOC_ID,string ID)
        {
            try
            {
                var PermissionsGroupCustomer = await _context.web_PhanQuyenNhomKhachHang!.FirstOrDefaultAsync(e =>  e.LOC_ID == LOC_ID&& e.ID == ID);

                if (PermissionsGroupCustomer == null)
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
                    Data = PermissionsGroupCustomer
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

        // PUT: api/PermissionsGroupCustomer/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{LOC_ID}/{ID}")]
		[Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PutPermissionsGroupCustomer(string LOC_ID,string ID, web_PhanQuyenNhomKhachHang PermissionsGroupCustomer)
        {
            try
            {
                if ( LOC_ID != PermissionsGroupCustomer.LOC_ID && ID != PermissionsGroupCustomer.ID)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Dữ liệu khóa không giống nhau!",
                        Data = ""
                    });
                }
                if (!PermissionsGroupCustomerExists(PermissionsGroupCustomer.LOC_ID,PermissionsGroupCustomer.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID +"-"+ ID +  " dữ liệu!",
                        Data = ""
                    });
                }
                _context.Entry(PermissionsGroupCustomer).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
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

        // POST: api/PermissionsGroupCustomer
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<web_PhanQuyenNhomKhachHang>> PostPermissionsGroupCustomer(web_PhanQuyenNhomKhachHang PermissionsGroupCustomer)
        {
            try
            {
                if (PermissionsGroupCustomerExists(PermissionsGroupCustomer.LOC_ID,PermissionsGroupCustomer.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại " + PermissionsGroupCustomer.LOC_ID + "-" + PermissionsGroupCustomer.ID +  " trong dữ liệu!",
                        Data = "",
                        CheckValue = true
                    });
                }
                _context.web_PhanQuyenNhomKhachHang!.Add(PermissionsGroupCustomer);
                AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = PermissionsGroupCustomer
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

        // DELETE: api/PermissionsGroupCustomer/5
        [HttpDelete("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeletePermissionsGroupCustomer(string LOC_ID,string ID)
        {
            try
            {
                var PermissionsGroupCustomer = await _context.web_PhanQuyenNhomKhachHang!.FirstOrDefaultAsync(e =>  e.LOC_ID == LOC_ID&& e.ID == ID);
                if (PermissionsGroupCustomer == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID +"-"+ ID +  " dữ liệu!",
                        Data = ""
                    });
                }
                _context.web_PhanQuyenNhomKhachHang!.Remove(PermissionsGroupCustomer);
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

        private bool PermissionsGroupCustomerExists(string LOC_ID,string ID)
        {
            return _context.web_PhanQuyenNhomKhachHang!.Any(e =>  e.LOC_ID == LOC_ID&& e.ID == ID);
        }

       
    }
}