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
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using Microsoft.CodeAnalysis.Differencing;
using Newtonsoft.Json;
using DatabaseTHP.StoredProcedure.Parameter;
using NuGet.Packaging;

namespace MyApiNetCore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KPI_SaleController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public KPI_SaleController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
        }
        [HttpGet("{LOC_ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetProduct(string LOC_ID)
        {
            try
            {
                var lstValue = await _context.view_dm_KPI_KinhDoanh!.Where(e => e.LOC_ID == LOC_ID).OrderBy(e => e.MA).ToListAsync();
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

        // GET: api/Product
        [HttpGet("{LOC_ID}/{Type}/{KeyWhere}/{ValuesSearch}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetProduct(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
        {
            try
            {
                ValuesSearch = ValuesSearch.Replace("%2f", "/");
                var lstValue = await _context.view_dm_KPI_KinhDoanh!.Where(e => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).OrderBy(e => e.MA).ToListAsync();
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


        //GET: api/Product/5
        [HttpGet("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetProduct(string LOC_ID, string ID)
        {
            try
            {
                var Product = await _context.view_dm_KPI_KinhDoanh!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (Product == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                v_view_dm_KPI_KinhDoanh dm_KPI_KinhDoanh = new v_view_dm_KPI_KinhDoanh();
                dm_KPI_KinhDoanh.lstdm_KPI_KinhDoanh_YeuCau = new List<v_dm_KPI_KinhDoanh_YeuCau>();
                if (Product != null)
                {
                    dm_KPI_KinhDoanh = JsonConvert.DeserializeObject<v_view_dm_KPI_KinhDoanh>(JsonConvert.SerializeObject(Product)) ?? new v_view_dm_KPI_KinhDoanh();
                    var lstValue_yeuCau = await _context.view_dm_KPI_KinhDoanh_YeuCau!.Where(e => e.LOC_ID == LOC_ID && e.ID_KPI_KINHDOANH == Product.ID).ToListAsync();
                    dm_KPI_KinhDoanh.lstdm_KPI_KinhDoanh_YeuCau = JsonConvert.DeserializeObject<List<v_dm_KPI_KinhDoanh_YeuCau>>(JsonConvert.SerializeObject(lstValue_yeuCau));

                    var lstValue_NhanVien = await _context.view_dm_KPI_KinhDoanh_NhanVien!.Where(e => e.LOC_ID == LOC_ID && e.ID_KPI_KINHDOANH == Product.ID).ToListAsync();
                    dm_KPI_KinhDoanh.lstdm_KPI_KinhDoanh_NhanVien = JsonConvert.DeserializeObject<List<v_dm_KPI_KinhDoanh_NhanVien>>(JsonConvert.SerializeObject(lstValue_NhanVien));
                }



                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = dm_KPI_KinhDoanh
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

        // PUT: api/Product/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{LOC_ID}/{MA}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PutProduct(string LOC_ID, string MA, [FromBody] v_dm_KPI_KinhDoanh ChuongTrinhKhuyenMai)
        {
            try
            {
                if (ProductExists(ChuongTrinhKhuyenMai))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + ChuongTrinhKhuyenMai.LOC_ID + "-" + ChuongTrinhKhuyenMai.MA + " trong dữ liệu!",
                        Data = ""
                    });
                }

                if (LOC_ID != ChuongTrinhKhuyenMai.LOC_ID || ChuongTrinhKhuyenMai.MA != MA)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Dữ liệu khóa không giống nhau!",
                        Data = ""
                    });
                }
                if (!ProductExistsID(ChuongTrinhKhuyenMai.LOC_ID, ChuongTrinhKhuyenMai.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + ChuongTrinhKhuyenMai.LOC_ID + "-" + ChuongTrinhKhuyenMai.ID + " dữ liệu!",
                        Data = ""
                    });
                }

                using var transaction = _context.Database.BeginTransaction();
                {
                    var ChuongTrinhKhuyenMai_YeuCau = await _context.dm_KPI_KinhDoanh_YeuCau!.Where(e => e.LOC_ID == LOC_ID && e.ID_KPI_KINHDOANH == ChuongTrinhKhuyenMai.ID).ToListAsync();
                    foreach (var itm in ChuongTrinhKhuyenMai_YeuCau)
                    {
                        _context.dm_KPI_KinhDoanh_YeuCau!.Remove(itm);
                    }
                    foreach (var itm in ChuongTrinhKhuyenMai.lstdm_KPI_KinhDoanh_YeuCau)
                    {
                        itm.ID_KPI_KINHDOANH = ChuongTrinhKhuyenMai.ID;
                        _context.dm_KPI_KinhDoanh_YeuCau!.Add(itm);
                    }


                    var ChuongTrinhKhuyenMai_NhanVien = await _context.dm_KPI_KinhDoanh_NhanVien!.Where(e => e.LOC_ID == LOC_ID && e.ID_KPI_KINHDOANH == ChuongTrinhKhuyenMai.ID).ToListAsync();
                    foreach (var itm in ChuongTrinhKhuyenMai_NhanVien)
                    {
                        _context.dm_KPI_KinhDoanh_NhanVien!.Remove(itm);
                    }
                    foreach (var itm in ChuongTrinhKhuyenMai.lstdm_KPI_KinhDoanh_NhanVien)
                    {
                        itm.ID_KPI_KINHDOANH = ChuongTrinhKhuyenMai.ID;
                        _context.dm_KPI_KinhDoanh_NhanVien!.Add(itm);
                    }
                    _context.Entry(ChuongTrinhKhuyenMai).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
                transaction.Commit();
                var OKProduct = await _context.view_dm_KPI_KinhDoanh!.FirstOrDefaultAsync(e => e.LOC_ID == ChuongTrinhKhuyenMai.LOC_ID && e.ID == ChuongTrinhKhuyenMai.ID);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = OKProduct
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

        // POST: api/Product
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<v_dm_KPI_KinhDoanh>> PostProduct([FromBody] v_dm_KPI_KinhDoanh ChuongTrinhKhuyenMai)
        {
            try
            {
                if (ProductExistsMA(ChuongTrinhKhuyenMai.LOC_ID, ChuongTrinhKhuyenMai.MA))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + ChuongTrinhKhuyenMai.LOC_ID + "-" + ChuongTrinhKhuyenMai.MA + " trong dữ liệu!",
                        Data = ""
                    });
                }

                using var transaction = _context.Database.BeginTransaction();
                {
                  
                    foreach (var itm in ChuongTrinhKhuyenMai.lstdm_KPI_KinhDoanh_YeuCau)
                    {
                        itm.ID_KPI_KINHDOANH = ChuongTrinhKhuyenMai.ID;
                        _context.dm_KPI_KinhDoanh_YeuCau!.Add(itm);
                    }
                    foreach (var itm in ChuongTrinhKhuyenMai.lstdm_KPI_KinhDoanh_NhanVien)
                    {
                        itm.ID_KPI_KINHDOANH = ChuongTrinhKhuyenMai.ID;
                        _context.dm_KPI_KinhDoanh_NhanVien!.Add(itm);
                    }
                    _context.dm_KPI_KinhDoanh!.Add(ChuongTrinhKhuyenMai);
                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
                transaction.Commit();
                var OKProduct = await _context.view_dm_KPI_KinhDoanh!.FirstOrDefaultAsync(e => e.LOC_ID == ChuongTrinhKhuyenMai.LOC_ID && e.ID == ChuongTrinhKhuyenMai.ID);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = OKProduct
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

        // DELETE: api/Product/5
        [HttpDelete("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeleteProduct(string LOC_ID, string ID)
        {
            try
            {
                using var transaction = _context.Database.BeginTransaction();
                {
                    var ChuongTrinhKhuyenMai = await _context.dm_KPI_KinhDoanh!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                    if (ChuongTrinhKhuyenMai == null)
                    {
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                            Data = ""
                        });
                    }
                    ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
                    ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<dm_KPI_KinhDoanh>(ChuongTrinhKhuyenMai, ChuongTrinhKhuyenMai.ID, ChuongTrinhKhuyenMai.NAME);
                    if (!apiResponse.Success)
                    {
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = apiResponse.Message,
                            Data = ""
                        });
                    }
                    var ChuongTrinhKhuyenMai_YeuCau = await _context.dm_KPI_KinhDoanh_YeuCau!.Where(e => e.LOC_ID == LOC_ID && e.ID_KPI_KINHDOANH == ID).ToListAsync();
                    foreach (var itm in ChuongTrinhKhuyenMai_YeuCau)
                    {
                        _context.dm_KPI_KinhDoanh_YeuCau!.Remove(itm);
                    }
                    var ChuongTrinhKhuyenMai_NhanVien = await _context.dm_KPI_KinhDoanh_NhanVien!.Where(e => e.LOC_ID == LOC_ID && e.ID_KPI_KINHDOANH == ID).ToListAsync();
                    foreach (var itm in ChuongTrinhKhuyenMai_NhanVien)
                    {
                        _context.dm_KPI_KinhDoanh_NhanVien!.Remove(itm);
                    }
                    _context.dm_KPI_KinhDoanh!.Remove(ChuongTrinhKhuyenMai);
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

        private bool ProductExistsMA(string LOC_ID, string MA)
        {
            return _context.dm_KPI_KinhDoanh!.Any(e => e.LOC_ID == LOC_ID && e.MA == MA);
        }

        private bool ProductExistsID(string LOC_ID, string ID)
        {
            return _context.dm_KPI_KinhDoanh!.Any(e => e.LOC_ID == LOC_ID && e.ID == ID);
        }

        private bool ProductExists(dm_KPI_KinhDoanh Product)
        {
            return _context.dm_KPI_KinhDoanh!.Any(e => e.LOC_ID == Product.LOC_ID && e.MA == Product.MA && e.ID != Product.ID);
        }

        [HttpPut]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PutProduct([FromBody] SP_Parameter SP_Parameter)
        {
            try 
            {
                var lst_ChiTiet = new List<DanhSachPhieuDatHang_ChiTiet_KPI>();
                var lst_ChiTiet_TraHang = new List<DanhSachPhieuTraHang_ChiTiet_KPI>();

                List<v_Tinh_KPI_KinhDoanh> lstTinh_KPI_KinhDoanh = new List<v_Tinh_KPI_KinhDoanh>();
                var lstValue = await _context.view_dm_KPI_KinhDoanh!.Where(e => e.LOC_ID == SP_Parameter.LOC_ID && e.TUNGAY <= SP_Parameter.TUNGAY && e.DENNGAY >= SP_Parameter.DENNGAY && e.ISACTIVE).OrderBy(e => e.CAPDO).ToListAsync();
                if(lstValue != null)
                {
                    ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);

                    var actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuTraHang_ChiTiet_KPI(SP_Parameter);
                    var okResult = actionResult as OkObjectResult;
                    if (okResult != null)
                    {
                        var ApiResponse = okResult.Value as ApiResponse;
                        if (ApiResponse != null)
                        {
                            if (ApiResponse.Data != null)
                            {
                                lst_ChiTiet_TraHang = ApiResponse.Data as List<DanhSachPhieuTraHang_ChiTiet_KPI>;
                            }
                        }
                    }

                    actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuDatHang_ChiTiet_KPI(SP_Parameter);
                    okResult = actionResult as OkObjectResult;
                    if (okResult != null)
                    {
                        var ApiResponse = okResult.Value as ApiResponse;
                        if (ApiResponse != null)
                        {
                            if (ApiResponse.Data != null)
                            {
                                lst_ChiTiet = ApiResponse.Data as List<DanhSachPhieuDatHang_ChiTiet_KPI>;
                                if (lst_ChiTiet != null && lst_ChiTiet_TraHang != null)
                                {
                                    foreach(var itm in lstValue)
                                    {
                                        var lstValue_NhanVien = await _context.view_dm_KPI_KinhDoanh_NhanVien!.Where(e => e.ID_KPI_KINHDOANH == itm.ID).ToListAsync();
                                        var lstValue_YeuCau = await _context.view_dm_KPI_KinhDoanh_YeuCau!.Where(e => e.ID_KPI_KINHDOANH == itm.ID).ToListAsync();
                                        foreach (var NhanVien in lstValue_NhanVien)
                                        {
                                            #region Nhân viên
                                            if (NhanVien.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham && ((string.IsNullOrEmpty(SP_Parameter.ID_NHANVIEN) || NhanVien.ID_NHANVIEN == SP_Parameter.ID_NHANVIEN)))
                                            {
                                                if(!string.IsNullOrEmpty(SP_Parameter.ID_NHOMQUYEN))
                                                {
                                                    var objNhanVien = await _context.view_dm_NhanVien!.Where(e => e.ID == NhanVien.ID_NHANVIEN).FirstOrDefaultAsync();
                                                    if (objNhanVien != null && objNhanVien.ID_NHOMQUYEN != SP_Parameter.ID_NHOMQUYEN)
                                                        continue;
                                                }
                                                var NhanVienKinhDoanh = lstTinh_KPI_KinhDoanh.FirstOrDefault(e => e.ID_NHANVIEN == NhanVien.ID_NHANVIEN);
                                                if(NhanVienKinhDoanh == null)
                                                {
                                                    NhanVienKinhDoanh = new v_Tinh_KPI_KinhDoanh();
                                                    NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet = new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
                                                    NhanVienKinhDoanh.ID_NHANVIEN = NhanVien.ID_NHANVIEN;
                                                    NhanVienKinhDoanh.NAME_NHANVIEN = NhanVien.MA + " - " + NhanVien.NAME;
                                                    var objNhanVien = await _context.dm_NhanVien!.FirstOrDefaultAsync(e => e.ID == NhanVien.ID_NHANVIEN);
                                                    //NhanVienKinhDoanh.ID_NHANVIEN = objNhanVien != null ? objNhanVien.NAME : "";
                                                    NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange(Get_ChiTiet(itm, NhanVien, lstValue_YeuCau, lst_ChiTiet, NhanVienKinhDoanh, lst_ChiTiet_TraHang));
                                                    NhanVienKinhDoanh.SOTIEN_KPI = NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Sum(e => e.SOTIEN_KPI);
                                                    if (NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet != null && NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Count > 0)
                                                        lstTinh_KPI_KinhDoanh.Add(NhanVienKinhDoanh);
                                                }
                                                else
                                                {
                                                    if (NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.FirstOrDefault(e => e.ID_KPI_KINHDOANH == itm.ID) == null)
                                                        NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange(Get_ChiTiet(itm, NhanVien, lstValue_YeuCau, lst_ChiTiet, NhanVienKinhDoanh, lst_ChiTiet_TraHang));
                                                    NhanVienKinhDoanh.SOTIEN_KPI = NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Sum(e => e.SOTIEN_KPI);
                                                }
                                                
                                            }
                                            #endregion

                                            #region Theo nhóm quyền
                                            if (NhanVien.HINHTHUC == (int)API.HinhThucKhuyenMai.NhomSanPham && ((string.IsNullOrEmpty(SP_Parameter.ID_NHOMQUYEN) || NhanVien.ID_NHANVIEN == SP_Parameter.ID_NHOMQUYEN)))
                                            {
                                                var lstNhanVien = await _context.view_dm_NhanVien!.Where(e => e.ID_NHOMQUYEN == NhanVien.ID_NHANVIEN && e.ISACTIVE).ToListAsync();
                                                if(lstNhanVien != null)
                                                {
                                                    foreach ( var item in lstNhanVien)
                                                    {
                                                        if (item != null)
                                                        {
                                                            if (!string.IsNullOrEmpty(SP_Parameter.ID_NHOMQUYEN))
                                                            {
                                                                if (item != null && item.ID_NHOMQUYEN != SP_Parameter.ID_NHOMQUYEN)
                                                                    continue;
                                                            }
                                                            var NhanVienKinhDoanh = lstTinh_KPI_KinhDoanh.FirstOrDefault(e => e.ID_NHANVIEN == item.ID);
                                                            if (NhanVienKinhDoanh == null)
                                                            {
                                                                NhanVienKinhDoanh = new v_Tinh_KPI_KinhDoanh();
                                                                NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet = new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
                                                                NhanVienKinhDoanh.ID_NHANVIEN = item.ID;
                                                                NhanVienKinhDoanh.NAME_NHANVIEN = item.MA + " - " + item.NAME;
                                                                var objNhanVien = await _context.dm_NhanVien!.FirstOrDefaultAsync(e => e.ID == item.ID);
                                                                //NhanVienKinhDoanh.ID_NHANVIEN = objNhanVien != null ? objNhanVien.NAME : "";
                                                                NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange(Get_ChiTiet(itm, NhanVien, lstValue_YeuCau, lst_ChiTiet, NhanVienKinhDoanh, lst_ChiTiet_TraHang));
                                                                NhanVienKinhDoanh.SOTIEN_KPI = NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Sum(e => e.SOTIEN_KPI);
                                                                if (NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet != null && NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Count > 0)
                                                                    lstTinh_KPI_KinhDoanh.Add(NhanVienKinhDoanh);
                                                            }
                                                            else
                                                            {
                                                                if (NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.FirstOrDefault(e => e.ID_KPI_KINHDOANH == itm.ID) == null)
                                                                    NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange(Get_ChiTiet(itm, NhanVien, lstValue_YeuCau, lst_ChiTiet, NhanVienKinhDoanh, lst_ChiTiet_TraHang));
                                                            }
                                                        }
                                                    } 
                                                }    
                                            }
                                            #endregion


                                        }
                                    }    
                                }
                            }
                        }
                    }
                }    
                
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = lstTinh_KPI_KinhDoanh
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

        private List<v_Tinh_KPI_KinhDoanh_ChiTiet> Get_ChiTiet(view_dm_KPI_KinhDoanh KPI_KinhDoanh, view_dm_KPI_KinhDoanh_NhanVien KPI_KinhDoanh_NhanVien, List<view_dm_KPI_KinhDoanh_YeuCau> lstValue_YeuCau, List<DanhSachPhieuDatHang_ChiTiet_KPI> lstChiTietDatHang, v_Tinh_KPI_KinhDoanh NhanVien_KinhDoanh, List<DanhSachPhieuTraHang_ChiTiet_KPI> lstChiTietTraHang)
        {
            try
            {
                List<v_Tinh_KPI_KinhDoanh_ChiTiet> lstTinh_KPI_KinhDoanh_ChiTiet = new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
                bool bolChietKhauTong = true;
                double TongTien_Tong = 0;
                double TongTien_TongDatKM = 0;
                double TongTien_Tong_TraHang = 0;
                foreach (var itm in lstValue_YeuCau)
                {
                    var TongTien = lstChiTietDatHang.Where(e => e.bolDaTinhKpi == false && (itm.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_HANGHOA == itm.ID_HANGHOA : e.ID_NHOMHANGHOA == itm.ID_HANGHOA)
                    && (KPI_KinhDoanh_NhanVien.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN : e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)).Sum(e => e.TONGCONG);
                    var TongTien_TraHang = lstChiTietTraHang.Where(e => e.bolDaTinhKpi == false && (itm.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_HANGHOA == itm.ID_HANGHOA : e.ID_NHOMHANGHOA == itm.ID_HANGHOA)
                    && (KPI_KinhDoanh_NhanVien.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN : e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)).Sum(e => e.TONGCONG);
                    var TongSoLuong = lstChiTietDatHang.Where(e => e.bolDaTinhKpi == false && (itm.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_HANGHOA == itm.ID_HANGHOA && e.ID_DVT == itm.ID_DVT : e.ID_NHOMHANGHOA == itm.ID_HANGHOA && e.ID_DVT == itm.ID_DVT)
                    && (KPI_KinhDoanh_NhanVien.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN : e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)).Sum(e => e.TONGSOLUONG / e.TYLE_QD);
                    var TongSoLuong_ChuaQuyDoi = Convert.ToInt32(lstChiTietDatHang.Where(e => e.bolDaTinhKpi == false && (itm.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_HANGHOA == itm.ID_HANGHOA && e.ID_DVT != itm.ID_DVT : e.ID_NHOMHANGHOA == itm.ID_HANGHOA && e.ID_DVT != itm.ID_DVT)
                    && (KPI_KinhDoanh_NhanVien.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN : e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)).Sum(e => e.TONGSOLUONG / (e.TYLE_QD_HH == 0 ? 1 : e.TYLE_QD_HH)));
                    TongSoLuong += TongSoLuong_ChuaQuyDoi;

                    var TongSoLuong_TraHang = lstChiTietTraHang.Where(e => e.bolDaTinhKpi == false && (itm.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_HANGHOA == itm.ID_HANGHOA && e.ID_DVT == itm.ID_DVT : e.ID_NHOMHANGHOA == itm.ID_HANGHOA && e.ID_DVT == itm.ID_DVT)
                    && (KPI_KinhDoanh_NhanVien.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN : e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)).Sum(e => e.TONGSOLUONG / e.TYLE_QD);
                    var TongSoLuong_TraHang_ChuaQuyDoi = Convert.ToInt32(lstChiTietTraHang.Where(e => e.bolDaTinhKpi == false && (itm.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_HANGHOA == itm.ID_HANGHOA && e.ID_DVT != itm.ID_DVT : e.ID_NHOMHANGHOA == itm.ID_HANGHOA && e.ID_DVT != itm.ID_DVT)
                    && (KPI_KinhDoanh_NhanVien.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN : e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)).Sum(e => e.TONGSOLUONG / (e.TYLE_QD_HH == 0 ? 1 : e.TYLE_QD_HH)));
                    TongSoLuong_TraHang += TongSoLuong_TraHang_ChuaQuyDoi;

                    if ((itm.SOTIEN > 0 || itm.SOLUONG > 0 || itm.CHIETKHAU > 0 || itm.TIENGIAM > 0) && (itm.SOTIEN <= (TongTien - TongTien_TraHang) && itm.SOLUONG <= (TongSoLuong - TongSoLuong_TraHang)))
                    {
                        //var HangHoa = _context.view_dm_HangHoa!.Where(e => e.ID == itm.ID_HANGHOA).FirstOrDefault();
                        var DVT = _context.dm_DonViTinh!.Where(e => e.ID == itm.ID_DVT).FirstOrDefault();
                        v_Tinh_KPI_KinhDoanh_ChiTiet newv_Tinh_KPI_KinhDoanh_ChiTiet = new v_Tinh_KPI_KinhDoanh_ChiTiet();
                        newv_Tinh_KPI_KinhDoanh_ChiTiet = new v_Tinh_KPI_KinhDoanh_ChiTiet();
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.HINHTHUC = itm.HINHTHUC;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_HINHTHUC = itm.NAME_HINHTHUC;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.ID_KPI_KINHDOANH = KPI_KinhDoanh.ID;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_KPI_KINHDOANH = KPI_KinhDoanh.NAME;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.ID_HANGHOA = itm.ID_HANGHOA;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_HANGHOA = itm.NAME;
                        if ((itm.CHIETKHAU > 0 || itm.TIENGIAM > 0))
                        {
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN = TongTien;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN_TRAHANG = TongTien_TraHang;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN_KPI = itm.SOTIEN;
                        }

                        int PhanNguyen = 0;
                        double TYLE_QD = 1;
                        //if (HangHoa != null)
                        //    TYLE_QD = HangHoa.TYLE_QD;

                        PhanNguyen = Convert.ToInt32(TongSoLuong) / Convert.ToInt32(TYLE_QD);

                        newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_DVT = itm.NAME_DVT;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_DVT_QD = DVT != null ? DVT.NAME : "";
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.TYLE_QD = TYLE_QD;
                        if ((itm.HINHTHUC_TINHKPI == ((int)API.HinhThucTinhKPI.ThuongTheoSoLuong1) && (itm.CHIETKHAU > 0 || itm.TIENGIAM > 0)))
                        {
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG = Convert.ToInt32(TongSoLuong) / Convert.ToInt32(TYLE_QD);
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG_TRAHANG = Convert.ToInt32(TongSoLuong_TraHang) / Convert.ToInt32(TYLE_QD); ;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG_KPI = itm.SOLUONG.ToString("N0").Replace(',', '.') + " " + itm.NAME_DVT;
                        }
                        if ((itm.HINHTHUC_TINHKPI == ((int)API.HinhThucTinhKPI.ThuongKhiDatMoc) && (itm.CHIETKHAU > 0 || itm.TIENGIAM > 0)))
                        {
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG = Convert.ToInt32(TongSoLuong) / Convert.ToInt32(TYLE_QD);
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG_TRAHANG = Convert.ToInt32(TongSoLuong_TraHang) / Convert.ToInt32(TYLE_QD); ;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG_KPI = itm.SOLUONG.ToString("N0").Replace(',', '.') + " " + itm.NAME_DVT;
                        }
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU = itm.CHIETKHAU;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.TIENTHUONG = itm.TIENGIAM; 
                        PhanNguyen = Convert.ToInt32(TongSoLuong - TongSoLuong_TraHang) / Convert.ToInt32(TYLE_QD);
                        if (itm.HINHTHUC_TINHKPI == ((int)API.HinhThucTinhKPI.ThuongTheoSoLuong1))
                        {
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.SOTIEN_KPI = newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU > 0 ? (newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU * (TongTien - TongTien_TraHang) / 100) : itm.TIENGIAM * (PhanNguyen - itm.SOLUONG);
                        }
                        else if (itm.HINHTHUC_TINHKPI == ((int)API.HinhThucTinhKPI.ThuongKhiDatMoc))
                        {
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.SOTIEN_KPI = newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU > 0 ? (newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU * (TongTien - TongTien_TraHang) / 100) : itm.TIENGIAM * (PhanNguyen);
                        }
                        else if (itm.HINHTHUC_TINHKPI == ((int)API.HinhThucTinhKPI.ThuongTuTroLen))
                        {
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.SOTIEN_KPI = newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU > 0 ? (newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU * (TongTien - TongTien_TraHang - itm.SOTIEN) / 100) : itm.TIENGIAM;
                        }
                        else
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.SOTIEN_KPI = newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU > 0 ? newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU * (TongTien - TongTien_TraHang) / 100 : itm.TIENGIAM;
                        lstTinh_KPI_KinhDoanh_ChiTiet.Add(newv_Tinh_KPI_KinhDoanh_ChiTiet);

                        foreach (var Dathang in lstChiTietDatHang.Where(e => e.bolDaTinhKpi == false && (itm.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_HANGHOA == itm.ID_HANGHOA : e.ID_NHOMHANGHOA == itm.ID_HANGHOA)
                         && (KPI_KinhDoanh_NhanVien.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN : e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)))
                        {
                            //Dathang.bolDaTinhKpi = true;
                        }

                        foreach (var Dathang in lstChiTietTraHang.Where(e => e.bolDaTinhKpi == false && (itm.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_HANGHOA == itm.ID_HANGHOA : e.ID_NHOMHANGHOA == itm.ID_HANGHOA)
                         && (KPI_KinhDoanh_NhanVien.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN : e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)))
                        {
                            //Dathang.bolDaTinhKpi = true;
                        }

                        foreach (var Dathang in lstChiTietDatHang.Where(e => e.bolDaTinhKpi == false && (itm.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_HANGHOA == itm.ID_HANGHOA && e.ID_DVT == itm.ID_DVT : e.ID_NHOMHANGHOA == itm.ID_HANGHOA && e.ID_DVT == itm.ID_DVT)
                         && (KPI_KinhDoanh_NhanVien.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN : e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)))
                        {
                            //Dathang.bolDaTinhKpi = true;
                        }

                        foreach (var Dathang in lstChiTietTraHang.Where(e => e.bolDaTinhKpi == false && (itm.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_HANGHOA == itm.ID_HANGHOA && e.ID_DVT == itm.ID_DVT : e.ID_NHOMHANGHOA == itm.ID_HANGHOA && e.ID_DVT == itm.ID_DVT)
                        && (KPI_KinhDoanh_NhanVien.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN : e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)))
                        {
                            //Dathang.bolDaTinhKpi = true;
                        }
                        TongTien_Tong += (TongTien - TongTien_TraHang);
                    }
                    else
                        bolChietKhauTong = false;

                    TongTien_TongDatKM += TongTien;
                    TongTien_Tong_TraHang += TongTien_TraHang;
                }
                var TongSoLuong_TongDatKM = lstChiTietDatHang.Where(e => e.ID_DVT == KPI_KinhDoanh.ID_DVT_DATKM).Sum(e => e.TONGCONG);
                var TongSoLuong_TongDatKM_TraHang = lstChiTietTraHang.Where(e => e.ID_DVT == KPI_KinhDoanh.ID_DVT_DATKM).Sum(e => e.TONGCONG);
                if (KPI_KinhDoanh.IS_YEUCAUCHITIET)
                {
                    if (bolChietKhauTong)
                    {
                        if (KPI_KinhDoanh.CHIETKHAU > 0 || KPI_KinhDoanh.TIENGIAM > 0)
                        {
                            v_Tinh_KPI_KinhDoanh_ChiTiet newv_Tinh_KPI_KinhDoanh_ChiTiet = new v_Tinh_KPI_KinhDoanh_ChiTiet();
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.HINHTHUC = -1;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_HINHTHUC = "Tổng";
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.ID_KPI_KINHDOANH = KPI_KinhDoanh.ID;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_KPI_KINHDOANH = KPI_KinhDoanh.NAME;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_HANGHOA = newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_HINHTHUC;

                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN = TongTien_TongDatKM;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN_TRAHANG = TongTien_Tong_TraHang;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN_KPI = KPI_KinhDoanh.TONGTIEN_DATKM;
                            if (!string.IsNullOrEmpty(KPI_KinhDoanh.NAME_DVT))
                            {
                                newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG = TongSoLuong_TongDatKM;
                                newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_DVT = KPI_KinhDoanh.NAME_DVT;
                                newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG_TRAHANG = TongSoLuong_TongDatKM_TraHang;
                                newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG_KPI = KPI_KinhDoanh.SOLUONG_DATKM + " " + KPI_KinhDoanh.NAME_DVT;
                            }

                            newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU = KPI_KinhDoanh.CHIETKHAU;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TIENTHUONG = KPI_KinhDoanh.TIENGIAM;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.SOTIEN_KPI = newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU > 0 ? newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU * TongTien_Tong / 100 : KPI_KinhDoanh.TIENGIAM;
                            lstTinh_KPI_KinhDoanh_ChiTiet.Add(newv_Tinh_KPI_KinhDoanh_ChiTiet);
                        }
                    }
                }
                else
                {

                    if ((KPI_KinhDoanh.CHIETKHAU > 0 || KPI_KinhDoanh.TIENGIAM > 0) && KPI_KinhDoanh.SOLUONG_DATKM <= TongSoLuong_TongDatKM && KPI_KinhDoanh.TONGTIEN_DATKM <= TongTien_TongDatKM)
                    {

                        v_Tinh_KPI_KinhDoanh_ChiTiet newv_Tinh_KPI_KinhDoanh_ChiTiet = new v_Tinh_KPI_KinhDoanh_ChiTiet();
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.HINHTHUC = -1;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_HINHTHUC = "Tổng";
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.ID_KPI_KINHDOANH = KPI_KinhDoanh.ID;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_KPI_KINHDOANH = KPI_KinhDoanh.NAME;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_HANGHOA = newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_HINHTHUC;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN = TongTien_TongDatKM;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN_TRAHANG = TongTien_Tong_TraHang;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN_KPI = KPI_KinhDoanh.TONGTIEN_DATKM;
                        if (!string.IsNullOrEmpty(KPI_KinhDoanh.NAME_DVT))
                        {
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG = TongSoLuong_TongDatKM;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_DVT = KPI_KinhDoanh.NAME_DVT;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG_TRAHANG = TongSoLuong_TongDatKM_TraHang;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG_KPI = KPI_KinhDoanh.SOLUONG_DATKM + " " + KPI_KinhDoanh.NAME_DVT;
                        }
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU = KPI_KinhDoanh.CHIETKHAU;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.TIENTHUONG = KPI_KinhDoanh.TIENGIAM;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.SOTIEN_KPI = newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU > 0 ? newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU * (newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN - newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN_TRAHANG) / 100 : KPI_KinhDoanh.TIENGIAM;
                        lstTinh_KPI_KinhDoanh_ChiTiet.Add(newv_Tinh_KPI_KinhDoanh_ChiTiet);
                    }
                    else
                    {
                        //if (!(KPI_KinhDoanh.SOLUONG_DATKM <= TongSoLuong_TongDatKM) || !(KPI_KinhDoanh.TONGTIEN_DATKM <= TongTien_TongDatKM))
                        //    lstTinh_KPI_KinhDoanh_ChiTiet.Clear();
                    }

                }
                return lstTinh_KPI_KinhDoanh_ChiTiet;
            } catch 
            {
                return new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
            }
           
        }

        // POST: api/Deposit
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostCreateKPI_Sale")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PostKPI_Sale([FromBody] List<Deposit> lstDeposit)
        {
            try
            {
                using var transaction = _context.Database.BeginTransaction();
                {
                    foreach (var item in lstDeposit)
                    {
                        var KPI_KinhDoanh = _context.dm_KPI_KinhDoanh!.AsNoTracking().Where(e => e.LOC_ID == item.LOC_ID && e.ID == item.ID).FirstOrDefault();
                        if(KPI_KinhDoanh != null)
                        {
                            KPI_KinhDoanh.ID_NGUOITAO = item.ID_NGUOITAO;
                            KPI_KinhDoanh.THOIGIANTHEM = DateTime.Now;
                            KPI_KinhDoanh.ID_NGUOISUA = null;
                            KPI_KinhDoanh.THOIGIANSUA = null;
                            KPI_KinhDoanh.ID = Guid.NewGuid().ToString();
                            KPI_KinhDoanh.TUNGAY = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                            KPI_KinhDoanh.DENNGAY = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                            KPI_KinhDoanh.MA = KPI_KinhDoanh.MA.Replace("(T" + DateTime.Now.AddMonths(-1).Month.ToString("00") + ")", "") + "(T" + DateTime.Now.Month.ToString("00") + ")";
                            var KPI_KinhDoanhCheck = _context.dm_KPI_KinhDoanh!.AsNoTracking().Where(e => e.LOC_ID == item.LOC_ID && e.MA == KPI_KinhDoanh.MA).FirstOrDefault();
                            if (KPI_KinhDoanhCheck != null)
                                KPI_KinhDoanh.MA += "_Copy";
                            KPI_KinhDoanh.NAME = KPI_KinhDoanh.NAME.Replace("(T" + DateTime.Now.AddMonths(-1).Month.ToString("00") + ")", "") + "(T" + DateTime.Now.Month.ToString("00") + ")";
                            var lstdm_KPI_KinhDoanh_NhanVien = _context.dm_KPI_KinhDoanh_NhanVien!.AsNoTracking().Where(e => e.LOC_ID == item.LOC_ID && e.ID_KPI_KINHDOANH == item.ID).ToList();
                            var lstdm_KPI_KinhDoanh_YeuCau = _context.dm_KPI_KinhDoanh_YeuCau!.AsNoTracking().Where(e => e.LOC_ID == item.LOC_ID && e.ID_KPI_KINHDOANH == item.ID).ToList();
                            if(lstdm_KPI_KinhDoanh_NhanVien != null)
                            {
                                foreach(var itm in lstdm_KPI_KinhDoanh_NhanVien)
                                {
                                    itm.ID = Guid.NewGuid().ToString();
                                    itm.ID_KPI_KINHDOANH = KPI_KinhDoanh.ID;
                                    _context.dm_KPI_KinhDoanh_NhanVien!.Add(itm);
                                }
                            }
                            if (lstdm_KPI_KinhDoanh_YeuCau != null)
                            {
                                foreach (var itm in lstdm_KPI_KinhDoanh_YeuCau)
                                {
                                    itm.ID = Guid.NewGuid().ToString();
                                    itm.ID_KPI_KINHDOANH = KPI_KinhDoanh.ID;
                                    _context.dm_KPI_KinhDoanh_YeuCau!.Add(itm);
                                }
                            }
                            _context.dm_KPI_KinhDoanh!.Add(KPI_KinhDoanh);
                        }    
                    }
                    AuditLogController auditLog = new AuditLogController(_context, _configuration); auditLog.InserAuditLog(); await _context.SaveChangesAsync();
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


        #region Lấy danh sách tạm KPI
        [HttpPut("PutKPI_Tam")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PutKPI_Tam([FromBody] SP_Parameter SP_Parameter)
        {
            try
            {
                var lst_ChiTiet = new List<DanhSachPhieuDatHang_ChiTiet_KPI>();
                var lst_ChiTiet_TraHang = new List<DanhSachPhieuTraHang_ChiTiet_KPI>();

                List<v_Tinh_KPI_KinhDoanh> lstTinh_KPI_KinhDoanh = new List<v_Tinh_KPI_KinhDoanh>();
                var lstValue = await _context.view_dm_KPI_KinhDoanh!.Where(e => e.LOC_ID == SP_Parameter.LOC_ID && e.TUNGAY <= SP_Parameter.TUNGAY && e.DENNGAY >= SP_Parameter.DENNGAY && e.ISACTIVE).OrderBy(e => e.CAPDO).ToListAsync();
                if (lstValue != null)
                {
                    ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);

                    var actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuTraHang_ChiTiet_KPI(SP_Parameter);
                    var okResult = actionResult as OkObjectResult;
                    if (okResult != null)
                    {
                        var ApiResponse = okResult.Value as ApiResponse;
                        if (ApiResponse != null)
                        {
                            if (ApiResponse.Data != null)
                            {
                                lst_ChiTiet_TraHang = ApiResponse.Data as List<DanhSachPhieuTraHang_ChiTiet_KPI>;
                            }
                        }
                    }

                    actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuDatHang_ChiTiet_KPI(SP_Parameter);
                    okResult = actionResult as OkObjectResult;
                    if (okResult != null)
                    {
                        var ApiResponse = okResult.Value as ApiResponse;
                        if (ApiResponse != null)
                        {
                            if (ApiResponse.Data != null)
                            {
                                lst_ChiTiet = ApiResponse.Data as List<DanhSachPhieuDatHang_ChiTiet_KPI>;
                                if (lst_ChiTiet != null && lst_ChiTiet_TraHang != null)
                                {
                                    foreach (var itm in lstValue)
                                    {
                                        var lstValue_NhanVien = await _context.view_dm_KPI_KinhDoanh_NhanVien!.Where(e => e.ID_KPI_KINHDOANH == itm.ID).ToListAsync();
                                        var lstValue_YeuCau = await _context.view_dm_KPI_KinhDoanh_YeuCau!.Where(e => e.ID_KPI_KINHDOANH == itm.ID).ToListAsync();
                                        foreach (var NhanVien in lstValue_NhanVien)
                                        {
                                            #region Nhân viên
                                            if (NhanVien.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham && ((string.IsNullOrEmpty(SP_Parameter.ID_NHANVIEN) || NhanVien.ID_NHANVIEN == SP_Parameter.ID_NHANVIEN)))
                                            {
                                                if (!string.IsNullOrEmpty(SP_Parameter.ID_NHOMQUYEN))
                                                {
                                                    var objNhanVien = await _context.view_dm_NhanVien!.Where(e => e.ID == NhanVien.ID_NHANVIEN).FirstOrDefaultAsync();
                                                    if (objNhanVien != null && objNhanVien.ID_NHOMQUYEN != SP_Parameter.ID_NHOMQUYEN)
                                                        continue;
                                                }
                                                var NhanVienKinhDoanh = lstTinh_KPI_KinhDoanh.FirstOrDefault(e => e.ID_NHANVIEN == NhanVien.ID_NHANVIEN);
                                                if (NhanVienKinhDoanh == null)
                                                {
                                                    NhanVienKinhDoanh = new v_Tinh_KPI_KinhDoanh();
                                                    NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet = new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
                                                    NhanVienKinhDoanh.ID_NHANVIEN = NhanVien.ID_NHANVIEN;
                                                    NhanVienKinhDoanh.NAME_NHANVIEN = NhanVien.MA + " - " + NhanVien.NAME;
                                                    var objNhanVien = await _context.dm_NhanVien!.FirstOrDefaultAsync(e => e.ID == NhanVien.ID_NHANVIEN);
                                                    //NhanVienKinhDoanh.ID_NHANVIEN = objNhanVien != null ? objNhanVien.NAME : "";
                                                    NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange(Get_ChiTietTam(itm, NhanVien, lstValue_YeuCau, lst_ChiTiet, NhanVienKinhDoanh, lst_ChiTiet_TraHang));
                                                    NhanVienKinhDoanh.SOTIEN_KPI = NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Sum(e => e.SOTIEN_KPI);
                                                    if (NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet != null && NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Count > 0)
                                                        lstTinh_KPI_KinhDoanh.Add(NhanVienKinhDoanh);
                                                }
                                                else
                                                {
                                                    if (NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.FirstOrDefault(e => e.ID_KPI_KINHDOANH == itm.ID) == null)
                                                        NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange(Get_ChiTietTam(itm, NhanVien, lstValue_YeuCau, lst_ChiTiet, NhanVienKinhDoanh, lst_ChiTiet_TraHang));
                                                    NhanVienKinhDoanh.SOTIEN_KPI = NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Sum(e => e.SOTIEN_KPI);
                                                }

                                            }
                                            #endregion

                                            #region Theo nhóm quyền
                                            if (NhanVien.HINHTHUC == (int)API.HinhThucKhuyenMai.NhomSanPham && ((string.IsNullOrEmpty(SP_Parameter.ID_NHOMQUYEN) || NhanVien.ID_NHANVIEN == SP_Parameter.ID_NHOMQUYEN)))
                                            {
                                                var lstNhanVien = await _context.view_dm_NhanVien!.Where(e => e.ID_NHOMQUYEN == NhanVien.ID_NHANVIEN && e.ISACTIVE).ToListAsync();
                                                if (lstNhanVien != null)
                                                {
                                                    foreach (var item in lstNhanVien)
                                                    {
                                                        if (item != null)
                                                        {
                                                            if (!string.IsNullOrEmpty(SP_Parameter.ID_NHOMQUYEN))
                                                            {
                                                                if (item != null && item.ID_NHOMQUYEN != SP_Parameter.ID_NHOMQUYEN)
                                                                    continue;
                                                            }
                                                            var NhanVienKinhDoanh = lstTinh_KPI_KinhDoanh.FirstOrDefault(e => e.ID_NHANVIEN == item.ID);
                                                            if (NhanVienKinhDoanh == null)
                                                            {
                                                                NhanVienKinhDoanh = new v_Tinh_KPI_KinhDoanh();
                                                                NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet = new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
                                                                NhanVienKinhDoanh.ID_NHANVIEN = item.ID;
                                                                NhanVienKinhDoanh.NAME_NHANVIEN = item.MA + " - " + item.NAME;
                                                                var objNhanVien = await _context.dm_NhanVien!.FirstOrDefaultAsync(e => e.ID == item.ID);
                                                                //NhanVienKinhDoanh.ID_NHANVIEN = objNhanVien != null ? objNhanVien.NAME : "";
                                                                NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange(Get_ChiTietTam(itm, NhanVien, lstValue_YeuCau, lst_ChiTiet, NhanVienKinhDoanh, lst_ChiTiet_TraHang));
                                                                NhanVienKinhDoanh.SOTIEN_KPI = NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Sum(e => e.SOTIEN_KPI);
                                                                if (NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet != null && NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.Count > 0)
                                                                    lstTinh_KPI_KinhDoanh.Add(NhanVienKinhDoanh);
                                                            }
                                                            else
                                                            {
                                                                if (NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.FirstOrDefault(e => e.ID_KPI_KINHDOANH == itm.ID) == null)
                                                                    NhanVienKinhDoanh.lstTinh_KPI_KinhDoanh_ChiTiet.AddRange(Get_ChiTietTam(itm, NhanVien, lstValue_YeuCau, lst_ChiTiet, NhanVienKinhDoanh, lst_ChiTiet_TraHang));
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            #endregion


                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = lstTinh_KPI_KinhDoanh
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

        private List<v_Tinh_KPI_KinhDoanh_ChiTiet> Get_ChiTietTam(view_dm_KPI_KinhDoanh KPI_KinhDoanh, view_dm_KPI_KinhDoanh_NhanVien KPI_KinhDoanh_NhanVien, List<view_dm_KPI_KinhDoanh_YeuCau> lstValue_YeuCau, List<DanhSachPhieuDatHang_ChiTiet_KPI> lstChiTietDatHang, v_Tinh_KPI_KinhDoanh NhanVien_KinhDoanh, List<DanhSachPhieuTraHang_ChiTiet_KPI> lstChiTietTraHang)
        {
            try
            {
                List<v_Tinh_KPI_KinhDoanh_ChiTiet> lstTinh_KPI_KinhDoanh_ChiTiet = new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
                bool bolChietKhauTong = true;
                double TongTien_Tong = 0;
                double TongTien_TongDatKM = 0;
                double TongTien_Tong_TraHang = 0;
                foreach (var itm in lstValue_YeuCau)
                {
                    var TongTien = lstChiTietDatHang.Where(e => e.bolDaTinhKpi == false && (itm.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_HANGHOA == itm.ID_HANGHOA : e.ID_NHOMHANGHOA == itm.ID_HANGHOA)
                    && (KPI_KinhDoanh_NhanVien.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN : e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)).Sum(e => e.TONGCONG);
                    var TongTien_TraHang = lstChiTietTraHang.Where(e => e.bolDaTinhKpi == false && (itm.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_HANGHOA == itm.ID_HANGHOA : e.ID_NHOMHANGHOA == itm.ID_HANGHOA)
                    && (KPI_KinhDoanh_NhanVien.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN : e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)).Sum(e => e.TONGCONG);
                    var TongSoLuong = lstChiTietDatHang.Where(e => e.bolDaTinhKpi == false && (itm.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_HANGHOA == itm.ID_HANGHOA && e.ID_DVT == itm.ID_DVT : e.ID_NHOMHANGHOA == itm.ID_HANGHOA && e.ID_DVT == itm.ID_DVT)
                    && (KPI_KinhDoanh_NhanVien.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN : e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)).Sum(e => e.TONGSOLUONG / e.TYLE_QD);
                    var TongSoLuong_ChuaQuyDoi = Convert.ToInt32(lstChiTietDatHang.Where(e => e.bolDaTinhKpi == false && (itm.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_HANGHOA == itm.ID_HANGHOA && e.ID_DVT != itm.ID_DVT : e.ID_NHOMHANGHOA == itm.ID_HANGHOA && e.ID_DVT != itm.ID_DVT)
                    && (KPI_KinhDoanh_NhanVien.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN : e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)).Sum(e => e.TONGSOLUONG / (e.TYLE_QD_HH == 0 ? 1 : e.TYLE_QD_HH)));
                    TongSoLuong += TongSoLuong_ChuaQuyDoi;

                    var TongSoLuong_TraHang = lstChiTietTraHang.Where(e => e.bolDaTinhKpi == false && (itm.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_HANGHOA == itm.ID_HANGHOA && e.ID_DVT == itm.ID_DVT : e.ID_NHOMHANGHOA == itm.ID_HANGHOA && e.ID_DVT == itm.ID_DVT)
                    && (KPI_KinhDoanh_NhanVien.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN : e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)).Sum(e => e.TONGSOLUONG / e.TYLE_QD);
                    var TongSoLuong_TraHang_ChuaQuyDoi = Convert.ToInt32(lstChiTietTraHang.Where(e => e.bolDaTinhKpi == false && (itm.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_HANGHOA == itm.ID_HANGHOA && e.ID_DVT != itm.ID_DVT : e.ID_NHOMHANGHOA == itm.ID_HANGHOA && e.ID_DVT != itm.ID_DVT)
                    && (KPI_KinhDoanh_NhanVien.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN : e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)).Sum(e => e.TONGSOLUONG / (e.TYLE_QD_HH == 0 ? 1 : e.TYLE_QD_HH)));
                    TongSoLuong_TraHang += TongSoLuong_TraHang_ChuaQuyDoi;

                    if ((itm.SOTIEN > 0 || itm.SOLUONG > 0 || itm.CHIETKHAU > 0 || itm.TIENGIAM > 0) && (0 < (TongTien - TongTien_TraHang) && 0 <= (TongSoLuong - TongSoLuong_TraHang)))
                    {
                        //var HangHoa = _context.view_dm_HangHoa!.Where(e => e.ID == itm.ID_HANGHOA).FirstOrDefault();
                        var DVT = _context.dm_DonViTinh!.Where(e => e.ID == itm.ID_DVT).FirstOrDefault();
                        v_Tinh_KPI_KinhDoanh_ChiTiet newv_Tinh_KPI_KinhDoanh_ChiTiet = new v_Tinh_KPI_KinhDoanh_ChiTiet();
                        newv_Tinh_KPI_KinhDoanh_ChiTiet = new v_Tinh_KPI_KinhDoanh_ChiTiet();
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.HINHTHUC = itm.HINHTHUC;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_HINHTHUC = itm.NAME_HINHTHUC;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.ID_KPI_KINHDOANH = KPI_KinhDoanh.ID;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_KPI_KINHDOANH = KPI_KinhDoanh.NAME;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.ID_HANGHOA = itm.ID_HANGHOA;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_HANGHOA = itm.NAME;
                        if ((itm.CHIETKHAU > 0 || itm.TIENGIAM > 0))
                        {
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN = TongTien;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN_TRAHANG = TongTien_TraHang;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN_KPI = itm.SOTIEN;
                        }

                        int PhanNguyen = 0;
                        double TYLE_QD = 1;
                        //if (HangHoa != null)
                        //    TYLE_QD = HangHoa.TYLE_QD;

                        PhanNguyen = Convert.ToInt32(TongSoLuong) / Convert.ToInt32(TYLE_QD);

                        newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_DVT = itm.NAME_DVT;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_DVT_QD = DVT != null ? DVT.NAME : "";
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.TYLE_QD = TYLE_QD;
                        if ((itm.HINHTHUC_TINHKPI == ((int)API.HinhThucTinhKPI.ThuongTheoSoLuong1) && (itm.CHIETKHAU > 0 || itm.TIENGIAM > 0)))
                        {
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG = Convert.ToInt32(TongSoLuong) / Convert.ToInt32(TYLE_QD);
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG_TRAHANG = Convert.ToInt32(TongSoLuong_TraHang) / Convert.ToInt32(TYLE_QD); ;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG_KPI = itm.SOLUONG.ToString("N0").Replace(',', '.') + " " + itm.NAME_DVT;
                        }
                        if ((itm.HINHTHUC_TINHKPI == ((int)API.HinhThucTinhKPI.ThuongKhiDatMoc) && (itm.CHIETKHAU > 0 || itm.TIENGIAM > 0)))
                        {
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG = Convert.ToInt32(TongSoLuong) / Convert.ToInt32(TYLE_QD);
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG_TRAHANG = Convert.ToInt32(TongSoLuong_TraHang) / Convert.ToInt32(TYLE_QD); ;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG_KPI = itm.SOLUONG.ToString("N0").Replace(',', '.') + " " + itm.NAME_DVT;
                        }
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU = itm.CHIETKHAU;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.TIENTHUONG = itm.TIENGIAM;
                        PhanNguyen = Convert.ToInt32(TongSoLuong - TongSoLuong_TraHang) / Convert.ToInt32(TYLE_QD);
                        if((itm.SOTIEN <= (TongTien - TongTien_TraHang) && itm.SOLUONG <= (TongSoLuong - TongSoLuong_TraHang)))
                        {
                            if (itm.HINHTHUC_TINHKPI == ((int)API.HinhThucTinhKPI.ThuongTheoSoLuong1))
                            {
                                newv_Tinh_KPI_KinhDoanh_ChiTiet.SOTIEN_KPI = newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU > 0 ? (newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU * (TongTien - TongTien_TraHang) / 100) : itm.TIENGIAM * (PhanNguyen - itm.SOLUONG);
                            }
                            else if (itm.HINHTHUC_TINHKPI == ((int)API.HinhThucTinhKPI.ThuongKhiDatMoc))
                            {
                                newv_Tinh_KPI_KinhDoanh_ChiTiet.SOTIEN_KPI = newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU > 0 ? (newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU * (TongTien - TongTien_TraHang) / 100) : itm.TIENGIAM * (PhanNguyen);
                            }
                            else if (itm.HINHTHUC_TINHKPI == ((int)API.HinhThucTinhKPI.ThuongTuTroLen))
                            {
                                newv_Tinh_KPI_KinhDoanh_ChiTiet.SOTIEN_KPI = newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU > 0 ? (newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU * (TongTien - TongTien_TraHang - itm.SOTIEN) / 100) : itm.TIENGIAM;
                            }
                            else
                                newv_Tinh_KPI_KinhDoanh_ChiTiet.SOTIEN_KPI = newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU > 0 ? newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU * (TongTien - TongTien_TraHang) / 100 : itm.TIENGIAM;
                        }    
                        
                        lstTinh_KPI_KinhDoanh_ChiTiet.Add(newv_Tinh_KPI_KinhDoanh_ChiTiet);

                        foreach (var Dathang in lstChiTietDatHang.Where(e => e.bolDaTinhKpi == false && (itm.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_HANGHOA == itm.ID_HANGHOA : e.ID_NHOMHANGHOA == itm.ID_HANGHOA)
                         && (KPI_KinhDoanh_NhanVien.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN : e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)))
                        {
                            //Dathang.bolDaTinhKpi = true;
                        }

                        foreach (var Dathang in lstChiTietTraHang.Where(e => e.bolDaTinhKpi == false && (itm.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_HANGHOA == itm.ID_HANGHOA : e.ID_NHOMHANGHOA == itm.ID_HANGHOA)
                         && (KPI_KinhDoanh_NhanVien.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN : e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)))
                        {
                            //Dathang.bolDaTinhKpi = true;
                        }

                        foreach (var Dathang in lstChiTietDatHang.Where(e => e.bolDaTinhKpi == false && (itm.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_HANGHOA == itm.ID_HANGHOA && e.ID_DVT == itm.ID_DVT : e.ID_NHOMHANGHOA == itm.ID_HANGHOA && e.ID_DVT == itm.ID_DVT)
                         && (KPI_KinhDoanh_NhanVien.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN : e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)))
                        {
                            //Dathang.bolDaTinhKpi = true;
                        }

                        foreach (var Dathang in lstChiTietTraHang.Where(e => e.bolDaTinhKpi == false && (itm.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_HANGHOA == itm.ID_HANGHOA && e.ID_DVT == itm.ID_DVT : e.ID_NHOMHANGHOA == itm.ID_HANGHOA && e.ID_DVT == itm.ID_DVT)
                        && (KPI_KinhDoanh_NhanVien.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham ? e.ID_TAIKHOAN == KPI_KinhDoanh_NhanVien.ID_TAIKHOAN : e.ID_NHOMQUYEN == KPI_KinhDoanh_NhanVien.ID_NHANVIEN)))
                        {
                            //Dathang.bolDaTinhKpi = true;
                        }
                        TongTien_Tong += (TongTien - TongTien_TraHang);
                    }
                    else
                        bolChietKhauTong = false;

                    TongTien_TongDatKM += TongTien;
                    TongTien_Tong_TraHang += TongTien_TraHang;
                }
                var TongSoLuong_TongDatKM = lstChiTietDatHang.Where(e => e.ID_DVT == KPI_KinhDoanh.ID_DVT_DATKM).Sum(e => e.TONGCONG);
                var TongSoLuong_TongDatKM_TraHang = lstChiTietTraHang.Where(e => e.ID_DVT == KPI_KinhDoanh.ID_DVT_DATKM).Sum(e => e.TONGCONG);
                if (KPI_KinhDoanh.IS_YEUCAUCHITIET)
                {
                    if (bolChietKhauTong)
                    {
                        if (KPI_KinhDoanh.CHIETKHAU > 0 || KPI_KinhDoanh.TIENGIAM > 0)
                        {
                            v_Tinh_KPI_KinhDoanh_ChiTiet newv_Tinh_KPI_KinhDoanh_ChiTiet = new v_Tinh_KPI_KinhDoanh_ChiTiet();
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.HINHTHUC = -1;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_HINHTHUC = "Tổng";
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.ID_KPI_KINHDOANH = KPI_KinhDoanh.ID;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_KPI_KINHDOANH = KPI_KinhDoanh.NAME;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_HANGHOA = newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_HINHTHUC;

                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN = TongTien_TongDatKM;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN_TRAHANG = TongTien_Tong_TraHang;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN_KPI = KPI_KinhDoanh.TONGTIEN_DATKM;
                            if (!string.IsNullOrEmpty(KPI_KinhDoanh.NAME_DVT))
                            {
                                newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG = TongSoLuong_TongDatKM;
                                newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_DVT = KPI_KinhDoanh.NAME_DVT;
                                newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG_TRAHANG = TongSoLuong_TongDatKM_TraHang;
                                newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG_KPI = KPI_KinhDoanh.SOLUONG_DATKM + " " + KPI_KinhDoanh.NAME_DVT;
                            }

                            newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU = KPI_KinhDoanh.CHIETKHAU;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TIENTHUONG = KPI_KinhDoanh.TIENGIAM;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.SOTIEN_KPI = newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU > 0 ? newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU * TongTien_Tong / 100 : KPI_KinhDoanh.TIENGIAM;
                            lstTinh_KPI_KinhDoanh_ChiTiet.Add(newv_Tinh_KPI_KinhDoanh_ChiTiet);
                        }
                    }
                }
                else
                {

                    if ((KPI_KinhDoanh.CHIETKHAU > 0 || KPI_KinhDoanh.TIENGIAM > 0) && 0 < TongSoLuong_TongDatKM && 0 < TongTien_TongDatKM)
                    {

                        v_Tinh_KPI_KinhDoanh_ChiTiet newv_Tinh_KPI_KinhDoanh_ChiTiet = new v_Tinh_KPI_KinhDoanh_ChiTiet();
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.HINHTHUC = -1;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_HINHTHUC = "Tổng";
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.ID_KPI_KINHDOANH = KPI_KinhDoanh.ID;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_KPI_KINHDOANH = KPI_KinhDoanh.NAME;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_HANGHOA = newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_HINHTHUC;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN = TongTien_TongDatKM;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN_TRAHANG = TongTien_Tong_TraHang;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN_KPI = KPI_KinhDoanh.TONGTIEN_DATKM;
                        if (!string.IsNullOrEmpty(KPI_KinhDoanh.NAME_DVT))
                        {
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG = TongSoLuong_TongDatKM;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.NAME_DVT = KPI_KinhDoanh.NAME_DVT;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG_TRAHANG = TongSoLuong_TongDatKM_TraHang;
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGSOLUONG_KPI = KPI_KinhDoanh.SOLUONG_DATKM + " " + KPI_KinhDoanh.NAME_DVT;
                        }
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU = KPI_KinhDoanh.CHIETKHAU;
                        newv_Tinh_KPI_KinhDoanh_ChiTiet.TIENTHUONG = KPI_KinhDoanh.TIENGIAM;
                        if(KPI_KinhDoanh.SOLUONG_DATKM <= TongSoLuong_TongDatKM && KPI_KinhDoanh.TONGTIEN_DATKM <= TongTien_TongDatKM)
                            newv_Tinh_KPI_KinhDoanh_ChiTiet.SOTIEN_KPI = newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU > 0 ? newv_Tinh_KPI_KinhDoanh_ChiTiet.CHIETKHAU * (newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN - newv_Tinh_KPI_KinhDoanh_ChiTiet.TONGTIEN_TRAHANG) / 100 : KPI_KinhDoanh.TIENGIAM;
                        lstTinh_KPI_KinhDoanh_ChiTiet.Add(newv_Tinh_KPI_KinhDoanh_ChiTiet);
                    }
                    else
                    {
                        //if (!(KPI_KinhDoanh.SOLUONG_DATKM <= TongSoLuong_TongDatKM) || !(KPI_KinhDoanh.TONGTIEN_DATKM <= TongTien_TongDatKM))
                        //    lstTinh_KPI_KinhDoanh_ChiTiet.Clear();
                    }

                }
                return lstTinh_KPI_KinhDoanh_ChiTiet;
            }
            catch
            {
                return new List<v_Tinh_KPI_KinhDoanh_ChiTiet>();
            }

        }
        #endregion
    }
}