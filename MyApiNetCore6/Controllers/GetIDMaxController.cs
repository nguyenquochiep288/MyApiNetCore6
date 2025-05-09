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
using DatabaseTHP.StoredProcedure;
using System.Data.Common;
using System.Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace API_QuanLyTHP.Controllers.DanhMuc
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetIDMaxController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public GetIDMaxController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("{TableName}/{LOC_ID}/{NgayLap}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<IActionResult> GetIDMax(string TableName, string LOC_ID, string NgayLap)
        {
            try
            {

                return Ok(await GetIDMaxTable(TableName, LOC_ID, NgayLap));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                });
            }

        }

        private async Task<ApiResponse> GetIDMaxTable(string TableName, string LOC_ID = "", string NgayLap = "")
        {
            if (string.IsNullOrEmpty(NgayLap))
                NgayLap = DateTime.Now.ToString("yyyy-MM-dd");
            string Value = "";
            ApiResponse apiResponse = new ApiResponse();
            DbConnection conn = _context.Database.GetDbConnection();
            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();


                await using (DbCommand command = conn.CreateCommand())
                {

                    command.CommandText = "SELECT ISNULL(MAX(SOPHIEU), 0) FROM " + TableName + " WHERE CAST(NGAYLAP AS date) = '" + NgayLap + "' " +  (string.IsNullOrEmpty(LOC_ID) ? "" : " AND LOC_ID = '" + LOC_ID + "'");
                    var max = await command.ExecuteScalarAsync();
                    if (max != null)
                        Value = max.ToString() ?? "0";
                }
                apiResponse = new ApiResponse
                {
                    Success = true,
                    Message = "Success!",
                    Data = Value
                };
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message, e.InnerException);
                apiResponse = new ApiResponse
                {
                    Success = false,
                    Message = e.Message,
                    Data = Value
                };
            }
            finally
            {
                conn.Close();
            }
            return apiResponse;
        }
    }
}