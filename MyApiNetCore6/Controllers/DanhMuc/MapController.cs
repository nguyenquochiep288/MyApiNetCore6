using System.Net.Http;
using System.Threading.Tasks;
using DatabaseTHP;
using DatabaseTHP.Class;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyApiNetCore6.Data;

namespace MyApiNetCore6.Controllers
{

	[Route("api/[controller]")]
	[ApiController]
	public class MapController : ControllerBase
	{
		private readonly dbTrangHiepPhatContext _context;

		private readonly IConfiguration _configuration;

		public MapController(dbTrangHiepPhatContext context, IConfiguration configuration)
		{
			_context = context;
			_context = context;
			_configuration = configuration;
		}

		[HttpGet("{origin}/{destination}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> TinhKhoangCachBangGoogleAPI(string origin, string destination)
		{
			string apiKey = "AIzaSyCyi0Viei8kI_pIPSzB1TBKe3lQNBXvol4";
			string url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={origin}&destinations={destination}&mode=driving&key={apiKey}";
			using HttpClient client = new HttpClient();
			HttpResponseMessage response = await client.GetAsync(url);
			if (response.IsSuccessStatusCode)
			{
				string responseBody = await response.Content.ReadAsStringAsync();
				return Ok(new ApiResponse
				{
					Success = false,
					Data = responseBody
				});
			}
			string responseBody2 = response.StatusCode.ToString();
			return Ok(new ApiResponse
			{
				Success = false,
				Data = responseBody2
			});
		}
	}
}
