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

namespace MyApiNetCore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public PermissionsController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
        }
        [HttpGet("{LOC_ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetPermissions(string LOC_ID)
        {
            try
            {

                var lstValue = await _context.web_PhanQuyen!.Where(e => e.LOC_ID == LOC_ID).ToListAsync();
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

        // GET: api/Permissions
        [HttpGet("{LOC_ID}/{Type}/{KeyWhere}/{ValuesSearch}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetPermissions(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
        {
            try
            {
                var lstValue = await _context.web_PhanQuyen!.Where(e => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).ToListAsync();
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
        

        //GET: api/Permissions/5
        [HttpGet("{LOC_ID}/{id}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetPermissions(string LOC_ID,string id)
        {
            try
            {
                var Permissions = await _context.web_PhanQuyen!.FirstOrDefaultAsync(e =>  e.LOC_ID == LOC_ID&& e.ID == id);

                if (Permissions == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID +"-"+ id +  " dữ liệu!",
                        Data = ""
                    });
                }
				

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = Permissions
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

        // PUT: api/Permissions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{LOC_ID}/{id}")]
		[Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PutPermissions(string LOC_ID,string id, web_PhanQuyen Permissions)
        {
            try
            {
                if ( LOC_ID != Permissions.LOC_ID&& id != Permissions.ID)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Dữ liệu khóa không giống nhau!",
                        Data = ""
                    });
                }
                if (!PermissionsExists(Permissions.LOC_ID,Permissions.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID +"-"+ id +  " dữ liệu!",
                        Data = ""
                    });
                }
                _context.Entry(Permissions).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
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
                    _context.Database.ExecuteSqlRaw("UPDATE web_PhanQuyen SET ISACTIVE = 0 WHERE LOC_ID ='" + FirstOrDefault.LOC_ID + "' AND ID_NHOMQUYEN = '" + FirstOrDefault.idNhomQuyen + "'");
                    foreach (Treeview itm in lstTreeview.Where(s => s.Name.StartsWith("TBL_ITEM-") && s.Checked))
                    {
                        var checkSP = await _context.web_PhanQuyen!.FirstOrDefaultAsync(e => e.ID_NHOMQUYEN == itm.idNhomQuyen && e.ID_QUYEN == itm.id);
                        if (checkSP == null)
                        {
                            web_PhanQuyen newweb_PhanQuyen = new web_PhanQuyen();
                            newweb_PhanQuyen.LOC_ID = itm.LOC_ID;
                            newweb_PhanQuyen.ID = Guid.NewGuid().ToString();
                            newweb_PhanQuyen.ID_QUYEN = itm.id;
                            newweb_PhanQuyen.ID_NHOMQUYEN = itm.idNhomQuyen;
                            newweb_PhanQuyen.ISACTIVE = itm.Checked;
                            _context.web_PhanQuyen!.Add(newweb_PhanQuyen);
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

        // DELETE: api/Permissions/5
        [HttpDelete("{LOC_ID}/{id}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeletePermissions(string LOC_ID,string id)
        {
            try
            {
                var Permissions = await _context.web_PhanQuyen!.FirstOrDefaultAsync(e =>  e.LOC_ID == LOC_ID&& e.ID == id);
                if (Permissions == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID +"-"+ id +  " dữ liệu!",
                        Data = ""
                    });
                }
                _context.web_PhanQuyen!.Remove(Permissions);
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

        private bool PermissionsExists(string LOC_ID,string id)
        {
            return _context.web_PhanQuyen!.Any(e =>  e.LOC_ID == LOC_ID && e.ID == id);
        }

       
    }
}