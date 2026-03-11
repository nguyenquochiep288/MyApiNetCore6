using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseTHP;
using DatabaseTHP.Class;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyApiNetCore6.Data;

namespace MyApiNetCore6.Controllers
{

	[Route("api/[controller]")]
	[ApiController]
	public class ProductPriceRangeController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public ProductPriceRangeController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetProductPriceRange(string LOC_ID)
		{
			try
			{
				List<dm_HangHoa_KhungGia_Master> lstValue = await _context.dm_HangHoa_KhungGia_Master.Where((dm_HangHoa_KhungGia_Master e) => e.LOC_ID == LOC_ID).ToListAsync();
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
		public async Task<IActionResult> GetProductPriceRange(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<dm_HangHoa_KhungGia_Master> lstValue = await (from e in _context.dm_HangHoa_KhungGia_Master
																   where e.LOC_ID == LOC_ID && (e.MA.Contains(ValuesSearch) || e.NAME.Contains(ValuesSearch))
																   orderby e.MA
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

		[HttpPost("{LOC_ID}/{ID_HANGHOA}/{ID_DVT}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> PostProductPriceRange(string LOC_ID, string ID_HANGHOA, string ID_DVT, [FromBody] List<v_ChiTietHoaDon> lstChiTietHoaDon)
		{
			try
			{
				new List<dm_HangHoa_KhungGia>();
				view_dm_HangHoa_KhungGia_HangHoa view_dm_HangHoa_KhungGia_HangHoa2 = await _context.view_dm_HangHoa_KhungGia_HangHoa.FirstOrDefaultAsync((view_dm_HangHoa_KhungGia_HangHoa e) => e.LOC_ID == LOC_ID && e.ID_HANGHOA == ID_HANGHOA && e.ISACTIVE);
				if (view_dm_HangHoa_KhungGia_HangHoa2 != null)
				{
					List<view_dm_HangHoa_KhungGia_HangHoa> lstValue_HangHoa = await _context.view_dm_HangHoa_KhungGia_HangHoa.Where((view_dm_HangHoa_KhungGia_HangHoa e) => e.LOC_ID == LOC_ID && e.ID_HANGHOA_KHUNGGIA_MASTER == view_dm_HangHoa_KhungGia_HangHoa2.ID_HANGHOA_KHUNGGIA_MASTER && e.ISACTIVE).ToListAsync();
					if (lstValue_HangHoa != null)
					{
						List<v_ChiTietHoaDon> lstDonGia = lstChiTietHoaDon.Where((v_ChiTietHoaDon s) => lstValue_HangHoa.Select((view_dm_HangHoa_KhungGia_HangHoa view_dm_HangHoa_KhungGia_HangHoa3) => view_dm_HangHoa_KhungGia_HangHoa3.ID_HANGHOA).Contains(s.ID_HANGHOA)).ToList();
						double TongSoLuong = lstChiTietHoaDon.Where((v_ChiTietHoaDon s) => lstValue_HangHoa.Select((view_dm_HangHoa_KhungGia_HangHoa view_dm_HangHoa_KhungGia_HangHoa3) => view_dm_HangHoa_KhungGia_HangHoa3.ID_HANGHOA).Contains(s.ID_HANGHOA)).Sum((v_ChiTietHoaDon s) => s.SOLUONG);
						List<dm_HangHoa_KhungGia> lstValue = await _context.dm_HangHoa_KhungGia.Where((dm_HangHoa_KhungGia e) => e.LOC_ID == LOC_ID && e.ID_HANGHOA_KHUNGGIA_MASTER == view_dm_HangHoa_KhungGia_HangHoa2.ID_HANGHOA_KHUNGGIA_MASTER && e.ISACTIVE && e.ID_DVT == ID_DVT && e.TU <= TongSoLuong && e.DEN >= TongSoLuong).ToListAsync();
						if (lstValue != null && lstDonGia != null)
						{
							foreach (v_ChiTietHoaDon itm in lstDonGia)
							{
								itm.DONGIAMOI = ((lstValue.FirstOrDefault() != null) ? lstValue.FirstOrDefault().DONGIA : itm.DONGIA);
							}
						}
						else
						{
							lstChiTietHoaDon.Clear();
						}
					}
					else
					{
						lstChiTietHoaDon.Clear();
					}
				}
				else
				{
					lstChiTietHoaDon.Clear();
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = lstChiTietHoaDon
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
		public async Task<IActionResult> GetProductPriceRange(string LOC_ID, string ID)
		{
			try
			{
				dm_HangHoa_KhungGia_Master ProductPriceRange = await _context.dm_HangHoa_KhungGia_Master.FirstOrDefaultAsync((dm_HangHoa_KhungGia_Master e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (ProductPriceRange == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				List<dm_HangHoa_KhungGia> OKProductPriceRange = await _context.dm_HangHoa_KhungGia.Where((dm_HangHoa_KhungGia e) => e.LOC_ID == LOC_ID && e.ID_HANGHOA_KHUNGGIA_MASTER == ID).ToListAsync();
				List<view_dm_HangHoa_KhungGia_HangHoa> OKProductPriceRangeHangHoa = await _context.view_dm_HangHoa_KhungGia_HangHoa.Where((view_dm_HangHoa_KhungGia_HangHoa e) => e.LOC_ID == LOC_ID && e.ID_HANGHOA_KHUNGGIA_MASTER == ID).ToListAsync();
				v_dm_HangHoa_KhungGia_Master Mater = new v_dm_HangHoa_KhungGia_Master();
				Mater.LOC_ID = ProductPriceRange.LOC_ID;
				Mater.ID = ProductPriceRange.ID;
				Mater.MA = ProductPriceRange.MA;
				Mater.NAME = ProductPriceRange.NAME;
				Mater.ISACTIVE = ProductPriceRange.ISACTIVE;
				Mater.lstdm_HangHoa_KhungGia = new List<dm_HangHoa_KhungGia>();
				Mater.lstdm_HangHoa_KhungGia = OKProductPriceRange;
				Mater.lstdm_HangHoa_KhungGia_HangHoa = new List<v_dm_HangHoa_KhungGia_HangHoa>();
				foreach (view_dm_HangHoa_KhungGia_HangHoa itm in OKProductPriceRangeHangHoa)
				{
					v_dm_HangHoa_KhungGia_HangHoa hanghoa = new v_dm_HangHoa_KhungGia_HangHoa
					{
						ID = itm.ID,
						LOC_ID = itm.LOC_ID,
						ID_HANGHOA = itm.ID_HANGHOA,
						NAME = itm.NAME,
						MA = itm.MA,
						ID_HANGHOA_KHUNGGIA_MASTER = itm.ID_HANGHOA_KHUNGGIA_MASTER
					};
					Mater.lstdm_HangHoa_KhungGia_HangHoa.Add(hanghoa);
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = Mater
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
		public async Task<IActionResult> PutProductPriceRange(string LOC_ID, string ID, [FromBody] v_dm_HangHoa_KhungGia_Master ProductPriceRange)
		{
			try
			{
				List<dm_HangHoa_KhungGia> lstProductPriceRange = await _context.dm_HangHoa_KhungGia.Where((dm_HangHoa_KhungGia e) => e.LOC_ID == LOC_ID && e.ID_HANGHOA_KHUNGGIA_MASTER == ID).ToListAsync();
				if (lstProductPriceRange == null)
				{
					lstProductPriceRange = new List<dm_HangHoa_KhungGia>();
				}
				foreach (dm_HangHoa_KhungGia itm in ProductPriceRange.lstdm_HangHoa_KhungGia)
				{
					dm_HangHoa_KhungGia objProductPriceRange = lstProductPriceRange.Where((dm_HangHoa_KhungGia s) => s.ID == itm.ID).FirstOrDefault();
					if (objProductPriceRange != null)
					{
						itm.ID = objProductPriceRange.ID;
						_context.Entry(objProductPriceRange).CurrentValues.SetValues(itm);
						lstProductPriceRange.Remove(objProductPriceRange);
						continue;
					}
					dm_HangHoa_KhungGia newdm_HangHoa_KhungGia = new dm_HangHoa_KhungGia();
					newdm_HangHoa_KhungGia.LOC_ID = itm.LOC_ID;
					newdm_HangHoa_KhungGia.ID = itm.ID;
					newdm_HangHoa_KhungGia.TU = itm.TU;
					newdm_HangHoa_KhungGia.DEN = itm.DEN;
					newdm_HangHoa_KhungGia.ID_DVT = itm.ID_DVT;
					newdm_HangHoa_KhungGia.DONGIA = itm.DONGIA;
					newdm_HangHoa_KhungGia.ISACTIVE = itm.ISACTIVE;
					newdm_HangHoa_KhungGia.TIEN_KPI = itm.TIEN_KPI;
					newdm_HangHoa_KhungGia.HINHTHUC_TINHKPI = itm.HINHTHUC_TINHKPI;
					newdm_HangHoa_KhungGia.CK_KPI = itm.CK_KPI;
					newdm_HangHoa_KhungGia.TIEN_KPI = itm.TIEN_KPI;
                    newdm_HangHoa_KhungGia.ID_HANGHOA_KHUNGGIA_MASTER = itm.ID_HANGHOA_KHUNGGIA_MASTER;

                    _context.dm_HangHoa_KhungGia.Add(newdm_HangHoa_KhungGia);
				}
				foreach (dm_HangHoa_KhungGia itm2 in lstProductPriceRange)
				{
					_context.dm_HangHoa_KhungGia.Remove(itm2);
				}
				List<dm_HangHoa_KhungGia_HangHoa> lstProductPriceRangeHangHoa = await _context.dm_HangHoa_KhungGia_HangHoa.Where((dm_HangHoa_KhungGia_HangHoa e) => e.LOC_ID == LOC_ID && e.ID_HANGHOA_KHUNGGIA_MASTER == ID).ToListAsync();
				if (lstProductPriceRangeHangHoa == null)
				{
					lstProductPriceRangeHangHoa = new List<dm_HangHoa_KhungGia_HangHoa>();
				}
				foreach (v_dm_HangHoa_KhungGia_HangHoa itm3 in ProductPriceRange.lstdm_HangHoa_KhungGia_HangHoa)
				{
					dm_HangHoa_KhungGia_HangHoa objProductPriceRange2 = lstProductPriceRangeHangHoa.Where((dm_HangHoa_KhungGia_HangHoa s) => s.ID == itm3.ID).FirstOrDefault();
					if (objProductPriceRange2 != null)
					{
						itm3.ID = objProductPriceRange2.ID;
						_context.Entry(objProductPriceRange2).CurrentValues.SetValues(itm3);
						lstProductPriceRangeHangHoa.Remove(objProductPriceRange2);
						continue;
					}
					dm_HangHoa_KhungGia_HangHoa newdm_HangHoa_KhungGia2 = new dm_HangHoa_KhungGia_HangHoa
					{
						LOC_ID = itm3.LOC_ID,
						ID = itm3.ID,
						ID_HANGHOA = itm3.ID_HANGHOA,
						ID_HANGHOA_KHUNGGIA_MASTER = ID
					};
					_context.dm_HangHoa_KhungGia_HangHoa.Add(newdm_HangHoa_KhungGia2);
				}
				foreach (dm_HangHoa_KhungGia_HangHoa itm4 in lstProductPriceRangeHangHoa)
				{
					_context.dm_HangHoa_KhungGia_HangHoa.Remove(itm4);
				}
				_context.Entry(ProductPriceRange).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				List<dm_HangHoa_KhungGia> OKProductPriceRange = await _context.dm_HangHoa_KhungGia.Where((dm_HangHoa_KhungGia e) => e.LOC_ID == LOC_ID && e.ID_HANGHOA_KHUNGGIA_MASTER == ID).ToListAsync();
				List<view_dm_HangHoa_KhungGia_HangHoa> OKProductPriceRangeHangHoa = await _context.view_dm_HangHoa_KhungGia_HangHoa.Where((view_dm_HangHoa_KhungGia_HangHoa e) => e.LOC_ID == LOC_ID && e.ID_HANGHOA_KHUNGGIA_MASTER == ID).ToListAsync();
				v_dm_HangHoa_KhungGia_Master Mater = new v_dm_HangHoa_KhungGia_Master();
				Mater.LOC_ID = ProductPriceRange.LOC_ID;
				Mater.ID = ProductPriceRange.ID;
				Mater.MA = ProductPriceRange.MA;
				Mater.NAME = ProductPriceRange.NAME;
				Mater.ISACTIVE = ProductPriceRange.ISACTIVE;
				Mater.lstdm_HangHoa_KhungGia = new List<dm_HangHoa_KhungGia>();
				Mater.lstdm_HangHoa_KhungGia = OKProductPriceRange;
				Mater.lstdm_HangHoa_KhungGia_HangHoa = new List<v_dm_HangHoa_KhungGia_HangHoa>();
				foreach (view_dm_HangHoa_KhungGia_HangHoa itm5 in OKProductPriceRangeHangHoa)
				{
					v_dm_HangHoa_KhungGia_HangHoa hanghoa = new v_dm_HangHoa_KhungGia_HangHoa
					{
						ID = itm5.ID,
						LOC_ID = itm5.LOC_ID,
						ID_HANGHOA = itm5.ID_HANGHOA,
						NAME = itm5.NAME,
						MA = itm5.MA,
						ID_HANGHOA_KHUNGGIA_MASTER = itm5.ID_HANGHOA_KHUNGGIA_MASTER
					};
					Mater.lstdm_HangHoa_KhungGia_HangHoa.Add(hanghoa);
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = Mater
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
		public async Task<ActionResult<dm_KhuVuc>> PostProductPriceRange([FromBody] v_dm_HangHoa_KhungGia_Master ProductPriceRange)
		{
			try
			{
				if (ProductPriceRangeExistsMA(ProductPriceRange.LOC_ID, ProductPriceRange.MA))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + ProductPriceRange.LOC_ID + "-" + ProductPriceRange.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				_context.dm_HangHoa_KhungGia_Master.Add(ProductPriceRange);
				foreach (dm_HangHoa_KhungGia itm in ProductPriceRange.lstdm_HangHoa_KhungGia)
				{
					itm.ID_HANGHOA_KHUNGGIA_MASTER = ProductPriceRange.ID;
					_context.dm_HangHoa_KhungGia.Add(itm);
				}
				foreach (v_dm_HangHoa_KhungGia_HangHoa itm2 in ProductPriceRange.lstdm_HangHoa_KhungGia_HangHoa)
				{
					itm2.ID_HANGHOA_KHUNGGIA_MASTER = ProductPriceRange.ID;
					_context.dm_HangHoa_KhungGia_HangHoa.Add(itm2);
				}
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				List<dm_HangHoa_KhungGia> OKProductPriceRange = await _context.dm_HangHoa_KhungGia.Where((dm_HangHoa_KhungGia e) => e.LOC_ID == ProductPriceRange.LOC_ID && e.ID_HANGHOA_KHUNGGIA_MASTER == ProductPriceRange.ID).ToListAsync();
				List<view_dm_HangHoa_KhungGia_HangHoa> OKProductPriceRangeHangHoa = await _context.view_dm_HangHoa_KhungGia_HangHoa.Where((view_dm_HangHoa_KhungGia_HangHoa e) => e.LOC_ID == ProductPriceRange.LOC_ID && e.ID_HANGHOA_KHUNGGIA_MASTER == ProductPriceRange.ID).ToListAsync();
				v_dm_HangHoa_KhungGia_Master Mater = new v_dm_HangHoa_KhungGia_Master();
				Mater.LOC_ID = ProductPriceRange.LOC_ID;
				Mater.ID = ProductPriceRange.ID;
				Mater.MA = ProductPriceRange.MA;
				Mater.NAME = ProductPriceRange.NAME;
				Mater.ISACTIVE = ProductPriceRange.ISACTIVE;
				Mater.lstdm_HangHoa_KhungGia = new List<dm_HangHoa_KhungGia>();
				Mater.lstdm_HangHoa_KhungGia = OKProductPriceRange;
				Mater.lstdm_HangHoa_KhungGia_HangHoa = new List<v_dm_HangHoa_KhungGia_HangHoa>();
				foreach (view_dm_HangHoa_KhungGia_HangHoa itm3 in OKProductPriceRangeHangHoa)
				{
					v_dm_HangHoa_KhungGia_HangHoa hanghoa = new v_dm_HangHoa_KhungGia_HangHoa
					{
						ID = itm3.ID,
						LOC_ID = itm3.LOC_ID,
						ID_HANGHOA = itm3.ID_HANGHOA,
						NAME = itm3.NAME,
						MA = itm3.MA,
						ID_HANGHOA_KHUNGGIA_MASTER = itm3.ID_HANGHOA_KHUNGGIA_MASTER
					};
					Mater.lstdm_HangHoa_KhungGia_HangHoa.Add(hanghoa);
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = Mater
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
		public async Task<IActionResult> DeleteProductPriceRange(string LOC_ID, string ID)
		{
			try
			{
				dm_HangHoa_KhungGia_Master ProductPriceRange = await _context.dm_HangHoa_KhungGia_Master.FirstOrDefaultAsync((dm_HangHoa_KhungGia_Master e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (ProductPriceRange == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				List<dm_HangHoa_KhungGia> OKProductPriceRange = await _context.dm_HangHoa_KhungGia.Where((dm_HangHoa_KhungGia e) => e.LOC_ID == LOC_ID && e.ID_HANGHOA_KHUNGGIA_MASTER == ID).ToListAsync();
				List<dm_HangHoa_KhungGia_HangHoa> OKProductPriceRangeHangHoa = await _context.dm_HangHoa_KhungGia_HangHoa.Where((dm_HangHoa_KhungGia_HangHoa e) => e.LOC_ID == LOC_ID && e.ID_HANGHOA_KHUNGGIA_MASTER == ID).ToListAsync();
				foreach (dm_HangHoa_KhungGia itm in OKProductPriceRange)
				{
					_context.dm_HangHoa_KhungGia.Remove(itm);
				}
				foreach (dm_HangHoa_KhungGia_HangHoa itm2 in OKProductPriceRangeHangHoa)
				{
					_context.dm_HangHoa_KhungGia_HangHoa.Remove(itm2);
				}
				_context.dm_HangHoa_KhungGia_Master.Remove(ProductPriceRange);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
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

		private bool ProductPriceRangeExistsMA(string LOC_ID, string MA)
		{
			return _context.dm_HangHoa_KhungGia_Master.Any((dm_HangHoa_KhungGia_Master e) => e.LOC_ID == LOC_ID && e.MA == MA);
		}

		private bool ProductPriceRangeExistsID(string LOC_ID, string ID)
		{
			return _context.dm_HangHoa_KhungGia_Master.Any((dm_HangHoa_KhungGia_Master e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}
	}
}
