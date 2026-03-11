// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Program
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using System;

#nullable enable
namespace MyApiNetCore6;

public class Program
{
  public static void Main(string[] args)
  {
    Program.CreateHostBuilder(args).Build().Run();
    WebApplication.CreateBuilder(args).WebHost.ConfigureKestrel((Action<KestrelServerOptions>) (options => options.Limits.MaxRequestLineSize = 302768));
  }

  public static IHostBuilder CreateHostBuilder(string[] args)
  {
    return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults((Action<IWebHostBuilder>) (webBuilder => webBuilder.UseStartup<Startup>()));
  }
}
