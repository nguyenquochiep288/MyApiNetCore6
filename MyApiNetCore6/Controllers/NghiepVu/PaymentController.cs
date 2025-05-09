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
using Newtonsoft.Json;
using NuGet.Packaging;
using System.Configuration;
using DatabaseTHP.StoredProcedure.Parameter;

namespace MyApiNetCore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public PaymentController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
        }
        [HttpGet("{LOC_ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetPayment(string LOC_ID)
        {
            try
            {
                var lstValue = await _context.ct_PhieuChi!.Where(e => e.LOC_ID == LOC_ID).ToListAsync();
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

        // GET: api/Payment
        [HttpGet("{LOC_ID}/{Type}/{KeyWhere}/{ValuesSearch}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetPayment(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
        {
            try
            {
                var lstValue = await _context.ct_PhieuChi!.Where(e => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).ToListAsync();
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


        //GET: api/Payment/5
        [HttpGet("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetPayment(string LOC_ID, string ID)
        {
            try
            {
                var Payment = await _context.ct_PhieuChi!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = Payment
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

        // PUT: api/Payment/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PutPayment(string LOC_ID, string ID, [FromBody] v_ct_PhieuChi Payment)
        {
            try
            {
                if (!PaymentExistsID(Payment.LOC_ID, Payment.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + Payment.LOC_ID + "-" + Payment.ID + " dữ liệu!",
                        Data = ""
                    });
                }
                var PhieuThuCheck = await _context.ct_PhieuChi!.AsNoTracking().FirstOrDefaultAsync(e => e.LOC_ID == Payment.LOC_ID && e.ID == Payment.ID);
                using var transaction = _context.Database.BeginTransaction();
                {
                    _context.Entry(Payment).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
                transaction.Commit();
                using var transaction1 = _context.Database.BeginTransaction();
                {
                    if (PhieuThuCheck != null)
                    {
                        if (Payment.CHUNGTUKEMTHEO != PhieuThuCheck.CHUNGTUKEMTHEO)
                        {
                            if (PhieuThuCheck.CHUNGTUKEMTHEO != null && PhieuThuCheck.CHUNGTUKEMTHEO.StartsWith("PX-"))
                            {
                                var PhieuXuat = await _context.ct_PhieuXuat!.FirstOrDefaultAsync(e => e.LOC_ID == PhieuThuCheck.LOC_ID && e.MAPHIEU == PhieuThuCheck.CHUNGTUKEMTHEO);
                                if (PhieuXuat != null)
                                {
                                    var ct_PhieuGiaoHang_ChiTiet = await _context.ct_PhieuGiaoHang_ChiTiet!.FirstOrDefaultAsync(e => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUGIAOHANG == PhieuXuat.ID);
                                    if (ct_PhieuGiaoHang_ChiTiet != null)
                                    {
                                        ct_PhieuGiaoHang_ChiTiet.ID_PHIEUCHI = "";
                                        _context.Entry(ct_PhieuGiaoHang_ChiTiet).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                    }
                                }
                            }
                        }
                        if (Payment.CHUNGTUKEMTHEO != null && Payment.CHUNGTUKEMTHEO.StartsWith("PX-"))
                        {
                            var PhieuXuat = await _context.ct_PhieuXuat!.FirstOrDefaultAsync(e => e.LOC_ID == Payment.LOC_ID && e.MAPHIEU == Payment.CHUNGTUKEMTHEO);
                            if (PhieuXuat != null)
                            {
                                var ct_PhieuGiaoHang_ChiTiet = await _context.ct_PhieuGiaoHang_ChiTiet!.FirstOrDefaultAsync(e => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUXUAT == PhieuXuat.ID);
                                if (ct_PhieuGiaoHang_ChiTiet != null)
                                {
                                    ct_PhieuGiaoHang_ChiTiet.ID_PHIEUCHI = Payment.MAPHIEU;
                                    _context.Entry(ct_PhieuGiaoHang_ChiTiet).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                }
                            }
                        }
                    }
                }
                transaction1.Commit();

                v_ct_PhieuChi PhieuChi = new v_ct_PhieuChi();
                SP_Parameter SP_Parameter = new SP_Parameter();
                SP_Parameter.ID_PHIEUCHI = Payment.ID;
                ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
                var actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuChi(SP_Parameter);
                var okResult = actionResult as OkObjectResult;
                if (okResult != null)
                {
                    var ApiResponse = okResult.Value as ApiResponse;
                    if (ApiResponse != null)
                    {

                        if (ApiResponse.Data != null)
                        {
                            var lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuChi>;
                            if (lstPhieuNhap != null && lstPhieuNhap.Count() > 0)
                            {
                                PhieuChi = lstPhieuNhap.FirstOrDefault() ?? new v_ct_PhieuChi();
                            }
                        }
                    }
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = PhieuChi
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

        // POST: api/Payment
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<ct_PhieuNhap>> PostPayment([FromBody] v_ct_PhieuChi Payment)
        {
            try
            {
                if (PaymentExistsID(Payment.LOC_ID, Payment.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + Payment.LOC_ID + "-" + Payment.ID + " trong dữ liệu!",
                        Data = "",
                        CheckValue = true
                    });
                }
                var objPhieuNhap = await _context.ct_PhieuChi!.FirstOrDefaultAsync(e => e.LOC_ID == Payment.LOC_ID && e.MAPHIEU == Payment.MAPHIEU);
                if (objPhieuNhap != null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + Payment.LOC_ID + "-" + Payment.MAPHIEU + " trong dữ liệu!",
                        Data = "",
                        CheckValue = true
                    });
                }
                using var transaction = _context.Database.BeginTransaction();
                {
                   
                    _context.ct_PhieuChi!.Add(Payment);
                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
                transaction.Commit();

                using var transaction1 = _context.Database.BeginTransaction();
                {
                    if (Payment.CHUNGTUKEMTHEO != null && Payment.CHUNGTUKEMTHEO.StartsWith("PX-"))
                    {
                        var PhieuXuat = await _context.ct_PhieuXuat!.FirstOrDefaultAsync(e => e.LOC_ID == Payment.LOC_ID && e.MAPHIEU == Payment.CHUNGTUKEMTHEO);
                        if (PhieuXuat != null)
                        {
                            var ct_PhieuGiaoHang_ChiTiet = await _context.ct_PhieuGiaoHang_ChiTiet!.FirstOrDefaultAsync(e => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUXUAT == PhieuXuat.ID);
                            if (ct_PhieuGiaoHang_ChiTiet != null)
                            {
                                var SoTien = await _context.ct_PhieuThu!.Where(e => e.LOC_ID == Payment.LOC_ID && e.CHUNGTUKEMTHEO == Payment.CHUNGTUKEMTHEO).SumAsync(e => e.SOTIEN);
                                ct_PhieuGiaoHang_ChiTiet.ID_PHIEUCHI = Payment.MAPHIEU;
                                _context.Entry(ct_PhieuGiaoHang_ChiTiet).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            }
                        }
                    }
                    AuditLogController auditLog = new AuditLogController(_context, _configuration); auditLog.InserAuditLog(); await _context.SaveChangesAsync();
                }
                transaction1.Commit();
                v_ct_PhieuChi PhieuChi = new v_ct_PhieuChi();
                SP_Parameter SP_Parameter = new SP_Parameter();
                SP_Parameter.ID_PHIEUCHI = Payment.ID;
                ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
                var actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuChi(SP_Parameter);
                var okResult = actionResult as OkObjectResult;
                if (okResult != null)
                {
                    var ApiResponse = okResult.Value as ApiResponse;
                    if (ApiResponse != null)
                    {

                        if (ApiResponse.Data != null)
                        {
                            var lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuChi>;
                            if (lstPhieuNhap != null && lstPhieuNhap.Count() > 0)
                            {
                                PhieuChi = lstPhieuNhap.FirstOrDefault() ?? new v_ct_PhieuChi();
                            }
                        }
                    }
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = PhieuChi
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

        // DELETE: api/Payment/5
        [HttpDelete("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeletePayment(string LOC_ID, string ID)
        {
            try
            {
                var Payment = await _context.ct_PhieuChi!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (Payment == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                using var transaction = _context.Database.BeginTransaction();
                {
                    if (Payment.CHUNGTUKEMTHEO != null && Payment.CHUNGTUKEMTHEO.StartsWith("PX-"))
                    {
                        var PhieuXuat = await _context.ct_PhieuXuat!.FirstOrDefaultAsync(e => e.LOC_ID == Payment.LOC_ID && e.MAPHIEU == Payment.CHUNGTUKEMTHEO);
                        if (PhieuXuat != null)
                        {
                            var ct_PhieuGiaoHang_ChiTiet = await _context.ct_PhieuGiaoHang_ChiTiet!.FirstOrDefaultAsync(e => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUXUAT == PhieuXuat.ID);
                            if (ct_PhieuGiaoHang_ChiTiet != null)
                            {
                                var SoTien = await _context.ct_PhieuThu!.Where(e => e.LOC_ID == Payment.LOC_ID && e.CHUNGTUKEMTHEO == Payment.CHUNGTUKEMTHEO).SumAsync(e => e.SOTIEN);
                                if(ct_PhieuGiaoHang_ChiTiet.ID_PHIEUCHI == Payment.MAPHIEU)
                                {
                                    ct_PhieuGiaoHang_ChiTiet.ID_PHIEUCHI = "";
                                    _context.Entry(ct_PhieuGiaoHang_ChiTiet).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                }                             
                            }
                        }
                    }
                    _context.ct_PhieuChi!.Remove(Payment);
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

        private bool PaymentExistsID(string LOC_ID, string ID)
        {
            return _context.ct_PhieuChi!.Any(e => e.LOC_ID == LOC_ID && e.ID == ID);
        }

    }
}