// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.MapController
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using DatabaseTHP.Class;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyApiNetCore6.Data;
using System.Net.Http;
using System.Threading.Tasks;

#nullable enable
namespace MyApiNetCore6.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MapController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public MapController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{origin}/{destination}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> TinhKhoangCachBangGoogleAPI(string origin, string destination)
  {
    string apiKey = "AIzaSyCyi0Viei8kI_pIPSzB1TBKe3lQNBXvol4";
    string url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={origin}&destinations={destination}&mode=driving&key={apiKey}";
    using (HttpClient client = new HttpClient())
    {
      HttpResponseMessage response = await client.GetAsync(url);
      if (response.IsSuccessStatusCode)
      {
        string responseBody = await response.Content.ReadAsStringAsync();
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Data = (object) responseBody
        });
      }
      string responseBody1 = response.StatusCode.ToString();
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Data = (object) responseBody1
      });
    }
  }
}
