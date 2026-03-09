// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.AuditLogController
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using DatabaseTHP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using MyApiNetCore6.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;


namespace MyApiNetCore6.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuditLogController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public AuditLogController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._configuration = configuration;
  }

  [HttpPost]
  public bool InserAuditLog()
  {
    try
    {
      List<AuditLog> auditLogList = new List<AuditLog>();
      List<EntityEntry> list = this._context.ChangeTracker.Entries().ToList<EntityEntry>();
      List<string> source = new List<string>();
      string str1 = Guid.NewGuid().ToString();
      foreach (EntityEntry entityEntry in list)
      {
        EntityEntry entry = entityEntry;
        if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
        {
          string str2 = "";
          string json = JsonConvert.SerializeObject(entry.CurrentValues.ToObject());
          if (!string.IsNullOrEmpty(json))
          {
            using (JsonDocument jsonDocument = JsonDocument.Parse(json))
              str2 = jsonDocument.RootElement.GetProperty("ID").GetString();
          }
          if (source.Any<string>((Func<string, bool>) (s => s == entry.Entity.GetType().Name)))
            source.Add(entry.Entity.GetType().Name);
          this._context.AuditLog.Add(new AuditLog()
          {
            ID_DINHKEM = str1,
            ID_PHIEU = str2,
            ENTITYNAME = entry.Entity.GetType().Name,
            OPERATION = entry.State.ToString(),
            DATA = JsonConvert.SerializeObject(entry.CurrentValues.ToObject()),
            TIMESTAMP = DateTime.Now
          });
        }
      }
    }
    catch (Exception ex)
    {
    }
    return true;
  }

  [HttpGet("{NAME}")]
  public void DeleteRequest(string NAME)
  {
  }
}
