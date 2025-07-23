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
using API_QuanLyTHP.Class;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Runtime.ConstrainedExecution;

namespace MyApiNetCore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InputController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public InputController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("TrangHiepPhat");
        }
        [HttpGet("{LOC_ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetInput(string LOC_ID)
        {
            try
            {

                var lstValue = await _context.ct_PhieuNhap!.Where(e => e.LOC_ID == LOC_ID).ToListAsync();
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
                var lstValue = await _context.ct_PhieuNhap!.Where(e => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).ToListAsync();
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


        //GET: api/Input/5
        [HttpGet("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetInput(string LOC_ID, string ID)
        {
            try
            {
                var Input = await _context.ct_PhieuNhap!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);

                if (Input == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                v_ct_PhieuNhap ct_PhieuNhap = new v_ct_PhieuNhap();
                if (Input != null)
                {
                    string strInput = JsonConvert.SerializeObject(Input);
                    ct_PhieuNhap = JsonConvert.DeserializeObject<v_ct_PhieuNhap>(strInput) ?? new v_ct_PhieuNhap();
                }

                ct_PhieuNhap.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuNhap_ChiTiet>();
                SP_Parameter SP_Parameter = new SP_Parameter();
                SP_Parameter.ID_PHIEUNHAP = ID;
                ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
                var actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuNhap_Chitiet(SP_Parameter);
                var okResult = actionResult as OkObjectResult;
                if (okResult != null)
                {
                    var ApiResponse = okResult.Value as ApiResponse;
                    if (ApiResponse != null)
                    {

                        if (ApiResponse.Data != null)
                        {
                            var lst_ChiTiet = ApiResponse.Data as List<v_ct_PhieuNhap_ChiTiet>;
                            if (lst_ChiTiet != null)
                            {
                                ct_PhieuNhap.lstct_PhieuNhap_ChiTiet.AddRange(lst_ChiTiet);
                            }
                        }
                    }
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = ct_PhieuNhap
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

        // PUT: api/Input/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PutInput(string LOC_ID, string ID, [FromBody] v_ct_PhieuNhap Input)
        {
            try
            {
                if (!InputExistsID(Input.LOC_ID, Input.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + Input.LOC_ID + "-" + Input.ID + " dữ liệu!",
                        Data = ""
                    });
                }

                using var transaction = _context.Database.BeginTransaction();
                {
                    var lstPhieuNhap_ChiTiet = await _context.ct_PhieuNhap_ChiTiet!.Where(e => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUNHAP == Input.ID).ToListAsync();
                    if (lstPhieuNhap_ChiTiet != null)
                    {
                        foreach (ct_PhieuNhap_ChiTiet itm in lstPhieuNhap_ChiTiet)
                        {
                            var objdm_HangHoa_Kho = await _context.dm_HangHoa_Kho!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO);
                            if (objdm_HangHoa_Kho != null)
                            {
                                itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
                                objdm_HangHoa_Kho.QTY -= itm.TONGSOLUONG;
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
                            var chkPhieuNhap_ChiTiet = Input.lstct_PhieuNhap_ChiTiet.Where(e => e.ID == itm.ID).FirstOrDefault();
                            if (chkPhieuNhap_ChiTiet != null)
                            {
                                chkPhieuNhap_ChiTiet.ISEDIT = true;
                                chkPhieuNhap_ChiTiet.ID_PHIEUNHAP = Input.ID;
                                _context.Entry(objdm_HangHoa_Kho).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                            }
                            else
                            {
                                _context.ct_PhieuNhap_ChiTiet!.Remove(itm);

                            }
                        }
                    }

                    if (Input.lstct_PhieuNhap_ChiTiet != null)
                    {
                        foreach (v_ct_PhieuNhap_ChiTiet itm in Input.lstct_PhieuNhap_ChiTiet)
                        {
                            itm.THANHTIEN = itm.SOLUONG * itm.DONGIA - itm.TONGTIENGIAMGIA;
                            itm.TONGCONG = itm.THANHTIEN + itm.TONGTIENVAT;
                            itm.ID_PHIEUNHAP = Input.ID;
                            var objdm_HangHoa_Kho = _context.dm_HangHoa_Kho!.FirstOrDefault(e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO && e.ID_KHO == Input.ID_KHO);
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
                            if (!itm.ISEDIT)
                            {
                                _context.ct_PhieuNhap_ChiTiet!.Add(itm);

                            }

                        }
                        Input.TONGTHANHTIEN = Math.Round(Input.lstct_PhieuNhap_ChiTiet.Sum(s => s.THANHTIEN), 0);
                        Input.TONGTIENGIAMGIA = Math.Round(Input.lstct_PhieuNhap_ChiTiet.Sum(s => s.TONGTIENGIAMGIA), 0);
                        Input.TONGTIENVAT = Math.Round(Input.lstct_PhieuNhap_ChiTiet.Sum(s => s.TONGTIENVAT), 0);
                        Input.TONGTIEN = Math.Round(Input.lstct_PhieuNhap_ChiTiet.Sum(s => s.TONGCONG), 0);
                    }
                    _context.Entry(Input).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    AuditLogController auditLog = new AuditLogController(_context, _configuration); auditLog.InserAuditLog(); await _context.SaveChangesAsync();
                }
                transaction.Commit();
                var InputCheck = await _context.ct_PhieuNhap!.FirstOrDefaultAsync(e => e.LOC_ID == Input.LOC_ID && e.ID == Input.ID);
                using var transaction1 = _context.Database.BeginTransaction();
                {
                    if (InputCheck != null)
                    {
                        if (InputCheck.CHUNGTUKEMTHEO != Input.CHUNGTUKEMTHEO)
                        {
                            if (InputCheck.CHUNGTUKEMTHEO != null && InputCheck.CHUNGTUKEMTHEO.StartsWith("PX-"))
                            {
                                var PhieuXuat = await _context.ct_PhieuXuat!.FirstOrDefaultAsync(e => e.LOC_ID == InputCheck.LOC_ID && e.MAPHIEU == InputCheck.CHUNGTUKEMTHEO);
                                if (PhieuXuat != null)
                                {
                                    var ct_PhieuGiaoHang_ChiTiet = await _context.ct_PhieuGiaoHang_ChiTiet!.FirstOrDefaultAsync(e => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUXUAT == PhieuXuat.ID);
                                    if (ct_PhieuGiaoHang_ChiTiet != null)
                                    {
                                        var SoTien = await _context.ct_PhieuThu!.Where(e => e.LOC_ID == InputCheck.LOC_ID && e.CHUNGTUKEMTHEO == InputCheck.CHUNGTUKEMTHEO).SumAsync(e => e.SOTIEN);
                                        ct_PhieuGiaoHang_ChiTiet.ISTRAHANG = false;
                                        _context.Entry(ct_PhieuGiaoHang_ChiTiet).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                    }
                                }
                            }
                            if (Input.CHUNGTUKEMTHEO != null && Input.CHUNGTUKEMTHEO.StartsWith("PX-"))
                            {
                                var PhieuXuat = await _context.ct_PhieuXuat!.FirstOrDefaultAsync(e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO);
                                if (PhieuXuat != null)
                                {
                                    var ct_PhieuGiaoHang_ChiTiet = await _context.ct_PhieuGiaoHang_ChiTiet!.FirstOrDefaultAsync(e => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUXUAT == PhieuXuat.ID);
                                    if (ct_PhieuGiaoHang_ChiTiet != null)
                                    {
                                        ct_PhieuGiaoHang_ChiTiet.ISTRAHANG = true;
                                        _context.Entry(ct_PhieuGiaoHang_ChiTiet).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                    }
                                }
                            }
                            AuditLogController auditLog = new AuditLogController(_context, _configuration); auditLog.InserAuditLog(); await _context.SaveChangesAsync();
                        }
                    }
                }
                transaction1.Commit();
                v_ct_PhieuNhap ct_PhieuNhap = new v_ct_PhieuNhap();
                ct_PhieuNhap.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuNhap_ChiTiet>();
                SP_Parameter SP_Parameter = new SP_Parameter();
                SP_Parameter.ID_PHIEUNHAP = Input.ID;
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
                            var lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuNhap>;
                            if (lstPhieuNhap != null && lstPhieuNhap.Count() > 0)
                            {
                                ct_PhieuNhap = lstPhieuNhap.FirstOrDefault() ?? new v_ct_PhieuNhap();
                            }
                        }
                    }
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = ct_PhieuNhap
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

        // POST: api/Input
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<ct_PhieuNhap>> PostInput([FromBody] v_ct_PhieuNhap Input)
        {
            try
            {
                if (InputExistsID(Input.LOC_ID, Input.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + Input.LOC_ID + "-" + Input.ID + " trong dữ liệu!",
                        Data = "",
                        CheckValue = true
                    });
                }
                var objPhieuNhap = await _context.ct_PhieuNhap!.FirstOrDefaultAsync(e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.MAPHIEU);
                if (objPhieuNhap != null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + Input.LOC_ID + "-" + Input.MAPHIEU + " trong dữ liệu!",
                        Data = "",
                        CheckValue = true
                    });
                }
                if (Input.CHUNGTUKEMTHEO != null && Input.CHUNGTUKEMTHEO.StartsWith("NCC-"))
                {
                    var ct_PhieuDatHangNCC = await _context.ct_PhieuDatHangNCC!.FirstOrDefaultAsync(e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO);
                    if (ct_PhieuDatHangNCC != null && ct_PhieuDatHangNCC.ISHOANTAT)
                    {
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Phiếu " + ct_PhieuDatHangNCC.MAPHIEU + " đã hoàn thành! Vui lòng kiểm tra lại!",
                            Data = "",
                            CheckValue = true
                        });
                    }
                }
                ct_PhieuDatHangNCC objct_PhieuDatHangNCC = new ct_PhieuDatHangNCC();
                using var transaction = _context.Database.BeginTransaction();
                {
                    if (Input.lstct_PhieuNhap_ChiTiet != null)
                    {
                        foreach (var itm in Input.lstct_PhieuNhap_ChiTiet)
                        {
                            itm.THANHTIEN = itm.SOLUONG * itm.DONGIA - itm.TONGTIENGIAMGIA;
                            itm.TONGCONG = itm.THANHTIEN  + itm.TONGTIENVAT;
                            var objdm_HangHoa_Kho = await _context.dm_HangHoa_Kho!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO && e.ID_KHO == Input.ID_KHO);
                            if (objdm_HangHoa_Kho != null)
                            {
                                itm.TONGSOLUONG = itm.SOLUONG * itm.TYLE_QD;
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
                            itm.LOC_ID = Input.LOC_ID;
                            itm.ID_PHIEUNHAP = Input.ID;
                            _context.ct_PhieuNhap_ChiTiet!.Add(itm);
                        }
                        Input.TONGTHANHTIEN = Math.Round(Input.lstct_PhieuNhap_ChiTiet.Sum(s => s.THANHTIEN), 0);
                        Input.TONGTIENGIAMGIA = Math.Round(Input.lstct_PhieuNhap_ChiTiet.Sum(s => s.TONGTIENGIAMGIA), 0);
                        Input.TONGTIENVAT = Math.Round(Input.lstct_PhieuNhap_ChiTiet.Sum(s => s.TONGTIENVAT), 0);
                        Input.TONGTIEN = Math.Round(Input.lstct_PhieuNhap_ChiTiet.Sum(s => s.TONGCONG), 0);
                    }
                    bool bolCheckMA = false;
                    while (!bolCheckMA)
                    {
                        //Output.MAPHIEU = API.GetMaPhieu(API.ct_PhieuXuat, Output.NGAYLAP, Output.SOPHIEU);
                        var check = _context.ct_PhieuNhap!.Where(e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.MAPHIEU).FirstOrDefault();
                        if (check != null)
                        {
                            Input.MAPHIEU = API.GetMaPhieu(API.ct_PhieuNhap, Input.NGAYLAP, Input.SOPHIEU);
                        }
                        else
                        {
                            bolCheckMA = true;
                        }
                    }
                    _context.ct_PhieuNhap!.Add(Input);
                    if (Input.CHUNGTUKEMTHEO != null && Input.CHUNGTUKEMTHEO.StartsWith("PX-"))
                    {
                        var PhieuXuat = await _context.ct_PhieuXuat!.FirstOrDefaultAsync(e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO);
                        if (PhieuXuat != null)
                        {
                            var ct_PhieuGiaoHang_ChiTiet = await _context.ct_PhieuGiaoHang_ChiTiet!.FirstOrDefaultAsync(e => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUXUAT == PhieuXuat.ID);
                            if (ct_PhieuGiaoHang_ChiTiet != null)
                            {
                                ct_PhieuGiaoHang_ChiTiet.ISTRAHANG = true;
                                _context.Entry(ct_PhieuGiaoHang_ChiTiet).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            }
                        }
                    }
                    if (Input.CHUNGTUKEMTHEO != null && Input.CHUNGTUKEMTHEO.StartsWith("NCC-"))
                    {
                        objct_PhieuDatHangNCC = await _context.ct_PhieuDatHangNCC!.FirstOrDefaultAsync(e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO);
                        if (objct_PhieuDatHangNCC != null)
                        {
                            var lstPhieuNhap = await _context.ct_PhieuNhap!.Where(e => e.LOC_ID == objct_PhieuDatHangNCC.LOC_ID && e.CHUNGTUKEMTHEO == objct_PhieuDatHangNCC.MAPHIEU).ToListAsync();
                            String ChungTu = "";
                            if (lstPhieuNhap != null && lstPhieuNhap.Count() > 0)
                            {
                                ChungTu = String.Join(";", lstPhieuNhap.Select(e => e.MAPHIEU));
                            }
                            objct_PhieuDatHangNCC.CHUNGTUKEMTHEO = "(" + Input.MAPHIEU + (string.IsNullOrEmpty(ChungTu) ? "" : ";") + ChungTu + ")";
                            _context.Entry(objct_PhieuDatHangNCC).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        }
                    }
                    AuditLogController auditLog = new AuditLogController(_context, _configuration); auditLog.InserAuditLog(); await _context.SaveChangesAsync();
                }
                transaction.Commit();
                if (Input.CHUNGTUKEMTHEO != null && Input.CHUNGTUKEMTHEO.StartsWith("NCC-") && objct_PhieuDatHangNCC != null && !string.IsNullOrEmpty(objct_PhieuDatHangNCC.ID))
                {
                    var result = GetQuantityCheck(Input.CHUNGTUKEMTHEO);
                    using var transaction1 = _context.Database.BeginTransaction();
                    {
                        if (result != null)
                        {
                            var TongSo = result.Sum(s => s.Status);
                            var OKCar = await _context.ct_PhieuDatHangNCC!.FirstOrDefaultAsync(e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO);
                            //if ()
                            {
                                if (OKCar != null)
                                {
                                    OKCar.ISHOANTAT = TongSo > 0 ? false : true;
                                    _context.Entry(OKCar).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                    AuditLogController auditLog = new AuditLogController(_context, _configuration); auditLog.InserAuditLog(); await _context.SaveChangesAsync();
                                }
                            }
                        }
                    }
                    transaction1.Commit();
                    v_ct_PhieuDatHangNCC ct_PhieuNhap = new v_ct_PhieuDatHangNCC();
                    ct_PhieuNhap.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuDatHangNCC_ChiTiet>();
                    SP_Parameter SP_Parameter = new SP_Parameter();
                    SP_Parameter.ID_PHIEUNHAP = objct_PhieuDatHangNCC.ID;
                    ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
                    var actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuDatHangNCC(SP_Parameter);
                    var okResult = actionResult as OkObjectResult;
                    if (okResult != null)
                    {
                        var ApiResponse = okResult.Value as ApiResponse;
                        if (ApiResponse != null)
                        {

                            if (ApiResponse.Data != null)
                            {
                                var lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuDatHangNCC>;
                                if (lstPhieuNhap != null && lstPhieuNhap.Count() > 0)
                                {
                                    ct_PhieuNhap = lstPhieuNhap.FirstOrDefault() ?? new v_ct_PhieuDatHangNCC();
                                }
                            }
                        }
                    }
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Success",
                        Data = ct_PhieuNhap
                    });
                }
                else
                {
                    v_ct_PhieuNhap ct_PhieuNhap = new v_ct_PhieuNhap();
                    ct_PhieuNhap.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuNhap_ChiTiet>();
                    SP_Parameter SP_Parameter = new SP_Parameter();
                    SP_Parameter.ID_PHIEUNHAP = Input.ID;
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
                                var lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuNhap>;
                                if (lstPhieuNhap != null && lstPhieuNhap.Count() > 0)
                                {
                                    ct_PhieuNhap = lstPhieuNhap.FirstOrDefault() ?? new v_ct_PhieuNhap();
                                }
                            }
                        }
                    }
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Success",
                        Data = ct_PhieuNhap
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

        [HttpPut("{MAPHIEU}")]
        public IEnumerable<QuantityCheckResult> GetQuantityCheck(string MAPHIEU)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<QuantityCheckResult>(
                    "CheckPhieuDatHang",
                    new { MAPHIEU = MAPHIEU },
                    commandType: CommandType.StoredProcedure);
            }
        }
        // DELETE: api/Input/5
        [HttpDelete("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeleteInput(string LOC_ID, string ID)
        {
            try
            {
                var Input = await _context.ct_PhieuNhap!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (Input == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                using var transaction = _context.Database.BeginTransaction();
                {
                    var lstPhieuThu = await _context.ct_PhieuThu!.Where(e => e.LOC_ID == Input.LOC_ID && e.CHUNGTUKEMTHEO == Input.MAPHIEU).ToListAsync();
                    if (lstPhieuThu != null && lstPhieuThu.Count() > 0)
                    {
                        String ChungTu = "(" + String.Join(";", lstPhieuThu.Select(e => e.MAPHIEU)) + ")";
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Có 'Phiếu Thu " + ChungTu + "' liên quan tới " + Input.MAPHIEU + "!",
                            Data = ""
                        });
                    }
                    var lstPhieuChi = await _context.ct_PhieuChi!.Where(e => e.LOC_ID == Input.LOC_ID && e.CHUNGTUKEMTHEO == Input.MAPHIEU).ToListAsync();
                    if (lstPhieuChi != null && lstPhieuChi.Count() > 0)
                    {
                        String ChungTu = "(" + String.Join(";", lstPhieuChi.Select(e => e.MAPHIEU)) + ")";
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Có 'Phiếu Chi " + ChungTu + "' liên quan tới " + Input.MAPHIEU + "!",
                            Data = ""
                        });
                    }
                    var lstPhieuXuat = await _context.ct_PhieuXuat!.Where(e => e.LOC_ID == Input.LOC_ID && e.CHUNGTUKEMTHEO == Input.MAPHIEU).ToListAsync();
                    if (lstPhieuXuat != null && lstPhieuXuat.Count() > 0)
                    {
                        String ChungTu = "(" + String.Join(";", lstPhieuXuat.Select(e => e.MAPHIEU)) + ")";
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Có 'Phiếu Xuất " + ChungTu + "' liên quan tới " + Input.MAPHIEU + "!",
                            Data = ""
                        });
                    }

                    var lstPhieuNhap_ChiTiet = await _context.ct_PhieuNhap_ChiTiet!.Where(e => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUNHAP == Input.ID).ToListAsync();
                    if (lstPhieuNhap_ChiTiet != null)
                    {
                        foreach (ct_PhieuNhap_ChiTiet itm in lstPhieuNhap_ChiTiet)
                        {
                            var objdm_HangHoa_Kho = await _context.dm_HangHoa_Kho!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO);
                            if (objdm_HangHoa_Kho != null)
                            {
                                itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
                                objdm_HangHoa_Kho.QTY -= itm.TONGSOLUONG;
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
                            _context.ct_PhieuNhap_ChiTiet!.Remove(itm);
                        }
                    }

                    _context.ct_PhieuNhap!.Remove(Input);

                    if (Input.CHUNGTUKEMTHEO != null && Input.CHUNGTUKEMTHEO.StartsWith("PX-"))
                    {
                        var PhieuXuat = await _context.ct_PhieuXuat!.Where(e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO).ToListAsync();
                        if (PhieuXuat != null && PhieuXuat.Count == 1)
                        {
                            var ct_PhieuGiaoHang_ChiTiet = await _context.ct_PhieuGiaoHang_ChiTiet!.FirstOrDefaultAsync(e => e.LOC_ID == PhieuXuat[0].LOC_ID && e.ID_PHIEUXUAT == PhieuXuat[0].ID);
                            if (ct_PhieuGiaoHang_ChiTiet != null)
                            {
                                ct_PhieuGiaoHang_ChiTiet.ISTRAHANG = false;
                                _context.Entry(ct_PhieuGiaoHang_ChiTiet).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                            }
                        }
                    }

                    if (Input.CHUNGTUKEMTHEO != null && Input.CHUNGTUKEMTHEO.StartsWith("NCC-"))
                    {
                        var PhieuXuat = await _context.ct_PhieuDatHangNCC!.Where(e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO).ToListAsync();
                        if (PhieuXuat != null && PhieuXuat.Count > 0)
                        {
                            foreach (var itm in PhieuXuat)
                            {
                                var lstPhieuNhap = await _context.ct_PhieuNhap!.Where(e => e.LOC_ID == Input.LOC_ID && e.CHUNGTUKEMTHEO == Input.CHUNGTUKEMTHEO && e.ID != Input.ID).ToListAsync();
                                String ChungTu = "";
                                itm.CHUNGTUKEMTHEO = "";
                                itm.ISHOANTAT = false;
                                if (lstPhieuNhap != null && lstPhieuNhap.Count() > 0)
                                {
                                    ChungTu = String.Join(";", lstPhieuNhap.Select(e => e.MAPHIEU));
                                    itm.CHUNGTUKEMTHEO = "(" + (string.IsNullOrEmpty(ChungTu) ? "" : ";") + ChungTu + ")";
                                }
                                _context.Entry(itm).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            }
                        }
                    }
                    AuditLogController auditLog = new AuditLogController(_context, _configuration); auditLog.InserAuditLog(); await _context.SaveChangesAsync();
                }

                transaction.Commit();
                if (Input != null && Input.CHUNGTUKEMTHEO != null && Input.CHUNGTUKEMTHEO.StartsWith("NCC-"))
                {
                    var result = GetQuantityCheck(Input.CHUNGTUKEMTHEO);
                    if (result != null)
                    {
                        var TongSo = result.Sum(s => s.Status);
                        var OKCar = await _context.ct_PhieuDatHangNCC!.FirstOrDefaultAsync(e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO);
                        //if ()
                        {
                            if (OKCar != null)
                            {
                                OKCar.ISHOANTAT = TongSo > 0 ? false : true;
                                _context.Entry(OKCar).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                AuditLogController auditLog = new AuditLogController(_context, _configuration); auditLog.InserAuditLog(); await _context.SaveChangesAsync();
                            }
                        }
                    }
                }

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
        private string strTable = "ct_PhieuNhap";
        private bool InputExistsID(string LOC_ID, string ID)
        {

            //bool bolCheckMA = false;
            //while (!bolCheckMA)
            //{
            //    using var transaction = _context.Database.BeginTransaction();
            //    {
            //        var check = _context.AspNetRequest!.Where(e => e.LOC_ID == LOC_ID && e.NAME == strTable).FirstOrDefault();
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
            //            _context.SaveChanges();
            //            bolCheckMA = true;
            //        }
            //    }
            //    transaction.Commit();
            //}

            return _context.ct_PhieuNhap!.Any(e => e.LOC_ID == LOC_ID && e.ID == ID);
        }

    }
}