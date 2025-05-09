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
using System.Reflection;

namespace MyApiNetCore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Delivery_ImageController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public Delivery_ImageController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
        }

        //GET: api/Input/5
        [HttpGet("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetInput(string LOC_ID, string ID)
        {
            try
            {
                //var Input = await _context.ct_PhieuGiaoHang_HinhAnh!.Where(e => e.LOC_ID == LOC_ID && e.ID_PHIEUXUAT == ID).ToListAsync();
                var Input = from itm in _context.ct_PhieuGiaoHang_HinhAnh
                            where itm.LOC_ID == LOC_ID && (itm.ID_PHIEUXUAT == ID || itm.ID_PHIEUGIAOHANG == ID)
                            join lpn in _context.AspNetUsers on itm.ID_NGUOITAO equals lpn.ID
                            join px in _context.ct_PhieuXuat on itm.ID_PHIEUXUAT equals px.ID
                            select new v_ct_PhieuGiaoHang_HinhAnh()
                            {
                                LOC_ID = itm.LOC_ID,
                                ID = itm.ID,
                                ID_PHIEUGIAOHANG = itm.ID_PHIEUGIAOHANG,
                                ID_PHIEUXUAT = itm.ID_PHIEUXUAT,
                                URL_IMAGE = itm.URL_IMAGE,
                                GHICHU = itm.GHICHU,
                                NGAYTAO = itm.NGAYTAO,
                                ID_NGUOITAO = itm.ID_NGUOITAO,
                                THOIGIANTHEM = itm.THOIGIANTHEM,
                                NAME_NGUOITAO = lpn.FullName,
                                MAPHIEUXUAT = px.MAPHIEU
                            };
                if (Input == null)
                {
                    
                }
                
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = Input
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

        // PUT: api/Input/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PostInput([FromBody] ct_PhieuGiaoHang_HinhAnh Input)
        {
            try
            {
                var PhieuGiaoHang = await _context.ct_PhieuGiaoHang!.FirstOrDefaultAsync(e => e.LOC_ID == Input.LOC_ID && e.ID == Input.ID_PHIEUGIAOHANG);
                if(PhieuGiaoHang != null && PhieuGiaoHang.ISHOANTAT)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Phiếu giao hàng " + PhieuGiaoHang.MAPHIEU + " đã được hoàn tất! Nên không thể thay đổi trạng thái!",
                        Data = ""
                    });
                }
                using var transaction = _context.Database.BeginTransaction();
                {
                    _context.ct_PhieuGiaoHang_HinhAnh!.Add(Input);
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

        // DELETE: api/Area/5
        [HttpDelete("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeleteArea(string LOC_ID, string ID)
        {
            try
            {
                var Area = await _context.ct_PhieuGiaoHang_HinhAnh!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (Area == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
                _context.ct_PhieuGiaoHang_HinhAnh!.Remove(Area);
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
    }
}