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

namespace MyApiNetCore6.Controllers
{

	[Route("api/[controller]")]
	[ApiController]
	public class ReceiptController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public ReceiptController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetReceipt(string LOC_ID)
		{
			try
			{
				List<ct_PhieuThu> lstValue = await _context.ct_PhieuThu.Where((ct_PhieuThu e) => e.LOC_ID == LOC_ID).ToListAsync();
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
		public async Task<IActionResult> GetReceipt(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				List<ct_PhieuThu> lstValue = await _context.ct_PhieuThu.Where((ct_PhieuThu e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).ToListAsync();
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
		public async Task<IActionResult> GetReceipt(string LOC_ID, string ID)
		{
			try
			{
				ct_PhieuThu Receipt = await _context.ct_PhieuThu.FirstOrDefaultAsync((ct_PhieuThu e) => e.LOC_ID == LOC_ID && e.ID == ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = Receipt
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
		public async Task<IActionResult> PutReceipt(string LOC_ID, string ID, [FromBody] v_ct_PhieuThu Receipt)
		{
			try
			{
				if (!ReceiptExistsID(Receipt.LOC_ID, Receipt.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + Receipt.LOC_ID + "-" + Receipt.ID + " dữ liệu!",
						Data = ""
					});
				}
				if (Receipt.ISCHUYENCONGNOCHONHANVIEN)
				{
					dm_LoaiPhieuThu LoaiPhieuThu = await _context.dm_LoaiPhieuThu.AsNoTracking().FirstOrDefaultAsync((dm_LoaiPhieuThu e) => e.LOC_ID == Receipt.LOC_ID && e.MA == API.PTGCNKHCNV);
					if (LoaiPhieuThu != null)
					{
						Receipt.ID_LOAIPHIEUTHU = LoaiPhieuThu.ID;
					}
				}
				ct_PhieuThu PhieuThuCheck = await _context.ct_PhieuThu.AsNoTracking().FirstOrDefaultAsync((ct_PhieuThu e) => e.LOC_ID == Receipt.LOC_ID && e.ID == Receipt.ID);
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				_context.Entry(Receipt).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
				using IDbContextTransaction transaction2 = _context.Database.BeginTransaction();
				if (PhieuThuCheck != null && PhieuThuCheck.CHUNGTUKEMTHEO != Receipt.CHUNGTUKEMTHEO)
				{
					if (PhieuThuCheck.CHUNGTUKEMTHEO != null && PhieuThuCheck.CHUNGTUKEMTHEO.StartsWith("PX-"))
					{
						ct_PhieuXuat PhieuXuat = await _context.ct_PhieuXuat.FirstOrDefaultAsync((ct_PhieuXuat e) => e.LOC_ID == PhieuThuCheck.LOC_ID && e.MAPHIEU == PhieuThuCheck.CHUNGTUKEMTHEO);
						if (PhieuXuat != null)
						{
							ct_PhieuGiaoHang_ChiTiet ct_PhieuGiaoHang_ChiTiet2 = await _context.ct_PhieuGiaoHang_ChiTiet.FirstOrDefaultAsync((ct_PhieuGiaoHang_ChiTiet e) => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUGIAOHANG == PhieuXuat.ID);
							if (ct_PhieuGiaoHang_ChiTiet2 != null)
							{
								ct_PhieuGiaoHang_ChiTiet2.SOTIENDATHU = await _context.ct_PhieuThu.Where((ct_PhieuThu e) => e.LOC_ID == PhieuThuCheck.LOC_ID && e.CHUNGTUKEMTHEO == PhieuThuCheck.CHUNGTUKEMTHEO).SumAsync((ct_PhieuThu e) => e.SOTIEN);
								_context.Entry(ct_PhieuGiaoHang_ChiTiet2).State = EntityState.Modified;
							}
						}
					}
					if (Receipt.CHUNGTUKEMTHEO != null && Receipt.CHUNGTUKEMTHEO.StartsWith("PX-"))
					{
						ct_PhieuXuat PhieuXuat2 = await _context.ct_PhieuXuat.FirstOrDefaultAsync((ct_PhieuXuat e) => e.LOC_ID == Receipt.LOC_ID && e.MAPHIEU == Receipt.CHUNGTUKEMTHEO);
						if (PhieuXuat2 != null)
						{
							ct_PhieuGiaoHang_ChiTiet ct_PhieuGiaoHang_ChiTiet3 = await _context.ct_PhieuGiaoHang_ChiTiet.FirstOrDefaultAsync((ct_PhieuGiaoHang_ChiTiet e) => e.LOC_ID == PhieuXuat2.LOC_ID && e.ID_PHIEUXUAT == PhieuXuat2.ID);
							if (ct_PhieuGiaoHang_ChiTiet3 != null)
							{
								ct_PhieuGiaoHang_ChiTiet3.SOTIENDATHU = await _context.ct_PhieuThu.Where((ct_PhieuThu e) => e.LOC_ID == Receipt.LOC_ID && e.CHUNGTUKEMTHEO == Receipt.CHUNGTUKEMTHEO).SumAsync((ct_PhieuThu e) => e.SOTIEN);
								ct_PhieuGiaoHang_ChiTiet3.ISCHUYENCONGNOCHONHANVIEN = Receipt.ISCHUYENCONGNOCHONHANVIEN;
								_context.Entry(ct_PhieuGiaoHang_ChiTiet3).State = EntityState.Modified;
							}
						}
					}
					AuditLogController auditLog2 = new AuditLogController(_context, _configuration);
					auditLog2.InserAuditLog();
					await _context.SaveChangesAsync();
				}
				transaction2.Commit();
				v_ct_PhieuThu PhieuThu = new v_ct_PhieuThu();
				SP_Parameter SP_Parameter = new SP_Parameter
				{
					ID_PHIEUTHU = Receipt.ID
				};
				ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
				if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuThu(SP_Parameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse })
				{
					List<v_ct_PhieuThu> lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuThu>;
					if (lstPhieuNhap != null && lstPhieuNhap.Count() > 0)
					{
						PhieuThu = lstPhieuNhap.FirstOrDefault() ?? new v_ct_PhieuThu();
					}
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = PhieuThu
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
		public async Task<ActionResult<ct_PhieuNhap>> PostReceipt([FromBody] v_ct_PhieuThu Receipt)
		{
			try
			{
				if (ReceiptExistsID(Receipt.LOC_ID, Receipt.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Receipt.LOC_ID + "-" + Receipt.ID + " trong dữ liệu!",
						Data = "",
						CheckValue = true
					});
				}
				if (Receipt.ISCHUYENCONGNOCHONHANVIEN)
				{
					dm_LoaiPhieuThu LoaiPhieuThu = await _context.dm_LoaiPhieuThu.AsNoTracking().FirstOrDefaultAsync((dm_LoaiPhieuThu e) => e.LOC_ID == Receipt.LOC_ID && e.MA == API.PTGCNKHCNV);
					if (LoaiPhieuThu != null)
					{
						Receipt.ID_LOAIPHIEUTHU = LoaiPhieuThu.ID;
					}
				}
				if (await _context.ct_PhieuThu.FirstOrDefaultAsync((ct_PhieuThu e) => e.LOC_ID == Receipt.LOC_ID && e.MAPHIEU == Receipt.MAPHIEU) != null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Receipt.LOC_ID + "-" + Receipt.MAPHIEU + " trong dữ liệu!",
						Data = "",
						CheckValue = true
					});
				}
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				_context.ct_PhieuThu.Add(Receipt);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
				using IDbContextTransaction transaction2 = _context.Database.BeginTransaction();
				if (Receipt.CHUNGTUKEMTHEO != null && Receipt.CHUNGTUKEMTHEO.StartsWith("PX-"))
				{
					ct_PhieuXuat PhieuXuat = await _context.ct_PhieuXuat.FirstOrDefaultAsync((ct_PhieuXuat e) => e.LOC_ID == Receipt.LOC_ID && e.MAPHIEU == Receipt.CHUNGTUKEMTHEO);
					if (PhieuXuat != null)
					{
						ct_PhieuGiaoHang_ChiTiet ct_PhieuGiaoHang_ChiTiet2 = await _context.ct_PhieuGiaoHang_ChiTiet.FirstOrDefaultAsync((ct_PhieuGiaoHang_ChiTiet e) => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUXUAT == PhieuXuat.ID);
						if (ct_PhieuGiaoHang_ChiTiet2 != null)
						{
							ct_PhieuGiaoHang_ChiTiet2.SOTIENDATHU = await _context.ct_PhieuThu.Where((ct_PhieuThu e) => e.LOC_ID == Receipt.LOC_ID && e.CHUNGTUKEMTHEO == Receipt.CHUNGTUKEMTHEO).SumAsync((ct_PhieuThu e) => e.SOTIEN);
							ct_PhieuGiaoHang_ChiTiet2.ISCHUYENCONGNOCHONHANVIEN = Receipt.ISCHUYENCONGNOCHONHANVIEN;
							_context.Entry(ct_PhieuGiaoHang_ChiTiet2).State = EntityState.Modified;
						}
					}
				}
				AuditLogController auditLog2 = new AuditLogController(_context, _configuration);
				auditLog2.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction2.Commit();
				List<ct_PhieuThu> lstPhieuDatHangCheck = await (from e in _context.ct_PhieuThu
																where e.LOC_ID == Receipt.LOC_ID && e.MAPHIEU == Receipt.MAPHIEU
																orderby e.NGAYLAP descending
																select e).ToListAsync();
				if (lstPhieuDatHangCheck != null && lstPhieuDatHangCheck.Count() > 1 && lstPhieuDatHangCheck.FirstOrDefault().ID == Receipt.ID)
				{
					int Max_ID = (from e in _context.ct_PhieuThu
								  where e.LOC_ID == Receipt.LOC_ID && e.NGAYLAP.Date == Receipt.NGAYLAP.Date
								  select e.SOPHIEU).DefaultIfEmpty().Max();
					Receipt.SOPHIEU = Max_ID + 1;
					Receipt.MAPHIEU = API.GetMaPhieu("Receipt", Receipt.NGAYLAP, Receipt.SOPHIEU);
					_context.Entry(Receipt).State = EntityState.Modified;
					await _context.SaveChangesAsync();
				}
				v_ct_PhieuThu PhieuThu = new v_ct_PhieuThu();
				SP_Parameter SP_Parameter = new SP_Parameter
				{
					ID_PHIEUTHU = Receipt.ID
				};
				ExecuteStoredProc ExecuteStoredProc1 = new ExecuteStoredProc(_context, _configuration);
				if (await ExecuteStoredProc1.Sp_Get_DanhSachPhieuThu(SP_Parameter) is OkObjectResult { Value: ApiResponse { Data: not null } ApiResponse })
				{
					List<v_ct_PhieuThu> lstPhieuNhap = ApiResponse.Data as List<v_ct_PhieuThu>;
					if (lstPhieuNhap != null && lstPhieuNhap.Count() > 0)
					{
						PhieuThu = lstPhieuNhap.FirstOrDefault() ?? new v_ct_PhieuThu();
					}
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = PhieuThu
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
		public async Task<IActionResult> DeleteReceipt(string LOC_ID, string ID)
		{
			try
			{
				ct_PhieuThu Receipt = await _context.ct_PhieuThu.FirstOrDefaultAsync((ct_PhieuThu e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Receipt == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				if (Receipt.CHUNGTUKEMTHEO != null && Receipt.CHUNGTUKEMTHEO.StartsWith("PX-"))
				{
					ct_PhieuXuat PhieuXuat = await _context.ct_PhieuXuat.FirstOrDefaultAsync((ct_PhieuXuat e) => e.LOC_ID == Receipt.LOC_ID && e.MAPHIEU == Receipt.CHUNGTUKEMTHEO);
					if (PhieuXuat != null)
					{
						ct_PhieuGiaoHang_ChiTiet ct_PhieuGiaoHang_ChiTiet2 = await _context.ct_PhieuGiaoHang_ChiTiet.FirstOrDefaultAsync((ct_PhieuGiaoHang_ChiTiet e) => e.LOC_ID == PhieuXuat.LOC_ID && e.ID_PHIEUXUAT == PhieuXuat.ID);
						if (ct_PhieuGiaoHang_ChiTiet2 != null)
						{
							ct_PhieuGiaoHang_ChiTiet2.SOTIENDATHU = await _context.ct_PhieuThu.Where((ct_PhieuThu e) => e.LOC_ID == Receipt.LOC_ID && e.CHUNGTUKEMTHEO == Receipt.CHUNGTUKEMTHEO).SumAsync((ct_PhieuThu e) => e.SOTIEN) - Receipt.SOTIEN;
							if (await _context.ct_PhieuThu.FirstOrDefaultAsync((ct_PhieuThu e) => e.LOC_ID == LOC_ID && e.MAPHIEU == Receipt.CHUNGTUKEMTHEO && e.ISCHUYENCONGNOCHONHANVIEN) == null)
							{
								ct_PhieuGiaoHang_ChiTiet2.ISCHUYENCONGNOCHONHANVIEN = false;
							}
							_context.Entry(ct_PhieuGiaoHang_ChiTiet2).State = EntityState.Modified;
						}
					}
				}
				_context.ct_PhieuThu.Remove(Receipt);
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

		private bool ReceiptExistsID(string LOC_ID, string ID)
		{
			return _context.ct_PhieuThu.Any((ct_PhieuThu e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}
	}
}
