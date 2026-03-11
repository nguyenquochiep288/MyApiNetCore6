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
	public class DepositController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		private static readonly Queue<v_ct_PhieuDatHang> requestQueue = new Queue<v_ct_PhieuDatHang>();

		private static bool isProcessing = false;

		private string strTable = "ct_PhieuDatHang";

		public DepositController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetDeposit(string LOC_ID)
		{
			try
			{
				List<ct_PhieuDatHang> lstValue = await _context.ct_PhieuDatHang.Where((ct_PhieuDatHang e) => e.LOC_ID == LOC_ID).ToListAsync();
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
				List<ct_PhieuDatHang> lstValue = await (from e in _context.ct_PhieuDatHang.Where((ct_PhieuDatHang e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch)
														orderby e.MAPHIEU
														select e).ToListAsync();
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

		[HttpGet("{LOC_ID}/{ID_KHO}/{FROMDATE}/{TODATE}/{Type}/{KeyWhere}/{ValuesSearch}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetDeposit(string LOC_ID, string ID_KHO, DateTime FROMDATE, DateTime TODATE, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				List<ct_PhieuDatHang> lstValue = await _context.ct_PhieuDatHang.Where((ct_PhieuDatHang e) => e.LOC_ID == LOC_ID && e.NGAYLAP.Date >= ((DateTime)FROMDATE).Date && e.NGAYLAP.Date <= ((DateTime)TODATE).Date).Where(KeyWhere, ValuesSearch).ToListAsync();
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
		public async Task<IActionResult> GetDeposit(string LOC_ID, string ID)
		{
			try
			{
				ct_PhieuDatHang Deposit = await _context.ct_PhieuDatHang.FirstOrDefaultAsync((ct_PhieuDatHang e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Deposit == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				v_ct_PhieuDatHang ct_PhieuDatHang2 = new v_ct_PhieuDatHang();
				if (Deposit != null)
				{
					string strDeposit = JsonConvert.SerializeObject(Deposit);
					ct_PhieuDatHang2 = JsonConvert.DeserializeObject<v_ct_PhieuDatHang>(strDeposit) ?? new v_ct_PhieuDatHang();
				}
				ct_PhieuDatHang2.lstct_PhieuDatHang_ChiTiet = new List<v_ct_PhieuDatHang_ChiTiet>();
				SP_Parameter SP_Parameter = new SP_Parameter
				{
					ID_PHIEUDATHANG = ID
				};
				ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
				if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuDatHang_ChiTiet(SP_Parameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse })
				{
					List<v_ct_PhieuDatHang_ChiTiet> lst_ChiTiet = ApiResponse.Data as List<v_ct_PhieuDatHang_ChiTiet>;
					if (lst_ChiTiet != null)
					{
						ct_PhieuDatHang2.lstct_PhieuDatHang_ChiTiet.AddRange(lst_ChiTiet);
					}
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ct_PhieuDatHang2
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

		[HttpPut("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> PutDeposit(string LOC_ID, [FromBody] List<Product_Detail> lstProduct_Detail)
		{
			try
			{
				return await Get_ChuongTrinhKhuyenMai(lstProduct_Detail, LOC_ID);
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

		[HttpPut("{LOC_ID}/{ID}")]
		[Authorize(Roles = "User")]
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
				if (!string.IsNullOrEmpty(Deposit.ID_PHIEUXUAT))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Phiếu đặt hàng " + Deposit.MAPHIEU + " đã được tạo phiếu xuất!",
						Data = ""
					});
				}
				string StrHetSoLuong = "";
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				List<ct_PhieuDatHang_ChiTiet> lstPhieuNhap_ChiTiet = await _context.ct_PhieuDatHang_ChiTiet.Where((ct_PhieuDatHang_ChiTiet e) => e.LOC_ID == Deposit.LOC_ID && e.ID_PHIEUDATHANG == Deposit.ID).ToListAsync();
				if (lstPhieuNhap_ChiTiet != null)
				{
					foreach (ct_PhieuDatHang_ChiTiet itm in lstPhieuNhap_ChiTiet)
					{
						if (!string.IsNullOrEmpty(itm.ID_THUESUAT))
						{
							dm_ThueSuat objVAT = _context.dm_ThueSuat.FirstOrDefault((dm_ThueSuat e) => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO && e.ID == itm.ID_THUESUAT);
							itm.THUESUAT = objVAT?.THUESUAT ?? itm.THUESUAT;
						}
						dm_HangHoa_Kho objdm_HangHoa_Kho = _context.dm_HangHoa_Kho.FromSqlRaw("\r\n                                    SELECT *\r\n                                    FROM dm_HangHoa_Kho WITH (UPDLOCK, ROWLOCK)\r\n                                    WHERE LOC_ID = @loc\r\n                                      AND ID = @id\r\n                                ", new SqlParameter("@loc", itm.LOC_ID), new SqlParameter("@id", itm.ID_HANGHOAKHO)).AsTracking().FirstOrDefault();
						if (objdm_HangHoa_Kho != null)
						{
							itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
							objdm_HangHoa_Kho.QTY += itm.TONGSOLUONG;
							_context.Entry(objdm_HangHoa_Kho).State = EntityState.Modified;
							v_ct_PhieuDatHang_ChiTiet chkPhieuNhap_ChiTiet = Deposit.lstct_PhieuDatHang_ChiTiet.Where((v_ct_PhieuDatHang_ChiTiet e) => e.ID == itm.ID).FirstOrDefault();
							if (chkPhieuNhap_ChiTiet != null)
							{
								chkPhieuNhap_ChiTiet.ISEDIT = true;
								chkPhieuNhap_ChiTiet.ID_PHIEUDATHANG = Deposit.ID;
								new ct_PhieuDatHang_ChiTiet();
								ct_PhieuDatHang_ChiTiet newct_PhieuDatHang_ChiTiet = ConvertobjectToct_PhieuDatHang_ChiTiet(chkPhieuNhap_ChiTiet, itm);
								newct_PhieuDatHang_ChiTiet.TONGSOLUONG = newct_PhieuDatHang_ChiTiet.TYLE_QD * newct_PhieuDatHang_ChiTiet.SOLUONG;
								_context.Entry(newct_PhieuDatHang_ChiTiet).State = EntityState.Modified;
							}
							else
							{
								_context.ct_PhieuDatHang_ChiTiet.Remove(itm);
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
				if (Deposit.lstct_PhieuDatHang_ChiTiet != null)
				{
					foreach (v_ct_PhieuDatHang_ChiTiet itm2 in Deposit.lstct_PhieuDatHang_ChiTiet)
					{
						if (!string.IsNullOrEmpty(itm2.ID_THUESUAT))
						{
							dm_ThueSuat objVAT2 = _context.dm_ThueSuat.FirstOrDefault((dm_ThueSuat e) => e.LOC_ID == itm2.LOC_ID && e.ID == itm2.ID_HANGHOAKHO && e.ID == itm2.ID_THUESUAT);
							itm2.THUESUAT = objVAT2?.THUESUAT ?? itm2.THUESUAT;
						}
						itm2.THANHTIEN = itm2.SOLUONG * itm2.DONGIA - itm2.TONGTIENGIAMGIA;
						itm2.TONGCONG = itm2.THANHTIEN + itm2.TONGTIENVAT;
						itm2.ID_PHIEUDATHANG = Deposit.ID;
						dm_HangHoa_Kho objdm_HangHoa_Kho2 = _context.dm_HangHoa_Kho.FromSqlRaw("\r\n                                    SELECT *\r\n                                    FROM dm_HangHoa_Kho WITH (UPDLOCK, ROWLOCK)\r\n                                    WHERE LOC_ID = @loc\r\n                                      AND ID = @id\r\n                                ", new SqlParameter("@loc", itm2.LOC_ID), new SqlParameter("@id", itm2.ID_HANGHOAKHO)).AsTracking().FirstOrDefault();
						if (objdm_HangHoa_Kho2 != null)
						{
							view_dm_HangHoa objdm_HangHoa = _context.view_dm_HangHoa.FirstOrDefault((view_dm_HangHoa e) => e.LOC_ID == itm2.LOC_ID && e.ID == itm2.ID_HANGHOA);
							itm2.TONGSOLUONG = itm2.TYLE_QD * itm2.SOLUONG;
							if (objdm_HangHoa_Kho2.QTY >= itm2.TONGSOLUONG)
							{
								objdm_HangHoa_Kho2.QTY -= itm2.TONGSOLUONG;
								_context.Entry(objdm_HangHoa_Kho2).State = EntityState.Modified;
							}
							else
							{
								string Strsoluong = "";
								if (objdm_HangHoa != null && itm2.TYLE_QD >= 1.0)
								{
									if (itm2.TYLE_QD > 1.0)
									{
										int soluong = Convert.ToInt32(objdm_HangHoa_Kho2.QTY) / Convert.ToInt32(itm2.TYLE_QD);
										if (soluong > 0)
										{
											Strsoluong = soluong.ToString("N0") + " " + objdm_HangHoa.NAME_DVT;
										}
										if (objdm_HangHoa_Kho2.QTY - (double)soluong * itm2.TYLE_QD > 0.0)
										{
											Strsoluong = (string.IsNullOrEmpty(Strsoluong) ? (Strsoluong + (objdm_HangHoa_Kho2.QTY - (double)soluong * itm2.TYLE_QD).ToString("N0") + " " + objdm_HangHoa.NAME_DVT_QD) : (Strsoluong + " " + (objdm_HangHoa_Kho2.QTY - (double)soluong * itm2.TYLE_QD).ToString("N0") + " " + objdm_HangHoa.NAME_DVT_QD + Environment.NewLine));
										}
										StrHetSoLuong = StrHetSoLuong + "Sản phẩm " + itm2.NAME + " không đủ tồn kho!" + Strsoluong + Environment.NewLine;
									}
									else
									{
										Strsoluong = objdm_HangHoa_Kho2.QTY.ToString("N0") + " " + objdm_HangHoa.NAME_DVT;
										StrHetSoLuong = StrHetSoLuong + "Sản phẩm " + itm2.NAME + " không đủ tồn kho!" + Strsoluong + Environment.NewLine;
									}
								}
							}
							if (!itm2.ISEDIT)
							{
								_context.ct_PhieuDatHang_ChiTiet.Add(itm2);
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
					if (!string.IsNullOrEmpty(StrHetSoLuong))
					{
						return Ok(new ApiResponse
						{
							Success = false,
							Message = StrHetSoLuong,
							Data = ""
						});
					}
					Deposit.TONGTHANHTIEN = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum((v_ct_PhieuDatHang_ChiTiet s) => s.THANHTIEN), 0);
					Deposit.TONGTIENGIAMGIA = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum((v_ct_PhieuDatHang_ChiTiet s) => s.TONGTIENGIAMGIA), 0);
					Deposit.TONGTIENVAT = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum((v_ct_PhieuDatHang_ChiTiet s) => s.TONGTIENVAT), 0);
					Deposit.TONGTIEN = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum((v_ct_PhieuDatHang_ChiTiet s) => s.TONGCONG), 0);
				}
				_context.Entry(Deposit).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
				v_ct_PhieuDatHang ct_PhieuDatHang2 = new v_ct_PhieuDatHang
				{
					lstct_PhieuDatHang_ChiTiet = new List<v_ct_PhieuDatHang_ChiTiet>()
				};
				SP_Parameter SP_Parameter = new SP_Parameter
				{
					ID_PHIEUNHAP = Deposit.ID
				};
				ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
				if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuNhap(SP_Parameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse })
				{
					List<v_ct_PhieuDatHang> lstPhieuDatHang = ApiResponse.Data as List<v_ct_PhieuDatHang>;
					if (lstPhieuDatHang != null && lstPhieuDatHang.Count() > 0)
					{
						ct_PhieuDatHang2 = lstPhieuDatHang.FirstOrDefault() ?? new v_ct_PhieuDatHang();
					}
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ct_PhieuDatHang2
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
				AuditLogController auditLog2 = new AuditLogController(_context, _configuration);
				auditLog2.DeleteRequest(strTable);
			}
		}

		[HttpPost]
		[Authorize(Roles = "User")]
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
				if (await _context.dm_NhanVien.FirstOrDefaultAsync((dm_NhanVien e) => e.LOC_ID == Deposit.LOC_ID && e.ID_TAIKHOAN == Deposit.ID_NHANVIEN) != null)
				{
					if (await _context.ct_PhieuDatHang.FirstOrDefaultAsync((ct_PhieuDatHang e) => e.LOC_ID == Deposit.LOC_ID && e.MAPHIEU == Deposit.MAPHIEU) != null)
					{
						return Ok(new ApiResponse
						{
							Success = false,
							Message = "Đã tồn tại" + Deposit.LOC_ID + "-" + Deposit.MAPHIEU + " trong dữ liệu!",
							Data = "",
							CheckValue = true
						});
					}
					using IDbContextTransaction transaction = _context.Database.BeginTransaction();
					if (Deposit.lstct_PhieuDatHang_ChiTiet != null)
					{
						string StrHetSoLuong = "";
						foreach (v_ct_PhieuDatHang_ChiTiet itm in Deposit.lstct_PhieuDatHang_ChiTiet)
						{
							if (!string.IsNullOrEmpty(itm.ID_THUESUAT))
							{
								dm_ThueSuat objVAT = _context.dm_ThueSuat.FirstOrDefault((dm_ThueSuat e) => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO && e.ID == itm.ID_THUESUAT);
								itm.THUESUAT = objVAT?.THUESUAT ?? itm.THUESUAT;
							}
							itm.THANHTIEN = itm.SOLUONG * itm.DONGIA - itm.TONGTIENGIAMGIA;
							itm.TONGCONG = itm.THANHTIEN + itm.TONGTIENVAT;
							dm_HangHoa_Kho objdm_HangHoa_Kho = _context.dm_HangHoa_Kho.FromSqlRaw("\r\n                                    SELECT *\r\n                                    FROM dm_HangHoa_Kho WITH (UPDLOCK, ROWLOCK)\r\n                                    WHERE LOC_ID = @loc\r\n                                      AND ID = @id\r\n                                ", new SqlParameter("@loc", itm.LOC_ID), new SqlParameter("@id", itm.ID_HANGHOAKHO)).AsTracking().FirstOrDefault();
							if (objdm_HangHoa_Kho != null)
							{
								view_dm_HangHoa objdm_HangHoa = _context.view_dm_HangHoa.FirstOrDefault((view_dm_HangHoa e) => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOA);
								itm.TONGSOLUONG = itm.SOLUONG * itm.TYLE_QD;
								if (objdm_HangHoa_Kho.QTY >= itm.TONGSOLUONG)
								{
									itm.GHICHU = objdm_HangHoa_Kho.QTY + ";";
									objdm_HangHoa_Kho.QTY -= itm.TONGSOLUONG;
									v_ct_PhieuDatHang_ChiTiet obj = itm;
									obj.GHICHU = obj.GHICHU + objdm_HangHoa_Kho.QTY + ";";
									_context.Entry(objdm_HangHoa_Kho).State = EntityState.Modified;
								}
								else
								{
									string Strsoluong = "";
									if (objdm_HangHoa != null && itm.TYLE_QD >= 1.0)
									{
										if (itm.TYLE_QD > 1.0)
										{
											int soluong = Convert.ToInt32(objdm_HangHoa_Kho.QTY) / Convert.ToInt32(itm.TYLE_QD);
											if (soluong > 0)
											{
												Strsoluong = soluong.ToString("N0") + " " + objdm_HangHoa.NAME_DVT;
											}
											if (objdm_HangHoa_Kho.QTY - (double)soluong * itm.TYLE_QD > 0.0)
											{
												Strsoluong = (string.IsNullOrEmpty(Strsoluong) ? (Strsoluong + (objdm_HangHoa_Kho.QTY - (double)soluong * itm.TYLE_QD).ToString("N0") + " " + objdm_HangHoa.NAME_DVT_QD) : (Strsoluong + " " + (objdm_HangHoa_Kho.QTY - (double)soluong * itm.TYLE_QD).ToString("N0") + " " + objdm_HangHoa.NAME_DVT_QD + Environment.NewLine));
											}
											StrHetSoLuong = StrHetSoLuong + "Sản phẩm " + itm.NAME + " không đủ tồn kho!" + Strsoluong + Environment.NewLine;
										}
										else
										{
											Strsoluong = objdm_HangHoa_Kho.QTY.ToString("N0") + " " + objdm_HangHoa.NAME_DVT;
											StrHetSoLuong = StrHetSoLuong + "Sản phẩm " + itm.NAME + " không đủ tồn kho!" + Strsoluong + Environment.NewLine;
										}
									}
								}
								if (await _context.ct_PhieuDatHang_ChiTiet.FirstOrDefaultAsync((ct_PhieuDatHang_ChiTiet e) => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID) != null)
								{
									itm.ID = Guid.NewGuid().ToString();
								}
								itm.LOC_ID = Deposit.LOC_ID;
								itm.ID_PHIEUDATHANG = Deposit.ID;
								_context.ct_PhieuDatHang_ChiTiet.Add(itm);
								continue;
							}
							return Ok(new ApiResponse
							{
								Success = false,
								Message = "Không tìm thấy sản phẩm kho!" + itm.ID_HANGHOAKHO,
								Data = ""
							});
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
						Deposit.TONGTHANHTIEN = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum((v_ct_PhieuDatHang_ChiTiet s) => s.THANHTIEN), 0);
						Deposit.TONGTIENGIAMGIA = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum((v_ct_PhieuDatHang_ChiTiet s) => s.TONGTIENGIAMGIA), 0);
						Deposit.TONGTIENVAT = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum((v_ct_PhieuDatHang_ChiTiet s) => s.TONGTIENVAT), 0);
						Deposit.TONGTIEN = Math.Round(Deposit.lstct_PhieuDatHang_ChiTiet.Sum((v_ct_PhieuDatHang_ChiTiet s) => s.TONGCONG), 0);
					}
					_context.ct_PhieuDatHang.Add(Deposit);
					AuditLogController auditLog = new AuditLogController(_context, _configuration);
					auditLog.InserAuditLog();
					await _context.SaveChangesAsync();
					transaction.Commit();
					List<ct_PhieuDatHang> lstPhieuDatHangCheck = await (from e in _context.ct_PhieuDatHang
																		where e.LOC_ID == Deposit.LOC_ID && e.MAPHIEU == Deposit.MAPHIEU
																		orderby e.NGAYLAP descending
																		select e).ToListAsync();
					if (lstPhieuDatHangCheck != null && lstPhieuDatHangCheck.Count() > 1 && lstPhieuDatHangCheck.FirstOrDefault().ID == Deposit.ID)
					{
						int Max_ID = (from e in _context.ct_PhieuDatHang
									  where e.LOC_ID == Deposit.LOC_ID && e.NGAYLAP.Date == Deposit.NGAYLAP.Date
									  select e.SOPHIEU).DefaultIfEmpty().Max();
						Deposit.SOPHIEU = Max_ID + 1;
						Deposit.MAPHIEU = API.GetMaPhieu("Deposit", Deposit.NGAYLAP, Deposit.SOPHIEU);
						_context.Entry(Deposit).State = EntityState.Modified;
						await _context.SaveChangesAsync();
					}
					v_ct_PhieuDatHang ct_PhieuDatHang2 = new v_ct_PhieuDatHang
					{
						lstct_PhieuDatHang_ChiTiet = new List<v_ct_PhieuDatHang_ChiTiet>()
					};
					SP_Parameter SP_Parameter = new SP_Parameter
					{
						ID_PHIEUNHAP = Deposit.ID
					};
					ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
					if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuNhap(SP_Parameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse })
					{
						List<v_ct_PhieuDatHang> lstPhieuDatHang = ApiResponse.Data as List<v_ct_PhieuDatHang>;
						if (lstPhieuDatHang != null && lstPhieuDatHang.Count() > 0)
						{
							ct_PhieuDatHang2 = lstPhieuDatHang.FirstOrDefault() ?? new v_ct_PhieuDatHang();
						}
					}
					return Ok(new ApiResponse
					{
						Success = true,
						Message = "Success",
						Data = ct_PhieuDatHang2
					});
				}
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Tài khoản chưa được gắn với nhân viên trong dữ liệu!",
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
			finally
			{
				AuditLogController auditLog2 = new AuditLogController(_context, _configuration);
				auditLog2.DeleteRequest(strTable);
			}
		}

		[HttpPost("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> PostDeposit(string LOC_ID, [FromBody] List<Product_Detail> lstProduct_Detail)
		{
			try
			{
				return await Get_ChuongTrinhKhuyenMai(lstProduct_Detail, LOC_ID);
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

		[HttpPost("PostCreateOutput")]
		[Authorize(Roles = "User")]
		public async Task<ActionResult<ct_PhieuDatHang>> PostDeposit([FromBody] List<Deposit> lstDeposit)
		{
			try
			{
				string LOC_ID = "";
				string ID_KHO = "";
				DateTime NGAYLAP = default(DateTime);
				NGAYLAP = DateTime.Now.Date;
				if (lstDeposit != null && lstDeposit.Count > 0)
				{
					Deposit Deposit = lstDeposit.FirstOrDefault() ?? new Deposit();
					LOC_ID = ((Deposit != null) ? Deposit.LOC_ID : "");
					string ID_NGUOITAO = ((Deposit != null) ? Deposit.ID_NGUOITAO : "");
					NGAYLAP = Deposit?.NGAYLAP ?? DateTime.Now.Date;
					Dictionary<string, ct_PhieuXuat> lstPhieuXuatCheck = new Dictionary<string, ct_PhieuXuat>();
					List<ct_PhieuDatHang_ChiTiet> lstPhieuDatHang_ChiTiet = new List<ct_PhieuDatHang_ChiTiet>();
					Dictionary<string, string> lstPhieuDatHang = new Dictionary<string, string>();
					using IDbContextTransaction transaction = _context.Database.BeginTransaction();
					int Max_ID = (from e in _context.ct_PhieuXuat
								  where e.LOC_ID == LOC_ID && e.NGAYLAP.Date == ((DateTime)NGAYLAP).Date
								  select e.SOPHIEU).DefaultIfEmpty().Max();
					foreach (Deposit itm in lstDeposit)
					{
						ct_PhieuDatHang PhieuDatHang = await _context.ct_PhieuDatHang.FirstOrDefaultAsync((ct_PhieuDatHang e) => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID);
						if (PhieuDatHang == null)
						{
							return Ok(new ApiResponse
							{
								Success = false,
								Message = "Không tìm thấy " + LOC_ID + "-" + itm.ID + " dữ liệu!",
								Data = ""
							});
						}
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
						ID_KHO = PhieuDatHang.ID_KHO;
						lstPhieuDatHang.Add(PhieuDatHang.ID, PhieuDatHang.ID_KHACHHANG);
						ct_PhieuDatHang_ChiTiet[] lstChiTietPhieuDatHang_CT = await _context.ct_PhieuDatHang_ChiTiet.Where((ct_PhieuDatHang_ChiTiet e) => e.LOC_ID == itm.LOC_ID && e.ID_PHIEUDATHANG == itm.ID).ToArrayAsync();
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
					dm_LoaiPhieuXuat dm_LoaiPhieuXuat2 = await _context.dm_LoaiPhieuXuat.FirstOrDefaultAsync((dm_LoaiPhieuXuat e) => e.LOC_ID == LOC_ID && e.MA == API.XHKH);
					foreach (IGrouping<string, string> itm2 in from e in lstPhieuDatHang
															   select e.Value into e
															   group e by e.ToString())
					{
						IEnumerable<string> lstPhieuDatHang_KH = from e in lstPhieuDatHang
																 where e.Value == itm2.Key.ToString()
																 select e.Key;
						List<ct_PhieuDatHang_ChiTiet> lstPhieuDatHang_ChiTiet_KH = lstPhieuDatHang_ChiTiet.Where((ct_PhieuDatHang_ChiTiet e) => lstPhieuDatHang_KH.Contains(e.ID_PHIEUDATHANG)).ToList();
						ct_PhieuXuat newct_PhieuXuat = new ct_PhieuXuat();
						Max_ID++;
						newct_PhieuXuat.ID = Guid.NewGuid().ToString();
						newct_PhieuXuat.LOC_ID = LOC_ID;
						newct_PhieuXuat.ID_LOAIPHIEUXUAT = ((dm_LoaiPhieuXuat2 != null) ? dm_LoaiPhieuXuat2.ID : "");
						newct_PhieuXuat.NGAYLAP = NGAYLAP;
						newct_PhieuXuat.SOPHIEU = Max_ID;
						newct_PhieuXuat.ID_KHACHHANG = itm2.Key;
						newct_PhieuXuat.ID_KHO = ID_KHO;
						newct_PhieuXuat.TONGTIENGIAMGIA = Math.Round(lstPhieuDatHang_ChiTiet_KH.Sum((ct_PhieuDatHang_ChiTiet s) => s.TONGTIENGIAMGIA), 0);
						newct_PhieuXuat.TONGTHANHTIEN = Math.Round(lstPhieuDatHang_ChiTiet_KH.Sum((ct_PhieuDatHang_ChiTiet s) => s.THANHTIEN), 0);
						newct_PhieuXuat.TONGTIENVAT = Math.Round(lstPhieuDatHang_ChiTiet_KH.Sum((ct_PhieuDatHang_ChiTiet s) => s.TONGTIENVAT), 0);
						newct_PhieuXuat.TONGTIEN = Math.Round(lstPhieuDatHang_ChiTiet_KH.Sum((ct_PhieuDatHang_ChiTiet s) => s.TONGCONG), 0);
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
						foreach (ct_PhieuDatHang_ChiTiet ct in lstPhieuDatHang_ChiTiet_KH)
						{
							ct_PhieuXuat_ChiTiet newct_PhieuXuat_CT = new ct_PhieuXuat_ChiTiet();
							newct_PhieuXuat_CT = ConvertobjectToct_PhieuXuat_ChiTiet(ct, newct_PhieuXuat_CT);
							newct_PhieuXuat_CT.ID_PHIEUXUAT = newct_PhieuXuat.ID;
							newct_PhieuXuat_CT.ID_PHIEUDIEUHANG_CHITIET = ct.ID;
							if (string.IsNullOrEmpty(ID_PHIEUDATHANG) || ct.ID_PHIEUDATHANG != ID_PHIEUDATHANG || (ct.ID_PHIEUDATHANG == ID_PHIEUDATHANG && ct.STT != STT_PHIEUDATHANG))
							{
								STT++;
								STT_PHIEUDATHANG = ct.STT;
								ID_PHIEUDATHANG = ct.ID_PHIEUDATHANG;
							}
							newct_PhieuXuat_CT.STT = STT;
							_context.ct_PhieuXuat_ChiTiet.Add(newct_PhieuXuat_CT);
						}
						foreach (string value in lstPhieuDatHang_KH)
						{
							ct_PhieuDatHang PhieuDatHang2 = await _context.ct_PhieuDatHang.FirstOrDefaultAsync((ct_PhieuDatHang e) => e.LOC_ID == LOC_ID && e.ID == value);
							if (PhieuDatHang2 != null)
							{
								newct_PhieuXuat.GHICHU = (string.IsNullOrEmpty(newct_PhieuXuat.GHICHU) ? "" : (newct_PhieuXuat.GHICHU + ",")) + PhieuDatHang2.MAPHIEU;
								PhieuDatHang2.ID_PHIEUXUAT = newct_PhieuXuat.ID;
								_context.Entry(PhieuDatHang2).State = EntityState.Modified;
							}
						}
						bool bolCheckMA = false;
						while (!bolCheckMA)
						{
							newct_PhieuXuat.MAPHIEU = API.GetMaPhieu("Output", newct_PhieuXuat.NGAYLAP, newct_PhieuXuat.SOPHIEU);
							ct_PhieuXuat check = _context.ct_PhieuXuat.Where((ct_PhieuXuat e) => e.LOC_ID == LOC_ID && e.MAPHIEU == newct_PhieuXuat.MAPHIEU).FirstOrDefault();
							if (check != null)
							{
								Max_ID++;
								newct_PhieuXuat.SOPHIEU = Max_ID;
							}
							else
							{
								bolCheckMA = true;
							}
						}
						_context.ct_PhieuXuat.Add(newct_PhieuXuat);
						lstPhieuXuatCheck.Add(newct_PhieuXuat.ID, newct_PhieuXuat);
					}
					AuditLogController auditLog = new AuditLogController(_context, _configuration);
					auditLog.InserAuditLog();
					await _context.SaveChangesAsync();
					transaction.Commit();
					foreach (KeyValuePair<string, ct_PhieuXuat> itm3 in lstPhieuXuatCheck)
					{
						List<ct_PhieuXuat> lstPhieuDatHangCheck = await (from e in _context.ct_PhieuXuat
																		 where e.LOC_ID == itm3.Value.LOC_ID && e.MAPHIEU == itm3.Value.MAPHIEU
																		 orderby e.NGAYLAP descending
																		 select e).ToListAsync();
						if (lstPhieuDatHangCheck != null && lstPhieuDatHangCheck.Count() > 1 && lstPhieuDatHangCheck.FirstOrDefault().ID == itm3.Value.ID)
						{
							int Max_ID2 = (from e in _context.ct_PhieuXuat
										   where e.LOC_ID == itm3.Value.LOC_ID && e.NGAYLAP.Date == itm3.Value.NGAYLAP.Date
										   select e.SOPHIEU).DefaultIfEmpty().Max();
							itm3.Value.SOPHIEU = Max_ID2 + 1;
							itm3.Value.MAPHIEU = API.GetMaPhieu("Output", itm3.Value.NGAYLAP, itm3.Value.SOPHIEU);
							_context.Entry(itm3.Value).State = EntityState.Modified;
							await _context.SaveChangesAsync();
						}
					}
					return Ok(new ApiResponse
					{
						Success = true,
						Message = "Success",
						Data = ""
					});
				}
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Không tìm thấy dữ liệu!",
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

		private static ct_PhieuXuat_ChiTiet ConvertobjectToct_PhieuXuat_ChiTiet<T>(T objectFrom, ct_PhieuXuat_ChiTiet objectTo)
		{
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
						PropertyInfo property = objectTo.GetType().GetProperty(propertyInfo.Name);
						if (property != null)
						{
							property.SetValue(objectTo, value);
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
						PropertyInfo property = objectTo.GetType().GetProperty(propertyInfo.Name);
						if (property != null)
						{
							property.SetValue(objectTo, value);
						}
					}
				}
			}
			return objectTo;
		}

		[HttpDelete("{LOC_ID}/{ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> DeleteDeposit(string LOC_ID, string ID)
		{
			try
			{
				ct_PhieuDatHang Deposit = await _context.ct_PhieuDatHang.FirstOrDefaultAsync((ct_PhieuDatHang e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Deposit == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				if (!string.IsNullOrEmpty(Deposit.ID_PHIEUXUAT))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Phiếu đặt hàng " + Deposit.MAPHIEU + " đã được tạo phiếu xuất!",
						Data = ""
					});
				}
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				List<ct_PhieuDatHang_ChiTiet> lstPhieuNhap_ChiTiet = await _context.ct_PhieuDatHang_ChiTiet.Where((ct_PhieuDatHang_ChiTiet e) => e.LOC_ID == Deposit.LOC_ID && e.ID_PHIEUDATHANG == Deposit.ID).ToListAsync();
				if (lstPhieuNhap_ChiTiet != null)
				{
					foreach (ct_PhieuDatHang_ChiTiet itm in lstPhieuNhap_ChiTiet)
					{
						dm_HangHoa_Kho objdm_HangHoa_Kho = _context.dm_HangHoa_Kho.FromSqlRaw("\r\n                                    SELECT *\r\n                                    FROM dm_HangHoa_Kho WITH (UPDLOCK, ROWLOCK)\r\n                                    WHERE LOC_ID = @loc\r\n                                      AND ID = @id\r\n                                ", new SqlParameter("@loc", itm.LOC_ID), new SqlParameter("@id", itm.ID_HANGHOAKHO)).AsTracking().FirstOrDefault();
						if (objdm_HangHoa_Kho != null)
						{
							itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
							objdm_HangHoa_Kho.QTY += itm.TONGSOLUONG;
							_context.Entry(objdm_HangHoa_Kho).State = EntityState.Modified;
							_context.ct_PhieuDatHang_ChiTiet.Remove(itm);
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
				_context.ct_PhieuDatHang.Remove(Deposit);
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

		private bool DepositExistsID(string LOC_ID, string ID)
		{
			return _context.ct_PhieuDatHang.Any((ct_PhieuDatHang e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}

		private async Task<IActionResult> Get_ChuongTrinhKhuyenMai(List<Product_Detail> lstProduct_Detail, string LOC_ID)
		{
			try
			{
				List<v_dm_ChuongTrinhKhuyenMai> lstdm_ChuongTrinhKhuyenMai = new List<v_dm_ChuongTrinhKhuyenMai>();
				SP_Parameter SP_Parameter = new SP_Parameter
				{
					LOC_ID = LOC_ID,
					TUNGAY = DateTime.Now.Date,
					DENNGAY = DateTime.Now.Date
				};
				ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
				if (await ExecuteStoredProc1.Sp_Get_ChuongTrinhKhuyenMai(SP_Parameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse })
				{
					List<v_dm_ChuongTrinhKhuyenMai> lst_ChiTiet = ApiResponse.Data as List<v_dm_ChuongTrinhKhuyenMai>;
					if (lst_ChiTiet != null)
					{
						lstdm_ChuongTrinhKhuyenMai.AddRange(lst_ChiTiet);
					}
				}
				List<Product_Detail> lstKhuyenMai = lstProduct_Detail.Where((Product_Detail e) => e.ISKHUYENMAI).ToList();
				if (lstKhuyenMai != null && lstKhuyenMai.Count() > 0)
				{
					foreach (Product_Detail itm in lstKhuyenMai)
					{
						lstProduct_Detail.Remove(itm);
					}
				}
				IEnumerable<Product_Detail> lstKhuyenMai2 = lstProduct_Detail.Where((Product_Detail e) => !string.IsNullOrEmpty(e.ID_KHUYENMAI));
				if (lstKhuyenMai2 != null && lstKhuyenMai2.Count() > 0)
				{
					foreach (Product_Detail itm2 in lstKhuyenMai2)
					{
						dm_ThueSuat clsdm_ThueSuat = new dm_ThueSuat();
						if (!string.IsNullOrEmpty(itm2.ID_THUESUAT))
						{
							clsdm_ThueSuat = (await _context.dm_ThueSuat.FirstOrDefaultAsync((dm_ThueSuat e) => e.LOC_ID == itm2.LOC_ID && e.ID == itm2.ID_THUESUAT)) ?? new dm_ThueSuat();
						}
						itm2.CHIETKHAU = 0.0;
						itm2.TYPE = "CHIETKHAU";
						API.TinhTong(itm2, "", lstProduct_Detail, clsdm_ThueSuat);
						itm2.TONGTIENGIAMGIA = 0.0;
						itm2.TYPE = "TONGTIENGIAMGIA";
						API.TinhTong(itm2, "", lstProduct_Detail, clsdm_ThueSuat);
						itm2.ISDALAYKHUYENMAI = false;
						itm2.ID_KHUYENMAI = "";
					}
				}
				double SOTIENTHUE_KM = 0.0;
				List<string> lstDanhSachDaLayKhuyenMai = new List<string>();
				List<Product_Detail> lstProduct_Detail_Tam = new List<Product_Detail>();
				foreach (v_dm_ChuongTrinhKhuyenMai itm3 in lstdm_ChuongTrinhKhuyenMai)
				{
					if (itm3.ID.StartsWith("4419b3ea-5fbc-4871-9c66-d29ce1d2134c") || itm3.ID.StartsWith("b442b878-78e3-48fa-9f61-86e6958ca858"))
					{
					}
					bool bolConSoLuong = false;
					string input = itm3.MA;
					int lastIndex = input.LastIndexOf('_');
					if (lastIndex != -1)
					{
						string result = input.Substring(0, lastIndex);
						if (lstDanhSachDaLayKhuyenMai.Where((string e) => e.StartsWith(result)).Count() > 0)
						{
							if (lstProduct_Detail_Tam.Where((Product_Detail product_Detail) => product_Detail.SOLUONG - product_Detail.SOLUONGDALAY_KM > 0.0).Count() <= 0)
							{
								continue;
							}
							bolConSoLuong = true;
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
					int intCoLayKhuyenMai = 0;
					List<dm_ChuongTrinhKhuyenMai_YeuCau> lstChuongTrinhKhuyenMai_YeuCau = await _context.dm_ChuongTrinhKhuyenMai_YeuCau.Where((dm_ChuongTrinhKhuyenMai_YeuCau e) => e.LOC_ID == LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == itm3.ID).ToListAsync();
					if (itm3.IS_YEUCAUCHITIET)
					{
						if (lstChuongTrinhKhuyenMai_YeuCau != null && lstChuongTrinhKhuyenMai_YeuCau.Count > 0)
						{
							int PhanNguyen = 0;
							bool bolThoatWhile = false;
							List<Product_Detail> lstSelectProduct_Detail = new List<Product_Detail>();
							List<Product_Detail> lstSelectProduct_Detail_HT = new List<Product_Detail>();
							List<Product_Detail> lstSelectProduct_Detail_Old = new List<Product_Detail>();
							while (!bolThoatWhile)
							{
								PhanNguyen++;
								foreach (dm_ChuongTrinhKhuyenMai_YeuCau ChiTiet in lstChuongTrinhKhuyenMai_YeuCau)
								{
									List<Product_Detail> getlst = lstProduct_Detail.Where((Product_Detail e) => (!itm3.ISTONGHOADON || (itm3.ISTONGHOADON && !e.ISDALAYKHUYENMAI)) && ((e.ID_HANGHOA == ChiTiet.ID_HANGHOA && ChiTiet.HINHTHUC == 0) || (e.ID_NHOMHANGHOA == ChiTiet.ID_HANGHOA && ChiTiet.HINHTHUC == 1)) && e.ID_DVT == ChiTiet.ID_DVT).ToList();
									double CTKM_YC = getlst.Sum((Product_Detail e) => (!(ChiTiet.SOLUONG > 0.0)) ? e.THANHTIEN : (bolConSoLuong ? (e.SOLUONG - e.SOLUONGDALAY_KM) : e.SOLUONG));
									if (CTKM_YC >= ((ChiTiet.SOLUONG > 0.0) ? ChiTiet.SOLUONG : ChiTiet.SOTIEN) * (double)PhanNguyen)
									{
										if (PhanNguyen == 1)
										{
											lstSelectProduct_Detail_HT.AddRange(getlst);
										}
										if (itm3.IS_YEUCAUCHITIET && ChiTiet.SOLUONG == 0.0 && ChiTiet.SOTIEN == 0.0)
										{
											bolThoatWhile = true;
											PhanNguyen = 0;
										}
									}
									else
									{
										PhanNguyen--;
										bolThoatWhile = true;
										lstSelectProduct_Detail = lstSelectProduct_Detail_Old.ToList();
									}
								}
								if (!itm3.ISTINHLUYTUYEN)
								{
									bolThoatWhile = true;
								}
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
								foreach (dm_ChuongTrinhKhuyenMai_YeuCau ChiTiet2 in lstChuongTrinhKhuyenMai_YeuCau)
								{
									List<Product_Detail> getlst2 = lstProduct_Detail.Where((Product_Detail e) => ((e.ID_HANGHOA == ChiTiet2.ID_HANGHOA && ChiTiet2.HINHTHUC == 0) || (e.ID_NHOMHANGHOA == ChiTiet2.ID_HANGHOA && ChiTiet2.HINHTHUC == 1)) && e.ID_DVT == ChiTiet2.ID_DVT).ToList();
									foreach (Product_Detail ChiTietHoaDon in getlst2)
									{
										dm_ThueSuat clsdm_ThueSuat2 = new dm_ThueSuat();
										if (!string.IsNullOrEmpty(ChiTietHoaDon.ID_THUESUAT))
										{
											clsdm_ThueSuat2 = (await _context.dm_ThueSuat.FirstOrDefaultAsync((dm_ThueSuat e) => e.LOC_ID == ChiTietHoaDon.LOC_ID && e.ID == ChiTietHoaDon.ID_THUESUAT)) ?? new dm_ThueSuat();
										}
										double TONGTIENGIAMGIA;
										if (ChiTietHoaDon.CHIETKHAU < ChiTiet2.CHIETKHAU)
										{
											TONGTIENGIAMGIA = ChiTietHoaDon.SOLUONG * ChiTietHoaDon.DONGIA * ChiTiet2.CHIETKHAU / 100.0;
											if (TONGTIENGIAMGIA > ChiTietHoaDon.TONGTIENGIAMGIA)
											{
												ChiTietHoaDon.CHIETKHAU = ChiTiet2.CHIETKHAU;
												ChiTietHoaDon.TYPE = "CHIETKHAU";
												API.TinhTong(ChiTietHoaDon, "", lstProduct_Detail, clsdm_ThueSuat2);
											}
										}
										TONGTIENGIAMGIA = ChiTiet2.TIENGIAM * (double)((!itm3.ISTINHLUYTUYEN) ? 1 : PhanNguyen);
										if (ChiTietHoaDon.TONGTIENGIAMGIA < TONGTIENGIAMGIA)
										{
											ChiTietHoaDon.TONGTIENGIAMGIA = TONGTIENGIAMGIA;
											ChiTietHoaDon.TYPE = "TONGTIENGIAMGIA";
											API.TinhTong(ChiTietHoaDon, "", lstProduct_Detail, clsdm_ThueSuat2);
										}
										ChiTietHoaDon.ISDALAYKHUYENMAI = false;
										ChiTietHoaDon.ID_KHUYENMAI = ChiTiet2.ID_CHUONGTRINHKHUYENMAI;
										view_dm_HangHoa objHangHoa = await _context.view_dm_HangHoa.FirstOrDefaultAsync((view_dm_HangHoa e) => e.LOC_ID == LOC_ID && e.ID == ChiTietHoaDon.ID_HANGHOA);
										if (objHangHoa != null && objHangHoa.MUCTHUE != 0.0)
										{
											SOTIENTHUE_KM += ChiTietHoaDon.TONGTIENGIAMGIA * objHangHoa.MUCTHUE / 100.0;
										}
									}
									intCoLayKhuyenMai++;
								}
							}
							List<dm_ChuongTrinhKhuyenMai_Tang> lstdm_ChuongTrinhKhuyenMai_Tang = await _context.dm_ChuongTrinhKhuyenMai_Tang.Where((dm_ChuongTrinhKhuyenMai_Tang e) => e.LOC_ID == LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == itm3.ID).ToListAsync();
							if (PhanNguyen > 0 && lstdm_ChuongTrinhKhuyenMai_Tang != null && lstdm_ChuongTrinhKhuyenMai_Tang.Count > 0)
							{
								string ID_KHO = lstProduct_Detail.Select((Product_Detail e) => e.ID_KHO).FirstOrDefault();
								if (lstdm_ChuongTrinhKhuyenMai_Tang != null)
								{
									foreach (dm_ChuongTrinhKhuyenMai_Tang CTKM_Tang in lstdm_ChuongTrinhKhuyenMai_Tang)
									{
										dm_HangHoa_Kho HangHoaKho = await _context.dm_HangHoa_Kho.FirstOrDefaultAsync((dm_HangHoa_Kho e) => e.LOC_ID == itm3.LOC_ID && e.ID_HANGHOA == CTKM_Tang.ID_HANGHOA && e.ID_KHO == ID_KHO);
										view_dm_HangHoa HangHoa = await _context.view_dm_HangHoa.FirstOrDefaultAsync((view_dm_HangHoa e) => e.LOC_ID == itm3.LOC_ID && e.ID == CTKM_Tang.ID_HANGHOA);
										if (HangHoaKho == null || HangHoa == null)
										{
											continue;
										}
										Product_Detail newProduct_Detail = new Product_Detail
										{
											STT = lstSelectProduct_Detail.Max((Product_Detail e) => e.STT),
											ID = Guid.NewGuid().ToString(),
											NAME = HangHoa.NAME,
											MA = HangHoa.MA,
											ID_HANGHOA = CTKM_Tang.ID_HANGHOA,
											ID_HANGHOAKHO = HangHoaKho.ID,
											DONGIA = 0.0,
											ID_DVT = CTKM_Tang.ID_DVT,
											SOLUONG = (double)PhanNguyen * CTKM_Tang.SOLUONG,
											CHIETKHAU = 0.0,
											TONGTIENGIAMGIA = 0.0,
											THANHTIEN = 0.0,
											THUESUAT = 0.0,
											TONGTIENVAT = 0.0,
											TONGCONG = 0.0
										};
										if (CTKM_Tang.SOTIEN > 0.0)
										{
											newProduct_Detail.TONGTIENGIAMGIA = CTKM_Tang.SOTIEN * (double)PhanNguyen;
											newProduct_Detail.TYPE = "TONGTIENGIAMGIA";
											dm_ThueSuat clsdm_ThueSuat3 = new dm_ThueSuat();
											if (!string.IsNullOrEmpty(HangHoa.ID_THUESUAT))
											{
												clsdm_ThueSuat3 = (await _context.dm_ThueSuat.FirstOrDefaultAsync((dm_ThueSuat e) => e.LOC_ID == HangHoa.LOC_ID && e.ID == HangHoa.ID_THUESUAT)) ?? new dm_ThueSuat();
											}
											API.TinhTong(newProduct_Detail, "", lstProduct_Detail, clsdm_ThueSuat3);
											if (HangHoa != null && HangHoa.MUCTHUE != 0.0)
											{
												SOTIENTHUE_KM += newProduct_Detail.TONGTIENGIAMGIA * HangHoa.MUCTHUE / 100.0;
											}
										}
										newProduct_Detail.ID_KHO = ID_KHO;
										newProduct_Detail.ISKHUYENMAI = true;
										newProduct_Detail.ID_KHUYENMAI = itm3.ID;
										if (HangHoa != null && HangHoa.ID_DVT == newProduct_Detail.ID_DVT)
										{
											newProduct_Detail.NAME_DVT = HangHoa.NAME_DVT;
											if (!string.IsNullOrEmpty(HangHoa.ID_DVT_QD))
											{
												newProduct_Detail.TYLE_QD = HangHoa.TYLE_QD;
											}
											else if (HangHoa.LOAIHANGHOA == 2.ToString())
											{
												newProduct_Detail.TYLE_QD = 0.0;
											}
											else
											{
												newProduct_Detail.TYLE_QD = 1.0;
											}
											if (newProduct_Detail.SOLUONG != 0.0 && HangHoa != null && HangHoa.MUCTHUE != 0.0)
											{
												SOTIENTHUE_KM += newProduct_Detail.SOLUONG * newProduct_Detail.TYLE_QD * HangHoa.GIA01 * HangHoa.MUCTHUE / 100.0;
											}
										}
										else
										{
											if (HangHoa == null || !(HangHoa.ID_DVT_QD == newProduct_Detail.ID_DVT))
											{
												return Ok(new ApiResponse
												{
													Success = false,
													Message = "Không tìm thấy thông tin sản phẩm với đơn vị tính " + newProduct_Detail.ID_DVT + " Kiểm tra CTKM" + itm3.NAME,
													Data = null
												});
											}
											if (!string.IsNullOrEmpty(HangHoa.ID_DVT_QD))
											{
												newProduct_Detail.NAME_DVT = HangHoa.NAME_DVT_QD;
												newProduct_Detail.TYLE_QD = 1.0;
											}
											if (newProduct_Detail.SOLUONG != 0.0 && HangHoa != null && HangHoa.MUCTHUE != 0.0)
											{
												SOTIENTHUE_KM += newProduct_Detail.SOLUONG * newProduct_Detail.TYLE_QD * HangHoa.GIA01_QD * HangHoa.MUCTHUE / 100.0;
											}
										}
										newProduct_Detail.TONGSOLUONG = newProduct_Detail.TYLE_QD * newProduct_Detail.SOLUONG;
										if (HangHoa != null && HangHoa.LOAIHANGHOA == 1.ToString())
										{
											newProduct_Detail.ID_KHUYENMAI = newProduct_Detail.ID_HANGHOA;
											SP_Parameter objParameter = new SP_Parameter
											{
												LOC_ID = itm3.LOC_ID,
												ID_KHO = newProduct_Detail.ID_KHO,
												ID_COMBO = newProduct_Detail.ID_HANGHOA
											};
											ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
											if (await ExecuteStoredProc1.Sp_Get_DanhSachSanPhamKho_Combo(objParameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse2 })
											{
												List<Product_Detail> lst_ChiTiet2 = ApiResponse2.Data as List<Product_Detail>;
												if (lst_ChiTiet2 != null)
												{
													foreach (Product_Detail ChiTiet3 in lst_ChiTiet2)
													{
														ChiTiet3.STT = newProduct_Detail.STT;
														ChiTiet3.ID = Guid.NewGuid().ToString();
														ChiTiet3.ID_DVT = ChiTiet3.ID_DVT_COMBO;
														ChiTiet3.SOLUONG = newProduct_Detail.SOLUONG * ChiTiet3.QTY_COMBO;
														ChiTiet3.TYLE_QD = ChiTiet3.TYLE_QD_COMBO;
														ChiTiet3.TONGSOLUONG = newProduct_Detail.SOLUONG * ChiTiet3.QTY_TOTAL_COMBO;
														ChiTiet3.DONGIA = 0.0;
														ChiTiet3.CHIETKHAU = 0.0;
														ChiTiet3.TONGTIENGIAMGIA = 0.0;
														ChiTiet3.THANHTIEN = 0.0;
														ChiTiet3.THUESUAT = 0.0;
														ChiTiet3.TONGTIENVAT = 0.0;
														ChiTiet3.TONGCONG = 0.0;
														ChiTiet3.ISKHUYENMAI = true;
														ChiTiet3.ID_KHUYENMAI = itm3.ID;
														ChiTiet3.ISCOMBO = true;
														ChiTiet3.ID_COMBO = newProduct_Detail.ID_HANGHOA;
														lstProduct_Detail.Add(ChiTiet3);
													}
												}
											}
										}
										lstProduct_Detail.Add(newProduct_Detail);
									}
									intCoLayKhuyenMai++;
								}
							}
							if (PhanNguyen > 0 && (itm3.CHIETKHAU > 0.0 || itm3.TIENGIAM > 0.0))
							{
								lstSelectProduct_Detail = lstSelectProduct_Detail.Where((Product_Detail e) => !e.ISKHUYENMAI).ToList();
								Dictionary<string, int> lstCTKM_YC = new Dictionary<string, int>();
								if (lstChuongTrinhKhuyenMai_YeuCau != null && lstChuongTrinhKhuyenMai_YeuCau.Count > 0)
								{
									foreach (dm_ChuongTrinhKhuyenMai_YeuCau CTKM_YC2 in lstChuongTrinhKhuyenMai_YeuCau)
									{
										lstCTKM_YC.Add(CTKM_YC2.ID_HANGHOA, CTKM_YC2.HINHTHUC);
									}
								}
								if (lstSelectProduct_Detail != null && lstSelectProduct_Detail.Count > 0)
								{
									double SumSoLuong = 0.0;
									double SumTien;
									if (lstChuongTrinhKhuyenMai_YeuCau != null && lstChuongTrinhKhuyenMai_YeuCau.Count > 0)
									{
										SumTien = lstSelectProduct_Detail.Where((Product_Detail e) => (!e.ISDALAYKHUYENMAI && lstCTKM_YC.Count<KeyValuePair<string, int>>((KeyValuePair<string, int> keyValuePair) => keyValuePair.Key.Contains(e.ID_HANGHOA) && keyValuePair.Value == 0) > 0) || lstCTKM_YC.Count<KeyValuePair<string, int>>((KeyValuePair<string, int> keyValuePair) => keyValuePair.Key.Contains(e.ID_NHOMHANGHOA) && keyValuePair.Value == 1) > 0).Sum((Product_Detail product_Detail) => Convert.ToDouble(product_Detail.THANHTIEN));
										if (!string.IsNullOrEmpty(itm3.ID_DVT_DATKM))
										{
											SumSoLuong = lstSelectProduct_Detail.Where((Product_Detail e) => !e.ISDALAYKHUYENMAI && (lstCTKM_YC.Count<KeyValuePair<string, int>>((KeyValuePair<string, int> keyValuePair) => keyValuePair.Key.Contains(e.ID_HANGHOA) && keyValuePair.Value == 0) > 0 || lstCTKM_YC.Count<KeyValuePair<string, int>>((KeyValuePair<string, int> keyValuePair) => keyValuePair.Key.Contains(e.ID_NHOMHANGHOA) && keyValuePair.Value == 1) > 0) && e.ID_DVT == itm3.ID_DVT_DATKM).Sum((Product_Detail product_Detail) => Convert.ToDouble(product_Detail.SOLUONG));
										}
									}
									else
									{
										SumTien = lstSelectProduct_Detail.Where((Product_Detail e) => !e.ISDALAYKHUYENMAI && !e.ISKHUYENMAI).Sum((Product_Detail product_Detail) => Convert.ToDouble(product_Detail.TONGCONG));
										if (!string.IsNullOrEmpty(itm3.ID_DVT_DATKM))
										{
											SumSoLuong = lstSelectProduct_Detail.Where((Product_Detail e) => !e.ISDALAYKHUYENMAI && e.ID_DVT == itm3.ID_DVT_DATKM).Sum((Product_Detail product_Detail) => Convert.ToDouble(product_Detail.TONGSOLUONG));
										}
									}
									if (((itm3.SOLUONG_DATKM != 0.0 || itm3.SOLUONG_DATKM_DEN != 0.0) && SumSoLuong != 0.0 && itm3.SOLUONG_DATKM <= SumSoLuong && (itm3.SOLUONG_DATKM_DEN == 0.0 || itm3.SOLUONG_DATKM_DEN >= SumSoLuong)) || ((itm3.TONGTIEN_DATKM != 0.0 || itm3.TONGTIEN_DATKM_DEN != 0.0) && SumTien != 0.0 && itm3.TONGTIEN_DATKM <= SumTien && (itm3.TONGTIEN_DATKM_DEN == 0.0 || itm3.TONGTIEN_DATKM_DEN >= SumTien)))
									{
										if (!itm3.ISTONGHOADON)
										{
											foreach (Product_Detail ChiTiet4 in lstSelectProduct_Detail)
											{
												dm_ThueSuat clsdm_ThueSuat4 = new dm_ThueSuat();
												if (!string.IsNullOrEmpty(ChiTiet4.ID_THUESUAT))
												{
													clsdm_ThueSuat4 = (await _context.dm_ThueSuat.FirstOrDefaultAsync((dm_ThueSuat e) => e.LOC_ID == ChiTiet4.LOC_ID && e.ID == ChiTiet4.ID_THUESUAT)) ?? new dm_ThueSuat();
												}
												if (itm3.CHIETKHAU > 0.0)
												{
													ChiTiet4.CHIETKHAU = itm3.CHIETKHAU;
													ChiTiet4.TYPE = "CHIETKHAU";
													API.TinhTong(ChiTiet4, "", lstProduct_Detail, clsdm_ThueSuat4);
													ChiTiet4.ID_KHUYENMAI = itm3.ID;
													view_dm_HangHoa objHangHoa2 = await _context.view_dm_HangHoa.FirstOrDefaultAsync((view_dm_HangHoa e) => e.LOC_ID == ChiTiet4.LOC_ID && e.ID == ChiTiet4.ID_HANGHOA);
													if (objHangHoa2 != null && objHangHoa2.MUCTHUE != 0.0)
													{
														SOTIENTHUE_KM += ChiTiet4.TONGTIENGIAMGIA * objHangHoa2.MUCTHUE / 100.0;
													}
												}
												else if (itm3.TIENGIAM > 0.0)
												{
													ChiTiet4.TONGTIENGIAMGIA = itm3.TIENGIAM;
													ChiTiet4.TYPE = "TONGTIENGIAMGIA";
													API.TinhTong(ChiTiet4, "", lstProduct_Detail, clsdm_ThueSuat4);
													ChiTiet4.ID_KHUYENMAI = itm3.ID;
													view_dm_HangHoa objHangHoa3 = await _context.view_dm_HangHoa.FirstOrDefaultAsync((view_dm_HangHoa e) => e.LOC_ID == ChiTiet4.LOC_ID && e.ID == ChiTiet4.ID_HANGHOA);
													if (objHangHoa3 != null && objHangHoa3.MUCTHUE != 0.0)
													{
														SOTIENTHUE_KM += ChiTiet4.TONGTIENGIAMGIA * objHangHoa3.MUCTHUE / 100.0;
													}
												}
											}
										}
										else
										{
											string ID_KHO2 = lstProduct_Detail.Select((Product_Detail e) => e.ID_KHO).FirstOrDefault();
											view_dm_HangHoa HangHoa2 = await _context.view_dm_HangHoa.FirstOrDefaultAsync((view_dm_HangHoa e) => e.LOC_ID == itm3.LOC_ID && e.MA == API.GTBH);
											if (HangHoa2 != null)
											{
												dm_HangHoa_Kho HangHoaKho2 = await _context.dm_HangHoa_Kho.FirstOrDefaultAsync((dm_HangHoa_Kho e) => e.LOC_ID == itm3.LOC_ID && e.ID_HANGHOA == HangHoa2.ID && e.ID_KHO == ID_KHO2);
												if (HangHoaKho2 != null)
												{
													Product_Detail newProduct_Detail2 = new Product_Detail
													{
														STT = lstSelectProduct_Detail.Max((Product_Detail e) => e.STT),
														ID = Guid.NewGuid().ToString(),
														NAME = HangHoa2.NAME,
														MA = HangHoa2.MA,
														ID_HANGHOA = HangHoaKho2.ID_HANGHOA,
														ID_HANGHOAKHO = HangHoaKho2.ID,
														DONGIA = 0.0,
														ID_DVT = HangHoa2.ID_DVT,
														NAME_DVT = HangHoa2.NAME_DVT,
														SOLUONG = 0.0
													};
													dm_ThueSuat clsdm_ThueSuat5 = new dm_ThueSuat();
													if (!string.IsNullOrEmpty(HangHoa2.ID_THUESUAT))
													{
														clsdm_ThueSuat5 = (await _context.dm_ThueSuat.FirstOrDefaultAsync((dm_ThueSuat e) => e.LOC_ID == itm3.LOC_ID && e.ID == HangHoa2.ID_THUESUAT)) ?? new dm_ThueSuat();
													}
													if (itm3.CHIETKHAU > 0.0)
													{
														newProduct_Detail2.CHIETKHAU = itm3.CHIETKHAU;
														newProduct_Detail2.TONGTIENGIAMGIA = lstSelectProduct_Detail.Where((Product_Detail e) => !e.ISKHUYENMAI).Sum((Product_Detail e) => e.THANHTIEN) * newProduct_Detail2.CHIETKHAU / 100.0;
														newProduct_Detail2.TYPE = "TONGTIENGIAMGIA";
														API.TinhTong(newProduct_Detail2, "", lstProduct_Detail, clsdm_ThueSuat5);
														newProduct_Detail2.ISDALAYKHUYENMAI = true;
														newProduct_Detail2.SOLUONGDALAYKHUYENMAI = newProduct_Detail2.TONGSOLUONG;
														newProduct_Detail2.ID_KHUYENMAI = itm3.ID;
														foreach (Product_Detail s in lstSelectProduct_Detail)
														{
															view_dm_HangHoa objHangHoa4 = await _context.view_dm_HangHoa.FirstOrDefaultAsync((view_dm_HangHoa e) => e.LOC_ID == itm3.LOC_ID && (e.ID == s.ID || e.ID_NHOMHANGHOA == s.ID));
															if (objHangHoa4 != null && objHangHoa4.MUCTHUE != 0.0)
															{
																SOTIENTHUE_KM += newProduct_Detail2.TONGTIENGIAMGIA * objHangHoa4.MUCTHUE / 100.0;
																break;
															}
														}
													}
													else if (itm3.TIENGIAM > 0.0)
													{
														newProduct_Detail2.TONGTIENGIAMGIA = itm3.TIENGIAM * (double)PhanNguyen;
														newProduct_Detail2.TYPE = "TONGTIENGIAMGIA";
														API.TinhTong(newProduct_Detail2, "", lstProduct_Detail, clsdm_ThueSuat5);
														newProduct_Detail2.ISDALAYKHUYENMAI = true;
														newProduct_Detail2.SOLUONGDALAYKHUYENMAI = newProduct_Detail2.TONGSOLUONG;
														newProduct_Detail2.ID_KHUYENMAI = itm3.ID;
														foreach (Product_Detail s2 in lstSelectProduct_Detail)
														{
															view_dm_HangHoa objHangHoa5 = await _context.view_dm_HangHoa.FirstOrDefaultAsync((view_dm_HangHoa e) => e.LOC_ID == itm3.LOC_ID && (e.ID == s2.ID || e.ID_NHOMHANGHOA == s2.ID));
															if (objHangHoa5 != null && objHangHoa5.MUCTHUE != 0.0)
															{
																SOTIENTHUE_KM += newProduct_Detail2.TONGTIENGIAMGIA * objHangHoa5.MUCTHUE / 100.0;
																break;
															}
														}
													}
													newProduct_Detail2.ID_KHO = ID_KHO2;
													newProduct_Detail2.ISDALAYKHUYENMAI = true;
													newProduct_Detail2.ISKHUYENMAI = true;
													newProduct_Detail2.ID_KHUYENMAI = itm3.ID;
													lstProduct_Detail.Add(newProduct_Detail2);
													foreach (Product_Detail ChiTiet5 in lstSelectProduct_Detail)
													{
														ChiTiet5.ISDALAYKHUYENMAI = true;
													}
												}
											}
										}
										intCoLayKhuyenMai++;
									}
								}
							}
						}
					}
					else
					{
						List<Product_Detail> lstSelectProduct_Detail2 = new List<Product_Detail>();
						Dictionary<string, int> lstCTKM_YC2 = new Dictionary<string, int>();
						double MUCTHUE = 0.0;
						int PhanNguyen2 = 0;
						bool bolBatBuoc = false;
						if (lstChuongTrinhKhuyenMai_YeuCau != null && lstChuongTrinhKhuyenMai_YeuCau.Count > 0)
						{
							bool isOk = true;
							foreach (dm_ChuongTrinhKhuyenMai_YeuCau CTKM_YC3 in lstChuongTrinhKhuyenMai_YeuCau)
							{
								lstCTKM_YC2.Add(CTKM_YC3.ID_HANGHOA, CTKM_YC3.HINHTHUC);
								view_dm_HangHoa objHangHoa6 = await _context.view_dm_HangHoa.FirstOrDefaultAsync((view_dm_HangHoa e) => e.LOC_ID == itm3.LOC_ID && (e.ID == CTKM_YC3.ID_HANGHOA || e.ID_NHOMHANGHOA == CTKM_YC3.ID_HANGHOA));
								if (MUCTHUE == 0.0 && objHangHoa6 != null && objHangHoa6.MUCTHUE != 0.0)
								{
									MUCTHUE = objHangHoa6.MUCTHUE;
								}
								if (CTKM_YC3.ISBATBUOC)
								{
									double SumSoLuong2 = ((!bolConSoLuong) ? lstProduct_Detail.Where((Product_Detail e) => (!itm3.ISTONGHOADON || (itm3.ISTONGHOADON && !e.ISDALAYKHUYENMAI)) && ((e.ID_HANGHOA == CTKM_YC3.ID_HANGHOA && CTKM_YC3.HINHTHUC == 0) || (e.ID_NHOMHANGHOA == CTKM_YC3.ID_HANGHOA && CTKM_YC3.HINHTHUC == 1)) && e.ID_DVT == CTKM_YC3.ID_DVT).Sum((Product_Detail product_Detail) => Convert.ToDouble(product_Detail.SOLUONG)) : lstProduct_Detail_Tam.Where((Product_Detail e) => (!itm3.ISTONGHOADON || (itm3.ISTONGHOADON && !e.ISDALAYKHUYENMAI)) && ((e.ID_HANGHOA == CTKM_YC3.ID_HANGHOA && CTKM_YC3.HINHTHUC == 0) || (e.ID_NHOMHANGHOA == CTKM_YC3.ID_HANGHOA && CTKM_YC3.HINHTHUC == 1)) && e.ID_DVT == CTKM_YC3.ID_DVT).Sum((Product_Detail product_Detail) => Convert.ToDouble(product_Detail.SOLUONG - product_Detail.SOLUONGDALAY_KM)));
									if (SumSoLuong2 < CTKM_YC3.SOLUONG_BATBUOC)
									{
										isOk = false;
										break;
									}
								}
							}
							if (!isOk)
							{
								continue;
							}
							if (itm3.ISTINHLUYTUYEN)
							{
								bool bolThoatwhile = false;
								IEnumerable<dm_ChuongTrinhKhuyenMai_YeuCau> lstbatBuoc = lstChuongTrinhKhuyenMai_YeuCau.Where((dm_ChuongTrinhKhuyenMai_YeuCau e) => e.ISBATBUOC);
								if (lstbatBuoc != null && lstbatBuoc.Count() > 0)
								{
									while (!bolThoatwhile)
									{
										PhanNguyen2++;
										foreach (dm_ChuongTrinhKhuyenMai_YeuCau CTKM_YC4 in lstbatBuoc)
										{
											double SumSoLuong3 = ((!bolConSoLuong) ? lstProduct_Detail.Where((Product_Detail e) => (!itm3.ISTONGHOADON || (itm3.ISTONGHOADON && !e.ISDALAYKHUYENMAI)) && ((e.ID_HANGHOA == CTKM_YC4.ID_HANGHOA && CTKM_YC4.HINHTHUC == 0) || (e.ID_NHOMHANGHOA == CTKM_YC4.ID_HANGHOA && CTKM_YC4.HINHTHUC == 1)) && e.ID_DVT == CTKM_YC4.ID_DVT).Sum((Product_Detail product_Detail) => Convert.ToDouble(product_Detail.SOLUONG)) : lstProduct_Detail_Tam.Where((Product_Detail e) => (!itm3.ISTONGHOADON || (itm3.ISTONGHOADON && !e.ISDALAYKHUYENMAI)) && ((e.ID_HANGHOA == CTKM_YC4.ID_HANGHOA && CTKM_YC4.HINHTHUC == 0) || (e.ID_NHOMHANGHOA == CTKM_YC4.ID_HANGHOA && CTKM_YC4.HINHTHUC == 1)) && e.ID_DVT == CTKM_YC4.ID_DVT).Sum((Product_Detail product_Detail) => Convert.ToDouble(product_Detail.SOLUONG - product_Detail.SOLUONGDALAY_KM)));
											if (SumSoLuong3 < CTKM_YC4.SOLUONG_BATBUOC * (double)PhanNguyen2)
											{
												PhanNguyen2--;
												bolThoatwhile = true;
												break;
											}
											bolBatBuoc = true;
										}
									}
								}
							}
							List<Product_Detail> Tam = ((!bolConSoLuong) ? lstProduct_Detail.Where((Product_Detail e) => !string.IsNullOrEmpty(e.ID_HANGHOA) && !string.IsNullOrEmpty(e.ID_NHOMHANGHOA) && !e.ISKHUYENMAI).ToList() : lstProduct_Detail_Tam.Where((Product_Detail e) => !string.IsNullOrEmpty(e.ID_HANGHOA) && !string.IsNullOrEmpty(e.ID_NHOMHANGHOA) && !e.ISKHUYENMAI && e.SOLUONG - e.SOLUONGDALAY_KM > 0.0).ToList());
							if (Tam != null)
							{
								lstSelectProduct_Detail2 = Tam.Where((Product_Detail e) => lstCTKM_YC2.Count<KeyValuePair<string, int>>((KeyValuePair<string, int> keyValuePair) => keyValuePair.Key.Contains(e.ID_HANGHOA) && keyValuePair.Value == 0) > 0 || lstCTKM_YC2.Count<KeyValuePair<string, int>>((KeyValuePair<string, int> keyValuePair) => keyValuePair.Key.Contains(e.ID_NHOMHANGHOA) && keyValuePair.Value == 1) > 0).ToList();
							}
						}
						else if (bolConSoLuong)
						{
							lstSelectProduct_Detail2 = lstProduct_Detail.Where((Product_Detail e) => !e.ISKHUYENMAI && e.SOLUONG - e.SOLUONGDALAY_KM > 0.0).ToList();
						}
						else
						{
							lstSelectProduct_Detail2 = lstProduct_Detail.Where((Product_Detail e) => !e.ISKHUYENMAI).ToList();
						}
						if (lstSelectProduct_Detail2 != null && lstSelectProduct_Detail2.Count > 0)
						{
							double SumSoLuong4 = 0.0;
							double SumTien2;
							if (lstChuongTrinhKhuyenMai_YeuCau != null && lstChuongTrinhKhuyenMai_YeuCau.Count > 0)
							{
								SumTien2 = lstSelectProduct_Detail2.Where((Product_Detail e) => (!e.ISKHUYENMAI && lstCTKM_YC2.Count<KeyValuePair<string, int>>((KeyValuePair<string, int> keyValuePair) => keyValuePair.Key.Contains(e.ID_HANGHOA) && keyValuePair.Value == 0) > 0) || lstCTKM_YC2.Count<KeyValuePair<string, int>>((KeyValuePair<string, int> keyValuePair) => keyValuePair.Key.Contains(e.ID_NHOMHANGHOA) && keyValuePair.Value == 1) > 0).Sum((Product_Detail product_Detail) => Convert.ToDouble(product_Detail.THANHTIEN));
								if (!string.IsNullOrEmpty(itm3.ID_DVT_DATKM))
								{
									SumSoLuong4 = ((!bolConSoLuong) ? lstSelectProduct_Detail2.Where((Product_Detail e) => !e.ISKHUYENMAI && (lstCTKM_YC2.Count<KeyValuePair<string, int>>((KeyValuePair<string, int> keyValuePair) => keyValuePair.Key.Contains(e.ID_HANGHOA) && keyValuePair.Value == 0) > 0 || lstCTKM_YC2.Count<KeyValuePair<string, int>>((KeyValuePair<string, int> keyValuePair) => keyValuePair.Key.Contains(e.ID_NHOMHANGHOA) && keyValuePair.Value == 1) > 0) && e.ID_DVT == itm3.ID_DVT_DATKM).Sum((Product_Detail product_Detail) => Convert.ToDouble(product_Detail.SOLUONG)) : lstSelectProduct_Detail2.Where((Product_Detail e) => !e.ISKHUYENMAI && (lstCTKM_YC2.Count<KeyValuePair<string, int>>((KeyValuePair<string, int> keyValuePair) => keyValuePair.Key.Contains(e.ID_HANGHOA) && keyValuePair.Value == 0) > 0 || lstCTKM_YC2.Count<KeyValuePair<string, int>>((KeyValuePair<string, int> keyValuePair) => keyValuePair.Key.Contains(e.ID_NHOMHANGHOA) && keyValuePair.Value == 1) > 0) && e.ID_DVT == itm3.ID_DVT_DATKM).Sum((Product_Detail product_Detail) => Convert.ToDouble(product_Detail.SOLUONG - product_Detail.SOLUONGDALAY_KM)));
								}
							}
							else
							{
								SumTien2 = lstSelectProduct_Detail2.Where((Product_Detail e) => !e.ISKHUYENMAI).Sum((Product_Detail product_Detail) => Convert.ToDouble(product_Detail.TONGCONG));
								if (!string.IsNullOrEmpty(itm3.ID_DVT_DATKM))
								{
									SumSoLuong4 = ((!bolConSoLuong) ? lstSelectProduct_Detail2.Where((Product_Detail e) => !e.ISKHUYENMAI && e.ID_DVT == itm3.ID_DVT_DATKM).Sum((Product_Detail product_Detail) => Convert.ToDouble(product_Detail.SOLUONG)) : lstSelectProduct_Detail2.Where((Product_Detail e) => !e.ISKHUYENMAI && e.ID_DVT == itm3.ID_DVT_DATKM).Sum((Product_Detail product_Detail) => Convert.ToDouble(product_Detail.SOLUONG - product_Detail.SOLUONGDALAY_KM)));
								}
							}
							if (((itm3.SOLUONG_DATKM != 0.0 || itm3.SOLUONG_DATKM_DEN != 0.0) && SumSoLuong4 != 0.0 && itm3.SOLUONG_DATKM <= SumSoLuong4 && (itm3.SOLUONG_DATKM_DEN == 0.0 || itm3.SOLUONG_DATKM_DEN >= SumSoLuong4)) || ((itm3.TONGTIEN_DATKM != 0.0 || itm3.TONGTIEN_DATKM != 0.0) && SumTien2 != 0.0 && itm3.TONGTIEN_DATKM <= SumTien2 && (itm3.TONGTIEN_DATKM_DEN == 0.0 || itm3.TONGTIEN_DATKM_DEN >= SumTien2)))
							{
								List<dm_ChuongTrinhKhuyenMai_Tang> lstdm_ChuongTrinhKhuyenMai_Tang2 = await _context.dm_ChuongTrinhKhuyenMai_Tang.Where((dm_ChuongTrinhKhuyenMai_Tang e) => e.LOC_ID == itm3.LOC_ID && e.ID_CHUONGTRINHKHUYENMAI == itm3.ID).ToListAsync();
								if (lstdm_ChuongTrinhKhuyenMai_Tang2 != null && lstdm_ChuongTrinhKhuyenMai_Tang2.Count > 0)
								{
									string ID_KHO3 = lstProduct_Detail.Select((Product_Detail e) => e.ID_KHO).FirstOrDefault();
									if (lstdm_ChuongTrinhKhuyenMai_Tang2 != null)
									{
										int SLKM_SL = ((itm3.SOLUONG_DATKM_DEN != 0.0) ? (Convert.ToInt32(SumSoLuong4) / Convert.ToInt32(itm3.SOLUONG_DATKM_DEN)) : ((itm3.SOLUONG_DATKM != 0.0) ? (Convert.ToInt32(SumSoLuong4) / Convert.ToInt32(itm3.SOLUONG_DATKM)) : 0));
										int SLKM_TIEN = ((itm3.TONGTIEN_DATKM_DEN != 0.0) ? (Convert.ToInt32(SumTien2) / Convert.ToInt32(itm3.TONGTIEN_DATKM_DEN)) : ((itm3.TONGTIEN_DATKM != 0.0) ? (Convert.ToInt32(SumTien2) / Convert.ToInt32(itm3.TONGTIEN_DATKM)) : 0));
										if (bolBatBuoc)
										{
											SLKM_SL = ((SLKM_SL > PhanNguyen2) ? PhanNguyen2 : SLKM_SL);
										}
										foreach (dm_ChuongTrinhKhuyenMai_Tang CTKM_Tang2 in lstdm_ChuongTrinhKhuyenMai_Tang2)
										{
											dm_HangHoa_Kho HangHoaKho3 = await _context.dm_HangHoa_Kho.FirstOrDefaultAsync((dm_HangHoa_Kho e) => e.LOC_ID == itm3.LOC_ID && e.ID_HANGHOA == CTKM_Tang2.ID_HANGHOA && e.ID_KHO == ID_KHO3);
											view_dm_HangHoa HangHoa3 = await _context.view_dm_HangHoa.FirstOrDefaultAsync((view_dm_HangHoa e) => e.LOC_ID == itm3.LOC_ID && e.ID == CTKM_Tang2.ID_HANGHOA);
											if (HangHoaKho3 == null || HangHoa3 == null)
											{
												continue;
											}
											Product_Detail newProduct_Detail3 = new Product_Detail
											{
												STT = lstSelectProduct_Detail2.Max((Product_Detail e) => e.STT),
												ID = Guid.NewGuid().ToString(),
												NAME = HangHoa3.NAME,
												MA = HangHoa3.MA,
												ID_HANGHOA = CTKM_Tang2.ID_HANGHOA,
												ID_HANGHOAKHO = HangHoaKho3.ID,
												DONGIA = 0.0,
												ID_DVT = CTKM_Tang2.ID_DVT
											};
											if (SLKM_SL > SLKM_TIEN)
											{
												newProduct_Detail3.SOLUONG = (double)((!itm3.ISTINHLUYTUYEN) ? 1 : SLKM_SL) * CTKM_Tang2.SOLUONG;
											}
											else
											{
												newProduct_Detail3.SOLUONG = (double)((!itm3.ISTINHLUYTUYEN) ? 1 : SLKM_TIEN) * CTKM_Tang2.SOLUONG;
											}
											newProduct_Detail3.CHIETKHAU = 0.0;
											newProduct_Detail3.TONGTIENGIAMGIA = 0.0;
											newProduct_Detail3.THANHTIEN = 0.0;
											newProduct_Detail3.THUESUAT = 0.0;
											newProduct_Detail3.TONGTIENVAT = 0.0;
											newProduct_Detail3.TONGCONG = 0.0;
											if (CTKM_Tang2.SOTIEN > 0.0)
											{
												newProduct_Detail3.TONGTIENGIAMGIA = CTKM_Tang2.SOTIEN;
												newProduct_Detail3.TYPE = "TONGTIENGIAMGIA";
												dm_ThueSuat clsdm_ThueSuat6 = new dm_ThueSuat();
												if (!string.IsNullOrEmpty(HangHoa3.ID_THUESUAT))
												{
													clsdm_ThueSuat6 = (await _context.dm_ThueSuat.FirstOrDefaultAsync((dm_ThueSuat e) => e.LOC_ID == HangHoa3.LOC_ID && e.ID == HangHoa3.ID_THUESUAT)) ?? new dm_ThueSuat();
												}
												API.TinhTong(newProduct_Detail3, "", lstProduct_Detail, clsdm_ThueSuat6);
												if (MUCTHUE != 0.0)
												{
													SOTIENTHUE_KM += newProduct_Detail3.TONGTIENGIAMGIA * MUCTHUE / 100.0;
												}
											}
											newProduct_Detail3.ID_KHO = ID_KHO3;
											newProduct_Detail3.ISDALAYKHUYENMAI = true;
											newProduct_Detail3.ISKHUYENMAI = true;
											newProduct_Detail3.ID_KHUYENMAI = itm3.ID;
											if (HangHoa3 != null && HangHoa3.ID_DVT == newProduct_Detail3.ID_DVT)
											{
												newProduct_Detail3.NAME_DVT = HangHoa3.NAME_DVT;
												if (!string.IsNullOrEmpty(HangHoa3.ID_DVT_QD))
												{
													newProduct_Detail3.TYLE_QD = HangHoa3.TYLE_QD;
												}
												else if (HangHoa3.LOAIHANGHOA == 2.ToString())
												{
													newProduct_Detail3.TYLE_QD = 0.0;
												}
												else
												{
													newProduct_Detail3.TYLE_QD = 1.0;
												}
												if (newProduct_Detail3.SOLUONG != 0.0 && HangHoa3 != null && HangHoa3.MUCTHUE != 0.0)
												{
													SOTIENTHUE_KM += newProduct_Detail3.SOLUONG * HangHoa3.GIA01 * HangHoa3.MUCTHUE / 100.0;
												}
											}
											else
											{
												if (HangHoa3 == null || !(HangHoa3.ID_DVT_QD == newProduct_Detail3.ID_DVT))
												{
													return Ok(new ApiResponse
													{
														Success = false,
														Message = "Không tìm thấy thông tin sản phẩm với đơn vị tính " + newProduct_Detail3.ID_DVT + " Kiểm tra CTKM" + itm3.NAME,
														Data = null
													});
												}
												if (!string.IsNullOrEmpty(HangHoa3.ID_DVT_QD))
												{
													newProduct_Detail3.NAME_DVT = HangHoa3.NAME_DVT_QD;
													newProduct_Detail3.TYLE_QD = 1.0;
												}
												if (newProduct_Detail3.SOLUONG != 0.0 && HangHoa3 != null && HangHoa3.MUCTHUE != 0.0)
												{
													SOTIENTHUE_KM += newProduct_Detail3.SOLUONG * newProduct_Detail3.TYLE_QD * HangHoa3.GIA01_QD * HangHoa3.MUCTHUE / 100.0;
												}
											}
											newProduct_Detail3.TONGSOLUONG = newProduct_Detail3.TYLE_QD * newProduct_Detail3.SOLUONG;
											if (HangHoa3 != null && HangHoa3.LOAIHANGHOA == 1.ToString())
											{
												newProduct_Detail3.ID_KHUYENMAI = newProduct_Detail3.ID_HANGHOA;
												SP_Parameter objParameter2 = new SP_Parameter
												{
													LOC_ID = itm3.LOC_ID,
													ID_KHO = newProduct_Detail3.ID_KHO,
													ID_COMBO = newProduct_Detail3.ID_HANGHOA
												};
												ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
												if (await ExecuteStoredProc1.Sp_Get_DanhSachSanPhamKho_Combo(objParameter2) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse3 })
												{
													List<Product_Detail> lst_ChiTiet3 = ApiResponse3.Data as List<Product_Detail>;
													if (lst_ChiTiet3 != null)
													{
														foreach (Product_Detail ChiTiet6 in lst_ChiTiet3)
														{
															ChiTiet6.STT = newProduct_Detail3.STT;
															ChiTiet6.ID = Guid.NewGuid().ToString();
															ChiTiet6.ID_DVT = ChiTiet6.ID_DVT_COMBO;
															ChiTiet6.SOLUONG = newProduct_Detail3.SOLUONG * ChiTiet6.QTY_COMBO;
															ChiTiet6.TYLE_QD = ChiTiet6.TYLE_QD_COMBO;
															ChiTiet6.TONGSOLUONG = newProduct_Detail3.SOLUONG * ChiTiet6.QTY_TOTAL_COMBO;
															ChiTiet6.DONGIA = 0.0;
															ChiTiet6.CHIETKHAU = 0.0;
															ChiTiet6.TONGTIENGIAMGIA = 0.0;
															ChiTiet6.THANHTIEN = 0.0;
															ChiTiet6.THUESUAT = 0.0;
															ChiTiet6.TONGTIENVAT = 0.0;
															ChiTiet6.TONGCONG = 0.0;
															ChiTiet6.ISKHUYENMAI = true;
															ChiTiet6.ID_KHUYENMAI = itm3.ID;
															ChiTiet6.ISCOMBO = true;
															ChiTiet6.ID_COMBO = newProduct_Detail3.ID_HANGHOA;
															lstProduct_Detail.Add(ChiTiet6);
														}
													}
												}
											}
											lstProduct_Detail.Add(newProduct_Detail3);
										}
										if (lstChuongTrinhKhuyenMai_YeuCau != null && lstChuongTrinhKhuyenMai_YeuCau.Count > 0)
										{
											double SoLuongYeuCau = ((itm3.SOLUONG_DATKM_DEN != 0.0) ? itm3.SOLUONG_DATKM_DEN : itm3.SOLUONG_DATKM) * (double)((!itm3.ISTINHLUYTUYEN) ? 1 : SLKM_SL);
											foreach (dm_ChuongTrinhKhuyenMai_YeuCau CTKM_YC5 in lstChuongTrinhKhuyenMai_YeuCau.OrderByDescending((dm_ChuongTrinhKhuyenMai_YeuCau e) => e.ISBATBUOC))
											{
												foreach (Product_Detail ChiTiet7 in lstProduct_Detail_Tam.Where((Product_Detail e) => (!itm3.ISTONGHOADON || (itm3.ISTONGHOADON && !e.ISDALAYKHUYENMAI)) && ((e.ID_HANGHOA == CTKM_YC5.ID_HANGHOA && CTKM_YC5.HINHTHUC == 0) || (e.ID_NHOMHANGHOA == CTKM_YC5.ID_HANGHOA && CTKM_YC5.HINHTHUC == 1)) && e.ID_DVT == CTKM_YC5.ID_DVT && e.SOLUONG - e.SOLUONGDALAY_KM > 0.0).ToList())
												{
													if (SoLuongYeuCau == 0.0)
													{
														break;
													}
													if (SoLuongYeuCau - (ChiTiet7.SOLUONG - ChiTiet7.SOLUONGDALAY_KM) > 0.0)
													{
														ChiTiet7.SOLUONGDALAY_KM += ChiTiet7.SOLUONG - ChiTiet7.SOLUONGDALAY_KM;
														SoLuongYeuCau -= ChiTiet7.SOLUONGDALAY_KM;
													}
													else
													{
														ChiTiet7.SOLUONGDALAY_KM += SoLuongYeuCau;
														SoLuongYeuCau = 0.0;
													}
												}
											}
										}
										intCoLayKhuyenMai++;
									}
								}
								if (itm3.CHIETKHAU > 0.0 || itm3.TIENGIAM > 0.0)
								{
									if (!itm3.ISTONGHOADON)
									{
										foreach (Product_Detail ChiTiet8 in lstProduct_Detail.Where((Product_Detail product_Detail) => lstSelectProduct_Detail2.Where((Product_Detail e) => e.ID == product_Detail.ID).Count() > 0))
										{
											double SLKM_SL2 = ((itm3.SOLUONG_DATKM_DEN != 0.0) ? (ChiTiet8.SOLUONG / itm3.SOLUONG_DATKM_DEN) : ((itm3.SOLUONG_DATKM != 0.0) ? (ChiTiet8.SOLUONG / itm3.SOLUONG_DATKM) : 0.0));
											dm_ThueSuat clsdm_ThueSuat7 = new dm_ThueSuat();
											if (!string.IsNullOrEmpty(ChiTiet8.ID_THUESUAT))
											{
												clsdm_ThueSuat7 = (await _context.dm_ThueSuat.FirstOrDefaultAsync((dm_ThueSuat e) => e.LOC_ID == ChiTiet8.LOC_ID && e.ID == ChiTiet8.ID_THUESUAT)) ?? new dm_ThueSuat();
											}
											if (itm3.CHIETKHAU > 0.0)
											{
												ChiTiet8.CHIETKHAU = itm3.CHIETKHAU;
												ChiTiet8.TYPE = "CHIETKHAU";
												API.TinhTong(ChiTiet8, "", lstProduct_Detail, clsdm_ThueSuat7);
												ChiTiet8.SOLUONGDALAYKHUYENMAI = ChiTiet8.TONGSOLUONG;
												ChiTiet8.ID_KHUYENMAI = itm3.ID;
												view_dm_HangHoa objHangHoa7 = await _context.view_dm_HangHoa.FirstOrDefaultAsync((view_dm_HangHoa e) => e.LOC_ID == ChiTiet8.LOC_ID && e.ID == ChiTiet8.ID_HANGHOA);
												if (objHangHoa7 != null && objHangHoa7.MUCTHUE != 0.0)
												{
													SOTIENTHUE_KM += ChiTiet8.TONGTIENGIAMGIA * objHangHoa7.MUCTHUE / 100.0;
												}
											}
											else if (itm3.TIENGIAM > 0.0)
											{
												ChiTiet8.TONGTIENGIAMGIA = itm3.TIENGIAM * (itm3.ISTINHLUYTUYEN ? SLKM_SL2 : 1.0);
												ChiTiet8.TYPE = "TONGTIENGIAMGIA";
												API.TinhTong(ChiTiet8, "", lstProduct_Detail, clsdm_ThueSuat7);
												ChiTiet8.SOLUONGDALAYKHUYENMAI = ChiTiet8.TONGSOLUONG;
												ChiTiet8.ID_KHUYENMAI = itm3.ID;
												view_dm_HangHoa objHangHoa8 = await _context.view_dm_HangHoa.FirstOrDefaultAsync((view_dm_HangHoa e) => e.LOC_ID == ChiTiet8.LOC_ID && e.ID == ChiTiet8.ID_HANGHOA);
												if (objHangHoa8 != null && objHangHoa8.MUCTHUE != 0.0)
												{
													SOTIENTHUE_KM += ChiTiet8.TONGTIENGIAMGIA * objHangHoa8.MUCTHUE / 100.0;
												}
											}
										}
									}
									else
									{
										string ID_KHO4 = lstProduct_Detail.Select((Product_Detail e) => e.ID_KHO).FirstOrDefault();
										view_dm_HangHoa HangHoa4 = await _context.view_dm_HangHoa.FirstOrDefaultAsync((view_dm_HangHoa e) => e.LOC_ID == itm3.LOC_ID && e.MA == API.GTBH);
										if (HangHoa4 != null)
										{
											dm_HangHoa_Kho HangHoaKho4 = await _context.dm_HangHoa_Kho.FirstOrDefaultAsync((dm_HangHoa_Kho e) => e.LOC_ID == itm3.LOC_ID && e.ID_HANGHOA == HangHoa4.ID && e.ID_KHO == ID_KHO4);
											if (HangHoaKho4 != null)
											{
												Product_Detail newProduct_Detail4 = new Product_Detail
												{
													STT = lstSelectProduct_Detail2.Max((Product_Detail e) => e.STT) + 1,
													ID = Guid.NewGuid().ToString(),
													NAME = HangHoa4.NAME,
													MA = HangHoa4.MA,
													ID_HANGHOA = HangHoaKho4.ID_HANGHOA,
													ID_HANGHOAKHO = HangHoaKho4.ID,
													DONGIA = 0.0,
													ID_DVT = HangHoa4.ID_DVT,
													NAME_DVT = HangHoa4.NAME_DVT,
													SOLUONG = 0.0
												};
												dm_ThueSuat clsdm_ThueSuat8 = new dm_ThueSuat();
												if (!string.IsNullOrEmpty(HangHoa4.ID_THUESUAT))
												{
													clsdm_ThueSuat8 = (await _context.dm_ThueSuat.FirstOrDefaultAsync((dm_ThueSuat e) => e.LOC_ID == itm3.LOC_ID && e.ID == HangHoa4.ID_THUESUAT)) ?? new dm_ThueSuat();
												}
												if (itm3.CHIETKHAU > 0.0)
												{
													newProduct_Detail4.CHIETKHAU = itm3.CHIETKHAU;
													newProduct_Detail4.TONGTIENGIAMGIA = SumTien2 * newProduct_Detail4.CHIETKHAU / 100.0;
													newProduct_Detail4.TYPE = "TONGTIENGIAMGIA";
													API.TinhTong(newProduct_Detail4, "", lstProduct_Detail, clsdm_ThueSuat8);
													newProduct_Detail4.ISDALAYKHUYENMAI = true;
													newProduct_Detail4.SOLUONGDALAYKHUYENMAI = newProduct_Detail4.TONGSOLUONG;
													newProduct_Detail4.ID_KHUYENMAI = itm3.ID;
													if (MUCTHUE != 0.0)
													{
														SOTIENTHUE_KM += newProduct_Detail4.TONGTIENGIAMGIA * MUCTHUE / 100.0;
													}
												}
												else if (itm3.TIENGIAM > 0.0)
												{
													newProduct_Detail4.TONGTIENGIAMGIA = itm3.TIENGIAM * (double)((!itm3.ISTINHLUYTUYEN) ? 1 : PhanNguyen2);
													newProduct_Detail4.TYPE = "TONGTIENGIAMGIA";
													API.TinhTong(newProduct_Detail4, "", lstProduct_Detail, clsdm_ThueSuat8);
													newProduct_Detail4.ISDALAYKHUYENMAI = true;
													newProduct_Detail4.SOLUONGDALAYKHUYENMAI = newProduct_Detail4.TONGSOLUONG;
													newProduct_Detail4.ID_KHUYENMAI = itm3.ID;
													if (MUCTHUE != 0.0)
													{
														SOTIENTHUE_KM += newProduct_Detail4.TONGTIENGIAMGIA * MUCTHUE / 100.0;
													}
												}
												newProduct_Detail4.ID_KHO = ID_KHO4;
												newProduct_Detail4.ISDALAYKHUYENMAI = true;
												newProduct_Detail4.ISKHUYENMAI = true;
												newProduct_Detail4.ID_KHUYENMAI = itm3.ID;
												lstProduct_Detail.Add(newProduct_Detail4);
												foreach (Product_Detail ChiTiet9 in lstProduct_Detail)
												{
													ChiTiet9.ISDALAYKHUYENMAI = true;
												}
											}
										}
									}
									intCoLayKhuyenMai++;
								}
							}
						}
					}
					if (intCoLayKhuyenMai > 0)
					{
						lstDanhSachDaLayKhuyenMai.Add(itm3.MA);
					}
				}
				if (SOTIENTHUE_KM != 0.0)
				{
					lstProduct_Detail.Select((Product_Detail e) => e.ID_KHO).FirstOrDefault();
					view_dm_HangHoa HangHoa5 = await _context.view_dm_HangHoa.FirstOrDefaultAsync((view_dm_HangHoa e) => e.LOC_ID == LOC_ID && e.MA == API.TINHTHUE_KM);
					if (HangHoa5 != null)
					{
						dm_HangHoa_Kho HangHoaKho5 = await _context.dm_HangHoa_Kho.FirstOrDefaultAsync((dm_HangHoa_Kho e) => e.LOC_ID == LOC_ID && e.ID_HANGHOA == HangHoa5.ID);
						if (HangHoaKho5 != null)
						{
							Product_Detail newProduct_Detail5 = new Product_Detail
							{
								STT = lstProduct_Detail.Max((Product_Detail e) => e.STT),
								ID = Guid.NewGuid().ToString(),
								NAME = HangHoa5.NAME,
								MA = HangHoa5.MA,
								ID_HANGHOA = HangHoaKho5.ID_HANGHOA,
								ID_HANGHOAKHO = HangHoaKho5.ID,
								DONGIA = 0.0,
								ID_DVT = HangHoa5.ID_DVT,
								NAME_DVT = HangHoa5.NAME_DVT,
								SOLUONG = 0.0
							};
							dm_ThueSuat clsdm_ThueSuat9 = new dm_ThueSuat();
							if (!string.IsNullOrEmpty(HangHoa5.ID_THUESUAT))
							{
								clsdm_ThueSuat9 = (await _context.dm_ThueSuat.FirstOrDefaultAsync((dm_ThueSuat e) => e.LOC_ID == LOC_ID && e.ID == HangHoa5.ID_THUESUAT)) ?? new dm_ThueSuat();
							}
							newProduct_Detail5.TONGTIENGIAMGIA = -1.0 * Math.Ceiling(SOTIENTHUE_KM);
							newProduct_Detail5.TYPE = "TONGTIENGIAMGIA";
							API.TinhTong(newProduct_Detail5, "", lstProduct_Detail, clsdm_ThueSuat9);
							newProduct_Detail5.ISKHUYENMAI = true;
							newProduct_Detail5.GHICHU = "";
							lstProduct_Detail.Add(newProduct_Detail5);
						}
					}
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = from x in lstProduct_Detail
						   orderby x.STT, x.ISKHUYENMAI
						   select x
				});
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				return Ok(new ApiResponse
				{
					Success = false,
					Message = ex2.Message,
					Data = lstProduct_Detail
				});
			}
		}
	}
}
