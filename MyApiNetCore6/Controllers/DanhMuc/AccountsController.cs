// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.AccountsController
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using DatabaseTHP;
using DatabaseTHP.Class;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyApiNetCore6.Data;
using MyApiNetCore6.Models;
using MyApiNetCore6.Repositories;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


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

  public AccountsController(
    dbApplicationUserContext context,
    dbTrangHiepPhatContext contextTHP,
    IAccountRepository repo,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration)
  {
    this.accountRepo = repo;
    this.userManager = userManager;
    this.roleManager = roleManager;
    this._configuration = configuration;
    this._context = context;
    this._contextTHP = contextTHP;
  }

  [HttpPut("ChangeUser/{id}")]
  public async Task<IActionResult> ChangeUser([FromBody] SignUpModel signUpModel, string id)
  {
    ApplicationUser userExist = await this.userManager.FindByIdAsync(id);
    if (userExist != null)
    {
      ApplicationUser userExist1 = await this.userManager.FindByNameAsync(signUpModel.UserName);
      if (userExist1 != null && userExist1.Id != id)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Tên đăng nhập {userExist1.UserName} đã tồn tại!"
        });
      IdentityResult result = new IdentityResult();
      if (!string.IsNullOrEmpty(signUpModel.Password))
      {
        result = await this.userManager.RemovePasswordAsync(userExist);
        if (!result.Succeeded)
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = "Đổi mật khẩu thất bại!"
          });
        result = await this.userManager.AddPasswordAsync(userExist, clsMaHoa.Decrypt(signUpModel.Password, "tmt6364"));
        if (!result.Succeeded)
        {
          string mes = "";
          foreach (IdentityError itm in result.Errors)
            mes += itm.Description;
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = ("Đổi mật khẩu thất bại!" + mes)
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
      result = await this.userManager.UpdateAsync(userExist);
      if (!result.Succeeded)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Thay đổi tài khoản thất bại!"
        });
      view_AspNetUsers OKUser = await this._contextTHP.view_AspNetUsers.FirstOrDefaultAsync<view_AspNetUsers>((Expression<Func<view_AspNetUsers, bool>>) (e => e.ID == userExist.Id));
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Thay đổi tài khoản thành công!",
        Data = (object) OKUser
      });
    }
    return (IActionResult) this.Ok((object) new ApiResponse()
    {
      Success = false,
      Message = "Tài khoản không tồn tại!"
    });
  }

  [HttpPut("ChangeUserPassword/{id}")]
  public async Task<IActionResult> ChangeUserPassword([FromBody] SignUpModel signUpModel, string id)
  {
    ApplicationUser userExist = await this.userManager.FindByIdAsync(id);
    if (userExist != null)
    {
      IdentityResult result = new IdentityResult();
      if (!string.IsNullOrEmpty(signUpModel.Password))
      {
        if (userExist.PasswordDecrypt != signUpModel.Password)
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = "Mật khẩu không chính xác!"
          });
        result = await this.userManager.RemovePasswordAsync(userExist);
        if (!result.Succeeded)
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = "Đổi mật khẩu thất bại!"
          });
        result = await this.userManager.AddPasswordAsync(userExist, clsMaHoa.Decrypt(signUpModel.ConfirmPassword, "tmt6364"));
        if (!result.Succeeded)
        {
          string mes = "";
          foreach (IdentityError itm in result.Errors)
            mes += itm.Description;
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = ("Đổi mật khẩu thất bại!" + mes)
          });
        }
        userExist.PasswordDecrypt = signUpModel.ConfirmPassword ?? "";
      }
      result = await this.userManager.UpdateAsync(userExist);
      if (!result.Succeeded)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Thay đổi tài khoản thất bại!"
        });
      view_AspNetUsers OKUser = await this._contextTHP.view_AspNetUsers.FirstOrDefaultAsync<view_AspNetUsers>((Expression<Func<view_AspNetUsers, bool>>) (e => e.ID == userExist.Id));
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Thay đổi tài khoản thành công!",
        Data = (object) OKUser
      });
    }
    return (IActionResult) this.Ok((object) new ApiResponse()
    {
      Success = false,
      Message = "Tài khoản không tồn tại!"
    });
  }

  [HttpPost("SignUp")]
  public async Task<IActionResult> SignUp([FromBody] SignUpModel signUpModel)
  {
    if (string.IsNullOrEmpty(clsMaHoa.Decrypt(signUpModel.Password, "tmt6364")))
      signUpModel.Password = clsMaHoa.Encrypt(signUpModel.Password, "tmt6364");
    ApplicationUser userExist = await this.userManager.FindByNameAsync(signUpModel.UserName);
    if (userExist != null)
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = "Tài khoản đã tồn tại!"
      });
    ApplicationUser applicationUser = new ApplicationUser();
    applicationUser.SecurityStamp = Guid.NewGuid().ToString();
    applicationUser.UserName = signUpModel.UserName ?? "";
    applicationUser.FullName = signUpModel.FullName ?? "";
    applicationUser.PasswordDecrypt = signUpModel.Password ?? "";
    applicationUser.ID_NHOMQUYEN = signUpModel.ID_NHOMQUYEN ?? "";
    applicationUser.PhoneNumber = signUpModel.PhoneNumber ?? "";
    applicationUser.Email = signUpModel.Email;
    applicationUser.URL_IMAGE = signUpModel.URL_IMAGE ?? "";
    ApplicationUser user = applicationUser;
    IdentityResult result = await this.userManager.CreateAsync(user, clsMaHoa.Decrypt(signUpModel.Password, "tmt6364"));
    if (!result.Succeeded)
    {
      string mes = "";
      foreach (IdentityError itm in result.Errors)
        mes += itm.Description;
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ("Tạo tài khoản thất bại!" + mes)
      });
    }
    bool flag1 = await this.roleManager.RoleExistsAsync("Admin");
    if (!flag1)
    {
      IdentityResult async1 = await this.roleManager.CreateAsync(new IdentityRole("Admin"));
    }
    bool flag2 = await this.roleManager.RoleExistsAsync("User");
    if (!flag2)
    {
      IdentityResult async2 = await this.roleManager.CreateAsync(new IdentityRole("User"));
    }
    if (await this.roleManager.RoleExistsAsync("Admin"))
    {
      IdentityResult roleAsync1 = await this.userManager.AddToRoleAsync(user, "Admin");
      IdentityResult roleAsync2 = await this.userManager.AddToRoleAsync(user, "User");
    }
    ApplicationUser user1 = await this.userManager.FindByNameAsync(signUpModel.UserName);
    view_AspNetUsers OKUser = await this._contextTHP.view_AspNetUsers.FirstOrDefaultAsync<view_AspNetUsers>((Expression<Func<view_AspNetUsers, bool>>) (e => e.ID == user1.Id));
    return (IActionResult) this.Ok((object) new ApiResponse()
    {
      Success = true,
      Message = "Tạo tài khoản thành công!",
      ID = (user1 != null ? user1.Id : ""),
      Data = (object) OKUser
    });
  }

  [HttpPost]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetUser()
  {
    string accessToken = await this.HttpContext.GetTokenAsync("access_token");
    MyApiNetCore6.Class.Security clsSecurity = new MyApiNetCore6.Class.Security(this._configuration);
    string UserName = await clsSecurity.GetUserName(accessToken ?? "");
    ApplicationUser userExist = await this.userManager.FindByNameAsync(UserName);
    if (userExist != null)
    {
      SignUpModel user = new SignUpModel()
      {
        UserName = userExist.UserName,
        FullName = userExist.FullName,
        ID_NHOMQUYEN = userExist.ID_NHOMQUYEN,
        PhoneNumber = userExist.PhoneNumber,
        Email = userExist.Email
      };
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) user
      });
    }
    return (IActionResult) this.Ok((object) new ApiResponse()
    {
      Success = false,
      Message = "Không tìm thấy thông tin tài khoản"
    });
  }

  [HttpPost("ChangePassword")]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> ChangePassword([FromBody] SignUpModel signUpModel)
  {
    ApplicationUser user = await this.userManager.FindByNameAsync(signUpModel.UserName);
    bool flag = user != null;
    if (flag)
      flag = await this.userManager.CheckPasswordAsync(user, clsMaHoa.Decrypt(signUpModel.Password, "tmt6364"));
    if (flag)
    {
      IdentityResult result = await this.userManager.RemovePasswordAsync(user);
      if (!result.Succeeded)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Đổi mật khẩu thất bại!"
        });
      result = await this.userManager.AddPasswordAsync(user, clsMaHoa.Decrypt(signUpModel.ConfirmPassword, "tmt6364"));
      if (!result.Succeeded)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Đổi mật khẩu thất bại!"
        });
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Tạo tài khoản thành công!"
      });
    }
    return (IActionResult) this.Ok((object) new ApiResponse()
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
        signInModel.Password = clsMaHoa.Encrypt(signInModel.Password, "tmt6364");
      ApplicationUser user = await this.userManager.FindByNameAsync(signInModel.UserName);
      bool flag = user != null;
      if (flag)
        flag = await this.userManager.CheckPasswordAsync(user, clsMaHoa.Decrypt(signInModel.Password, "tmt6364"));
      if (flag)
      {
        if (!user.LockoutEnabled)
          return (IActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = "Tài khoản đã bị khóa!"
          });
        IList<string> userRoles = await this.userManager.GetRolesAsync(user);
        List<Claim> authClaims = new List<Claim>()
        {
          new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", user.UserName),
          new Claim("jti", Guid.NewGuid().ToString())
        };
        foreach (string userRole in (IEnumerable<string>) userRoles)
          authClaims.Add(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", userRole));
        SymmetricSecurityKey authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._configuration["JWT:Secret"]));
        DateTime expires = DateTime.Now.AddMinutes(30.0);
        string issuer = this._configuration["JWT:ValidIssuer"];
        string audience = this._configuration["JWT:ValidAudience"];
        DateTime? nullable = new DateTime?(expires);
        List<Claim> claimList = authClaims;
        SigningCredentials signingCredentials1 = new SigningCredentials((SecurityKey) authenKey, "http://www.w3.org/2001/04/xmldsig-more#hmac-sha512");
        DateTime? notBefore = new DateTime?();
        DateTime? expires1 = nullable;
        SigningCredentials signingCredentials2 = signingCredentials1;
        JwtSecurityToken token = new JwtSecurityToken(issuer, audience, (IEnumerable<Claim>) claimList, notBefore, expires1, signingCredentials2);
        web_NhomQuyen GroupPermissions = await this._contextTHP.web_NhomQuyen.FirstOrDefaultAsync<web_NhomQuyen>((Expression<Func<web_NhomQuyen, bool>>) (e => e.ID == user.ID_NHOMQUYEN));
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Authenticate success",
          Data = (object) new JwtSecurityTokenHandler().WriteToken((SecurityToken) token),
          Expires = (object) expires.AddMinutes(-1.0),
          Detail = (object) new ApiResponseUser()
          {
            FullName = user.FullName,
            idUser = user.Id,
            UserName = user.UserName,
            idNhomQuyen = (GroupPermissions == null || GroupPermissions.ISPHANQUYEN ? user.ID_NHOMQUYEN : "-1")
          }
        });
      }
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = (user != null ? "Mật khẩu tài khoản không chính xác!" : "Tài khoản không tồn tại!")
      });
    }
    catch (Exception ex)
    {
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message
      });
    }
  }

  [HttpGet("GetNoteClass")]
  public async Task<IActionResult> GetNoteClass()
  {
    try
    {
      List<view_web_NoteClass> lstValue = await this._contextTHP.view_web_NoteClass.ToListAsync<view_web_NoteClass>();
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) lstValue
      });
    }
    catch (Exception ex)
    {
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpGet("GetPhanQuyen/{LOC_ID}/{ID_NHOMQUYEN}")]
  public async Task<IActionResult> GetPhanQuyen(string ID_NHOMQUYEN)
  {
    try
    {
      List<view_web_PhanQuyen> lstValue = await this._contextTHP.view_web_PhanQuyen.Where<view_web_PhanQuyen>((Expression<Func<view_web_PhanQuyen, bool>>) (e => e.ID_NHOMQUYEN == ID_NHOMQUYEN)).ToListAsync<view_web_PhanQuyen>();
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) lstValue
      });
    }
    catch (Exception ex)
    {
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }
}
