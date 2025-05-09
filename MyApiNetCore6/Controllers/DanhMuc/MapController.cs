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
        //GET: api/Customer/5
        [HttpGet("{origin}/{destination}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> TinhKhoangCachBangGoogleAPI(string origin, string destination)
        {
            string apiKey = "AIzaSyCyi0Viei8kI_pIPSzB1TBKe3lQNBXvol4"; // Thay thế bằng API key của bạn
            //string origin = "10.762622,106.660172"; // Tọa độ điểm xuất phát
            //string destination = "10.823099,106.629664"; // Tọa độ điểm đến

            string url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={origin}&destinations={destination}&mode=driving&key={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);  
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        //Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = responseBody
                    });
                }
                else
                {
                    string responseBody = response.StatusCode.ToString();
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        //Message = "Không tìm thấy " + LOC_ID + "-" + ID + " dữ liệu!",
                        Data = responseBody
                    });
                    //Console.WriteLine($"Error: {response.StatusCode}");
                }
            }
        }
    }
}