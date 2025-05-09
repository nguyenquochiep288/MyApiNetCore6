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

namespace MyApiNetCore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Order_ProviderController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public Order_ProviderController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
        }
        [HttpGet("{LOC_ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetInput(string LOC_ID)
        {
            try
            {

                var lstValue = await _context.ct_PhieuDatHangNCC!.Where(e => e.LOC_ID == LOC_ID).ToListAsync();
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
                var lstValue = await _context.ct_PhieuDatHangNCC!.Where(e => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).ToListAsync();
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
                var Input = await _context.ct_PhieuDatHangNCC!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);

                if (Input == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC = new v_ct_PhieuDatHangNCC();
                if(Input != null)
                {
                    string strInput = JsonConvert.SerializeObject(Input);
                    ct_PhieuDatHangNCC = JsonConvert.DeserializeObject<v_ct_PhieuDatHangNCC>(strInput) ?? new v_ct_PhieuDatHangNCC();
                }
                
                ct_PhieuDatHangNCC.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuDatHangNCC_ChiTiet>();
                SP_Parameter SP_Parameter = new SP_Parameter();
                SP_Parameter.ID_PHIEUNHAP = ID;
                ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
                var actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuDatHangNCC_Chitiet(SP_Parameter);
                var okResult = actionResult as OkObjectResult;
                if (okResult != null)
                {
                    var ApiResponse = okResult.Value as ApiResponse;
                    if (ApiResponse != null)
                    {

                        if (ApiResponse.Data != null)
                        {
                            var lst_ChiTiet = ApiResponse.Data as List<v_ct_PhieuDatHangNCC_ChiTiet>;
                            if (lst_ChiTiet != null)
                            {
                                ct_PhieuDatHangNCC.lstct_PhieuNhap_ChiTiet.AddRange(lst_ChiTiet);
                            }
                        }
                    }
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = ct_PhieuDatHangNCC
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
        public async Task<IActionResult> PutInput(string LOC_ID, string ID, [FromBody] v_ct_PhieuDatHangNCC Input)
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
                    var lstPhieuNhap_ChiTiet = await _context.ct_PhieuDatHangNCC_ChiTiet!.Where(e => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUDATHANGNCC == Input.ID).ToListAsync();
                    if (lstPhieuNhap_ChiTiet != null)
                    {
                        foreach (ct_PhieuDatHangNCC_ChiTiet itm in lstPhieuNhap_ChiTiet)
                        {
                            var objdm_HangHoa_Kho = await _context.dm_HangHoa_Kho!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO);
                            if (objdm_HangHoa_Kho != null)
                            {
                                itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
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
                                chkPhieuNhap_ChiTiet.ID_PHIEUDATHANGNCC = Input.ID;
                                _context.Entry(objdm_HangHoa_Kho).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                
                            }
                            else
                            {
                                _context.ct_PhieuDatHangNCC_ChiTiet!.Remove(itm);
                                
                            }
                        }
                    }

                    if (Input.lstct_PhieuNhap_ChiTiet != null)
                    {
                        foreach (v_ct_PhieuDatHangNCC_ChiTiet itm in Input.lstct_PhieuNhap_ChiTiet)
                        {
                            itm.ID_PHIEUDATHANGNCC = Input.ID;
                            var objdm_HangHoa_Kho = _context.dm_HangHoa_Kho!.FirstOrDefault(e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO);
                            if (objdm_HangHoa_Kho != null)
                            {
                                itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
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
                                _context.ct_PhieuDatHangNCC_ChiTiet!.Add(itm);
                                
                            }

                        }
                    }
                    _context.Entry(Input).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
                transaction.Commit();
                //var InputCheck = await _context.ct_PhieuDatHangNCC!.FirstOrDefaultAsync(e => e.LOC_ID == Input.LOC_ID && e.ID == Input.ID);

                v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC = new v_ct_PhieuDatHangNCC();
                ct_PhieuDatHangNCC.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuDatHangNCC_ChiTiet>();
                SP_Parameter SP_Parameter = new SP_Parameter();
                SP_Parameter.ID_PHIEUNHAP = Input.ID;
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
                                ct_PhieuDatHangNCC = lstPhieuNhap.FirstOrDefault() ?? new v_ct_PhieuDatHangNCC();
                            }
                        }
                    }
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = ct_PhieuDatHangNCC
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

        // PUT: api/Input/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{LOC_ID}/{ID}/{TRANGTHAI}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PutInput(string LOC_ID, string ID, string TRANGTHAI)
        {
            try
            {
                if (!InputExistsID(LOC_ID, ID))
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
                    var objct_PhieuDatHangNCC = await _context.ct_PhieuDatHangNCC!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                    if(objct_PhieuDatHangNCC != null)
                    {
                        objct_PhieuDatHangNCC.ISHOANTAT = TRANGTHAI == "1";
                        _context.Entry(objct_PhieuDatHangNCC).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        AuditLogController auditLog = new AuditLogController(_context, _configuration); auditLog.InserAuditLog(); await _context.SaveChangesAsync();
                    }
                }
                transaction.Commit();
                v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC = new v_ct_PhieuDatHangNCC();
                ct_PhieuDatHangNCC.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuDatHangNCC_ChiTiet>();
                SP_Parameter SP_Parameter = new SP_Parameter();
                SP_Parameter.ID_PHIEUNHAP = ID;
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
                                ct_PhieuDatHangNCC = lstPhieuNhap.FirstOrDefault() ?? new v_ct_PhieuDatHangNCC();
                            }
                        }
                    }
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = ct_PhieuDatHangNCC
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
        // POST: api/Input
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<ct_PhieuDatHangNCC>> PostInput([FromBody] v_ct_PhieuDatHangNCC Input)
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
                var objPhieuNhap = await _context.ct_PhieuDatHangNCC!.FirstOrDefaultAsync(e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.MAPHIEU);
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
                using var transaction = _context.Database.BeginTransaction();
                {
                    if (Input.lstct_PhieuNhap_ChiTiet != null)
                    {
                        foreach (var itm in Input.lstct_PhieuNhap_ChiTiet)
                        {
                            var objdm_HangHoa_Kho = await _context.dm_HangHoa_Kho!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO);
                            if (objdm_HangHoa_Kho != null)
                            {
                                itm.TONGSOLUONG = itm.SOLUONG * itm.TYLE_QD;
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
                            itm.ID_PHIEUDATHANGNCC = Input.ID;
                            _context.ct_PhieuDatHangNCC_ChiTiet!.Add(itm);
                        }
                    }
                    _context.ct_PhieuDatHangNCC!.Add(Input);
                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
                transaction.Commit();

                v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC = new v_ct_PhieuDatHangNCC();
                ct_PhieuDatHangNCC.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuDatHangNCC_ChiTiet>();
                SP_Parameter SP_Parameter = new SP_Parameter();
                SP_Parameter.ID_PHIEUNHAP = Input.ID;
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
                                ct_PhieuDatHangNCC = lstPhieuNhap.FirstOrDefault() ?? new v_ct_PhieuDatHangNCC();
                            }
                        }
                    }
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = ct_PhieuDatHangNCC
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

        // DELETE: api/Input/5
        [HttpDelete("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeleteInput(string LOC_ID, string ID)
        {
            try
            {
                var Input = await _context.ct_PhieuDatHangNCC!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (Input == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                if (Input.ISHOANTAT)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Phiếu " + Input.MAPHIEU + " đã hoàn thành! Vui lòng kiểm tra lại!",
                        Data = ""
                    });
                }
                using var transaction = _context.Database.BeginTransaction();
                {
                    var lstPhieuNhap_ChiTiet = await _context.ct_PhieuDatHangNCC_ChiTiet!.Where(e => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUDATHANGNCC == Input.ID).ToListAsync();
                    if (lstPhieuNhap_ChiTiet != null)
                    {
                        foreach (ct_PhieuDatHangNCC_ChiTiet itm in lstPhieuNhap_ChiTiet)
                        {
                            var objdm_HangHoa_Kho = await _context.dm_HangHoa_Kho!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO);
                            if (objdm_HangHoa_Kho != null)
                            {
                                itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
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
                            _context.ct_PhieuDatHangNCC_ChiTiet!.Remove(itm);
                        }
                    }

                    _context.ct_PhieuDatHangNCC!.Remove(Input);
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

        private bool InputExistsID(string LOC_ID, string ID)
        {
            return _context.ct_PhieuDatHangNCC!.Any(e => e.LOC_ID == LOC_ID && e.ID == ID);
        }

    }
}