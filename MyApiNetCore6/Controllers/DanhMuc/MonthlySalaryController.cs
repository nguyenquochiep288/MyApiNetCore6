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
    public class MonthlySalaryController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public MonthlySalaryController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
        }
        [HttpGet("{LOC_ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetMonthlySalary(string LOC_ID)
        {
            try
            {

                var lstValue = await _context.dm_ThangLuong!.Where(e => e.LOC_ID == LOC_ID).OrderBy(e => e.MA).ToListAsync();
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

        // GET: api/MonthlySalary
        [HttpGet("{LOC_ID}/{Type}/{KeyWhere}/{ValuesSearch}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetMonthlySalary(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
        {
            try
            {
                ValuesSearch = ValuesSearch.Replace("%2f", "/");
                var lstValue = await _context.dm_ThangLuong!.Where(e => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).OrderBy(e => e.MA).ToListAsync();
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


        //GET: api/MonthlySalary/5
        [HttpGet("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetMonthlySalary(string LOC_ID, string ID)
        {
            try
            {
                var MonthlySalary = await _context.dm_ThangLuong!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);

                if (MonthlySalary == null)
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
                    Data = MonthlySalary
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

        // PUT: api/MonthlySalary/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{LOC_ID}/{MA}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PutMonthlySalary(string LOC_ID, string MA, dm_ThangLuong MonthlySalary)
        {
            try
            {
               

                if (LOC_ID != MonthlySalary.LOC_ID || MonthlySalary.MA != MA)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Dữ liệu khóa không giống nhau!",
                        Data = ""
                    });
                }
                if (MonthlySalaryExists(MonthlySalary))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + MonthlySalary.LOC_ID + "-" + MonthlySalary.MA + " trong dữ liệu!",
                        Data = ""
                    });
                }
                if (!MonthlySalaryExistsID(LOC_ID, MonthlySalary.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + MonthlySalary.ID + " dữ liệu!",
                        Data = ""
                    });
                }
                _context.Entry(MonthlySalary).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                var OKMonthlySalary = await _context.dm_ThangLuong!.FirstOrDefaultAsync(e => e.LOC_ID == MonthlySalary.LOC_ID && e.ID == MonthlySalary.ID);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = OKMonthlySalary
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

        // POST: api/MonthlySalary
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<dm_ThangLuong>> PostMonthlySalary(dm_ThangLuong MonthlySalary)
        {
            try
            {
                if (MonthlySalaryExistsMA(MonthlySalary.LOC_ID, MonthlySalary.MA))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + MonthlySalary.LOC_ID + "-" + MonthlySalary.MA + " trong dữ liệu!",
                        Data = ""
                    });
                }
                _context.dm_ThangLuong!.Add(MonthlySalary);
                AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                var OKMonthlySalary = await _context.dm_ThangLuong!.FirstOrDefaultAsync(e => e.LOC_ID == MonthlySalary.LOC_ID && e.ID == MonthlySalary.ID);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = OKMonthlySalary
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

        // DELETE: api/MonthlySalary/5
        [HttpDelete("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeleteMonthlySalary(string LOC_ID, string ID)
        {
            try
            {
               
                var MonthlySalary = await _context.dm_ThangLuong!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (MonthlySalary == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
                ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_ThangLuong>(MonthlySalary, MonthlySalary.ID, MonthlySalary.MA);
                if (!apiResponse.Success)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = apiResponse.Message,
                        Data = ""
                    });
                }
                _context.dm_ThangLuong!.Remove(MonthlySalary);
                AuditLogController auditLog = new AuditLogController(_context, _configuration); auditLog.InserAuditLog(); await _context.SaveChangesAsync();
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

        private bool MonthlySalaryExistsMA(string LOC_ID, string MA)
        {
            return _context.dm_ThangLuong!.Any(e => e.LOC_ID == LOC_ID && e.MA == MA);
        }

        private bool MonthlySalaryExistsID(string LOC_ID, string ID)
        {
            return _context.dm_ThangLuong!.Any(e => e.LOC_ID == LOC_ID && e.ID == ID);
        }
        private bool MonthlySalaryExists(dm_ThangLuong MonthlySalary)
        {
            return _context.dm_ThangLuong!.Any(e => e.LOC_ID == MonthlySalary.LOC_ID && e.MA == MonthlySalary.MA && e.ID != MonthlySalary.ID);
        }
    }
}