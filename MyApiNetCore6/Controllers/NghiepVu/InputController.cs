using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using API_QuanLyTHP.Class;
using Dapper;
using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.StoredProcedure.Parameter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using MyApiNetCore6.Data;
using Newtonsoft.Json;

namespace MyApiNetCore6.Controllers
{

	[Route("api/[controller]")]
	[ApiController]
	public class InputController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		private readonly string _connectionString;

		private string strTable = "ct_PhieuNhap";

		public InputController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
			_connectionString = configuration.GetConnectionString("TrangHiepPhat");
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetInput(string LOC_ID)
		{
			try
			{
				List<ct_PhieuNhap> lstValue = await _context.ct_PhieuNhap.Where((ct_PhieuNhap e) => e.LOC_ID == LOC_ID).ToListAsync();
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = lstValue
				});
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				return Ok(new ApiResponse
				{
					Success = false,
					Message = ex2.Message,
					Data = ""
				});
			}
		}

		[HttpGet("{LOC_ID}/{Type}/{KeyWhere}/{ValuesSearch}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetInput(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				List<ct_PhieuNhap> lstValue = await _context.ct_PhieuNhap.Where((ct_PhieuNhap e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).ToListAsync();
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = lstValue
				});
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				return Ok(new ApiResponse
				{
					Success = false,
					Message = ex2.Message,
					Data = ""
				});
			}
		}

		[HttpGet("{LOC_ID}/{ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetInput(string LOC_ID, string ID)
		{
			try
			{
				ct_PhieuNhap Input = await _context.ct_PhieuNhap.FirstOrDefaultAsync((ct_PhieuNhap e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Input == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				v_ct_PhieuNhap ct_PhieuNhap2 = new v_ct_PhieuNhap();
				if (Input != null)
				{
					string strInput = JsonConvert.SerializeObject(Input);
					ct_PhieuNhap2 = JsonConvert.DeserializeObject<v_ct_PhieuNhap>(strInput) ?? new v_ct_PhieuNhap();
				}
				ct_PhieuNhap2.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuNhap_ChiTiet>();
				SP_Parameter SP_Parameter = new SP_Parameter
				{
					ID_PHIEUNHAP = ID
				};
				ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
				if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuNhap_Chitiet(SP_Parameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse })
				{
					List<v_ct_PhieuNhap_ChiTiet> lst_ChiTiet = ApiResponse.Data as List<v_ct_PhieuNhap_ChiTiet>;
					if (lst_ChiTiet != null)
					{
						ct_PhieuNhap2.lstct_PhieuNhap_ChiTiet.AddRange(lst_ChiTiet);
					}
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ct_PhieuNhap2
				});
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				return Ok(new ApiResponse
				{
					Success = false,
					Message = ex2.Message,
					Data = ""
				});
			}
		}

		[HttpPut("{LOC_ID}/{ID}")]
		[Authorize(Roles = "User")]
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
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				List<ct_PhieuNhap_ChiTiet> lstPhieuNhap_ChiTiet = await _context.ct_PhieuNhap_ChiTiet.Where((ct_PhieuNhap_ChiTiet e) => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUNHAP == Input.ID).ToListAsync();
				if (lstPhieuNhap_ChiTiet != null)
				{
					foreach (ct_PhieuNhap_ChiTiet itm in lstPhieuNhap_ChiTiet)
					{
						if (!string.IsNullOrEmpty(itm.ID_THUESUAT))
						{
							dm_ThueSuat objVAT = _context.dm_ThueSuat.FirstOrDefault((dm_ThueSuat e) => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO && e.ID == itm.ID_THUESUAT);
							itm.THUESUAT = objVAT?.THUESUAT ?? itm.THUESUAT;
						}
						dm_HangHoa_Kho objdm_HangHoa_Kho = _context.dm_HangHoa_Kho.FromSqlRaw("\r\n                                        SELECT *\r\n                                        FROM dm_HangHoa_Kho WITH (UPDLOCK, ROWLOCK)\r\n                                        WHERE LOC_ID = @loc\r\n                                          AND ID = @id\r\n                                    ", new SqlParameter("@loc", itm.LOC_ID), new SqlParameter("@id", itm.ID_HANGHOAKHO)).AsTracking().FirstOrDefault();
						if (objdm_HangHoa_Kho != null)
						{
							itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
							objdm_HangHoa_Kho.QTY -= itm.TONGSOLUONG;
							_context.Entry(objdm_HangHoa_Kho).State = EntityState.Modified;
							v_ct_PhieuNhap_ChiTiet chkPhieuNhap_ChiTiet = Input.lstct_PhieuNhap_ChiTiet.Where((v_ct_PhieuNhap_ChiTiet e) => e.ID == itm.ID).FirstOrDefault();
							if (chkPhieuNhap_ChiTiet != null)
							{
								chkPhieuNhap_ChiTiet.ISEDIT = true;
								chkPhieuNhap_ChiTiet.ID_PHIEUNHAP = Input.ID;
								_context.Entry(objdm_HangHoa_Kho).State = EntityState.Modified;
							}
							else
							{
								_context.ct_PhieuNhap_ChiTiet.Remove(itm);
							}
							continue;
						}
						return Ok(new ApiResponse
						{
							Success = false,
							Message = "Không tìm thấy sản phẩm kho!" + itm.ID_HANGHOAKHO,
							Data = ""
						});
					}
				}
				if (Input.lstct_PhieuNhap_ChiTiet != null)
				{
					foreach (v_ct_PhieuNhap_ChiTiet itm2 in Input.lstct_PhieuNhap_ChiTiet)
					{
						if (!string.IsNullOrEmpty(itm2.ID_THUESUAT))
						{
							dm_ThueSuat objVAT2 = _context.dm_ThueSuat.FirstOrDefault((dm_ThueSuat e) => e.LOC_ID == itm2.LOC_ID && e.ID == itm2.ID_HANGHOAKHO && e.ID == itm2.ID_THUESUAT);
							itm2.THUESUAT = objVAT2?.THUESUAT ?? itm2.THUESUAT;
						}
						itm2.THANHTIEN = itm2.SOLUONG * itm2.DONGIA - itm2.TONGTIENGIAMGIA;
						itm2.TONGCONG = itm2.THANHTIEN + itm2.TONGTIENVAT;
						itm2.ID_PHIEUNHAP = Input.ID;
						dm_HangHoa_Kho objdm_HangHoa_Kho2 = _context.dm_HangHoa_Kho.FromSqlRaw("\r\n                                        SELECT *\r\n                                        FROM dm_HangHoa_Kho WITH (UPDLOCK, ROWLOCK)\r\n                                        WHERE LOC_ID = @loc\r\n                                          AND ID = @id\r\n                                    ", new SqlParameter("@loc", itm2.LOC_ID), new SqlParameter("@id", itm2.ID_HANGHOAKHO)).AsTracking().FirstOrDefault();
						if (objdm_HangHoa_Kho2 != null)
						{
							itm2.TONGSOLUONG = itm2.TYLE_QD * itm2.SOLUONG;
							objdm_HangHoa_Kho2.QTY += itm2.TONGSOLUONG;
							_context.Entry(objdm_HangHoa_Kho2).State = EntityState.Modified;
							if (!itm2.ISEDIT)
							{
								_context.ct_PhieuNhap_ChiTiet.Add(itm2);
							}
							else
							{
								var stored = lstPhieuNhap_ChiTiet.Single(x => x.ID == itm2.ID);
								_context.Entry(stored).CurrentValues.SetValues(itm2);
							}
							continue;
						}
						return Ok(new ApiResponse
						{
							Success = false,
							Message = "Không tìm thấy sản phẩm kho!" + itm2.ID_HANGHOAKHO,
							Data = ""
						});
					}
					Input.TONGTHANHTIEN = Math.Round(Input.lstct_PhieuNhap_ChiTiet.Sum((v_ct_PhieuNhap_ChiTiet s) => s.THANHTIEN), 0);
					Input.TONGTIENGIAMGIA = Math.Round(Input.lstct_PhieuNhap_ChiTiet.Sum((v_ct_PhieuNhap_ChiTiet s) => s.TONGTIENGIAMGIA), 0);
					Input.TONGTIENVAT = Math.Round(Input.lstct_PhieuNhap_ChiTiet.Sum((v_ct_PhieuNhap_ChiTiet s) => s.TONGTIENVAT), 0);
					Input.TONGTIEN = Math.Round(Input.lstct_PhieuNhap_ChiTiet.Sum((v_ct_PhieuNhap_ChiTiet s) => s.TONGCONG), 0);
				}
				_context.Entry(Input).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await MyApiNetCore6.Services.StockCostPosting.SaveMovements(_context);
				transaction.Commit();
				ct_PhieuNhap InputCheck = await _context.ct_PhieuNhap.FirstOrDefaultAsync((ct_PhieuNhap e) => e.LOC_ID == Input.LOC_ID && e.ID == Input.ID);
				using IDbContextTransaction transaction2 = _context.Database.BeginTransaction();
				if (InputCheck != null && InputCheck.CHUNGTUKEMTHEO != Input.CHUNGTUKEMTHEO)
				{
					if (InputCheck.CHUNGTUKEMTHEO != null && InputCheck.CHUNGTUKEMTHEO.StartsWith("PX-"))
					{
						ct_PhieuXuat PhieuXuat = await _context.ct_PhieuXuat.FirstOrDefaultAsync((ct_PhieuXuat e) => e.LOC_ID == InputCheck.LOC_ID && e.MAPHIEU == InputCheck.CHUNGTUKEMTHEO);
						if (PhieuXuat != null)
						{
							ct_PhieuGiaoHang_ChiTiet ct_PhieuGiaoHang_ChiTiet2 = await _context.ct_PhieuGiaoHang_ChiTiet.FirstOrDefaultAsync((ct_PhieuGiaoHang_ChiTiet e) => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUXUAT == PhieuXuat.ID);
							if (ct_PhieuGiaoHang_ChiTiet2 != null)
							{
								await _context.ct_PhieuThu.Where((ct_PhieuThu e) => e.LOC_ID == InputCheck.LOC_ID && e.CHUNGTUKEMTHEO == InputCheck.CHUNGTUKEMTHEO).SumAsync((ct_PhieuThu e) => e.SOTIEN);
								ct_PhieuGiaoHang_ChiTiet2.ISTRAHANG = false;
								_context.Entry(ct_PhieuGiaoHang_ChiTiet2).State = EntityState.Modified;
							}
						}
					}
					if (Input.CHUNGTUKEMTHEO != null && Input.CHUNGTUKEMTHEO.StartsWith("PX-"))
					{
						ct_PhieuXuat PhieuXuat2 = await _context.ct_PhieuXuat.FirstOrDefaultAsync((ct_PhieuXuat e) => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO);
						if (PhieuXuat2 != null)
						{
							ct_PhieuGiaoHang_ChiTiet ct_PhieuGiaoHang_ChiTiet3 = await _context.ct_PhieuGiaoHang_ChiTiet.FirstOrDefaultAsync((ct_PhieuGiaoHang_ChiTiet e) => e.LOC_ID == PhieuXuat2.LOC_ID && e.ID_PHIEUXUAT == PhieuXuat2.ID);
							if (ct_PhieuGiaoHang_ChiTiet3 != null)
							{
								ct_PhieuGiaoHang_ChiTiet3.ISTRAHANG = true;
								_context.Entry(ct_PhieuGiaoHang_ChiTiet3).State = EntityState.Modified;
							}
						}
					}
					AuditLogController auditLog2 = new AuditLogController(_context, _configuration);
					auditLog2.InserAuditLog();
					await _context.SaveChangesAsync();
				}
				transaction2.Commit();
				v_ct_PhieuNhap ct_PhieuNhap2 = new v_ct_PhieuNhap
				{
					lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuNhap_ChiTiet>()
				};
				SP_Parameter SP_Parameter = new SP_Parameter
				{
					ID_PHIEUNHAP = Input.ID
				};
				ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
				if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuNhap(SP_Parameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse })
				{
					List<v_ct_PhieuNhap> lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuNhap>;
					if (lstPhieuNhap != null && lstPhieuNhap.Count() > 0)
					{
						ct_PhieuNhap2 = lstPhieuNhap.FirstOrDefault() ?? new v_ct_PhieuNhap();
					}
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ct_PhieuNhap2
				});
			}
			catch (DbUpdateConcurrencyException ex)
			{
				DbUpdateConcurrencyException ex2 = ex;
				return Ok(new ApiResponse
				{
					Success = false,
					Message = ex2.Message,
					Data = ""
				});
			}
			finally
			{
				AuditLogController auditLog3 = new AuditLogController(_context, _configuration);
				auditLog3.DeleteRequest(strTable);
			}
		}

		[HttpPost]
		[Authorize(Roles = "User")]
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
				if (await _context.ct_PhieuNhap.FirstOrDefaultAsync((ct_PhieuNhap e) => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.MAPHIEU) != null)
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
					ct_PhieuDatHangNCC ct_PhieuDatHangNCC2 = await _context.ct_PhieuDatHangNCC.FirstOrDefaultAsync((ct_PhieuDatHangNCC e) => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO);
					if (ct_PhieuDatHangNCC2?.ISHOANTAT ?? false)
					{
						return Ok(new ApiResponse
						{
							Success = false,
							Message = "Phiếu " + ct_PhieuDatHangNCC2.MAPHIEU + " đã hoàn thành! Vui lòng kiểm tra lại!",
							Data = "",
							CheckValue = true
						});
					}
				}
				ct_PhieuDatHangNCC objct_PhieuDatHangNCC = new ct_PhieuDatHangNCC();
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				if (Input.lstct_PhieuNhap_ChiTiet != null)
				{
					foreach (v_ct_PhieuNhap_ChiTiet itm in Input.lstct_PhieuNhap_ChiTiet)
					{
						itm.THANHTIEN = itm.SOLUONG * itm.DONGIA - itm.TONGTIENGIAMGIA;
						itm.TONGCONG = itm.THANHTIEN + itm.TONGTIENVAT;
						dm_HangHoa_Kho objdm_HangHoa_Kho = _context.dm_HangHoa_Kho.FromSqlRaw("\r\n                                        SELECT *\r\n                                        FROM dm_HangHoa_Kho WITH (UPDLOCK, ROWLOCK)\r\n                                        WHERE LOC_ID = @loc\r\n                                          AND ID = @id\r\n                                    ", new SqlParameter("@loc", itm.LOC_ID), new SqlParameter("@id", itm.ID_HANGHOAKHO)).AsTracking().FirstOrDefault();
						if (objdm_HangHoa_Kho != null)
						{
							itm.TONGSOLUONG = itm.SOLUONG * itm.TYLE_QD;
							objdm_HangHoa_Kho.QTY += itm.TONGSOLUONG;
							_context.Entry(objdm_HangHoa_Kho).State = EntityState.Modified;
							itm.LOC_ID = Input.LOC_ID;
							itm.ID_PHIEUNHAP = Input.ID;
							_context.ct_PhieuNhap_ChiTiet.Add(itm);
							continue;
						}
						return Ok(new ApiResponse
						{
							Success = false,
							Message = "Không tìm thấy sản phẩm kho!" + itm.ID_HANGHOAKHO,
							Data = ""
						});
					}
					Input.TONGTHANHTIEN = Math.Round(Input.lstct_PhieuNhap_ChiTiet.Sum((v_ct_PhieuNhap_ChiTiet s) => s.THANHTIEN), 0);
					Input.TONGTIENGIAMGIA = Math.Round(Input.lstct_PhieuNhap_ChiTiet.Sum((v_ct_PhieuNhap_ChiTiet s) => s.TONGTIENGIAMGIA), 0);
					Input.TONGTIENVAT = Math.Round(Input.lstct_PhieuNhap_ChiTiet.Sum((v_ct_PhieuNhap_ChiTiet s) => s.TONGTIENVAT), 0);
					Input.TONGTIEN = Math.Round(Input.lstct_PhieuNhap_ChiTiet.Sum((v_ct_PhieuNhap_ChiTiet s) => s.TONGCONG), 0);
				}
				bool bolCheckMA = false;
				while (!bolCheckMA)
				{
					ct_PhieuNhap check = _context.ct_PhieuNhap.Where((ct_PhieuNhap e) => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.MAPHIEU).FirstOrDefault();
					if (check != null)
					{
						Input.MAPHIEU = API.GetMaPhieu("Input", Input.NGAYLAP, Input.SOPHIEU);
					}
					else
					{
						bolCheckMA = true;
					}
				}
				_context.ct_PhieuNhap.Add(Input);
				if (Input.CHUNGTUKEMTHEO != null && Input.CHUNGTUKEMTHEO.StartsWith("PX-"))
				{
					ct_PhieuXuat PhieuXuat = await _context.ct_PhieuXuat.FirstOrDefaultAsync((ct_PhieuXuat e) => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO);
					if (PhieuXuat != null)
					{
						ct_PhieuGiaoHang_ChiTiet ct_PhieuGiaoHang_ChiTiet2 = await _context.ct_PhieuGiaoHang_ChiTiet.FirstOrDefaultAsync((ct_PhieuGiaoHang_ChiTiet e) => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUXUAT == PhieuXuat.ID);
						if (ct_PhieuGiaoHang_ChiTiet2 != null)
						{
							ct_PhieuGiaoHang_ChiTiet2.ISTRAHANG = true;
							_context.Entry(ct_PhieuGiaoHang_ChiTiet2).State = EntityState.Modified;
						}
					}
				}
				if (Input.CHUNGTUKEMTHEO != null && Input.CHUNGTUKEMTHEO.StartsWith("NCC-"))
				{
					objct_PhieuDatHangNCC = await _context.ct_PhieuDatHangNCC.FirstOrDefaultAsync((ct_PhieuDatHangNCC e) => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO);
					if (objct_PhieuDatHangNCC != null)
					{
						List<ct_PhieuNhap> lstPhieuNhap = await _context.ct_PhieuNhap.Where((ct_PhieuNhap e) => e.LOC_ID == objct_PhieuDatHangNCC.LOC_ID && e.CHUNGTUKEMTHEO == objct_PhieuDatHangNCC.MAPHIEU).ToListAsync();
						string ChungTu = "";
						if (lstPhieuNhap != null && lstPhieuNhap.Count() > 0)
						{
							ChungTu = string.Join(";", lstPhieuNhap.Select((ct_PhieuNhap e) => e.MAPHIEU));
						}
						objct_PhieuDatHangNCC.CHUNGTUKEMTHEO = "(" + Input.MAPHIEU + (string.IsNullOrEmpty(ChungTu) ? "" : ";") + ChungTu + ")";
						_context.Entry(objct_PhieuDatHangNCC).State = EntityState.Modified;
					}
				}
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await MyApiNetCore6.Services.StockCostPosting.SaveMovements(_context);
				transaction.Commit();
				List<ct_PhieuNhap> lstPhieuDatHangCheck = await (from e in _context.ct_PhieuNhap
																 where e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.MAPHIEU
																 orderby e.NGAYLAP descending
																 select e).ToListAsync();
				if (lstPhieuDatHangCheck != null && lstPhieuDatHangCheck.Count() > 1 && lstPhieuDatHangCheck.FirstOrDefault().ID == Input.ID)
				{
					int Max_ID = (from e in _context.ct_PhieuNhap
								  where e.LOC_ID == Input.LOC_ID && e.NGAYLAP.Date == Input.NGAYLAP.Date
								  select e.SOPHIEU).DefaultIfEmpty().Max();
					Input.SOPHIEU = Max_ID + 1;
					Input.MAPHIEU = API.GetMaPhieu("Input", Input.NGAYLAP, Input.SOPHIEU);
					_context.Entry(Input).State = EntityState.Modified;
					await _context.SaveChangesAsync();
				}
				if (Input.CHUNGTUKEMTHEO != null && Input.CHUNGTUKEMTHEO.StartsWith("NCC-") && objct_PhieuDatHangNCC != null && !string.IsNullOrEmpty(objct_PhieuDatHangNCC.ID))
				{
					IEnumerable<QuantityCheckResult> result = GetQuantityCheck(Input.CHUNGTUKEMTHEO);
					using IDbContextTransaction transaction2 = _context.Database.BeginTransaction();
					if (result != null)
					{
						int TongSo = result.Sum((QuantityCheckResult s) => s.Status);
						ct_PhieuDatHangNCC OKCar = await _context.ct_PhieuDatHangNCC.FirstOrDefaultAsync((ct_PhieuDatHangNCC e) => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO);
						if (OKCar != null)
						{
							OKCar.ISHOANTAT = TongSo <= 0;
							_context.Entry(OKCar).State = EntityState.Modified;
							AuditLogController auditLog2 = new AuditLogController(_context, _configuration);
							auditLog2.InserAuditLog();
							await _context.SaveChangesAsync();
						}
					}
					transaction2.Commit();
					v_ct_PhieuDatHangNCC ct_PhieuNhap2 = new v_ct_PhieuDatHangNCC
					{
						lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuDatHangNCC_ChiTiet>()
					};
					SP_Parameter SP_Parameter = new SP_Parameter
					{
						ID_PHIEUNHAP = objct_PhieuDatHangNCC.ID
					};
					ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
					if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuDatHangNCC(SP_Parameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse })
					{
						List<v_ct_PhieuDatHangNCC> lstPhieuNhap2 = ApiResponse.Data as List<v_ct_PhieuDatHangNCC>;
						if (lstPhieuNhap2 != null && lstPhieuNhap2.Count() > 0)
						{
							ct_PhieuNhap2 = lstPhieuNhap2.FirstOrDefault() ?? new v_ct_PhieuDatHangNCC();
						}
					}
					return Ok(new ApiResponse
					{
						Success = true,
						Message = "Success",
						Data = ct_PhieuNhap2
					});
				}
				v_ct_PhieuNhap ct_PhieuNhap3 = new v_ct_PhieuNhap
				{
					lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuNhap_ChiTiet>()
				};
				SP_Parameter SP_Parameter2 = new SP_Parameter
				{
					ID_PHIEUNHAP = Input.ID
				};
				ExecuteStoredProc ExecuteStoredProc2 = new ExecuteStoredProc(_context, _configuration);
				if (await ExecuteStoredProc2.Sp_Get_DanhSachPhieuNhap(SP_Parameter2) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse2 })
				{
					List<v_ct_PhieuNhap> lstPhieuNhap3 = ApiResponse2.Data as List<v_ct_PhieuNhap>;
					if (lstPhieuNhap3 != null && lstPhieuNhap3.Count() > 0)
					{
						ct_PhieuNhap3 = lstPhieuNhap3.FirstOrDefault() ?? new v_ct_PhieuNhap();
					}
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ct_PhieuNhap3
				});
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				return Ok(new ApiResponse
				{
					Success = false,
					Message = ex2.Message,
					Data = ""
				});
			}
			finally
			{
				AuditLogController auditLog3 = new AuditLogController(_context, _configuration);
				auditLog3.DeleteRequest(strTable);
			}
		}

		[HttpPut("{MAPHIEU}")]
		public IEnumerable<QuantityCheckResult> GetQuantityCheck(string MAPHIEU)
		{
			using SqlConnection cnn = new SqlConnection(_connectionString);
			var param = new { MAPHIEU };
			CommandType? commandType = CommandType.StoredProcedure;
			return cnn.Query<QuantityCheckResult>("CheckPhieuDatHang", param, null, buffered: true, null, commandType);
		}

		[HttpDelete("{LOC_ID}/{ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> DeleteInput(string LOC_ID, string ID)
		{
			try
			{
				ct_PhieuNhap Input = await _context.ct_PhieuNhap.FirstOrDefaultAsync((ct_PhieuNhap e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Input == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				List<ct_PhieuThu> lstPhieuThu = await _context.ct_PhieuThu.Where((ct_PhieuThu e) => e.LOC_ID == Input.LOC_ID && e.CHUNGTUKEMTHEO == Input.MAPHIEU).ToListAsync();
				if (lstPhieuThu != null && lstPhieuThu.Count() > 0)
				{
					string ChungTu = "(" + string.Join(";", lstPhieuThu.Select((ct_PhieuThu e) => e.MAPHIEU)) + ")";
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Có 'Phiếu Thu " + ChungTu + "' liên quan tới " + Input.MAPHIEU + "!",
						Data = ""
					});
				}
				List<ct_PhieuChi> lstPhieuChi = await _context.ct_PhieuChi.Where((ct_PhieuChi e) => e.LOC_ID == Input.LOC_ID && e.CHUNGTUKEMTHEO == Input.MAPHIEU).ToListAsync();
				if (lstPhieuChi != null && lstPhieuChi.Count() > 0)
				{
					string ChungTu2 = "(" + string.Join(";", lstPhieuChi.Select((ct_PhieuChi e) => e.MAPHIEU)) + ")";
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Có 'Phiếu Chi " + ChungTu2 + "' liên quan tới " + Input.MAPHIEU + "!",
						Data = ""
					});
				}
				List<ct_PhieuXuat> lstPhieuXuat = await _context.ct_PhieuXuat.Where((ct_PhieuXuat e) => e.LOC_ID == Input.LOC_ID && e.CHUNGTUKEMTHEO == Input.MAPHIEU).ToListAsync();
				if (lstPhieuXuat != null && lstPhieuXuat.Count() > 0)
				{
					string ChungTu3 = "(" + string.Join(";", lstPhieuXuat.Select((ct_PhieuXuat e) => e.MAPHIEU)) + ")";
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Có 'Phiếu Xuất " + ChungTu3 + "' liên quan tới " + Input.MAPHIEU + "!",
						Data = ""
					});
				}
				List<ct_PhieuNhap_ChiTiet> lstPhieuNhap_ChiTiet = await _context.ct_PhieuNhap_ChiTiet.Where((ct_PhieuNhap_ChiTiet e) => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUNHAP == Input.ID).ToListAsync();
				if (lstPhieuNhap_ChiTiet != null)
				{
					foreach (ct_PhieuNhap_ChiTiet itm in lstPhieuNhap_ChiTiet)
					{
						dm_HangHoa_Kho objdm_HangHoa_Kho = _context.dm_HangHoa_Kho.FromSqlRaw("\r\n                                        SELECT *\r\n                                        FROM dm_HangHoa_Kho WITH (UPDLOCK, ROWLOCK)\r\n                                        WHERE LOC_ID = @loc\r\n                                          AND ID = @id\r\n                                    ", new SqlParameter("@loc", itm.LOC_ID), new SqlParameter("@id", itm.ID_HANGHOAKHO)).AsTracking().FirstOrDefault();
						if (objdm_HangHoa_Kho != null)
						{
							itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
							objdm_HangHoa_Kho.QTY -= itm.TONGSOLUONG;
							_context.Entry(objdm_HangHoa_Kho).State = EntityState.Modified;
							_context.ct_PhieuNhap_ChiTiet.Remove(itm);
							continue;
						}
						return Ok(new ApiResponse
						{
							Success = false,
							Message = "Không tìm thấy sản phẩm kho!" + itm.ID_HANGHOAKHO,
							Data = ""
						});
					}
				}
				_context.ct_PhieuNhap.Remove(Input);
				if (Input.CHUNGTUKEMTHEO != null && Input.CHUNGTUKEMTHEO.StartsWith("PX-"))
				{
					List<ct_PhieuXuat> PhieuXuat = await _context.ct_PhieuXuat.Where((ct_PhieuXuat e) => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO).ToListAsync();
					if (PhieuXuat != null && PhieuXuat.Count == 1)
					{
						ct_PhieuGiaoHang_ChiTiet ct_PhieuGiaoHang_ChiTiet2 = await _context.ct_PhieuGiaoHang_ChiTiet.FirstOrDefaultAsync((ct_PhieuGiaoHang_ChiTiet e) => e.LOC_ID == PhieuXuat[0].LOC_ID && e.ID_PHIEUXUAT == PhieuXuat[0].ID);
						if (ct_PhieuGiaoHang_ChiTiet2 != null)
						{
							ct_PhieuGiaoHang_ChiTiet2.ISTRAHANG = false;
							_context.Entry(ct_PhieuGiaoHang_ChiTiet2).State = EntityState.Modified;
						}
					}
				}
				if (Input.CHUNGTUKEMTHEO != null && Input.CHUNGTUKEMTHEO.StartsWith("NCC-"))
				{
					List<ct_PhieuDatHangNCC> PhieuXuat2 = await _context.ct_PhieuDatHangNCC.Where((ct_PhieuDatHangNCC e) => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO).ToListAsync();
					if (PhieuXuat2 != null && PhieuXuat2.Count > 0)
					{
						foreach (ct_PhieuDatHangNCC itm2 in PhieuXuat2)
						{
							List<ct_PhieuNhap> lstPhieuNhap = await _context.ct_PhieuNhap.Where((ct_PhieuNhap e) => e.LOC_ID == Input.LOC_ID && e.CHUNGTUKEMTHEO == Input.CHUNGTUKEMTHEO && e.ID != Input.ID).ToListAsync();
							itm2.CHUNGTUKEMTHEO = "";
							itm2.ISHOANTAT = false;
							if (lstPhieuNhap != null && lstPhieuNhap.Count() > 0)
							{
								string ChungTu4 = string.Join(";", lstPhieuNhap.Select((ct_PhieuNhap e) => e.MAPHIEU));
								itm2.CHUNGTUKEMTHEO = "(" + (string.IsNullOrEmpty(ChungTu4) ? "" : ";") + ChungTu4 + ")";
							}
							_context.Entry(itm2).State = EntityState.Modified;
						}
					}
				}
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await MyApiNetCore6.Services.StockCostPosting.SaveMovements(_context);
				transaction.Commit();
				if (Input != null && Input.CHUNGTUKEMTHEO != null && Input.CHUNGTUKEMTHEO.StartsWith("NCC-"))
				{
					IEnumerable<QuantityCheckResult> result = GetQuantityCheck(Input.CHUNGTUKEMTHEO);
					if (result != null)
					{
						int TongSo = result.Sum((QuantityCheckResult s) => s.Status);
						ct_PhieuDatHangNCC OKCar = await _context.ct_PhieuDatHangNCC.FirstOrDefaultAsync((ct_PhieuDatHangNCC e) => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.CHUNGTUKEMTHEO);
						if (OKCar != null)
						{
							OKCar.ISHOANTAT = TongSo <= 0;
							_context.Entry(OKCar).State = EntityState.Modified;
							AuditLogController auditLog2 = new AuditLogController(_context, _configuration);
							auditLog2.InserAuditLog();
							await _context.SaveChangesAsync();
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
				Exception ex2 = ex;
				return Ok(new ApiResponse
				{
					Success = false,
					Message = ex2.Message,
					Data = ""
				});
			}
		}

		private bool InputExistsID(string LOC_ID, string ID)
		{
			return _context.ct_PhieuNhap.Any((ct_PhieuNhap e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}
	}
}