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
    public class ReceiptController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public ReceiptController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
        }
        [HttpGet("{LOC_ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetReceipt(string LOC_ID)
        {
            try
            {

                var lstValue = await _context.ct_PhieuThu!.Where(e => e.LOC_ID == LOC_ID).ToListAsync();
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

        // GET: api/Receipt
        [HttpGet("{LOC_ID}/{Type}/{KeyWhere}/{ValuesSearch}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetReceipt(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
        {
            try
            {
                var lstValue = await _context.ct_PhieuThu!.Where(e => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).ToListAsync();
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


        //GET: api/Receipt/5
        [HttpGet("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetReceipt(string LOC_ID, string ID)
        {
            try
            {
                var Receipt = await _context.ct_PhieuThu!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = Receipt
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

        // PUT: api/Receipt/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PutReceipt(string LOC_ID, string ID, [FromBody] v_ct_PhieuThu Receipt)
        {
            try
            {
                if (!ReceiptExistsID(Receipt.LOC_ID, Receipt.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + Receipt.LOC_ID + "-" + Receipt.ID + " dữ liệu!",
                        Data = ""
                    });
                }
                if(Receipt.ISCHUYENCONGNOCHONHANVIEN)
                {
                    var LoaiPhieuThu = await _context.dm_LoaiPhieuThu!.AsNoTracking().FirstOrDefaultAsync(e => e.LOC_ID == Receipt.LOC_ID && e.MA == API.PTGCNKHCNV);
                    if (LoaiPhieuThu != null)
                        Receipt.ID_LOAIPHIEUTHU = LoaiPhieuThu.ID;
                }    
                var PhieuThuCheck = await _context.ct_PhieuThu!.AsNoTracking().FirstOrDefaultAsync(e => e.LOC_ID == Receipt.LOC_ID && e.ID == Receipt.ID);
                using var transaction = _context.Database.BeginTransaction();
                {
                    _context.Entry(Receipt).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
                transaction.Commit();

                using var transaction1 = _context.Database.BeginTransaction();
                {
                    if (PhieuThuCheck != null)
                    {
                        if (PhieuThuCheck.CHUNGTUKEMTHEO != Receipt.CHUNGTUKEMTHEO)
                        {
                            if (PhieuThuCheck.CHUNGTUKEMTHEO != null && PhieuThuCheck.CHUNGTUKEMTHEO.StartsWith("PX-"))
                            {
                                var PhieuXuat = await _context.ct_PhieuXuat!.FirstOrDefaultAsync(e => e.LOC_ID == PhieuThuCheck.LOC_ID && e.MAPHIEU == PhieuThuCheck.CHUNGTUKEMTHEO);
                                if (PhieuXuat != null)
                                {
                                    var ct_PhieuGiaoHang_ChiTiet = await _context.ct_PhieuGiaoHang_ChiTiet!.FirstOrDefaultAsync(e => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUGIAOHANG == PhieuXuat.ID);
                                    if (ct_PhieuGiaoHang_ChiTiet != null)
                                    {
                                        var SoTien = await _context.ct_PhieuThu!.Where(e => e.LOC_ID == PhieuThuCheck.LOC_ID && e.CHUNGTUKEMTHEO == PhieuThuCheck.CHUNGTUKEMTHEO).SumAsync(e => e.SOTIEN);
                                        ct_PhieuGiaoHang_ChiTiet.SOTIENDATHU = SoTien;
                                        _context.Entry(ct_PhieuGiaoHang_ChiTiet).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                    }
                                }
                            }
                            if (Receipt.CHUNGTUKEMTHEO != null && Receipt.CHUNGTUKEMTHEO.StartsWith("PX-"))
                            {
                                var PhieuXuat = await _context.ct_PhieuXuat!.FirstOrDefaultAsync(e => e.LOC_ID == Receipt.LOC_ID && e.MAPHIEU == Receipt.CHUNGTUKEMTHEO);
                                if (PhieuXuat != null)
                                {
                                    var ct_PhieuGiaoHang_ChiTiet = await _context.ct_PhieuGiaoHang_ChiTiet!.FirstOrDefaultAsync(e => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUXUAT == PhieuXuat.ID);
                                    if (ct_PhieuGiaoHang_ChiTiet != null)
                                    {
                                        var SoTien = await _context.ct_PhieuThu!.Where(e => e.LOC_ID == Receipt.LOC_ID && e.CHUNGTUKEMTHEO == Receipt.CHUNGTUKEMTHEO).SumAsync(e => e.SOTIEN);
                                        ct_PhieuGiaoHang_ChiTiet.SOTIENDATHU = SoTien;
                                        ct_PhieuGiaoHang_ChiTiet.ISCHUYENCONGNOCHONHANVIEN = Receipt.ISCHUYENCONGNOCHONHANVIEN;
                                        _context.Entry(ct_PhieuGiaoHang_ChiTiet).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                    }
                                }
                            }
                            AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                        }
                    }
                }
                transaction1.Commit();
                v_ct_PhieuThu PhieuThu = new v_ct_PhieuThu();
                SP_Parameter SP_Parameter = new SP_Parameter();
                SP_Parameter.ID_PHIEUTHU = Receipt.ID;
                ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
                var actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuThu(SP_Parameter);
                var okResult = actionResult as OkObjectResult;
                if (okResult != null)
                {
                    var ApiResponse = okResult.Value as ApiResponse;
                    if (ApiResponse != null)
                    {

                        if (ApiResponse.Data != null)
                        {
                            var lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuThu>;
                            if (lstPhieuNhap != null && lstPhieuNhap.Count() > 0)
                            {
                                PhieuThu = lstPhieuNhap.FirstOrDefault() ?? new v_ct_PhieuThu();
                            }
                        }
                    }
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = PhieuThu
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

        // POST: api/Receipt
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<ct_PhieuNhap>> PostReceipt([FromBody] v_ct_PhieuThu Receipt)
        {
            try
            {
                if (ReceiptExistsID(Receipt.LOC_ID, Receipt.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + Receipt.LOC_ID + "-" + Receipt.ID + " trong dữ liệu!",
                        Data = "",
                        CheckValue = true
                    });
                }
                if (Receipt.ISCHUYENCONGNOCHONHANVIEN)
                {
                    var LoaiPhieuThu = await _context.dm_LoaiPhieuThu!.AsNoTracking().FirstOrDefaultAsync(e => e.LOC_ID == Receipt.LOC_ID && e.MA == API.PTGCNKHCNV);
                    if(LoaiPhieuThu != null)
                        Receipt.ID_LOAIPHIEUTHU = LoaiPhieuThu.ID;
                }
                var objPhieuNhap = await _context.ct_PhieuThu!.FirstOrDefaultAsync(e => e.LOC_ID == Receipt.LOC_ID && e.MAPHIEU == Receipt.MAPHIEU);
                if (objPhieuNhap != null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + Receipt.LOC_ID + "-" + Receipt.MAPHIEU + " trong dữ liệu!",
                        Data = "",
                        CheckValue = true
                    });
                }
                using var transaction = _context.Database.BeginTransaction();
                {
                    _context.ct_PhieuThu!.Add(Receipt);
                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
                transaction.Commit();

                using var transaction1 = _context.Database.BeginTransaction();
                {
                    if (Receipt.CHUNGTUKEMTHEO != null && Receipt.CHUNGTUKEMTHEO.StartsWith("PX-"))
                    {
                        var PhieuXuat = await _context.ct_PhieuXuat!.FirstOrDefaultAsync(e => e.LOC_ID == Receipt.LOC_ID && e.MAPHIEU == Receipt.CHUNGTUKEMTHEO);
                        if (PhieuXuat != null)
                        {
                            var ct_PhieuGiaoHang_ChiTiet = await _context.ct_PhieuGiaoHang_ChiTiet!.FirstOrDefaultAsync(e => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUXUAT == PhieuXuat.ID);
                            if (ct_PhieuGiaoHang_ChiTiet != null)
                            {
                                var SoTien = await _context.ct_PhieuThu!.Where(e => e.LOC_ID == Receipt.LOC_ID && e.CHUNGTUKEMTHEO == Receipt.CHUNGTUKEMTHEO).SumAsync(e => e.SOTIEN);
                                ct_PhieuGiaoHang_ChiTiet.SOTIENDATHU = SoTien;
                                ct_PhieuGiaoHang_ChiTiet.ISCHUYENCONGNOCHONHANVIEN = Receipt.ISCHUYENCONGNOCHONHANVIEN;
                                _context.Entry(ct_PhieuGiaoHang_ChiTiet).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            }
                        }
                    }
                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }

                transaction1.Commit();
                v_ct_PhieuThu PhieuThu = new v_ct_PhieuThu();
                SP_Parameter SP_Parameter = new SP_Parameter();
                SP_Parameter.ID_PHIEUTHU = Receipt.ID;
                ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
                var actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuThu(SP_Parameter);
                var okResult = actionResult as OkObjectResult;
                if (okResult != null)
                {
                    var ApiResponse = okResult.Value as ApiResponse;
                    if (ApiResponse != null)
                    {

                        if (ApiResponse.Data != null)
                        {
                            var lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuThu>;
                            if (lstPhieuNhap != null && lstPhieuNhap.Count() > 0)
                            {
                                PhieuThu = lstPhieuNhap.FirstOrDefault() ?? new v_ct_PhieuThu();
                            }
                        }
                    }
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = PhieuThu
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

        // DELETE: api/Receipt/5
        [HttpDelete("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeleteReceipt(string LOC_ID, string ID)
        {
            try
            {
                var Receipt = await _context.ct_PhieuThu!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (Receipt == null)
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
                    if (Receipt.CHUNGTUKEMTHEO != null && Receipt.CHUNGTUKEMTHEO.StartsWith("PX-"))
                    {
                        var PhieuXuat = await _context.ct_PhieuXuat!.FirstOrDefaultAsync(e => e.LOC_ID == Receipt.LOC_ID && e.MAPHIEU == Receipt.CHUNGTUKEMTHEO);
                        if (PhieuXuat != null)
                        {
                            var ct_PhieuGiaoHang_ChiTiet = await _context.ct_PhieuGiaoHang_ChiTiet!.FirstOrDefaultAsync(e => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUXUAT == PhieuXuat.ID);
                            if (ct_PhieuGiaoHang_ChiTiet != null)
                            {
                                var SoTien = await _context.ct_PhieuThu!.Where(e => e.LOC_ID == Receipt.LOC_ID && e.CHUNGTUKEMTHEO == Receipt.CHUNGTUKEMTHEO).SumAsync(e => e.SOTIEN);
                                ct_PhieuGiaoHang_ChiTiet.SOTIENDATHU = SoTien - Receipt.SOTIEN;

                                var ReceiptCheck = await _context.ct_PhieuThu!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.MAPHIEU == Receipt.CHUNGTUKEMTHEO && e.ISCHUYENCONGNOCHONHANVIEN);
                                if(ReceiptCheck == null)
                                {
                                    ct_PhieuGiaoHang_ChiTiet.ISCHUYENCONGNOCHONHANVIEN = false;
                                }
                                _context.Entry(ct_PhieuGiaoHang_ChiTiet).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            }
                        }
                    }
                    _context.ct_PhieuThu!.Remove(Receipt);
                    AuditLogController auditLog = new AuditLogController(_context, _configuration); auditLog.InserAuditLog(); await _context.SaveChangesAsync();
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

        private bool ReceiptExistsID(string LOC_ID, string ID)
        {
            return _context.ct_PhieuThu!.Any(e => e.LOC_ID == LOC_ID && e.ID == ID);
        }

    }
}