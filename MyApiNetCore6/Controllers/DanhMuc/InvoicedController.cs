// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.InvoicedController
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.StoredProcedure.Parameter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using MyApiNetCore6.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace MyApiNetCore6.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoicedController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;
  private readonly string _connectionString;
  private string strTable = "ct_HoaDon";

  public InvoicedController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
    this._connectionString = configuration.GetConnectionString("TrangHiepPhat");
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetInput(string LOC_ID)
  {
    try
    {
      List<ct_HoaDon> lstValue = await this._context.ct_HoaDon.Where<ct_HoaDon>((Expression<Func<ct_HoaDon, bool>>) (e => e.LOC_ID == LOC_ID)).ToListAsync<ct_HoaDon>();
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) lstValue
      });
    }
    catch (Exception ex)
    {
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpGet("{LOC_ID}/{ID_KHACHHANG}/{CHUNGTUKEMTHEO}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetInput(
    string LOC_ID,
    string ID_KHACHHANG,
    string CHUNGTUKEMTHEO)
  {
    try
    {
      v_ct_HoaDon ct_HoaDon = new v_ct_HoaDon();
      ct_HoaDon = await this.GetHoaDon(LOC_ID, ID_KHACHHANG, CHUNGTUKEMTHEO);
      ct_HoaDon.lstct_HoaDon_ChiTiet = new List<v_ct_HoaDon_ChiTiet>();
      ct_HoaDon.TONGTHANHTIEN = Math.Round(ct_HoaDon.lstct_HoaDon_ChiTiet_TraVe.Sum<Product_Detail>((Func<Product_Detail, double>) (e => e.TINHCHAT != 3 ? e.THANHTIEN : -1.0 * e.THANHTIEN)), 0);
      ct_HoaDon.TONGTIENVAT = Math.Round(ct_HoaDon.lstct_HoaDon_ChiTiet_TraVe.Sum<Product_Detail>((Func<Product_Detail, double>) (e => e.TINHCHAT != 3 ? e.TONGTIENVAT : e.TONGTIENVAT * -1.0)), 0);
      ct_HoaDon.TONGTIENGIAMGIA = Math.Round(ct_HoaDon.lstct_HoaDon_ChiTiet_TraVe.Sum<Product_Detail>((Func<Product_Detail, double>) (e => e.TONGTIENGIAMGIA)), 0);
      ct_HoaDon.TONGTIEN = Math.Round(ct_HoaDon.lstct_HoaDon_ChiTiet_TraVe.Sum<Product_Detail>((Func<Product_Detail, double>) (e => e.TINHCHAT != 3 ? e.TONGCONG : e.TONGCONG * -1.0)), 0);
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) ct_HoaDon
      });
    }
    catch (Exception ex)
    {
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  private async Task<v_ct_HoaDon?> GetHoaDon(
    string LOC_ID,
    string ID_KHACHHANG,
    string CHUNGTUKEMTHEO,
    bool bolTaoHangLoat = false)
  {
    v_ct_HoaDon ct_HoaDon = new v_ct_HoaDon();
    if (!string.IsNullOrEmpty(ID_KHACHHANG))
    {
      dm_KhachHang KhachHang = await this._context.dm_KhachHang.FirstOrDefaultAsync<dm_KhachHang>((Expression<Func<dm_KhachHang, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID_KHACHHANG));
      if (KhachHang != null)
      {
        ct_HoaDon.ID_KHACHHANG = KhachHang.ID;
        ct_HoaDon.TENKHACHHANG = KhachHang.TENKHACHHANG;
        ct_HoaDon.TENDONVI = KhachHang.TENDONVI;
        ct_HoaDon.DIACHI = KhachHang.DIACHI;
        ct_HoaDon.MASOTHUE = string.IsNullOrEmpty(KhachHang.TENDONVI) ? "" : KhachHang.MASOTHUE;
        ct_HoaDon.DIENTHOAI = KhachHang.TEL;
        ct_HoaDon.EMAIL = KhachHang.EMAIL;
        ct_HoaDon.CCCD = KhachHang.CCCD;
        if (string.IsNullOrEmpty(KhachHang.TENKHACHHANG) && string.IsNullOrEmpty(KhachHang.TENDONVI))
          ct_HoaDon.TENKHACHHANG = "Khách hàng không lấy hóa đơn";
      }
      else
        ct_HoaDon.ID_KHACHHANG = (string) null;
      ct_PhieuXuat Input = await this._context.ct_PhieuXuat.Where<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == LOC_ID && (e.MAPHIEU == CHUNGTUKEMTHEO || e.ID == CHUNGTUKEMTHEO))).FirstOrDefaultAsync<ct_PhieuXuat>();
      if (Input != null)
      {
        view_dm_HangHoa HangHoa_GTBH = await this._context.view_dm_HangHoa.FirstOrDefaultAsync<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == API.GTBH));
        view_dm_HangHoa HangHoa_TINHTHUE_KM = await this._context.view_dm_HangHoa.FirstOrDefaultAsync<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == API.TINHTHUE_KM));
        List<ct_PhieuXuat_ChiTiet> lstInput_ChiTiet = await this._context.ct_PhieuXuat_ChiTiet.AsNoTracking<ct_PhieuXuat_ChiTiet>().Where<ct_PhieuXuat_ChiTiet>((Expression<Func<ct_PhieuXuat_ChiTiet, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_PHIEUXUAT == Input.ID)).ToListAsync<ct_PhieuXuat_ChiTiet>();
        if (lstInput_ChiTiet != null && lstInput_ChiTiet.Count<ct_PhieuXuat_ChiTiet>() > 0)
        {
          ct_HoaDon.lstct_HoaDon_ChiTiet_TraVe = new List<Product_Detail>();
          ct_HoaDon.lstct_HoaDon_ChiTiet = new List<v_ct_HoaDon_ChiTiet>();
          List<ct_PhieuXuat_ChiTiet> result = lstInput_ChiTiet.OrderBy<ct_PhieuXuat_ChiTiet, int>((Func<ct_PhieuXuat_ChiTiet, int>) (hd => hd.STT)).ThenBy<ct_PhieuXuat_ChiTiet, bool>((Func<ct_PhieuXuat_ChiTiet, bool>) (hd => hd.ISKHUYENMAI)).ToList<ct_PhieuXuat_ChiTiet>();
          int STT = 1;
          dm_ThueSuat VAT10 = await this._context.dm_ThueSuat.FirstOrDefaultAsync<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == "10"));
          dm_ThueSuat VAT8 = await this._context.dm_ThueSuat.FirstOrDefaultAsync<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == "8"));
          dm_ThueSuat VAT0 = await this._context.dm_ThueSuat.FirstOrDefaultAsync<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == "0"));
          dm_ThueSuat VAT5 = await this._context.dm_ThueSuat.FirstOrDefaultAsync<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == "5"));
          string[] listMaHang10 = new string[0];
          string[] listMaHang5 = new string[0];
          if (VAT10 != null && !string.IsNullOrEmpty(VAT10.GHICHU))
            listMaHang10 = ((IEnumerable<string>) VAT10.GHICHU.Replace("\r", "").Replace("\n", "").Split(new char[1]
            {
              ','
            }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (x => x.Trim())).Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x))).ToArray<string>();
          if (VAT5 != null && !string.IsNullOrEmpty(VAT5.GHICHU))
            listMaHang5 = ((IEnumerable<string>) VAT5.GHICHU.Replace("\r", "").Replace("\n", "").Split(new char[1]
            {
              ','
            }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (x => x.Trim())).Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x))).ToArray<string>();
          foreach (ct_PhieuXuat_ChiTiet phieuXuatChiTiet in result)
          {
            ct_PhieuXuat_ChiTiet itm = phieuXuatChiTiet;
            bool bolBaoKhuyenMaiVaChietKhau = false;
            dm_HangHoa_Kho HangHoaKho = await this._context.dm_HangHoa_Kho.FirstOrDefaultAsync<dm_HangHoa_Kho>((Expression<Func<dm_HangHoa_Kho, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO));
            if (HangHoaKho != null)
            {
              if (HangHoaKho == null || HangHoa_TINHTHUE_KM == null || !(HangHoaKho.ID_HANGHOA == HangHoa_TINHTHUE_KM.ID))
              {
                view_dm_HangHoa HangHoa_ = await this._context.view_dm_HangHoa.FirstOrDefaultAsync<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == HangHoaKho.ID_HANGHOA));
                if (HangHoa_ != null)
                {
                  Product_Detail Product_Detail = new Product_Detail();
                  v_ct_HoaDon_ChiTiet ct_HoaDon_ChiTiet = new v_ct_HoaDon_ChiTiet();
                  string KM = "";
                  if (itm.ISKHUYENMAI || itm.DONGIA == 0.0 && itm.THANHTIEN == 0.0)
                  {
                    Product_Detail.TINHCHAT = ct_HoaDon_ChiTiet.TINHCHAT = 1;
                    KM = " (Hàng khuyến mãi không thu tiền)";
                  }
                  else
                    Product_Detail.TINHCHAT = ct_HoaDon_ChiTiet.TINHCHAT = 1;
                  if (HangHoaKho != null && HangHoa_GTBH != null && HangHoa_.ID == HangHoa_GTBH.ID)
                  {
                    Product_Detail.TINHCHAT = ct_HoaDon_ChiTiet.TINHCHAT = 3;
                    KM = "";
                  }
                  if (itm.TONGCONG < 0.0 && itm.DONGIA == 0.0)
                  {
                    if (itm.SOLUONG > 0.0)
                      bolBaoKhuyenMaiVaChietKhau = true;
                    else
                      Product_Detail.TINHCHAT = ct_HoaDon_ChiTiet.TINHCHAT = 3;
                  }
                  if (HangHoaKho != null)
                  {
                    view_dm_HangHoa HangHoa = await this._context.view_dm_HangHoa.FirstOrDefaultAsync<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == HangHoaKho.ID_HANGHOA));
                    if (HangHoa == null || !HangHoa.ISXUATHOADON)
                    {
                      if (bolTaoHangLoat && HangHoa != null && !string.IsNullOrEmpty(HangHoa.VAT) && HangHoa.VAT != "8")
                        return (v_ct_HoaDon) null;
                      if (HangHoa != null)
                      {
                        double THUESUAT = !((IEnumerable<string>) listMaHang5).Any<string>((Func<string, bool>) (x => HangHoa.MA.StartsWith(x))) || VAT5 == null ? (!((IEnumerable<string>) listMaHang10).Any<string>((Func<string, bool>) (x => HangHoa.MA.StartsWith(x))) || VAT10 == null ? (VAT8 != null ? VAT8.THUESUAT : 0.0) : VAT10.THUESUAT) : VAT5.THUESUAT;
                        string ID_THUESUAT = !((IEnumerable<string>) listMaHang5).Any<string>((Func<string, bool>) (x => HangHoa.MA.StartsWith(x))) || VAT5 == null ? (!((IEnumerable<string>) listMaHang10).Any<string>((Func<string, bool>) (x => HangHoa.MA.StartsWith(x))) || VAT10 == null ? (VAT8 != null ? VAT8.ID : "") : VAT10.ID) : VAT5.ID;
                        dm_DonViTinh DVT = await this._context.dm_DonViTinh.FirstOrDefaultAsync<dm_DonViTinh>((Expression<Func<dm_DonViTinh, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_DVT));
                        Product_Detail productDetail1 = Product_Detail;
                        v_ct_HoaDon_ChiTiet vCtHoaDonChiTiet1 = ct_HoaDon_ChiTiet;
                        Guid guid1 = Guid.NewGuid();
                        string str1;
                        string str2 = str1 = guid1.ToString();
                        vCtHoaDonChiTiet1.ID = str1;
                        string str3 = str2;
                        productDetail1.ID = str3;
                        Product_Detail.MAHANGHOA = ct_HoaDon_ChiTiet.MAHANGHOA = HangHoa.MA;
                        Product_Detail.TENHANGHOA = ct_HoaDon_ChiTiet.TENHANGHOA = HangHoa.NAME + KM;
                        Product_Detail.ID_DVT = ct_HoaDon_ChiTiet.ID_DVT = DVT != null ? DVT.ID : (string) null;
                        Product_Detail.DVT = ct_HoaDon_ChiTiet.DVT = DVT != null ? DVT.NAME : (string) null;
                        Product_Detail.SOLUONG = ct_HoaDon_ChiTiet.SOLUONG = itm.SOLUONG;
                        Product_Detail.DONGIA = ct_HoaDon_ChiTiet.DONGIA = itm.DONGIA;
                        Product_Detail.CHIETKHAU = ct_HoaDon_ChiTiet.CHIETKHAU = itm.CHIETKHAU;
                        Product_Detail.TONGTIENGIAMGIA = ct_HoaDon_ChiTiet.TONGTIENGIAMGIA = itm.TONGTIENGIAMGIA;
                        Product_Detail.THANHTIEN = ct_HoaDon_ChiTiet.THANHTIEN = itm.THANHTIEN;
                        Product_Detail.ID_THUESUAT = ct_HoaDon_ChiTiet.ID_THUESUAT = itm.ID_THUESUAT;
                        Product_Detail.THUESUAT = ct_HoaDon_ChiTiet.THUESUAT = itm.THUESUAT;
                        Product_Detail.TONGTIENVAT = ct_HoaDon_ChiTiet.TONGTIENVAT = itm.TONGTIENVAT;
                        Product_Detail.TONGCONG = ct_HoaDon_ChiTiet.TONGCONG = Math.Round(itm.TONGCONG, 0);
                        Product_Detail.ID_PHIEUXUAT_CHITIET = ct_HoaDon_ChiTiet.ID_PHIEUXUAT_CHITIET = itm.ID;
                        Product_Detail.ISKHUYENMAI = itm.ISKHUYENMAI;
                        Product_Detail.STT = ct_HoaDon_ChiTiet.STT = STT;
                        if (!string.IsNullOrEmpty(KM))
                        {
                          Product_Detail.ID_THUESUAT = ct_HoaDon_ChiTiet.ID_THUESUAT = itm.ID_THUESUAT = ID_THUESUAT;
                          Product_Detail.THUESUAT = THUESUAT;
                          Product_Detail.CHIETKHAU = ct_HoaDon_ChiTiet.CHIETKHAU = 0.0;
                          Product_Detail.TONGTIENGIAMGIA = ct_HoaDon_ChiTiet.TONGTIENGIAMGIA = 0.0;
                          Product_Detail.THANHTIEN = ct_HoaDon_ChiTiet.THANHTIEN = 0.0;
                          Product_Detail.TONGTIENVAT = ct_HoaDon_ChiTiet.TONGTIENVAT = 0.0;
                          Product_Detail.TONGCONG = ct_HoaDon_ChiTiet.TONGCONG = 0.0;
                        }
                        else if (ct_HoaDon_ChiTiet.TINHCHAT == 3)
                        {
                          Product_Detail.SOLUONG = ct_HoaDon_ChiTiet.SOLUONG = 1.0;
                          if (itm.THANHTIEN < 0.0)
                            itm.THANHTIEN *= -1.0;
                          Product_Detail.THUESUAT = ct_HoaDon_ChiTiet.THUESUAT = itm.THUESUAT = THUESUAT;
                          Product_Detail.ID_THUESUAT = ct_HoaDon_ChiTiet.ID_THUESUAT = itm.ID_THUESUAT = ID_THUESUAT;
                          double GiaChuaVAT = itm.THANHTIEN / (1.0 + Product_Detail.THUESUAT / 100.0);
                          Product_Detail.DONGIA = ct_HoaDon_ChiTiet.DONGIA = Math.Round(GiaChuaVAT, 2);
                          Product_Detail.CHIETKHAU = ct_HoaDon_ChiTiet.CHIETKHAU = 0.0;
                          Product_Detail.TONGTIENGIAMGIA = ct_HoaDon_ChiTiet.TONGTIENGIAMGIA = 0.0;
                          Product_Detail.THANHTIEN = ct_HoaDon_ChiTiet.THANHTIEN = Math.Round(GiaChuaVAT * Product_Detail.SOLUONG, 0);
                          Product_Detail.TONGCONG = ct_HoaDon_ChiTiet.TONGCONG = Math.Round(itm.THANHTIEN, 0);
                          Product_Detail.TONGTIENVAT = ct_HoaDon_ChiTiet.TONGTIENVAT = Math.Round(Product_Detail.TONGCONG - Product_Detail.THANHTIEN, 0);
                        }
                        else if (string.IsNullOrEmpty(itm.ID_THUESUAT))
                        {
                          Product_Detail.THUESUAT = ct_HoaDon_ChiTiet.THUESUAT = itm.THUESUAT = THUESUAT;
                          Product_Detail.ID_THUESUAT = ct_HoaDon_ChiTiet.ID_THUESUAT = itm.ID_THUESUAT = ID_THUESUAT;
                          double GiaChuaVAT = Product_Detail.DONGIA / (1.0 + itm.THUESUAT / 100.0);
                          Product_Detail.DONGIA = ct_HoaDon_ChiTiet.DONGIA = Math.Round(GiaChuaVAT, 2);
                          Product_Detail.TONGTIENGIAMGIA = ct_HoaDon_ChiTiet.TONGTIENGIAMGIA = itm.TONGTIENGIAMGIA = Math.Round(itm.TONGTIENGIAMGIA / (1.0 + Product_Detail.THUESUAT / 100.0), 0);
                          Product_Detail.CHIETKHAU = itm.TONGTIENGIAMGIA <= 0.0 ? (ct_HoaDon_ChiTiet.CHIETKHAU = itm.CHIETKHAU) : (ct_HoaDon_ChiTiet.CHIETKHAU = GiaChuaVAT == 0.0 || itm.SOLUONG == 0.0 ? 0.0 : Math.Round(itm.TONGTIENGIAMGIA / (Product_Detail.DONGIA * itm.SOLUONG) * 100.0, 2));
                          Product_Detail.THANHTIEN = ct_HoaDon_ChiTiet.THANHTIEN = Math.Round(Product_Detail.DONGIA * Product_Detail.SOLUONG - Product_Detail.TONGTIENGIAMGIA, 0);
                          double VAT = Math.Round(Product_Detail.THANHTIEN * (Product_Detail.THUESUAT / 100.0), 0);
                          Product_Detail.TONGTIENVAT = ct_HoaDon_ChiTiet.TONGTIENVAT = Math.Round(Product_Detail.TONGCONG - Product_Detail.TONGTIENVAT - Product_Detail.THANHTIEN, 0);
                          if (VAT - Product_Detail.TONGTIENVAT > 1.0)
                          {
                            --Product_Detail.THANHTIEN;
                            ++Product_Detail.TONGTIENVAT;
                          }
                          Product_Detail.TONGCONG = ct_HoaDon_ChiTiet.TONGCONG = Product_Detail.TONGTIENVAT + Product_Detail.THANHTIEN;
                        }
                        if (bolBaoKhuyenMaiVaChietKhau && HangHoa_GTBH != null)
                        {
                          Product_Detail = new Product_Detail();
                          ct_HoaDon_ChiTiet = new v_ct_HoaDon_ChiTiet();
                          Product_Detail.TINHCHAT = ct_HoaDon_ChiTiet.TINHCHAT = 3;
                          Product_Detail productDetail2 = Product_Detail;
                          v_ct_HoaDon_ChiTiet vCtHoaDonChiTiet2 = ct_HoaDon_ChiTiet;
                          Guid guid2 = Guid.NewGuid();
                          string str4;
                          string str5 = str4 = guid2.ToString();
                          vCtHoaDonChiTiet2.ID = str4;
                          string str6 = str5;
                          productDetail2.ID = str6;
                          Product_Detail.MAHANGHOA = ct_HoaDon_ChiTiet.MAHANGHOA = HangHoa_GTBH.MA;
                          Product_Detail.TENHANGHOA = ct_HoaDon_ChiTiet.TENHANGHOA = HangHoa_GTBH.NAME;
                          Product_Detail.ID_DVT = ct_HoaDon_ChiTiet.ID_DVT = (string) null;
                          Product_Detail.DVT = ct_HoaDon_ChiTiet.DVT = "Tiền";
                          Product_Detail.SOLUONG = ct_HoaDon_ChiTiet.SOLUONG = 1.0;
                          if (itm.THANHTIEN < 0.0)
                            itm.THANHTIEN *= -1.0;
                          Product_Detail.THUESUAT = ct_HoaDon_ChiTiet.THUESUAT = itm.THUESUAT = THUESUAT;
                          Product_Detail.ID_THUESUAT = ct_HoaDon_ChiTiet.ID_THUESUAT = itm.ID_THUESUAT = ID_THUESUAT;
                          double GiaChuaVAT = itm.THANHTIEN / (1.0 + Product_Detail.THUESUAT / 100.0);
                          Product_Detail.DONGIA = ct_HoaDon_ChiTiet.DONGIA = Math.Round(GiaChuaVAT, 2);
                          Product_Detail.CHIETKHAU = ct_HoaDon_ChiTiet.CHIETKHAU = 0.0;
                          Product_Detail.TONGTIENGIAMGIA = ct_HoaDon_ChiTiet.TONGTIENGIAMGIA = 0.0;
                          Product_Detail.THANHTIEN = ct_HoaDon_ChiTiet.THANHTIEN = Math.Round(GiaChuaVAT * Product_Detail.SOLUONG, 0);
                          Product_Detail.TONGCONG = ct_HoaDon_ChiTiet.TONGCONG = Math.Round(itm.THANHTIEN, 0);
                          Product_Detail.TONGTIENVAT = ct_HoaDon_ChiTiet.TONGTIENVAT = Math.Round(Product_Detail.TONGCONG - Product_Detail.THANHTIEN, 0);
                          Product_Detail.ID_PHIEUXUAT_CHITIET = ct_HoaDon_ChiTiet.ID_PHIEUXUAT_CHITIET = itm.ID;
                          Product_Detail.ISKHUYENMAI = itm.ISKHUYENMAI;
                          Product_Detail.STT = ct_HoaDon_ChiTiet.STT = STT;
                          ct_HoaDon.lstct_HoaDon_ChiTiet_TraVe.Add(Product_Detail);
                          ct_HoaDon.lstct_HoaDon_ChiTiet.Add(ct_HoaDon_ChiTiet);
                          ++STT;
                        }
                        else
                        {
                          ct_HoaDon.lstct_HoaDon_ChiTiet_TraVe.Add(Product_Detail);
                          ct_HoaDon.lstct_HoaDon_ChiTiet.Add(ct_HoaDon_ChiTiet);
                          ++STT;
                        }
                        ID_THUESUAT = (string) null;
                        DVT = (dm_DonViTinh) null;
                      }
                    }
                    else
                      continue;
                  }
                  HangHoa_ = (view_dm_HangHoa) null;
                  Product_Detail = (Product_Detail) null;
                  ct_HoaDon_ChiTiet = (v_ct_HoaDon_ChiTiet) null;
                  KM = (string) null;
                }
                else
                  continue;
              }
              else
                continue;
            }
          }
          result = (List<ct_PhieuXuat_ChiTiet>) null;
          VAT10 = (dm_ThueSuat) null;
          VAT8 = (dm_ThueSuat) null;
          VAT0 = (dm_ThueSuat) null;
          VAT5 = (dm_ThueSuat) null;
          listMaHang10 = (string[]) null;
          listMaHang5 = (string[]) null;
        }
        HangHoa_GTBH = (view_dm_HangHoa) null;
        HangHoa_TINHTHUE_KM = (view_dm_HangHoa) null;
        lstInput_ChiTiet = (List<ct_PhieuXuat_ChiTiet>) null;
      }
      KhachHang = (dm_KhachHang) null;
    }
    return ct_HoaDon;
  }

  [HttpGet("{LOC_ID}/{Type}/{KeyWhere}/{ValuesSearch}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetInput(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      List<ct_HoaDon> lstValue = await this._context.ct_HoaDon.Where<ct_HoaDon>((Expression<Func<ct_HoaDon, bool>>) (e => e.LOC_ID == LOC_ID)).Where<ct_HoaDon>(KeyWhere, (object) ValuesSearch).ToListAsync<ct_HoaDon>();
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) lstValue
      });
    }
    catch (Exception ex)
    {
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpGet("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetInput(string LOC_ID, string ID)
  {
    try
    {
      ct_HoaDon Input = await this._context.ct_HoaDon.FirstOrDefaultAsync<ct_HoaDon>((Expression<Func<ct_HoaDon, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Input == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      v_ct_HoaDon ct_HoaDon = new v_ct_HoaDon();
      if (Input != null)
      {
        string strInput = JsonConvert.SerializeObject((object) Input);
        ct_HoaDon = JsonConvert.DeserializeObject<v_ct_HoaDon>(strInput) ?? new v_ct_HoaDon();
        strInput = (string) null;
      }
      ct_HoaDon.lstct_HoaDon_ChiTiet = new List<v_ct_HoaDon_ChiTiet>();
      SP_Parameter SP_Parameter = new SP_Parameter();
      SP_Parameter.ID_HOADON = ID;
      ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
      IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachHoaDon_Chitiet(SP_Parameter);
      if (actionResult is OkObjectResult okResult)
      {
        ApiResponse ApiResponse = okResult.Value as ApiResponse;
        if (ApiResponse != null && ApiResponse.Data != null)
        {
          if (ApiResponse.Data is List<v_ct_HoaDon_ChiTiet> lst_ChiTiet)
            ct_HoaDon.lstct_HoaDon_ChiTiet.AddRange((IEnumerable<v_ct_HoaDon_ChiTiet>) lst_ChiTiet);
          lst_ChiTiet = (List<v_ct_HoaDon_ChiTiet>) null;
        }
        ApiResponse = (ApiResponse) null;
      }
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) ct_HoaDon
      });
    }
    catch (Exception ex)
    {
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpPut("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PutInput(string LOC_ID, string ID, [FromBody] v_ct_HoaDon Input)
  {
    try
    {
      if (!this.InputExistsID(Input.LOC_ID, Input.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {Input.LOC_ID}-{Input.ID} dữ liệu!",
          Data = (object) ""
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        ct_HoaDon objHoaDon = await this._context.ct_HoaDon.AsNoTracking<ct_HoaDon>().FirstOrDefaultAsync<ct_HoaDon>((Expression<Func<ct_HoaDon, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.ID == ID));
        if (objHoaDon != null && objHoaDon.ISXUATHOADON)
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = $"Chứng từ {objHoaDon.MAPHIEU} đã được lập hóa đơn!",
            Data = (object) "",
            CheckValue = true
          });
        if (objHoaDon != null && objHoaDon.CHUNGTUKEMTHEO != Input.CHUNGTUKEMTHEO)
        {
          if (!string.IsNullOrEmpty(Input.CHUNGTUKEMTHEO))
          {
            ct_PhieuXuat objOutput = await this._context.ct_PhieuXuat.FirstOrDefaultAsync<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO));
            if (objOutput == null)
              return (IActionResult) this.Ok((object) new ApiResponse()
              {
                Success = false,
                Message = $"Không tồn tại{Input.LOC_ID}-{Input.CHUNGTUKEMTHEO} trong dữ liệu!",
                Data = (object) "",
                CheckValue = true
              });
            if (!string.IsNullOrEmpty(objOutput.ID_HOADON))
              return (IActionResult) this.Ok((object) new ApiResponse()
              {
                Success = false,
                Message = $"Chứng từ {Input.CHUNGTUKEMTHEO} đã được lập hóa đơn!",
                Data = (object) "",
                CheckValue = true
              });
            objOutput = (ct_PhieuXuat) null;
          }
          ct_PhieuXuat objOutput_Old = await this._context.ct_PhieuXuat.FirstOrDefaultAsync<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == objHoaDon.LOC_ID && e.MAPHIEU == objHoaDon.CHUNGTUKEMTHEO));
          if (objOutput_Old != null)
          {
            objOutput_Old.ID_HOADON = (string) null;
            this._context.Entry<ct_PhieuXuat>(objOutput_Old).State = EntityState.Modified;
          }
          objOutput_Old = (ct_PhieuXuat) null;
        }
        List<ct_HoaDon_ChiTiet> lstHoaDon_ChiTiet = await this._context.ct_HoaDon_ChiTiet.Where<ct_HoaDon_ChiTiet>((Expression<Func<ct_HoaDon_ChiTiet, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.ID_HOADON == Input.ID)).ToListAsync<ct_HoaDon_ChiTiet>();
        if (lstHoaDon_ChiTiet != null)
        {
          foreach (ct_HoaDon_ChiTiet ctHoaDonChiTiet in lstHoaDon_ChiTiet)
          {
            ct_HoaDon_ChiTiet itm = ctHoaDonChiTiet;
            v_ct_HoaDon_ChiTiet chkHoaDon_ChiTiet = Input.lstct_HoaDon_ChiTiet.Where<v_ct_HoaDon_ChiTiet>((Func<v_ct_HoaDon_ChiTiet, bool>) (e => e.ID == itm.ID)).FirstOrDefault<v_ct_HoaDon_ChiTiet>();
            if (chkHoaDon_ChiTiet != null)
            {
              itm.TINHCHAT = chkHoaDon_ChiTiet.TINHCHAT;
              itm.ID_HANGHOAKHO = chkHoaDon_ChiTiet.ID_HANGHOAKHO;
              itm.MAHANGHOA = chkHoaDon_ChiTiet.MAHANGHOA;
              itm.TENHANGHOA = chkHoaDon_ChiTiet.TENHANGHOA;
              itm.ID_DVT = chkHoaDon_ChiTiet.ID_DVT;
              itm.DVT = chkHoaDon_ChiTiet.DVT;
              itm.DONGIA = chkHoaDon_ChiTiet.DONGIA;
              itm.CHIETKHAU = chkHoaDon_ChiTiet.CHIETKHAU;
              itm.TONGTIENGIAMGIA = chkHoaDon_ChiTiet.TONGTIENGIAMGIA;
              itm.THANHTIEN = chkHoaDon_ChiTiet.THANHTIEN;
              itm.ID_THUESUAT = chkHoaDon_ChiTiet.ID_THUESUAT;
              dm_ThueSuat VAT = await this._context.dm_ThueSuat.FirstOrDefaultAsync<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == objHoaDon.LOC_ID && e.ID == chkHoaDon_ChiTiet.ID_THUESUAT));
              itm.THUESUAT = VAT != null ? VAT.THUESUAT : chkHoaDon_ChiTiet.THUESUAT;
              itm.TONGTIENVAT = chkHoaDon_ChiTiet.TONGTIENVAT;
              itm.TONGCONG = chkHoaDon_ChiTiet.TONGCONG;
              itm.STT = chkHoaDon_ChiTiet.STT;
              itm.LOC_ID = chkHoaDon_ChiTiet.LOC_ID;
              itm.SOLUONG = chkHoaDon_ChiTiet.SOLUONG;
              chkHoaDon_ChiTiet.ISEDIT = true;
              chkHoaDon_ChiTiet.ID_HOADON = Input.ID;
              this._context.Entry<ct_HoaDon_ChiTiet>(itm).State = EntityState.Modified;
              VAT = (dm_ThueSuat) null;
            }
            else
              this._context.ct_HoaDon_ChiTiet.Remove(itm);
          }
        }
        if (Input.lstct_HoaDon_ChiTiet != null)
        {
          foreach (v_ct_HoaDon_ChiTiet itm in Input.lstct_HoaDon_ChiTiet)
          {
            if (!itm.ISEDIT)
            {
              itm.ID_HOADON = Input.ID;
              this._context.ct_HoaDon_ChiTiet.Add((ct_HoaDon_ChiTiet) itm);
            }
          }
        }
        this._context.Entry<v_ct_HoaDon>(Input).State = EntityState.Modified;
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        lstHoaDon_ChiTiet = (List<ct_HoaDon_ChiTiet>) null;
        auditLog = (AuditLogController) null;
        transaction.Commit();
        v_ct_HoaDon ct_HoaDon = new v_ct_HoaDon();
        ct_HoaDon.lstct_HoaDon_ChiTiet = new List<v_ct_HoaDon_ChiTiet>();
        SP_Parameter SP_Parameter = new SP_Parameter();
        SP_Parameter.ID_HOADON = Input.ID;
        ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
        IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachHoaDon(SP_Parameter);
        if (actionResult is OkObjectResult okResult)
        {
          ApiResponse ApiResponse = okResult.Value as ApiResponse;
          if (ApiResponse != null && ApiResponse.Data != null)
          {
            List<v_ct_HoaDon> lstHoaDon = ApiResponse.Data as List<v_ct_HoaDon>;
            if (lstHoaDon != null && lstHoaDon.Count<v_ct_HoaDon>() > 0)
              ct_HoaDon = lstHoaDon.FirstOrDefault<v_ct_HoaDon>() ?? new v_ct_HoaDon();
            lstHoaDon = (List<v_ct_HoaDon>) null;
          }
          ApiResponse = (ApiResponse) null;
        }
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) ct_HoaDon
        });
      }
    }
    catch (DbUpdateConcurrencyException ex)
    {
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
    finally
    {
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.DeleteRequest(this.strTable);
      auditLog = (AuditLogController) null;
    }
  }

  [HttpPost]
  [Authorize(Roles = "User")]
  public async Task<ActionResult<ct_HoaDon>> PostInput([FromBody] v_ct_HoaDon Input)
  {
    try
    {
      if (this.InputExistsID(Input.LOC_ID, Input.ID))
        return (ActionResult<ct_HoaDon>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Input.LOC_ID}-{Input.ID} trong dữ liệu!",
          Data = (object) "",
          CheckValue = true
        });
      ct_PhieuXuat objOutput = new ct_PhieuXuat();
      if (!string.IsNullOrEmpty(Input.CHUNGTUKEMTHEO))
      {
        ct_PhieuXuat result = await this._context.ct_PhieuXuat.FirstOrDefaultAsync<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO));
        if (result == null)
          return (ActionResult<ct_HoaDon>) (ActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = $"Không tồn tại{Input.LOC_ID}-{Input.CHUNGTUKEMTHEO} trong dữ liệu!",
            Data = (object) "",
            CheckValue = true
          });
        objOutput = result;
        if (!string.IsNullOrEmpty(result.ID_HOADON))
          return (ActionResult<ct_HoaDon>) (ActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = $"Chứng từ {Input.CHUNGTUKEMTHEO} đã được lập hóa đơn!",
            Data = (object) "",
            CheckValue = true
          });
        ct_HoaDon objHoaDon = await this._context.ct_HoaDon.FirstOrDefaultAsync<ct_HoaDon>((Expression<Func<ct_HoaDon, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.CHUNGTUKEMTHEO == Input.CHUNGTUKEMTHEO));
        if (objHoaDon != null)
          return (ActionResult<ct_HoaDon>) (ActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = $"Đã tồn tại{Input.LOC_ID}-{Input.CHUNGTUKEMTHEO} trong dữ liệu!",
            Data = (object) "",
            CheckValue = true
          });
        result = (ct_PhieuXuat) null;
        objHoaDon = (ct_HoaDon) null;
      }
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        if (Input.lstct_HoaDon_ChiTiet != null)
        {
          foreach (v_ct_HoaDon_ChiTiet vCtHoaDonChiTiet in Input.lstct_HoaDon_ChiTiet)
          {
            v_ct_HoaDon_ChiTiet itm = vCtHoaDonChiTiet;
            itm.LOC_ID = Input.LOC_ID;
            itm.ID_HOADON = Input.ID;
            dm_ThueSuat VAT = await this._context.dm_ThueSuat.FirstOrDefaultAsync<dm_ThueSuat>((Expression<Func<dm_ThueSuat, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_THUESUAT));
            itm.THUESUAT = VAT != null ? VAT.THUESUAT : itm.THUESUAT;
            this._context.ct_HoaDon_ChiTiet.Add((ct_HoaDon_ChiTiet) itm);
            VAT = (dm_ThueSuat) null;
          }
        }
        Input.ID_PHIEUXUAT = objOutput != null ? objOutput.ID : (string) null;
        this._context.ct_HoaDon.Add((ct_HoaDon) Input);
        if (objOutput != null && !string.IsNullOrEmpty(objOutput.ID))
        {
          objOutput.ID_HOADON = Input.ID;
          this._context.Entry<ct_PhieuXuat>(objOutput).State = EntityState.Modified;
        }
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num1 = await this._context.SaveChangesAsync();
        auditLog = (AuditLogController) null;
        transaction.Commit();
        List<ct_HoaDon> lstPhieuDatHangCheck = await this._context.ct_HoaDon.Where<ct_HoaDon>((Expression<Func<ct_HoaDon, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.MAPHIEU)).OrderByDescending<ct_HoaDon, DateTime>((Expression<Func<ct_HoaDon, DateTime>>) (e => e.NGAYLAP)).ToListAsync<ct_HoaDon>();
        if (lstPhieuDatHangCheck != null && lstPhieuDatHangCheck.Count<ct_HoaDon>() > 1 && lstPhieuDatHangCheck.FirstOrDefault<ct_HoaDon>().ID == Input.ID)
        {
          int Max_ID = this._context.ct_HoaDon.Where<ct_HoaDon>((Expression<Func<ct_HoaDon, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.NGAYLAP.Date == Input.NGAYLAP.Date)).Select<ct_HoaDon, int>((Expression<Func<ct_HoaDon, int>>) (e => e.SOPHIEU)).DefaultIfEmpty<int>().Max<int>();
          Input.SOPHIEU = Max_ID + 1;
          Input.MAPHIEU = API.GetMaPhieu("Invoiced", Input.NGAYLAP, Input.SOPHIEU);
          this._context.Entry<v_ct_HoaDon>(Input).State = EntityState.Modified;
          int num2 = await this._context.SaveChangesAsync();
        }
        v_ct_HoaDon ct_HoaDon = new v_ct_HoaDon();
        ct_HoaDon.lstct_HoaDon_ChiTiet = new List<v_ct_HoaDon_ChiTiet>();
        SP_Parameter SP_Parameter = new SP_Parameter();
        SP_Parameter.ID_HOADON = Input.ID;
        ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(this._context, this._configuration);
        IActionResult actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachHoaDon(SP_Parameter);
        if (actionResult is OkObjectResult okResult)
        {
          ApiResponse ApiResponse = okResult.Value as ApiResponse;
          if (ApiResponse != null && ApiResponse.Data != null)
          {
            List<v_ct_HoaDon> lstHoaDon = ApiResponse.Data as List<v_ct_HoaDon>;
            if (lstHoaDon != null && lstHoaDon.Count<v_ct_HoaDon>() > 0)
              ct_HoaDon = lstHoaDon.FirstOrDefault<v_ct_HoaDon>() ?? new v_ct_HoaDon();
            lstHoaDon = (List<v_ct_HoaDon>) null;
          }
          ApiResponse = (ApiResponse) null;
        }
        return (ActionResult<ct_HoaDon>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) ct_HoaDon
        });
      }
    }
    catch (Exception ex)
    {
      return (ActionResult<ct_HoaDon>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
    finally
    {
      AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
      auditLog.DeleteRequest(this.strTable);
      auditLog = (AuditLogController) null;
    }
  }

  [HttpPost("PostCreateOutput")]
  [Authorize(Roles = "User")]
  public async Task<ActionResult<ct_PhieuDatHang>> PostDeposit([FromBody] List<Deposit> lstDeposit)
  {
    try
    {
      string LOC_ID = "";
      string ID_NGUOITAO = "";
      string ID_LOAIHOADON = "";
      DateTime NGAYLAP = new DateTime();
      DateTime dateTime1 = DateTime.Now;
      NGAYLAP = dateTime1.Date;
      if (lstDeposit != null && lstDeposit.Count > 0)
      {
        Deposit Deposit = lstDeposit.FirstOrDefault<Deposit>() ?? new Deposit();
        LOC_ID = Deposit != null ? Deposit.LOC_ID : "";
        ID_NGUOITAO = Deposit != null ? Deposit.ID_NGUOITAO : "";
        DateTime dateTime2;
        if (Deposit == null)
        {
          dateTime1 = DateTime.Now;
          dateTime2 = dateTime1.Date;
        }
        else
          dateTime2 = Deposit.NGAYLAP;
        NGAYLAP = dateTime2;
        ID_LOAIHOADON = Deposit != null ? Deposit.ID_LOAIHOADON : "";
        Deposit = (Deposit) null;
        Dictionary<string, string> lstPhieuDatHang = new Dictionary<string, string>();
        using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
        {
          int Max_ID = this._context.ct_HoaDon.Where<ct_HoaDon>((Expression<Func<ct_HoaDon, bool>>) (e => e.LOC_ID == LOC_ID && e.NGAYLAP.Date == NGAYLAP.Date)).Select<ct_HoaDon, int>((Expression<Func<ct_HoaDon, int>>) (e => e.SOPHIEU)).DefaultIfEmpty<int>().Max<int>();
          foreach (Deposit deposit in lstDeposit)
          {
            Deposit itm = deposit;
            ct_PhieuXuat PhieuDatHang = await this._context.ct_PhieuXuat.FirstOrDefaultAsync<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID));
            if (PhieuDatHang == null)
              return (ActionResult<ct_PhieuDatHang>) (ActionResult) this.Ok((object) new ApiResponse()
              {
                Success = false,
                Message = $"Không tìm thấy {LOC_ID}-{itm.ID} dữ liệu phiếu xuất!",
                Data = (object) ""
              });
            if (!string.IsNullOrEmpty(PhieuDatHang.ID_HOADON))
              return (ActionResult<ct_PhieuDatHang>) (ActionResult) this.Ok((object) new ApiResponse()
              {
                Success = false,
                Message = $"Phiếu xuất {PhieuDatHang.MAPHIEU} đã được tạo hóa đơn!",
                Data = (object) ""
              });
            ++Max_ID;
            v_ct_HoaDon ct_HoaDon = new v_ct_HoaDon();
            ct_HoaDon = await this.GetHoaDon(LOC_ID, PhieuDatHang.ID_KHACHHANG, PhieuDatHang.ID, true);
            if (ct_HoaDon != null)
            {
              ct_HoaDon.ID = Guid.NewGuid().ToString();
              if (ct_HoaDon.lstct_HoaDon_ChiTiet != null)
              {
                foreach (v_ct_HoaDon_ChiTiet s in ct_HoaDon.lstct_HoaDon_ChiTiet)
                {
                  s.LOC_ID = LOC_ID;
                  s.ID_HOADON = ct_HoaDon.ID;
                  this._context.ct_HoaDon_ChiTiet.Add((ct_HoaDon_ChiTiet) s);
                }
                ct_HoaDon.TONGTHANHTIEN = Math.Round(ct_HoaDon.lstct_HoaDon_ChiTiet_TraVe.Sum<Product_Detail>((Func<Product_Detail, double>) (e => e.TINHCHAT != 3 ? e.THANHTIEN : -1.0 * e.THANHTIEN)), 0);
                ct_HoaDon.TONGTIENVAT = Math.Round(ct_HoaDon.lstct_HoaDon_ChiTiet_TraVe.Sum<Product_Detail>((Func<Product_Detail, double>) (e => e.TINHCHAT != 3 ? e.TONGTIENVAT : e.TONGTIENVAT * -1.0)), 0);
                ct_HoaDon.TONGTIENGIAMGIA = Math.Round(ct_HoaDon.lstct_HoaDon_ChiTiet_TraVe.Sum<Product_Detail>((Func<Product_Detail, double>) (e => e.TONGTIENGIAMGIA)), 0);
                ct_HoaDon.TONGTIEN = Math.Round(ct_HoaDon.lstct_HoaDon_ChiTiet_TraVe.Sum<Product_Detail>((Func<Product_Detail, double>) (e => e.TINHCHAT != 3 ? e.TONGCONG : e.TONGCONG * -1.0)), 0);
              }
              ct_HoaDon.HTTT = "TM/CK";
              ct_HoaDon.LOAITIEN = "VND";
              ct_HoaDon.TYGIA = new double?(1.0);
              v_ct_HoaDon vCtHoaDon = ct_HoaDon;
              ct_HoaDon.NGAYLAP = dateTime1 = NGAYLAP;
              DateTime dateTime3 = dateTime1;
              vCtHoaDon.NGAYHOADON = dateTime3;
              ct_HoaDon.ID_NGUOITAO = ID_NGUOITAO;
              ct_HoaDon.LOC_ID = LOC_ID;
              ct_HoaDon.ID_LOAIHOADON = ID_LOAIHOADON;
              ct_HoaDon.ID_PHIEUXUAT = PhieuDatHang != null ? PhieuDatHang.ID : (string) null;
              ct_HoaDon.CHUNGTUKEMTHEO = PhieuDatHang != null ? PhieuDatHang.MAPHIEU : (string) null;
              ct_HoaDon.SOPHIEU = Max_ID;
              ct_HoaDon.MAPHIEU = API.GetMaPhieu("Invoiced", ct_HoaDon.NGAYLAP, ct_HoaDon.SOPHIEU);
              this._context.ct_HoaDon.Add((ct_HoaDon) ct_HoaDon);
              if (ct_HoaDon != null && !string.IsNullOrEmpty(ct_HoaDon.ID) && PhieuDatHang != null && !string.IsNullOrEmpty(PhieuDatHang.ID))
              {
                PhieuDatHang.ID_HOADON = ct_HoaDon.ID;
                this._context.Entry<ct_PhieuXuat>(PhieuDatHang).State = EntityState.Modified;
              }
              PhieuDatHang = (ct_PhieuXuat) null;
              ct_HoaDon = (v_ct_HoaDon) null;
            }
          }
          AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
          auditLog.InserAuditLog();
          int num = await this._context.SaveChangesAsync();
          auditLog = (AuditLogController) null;
          transaction.Commit();
          return (ActionResult<ct_PhieuDatHang>) (ActionResult) this.Ok((object) new ApiResponse()
          {
            Success = true,
            Message = "Success",
            Data = (object) ""
          });
        }
      }
      return (ActionResult<ct_PhieuDatHang>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = "Không tìm thấy dữ liệu!",
        Data = (object) ""
      });
    }
    catch (Exception ex)
    {
      return (ActionResult<ct_PhieuDatHang>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteInput(string LOC_ID, string ID)
  {
    try
    {
      ct_HoaDon Input = await this._context.ct_HoaDon.FirstOrDefaultAsync<ct_HoaDon>((Expression<Func<ct_HoaDon, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Input == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      if (Input.ISXUATHOADON)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Chứng từ {Input.MAPHIEU} đã được tạo hóa đơn!",
          Data = (object) ""
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        ct_PhieuXuat objOutput = await this._context.ct_PhieuXuat.FirstOrDefaultAsync<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO));
        if (objOutput != null)
        {
          objOutput.ID_HOADON = (string) null;
          this._context.Entry<ct_PhieuXuat>(objOutput).State = EntityState.Modified;
        }
        List<ct_HoaDon_ChiTiet> lstHoaDon_ChiTiet = await this._context.ct_HoaDon_ChiTiet.Where<ct_HoaDon_ChiTiet>((Expression<Func<ct_HoaDon_ChiTiet, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.ID_HOADON == Input.ID)).ToListAsync<ct_HoaDon_ChiTiet>();
        if (lstHoaDon_ChiTiet != null)
        {
          foreach (ct_HoaDon_ChiTiet itm in lstHoaDon_ChiTiet)
            this._context.ct_HoaDon_ChiTiet.Remove(itm);
        }
        this._context.ct_HoaDon.Remove(Input);
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        objOutput = (ct_PhieuXuat) null;
        lstHoaDon_ChiTiet = (List<ct_HoaDon_ChiTiet>) null;
        auditLog = (AuditLogController) null;
        transaction.Commit();
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) ""
        });
      }
    }
    catch (Exception ex)
    {
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpPut("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> Delete(string LOC_ID, [FromBody] List<Deposit> lstDeposit)
  {
    try
    {
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        foreach (Deposit deposit in lstDeposit)
        {
          Deposit s = deposit;
          ct_HoaDon Input = await this._context.ct_HoaDon.FirstOrDefaultAsync<ct_HoaDon>((Expression<Func<ct_HoaDon, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == s.ID));
          if (Input == null)
            return (IActionResult) this.Ok((object) new ApiResponse()
            {
              Success = false,
              Message = $"Không tìm thấy {LOC_ID}-{s.ID} dữ liệu!",
              Data = (object) ""
            });
          if (Input.ISXUATHOADON)
            return (IActionResult) this.Ok((object) new ApiResponse()
            {
              Success = false,
              Message = $"Chứng từ {Input.MAPHIEU} đã được tạo hóa đơn!",
              Data = (object) ""
            });
          ct_PhieuXuat objOutput = await this._context.ct_PhieuXuat.FirstOrDefaultAsync<ct_PhieuXuat>((Expression<Func<ct_PhieuXuat, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO));
          if (objOutput != null)
          {
            objOutput.ID_HOADON = (string) null;
            this._context.Entry<ct_PhieuXuat>(objOutput).State = EntityState.Modified;
          }
          List<ct_HoaDon_ChiTiet> lstHoaDon_ChiTiet = await this._context.ct_HoaDon_ChiTiet.Where<ct_HoaDon_ChiTiet>((Expression<Func<ct_HoaDon_ChiTiet, bool>>) (e => e.LOC_ID == Input.LOC_ID && e.ID_HOADON == Input.ID)).ToListAsync<ct_HoaDon_ChiTiet>();
          if (lstHoaDon_ChiTiet != null)
          {
            foreach (ct_HoaDon_ChiTiet itm in lstHoaDon_ChiTiet)
              this._context.ct_HoaDon_ChiTiet.Remove(itm);
          }
          this._context.ct_HoaDon.Remove(Input);
          objOutput = (ct_PhieuXuat) null;
          lstHoaDon_ChiTiet = (List<ct_HoaDon_ChiTiet>) null;
        }
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        auditLog = (AuditLogController) null;
        transaction.Commit();
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) ""
        });
      }
    }
    catch (Exception ex)
    {
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  private bool InputExistsID(string LOC_ID, string ID)
  {
    return this._context.ct_HoaDon.Any<ct_HoaDon>((Expression<Func<ct_HoaDon, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }
}
