using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
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
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetInput(string LOC_ID)
		{
			try
			{
				List<ct_PhieuGiaoHang> lstValue = await _context.ct_PhieuGiaoHang.Where((ct_PhieuGiaoHang e) => e.LOC_ID == LOC_ID).ToListAsync();
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
				List<ct_PhieuGiaoHang> lstValue = await _context.ct_PhieuGiaoHang.Where((ct_PhieuGiaoHang e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).ToListAsync();
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
				if (await _context.ct_PhieuGiaoHang.FirstOrDefaultAsync((ct_PhieuGiaoHang e) => e.LOC_ID == LOC_ID && e.ID == ID) == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				v_ct_PhieuGiaoHang ct_PhieuGiaoHang2 = new v_ct_PhieuGiaoHang();
				SP_Parameter SP_Parameter = new SP_Parameter
				{
					ID_PHIEUGIAOHANG = ID
				};
				ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
				if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang(SP_Parameter) is OkObjectResult { Value: ApiResponse ApiResponse })
				{
					if (!ApiResponse.Success || ApiResponse.Data == null)
					{
						return Ok(ApiResponse);
					}
					List<v_ct_PhieuGiaoHang> lst = ApiResponse.Data as List<v_ct_PhieuGiaoHang>;
					if (lst != null && lst.Count > 0)
					{
						ct_PhieuGiaoHang2 = lst.FirstOrDefault() ?? new v_ct_PhieuGiaoHang();
					}
				}
				ct_PhieuGiaoHang2.lstct_PhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>();
				ct_PhieuGiaoHang2.lstct_PhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
				SP_Parameter = new SP_Parameter
				{
					ID_PHIEUGIAOHANG = ID
				};
				ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
				if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang_ChiTiet(SP_Parameter) is OkObjectResult { Value: ApiResponse ApiResponse2 })
				{
					if (!ApiResponse2.Success || ApiResponse2.Data == null)
					{
						return Ok(ApiResponse2);
					}
					List<v_ct_PhieuGiaoHang_ChiTiet> lst_ChiTiet = ApiResponse2.Data as List<v_ct_PhieuGiaoHang_ChiTiet>;
					if (lst_ChiTiet != null)
					{
						ct_PhieuGiaoHang2.lstct_PhieuGiaoHang_ChiTiet.AddRange(lst_ChiTiet);
					}
				}
				if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang_NhanVienGiao(SP_Parameter) is OkObjectResult { Value: ApiResponse ApiResponse3 })
				{
					if (!ApiResponse3.Success || ApiResponse3.Data == null)
					{
						return Ok(ApiResponse3);
					}
					List<v_ct_PhieuGiaoHang_NhanVienGiao> lst_ChiTiet2 = ApiResponse3.Data as List<v_ct_PhieuGiaoHang_NhanVienGiao>;
					if (lst_ChiTiet2 != null)
					{
						ct_PhieuGiaoHang2.lstct_PhieuGiaoHang_NhanVienGiao.AddRange(lst_ChiTiet2);
					}
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ct_PhieuGiaoHang2
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
				ct_PhieuGiaoHang PhieuNhap = await _context.ct_PhieuGiaoHang.FirstOrDefaultAsync((ct_PhieuGiaoHang e) => e.LOC_ID == Input.LOC_ID && e.ID == Input.ID);
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				List<ct_PhieuGiaoHang_ChiTiet> lstPhieuNhap_ChiTiet = await _context.ct_PhieuGiaoHang_ChiTiet.Where((ct_PhieuGiaoHang_ChiTiet e) => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUGIAOHANG == Input.ID).ToListAsync();
				if (lstPhieuNhap_ChiTiet != null)
				{
					foreach (ct_PhieuGiaoHang_ChiTiet itm in lstPhieuNhap_ChiTiet)
					{
						v_ct_PhieuGiaoHang_ChiTiet chkPhieuNhap_ChiTiet = Input.lstct_PhieuGiaoHang_ChiTiet.Where((v_ct_PhieuGiaoHang_ChiTiet e) => e.ID == itm.ID).FirstOrDefault();
						if (chkPhieuNhap_ChiTiet != null)
						{
							if (PhieuNhap != null && PhieuNhap.ISHOANTAT != Input.ISHOANTAT && Input.ISHOANTAT)
							{
								chkPhieuNhap_ChiTiet.ISDAGIAOHANG = true;
							}
							chkPhieuNhap_ChiTiet.ISEDIT = true;
							chkPhieuNhap_ChiTiet.ID_PHIEUGIAOHANG = Input.ID;
							_context.Entry(itm).State = EntityState.Modified;
						}
						else
						{
							_context.ct_PhieuGiaoHang_ChiTiet.Remove(itm);
						}
					}
				}
				if (Input.lstct_PhieuGiaoHang_ChiTiet != null)
				{
					foreach (v_ct_PhieuGiaoHang_ChiTiet itm2 in Input.lstct_PhieuGiaoHang_ChiTiet)
					{
						if (PhieuNhap != null && PhieuNhap.ISHOANTAT != Input.ISHOANTAT && Input.ISHOANTAT)
						{
							itm2.ISDAGIAOHANG = true;
						}
						itm2.ID_PHIEUGIAOHANG = Input.ID;
						if (!itm2.ISEDIT)
						{
							itm2.SOLAN = ((lstPhieuNhap_ChiTiet == null) ? 1 : (lstPhieuNhap_ChiTiet.Max((ct_PhieuGiaoHang_ChiTiet s) => s.SOLAN) + 1));
							_context.ct_PhieuGiaoHang_ChiTiet.Add(itm2);
						}
					}
					Input.SOLUONG_DONHANG = Input.lstct_PhieuGiaoHang_ChiTiet.Count();
					Input.SOTIENGIAOHANG = Input.lstct_PhieuGiaoHang_ChiTiet.Sum((v_ct_PhieuGiaoHang_ChiTiet e) => e.SOTIENGIAOHANG);
				}
				List<ct_PhieuGiaoHang_NhanVienGiao> lstPhieuNhap_NhanVienGiao = await _context.ct_PhieuGiaoHang_NhanVienGiao.Where((ct_PhieuGiaoHang_NhanVienGiao e) => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUGIAOHANG == Input.ID).ToListAsync();
				if (lstPhieuNhap_NhanVienGiao != null)
				{
					foreach (ct_PhieuGiaoHang_NhanVienGiao itm3 in lstPhieuNhap_NhanVienGiao)
					{
						v_ct_PhieuGiaoHang_NhanVienGiao chkPhieuNhap_NhanVienGiao = Input.lstct_PhieuGiaoHang_NhanVienGiao.Where((v_ct_PhieuGiaoHang_NhanVienGiao e) => e.ID == itm3.ID).FirstOrDefault();
						if (chkPhieuNhap_NhanVienGiao != null)
						{
							chkPhieuNhap_NhanVienGiao.ISEDIT = true;
							chkPhieuNhap_NhanVienGiao.ID_PHIEUGIAOHANG = Input.ID;
							_context.Entry(itm3).State = EntityState.Modified;
						}
						else
						{
							_context.ct_PhieuGiaoHang_NhanVienGiao.Remove(itm3);
						}
					}
				}
				if (Input.lstct_PhieuGiaoHang_NhanVienGiao != null)
				{
					foreach (v_ct_PhieuGiaoHang_NhanVienGiao itm4 in Input.lstct_PhieuGiaoHang_NhanVienGiao)
					{
						itm4.ID_PHIEUGIAOHANG = Input.ID;
						if (!itm4.ISEDIT)
						{
							itm4.ID = Guid.NewGuid().ToString();
							_context.ct_PhieuGiaoHang_NhanVienGiao.Add(itm4);
						}
					}
				}
				if (PhieuNhap != null)
				{
					_context.Entry(PhieuNhap).State = EntityState.Detached;
				}
				_context.Entry(Input).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
				v_ct_PhieuGiaoHang ct_PhieuGiaoHang2 = new v_ct_PhieuGiaoHang
				{
					lstct_PhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>(),
					lstct_PhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>()
				};
				SP_Parameter SP_Parameter = new SP_Parameter
				{
					LOC_ID = Input.LOC_ID,
					ID_PHIEUGIAOHANG = Input.ID
				};
				ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
				if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang(SP_Parameter) is OkObjectResult { Value: ApiResponse ApiResponse })
				{
					if (!ApiResponse.Success || ApiResponse.Data == null)
					{
						return Ok(ApiResponse);
					}
					List<v_ct_PhieuGiaoHang> lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuGiaoHang>;
					if (lstPhieuNhap != null && lstPhieuNhap.Count() > 0)
					{
						ct_PhieuGiaoHang2 = lstPhieuNhap.FirstOrDefault() ?? new v_ct_PhieuGiaoHang();
					}
				}
				ct_PhieuGiaoHang2.lstct_PhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>();
				ct_PhieuGiaoHang2.lstct_PhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
				SP_Parameter = new SP_Parameter
				{
					ID_PHIEUGIAOHANG = Input.ID
				};
				if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang_ChiTiet(SP_Parameter) is OkObjectResult { Value: ApiResponse ApiResponse2 })
				{
					if (!ApiResponse2.Success || ApiResponse2.Data == null)
					{
						return Ok(ApiResponse2);
					}
					List<v_ct_PhieuGiaoHang_ChiTiet> lst_ChiTiet = ApiResponse2.Data as List<v_ct_PhieuGiaoHang_ChiTiet>;
					if (lst_ChiTiet != null)
					{
						ct_PhieuGiaoHang2.lstct_PhieuGiaoHang_ChiTiet.AddRange(lst_ChiTiet);
					}
				}
				if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang_NhanVienGiao(SP_Parameter) is OkObjectResult { Value: ApiResponse ApiResponse3 })
				{
					if (!ApiResponse3.Success || ApiResponse3.Data == null)
					{
						return Ok(ApiResponse3);
					}
					List<v_ct_PhieuGiaoHang_NhanVienGiao> lst_ChiTiet2 = ApiResponse3.Data as List<v_ct_PhieuGiaoHang_NhanVienGiao>;
					if (lst_ChiTiet2 != null)
					{
						ct_PhieuGiaoHang2.lstct_PhieuGiaoHang_NhanVienGiao.AddRange(lst_ChiTiet2);
					}
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ct_PhieuGiaoHang2
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

		private static ct_PhieuGiaoHang ConvertobjectToct_PhieuGiaoHang<T>(T objectFrom)
		{
			ct_PhieuGiaoHang ct_PhieuGiaoHang2 = new ct_PhieuGiaoHang();
			if (objectFrom != null)
			{
				PropertyInfo[] properties = objectFrom.GetType().GetProperties();
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					if (!(propertyInfo != null))
					{
						continue;
					}
					object value = propertyInfo.GetValue(objectFrom);
					if (value != null)
					{
						PropertyInfo property = ct_PhieuGiaoHang2.GetType().GetProperty(propertyInfo.Name);
						if (property != null)
						{
							property.SetValue(ct_PhieuGiaoHang2, value);
						}
					}
				}
			}
			return ct_PhieuGiaoHang2;
		}

		[HttpPost]
		[Authorize(Roles = "User")]
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
				if (await _context.ct_PhieuGiaoHang.FirstOrDefaultAsync((ct_PhieuGiaoHang e) => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.MAPHIEU) != null)
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
				if (Input.lstct_PhieuGiaoHang_ChiTiet != null)
				{
					foreach (v_ct_PhieuGiaoHang_ChiTiet itm in Input.lstct_PhieuGiaoHang_ChiTiet)
					{
						ct_PhieuXuat objct_PhieuXuat = await _context.ct_PhieuXuat.FirstOrDefaultAsync((ct_PhieuXuat e) => e.LOC_ID == Input.LOC_ID && e.ID == itm.ID_PHIEUXUAT);
						if (objct_PhieuXuat != null)
						{
							if (await _context.ct_PhieuGiaoHang_ChiTiet.FirstOrDefaultAsync((ct_PhieuGiaoHang_ChiTiet e) => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUXUAT == itm.ID_PHIEUXUAT) != null)
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
							_context.ct_PhieuGiaoHang_ChiTiet.Add(itm);
							continue;
						}
						return Ok(new ApiResponse
						{
							Success = false,
							Message = "Không tìm thấy phiếu xuất " + Input.LOC_ID + "-" + itm.ID_PHIEUXUAT + " trong dữ liệu!",
							Data = "",
							CheckValue = true
						});
					}
					Input.SOLUONG_DONHANG = Input.lstct_PhieuGiaoHang_ChiTiet.Count();
					Input.SOTIENGIAOHANG = Input.lstct_PhieuGiaoHang_ChiTiet.Sum((v_ct_PhieuGiaoHang_ChiTiet e) => e.SOTIENGIAOHANG);
				}
				if (Input.lstct_PhieuGiaoHang_NhanVienGiao != null)
				{
					foreach (v_ct_PhieuGiaoHang_NhanVienGiao itm2 in Input.lstct_PhieuGiaoHang_NhanVienGiao)
					{
						itm2.ID = Guid.NewGuid().ToString();
						itm2.LOC_ID = Input.LOC_ID;
						itm2.ID_PHIEUGIAOHANG = Input.ID;
						_context.ct_PhieuGiaoHang_NhanVienGiao.Add(itm2);
					}
				}
				bool bolCheckMA = false;
				while (!bolCheckMA)
				{
					ct_PhieuGiaoHang check = _context.ct_PhieuGiaoHang.Where((ct_PhieuGiaoHang e) => e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.MAPHIEU).FirstOrDefault();
					if (check != null)
					{
						Input.MAPHIEU = API.GetMaPhieu("Delivery", Input.NGAYLAP, Input.SOPHIEU);
					}
					else
					{
						bolCheckMA = true;
					}
				}
				_context.ct_PhieuGiaoHang.Add(Input);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
				List<ct_PhieuGiaoHang> lstPhieuDatHangCheck = await (from e in _context.ct_PhieuGiaoHang
																	 where e.LOC_ID == Input.LOC_ID && e.MAPHIEU == Input.MAPHIEU
																	 orderby e.NGAYLAP descending
																	 select e).ToListAsync();
				if (lstPhieuDatHangCheck != null && lstPhieuDatHangCheck.Count() > 1 && lstPhieuDatHangCheck.FirstOrDefault().ID == Input.ID)
				{
					int Max_ID = (from e in _context.ct_PhieuGiaoHang
								  where e.LOC_ID == Input.LOC_ID && e.NGAYLAP.Date == Input.NGAYLAP.Date
								  select e.SOPHIEU).DefaultIfEmpty().Max();
					Input.SOPHIEU = Max_ID + 1;
					Input.MAPHIEU = API.GetMaPhieu("Delivery", Input.NGAYLAP, Input.SOPHIEU);
					_context.Entry(Input).State = EntityState.Modified;
					await _context.SaveChangesAsync();
				}
				v_ct_PhieuGiaoHang ct_PhieuGiaoHang2 = new v_ct_PhieuGiaoHang
				{
					lstct_PhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>(),
					lstct_PhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>()
				};
				SP_Parameter SP_Parameter = new SP_Parameter
				{
					LOC_ID = Input.LOC_ID,
					ID_PHIEUGIAOHANG = Input.ID
				};
				ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
				if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang(SP_Parameter) is OkObjectResult { Value: ApiResponse ApiResponse })
				{
					if (!ApiResponse.Success || ApiResponse.Data == null)
					{
						return Ok(ApiResponse);
					}
					List<v_ct_PhieuGiaoHang> lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuGiaoHang>;
					if (lstPhieuNhap != null && lstPhieuNhap.Count() > 0)
					{
						ct_PhieuGiaoHang2 = lstPhieuNhap.FirstOrDefault() ?? new v_ct_PhieuGiaoHang();
					}
				}
				ct_PhieuGiaoHang2.lstct_PhieuGiaoHang_ChiTiet = new List<v_ct_PhieuGiaoHang_ChiTiet>();
				ct_PhieuGiaoHang2.lstct_PhieuGiaoHang_NhanVienGiao = new List<v_ct_PhieuGiaoHang_NhanVienGiao>();
				SP_Parameter = new SP_Parameter
				{
					ID_PHIEUGIAOHANG = Input.ID
				};
				if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang_ChiTiet(SP_Parameter) is OkObjectResult { Value: ApiResponse ApiResponse2 })
				{
					if (!ApiResponse2.Success || ApiResponse2.Data == null)
					{
						return Ok(ApiResponse2);
					}
					List<v_ct_PhieuGiaoHang_ChiTiet> lst_ChiTiet = ApiResponse2.Data as List<v_ct_PhieuGiaoHang_ChiTiet>;
					if (lst_ChiTiet != null)
					{
						ct_PhieuGiaoHang2.lstct_PhieuGiaoHang_ChiTiet.AddRange(lst_ChiTiet);
					}
				}
				if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuGiaoHang_NhanVienGiao(SP_Parameter) is OkObjectResult { Value: ApiResponse ApiResponse3 })
				{
					if (!ApiResponse3.Success || ApiResponse3.Data == null)
					{
						return Ok(ApiResponse3);
					}
					List<v_ct_PhieuGiaoHang_NhanVienGiao> lst_ChiTiet2 = ApiResponse3.Data as List<v_ct_PhieuGiaoHang_NhanVienGiao>;
					if (lst_ChiTiet2 != null)
					{
						ct_PhieuGiaoHang2.lstct_PhieuGiaoHang_NhanVienGiao.AddRange(lst_ChiTiet2);
					}
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ct_PhieuGiaoHang2
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
				ct_PhieuGiaoHang Input = await _context.ct_PhieuGiaoHang.FirstOrDefaultAsync((ct_PhieuGiaoHang e) => e.LOC_ID == LOC_ID && e.ID == ID);
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
				List<ct_PhieuNhap> lstPhieuNhap = await _context.ct_PhieuNhap.Where((ct_PhieuNhap e) => e.LOC_ID == Input.LOC_ID && e.CHUNGTUKEMTHEO == Input.MAPHIEU).ToListAsync();
				if (lstPhieuNhap != null && lstPhieuNhap.Count() > 0)
				{
					string ChungTu3 = "(" + string.Join(";", lstPhieuNhap.Select((ct_PhieuNhap e) => e.MAPHIEU)) + ")";
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Có 'Phiếu Nhập " + ChungTu3 + "' liên quan tới " + Input.MAPHIEU + "!",
						Data = ""
					});
				}
				List<ct_PhieuXuat> lstPhieuXuat = await _context.ct_PhieuXuat.Where((ct_PhieuXuat e) => e.LOC_ID == Input.LOC_ID && e.CHUNGTUKEMTHEO == Input.MAPHIEU).ToListAsync();
				if (lstPhieuXuat != null && lstPhieuXuat.Count() > 0)
				{
					string ChungTu4 = "(" + string.Join(";", lstPhieuXuat.Select((ct_PhieuXuat e) => e.MAPHIEU)) + ")";
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Có 'Phiếu Xuất " + ChungTu4 + "' liên quan tới " + Input.MAPHIEU + "!",
						Data = ""
					});
				}
				List<ct_PhieuGiaoHang_ChiTiet> lstPhieuNhap_ChiTiet = await _context.ct_PhieuGiaoHang_ChiTiet.Where((ct_PhieuGiaoHang_ChiTiet e) => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUGIAOHANG == Input.ID).ToListAsync();
				if (lstPhieuNhap_ChiTiet != null)
				{
					foreach (ct_PhieuGiaoHang_ChiTiet itm in lstPhieuNhap_ChiTiet)
					{
						_context.ct_PhieuGiaoHang_ChiTiet.Remove(itm);
					}
				}
				List<ct_PhieuGiaoHang_NhanVienGiao> lstPhieuNhap_NhanVienGiao = await _context.ct_PhieuGiaoHang_NhanVienGiao.Where((ct_PhieuGiaoHang_NhanVienGiao e) => e.LOC_ID == Input.LOC_ID && e.ID_PHIEUGIAOHANG == Input.ID).ToListAsync();
				if (lstPhieuNhap_ChiTiet != null)
				{
					foreach (ct_PhieuGiaoHang_NhanVienGiao itm2 in lstPhieuNhap_NhanVienGiao)
					{
						_context.ct_PhieuGiaoHang_NhanVienGiao.Remove(itm2);
					}
				}
				_context.ct_PhieuGiaoHang.Remove(Input);
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
			return _context.ct_PhieuGiaoHang.Any((ct_PhieuGiaoHang e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}
	}
}
