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
	public class CustomerController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public CustomerController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{LOC_ID}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> GetCustomer(string LOC_ID)
		{
			try
			{
				List<view_dm_KhachHang> lstValue = await (from e in _context.view_dm_KhachHang
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
		public async Task<IActionResult> GetCustomer(string LOC_ID, int Type, string KeyWhere = "", string ValuesSearch = "")
		{
			try
			{
				ValuesSearch = ValuesSearch.Replace("%2f", "/");
				List<view_dm_KhachHang> lstValue = await (from e in _context.view_dm_KhachHang.Where((view_dm_KhachHang e) => e.LOC_ID == LOC_ID).Where(KeyWhere, ValuesSearch)
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
		public async Task<IActionResult> GetCustomer(string LOC_ID, string ID)
		{
			try
			{
				view_dm_KhachHang Customer = await _context.view_dm_KhachHang.FirstOrDefaultAsync((view_dm_KhachHang e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Customer == null)
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
					Data = Customer
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
		public async Task<IActionResult> PutCustomer(string LOC_ID, string MA, v_dm_KhachHang Customer)
		{
			try
			{
				if (LOC_ID != Customer.LOC_ID || Customer.MA != MA)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Dữ liệu khóa không giống nhau!",
						Data = ""
					});
				}
				if (CustomerExists(Customer))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Customer.LOC_ID + "-" + Customer.MA + " trong dữ liệu!",
						Data = ""
					});
				}
				if (!CustomerExistsID(LOC_ID, Customer.ID))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + Customer.ID + " dữ liệu!",
						Data = ""
					});
				}
				_context.Entry(Customer).State = EntityState.Modified;
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				view_dm_KhachHang OKCustomer = await _context.view_dm_KhachHang.FirstOrDefaultAsync((view_dm_KhachHang e) => e.LOC_ID == LOC_ID && e.ID == Customer.ID);
                if (!string.IsNullOrEmpty(Customer.PICTURE) && Customer.FILENEW)
                {
                    string path = "C:\\FTP\\Images_Upload\\Customer";
                    if (Customer.FILENEW)
                    {
                        try
                        {
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            if (System.IO.File.Exists(path + "\\" + Customer.PICTURE))
                            {
                                System.IO.File.Delete(path + "\\" + Customer.PICTURE);
                            }
                            System.IO.File.WriteAllBytes(bytes: Convert.FromBase64String(Customer.FILEBASE64), path: path + "\\" + Customer.PICTURE);
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
					Data = OKCustomer
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
		public async Task<ActionResult<dm_KhachHang>> PostCustomer(v_dm_KhachHang Customer)
		{
			try
			{
				if (CustomerExistsMA(Customer.LOC_ID, Customer.MA))
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đã tồn tại" + Customer.LOC_ID + "-" + Customer.MA + " trong dữ liệu!",
						Data = ""
					});
				}
                if (!string.IsNullOrEmpty(Customer.PICTURE) && !string.IsNullOrEmpty(Customer.FILEBASE64))
                {
                    try
                    {
                        string path = "C:\\FTP\\Images_Upload\\Customer";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        System.IO.File.WriteAllBytes(bytes: Convert.FromBase64String(Customer.FILEBASE64), path: path + "\\" + Customer.PICTURE);
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
                _context.dm_KhachHang.Add(Customer);
				AuditLogController auditLog = new AuditLogController(_context, _configuration);
				auditLog.InserAuditLog();
				await _context.SaveChangesAsync();
				view_dm_KhachHang OKCustomer = await _context.view_dm_KhachHang.FirstOrDefaultAsync((view_dm_KhachHang e) => e.LOC_ID == Customer.LOC_ID && e.ID == Customer.ID);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Success",
					Data = OKCustomer
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
		public async Task<IActionResult> DeleteCustomer(string LOC_ID, string ID)
		{
			try
			{
				dm_KhachHang Customer = await _context.dm_KhachHang.FirstOrDefaultAsync((dm_KhachHang e) => e.LOC_ID == LOC_ID && e.ID == ID);
				if (Customer == null)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
						Data = ""
					});
				}
				ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(_context, _configuration);
				ApiResponse apiResponse = await ExecuteStoredProc.CheckDelete(Customer, Customer.ID, Customer.MA);
				if (!apiResponse.Success)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = apiResponse.Message,
						Data = ""
					});
				}
				List<web_PhanQuyenKhachHang> lstweb_PhanQuyenSanPham = await _context.web_PhanQuyenKhachHang.Where((web_PhanQuyenKhachHang e) => e.LOC_ID == LOC_ID && e.ID_KHACHHANG == ID).ToListAsync();
				if (lstweb_PhanQuyenSanPham != null)
				{
					foreach (web_PhanQuyenKhachHang itm in lstweb_PhanQuyenSanPham)
					{
						_context.web_PhanQuyenKhachHang.Remove(itm);
					}
				}
				_context.dm_KhachHang.Remove(Customer);
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

		private bool CustomerExistsMA(string LOC_ID, string MA)
		{
			return _context.dm_KhachHang.Any((dm_KhachHang e) => e.LOC_ID == LOC_ID && e.MA == MA);
		}

		private bool CustomerExistsID(string LOC_ID, string ID)
		{
			return _context.dm_KhachHang.Any((dm_KhachHang e) => e.LOC_ID == LOC_ID && e.ID == ID);
		}

		private bool CustomerExists(dm_KhachHang Customer)
		{
			return _context.dm_KhachHang.Any((dm_KhachHang e) => e.LOC_ID == Customer.LOC_ID && e.MA == Customer.MA && e.ID != Customer.ID);
		}
	}
}
