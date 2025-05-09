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
    public class EmployeeController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public EmployeeController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
        }
        [HttpGet("{LOC_ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetEmployee(string LOC_ID)
        {
            try
            {

                var lstValue = await _context.view_dm_NhanVien!.Where(e => e.LOC_ID == LOC_ID).OrderBy(e => e.MA).ToListAsync();
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

        // GET: api/Employee
        [HttpGet("{LOC_ID}/{Type}/{KeyWhere}/{ValuesSearch}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetEmployee(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
        {
            try
            {
                ValuesSearch = ValuesSearch.Replace("%2f", "/");
                var lstValue = await _context.view_dm_NhanVien!.Where(e => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).OrderBy(e => e.MA).ToListAsync();
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


        //GET: api/Employee/5
        [HttpGet("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetEmployee(string LOC_ID, string ID)
        {
            try
            {
                var Employee = await _context.view_dm_NhanVien!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);

                if (Employee == null)
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
                    Data = Employee
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

        // PUT: api/Employee/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{LOC_ID}/{MA}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PutEmployee(string LOC_ID, string MA, dm_NhanVien Employee)
        {
            try
            {
                if (EmployeeExists(Employee))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + Employee.LOC_ID + "-" + Employee.MA + " trong dữ liệu!",
                        Data = ""
                    });
                }

                if (LOC_ID != Employee.LOC_ID || Employee.MA != MA)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Dữ liệu khóa không giống nhau!",
                        Data = ""
                    });
                }
                if (!EmployeeExistsID(LOC_ID, Employee.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + Employee.ID + " dữ liệu!",
                        Data = ""
                    });
                }
                if (!string.IsNullOrEmpty(Employee.ID_TAIKHOAN))
                {
                    var check = await _context.view_dm_NhanVien!.FirstOrDefaultAsync(e => e.LOC_ID == Employee.LOC_ID && e.ID != Employee.ID && e.ID_TAIKHOAN == Employee.ID_TAIKHOAN);

                    if (check != null)
                    {
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Tài khoản cấp cho " + Employee.MA + " - " + Employee.NAME + " đã được cấp cho nhân viên" + check.MA + " - "  + check.NAME,
                            Data = ""
                        });
                    }    
                }
                    
                   
                _context.Entry(Employee).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                var OKEmployee = await _context.view_dm_NhanVien!.FirstOrDefaultAsync(e => e.LOC_ID == Employee.LOC_ID && e.ID == Employee.ID);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = OKEmployee
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

        // POST: api/Employee
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<dm_NhanVien>> PostEmployee(dm_NhanVien Employee)
        {
            try
            {
                if (EmployeeExistsMA(Employee.LOC_ID, Employee.MA))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + Employee.LOC_ID + "-" + Employee.MA + " trong dữ liệu!",
                        Data = ""
                    });
                }
                if (!string.IsNullOrEmpty(Employee.ID_TAIKHOAN))
                {
                    var check = await _context.view_dm_NhanVien!.FirstOrDefaultAsync(e => e.LOC_ID == Employee.LOC_ID && e.ID != Employee.ID && e.ID_TAIKHOAN == Employee.ID_TAIKHOAN);

                    if (check != null)
                    {
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Tài khoản cấp cho " + Employee.MA + " - " + Employee.NAME + " đã được cấp cho nhân viên" + check.MA + " - " + check.NAME,
                            Data = ""
                        });
                    }
                }
                _context.dm_NhanVien!.Add(Employee);
                AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                var OKEmployee = await _context.view_dm_NhanVien!.FirstOrDefaultAsync(e => e.LOC_ID == Employee.LOC_ID && e.ID == Employee.ID);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = OKEmployee
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

        // DELETE: api/Employee/5
        [HttpDelete("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeleteEmployee(string LOC_ID, string ID)
        {
            try
            {
                var Employee = await _context.dm_NhanVien!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (Employee == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
                ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_NhanVien>(Employee, Employee.ID, Employee.MA);
                if (!apiResponse.Success)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = apiResponse.Message,
                        Data = ""
                    });
                }
                _context.dm_NhanVien!.Remove(Employee);
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

        private bool EmployeeExistsMA(string LOC_ID, string MA)
        {
            return _context.dm_NhanVien!.Any(e => e.LOC_ID == LOC_ID && e.MA == MA);
        }

        private bool EmployeeExistsID(string LOC_ID, string ID)
        {
            return _context.dm_NhanVien!.Any(e => e.LOC_ID == LOC_ID && e.ID == ID);
        }

        private bool EmployeeExists(dm_NhanVien Employee)
        {
            return _context.dm_NhanVien!.Any(e => e.LOC_ID == Employee.LOC_ID && e.MA == Employee.MA && e.ID != Employee.ID);
        }
    }
}