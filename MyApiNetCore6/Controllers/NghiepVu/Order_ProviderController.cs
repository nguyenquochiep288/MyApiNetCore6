using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
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
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetInput(string LOC_ID)
		{
			try
			{
				List<ct_PhieuDatHangNCC> lstValue = await _context.ct_PhieuDatHangNCC.Where((ct_PhieuDatHangNCC e) => e.LOC_ID == LOC_ID).ToListAsync();
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
				List<ct_PhieuDatHangNCC> lstValue = await _context.ct_PhieuDatHangNCC.Where((ct_PhieuDatHangNCC e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).ToListAsync();
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
				ct_PhieuDatHangNCC Input = await _context.ct_PhieuDatHangNCC.FirstOrDefaultAsync((ct_PhieuDatHangNCC e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Input == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC2 = new v_ct_PhieuDatHangNCC();
				if (Input != null)
				{
					string strInput = JsonConvert.SerializeObject(Input);
					ct_PhieuDatHangNCC2 = JsonConvert.DeserializeObject<v_ct_PhieuDatHangNCC>(strInput) ?? new v_ct_PhieuDatHangNCC();
				}
				ct_PhieuDatHangNCC2.lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuDatHangNCC_ChiTiet>();
				SP_Parameter SP_Parameter = new SP_Parameter
				{
					ID_PHIEUNHAP = ID
				};
				ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
				if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuDatHangNCC_Chitiet(SP_Parameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse })
				{
					List<v_ct_PhieuDatHangNCC_ChiTiet> lst_ChiTiet = ApiResponse.Data as List<v_ct_PhieuDatHangNCC_ChiTiet>;
					if (lst_ChiTiet != null)
					{
						ct_PhieuDatHangNCC2.lstct_PhieuNhap_ChiTiet.AddRange(lst_ChiTiet);
					}
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ct_PhieuDatHangNCC2
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
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				List<ct_PhieuDatHangNCC_ChiTiet> lstPhieuNhap_ChiTiet = await _context.ct_PhieuDatHangNCC_ChiTiet.Where((ct_PhieuDatHangNCC_ChiTiet e) => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUDATHANGNCC == Input.ID).ToListAsync();
				if (lstPhieuNhap_ChiTiet != null)
				{
					foreach (ct_PhieuDatHangNCC_ChiTiet itm in lstPhieuNhap_ChiTiet)
					{
						dm_HangHoa_Kho objdm_HangHoa_Kho = await _context.dm_HangHoa_Kho.FirstOrDefaultAsync((dm_HangHoa_Kho e) => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO);
						if (objdm_HangHoa_Kho != null)
						{
							itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
							v_ct_PhieuDatHangNCC_ChiTiet chkPhieuNhap_ChiTiet = Input.lstct_PhieuNhap_ChiTiet.Where((v_ct_PhieuDatHangNCC_ChiTiet e) => e.ID == itm.ID).FirstOrDefault();
							if (chkPhieuNhap_ChiTiet != null)
							{
								chkPhieuNhap_ChiTiet.ISEDIT = true;
								chkPhieuNhap_ChiTiet.ID_PHIEUDATHANGNCC = Input.ID;
								_context.Entry(objdm_HangHoa_Kho).State = EntityState.Modified;
							}
							else
							{
								_context.ct_PhieuDatHangNCC_ChiTiet.Remove(itm);
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
					foreach (v_ct_PhieuDatHangNCC_ChiTiet itm2 in Input.lstct_PhieuNhap_ChiTiet)
					{
						itm2.ID_PHIEUDATHANGNCC = Input.ID;
						dm_HangHoa_Kho objdm_HangHoa_Kho2 = _context.dm_HangHoa_Kho.FirstOrDefault((dm_HangHoa_Kho e) => e.LOC_ID == itm2.LOC_ID && e.ID == itm2.ID_HANGHOAKHO);
						if (objdm_HangHoa_Kho2 != null)
						{
							itm2.TONGSOLUONG = itm2.TYLE_QD * itm2.SOLUONG;
							if (!itm2.ISEDIT)
							{
								_context.ct_PhieuDatHangNCC_ChiTiet.Add(itm2);
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
				}
				_context.Entry(Input).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
				v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC2 = new v_ct_PhieuDatHangNCC
				{
					lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuDatHangNCC_ChiTiet>()
				};
				SP_Parameter SP_Parameter = new SP_Parameter
				{
					ID_PHIEUNHAP = Input.ID
				};
				ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
				if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuDatHangNCC(SP_Parameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse })
				{
					List<v_ct_PhieuDatHangNCC> lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuDatHangNCC>;
					if (lstPhieuNhap != null && lstPhieuNhap.Count() > 0)
					{
						ct_PhieuDatHangNCC2 = lstPhieuNhap.FirstOrDefault() ?? new v_ct_PhieuDatHangNCC();
					}
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ct_PhieuDatHangNCC2
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
		}

		[HttpPut("{LOC_ID}/{ID}/{TRANGTHAI}")]
		[Authorize(Roles = "User")]
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
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				ct_PhieuDatHangNCC objct_PhieuDatHangNCC = await _context.ct_PhieuDatHangNCC.FirstOrDefaultAsync((ct_PhieuDatHangNCC e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (objct_PhieuDatHangNCC != null)
				{
					objct_PhieuDatHangNCC.ISHOANTAT = TRANGTHAI == "1";
					_context.Entry(objct_PhieuDatHangNCC).State = EntityState.Modified;
					AuditLogController auditLog = new AuditLogController(_context, _configuration);
					auditLog.InserAuditLog();
					await _context.SaveChangesAsync();
				}
				transaction.Commit();
				v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC2 = new v_ct_PhieuDatHangNCC
				{
					lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuDatHangNCC_ChiTiet>()
				};
				SP_Parameter SP_Parameter = new SP_Parameter
				{
					ID_PHIEUNHAP = ID
				};
				ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
				if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuDatHangNCC(SP_Parameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse })
				{
					List<v_ct_PhieuDatHangNCC> lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuDatHangNCC>;
					if (lstPhieuNhap != null && lstPhieuNhap.Count() > 0)
					{
						ct_PhieuDatHangNCC2 = lstPhieuNhap.FirstOrDefault() ?? new v_ct_PhieuDatHangNCC();
					}
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ct_PhieuDatHangNCC2
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
		}

		[HttpPost]
		[Authorize(Roles = "User")]
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
				if (await _context.ct_PhieuDatHangNCC.FirstOrDefaultAsync((ct_PhieuDatHangNCC e) => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.MAPHIEU) != null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Input.LOC_ID + "-" + Input.MAPHIEU + " trong dữ liệu!",
						Data = "",
						CheckValue = true
					});
				}
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				if (Input.lstct_PhieuNhap_ChiTiet != null)
				{
					foreach (v_ct_PhieuDatHangNCC_ChiTiet itm in Input.lstct_PhieuNhap_ChiTiet)
					{
						if (await _context.dm_HangHoa_Kho.FirstOrDefaultAsync((dm_HangHoa_Kho e) => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO) != null)
						{
							itm.TONGSOLUONG = itm.SOLUONG * itm.TYLE_QD;
							itm.LOC_ID = Input.LOC_ID;
							itm.ID_PHIEUDATHANGNCC = Input.ID;
							_context.ct_PhieuDatHangNCC_ChiTiet.Add(itm);
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
				_context.ct_PhieuDatHangNCC.Add(Input);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
				List<ct_PhieuDatHangNCC> lstPhieuDatHangCheck = await (from e in _context.ct_PhieuDatHangNCC
																	   where e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.MAPHIEU
																	   orderby e.NGAYLAP descending
																	   select e).ToListAsync();
				if (lstPhieuDatHangCheck != null && lstPhieuDatHangCheck.Count() > 1 && lstPhieuDatHangCheck.FirstOrDefault().ID == Input.ID)
				{
					int Max_ID = (from e in _context.ct_PhieuDatHangNCC
								  where e.LOC_ID == Input.LOC_ID && e.NGAYLAP.Date == Input.NGAYLAP.Date
								  select e.SOPHIEU).DefaultIfEmpty().Max();
					Input.SOPHIEU = Max_ID + 1;
					Input.MAPHIEU = API.GetMaPhieu("Order_Provider", Input.NGAYLAP, Input.SOPHIEU);
					_context.Entry(Input).State = EntityState.Modified;
					await _context.SaveChangesAsync();
				}
				v_ct_PhieuDatHangNCC ct_PhieuDatHangNCC2 = new v_ct_PhieuDatHangNCC
				{
					lstct_PhieuNhap_ChiTiet = new List<v_ct_PhieuDatHangNCC_ChiTiet>()
				};
				SP_Parameter SP_Parameter = new SP_Parameter
				{
					ID_PHIEUNHAP = Input.ID
				};
				ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
				if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuDatHangNCC(SP_Parameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse })
				{
					List<v_ct_PhieuDatHangNCC> lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuDatHangNCC>;
					if (lstPhieuNhap != null && lstPhieuNhap.Count() > 0)
					{
						ct_PhieuDatHangNCC2 = lstPhieuNhap.FirstOrDefault() ?? new v_ct_PhieuDatHangNCC();
					}
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ct_PhieuDatHangNCC2
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

		[HttpDelete("{LOC_ID}/{ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> DeleteInput(string LOC_ID, string ID)
		{
			try
			{
				ct_PhieuDatHangNCC Input = await _context.ct_PhieuDatHangNCC.FirstOrDefaultAsync((ct_PhieuDatHangNCC e) => e.LOC_ID == LOC_ID && e.ID == ID);
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
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				List<ct_PhieuDatHangNCC_ChiTiet> lstPhieuNhap_ChiTiet = await _context.ct_PhieuDatHangNCC_ChiTiet.Where((ct_PhieuDatHangNCC_ChiTiet e) => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUDATHANGNCC == Input.ID).ToListAsync();
				if (lstPhieuNhap_ChiTiet != null)
				{
					foreach (ct_PhieuDatHangNCC_ChiTiet itm in lstPhieuNhap_ChiTiet)
					{
						if (await _context.dm_HangHoa_Kho.FirstOrDefaultAsync((dm_HangHoa_Kho e) => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO) != null)
						{
							itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
							_context.ct_PhieuDatHangNCC_ChiTiet.Remove(itm);
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
				_context.ct_PhieuDatHangNCC.Remove(Input);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
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
			return _context.ct_PhieuDatHangNCC.Any((ct_PhieuDatHangNCC e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}
	}
}
