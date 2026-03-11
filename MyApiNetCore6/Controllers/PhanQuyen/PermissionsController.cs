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
	public class PermissionsController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public PermissionsController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetPermissions(string LOC_ID)
		{
			try
			{
				List<web_PhanQuyen> lstValue = await _context.web_PhanQuyen.Where((web_PhanQuyen e) => e.LOC_ID == LOC_ID).ToListAsync();
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
		public async Task<IActionResult> GetPermissions(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				List<web_PhanQuyen> lstValue = await _context.web_PhanQuyen.Where((web_PhanQuyen e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch).ToListAsync();
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

		[HttpGet("{LOC_ID}/{id}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetPermissions(string LOC_ID, string id)
		{
			try
			{
				web_PhanQuyen Permissions = await _context.web_PhanQuyen.FirstOrDefaultAsync((web_PhanQuyen e) => e.LOC_ID == LOC_ID && e.ID == id);
				if (Permissions == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + id + " dữ liệu!",
						Data = ""
					});
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = Permissions
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

		[HttpPut("{LOC_ID}/{id}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> PutPermissions(string LOC_ID, string id, web_PhanQuyen Permissions)
		{
			try
			{
				if (LOC_ID != Permissions.LOC_ID && id != Permissions.ID)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (!PermissionsExists(Permissions.LOC_ID, Permissions.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + id + " dữ liệu!",
						Data = ""
					});
				}
				_context.Entry(Permissions).State = EntityState.Modified;
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
		public async Task<ActionResult<List<Treeview>>> PostPermissionsCustomer(List<Treeview> lstTreeview)
		{
			try
			{
				using IDbContextTransaction transaction = _context.Database.BeginTransaction();
				Treeview FirstOrDefault = lstTreeview.FirstOrDefault();
				if (FirstOrDefault != null)
				{
					_context.Database.ExecuteSqlRaw("UPDATE web_PhanQuyen SET ISACTIVE = 0 WHERE LOC_ID ='" + FirstOrDefault.LOC_ID + "' AND ID_NHOMQUYEN = '" + FirstOrDefault.idNhomQuyen + "'");
					foreach (Treeview itm in lstTreeview.Where((Treeview s) => s.Name.StartsWith("TBL_ITEM-") && s.Checked))
					{
						web_PhanQuyen checkSP = await _context.web_PhanQuyen.FirstOrDefaultAsync((web_PhanQuyen e) => e.ID_NHOMQUYEN == itm.idNhomQuyen && e.ID_QUYEN == itm.id);
						if (checkSP == null)
						{
							web_PhanQuyen newweb_PhanQuyen = new web_PhanQuyen
							{
								LOC_ID = itm.LOC_ID,
								ID = Guid.NewGuid().ToString(),
								ID_QUYEN = itm.id,
								ID_NHOMQUYEN = itm.idNhomQuyen,
								ISACTIVE = itm.Checked
							};
							_context.web_PhanQuyen.Add(newweb_PhanQuyen);
						}
						else
						{
							checkSP.ISACTIVE = itm.Checked;
							_context.Entry(checkSP).State = EntityState.Modified;
						}
						AuditLogController auditLog = new AuditLogController(_context, _configuration);
						auditLog.InserAuditLog();
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

		[HttpDelete("{LOC_ID}/{id}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> DeletePermissions(string LOC_ID, string id)
		{
			try
			{
				web_PhanQuyen Permissions = await _context.web_PhanQuyen.FirstOrDefaultAsync((web_PhanQuyen e) => e.LOC_ID == LOC_ID && e.ID == id);
				if (Permissions == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + id + " dữ liệu!",
						Data = ""
					});
				}
				_context.web_PhanQuyen.Remove(Permissions);
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

		private bool PermissionsExists(string LOC_ID, string id)
		{
			return _context.web_PhanQuyen.Any((web_PhanQuyen e) => e.LOC_ID == LOC_ID && e.ID == id);
		}
	}
}
