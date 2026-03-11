// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Startup
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using API_QuanLyTHP;
using API_QuanLyTHP.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyApiNetCore6.Controllers;
using MyApiNetCore6.Data;
using MyApiNetCore6.Repositories;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace MyApiNetCore6;

public class Startup
{
  public Startup(IConfiguration configuration) => this.Configuration = configuration;

  public IConfiguration Configuration { get; }

  public void ConfigureServices(IServiceCollection services)
  {
    new WebHostBuilder().UseKestrel((Action<KestrelServerOptions>) (options =>
    {
      options.Limits.MaxRequestBufferSize = new long?(302768L);
      options.Limits.MaxRequestLineSize = 302768;
    }));
    services.AddControllers();
    services.AddSingleton<RequestQueue>();
    services.AddControllers().AddNewtonsoftJson();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen((Action<SwaggerGenOptions>) (c =>
    {
      c.SwaggerDoc("v1", new OpenApiInfo()
      {
        Title = "Quản lý API",
        Version = "v1"
      });
      c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
      {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\""
      });
      SwaggerGenOptions swaggerGenOptions = c;
      OpenApiSecurityRequirement securityRequirement1 = new OpenApiSecurityRequirement();
      OpenApiSecurityRequirement securityRequirement2 = securityRequirement1;
      OpenApiSecurityScheme key = new OpenApiSecurityScheme();
      key.Reference = new OpenApiReference()
      {
        Type = new ReferenceType?(ReferenceType.SecurityScheme),
        Id = "Bearer"
      };
      string[] strArray = new string[0];
      securityRequirement2.Add(key, (IList<string>) strArray);
      OpenApiSecurityRequirement securityRequirement3 = securityRequirement1;
      swaggerGenOptions.AddSecurityRequirement(securityRequirement3);
    }));
    services.AddDbContext<dbTrangHiepPhatContext>((Action<DbContextOptionsBuilder>) (options => options.UseSqlServer(this.Configuration.GetConnectionString("TrangHiepPhat"), (Action<SqlServerDbContextOptionsBuilder>) (options => options.CommandTimeout(new int?(180))))));
    services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<dbApplicationUserContext>().AddDefaultTokenProviders();
    services.AddDbContext<dbApplicationUserContext>((Action<DbContextOptionsBuilder>) (options => options.UseSqlServer(this.Configuration.GetConnectionString("TrangHiepPhat"))));
    services.AddHostedService<FiveMinuteService>();
    services.AddAutoMapper(typeof (Program));
    services.AddScoped<IAccountRepository, AccountRepository>();
    services.AddAuthentication((Action<AuthenticationOptions>) (options =>
    {
      options.DefaultAuthenticateScheme = "Bearer";
      options.DefaultChallengeScheme = "Bearer";
      options.DefaultScheme = "Bearer";
    })).AddJwtBearer((Action<JwtBearerOptions>) (options =>
    {
      options.SaveToken = true;
      options.RequireHttpsMetadata = false;
      options.TokenValidationParameters = new TokenValidationParameters()
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = this.Configuration["JWT:ValidAudience"],
        ValidIssuer = this.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = (SecurityKey) new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["JWT:Secret"]))
      };
    }));
  }

  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
  {
    if (env.IsDevelopment() || env.IsProduction())
    {
      app.UseDeveloperExceptionPage();
      app.UseSwagger();
      app.UseSwaggerUI((Action<SwaggerUIOptions>) (c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyWebApiApp v1")));
    }
    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseEndpoints((Action<IEndpointRouteBuilder>) (endpoints => endpoints.MapControllers()));
  }
}
