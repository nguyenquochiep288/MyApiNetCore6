using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using DatabaseTHP;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using MyApiNetCore6.Data;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using DatabaseTHP.Class;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

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
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetMenu()
        {
            try
            {

                var lstValue = await _context.view_web_Menu!.ToListAsync();
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = lstValue
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                });
            }

        }

        // GET: api/Menu
        [HttpGet("{Type}/{KeyWhere}/{ValuesSearch}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetMenu(int Type, string KeyWhere = "", string ValuesSearch = "")
        {
            try
            {
                ValuesSearch = ValuesSearch.Replace("%2f", "/");
                var lstValue = await _context.view_web_Menu!.Where(KeyWhere, ValuesSearch).ToListAsync();
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = lstValue
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                });
            }
        }


        //GET: api/Menu/5
        [HttpGet("{id}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetMenu(string id)
        {
            try
            {
                var Menu = await _context.view_web_Menu!.FirstOrDefaultAsync(e => e.ID == id);

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
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                });
            }

        }

        // PUT: api/Menu/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = UserRoles.User)]
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
              
                var lstMenu = await _context.view_web_Menu!.Where(e => e.ID == Menu.ID).ToListAsync();
                foreach (var itm in lstMenu)
                {
                    var lstQuyen = await _context.web_Quyen!.Where(e => e.ID_MENU == itm.ID).ToListAsync();
                    if (lstQuyen != null && lstQuyen.Count() > 0)
                    {
                        continue;
                    }

                    if (itm.ACTIONNAME == "Index" && itm.CONTROLLERNAME != API.BaoCao)
                    {
                        web_Quyen newweb_Quyen = new web_Quyen();
                        var Quyen = await _context.web_Quyen!.FirstOrDefaultAsync(e => e.ID_MENU == itm.ID && e.MAQUYEN == API.Xem);
                        if (Quyen == null)
                        {
                            newweb_Quyen.ID = Guid.NewGuid().ToString();
                            newweb_Quyen.LOC_ID = "02";
                            newweb_Quyen.MAQUYEN = API.Xem;
                            newweb_Quyen.TENQUYEN = "Xem";
                            newweb_Quyen.ID_MENU = itm.ID;

                            _context.web_Quyen!.Add(newweb_Quyen);
                        }

                        Quyen = await _context.web_Quyen!.FirstOrDefaultAsync(e => e.ID_MENU == itm.ID && e.MAQUYEN == API.Edit);
                        if (Quyen == null)
                        {
                            newweb_Quyen.ID = Guid.NewGuid().ToString();
                            newweb_Quyen.LOC_ID = "02";
                            newweb_Quyen.MAQUYEN = API.Edit;
                            newweb_Quyen.TENQUYEN = "Sửa";
                            newweb_Quyen.ID_MENU = itm.ID;

                            _context.web_Quyen!.Add(newweb_Quyen);
                        }

                        Quyen = await _context.web_Quyen!.FirstOrDefaultAsync(e => e.ID_MENU == itm.ID && e.MAQUYEN == API.Delete);
                        if (Quyen == null)
                        {
                            newweb_Quyen.ID = Guid.NewGuid().ToString();
                            newweb_Quyen.LOC_ID = "02";
                            newweb_Quyen.MAQUYEN = API.Delete;
                            newweb_Quyen.TENQUYEN = "Xóa";
                            newweb_Quyen.ID_MENU = itm.ID;

                            _context.web_Quyen!.Add(newweb_Quyen);
                        }
                    }
                    else if (string.IsNullOrEmpty(itm.ACTIONNAME) && !string.IsNullOrEmpty(itm.CONTROLLERNAME))
                    {
                        var MenuBaoCao = await _context.web_Menu!.FirstOrDefaultAsync(e => e.ID == itm.ID_QUYENCHA);
                        if(MenuBaoCao != null && MenuBaoCao.CONTROLLERNAME == API.BaoCao)
                        {
                            web_Quyen newweb_Quyen = new web_Quyen();
                            var Quyen = await _context.web_Quyen!.FirstOrDefaultAsync(e => e.ID_MENU == itm.ID && e.MAQUYEN == API.Xem);
                            if (Quyen == null)
                            {
                                newweb_Quyen.ID = Guid.NewGuid().ToString();
                                newweb_Quyen.LOC_ID = "02";
                                newweb_Quyen.MAQUYEN = API.Xem;
                                newweb_Quyen.TENQUYEN = "Xem";
                                newweb_Quyen.ID_MENU = itm.ID;

                                _context.web_Quyen!.Add(newweb_Quyen);
                            }
                        }    
                        else
                        {
                            while (MenuBaoCao != null && MenuBaoCao.CONTROLLERNAME == API.BaoCao)
                            {
                                MenuBaoCao = await _context.web_Menu!.FirstOrDefaultAsync(e => e.ID == MenuBaoCao.ID_QUYENCHA);
                                if (MenuBaoCao != null && MenuBaoCao.CONTROLLERNAME == API.BaoCao)
                                {
                                    web_Quyen newweb_Quyen = new web_Quyen();
                                    var Quyen = await _context.web_Quyen!.FirstOrDefaultAsync(e => e.ID_MENU == itm.ID && e.MAQUYEN == API.Xem);
                                    if (Quyen == null)
                                    {
                                        newweb_Quyen.ID = Guid.NewGuid().ToString();
                                        newweb_Quyen.LOC_ID = "02";
                                        newweb_Quyen.MAQUYEN = API.Xem;
                                        newweb_Quyen.TENQUYEN = "Xem";
                                        newweb_Quyen.ID_MENU = itm.ID;

                                        _context.web_Quyen!.Add(newweb_Quyen);
                                    }
                                }
                            }
                        }    
                    }
                    AuditLogController auditLog = new AuditLogController(_context, _configuration); auditLog.InserAuditLog(); await _context.SaveChangesAsync();
                }
                var OkMenu = await _context.view_web_Menu!.FirstOrDefaultAsync(e => e.ID == id);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = OkMenu
                });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                });
            }
        }

        // POST: api/Menu
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = UserRoles.User)]
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
                _context.web_Menu!.Add(Menu);
              

                var lstMenu = await _context.view_web_Menu!.Where(e => e.ID == Menu.ID).ToListAsync();
                foreach (var itm in lstMenu)
                {
                    var lstQuyen = await _context.web_Quyen!.Where(e => e.ID_MENU == itm.ID).ToListAsync();
                    if (lstQuyen != null && lstQuyen.Count() > 0)
                    {
                        continue;
                    }

                    if (itm.ACTIONNAME == "Index" && itm.CONTROLLERNAME != API.BaoCao)
                    {
                        web_Quyen newweb_Quyen = new web_Quyen();
                        var Quyen = await _context.web_Quyen!.FirstOrDefaultAsync(e => e.ID_MENU == itm.ID && e.MAQUYEN == API.Xem);
                        if (Quyen == null)
                        {
                            newweb_Quyen.ID = Guid.NewGuid().ToString();
                            newweb_Quyen.LOC_ID = "02";
                            newweb_Quyen.MAQUYEN = API.Xem;
                            newweb_Quyen.TENQUYEN = "Xem";
                            newweb_Quyen.ID_MENU = itm.ID;

                            _context.web_Quyen!.Add(newweb_Quyen);
                           
                        }

                        Quyen = await _context.web_Quyen!.FirstOrDefaultAsync(e => e.ID_MENU == itm.ID && e.MAQUYEN == API.Edit);
                        if (Quyen == null)
                        {
                            newweb_Quyen.ID = Guid.NewGuid().ToString();
                            newweb_Quyen.LOC_ID = "02";
                            newweb_Quyen.MAQUYEN = API.Edit;
                            newweb_Quyen.TENQUYEN = "Sửa";
                            newweb_Quyen.ID_MENU = itm.ID;

                            _context.web_Quyen!.Add(newweb_Quyen);
                            
                        }

                        Quyen = await _context.web_Quyen!.FirstOrDefaultAsync(e => e.ID_MENU == itm.ID && e.MAQUYEN == API.Delete);
                        if (Quyen == null)
                        {
                            newweb_Quyen.ID = Guid.NewGuid().ToString();
                            newweb_Quyen.LOC_ID = "02";
                            newweb_Quyen.MAQUYEN = API.Delete;
                            newweb_Quyen.TENQUYEN = "Xóa";
                            newweb_Quyen.ID_MENU = itm.ID;

                            _context.web_Quyen!.Add(newweb_Quyen);
                            
                        }

                        Quyen = await _context.web_Quyen!.FirstOrDefaultAsync(e => e.ID_MENU == itm.ID && e.MAQUYEN == API.Create);
                        if (Quyen == null)
                        {
                            newweb_Quyen.ID = Guid.NewGuid().ToString();
                            newweb_Quyen.LOC_ID = "02";
                            newweb_Quyen.MAQUYEN = API.Create;
                            newweb_Quyen.TENQUYEN = "Thêm";
                            newweb_Quyen.ID_MENU = itm.ID;

                            _context.web_Quyen!.Add(newweb_Quyen);
                            
                        }
                    }
                    else if (string.IsNullOrEmpty(itm.ACTIONNAME) && !string.IsNullOrEmpty(itm.CONTROLLERNAME))
                    {
                        var MenuBaoCao = await _context.web_Menu!.FirstOrDefaultAsync(e => e.ID == itm.ID_QUYENCHA);
                        if (MenuBaoCao != null && MenuBaoCao.CONTROLLERNAME == API.BaoCao)
                        {
                            web_Quyen newweb_Quyen = new web_Quyen();
                            var Quyen = await _context.web_Quyen!.FirstOrDefaultAsync(e => e.ID_MENU == itm.ID && e.MAQUYEN == API.Xem);
                            if (Quyen == null)
                            {
                                newweb_Quyen.ID = Guid.NewGuid().ToString();
                                newweb_Quyen.LOC_ID = "02";
                                newweb_Quyen.MAQUYEN = API.Xem;
                                newweb_Quyen.TENQUYEN = "Xem";
                                newweb_Quyen.ID_MENU = itm.ID;

                                _context.web_Quyen!.Add(newweb_Quyen);
                               
                            }
                        }
                        else
                        {
                            while (MenuBaoCao != null && MenuBaoCao.CONTROLLERNAME == API.BaoCao)
                            {
                                MenuBaoCao = await _context.web_Menu!.FirstOrDefaultAsync(e => e.ID == MenuBaoCao.ID_QUYENCHA);
                                if (MenuBaoCao != null && MenuBaoCao.CONTROLLERNAME == API.BaoCao)
                                {
                                    web_Quyen newweb_Quyen = new web_Quyen();
                                    var Quyen = await _context.web_Quyen!.FirstOrDefaultAsync(e => e.ID_MENU == itm.ID && e.MAQUYEN == API.Xem);
                                    if (Quyen == null)
                                    {
                                        newweb_Quyen.ID = Guid.NewGuid().ToString();
                                        newweb_Quyen.LOC_ID = "02";
                                        newweb_Quyen.MAQUYEN = API.Xem;
                                        newweb_Quyen.TENQUYEN = "Xem";
                                        newweb_Quyen.ID_MENU = itm.ID;

                                        _context.web_Quyen!.Add(newweb_Quyen);
                                        
                                    }
                                }
                            }
                        }
                    }
                    AuditLogController auditLog = new AuditLogController(_context, _configuration); auditLog.InserAuditLog(); await _context.SaveChangesAsync();
                }
                var OkMenu = await _context.view_web_Menu!.FirstOrDefaultAsync(e => e.ID == Menu.ID);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = OkMenu
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                });
            }
        }

        // DELETE: api/Menu/5
        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> DeleteMenu(string id)
        {
            try
            {
                var Menu = await _context.web_Menu!.FirstOrDefaultAsync(e => e.ID == id);
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
                ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete<web_Menu>(Menu, Menu.ID, Menu.NAME);
                if (!apiResponse.Success)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = apiResponse.Message,
                        Data = ""
                    });
                }
                var lstweb_Quyen = await _context.web_Quyen!.Where(e =>e.ID_MENU == Menu.ID).ToListAsync();
                if (lstweb_Quyen != null)
                {
                    foreach (var itm in lstweb_Quyen)
                    {
                        var lstweb_PhanQuyen = await _context.web_PhanQuyen!.Where(e => e.ID_QUYEN == itm.ID).ToListAsync();
                        if (lstweb_PhanQuyen != null)
                        {
                            foreach (var itm1 in lstweb_PhanQuyen)
                            {
                                _context.web_PhanQuyen!.Remove(itm1);
                            }
                        }

                        _context.web_Quyen!.Remove(itm);
                    }
                }
                
                _context.web_Menu!.Remove(Menu);
                AuditLogController auditLog = new AuditLogController(_context, _configuration);auditLog.InserAuditLog();await _context.SaveChangesAsync();
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = ""
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                });
            }
        }

        private bool MenuExists(string id)
        {
            return _context.web_Menu!.Any(e => e.ID == id);
        }
    }
}