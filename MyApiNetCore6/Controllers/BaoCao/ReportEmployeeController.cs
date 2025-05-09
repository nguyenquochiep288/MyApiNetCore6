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
using NuGet.ContentModel;
using DatabaseTHP.StoredProcedure;

namespace MyApiNetCore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportEmployeeController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public ReportEmployeeController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
        }

        // POST: api/Input
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<List<v_ThongKeCongNo_ChiTiet>>> PostDetail([FromBody] Sp_Get_BaoCaoTheoNhanVien_Result KhachHang)
        {
            try
            {
                List<v_ThongKeCongNo_ChiTiet> lstThongKeCongNo_ChiTiet = new List<v_ThongKeCongNo_ChiTiet>();
                var rlsPhieuNhap = from itm in _context.ct_PhieuNhap
                                  where itm.ID_NGUOITAO == KhachHang.ID_NHANVIEN
                                  where ((KhachHang.TYPE == 0) || (KhachHang.TYPE != 0 & itm.ID_LOAIPHIEUNHAP == KhachHang.ID_LOAIPHIEU))
                                  where (!KhachHang.ISTHEOTHOIGIAN) || (itm.NGAYLAP.Date >= KhachHang.TUNGAY.Date & itm.NGAYLAP.Date <= KhachHang.DENNGAY.Date)
                                  join lpn in _context.dm_LoaiPhieuNhap on itm.ID_LOAIPHIEUNHAP equals lpn.ID
                                  select new v_ThongKeCongNo_ChiTiet()
                                  {
                                      LOAIPHIEU = "3",
                                      ID_PHIEU = itm.ID,
                                      MAPHIEU = itm.MAPHIEU,
                                      NGAY = itm.NGAYLAP,
                                      DIENGIAI = lpn.NAME + (string.IsNullOrEmpty(itm.MAPHIEU) ? "" : " - " + itm.MAPHIEU),
                                      THU = itm.TONGTIEN,
                                  };

                var rlsPhieuThu = from itm in _context.ct_PhieuThu
                                  where itm.ID_NGUOITAO == KhachHang.ID_NHANVIEN
                                  where ((KhachHang.TYPE == 0) || (KhachHang.TYPE != 0 & itm.ID_LOAIPHIEUTHU == KhachHang.ID_LOAIPHIEU))
                                  where (!KhachHang.ISTHEOTHOIGIAN) || (itm.NGAYLAP.Date >= KhachHang.TUNGAY.Date & itm.NGAYLAP.Date <= KhachHang.DENNGAY.Date)
                                  join lpn in _context.dm_LoaiPhieuThu on itm.ID_LOAIPHIEUTHU equals lpn.ID
                                  select new v_ThongKeCongNo_ChiTiet()
                                  {
                                      LOAIPHIEU = "2",
                                      ID_PHIEU = itm.ID,
                                      MAPHIEU = itm.MAPHIEU,
                                      NGAY = itm.NGAYLAP,
                                      DIENGIAI = lpn.NAME + (string.IsNullOrEmpty(itm.LYDO) ? "" : " - " + itm.LYDO),
                                      THU = itm.SOTIEN,
                                  };

                var rlsPhieuXuat = from itm in _context.ct_PhieuXuat
                                   where itm.ID_NGUOITAO == KhachHang.ID_NHANVIEN
                                   where ((KhachHang.TYPE == 0) || (KhachHang.TYPE != 0 & itm.ID_LOAIPHIEUXUAT == KhachHang.ID_LOAIPHIEU))
                                   where (!KhachHang.ISTHEOTHOIGIAN) || (itm.NGAYLAP.Date >= KhachHang.TUNGAY.Date & itm.NGAYLAP.Date <= KhachHang.DENNGAY.Date)
                                   join lpn in _context.dm_LoaiPhieuXuat on itm.ID_LOAIPHIEUXUAT equals lpn.ID
                                   select new v_ThongKeCongNo_ChiTiet()
                                   {
                                       LOAIPHIEU = "1",
                                       ID_PHIEU = itm.ID,
                                       MAPHIEU = itm.MAPHIEU,
                                       NGAY = itm.NGAYLAP,
                                       DIENGIAI = lpn.NAME + (string.IsNullOrEmpty(itm.MAPHIEU) ? "" : " - " + itm.MAPHIEU),
                                       THU = itm.TONGTIEN,
                                   };

                var rlsPhieuChi = from itm in _context.ct_PhieuChi
                                  where itm.ID_NGUOITAO == KhachHang.ID_NHANVIEN
                                  where ((KhachHang.TYPE == 0) || (KhachHang.TYPE != 0 & itm.ID_LOAIPHIEUCHI == KhachHang.ID_LOAIPHIEU))
                                  where (!KhachHang.ISTHEOTHOIGIAN) || (itm.NGAYLAP.Date >= KhachHang.TUNGAY.Date & itm.NGAYLAP.Date <= KhachHang.DENNGAY.Date)
                                  join lpn in _context.dm_LoaiPhieuChi on itm.ID_LOAIPHIEUCHI equals lpn.ID
                                  select new v_ThongKeCongNo_ChiTiet()
                                  {
                                      LOAIPHIEU = "4",
                                      ID_PHIEU = itm.ID,
                                      MAPHIEU = itm.MAPHIEU,
                                      NGAY = itm.NGAYLAP,
                                      DIENGIAI = lpn.NAME + (string.IsNullOrEmpty(itm.LYDO) ? "" : " - " + itm.LYDO),
                                      CHI = itm.SOTIEN,
                                  };
                lstThongKeCongNo_ChiTiet.AddRange(rlsPhieuNhap);
                lstThongKeCongNo_ChiTiet.AddRange(rlsPhieuChi);
                lstThongKeCongNo_ChiTiet.AddRange(rlsPhieuXuat);
                lstThongKeCongNo_ChiTiet.AddRange(rlsPhieuThu);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = lstThongKeCongNo_ChiTiet
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

        // DELETE: api/Input/5
        [HttpDelete("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeleteInput(string LOC_ID, string ID)
        {
            try
            {
                var Input = await _context.ct_PhieuGiaoHang!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (Input == null)
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
                    var lstPhieuNhap_ChiTiet = await _context.ct_PhieuGiaoHang_ChiTiet!.Where(e => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUGIAOHANG == Input.ID).ToListAsync();
                    if (lstPhieuNhap_ChiTiet != null)
                    {
                        foreach (ct_PhieuGiaoHang_ChiTiet itm in lstPhieuNhap_ChiTiet)
                        {
                            _context.ct_PhieuGiaoHang_ChiTiet!.Remove(itm);
                        }
                    }
                    var lstPhieuNhap_NhanVienGiao = await _context.ct_PhieuGiaoHang_NhanVienGiao!.Where(e => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUGIAOHANG == Input.ID).ToListAsync();
                    if (lstPhieuNhap_ChiTiet != null)
                    {
                        foreach (ct_PhieuGiaoHang_NhanVienGiao itm in lstPhieuNhap_NhanVienGiao)
                        {
                            _context.ct_PhieuGiaoHang_NhanVienGiao!.Remove(itm);
                        }
                    }

                    _context.ct_PhieuGiaoHang!.Remove(Input);
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

        private bool InputExistsID(string LOC_ID, string ID)
        {
            return _context.ct_PhieuGiaoHang!.Any(e => e.LOC_ID == LOC_ID && e.ID == ID);
        }

    }
}