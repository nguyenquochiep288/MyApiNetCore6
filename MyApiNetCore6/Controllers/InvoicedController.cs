using API_QuanLyTHP.Controllers;
using DatabaseTHP;
using DatabaseTHP.Class;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using MyApiNetCore6.Data;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyApiNetCore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicedController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private string link = "https://testapi.meinvoice.vn/api/integration/webapp";
        private string linkToken =  "/token";
        private string linkInvoiced = "/insert";
        private string linkTemplate = "/templates?invoiceWithCode=true";
        public InvoicedController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        [HttpPost("{LOC_ID}")]
        public async Task<IActionResult> Invoice(string LOC_ID, [FromBody] InvoiceMaster invoice)
        {
            try
            {
                var TaiKhoan = await _context.dm_TaiKhoan_Misa!.Where(e => e.LOC_ID == LOC_ID && e.ISACTIVE).FirstAsync();
                if (TaiKhoan == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + " dữ liệu!",
                        Data = ""
                    });
                }
                link = TaiKhoan.LINK;
                InvoiceService _invoiceService = new InvoiceService();
                bool bolCheck = await CheckToken(TaiKhoan);
                if (!bolCheck)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không lấy được Token",
                        Data = ""
                    });
                }

                var invoices = new List<InvoiceMaster>();
                invoices.Add(invoice);
                var response = await _invoiceService.SubmitInvoiceAsync(invoices, TaiKhoan.ACCESSTOKEN, TaiKhoan.MASOTHUE, link + linkInvoiced);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = response
                });
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

        [HttpGet("{LOC_ID}")]
        public async Task<IActionResult> Template(string LOC_ID)
        {
            try
            {
                var TaiKhoan = await _context.dm_TaiKhoan_Misa!.Where(e => e.LOC_ID == LOC_ID && e.ISACTIVE).FirstAsync();
                if (TaiKhoan == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy " + LOC_ID + " dữ liệu!",
                        Data = ""
                    });
                }
                link = TaiKhoan.LINK;
                InvoiceService _invoiceService = new InvoiceService();
                TokenRequest request = new TokenRequest { TaxCode = TaiKhoan.MASOTHUE, Username = TaiKhoan.USERNAME, Password = TaiKhoan.PASSWORD };
                bool bolCheck = await CheckToken(TaiKhoan);
                if (!bolCheck)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Không lấy được Token",
                        Data = ""
                    });
                }
                var response = await _invoiceService.SubmitTemplateAsync(request, link + linkTemplate);

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Success",
                    Data = response
                });
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

        private async Task<Boolean> CheckToken(dm_TaiKhoan_Misa TaiKhoan)
        {
            InvoiceService _invoiceService = new InvoiceService();
            TokenRequest request = new TokenRequest { TaxCode = TaiKhoan.MASOTHUE, Username = TaiKhoan.USERNAME, Password = TaiKhoan.PASSWORD };
            if (string.IsNullOrEmpty(TaiKhoan.ACCESSTOKEN)
              || string.IsNullOrEmpty(TaiKhoan.USERID)
              || TaiKhoan.COMPANYID == null
              || TaiKhoan.THOIGIANLAYTOKEN == null
              || (TaiKhoan.THOIGIANLAYTOKEN != null && TaiKhoan.THOIGIANLAYTOKEN.Value.AddDays(1) > DateTime.Now))
            {
                var token = await _invoiceService.GetTokenAsync(request, link + linkToken);
                if (token != null)
                {
                    TaiKhoan.ACCESSTOKEN = token.AccessToken;
                    TaiKhoan.EXPIRESIN = token.ExpiresIn;
                    TaiKhoan.USERID = token.UserID;
                    TaiKhoan.ORGANIZATIONUNITID = token.OrganizationUnitID;
                    TaiKhoan.COMPANYID = token.ExpiresIn;
                    TaiKhoan.THOIGIANLAYTOKEN = DateTime.Now.AddMinutes(-1);
                    _context.Entry(TaiKhoan).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    AuditLogController auditLog = new AuditLogController(_context, _configuration); auditLog.InserAuditLog(); await _context.SaveChangesAsync();
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}