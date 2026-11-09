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
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using MyApiNetCore6.Data;

namespace MyApiNetCore6.Controllers
{

	[Route("api/[controller]")]
	[ApiController]
	public class OutputController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		private string strTable = "ct_PhieuXuat";

		public OutputController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetOutput(string LOC_ID)
		{
			try
			{
				List<ct_PhieuXuat> lstValue = await _context.ct_PhieuXuat.Where((ct_PhieuXuat e) => e.LOC_ID == LOC_ID).ToListAsync();
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
		public async Task<IActionResult> GetOutput(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				List<ct_PhieuXuat> lstValue = await _context.ct_PhieuXuat.Where((ct_PhieuXuat e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).ToListAsync();
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
		public async Task<IActionResult> GetOutput(string LOC_ID, string ID_KHO, DateTime FROMDATE, DateTime TODATE, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				List<ct_PhieuXuat> lstValue = await _context.ct_PhieuXuat.Where((ct_PhieuXuat e) => e.LOC_ID == LOC_ID && e.NGAYLAP.Date >= ((DateTime)FROMDATE).Date && e.NGAYLAP.Date <= ((DateTime)TODATE).Date).Where(KeyWhere, ValuesSearch).ToListAsync();
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
		public async Task<IActionResult> GetOutput(string LOC_ID, string ID)
		{
			try
			{
				if (await _context.ct_PhieuXuat.FirstOrDefaultAsync((ct_PhieuXuat e) => e.LOC_ID == LOC_ID && e.ID == ID) == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				v_ct_PhieuXuat ct_PhieuXuat2 = new v_ct_PhieuXuat();
				SP_Parameter SP_Parameter = new SP_Parameter
				{
					LOC_ID = LOC_ID,
					ID_PHIEUXUAT = ID
				};
				ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
				if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuXuat(SP_Parameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse })
				{
					ct_PhieuXuat2 = (ApiResponse.Data as List<v_ct_PhieuXuat>)?.FirstOrDefault() ?? new v_ct_PhieuXuat();
				}
				SP_Parameter = new SP_Parameter
				{
					ID_PHIEUXUAT = ID
				};
				ct_PhieuXuat2.lstct_PhieuXuat_ChiTiet = new List<v_ct_PhieuXuat_ChiTiet>();
				ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
				if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuXuat_Chitiet(SP_Parameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse2 })
				{
					List<v_ct_PhieuXuat_ChiTiet> lst_ChiTiet = ApiResponse2.Data as List<v_ct_PhieuXuat_ChiTiet>;
					if (lst_ChiTiet != null)
					{
						ct_PhieuXuat2.lstct_PhieuXuat_ChiTiet.AddRange(lst_ChiTiet);
					}
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ct_PhieuXuat2
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
				string StrHetSoLuong = "";
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				List<ct_PhieuXuat_ChiTiet> lstPhieuNhap_ChiTiet = await _context.ct_PhieuXuat_ChiTiet.Where((ct_PhieuXuat_ChiTiet e) => e.LOC_ID == Output.LOC_ID && e.ID_PHIEUXUAT == Output.ID).ToListAsync();
				if (lstPhieuNhap_ChiTiet != null)
				{
					foreach (ct_PhieuXuat_ChiTiet itm in lstPhieuNhap_ChiTiet)
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
							objdm_HangHoa_Kho.QTY += itm.TONGSOLUONG;
							_context.Entry(objdm_HangHoa_Kho).State = EntityState.Modified;
							v_ct_PhieuXuat_ChiTiet chkPhieuNhap_ChiTiet = Output.lstct_PhieuXuat_ChiTiet.Where((v_ct_PhieuXuat_ChiTiet e) => e.ID == itm.ID).FirstOrDefault();
							if (chkPhieuNhap_ChiTiet != null)
							{
								chkPhieuNhap_ChiTiet.ISEDIT = true;
								chkPhieuNhap_ChiTiet.ID_PHIEUXUAT = Output.ID;
								_context.Entry(objdm_HangHoa_Kho).State = EntityState.Modified;
							}
							else
							{
								_context.ct_PhieuXuat_ChiTiet.Remove(itm);
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
				if (Output.lstct_PhieuXuat_ChiTiet != null)
				{
					foreach (v_ct_PhieuXuat_ChiTiet itm2 in Output.lstct_PhieuXuat_ChiTiet)
					{
						if (!string.IsNullOrEmpty(itm2.ID_THUESUAT))
						{
							dm_ThueSuat objVAT2 = _context.dm_ThueSuat.FirstOrDefault((dm_ThueSuat e) => e.LOC_ID == itm2.LOC_ID && e.ID == itm2.ID_HANGHOAKHO && e.ID == itm2.ID_THUESUAT);
							itm2.THUESUAT = objVAT2?.THUESUAT ?? itm2.THUESUAT;
						}
						itm2.THANHTIEN = itm2.SOLUONG * itm2.DONGIA - itm2.TONGTIENGIAMGIA;
						itm2.TONGCONG = itm2.THANHTIEN + itm2.TONGTIENVAT;
						itm2.ID_PHIEUXUAT = Output.ID;
						dm_HangHoa_Kho objdm_HangHoa_Kho2 = _context.dm_HangHoa_Kho.FromSqlRaw("\r\n                                        SELECT *\r\n                                        FROM dm_HangHoa_Kho WITH (UPDLOCK, ROWLOCK)\r\n                                        WHERE LOC_ID = @loc\r\n                                          AND ID = @id\r\n                                          AND ID_KHO = @kho\r\n                                    ", new SqlParameter("@loc", itm2.LOC_ID), new SqlParameter("@id", itm2.ID_HANGHOAKHO), new SqlParameter("@kho", Output.ID_KHO)).AsTracking().FirstOrDefault();
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
								_context.ct_PhieuXuat_ChiTiet.Add(itm2);
							}
							else
							{
								var stored = lstPhieuNhap_ChiTiet.Single(x => x.ID == itm2.ID);
								itm2.GIAVON = stored.GIAVON;
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
					Output.TONGTHANHTIEN = Math.Round(Output.lstct_PhieuXuat_ChiTiet.Sum((v_ct_PhieuXuat_ChiTiet s) => s.THANHTIEN), 0);
					Output.TONGTIENGIAMGIA = Math.Round(Output.lstct_PhieuXuat_ChiTiet.Sum((v_ct_PhieuXuat_ChiTiet s) => s.TONGTIENGIAMGIA), 0);
					Output.TONGTIENVAT = Math.Round(Output.lstct_PhieuXuat_ChiTiet.Sum((v_ct_PhieuXuat_ChiTiet s) => s.TONGTIENVAT), 0);
					Output.TONGTIEN = Math.Round(Output.lstct_PhieuXuat_ChiTiet.Sum((v_ct_PhieuXuat_ChiTiet s) => s.TONGCONG), 0);
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
				_context.Entry(Output).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await MyApiNetCore6.Services.StockCostPosting.SaveMovements(_context);
				transaction.Commit();
				v_ct_PhieuXuat ct_PhieuXuat2 = new v_ct_PhieuXuat
				{
					lstct_PhieuXuat_ChiTiet = new List<v_ct_PhieuXuat_ChiTiet>()
				};
				SP_Parameter SP_Parameter = new SP_Parameter
				{
					ID_PHIEUXUAT = Output.ID
				};
				ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
				if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuXuat(SP_Parameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse })
				{
					List<v_ct_PhieuXuat> lstPhieuDatHang = ApiResponse.Data as List<v_ct_PhieuXuat>;
					if (lstPhieuDatHang != null && lstPhieuDatHang.Count() > 0)
					{
						ct_PhieuXuat2 = lstPhieuDatHang.FirstOrDefault() ?? new v_ct_PhieuXuat();
					}
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ct_PhieuXuat2
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
				if (await _context.ct_PhieuXuat.FirstOrDefaultAsync((ct_PhieuXuat e) => e.LOC_ID == Output.LOC_ID && e.MAPHIEU == Output.MAPHIEU) != null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Output.LOC_ID + "-" + Output.MAPHIEU + " trong dữ liệu!",
						Data = "",
						CheckValue = true
					});
				}
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				if (Output.lstct_PhieuXuat_ChiTiet != null)
				{
					string StrHetSoLuong = "";
					foreach (v_ct_PhieuXuat_ChiTiet itm in Output.lstct_PhieuXuat_ChiTiet)
					{
						if (!string.IsNullOrEmpty(itm.ID_THUESUAT))
						{
							dm_ThueSuat objVAT = _context.dm_ThueSuat.FirstOrDefault((dm_ThueSuat e) => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO && e.ID == itm.ID_THUESUAT);
							itm.THUESUAT = objVAT?.THUESUAT ?? itm.THUESUAT;
						}
						itm.THANHTIEN = itm.SOLUONG * itm.DONGIA - itm.TONGTIENGIAMGIA;
						itm.TONGCONG = itm.THANHTIEN + itm.TONGTIENVAT;
						dm_HangHoa_Kho objdm_HangHoa_Kho = await _context.dm_HangHoa_Kho.FirstOrDefaultAsync((dm_HangHoa_Kho e) => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOAKHO && e.ID_KHO == Output.ID_KHO);
						if (objdm_HangHoa_Kho != null)
						{
							view_dm_HangHoa objdm_HangHoa = _context.view_dm_HangHoa.FirstOrDefault((view_dm_HangHoa e) => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID_HANGHOA);
							itm.TONGSOLUONG = itm.SOLUONG * itm.TYLE_QD;
							if (objdm_HangHoa_Kho.QTY >= itm.TONGSOLUONG)
							{
								objdm_HangHoa_Kho.QTY -= itm.TONGSOLUONG;
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
							if (await _context.ct_PhieuXuat_ChiTiet.FirstOrDefaultAsync((ct_PhieuXuat_ChiTiet e) => e.LOC_ID == itm.LOC_ID && e.ID == itm.ID) != null)
							{
								itm.ID = Guid.NewGuid().ToString();
							}
							itm.LOC_ID = Output.LOC_ID;
							itm.ID_PHIEUXUAT = Output.ID;
							_context.ct_PhieuXuat_ChiTiet.Add(itm);
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
					Output.TONGTHANHTIEN = Math.Round(Output.lstct_PhieuXuat_ChiTiet.Sum((v_ct_PhieuXuat_ChiTiet s) => s.THANHTIEN), 0);
					Output.TONGTIENGIAMGIA = Math.Round(Output.lstct_PhieuXuat_ChiTiet.Sum((v_ct_PhieuXuat_ChiTiet s) => s.TONGTIENGIAMGIA), 0);
					Output.TONGTIENVAT = Math.Round(Output.lstct_PhieuXuat_ChiTiet.Sum((v_ct_PhieuXuat_ChiTiet s) => s.TONGTIENVAT), 0);
					Output.TONGTIEN = Math.Round(Output.lstct_PhieuXuat_ChiTiet.Sum((v_ct_PhieuXuat_ChiTiet s) => s.TONGCONG), 0);
				}
				bool bolCheckMA = false;
				while (!bolCheckMA)
				{
					ct_PhieuXuat check = _context.ct_PhieuXuat.Where((ct_PhieuXuat e) => e.LOC_ID == Output.LOC_ID && e.MAPHIEU == Output.MAPHIEU).FirstOrDefault();
					if (check != null)
					{
						Output.MAPHIEU = API.GetMaPhieu("Output", Output.NGAYLAP, Output.SOPHIEU);
					}
					else
					{
						bolCheckMA = true;
					}
				}
				_context.ct_PhieuXuat.Add(Output);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await MyApiNetCore6.Services.StockCostPosting.SaveMovements(_context);
				transaction.Commit();
				List<ct_PhieuXuat> lstPhieuDatHangCheck = await (from e in _context.ct_PhieuXuat
																 where e.LOC_ID == Output.LOC_ID && e.MAPHIEU == Output.MAPHIEU
																 orderby e.NGAYLAP descending
																 select e).ToListAsync();
				if (lstPhieuDatHangCheck != null && lstPhieuDatHangCheck.Count() > 1 && lstPhieuDatHangCheck.FirstOrDefault().ID == Output.ID)
				{
					int Max_ID = (from e in _context.ct_PhieuXuat
								  where e.LOC_ID == Output.LOC_ID && e.NGAYLAP.Date == Output.NGAYLAP.Date
								  select e.SOPHIEU).DefaultIfEmpty().Max();
					Output.SOPHIEU = Max_ID + 1;
					Output.MAPHIEU = API.GetMaPhieu("Output", Output.NGAYLAP, Output.SOPHIEU);
					_context.Entry(Output).State = EntityState.Modified;
					await _context.SaveChangesAsync();
				}
				v_ct_PhieuXuat ct_PhieuXuat2 = new v_ct_PhieuXuat
				{
					lstct_PhieuXuat_ChiTiet = new List<v_ct_PhieuXuat_ChiTiet>()
				};
				SP_Parameter SP_Parameter = new SP_Parameter
				{
					ID_PHIEUXUAT = Output.ID
				};
				ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
				if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuXuat(SP_Parameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse })
				{
					List<v_ct_PhieuXuat> lstPhieuDatHang = ApiResponse.Data as List<v_ct_PhieuXuat>;
					if (lstPhieuDatHang != null && lstPhieuDatHang.Count() > 0)
					{
						ct_PhieuXuat2 = lstPhieuDatHang.FirstOrDefault() ?? new v_ct_PhieuXuat();
					}
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = ct_PhieuXuat2
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

		[HttpDelete("{LOC_ID}/{ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> DeleteOutput(string LOC_ID, string ID)
		{
			try
			{
				ct_PhieuXuat Output = await _context.ct_PhieuXuat.FirstOrDefaultAsync((ct_PhieuXuat e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Output == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				List<ct_PhieuGiaoHang_ChiTiet> lstPhieuGiaoHang_ChiTiet = await _context.ct_PhieuGiaoHang_ChiTiet.Where((ct_PhieuGiaoHang_ChiTiet e) => e.LOC_ID == Output.LOC_ID && e.ID_PHIEUXUAT == Output.ID).ToListAsync();
				if (lstPhieuGiaoHang_ChiTiet != null && lstPhieuGiaoHang_ChiTiet.Count() > 0)
				{
					string ChungTu = "(" + string.Join(";", lstPhieuGiaoHang_ChiTiet.Select((ct_PhieuGiaoHang_ChiTiet e) => e.ID_PHIEUGIAOHANG)) + ")";
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Có 'Phiếu Giao Hàng " + ChungTu + "' liên quan tới " + Output.MAPHIEU + "!",
						Data = ""
					});
				}
				List<ct_PhieuChi> lstPhieuChi = await _context.ct_PhieuChi.Where((ct_PhieuChi e) => e.LOC_ID == Output.LOC_ID && e.CHUNGTUKEMTHEO == Output.MAPHIEU).ToListAsync();
				if (lstPhieuChi != null && lstPhieuChi.Count() > 0)
				{
					string ChungTu2 = "(" + string.Join(";", lstPhieuChi.Select((ct_PhieuChi e) => e.MAPHIEU)) + ")";
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Có 'Phiếu Chi " + ChungTu2 + "' liên quan tới " + Output.MAPHIEU + "!",
						Data = ""
					});
				}
				List<ct_PhieuThu> lstPhieuThu = await _context.ct_PhieuThu.Where((ct_PhieuThu e) => e.LOC_ID == Output.LOC_ID && e.CHUNGTUKEMTHEO == Output.MAPHIEU).ToListAsync();
				if (lstPhieuThu != null && lstPhieuThu.Count() > 0)
				{
					string ChungTu3 = "(" + string.Join(";", lstPhieuThu.Select((ct_PhieuThu e) => e.MAPHIEU)) + ")";
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Có 'Phiếu Thu " + ChungTu3 + "' liên quan tới " + Output.MAPHIEU + "!",
						Data = ""
					});
				}
				List<ct_PhieuNhap> lstPhieuNhap = await _context.ct_PhieuNhap.Where((ct_PhieuNhap e) => e.LOC_ID == Output.LOC_ID && e.CHUNGTUKEMTHEO == Output.MAPHIEU).ToListAsync();
				if (lstPhieuNhap != null && lstPhieuNhap.Count() > 0)
				{
					string ChungTu4 = "(" + string.Join(";", lstPhieuNhap.Select((ct_PhieuNhap e) => e.MAPHIEU)) + ")";
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Có 'Phiếu Nhập " + ChungTu4 + "' liên quan tới " + Output.MAPHIEU + "!",
						Data = ""
					});
				}
				List<ct_PhieuXuat_ChiTiet> lstPhieuNhap_ChiTiet = await _context.ct_PhieuXuat_ChiTiet.Where((ct_PhieuXuat_ChiTiet e) => e.LOC_ID == Output.LOC_ID && e.ID_PHIEUXUAT == Output.ID).ToListAsync();
				if (lstPhieuNhap_ChiTiet != null)
				{
					foreach (ct_PhieuXuat_ChiTiet itm in lstPhieuNhap_ChiTiet)
					{
						if (!Output.ISPHIEUDIEUHANG)
						{
							dm_HangHoa_Kho objdm_HangHoa_Kho = _context.dm_HangHoa_Kho.FromSqlRaw("\r\n                                        SELECT *\r\n                                        FROM dm_HangHoa_Kho WITH (UPDLOCK, ROWLOCK)\r\n                                        WHERE LOC_ID = @loc\r\n                                          AND ID = @id\r\n                                    ", new SqlParameter("@loc", itm.LOC_ID), new SqlParameter("@id", itm.ID_HANGHOAKHO)).AsTracking().FirstOrDefault();
							if (objdm_HangHoa_Kho == null)
							{
								return Ok(new ApiResponse
								{
									Success = false,
									Message = "Không tìm thấy sản phẩm kho!" + itm.ID_HANGHOAKHO,
									Data = ""
								});
							}
							itm.TONGSOLUONG = itm.TYLE_QD * itm.SOLUONG;
							objdm_HangHoa_Kho.QTY += itm.TONGSOLUONG;
							_context.Entry(objdm_HangHoa_Kho).State = EntityState.Modified;
						}
						_context.ct_PhieuXuat_ChiTiet.Remove(itm);
					}
				}
				foreach (ct_PhieuDatHang itm2 in await _context.ct_PhieuDatHang.Where((ct_PhieuDatHang e) => e.LOC_ID == Output.LOC_ID && e.ID_PHIEUXUAT == Output.ID).ToListAsync())
				{
					itm2.ID_PHIEUXUAT = "";
				}
				_context.ct_PhieuXuat.Remove(Output);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await MyApiNetCore6.Services.StockCostPosting.SaveMovements(_context);
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

		private bool OutputExistsID(string LOC_ID, string ID)
		{
			return _context.ct_PhieuXuat.Any((ct_PhieuXuat e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}
	}
}
