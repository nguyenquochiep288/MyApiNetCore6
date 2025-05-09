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

namespace MyApiNetCore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DebtEmployeeController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public DebtEmployeeController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
        }

        // POST: api/Input
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<List<v_ThongKeCongNo_ChiTiet>>> PostDetail([FromBody] v_ThongKeCongNoNhanVien NhaCungCap)
        {
            try
            {
                List<v_ThongKeCongNo_ChiTiet> lstThongKeCongNo_ChiTiet = new List<v_ThongKeCongNo_ChiTiet>();
                var rlsPhieuNhap = from itm in _context.ct_PhieuNhap
                                   where itm.ID_NHANVIEN == NhaCungCap.ID
                                   where (!NhaCungCap.ISTHEOTHOIGIAN) || (itm.NGAYLAP.Date >= NhaCungCap.TUNGAY.Date & itm.NGAYLAP.Date <= NhaCungCap.DENNGAY.Date)
                                   join lpn in _context.dm_LoaiPhieuNhap on itm.ID_LOAIPHIEUNHAP equals lpn.ID
                                   select new v_ThongKeCongNo_ChiTiet()
                                   {
                                       LOAIPHIEU = "2",
                                       ID_PHIEU = itm.ID,
                                       MAPHIEU = itm.MAPHIEU,
                                       NGAY = itm.NGAYLAP,
                                       DIENGIAI = lpn.NAME + (string.IsNullOrEmpty(itm.GHICHU) ? "" : " - " + itm.GHICHU),
                                       THU = itm.TONGTIEN,
                                   };

                var rlsBangLuong = from itm in _context.nv_BangLuong
                                   where itm.ID_NHANVIEN == NhaCungCap.ID 
                                   where itm.ISTINHLUONG == true
                                   where (!NhaCungCap.ISTHEOTHOIGIAN) || (itm.NGAYLAP.Date >= NhaCungCap.TUNGAY.Date & itm.NGAYLAP.Date <= NhaCungCap.DENNGAY.Date)
                                   join lpn in _context.dm_ThangLuong on itm.ID_THANGLUONG equals lpn.ID
                                   select new v_ThongKeCongNo_ChiTiet()
                                   {
                                       LOAIPHIEU = "2",
                                       ID_PHIEU = itm.ID,
                                       MAPHIEU = itm.MAPHIEU,
                                       NGAY = itm.NGAYLAP,
                                       DIENGIAI = lpn.NAME + (string.IsNullOrEmpty(itm.GHICHU) ? "" : " - " + itm.GHICHU),
                                       THU = itm.TIENTHUCNHAN,
                                   };

                var rlsPhieuXuat = from itm in _context.ct_PhieuXuat
                                   where itm.ID_NHANVIEN == NhaCungCap.ID
                                   where (!NhaCungCap.ISTHEOTHOIGIAN) || (itm.NGAYLAP.Date >= NhaCungCap.TUNGAY.Date & itm.NGAYLAP.Date <= NhaCungCap.DENNGAY.Date)
                                   join lpn in _context.dm_LoaiPhieuXuat on itm.ID_LOAIPHIEUXUAT equals lpn.ID
                                   select new v_ThongKeCongNo_ChiTiet()
                                   {
                                       LOAIPHIEU = "4",
                                       ID_PHIEU = itm.ID,
                                       MAPHIEU = itm.MAPHIEU,
                                       NGAY = itm.NGAYLAP,
                                       DIENGIAI = lpn.NAME + (string.IsNullOrEmpty(itm.GHICHU) ? "" : " - " + itm.GHICHU),
                                       CHI = itm.TONGTIEN,
                                   };

                var rlsPhieuThu = from itm in _context.ct_PhieuThu
                                  where itm.ID_NHANVIEN == NhaCungCap.ID
                                  where (!NhaCungCap.ISTHEOTHOIGIAN) || (itm.NGAYLAP.Date >= NhaCungCap.TUNGAY.Date & itm.NGAYLAP.Date <= NhaCungCap.DENNGAY.Date)
                                  join lpn in _context.dm_LoaiPhieuThu on itm.ID_LOAIPHIEUTHU equals lpn.ID
                                  select new v_ThongKeCongNo_ChiTiet()
                                  {
                                      LOAIPHIEU = "1",
                                      ID_PHIEU = itm.ID,
                                      MAPHIEU = itm.MAPHIEU,
                                      NGAY = itm.NGAYLAP,
                                      DIENGIAI = lpn.NAME + (string.IsNullOrEmpty(itm.LYDO) ? "" : " - " + itm.LYDO),
                                      THU = itm.SOTIEN,
                                  };

                var rlsPhieuChi = from itm in _context.ct_PhieuChi
                                  where itm.ID_NHANVIEN == NhaCungCap.ID
                                  where (!NhaCungCap.ISTHEOTHOIGIAN) || (itm.NGAYLAP.Date >= NhaCungCap.TUNGAY.Date & itm.NGAYLAP.Date <= NhaCungCap.DENNGAY.Date)
                                  join lpn in _context.dm_LoaiPhieuChi on itm.ID_LOAIPHIEUCHI equals lpn.ID
                                  select new v_ThongKeCongNo_ChiTiet()
                                  {
                                      LOAIPHIEU = "3",
                                      ID_PHIEU = itm.ID,
                                      MAPHIEU = itm.MAPHIEU,
                                      NGAY = itm.NGAYLAP,
                                      DIENGIAI = lpn.NAME + (string.IsNullOrEmpty(itm.LYDO) ? "" : " - " + itm.LYDO),
                                      CHI = itm.SOTIEN,
                                  };

                lstThongKeCongNo_ChiTiet.AddRange(rlsPhieuNhap);
                lstThongKeCongNo_ChiTiet.AddRange(rlsPhieuXuat);
                lstThongKeCongNo_ChiTiet.AddRange(rlsPhieuChi);
                lstThongKeCongNo_ChiTiet.AddRange(rlsPhieuThu);
                lstThongKeCongNo_ChiTiet.AddRange(rlsBangLuong);
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

    }
}