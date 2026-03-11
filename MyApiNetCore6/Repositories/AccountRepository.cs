// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Repositories.AccountRepository
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyApiNetCore6.Data;
using MyApiNetCore6.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

#nullable enable
namespace MyApiNetCore6.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IConfiguration configuration;

    public AccountRepository(
      UserManager<ApplicationUser> userManager,
      SignInManager<ApplicationUser> signInManager,
      IConfiguration configuration)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.configuration = configuration;
    }

    public async Task<string> SignInAsync(SignInModel model)
    {
        SignInResult result = await this.signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
        if (!result.Succeeded)
            return string.Empty;
        List<Claim> authClaims = new List<Claim>()
    {
      new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/userdata", model.UserName),
      new Claim("jti", Guid.NewGuid().ToString())
    };
        SymmetricSecurityKey authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["JWT:Secret"]));
        string issuer = this.configuration["JWT:ValidIssuer"];
        string audience = this.configuration["JWT:ValidAudience"];
        DateTime? nullable = new DateTime?(DateTime.Now.AddMinutes(20.0));
        List<Claim> claimList = authClaims;
        SigningCredentials signingCredentials1 = new SigningCredentials((SecurityKey)authenKey, "http://www.w3.org/2001/04/xmldsig-more#hmac-sha512");
        DateTime? notBefore = new DateTime?();
        DateTime? expires = nullable;
        SigningCredentials signingCredentials2 = signingCredentials1;
        JwtSecurityToken token = new JwtSecurityToken(issuer, audience, (IEnumerable<Claim>)claimList, notBefore, expires, signingCredentials2);
        return new JwtSecurityTokenHandler().WriteToken((SecurityToken)token);
    }

    public async Task<IdentityResult> SignUpAsync(SignUpModel model)
    {
        ApplicationUser applicationUser = new ApplicationUser();
        applicationUser.FullName = model == null || model.FullName == null ? "" : model.FullName;
        applicationUser.UserName = model != null ? model.UserName : "";
        ApplicationUser user = applicationUser;
        IdentityResult async = await this.userManager.CreateAsync(user, model == null || model.Password == null ? "" : model.Password);
        user = (ApplicationUser)null;
        return async;
    }
}
