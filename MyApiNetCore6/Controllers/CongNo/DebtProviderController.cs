using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseTHP;
using DatabaseTHP.Class;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using MyApiNetCore6.Data;

namespace MyApiNetCore6.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class DebtProviderController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;

        private readonly IConfiguration _configuration;

        public DebtProviderController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<List<v_ThongKeCongNo_ChiTiet>>> PostDetail([FromBody] v_ThongKeCongNoNhaCungCap NhaCungCap)
        {
            try
            {
                List<v_ThongKeCongNo_ChiTiet> lstThongKeCongNo_ChiTiet = new List<v_ThongKeCongNo_ChiTiet>();
                IQueryable<v_ThongKeCongNo_ChiTiet> rlsPhieuNhap = from itm in _context.ct_PhieuNhap
                                                                   where itm.ID_NHACUNGCAP == NhaCungCap.ID
                                                                   where !NhaCungCap.ISTHEOTHOIGIAN || ((itm.NGAYLAP.Date >= NhaCungCap.TUNGAY.Date) & (itm.NGAYLAP.Date <= NhaCungCap.DENNGAY.Date))
                                                                   join lpn in _context.dm_LoaiPhieuNhap on itm.ID_LOAIPHIEUNHAP equals lpn.ID
                                                                   select new v_ThongKeCongNo_ChiTiet
                                                                   {
                                                                       LOAIPHIEU = "2",
                                                                       ID_PHIEU = itm.ID,
                                                                       MAPHIEU = itm.MAPHIEU,
                                                                       NGAY = itm.NGAYLAP,
                                                                       DIENGIAI = lpn.NAME + (string.IsNullOrEmpty(itm.GHICHU) ? "" : (" - " + itm.GHICHU)),
                                                                       THU = itm.TONGTIEN
                                                                   };
                IQueryable<v_ThongKeCongNo_ChiTiet> rlsPhieuXuat = from itm in _context.ct_PhieuXuat
                                                                   where itm.ID_NHACUNGCAP == NhaCungCap.ID
                                                                   where !NhaCungCap.ISTHEOTHOIGIAN || ((itm.NGAYLAP.Date >= NhaCungCap.TUNGAY.Date) & (itm.NGAYLAP.Date <= NhaCungCap.DENNGAY.Date))
                                                                   join lpn in _context.dm_LoaiPhieuXuat on itm.ID_LOAIPHIEUXUAT equals lpn.ID
                                                                   select new v_ThongKeCongNo_ChiTiet
                                                                   {
                                                                       LOAIPHIEU = "4",
                                                                       ID_PHIEU = itm.ID,
                                                                       MAPHIEU = itm.MAPHIEU,
                                                                       NGAY = itm.NGAYLAP,
                                                                       DIENGIAI = lpn.NAME + (string.IsNullOrEmpty(itm.GHICHU) ? "" : (" - " + itm.GHICHU)),
                                                                       CHI = itm.TONGTIEN
                                                                   };
                IQueryable<v_ThongKeCongNo_ChiTiet> rlsPhieuThu = from itm in _context.ct_PhieuThu
                                                                  where itm.ID_NHACUNGCAP == NhaCungCap.ID
                                                                  where !NhaCungCap.ISTHEOTHOIGIAN || ((itm.NGAYLAP.Date >= NhaCungCap.TUNGAY.Date) & (itm.NGAYLAP.Date <= NhaCungCap.DENNGAY.Date))
                                                                  join lpn in _context.dm_LoaiPhieuThu on itm.ID_LOAIPHIEUTHU equals lpn.ID
                                                                  select new v_ThongKeCongNo_ChiTiet
                                                                  {
                                                                      LOAIPHIEU = "1",
                                                                      ID_PHIEU = itm.ID,
                                                                      MAPHIEU = itm.MAPHIEU,
                                                                      NGAY = itm.NGAYLAP,
                                                                      DIENGIAI = lpn.NAME + (string.IsNullOrEmpty(itm.LYDO) ? "" : (" - " + itm.LYDO)),
                                                                      THU = itm.SOTIEN
                                                                  };
                IQueryable<v_ThongKeCongNo_ChiTiet> rlsPhieuChi = from itm in _context.ct_PhieuChi
                                                                  where itm.ID_NHACUNGCAP == NhaCungCap.ID
                                                                  where !NhaCungCap.ISTHEOTHOIGIAN || ((itm.NGAYLAP.Date >= NhaCungCap.TUNGAY.Date) & (itm.NGAYLAP.Date <= NhaCungCap.DENNGAY.Date))
                                                                  join lpn in _context.dm_LoaiPhieuChi on itm.ID_LOAIPHIEUCHI equals lpn.ID
                                                                  select new v_ThongKeCongNo_ChiTiet
                                                                  {
                                                                      LOAIPHIEU = "3",
                                                                      ID_PHIEU = itm.ID,
                                                                      MAPHIEU = itm.MAPHIEU,
                                                                      NGAY = itm.NGAYLAP,
                                                                      DIENGIAI = lpn.NAME + (string.IsNullOrEmpty(itm.LYDO) ? "" : (" - " + itm.LYDO)),
                                                                      CHI = itm.SOTIEN
                                                                  };
                lstThongKeCongNo_ChiTiet.AddRange(rlsPhieuNhap);
                lstThongKeCongNo_ChiTiet.AddRange(rlsPhieuXuat);
                lstThongKeCongNo_ChiTiet.AddRange(rlsPhieuChi);
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
                Exception ex2 = ex;
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex2.Message,
                    Data = ""
                });
            }
        }

        [HttpDelete("{LOC_ID}/{ID}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> DeleteInput(string LOC_ID, string ID)
        {
            try
            {
                ct_PhieuGiaoHang Input = await _context.ct_PhieuGiaoHang.FirstOrDefaultAsync((ct_PhieuGiaoHang e) => e.LOC_ID == LOC_ID && e.ID == ID);
                if (Input == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                using IDbContextTransaction transaction = _context.Database.BeginTransaction();
                List<ct_PhieuGiaoHang_ChiTiet> lstPhieuNhap_ChiTiet = await _context.ct_PhieuGiaoHang_ChiTiet.Where((ct_PhieuGiaoHang_ChiTiet e) => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUGIAOHANG == Input.ID).ToListAsync();
                if (lstPhieuNhap_ChiTiet != null)
                {
                    foreach (ct_PhieuGiaoHang_ChiTiet itm in lstPhieuNhap_ChiTiet)
                    {
                        _context.ct_PhieuGiaoHang_ChiTiet.Remove(itm);
                    }
                }
                List<ct_PhieuGiaoHang_NhanVienGiao> lstPhieuNhap_NhanVienGiao = await _context.ct_PhieuGiaoHang_NhanVienGiao.Where((ct_PhieuGiaoHang_NhanVienGiao e) => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUGIAOHANG == Input.ID).ToListAsync();
                if (lstPhieuNhap_ChiTiet != null)
                {
                    foreach (ct_PhieuGiaoHang_NhanVienGiao itm2 in lstPhieuNhap_NhanVienGiao)
                    {
                        _context.ct_PhieuGiaoHang_NhanVienGiao.Remove(itm2);
                    }
                }
                _context.ct_PhieuGiaoHang.Remove(Input);
                AuditLogController auditLog = new AuditLogController(_context, _configuration);
                auditLog.InserAuditLog();
                await _context.SaveChangesAsync();
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
                Exception ex2 = ex;
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex2.Message,
                    Data = ""
                });
            }
        }

        private bool InputExistsID(string LOC_ID, string ID)
        {
            return _context.ct_PhieuGiaoHang.Any((ct_PhieuGiaoHang e) => e.LOC_ID == LOC_ID && e.ID == ID);
        }
    }
}
