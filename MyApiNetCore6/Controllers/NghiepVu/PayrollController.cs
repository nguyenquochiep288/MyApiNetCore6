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
using DatabaseTHP.StoredProcedure;
using Newtonsoft.Json;

namespace MyApiNetCore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayrollController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public PayrollController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
        }
        [HttpGet("{LOC_ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetArea(string LOC_ID)
        {
            try
            {

                var lstValue = await _context.view_nv_BangLuong!.Where(e => e.LOC_ID == LOC_ID).OrderBy(e => e.NAMTHANG_ORDERBY).ToListAsync();
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

        // GET: api/Area
        [HttpGet("{LOC_ID}/{Type}/{KeyWhere}/{ValuesSearch}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetArea(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
        {
            try
            {
                ValuesSearch = ValuesSearch.Replace("%2f", "/");
                var lstValue = await _context.view_nv_BangLuong!.Where(e => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).OrderBy(e => e.NAMTHANG_ORDERBY).ToListAsync();
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


        //GET: api/Area/5
        [HttpGet("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetArea(string LOC_ID, string ID)
        {
            try
            {
                var Area = await _context.view_nv_BangLuong!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (Area == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                v_nv_BangLuong ct_PhieuDatHang = new v_nv_BangLuong();
                if (Area != null)
                {
                    string strDeposit = JsonConvert.SerializeObject(Area);
                    ct_PhieuDatHang = JsonConvert.DeserializeObject<v_nv_BangLuong>(strDeposit) ?? new v_nv_BangLuong();
                }

                ct_PhieuDatHang.lstnv_BangLuong_ChiTiet = new List<nv_BangLuong_ChiTiet>();
                var lstValue = await _context.nv_BangLuong_ChiTiet!.Where(e => e.LOC_ID == LOC_ID && e.ID_BANGLUONG == ID).ToListAsync();
                if (lstValue != null)
                    ct_PhieuDatHang.lstnv_BangLuong_ChiTiet = lstValue;
                
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = ct_PhieuDatHang
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

        // PUT: api/Area/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PutArea(v_nv_BangLuong Area)
        {
            try
            {
                var Area1 = await _context.view_nv_BangLuong!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.ID_THANGLUONG == Area.ID_THANGLUONG && e.ID != Area.ID);
                if (Area1 != null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại tháng lương " + Area.LOC_ID + "-" + Area.ID_THANGLUONG + " dữ liệu bảng lương!",
                        Data = ""
                    });
                }
                var lstValueChiTiet = await _context.nv_BangLuong_ChiTiet!.AsNoTracking().Where(e => e.LOC_ID == Area.LOC_ID && e.ID_BANGLUONG == Area.ID).ToListAsync();
                using var transaction = _context.Database.BeginTransaction();
                {
                    foreach (var itm in lstValueChiTiet)
                    {
                        if (!(Area.lstnv_BangLuong_ChiTiet.Count(s => s.ID == itm.ID) > 0))
                            _context.nv_BangLuong_ChiTiet!.Remove(itm);
                          
                    }
                    foreach (var itm in Area.lstnv_BangLuong_ChiTiet)
                    {
                        if (!(lstValueChiTiet.Count(s => s.ID == itm.ID) > 0))
                        {
                            itm.ID_BANGLUONG = Area.ID;
                            _context.nv_BangLuong_ChiTiet!.Add(itm);
                        }
                        else
                        {
                            _context.Entry(itm).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        }
                    }
                    Area.TIENLUONG = Area.lstnv_BangLuong_ChiTiet.Where(s => Convert.ToInt32(s.TYPE) > 0).Sum(s => s.SOTIEN);
                    Area.TIENGIAM = Area.lstnv_BangLuong_ChiTiet.Where(s => Convert.ToInt32(s.TYPE) < 0).Sum(s => s.SOTIEN);
                    Area.TIENTHUCNHAN = Area.TIENLUONG - Area.TIENGIAM;
                    _context.Entry(Area).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
                transaction.Commit();
                var view_nv_BangLuong = await _context.view_nv_BangLuong!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID);
                if (view_nv_BangLuong == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + Area.LOC_ID + "-" + Area.ID + " dữ liệu!",
                        Data = ""
                    });
                }
                v_nv_BangLuong ct_PhieuDatHang = new v_nv_BangLuong();
                if (view_nv_BangLuong != null)
                {
                    string strDeposit = JsonConvert.SerializeObject(view_nv_BangLuong);
                    ct_PhieuDatHang = JsonConvert.DeserializeObject<v_nv_BangLuong>(strDeposit) ?? new v_nv_BangLuong();
                }

                ct_PhieuDatHang.lstnv_BangLuong_ChiTiet = new List<nv_BangLuong_ChiTiet>();
                var lstValue = await _context.nv_BangLuong_ChiTiet!.Where(e => e.LOC_ID == Area.LOC_ID && e.ID_BANGLUONG == Area.ID).ToListAsync();
                if (lstValue != null)
                    ct_PhieuDatHang.lstnv_BangLuong_ChiTiet = lstValue;

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = ct_PhieuDatHang
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

        // POST: api/Area
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<dm_KhuVuc>> PostArea(v_nv_BangLuong Area)
        {
            try
            {
                var Area1 = await _context.view_nv_BangLuong!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.ID_THANGLUONG == Area.ID_THANGLUONG);
                if (Area1 != null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại tháng lương " + Area.LOC_ID + "-" + Area.ID_THANGLUONG + " dữ liệu bảng lương!",
                        Data = ""
                    });
                }
                using var transaction = _context.Database.BeginTransaction();
                {
                    foreach (var itm in Area.lstnv_BangLuong_ChiTiet)
                    {
                        _context.nv_BangLuong_ChiTiet!.Add(itm);
                    }
                    Area.TIENLUONG = Area.lstnv_BangLuong_ChiTiet.Where(s => Convert.ToInt32(s.TYPE) > 0).Sum(s => s.SOTIEN);
                    Area.TIENGIAM = Area.lstnv_BangLuong_ChiTiet.Where(s => Convert.ToInt32(s.TYPE) < 0).Sum(s => s.SOTIEN);
                    Area.TIENTHUCNHAN = Area.TIENLUONG - Area.TIENGIAM;
                    _context.nv_BangLuong!.Add(Area);
                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
                transaction.Commit();
                var view_nv_BangLuong = await _context.view_nv_BangLuong!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID);
                if (view_nv_BangLuong == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + Area.LOC_ID + "-" + Area.ID + " dữ liệu!",
                        Data = ""
                    });
                }
                v_nv_BangLuong ct_PhieuDatHang = new v_nv_BangLuong();
                if (view_nv_BangLuong != null)
                {
                    string strDeposit = JsonConvert.SerializeObject(view_nv_BangLuong);
                    ct_PhieuDatHang = JsonConvert.DeserializeObject<v_nv_BangLuong>(strDeposit) ?? new v_nv_BangLuong();
                }

                ct_PhieuDatHang.lstnv_BangLuong_ChiTiet = new List<nv_BangLuong_ChiTiet>();
                var lstValue = await _context.nv_BangLuong_ChiTiet!.Where(e => e.LOC_ID == Area.LOC_ID && e.ID_BANGLUONG == Area.ID).ToListAsync();
                if (lstValue != null)
                    ct_PhieuDatHang.lstnv_BangLuong_ChiTiet = lstValue;

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = ct_PhieuDatHang
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

        // DELETE: api/Area/5
        [HttpDelete("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeleteArea(string LOC_ID, string ID)
        {
            try
            {
                
                var Area = await _context.nv_BangLuong!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (Area == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                var lstValueChiTiet = await _context.nv_BangLuong_ChiTiet!.AsNoTracking().Where(e => e.LOC_ID == LOC_ID && e.ID_BANGLUONG == ID).ToListAsync();
                foreach(var itm in lstValueChiTiet)
                {
                    _context.nv_BangLuong_ChiTiet!.Remove(itm);
                }
                    
                _context.nv_BangLuong!.Remove(Area);
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

        [HttpPost("{LOC_ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<dm_KhuVuc>> GetLuongThang(v_nv_BangLuong Area, string LOC_ID)
        {
            try
            {
                var Area1 = await _context.view_nv_BangLuong!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.ID_THANGLUONG == Area.ID_THANGLUONG);
                if (Area1 != null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại tháng lương " + Area.LOC_ID + "-" + Area.ID_THANGLUONG + " dữ liệu bảng lương!",
                        Data = ""
                    });
                }
                
                var NhanVien = await _context.view_dm_NhanVien!.FirstOrDefaultAsync(e => e.ID == Area.ID_NHANVIEN);
                if (NhanVien == null || string.IsNullOrEmpty(NhanVien.ID_TAIKHOAN)) return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Không tìm thấy nhân viên: " + Area.ID_NHANVIEN,
                    Data = ""
                });
                var PhongBan = await _context.dm_PhongBan!.FirstOrDefaultAsync(e => e.ID == NhanVien.ID_PHONGBAN);
                if (PhongBan == null) return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Không tìm thấy phòng ban: " + NhanVien.ID_PHONGBAN,
                    Data = ""
                });
                var dm_ThangLuong = await _context.dm_ThangLuong!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.ID == Area.ID_THANGLUONG && ("," + e.ID_PHONGBAN + ",").Contains("," + PhongBan.MA + ",") && e.ISACTIVE);
                if (dm_ThangLuong == null) return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Không tìm thấy danh mục tháng lương: " + Area.ID_THANGLUONG + "-" + PhongBan.MA,
                    Data = ""
                });
                var TaiKhoan = await _context.AspNetUsers!.FirstOrDefaultAsync(e => e.ID == NhanVien.ID_TAIKHOAN);
                if (TaiKhoan == null) return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Không tìm thấy tài khoản: " + NhanVien.ID_TAIKHOAN,
                    Data = ""
                });
                
                
                var vdm_BangLuong = await _context.dm_BangLuong!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && (string.IsNullOrEmpty(e.ID_PHONGBAN) || e.ID_PHONGBAN == NhanVien.ID_PHONGBAN));
                if (vdm_BangLuong == null) return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Không tìm thấy danh mục bảng lương cho phòng ban: " + NhanVien.ID_PHONGBAN,
                    Data = ""
                });
                var lstdm_BangLuong_ChiTiet = await _context.dm_BangLuong_ChiTiet!.Where(e => e.LOC_ID == Area.LOC_ID && e.ID_BANGLUONG == vdm_BangLuong.ID).ToListAsync();
                string GhiChuNghiPhep = "";
                string GhiChuNghiKhongPhep = "";
                string GhiChuVeSom = "";
                double SoNgayCong = 0;
                double SoNgayNghiPhep = 0;
                double SoNgayLamViec = 0;
                double SoNgayNghiKhongPhep = 0;
                double SoNgayCoDiLam = 0;
                for (DateTime date = dm_ThangLuong.NGAYBATDAU; date <= dm_ThangLuong.NGAYKETTHUC; date = date.AddDays(1))
                {
                    bool bolDungPhep = false;
                    if (!string.IsNullOrEmpty(dm_ThangLuong.DANHSACHNGAYNGHI) && !("," + dm_ThangLuong.DANHSACHNGAYNGHI +",").Contains("," + date.Day + ","))
                    {
                        var nv_NghiPhep = await _context.nv_NghiPhep!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.THOIGIANRA.Date >= date.Date && e.THOIGIANVAO.Date <= date.Date && e.ID_NHANVIEN == NhanVien.ID);
                        if (nv_NghiPhep != null && nv_NghiPhep.ISNGHIPHEP)
                        {
                            if (string.IsNullOrEmpty(GhiChuNghiPhep))
                                GhiChuNghiPhep = "Nghỉ phép: ";

                            SoNgayCong += 1;
                            if(nv_NghiPhep.HINHTHUCNGHIPHEP  == (int)API.HinhThucNghiPhep.NguyenNgay)
                            {
                                SoNgayNghiPhep += 1;
                                GhiChuNghiPhep += date.Day + "("+ (nv_NghiPhep.ISDUYETPHEP ? "Duyệt" : "Chưa duyệt") + " 1 ngày);";
                                continue;
                            }
                            else
                            {
                                SoNgayNghiPhep += 0.5;
                                GhiChuNghiPhep += date.Day + "("+ (nv_NghiPhep.ISDUYETPHEP ? "Duyệt" : "Chưa duyệt") + " 0.5 ngày);";
                                bolDungPhep = true;
                            }    
                        }
                        var nv_ChamCong = await _context.nv_ChamCong!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.NGAYCONG.Date == date.Date);
                        if (nv_ChamCong != null)
                        {
                            TimeSpan GIOBATDAU = dm_ThangLuong.GIOBATDAU;
                            TimeSpan GIOKETTHUC = dm_ThangLuong.GIOKETTHUC;
                            double SoGioLe = 0;
                            if (nv_ChamCong.THOIGIANVAO != null && nv_ChamCong.THOIGIANRA != null)
                            {
                                if (nv_ChamCong.THOIGIANVAO != null)
                                {
                                    if (nv_ChamCong.THOIGIANVAO.Value.TimeOfDay > dm_ThangLuong.GIOBATDAU)
                                    {
                                        GIOBATDAU = nv_ChamCong.THOIGIANVAO.Value.TimeOfDay;
                                        if (!bolDungPhep)
                                        {
                                            if (string.IsNullOrEmpty(GhiChuVeSom))
                                                GhiChuVeSom = "Đi trễ, về sớm: ";
                                            GhiChuVeSom += "(v1) " + nv_ChamCong.THOIGIANVAO.Value.ToString("dd HH:mm") + ";";
                                        }
                                    }
                                    else
                                    {
                                        SoGioLe = 1;
                                    }
                                }
                                if (nv_ChamCong.THOIGIANRA != null)
                                {
                                    if (nv_ChamCong.THOIGIANRA.Value.TimeOfDay < dm_ThangLuong.GIOKETTHUC)
                                    {
                                        GIOKETTHUC = nv_ChamCong.THOIGIANRA.Value.TimeOfDay;
                                        if (!bolDungPhep)
                                        {
                                            if (string.IsNullOrEmpty(GhiChuVeSom))
                                                GhiChuVeSom = "Đi trễ, về sớm: ";
                                            GhiChuVeSom += "(v2) " + nv_ChamCong.THOIGIANRA.Value.ToString("dd HH:mm") + ";";
                                        }
                                    }
                                    else
                                    {
                                        SoGioLe = 1;
                                    }
                                }
                                double SoGioLamViec = (GIOKETTHUC - GIOBATDAU).TotalHours;
                                TimeSpan SoTiengLamTrongNgay = dm_ThangLuong.GIOKETTHUC - dm_ThangLuong.GIOBATDAU;
                                double SoGioNghiTrua = (dm_ThangLuong.GIOKETTHUC_NGHITRUA - dm_ThangLuong.GIOBATDAU_NGHITRUA).TotalHours;
                                double SoGioNghiTruaTrongNgay = (dm_ThangLuong.GIOKETTHUC_NGHITRUA - dm_ThangLuong.GIOBATDAU_NGHITRUA).TotalHours;
                                // 10h < 12h - 13h <= 13h30 (10h - 13h)
                                if (GIOBATDAU < dm_ThangLuong.GIOBATDAU_NGHITRUA && GIOKETTHUC <= dm_ThangLuong.GIOKETTHUC_NGHITRUA)
                                    SoGioNghiTrua = (GIOKETTHUC - dm_ThangLuong.GIOBATDAU_NGHITRUA).TotalHours;

                                // 13h > 13h30 - 15h > 13h30 (13h - 15h)
                                if (GIOBATDAU > dm_ThangLuong.GIOKETTHUC_NGHITRUA && GIOKETTHUC > dm_ThangLuong.GIOKETTHUC_NGHITRUA)
                                    SoGioNghiTrua = (dm_ThangLuong.GIOKETTHUC_NGHITRUA - dm_ThangLuong.GIOBATDAU_NGHITRUA).TotalHours;

                                // 8h < 12h - 11h <= 12h (8h - 11h)
                                if (GIOBATDAU < dm_ThangLuong.GIOBATDAU_NGHITRUA && GIOKETTHUC <= dm_ThangLuong.GIOBATDAU_NGHITRUA)
                                    SoGioNghiTrua = 0;

                                // 14h >= 13h30
                                if (GIOBATDAU >= dm_ThangLuong.GIOKETTHUC_NGHITRUA)
                                    SoGioNghiTrua = 0;

                                SoGioLe = (SoGioLamViec - SoGioNghiTrua) / (SoTiengLamTrongNgay.TotalHours - SoGioNghiTruaTrongNgay);
                                if (bolDungPhep)
                                    SoGioLe = 0.5;

                            }
                            else
                            {
                                if (string.IsNullOrEmpty(GhiChuVeSom))
                                    GhiChuVeSom = "Đi trễ, về sớm: ";
                                GhiChuVeSom += "(v1v2) " + date.ToString("dd") + ";";
                            }

                            SoNgayCong += SoGioLe;
                            SoNgayLamViec += SoGioLe;
                            SoNgayCoDiLam += 1;
                        }
                        else
                        {
                            if(!dm_ThangLuong.ISCHAMCONG && !bolDungPhep)
                            {
                                SoNgayCong += 1;
                                SoNgayLamViec += 1;
                                SoNgayCoDiLam += 1;
                            }    
                        }

                        if((nv_NghiPhep == null && nv_ChamCong == null && dm_ThangLuong.ISCHAMCONG) || (nv_NghiPhep != null && !nv_NghiPhep.ISNGHIPHEP))
                        {
                            SoNgayNghiKhongPhep += 1;
                            if (string.IsNullOrEmpty(GhiChuNghiKhongPhep))
                                GhiChuNghiKhongPhep = "Nghỉ không phép: ";
                            GhiChuNghiKhongPhep +=  "Ngày "+ date.Day + ((nv_NghiPhep != null && nv_NghiPhep.ISDUYETPHEP) ? " Đã duyệt" : ((nv_NghiPhep != null) ? " Chưa duyệt nghỉ" : " Chưa xin nghỉ")) + ";";
                        }
                        else
                        {

                        }    
                    }
                }

                Area.MUCLUONG = NhanVien.LUONGCOBAN;
                Area.SONGAYCONG = dm_ThangLuong.SONGAYCONG;
                Area.SONGAYLAMVIEC = Math.Round(SoNgayLamViec, 2);
                Area.SONGAYNGHIPHEP = SoNgayNghiPhep;
                Area.SONGAYNGHIKHONGPHEP = SoNgayNghiKhongPhep;
                Area.GHICHU = GhiChuNghiPhep + (string.IsNullOrEmpty(GhiChuNghiKhongPhep) ? "" : Environment.NewLine + GhiChuNghiKhongPhep) + (string.IsNullOrEmpty(GhiChuVeSom) ? "" : Environment.NewLine + GhiChuVeSom) ;
                //Area.TIENLUONG = NhanVien.LUONGCOBAN;
                if (lstdm_BangLuong_ChiTiet != null)
                {
                    Area.lstnv_BangLuong_ChiTiet = new List<nv_BangLuong_ChiTiet>();
                    foreach (var itm in lstdm_BangLuong_ChiTiet)
                    {
                        nv_BangLuong_ChiTiet newnv_BangLuong_ChiTiet = new nv_BangLuong_ChiTiet();
                        newnv_BangLuong_ChiTiet.ID_LOAILUONG = itm.ID_LOAILUONG;
                        newnv_BangLuong_ChiTiet.ID_BANGLUONG = Area.ID;
                        newnv_BangLuong_ChiTiet.ID = Guid.NewGuid().ToString();
                        if (itm.TYPE_QUYTACTINHLUONG == (int)API.QuyTacTinhLuong.ThoiGianLamViec)
                        {
                            if (itm.TYPE_LUONG == (int)API.Luong.Luong)
                            {
                                newnv_BangLuong_ChiTiet.SOTIEN = Math.Ceiling(((itm.SOTIEN > 0 ? itm.SOTIEN : NhanVien.LUONGCOBAN) / dm_ThangLuong.SONGAYCONG) * (Area.SONGAYLAMVIEC + SoNgayNghiPhep));
                            }
                            if (itm.TYPE_LUONG == (int)API.Luong.KPI)
                            {
                                DatabaseTHP.StoredProcedure.Parameter.SP_Parameter SP_Parameter = new DatabaseTHP.StoredProcedure.Parameter.SP_Parameter();
                                KPI_SaleController KPI = new KPI_SaleController(_context, _configuration);
                                SP_Parameter.LOC_ID = Area.LOC_ID;
                                SP_Parameter.TUNGAY = dm_ThangLuong.NGAYBATDAU;
                                SP_Parameter.DENNGAY = dm_ThangLuong.NGAYKETTHUC;
                                SP_Parameter.ID_NHANVIEN = NhanVien.ID;
                                var KetQua = await KPI.PutProduct(SP_Parameter);

                                var okResult = KetQua as OkObjectResult;
                                if (okResult != null)
                                {
                                    var ApiResponse = okResult.Value as ApiResponse;
                                    if (ApiResponse != null)
                                    {

                                        if (ApiResponse.Data != null)
                                        {
                                            var lst_ChiTiet = ApiResponse.Data as List<v_Tinh_KPI_KinhDoanh>;
                                            newnv_BangLuong_ChiTiet.SOTIEN = Math.Ceiling(((itm.SOTIEN > 0 ? itm.SOTIEN : (lst_ChiTiet != null && lst_ChiTiet.Count > 0 ? lst_ChiTiet[0].SOTIEN_KPI : 0)) / dm_ThangLuong.SONGAYCONG) * (Area.SONGAYLAMVIEC + SoNgayNghiPhep));
                                        }
                                    }
                                }
                            }
                            if (itm.TYPE_LUONG == (int)API.Luong.Khac)
                            {
                                newnv_BangLuong_ChiTiet.SOTIEN = Math.Ceiling((itm.SOTIEN / dm_ThangLuong.SONGAYCONG) * (Area.SONGAYLAMVIEC + SoNgayNghiPhep));
                            }
                        }
                        else if (itm.TYPE_QUYTACTINHLUONG == (int)API.QuyTacTinhLuong.Ngay)
                        {
                            if (itm.TYPE_LUONG == (int)API.Luong.Luong)
                            {
                                newnv_BangLuong_ChiTiet.SOTIEN = Math.Ceiling(((itm.SOTIEN > 0 ? itm.SOTIEN : NhanVien.LUONGCOBAN) / dm_ThangLuong.SONGAYCONG) * (SoNgayCoDiLam));
                            }
                            if (itm.TYPE_LUONG == (int)API.Luong.KPI)
                            {
                                DatabaseTHP.StoredProcedure.Parameter.SP_Parameter SP_Parameter = new DatabaseTHP.StoredProcedure.Parameter.SP_Parameter();
                                KPI_SaleController KPI = new KPI_SaleController(_context, _configuration);
                                SP_Parameter.LOC_ID = Area.LOC_ID;
                                SP_Parameter.TUNGAY = dm_ThangLuong.NGAYBATDAU;
                                SP_Parameter.DENNGAY = dm_ThangLuong.NGAYKETTHUC;
                                SP_Parameter.ID_NHANVIEN = NhanVien.ID;
                                var KetQua = await KPI.PutProduct(SP_Parameter);

                                var okResult = KetQua as OkObjectResult;
                                if (okResult != null)
                                {
                                    var ApiResponse = okResult.Value as ApiResponse;
                                    if (ApiResponse != null)
                                    {

                                        if (ApiResponse.Data != null)
                                        {
                                            var lst_ChiTiet = ApiResponse.Data as List<v_Tinh_KPI_KinhDoanh>;
                                            newnv_BangLuong_ChiTiet.SOTIEN = Math.Ceiling(((itm.SOTIEN > 0 ? itm.SOTIEN : (lst_ChiTiet != null && lst_ChiTiet.Count > 0 ? lst_ChiTiet[0].SOTIEN_KPI : 0)) / dm_ThangLuong.SONGAYCONG) * (SoNgayCoDiLam));
                                        }
                                    }
                                }
                                
                            }
                            if (itm.TYPE_LUONG == (int)API.Luong.Khac)
                            {
                                newnv_BangLuong_ChiTiet.SOTIEN = Math.Ceiling((itm.SOTIEN / dm_ThangLuong.SONGAYCONG) * (SoNgayCoDiLam));
                            }
                        }
                        else if (itm.TYPE_QUYTACTINHLUONG == (int)API.QuyTacTinhLuong.Thang)
                        {
                            if (itm.TYPE_LUONG == (int)API.Luong.Luong)
                            {
                                newnv_BangLuong_ChiTiet.SOTIEN = Math.Ceiling(itm.SOTIEN > 0 ? itm.SOTIEN : NhanVien.LUONGCOBAN);
                            }
                            if (itm.TYPE_LUONG == (int)API.Luong.KPI)
                            {
                                DatabaseTHP.StoredProcedure.Parameter.SP_Parameter SP_Parameter = new DatabaseTHP.StoredProcedure.Parameter.SP_Parameter();
                                KPI_SaleController KPI = new KPI_SaleController(_context, _configuration);
                                SP_Parameter.LOC_ID = Area.LOC_ID;
                                SP_Parameter.TUNGAY = dm_ThangLuong.NGAYBATDAU;
                                SP_Parameter.DENNGAY = dm_ThangLuong.NGAYKETTHUC;
                                SP_Parameter.ID_NHANVIEN = NhanVien.ID;
                                var KetQua = await KPI.PutProduct(SP_Parameter);

                                var okResult = KetQua as OkObjectResult;
                                if (okResult != null)
                                {
                                    var ApiResponse = okResult.Value as ApiResponse;
                                    if (ApiResponse != null)
                                    {

                                        if (ApiResponse.Data != null)
                                        {
                                            var lst_ChiTiet = ApiResponse.Data as List<v_Tinh_KPI_KinhDoanh>;
                                            newnv_BangLuong_ChiTiet.SOTIEN = Math.Ceiling(((itm.SOTIEN > 0 ? itm.SOTIEN : (lst_ChiTiet != null && lst_ChiTiet.Count > 0 ? lst_ChiTiet[0].SOTIEN_KPI : 0))));
                                        }
                                    }
                                }
                                
                            }
                            if (itm.TYPE_LUONG == (int)API.Luong.Khac)
                            {
                                newnv_BangLuong_ChiTiet.SOTIEN = (itm.SOTIEN);
                            }
                        }
                        var dm_LoaiLuong = await _context.dm_LoaiLuong!.FirstOrDefaultAsync(e => e.LOC_ID == Area.LOC_ID && e.ID == itm.ID_LOAILUONG);
                        newnv_BangLuong_ChiTiet.TYPE = dm_LoaiLuong != null ? dm_LoaiLuong.TYPE.ToString() : "0";
                        Area.lstnv_BangLuong_ChiTiet.Add(newnv_BangLuong_ChiTiet);
                    }
                }
                Area.TIENLUONG = Math.Ceiling(Area.lstnv_BangLuong_ChiTiet.Where(s => Convert.ToInt32(s.TYPE) > 0).Sum(s => s.SOTIEN));
                Area.TIENGIAM = Math.Ceiling(Area.lstnv_BangLuong_ChiTiet.Where(s => Convert.ToInt32(s.TYPE) < 0).Sum(s => s.SOTIEN));
                Area.TIENTHUCNHAN = Area.TIENLUONG - Area.TIENGIAM;
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = Area
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

        [HttpPost("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<dm_KhuVuc>> GetPhieuIn(string LOC_ID, string ID)
        {
            try
            {
                var lstValue = await _context.view_nv_BangLuong_ChiTiet!.Where(e => e.LOC_ID == LOC_ID && e.ID_BANGLUONG == ID).ToListAsync();
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
    }
}