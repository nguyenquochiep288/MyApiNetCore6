using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
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
	public class MenuController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public MenuController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetMenu()
		{
			try
			{
				List<view_web_Menu> lstValue = await _context.view_web_Menu.ToListAsync();
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

		[HttpGet("{Type}/{KeyWhere}/{ValuesSearch}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetMenu(int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<view_web_Menu> lstValue = await _context.view_web_Menu.Where(KeyWhere, ValuesSearch).ToListAsync();
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

		[HttpGet("{id}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetMenu(string id)
		{
			try
			{
				view_web_Menu Menu = await _context.view_web_Menu.FirstOrDefaultAsync((view_web_Menu e) => e.ID == id);
				if (Menu == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + id + " dữ liệu!",
						Data = ""
					});
				}
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = Menu
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

		[HttpPut("{id}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> PutMenu(string id, web_Menu Menu)
		{
			try
			{
				if (id != Menu.ID)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (!MenuExists(Menu.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + id + " dữ liệu!",
						Data = ""
					});
				}
				_context.Entry(Menu).State = EntityState.Modified;
				view_web_Menu itm;
				web_Menu MenuBaoCao;
				foreach (view_web_Menu item in await _context.view_web_Menu.Where((view_web_Menu e) => e.ID == Menu.ID).ToListAsync())
				{
					itm = item;
					List<web_Quyen> lstQuyen = await _context.web_Quyen.Where((web_Quyen e) => e.ID_MENU == itm.ID).ToListAsync();
					if (lstQuyen != null && lstQuyen.Count() > 0)
					{
						continue;
					}
					if (itm.ACTIONNAME == "Index" && itm.CONTROLLERNAME != "ViewReport")
					{
						new web_Quyen();
						if (await _context.web_Quyen.FirstOrDefaultAsync((web_Quyen e) => e.ID_MENU == itm.ID && e.MAQUYEN == "View") == null)
						{
							web_Quyen newweb_Quyen = new web_Quyen
							{
								ID = Guid.NewGuid().ToString(),
								LOC_ID = "02",
								MAQUYEN = "View",
								TENQUYEN = "Xem",
								ID_MENU = itm.ID
							};
							_context.web_Quyen.Add(newweb_Quyen);
						}
						if (await _context.web_Quyen.FirstOrDefaultAsync((web_Quyen e) => e.ID_MENU == itm.ID && e.MAQUYEN == "Edit") == null)
						{
							web_Quyen newweb_Quyen = new web_Quyen
							{
								ID = Guid.NewGuid().ToString(),
								LOC_ID = "02",
								MAQUYEN = "Edit",
								TENQUYEN = "Sửa",
								ID_MENU = itm.ID
							};
							_context.web_Quyen.Add(newweb_Quyen);
						}
						if (await _context.web_Quyen.FirstOrDefaultAsync((web_Quyen e) => e.ID_MENU == itm.ID && e.MAQUYEN == "Delete") == null)
						{
							web_Quyen newweb_Quyen = new web_Quyen
							{
								ID = Guid.NewGuid().ToString(),
								LOC_ID = "02",
								MAQUYEN = "Delete",
								TENQUYEN = "Xóa",
								ID_MENU = itm.ID
							};
							_context.web_Quyen.Add(newweb_Quyen);
						}
					}
					else
					{
						if (!string.IsNullOrEmpty(itm.ACTIONNAME) || string.IsNullOrEmpty(itm.CONTROLLERNAME))
						{
							continue;
						}
						MenuBaoCao = await _context.web_Menu.FirstOrDefaultAsync((web_Menu e) => e.ID == itm.ID_QUYENCHA);
						if (MenuBaoCao != null && MenuBaoCao.CONTROLLERNAME == "ViewReport")
						{
							new web_Quyen();
							if (await _context.web_Quyen.FirstOrDefaultAsync((web_Quyen e) => e.ID_MENU == itm.ID && e.MAQUYEN == "View") == null)
							{
								web_Quyen newweb_Quyen2 = new web_Quyen
								{
									ID = Guid.NewGuid().ToString(),
									LOC_ID = "02",
									MAQUYEN = "View",
									TENQUYEN = "Xem",
									ID_MENU = itm.ID
								};
								_context.web_Quyen.Add(newweb_Quyen2);
							}
							continue;
						}
						while (true)
						{
							if (MenuBaoCao == null || !(MenuBaoCao.CONTROLLERNAME == "ViewReport"))
							{
								break;
							}
							MenuBaoCao = await _context.web_Menu.FirstOrDefaultAsync((web_Menu e) => e.ID == MenuBaoCao.ID_QUYENCHA);
							if (MenuBaoCao != null && MenuBaoCao.CONTROLLERNAME == "ViewReport")
							{
								new web_Quyen();
								if (await _context.web_Quyen.FirstOrDefaultAsync((web_Quyen e) => e.ID_MENU == itm.ID && e.MAQUYEN == "View") == null)
								{
									web_Quyen newweb_Quyen3 = new web_Quyen
									{
										ID = Guid.NewGuid().ToString(),
										LOC_ID = "02",
										MAQUYEN = "View",
										TENQUYEN = "Xem",
										ID_MENU = itm.ID
									};
									_context.web_Quyen.Add(newweb_Quyen3);
								}
							}
						}
					}
				}
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				view_web_Menu OkMenu = await _context.view_web_Menu.FirstOrDefaultAsync((view_web_Menu e) => e.ID == id);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OkMenu
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
		public async Task<ActionResult<web_Menu>> PostMenu(web_Menu Menu)
		{
			try
			{
				if (MenuExists(Menu.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại " + Menu.ID + " trong dữ liệu!",
						Data = "",
						CheckValue = true
					});
				}
				_context.web_Menu.Add(Menu);
				view_web_Menu itm;
				web_Menu MenuBaoCao;
				foreach (view_web_Menu item in await _context.view_web_Menu.Where((view_web_Menu e) => e.ID == Menu.ID).ToListAsync())
				{
					itm = item;
					List<web_Quyen> lstQuyen = await _context.web_Quyen.Where((web_Quyen e) => e.ID_MENU == itm.ID).ToListAsync();
					if (lstQuyen != null && lstQuyen.Count() > 0)
					{
						continue;
					}
					if (itm.ACTIONNAME == "Index" && itm.CONTROLLERNAME != "ViewReport")
					{
						web_Quyen newweb_Quyen = new web_Quyen();
						if (await _context.web_Quyen.FirstOrDefaultAsync((web_Quyen e) => e.ID_MENU == itm.ID && e.MAQUYEN == "View") == null)
						{
							newweb_Quyen.ID = Guid.NewGuid().ToString();
							newweb_Quyen.LOC_ID = "02";
							newweb_Quyen.MAQUYEN = "View";
							newweb_Quyen.TENQUYEN = "Xem";
							newweb_Quyen.ID_MENU = itm.ID;
							_context.web_Quyen.Add(newweb_Quyen);
						}
						if (await _context.web_Quyen.FirstOrDefaultAsync((web_Quyen e) => e.ID_MENU == itm.ID && e.MAQUYEN == "Edit") == null)
						{
							newweb_Quyen.ID = Guid.NewGuid().ToString();
							newweb_Quyen.LOC_ID = "02";
							newweb_Quyen.MAQUYEN = "Edit";
							newweb_Quyen.TENQUYEN = "Sửa";
							newweb_Quyen.ID_MENU = itm.ID;
							_context.web_Quyen.Add(newweb_Quyen);
						}
						if (await _context.web_Quyen.FirstOrDefaultAsync((web_Quyen e) => e.ID_MENU == itm.ID && e.MAQUYEN == "Delete") == null)
						{
							newweb_Quyen.ID = Guid.NewGuid().ToString();
							newweb_Quyen.LOC_ID = "02";
							newweb_Quyen.MAQUYEN = "Delete";
							newweb_Quyen.TENQUYEN = "Xóa";
							newweb_Quyen.ID_MENU = itm.ID;
							_context.web_Quyen.Add(newweb_Quyen);
						}
						if (await _context.web_Quyen.FirstOrDefaultAsync((web_Quyen e) => e.ID_MENU == itm.ID && e.MAQUYEN == "Create") == null)
						{
							newweb_Quyen.ID = Guid.NewGuid().ToString();
							newweb_Quyen.LOC_ID = "02";
							newweb_Quyen.MAQUYEN = "Create";
							newweb_Quyen.TENQUYEN = "Thêm";
							newweb_Quyen.ID_MENU = itm.ID;
							_context.web_Quyen.Add(newweb_Quyen);
						}
					}
					else
					{
						if (!string.IsNullOrEmpty(itm.ACTIONNAME) || string.IsNullOrEmpty(itm.CONTROLLERNAME))
						{
							continue;
						}
						MenuBaoCao = await _context.web_Menu.FirstOrDefaultAsync((web_Menu e) => e.ID == itm.ID_QUYENCHA);
						if (MenuBaoCao != null && MenuBaoCao.CONTROLLERNAME == "ViewReport")
						{
							web_Quyen newweb_Quyen2 = new web_Quyen();
							if (await _context.web_Quyen.FirstOrDefaultAsync((web_Quyen e) => e.ID_MENU == itm.ID && e.MAQUYEN == "View") == null)
							{
								newweb_Quyen2.ID = Guid.NewGuid().ToString();
								newweb_Quyen2.LOC_ID = "02";
								newweb_Quyen2.MAQUYEN = "View";
								newweb_Quyen2.TENQUYEN = "Xem";
								newweb_Quyen2.ID_MENU = itm.ID;
								_context.web_Quyen.Add(newweb_Quyen2);
							}
							continue;
						}
						while (true)
						{
							if (MenuBaoCao == null || !(MenuBaoCao.CONTROLLERNAME == "ViewReport"))
							{
								break;
							}
							MenuBaoCao = await _context.web_Menu.FirstOrDefaultAsync((web_Menu e) => e.ID == MenuBaoCao.ID_QUYENCHA);
							if (MenuBaoCao != null && MenuBaoCao.CONTROLLERNAME == "ViewReport")
							{
								web_Quyen newweb_Quyen3 = new web_Quyen();
								if (await _context.web_Quyen.FirstOrDefaultAsync((web_Quyen e) => e.ID_MENU == itm.ID && e.MAQUYEN == "View") == null)
								{
									newweb_Quyen3.ID = Guid.NewGuid().ToString();
									newweb_Quyen3.LOC_ID = "02";
									newweb_Quyen3.MAQUYEN = "View";
									newweb_Quyen3.TENQUYEN = "Xem";
									newweb_Quyen3.ID_MENU = itm.ID;
									_context.web_Quyen.Add(newweb_Quyen3);
								}
							}
						}
					}
				}
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				view_web_Menu OkMenu = await _context.view_web_Menu.FirstOrDefaultAsync((view_web_Menu e) => e.ID == Menu.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OkMenu
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

		[HttpDelete("{id}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> DeleteMenu(string id)
		{
			try
			{
				web_Menu Menu = await _context.web_Menu.FirstOrDefaultAsync((web_Menu e) => e.ID == id);
				if (Menu == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + id + " dữ liệu!",
						Data = ""
					});
				}
				ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
				ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete(Menu, Menu.ID, Menu.NAME);
				if (!apiResponse.Success)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = apiResponse.Message,
						Data = ""
					});
				}
				List<web_Quyen> lstweb_Quyen = await _context.web_Quyen.Where((web_Quyen e) => e.ID_MENU == Menu.ID).ToListAsync();
				if (lstweb_Quyen != null)
				{
					foreach (web_Quyen itm in lstweb_Quyen)
					{
						List<web_PhanQuyen> lstweb_PhanQuyen = await _context.web_PhanQuyen.Where((web_PhanQuyen e) => e.ID_QUYEN == itm.ID).ToListAsync();
						if (lstweb_PhanQuyen != null)
						{
							foreach (web_PhanQuyen itm2 in lstweb_PhanQuyen)
							{
								_context.web_PhanQuyen.Remove(itm2);
							}
						}
						_context.web_Quyen.Remove(itm);
					}
				}
				_context.web_Menu.Remove(Menu);
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

		private bool MenuExists(string id)
		{
			return _context.web_Menu.Any((web_Menu e) => e.ID == id);
		}
	}
}
