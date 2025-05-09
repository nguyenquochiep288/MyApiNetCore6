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
    public class QuyenController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public QuyenController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
        }
        [HttpGet("{LOC_ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetQuyen(string LOC_ID)
        {
            try
            {

                var lstValue = await _context.view_web_Quyen!.Where(e => e.LOC_ID == LOC_ID).ToListAsync();
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

        // GET: api/Quyen
        [HttpGet("{LOC_ID}/{Type}/{KeyWhere}/{ValuesSearch}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetQuyen(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
        {
            try
            {
                var lstValue = await _context.view_web_Quyen!.Where(e => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).ToListAsync();
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
        

        //GET: api/Quyen/5
        [HttpGet("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetQuyen(string LOC_ID,string ID)
        {
            try
            {
                var Quyen = await _context.view_web_Quyen!.FirstOrDefaultAsync(e =>  e.LOC_ID == LOC_ID&& e.ID == ID);

                if (Quyen == null)
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
                    Data = Quyen
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

        // PUT: api/Quyen/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{LOC_ID}/{ID}")]
		[Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PutQuyen(string LOC_ID,string ID, web_Quyen Quyen)
        {
            try
            {
                if ( LOC_ID != Quyen.LOC_ID && ID != Quyen.ID)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Dữ liệu khóa không giống nhau!",
                        Data = ""
                    });
                }
                if (!QuyenExists(Quyen.LOC_ID,Quyen.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID +"-"+ ID +  " dữ liệu!",
                        Data = ""
                    });
                }
                _context.Entry(Quyen).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                var OKQuyen = await _context.view_web_Quyen!.FirstOrDefaultAsync(e => e.LOC_ID == Quyen.LOC_ID && e.ID == Quyen.ID);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = OKQuyen
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

        // POST: api/Quyen
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<web_Quyen>> PostQuyen(web_Quyen Quyen)
        {
            try
            {
                if (QuyenExists(Quyen.LOC_ID,Quyen.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + Quyen.LOC_ID +"-"+ Quyen.ID +  " trong dữ liệu!",
                        Data = ""
                    });
                }
                _context.web_Quyen!.Add(Quyen);
                AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                var OKQuyen = await _context.view_web_Quyen!.FirstOrDefaultAsync(e => e.LOC_ID == Quyen.LOC_ID && e.ID == Quyen.ID);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = OKQuyen
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

        // DELETE: api/Quyen/5
        [HttpDelete("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeleteQuyen(string LOC_ID,string ID)
        {
            try
            {
               
                var Quyen = await _context.web_Quyen!.FirstOrDefaultAsync(e =>  e.LOC_ID == LOC_ID&& e.ID == ID);
                var lstPhanQuyen = await _context.web_PhanQuyen!.Where(e => e.LOC_ID == LOC_ID && e.ID_QUYEN == ID).ToListAsync();

                if (Quyen == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID +"-"+ ID +  " dữ liệu!",
                        Data = ""
                    });
                }
                if (lstPhanQuyen != null)
                {
                    foreach(var web_PhanQuyen in lstPhanQuyen)
                    {
                        _context.web_PhanQuyen!.Remove(web_PhanQuyen);
                    }    
                }
                    
                _context.web_Quyen!.Remove(Quyen);
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

        private bool QuyenExists(string LOC_ID,string ID)
        {
            return _context.web_Quyen!.Any(e =>  e.LOC_ID == LOC_ID && e.ID == ID);
        }

       
    }
}