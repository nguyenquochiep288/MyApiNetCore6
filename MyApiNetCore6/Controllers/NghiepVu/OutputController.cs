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
using Microsoft.Build.Framework;

namespace MyApiNetCore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OutputController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public OutputController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
        }
        [HttpGet("{LOC_ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetOutput(string LOC_ID)
        {
            try
            {

                var lstValue = await _context.ct_PhieuXuat!.Where(e => e.LOC_ID == LOC_ID).ToListAsync();
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
        public async Task<IActionResult> GetOutput(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
        {
            try
            {
                var lstValue = await _context.ct_PhieuXuat!.Where(e => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).ToListAsync();
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
        // GET: api/Output
        [HttpGet("{LOC_ID}/{ID_KHO}/{FROMDATE}/{TODATE}/{Type}/{KeyWhere}/{ValuesSearch}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetOutput(string LOC_ID, string ID_KHO, DateTime FROMDATE, DateTime TODATE, int Type, string KeyWhere = "", string ValuesSearch = "")
        {
            try
            {
                var lstValue = await _context.ct_PhieuXuat!.Where(e => e.LOC_ID == LOC_ID && e.NGAYLAP.Date >= FROMDATE.Date && e.NGAYLAP.Date <= TODATE.Date).Where(KeyWhere, ValuesSearch).ToListAsync();
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


        //GET: api/Output/5
        [HttpGet("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetOutput(string LOC_ID, string ID)
        {
            try
            {
                var Output = await _context.ct_PhieuXuat!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);

                if (Output == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = ""
                    });
                }
                v_ct_PhieuXuat ct_PhieuXuat = new v_ct_PhieuXuat();
                SP_Parameter SP_Parameter = new SP_Parameter();
                SP_Parameter.LOC_ID = LOC_ID;
                SP_Parameter.ID_PHIEUXUAT = ID;
                ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
                var actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuXuat(SP_Parameter);
                var okResult = actionResult as OkObjectResult;
                if (okResult != null)
                {
                    var ApiResponse = okResult.Value as ApiResponse;
                    if (ApiResponse != null)
                    {
                        if (ApiResponse.Data != null)
                        {
                            ct_PhieuXuat = (ApiResponse.Data as List<v_ct_PhieuXuat> ?? new List<v_ct_PhieuXuat>()).FirstOrDefault();
                        }
                    }
                }

                SP_Parameter = new SP_Parameter();
                SP_Parameter.ID_PHIEUXUAT = ID;
                ct_PhieuXuat.lstct_PhieuXuat_ChiTiet = new List<v_ct_PhieuXuat_ChiTiet>();
                ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
                actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuXuat_Chitiet(SP_Parameter);
                okResult = actionResult as OkObjectResult;
                if (okResult != null)
                {
                    var ApiResponse = okResult.Value as ApiResponse;
                    if (ApiResponse != null)
                    {
                        if (ApiResponse.Data != null)
                        {
                            var lst_ChiTiet = ApiResponse.Data as List<v_ct_PhieuXuat_ChiTiet>;
                            if (lst_ChiTiet != null)
                            {
                                ct_PhieuXuat.lstct_PhieuXuat_ChiTiet.AddRange(lst_ChiTiet);
                            }
                        }

                    }
                }

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = ct_PhieuXuat
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

        // PUT: api/Output/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> PutOutput(string LOC_ID, string ID, [FromBody] v_ct_PhieuXuat Output)
        {
            try
            {
                if (!OutputExistsID(Output.LOC_ID, Output.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + Output.LOC_ID + "-" + Output.ID + " dữ liệu!",
                        Data = ""
                    });
                }

                //Output.TONGTIENTINHTHUE = 0;
                string StrHetSoLuong = "";
                using var transaction = _context.Database.BeginTransaction();
                {
                    var lstPhieuNhap_ChiTiet = await _context.ct_PhieuXuat_ChiTiet!.Where(e => e.LOC_ID == Output.LOC_ID && e.ID_PHIEUXUAT == Output.ID).ToListAsync();
                    if (lstPhieuNhap_ChiTiet != null)
                    {
                        foreach (ct_PhieuXuat_ChiTiet itm in lstPhieuNhap_ChiTiet)
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
                            var chkPhieuNhap_ChiTiet = Output.lstct_PhieuXuat_ChiTiet.Where(e => e.ID == itm.ID).FirstOrDefault();
                            if (chkPhieuNhap_ChiTiet != null)
                            {
                                chkPhieuNhap_ChiTiet.ISEDIT = true;
                                chkPhieuNhap_ChiTiet.ID_PHIEUXUAT = Output.ID;
                                _context.Entry(objdm_HangHoa_Kho).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            }
                            else
                            {
                                _context.ct_PhieuXuat_ChiTiet!.Remove(itm);
                            }
                        }
                    }

                    if (Output.lstct_PhieuXuat_ChiTiet != null)
                    {
                        foreach (v_ct_PhieuXuat_ChiTiet itm in Output.lstct_PhieuXuat_ChiTiet)
                        {
                            //Output.TONGTIENTINHTHUE += itm.TONGTIENGIAMGIA;  
                            itm.THANHTIEN = itm.SOLUONG * itm.DONGIA;
                            itm.TONGCONG = itm.THANHTIEN - itm.TONGTIENGIAMGIA + itm.TONGTIENVAT;
                            itm.ID_PHIEUXUAT = Output.ID;
                            var objdm_HangHoa_Kho = _context.dm_HangHoa_Kho!.FirstOrDefault(e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO && e.ID_KHO == Output.ID_KHO);
                            if (objdm_HangHoa_Kho != null)
                            {
                                var objdm_HangHoa = _context.view_dm_HangHoa!.FirstOrDefault(e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOA);
                                itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
                                if (objdm_HangHoa_Kho.QTY >= itm.TONGSOLUONG)
                                {
                                    //if (objdm_HangHoa != null)
                                    //{
                                    //    Output.TONGTIENTINHTHUE += (objdm_HangHoa.ID_DVT == itm.ID_DVT ? objdm_HangHoa.GIA01  : objdm_HangHoa.GIA01_QD) * itm.SOLUONG;
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
                                        if(itm.TYLE_QD > 1)
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
                                _context.ct_PhieuXuat_ChiTiet!.Add(itm);
                            }
                        }
                        Output.TONGTHANHTIEN = Math.Round(Output.lstct_PhieuXuat_ChiTiet.Sum(s => s.THANHTIEN), 0);
                        Output.TONGTIENGIAMGIA = Math.Round(Output.lstct_PhieuXuat_ChiTiet.Sum(s => s.TONGTIENGIAMGIA),0);
                        Output.TONGTIENVAT = Math.Round(Output.lstct_PhieuXuat_ChiTiet.Sum(s => s.TONGTIENVAT), 0);
                        Output.TONGTIEN = Math.Round(Output.lstct_PhieuXuat_ChiTiet.Sum(s => s.TONGCONG), 0);
                        if (!string.IsNullOrEmpty(StrHetSoLuong))
                        {
                            return Ok(new ApiResponse
                            {
                                Success = false,
                                Message = StrHetSoLuong,
                                Data = ""
                            });
                        }
                            
                    }

                    _context.Entry(Output).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
                transaction.Commit();

                v_ct_PhieuXuat ct_PhieuXuat = new v_ct_PhieuXuat();
                ct_PhieuXuat.lstct_PhieuXuat_ChiTiet = new List<v_ct_PhieuXuat_ChiTiet>();
                SP_Parameter SP_Parameter = new SP_Parameter();
                SP_Parameter.ID_PHIEUXUAT = Output.ID;
                ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
                var actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuXuat(SP_Parameter);
                var okResult = actionResult as OkObjectResult;
                if (okResult != null)
                {
                    var ApiResponse = okResult.Value as ApiResponse;
                    if (ApiResponse != null)
                    {
                        if (ApiResponse.Data != null)
                        {
                            var lstPhieuDatHang = ApiResponse.Data as List<v_ct_PhieuXuat>;
                            if (lstPhieuDatHang != null && lstPhieuDatHang.Count() > 0)
                            {
                                ct_PhieuXuat = lstPhieuDatHang.FirstOrDefault() ?? new v_ct_PhieuXuat();
                            }

                        }

                    }
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = ct_PhieuXuat
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

        // POST: api/Output
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<ct_PhieuXuat>> PostOutput([FromBody] v_ct_PhieuXuat Output)
        {
            try
            {
                if (OutputExistsID(Output.LOC_ID, Output.ID))
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + Output.LOC_ID + "-" + Output.ID + " trong dữ liệu!",
                        Data = "",
                        CheckValue = true
                    });
                }
                var objPhieuNhap = await _context.ct_PhieuXuat!.FirstOrDefaultAsync(e => e.LOC_ID == Output.LOC_ID && e.MAPHIEU == Output.MAPHIEU);
                if (objPhieuNhap != null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Đã tồn tại" + Output.LOC_ID + "-" + Output.MAPHIEU + " trong dữ liệu!",
                        Data = "",
                        CheckValue = true
                    });
                }
              
                using var transaction = _context.Database.BeginTransaction();
                {
                    if (Output.lstct_PhieuXuat_ChiTiet != null)
                    {
                        string StrHetSoLuong = "";
                        foreach (var itm in Output.lstct_PhieuXuat_ChiTiet)
                        {
                            itm.THANHTIEN = itm.SOLUONG * itm.DONGIA;
                            itm.TONGCONG = itm.THANHTIEN - itm.TONGTIENGIAMGIA + itm.TONGTIENVAT;
                            //Output.TONGTIENTINHTHUE += itm.TONGTIENGIAMGIA;
                            var objdm_HangHoa_Kho = await _context.dm_HangHoa_Kho!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO && e.ID_KHO == Output.ID_KHO);
                            if (objdm_HangHoa_Kho != null)
                            {
                                var objdm_HangHoa = _context.view_dm_HangHoa!.FirstOrDefault(e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOA);
                                itm.TONGSOLUONG = itm.SOLUONG * itm.TYLE_QD;
                                if (objdm_HangHoa_Kho.QTY >= itm.TONGSOLUONG)
                                {
                                    //if (objdm_HangHoa != null)
                                    //{
                                    //    Output.TONGTIENTINHTHUE += (objdm_HangHoa.ID_DVT == itm.ID_DVT ? objdm_HangHoa.GIA01 : objdm_HangHoa.GIA01_QD) * itm.SOLUONG;
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
                            var objct_PhieuXuat_ChiTiet = await _context.ct_PhieuXuat_ChiTiet!.FirstOrDefaultAsync(e => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID);
                            if (objct_PhieuXuat_ChiTiet != null)
                                itm.ID = Guid.NewGuid().ToString();

                            itm.LOC_ID = Output.LOC_ID;
                            itm.ID_PHIEUXUAT = Output.ID;
                            _context.ct_PhieuXuat_ChiTiet!.Add(itm);
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
                        Output.TONGTHANHTIEN = Math.Round(Output.lstct_PhieuXuat_ChiTiet.Sum(s => s.THANHTIEN),0);
                        Output.TONGTIENGIAMGIA = Math.Round(Output.lstct_PhieuXuat_ChiTiet.Sum(s => s.TONGTIENGIAMGIA), 0);
                        Output.TONGTIENVAT = Math.Round(Output.lstct_PhieuXuat_ChiTiet.Sum(s => s.TONGTIENVAT), 0);
                        Output.TONGTIEN = Math.Round(Output.lstct_PhieuXuat_ChiTiet.Sum(s => s.TONGCONG), 0);
                    }
                    
                    bool bolCheckMA = false;
                    while (!bolCheckMA)
                    {
                        //Output.MAPHIEU = API.GetMaPhieu(API.ct_PhieuXuat, Output.NGAYLAP, Output.SOPHIEU);
                        var check = _context.ct_PhieuXuat!.Where(e => e.LOC_ID == Output.LOC_ID && e.MAPHIEU == Output.MAPHIEU).FirstOrDefault();
                        if (check != null)
                        {
                            Output.MAPHIEU = API.GetMaPhieu(API.ct_PhieuXuat, Output.NGAYLAP, Output.SOPHIEU);
                        }
                        else
                        {
                            bolCheckMA = true;
                        }
                    }
                    _context.ct_PhieuXuat!.Add(Output);
                    AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                }
                
                transaction.Commit();

                v_ct_PhieuXuat ct_PhieuXuat = new v_ct_PhieuXuat();
                ct_PhieuXuat.lstct_PhieuXuat_ChiTiet = new List<v_ct_PhieuXuat_ChiTiet>();
                SP_Parameter SP_Parameter = new SP_Parameter();
                SP_Parameter.ID_PHIEUXUAT = Output.ID;
                ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
                var actionResult = await ExecuteStoredProc1.Sp_Get_DanhSachPhieuXuat(SP_Parameter);
                var okResult = actionResult as OkObjectResult;
                if (okResult != null)
                {
                    var ApiResponse = okResult.Value as ApiResponse;
                    if (ApiResponse != null)
                    {

                        if (ApiResponse.Data != null)
                        {
                            var lstPhieuDatHang = ApiResponse.Data as List<v_ct_PhieuXuat>;
                            if (lstPhieuDatHang != null && lstPhieuDatHang.Count() > 0)
                            {
                                ct_PhieuXuat = lstPhieuDatHang.FirstOrDefault() ?? new v_ct_PhieuXuat();
                            }
                        }
                    }
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = ct_PhieuXuat
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
            finally
            {
                AuditLogController auditLog = new AuditLogController(_context, _configuration);
                auditLog.DeleteRequest(strTable);
            }
        }

        // DELETE: api/Output/5
        [HttpDelete("{LOC_ID}/{ID}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeleteOutput(string LOC_ID, string ID)
        {
            try
            {
                var Output = await _context.ct_PhieuXuat!.FirstOrDefaultAsync(e => e.LOC_ID == LOC_ID && e.ID == ID);
                if (Output == null)
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
                    var lstPhieuGiaoHang_ChiTiet = await _context.ct_PhieuGiaoHang_ChiTiet!.Where(e => e.LOC_ID == Output.LOC_ID && e.ID_PHIEUXUAT == Output.ID).ToListAsync();
                    if (lstPhieuGiaoHang_ChiTiet != null && lstPhieuGiaoHang_ChiTiet.Count() > 0)
                    {
                        String ChungTu = "(" + String.Join(";", lstPhieuGiaoHang_ChiTiet.Select(e => e.ID_PHIEUGIAOHANG)) + ")";
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Có 'Phiếu Giao Hàng " + ChungTu + "' liên quan tới " + Output.MAPHIEU + "!",
                            Data = ""
                        });
                    }
                    var lstPhieuChi = await _context.ct_PhieuChi!.Where(e => e.LOC_ID == Output.LOC_ID && e.CHUNGTUKEMTHEO == Output.MAPHIEU).ToListAsync();
                    if (lstPhieuChi != null && lstPhieuChi.Count() > 0)
                    {
                        String ChungTu = "(" + String.Join(";", lstPhieuChi.Select(e => e.MAPHIEU)) + ")";
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Có 'Phiếu Chi " + ChungTu + "' liên quan tới " + Output.MAPHIEU + "!",
                            Data = ""
                        });
                    }
                    var lstPhieuThu = await _context.ct_PhieuThu!.Where(e => e.LOC_ID == Output.LOC_ID && e.CHUNGTUKEMTHEO == Output.MAPHIEU).ToListAsync();
                    if (lstPhieuThu != null && lstPhieuThu.Count() > 0)
                    {
                        String ChungTu = "(" + String.Join(";", lstPhieuThu.Select(e => e.MAPHIEU)) + ")";
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Có 'Phiếu Thu "+ ChungTu + "' liên quan tới " + Output.MAPHIEU + "!",
                            Data = ""
                        });
                    }
                    var lstPhieuNhap = await _context.ct_PhieuNhap!.Where(e => e.LOC_ID == Output.LOC_ID && e.CHUNGTUKEMTHEO == Output.MAPHIEU).ToListAsync();
                    if (lstPhieuNhap != null && lstPhieuNhap.Count() > 0)
                    {
                        String ChungTu = "(" + String.Join(";", lstPhieuNhap.Select(e => e.MAPHIEU)) + ")";
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Có 'Phiếu Nhập "+ ChungTu + "' liên quan tới " + Output.MAPHIEU + "!",
                            Data = ""
                        });
                    }
                    var lstPhieuNhap_ChiTiet = await _context.ct_PhieuXuat_ChiTiet!.Where(e => e.LOC_ID == Output.LOC_ID && e.ID_PHIEUXUAT == Output.ID).ToListAsync();
                    if (lstPhieuNhap_ChiTiet != null)
                    {
                        foreach (ct_PhieuXuat_ChiTiet itm in lstPhieuNhap_ChiTiet)
                        {
                            if (!Output.ISPHIEUDIEUHANG)
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
                            }
                            _context.ct_PhieuXuat_ChiTiet!.Remove(itm);
                        }
                    }
                    var lstPhieuDatHang = await _context.ct_PhieuDatHang!.Where(e => e.LOC_ID == Output.LOC_ID && e.ID_PHIEUXUAT == Output.ID).ToListAsync();
                    foreach(var itm in lstPhieuDatHang)
                    {
                        itm.ID_PHIEUXUAT = "";
                    }
                    _context.ct_PhieuXuat!.Remove(Output);
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
        private string strTable = "ct_PhieuXuat";
        private bool OutputExistsID(string LOC_ID, string ID)
        {
            //bool bolCheckMA = false;
            //while (!bolCheckMA)
            //{
            //    var check = _context.AspNetRequest!.Where(e => e.LOC_ID == LOC_ID && e.NAME == strTable).FirstOrDefault();
            //    if (check != null)
            //    {
            //        if (check.THOIGIAN < DateTime.Now.AddSeconds(-5))
            //        {
            //            _context.AspNetRequest!.Remove(check);
            //            _context.SaveChanges();
            //        }
            //    }
            //    else
            //    {
            //        AspNetRequest newAspNetRequest = new AspNetRequest();
            //        newAspNetRequest.ID = ID;
            //        newAspNetRequest.NAME = strTable;
            //        newAspNetRequest.THOIGIAN = DateTime.Now;
            //        newAspNetRequest.LOC_ID = LOC_ID;
            //        _context.AspNetRequest!.Add(newAspNetRequest);
            //        _context.SaveChanges();
            //        bolCheckMA = true;
            //    }
            //}
            return _context.ct_PhieuXuat!.Any(e => e.LOC_ID == LOC_ID && e.ID == ID);
        }
    }
}
