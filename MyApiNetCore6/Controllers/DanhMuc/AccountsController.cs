using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
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

	private const string RefreshTokenProvider = "THP_REFRESH";

	private const string DeviceTokenPrefix = "DEV-";

	private static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromDays(1);

	private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(14);

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
				(string refreshToken, DateTime refreshExpires) = await CreateRefreshTokenAsync(user, signInModel.DeviceId);
				return Ok(await CreateAuthenticationResponseAsync(user, refreshToken, refreshExpires));
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

	[HttpPost("Refresh")]
	public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
	{
		if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
		{
			return Unauthorized(new ApiResponse { Success = false, Message = "Refresh token không hợp lệ." });
		}

		string[] parts = request.RefreshToken.Split('.');
		if (parts.Length != 3 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]) || string.IsNullOrWhiteSpace(parts[2]))
		{
			return Unauthorized(new ApiResponse { Success = false, Message = "Refresh token không hợp lệ." });
		}

		await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
		ApplicationUser user = await userManager.FindByIdAsync(parts[0]);
		if (user == null || !user.LockoutEnabled)
		{
			return Unauthorized(new ApiResponse { Success = false, Message = "Refresh token đã hết hạn hoặc bị thu hồi." });
		}

		IdentityUserToken<string> storedToken = await _context.UserTokens.FirstOrDefaultAsync(token =>
			token.UserId == user.Id && token.LoginProvider == RefreshTokenProvider && token.Name == parts[1]);
		string[] storedValue = (storedToken?.Value ?? string.Empty).Split('|');
		if (storedValue.Length != 2 || !long.TryParse(storedValue[1], out long expiryTicks) || expiryTicks <= DateTime.UtcNow.Ticks || !IsRefreshTokenSecretValid(parts[2], storedValue[0]))
		{
			return Unauthorized(new ApiResponse { Success = false, Message = "Refresh token đã hết hạn hoặc bị thu hồi." });
		}

		string deviceTokenId = storedToken.Name.StartsWith(DeviceTokenPrefix, StringComparison.Ordinal)
			? storedToken.Name
			: CreateDeviceTokenId(request.DeviceId);
		List<IdentityUserToken<string>> tokensToRemove = await _context.UserTokens
			.Where(token => token.UserId == user.Id && token.LoginProvider == RefreshTokenProvider)
			.ToListAsync();
		foreach (IdentityUserToken<string> token in tokensToRemove.Where(token =>
			token.Name == storedToken.Name ||
				!token.Name.StartsWith(DeviceTokenPrefix, StringComparison.Ordinal) ||
				IsRefreshTokenExpired(token.Value) ||
				token.Name == deviceTokenId))
		{
			_context.UserTokens.Remove(token);
		}
		(string refreshToken, DateTime refreshExpires) = CreateRefreshToken(user.Id, deviceTokenId);
		_context.UserTokens.Add(new IdentityUserToken<string>
		{
			UserId = user.Id,
			LoginProvider = RefreshTokenProvider,
			Name = deviceTokenId,
			Value = HashRefreshSecret(refreshToken.Split('.')[2]) + "|" + refreshExpires.Ticks
		});
		await _context.SaveChangesAsync();
		ApiResponse response = await CreateAuthenticationResponseAsync(user, refreshToken, refreshExpires);
		await transaction.CommitAsync();

		return Ok(response);
	}

	[HttpPost("Revoke")]
	public async Task<IActionResult> Revoke([FromBody] RefreshTokenRequest request)
	{
		if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
		{
			return Ok(new ApiResponse { Success = true, Message = "Đã đăng xuất." });
		}

		string[] parts = request.RefreshToken.Split('.');
		if (parts.Length == 3)
		{
			IdentityUserToken<string> storedToken = await _context.UserTokens.FirstOrDefaultAsync(token =>
				token.UserId == parts[0] && token.LoginProvider == RefreshTokenProvider && token.Name == parts[1]);
			if (storedToken != null && IsRefreshTokenSecretValid(parts[2], storedToken.Value.Split('|')[0]))
			{
				_context.UserTokens.Remove(storedToken);
				await _context.SaveChangesAsync();
			}
		}

		return Ok(new ApiResponse { Success = true, Message = "Đã đăng xuất." });
	}

	private async Task<ApiResponse> CreateAuthenticationResponseAsync(ApplicationUser user, string refreshToken, DateTime refreshExpires)
	{
		IList<string> roles = await userManager.GetRolesAsync(user);
		List<Claim> claims = new List<Claim>
		{
			new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
			new Claim("jti", Guid.NewGuid().ToString())
		};
		claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

		DateTime accessExpires = DateTime.UtcNow.Add(AccessTokenLifetime);
		var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
		var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512);
		var jwt = new JwtSecurityToken(_configuration["JWT:ValidIssuer"], _configuration["JWT:ValidAudience"], claims, null, accessExpires, signingCredentials);
		web_NhomQuyen groupPermissions = await _contextTHP.web_NhomQuyen.FirstOrDefaultAsync(group => group.ID == user.ID_NHOMQUYEN);

		return new ApiResponse
		{
			Success = true,
			Message = "Authenticate success",
			Data = new JwtSecurityTokenHandler().WriteToken(jwt),
			Expires = accessExpires.AddMinutes(-1),
			RefreshToken = refreshToken,
			RefreshTokenExpires = refreshExpires,
			Detail = new ApiResponseUser
			{
				FullName = user.FullName,
				idUser = user.Id,
				UserName = user.UserName,
				idNhomQuyen = groupPermissions != null && !groupPermissions.ISPHANQUYEN ? "-1" : user.ID_NHOMQUYEN
			}
		};
	}

	private async Task<(string Token, DateTime Expires)> CreateRefreshTokenAsync(ApplicationUser user, string deviceId)
	{
		string deviceTokenId = CreateDeviceTokenId(deviceId);
		await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
		List<IdentityUserToken<string>> tokensToRemove = await _context.UserTokens
			.Where(token => token.UserId == user.Id && token.LoginProvider == RefreshTokenProvider)
			.ToListAsync();
		foreach (IdentityUserToken<string> token in tokensToRemove.Where(token =>
			!token.Name.StartsWith(DeviceTokenPrefix, StringComparison.Ordinal) ||
				IsRefreshTokenExpired(token.Value) ||
				token.Name == deviceTokenId))
		{
			_context.UserTokens.Remove(token);
		}

		(string generatedToken, DateTime expires) = CreateRefreshToken(user.Id, deviceTokenId);
		_context.UserTokens.Add(new IdentityUserToken<string>
		{
			UserId = user.Id,
			LoginProvider = RefreshTokenProvider,
			Name = deviceTokenId,
			Value = HashRefreshSecret(generatedToken.Split('.')[2]) + "|" + expires.Ticks
		});
		await _context.SaveChangesAsync();
		await transaction.CommitAsync();
		return (generatedToken, expires);
	}

	private static (string Token, DateTime Expires) CreateRefreshToken(string userId, string deviceTokenId)
	{
		string secret = Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
		return ($"{userId}.{deviceTokenId}.{secret}", DateTime.UtcNow.Add(RefreshTokenLifetime));
	}

	private static string CreateDeviceTokenId(string deviceId)
	{
		return DeviceTokenPrefix + (Guid.TryParse(deviceId, out Guid parsedDeviceId)
			? parsedDeviceId.ToString("N")
			: Guid.NewGuid().ToString("N"));
	}

	private static bool IsRefreshTokenExpired(string value)
	{
		string[] parts = (value ?? string.Empty).Split('|');
		return parts.Length != 2 || !long.TryParse(parts[1], out long expiryTicks) || expiryTicks <= DateTime.UtcNow.Ticks;
	}

	private static string HashRefreshSecret(string secret)
	{
		return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(secret)));
	}

	private static bool IsRefreshTokenSecretValid(string secret, string expectedHash)
	{
		try
		{
			byte[] expected = Convert.FromBase64String(expectedHash);
			byte[] actual = SHA256.HashData(Encoding.UTF8.GetBytes(secret));
			return CryptographicOperations.FixedTimeEquals(actual, expected);
		}
		catch
		{
			return false;
		}
	}

	private static string Base64UrlEncode(byte[] bytes)
	{
		return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
	}

	[HttpGet("GetNoteClass")]
	public async Task<IActionResult> GetNoteClass()
	{
		try
		{
			List<view_web_NoteClass> lstValue = await _contextTHP.view_web_NoteClass.AsNoTracking().ToListAsync();
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
			List<view_web_PhanQuyen> lstValue = await _contextTHP.view_web_PhanQuyen.AsNoTracking().Where((view_web_PhanQuyen e) => e.ID_NHOMQUYEN == ID_NHOMQUYEN).ToListAsync();
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

	[HttpGet("GetCacheVersion/{LOC_ID}/{ID_NHOMQUYEN}")]
	[Authorize(Roles = "User")]
	public async Task<IActionResult> GetCacheVersion(string LOC_ID, string ID_NHOMQUYEN)
	{
		DbConnection connection = _contextTHP.Database.GetDbConnection();
		bool closeConnection = connection.State != ConnectionState.Open;

		try
		{
			if (closeConnection)
			{
				await connection.OpenAsync();
			}

			await using DbCommand command = connection.CreateCommand();
			command.CommandText = "SELECT [Version] FROM dbo.web_CacheVersion WHERE [ID] = 1";
			object result = await command.ExecuteScalarAsync();
			if (result == null || result == DBNull.Value)
			{
				return StatusCode(503, new ApiResponse
				{
					Success = false,
					Message = "Cache version is not configured.",
					Data = ""
				});
			}

			return Ok(new ApiResponse
			{
				Success = true,
				Message = "Success",
				Data = Convert.ToBase64String((byte[])result)
			});
		}
		catch (Exception)
		{
			return StatusCode(503, new ApiResponse
			{
				Success = false,
				Message = "Cache version is not configured.",
				Data = ""
			});
		}
		finally
		{
			if (closeConnection)
			{
				await connection.CloseAsync();
			}
		}
	}
}
