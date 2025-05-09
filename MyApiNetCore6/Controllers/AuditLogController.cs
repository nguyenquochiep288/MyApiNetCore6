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
using System.Security.Permissions;
using MyApiNetCore6.Models;
using System.Text.Json;

namespace MyApiNetCore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogController : ControllerBase
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        public AuditLogController(dbTrangHiepPhatContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpPost]
        public bool InserAuditLog()
        {
            try
            {
                var auditEntries = new List<AuditLog>();
                var lst = _context.ChangeTracker.Entries().ToList();

                List<string> lstTable = new List<string>();
                string newGUID = Guid.NewGuid().ToString();
                foreach (var entry in lst)
                {
                    if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                    {
                        string ID_PHIEU = "";
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(entry.CurrentValues.ToObject());
                        if (!string.IsNullOrEmpty(json))
                        {
                            using (JsonDocument doc = JsonDocument.Parse(json))
                            {
                                var root = doc.RootElement;
                                ID_PHIEU = root.GetProperty("ID").GetString();
                            }
                        }
                        if (lstTable.Any(s => s == entry.Entity.GetType().Name))
                            lstTable.Add(entry.Entity.GetType().Name);
                        var auditEntry = new AuditLog
                        {
                            ID_DINHKEM = newGUID,
                            ID_PHIEU = ID_PHIEU,
                            ENTITYNAME = entry.Entity.GetType().Name,
                            OPERATION = entry.State.ToString(),
                            DATA = Newtonsoft.Json.JsonConvert.SerializeObject(entry.CurrentValues.ToObject()),
                            TIMESTAMP = DateTime.Now
                        };

                        _context.AuditLog!.Add(auditEntry);
                    }
                }
                //if (lstTable != null && lstTable.Count > 0)
                //{
                //    foreach (string itm in lstTable)
                //    {
                //        if (itm.Replace("v_", "").Contains("ct_PhieuDatHang")
                //            || itm.Replace("v_", "").Contains("ct_PhieuXuat")
                //            || itm.Replace("v_", "").Contains("ct_PhieuNhap"))
                //        {
                //            var check = _context.AspNetRequest!.Where(e => e.NAME == itm.Replace("v_", "")).ToList();
                //            if (check != null)
                //            {
                //                foreach (AspNetRequest request in check)
                //                {
                //                    _context.AspNetRequest!.Remove(request);
                //                }
                //            }
                //        }
                //    }
                //}
            }
            catch (Exception)
            {

            }

            return true;
        }
        [HttpGet("{NAME}")]
        public void DeleteRequest(string NAME)
        {
            //var check = _context.AspNetRequest!.Where(e => e.NAME == NAME).ToList();
            //if (check != null)
            //{
            //    foreach (AspNetRequest request in check)
            //    {
            //        _context.AspNetRequest!.Remove(request);
            //        InserAuditLog();
            //        _context.SaveChanges();
            //    }
            //}
        }
    }
}