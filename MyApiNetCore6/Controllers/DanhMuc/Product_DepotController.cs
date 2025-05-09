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
    public class Product_DepotController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public Product_DepotController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
        }
        [HttpGet("{LOC_ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetProduct_Depot(string LOC_ID)
        {
            try
            {

                var lstValue = await _context.dm_HangHoa_Kho!.Where(e => e.LOC_ID == LOC_ID).ToListAsync();
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

        // GET: api/Product_Depot
        [HttpGet("{LOC_ID}/{Type}/{KeyWhere}/{ValuesSearch}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetProduct_Depot(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
        {
            try
            {
                var lstValue = await _context.dm_HangHoa_Kho!.Where(e => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).ToListAsync();
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
        

        //GET: api/Product_Depot/5
        [HttpGet("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetProduct_Depot(string LOC_ID, string ID)
        {
            try
            {
                var Product_Depot = await _context.dm_HangHoa_Kho!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);

                if (Product_Depot == null)
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
                    Data = Product_Depot
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

        // PUT: api/Product_Depot/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PutProduct_Depot(string LOC_ID, string ID, dm_HangHoa_Kho Product_Depot)
        {
            try
            {
                if (Product_Depot.ID != ID)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Dữ liệu khóa không giống nhau!",
                        Data = ""
                    });
                }
                if (!Product_DepotExists(LOC_ID, Product_Depot.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                _context.Entry(Product_Depot).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
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

        // POST: api/Product_Depot
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<dm_HangHoa_Kho>> PostProduct_Depot(dm_HangHoa_Kho Product_Depot)
        {
            try
            {
                if (Product_DepotExists(Product_Depot.LOC_ID, Product_Depot.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + Product_Depot.LOC_ID + "-" + Product_Depot.ID + " trong dữ liệu!",
                        Data = ""
                    });
                }
                _context.dm_HangHoa_Kho!.Add(Product_Depot);
                AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = Product_Depot
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

        // DELETE: api/Product_Depot/5
        [HttpDelete("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeleteProduct_Depot(string LOC_ID, string ID)
        {
            try
            {
                var Product_Depot = await _context.dm_HangHoa_Kho!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (Product_Depot == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID +  " dữ liệu!",
                        Data = ""
                    });
                }
                _context.dm_HangHoa_Kho!.Remove(Product_Depot);
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

        private bool Product_DepotExists(string LOC_ID, string ID)
        {
            return _context.dm_HangHoa_Kho!.Any(e => e.LOC_ID == LOC_ID && e.ID == ID);
        }

       
    }
}