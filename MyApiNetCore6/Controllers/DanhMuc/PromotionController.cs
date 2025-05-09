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
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using Microsoft.CodeAnalysis.Differencing;
using Newtonsoft.Json;

namespace MyApiNetCore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public PromotionController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
        }
        [HttpGet("{LOC_ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetProduct(string LOC_ID)
        {
            try
            {
                var lstValue = await _context.view_dm_ChuongTrinhKhuyenMai!.Where(e => e.LOC_ID == LOC_ID).OrderByDescending(e => e.DENNGAY).ToListAsync();
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

        // GET: api/Product
        [HttpGet("{LOC_ID}/{Type}/{KeyWhere}/{ValuesSearch}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetProduct(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
        {
            try
            {
                ValuesSearch = ValuesSearch.Replace("%2f", "/");
                var lstValue = await _context.view_dm_ChuongTrinhKhuyenMai!.Where(e => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).OrderByDescending(e => e.DENNGAY).ToListAsync();
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


        //GET: api/Product/5
        [HttpGet("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetProduct(string LOC_ID, string ID)
        {
            try
            {
                var Product = await _context.view_dm_ChuongTrinhKhuyenMai!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (Product == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                v_view_dm_ChuongTrinhKhuyenMai dm_ChuongTrinhKhuyenMai = new v_view_dm_ChuongTrinhKhuyenMai();
                dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_Tang = new List<v_dm_ChuongTrinhKhuyenMai_Tang>();
                dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_YeuCau = new List<v_dm_ChuongTrinhKhuyenMai_YeuCau>();
                if (Product != null)
                {
                    dm_ChuongTrinhKhuyenMai = JsonConvert.DeserializeObject<v_view_dm_ChuongTrinhKhuyenMai>(JsonConvert.SerializeObject(Product)) ?? new v_view_dm_ChuongTrinhKhuyenMai();

                    var lstValue_Tang = await _context.view_dm_ChuongTrinhKhuyenMai_Tang!.Where(e => e.LOC_ID == LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == Product.ID).ToListAsync();
                    dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_Tang = JsonConvert.DeserializeObject<List<v_dm_ChuongTrinhKhuyenMai_Tang>>(JsonConvert.SerializeObject(lstValue_Tang));

                    var lstValue_yeuCau = await _context.view_dm_ChuongTrinhKhuyenMai_YeuCau!.Where(e => e.LOC_ID == LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == Product.ID).ToListAsync();
                    dm_ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_YeuCau = JsonConvert.DeserializeObject<List<v_dm_ChuongTrinhKhuyenMai_YeuCau>>(JsonConvert.SerializeObject(lstValue_yeuCau));
                }



                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = dm_ChuongTrinhKhuyenMai
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

        // PUT: api/Product/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{LOC_ID}/{MA}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PutProduct(string LOC_ID, string MA, [FromBody] v_dm_ChuongTrinhKhuyenMai ChuongTrinhKhuyenMai)
        {
            try
            {
                if (ProductExists(ChuongTrinhKhuyenMai))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + ChuongTrinhKhuyenMai.LOC_ID + "-" + ChuongTrinhKhuyenMai.MA + " trong dữ liệu!",
                        Data = ""
                    });
                }

                if (LOC_ID != ChuongTrinhKhuyenMai.LOC_ID || ChuongTrinhKhuyenMai.MA != MA)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Dữ liệu khóa không giống nhau!",
                        Data = ""
                    });
                }
                if (!ProductExistsID(ChuongTrinhKhuyenMai.LOC_ID, ChuongTrinhKhuyenMai.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + ChuongTrinhKhuyenMai.LOC_ID + "-" + ChuongTrinhKhuyenMai.ID + " dữ liệu!",
                        Data = ""
                    });
                }

                using var transaction = _context.Database.BeginTransaction();
                {
                    var ChuongTrinhKhuyenMai_Tang = await _context.dm_ChuongTrinhKhuyenMai_Tang!.Where(e => e.LOC_ID == LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == ChuongTrinhKhuyenMai.ID).ToListAsync();
                    foreach (var itm in ChuongTrinhKhuyenMai_Tang)
                    {
                        _context.dm_ChuongTrinhKhuyenMai_Tang!.Remove(itm);
                    }

                    var ChuongTrinhKhuyenMai_YeuCau = await _context.dm_ChuongTrinhKhuyenMai_YeuCau!.Where(e => e.LOC_ID == LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == ChuongTrinhKhuyenMai.ID).ToListAsync();
                    foreach (var itm in ChuongTrinhKhuyenMai_YeuCau)
                    {
                        _context.dm_ChuongTrinhKhuyenMai_YeuCau!.Remove(itm);
                    }

                    foreach (var itm in ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_Tang)
                    {
                        itm.ID_CHUONGTRINHKHUYENMAI = ChuongTrinhKhuyenMai.ID;
                        _context.dm_ChuongTrinhKhuyenMai_Tang!.Add(itm);
                    }

                    foreach (var itm in ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_YeuCau)
                    {
                        itm.ID_CHUONGTRINHKHUYENMAI = ChuongTrinhKhuyenMai.ID;
                        _context.dm_ChuongTrinhKhuyenMai_YeuCau!.Add(itm);
                    }

                    _context.Entry(ChuongTrinhKhuyenMai).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
                transaction.Commit();
                var OKProduct = await _context.view_dm_ChuongTrinhKhuyenMai!.FirstOrDefaultAsync(e => e.LOC_ID == ChuongTrinhKhuyenMai.LOC_ID && e.ID == ChuongTrinhKhuyenMai.ID);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = OKProduct
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

        // POST: api/Product
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<v_dm_ChuongTrinhKhuyenMai>> PostProduct([FromBody] v_dm_ChuongTrinhKhuyenMai ChuongTrinhKhuyenMai)
        {
            try
            {
                if (ProductExistsMA(ChuongTrinhKhuyenMai.LOC_ID, ChuongTrinhKhuyenMai.MA))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + ChuongTrinhKhuyenMai.LOC_ID + "-" + ChuongTrinhKhuyenMai.MA + " trong dữ liệu!",
                        Data = ""
                    });
                }
               
                using var transaction = _context.Database.BeginTransaction();
                {
                    foreach(var itm in ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_Tang)
                    {
                        itm.ID_CHUONGTRINHKHUYENMAI = ChuongTrinhKhuyenMai.ID;
                        _context.dm_ChuongTrinhKhuyenMai_Tang!.Add(itm);
                    }

                    foreach (var itm in ChuongTrinhKhuyenMai.lstdm_ChuongTrinhKhuyenMai_YeuCau)
                    {
                        itm.ID_CHUONGTRINHKHUYENMAI = ChuongTrinhKhuyenMai.ID;
                        _context.dm_ChuongTrinhKhuyenMai_YeuCau!.Add(itm);
                    }

                    _context.dm_ChuongTrinhKhuyenMai!.Add(ChuongTrinhKhuyenMai);
                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
                transaction.Commit();
                var OKProduct = await _context.view_dm_ChuongTrinhKhuyenMai!.FirstOrDefaultAsync(e => e.LOC_ID == ChuongTrinhKhuyenMai.LOC_ID && e.ID == ChuongTrinhKhuyenMai.ID);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = OKProduct
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

        // DELETE: api/Product/5
        [HttpDelete("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeleteProduct(string LOC_ID, string ID)
        {
            try
            {
               
                var ChuongTrinhKhuyenMai = await _context.dm_ChuongTrinhKhuyenMai!.AsNoTracking().FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (ChuongTrinhKhuyenMai == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
                ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_ChuongTrinhKhuyenMai>(ChuongTrinhKhuyenMai, ChuongTrinhKhuyenMai.ID, ChuongTrinhKhuyenMai.NAME);
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
                    var ChuongTrinhKhuyenMai_Tang = await _context.dm_ChuongTrinhKhuyenMai_Tang!.Where(e => e.LOC_ID == LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == ID).ToListAsync();
                    foreach (var itm in ChuongTrinhKhuyenMai_Tang)
                    {
                        _context.dm_ChuongTrinhKhuyenMai_Tang!.Remove(itm);
                    }

                    var ChuongTrinhKhuyenMai_YeuCau = await _context.dm_ChuongTrinhKhuyenMai_YeuCau!.Where(e => e.LOC_ID == LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == ID).ToListAsync();
                    foreach (var itm in ChuongTrinhKhuyenMai_YeuCau)
                    {
                        _context.dm_ChuongTrinhKhuyenMai_YeuCau!.Remove(itm);
                    }

                    _context.dm_ChuongTrinhKhuyenMai!.Remove(ChuongTrinhKhuyenMai);
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

        private bool ProductExistsMA(string LOC_ID, string MA)
        {
            return _context.dm_ChuongTrinhKhuyenMai!.Any(e => e.LOC_ID == LOC_ID && e.MA == MA);
        }

        private bool ProductExistsID(string LOC_ID, string ID)
        {
            return _context.dm_ChuongTrinhKhuyenMai!.Any(e => e.LOC_ID == LOC_ID && e.ID == ID);
        }
        private bool ProductExists(dm_ChuongTrinhKhuyenMai Position)
        {
            return _context.dm_ChuongTrinhKhuyenMai!.Any(e => e.LOC_ID == Position.LOC_ID && e.MA == Position.MA && e.ID != Position.ID);
        }
    }
}