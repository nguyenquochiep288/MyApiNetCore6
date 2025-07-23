using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyApiNetCore6.Data;
using MyApiNetCore6.Repositories;
using System.Data.Common;
using System.Data;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using DatabaseTHP.Class;
using System;
using DatabaseTHP.StoredProcedure.Parameter;
using DatabaseTHP;
using Microsoft.AspNetCore.Authorization;
using DatabaseTHP.StoredProcedure;
using MyApiNetCore6.Models;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;
using MyApiNetCore6.Class;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Components.Web;

namespace MyApiNetCore6.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ExecuteStoredProc : Controller
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;

        public ExecuteStoredProc(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost(API.web_Sp_Get_DSKhachHang)]
        public async Task<IActionResult> web_Sp_Get_DSKhachHang(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_dm_KhachHang>(API.web_Sp_Get_DSKhachHang, procParams));
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

        [HttpPost(API.web_Sp_Get_DSNhomSanPham)]
        public async Task<IActionResult> web_Sp_Get_DSNhomSanPham(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<web_Sp_Get_DSNhomSanPham_Result>(API.web_Sp_Get_DSNhomSanPham, procParams));
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

        [HttpPost(API.web_Sp_Get_DSSanPham)]
        public async Task<IActionResult> web_Sp_Get_DSSanPham(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<web_Sp_Get_DSSanPham_Result>(API.web_Sp_Get_DSSanPham, procParams));
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


        [HttpPost(API.web_Sp_Get_SanPham)]
        public async Task<IActionResult> web_Sp_Get_SanPham(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<web_Sp_Get_SanPham_Result>(API.web_Sp_Get_SanPham, procParams));
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

        [HttpPost(API.web_Sp_Get_SanPham_Combo)]
        public async Task<IActionResult> web_Sp_Get_DSSanPham_Combo(SP_Parameter sp_Parameter)
        {
            try
            {

                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<web_Sp_Get_SanPham_Combo_Result>(API.web_Sp_Get_SanPham_Combo, procParams));
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


        [HttpPost(API.web_Sp_Get_SanPham_Group)]
        public async Task<IActionResult> web_Sp_Get_DSSanPham_Group(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<web_Sp_Get_SanPham_Group_Result>(API.web_Sp_Get_SanPham_Group, procParams));
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


        [HttpPost(API.web_Sp_Get_DanhSachKhuyenMai)]
        public async Task<IActionResult> web_Sp_Get_DanhSachKhuyenMai(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<web_Sp_Get_DanhSachKhuyenMai_Result>(API.web_Sp_Get_DanhSachKhuyenMai, procParams));
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


        [HttpPost(API.web_Sp_Get_DanhSachChietKhauHoaDon)]
        public async Task<IActionResult> web_Sp_Get_DanhSachChietKhauHoaDon(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<web_Sp_Get_DanhSachChietKhauHoaDon_Result>(API.web_Sp_Get_DanhSachChietKhauHoaDon, procParams));
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


        [HttpPost(API.web_Sp_Get_DSKhuVuc)]
        public async Task<IActionResult> web_Sp_Get_DSKhuVuc(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<web_Sp_Get_DSKhuVuc_Result>(API.web_Sp_Get_DSKhuVuc, procParams));
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

        [HttpPost(API.Sp_Get_BaoCaoTheoNhanVien)]
        public async Task<IActionResult> Sp_Get_BaoCaoTheoNhanVien(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<Sp_Get_BaoCaoTheoNhanVien_Result>(API.Sp_Get_BaoCaoTheoNhanVien, procParams));
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


        [HttpPost(API.Sp_Get_BaoCaoTaiChinh_New)]
        public async Task<IActionResult> Sp_Get_BaoCaoTaiChinh_New(SP_Parameter_Report sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
                return Ok(await Execute_StoredProc<Sp_Get_BaoCaoTaiChinh_New_Result>(API.Sp_Get_BaoCaoTaiChinh_New, procParams));
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

        #region Sản phẩm
        [HttpPost(API.Sp_Get_DanhSachSanPhamKho)]
        public async Task<IActionResult> Sp_Get_DanhSachSanPhamKho(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<Product_Detail>(API.Sp_Get_DanhSachSanPhamKho, procParams));
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


        [HttpPost(API.Sp_Get_DanhSachSanPhamKho_Combo)]
        public async Task<IActionResult> Sp_Get_DanhSachSanPhamKho_Combo(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<Product_Detail>(API.Sp_Get_DanhSachSanPhamKho_Combo, procParams));
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
        #endregion

        #region Báo cáo giao hàng
        [HttpPost(API.Sp_Get_BaoCaoGiaoHang)]
        public async Task<IActionResult> Sp_Get_BaoCaoGiaoHang(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<Sp_Get_BaoCaoGiaoHang_Result>(API.Sp_Get_BaoCaoGiaoHang, procParams));
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

        [HttpPost(API.Sp_Get_BaoCaoGiaoHang_ChiTiet)]
        public async Task<IActionResult> Sp_Get_BaoCaoGiaoHang_ChiTiet(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ThongKeCongNo_ChiTiet>(API.Sp_Get_BaoCaoGiaoHang_ChiTiet, procParams));
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
        #endregion

        #region Báo cáo tài chính
        [HttpPost(API.Sp_Get_BaoCaoTaiChinh)]
        public async Task<IActionResult> Sp_Get_BaoCaoTaiChinh(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<Sp_Get_BaoCaoTaiChinh_Result>(API.Sp_Get_BaoCaoTaiChinh, procParams));
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
        #endregion

        #region Phiếu nhập, chi tiết phiếu nhập
        [HttpPost(API.Sp_Get_DanhSachPhieuNhap)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuNhap(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ct_PhieuNhap>(API.Sp_Get_DanhSachPhieuNhap, procParams));
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

        [HttpPost(API.Sp_Get_DanhSachPhieuNhap_PhieuGiaoHang)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuNhap_PhieuGiaoHang(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ct_PhieuNhap>(API.Sp_Get_DanhSachPhieuNhap_PhieuGiaoHang, procParams));
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

        [HttpPost(API.Sp_Get_DanhSachPhieuNhap_Chitiet)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuNhap_Chitiet(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ct_PhieuNhap_ChiTiet>(API.Sp_Get_DanhSachPhieuNhap_Chitiet, procParams));
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
        #endregion

        #region Phiếu đặt hàng NCC
        [HttpPost(API.Sp_Get_DanhSachPhieuDatHangNCC)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuDatHangNCC(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ct_PhieuDatHangNCC>(API.Sp_Get_DanhSachPhieuDatHangNCC, procParams));
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

        [HttpPost(API.Sp_Get_DanhSachPhieuDatHangNCC_Chitiet)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuDatHangNCC_Chitiet(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ct_PhieuDatHangNCC_ChiTiet>(API.Sp_Get_DanhSachPhieuDatHangNCC_Chitiet, procParams));
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
        #endregion

        #region Phiếu danh sách phiếu chấm công
        [HttpPost(API.Sp_Get_DanhSachChamCong)]
        public async Task<IActionResult> Sp_Get_DanhSachChamCong(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_nv_ChamCong>(API.Sp_Get_DanhSachChamCong, procParams));
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
        #endregion

        #region Phiếu danh sách phiếu nghỉ phép
        [HttpPost(API.Sp_Get_DanhSachNghiPhep)]
        public async Task<IActionResult> Sp_Get_DanhSachNghiPhep(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_nv_NghiPhep>(API.Sp_Get_DanhSachNghiPhep, procParams));
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
        #endregion

        #region Phiếu điều hàng tạm, chi tiết phiếu điều hàng
        [HttpPost(API.Sp_Get_DanhSachPhieuDatHang)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuDatHang(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ct_PhieuDatHang>(API.Sp_Get_DanhSachPhieuDatHang, procParams));
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

        [HttpPost(API.Sp_Get_DanhSachPhieuDatHang_ChiTiet)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuDatHang_ChiTiet(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ct_PhieuDatHang_ChiTiet>(API.Sp_Get_DanhSachPhieuDatHang_ChiTiet, procParams));
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

        [HttpPost(API.Sp_Get_BaoCaoPhieuDatHang)]
        public async Task<IActionResult> Sp_Get_BaoCaoPhieuDatHang(SP_Parameter_Report sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ct_PhieuDatHang_ChiTiet_BaoCao>(API.Sp_Get_DanhSachPhieuDatHang_ChiTiet_BaoCao, procParams));
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

        [HttpPost(API.Sp_Get_DanhSachPhieuNhap_ChiTiet_BaoCao)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuNhap_ChiTiet_BaoCao(SP_Parameter_Report sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ct_PhieuDatHang_ChiTiet_BaoCao>(API.Sp_Get_DanhSachPhieuNhap_ChiTiet_BaoCao, procParams));
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

        [HttpPost(API.Sp_Get_DanhSachPhieuNhapTraHang_ChiTiet_BaoCao)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuNhapTraHang_ChiTiet_BaoCao(SP_Parameter_Report sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ct_PhieuDatHang_ChiTiet_BaoCao>(API.Sp_Get_DanhSachPhieuNhapTraHang_ChiTiet_BaoCao, procParams));
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

        [HttpPost(API.Sp_Get_DanhSachPhieuGiaoHang_ChiTiet_BaoCao)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuGiaoHang_ChiTiet_BaoCao(SP_Parameter_Report sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ct_PhieuDatHang_ChiTiet_BaoCao>(API.Sp_Get_DanhSachPhieuGiaoHang_ChiTiet_BaoCao, procParams));
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
        #endregion

        #region Phiếu nhập, chi tiết phiếu xuất
        [HttpPost(API.Sp_Get_DanhSachPhieuXuat_PhieuGiaoHang)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuXuat_PhieuGiaoHang(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ct_PhieuXuat>(API.Sp_Get_DanhSachPhieuXuat_PhieuGiaoHang, procParams));
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

        [HttpPost(API.Sp_Get_DanhSachPhieuXuat)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuXuat(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ct_PhieuXuat>(API.Sp_Get_DanhSachPhieuXuat, procParams));
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

        [HttpPost(API.Sp_Get_DanhSachPhieuXuat_TimKiem)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuXuat_TimKiem(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ct_PhieuXuat>(API.Sp_Get_DanhSachPhieuXuat_TimKiem, procParams));
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

        [HttpPost(API.Sp_Get_DanhSachPhieuXuat_Chitiet)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuXuat_Chitiet(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ct_PhieuXuat_ChiTiet>(API.Sp_Get_DanhSachPhieuXuat_Chitiet, procParams));
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

        [HttpPost(API.Sp_Get_DanhSachPhieuGiaoHang_In)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuXuat_In(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<Sp_Get_DanhSachPhieuGiaoHang_In>(API.Sp_Get_DanhSachPhieuGiaoHang_In, procParams));
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

        [HttpPost(API.Sp_Get_DanhSachPhieuGiaoHang_InPhieuGiao)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuGiaoHang_InPhieuGiao(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<Sp_Get_DanhSachPhieuGiaoHang_In>(API.Sp_Get_DanhSachPhieuGiaoHang_InPhieuGiao, procParams));
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
        #endregion

        #region Phiếu giao hàng, chi tiết phiếu giao hàng
        [HttpPost(API.Sp_Get_DanhSachPhieuGiaoHang)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuGiaoHang(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ct_PhieuGiaoHang>(API.Sp_Get_DanhSachPhieuGiaoHang, procParams));
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

        [HttpPost(API.Sp_Get_DanhSachPhieuGiaoHang_ChiTiet)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuGiaoHang_ChiTiet(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ct_PhieuGiaoHang_ChiTiet>(API.Sp_Get_DanhSachPhieuGiaoHang_ChiTiet, procParams));
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

        [HttpPost(API.Sp_Get_DanhSachPhieuGiaoHang_NhanVienGiao)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuGiaoHang_NhanVienGiao(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ct_PhieuGiaoHang_NhanVienGiao>(API.Sp_Get_DanhSachPhieuGiaoHang_NhanVienGiao, procParams));
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

        [HttpPost(API.Sp_Get_DanhSachPhieuGiaoHang_PhieuXuat)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuGiaoHang_PhieuXuat(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ct_PhieuGiaoHang>(API.Sp_Get_DanhSachPhieuGiaoHang_PhieuXuat, procParams));
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
        #endregion

        #region Phiêu thu
        [HttpPost(API.Sp_Get_DanhSachPhieuThu)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuThu(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ct_PhieuThu>(API.Sp_Get_DanhSachPhieuThu, procParams));
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

        [HttpPost(API.Sp_Get_DanhSachPhieuThu_PhieuGiaoHang)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuThu_PhieuGiaoHang(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ct_PhieuThu>(API.Sp_Get_DanhSachPhieuThu_PhieuGiaoHang, procParams));
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
        #endregion

        #region Phiêu chi
        [HttpPost(API.Sp_Get_DanhSachPhieuChi)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuChi(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ct_PhieuChi>(API.Sp_Get_DanhSachPhieuChi, procParams));
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

        [HttpPost(API.Sp_Get_DanhSachPhieuChi_PhieuGiaoHang)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuChi_PhieuGiaoHang(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ct_PhieuChi>(API.Sp_Get_DanhSachPhieuChi_PhieuGiaoHang, procParams));
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
        #endregion

        #region Chương trình khuyến mãi
        [HttpPost(API.Sp_Get_ChuongTrinhKhuyenMai)]
        public async Task<IActionResult> Sp_Get_ChuongTrinhKhuyenMai(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_dm_ChuongTrinhKhuyenMai>(API.Sp_Get_ChuongTrinhKhuyenMai, procParams));
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
        #endregion

        #region Chương trình khuyến mãi Sp_Get_DanhSachPhieuXuatHang_KhuyenMai
        [HttpPost(API.Sp_Get_DanhSachPhieuXuatHang_KhuyenMai)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuXuatHang_KhuyenMai(SP_Parameter_Report sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
                return Ok(await Execute_StoredProc<Sp_Get_DanhSachPhieuXuatHang_KhuyenMai>(API.Sp_Get_DanhSachPhieuXuatHang_KhuyenMai, procParams));
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
        #endregion

        #region Lấy danh sách tính KPI nhân viên giao hàng
        [HttpPost(API.Sp_Get_DanhSachPhieuGiaoHang_KPI)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuGiaoHang_KPI(SP_Parameter_Report sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
                return Ok(await Execute_StoredProc<Sp_Get_DanhSachPhieuGiaoHang_KPI_Result>(API.Sp_Get_DanhSachPhieuGiaoHang_KPI, procParams));
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
        #endregion

        #region Report
        [HttpPost(API.SP_GetReport)]
        public async Task<IActionResult> Sp_GetReport(SP_Parameter_Report sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
                return Ok(await Execute_StoredProc_DataTable<DataTable>(sp_Parameter.NAME_SP, procParams));
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
        #endregion

        #region Công nợ, tồn kho
        [HttpPost(API.Sp_Get_ThongKeCongNoKhachHang)]
        public async Task<IActionResult> Sp_Get_ThongKeCongNoKhachHang(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ThongKeCongNoKhachHang>(API.Sp_Get_ThongKeCongNoKhachHang, procParams));
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

        [HttpPost(API.Sp_Get_ThongKeCongNoNhaCungCap)]
        public async Task<IActionResult> Sp_Get_ThongKeCongNoNhaCungCap(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ThongKeCongNoNhaCungCap>(API.Sp_Get_ThongKeCongNoNhaCungCap, procParams));
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

        [HttpPost(API.Sp_Get_ThongKeCongNoNhanVien)]
        public async Task<IActionResult> Sp_Get_ThongKeCongNoNhanVien(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ThongKeCongNoNhanVien>(API.Sp_Get_ThongKeCongNoNhanVien, procParams));
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

        [HttpPost(API.Sp_Get_ThongKeTonKhoHangHoa)]
        public async Task<IActionResult> Sp_Get_ThongKeTonKhoHangHoa(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ThongKeTonKhoHangHoa>(API.Sp_Get_ThongKeTonKhoHangHoa, procParams));
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
        #endregion

        #region Thu chi
        [HttpPost(API.Sp_Get_ThongKeThuChi_GroupBy)]
        public async Task<IActionResult> Sp_Get_ThongKeThuChi_GroupBy(SP_Parameter_Report sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
                return Ok(await Execute_StoredProc<Sp_Get_ThongKeThuChi_Result>(API.Sp_Get_ThongKeThuChi_GroupBy, procParams));
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

        [HttpPost(API.Sp_Get_ThongKeThuChi)]
        public async Task<IActionResult> Sp_Get_ThongKeThuChi(SP_Parameter_Report sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
                return Ok(await Execute_StoredProc<Sp_Get_ThongKeThuChi_Result>(API.Sp_Get_ThongKeThuChi, procParams));
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
        #endregion

        #region Tính KPI kinh doanh
        [HttpPost(API.Sp_Get_DanhSachPhieuDatHang_ChiTiet_KPI)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuDatHang_ChiTiet_KPI(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<DanhSachPhieuDatHang_ChiTiet_KPI>(API.Sp_Get_DanhSachPhieuDatHang_ChiTiet_KPI, procParams));
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
        #endregion

        #region Tính KPI kinh doanh trả hàng
        [HttpPost(API.Sp_Get_DanhSachPhieuTraHang_ChiTiet_KPI)]
        public async Task<IActionResult> Sp_Get_DanhSachPhieuTraHang_ChiTiet_KPI(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<DanhSachPhieuTraHang_ChiTiet_KPI>(API.Sp_Get_DanhSachPhieuTraHang_ChiTiet_KPI, procParams));
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
        #endregion

        #region Báo cáo hàng hóa nhân viên, khách hàng
        [HttpPost(API.Sp_Get_DanhSachHangHoa)]
        public async Task<IActionResult> Sp_Get_DanhSachHangHoa(SP_Parameter_Report sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
                return Ok(await Execute_StoredProc<Sp_Get_DanhSachHangHoa>(API.Sp_Get_DanhSachHangHoa, procParams));
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

        [HttpPost(API.Sp_Get_DanhSachHangHoa_NhanVien)]
        public async Task<IActionResult> Sp_Get_DanhSachHangHoa_NhanVien(SP_Parameter_Report sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
                return Ok(await Execute_StoredProc<Sp_Get_DanhSachHangHoa_Result>(API.Sp_Get_DanhSachHangHoa_NhanVien, procParams));
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

        [HttpPost(API.Sp_Get_DanhSachHangHoa_KhachHang)]
        public async Task<IActionResult> Sp_Get_DanhSachHangHoa_KhachHang(SP_Parameter_Report sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
                return Ok(await Execute_StoredProc<Sp_Get_DanhSachHangHoa_Result>(API.Sp_Get_DanhSachHangHoa_KhachHang, procParams));
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
        #endregion

        #region
        [HttpPost(API.Sp_Get_DanhSachHangHoa_BanChay)]
        public async Task<IActionResult> Sp_Get_DanhSachHangHoa_BanChay(SP_Parameter_Report sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
                return Ok(await Execute_StoredProc<Sp_Get_DanhSachHangHoa_Result>(API.Sp_Get_DanhSachHangHoa_BanChay, procParams));
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
        #endregion

        #region Qũy tiền
        [HttpPost(API.Sp_Get_ThongKeQuyTien)]
        public async Task<IActionResult> Sp_Get_ThongKeQuyTien(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<v_ThongKeQuyTien>(API.Sp_Get_ThongKeQuyTien, procParams));
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
        #endregion

        #region Bảng lương
        [HttpPost(API.Sp_Get_DanhSachBangLuong)]
        public async Task<IActionResult> Sp_Get_DanhSachBangLuong(SP_Parameter sp_Parameter)
        {
            try
            {
                Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
                return Ok(await Execute_StoredProc<Sp_Get_DanhSachBangLuong_Result>(API.Sp_Get_DanhSachBangLuong, procParams));
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
        #endregion
        public async Task<ApiResponse> Execute_StoredProc<T>(string storedProcName, Dictionary<string, object> procParams) where T : class
        {
            ApiResponse apiResponse = new ApiResponse();
            List<T> objList = new List<T>();
            DbConnection conn = _context.Database.GetDbConnection();
            
            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                await using (DbCommand command = conn.CreateCommand())
                {
                    command.CommandTimeout = 120;
                    command.CommandText = storedProcName;
                    command.CommandType = CommandType.StoredProcedure;

                    foreach (KeyValuePair<string, object> procParam in procParams)
                    {
                        DbParameter param = command.CreateParameter();
                        param.ParameterName = procParam.Key;
                        param.Value = procParam.Value;
                        command.Parameters.Add(param);
                    }

                    DbDataReader reader = await command.ExecuteReaderAsync();
                    
                    IEnumerable<PropertyInfo> props = typeof(T).GetRuntimeProperties();
                    Dictionary<string, DbColumn> colMapping = reader.GetColumnSchema()
                        .Where(x => props.Any(y => y.Name.ToLower() == x.ColumnName.ToLower()))
                        .ToDictionary(key => key.ColumnName.ToLower());

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            if(colMapping != null && colMapping.Count > 0)
                            {
                                T obj = Activator.CreateInstance<T>();
                                foreach (PropertyInfo prop in props)
                                {
                                    if(colMapping.ContainsKey(prop.Name.ToLower()))
                                    {
                                        object val = reader.GetValue(colMapping[prop.Name.ToLower()].ColumnOrdinal!.Value);
                                        if (val != null)
                                        {  
                                            if (prop.PropertyType.Name.ToUpper().Contains("STRING"))
                                            {
                                                prop.SetValue(obj, val == DBNull.Value ? "" : val);
                                            }
                                            else
                                            {
                                                prop.SetValue(obj, val == DBNull.Value ? null : val);
                                            }
                                         }
                                        else
                                        {
                                            if (prop.PropertyType.Name.ToUpper().Contains("STRING"))
                                            {
                                                prop.SetValue(obj, "");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (prop.PropertyType.Name.ToUpper().Contains("STRING"))
                                        {
                                            prop.SetValue(obj, "");
                                        }
                                    }
                                }
                                objList.Add(obj);
                            }
                        }
                    }
                    reader.Dispose();
                }
                apiResponse = new ApiResponse
                {
                    Success = true,
                    Message = "Success!",
                    Data = objList
                };
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message, e.InnerException);
                objList = new List<T>();
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = e.Message,
                    Data = objList
                };
            }
            finally
            {
                conn.Close();
            }
            return apiResponse;
        }

        #region Execute return DataTable
        public async Task<ApiResponse> Execute_StoredProc_DataTable<T>(string storedProcName, Dictionary<string, object> procParams) where T : class
        {
            ApiResponse apiResponse = new ApiResponse();
            DbConnection conn = _context.Database.GetDbConnection();
            DataTable data= new DataTable();
            bool CheckValue = false;
            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                await using (DbCommand command = conn.CreateCommand())
                {
                    command.CommandText = storedProcName;
                    command.CommandType = CommandType.StoredProcedure;

                    foreach (KeyValuePair<string, object> procParam in procParams)
                    {
                        DbParameter param = command.CreateParameter();
                        param.ParameterName = procParam.Key;
                        param.Value = procParam.Value;
                        command.Parameters.Add(param);
                    }

                    DbDataReader reader = await command.ExecuteReaderAsync();
                    data.Load(reader);
                    if (data != null && data.Rows.Count == 0)
                    {
                        List<string> lstDataColumn = new List<string>();
                        foreach (DataColumn cl in data.Columns)
                        {
                            lstDataColumn.Add(cl.ColumnName);
                        }
                        data.Columns.Clear();
                        foreach(string cl in lstDataColumn)
                        {
                            data.Columns.Add(cl, typeof(string));
                        }
                        DataRow dr = data.NewRow();
                        data.Rows.Add(dr);
                        CheckValue = true;
                    }    
                    reader.Dispose();
                }
                apiResponse = new ApiResponse
                {
                    Success = true,
                    Message = "Success!",
                    Data = data,
                    CheckValue = CheckValue
                };
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message, e.InnerException);
                data = new DataTable();
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = e.Message,
                    Data = data
                };
            }
            finally
            {
                conn.Close();
            }
            return apiResponse;
        }

        private bool IsNumericType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
        #endregion

        #region Check delete data
        public async Task<ApiResponse> CheckDelete<T>(T Data,string ID, string MA) where T : class
        {

            string sqlQuery = "";
            string Error = "";
            ApiResponse apiResponse = new ApiResponse();
            DbConnection conn = _context.Database.GetDbConnection();
            try
            {
                var lstNoteClass = await _context.web_NoteClass!.Where(e => e.FOREIGNKEY == Data.GetType().Name).ToListAsync();
                if(lstNoteClass != null && lstNoteClass.Count > 0)
                {
                    foreach(var note in lstNoteClass)
                    {
                        try
                        {
                            if ((note.NAMECLASS != null ? note.NAMECLASS : "").ToUpper().Contains("PHANQUYEN")) continue;
                            int totalRow = 0;
                            sqlQuery = "SELECT COUNT(ID) FROM " + note.NAMECLASS + " WHERE " + note.NAMECOLUMN + " = '" + ID + "'";
                            if (conn.State != ConnectionState.Open)
                                conn.Open();

                            await using (DbCommand command = conn.CreateCommand())
                            {
                                command.CommandText = sqlQuery;
                                command.CommandType = CommandType.Text;
                                var wantedRow = command.ExecuteReader();
                                while (wantedRow.Read())
                                {
                                    totalRow = Convert.ToInt32(wantedRow[0].ToString());
                                }                        
                            }
                            conn.Close();
                            if (totalRow > 0)
                            {
                                var web_NoteTable = await _context.web_NoteTable!.FirstOrDefaultAsync(e => e.NAMECLASS == note.NAMECLASS);
                                Error += "Không thể xóa '"+ MA + "' do dữ liệu đang liên kết tới '" + (web_NoteTable != null && !string.IsNullOrEmpty(web_NoteTable.NOTE) ? web_NoteTable.NOTE : note.NAMECLASS) + "'!" + Environment.NewLine;
                            }
                        }
                        catch
                        {
                            if (conn.State == ConnectionState.Open)
                                conn.Close();
                        } 
                    }
                }    
            }
            catch (Exception e)
            {
               
                System.Diagnostics.Debug.WriteLine(e.Message, e.InnerException);
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = e.Message,
                    Data = ""
                };
            }
            finally
            {
                apiResponse = new ApiResponse
                {
                    Success = string.IsNullOrEmpty(Error),
                    Message = Error,
                    Data = ""
                };
            }
            return apiResponse;
        }
        #endregion

        #region procParams
        public static Dictionary<string, object> GetSP_Parameter_Report(SP_Parameter_Report sp_Parameter)
        {
            Dictionary<string, object> procParams = new Dictionary<string, object>();
            Type t = sp_Parameter.GetType();
            PropertyInfo[] props = t.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (prop.Name != "NAME_SP")
                {
                    if(prop.Name != "ID_REPORT")
                    {
                        if(prop.Name != "HINHTHUC" &&
                            prop.Name != "HINHTHUC_BAOCAOTAICHINH" &&
                            prop.Name != "HINHTHUC_PHIEUXUATHANG_KHUYENMAI" &&
                            prop.Name != "ISDETAIL")
                        {
                            object val = prop.GetValue(sp_Parameter) ?? DBNull.Value;
                            if (val != null && !string.IsNullOrEmpty(val.ToString()))
                                procParams.Add(prop.Name, val);
                        }
                    }
                }
            }
            return procParams;
        }

        public static Dictionary<string, object> GetSP_Parameter(SP_Parameter sp_Parameter)
        {
            Dictionary<string, object> procParams = new Dictionary<string, object>();
            if (sp_Parameter.LOC_ID != null)
                procParams.Add("LOC_ID", sp_Parameter.LOC_ID);
            if (sp_Parameter.ID_NHANVIEN != null)
                procParams.Add("ID_NHANVIEN", sp_Parameter.ID_NHANVIEN);
            if (sp_Parameter.ID_NHOMQUYEN != null)
                procParams.Add("ID_NHOMQUYEN", sp_Parameter.ID_NHOMQUYEN);
            if (sp_Parameter.ID_NHOMHANGHOA != null)
                procParams.Add("ID_NHOMHANGHOA", sp_Parameter.ID_NHOMHANGHOA);
            if (sp_Parameter.KEY != null)
                procParams.Add("KEY", sp_Parameter.KEY);
            if (sp_Parameter.ID_HANGHOA != null)
                procParams.Add("ID_HANGHOA", sp_Parameter.ID_HANGHOA);
            if (sp_Parameter.ID_KHUVUC != null)
                procParams.Add("ID_KHUVUC", sp_Parameter.ID_KHUVUC);
            if (sp_Parameter.ID_KHO != null)
                procParams.Add("ID_KHO", sp_Parameter.ID_KHO);
            if (sp_Parameter.ID_HANGHOAKHO != null)
                procParams.Add("ID_HANGHOAKHO", sp_Parameter.ID_HANGHOAKHO);
            if (sp_Parameter.TUNGAY != null)
                procParams.Add("TUNGAY", sp_Parameter.TUNGAY);
            if (sp_Parameter.DENNGAY != null)
                procParams.Add("DENNGAY", sp_Parameter.DENNGAY);
            if (sp_Parameter.ID_PHIEUNHAP != null)
                procParams.Add("ID_PHIEUNHAP", sp_Parameter.ID_PHIEUNHAP);
            if (sp_Parameter.ID_PHIEUXUAT != null)
                procParams.Add("ID_PHIEUXUAT", sp_Parameter.ID_PHIEUXUAT);
            if (sp_Parameter.ID_PHIEUCHI != null)
                procParams.Add("ID_PHIEUCHI", sp_Parameter.ID_PHIEUCHI);
            if (sp_Parameter.ID_PHIEUTHU != null)
                procParams.Add("ID_PHIEUTHU", sp_Parameter.ID_PHIEUTHU);
            if (sp_Parameter.ID_PHIEUDATHANG != null)
                procParams.Add("ID_PHIEUDATHANG", sp_Parameter.ID_PHIEUDATHANG);
            if (sp_Parameter.ID_PHIEUGIAOHANG != null)
                procParams.Add("ID_PHIEUGIAOHANG", sp_Parameter.ID_PHIEUGIAOHANG);
            if (sp_Parameter.ID_COMBO != null)
                procParams.Add("ID_COMBO", sp_Parameter.ID_COMBO);
            if (sp_Parameter.BOLTONKHO != null)
                procParams.Add("BOLTONKHO", sp_Parameter.BOLTONKHO);
            if (sp_Parameter.ID_KHACHHANG != null)
                procParams.Add("ID_KHACHHANG", sp_Parameter.ID_KHACHHANG);
            if (sp_Parameter.ID_NHACUNGCAP != null)
                procParams.Add("ID_NHACUNGCAP", sp_Parameter.ID_NHACUNGCAP);
            if (sp_Parameter.ID_NHOMKHACHHANG != null)
                procParams.Add("ID_NHOMKHACHHANG", sp_Parameter.ID_NHOMKHACHHANG);
            if (sp_Parameter.ID_NHOMNCC != null)
                procParams.Add("ID_NHOMNCC", sp_Parameter.ID_NHOMNCC);
            if (sp_Parameter.ISTHEOTHOIGIAN != null)
                procParams.Add("ISTHEOTHOIGIAN", sp_Parameter.ISTHEOTHOIGIAN);
            if (sp_Parameter.ISPHATSINHCONGNO != null)
                procParams.Add("ISPHATSINHCONGNO", sp_Parameter.ISPHATSINHCONGNO);
            if (sp_Parameter.ISPHATSINHCONGNOTRONGKY != null)
                procParams.Add("ISPHATSINHCONGNOTRONGKY", sp_Parameter.ISPHATSINHCONGNOTRONGKY);
            if (sp_Parameter.ISCONCONGNO != null)
                procParams.Add("ISCONCONGNO", sp_Parameter.ISCONCONGNO);
            if (sp_Parameter.LOAITIMKIEM != null)
                procParams.Add("LOAITIMKIEM", sp_Parameter.LOAITIMKIEM);
            if (sp_Parameter.THU != null)
                procParams.Add("THU", GetThu(sp_Parameter.THU));
            if (sp_Parameter.ID_PHONGBAN != null)
                procParams.Add("ID_PHONGBAN", sp_Parameter.ID_PHONGBAN);
            if (sp_Parameter.SOLAN != null)
                procParams.Add("SOLAN", sp_Parameter.SOLAN);
            if (sp_Parameter.ID_XE != null)
                procParams.Add("ID_XE", sp_Parameter.ID_XE);
            if (sp_Parameter.ID_TAIKHOANNGANHANG != null)
                procParams.Add("ID_TAIKHOANNGANHANG", sp_Parameter.ID_TAIKHOANNGANHANG);
            if (sp_Parameter.NGAYCONG != null)
                procParams.Add("NGAYCONG", sp_Parameter.NGAYCONG);
            return procParams;
        }
        #endregion

        private static string GetThu(string THU = "")
        {
            if (THU == "-1") return THU;
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    // T2   
                    THU = "T2";
                    break;
                case DayOfWeek.Tuesday:
                    // T3
                    THU = "T3";
                    break;
                case DayOfWeek.Wednesday:
                    // T4
                    THU = "T4";
                    break;
                case DayOfWeek.Thursday:
                    // T5
                    THU = "T5";
                    break;
                case DayOfWeek.Friday:
                    // T6
                    THU = "T6";
                    break;
                case DayOfWeek.Saturday:
                    // T7
                    THU = "T7";
                    break;
                case DayOfWeek.Sunday:
                    // CN
                    THU = "CN";
                    break;

            }
            return THU;
        }

        [HttpPost(API.Insert_Customer_Map)]
        public async Task<ApiResponse> Insert_Customer_Map(view_dm_KhachHang KhachHang)
        {
            ApiResponse apiResponse = new ApiResponse();
            DbConnection conn = _context.Database.GetDbConnection();
            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                await using (DbCommand command = conn.CreateCommand())
                {
                    command.CommandText = API.Insert_Customer_Map;
                    command.CommandType = CommandType.StoredProcedure;
                    DbParameter param = command.CreateParameter();
                    param.ParameterName = "ID";
                    param.Value = KhachHang.ID;
                    command.Parameters.Add(param);

                    param = command.CreateParameter();
                    param.ParameterName = "CONTENT_MAP";
                    param.Value = KhachHang.CONTENT_MAP;
                    command.Parameters.Add(param);
                    if (KhachHang.LATITUDE != null)
                    {
                        param = command.CreateParameter();
                        param.ParameterName = "LATITUDE";
                        param.Value = KhachHang.LATITUDE;
                        command.Parameters.Add(param);
                    }    
                       
                    if (KhachHang.LONGITUDE != null)
                    {
                        param = command.CreateParameter();
                        param.ParameterName = "LONGITUDE";
                        param.Value = KhachHang.LONGITUDE;
                        command.Parameters.Add(param);
                    }
                    var reader = await command.ExecuteNonQueryAsync();
                }
                apiResponse = new ApiResponse
                {
                    Success = true,
                    Message = "Success!",
                    Data = null
                };
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message, e.InnerException);
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = e.Message,
                    Data = null
                };
            }
            finally
            {
                conn.Close();
            }
            return apiResponse;
        }
    }
}
