using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatabaseTHP;
using DatabaseTHP.Class;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyApiNetCore6.Class;
using MyApiNetCore6.Data;
using MyApiNetCore6.Models;
using MyApiNetCore6.Repositories;

namespace MyApiNetCore6.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountsController : ControllerBase
{
	private readonly dbApplicationUserContext _context;

	private readonly UserManager<ApplicationUser> userManager;

	private readonly RoleManager<IdentityRole> roleManager;

	private readonly IConfiguration _configuration;

	private readonly IAccountRepository accountRepo;

	private readonly dbTrangHiepPhatContext _contextTHP;

	public AccountsController(dbApplicationUserContext context, dbTrangHiepPhatContext contextTHP, IAccountRepository repo, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
	{
		accountRepo = repo;
		this.userManager = userManager;
		this.roleManager = roleManager;
		_configuration = configuration;
		_context = context;
		_contextTHP = contextTHP;
	}

	[HttpPut("ChangeUser/{id}")]
	public async Task<IActionResult> ChangeUser([FromBody] SignUpModel signUpModel, string id)
	{
		ApplicationUser userExist = await userManager.FindByIdAsync(id);
		if (userExist != null)
		{
			ApplicationUser userExist2 = await userManager.FindByNameAsync(signUpModel.UserName);
			if (userExist2 != null && userExist2.Id != id)
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Tên đăng nhập " + userExist2.UserName + " đã tồn tại!"
				});
			}
			new IdentityResult();
			if (!string.IsNullOrEmpty(signUpModel.Password))
			{
				if (!(await userManager.RemovePasswordAsync(userExist)).Succeeded)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đổi mật khẩu thất bại!"
					});
				}
				IdentityResult result = await userManager.AddPasswordAsync(userExist, clsMaHoa.Decrypt(signUpModel.Password, "tmt6364"));
				if (!result.Succeeded)
				{
					string mes = "";
					foreach (IdentityError itm in result.Errors)
					{
						mes += itm.Description;
					}
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đổi mật khẩu thất bại!" + mes
					});
				}
				userExist.PasswordDecrypt = signUpModel.Password ?? "";
			}
			userExist.URL_IMAGE = signUpModel.URL_IMAGE ?? "";
			userExist.UserName = signUpModel.UserName;
			userExist.FullName = signUpModel.FullName ?? "";
			userExist.ID_NHOMQUYEN = signUpModel.ID_NHOMQUYEN ?? "";
			userExist.PhoneNumber = signUpModel.PhoneNumber ?? "";
			userExist.Email = signUpModel.Email;
			if (!(await userManager.UpdateAsync(userExist)).Succeeded)
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Thay đổi tài khoản thất bại!"
				});
			}
			view_AspNetUsers OKUser = await _contextTHP.view_AspNetUsers.FirstOrDefaultAsync((view_AspNetUsers e) => e.ID == userExist.Id);
			return Ok(new ApiResponse
			{
				Success = true,
				Message = "Thay đổi tài khoản thành công!",
				Data = OKUser
			});
		}
		return Ok(new ApiResponse
		{
			Success = false,
			Message = "Tài khoản không tồn tại!"
		});
	}

	[HttpPut("ChangeUserPassword/{id}")]
	public async Task<IActionResult> ChangeUserPassword([FromBody] SignUpModel signUpModel, string id)
	{
		ApplicationUser userExist = await userManager.FindByIdAsync(id);
		if (userExist != null)
		{
			new IdentityResult();
			if (!string.IsNullOrEmpty(signUpModel.Password))
			{
				if (userExist.PasswordDecrypt != signUpModel.Password)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Mật khẩu không chính xác!"
					});
				}
				if (!(await userManager.RemovePasswordAsync(userExist)).Succeeded)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đổi mật khẩu thất bại!"
					});
				}
				IdentityResult result = await userManager.AddPasswordAsync(userExist, clsMaHoa.Decrypt(signUpModel.ConfirmPassword, "tmt6364"));
				if (!result.Succeeded)
				{
					string mes = "";
					foreach (IdentityError itm in result.Errors)
					{
						mes += itm.Description;
					}
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Đổi mật khẩu thất bại!" + mes
					});
				}
				userExist.PasswordDecrypt = signUpModel.ConfirmPassword ?? "";
			}
			if (!(await userManager.UpdateAsync(userExist)).Succeeded)
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Thay đổi tài khoản thất bại!"
				});
			}
			view_AspNetUsers OKUser = await _contextTHP.view_AspNetUsers.FirstOrDefaultAsync((view_AspNetUsers e) => e.ID == userExist.Id);
			return Ok(new ApiResponse
			{
				Success = true,
				Message = "Thay đổi tài khoản thành công!",
				Data = OKUser
			});
		}
		return Ok(new ApiResponse
		{
			Success = false,
			Message = "Tài khoản không tồn tại!"
		});
	}

	[HttpPost("SignUp")]
	public async Task<IActionResult> SignUp([FromBody] SignUpModel signUpModel)
	{
		if (string.IsNullOrEmpty(clsMaHoa.Decrypt(signUpModel.Password, "tmt6364")))
		{
			signUpModel.Password = clsMaHoa.Encrypt(signUpModel.Password, "tmt6364");
		}
		if (await userManager.FindByNameAsync(signUpModel.UserName) != null)
		{
			return Ok(new ApiResponse
			{
				Success = false,
				Message = "Tài khoản đã tồn tại!"
			});
		}
		ApplicationUser user = new ApplicationUser
		{
			SecurityStamp = Guid.NewGuid().ToString(),
			UserName = (signUpModel.UserName ?? ""),
			FullName = (signUpModel.FullName ?? ""),
			PasswordDecrypt = (signUpModel.Password ?? ""),
			ID_NHOMQUYEN = (signUpModel.ID_NHOMQUYEN ?? ""),
			PhoneNumber = (signUpModel.PhoneNumber ?? ""),
			Email = signUpModel.Email,
			URL_IMAGE = (signUpModel.URL_IMAGE ?? "")
		};
		IdentityResult result = await userManager.CreateAsync(user, clsMaHoa.Decrypt(signUpModel.Password, "tmt6364"));
		if (!result.Succeeded)
		{
			string mes = "";
			foreach (IdentityError itm in result.Errors)
			{
				mes += itm.Description;
			}
			return Ok(new ApiResponse
			{
				Success = false,
				Message = "Tạo tài khoản thất bại!" + mes
			});
		}
		if (!(await roleManager.RoleExistsAsync("Admin")))
		{
			await roleManager.CreateAsync(new IdentityRole("Admin"));
		}
		if (!(await roleManager.RoleExistsAsync("User")))
		{
			await roleManager.CreateAsync(new IdentityRole("User"));
		}
		if (await roleManager.RoleExistsAsync("Admin"))
		{
			await userManager.AddToRoleAsync(user, "Admin");
			await userManager.AddToRoleAsync(user, "User");
		}
		ApplicationUser user2 = await userManager.FindByNameAsync(signUpModel.UserName);
		view_AspNetUsers OKUser = await _contextTHP.view_AspNetUsers.FirstOrDefaultAsync((view_AspNetUsers e) => e.ID == user2.Id);
		return Ok(new ApiResponse
		{
			Success = true,
			Message = "Tạo tài khoản thành công!",
			ID = ((user2 != null) ? user2.Id : ""),
			Data = OKUser
		});
	}

	[HttpPost]
	[Authorize(Roles = "User")]
	public async Task<IActionResult> GetUser()
	{
		string accessToken = await base.HttpContext.GetTokenAsync("access_token");
		Security clsSecurity = new Security(_configuration);
		string UserName = await clsSecurity.GetUserName(accessToken ?? "");
		ApplicationUser userExist = await userManager.FindByNameAsync(UserName);
		if (userExist != null)
		{
			SignUpModel user = new SignUpModel
			{
				UserName = userExist.UserName,
				FullName = userExist.FullName,
				ID_NHOMQUYEN = userExist.ID_NHOMQUYEN,
				PhoneNumber = userExist.PhoneNumber,
				Email = userExist.Email
			};
			return Ok(new ApiResponse
			{
				Success = true,
				Message = "Success",
				Data = user
			});
		}
		return Ok(new ApiResponse
		{
			Success = false,
			Message = "Không tìm thấy thông tin tài khoản"
		});
	}

	[HttpPost("ChangePassword")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> ChangePassword([FromBody] SignUpModel signUpModel)
	{
		ApplicationUser user = await userManager.FindByNameAsync(signUpModel.UserName);
		bool flag = user != null;
		bool flag2 = flag;
		if (flag2)
		{
			flag2 = await userManager.CheckPasswordAsync(user, clsMaHoa.Decrypt(signUpModel.Password, "tmt6364"));
		}
		if (flag2)
		{
			if (!(await userManager.RemovePasswordAsync(user)).Succeeded)
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Đổi mật khẩu thất bại!"
				});
			}
			if (!(await userManager.AddPasswordAsync(user, clsMaHoa.Decrypt(signUpModel.ConfirmPassword, "tmt6364"))).Succeeded)
			{
				return Ok(new ApiResponse
				{
					Success = false,
					Message = "Đổi mật khẩu thất bại!"
				});
			}
			return Ok(new ApiResponse
			{
				Success = true,
				Message = "Tạo tài khoản thành công!"
			});
		}
		return Ok(new ApiResponse
		{
			Success = true,
			Message = "Mật khẩu không chính xác. Vui lòng kiểm tra lại!"
		});
	}

	[HttpPost("Login")]
	public async Task<IActionResult> Login(SignInModel signInModel)
	{
		try
		{
			if (string.IsNullOrEmpty(clsMaHoa.Decrypt(signInModel.Password, "tmt6364")))
			{
				signInModel.Password = clsMaHoa.Encrypt(signInModel.Password, "tmt6364");
			}
			ApplicationUser user = await userManager.FindByNameAsync(signInModel.UserName);
			bool flag = user != null;
			bool flag2 = flag;
			if (flag2)
			{
				flag2 = await userManager.CheckPasswordAsync(user, clsMaHoa.Decrypt(signInModel.Password, "tmt6364"));
			}
			if (flag2)
			{
				if (!user.LockoutEnabled)
				{
					return Ok(new ApiResponse
					{
						Success = false,
						Message = "Tài khoản đã bị khóa!"
					});
				}
				IList<string> userRoles = await userManager.GetRolesAsync(user);
				List<Claim> authClaims = new List<Claim>
				{
					new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", user.UserName),
					new Claim("jti", Guid.NewGuid().ToString())
				};
				foreach (string userRole in userRoles)
				{
					authClaims.Add(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", userRole));
				}
				SymmetricSecurityKey authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
				DateTime expires = DateTime.Now.AddMinutes(30.0);
				string issuer = _configuration["JWT:ValidIssuer"];
				string audience = _configuration["JWT:ValidAudience"];
				DateTime? expires2 = expires;
				SigningCredentials signingCredentials = new SigningCredentials(authenKey, "http://www.w3.org/2001/04/xmldsig-more#hmac-sha512");
				JwtSecurityToken token = new JwtSecurityToken(issuer, audience, authClaims, null, expires2, signingCredentials);
				web_NhomQuyen GroupPermissions = await _contextTHP.web_NhomQuyen.FirstOrDefaultAsync((web_NhomQuyen e) => e.ID == user.ID_NHOMQUYEN);
				return Ok(new ApiResponse
				{
					Success = true,
					Message = "Authenticate success",
					Data = new JwtSecurityTokenHandler().WriteToken(token),
					Expires = expires.AddMinutes(-1.0),
					Detail = new ApiResponseUser
					{
						FullName = user.FullName,
						idUser = user.Id,
						UserName = user.UserName,
						idNhomQuyen = ((GroupPermissions != null && !GroupPermissions.ISPHANQUYEN) ? "-1" : user.ID_NHOMQUYEN)
					}
				});
			}
			return Ok(new ApiResponse
			{
				Success = false,
				Message = ((user != null) ? "Mật khẩu tài khoản không chính xác!" : "Tài khoản không tồn tại!")
			});
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			return Ok(new ApiResponse
			{
				Success = false,
				Message = ex2.Message
			});
		}
	}

	[HttpGet("GetNoteClass")]
	public async Task<IActionResult> GetNoteClass()
	{
		try
		{
			List<view_web_NoteClass> lstValue = await _contextTHP.view_web_NoteClass.ToListAsync();
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

	[HttpGet("GetPhanQuyen/{LOC_ID}/{ID_NHOMQUYEN}")]
	public async Task<IActionResult> GetPhanQuyen(string ID_NHOMQUYEN)
	{
		try
		{
			List<view_web_PhanQuyen> lstValue = await _contextTHP.view_web_PhanQuyen.Where((view_web_PhanQuyen e) => e.ID_NHOMQUYEN == ID_NHOMQUYEN).ToListAsync();
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
}
