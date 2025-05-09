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
using System.Reflection;

namespace MyApiNetCore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public DeliveryController(dbTrangHiepPhatContext context, IConfiguration configuration)
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

                var lstValue = await _context.ct_PhieuGiaoHang!.Where(e => e.LOC_ID == LOC_ID).ToListAsync();
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
                var lstValue = await _context.ct_PhieuGiaoHang!.Where(e => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).ToListAsync();
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
                var Input = await _context.ct_PhieuGiaoHang!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);

                if (Input == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                v_ct_PhieuGiaoHang ct_PhieuGiaoHang = new v_ct_PhieuGiaoHang();
                SP_Parameter SP_Parameter = new SP_Parameter();
                SP_Parameter.ID_PHIEUGIAOHANG = ID;
                ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
                var actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang(SP_Parameter);
                var okResult = actionResult as OkObjectResult;
                if (okResult != null)
                {
                    var ApiResponse = okResult.Value as ApiResponse;
                    if (ApiResponse != null)
                    {
                        if (ApiResponse.Success && ApiResponse.Data != null)
                        {
                            var lst = ApiResponse.Data as List<v_ct_PhieuGiaoHang>;
                            if (lst != null && lst.Count > 0)
                                ct_PhieuGiaoHang = lst.FirstOrDefault() ?? new v_ct_PhieuGiaoHang();
                        }
                        else
                        {
                            return Ok(ApiResponse);
                        }
                    }
                }
                //if (Input != null)
                //{
                //    string strInput = JsonConvert.SerializeObject(Input);
                //    ct_PhieuGiaoHang = JsonConvert.DeserializeObject<v_ct_PhieuGiaoHang>(strInput) ?? new v_ct_PhieuGiaoHang();
                //}

                ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>();
                ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
                SP_Parameter = new SP_Parameter();
                SP_Parameter.ID_PHIEUGIAOHANG = ID;
                ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
                actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang_ChiTiet(SP_Parameter);
                okResult = actionResult as OkObjectResult;
                if (okResult != null)
                {
                    var ApiResponse = okResult.Value as ApiResponse;
                    if (ApiResponse != null)
                    {

                        if (ApiResponse.Success && ApiResponse.Data != null)
                        {
                            var lst_ChiTiet = ApiResponse.Data as List<v_ct_PhieuGiaoHang_ChiTiet>;
                            if (lst_ChiTiet != null)
                            {
                                ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet.AddRange(lst_ChiTiet);
                            }
                        }
                        else
                        {
                            return Ok(ApiResponse);
                        }
                    }
                }

                actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang_NhanVienGiao(SP_Parameter);
                okResult = actionResult as OkObjectResult;
                if (okResult != null)
                {
                    var ApiResponse = okResult.Value as ApiResponse;
                    if (ApiResponse != null)
                    {

                        if (ApiResponse.Success && ApiResponse.Data != null)
                        {
                            var lst_ChiTiet = ApiResponse.Data as List<v_ct_PhieuGiaoHang_NhanVienGiao>;
                            if (lst_ChiTiet != null)
                            {
                                ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao.AddRange(lst_ChiTiet);
                            }
                        }
                        else
                        {
                            return Ok(ApiResponse);
                        }
                    }
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = ct_PhieuGiaoHang
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
        public async Task<IActionResult> PutInput(string LOC_ID, string ID, [FromBody] v_ct_PhieuGiaoHang Input)
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
                var PhieuNhap = await _context.ct_PhieuGiaoHang!.FirstOrDefaultAsync(e => e.LOC_ID == Input.LOC_ID && e.ID == Input.ID);
                using var transaction = _context.Database.BeginTransaction();
                {
                    var lstPhieuNhap_ChiTiet = await _context.ct_PhieuGiaoHang_ChiTiet!.Where(e => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUGIAOHANG == Input.ID).ToListAsync();
                    if (lstPhieuNhap_ChiTiet != null)
                    {
                        foreach (ct_PhieuGiaoHang_ChiTiet itm in lstPhieuNhap_ChiTiet)
                        {
                            var chkPhieuNhap_ChiTiet = Input.lstct_PhieuGiaoHang_ChiTiet.Where(e => e.ID == itm.ID).FirstOrDefault();
                            if (chkPhieuNhap_ChiTiet != null)
                            {
                                if (PhieuNhap != null && PhieuNhap.ISHOANTAT != Input.ISHOANTAT && Input.ISHOANTAT)
                                {
                                    chkPhieuNhap_ChiTiet.ISDAGIAOHANG = true;
                                }
                                chkPhieuNhap_ChiTiet.ISEDIT = true;
                                chkPhieuNhap_ChiTiet.ID_PHIEUGIAOHANG = Input.ID;
                                _context.Entry(itm).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            }
                            else
                            {
                                _context.ct_PhieuGiaoHang_ChiTiet!.Remove(itm);
                            }
                        }
                    }

                    if (Input.lstct_PhieuGiaoHang_ChiTiet != null)
                    {
                        foreach (v_ct_PhieuGiaoHang_ChiTiet itm in Input.lstct_PhieuGiaoHang_ChiTiet)
                        {
                            if (PhieuNhap != null && PhieuNhap.ISHOANTAT != Input.ISHOANTAT && Input.ISHOANTAT)
                            {
                                itm.ISDAGIAOHANG = true;
                            }
                            itm.ID_PHIEUGIAOHANG = Input.ID;
                            if (!itm.ISEDIT)
                            {
                                itm.SOLAN = lstPhieuNhap_ChiTiet != null ? lstPhieuNhap_ChiTiet.Max(s => s.SOLAN) + 1 : 1;
                                _context.ct_PhieuGiaoHang_ChiTiet!.Add(itm);
                            }
                        }
                        Input.SOLUONG_DONHANG = Input.lstct_PhieuGiaoHang_ChiTiet.Count();
                        Input.SOTIENGIAOHANG = Input.lstct_PhieuGiaoHang_ChiTiet.Sum(e => e.SOTIENGIAOHANG);
                    }

                    var lstPhieuNhap_NhanVienGiao = await _context.ct_PhieuGiaoHang_NhanVienGiao!.Where(e => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUGIAOHANG == Input.ID).ToListAsync();
                    if (lstPhieuNhap_NhanVienGiao != null)
                    {
                        foreach (ct_PhieuGiaoHang_NhanVienGiao itm in lstPhieuNhap_NhanVienGiao)
                        {
                            var chkPhieuNhap_NhanVienGiao = Input.lstct_PhieuGiaoHang_NhanVienGiao.Where(e => e.ID == itm.ID).FirstOrDefault();
                            if (chkPhieuNhap_NhanVienGiao != null)
                            {
                                chkPhieuNhap_NhanVienGiao.ISEDIT = true;
                                chkPhieuNhap_NhanVienGiao.ID_PHIEUGIAOHANG = Input.ID;
                                _context.Entry(itm).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            }
                            else
                            {
                                _context.ct_PhieuGiaoHang_NhanVienGiao!.Remove(itm);
                            }
                        }
                    }

                    if (Input.lstct_PhieuGiaoHang_NhanVienGiao != null)
                    {
                        foreach (v_ct_PhieuGiaoHang_NhanVienGiao itm in Input.lstct_PhieuGiaoHang_NhanVienGiao)
                        {
                            itm.ID_PHIEUGIAOHANG = Input.ID;
                            if (!itm.ISEDIT)
                            {
                                itm.ID = Guid.NewGuid().ToString();
                                _context.ct_PhieuGiaoHang_NhanVienGiao!.Add(itm);
                            }

                        }
                    }
                    if (PhieuNhap != null)
                        _context.Entry(PhieuNhap).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                    _context.Entry(Input).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
                transaction.Commit();

                v_ct_PhieuGiaoHang ct_PhieuGiaoHang = new v_ct_PhieuGiaoHang();
                ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>();
                ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
                SP_Parameter SP_Parameter = new SP_Parameter();
                SP_Parameter.LOC_ID = Input.LOC_ID;
                SP_Parameter.ID_PHIEUGIAOHANG = Input.ID;
                ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
                var actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang(SP_Parameter);
                var okResult = actionResult as OkObjectResult;
                if (okResult != null)
                {
                    var ApiResponse = okResult.Value as ApiResponse;
                    if (ApiResponse != null)
                    {
                        if (ApiResponse.Success && ApiResponse.Data != null)
                        {
                            var lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuGiaoHang>;
                            if (lstPhieuNhap != null && lstPhieuNhap.Count() > 0)
                            {
                                ct_PhieuGiaoHang = lstPhieuNhap.FirstOrDefault() ?? new v_ct_PhieuGiaoHang();
                            }
                        }
                        else
                        {
                            return Ok(ApiResponse);
                        }
                    }
                }
                ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>();
                ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
                SP_Parameter = new SP_Parameter();
                SP_Parameter.ID_PHIEUGIAOHANG = Input.ID;
                actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang_ChiTiet(SP_Parameter);
                okResult = actionResult as OkObjectResult;
                if (okResult != null)
                {
                    var ApiResponse = okResult.Value as ApiResponse;
                    if (ApiResponse != null)
                    {

                        if (ApiResponse.Success && ApiResponse.Data != null)
                        {
                            var lst_ChiTiet = ApiResponse.Data as List<v_ct_PhieuGiaoHang_ChiTiet>;
                            if (lst_ChiTiet != null)
                            {
                                ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet.AddRange(lst_ChiTiet);
                            }
                        }
                        else
                        {
                            return Ok(ApiResponse);
                        }
                    }
                }

                actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang_NhanVienGiao(SP_Parameter);
                okResult = actionResult as OkObjectResult;
                if (okResult != null)
                {
                    var ApiResponse = okResult.Value as ApiResponse;
                    if (ApiResponse != null)
                    {

                        if (ApiResponse.Success && ApiResponse.Data != null)
                        {
                            var lst_ChiTiet = ApiResponse.Data as List<v_ct_PhieuGiaoHang_NhanVienGiao>;
                            if (lst_ChiTiet != null)
                            {
                                ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao.AddRange(lst_ChiTiet);
                            }
                        }
                        else
                        {
                            return Ok(ApiResponse);
                        }
                    }
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = ct_PhieuGiaoHang
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

        private static ct_PhieuGiaoHang ConvertobjectToct_PhieuGiaoHang<T>(T objectFrom)
        {
            ct_PhieuGiaoHang objectTo = new ct_PhieuGiaoHang();
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

        // POST: api/Input
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<ct_PhieuGiaoHang>> PostInput([FromBody] v_ct_PhieuGiaoHang Input)
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
                var objPhieuNhap = await _context.ct_PhieuGiaoHang!.FirstOrDefaultAsync(e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.MAPHIEU);
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
                    if (Input.lstct_PhieuGiaoHang_ChiTiet != null)
                    {
                        foreach (var itm in Input.lstct_PhieuGiaoHang_ChiTiet)
                        {
                            var objct_PhieuXuat = await _context.ct_PhieuXuat!.FirstOrDefaultAsync(e => e.LOC_ID == Input.LOC_ID && e.ID == itm.ID_PHIEUXUAT);
                            if (objct_PhieuXuat != null)
                            {
                                var objct_PhieuGiaoHang_ChiTiet = await _context.ct_PhieuGiaoHang_ChiTiet!.FirstOrDefaultAsync(e => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUXUAT == itm.ID_PHIEUXUAT);
                                if (objct_PhieuGiaoHang_ChiTiet != null)
                                {
                                    return Ok(new ApiResponse
                                    {
                                        Success = false,
                                        Message = "Phiếu xuất " + objct_PhieuXuat.LOC_ID + "-" + objct_PhieuXuat.MAPHIEU + " đã được tạo phiếu giao hàng!",
                                        Data = "",
                                        CheckValue = true
                                    });
                                }

                                if (objct_PhieuXuat.ISHOANTAT)
                                {
                                    return Ok(new ApiResponse
                                    {
                                        Success = false,
                                        Message = "Đã hoàn tất " + objct_PhieuXuat.LOC_ID + "-" + objct_PhieuXuat.MAPHIEU + " trong dữ liệu!",
                                        Data = "",
                                        CheckValue = true
                                    });
                                }
                                itm.ID = Guid.NewGuid().ToString();
                                itm.LOC_ID = Input.LOC_ID;
                                itm.ID_PHIEUGIAOHANG = Input.ID;
                                itm.SOLAN = 1;
                                _context.ct_PhieuGiaoHang_ChiTiet!.Add(itm);
                            }
                            else
                            {
                                return Ok(new ApiResponse
                                {
                                    Success = false,
                                    Message = "Không tìm thấy phiếu xuất " + Input.LOC_ID + "-" + itm.ID_PHIEUXUAT + " trong dữ liệu!",
                                    Data = "",
                                    CheckValue = true
                                });
                            }

                        }
                        Input.SOLUONG_DONHANG = Input.lstct_PhieuGiaoHang_ChiTiet.Count();
                        Input.SOTIENGIAOHANG = Input.lstct_PhieuGiaoHang_ChiTiet.Sum(e => e.SOTIENGIAOHANG);
                    }

                    if (Input.lstct_PhieuGiaoHang_NhanVienGiao != null)
                    {
                        foreach (var itm in Input.lstct_PhieuGiaoHang_NhanVienGiao)
                        {
                            itm.ID = Guid.NewGuid().ToString();
                            itm.LOC_ID = Input.LOC_ID;
                            itm.ID_PHIEUGIAOHANG = Input.ID;
                            _context.ct_PhieuGiaoHang_NhanVienGiao!.Add(itm);
                        }
                    }
                    bool bolCheckMA = false;
                    while (!bolCheckMA)
                    {
                        //Output.MAPHIEU = API.GetMaPhieu(API.ct_PhieuXuat, Output.NGAYLAP, Output.SOPHIEU);
                        var check = _context.ct_PhieuGiaoHang!.Where(e => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.MAPHIEU).FirstOrDefault();
                        if (check != null)
                        {
                            Input.MAPHIEU = API.GetMaPhieu(API.ct_PhieuGiaoHang, Input.NGAYLAP, Input.SOPHIEU);
                        }
                        else
                        {
                            bolCheckMA = true;
                        }
                    }
                    _context.ct_PhieuGiaoHang!.Add(Input);
                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
                transaction.Commit();

                v_ct_PhieuGiaoHang ct_PhieuGiaoHang = new v_ct_PhieuGiaoHang();
                ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>();
                ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
                SP_Parameter SP_Parameter = new SP_Parameter();
                SP_Parameter.LOC_ID = Input.LOC_ID;
                SP_Parameter.ID_PHIEUGIAOHANG = Input.ID;
                ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
                var actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang(SP_Parameter);
                var okResult = actionResult as OkObjectResult;
                if (okResult != null)
                {
                    var ApiResponse = okResult.Value as ApiResponse;
                    if (ApiResponse != null)
                    {
                        if (ApiResponse.Success && ApiResponse.Data != null)
                        {
                            var lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuGiaoHang>;
                            if (lstPhieuNhap != null && lstPhieuNhap.Count() > 0)
                            {
                                ct_PhieuGiaoHang = lstPhieuNhap.FirstOrDefault() ?? new v_ct_PhieuGiaoHang();
                            }
                        }
                        else
                        {
                            return Ok(ApiResponse);
                        }
                    }
                }
                ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>();
                ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
                SP_Parameter = new SP_Parameter();
                SP_Parameter.ID_PHIEUGIAOHANG = Input.ID;
                actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang_ChiTiet(SP_Parameter);
                okResult = actionResult as OkObjectResult;
                if (okResult != null)
                {
                    var ApiResponse = okResult.Value as ApiResponse;
                    if (ApiResponse != null)
                    {

                        if (ApiResponse.Success && ApiResponse.Data != null)
                        {
                            var lst_ChiTiet = ApiResponse.Data as List<v_ct_PhieuGiaoHang_ChiTiet>;
                            if (lst_ChiTiet != null)
                            {
                                ct_PhieuGiaoHang.lstct_PhieuGiaoHang_ChiTiet.AddRange(lst_ChiTiet);
                            }
                        }
                        else
                        {
                            return Ok(ApiResponse);
                        }
                    }
                }

                actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang_NhanVienGiao(SP_Parameter);
                okResult = actionResult as OkObjectResult;
                if (okResult != null)
                {
                    var ApiResponse = okResult.Value as ApiResponse;
                    if (ApiResponse != null)
                    {

                        if (ApiResponse.Success && ApiResponse.Data != null)
                        {
                            var lst_ChiTiet = ApiResponse.Data as List<v_ct_PhieuGiaoHang_NhanVienGiao>;
                            if (lst_ChiTiet != null)
                            {
                                ct_PhieuGiaoHang.lstct_PhieuGiaoHang_NhanVienGiao.AddRange(lst_ChiTiet);
                            }
                        }
                        else
                        {
                            return Ok(ApiResponse);
                        }
                    }
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = ct_PhieuGiaoHang
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
                var Input = await _context.ct_PhieuGiaoHang!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
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
                            Message = "Có 'Phiếu Thu "+ ChungTu + "' liên quan tới " + Input.MAPHIEU + "!",
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
                            Message = "Có 'Phiếu Chi "+ ChungTu + "' liên quan tới " + Input.MAPHIEU + "!",
                            Data = ""
                        });
                    }

                    var lstPhieuNhap = await _context.ct_PhieuNhap!.Where(e => e.LOC_ID == Input.LOC_ID && e.CHUNGTUKEMTHEO == Input.MAPHIEU).ToListAsync();
                    if (lstPhieuNhap != null && lstPhieuNhap.Count() > 0)
                    {
                        String ChungTu = "(" + String.Join(";", lstPhieuNhap.Select(e => e.MAPHIEU)) + ")";
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Có 'Phiếu Nhập "+ ChungTu + "' liên quan tới " + Input.MAPHIEU + "!",
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
                            Message = "Có 'Phiếu Xuất "+ ChungTu + "' liên quan tới " + Input.MAPHIEU + "!",
                            Data = ""
                        });
                    }
                    var lstPhieuNhap_ChiTiet = await _context.ct_PhieuGiaoHang_ChiTiet!.Where(e => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUGIAOHANG == Input.ID).ToListAsync();
                    if (lstPhieuNhap_ChiTiet != null)
                    {
                        foreach (ct_PhieuGiaoHang_ChiTiet itm in lstPhieuNhap_ChiTiet)
                        {
                            _context.ct_PhieuGiaoHang_ChiTiet!.Remove(itm);
                        }
                    }
                    var lstPhieuNhap_NhanVienGiao = await _context.ct_PhieuGiaoHang_NhanVienGiao!.Where(e => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUGIAOHANG == Input.ID).ToListAsync();
                    if (lstPhieuNhap_ChiTiet != null)
                    {
                        foreach (ct_PhieuGiaoHang_NhanVienGiao itm in lstPhieuNhap_NhanVienGiao)
                        {
                            _context.ct_PhieuGiaoHang_NhanVienGiao!.Remove(itm);
                        }
                    }

                    _context.ct_PhieuGiaoHang!.Remove(Input);
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
            return _context.ct_PhieuGiaoHang!.Any(e => e.LOC_ID == LOC_ID && e.ID == ID);
        }

    }
}