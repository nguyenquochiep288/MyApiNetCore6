using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DatabaseTHP;
using Microsoft.AspNetCore.Identity;
using DatabaseTHP.StoredProcedure;
//using System.Data.Entity;

namespace MyApiNetCore6.Data
{
    public class dbTrangHiepPhatContext : DbContext
    {
        public dbTrangHiepPhatContext(DbContextOptions<dbTrangHiepPhatContext> opt): base(opt)
        {
        }

        #region DbSet 
        public virtual DbSet<AspNetRequest> AspNetRequest { get; set; }
        public virtual DbSet<dm_HangHoa_HinhAnh> dm_HangHoa_HinhAnh { get; set; }
        public virtual DbSet<dm_HangHoa_MoTa> dm_HangHoa_MoTa { get; set; }
        public virtual DbSet<view_nv_BangLuong_ChiTiet> view_nv_BangLuong_ChiTiet { get; set; }
        public virtual DbSet<AuditLog> AuditLog { get; set; }
        public virtual DbSet<view_dm_BangLuong> view_dm_BangLuong { get; set; }
        public virtual DbSet<dm_BangLuong> dm_BangLuong { get; set; }
        public virtual DbSet<dm_BangLuong_ChiTiet> dm_BangLuong_ChiTiet { get; set; }
        public virtual DbSet<view_nv_PhepNam> view_nv_PhepNam { get; set; }
        public virtual DbSet<dm_DiaDiemChamCong> dm_DiaDiemChamCong { get; set; }
        public virtual DbSet<nv_NghiPhep> nv_NghiPhep { get; set; }
        public virtual DbSet<nv_PhepNam> nv_PhepNam { get; set; }
        public virtual DbSet<nv_BangLuong_ChiTiet> nv_BangLuong_ChiTiet { get; set; }
        public virtual DbSet<nv_ChamCong> nv_ChamCong { get; set; }
        public virtual DbSet<nv_BangLuong> nv_BangLuong { get; set; }
        public virtual DbSet<view_nv_BangLuong> view_nv_BangLuong { get; set; }
        public virtual DbSet<dm_ThangLuong> dm_ThangLuong { get; set; }
        public virtual DbSet<ct_PhieuDatHangNCC> ct_PhieuDatHangNCC { get; set; }
        public virtual DbSet<ct_PhieuDatHangNCC_ChiTiet> ct_PhieuDatHangNCC_ChiTiet { get; set; }
        public virtual DbSet<dm_LichLamViec> dm_LichLamViec { get; set; }
        public virtual DbSet<ct_PhieuGiaoHang_HinhAnh> ct_PhieuGiaoHang_HinhAnh { get; set; }
        public virtual DbSet<view_dm_KPI_KinhDoanh> view_dm_KPI_KinhDoanh { get; set; }
        public virtual DbSet<view_dm_KPI_KinhDoanh_YeuCau> view_dm_KPI_KinhDoanh_YeuCau { get; set; }
        public virtual DbSet<view_dm_KPI_KinhDoanh_NhanVien> view_dm_KPI_KinhDoanh_NhanVien { get; set; }
        public virtual DbSet<dm_KPI_KinhDoanh> dm_KPI_KinhDoanh { get; set; }
        public virtual DbSet<dm_KPI_KinhDoanh_YeuCau> dm_KPI_KinhDoanh_YeuCau { get; set; }
        public virtual DbSet<dm_KPI_KinhDoanh_NhanVien> dm_KPI_KinhDoanh_NhanVien { get; set; }
        public virtual DbSet<dm_ChuongTrinhKhuyenMai> dm_ChuongTrinhKhuyenMai { get; set; }
        public virtual DbSet<dm_ChuongTrinhKhuyenMai_Tang> dm_ChuongTrinhKhuyenMai_Tang { get; set; }
        public virtual DbSet<dm_ChuongTrinhKhuyenMai_YeuCau> dm_ChuongTrinhKhuyenMai_YeuCau { get; set; }
        public virtual DbSet<web_ThongBao> web_ThongBao { get; set; }
        public virtual DbSet<view_dm_ChuongTrinhKhuyenMai_Tang> view_dm_ChuongTrinhKhuyenMai_Tang { get; set; }
        public virtual DbSet<view_dm_ChuongTrinhKhuyenMai_YeuCau> view_dm_ChuongTrinhKhuyenMai_YeuCau { get; set; }
        public virtual DbSet<view_dm_ChuongTrinhKhuyenMai> view_dm_ChuongTrinhKhuyenMai { get; set; }
        public virtual DbSet<view_dm_HangHoa_Combo> view_dm_HangHoa_Combo { get; set; }
        public virtual DbSet<web_Parameter> web_Parameter { get; set; }
        public virtual DbSet<web_Report> web_Report { get; set; }
        public virtual DbSet<web_Report_Parameter> web_Report_Parameter { get; set; }
        public virtual DbSet<view_web_Report> view_web_Report { get; set; }
        public virtual DbSet<view_web_Report_Parameter> view_web_Report_Parameter { get; set; }
        public virtual DbSet<view_dm_KhachHang> view_dm_KhachHang { get; set; }
        public virtual DbSet<view_dm_NhaCungCap> view_dm_NhaCungCap { get; set; }
        public virtual DbSet<view_dm_NhanVien> view_dm_NhanVien { get; set; }
        public virtual DbSet<view_dm_HangHoa> view_dm_HangHoa { get; set; }
        public virtual DbSet<view_web_Menu> view_web_Menu { get; set; }
        public virtual DbSet<web_Quyen> web_Quyen { get; set; }
        public virtual DbSet<view_web_Quyen> view_web_Quyen { get; set; }
        public virtual DbSet<view_web_PhanQuyen> view_web_PhanQuyen { get; set; }
        public virtual DbSet<AspNetRole> AspNetRole { get; set; }
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaim { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<view_AspNetUsers> view_AspNetUsers { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaim { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogin { get; set; }
        public virtual DbSet<AspNetUserToken> AspNetUserToken { get; set; }
        public virtual DbSet<ct_PhieuChi> ct_PhieuChi { get; set; }

        public virtual DbSet<ct_PhieuGiaoHang> ct_PhieuGiaoHang { get; set; }
        public virtual DbSet<ct_PhieuGiaoHang_ChiTiet> ct_PhieuGiaoHang_ChiTiet { get; set; }
        public virtual DbSet<ct_PhieuGiaoHang_NhanVienGiao> ct_PhieuGiaoHang_NhanVienGiao { get; set; }
        public virtual DbSet<ct_PhieuDatHang> ct_PhieuDatHang { get; set; }
        public virtual DbSet<ct_PhieuDatHang_ChiTiet> ct_PhieuDatHang_ChiTiet { get; set; }
        public virtual DbSet<ct_PhieuChuyen> ct_PhieuChuyen { get; set; }
        public virtual DbSet<ct_PhieuChuyen_ChiTiet> ct_PhieuChuyen_ChiTiet { get; set; }
        public virtual DbSet<ct_PhieuNhap> ct_PhieuNhap { get; set; }
        public virtual DbSet<ct_PhieuNhap_ChiTiet> ct_PhieuNhap_ChiTiet { get; set; }
        public virtual DbSet<ct_PhieuThu> ct_PhieuThu { get; set; }
        public virtual DbSet<ct_PhieuXuat> ct_PhieuXuat { get; set; }
        public virtual DbSet<ct_PhieuXuat_ChiTiet> ct_PhieuXuat_ChiTiet { get; set; }
        public virtual DbSet<C__EFMigrationsHistory> C__EFMigrationsHistory { get; set; }
        public virtual DbSet<dm_ChucVu> dm_ChucVu { get; set; }
        public virtual DbSet<dm_CongTy> dm_CongTy { get; set; }
        public virtual DbSet<dm_DonViTinh> dm_DonViTinh { get; set; }
        public virtual DbSet<dm_HangHoa> dm_HangHoa { get; set; }
        public virtual DbSet<dm_HangHoa_Combo> dm_HangHoa_Combo { get; set; }
        public virtual DbSet<dm_HangHoa_Kho> dm_HangHoa_Kho { get; set; }
        public virtual DbSet<dm_KhachHang> dm_KhachHang { get; set; }
        public virtual DbSet<dm_Kho> dm_Kho { get; set; }
        public virtual DbSet<dm_KhuVuc> dm_KhuVuc { get; set; }
        public virtual DbSet<dm_Xe> dm_Xe { get; set; }
        public virtual DbSet<dm_LoaiLuong> dm_LoaiLuong { get; set; }
        public virtual DbSet<dm_LoaiPhieuChi> dm_LoaiPhieuChi { get; set; }
        public virtual DbSet<dm_LoaiPhieuThu> dm_LoaiPhieuThu { get; set; }
        public virtual DbSet<dm_LoaiPhieuNhap> dm_LoaiPhieuNhap { get; set; }
        public virtual DbSet<dm_LoaiPhieuXuat> dm_LoaiPhieuXuat { get; set; }
        public virtual DbSet<dm_NhaCungCap> dm_NhaCungCap { get; set; }
        public virtual DbSet<dm_NhanVien> dm_NhanVien { get; set; }
        public virtual DbSet<dm_NhomHangHoa> dm_NhomHangHoa { get; set; }
        public virtual DbSet<dm_NhomKhachHang> dm_NhomKhachHang { get; set; }
        public virtual DbSet<dm_NhomNhaCungCap> dm_NhomNhaCungCap { get; set; }
        public virtual DbSet<dm_PhongBan> dm_PhongBan { get; set; }
        public virtual DbSet<dm_TaiKhoanNganHang> dm_TaiKhoanNganHang { get; set; }
        public virtual DbSet<dm_ThueSuat> dm_ThueSuat { get; set; }
        public virtual DbSet<dm_TienTe> dm_TienTe { get; set; }
        public virtual DbSet<LogError> LogError { get; set; }
        public virtual DbSet<view_web_NoteClass> view_web_NoteClass { get; set; }
        public virtual DbSet<web_Menu> web_Menu { get; set; }
        public virtual DbSet<web_NhomQuyen> web_NhomQuyen { get; set; }
        public virtual DbSet<web_NoteClass> web_NoteClass { get; set; }
        public virtual DbSet<web_NoteTable> web_NoteTable { get; set; }
        public virtual DbSet<web_NoteType> web_NoteType { get; set; }
        public virtual DbSet<web_PhanQuyen> web_PhanQuyen { get; set; }
        public virtual DbSet<web_PhanQuyenKhachHang> web_PhanQuyenKhachHang { get; set; }
        public virtual DbSet<web_PhanQuyenKhuVuc> web_PhanQuyenKhuVuc { get; set; }
        public virtual DbSet<web_PhanQuyenNhomKhachHang> web_PhanQuyenNhomKhachHang { get; set; }
        public virtual DbSet<web_PhanQuyenNhomSanPham> web_PhanQuyenNhomSanPham { get; set; }
        public virtual DbSet<web_PhanQuyenSanPham> web_PhanQuyenSanPham { get; set; }



        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetUserLogin>()
            .HasKey(m => new { m.LoginProvider, m.ProviderKey, });
            modelBuilder.Entity<AspNetUserToken>()
            .HasKey(m => new { m.UserId, m.LoginProvider, m.Name, });
            modelBuilder.Entity<ct_PhieuChi>()
            .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<ct_PhieuChuyen>()
            .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<ct_PhieuChuyen_ChiTiet>()
            .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<ct_PhieuNhap>()
            .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<ct_PhieuNhap_ChiTiet>()
            .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<ct_PhieuThu>()
            .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<ct_PhieuXuat>()
            .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<ct_PhieuXuat_ChiTiet>()
            .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<C__EFMigrationsHistory>()
            .HasKey(m => new { m.MigrationId, });
            modelBuilder.Entity<dm_ChucVu>()
            .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_DonViTinh>()
            .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_HangHoa>()
            .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_HangHoa_Combo>()
            .HasKey(m => new { m.ID, m.LOC_ID, });
            modelBuilder.Entity<dm_HangHoa_Kho>()
            .HasKey(m => new { m.ID, m.LOC_ID, });
            modelBuilder.Entity<dm_KhachHang>()
            .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_Kho>()
            .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_KhuVuc>()
            .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_Xe>()
           .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_LoaiLuong>()
           .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_LoaiPhieuChi>()
            .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_LoaiPhieuThu>()
            .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_LoaiPhieuNhap>()
           .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_LoaiPhieuXuat>()
           .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_NhaCungCap>()
            .HasKey(m => new { m.ID, m.LOC_ID, });
            modelBuilder.Entity<dm_NhanVien>()
            .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_NhomHangHoa>()
            .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_NhomKhachHang>()
            .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_NhomNhaCungCap>()
           .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_PhongBan>()
            .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_TaiKhoanNganHang>()
            .HasKey(m => new { m.ID, m.LOC_ID, });
            modelBuilder.Entity<dm_ThueSuat>()
            .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_TienTe>()
            .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<LogError>()
            .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<view_web_NoteClass>()
            .HasKey(m => new { m.NAMESPACE, m.NAMECLASS, m.NAMECOLUMN, });
            modelBuilder.Entity<web_NhomQuyen>()
            .HasKey(m => new { m.ID, m.LOC_ID, });
            modelBuilder.Entity<web_NoteClass>()
            .HasKey(m => new { m.NAMESPACE, m.NAMECLASS, m.NAMECOLUMN, });
            modelBuilder.Entity<web_NoteTable>()
            .HasKey(m => new { m.NAMECLASS, });
            modelBuilder.Entity<web_PhanQuyen>()
            .HasKey(m => new { m.ID, m.LOC_ID, });
            modelBuilder.Entity<web_PhanQuyenKhachHang>()
            .HasKey(m => new { m.ID, m.LOC_ID, });
            modelBuilder.Entity<web_PhanQuyenKhuVuc>()
            .HasKey(m => new { m.ID, m.LOC_ID, });
            modelBuilder.Entity<web_PhanQuyenNhomKhachHang>()
            .HasKey(m => new { m.ID, m.LOC_ID, });
            modelBuilder.Entity<web_PhanQuyenNhomSanPham>()
            .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<web_PhanQuyenSanPham>()
            .HasKey(m => new { m.ID, m.LOC_ID, });
            modelBuilder.Entity<web_Quyen>()
           .HasKey(m => new { m.ID, m.LOC_ID, });
            modelBuilder.Entity<dm_ChuongTrinhKhuyenMai>()
           .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_ChuongTrinhKhuyenMai_Tang>()
           .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_ChuongTrinhKhuyenMai_YeuCau>()
           .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<ct_PhieuDatHang>()
          .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<ct_PhieuDatHang_ChiTiet>()
          .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<AspNetUsers>()
           .HasKey(m => new { m.ID });
            modelBuilder.Entity<web_Report>()
          .HasKey(m => new { m.ID });
            modelBuilder.Entity<web_Report_Parameter>()
          .HasKey(m => new { m.ID });
            modelBuilder.Entity<web_Parameter>()
          .HasKey(m => new { m.ID });
            modelBuilder.Entity<ct_PhieuGiaoHang>()
         .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<ct_PhieuGiaoHang_ChiTiet>()
         .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<ct_PhieuGiaoHang_NhanVienGiao>()
         .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_KPI_KinhDoanh>()
        .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_KPI_KinhDoanh_YeuCau>()
           .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_KPI_KinhDoanh_NhanVien>()
          .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<ct_PhieuGiaoHang_HinhAnh>()
         .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_LichLamViec>()
         .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<ct_PhieuDatHangNCC>()
        .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<ct_PhieuDatHangNCC_ChiTiet>()
        .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_ThangLuong>()
        .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<nv_BangLuong>()
        .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<nv_ChamCong>()
        .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<nv_BangLuong_ChiTiet>()
        .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<nv_NghiPhep>()
       .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_DiaDiemChamCong>()
       .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<nv_PhepNam>()
       .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_BangLuong_ChiTiet>()
      .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<dm_BangLuong>()
      .HasKey(m => new { m.LOC_ID, m.ID, });
            modelBuilder.Entity<AuditLog>()
      .HasKey(m => new { m.ID });
            modelBuilder.Entity<view_nv_BangLuong_ChiTiet>()
      .HasKey(m => new { m.LOC_ID, m.ID });
            modelBuilder.Entity<dm_HangHoa_HinhAnh>()
     .HasKey(m => new { m.LOC_ID, m.ID });
            modelBuilder.Entity<dm_HangHoa_MoTa>()
     .HasKey(m => new { m.LOC_ID, m.ID });
            modelBuilder.Entity<AspNetRequest>()
     .HasKey(m => new { m.LOC_ID, m.ID });
        }
    }
}
