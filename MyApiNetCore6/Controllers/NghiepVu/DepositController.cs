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
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using static DatabaseTHP.Class.API;

namespace MyApiNetCore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepositController : ControllerBase
    {
        
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public DepositController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
        }
        [HttpGet("{LOC_ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetDeposit(string LOC_ID)
        {
            try
            {

                var lstValue = await _context.ct_PhieuDatHang!.Where(e => e.LOC_ID == LOC_ID).ToListAsync();
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

        // GET: api/Input
        [HttpGet("{LOC_ID}/{Type}/{KeyWhere}/{ValuesSearch}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetInput(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
        {
            try
            {
                var lstValue = await _context.ct_PhieuDatHang!.Where(e => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).OrderBy(e => e.MAPHIEU).ToListAsync();
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

        // GET: api/Deposit
        [HttpGet("{LOC_ID}/{ID_KHO}/{FROMDATE}/{TODATE}/{Type}/{KeyWhere}/{ValuesSearch}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetDeposit(string LOC_ID, string ID_KHO, DateTime FROMDATE, DateTime TODATE, int Type, string KeyWhere = "", string ValuesSearch = "")
        {
            try
            {
                var lstValue = await _context.ct_PhieuDatHang!.Where(e => e.LOC_ID == LOC_ID && e.NGAYLAP.Date >= FROMDATE.Date && e.NGAYLAP.Date <= TODATE.Date).Where(KeyWhere, ValuesSearch).ToListAsync();
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


        //GET: api/Deposit/5
        [HttpGet("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetDeposit(string LOC_ID, string ID)
        {
            try
            {
                var Deposit = await _context.ct_PhieuDatHang!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);

                if (Deposit == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                v_ct_PhieuDatHang ct_PhieuDatHang = new v_ct_PhieuDatHang();
                if (Deposit != null)
                {
                    string strDeposit = JsonConvert.SerializeObject(Deposit);
                    ct_PhieuDatHang = JsonConvert.DeserializeObject<v_ct_PhieuDatHang>(strDeposit) ?? new v_ct_PhieuDatHang();
                }

                ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet = new List<v_ct_PhieuDatHang_ChiTiet>();
                SP_Parameter SP_Parameter = new SP_Parameter();
                SP_Parameter.ID_PHIEUDATHANG = ID;
                ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
                var actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuDatHang_ChiTiet(SP_Parameter);
                var okResult = actionResult as OkObjectResult;
                if (okResult != null)
                {
                    var ApiResponse = okResult.Value as ApiResponse;
                    if (ApiResponse != null)
                    {
                        if (ApiResponse.Data != null)
                        {
                            var lst_ChiTiet = ApiResponse.Data as List<v_ct_PhieuDatHang_ChiTiet>;
                            if (lst_ChiTiet != null)
                            {
                                ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet.AddRange(lst_ChiTiet);
                            }
                        }

                    }
                }

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

        // PUT: api/Deposit/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{LOC_ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PutDeposit(string LOC_ID, [FromBody] List<Product_Detail> lstProduct_Detail)
        {
            try
            {
                return await Get_ChuongTrinhKhuyenMai(lstProduct_Detail, LOC_ID);
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

        private static readonly Queue<v_ct_PhieuDatHang> requestQueue = new Queue<v_ct_PhieuDatHang>();
        private static bool isProcessing = false;
        // PUT: api/Deposit/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PutDeposit(string LOC_ID, string ID, [FromBody] v_ct_PhieuDatHang Deposit)
        {
            try
            {
                if (!DepositExistsID(Deposit.LOC_ID, Deposit.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + Deposit.LOC_ID + "-" + Deposit.ID + " dữ liệu!",
                        Data = ""
                    });
                }
                else
                {
                    if (!string.IsNullOrEmpty(Deposit.ID_PHIEUXUAT))
                    {
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Phiếu đặt hàng " + Deposit.MAPHIEU + " đã được tạo phiếu xuất!",
                            Data = ""
                        });
                    }
                }


                string StrHetSoLuong = "";
                using var transaction = _context.Database.BeginTransaction();
                {
                    //Deposit.TONGTIENTINHTHUE = 0;
                    var lstPhieuNhap_ChiTiet = await _context.ct_PhieuDatHang_ChiTiet!.Where(e => e.LOC_ID == Deposit.LOC_ID && e.ID_PHIEUDATHANG == Deposit.ID).ToListAsync();
                    if (lstPhieuNhap_ChiTiet != null)
                    {
                        foreach (ct_PhieuDatHang_ChiTiet itm in lstPhieuNhap_ChiTiet)
                        {
                            var objdm_HangHoa_Kho = await _context.dm_HangHoa_Kho!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO);
                            if (objdm_HangHoa_Kho != null)
                            {
                                itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
                                objdm_HangHoa_Kho.QTY += itm.TONGSOLUONG;
                                _context.Entry(objdm_HangHoa_Kho).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            }
                            else
                            {
                                return Ok(new ApiResponse
                                {
                                    Success = false,
                                    Message = "Không tìm thấy sản phẩm kho!" + itm.ID_HANGHOAKHO,
                                    Data = ""
                                });
                            }
                            var chkPhieuNhap_ChiTiet = Deposit.lstct_PhieuDatHang_ChiTiet.Where(e => e.ID == itm.ID).FirstOrDefault();
                            if (chkPhieuNhap_ChiTiet != null)
                            {
                                chkPhieuNhap_ChiTiet.ISEDIT = true;
                                chkPhieuNhap_ChiTiet.ID_PHIEUDATHANG = Deposit.ID;
                                ct_PhieuDatHang_ChiTiet newct_PhieuDatHang_ChiTiet = new ct_PhieuDatHang_ChiTiet();
                                newct_PhieuDatHang_ChiTiet = ConvertobjectToct_PhieuDatHang_ChiTiet<v_ct_PhieuDatHang_ChiTiet>(chkPhieuNhap_ChiTiet, itm);
                                newct_PhieuDatHang_ChiTiet.TONGSOLUONG = newct_PhieuDatHang_ChiTiet.TYLE_QD * newct_PhieuDatHang_ChiTiet.SOLUONG;
                                _context.Entry(newct_PhieuDatHang_ChiTiet).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            }
                            else
                            {
                                _context.ct_PhieuDatHang_ChiTiet!.Remove(itm);
                            }
                        }
                    }

                    if (Deposit.lstct_PhieuDatHang_ChiTiet != null)
                    {

                        foreach (v_ct_PhieuDatHang_ChiTiet itm in Deposit.lstct_PhieuDatHang_ChiTiet)
                        {
                            itm.THANHTIEN = itm.SOLUONG * itm.DONGIA - itm.TONGTIENGIAMGIA;
                            itm.TONGCONG = itm.THANHTIEN + itm.TONGTIENVAT;
                            itm.ID_PHIEUDATHANG = Deposit.ID;
                            var objdm_HangHoa_Kho = _context.dm_HangHoa_Kho!.FirstOrDefault(e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO && e.ID_KHO == Deposit.ID_KHO);
                            if (objdm_HangHoa_Kho != null)
                            {
                                var objdm_HangHoa = _context.view_dm_HangHoa!.FirstOrDefault(e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOA);
                                itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
                                if (objdm_HangHoa_Kho.QTY >= itm.TONGSOLUONG)
                                {
                                    //if (objdm_HangHoa != null)
                                    //{
                                    //    Deposit.TONGTIENTINHTHUE += (itm.TONGTIENGIAMGIA + (itm.ISKHUYENMAI ? ((objdm_HangHoa.ID_DVT == itm.ID_DVT ? objdm_HangHoa.GIA01 : objdm_HangHoa.GIA01_QD) * itm.SOLUONG) : 0) * objdm_HangHoa.MUCTHUE) / 100 ;
                                    //}
                                    objdm_HangHoa_Kho.QTY -= itm.TONGSOLUONG;
                                    _context.Entry(objdm_HangHoa_Kho).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                }
                                else
                                {

                                    string Strsoluong = "";
                                    if (objdm_HangHoa != null && itm.TYLE_QD >= 1)
                                    {
                                        int soluong = 0;
                                        if (itm.TYLE_QD > 1)
                                        {
                                            soluong = Convert.ToInt32(objdm_HangHoa_Kho.QTY) / Convert.ToInt32(itm.TYLE_QD);
                                            if (soluong > 0)
                                                Strsoluong = soluong.ToString("N0") + " " + objdm_HangHoa.NAME_DVT;

                                            if (objdm_HangHoa_Kho.QTY - (soluong * itm.TYLE_QD) > 0)
                                            {
                                                if (!string.IsNullOrEmpty(Strsoluong))
                                                    Strsoluong += (" " + (objdm_HangHoa_Kho.QTY - (soluong * itm.TYLE_QD)).ToString("N0") + " " + objdm_HangHoa.NAME_DVT_QD) + Environment.NewLine;
                                                else
                                                    Strsoluong += (objdm_HangHoa_Kho.QTY - (soluong * itm.TYLE_QD)).ToString("N0") + " " + objdm_HangHoa.NAME_DVT_QD;
                                            }
                                            StrHetSoLuong += "Sản phẩm " + itm.NAME + " không đủ tồn kho!" + Strsoluong + Environment.NewLine;
                                        }
                                        else
                                        {
                                            Strsoluong = objdm_HangHoa_Kho.QTY.ToString("N0") + " " + objdm_HangHoa.NAME_DVT;
                                            StrHetSoLuong += "Sản phẩm " + itm.NAME + " không đủ tồn kho!" + Strsoluong + Environment.NewLine;
                                        }
                                    }


                                }
                            }
                            else
                            {
                                return Ok(new ApiResponse
                                {
                                    Success = false,
                                    Message = "Không tìm thấy sản phẩm kho!" + itm.ID_HANGHOAKHO,
                                    Data = ""
                                });
                            }
                            if (!itm.ISEDIT)
                            {
                                _context.ct_PhieuDatHang_ChiTiet!.Add(itm);
                            }
                        }

                        if (!string.IsNullOrEmpty(StrHetSoLuong))
                        {
                            return Ok(new ApiResponse
                            {
                                Success = false,
                                Message = StrHetSoLuong,
                                Data = ""
                            });
                        }
                        Deposit.TONGTHANHTIEN = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum(s => s.THANHTIEN), 0);
                        Deposit.TONGTIENGIAMGIA = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum(s => s.TONGTIENGIAMGIA), 0);
                        Deposit.TONGTIENVAT = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum(s => s.TONGTIENVAT), 0);
                        Deposit.TONGTIEN = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum(s => s.TONGCONG), 0);
                    }
                    _context.Entry(Deposit).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    AuditLogController auditLog = new AuditLogController(_context, _configuration); auditLog.InserAuditLog(); await _context.SaveChangesAsync();
                }
                transaction.Commit();

                v_ct_PhieuDatHang ct_PhieuDatHang = new v_ct_PhieuDatHang();
                ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet = new List<v_ct_PhieuDatHang_ChiTiet>();
                SP_Parameter SP_Parameter = new SP_Parameter();
                SP_Parameter.ID_PHIEUNHAP = Deposit.ID;
                ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
                var actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuNhap(SP_Parameter);
                var okResult = actionResult as OkObjectResult;
                if (okResult != null)
                {
                    var ApiResponse = okResult.Value as ApiResponse;
                    if (ApiResponse != null)
                    {
                        if (ApiResponse.Data != null)
                        {
                            var lstPhieuDatHang = ApiResponse.Data as List<v_ct_PhieuDatHang>;
                            if (lstPhieuDatHang != null && lstPhieuDatHang.Count() > 0)
                            {
                                ct_PhieuDatHang = lstPhieuDatHang.FirstOrDefault() ?? new v_ct_PhieuDatHang();
                            }

                        }

                    }
                }
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
            finally
            {
                AuditLogController auditLog = new AuditLogController(_context, _configuration); 
                auditLog.DeleteRequest(strTable);
            }
        }
        private string strTable = "ct_PhieuDatHang";
        // POST: api/Deposit
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<ct_PhieuDatHang>> PostDeposit([FromBody] v_ct_PhieuDatHang Deposit)
        {
            try
            {
                if (DepositExistsID(Deposit.LOC_ID, Deposit.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + Deposit.LOC_ID + "-" + Deposit.ID + " trong dữ liệu!",
                        Data = "",
                        CheckValue = true
                    });
                }

                var objdm_NhanVien = await _context.dm_NhanVien!.FirstOrDefaultAsync(e => e.LOC_ID == Deposit.LOC_ID && e.ID_TAIKHOAN == Deposit.ID_NHANVIEN);
                if (objdm_NhanVien != null)
                {
                    var objPhieuNhap = await _context.ct_PhieuDatHang!.FirstOrDefaultAsync(e => e.LOC_ID == Deposit.LOC_ID && e.MAPHIEU == Deposit.MAPHIEU);
                    if (objPhieuNhap != null)
                    {
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Đã tồn tại" + Deposit.LOC_ID + "-" + Deposit.MAPHIEU + " trong dữ liệu!",
                            Data = "",
                            CheckValue = true
                        });
                    }

                    using var transaction = _context.Database.BeginTransaction();
                    {

                        if (Deposit.lstct_PhieuDatHang_ChiTiet != null)
                        {
                            string StrHetSoLuong = "";
                            foreach (var itm in Deposit.lstct_PhieuDatHang_ChiTiet)
                            {
                                itm.THANHTIEN = itm.SOLUONG * itm.DONGIA - itm.TONGTIENGIAMGIA;
                                itm.TONGCONG = itm.THANHTIEN + itm.TONGTIENVAT;
                                var objdm_HangHoa_Kho = await _context.dm_HangHoa_Kho!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO && e.ID_KHO == Deposit.ID_KHO);
                                if (objdm_HangHoa_Kho != null)
                                {
                                    var objdm_HangHoa = _context.view_dm_HangHoa!.FirstOrDefault(e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOA);
                                    itm.TONGSOLUONG = itm.SOLUONG * itm.TYLE_QD;
                                    if (objdm_HangHoa_Kho.QTY >= itm.TONGSOLUONG)
                                    {
                                        itm.GHICHU = objdm_HangHoa_Kho.QTY.ToString() + ";";

                                        //if (itm.ISKHUYENMAI && objdm_HangHoa != null)
                                        //{
                                        //    Deposit.TONGTIENTINHTHUE += (itm.TONGTIENGIAMGIA + (itm.ISKHUYENMAI ? ((objdm_HangHoa.ID_DVT == itm.ID_DVT ? objdm_HangHoa.GIA01 : objdm_HangHoa.GIA01_QD) * itm.SOLUONG) : 0) * objdm_HangHoa.MUCTHUE) / 100;
                                        //}
                                        objdm_HangHoa_Kho.QTY -= itm.TONGSOLUONG;
                                        itm.GHICHU += (objdm_HangHoa_Kho.QTY.ToString() + ";");

                                        _context.Entry(objdm_HangHoa_Kho).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                    }
                                    else
                                    {

                                        string Strsoluong = "";
                                        if (objdm_HangHoa != null && itm.TYLE_QD >= 1)
                                        {
                                            int soluong = 0;
                                            if (itm.TYLE_QD > 1)
                                            {
                                                soluong = Convert.ToInt32(objdm_HangHoa_Kho.QTY) / Convert.ToInt32(itm.TYLE_QD);
                                                if (soluong > 0)
                                                    Strsoluong = soluong.ToString("N0") + " " + objdm_HangHoa.NAME_DVT;

                                                if (objdm_HangHoa_Kho.QTY - (soluong * itm.TYLE_QD) > 0)
                                                {
                                                    if (!string.IsNullOrEmpty(Strsoluong))
                                                        Strsoluong += (" " + (objdm_HangHoa_Kho.QTY - (soluong * itm.TYLE_QD)).ToString("N0") + " " + objdm_HangHoa.NAME_DVT_QD) + Environment.NewLine;
                                                    else
                                                        Strsoluong += (objdm_HangHoa_Kho.QTY - (soluong * itm.TYLE_QD)).ToString("N0") + " " + objdm_HangHoa.NAME_DVT_QD;
                                                }
                                                StrHetSoLuong += "Sản phẩm " + itm.NAME + " không đủ tồn kho!" + Strsoluong + Environment.NewLine;
                                            }
                                            else
                                            {
                                                Strsoluong = objdm_HangHoa_Kho.QTY.ToString("N0") + " " + objdm_HangHoa.NAME_DVT;
                                                StrHetSoLuong += "Sản phẩm " + itm.NAME + " không đủ tồn kho!" + Strsoluong + Environment.NewLine;
                                            }
                                        }


                                    }

                                }
                                else
                                {
                                    return Ok(new ApiResponse
                                    {
                                        Success = false,
                                        Message = "Không tìm thấy sản phẩm kho!" + itm.ID_HANGHOAKHO,
                                        Data = ""
                                    });
                                }
                                var objct_PhieuDatHang_ChiTiet = await _context.ct_PhieuDatHang_ChiTiet!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID);
                                if (objct_PhieuDatHang_ChiTiet != null)
                                    itm.ID = Guid.NewGuid().ToString();

                                itm.LOC_ID = Deposit.LOC_ID;
                                itm.ID_PHIEUDATHANG = Deposit.ID;
                                _context.ct_PhieuDatHang_ChiTiet!.Add(itm);
                            }

                            if (!string.IsNullOrEmpty(StrHetSoLuong))
                            {
                                return Ok(new ApiResponse
                                {
                                    Success = false,
                                    Message = StrHetSoLuong,
                                    Data = ""
                                });
                            }
                            Deposit.TONGTHANHTIEN = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum(s => s.THANHTIEN),0);
                            Deposit.TONGTIENGIAMGIA = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum(s => s.TONGTIENGIAMGIA),0);
                            Deposit.TONGTIENVAT = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum(s => s.TONGTIENVAT),0);
                            Deposit.TONGTIEN = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum(s => s.TONGCONG),0);
                        }
                        _context.ct_PhieuDatHang!.Add(Deposit);
                        AuditLogController auditLog = new AuditLogController(_context, _configuration); auditLog.InserAuditLog(); await _context.SaveChangesAsync();
                    }
                    transaction.Commit();

                    v_ct_PhieuDatHang ct_PhieuDatHang = new v_ct_PhieuDatHang();
                    ct_PhieuDatHang.lstct_PhieuDatHang_ChiTiet = new List<v_ct_PhieuDatHang_ChiTiet>();
                    SP_Parameter SP_Parameter = new SP_Parameter();
                    SP_Parameter.ID_PHIEUNHAP = Deposit.ID;
                    ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
                    var actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuNhap(SP_Parameter);
                    var okResult = actionResult as OkObjectResult;
                    if (okResult != null)
                    {
                        var ApiResponse = okResult.Value as ApiResponse;
                        if (ApiResponse != null)
                        {

                            if (ApiResponse.Data != null)
                            {
                                var lstPhieuDatHang = ApiResponse.Data as List<v_ct_PhieuDatHang>;
                                if (lstPhieuDatHang != null && lstPhieuDatHang.Count() > 0)
                                {
                                    ct_PhieuDatHang = lstPhieuDatHang.FirstOrDefault() ?? new v_ct_PhieuDatHang();
                                }
                            }
                        }
                    }
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Success",
                        Data = ct_PhieuDatHang
                    });
                }
                else
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Tài khoản chưa được gắn với nhân viên trong dữ liệu!",
                        Data = ""
                    });
                }
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
            finally
            {
                AuditLogController auditLog = new AuditLogController(_context, _configuration); 
                auditLog.DeleteRequest(strTable);
            }
        }


        // POST: api/Deposit
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{LOC_ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PostDeposit(string LOC_ID, [FromBody] List<Product_Detail> lstProduct_Detail)
        {
            try
            {
                return await Get_ChuongTrinhKhuyenMai(lstProduct_Detail, LOC_ID);
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

        // POST: api/Deposit
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostCreateOutput")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<ct_PhieuDatHang>> PostDeposit([FromBody] List<Deposit> lstDeposit)
        {
            try
            {
                string LOC_ID = "";
                string ID_KHO = "";
                string ID_NGUOITAO = "";
                DateTime NGAYLAP = new DateTime();
                NGAYLAP = DateTime.Now.Date;
                if (lstDeposit != null && lstDeposit.Count > 0)
                {
                    Deposit Deposit = lstDeposit.FirstOrDefault() ?? new Deposit();
                    LOC_ID = Deposit != null ? Deposit.LOC_ID : "";
                    ID_NGUOITAO = Deposit != null ? Deposit.ID_NGUOITAO : "";
                    NGAYLAP = Deposit != null ? Deposit.NGAYLAP : DateTime.Now.Date;
                }
                else
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy dữ liệu!",
                        Data = ""
                    });
                }
                List<ct_PhieuDatHang_ChiTiet> lstPhieuDatHang_ChiTiet = new List<ct_PhieuDatHang_ChiTiet>();
                Dictionary<string, string> lstPhieuDatHang = new Dictionary<string, string>();
                using var transaction = _context.Database.BeginTransaction();
                {
                    var Max_ID = _context.ct_PhieuXuat!.Where(e => e.LOC_ID == LOC_ID && e.NGAYLAP.Date == NGAYLAP.Date).Select(e => e.SOPHIEU).DefaultIfEmpty().Max();
                    foreach (Deposit itm in lstDeposit)
                    {
                        var PhieuDatHang = await _context.ct_PhieuDatHang!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID);
                        if (PhieuDatHang == null)
                        {
                            return Ok(new ApiResponse
                            {
                                Success = false,
                                Message = "Không tìm thấy " + LOC_ID + "-" + itm.ID + " dữ liệu!",
                                Data = ""
                            });
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(PhieuDatHang.ID_PHIEUXUAT))
                            {
                                return Ok(new ApiResponse
                                {
                                    Success = false,
                                    Message = "Phiếu đặt hàng " + PhieuDatHang.MAPHIEU + " đã được tạo phiếu xuất!",
                                    Data = ""
                                });
                            }
                            if (!string.IsNullOrEmpty(ID_KHO) && ID_KHO != PhieuDatHang.ID_KHO)
                            {
                                return Ok(new ApiResponse
                                {
                                    Success = false,
                                    Message = "Phiếu đặt hàng " + PhieuDatHang.MAPHIEU + " khác kho với các phiếu khác!",
                                    Data = ""
                                });
                            }
                        }


                        ID_KHO = PhieuDatHang.ID_KHO;
                        lstPhieuDatHang.Add(PhieuDatHang.ID, PhieuDatHang.ID_KHACHHANG);

                        var lstChiTietPhieuDatHang_CT = await _context.ct_PhieuDatHang_ChiTiet!.Where(e => e.LOC_ID == itm.LOC_ID && e.ID_PHIEUDATHANG == itm.ID).ToArrayAsync();
                        if (lstChiTietPhieuDatHang_CT == null || lstChiTietPhieuDatHang_CT.Count() == 0)
                        {
                            return Ok(new ApiResponse
                            {
                                Success = false,
                                Message = "Không tìm thấy chi tiết phiếu đặt hàng " + LOC_ID + "-" + itm.ID + " dữ liệu!",
                                Data = ""
                            });
                        }
                        lstPhieuDatHang_ChiTiet.AddRange(lstChiTietPhieuDatHang_CT);

                    }
                    var dm_LoaiPhieuXuat = await _context.dm_LoaiPhieuXuat!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.MA == API.XHKH);

                    foreach (var itm in lstPhieuDatHang.Select(e => e.Value).GroupBy(e => e.ToString()))
                    {
                        var lstPhieuDatHang_KH = lstPhieuDatHang.Where(e => e.Value == itm.Key.ToString()).Select(e => e.Key);
                        var lstPhieuDatHang_ChiTiet_KH = lstPhieuDatHang_ChiTiet.Where(e => lstPhieuDatHang_KH.Contains(e.ID_PHIEUDATHANG)).ToList();
                        ct_PhieuXuat newct_PhieuXuat = new ct_PhieuXuat();
                        Max_ID += 1;
                        newct_PhieuXuat.ID = Guid.NewGuid().ToString();
                        newct_PhieuXuat.LOC_ID = LOC_ID;
                        newct_PhieuXuat.ID_LOAIPHIEUXUAT = (dm_LoaiPhieuXuat != null ? dm_LoaiPhieuXuat.ID : "");
                        newct_PhieuXuat.NGAYLAP = NGAYLAP;
                        newct_PhieuXuat.SOPHIEU = Max_ID;
                        
                       
                        newct_PhieuXuat.ID_KHACHHANG = itm.Key;
                        newct_PhieuXuat.ID_KHO = ID_KHO;
                        newct_PhieuXuat.TONGTIENGIAMGIA = Math.Round(lstPhieuDatHang_ChiTiet_KH.Sum(s => s.TONGTIENGIAMGIA), 0);
                        newct_PhieuXuat.TONGTHANHTIEN = Math.Round(lstPhieuDatHang_ChiTiet_KH.Sum(s => s.THANHTIEN), 0);
                        newct_PhieuXuat.TONGTIENVAT = Math.Round(lstPhieuDatHang_ChiTiet_KH.Sum(s => s.TONGTIENVAT), 0);
                        newct_PhieuXuat.TONGTIEN = Math.Round(lstPhieuDatHang_ChiTiet_KH.Sum(s => s.TONGCONG), 0);
                        newct_PhieuXuat.ID_NGUOITAO = ID_NGUOITAO;
                        newct_PhieuXuat.THOIGIANTHEM = DateTime.Now;
                        newct_PhieuXuat.ISKHUYENMAI = true;
                        newct_PhieuXuat.ISPHIEUDIEUHANG = true;
                        lstPhieuDatHang_ChiTiet_KH = (from s in lstPhieuDatHang_ChiTiet_KH
                                                      orderby s.ID_PHIEUDATHANG, s.STT, s.ISKHUYENMAI
                                                      select s).ToList();
                        int STT = 0;
                        string ID_PHIEUDATHANG = "";
                        int STT_PHIEUDATHANG = 0;
                        foreach (var ct in lstPhieuDatHang_ChiTiet_KH)
                        {
                            ct_PhieuXuat_ChiTiet newct_PhieuXuat_CT = new ct_PhieuXuat_ChiTiet();
                            newct_PhieuXuat_CT = ConvertobjectToct_PhieuXuat_ChiTiet(ct, newct_PhieuXuat_CT);
                            newct_PhieuXuat_CT.ID_PHIEUXUAT = newct_PhieuXuat.ID;
                            newct_PhieuXuat_CT.ID_PHIEUDIEUHANG_CHITIET = ct.ID;
                            if (string.IsNullOrEmpty(ID_PHIEUDATHANG) || (ct.ID_PHIEUDATHANG != ID_PHIEUDATHANG) || (ct.ID_PHIEUDATHANG == ID_PHIEUDATHANG && ct.STT != STT_PHIEUDATHANG))
                            {
                                STT += 1;
                                STT_PHIEUDATHANG = ct.STT;
                                ID_PHIEUDATHANG = ct.ID_PHIEUDATHANG;
                            }
                            newct_PhieuXuat_CT.STT = STT;
                            _context.ct_PhieuXuat_ChiTiet!.Add(newct_PhieuXuat_CT);
                        }

                        foreach (var value in lstPhieuDatHang_KH)
                        {
                            var PhieuDatHang = await _context.ct_PhieuDatHang!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == value);
                            if (PhieuDatHang != null)
                            {
                                newct_PhieuXuat.GHICHU = (string.IsNullOrEmpty(newct_PhieuXuat.GHICHU) ? "" : newct_PhieuXuat.GHICHU + ",") + PhieuDatHang.MAPHIEU;
                                PhieuDatHang.ID_PHIEUXUAT = newct_PhieuXuat.ID;
                                //newct_PhieuXuat.TONGTIENTINHTHUE = PhieuDatHang.TONGTIENTINHTHUE;
                                _context.Entry(PhieuDatHang).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            }
                        }

                        bool bolCheckMA = false;
                        while (!bolCheckMA)
                        {
                            newct_PhieuXuat.MAPHIEU = API.GetMaPhieu(API.ct_PhieuXuat, newct_PhieuXuat.NGAYLAP, newct_PhieuXuat.SOPHIEU);
                            var check = _context.ct_PhieuXuat!.Where(e => e.LOC_ID == LOC_ID && e.MAPHIEU == newct_PhieuXuat.MAPHIEU).FirstOrDefault();
                            if(check != null)
                            {
                                Max_ID += 1;
                                newct_PhieuXuat.SOPHIEU = Max_ID;
                            }    
                            else
                            {
                                bolCheckMA = true;
                            }    
                        }
                        _context.ct_PhieuXuat!.Add(newct_PhieuXuat);
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

        private static ct_PhieuXuat_ChiTiet ConvertobjectToct_PhieuXuat_ChiTiet<T>(T objectFrom, ct_PhieuXuat_ChiTiet objectTo)
        {
            if (objectFrom != null)
            {
                var properties = objectFrom.GetType().GetProperties();
                foreach (PropertyInfo itmPropertyInfo in properties)
                {
                    if (itmPropertyInfo != null)
                    {
                        var val = itmPropertyInfo.GetValue(objectFrom);
                        if (val != null)
                        {
                            var piShared = objectTo.GetType().GetProperty(itmPropertyInfo.Name);
                            if (piShared != null)
                                piShared.SetValue(objectTo, val);
                        }
                    }
                }
            }

            return objectTo;
        }

        private static ct_PhieuDatHang_ChiTiet ConvertobjectToct_PhieuDatHang_ChiTiet<T>(T objectFrom, ct_PhieuDatHang_ChiTiet objectTo)
        {
            if (objectFrom != null)
            {
                var properties = objectFrom.GetType().GetProperties();
                foreach (PropertyInfo itmPropertyInfo in properties)
                {
                    if (itmPropertyInfo != null)
                    {
                        var val = itmPropertyInfo.GetValue(objectFrom);
                        if (val != null)
                        {
                            var piShared = objectTo.GetType().GetProperty(itmPropertyInfo.Name);
                            if (piShared != null)
                                piShared.SetValue(objectTo, val);
                        }
                    }
                }
            }

            return objectTo;
        }
        // DELETE: api/Deposit/5
        [HttpDelete("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeleteDeposit(string LOC_ID, string ID)
        {
            try
            {
                var Deposit = await _context.ct_PhieuDatHang!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (Deposit == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                else
                {
                    if (!string.IsNullOrEmpty(Deposit.ID_PHIEUXUAT))
                    {
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Phiếu đặt hàng " + Deposit.MAPHIEU + " đã được tạo phiếu xuất!",
                            Data = ""
                        });
                    }
                }

                using var transaction = _context.Database.BeginTransaction();
                {
                    var lstPhieuNhap_ChiTiet = await _context.ct_PhieuDatHang_ChiTiet!.Where(e => e.LOC_ID == Deposit.LOC_ID && e.ID_PHIEUDATHANG == Deposit.ID).ToListAsync();
                    if (lstPhieuNhap_ChiTiet != null)
                    {
                        foreach (ct_PhieuDatHang_ChiTiet itm in lstPhieuNhap_ChiTiet)
                        {
                            var objdm_HangHoa_Kho = await _context.dm_HangHoa_Kho!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO);
                            if (objdm_HangHoa_Kho != null)
                            {
                                itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
                                objdm_HangHoa_Kho.QTY += itm.TONGSOLUONG;
                                _context.Entry(objdm_HangHoa_Kho).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            }
                            else
                            {
                                return Ok(new ApiResponse
                                {
                                    Success = false,
                                    Message = "Không tìm thấy sản phẩm kho!" + itm.ID_HANGHOAKHO,
                                    Data = ""
                                });
                            }
                            _context.ct_PhieuDatHang_ChiTiet!.Remove(itm);
                        }
                    }

                    _context.ct_PhieuDatHang!.Remove(Deposit);
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

        private bool DepositExistsID(string LOC_ID, string ID)
        {
            //bool bolCheckMA = false;
            //while (!bolCheckMA)
            //{
            //    using var transaction = _context.Database.BeginTransaction();
            //    {
            //        var check = _context.AspNetRequest!.Where(e => e.LOC_ID == LOC_ID && e.NAME == strTable).OrderByDescending(e => e.THOIGIAN).FirstOrDefault();
            //        if (check != null)
            //        {
            //            if (check.THOIGIAN < DateTime.Now.AddSeconds(-5))
            //            {
            //                _context.AspNetRequest!.Remove(check);
            //                _context.SaveChanges();
            //            }
            //        }
            //        else
            //        {
            //            AspNetRequest newAspNetRequest = new AspNetRequest();
            //            newAspNetRequest.ID = ID;
            //            newAspNetRequest.NAME = strTable;
            //            newAspNetRequest.THOIGIAN = DateTime.Now;
            //            newAspNetRequest.LOC_ID = LOC_ID;
            //            _context.AspNetRequest!.Add(newAspNetRequest);
            //            AuditLogController auditLog = new AuditLogController(_context, _configuration); auditLog.InserAuditLog();
            //            _context.SaveChanges();
            //            check = _context.AspNetRequest!.Where(e => e.LOC_ID == LOC_ID && e.NAME == strTable).OrderByDescending(e => e.THOIGIAN).FirstOrDefault();
            //            if (check != null && check.ID == ID)
            //            {
            //                bolCheckMA = true;
            //            }
            //        }
            //    }
            //    transaction.Commit();
            //}
            return _context.ct_PhieuDatHang!.Any(e => e.LOC_ID == LOC_ID && e.ID == ID);
        }

        #region Chương trình khuyến mãi
        private async Task<IActionResult> Get_ChuongTrinhKhuyenMai(List<Product_Detail> lstProduct_Detail, string LOC_ID)
        {
            try
            {

                List<v_dm_ChuongTrinhKhuyenMai> lstdm_ChuongTrinhKhuyenMai = new List<v_dm_ChuongTrinhKhuyenMai>();
                SP_Parameter SP_Parameter = new SP_Parameter();
                SP_Parameter.LOC_ID = LOC_ID;
                SP_Parameter.TUNGAY = DateTime.Now.Date;
                SP_Parameter.DENNGAY = DateTime.Now.Date;
                ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
                var actionResult = await ExecuteStoredProc1.Sp_Get_ChuongTrinhKhuyenMai(SP_Parameter);
                var okResult = actionResult as OkObjectResult;
                if (okResult != null)
                {
                    var ApiResponse = okResult.Value as ApiResponse;
                    if (ApiResponse != null)
                    {
                        if (ApiResponse.Data != null)
                        {
                            var lst_ChiTiet = ApiResponse.Data as List<v_dm_ChuongTrinhKhuyenMai>;
                            if (lst_ChiTiet != null)
                            {
                                lstdm_ChuongTrinhKhuyenMai.AddRange(lst_ChiTiet);
                            }
                        }
                    }
                }

                var lstKhuyenMai = lstProduct_Detail.Where(e => e.ISKHUYENMAI == true).ToList();
                if (lstKhuyenMai != null && lstKhuyenMai.Count() > 0)
                {
                    foreach (var itm in lstKhuyenMai)
                    {
                        lstProduct_Detail.Remove(itm);
                    }
                }

                var lstKhuyenMai1 = lstProduct_Detail.Where(e => !string.IsNullOrEmpty(e.ID_KHUYENMAI));
                if (lstKhuyenMai1 != null && lstKhuyenMai1.Count() > 0)
                {
                    foreach (var itm in lstKhuyenMai1)
                    {
                        dm_ThueSuat clsdm_ThueSuat = new dm_ThueSuat();
                        if (!string.IsNullOrEmpty(itm.ID_THUESUAT))
                            clsdm_ThueSuat = await _context.dm_ThueSuat!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_THUESUAT) ?? new dm_ThueSuat();

                        itm.CHIETKHAU = 0;
                        itm.TYPE = "CHIETKHAU";
                        API.TinhTong(itm, "", lstProduct_Detail, clsdm_ThueSuat);

                        itm.TONGTIENGIAMGIA = 0;
                        itm.TYPE = "TONGTIENGIAMGIA";
                        API.TinhTong(itm, "", lstProduct_Detail, clsdm_ThueSuat);
                        itm.ISDALAYKHUYENMAI = false;
                        itm.ID_KHUYENMAI = "";
                    }
                }
                double SOTIENTHUE_KM = 0;
                List<string> lstDanhSachDaLayKhuyenMai = new List<string>();
                List<Product_Detail> lstProduct_Detail_Tam = new List<Product_Detail>();

                foreach (v_dm_ChuongTrinhKhuyenMai itm in lstdm_ChuongTrinhKhuyenMai)
                {
                    if (itm.MA.StartsWith("48"))
                    {
                        //string s = "";
                    }
                    bool bolConSoLuong = false;
                    string input = itm.MA;
                    int lastIndex = input.LastIndexOf('_');

                    if (lastIndex != -1) // Kiểm tra xem có dấu gạch nào không
                    {
                        string result = input.Substring(0, lastIndex);
                        if (lstDanhSachDaLayKhuyenMai.Where(e => e.StartsWith(result)).Count() > 0)
                        {
                            if (lstProduct_Detail_Tam.Where(s => (s.SOLUONG - s.SOLUONGDALAY_KM) > 0).Count() > 0)
                                bolConSoLuong = true;
                            else
                                continue;
                        }
                        else
                        {
                            lstDanhSachDaLayKhuyenMai = new List<string>();
                            lstProduct_Detail_Tam = lstProduct_Detail.ToList();
                            bolConSoLuong = true;
                        }
                    }
                    else
                    {
                        lstDanhSachDaLayKhuyenMai = new List<string>();
                        lstProduct_Detail_Tam = lstProduct_Detail.ToList();
                    }
                    //string MA = itm.MA.Substring(0, itm.MA.Length - 1);


                    int intCoLayKhuyenMai = 0;
                    var lstChuongTrinhKhuyenMai_YeuCau = await _context.dm_ChuongTrinhKhuyenMai_YeuCau!.Where(e => e.LOC_ID == LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == itm.ID).ToListAsync();

                    if (itm.IS_YEUCAUCHITIET)
                    {
                        #region Chương trình khuyến mãi theo số lượng chi tiết
                        if (lstChuongTrinhKhuyenMai_YeuCau != null && lstChuongTrinhKhuyenMai_YeuCau.Count > 0)
                        {
                            int PhanNguyen = 0;
                            bool bolThoatWhile = false;
                            List<Product_Detail> lstSelectProduct_Detail = new List<Product_Detail>();
                            List<Product_Detail> lstSelectProduct_Detail_HT = new List<Product_Detail>();
                            List<Product_Detail> lstSelectProduct_Detail_Old = new List<Product_Detail>();
                            while (bolThoatWhile != true)
                            {
                                PhanNguyen += 1;
                                foreach (var ChiTiet in lstChuongTrinhKhuyenMai_YeuCau)
                                {
                                    var getlst = lstProduct_Detail.Where(e => (!itm.ISTONGHOADON || (itm.ISTONGHOADON && !e.ISDALAYKHUYENMAI)) && ((e.ID_HANGHOA == ChiTiet.ID_HANGHOA && ChiTiet.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham) || (e.ID_NHOMHANGHOA == ChiTiet.ID_HANGHOA && ChiTiet.HINHTHUC == (int)API.HinhThucKhuyenMai.NhomSanPham)) && e.ID_DVT == ChiTiet.ID_DVT).ToList();
                                    var CTKM_YC = getlst.Sum(e => (ChiTiet.SOLUONG > 0 ? e.SOLUONG : e.THANHTIEN));
                                    if (CTKM_YC >= ((ChiTiet.SOLUONG > 0 ? ChiTiet.SOLUONG : ChiTiet.SOTIEN) * PhanNguyen))
                                    {
                                        if (PhanNguyen == 1)
                                            lstSelectProduct_Detail_HT.AddRange(getlst);

                                        if (itm.IS_YEUCAUCHITIET && ChiTiet.SOLUONG == 0 && ChiTiet.SOTIEN == 0)
                                        {
                                            bolThoatWhile = true;
                                            PhanNguyen = 0;
                                        }
                                    }
                                    else
                                    {
                                        PhanNguyen -= 1;
                                        bolThoatWhile = true;
                                        lstSelectProduct_Detail = lstSelectProduct_Detail_Old.ToList();
                                    }
                                }
                                if (!itm.ISTINHLUYTUYEN)
                                    bolThoatWhile = true;

                                if (!bolThoatWhile)
                                {
                                    lstSelectProduct_Detail_Old = lstSelectProduct_Detail_HT.ToList();
                                }
                                else
                                {
                                    lstSelectProduct_Detail = lstSelectProduct_Detail_HT.ToList();
                                }

                            }
                            if (PhanNguyen > 0)
                            {
                                foreach (var ChiTiet in lstChuongTrinhKhuyenMai_YeuCau)
                                {
                                    double TONGTIENGIAMGIA = 0;
                                    var getlst = lstProduct_Detail.Where(e => ((e.ID_HANGHOA == ChiTiet.ID_HANGHOA && ChiTiet.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham) || (e.ID_NHOMHANGHOA == ChiTiet.ID_HANGHOA && ChiTiet.HINHTHUC == (int)API.HinhThucKhuyenMai.NhomSanPham)) && e.ID_DVT == ChiTiet.ID_DVT).ToList();
                                    foreach (var ChiTietHoaDon in getlst)
                                    {
                                        dm_ThueSuat clsdm_ThueSuat = new dm_ThueSuat();
                                        if (!string.IsNullOrEmpty(ChiTietHoaDon.ID_THUESUAT))
                                            clsdm_ThueSuat = await _context.dm_ThueSuat!.FirstOrDefaultAsync(e => e.LOC_ID == ChiTietHoaDon.LOC_ID && e.ID == ChiTietHoaDon.ID_THUESUAT) ?? new dm_ThueSuat();
                                        if (ChiTietHoaDon.CHIETKHAU < ChiTiet.CHIETKHAU)
                                        {
                                            TONGTIENGIAMGIA = (ChiTietHoaDon.SOLUONG * ChiTietHoaDon.DONGIA) * ChiTiet.CHIETKHAU / 100;
                                            if (TONGTIENGIAMGIA > ChiTietHoaDon.TONGTIENGIAMGIA)
                                            {
                                                ChiTietHoaDon.CHIETKHAU = ChiTiet.CHIETKHAU;
                                                ChiTietHoaDon.TYPE = "CHIETKHAU";
                                                API.TinhTong(ChiTietHoaDon, "", lstProduct_Detail, clsdm_ThueSuat);
                                            }
                                        }
                                        TONGTIENGIAMGIA = ChiTiet.TIENGIAM * (itm.ISTINHLUYTUYEN ? PhanNguyen : 1);
                                        if (ChiTietHoaDon.TONGTIENGIAMGIA < TONGTIENGIAMGIA)
                                        {
                                            ChiTietHoaDon.TONGTIENGIAMGIA = TONGTIENGIAMGIA;
                                            ChiTietHoaDon.TYPE = "TONGTIENGIAMGIA";
                                            API.TinhTong(ChiTietHoaDon, "", lstProduct_Detail, clsdm_ThueSuat);
                                        }


                                        ChiTietHoaDon.ISDALAYKHUYENMAI = false;
                                        ChiTietHoaDon.ID_KHUYENMAI = ChiTiet.ID_CHUONGTRINHKHUYENMAI;

                                        var objHangHoa = await _context.view_dm_HangHoa!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ChiTietHoaDon.ID_HANGHOA);
                                        if (objHangHoa != null && objHangHoa.MUCTHUE != 0)
                                            SOTIENTHUE_KM += (ChiTietHoaDon.TONGTIENGIAMGIA * objHangHoa.MUCTHUE / 100);
                                    }
                                    intCoLayKhuyenMai += 1;
                                }
                            }


                            var lstdm_ChuongTrinhKhuyenMai_Tang = await _context.dm_ChuongTrinhKhuyenMai_Tang!.Where(e => e.LOC_ID == LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == itm.ID).ToListAsync();

                            #region Khuyến mãi tặng hàng hóa
                            if (PhanNguyen > 0 && lstdm_ChuongTrinhKhuyenMai_Tang != null && lstdm_ChuongTrinhKhuyenMai_Tang.Count > 0)
                            {
                                var ID_KHO = lstProduct_Detail.Select(e => e.ID_KHO).FirstOrDefault();

                                if (lstdm_ChuongTrinhKhuyenMai_Tang != null)
                                {
                                    foreach (var CTKM_Tang in lstdm_ChuongTrinhKhuyenMai_Tang)
                                    {
                                        var HangHoaKho = await _context.dm_HangHoa_Kho!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && e.ID_HANGHOA == CTKM_Tang.ID_HANGHOA && e.ID_KHO == ID_KHO);
                                        var HangHoa = await _context.view_dm_HangHoa!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && e.ID == CTKM_Tang.ID_HANGHOA);
                                        if (HangHoaKho != null && HangHoa != null)
                                        {
                                            Product_Detail newProduct_Detail = new Product_Detail();
                                            newProduct_Detail.STT = lstSelectProduct_Detail.Max(e => e.STT);
                                            newProduct_Detail.ID = Guid.NewGuid().ToString();
                                            newProduct_Detail.NAME = HangHoa.NAME;
                                            newProduct_Detail.MA = HangHoa.MA;
                                            newProduct_Detail.ID_HANGHOA = CTKM_Tang.ID_HANGHOA;
                                            newProduct_Detail.ID_HANGHOAKHO = HangHoaKho.ID;
                                            newProduct_Detail.DONGIA = 0;
                                            newProduct_Detail.ID_DVT = CTKM_Tang.ID_DVT;
                                            newProduct_Detail.SOLUONG = PhanNguyen * CTKM_Tang.SOLUONG;
                                            newProduct_Detail.CHIETKHAU = 0;
                                            newProduct_Detail.TONGTIENGIAMGIA = 0;
                                            newProduct_Detail.THANHTIEN = 0;
                                            newProduct_Detail.THUESUAT = 0;
                                            newProduct_Detail.TONGTIENVAT = 0;
                                            newProduct_Detail.TONGCONG = 0;
                                            if (CTKM_Tang.SOTIEN > 0)
                                            {
                                                newProduct_Detail.TONGTIENGIAMGIA = CTKM_Tang.SOTIEN * PhanNguyen;
                                                newProduct_Detail.TYPE = "TONGTIENGIAMGIA";
                                                dm_ThueSuat clsdm_ThueSuat = new dm_ThueSuat();
                                                if (!string.IsNullOrEmpty(HangHoa.ID_THUESUAT))
                                                    clsdm_ThueSuat = await _context.dm_ThueSuat!.FirstOrDefaultAsync(e => e.LOC_ID == HangHoa.LOC_ID && e.ID == HangHoa.ID_THUESUAT) ?? new dm_ThueSuat();

                                                API.TinhTong(newProduct_Detail, "", lstProduct_Detail, clsdm_ThueSuat);
                                                if (HangHoa != null && HangHoa.MUCTHUE != 0)
                                                    SOTIENTHUE_KM += (newProduct_Detail.TONGTIENGIAMGIA * HangHoa.MUCTHUE / 100);
                                            }
                                            newProduct_Detail.ID_KHO = ID_KHO;
                                            //newProduct_Detail.ISDALAYKHUYENMAI = true;
                                            newProduct_Detail.ISKHUYENMAI = true;
                                            newProduct_Detail.ID_KHUYENMAI = itm.ID;
                                            if (HangHoa != null && HangHoa.ID_DVT == newProduct_Detail.ID_DVT)
                                            {
                                                newProduct_Detail.NAME_DVT = HangHoa.NAME_DVT;
                                                if (!string.IsNullOrEmpty(HangHoa.ID_DVT_QD))
                                                {
                                                    newProduct_Detail.TYLE_QD = HangHoa.TYLE_QD;
                                                }
                                                else
                                                {
                                                    if (HangHoa.LOAIHANGHOA == ((int)API.LoaiSanPham.KhongQuanLyTonKho).ToString())
                                                        newProduct_Detail.TYLE_QD = 0;
                                                    else
                                                        newProduct_Detail.TYLE_QD = 1;
                                                }
                                                if (newProduct_Detail.SOLUONG != 0)
                                                {
                                                    if (HangHoa != null && HangHoa.MUCTHUE != 0)
                                                        SOTIENTHUE_KM += (newProduct_Detail.SOLUONG * newProduct_Detail.TYLE_QD * HangHoa.GIA01 * HangHoa.MUCTHUE / 100);
                                                }
                                            }
                                            else if (HangHoa != null && HangHoa.ID_DVT_QD == newProduct_Detail.ID_DVT)
                                            {
                                                if (!string.IsNullOrEmpty(HangHoa.ID_DVT_QD))
                                                {
                                                    newProduct_Detail.NAME_DVT = HangHoa.NAME_DVT_QD;
                                                    newProduct_Detail.TYLE_QD = 1;
                                                }
                                                if (newProduct_Detail.SOLUONG != 0)
                                                {
                                                    if (HangHoa != null && HangHoa.MUCTHUE != 0)
                                                        SOTIENTHUE_KM += (newProduct_Detail.SOLUONG * newProduct_Detail.TYLE_QD * HangHoa.GIA01_QD * HangHoa.MUCTHUE / 100);
                                                }
                                            }
                                            else
                                            {
                                                return Ok(new ApiResponse
                                                {
                                                    Success = false,
                                                    Message = "Không tìm thấy thông tin sản phẩm với đơn vị tính " + newProduct_Detail.ID_DVT + " Kiểm tra CTKM" + itm.NAME,
                                                    Data = null
                                                });

                                            }
                                            newProduct_Detail.TONGSOLUONG = newProduct_Detail.TYLE_QD * newProduct_Detail.SOLUONG;


                                            #region Sản phẩm tặng là Combo
                                            if (HangHoa != null && HangHoa.LOAIHANGHOA == ((int)API.LoaiSanPham.Combo).ToString())
                                            {
                                                newProduct_Detail.ID_KHUYENMAI = newProduct_Detail.ID_HANGHOA;
                                                SP_Parameter objParameter = new SP_Parameter();
                                                objParameter.LOC_ID = itm.LOC_ID;
                                                objParameter.ID_KHO = newProduct_Detail.ID_KHO;
                                                objParameter.ID_COMBO = newProduct_Detail.ID_HANGHOA;
                                                ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
                                                actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachSanPhamKho_Combo(objParameter);
                                                okResult = actionResult as OkObjectResult;
                                                if (okResult != null)
                                                {
                                                    var ApiResponse = okResult.Value as ApiResponse;
                                                    if (ApiResponse != null)
                                                    {
                                                        if (ApiResponse.Data != null)
                                                        {
                                                            var lst_ChiTiet = ApiResponse.Data as List<Product_Detail>;
                                                            if (lst_ChiTiet != null)
                                                            {
                                                                foreach (var ChiTiet in lst_ChiTiet)
                                                                {
                                                                    ChiTiet.STT = newProduct_Detail.STT;
                                                                    ChiTiet.ID = Guid.NewGuid().ToString();
                                                                    ChiTiet.ID_DVT = ChiTiet.ID_DVT_COMBO;
                                                                    ChiTiet.SOLUONG = newProduct_Detail.SOLUONG * ChiTiet.QTY_COMBO;
                                                                    ChiTiet.TYLE_QD = ChiTiet.TYLE_QD_COMBO;
                                                                    ChiTiet.TONGSOLUONG = newProduct_Detail.SOLUONG * ChiTiet.QTY_TOTAL_COMBO;
                                                                    ChiTiet.DONGIA = 0;
                                                                    ChiTiet.CHIETKHAU = 0;
                                                                    ChiTiet.TONGTIENGIAMGIA = 0;
                                                                    ChiTiet.THANHTIEN = 0;
                                                                    ChiTiet.THUESUAT = 0;
                                                                    ChiTiet.TONGTIENVAT = 0;
                                                                    ChiTiet.TONGCONG = 0;
                                                                    ChiTiet.ISKHUYENMAI = true;
                                                                    ChiTiet.ID_KHUYENMAI = itm.ID;
                                                                    ChiTiet.ISCOMBO = true;
                                                                    ChiTiet.ID_COMBO = newProduct_Detail.ID_HANGHOA;
                                                                    lstProduct_Detail.Add(ChiTiet);
                                                                }

                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            #endregion

                                            lstProduct_Detail.Add(newProduct_Detail);
                                        }
                                    }
                                    intCoLayKhuyenMai += 1;
                                }
                            }
                            #endregion

                            #region Chiết khấu tiền, giảm tiền
                            //bool bolTinhThue = false;
                            if (PhanNguyen > 0 && (itm.CHIETKHAU > 0 || itm.TIENGIAM > 0))
                            {
                                lstSelectProduct_Detail = lstSelectProduct_Detail.Where(e => !e.ISKHUYENMAI).ToList();
                                Dictionary<string, int> lstCTKM_YC = new Dictionary<string, int>();
                                if (lstChuongTrinhKhuyenMai_YeuCau != null && lstChuongTrinhKhuyenMai_YeuCau.Count > 0)
                                {
                                    foreach (var CTKM_YC in lstChuongTrinhKhuyenMai_YeuCau)
                                    {
                                        lstCTKM_YC.Add(CTKM_YC.ID_HANGHOA, CTKM_YC.HINHTHUC);
                                        //var HangHoa = await _context.view_dm_HangHoa!.FirstOrDefaultAsync(e => e.LOC_ID == CTKM_YC.LOC_ID && ((e.ID == CTKM_YC.ID_HANGHOA && CTKM_YC.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham) || (e.ID_NHOMHANGHOA == CTKM_YC.ID_HANGHOA && CTKM_YC.HINHTHUC == (int)API.HinhThucKhuyenMai.NhomSanPham)));
                                        //if (HangHoa != null && HangHoa.MUCTHUE != 0)
                                        //    bolTinhThue = true;
                                    }

                                }

                                if (lstSelectProduct_Detail != null && lstSelectProduct_Detail.Count > 0)
                                {
                                    double SumTien = 0;
                                    double SumSoLuong = 0;
                                    if (lstChuongTrinhKhuyenMai_YeuCau != null && lstChuongTrinhKhuyenMai_YeuCau.Count > 0)
                                    {
                                        SumTien = lstSelectProduct_Detail.Where(e => !e.ISDALAYKHUYENMAI && lstCTKM_YC.Count(s => s.Key.Contains(e.ID_HANGHOA) && s.Value == (int)API.HinhThucKhuyenMai.SanPham) > 0 || lstCTKM_YC.Count(s => s.Key.Contains(e.ID_NHOMHANGHOA) && s.Value == (int)API.HinhThucKhuyenMai.NhomSanPham) > 0).Sum(s => Convert.ToDouble(s.THANHTIEN));
                                        if (!string.IsNullOrEmpty(itm.ID_DVT_DATKM))
                                            SumSoLuong = lstSelectProduct_Detail.Where(e => !e.ISDALAYKHUYENMAI && (lstCTKM_YC.Count(s => s.Key.Contains(e.ID_HANGHOA) && s.Value == (int)API.HinhThucKhuyenMai.SanPham) > 0 || lstCTKM_YC.Count(s => s.Key.Contains(e.ID_NHOMHANGHOA) && s.Value == (int)API.HinhThucKhuyenMai.NhomSanPham) > 0) && e.ID_DVT == itm.ID_DVT_DATKM).Sum(s => Convert.ToDouble(s.SOLUONG));
                                    }
                                    else
                                    {
                                        SumTien = lstSelectProduct_Detail.Where(e => !e.ISDALAYKHUYENMAI && !e.ISKHUYENMAI).Sum(s => Convert.ToDouble(s.TONGCONG));
                                        if (!string.IsNullOrEmpty(itm.ID_DVT_DATKM))
                                            SumSoLuong = lstSelectProduct_Detail.Where(e => !e.ISDALAYKHUYENMAI && e.ID_DVT == itm.ID_DVT_DATKM).Sum(s => Convert.ToDouble(s.TONGSOLUONG));
                                    }
                                    if (((itm.SOLUONG_DATKM != 0 || itm.SOLUONG_DATKM_DEN != 0) && SumSoLuong != 0 && itm.SOLUONG_DATKM <= SumSoLuong && (itm.SOLUONG_DATKM_DEN == 0 || itm.SOLUONG_DATKM_DEN >= SumSoLuong)) || ((itm.TONGTIEN_DATKM != 0 || itm.TONGTIEN_DATKM_DEN != 0) && SumTien != 0 && itm.TONGTIEN_DATKM <= SumTien && (itm.TONGTIEN_DATKM_DEN == 0 || itm.TONGTIEN_DATKM_DEN >= SumTien)))
                                    {
                                        if (!itm.ISTONGHOADON)
                                        {
                                            foreach (var ChiTiet in lstSelectProduct_Detail)
                                            {
                                                dm_ThueSuat clsdm_ThueSuat = new dm_ThueSuat();
                                                if (!string.IsNullOrEmpty(ChiTiet.ID_THUESUAT))
                                                    clsdm_ThueSuat = await _context.dm_ThueSuat!.FirstOrDefaultAsync(e => e.LOC_ID == ChiTiet.LOC_ID && e.ID == ChiTiet.ID_THUESUAT) ?? new dm_ThueSuat();

                                                if (itm.CHIETKHAU > 0)
                                                {
                                                    ChiTiet.CHIETKHAU = itm.CHIETKHAU;
                                                    ChiTiet.TYPE = "CHIETKHAU";
                                                    API.TinhTong(ChiTiet, "", lstProduct_Detail, clsdm_ThueSuat);
                                                    //ChiTiet.ISDALAYKHUYENMAI = true;
                                                    ChiTiet.ID_KHUYENMAI = itm.ID;
                                                    var objHangHoa = await _context.view_dm_HangHoa!.FirstOrDefaultAsync(e => e.LOC_ID == ChiTiet.LOC_ID && e.ID == ChiTiet.ID_HANGHOA);
                                                    if (objHangHoa != null && objHangHoa.MUCTHUE != 0)
                                                        SOTIENTHUE_KM += (ChiTiet.TONGTIENGIAMGIA * objHangHoa.MUCTHUE / 100);

                                                }
                                                else if (itm.TIENGIAM > 0)
                                                {
                                                    ChiTiet.TONGTIENGIAMGIA = itm.TIENGIAM;
                                                    ChiTiet.TYPE = "TONGTIENGIAMGIA";
                                                    API.TinhTong(ChiTiet, "", lstProduct_Detail, clsdm_ThueSuat);
                                                    //ChiTiet.ISDALAYKHUYENMAI = true;
                                                    ChiTiet.ID_KHUYENMAI = itm.ID;
                                                    var objHangHoa = await _context.view_dm_HangHoa!.FirstOrDefaultAsync(e => e.LOC_ID == ChiTiet.LOC_ID && e.ID == ChiTiet.ID_HANGHOA);
                                                    if (objHangHoa != null && objHangHoa.MUCTHUE != 0)
                                                        SOTIENTHUE_KM += (ChiTiet.TONGTIENGIAMGIA * objHangHoa.MUCTHUE / 100);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var ID_KHO = lstProduct_Detail.Select(e => e.ID_KHO).FirstOrDefault();
                                            var HangHoa = await _context.view_dm_HangHoa!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && e.MA == API.GTBH);
                                            if (HangHoa != null)
                                            {
                                                var HangHoaKho = await _context.dm_HangHoa_Kho!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && e.ID_HANGHOA == HangHoa.ID && e.ID_KHO == ID_KHO);
                                                if (HangHoaKho != null)
                                                {
                                                    Product_Detail newProduct_Detail = new Product_Detail();
                                                    newProduct_Detail.STT = lstSelectProduct_Detail.Max(e => e.STT);
                                                    newProduct_Detail.ID = Guid.NewGuid().ToString();
                                                    newProduct_Detail.NAME = HangHoa.NAME;
                                                    newProduct_Detail.MA = HangHoa.MA;
                                                    newProduct_Detail.ID_HANGHOA = HangHoaKho.ID_HANGHOA;
                                                    newProduct_Detail.ID_HANGHOAKHO = HangHoaKho.ID;
                                                    newProduct_Detail.DONGIA = 0;
                                                    newProduct_Detail.ID_DVT = HangHoa.ID_DVT;
                                                    newProduct_Detail.NAME_DVT = HangHoa.NAME_DVT;
                                                    newProduct_Detail.SOLUONG = 0;
                                                    dm_ThueSuat clsdm_ThueSuat = new dm_ThueSuat();
                                                    if (!string.IsNullOrEmpty(HangHoa.ID_THUESUAT))
                                                        clsdm_ThueSuat = await _context.dm_ThueSuat!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && e.ID == HangHoa.ID_THUESUAT) ?? new dm_ThueSuat();

                                                    if (itm.CHIETKHAU > 0)
                                                    {
                                                        newProduct_Detail.CHIETKHAU = itm.CHIETKHAU;
                                                        newProduct_Detail.TONGTIENGIAMGIA = lstSelectProduct_Detail.Where(e => !e.ISKHUYENMAI).Sum(e => e.THANHTIEN) * newProduct_Detail.CHIETKHAU / 100;
                                                        newProduct_Detail.TYPE = "TONGTIENGIAMGIA";
                                                        API.TinhTong(newProduct_Detail, "", lstProduct_Detail, clsdm_ThueSuat);
                                                        newProduct_Detail.ISDALAYKHUYENMAI = true;
                                                        newProduct_Detail.SOLUONGDALAYKHUYENMAI = newProduct_Detail.TONGSOLUONG;
                                                        newProduct_Detail.ID_KHUYENMAI = itm.ID;
                                                        foreach (var s in lstSelectProduct_Detail)
                                                        {
                                                            var objHangHoa = await _context.view_dm_HangHoa!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && (e.ID == s.ID || e.ID_NHOMHANGHOA == s.ID));
                                                            if (objHangHoa != null && objHangHoa.MUCTHUE != 0)
                                                            {
                                                                SOTIENTHUE_KM += (newProduct_Detail.TONGTIENGIAMGIA * objHangHoa.MUCTHUE / 100);
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    else if (itm.TIENGIAM > 0)
                                                    {
                                                        newProduct_Detail.TONGTIENGIAMGIA = itm.TIENGIAM * PhanNguyen;
                                                        newProduct_Detail.TYPE = "TONGTIENGIAMGIA";
                                                        API.TinhTong(newProduct_Detail, "", lstProduct_Detail, clsdm_ThueSuat);
                                                        newProduct_Detail.ISDALAYKHUYENMAI = true;
                                                        newProduct_Detail.SOLUONGDALAYKHUYENMAI = newProduct_Detail.TONGSOLUONG;
                                                        newProduct_Detail.ID_KHUYENMAI = itm.ID;
                                                        foreach (var s in lstSelectProduct_Detail)
                                                        {
                                                            var objHangHoa = await _context.view_dm_HangHoa!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && (e.ID == s.ID || e.ID_NHOMHANGHOA == s.ID));
                                                            if (objHangHoa != null && objHangHoa.MUCTHUE != 0)
                                                            {
                                                                SOTIENTHUE_KM += (newProduct_Detail.TONGTIENGIAMGIA * objHangHoa.MUCTHUE / 100);
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    newProduct_Detail.ID_KHO = ID_KHO;
                                                    newProduct_Detail.ISDALAYKHUYENMAI = true;
                                                    newProduct_Detail.ISKHUYENMAI = true;
                                                    newProduct_Detail.ID_KHUYENMAI = itm.ID;
                                                    lstProduct_Detail.Add(newProduct_Detail);

                                                    foreach (var ChiTiet in lstSelectProduct_Detail)
                                                    {
                                                        ChiTiet.ISDALAYKHUYENMAI = true;
                                                    }
                                                }
                                            }
                                        }

                                        //foreach (var ChiTiet in lstSelectProduct_Detail)
                                        //{
                                        //    ChiTiet.ISDALAYKHUYENMAI = true;
                                        //}
                                        intCoLayKhuyenMai += 1;
                                    }
                                }



                            }
                            #endregion
                        }
                        #endregion
                    }
                    else
                    {
                        #region Chương trình khuyến mãi tổng
                        List<Product_Detail> lstSelectProduct_Detail = new List<Product_Detail>();
                        Dictionary<string, int> lstCTKM_YC = new Dictionary<string, int>();
                        double MUCTHUE = 0;
                        int PhanNguyen = 0;
                        bool bolBatBuoc = false;
                        if (lstChuongTrinhKhuyenMai_YeuCau != null && lstChuongTrinhKhuyenMai_YeuCau.Count > 0)
                        {
                            bool isOk = true;
                            foreach (var CTKM_YC in lstChuongTrinhKhuyenMai_YeuCau)
                            {
                                lstCTKM_YC.Add(CTKM_YC.ID_HANGHOA, CTKM_YC.HINHTHUC);
                                var objHangHoa = await _context.view_dm_HangHoa!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && (e.ID == CTKM_YC.ID_HANGHOA || e.ID_NHOMHANGHOA == CTKM_YC.ID_HANGHOA));
                                if (MUCTHUE == 0 && objHangHoa != null && objHangHoa.MUCTHUE != 0)
                                {
                                    MUCTHUE = objHangHoa.MUCTHUE;
                                }
                                if (CTKM_YC.ISBATBUOC)
                                {
                                    double SumSoLuong = 0;
                                    if (bolConSoLuong)
                                        SumSoLuong = lstProduct_Detail_Tam.Where(e => (!itm.ISTONGHOADON || (itm.ISTONGHOADON && !e.ISDALAYKHUYENMAI)) && ((e.ID_HANGHOA == CTKM_YC.ID_HANGHOA && CTKM_YC.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham) || (e.ID_NHOMHANGHOA == CTKM_YC.ID_HANGHOA && CTKM_YC.HINHTHUC == (int)API.HinhThucKhuyenMai.NhomSanPham)) && e.ID_DVT == CTKM_YC.ID_DVT).Sum(s => Convert.ToDouble(s.SOLUONG - s.SOLUONGDALAY_KM));
                                    else
                                        SumSoLuong = lstProduct_Detail.Where(e => (!itm.ISTONGHOADON || (itm.ISTONGHOADON && !e.ISDALAYKHUYENMAI)) && ((e.ID_HANGHOA == CTKM_YC.ID_HANGHOA && CTKM_YC.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham) || (e.ID_NHOMHANGHOA == CTKM_YC.ID_HANGHOA && CTKM_YC.HINHTHUC == (int)API.HinhThucKhuyenMai.NhomSanPham)) && e.ID_DVT == CTKM_YC.ID_DVT).Sum(s => Convert.ToDouble(s.SOLUONG));
                                    if (SumSoLuong < CTKM_YC.SOLUONG_BATBUOC)
                                    {
                                        isOk = false;
                                        break;
                                    }
                                }
                            }
                            if (!isOk)
                                continue;

                            if (itm.ISTINHLUYTUYEN)
                            {
                                bool bolThoatwhile = false;
                                var lstbatBuoc = lstChuongTrinhKhuyenMai_YeuCau.Where(e => e.ISBATBUOC);
                                if (lstbatBuoc != null && lstbatBuoc.Count() > 0)
                                {
                                    while (bolThoatwhile != true)
                                    {
                                        PhanNguyen += 1;
                                        foreach (var CTKM_YC in lstbatBuoc)
                                        {
                                            double SumSoLuong = 0;
                                            if (bolConSoLuong)
                                                SumSoLuong = lstProduct_Detail_Tam.Where(e => (!itm.ISTONGHOADON || (itm.ISTONGHOADON && !e.ISDALAYKHUYENMAI)) && ((e.ID_HANGHOA == CTKM_YC.ID_HANGHOA && CTKM_YC.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham) || (e.ID_NHOMHANGHOA == CTKM_YC.ID_HANGHOA && CTKM_YC.HINHTHUC == (int)API.HinhThucKhuyenMai.NhomSanPham)) && e.ID_DVT == CTKM_YC.ID_DVT).Sum(s => Convert.ToDouble(s.SOLUONG - s.SOLUONGDALAY_KM));
                                            else
                                                SumSoLuong = lstProduct_Detail.Where(e => (!itm.ISTONGHOADON || (itm.ISTONGHOADON && !e.ISDALAYKHUYENMAI)) && ((e.ID_HANGHOA == CTKM_YC.ID_HANGHOA && CTKM_YC.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham) || (e.ID_NHOMHANGHOA == CTKM_YC.ID_HANGHOA && CTKM_YC.HINHTHUC == (int)API.HinhThucKhuyenMai.NhomSanPham)) && e.ID_DVT == CTKM_YC.ID_DVT).Sum(s => Convert.ToDouble(s.SOLUONG));
                                            if (SumSoLuong < CTKM_YC.SOLUONG_BATBUOC * PhanNguyen)
                                            {
                                                PhanNguyen -= 1;
                                                bolThoatwhile = true;
                                                break;
                                            }
                                            else
                                            {
                                                bolBatBuoc = true;
                                            }
                                        }
                                    }
                                }
                            }

                            List<Product_Detail>? Tam = null;
                            if (bolConSoLuong)
                                Tam = lstProduct_Detail_Tam.Where(e => !string.IsNullOrEmpty(e.ID_HANGHOA) && !string.IsNullOrEmpty(e.ID_NHOMHANGHOA) && !e.ISKHUYENMAI && (e.SOLUONG - e.SOLUONGDALAY_KM > 0)).ToList();
                            else
                                Tam = lstProduct_Detail.Where(e => !string.IsNullOrEmpty(e.ID_HANGHOA) && !string.IsNullOrEmpty(e.ID_NHOMHANGHOA) && !e.ISKHUYENMAI).ToList();
                            if (Tam != null)
                                lstSelectProduct_Detail = Tam.Where(e => lstCTKM_YC.Count(s => s.Key.Contains(e.ID_HANGHOA) && s.Value == (int)API.HinhThucKhuyenMai.SanPham) > 0 || lstCTKM_YC.Count(s => s.Key.Contains(e.ID_NHOMHANGHOA) && s.Value == (int)API.HinhThucKhuyenMai.NhomSanPham) > 0).ToList();
                        }
                        else
                        {
                            if (bolConSoLuong)
                                lstSelectProduct_Detail = lstProduct_Detail.Where(e => !e.ISKHUYENMAI && (e.SOLUONG - e.SOLUONGDALAY_KM > 0)).ToList();
                            else
                                lstSelectProduct_Detail = lstProduct_Detail.Where(e => !e.ISKHUYENMAI).ToList();
                        }

                        if (lstSelectProduct_Detail != null && lstSelectProduct_Detail.Count > 0)
                        {
                            double SumTien = 0;
                            double SumSoLuong = 0;
                            if (lstChuongTrinhKhuyenMai_YeuCau != null && lstChuongTrinhKhuyenMai_YeuCau.Count > 0)
                            {
                                SumTien = lstSelectProduct_Detail.Where(e => !e.ISKHUYENMAI && lstCTKM_YC.Count(s => s.Key.Contains(e.ID_HANGHOA) && s.Value == (int)API.HinhThucKhuyenMai.SanPham) > 0 || lstCTKM_YC.Count(s => s.Key.Contains(e.ID_NHOMHANGHOA) && s.Value == (int)API.HinhThucKhuyenMai.NhomSanPham) > 0).Sum(s => Convert.ToDouble(s.THANHTIEN));
                                if (!string.IsNullOrEmpty(itm.ID_DVT_DATKM))
                                {
                                    if (bolConSoLuong)
                                        SumSoLuong = lstSelectProduct_Detail.Where(e => !e.ISKHUYENMAI && (lstCTKM_YC.Count(s => s.Key.Contains(e.ID_HANGHOA) && s.Value == (int)API.HinhThucKhuyenMai.SanPham) > 0 || lstCTKM_YC.Count(s => s.Key.Contains(e.ID_NHOMHANGHOA) && s.Value == (int)API.HinhThucKhuyenMai.NhomSanPham) > 0) && e.ID_DVT == itm.ID_DVT_DATKM).Sum(s => Convert.ToDouble(s.SOLUONG - s.SOLUONGDALAY_KM));
                                    else
                                        SumSoLuong = lstSelectProduct_Detail.Where(e => !e.ISKHUYENMAI && (lstCTKM_YC.Count(s => s.Key.Contains(e.ID_HANGHOA) && s.Value == (int)API.HinhThucKhuyenMai.SanPham) > 0 || lstCTKM_YC.Count(s => s.Key.Contains(e.ID_NHOMHANGHOA) && s.Value == (int)API.HinhThucKhuyenMai.NhomSanPham) > 0) && e.ID_DVT == itm.ID_DVT_DATKM).Sum(s => Convert.ToDouble(s.SOLUONG));
                                }
                            }
                            else
                            {
                                SumTien = lstSelectProduct_Detail.Where(e => !e.ISKHUYENMAI).Sum(s => Convert.ToDouble(s.TONGCONG));
                                if (!string.IsNullOrEmpty(itm.ID_DVT_DATKM))
                                {
                                    if (bolConSoLuong)
                                        SumSoLuong = lstSelectProduct_Detail.Where(e => !e.ISKHUYENMAI && e.ID_DVT == itm.ID_DVT_DATKM).Sum(s => Convert.ToDouble(s.SOLUONG - s.SOLUONGDALAY_KM));
                                    else
                                        SumSoLuong = lstSelectProduct_Detail.Where(e => !e.ISKHUYENMAI && e.ID_DVT == itm.ID_DVT_DATKM).Sum(s => Convert.ToDouble(s.SOLUONG));
                                }
                            }


                            if (((itm.SOLUONG_DATKM != 0 || itm.SOLUONG_DATKM_DEN != 0) && SumSoLuong != 0 && itm.SOLUONG_DATKM <= SumSoLuong && (itm.SOLUONG_DATKM_DEN == 0 || itm.SOLUONG_DATKM_DEN >= SumSoLuong)) || ((itm.TONGTIEN_DATKM != 0 || itm.TONGTIEN_DATKM != 0) && SumTien != 0 && itm.TONGTIEN_DATKM <= SumTien && (itm.TONGTIEN_DATKM_DEN == 0 || itm.TONGTIEN_DATKM_DEN >= SumTien)))
                            {
                                var lstdm_ChuongTrinhKhuyenMai_Tang = await _context.dm_ChuongTrinhKhuyenMai_Tang!.Where(e => e.LOC_ID == itm.LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == itm.ID).ToListAsync();

                                #region Khuyến mãi tặng hàng hóa
                                if (lstdm_ChuongTrinhKhuyenMai_Tang != null && lstdm_ChuongTrinhKhuyenMai_Tang.Count > 0)
                                {
                                    var ID_KHO = lstProduct_Detail.Select(e => e.ID_KHO).FirstOrDefault();
                                    if (lstdm_ChuongTrinhKhuyenMai_Tang != null)
                                    {
                                        int SLKM_SL = (itm.SOLUONG_DATKM_DEN != 0 ? (Convert.ToInt32(SumSoLuong) / Convert.ToInt32(itm.SOLUONG_DATKM_DEN)) : (itm.SOLUONG_DATKM != 0 ? Convert.ToInt32(SumSoLuong) / Convert.ToInt32(itm.SOLUONG_DATKM) : 0));
                                        int SLKM_TIEN = (itm.TONGTIEN_DATKM_DEN != 0 ? (Convert.ToInt32(SumTien) / Convert.ToInt32(itm.TONGTIEN_DATKM_DEN)) : (itm.TONGTIEN_DATKM != 0 ? Convert.ToInt32(SumTien) / Convert.ToInt32(itm.TONGTIEN_DATKM) : 0));

                                        if (bolBatBuoc)
                                            SLKM_SL = (SLKM_SL > PhanNguyen ? PhanNguyen : SLKM_SL);

                                        foreach (var CTKM_Tang in lstdm_ChuongTrinhKhuyenMai_Tang)
                                        {
                                            var HangHoaKho = await _context.dm_HangHoa_Kho!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && e.ID_HANGHOA == CTKM_Tang.ID_HANGHOA && e.ID_KHO == ID_KHO);
                                            var HangHoa = await _context.view_dm_HangHoa!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && e.ID == CTKM_Tang.ID_HANGHOA);
                                            if (HangHoaKho != null && HangHoa != null)
                                            {
                                                Product_Detail newProduct_Detail = new Product_Detail();
                                                newProduct_Detail.STT = lstSelectProduct_Detail.Max(e => e.STT);
                                                newProduct_Detail.ID = Guid.NewGuid().ToString();
                                                newProduct_Detail.NAME = HangHoa.NAME;
                                                newProduct_Detail.MA = HangHoa.MA;
                                                newProduct_Detail.ID_HANGHOA = CTKM_Tang.ID_HANGHOA;
                                                newProduct_Detail.ID_HANGHOAKHO = HangHoaKho.ID;
                                                newProduct_Detail.DONGIA = 0;
                                                newProduct_Detail.ID_DVT = CTKM_Tang.ID_DVT;
                                                if (SLKM_SL > SLKM_TIEN)
                                                    newProduct_Detail.SOLUONG = (itm.ISTINHLUYTUYEN ? SLKM_SL : 1) * CTKM_Tang.SOLUONG;
                                                else
                                                    newProduct_Detail.SOLUONG = (itm.ISTINHLUYTUYEN ? SLKM_TIEN : 1) * CTKM_Tang.SOLUONG;

                                                newProduct_Detail.CHIETKHAU = 0;
                                                newProduct_Detail.TONGTIENGIAMGIA = 0;
                                                newProduct_Detail.THANHTIEN = 0;
                                                newProduct_Detail.THUESUAT = 0;
                                                newProduct_Detail.TONGTIENVAT = 0;
                                                newProduct_Detail.TONGCONG = 0;
                                                if (CTKM_Tang.SOTIEN > 0)
                                                {
                                                    newProduct_Detail.TONGTIENGIAMGIA = CTKM_Tang.SOTIEN;
                                                    newProduct_Detail.TYPE = "TONGTIENGIAMGIA";
                                                    dm_ThueSuat clsdm_ThueSuat = new dm_ThueSuat();
                                                    if (!string.IsNullOrEmpty(HangHoa.ID_THUESUAT))
                                                        clsdm_ThueSuat = await _context.dm_ThueSuat!.FirstOrDefaultAsync(e => e.LOC_ID == HangHoa.LOC_ID && e.ID == HangHoa.ID_THUESUAT) ?? new dm_ThueSuat();

                                                    API.TinhTong(newProduct_Detail, "", lstProduct_Detail, clsdm_ThueSuat);
                                                    if (MUCTHUE != 0)
                                                        SOTIENTHUE_KM += (newProduct_Detail.TONGTIENGIAMGIA * MUCTHUE / 100);
                                                }
                                                newProduct_Detail.ID_KHO = ID_KHO;
                                                newProduct_Detail.ISDALAYKHUYENMAI = true;
                                                newProduct_Detail.ISKHUYENMAI = true;
                                                newProduct_Detail.ID_KHUYENMAI = itm.ID;
                                                if (HangHoa != null && HangHoa.ID_DVT == newProduct_Detail.ID_DVT)
                                                {
                                                    newProduct_Detail.NAME_DVT = HangHoa.NAME_DVT;
                                                    if (!string.IsNullOrEmpty(HangHoa.ID_DVT_QD))
                                                    {
                                                        newProduct_Detail.TYLE_QD = HangHoa.TYLE_QD;
                                                    }
                                                    else
                                                    {
                                                        if (HangHoa.LOAIHANGHOA == ((int)API.LoaiSanPham.KhongQuanLyTonKho).ToString())
                                                            newProduct_Detail.TYLE_QD = 0;
                                                        else
                                                            newProduct_Detail.TYLE_QD = 1;

                                                    }
                                                    if (newProduct_Detail.SOLUONG != 0)
                                                    {
                                                        if (HangHoa != null && HangHoa.MUCTHUE != 0)
                                                            SOTIENTHUE_KM += (newProduct_Detail.SOLUONG * HangHoa.GIA01 * HangHoa.MUCTHUE / 100);
                                                    }
                                                }
                                                else if (HangHoa != null && HangHoa.ID_DVT_QD == newProduct_Detail.ID_DVT)
                                                {
                                                    if (!string.IsNullOrEmpty(HangHoa.ID_DVT_QD))
                                                    {
                                                        newProduct_Detail.NAME_DVT = HangHoa.NAME_DVT_QD;
                                                        newProduct_Detail.TYLE_QD = 1;
                                                    }
                                                    if (newProduct_Detail.SOLUONG != 0)
                                                    {
                                                        if (HangHoa != null && HangHoa.MUCTHUE != 0)
                                                            SOTIENTHUE_KM += (newProduct_Detail.SOLUONG * newProduct_Detail.TYLE_QD * HangHoa.GIA01_QD * HangHoa.MUCTHUE / 100);
                                                    }
                                                }
                                                else
                                                {
                                                    return Ok(new ApiResponse
                                                    {
                                                        Success = false,
                                                        Message = "Không tìm thấy thông tin sản phẩm với đơn vị tính " + newProduct_Detail.ID_DVT + " Kiểm tra CTKM" + itm.NAME,
                                                        Data = null
                                                    });

                                                }
                                                newProduct_Detail.TONGSOLUONG = newProduct_Detail.TYLE_QD * newProduct_Detail.SOLUONG;

                                                #region Sản phẩm tặng là Combo
                                                if (HangHoa != null && HangHoa.LOAIHANGHOA == ((int)API.LoaiSanPham.Combo).ToString())
                                                {
                                                    newProduct_Detail.ID_KHUYENMAI = newProduct_Detail.ID_HANGHOA;
                                                    SP_Parameter objParameter = new SP_Parameter();
                                                    objParameter.LOC_ID = itm.LOC_ID;
                                                    objParameter.ID_KHO = newProduct_Detail.ID_KHO;
                                                    objParameter.ID_COMBO = newProduct_Detail.ID_HANGHOA;
                                                    ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
                                                    actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachSanPhamKho_Combo(objParameter);
                                                    okResult = actionResult as OkObjectResult;
                                                    if (okResult != null)
                                                    {
                                                        var ApiResponse = okResult.Value as ApiResponse;
                                                        if (ApiResponse != null)
                                                        {
                                                            if (ApiResponse.Data != null)
                                                            {
                                                                var lst_ChiTiet = ApiResponse.Data as List<Product_Detail>;
                                                                if (lst_ChiTiet != null)
                                                                {
                                                                    foreach (var ChiTiet in lst_ChiTiet)
                                                                    {
                                                                        ChiTiet.STT = newProduct_Detail.STT;
                                                                        ChiTiet.ID = Guid.NewGuid().ToString();
                                                                        ChiTiet.ID_DVT = ChiTiet.ID_DVT_COMBO;
                                                                        ChiTiet.SOLUONG = newProduct_Detail.SOLUONG * ChiTiet.QTY_COMBO;
                                                                        ChiTiet.TYLE_QD = ChiTiet.TYLE_QD_COMBO;
                                                                        ChiTiet.TONGSOLUONG = newProduct_Detail.SOLUONG * ChiTiet.QTY_TOTAL_COMBO;
                                                                        ChiTiet.DONGIA = 0;
                                                                        ChiTiet.CHIETKHAU = 0;
                                                                        ChiTiet.TONGTIENGIAMGIA = 0;
                                                                        ChiTiet.THANHTIEN = 0;
                                                                        ChiTiet.THUESUAT = 0;
                                                                        ChiTiet.TONGTIENVAT = 0;
                                                                        ChiTiet.TONGCONG = 0;
                                                                        ChiTiet.ISKHUYENMAI = true;
                                                                        ChiTiet.ID_KHUYENMAI = itm.ID;
                                                                        ChiTiet.ISCOMBO = true;
                                                                        ChiTiet.ID_COMBO = newProduct_Detail.ID_HANGHOA;
                                                                        lstProduct_Detail.Add(ChiTiet);
                                                                    }

                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                #endregion

                                                //if (SLKM_SL > SLKM_TIEN)
                                                //{
                                                //    var SoLuongBang = lstProduct_Detail.Where(s => lstSelectProduct_Detail.Where(e => e.ID == s.ID).Count() > 0 && s.SOLUONG == SLKM_SL * itm.SOLUONG_DATKM).FirstOrDefault();
                                                //    if (SoLuongBang != null)
                                                //    {
                                                //        SoLuongBang.SOLUONGDALAYKHUYENMAI = SoLuongBang.SOLUONG;
                                                //        SoLuongBang.ISDALAYKHUYENMAI = true;
                                                //        SoLuongBang.ID_KHUYENMAI = itm.ID;
                                                //    }
                                                //    else
                                                //    {
                                                //        double SoLuongConLai = SumSoLuong;
                                                //        foreach (var chitiet in lstProduct_Detail.Where(s => lstSelectProduct_Detail.Where(e => e.ID == s.ID).Count() > 0))
                                                //        {
                                                //            if (chitiet.SOLUONG >= SoLuongConLai)
                                                //            {
                                                //                chitiet.SOLUONGDALAYKHUYENMAI = chitiet.SOLUONG;
                                                //                chitiet.ISDALAYKHUYENMAI = true;
                                                //                chitiet.ID_KHUYENMAI = itm.ID;
                                                //                SoLuongConLai -= chitiet.SOLUONG;
                                                //            }
                                                //        }
                                                //    }
                                                //}
                                                lstProduct_Detail.Add(newProduct_Detail);
                                            }
                                        }

                                        if (lstChuongTrinhKhuyenMai_YeuCau != null && lstChuongTrinhKhuyenMai_YeuCau.Count > 0)
                                        {
                                            double SoLuongYeuCau = (itm.SOLUONG_DATKM_DEN != 0 ? itm.SOLUONG_DATKM_DEN : itm.SOLUONG_DATKM) * (itm.ISTINHLUYTUYEN ? SLKM_SL : 1);
                                            foreach (var CTKM_YC in lstChuongTrinhKhuyenMai_YeuCau.OrderByDescending(e => e.ISBATBUOC))
                                            {
                                                //var SumSoLuong_Tam = lstProduct_Detail_Tam.Where(e => (!itm.ISTONGHOADON || (itm.ISTONGHOADON && !e.ISDALAYKHUYENMAI)) && ((e.ID_HANGHOA == CTKM_YC.ID_HANGHOA && CTKM_YC.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham) || (e.ID_NHOMHANGHOA == CTKM_YC.ID_HANGHOA && CTKM_YC.HINHTHUC == (int)API.HinhThucKhuyenMai.NhomSanPham)) && e.ID_DVT == CTKM_YC.ID_DVT).Sum(s => Convert.ToDouble(s.SOLUONG - s.SOLUONGDALAY_KM));
                                                foreach (var ChiTiet in lstProduct_Detail_Tam.Where(e => (!itm.ISTONGHOADON || (itm.ISTONGHOADON && !e.ISDALAYKHUYENMAI)) && ((e.ID_HANGHOA == CTKM_YC.ID_HANGHOA && CTKM_YC.HINHTHUC == (int)API.HinhThucKhuyenMai.SanPham) || (e.ID_NHOMHANGHOA == CTKM_YC.ID_HANGHOA && CTKM_YC.HINHTHUC == (int)API.HinhThucKhuyenMai.NhomSanPham)) && e.ID_DVT == CTKM_YC.ID_DVT && (e.SOLUONG - e.SOLUONGDALAY_KM > 0)).ToList())
                                                {
                                                    if (SoLuongYeuCau == 0) break;
                                                    if (SoLuongYeuCau - ((ChiTiet.SOLUONG - ChiTiet.SOLUONGDALAY_KM)) > 0)
                                                    {
                                                        ChiTiet.SOLUONGDALAY_KM += ((ChiTiet.SOLUONG - ChiTiet.SOLUONGDALAY_KM));
                                                        SoLuongYeuCau -= ((ChiTiet.SOLUONGDALAY_KM));
                                                    }
                                                    else
                                                    {
                                                        ChiTiet.SOLUONGDALAY_KM += (SoLuongYeuCau);
                                                        SoLuongYeuCau = 0;
                                                    }
                                                }
                                            }
                                        }


                                        intCoLayKhuyenMai += 1;
                                    }
                                }
                                #endregion

                                #region Chiết khấu tiền, giảm tiền
                                if (itm.CHIETKHAU > 0 || itm.TIENGIAM > 0)
                                {
                                    if (!itm.ISTONGHOADON)
                                    {
                                        foreach (var ChiTiet in lstProduct_Detail.Where(s => lstSelectProduct_Detail.Where(e => e.ID == s.ID).Count() > 0))
                                        {
                                            double SLKM_SL = (itm.SOLUONG_DATKM_DEN != 0 ? (ChiTiet.SOLUONG / itm.SOLUONG_DATKM_DEN) : (itm.SOLUONG_DATKM != 0 ? (ChiTiet.SOLUONG) / (itm.SOLUONG_DATKM) : 0));
                                            dm_ThueSuat clsdm_ThueSuat = new dm_ThueSuat();
                                            if (!string.IsNullOrEmpty(ChiTiet.ID_THUESUAT))
                                                clsdm_ThueSuat = await _context.dm_ThueSuat!.FirstOrDefaultAsync(e => e.LOC_ID == ChiTiet.LOC_ID && e.ID == ChiTiet.ID_THUESUAT) ?? new dm_ThueSuat();// GetDetail<v_v_dm_ThueSuat>(LOC_ID + "/" + Product_Detail.ID_THUESUAT, API.dm_ThueSuat);
                                            if (itm.CHIETKHAU > 0)
                                            {
                                                ChiTiet.CHIETKHAU = itm.CHIETKHAU;
                                                ChiTiet.TYPE = "CHIETKHAU";
                                                API.TinhTong(ChiTiet, "", lstProduct_Detail, clsdm_ThueSuat);
                                                //ChiTiet.ISDALAYKHUYENMAI = true;
                                                ChiTiet.SOLUONGDALAYKHUYENMAI = ChiTiet.TONGSOLUONG;
                                                ChiTiet.ID_KHUYENMAI = itm.ID;
                                                var objHangHoa = await _context.view_dm_HangHoa!.FirstOrDefaultAsync(e => e.LOC_ID == ChiTiet.LOC_ID && e.ID == ChiTiet.ID_HANGHOA);
                                                if (objHangHoa != null && objHangHoa.MUCTHUE != 0)
                                                    SOTIENTHUE_KM += (ChiTiet.TONGTIENGIAMGIA * objHangHoa.MUCTHUE / 100);
                                            }
                                            else if (itm.TIENGIAM > 0)
                                            {
                                                ChiTiet.TONGTIENGIAMGIA = itm.TIENGIAM * (itm.ISTINHLUYTUYEN ? SLKM_SL : 1);
                                                ChiTiet.TYPE = "TONGTIENGIAMGIA";
                                                API.TinhTong(ChiTiet, "", lstProduct_Detail, clsdm_ThueSuat);
                                                //ChiTiet.ISDALAYKHUYENMAI = true;
                                                ChiTiet.SOLUONGDALAYKHUYENMAI = ChiTiet.TONGSOLUONG;
                                                ChiTiet.ID_KHUYENMAI = itm.ID;
                                                var objHangHoa = await _context.view_dm_HangHoa!.FirstOrDefaultAsync(e => e.LOC_ID == ChiTiet.LOC_ID && e.ID == ChiTiet.ID_HANGHOA);
                                                if (objHangHoa != null && objHangHoa.MUCTHUE != 0)
                                                    SOTIENTHUE_KM += (ChiTiet.TONGTIENGIAMGIA * objHangHoa.MUCTHUE / 100);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var ID_KHO = lstProduct_Detail.Select(e => e.ID_KHO).FirstOrDefault();
                                        var HangHoa = await _context.view_dm_HangHoa!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && e.MA == API.GTBH);
                                        if (HangHoa != null)
                                        {
                                            var HangHoaKho = await _context.dm_HangHoa_Kho!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && e.ID_HANGHOA == HangHoa.ID && e.ID_KHO == ID_KHO);
                                            if (HangHoaKho != null)
                                            {
                                                Product_Detail newProduct_Detail = new Product_Detail();
                                                newProduct_Detail.STT = lstSelectProduct_Detail.Max(e => e.STT) + 1;
                                                newProduct_Detail.ID = Guid.NewGuid().ToString();
                                                newProduct_Detail.NAME = HangHoa.NAME;
                                                newProduct_Detail.MA = HangHoa.MA;
                                                newProduct_Detail.ID_HANGHOA = HangHoaKho.ID_HANGHOA;
                                                newProduct_Detail.ID_HANGHOAKHO = HangHoaKho.ID;
                                                newProduct_Detail.DONGIA = 0;
                                                newProduct_Detail.ID_DVT = HangHoa.ID_DVT;
                                                newProduct_Detail.NAME_DVT = HangHoa.NAME_DVT;
                                                newProduct_Detail.SOLUONG = 0;
                                                dm_ThueSuat clsdm_ThueSuat = new dm_ThueSuat();
                                                if (!string.IsNullOrEmpty(HangHoa.ID_THUESUAT))
                                                    clsdm_ThueSuat = await _context.dm_ThueSuat!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && e.ID == HangHoa.ID_THUESUAT) ?? new dm_ThueSuat();
                                                if (itm.CHIETKHAU > 0)
                                                {
                                                    newProduct_Detail.CHIETKHAU = itm.CHIETKHAU;
                                                    newProduct_Detail.TONGTIENGIAMGIA = SumTien * newProduct_Detail.CHIETKHAU / 100;
                                                    newProduct_Detail.TYPE = "TONGTIENGIAMGIA";
                                                    API.TinhTong(newProduct_Detail, "", lstProduct_Detail, clsdm_ThueSuat);
                                                    newProduct_Detail.ISDALAYKHUYENMAI = true;
                                                    newProduct_Detail.SOLUONGDALAYKHUYENMAI = newProduct_Detail.TONGSOLUONG;
                                                    newProduct_Detail.ID_KHUYENMAI = itm.ID;
                                                    if (MUCTHUE != 0)
                                                        SOTIENTHUE_KM += (newProduct_Detail.TONGTIENGIAMGIA * MUCTHUE / 100);

                                                }
                                                else if (itm.TIENGIAM > 0)
                                                {
                                                    newProduct_Detail.TONGTIENGIAMGIA = itm.TIENGIAM * (itm.ISTINHLUYTUYEN ? PhanNguyen : 1);
                                                    newProduct_Detail.TYPE = "TONGTIENGIAMGIA";
                                                    API.TinhTong(newProduct_Detail, "", lstProduct_Detail, clsdm_ThueSuat);
                                                    newProduct_Detail.ISDALAYKHUYENMAI = true;
                                                    newProduct_Detail.SOLUONGDALAYKHUYENMAI = newProduct_Detail.TONGSOLUONG;
                                                    newProduct_Detail.ID_KHUYENMAI = itm.ID;
                                                    if (MUCTHUE != 0)
                                                        SOTIENTHUE_KM += (newProduct_Detail.TONGTIENGIAMGIA * MUCTHUE / 100);
                                                }
                                                newProduct_Detail.ID_KHO = ID_KHO;
                                                newProduct_Detail.ISDALAYKHUYENMAI = true;
                                                newProduct_Detail.ISKHUYENMAI = true;
                                                newProduct_Detail.ID_KHUYENMAI = itm.ID;
                                                lstProduct_Detail.Add(newProduct_Detail);
                                                foreach (var ChiTiet in lstProduct_Detail)
                                                {
                                                    ChiTiet.ISDALAYKHUYENMAI = true;
                                                }
                                            }

                                        }
                                    }

                                    intCoLayKhuyenMai += 1;
                                }
                                #endregion
                            }
                        }

                        #endregion
                        //if (itm.ISTONGHOADON)
                        //    break;
                    }

                    if (intCoLayKhuyenMai > 0)
                    {
                        lstDanhSachDaLayKhuyenMai.Add(itm.MA);
                    }
                }

                #region Thêm tính thuế khuyến mãi
                if (SOTIENTHUE_KM != 0)
                {
                    var ID_KHO = lstProduct_Detail.Select(e => e.ID_KHO).FirstOrDefault();
                    var HangHoa = await _context.view_dm_HangHoa!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.MA == API.TINHTHUE_KM);
                    if (HangHoa != null)
                    {
                        var HangHoaKho = await _context.dm_HangHoa_Kho!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID_HANGHOA == HangHoa.ID);
                        if (HangHoaKho != null)
                        {
                            Product_Detail newProduct_Detail = new Product_Detail();
                            newProduct_Detail.STT = lstProduct_Detail.Max(e => e.STT);
                            newProduct_Detail.ID = Guid.NewGuid().ToString();
                            newProduct_Detail.NAME = HangHoa.NAME;
                            newProduct_Detail.MA = HangHoa.MA;
                            newProduct_Detail.ID_HANGHOA = HangHoaKho.ID_HANGHOA;
                            newProduct_Detail.ID_HANGHOAKHO = HangHoaKho.ID;
                            newProduct_Detail.DONGIA = 0;
                            newProduct_Detail.ID_DVT = HangHoa.ID_DVT;
                            newProduct_Detail.NAME_DVT = HangHoa.NAME_DVT;
                            newProduct_Detail.SOLUONG = 0;
                            dm_ThueSuat clsdm_ThueSuat = new dm_ThueSuat();
                            if (!string.IsNullOrEmpty(HangHoa.ID_THUESUAT))
                                clsdm_ThueSuat = await _context.dm_ThueSuat!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == HangHoa.ID_THUESUAT) ?? new dm_ThueSuat();

                            newProduct_Detail.TONGTIENGIAMGIA = -1 * Math.Ceiling(SOTIENTHUE_KM);
                            newProduct_Detail.TYPE = "TONGTIENGIAMGIA";
                            API.TinhTong(newProduct_Detail, "", lstProduct_Detail, clsdm_ThueSuat);
                            newProduct_Detail.ISKHUYENMAI = true;
                            newProduct_Detail.GHICHU = "";
                            lstProduct_Detail.Add(newProduct_Detail);
                        }
                    }
                }
                #endregion

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = (from x in lstProduct_Detail
                            orderby x.STT ascending, x.ISKHUYENMAI ascending
                            select x)
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = lstProduct_Detail
                });
            }
        }
        #endregion


    }
}