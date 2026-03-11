using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using DatabaseTHP;
using DatabaseTHP.Class;
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
	public class ProductController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public ProductController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetProduct(string LOC_ID)
		{
			try
			{
				List<view_dm_HangHoa> lstValue = await (from e in _context.view_dm_HangHoa
														where e.LOC_ID == LOC_ID
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

		[HttpGet("{LOC_ID}/{Type}/{KeyWhere}/{ValuesSearch}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetProduct(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<view_dm_HangHoa> lstValue = await (from e in _context.view_dm_HangHoa.Where((view_dm_HangHoa e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch)
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

		[HttpGet("{LOC_ID}/{ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetProduct(string LOC_ID, string ID)
		{
			try
			{
				view_dm_HangHoa Product = await _context.view_dm_HangHoa.FirstOrDefaultAsync((view_dm_HangHoa e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Product == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.dm_HangHoa_Kho.Any((dm_HangHoa_Kho e) => e.LOC_ID == LOC_ID && e.ID == ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = Product
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

		[HttpPut("{LOC_ID}/{MA}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> PutProduct(string LOC_ID, string MA, [FromBody] v_dm_HangHoa Product)
		{
			try
			{
				if (ProductExists(Product))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Product.LOC_ID + "-" + Product.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				if (LOC_ID != Product.LOC_ID || Product.MA != MA)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (!ProductExistsID(Product.LOC_ID, Product.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + Product.LOC_ID + "-" + Product.ID + " dữ liệu!",
						Data = ""
					});
				}
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				if (!Product.BAOGOMTHUESUAT)
				{
					Product.ID_THUESUAT = null;
				}
				if (Product.LOAIHANGHOA == 0.ToString())
				{
					List<dm_HangHoa_Combo> lstdm_HangHoa_Combo = await _context.dm_HangHoa_Combo.Where((dm_HangHoa_Combo dm_HangHoa_Combo2) => dm_HangHoa_Combo2.LOC_ID == Product.LOC_ID && dm_HangHoa_Combo2.ID_HANGHOA == Product.ID).ToListAsync();
					if (lstdm_HangHoa_Combo != null)
					{
						foreach (dm_HangHoa_Combo itm in lstdm_HangHoa_Combo)
						{
							itm.TYLE_QD = Product.TYLE_QD;
							itm.QTY_TOTAL = itm.TYLE_QD * itm.QTY;
							_context.Entry(itm).State = EntityState.Modified;
						}
					}
				}
				else if (Product.LOAIHANGHOA == 2.ToString())
				{
					Product.ISCOMBO = false;
					Product.STATUS_QD = false;
					Product.TYLE_QD = 0.0;
				}
				else if (Product.LOAIHANGHOA == 1.ToString())
				{
					Product.ISCOMBO = false;
					Product.STATUS_QD = false;
					Product.TYLE_QD = 1.0;
					List<dm_HangHoa_Combo> lstdm_HangHoa_Combo2 = await _context.dm_HangHoa_Combo.Where((dm_HangHoa_Combo dm_HangHoa_Combo2) => dm_HangHoa_Combo2.LOC_ID == Product.LOC_ID && dm_HangHoa_Combo2.ID_HANGHOACOMBO == Product.ID).ToListAsync();
					if (lstdm_HangHoa_Combo2 != null)
					{
						foreach (dm_HangHoa_Combo itm2 in lstdm_HangHoa_Combo2)
						{
							v_dm_HangHoa_Combo checkHangHoa_Combo = Product.lstdm_HangHoa_Combo.Where((v_dm_HangHoa_Combo v_dm_HangHoa_Combo2) => v_dm_HangHoa_Combo2.ID_HANGHOA == itm2.ID_HANGHOA && v_dm_HangHoa_Combo2.ID_DVT == itm2.ID_DVT).FirstOrDefault();
							if (checkHangHoa_Combo != null)
							{
								checkHangHoa_Combo.ISEDIT = true;
								itm2.QTY = checkHangHoa_Combo.QTY;
								itm2.TYLE_QD = checkHangHoa_Combo.TYLE_QD;
								itm2.QTY_TOTAL = itm2.QTY * itm2.TYLE_QD;
								itm2.ID_NGUOISUA = checkHangHoa_Combo.ID_NGUOITAO;
								itm2.THOIGIANSUA = checkHangHoa_Combo.THOIGIANTHEM;
							}
							else
							{
								_context.dm_HangHoa_Combo.Remove(itm2);
							}
						}
					}
					if (Product.lstdm_HangHoa_Combo != null)
					{
						foreach (v_dm_HangHoa_Combo itm3 in Product.lstdm_HangHoa_Combo.Where((v_dm_HangHoa_Combo v_dm_HangHoa_Combo2) => !v_dm_HangHoa_Combo2.ISEDIT))
						{
							itm3.ID = Guid.NewGuid().ToString();
							itm3.LOC_ID = Product.LOC_ID;
							itm3.ID_HANGHOACOMBO = Product.ID;
							itm3.ISACTIVE = true;
							itm3.QTY_TOTAL = itm3.QTY * itm3.TYLE_QD;
							_context.dm_HangHoa_Combo.Add(itm3);
						}
					}
				}
				_context.Entry(Product).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
				view_dm_HangHoa OKProduct = await _context.view_dm_HangHoa.FirstOrDefaultAsync((view_dm_HangHoa view_dm_HangHoa2) => view_dm_HangHoa2.LOC_ID == Product.LOC_ID && view_dm_HangHoa2.ID == Product.ID);
				if (!string.IsNullOrEmpty(Product.PICTURE) && Product.FILENEW)
				{
					string path = "C:\\FTP\\Images_Upload\\Product";
					if (Product.FILENEW)
					{
						try
						{
							if (!Directory.Exists(path))
							{
								Directory.CreateDirectory(path);
							}
							if (System.IO.File.Exists(path + "\\" + Product.PICTURE))
							{
								System.IO.File.Delete(path + "\\" + Product.PICTURE);
							}
							System.IO.File.WriteAllBytes(bytes: Convert.FromBase64String(Product.FILEBASE64), path: path + "\\" + Product.PICTURE);
						}
						catch (Exception ex)
						{
							Exception e = ex;
							return Ok(new ApiResponse
							{
								Success = false,
								Message = e.Message,
								Data = ""
							});
						}
					}
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKProduct
				});
			}
			catch (DbUpdateConcurrencyException ex2)
			{
				DbUpdateConcurrencyException ex3 = ex2;
				return Ok(new ApiResponse
				{
					Success = false,
					Message = ex3.Message,
					Data = ""
				});
			}
		}

		[HttpPost]
		[Authorize(Roles = "User")]
		public async Task<ActionResult<v_dm_HangHoa>> PostProduct([FromBody] v_dm_HangHoa Product)
		{
			try
			{
				if (ProductExistsMA(Product.LOC_ID, Product.MA))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Product.LOC_ID + "-" + Product.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				if (!string.IsNullOrEmpty(Product.PICTURE) && !string.IsNullOrEmpty(Product.FILEBASE64))
				{
					try
					{
						string path = "C:\\FTP\\Images_Upload\\Product";
						if (!Directory.Exists(path))
						{
							Directory.CreateDirectory(path);
						}
						System.IO.File.WriteAllBytes(bytes: Convert.FromBase64String(Product.FILEBASE64), path: path + "\\" + Product.PICTURE);
					}
					catch (Exception ex)
					{
						Exception e = ex;
						return Ok(new ApiResponse
						{
							Success = false,
							Message = e.Message,
							Data = ""
						});
					}
				}
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				if (!Product.BAOGOMTHUESUAT)
				{
					Product.ID_THUESUAT = null;
				}
				if (!(Product.LOAIHANGHOA == 0.ToString()))
				{
					if (Product.LOAIHANGHOA == 0.ToString())
					{
						Product.ISCOMBO = false;
						Product.STATUS_QD = false;
						Product.TYLE_QD = 0.0;
					}
					else if (Product.LOAIHANGHOA == 1.ToString())
					{
						Product.ISCOMBO = false;
						Product.STATUS_QD = false;
						Product.TYLE_QD = 1.0;
						if (Product.lstdm_HangHoa_Combo != null)
						{
							foreach (v_dm_HangHoa_Combo itm in Product.lstdm_HangHoa_Combo)
							{
								itm.ID = Guid.NewGuid().ToString();
								itm.LOC_ID = Product.LOC_ID;
								itm.ID_HANGHOACOMBO = Product.ID;
								itm.ISACTIVE = true;
								itm.QTY_TOTAL = itm.QTY * itm.TYLE_QD;
								_context.dm_HangHoa_Combo.Add(itm);
							}
						}
					}
				}
				_context.dm_HangHoa.Add(Product);
				List<dm_Kho> lstKho = await _context.dm_Kho.Where((dm_Kho dm_Kho2) => dm_Kho2.LOC_ID == Product.LOC_ID).ToListAsync();
				if (lstKho != null)
				{
					foreach (dm_Kho itm2 in lstKho)
					{
						dm_HangHoa_Kho dm_HangHoa_Kho2 = new dm_HangHoa_Kho
						{
							ID = Guid.NewGuid().ToString(),
							LOC_ID = Product.LOC_ID,
							ID_KHO = itm2.ID,
							ID_HANGHOA = Product.ID,
							QTY = 0.0
						};
						_context.dm_HangHoa_Kho.Add(dm_HangHoa_Kho2);
					}
				}
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				transaction.Commit();
				view_dm_HangHoa OKProduct = await _context.view_dm_HangHoa.FirstOrDefaultAsync((view_dm_HangHoa view_dm_HangHoa2) => view_dm_HangHoa2.LOC_ID == Product.LOC_ID && view_dm_HangHoa2.ID == Product.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKProduct
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
		public async Task<IActionResult> DeleteProduct(string LOC_ID, string ID)
		{
			try
			{
				dm_HangHoa Product = await _context.dm_HangHoa.FirstOrDefaultAsync((dm_HangHoa e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Product == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
				List<dm_HangHoa_Kho> lstdm_HangHoa_Kho = await _context.dm_HangHoa_Kho.Where((dm_HangHoa_Kho e) => e.LOC_ID == LOC_ID && e.ID_HANGHOA == ID).ToListAsync();
				new ApiResponse();
				ApiResponse apiResponse;
				if (lstdm_HangHoa_Kho != null)
				{
					foreach (dm_HangHoa_Kho itm in lstdm_HangHoa_Kho)
					{
						apiResponse = await ExecuteStoredProc.CheckDelete(itm, itm.ID, Product.NAME);
						if (!apiResponse.Success)
						{
							return Ok(new ApiResponse
							{
								Success = false,
								Message = apiResponse.Message,
								Data = ""
							});
						}
					}
				}
				apiResponse = await ExecuteStoredProc.CheckDelete(Product, Product.ID, Product.NAME);
				if (!apiResponse.Success)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = apiResponse.Message,
						Data = ""
					});
				}
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				List<web_PhanQuyenSanPham> lstweb_PhanQuyenSanPham = await _context.web_PhanQuyenSanPham.Where((web_PhanQuyenSanPham e) => e.LOC_ID == LOC_ID && e.ID_SANPHAM == ID).ToListAsync();
				if (lstweb_PhanQuyenSanPham != null)
				{
					foreach (web_PhanQuyenSanPham itm2 in lstweb_PhanQuyenSanPham)
					{
						_context.web_PhanQuyenSanPham.Remove(itm2);
					}
				}
				List<dm_HangHoa_Combo> lstdm_HangHoa_Combo = await _context.dm_HangHoa_Combo.Where((dm_HangHoa_Combo e) => e.LOC_ID == LOC_ID && e.ID_HANGHOACOMBO == ID).ToListAsync();
				if (lstdm_HangHoa_Combo != null)
				{
					foreach (dm_HangHoa_Combo itm3 in lstdm_HangHoa_Combo)
					{
						_context.dm_HangHoa_Combo.Remove(itm3);
					}
				}
				if (lstdm_HangHoa_Kho != null)
				{
					foreach (dm_HangHoa_Kho itm4 in lstdm_HangHoa_Kho)
					{
						_context.dm_HangHoa_Kho.Remove(itm4);
					}
				}
				_context.dm_HangHoa.Remove(Product);
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

		private bool ProductExistsMA(string LOC_ID, string MA)
		{
			return _context.dm_HangHoa.Any((dm_HangHoa e) => e.LOC_ID == LOC_ID && e.MA == MA);
		}

		private bool ProductExistsID(string LOC_ID, string ID)
		{
			return _context.dm_HangHoa.Any((dm_HangHoa e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}

		private bool ProductExists(dm_HangHoa HangHoa)
		{
			return _context.dm_HangHoa.Any((dm_HangHoa e) => e.LOC_ID == HangHoa.LOC_ID && e.MA == HangHoa.MA && e.ID != HangHoa.ID);
		}
	}
}
