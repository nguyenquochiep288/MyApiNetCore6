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

namespace MyApiNetCore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public ProductController(dbTrangHiepPhatContext context, IConfiguration configuration)
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

                var lstValue = await _context.view_dm_HangHoa!.Where(e => e.LOC_ID == LOC_ID).OrderBy(e => e.MA).ToListAsync();
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
                var lstValue = await _context.view_dm_HangHoa!.Where(e => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).OrderBy(e => e.MA).ToListAsync();
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
                var Product = await _context.view_dm_HangHoa!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (Product == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }

                _context.dm_HangHoa_Kho!.Any(e => e.LOC_ID == LOC_ID && e.ID == ID);

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = Product
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
        public async Task<IActionResult> PutProduct(string LOC_ID, string MA, [FromBody] v_dm_HangHoa Product)
        {
            try
            {
                if (ProductExists(Product))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + Product.LOC_ID + "-" + Product.MA + " trong dữ liệu!",
                        Data = ""
                    });
                }

                if (LOC_ID != Product.LOC_ID || Product.MA != MA)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Dữ liệu khóa không giống nhau!",
                        Data = ""
                    });
                }
                if (!ProductExistsID(Product.LOC_ID, Product.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + Product.LOC_ID + "-" + Product.ID + " dữ liệu!",
                        Data = ""
                    });
                }
                
                using var transaction = _context.Database.BeginTransaction();
                {
                    if (!Product.BAOGOMTHUESUAT)
                    {
                        Product.ID_THUESUAT = null;
                    }
                    if (Product.LOAIHANGHOA == ((int)API.LoaiSanPham.BinhThuong).ToString())
                    {
                        var lstdm_HangHoa_Combo = await _context.dm_HangHoa_Combo!.Where(e => e.LOC_ID == Product.LOC_ID && e.ID_HANGHOA == Product.ID).ToListAsync();
                        if (lstdm_HangHoa_Combo != null)
                        {
                            foreach (var itm in lstdm_HangHoa_Combo)
                            {
                                itm.TYLE_QD = Product.TYLE_QD;
                                itm.QTY_TOTAL = itm.TYLE_QD * itm.QTY;
                                _context.Entry(itm).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            }
                        }
                    }
                    else if (Product.LOAIHANGHOA == ((int)API.LoaiSanPham.KhongQuanLyTonKho).ToString())
                    {
                        Product.ISCOMBO = false;
                        Product.STATUS_QD = false;
                        Product.TYLE_QD = 0;
                    }
                    else if (Product.LOAIHANGHOA == ((int)API.LoaiSanPham.Combo).ToString())
                    {
                        Product.ISCOMBO = false;
                        Product.STATUS_QD = false;
                        Product.TYLE_QD = 1;
                        var lstdm_HangHoa_Combo = await _context.dm_HangHoa_Combo!.Where(e => e.LOC_ID == Product.LOC_ID && e.ID_HANGHOACOMBO == Product.ID).ToListAsync();
                        if (lstdm_HangHoa_Combo != null)
                        {
                            foreach (var itm in lstdm_HangHoa_Combo)
                            {
                                var checkHangHoa_Combo =  Product.lstdm_HangHoa_Combo.Where(e => e.ID_HANGHOA == itm.ID_HANGHOA && e.ID_DVT == itm.ID_DVT).FirstOrDefault();
                                if(checkHangHoa_Combo != null)
                                {
                                    checkHangHoa_Combo.ISEDIT = true;
                                    itm.QTY = checkHangHoa_Combo.QTY;
                                    itm.TYLE_QD = checkHangHoa_Combo.TYLE_QD;
                                    itm.QTY_TOTAL = itm.QTY * itm.TYLE_QD;
                                    itm.ID_NGUOISUA = checkHangHoa_Combo.ID_NGUOITAO;
                                    itm.THOIGIANSUA = checkHangHoa_Combo.THOIGIANTHEM;
                                }
                                else
                                    _context.dm_HangHoa_Combo!.Remove(itm);
                            }
                        }

                        if (Product.lstdm_HangHoa_Combo != null)
                        {
                            foreach (dm_HangHoa_Combo itm in Product.lstdm_HangHoa_Combo.Where(e => e.ISEDIT == false))
                            {
                                itm.ID = Guid.NewGuid().ToString();
                                itm.LOC_ID = Product.LOC_ID;
                                itm.ID_HANGHOACOMBO = Product.ID;
                                itm.ISACTIVE = true;
                                itm.QTY_TOTAL = itm.QTY * itm.TYLE_QD;
                                _context.dm_HangHoa_Combo!.Add(itm);
                            }
                        }
                    }
                    _context.Entry(Product).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
                transaction.Commit();
                var OKProduct = await _context.view_dm_HangHoa!.FirstOrDefaultAsync(e => e.LOC_ID == Product.LOC_ID && e.ID == Product.ID);

                if (!string.IsNullOrEmpty(Product.PICTURE) && Product.FILENEW)
                {
                    string path = API.PathProductServer;
                    if (Product.FILENEW)
                    {
                        try
                        {
                            if (!System.IO.Directory.Exists(path))
                            {
                                System.IO.Directory.CreateDirectory(path);
                            }
                            if (System.IO.File.Exists(path + "\\" + Product.PICTURE))
                            {
                                System.IO.File.Delete(path + "\\" + Product.PICTURE);
                            }
                            byte[] tempBytes = Convert.FromBase64String(Product.FILEBASE64);
                            System.IO.File.WriteAllBytes(path + "\\" + Product.PICTURE, tempBytes);
                        }
                        catch (Exception e)
                        {
                            return Ok(new ApiResponse
                            {
                                Success = false,
                                Message = e.Message,
                                Data = ""
                            });
                        }

                    }
                }

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
        public async Task<ActionResult<v_dm_HangHoa>> PostProduct([FromBody] v_dm_HangHoa Product)
        {
            try
            {
                if (ProductExistsMA(Product.LOC_ID, Product.MA))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + Product.LOC_ID + "-" + Product.MA + " trong dữ liệu!",
                        Data = ""
                    });
                }
                if (!string.IsNullOrEmpty(Product.PICTURE) && !string.IsNullOrEmpty(Product.FILEBASE64))
                {
                    try
                    {
                        string path = API.PathProductServer;
                        if (!System.IO.Directory.Exists(path))
                        {
                            System.IO.Directory.CreateDirectory(path);
                        }
                        byte[] tempBytes = Convert.FromBase64String(Product.FILEBASE64);
                        System.IO.File.WriteAllBytes(path + "\\" + Product.PICTURE, tempBytes);
                    }
                    catch (Exception e)
                    {
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = e.Message,
                            Data = ""
                        });
                    }
                }
                using var transaction = _context.Database.BeginTransaction();
                {
                    if (!Product.BAOGOMTHUESUAT)
                    {
                        Product.ID_THUESUAT = null;
                    }
                    if (Product.LOAIHANGHOA == ((int)API.LoaiSanPham.BinhThuong).ToString())
                    {

                    }
                    else if (Product.LOAIHANGHOA == ((int)API.LoaiSanPham.BinhThuong).ToString())
                    {
                        Product.ISCOMBO = false;
                        Product.STATUS_QD = false;
                        Product.TYLE_QD = 0;
                    }
                    else if (Product.LOAIHANGHOA == ((int)API.LoaiSanPham.Combo).ToString())
                    {
                        Product.ISCOMBO = false;
                        Product.STATUS_QD = false;
                        Product.TYLE_QD = 1;
                        if (Product.lstdm_HangHoa_Combo != null)
                        {
                            foreach (dm_HangHoa_Combo itm in Product.lstdm_HangHoa_Combo)
                            {
                                itm.ID = Guid.NewGuid().ToString();
                                itm.LOC_ID = Product.LOC_ID;
                                itm.ID_HANGHOACOMBO = Product.ID;
                                itm.ISACTIVE = true;
                                itm.QTY_TOTAL = itm.QTY * itm.TYLE_QD;
                                _context.dm_HangHoa_Combo!.Add(itm);
                            }
                        }
                    }

                    _context.dm_HangHoa!.Add((dm_HangHoa)Product);

                    var lstKho = await _context.dm_Kho!.Where(e => e.LOC_ID == Product.LOC_ID).ToListAsync();
                    if (lstKho != null)
                    {
                        foreach (dm_Kho itm in lstKho)
                        {
                            dm_HangHoa_Kho dm_HangHoa_Kho = new dm_HangHoa_Kho();
                            dm_HangHoa_Kho.ID = Guid.NewGuid().ToString();
                            dm_HangHoa_Kho.LOC_ID = Product.LOC_ID;
                            dm_HangHoa_Kho.ID_KHO = itm.ID;
                            dm_HangHoa_Kho.ID_HANGHOA = Product.ID;
                            dm_HangHoa_Kho.QTY = 0;
                            _context.dm_HangHoa_Kho!.Add(dm_HangHoa_Kho);
                        }
                    }
                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
                transaction.Commit();
                var OKProduct = await _context.view_dm_HangHoa!.FirstOrDefaultAsync(e => e.LOC_ID == Product.LOC_ID && e.ID == Product.ID);
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
                var Product = await _context.dm_HangHoa!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (Product == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
                var lstdm_HangHoa_Kho = await _context.dm_HangHoa_Kho!.Where(e => e.LOC_ID == LOC_ID && e.ID_HANGHOA == ID).ToListAsync();
                ApiResponse apiResponse = new ApiResponse();
                if (lstdm_HangHoa_Kho != null)
                {
                    foreach (var itm in lstdm_HangHoa_Kho)
                    {
                        apiResponse = await ExecuteStoredProc.CheckDelete<dm_HangHoa_Kho>(itm, itm.ID, Product.NAME);
                        if (!apiResponse.Success)
                        {
                            return Ok(new ApiResponse
                            {
                                Success = false,
                                Message = apiResponse.Message,
                                Data = ""
                            });
                        }
                    }
                }
                apiResponse = await ExecuteStoredProc.CheckDelete<dm_HangHoa>(Product, Product.ID, Product.NAME);
                if (!apiResponse.Success)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = apiResponse.Message,
                        Data = ""
                    });
                }

                using var transaction = _context.Database.BeginTransaction();
                {
                    var lstweb_PhanQuyenSanPham = await _context.web_PhanQuyenSanPham!.Where(e => e.LOC_ID == LOC_ID && e.ID_SANPHAM == ID).ToListAsync();
                    if (lstweb_PhanQuyenSanPham != null)
                    {
                        foreach (var itm in lstweb_PhanQuyenSanPham)
                        {
                            _context.web_PhanQuyenSanPham!.Remove(itm);
                        }
                    }

                    var lstdm_HangHoa_Combo = await _context.dm_HangHoa_Combo!.Where(e => e.LOC_ID == LOC_ID && e.ID_HANGHOACOMBO == ID).ToListAsync();
                    if (lstdm_HangHoa_Combo != null)
                    {
                        foreach (var itm in lstdm_HangHoa_Combo)
                        {
                            _context.dm_HangHoa_Combo!.Remove(itm);
                        }
                    }

                    if (lstdm_HangHoa_Kho != null)
                    {
                        foreach (var itm in lstdm_HangHoa_Kho)
                        {
                            _context.dm_HangHoa_Kho!.Remove(itm);
                        }
                    }
                    _context.dm_HangHoa!.Remove(Product);
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
            return _context.dm_HangHoa!.Any(e => e.LOC_ID == LOC_ID && e.MA == MA);
        }

        private bool ProductExistsID(string LOC_ID, string ID)
        {
            return _context.dm_HangHoa!.Any(e => e.LOC_ID == LOC_ID && e.ID == ID);
        }

        private bool ProductExists(dm_HangHoa HangHoa)
        {
            return _context.dm_HangHoa!.Any(e => e.LOC_ID == HangHoa.LOC_ID && e.MA == HangHoa.MA && e.ID != HangHoa.ID);
        }
    }
}