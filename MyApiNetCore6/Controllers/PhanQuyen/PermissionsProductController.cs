using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using DatabaseTHP;
using DatabaseTHP.Class;
using DatabaseTHP.Treeview;
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
	public class PermissionsProductController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public PermissionsProductController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetPermissionsProduct(string LOC_ID)
		{
			try
			{
				List<web_PhanQuyenSanPham> lstValue = await _context.web_PhanQuyenSanPham.Where((web_PhanQuyenSanPham e) => e.LOC_ID == LOC_ID).ToListAsync();
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
		public async Task<IActionResult> GetPermissionsProduct(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				List<web_PhanQuyenSanPham> lstValue = await _context.web_PhanQuyenSanPham.Where((web_PhanQuyenSanPham e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).ToListAsync();
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
		public async Task<IActionResult> GetPermissionsProduct(string LOC_ID, string ID)
		{
			try
			{
				web_PhanQuyenSanPham PermissionsProduct = await _context.web_PhanQuyenSanPham.FirstOrDefaultAsync((web_PhanQuyenSanPham e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (PermissionsProduct == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = PermissionsProduct
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
		public async Task<IActionResult> PutPermissionsProduct(string LOC_ID, string ID, web_PhanQuyenSanPham PermissionsProduct)
		{
			try
			{
				if (LOC_ID != PermissionsProduct.LOC_ID && ID != PermissionsProduct.ID)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (!PermissionsProductExists(PermissionsProduct.LOC_ID, PermissionsProduct.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.Entry(PermissionsProduct).State = EntityState.Modified;
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
		public async Task<ActionResult<List<Treeview>>> PostPermissionsProduct(List<Treeview> lstTreeview)
		{
			try
			{
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				Treeview FirstOrDefault = lstTreeview.FirstOrDefault();
				if (FirstOrDefault != null)
				{
					_context.Database.ExecuteSqlRaw("UPDATE web_PhanQuyenNhomSanPham SET ISACTIVE = 0 WHERE LOC_ID ='" + FirstOrDefault.LOC_ID + "' AND ID_NHOMQUYEN = '" + FirstOrDefault.idNhomQuyen + "'");
					foreach (Treeview itm in lstTreeview.Where((Treeview s) => s.Name == "TBL_DEPT"))
					{
						web_PhanQuyenNhomSanPham checkSP = await _context.web_PhanQuyenNhomSanPham.FirstOrDefaultAsync((web_PhanQuyenNhomSanPham e) => e.ID_NHOMQUYEN == itm.idNhomQuyen && e.ID_NHOMSANPHAM == itm.id);
						if (checkSP == null && lstTreeview.Where((Treeview s) => s.idNhomSanPham == itm.id && s.Checked).Count() > 0)
						{
							web_PhanQuyenNhomSanPham web_PhanQuyenNhomSanPham2 = new web_PhanQuyenNhomSanPham
							{
								LOC_ID = itm.LOC_ID,
								ID = Guid.NewGuid().ToString(),
								ID_NHOMSANPHAM = itm.id,
								ID_NHOMQUYEN = itm.idNhomQuyen,
								ISACTIVE = itm.Checked,
								ISPHANQUYENSANPHAM = (from s in lstTreeview
													  where s.Name == "TBL_DEPTALL" && s.id == itm.id
													  select s.Checked).FirstOrDefault()
							};
							_context.web_PhanQuyenNhomSanPham.Add(web_PhanQuyenNhomSanPham2);
						}
						else if (checkSP != null)
						{
							checkSP.ISACTIVE = itm.Checked;
							checkSP.ISPHANQUYENSANPHAM = (from s in lstTreeview
														  where s.Name == "TBL_DEPTALL" && s.id == itm.id
														  select s.Checked).FirstOrDefault();
							_context.Entry(checkSP).State = EntityState.Modified;
						}
						AuditLogController auditLog = new AuditLogController(_context, _configuration);
						auditLog.InserAuditLog();
						await _context.SaveChangesAsync();
					}
					_context.Database.ExecuteSqlRaw("UPDATE web_PhanQuyenSanPham SET ISACTIVE = 0 WHERE LOC_ID ='" + FirstOrDefault.LOC_ID + "' AND ID_NHOMQUYEN = '" + FirstOrDefault.idNhomQuyen + "'");
					foreach (Treeview itm2 in lstTreeview.Where((Treeview s) => s.Name.StartsWith("TBL_ITEM-") && s.Checked))
					{
						web_PhanQuyenSanPham checkSP2 = await _context.web_PhanQuyenSanPham.FirstOrDefaultAsync((web_PhanQuyenSanPham e) => e.ID_NHOMQUYEN == itm2.idNhomQuyen && e.ID_SANPHAM == itm2.id);
						if (checkSP2 == null)
						{
							web_PhanQuyenSanPham web_PhanQuyenSanPham2 = new web_PhanQuyenSanPham
							{
								LOC_ID = itm2.LOC_ID,
								ID = Guid.NewGuid().ToString(),
								ID_SANPHAM = itm2.id,
								ID_NHOMQUYEN = itm2.idNhomQuyen,
								ISACTIVE = itm2.Checked
							};
							_context.web_PhanQuyenSanPham.Add(web_PhanQuyenSanPham2);
						}
						else
						{
							checkSP2.ISACTIVE = itm2.Checked;
							_context.Entry(checkSP2).State = EntityState.Modified;
						}
						AuditLogController auditLog2 = new AuditLogController(_context, _configuration);
						auditLog2.InserAuditLog();
						await _context.SaveChangesAsync();
					}
				}
				transaction.Commit();
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = lstTreeview
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
		public async Task<IActionResult> DeletePermissionsProduct(string LOC_ID, string ID)
		{
			try
			{
				web_PhanQuyenSanPham PermissionsProduct = await _context.web_PhanQuyenSanPham.FirstOrDefaultAsync((web_PhanQuyenSanPham e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (PermissionsProduct == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.web_PhanQuyenSanPham.Remove(PermissionsProduct);
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

		private bool PermissionsProductExists(string LOC_ID, string ID)
		{
			return _context.web_PhanQuyenSanPham.Any((web_PhanQuyenSanPham e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}
	}
}
