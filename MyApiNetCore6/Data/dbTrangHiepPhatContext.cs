// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Data.dbTrangHiepPhatContext
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;


namespace MyApiNetCore6.Data;

public class dbTrangHiepPhatContext(DbContextOptions<dbTrangHiepPhatContext> opt) : DbContext((DbContextOptions) opt)
{
  public virtual DbSet<DatabaseTHP.AspNetRequest> AspNetRequest { get; set; }

  public virtual DbSet<DatabaseTHP.dm_HangHoa_HinhAnh> dm_HangHoa_HinhAnh { get; set; }

  public virtual DbSet<DatabaseTHP.dm_HangHoa_MoTa> dm_HangHoa_MoTa { get; set; }

  public virtual DbSet<DatabaseTHP.StoredProcedure.view_nv_BangLuong_ChiTiet> view_nv_BangLuong_ChiTiet { get; set; }

  public virtual DbSet<DatabaseTHP.AuditLog> AuditLog { get; set; }

  public virtual DbSet<DatabaseTHP.view_dm_BangLuong> view_dm_BangLuong { get; set; }

  public virtual DbSet<DatabaseTHP.dm_BangLuong> dm_BangLuong { get; set; }

  public virtual DbSet<DatabaseTHP.dm_TaiKhoan_Misa> dm_TaiKhoan_Misa { get; set; }

  public virtual DbSet<DatabaseTHP.dm_TaiKhoan_Uniben> dm_TaiKhoan_Uniben { get; set; }

  public virtual DbSet<DatabaseTHP.dm_BangLuong_ChiTiet> dm_BangLuong_ChiTiet { get; set; }

  public virtual DbSet<DatabaseTHP.view_nv_PhepNam> view_nv_PhepNam { get; set; }

  public virtual DbSet<DatabaseTHP.dm_DiaDiemChamCong> dm_DiaDiemChamCong { get; set; }

  public virtual DbSet<DatabaseTHP.nv_NghiPhep> nv_NghiPhep { get; set; }

  public virtual DbSet<DatabaseTHP.nv_PhepNam> nv_PhepNam { get; set; }

  public virtual DbSet<DatabaseTHP.nv_BangLuong_ChiTiet> nv_BangLuong_ChiTiet { get; set; }

  public virtual DbSet<DatabaseTHP.nv_ChamCong> nv_ChamCong { get; set; }

  public virtual DbSet<DatabaseTHP.nv_BangLuong> nv_BangLuong { get; set; }

  public virtual DbSet<DatabaseTHP.StoredProcedure.view_nv_BangLuong> view_nv_BangLuong { get; set; }

  public virtual DbSet<DatabaseTHP.dm_ThangLuong> dm_ThangLuong { get; set; }

  public virtual DbSet<DatabaseTHP.ct_PhieuDatHangNCC> ct_PhieuDatHangNCC { get; set; }

  public virtual DbSet<DatabaseTHP.ct_PhieuDatHangNCC_ChiTiet> ct_PhieuDatHangNCC_ChiTiet { get; set; }

  public virtual DbSet<DatabaseTHP.dm_LichLamViec> dm_LichLamViec { get; set; }

  public virtual DbSet<DatabaseTHP.ct_PhieuGiaoHang_HinhAnh> ct_PhieuGiaoHang_HinhAnh { get; set; }

  public virtual DbSet<DatabaseTHP.view_dm_KPI_KinhDoanh> view_dm_KPI_KinhDoanh { get; set; }

  public virtual DbSet<DatabaseTHP.view_dm_KPI_KinhDoanh_YeuCau> view_dm_KPI_KinhDoanh_YeuCau { get; set; }

  public virtual DbSet<DatabaseTHP.view_dm_KPI_KinhDoanh_NhanVien> view_dm_KPI_KinhDoanh_NhanVien { get; set; }

  public virtual DbSet<DatabaseTHP.dm_KPI_KinhDoanh> dm_KPI_KinhDoanh { get; set; }

  public virtual DbSet<DatabaseTHP.dm_KPI_KinhDoanh_YeuCau> dm_KPI_KinhDoanh_YeuCau { get; set; }

  public virtual DbSet<DatabaseTHP.dm_KPI_KinhDoanh_NhanVien> dm_KPI_KinhDoanh_NhanVien { get; set; }

  public virtual DbSet<DatabaseTHP.dm_ChuongTrinhKhuyenMai> dm_ChuongTrinhKhuyenMai { get; set; }

  public virtual DbSet<DatabaseTHP.dm_ChuongTrinhKhuyenMai_Tang> dm_ChuongTrinhKhuyenMai_Tang { get; set; }

  public virtual DbSet<DatabaseTHP.dm_ChuongTrinhKhuyenMai_YeuCau> dm_ChuongTrinhKhuyenMai_YeuCau { get; set; }

  public virtual DbSet<DatabaseTHP.web_ThongBao> web_ThongBao { get; set; }

  public virtual DbSet<DatabaseTHP.view_dm_ChuongTrinhKhuyenMai_Tang> view_dm_ChuongTrinhKhuyenMai_Tang { get; set; }

  public virtual DbSet<DatabaseTHP.view_dm_ChuongTrinhKhuyenMai_YeuCau> view_dm_ChuongTrinhKhuyenMai_YeuCau { get; set; }

  public virtual DbSet<DatabaseTHP.view_dm_ChuongTrinhKhuyenMai> view_dm_ChuongTrinhKhuyenMai { get; set; }

  public virtual DbSet<DatabaseTHP.view_dm_HangHoa_Combo> view_dm_HangHoa_Combo { get; set; }

  public virtual DbSet<DatabaseTHP.web_Parameter> web_Parameter { get; set; }

  public virtual DbSet<DatabaseTHP.web_Report> web_Report { get; set; }

  public virtual DbSet<DatabaseTHP.web_Report_Parameter> web_Report_Parameter { get; set; }

  public virtual DbSet<DatabaseTHP.view_web_Report> view_web_Report { get; set; }

  public virtual DbSet<DatabaseTHP.view_web_Report_Parameter> view_web_Report_Parameter { get; set; }

  public virtual DbSet<DatabaseTHP.view_dm_KhachHang> view_dm_KhachHang { get; set; }

  public virtual DbSet<DatabaseTHP.view_dm_NhaCungCap> view_dm_NhaCungCap { get; set; }

  public virtual DbSet<DatabaseTHP.view_dm_NhanVien> view_dm_NhanVien { get; set; }

  public virtual DbSet<DatabaseTHP.view_dm_HangHoa> view_dm_HangHoa { get; set; }

  public virtual DbSet<DatabaseTHP.view_web_Menu> view_web_Menu { get; set; }

  public virtual DbSet<DatabaseTHP.web_Quyen> web_Quyen { get; set; }

  public virtual DbSet<DatabaseTHP.view_web_Quyen> view_web_Quyen { get; set; }

  public virtual DbSet<DatabaseTHP.view_web_PhanQuyen> view_web_PhanQuyen { get; set; }

  public virtual DbSet<DatabaseTHP.AspNetRole> AspNetRole { get; set; }

  public virtual DbSet<DatabaseTHP.AspNetRoleClaim> AspNetRoleClaim { get; set; }

  public virtual DbSet<DatabaseTHP.AspNetUsers> AspNetUsers { get; set; }

  public virtual DbSet<DatabaseTHP.view_AspNetUsers> view_AspNetUsers { get; set; }

  public virtual DbSet<DatabaseTHP.AspNetUserClaim> AspNetUserClaim { get; set; }

  public virtual DbSet<DatabaseTHP.AspNetUserLogin> AspNetUserLogin { get; set; }

  public virtual DbSet<DatabaseTHP.AspNetUserToken> AspNetUserToken { get; set; }

  public virtual DbSet<DatabaseTHP.ct_PhieuChi> ct_PhieuChi { get; set; }

  public virtual DbSet<DatabaseTHP.ct_PhieuGiaoHang> ct_PhieuGiaoHang { get; set; }

  public virtual DbSet<DatabaseTHP.ct_PhieuGiaoHang_ChiTiet> ct_PhieuGiaoHang_ChiTiet { get; set; }

  public virtual DbSet<DatabaseTHP.ct_PhieuGiaoHang_NhanVienGiao> ct_PhieuGiaoHang_NhanVienGiao { get; set; }

  public virtual DbSet<DatabaseTHP.ct_PhieuDatHang> ct_PhieuDatHang { get; set; }

  public virtual DbSet<DatabaseTHP.ct_PhieuDatHang_ChiTiet> ct_PhieuDatHang_ChiTiet { get; set; }

  public virtual DbSet<DatabaseTHP.ct_PhieuChuyen> ct_PhieuChuyen { get; set; }

  public virtual DbSet<DatabaseTHP.ct_PhieuChuyen_ChiTiet> ct_PhieuChuyen_ChiTiet { get; set; }

  public virtual DbSet<DatabaseTHP.ct_PhieuNhap> ct_PhieuNhap { get; set; }

  public virtual DbSet<DatabaseTHP.ct_PhieuNhap_ChiTiet> ct_PhieuNhap_ChiTiet { get; set; }

  public virtual DbSet<DatabaseTHP.ct_PhieuThu> ct_PhieuThu { get; set; }

  public virtual DbSet<DatabaseTHP.ct_PhieuXuat> ct_PhieuXuat { get; set; }

  public virtual DbSet<DatabaseTHP.ct_PhieuXuat_ChiTiet> ct_PhieuXuat_ChiTiet { get; set; }

  public virtual DbSet<DatabaseTHP.C__EFMigrationsHistory> C__EFMigrationsHistory { get; set; }

  public virtual DbSet<DatabaseTHP.dm_ChucVu> dm_ChucVu { get; set; }

  public virtual DbSet<DatabaseTHP.dm_CongTy> dm_CongTy { get; set; }

  public virtual DbSet<DatabaseTHP.dm_DonViTinh> dm_DonViTinh { get; set; }

  public virtual DbSet<DatabaseTHP.dm_HangHoa> dm_HangHoa { get; set; }

  public virtual DbSet<DatabaseTHP.dm_HangHoa_Combo> dm_HangHoa_Combo { get; set; }

  public virtual DbSet<DatabaseTHP.dm_HangHoa_Kho> dm_HangHoa_Kho { get; set; }

  public virtual DbSet<DatabaseTHP.dm_KhachHang> dm_KhachHang { get; set; }

  public virtual DbSet<DatabaseTHP.dm_Kho> dm_Kho { get; set; }

  public virtual DbSet<DatabaseTHP.dm_KhuVuc> dm_KhuVuc { get; set; }

  public virtual DbSet<DatabaseTHP.dm_Xe> dm_Xe { get; set; }

  public virtual DbSet<DatabaseTHP.dm_LoaiLuong> dm_LoaiLuong { get; set; }

  public virtual DbSet<DatabaseTHP.dm_LoaiPhieuChi> dm_LoaiPhieuChi { get; set; }

  public virtual DbSet<DatabaseTHP.dm_LoaiPhieuThu> dm_LoaiPhieuThu { get; set; }

  public virtual DbSet<DatabaseTHP.dm_LoaiPhieuNhap> dm_LoaiPhieuNhap { get; set; }

  public virtual DbSet<DatabaseTHP.dm_LoaiPhieuXuat> dm_LoaiPhieuXuat { get; set; }

  public virtual DbSet<DatabaseTHP.dm_NhaCungCap> dm_NhaCungCap { get; set; }

  public virtual DbSet<DatabaseTHP.dm_NhanVien> dm_NhanVien { get; set; }

  public virtual DbSet<DatabaseTHP.dm_NhomHangHoa> dm_NhomHangHoa { get; set; }

  public virtual DbSet<DatabaseTHP.dm_NhomKhachHang> dm_NhomKhachHang { get; set; }

  public virtual DbSet<DatabaseTHP.dm_NhomNhaCungCap> dm_NhomNhaCungCap { get; set; }

  public virtual DbSet<DatabaseTHP.dm_PhongBan> dm_PhongBan { get; set; }

  public virtual DbSet<DatabaseTHP.dm_TaiKhoanNganHang> dm_TaiKhoanNganHang { get; set; }

  public virtual DbSet<DatabaseTHP.dm_ThueSuat> dm_ThueSuat { get; set; }

  public virtual DbSet<DatabaseTHP.dm_TienTe> dm_TienTe { get; set; }

  public virtual DbSet<DatabaseTHP.LogError> LogError { get; set; }

  public virtual DbSet<DatabaseTHP.view_web_NoteClass> view_web_NoteClass { get; set; }

  public virtual DbSet<DatabaseTHP.web_Menu> web_Menu { get; set; }

  public virtual DbSet<DatabaseTHP.web_NhomQuyen> web_NhomQuyen { get; set; }

  public virtual DbSet<DatabaseTHP.web_NoteClass> web_NoteClass { get; set; }

  public virtual DbSet<DatabaseTHP.web_NoteTable> web_NoteTable { get; set; }

  public virtual DbSet<DatabaseTHP.web_NoteType> web_NoteType { get; set; }

  public virtual DbSet<DatabaseTHP.web_PhanQuyen> web_PhanQuyen { get; set; }

  public virtual DbSet<DatabaseTHP.web_PhanQuyenKhachHang> web_PhanQuyenKhachHang { get; set; }

  public virtual DbSet<DatabaseTHP.web_PhanQuyenKhuVuc> web_PhanQuyenKhuVuc { get; set; }

  public virtual DbSet<DatabaseTHP.web_PhanQuyenNhomKhachHang> web_PhanQuyenNhomKhachHang { get; set; }

  public virtual DbSet<DatabaseTHP.web_PhanQuyenNhomSanPham> web_PhanQuyenNhomSanPham { get; set; }

  public virtual DbSet<DatabaseTHP.web_PhanQuyenSanPham> web_PhanQuyenSanPham { get; set; }

  public virtual DbSet<DatabaseTHP.ct_HoaDon> ct_HoaDon { get; set; }

  public virtual DbSet<DatabaseTHP.ct_HoaDon_ChiTiet> ct_HoaDon_ChiTiet { get; set; }

  public virtual DbSet<DatabaseTHP.dm_LoaiHoaDon> dm_LoaiHoaDon { get; set; }

  public virtual DbSet<DatabaseTHP.uniben_dm_LienKet_KhachHang> uniben_dm_LienKet_KhachHang { get; set; }

  public virtual DbSet<DatabaseTHP.uniben_dm_LienKet_HangHoa> uniben_dm_LienKet_HangHoa { get; set; }

  public virtual DbSet<DatabaseTHP.uniben_dm_LienKet_NhanVien> uniben_dm_LienKet_NhanVien { get; set; }

  public virtual DbSet<DatabaseTHP.dm_HangHoa_KhungGia> dm_HangHoa_KhungGia { get; set; }

  public virtual DbSet<DatabaseTHP.view_dm_HangHoa_KhungGia> view_dm_HangHoa_KhungGia { get; set; }

  public virtual DbSet<DatabaseTHP.dm_HangHoa_KhungGia_HangHoa> dm_HangHoa_KhungGia_HangHoa { get; set; }

  public virtual DbSet<DatabaseTHP.dm_HangHoa_KhungGia_Master> dm_HangHoa_KhungGia_Master { get; set; }

  public virtual DbSet<DatabaseTHP.view_dm_HangHoa_KhungGia_HangHoa> view_dm_HangHoa_KhungGia_HangHoa { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<DatabaseTHP.AspNetUserLogin>().HasKey((Expression<Func<DatabaseTHP.AspNetUserLogin, object>>) (m => new
    {
      LoginProvider = m.LoginProvider,
      ProviderKey = m.ProviderKey
    }));
    modelBuilder.Entity<DatabaseTHP.AspNetUserToken>().HasKey((Expression<Func<DatabaseTHP.AspNetUserToken, object>>) (m => new
    {
      UserId = m.UserId,
      LoginProvider = m.LoginProvider,
      Name = m.Name
    }));
    modelBuilder.Entity<DatabaseTHP.ct_PhieuChi>().HasKey((Expression<Func<DatabaseTHP.ct_PhieuChi, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.ct_PhieuChuyen>().HasKey((Expression<Func<DatabaseTHP.ct_PhieuChuyen, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.ct_PhieuChuyen_ChiTiet>().HasKey((Expression<Func<DatabaseTHP.ct_PhieuChuyen_ChiTiet, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.ct_PhieuNhap>().HasKey((Expression<Func<DatabaseTHP.ct_PhieuNhap, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.ct_PhieuNhap_ChiTiet>().HasKey((Expression<Func<DatabaseTHP.ct_PhieuNhap_ChiTiet, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.ct_PhieuThu>().HasKey((Expression<Func<DatabaseTHP.ct_PhieuThu, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.ct_PhieuXuat>().HasKey((Expression<Func<DatabaseTHP.ct_PhieuXuat, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.ct_PhieuXuat_ChiTiet>().HasKey((Expression<Func<DatabaseTHP.ct_PhieuXuat_ChiTiet, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.C__EFMigrationsHistory>().HasKey((Expression<Func<DatabaseTHP.C__EFMigrationsHistory, object>>) (m => new
    {
      MigrationId = m.MigrationId
    }));
    modelBuilder.Entity<DatabaseTHP.dm_ChucVu>().HasKey((Expression<Func<DatabaseTHP.dm_ChucVu, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_DonViTinh>().HasKey((Expression<Func<DatabaseTHP.dm_DonViTinh, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_HangHoa>().HasKey((Expression<Func<DatabaseTHP.dm_HangHoa, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_HangHoa_Combo>().HasKey((Expression<Func<DatabaseTHP.dm_HangHoa_Combo, object>>) (m => new
    {
      ID = m.ID,
      LOC_ID = m.LOC_ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_HangHoa_Kho>().HasKey((Expression<Func<DatabaseTHP.dm_HangHoa_Kho, object>>) (m => new
    {
      ID = m.ID,
      LOC_ID = m.LOC_ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_KhachHang>().HasKey((Expression<Func<DatabaseTHP.dm_KhachHang, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_Kho>().HasKey((Expression<Func<DatabaseTHP.dm_Kho, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_KhuVuc>().HasKey((Expression<Func<DatabaseTHP.dm_KhuVuc, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_Xe>().HasKey((Expression<Func<DatabaseTHP.dm_Xe, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_LoaiLuong>().HasKey((Expression<Func<DatabaseTHP.dm_LoaiLuong, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_LoaiPhieuChi>().HasKey((Expression<Func<DatabaseTHP.dm_LoaiPhieuChi, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_LoaiPhieuThu>().HasKey((Expression<Func<DatabaseTHP.dm_LoaiPhieuThu, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_LoaiPhieuNhap>().HasKey((Expression<Func<DatabaseTHP.dm_LoaiPhieuNhap, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_LoaiPhieuXuat>().HasKey((Expression<Func<DatabaseTHP.dm_LoaiPhieuXuat, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_NhaCungCap>().HasKey((Expression<Func<DatabaseTHP.dm_NhaCungCap, object>>) (m => new
    {
      ID = m.ID,
      LOC_ID = m.LOC_ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_NhanVien>().HasKey((Expression<Func<DatabaseTHP.dm_NhanVien, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_NhomHangHoa>().HasKey((Expression<Func<DatabaseTHP.dm_NhomHangHoa, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_NhomKhachHang>().HasKey((Expression<Func<DatabaseTHP.dm_NhomKhachHang, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_NhomNhaCungCap>().HasKey((Expression<Func<DatabaseTHP.dm_NhomNhaCungCap, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_PhongBan>().HasKey((Expression<Func<DatabaseTHP.dm_PhongBan, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_TaiKhoanNganHang>().HasKey((Expression<Func<DatabaseTHP.dm_TaiKhoanNganHang, object>>) (m => new
    {
      ID = m.ID,
      LOC_ID = m.LOC_ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_ThueSuat>().HasKey((Expression<Func<DatabaseTHP.dm_ThueSuat, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_TienTe>().HasKey((Expression<Func<DatabaseTHP.dm_TienTe, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.LogError>().HasKey((Expression<Func<DatabaseTHP.LogError, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.view_web_NoteClass>().HasKey((Expression<Func<DatabaseTHP.view_web_NoteClass, object>>) (m => new
    {
      NAMESPACE = m.NAMESPACE,
      NAMECLASS = m.NAMECLASS,
      NAMECOLUMN = m.NAMECOLUMN
    }));
    modelBuilder.Entity<DatabaseTHP.web_NhomQuyen>().HasKey((Expression<Func<DatabaseTHP.web_NhomQuyen, object>>) (m => new
    {
      ID = m.ID,
      LOC_ID = m.LOC_ID
    }));
    modelBuilder.Entity<DatabaseTHP.web_NoteClass>().HasKey((Expression<Func<DatabaseTHP.web_NoteClass, object>>) (m => new
    {
      NAMESPACE = m.NAMESPACE,
      NAMECLASS = m.NAMECLASS,
      NAMECOLUMN = m.NAMECOLUMN
    }));
    modelBuilder.Entity<DatabaseTHP.web_NoteTable>().HasKey((Expression<Func<DatabaseTHP.web_NoteTable, object>>) (m => new
    {
      NAMECLASS = m.NAMECLASS
    }));
    modelBuilder.Entity<DatabaseTHP.web_PhanQuyen>().HasKey((Expression<Func<DatabaseTHP.web_PhanQuyen, object>>) (m => new
    {
      ID = m.ID,
      LOC_ID = m.LOC_ID
    }));
    modelBuilder.Entity<DatabaseTHP.web_PhanQuyenKhachHang>().HasKey((Expression<Func<DatabaseTHP.web_PhanQuyenKhachHang, object>>) (m => new
    {
      ID = m.ID,
      LOC_ID = m.LOC_ID
    }));
    modelBuilder.Entity<DatabaseTHP.web_PhanQuyenKhuVuc>().HasKey((Expression<Func<DatabaseTHP.web_PhanQuyenKhuVuc, object>>) (m => new
    {
      ID = m.ID,
      LOC_ID = m.LOC_ID
    }));
    modelBuilder.Entity<DatabaseTHP.web_PhanQuyenNhomKhachHang>().HasKey((Expression<Func<DatabaseTHP.web_PhanQuyenNhomKhachHang, object>>) (m => new
    {
      ID = m.ID,
      LOC_ID = m.LOC_ID
    }));
    modelBuilder.Entity<DatabaseTHP.web_PhanQuyenNhomSanPham>().HasKey((Expression<Func<DatabaseTHP.web_PhanQuyenNhomSanPham, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.web_PhanQuyenSanPham>().HasKey((Expression<Func<DatabaseTHP.web_PhanQuyenSanPham, object>>) (m => new
    {
      ID = m.ID,
      LOC_ID = m.LOC_ID
    }));
    modelBuilder.Entity<DatabaseTHP.web_Quyen>().HasKey((Expression<Func<DatabaseTHP.web_Quyen, object>>) (m => new
    {
      ID = m.ID,
      LOC_ID = m.LOC_ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_ChuongTrinhKhuyenMai>().HasKey((Expression<Func<DatabaseTHP.dm_ChuongTrinhKhuyenMai, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_ChuongTrinhKhuyenMai_Tang>().HasKey((Expression<Func<DatabaseTHP.dm_ChuongTrinhKhuyenMai_Tang, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_ChuongTrinhKhuyenMai_YeuCau>().HasKey((Expression<Func<DatabaseTHP.dm_ChuongTrinhKhuyenMai_YeuCau, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.ct_PhieuDatHang>().HasKey((Expression<Func<DatabaseTHP.ct_PhieuDatHang, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.ct_PhieuDatHang_ChiTiet>().HasKey((Expression<Func<DatabaseTHP.ct_PhieuDatHang_ChiTiet, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.AspNetUsers>().HasKey((Expression<Func<DatabaseTHP.AspNetUsers, object>>) (m => new
    {
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.web_Report>().HasKey((Expression<Func<DatabaseTHP.web_Report, object>>) (m => new
    {
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.web_Report_Parameter>().HasKey((Expression<Func<DatabaseTHP.web_Report_Parameter, object>>) (m => new
    {
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.web_Parameter>().HasKey((Expression<Func<DatabaseTHP.web_Parameter, object>>) (m => new
    {
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.ct_PhieuGiaoHang>().HasKey((Expression<Func<DatabaseTHP.ct_PhieuGiaoHang, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.ct_PhieuGiaoHang_ChiTiet>().HasKey((Expression<Func<DatabaseTHP.ct_PhieuGiaoHang_ChiTiet, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.ct_PhieuGiaoHang_NhanVienGiao>().HasKey((Expression<Func<DatabaseTHP.ct_PhieuGiaoHang_NhanVienGiao, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_KPI_KinhDoanh>().HasKey((Expression<Func<DatabaseTHP.dm_KPI_KinhDoanh, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_KPI_KinhDoanh_YeuCau>().HasKey((Expression<Func<DatabaseTHP.dm_KPI_KinhDoanh_YeuCau, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_KPI_KinhDoanh_NhanVien>().HasKey((Expression<Func<DatabaseTHP.dm_KPI_KinhDoanh_NhanVien, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.ct_PhieuGiaoHang_HinhAnh>().HasKey((Expression<Func<DatabaseTHP.ct_PhieuGiaoHang_HinhAnh, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_LichLamViec>().HasKey((Expression<Func<DatabaseTHP.dm_LichLamViec, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.ct_PhieuDatHangNCC>().HasKey((Expression<Func<DatabaseTHP.ct_PhieuDatHangNCC, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.ct_PhieuDatHangNCC_ChiTiet>().HasKey((Expression<Func<DatabaseTHP.ct_PhieuDatHangNCC_ChiTiet, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_ThangLuong>().HasKey((Expression<Func<DatabaseTHP.dm_ThangLuong, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.nv_BangLuong>().HasKey((Expression<Func<DatabaseTHP.nv_BangLuong, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.nv_ChamCong>().HasKey((Expression<Func<DatabaseTHP.nv_ChamCong, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.nv_BangLuong_ChiTiet>().HasKey((Expression<Func<DatabaseTHP.nv_BangLuong_ChiTiet, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.nv_NghiPhep>().HasKey((Expression<Func<DatabaseTHP.nv_NghiPhep, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_DiaDiemChamCong>().HasKey((Expression<Func<DatabaseTHP.dm_DiaDiemChamCong, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.nv_PhepNam>().HasKey((Expression<Func<DatabaseTHP.nv_PhepNam, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_BangLuong_ChiTiet>().HasKey((Expression<Func<DatabaseTHP.dm_BangLuong_ChiTiet, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_BangLuong>().HasKey((Expression<Func<DatabaseTHP.dm_BangLuong, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.AuditLog>().HasKey((Expression<Func<DatabaseTHP.AuditLog, object>>) (m => new
    {
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.StoredProcedure.view_nv_BangLuong_ChiTiet>().HasKey((Expression<Func<DatabaseTHP.StoredProcedure.view_nv_BangLuong_ChiTiet, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_HangHoa_HinhAnh>().HasKey((Expression<Func<DatabaseTHP.dm_HangHoa_HinhAnh, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_HangHoa_MoTa>().HasKey((Expression<Func<DatabaseTHP.dm_HangHoa_MoTa, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.AspNetRequest>().HasKey((Expression<Func<DatabaseTHP.AspNetRequest, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_TaiKhoan_Misa>().HasKey((Expression<Func<DatabaseTHP.dm_TaiKhoan_Misa, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_TaiKhoan_Uniben>().HasKey((Expression<Func<DatabaseTHP.dm_TaiKhoan_Uniben, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.ct_HoaDon>().HasKey((Expression<Func<DatabaseTHP.ct_HoaDon, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.ct_HoaDon_ChiTiet>().HasKey((Expression<Func<DatabaseTHP.ct_HoaDon_ChiTiet, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_LoaiHoaDon>().HasKey((Expression<Func<DatabaseTHP.dm_LoaiHoaDon, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.uniben_dm_LienKet_HangHoa>().HasKey((Expression<Func<DatabaseTHP.uniben_dm_LienKet_HangHoa, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.uniben_dm_LienKet_KhachHang>().HasKey((Expression<Func<DatabaseTHP.uniben_dm_LienKet_KhachHang, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.uniben_dm_LienKet_NhanVien>().HasKey((Expression<Func<DatabaseTHP.uniben_dm_LienKet_NhanVien, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_HangHoa_KhungGia>().HasKey((Expression<Func<DatabaseTHP.dm_HangHoa_KhungGia, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_HangHoa_KhungGia_Master>().HasKey((Expression<Func<DatabaseTHP.dm_HangHoa_KhungGia_Master, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
    modelBuilder.Entity<DatabaseTHP.dm_HangHoa_KhungGia_HangHoa>().HasKey((Expression<Func<DatabaseTHP.dm_HangHoa_KhungGia_HangHoa, object>>) (m => new
    {
      LOC_ID = m.LOC_ID,
      ID = m.ID
    }));
  }
}
