// Decompiled with JetBrains decompiler
// Type: API_QuanLyTHP.Controllers.DanhMuc.GetIDMaxController
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using DatabaseTHP;
using DatabaseTHP.Class;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyApiNetCore6.Data;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;


namespace API_QuanLyTHP.Controllers.DanhMuc;

[Route("api/[controller]")]
[ApiController]
public class GetIDMaxController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public GetIDMaxController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{TableName}/{LOC_ID}/{NgayLap}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetIDMax(string TableName, string LOC_ID, string NgayLap)
  {
    try
    {
      ApiResponse idMaxTable = await this.GetIDMaxTable(TableName, LOC_ID, NgayLap);
      return (IActionResult) this.Ok((object) idMaxTable);
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

  private async Task<ApiResponse> GetIDMaxTable(string TableName, string LOC_ID = "", string NgayLap = "")
  {
    if (string.IsNullOrEmpty(NgayLap))
      NgayLap = DateTime.Now.ToString("yyyy-MM-dd");
    string Value = "";
    ApiResponse apiResponse = new ApiResponse();
    DbConnection conn = this._context.Database.GetDbConnection();
    try
    {
      if (conn.State != ConnectionState.Open)
        conn.Open();
      await using (DbCommand command = conn.CreateCommand())
      {
        command.CommandText = $"SELECT ISNULL(MAX(SOPHIEU), 0) FROM {TableName} WHERE CAST(NGAYLAP AS date) = '{NgayLap}' {(string.IsNullOrEmpty(LOC_ID) ? "" : $" AND LOC_ID = '{LOC_ID}'")}";
        object max = await command.ExecuteScalarAsync();
        if (max != null)
          Value = max.ToString() ?? "0";
        max = (object) null;
      }
      apiResponse = new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) Value
      };
    }
    catch (Exception ex)
    {
      apiResponse = new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) Value
      };
    }
    finally
    {
      conn.Close();
    }
    ApiResponse idMaxTable = apiResponse;
    Value = (string) null;
    apiResponse = (ApiResponse) null;
    conn = (DbConnection) null;
    return idMaxTable;
  }
}
