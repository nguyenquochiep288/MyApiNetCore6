using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.StoredProcedure;
using DatabaseTHP.StoredProcedure.Parameter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyApiNetCore6.Data;

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

		[HttpPost("web_Sp_Get_DSKhachHang")]
		public async Task<IActionResult> web_Sp_Get_DSKhachHang(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_dm_KhachHang>("web_Sp_Get_DSKhachHang", procParams));
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

		[HttpPost("web_Sp_Get_DSNhomSanPham")]
		public async Task<IActionResult> web_Sp_Get_DSNhomSanPham(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<web_Sp_Get_DSNhomSanPham_Result>("web_Sp_Get_DSNhomSanPham", procParams));
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

		[HttpPost("web_Sp_Get_DSSanPham")]
		public async Task<IActionResult> web_Sp_Get_DSSanPham(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<web_Sp_Get_DSSanPham_Result>("web_Sp_Get_DSSanPham", procParams));
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

		[HttpPost("web_Sp_Get_SanPham")]
		public async Task<IActionResult> web_Sp_Get_SanPham(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<web_Sp_Get_SanPham_Result>("web_Sp_Get_SanPham", procParams));
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

		[HttpPost("web_Sp_Get_SanPham_Combo")]
		public async Task<IActionResult> web_Sp_Get_DSSanPham_Combo(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<web_Sp_Get_SanPham_Combo_Result>("web_Sp_Get_SanPham_Combo", procParams));
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

		[HttpPost("web_Sp_Get_SanPham_Group")]
		public async Task<IActionResult> web_Sp_Get_DSSanPham_Group(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<web_Sp_Get_SanPham_Group_Result>("web_Sp_Get_SanPham_Group", procParams));
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

		[HttpPost("web_Sp_Get_DanhSachKhuyenMai")]
		public async Task<IActionResult> web_Sp_Get_DanhSachKhuyenMai(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<web_Sp_Get_DanhSachKhuyenMai_Result>("web_Sp_Get_DanhSachKhuyenMai", procParams));
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

		[HttpPost("web_Sp_Get_DanhSachChietKhauHoaDon")]
		public async Task<IActionResult> web_Sp_Get_DanhSachChietKhauHoaDon(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<web_Sp_Get_DanhSachChietKhauHoaDon_Result>("web_Sp_Get_DanhSachChietKhauHoaDon", procParams));
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

		[HttpPost("web_Sp_Get_DSKhuVuc")]
		public async Task<IActionResult> web_Sp_Get_DSKhuVuc(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<web_Sp_Get_DSKhuVuc_Result>("web_Sp_Get_DSKhuVuc", procParams));
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

		[HttpPost("Sp_Get_BaoCaoTheoNhanVien")]
		public async Task<IActionResult> Sp_Get_BaoCaoTheoNhanVien(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<Sp_Get_BaoCaoTheoNhanVien_Result>("Sp_Get_BaoCaoTheoNhanVien", procParams));
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

		[HttpPost("Sp_Get_BaoCaoTaiChinh_New")]
		public async Task<IActionResult> Sp_Get_BaoCaoTaiChinh_New(SP_Parameter_Report sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
				return Ok(await Execute_StoredProc<Sp_Get_BaoCaoTaiChinh_New_Result>("Sp_Get_BaoCaoTaiChinh_New", procParams));
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

		[HttpPost("Sp_Get_DanhSachSanPhamKho")]
		public async Task<IActionResult> Sp_Get_DanhSachSanPhamKho(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<Product_Detail>("Sp_Get_DanhSachSanPhamKho", procParams));
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

		[HttpPost("Sp_Get_DanhSachSanPhamKho_Combo")]
		public async Task<IActionResult> Sp_Get_DanhSachSanPhamKho_Combo(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<Product_Detail>("Sp_Get_DanhSachSanPhamKho_Combo", procParams));
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

		[HttpPost("Sp_Get_BaoCaoGiaoHang")]
		public async Task<IActionResult> Sp_Get_BaoCaoGiaoHang(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<Sp_Get_BaoCaoGiaoHang_Result>("Sp_Get_BaoCaoGiaoHang", procParams));
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

		[HttpPost("Sp_Get_BaoCaoGiaoHang_ChiTiet")]
		public async Task<IActionResult> Sp_Get_BaoCaoGiaoHang_ChiTiet(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ThongKeCongNo_ChiTiet>("Sp_Get_BaoCaoGiaoHang_ChiTiet", procParams));
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

		[HttpPost("Sp_Get_BaoCaoTaiChinh")]
		public async Task<IActionResult> Sp_Get_BaoCaoTaiChinh(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<Sp_Get_BaoCaoTaiChinh_Result>("Sp_Get_BaoCaoTaiChinh", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuNhap")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuNhap(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ct_PhieuNhap>("Sp_Get_DanhSachPhieuNhap", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuNhap_PhieuGiaoHang")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuNhap_PhieuGiaoHang(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ct_PhieuNhap>("Sp_Get_DanhSachPhieuNhap_PhieuGiaoHang", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuNhap_Chitiet")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuNhap_Chitiet(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ct_PhieuNhap_ChiTiet>("Sp_Get_DanhSachPhieuNhap_Chitiet", procParams));
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

		[HttpPost("Sp_Get_DanhSachHoaDon")]
		public async Task<IActionResult> Sp_Get_DanhSachHoaDon(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ct_HoaDon>("Sp_Get_DanhSachHoaDon", procParams));
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

		[HttpPost("Sp_Get_DanhSachHoaDon_Chitiet")]
		public async Task<IActionResult> Sp_Get_DanhSachHoaDon_Chitiet(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ct_HoaDon_ChiTiet>("Sp_Get_DanhSachHoaDon_Chitiet", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuDatHangNCC")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuDatHangNCC(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ct_PhieuDatHangNCC>("Sp_Get_DanhSachPhieuDatHangNCC", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuDatHangNCC_Chitiet")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuDatHangNCC_Chitiet(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ct_PhieuDatHangNCC_ChiTiet>("Sp_Get_DanhSachPhieuDatHangNCC_Chitiet", procParams));
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

		[HttpPost("Sp_Get_DanhSachChamCong")]
		public async Task<IActionResult> Sp_Get_DanhSachChamCong(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_nv_ChamCong>("Sp_Get_DanhSachChamCong", procParams));
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

		[HttpPost("Sp_Get_DanhSachNghiPhep")]
		public async Task<IActionResult> Sp_Get_DanhSachNghiPhep(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_nv_NghiPhep>("Sp_Get_DanhSachNghiPhep", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuDatHang")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuDatHang(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ct_PhieuDatHang>("Sp_Get_DanhSachPhieuDatHang", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuDatHang_ChiTiet")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuDatHang_ChiTiet(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ct_PhieuDatHang_ChiTiet>("Sp_Get_DanhSachPhieuDatHang_ChiTiet", procParams));
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

		[HttpPost("Sp_Get_BaoCaoPhieuDatHang")]
		public async Task<IActionResult> Sp_Get_BaoCaoPhieuDatHang(SP_Parameter_Report sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ct_PhieuDatHang_ChiTiet_BaoCao>("Sp_Get_DanhSachPhieuDatHang_ChiTiet_BaoCao", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuNhap_ChiTiet_BaoCao")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuNhap_ChiTiet_BaoCao(SP_Parameter_Report sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ct_PhieuDatHang_ChiTiet_BaoCao>("Sp_Get_DanhSachPhieuNhap_ChiTiet_BaoCao", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuNhapTraHang_ChiTiet_BaoCao")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuNhapTraHang_ChiTiet_BaoCao(SP_Parameter_Report sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ct_PhieuDatHang_ChiTiet_BaoCao>("Sp_Get_DanhSachPhieuNhapTraHang_ChiTiet_BaoCao", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuGiaoHang_ChiTiet_BaoCao")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuGiaoHang_ChiTiet_BaoCao(SP_Parameter_Report sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ct_PhieuDatHang_ChiTiet_BaoCao>("Sp_Get_DanhSachPhieuGiaoHang_ChiTiet_BaoCao", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuXuat_PhieuGiaoHang")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuXuat_PhieuGiaoHang(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ct_PhieuXuat>("Sp_Get_DanhSachPhieuXuat_PhieuGiaoHang", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuXuat")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuXuat(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ct_PhieuXuat>("Sp_Get_DanhSachPhieuXuat", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuXuat_TimKiem")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuXuat_TimKiem(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ct_PhieuXuat>("Sp_Get_DanhSachPhieuXuat_TimKiem", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuXuat_ChiTiet")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuXuat_Chitiet(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ct_PhieuXuat_ChiTiet>("Sp_Get_DanhSachPhieuXuat_ChiTiet", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuGiaoHang_In")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuXuat_In(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<Sp_Get_DanhSachPhieuGiaoHang_In>("Sp_Get_DanhSachPhieuGiaoHang_In", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuGiaoHang_InPhieuGiao")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuGiaoHang_InPhieuGiao(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<Sp_Get_DanhSachPhieuGiaoHang_In>("Sp_Get_DanhSachPhieuGiaoHang_InPhieuGiao", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuGiaoHang")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuGiaoHang(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ct_PhieuGiaoHang>("Sp_Get_DanhSachPhieuGiaoHang", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuGiaoHang_ChiTiet")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuGiaoHang_ChiTiet(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ct_PhieuGiaoHang_ChiTiet>("Sp_Get_DanhSachPhieuGiaoHang_ChiTiet", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuGiaoHang_NhanVienGiao")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuGiaoHang_NhanVienGiao(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ct_PhieuGiaoHang_NhanVienGiao>("Sp_Get_DanhSachPhieuGiaoHang_NhanVienGiao", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuGiaoHang_PhieuXuat")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuGiaoHang_PhieuXuat(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ct_PhieuGiaoHang>("Sp_Get_DanhSachPhieuGiaoHang_PhieuXuat", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuThu")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuThu(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ct_PhieuThu>("Sp_Get_DanhSachPhieuThu", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuThu_PhieuGiaoHang")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuThu_PhieuGiaoHang(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ct_PhieuThu>("Sp_Get_DanhSachPhieuThu_PhieuGiaoHang", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuChi")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuChi(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ct_PhieuChi>("Sp_Get_DanhSachPhieuChi", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuChi_PhieuGiaoHang")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuChi_PhieuGiaoHang(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ct_PhieuChi>("Sp_Get_DanhSachPhieuChi_PhieuGiaoHang", procParams));
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

		[HttpPost("Sp_Get_ChuongTrinhKhuyenMai")]
		public async Task<IActionResult> Sp_Get_ChuongTrinhKhuyenMai(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_dm_ChuongTrinhKhuyenMai>("Sp_Get_ChuongTrinhKhuyenMai", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuXuatHang_KhuyenMai")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuXuatHang_KhuyenMai(SP_Parameter_Report sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
				return Ok(await Execute_StoredProc<Sp_Get_DanhSachPhieuXuatHang_KhuyenMai>("Sp_Get_DanhSachPhieuXuatHang_KhuyenMai", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuGiaoHang_KPI")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuGiaoHang_KPI(SP_Parameter_Report sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
				return Ok(await Execute_StoredProc<Sp_Get_DanhSachPhieuGiaoHang_KPI_Result>("Sp_Get_DanhSachPhieuGiaoHang_KPI", procParams));
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

		[HttpPost("SP_GetReport")]
		public async Task<IActionResult> Sp_GetReport(SP_Parameter_Report sp_Parameter)
		{
			try
			{
				return Ok(await Execute_StoredProc_DataTable<DataTable>(procParams: GetSP_Parameter_Report(sp_Parameter), storedProcName: sp_Parameter.NAME_SP));
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

		[HttpPost("Sp_Get_ThongKeCongNoKhachHang")]
		public async Task<IActionResult> Sp_Get_ThongKeCongNoKhachHang(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ThongKeCongNoKhachHang>("Sp_Get_ThongKeCongNoKhachHang", procParams));
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

		[HttpPost("Sp_Get_ThongKeCongNoNhaCungCap")]
		public async Task<IActionResult> Sp_Get_ThongKeCongNoNhaCungCap(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ThongKeCongNoNhaCungCap>("Sp_Get_ThongKeCongNoNhaCungCap", procParams));
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

		[HttpPost("Sp_Get_ThongKeCongNoNhanVien")]
		public async Task<IActionResult> Sp_Get_ThongKeCongNoNhanVien(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ThongKeCongNoNhanVien>("Sp_Get_ThongKeCongNoNhanVien", procParams));
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

		[HttpPost("Sp_Get_ThongKeTonKhoHangHoa")]
		public async Task<IActionResult> Sp_Get_ThongKeTonKhoHangHoa(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ThongKeTonKhoHangHoa>("Sp_Get_ThongKeTonKhoHangHoa", procParams));
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

		[HttpPost("Sp_Get_ThongKeThuChi_GroupBy")]
		public async Task<IActionResult> Sp_Get_ThongKeThuChi_GroupBy(SP_Parameter_Report sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
				return Ok(await Execute_StoredProc<Sp_Get_ThongKeThuChi_Result>("Sp_Get_ThongKeThuChi_GroupBy", procParams));
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

		[HttpPost("Sp_Get_ThongKeThuChi")]
		public async Task<IActionResult> Sp_Get_ThongKeThuChi(SP_Parameter_Report sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
				return Ok(await Execute_StoredProc<Sp_Get_ThongKeThuChi_Result>("Sp_Get_ThongKeThuChi", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuDatHang_ChiTiet_KPI")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuDatHang_ChiTiet_KPI(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<DanhSachPhieuDatHang_ChiTiet_KPI>("Sp_Get_DanhSachPhieuDatHang_ChiTiet_KPI", procParams));
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

		[HttpPost("Sp_Get_DanhSachPhieuTraHang_ChiTiet_KPI")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuTraHang_ChiTiet_KPI(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<DanhSachPhieuTraHang_ChiTiet_KPI>("Sp_Get_DanhSachPhieuTraHang_ChiTiet_KPI", procParams));
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

		[HttpPost("Sp_Get_DanhSachHangHoa")]
		public async Task<IActionResult> Sp_Get_DanhSachHangHoa(SP_Parameter_Report sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
				return Ok(await Execute_StoredProc<Sp_Get_DanhSachHangHoa>("Sp_Get_DanhSachHangHoa", procParams));
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

		[HttpPost("Sp_Get_DanhSachHangHoa_NhanVien")]
		public async Task<IActionResult> Sp_Get_DanhSachHangHoa_NhanVien(SP_Parameter_Report sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
				return Ok(await Execute_StoredProc<Sp_Get_DanhSachHangHoa_Result>("Sp_Get_DanhSachHangHoa_NhanVien", procParams));
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

		[HttpPost("Sp_Get_DanhSachHangHoa_KhachHang")]
		public async Task<IActionResult> Sp_Get_DanhSachHangHoa_KhachHang(SP_Parameter_Report sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
				return Ok(await Execute_StoredProc<Sp_Get_DanhSachHangHoa_Result>("Sp_Get_DanhSachHangHoa_KhachHang", procParams));
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

		[HttpPost("Sp_Get_DanhSachHangHoa_BanChay")]
		public async Task<IActionResult> Sp_Get_DanhSachHangHoa_BanChay(SP_Parameter_Report sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
				return Ok(await Execute_StoredProc<Sp_Get_DanhSachHangHoa_Result>("Sp_Get_DanhSachHangHoa_BanChay", procParams));
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

		[HttpPost("Sp_Get_ThongKeQuyTien")]
		public async Task<IActionResult> Sp_Get_ThongKeQuyTien(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<v_ThongKeQuyTien>("Sp_Get_ThongKeQuyTien", procParams));
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

		[HttpPost("Sp_Get_DanhSachBangLuong")]
		public async Task<IActionResult> Sp_Get_DanhSachBangLuong(SP_Parameter sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter(sp_Parameter);
				return Ok(await Execute_StoredProc<Sp_Get_DanhSachBangLuong_Result>("Sp_Get_DanhSachBangLuong", procParams));
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

		public async Task<ApiResponse> Execute_StoredProc<T>(string storedProcName, Dictionary<string, object> procParams) where T : class
		{
			ApiResponse apiResponse = new ApiResponse();
			List<T> objList = new List<T>();
			DbConnection conn = _context.Database.GetDbConnection();
			try
			{
				if (conn.State != ConnectionState.Open)
				{
					conn.Open();
				}
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
					Dictionary<string, DbColumn> colMapping = (from x in reader.GetColumnSchema()
															   where props.Any((PropertyInfo y) => y.Name.ToLower() == x.ColumnName.ToLower())
															   select x).ToDictionary((DbColumn key) => key.ColumnName.ToLower());
					if (reader.HasRows)
					{
						while (await reader.ReadAsync())
						{
							if (colMapping == null || colMapping.Count <= 0)
							{
								continue;
							}
							T obj = Activator.CreateInstance<T>();
							foreach (PropertyInfo prop in props)
							{
								if (colMapping.ContainsKey(prop.Name.ToLower()))
								{
									object val = reader.GetValue(colMapping[prop.Name.ToLower()].ColumnOrdinal.Value);
									if (val != null)
									{
										if (prop.PropertyType.Name.ToUpper().Contains("STRING"))
										{
											prop.SetValue(obj, (val == DBNull.Value) ? "" : val);
										}
										else
										{
											prop.SetValue(obj, (val == DBNull.Value) ? null : val);
										}
									}
									else if (prop.PropertyType.Name.ToUpper().Contains("STRING"))
									{
										prop.SetValue(obj, "");
									}
								}
								else if (prop.PropertyType.Name.ToUpper().Contains("STRING"))
								{
									prop.SetValue(obj, "");
								}
							}
							objList.Add(obj);
						}
					}
					reader.Dispose();
				}
				apiResponse = new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = objList
				};
			}
			catch (Exception ex)
			{
				Exception e = ex;
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

		public async Task<ApiResponse> Execute_StoredProc_DataTable<T>(string storedProcName, Dictionary<string, object> procParams) where T : class
		{
			ApiResponse apiResponse = new ApiResponse();
			DbConnection conn = _context.Database.GetDbConnection();
			DataTable data = new DataTable();
			bool CheckValue = false;
			try
			{
				if (conn.State != ConnectionState.Open)
				{
					conn.Open();
				}
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
						foreach (string cl2 in lstDataColumn)
						{
							data.Columns.Add(cl2, typeof(string));
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
					Message = "Success",
					Data = data,
					CheckValue = CheckValue
				};
			}
			catch (Exception ex)
			{
				Exception e = ex;
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
			TypeCode typeCode = Type.GetTypeCode(type);
			TypeCode typeCode2 = typeCode;
			if ((uint)(typeCode2 - 5) <= 10u)
			{
				return true;
			}
			return false;
		}

		public async Task<ApiResponse> CheckDelete<T>(T Data, string ID, string MA) where T : class
		{
			string Error = "";
			new ApiResponse();
			DbConnection conn = _context.Database.GetDbConnection();
			ApiResponse apiResponse;
			try
			{
				List<web_NoteClass> lstNoteClass = await _context.web_NoteClass.Where((web_NoteClass web_NoteClass2) => web_NoteClass2.FOREIGNKEY == Data.GetType().Name).ToListAsync();
				if (lstNoteClass != null && lstNoteClass.Count > 0)
				{
					foreach (web_NoteClass item in lstNoteClass)
					{
						web_NoteClass note = item;
						try
						{
							if (((note.NAMECLASS != null) ? note.NAMECLASS : "").ToUpper().Contains("PHANQUYEN"))
							{
								continue;
							}
							int totalRow = 0;
							string sqlQuery = "SELECT COUNT(ID) FROM " + note.NAMECLASS + " WHERE " + note.NAMECOLUMN + " = '" + ID + "'";
							if (conn.State != ConnectionState.Open)
							{
								conn.Open();
							}
							await using (DbCommand command = conn.CreateCommand())
							{
								command.CommandText = sqlQuery;
								command.CommandType = CommandType.Text;
								DbDataReader wantedRow = command.ExecuteReader();
								while (wantedRow.Read())
								{
									totalRow = Convert.ToInt32(wantedRow[0].ToString());
								}
							}
							conn.Close();
							if (totalRow > 0)
							{
								web_NoteTable web_NoteTable2 = await _context.web_NoteTable.FirstOrDefaultAsync((web_NoteTable web_NoteTable3) => web_NoteTable3.NAMECLASS == note.NAMECLASS);
								Error = Error + "Không thể xóa '" + MA + "' do dữ liệu đang liên kết tới '" + ((web_NoteTable2 != null && !string.IsNullOrEmpty(web_NoteTable2.NOTE)) ? web_NoteTable2.NOTE : note.NAMECLASS) + "'!" + Environment.NewLine;
							}
						}
						catch
						{
							if (conn.State == ConnectionState.Open)
							{
								conn.Close();
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Exception e = ex;
				new ApiResponse
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

		public static Dictionary<string, object> GetSP_Parameter_Report(SP_Parameter_Report sp_Parameter)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Type type = sp_Parameter.GetType();
			PropertyInfo[] properties = type.GetProperties();
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				if (propertyInfo.Name != "NAME_SP" && propertyInfo.Name != "ID_REPORT" && propertyInfo.Name != "HINHTHUC" && propertyInfo.Name != "HINHTHUC_BAOCAOTAICHINH" && propertyInfo.Name != "HINHTHUC_PHIEUXUATHANG_KHUYENMAI" && propertyInfo.Name != "ISDETAIL")
				{
					object obj = propertyInfo.GetValue(sp_Parameter) ?? DBNull.Value;
					if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
					{
						dictionary.Add(propertyInfo.Name, obj);
					}
				}
			}
			return dictionary;
		}

		public static Dictionary<string, object> GetSP_Parameter(SP_Parameter sp_Parameter)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (sp_Parameter.LOC_ID != null)
			{
				dictionary.Add("LOC_ID", sp_Parameter.LOC_ID);
			}
			if (sp_Parameter.ID_NHANVIEN != null)
			{
				dictionary.Add("ID_NHANVIEN", sp_Parameter.ID_NHANVIEN);
			}
			if (sp_Parameter.ID_NHOMQUYEN != null)
			{
				dictionary.Add("ID_NHOMQUYEN", sp_Parameter.ID_NHOMQUYEN);
			}
			if (sp_Parameter.ID_NHOMHANGHOA != null)
			{
				dictionary.Add("ID_NHOMHANGHOA", sp_Parameter.ID_NHOMHANGHOA);
			}
			if (sp_Parameter.KEY != null)
			{
				dictionary.Add("KEY", sp_Parameter.KEY);
			}
			if (sp_Parameter.ID_HANGHOA != null)
			{
				dictionary.Add("ID_HANGHOA", sp_Parameter.ID_HANGHOA);
			}
			if (sp_Parameter.ID_KHUVUC != null)
			{
				dictionary.Add("ID_KHUVUC", sp_Parameter.ID_KHUVUC);
			}
			if (sp_Parameter.ID_KHO != null)
			{
				dictionary.Add("ID_KHO", sp_Parameter.ID_KHO);
			}
			if (sp_Parameter.ID_HANGHOAKHO != null)
			{
				dictionary.Add("ID_HANGHOAKHO", sp_Parameter.ID_HANGHOAKHO);
			}
			if (sp_Parameter.TUNGAY.HasValue)
			{
				dictionary.Add("TUNGAY", sp_Parameter.TUNGAY);
			}
			if (sp_Parameter.DENNGAY.HasValue)
			{
				dictionary.Add("DENNGAY", sp_Parameter.DENNGAY);
			}
			if (sp_Parameter.ID_PHIEUNHAP != null)
			{
				dictionary.Add("ID_PHIEUNHAP", sp_Parameter.ID_PHIEUNHAP);
			}
			if (sp_Parameter.ID_PHIEUXUAT != null)
			{
				dictionary.Add("ID_PHIEUXUAT", sp_Parameter.ID_PHIEUXUAT);
			}
			if (sp_Parameter.ID_PHIEUCHI != null)
			{
				dictionary.Add("ID_PHIEUCHI", sp_Parameter.ID_PHIEUCHI);
			}
			if (sp_Parameter.ID_PHIEUTHU != null)
			{
				dictionary.Add("ID_PHIEUTHU", sp_Parameter.ID_PHIEUTHU);
			}
			if (sp_Parameter.ID_PHIEUDATHANG != null)
			{
				dictionary.Add("ID_PHIEUDATHANG", sp_Parameter.ID_PHIEUDATHANG);
			}
			if (sp_Parameter.ID_PHIEUGIAOHANG != null)
			{
				dictionary.Add("ID_PHIEUGIAOHANG", sp_Parameter.ID_PHIEUGIAOHANG);
			}
			if (sp_Parameter.ID_COMBO != null)
			{
				dictionary.Add("ID_COMBO", sp_Parameter.ID_COMBO);
			}
			if (sp_Parameter.BOLTONKHO.HasValue)
			{
				dictionary.Add("BOLTONKHO", sp_Parameter.BOLTONKHO);
			}
			if (sp_Parameter.ID_KHACHHANG != null)
			{
				dictionary.Add("ID_KHACHHANG", sp_Parameter.ID_KHACHHANG);
			}
			if (sp_Parameter.ISCHITIET.HasValue)
			{
				dictionary.Add("ISCHITIET", sp_Parameter.ISCHITIET);
			}
			if (sp_Parameter.ID_NHACUNGCAP != null)
			{
				dictionary.Add("ID_NHACUNGCAP", sp_Parameter.ID_NHACUNGCAP);
			}
			if (sp_Parameter.ID_NHOMKHACHHANG != null)
			{
				dictionary.Add("ID_NHOMKHACHHANG", sp_Parameter.ID_NHOMKHACHHANG);
			}
			if (sp_Parameter.ID_NHOMNCC != null)
			{
				dictionary.Add("ID_NHOMNCC", sp_Parameter.ID_NHOMNCC);
			}
			if (sp_Parameter.ISTHEOTHOIGIAN.HasValue)
			{
				dictionary.Add("ISTHEOTHOIGIAN", sp_Parameter.ISTHEOTHOIGIAN);
			}
			if (sp_Parameter.ISPHATSINHCONGNO.HasValue)
			{
				dictionary.Add("ISPHATSINHCONGNO", sp_Parameter.ISPHATSINHCONGNO);
			}
			if (sp_Parameter.ISPHATSINHCONGNOTRONGKY.HasValue)
			{
				dictionary.Add("ISPHATSINHCONGNOTRONGKY", sp_Parameter.ISPHATSINHCONGNOTRONGKY);
			}          
            if (sp_Parameter.ISCONCONGNO.HasValue)
			{
				dictionary.Add("ISCONCONGNO", sp_Parameter.ISCONCONGNO);
			}
			if (sp_Parameter.LOAITIMKIEM != null)
			{
				dictionary.Add("LOAITIMKIEM", sp_Parameter.LOAITIMKIEM);
			}
			if (sp_Parameter.THU != null)
			{
				dictionary.Add("THU", GetThu(sp_Parameter.THU));
			}
			if (sp_Parameter.ID_PHONGBAN != null)
			{
				dictionary.Add("ID_PHONGBAN", sp_Parameter.ID_PHONGBAN);
			}
			if (sp_Parameter.SOLAN.HasValue)
			{
				dictionary.Add("SOLAN", sp_Parameter.SOLAN);
			}
			if (sp_Parameter.ID_XE != null)
			{
				dictionary.Add("ID_XE", sp_Parameter.ID_XE);
			}
			if (sp_Parameter.ID_TAIKHOANNGANHANG != null)
			{
				dictionary.Add("ID_TAIKHOANNGANHANG", sp_Parameter.ID_TAIKHOANNGANHANG);
			}
			if (sp_Parameter.NGAYCONG.HasValue)
			{
				dictionary.Add("NGAYCONG", sp_Parameter.NGAYCONG);
			}
			if (sp_Parameter.ID_HOADON != null)
			{
				dictionary.Add("ID_HOADON", sp_Parameter.ID_HOADON);
			}
			return dictionary;
		}

		[HttpPost("Sp_Get_DanhSachPhieuXuat_ChiTiet_BC")]
		public async Task<IActionResult> Sp_Get_DanhSachPhieuXuat_ChiTiet_BC(SP_Parameter_Report sp_Parameter)
		{
			try
			{
				Dictionary<string, object> procParams = GetSP_Parameter_Report(sp_Parameter);
				return Ok(await Execute_StoredProc<Sp_Get_DanhSachPhieuXuat_ChiTiet_Result>("Sp_Get_DanhSachPhieuXuat_ChiTiet_BC", procParams));
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

		private static string GetThu(string THU = "")
		{
			if (THU == "-1")
			{
				return THU;
			}
			switch (DateTime.Now.DayOfWeek)
			{
				case DayOfWeek.Monday:
					THU = "T2";
					break;
				case DayOfWeek.Tuesday:
					THU = "T3";
					break;
				case DayOfWeek.Wednesday:
					THU = "T4";
					break;
				case DayOfWeek.Thursday:
					THU = "T5";
					break;
				case DayOfWeek.Friday:
					THU = "T6";
					break;
				case DayOfWeek.Saturday:
					THU = "T7";
					break;
				case DayOfWeek.Sunday:
					THU = "CN";
					break;
			}
			return THU;
		}

		[HttpPost("Insert_Customer_Map")]
		public async Task<ApiResponse> Insert_Customer_Map(view_dm_KhachHang KhachHang)
		{
			ApiResponse apiResponse = new ApiResponse();
			DbConnection conn = _context.Database.GetDbConnection();
			try
			{
				if (conn.State != ConnectionState.Open)
				{
					conn.Open();
				}
				await using (DbCommand command = conn.CreateCommand())
				{
					command.CommandText = "Insert_Customer_Map";
					command.CommandType = CommandType.StoredProcedure;
					DbParameter param = command.CreateParameter();
					param.ParameterName = "ID";
					param.Value = KhachHang.ID;
					command.Parameters.Add(param);
					param = command.CreateParameter();
					param.ParameterName = "CONTENT_MAP";
					param.Value = KhachHang.CONTENT_MAP;
					command.Parameters.Add(param);
					if (KhachHang.LATITUDE.HasValue)
					{
						param = command.CreateParameter();
						param.ParameterName = "LATITUDE";
						param.Value = KhachHang.LATITUDE;
						command.Parameters.Add(param);
					}
					if (KhachHang.LONGITUDE.HasValue)
					{
						param = command.CreateParameter();
						param.ParameterName = "LONGITUDE";
						param.Value = KhachHang.LONGITUDE;
						command.Parameters.Add(param);
					}
					await command.ExecuteNonQueryAsync();
				}
				apiResponse = new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = null
				};
			}
			catch (Exception ex)
			{
				Exception e = ex;
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
