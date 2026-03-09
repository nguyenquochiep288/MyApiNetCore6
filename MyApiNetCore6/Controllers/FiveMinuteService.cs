// Decompiled with JetBrains decompiler
// Type: API_QuanLyTHP.Controllers.FiveMinuteService
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using API_QuanLyTHP.Controllers.Misa;
using DatabaseTHP;
using DatabaseTHP.Class.Misa;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MyApiNetCore6.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;


namespace API_QuanLyTHP.Controllers;

public class FiveMinuteService : BackgroundService
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;
  private string linkGetlist = "/getlist?InvoiceWithCode=true";
  private int timerInterval = 5;

  public FiveMinuteService(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._configuration = configuration;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    InvoiceService _invoiceService = new InvoiceService();
    while (!stoppingToken.IsCancellationRequested)
    {
      try
      {
        TimeSpan now = DateTime.Now.TimeOfDay;
        this.timerInterval = !(now >= TimeSpan.FromHours(0.0)) || !(now < TimeSpan.FromHours(5.0)) ? 5 : 40;
        Console.WriteLine($"Hàm chạy lúc: {DateTime.Now}");
        dm_TaiKhoan_Misa TaiKhoan = await this._context.dm_TaiKhoan_Misa.Where<dm_TaiKhoan_Misa>((Expression<Func<dm_TaiKhoan_Misa, bool>>) (e => e.LOC_ID == "02" && e.ISACTIVE)).FirstOrDefaultAsync<dm_TaiKhoan_Misa>(stoppingToken);
        if (TaiKhoan != null)
        {
          List<ct_HoaDon> lstValue = await this._context.ct_HoaDon.Where<ct_HoaDon>((Expression<Func<ct_HoaDon, bool>>) (e => e.ISXUATHOADON && string.IsNullOrEmpty(e.MATRACUU_MISA) && string.IsNullOrEmpty(e.ERROR))).ToListAsync<ct_HoaDon>(stoppingToken);
          if (lstValue != null && lstValue.Count > 0)
          {
            List<string> lstID = lstValue.Select<ct_HoaDon, string>((Func<ct_HoaDon, string>) (i => i.ID)).ToList<string>();
            MisaApiResponseInvoiced response = await _invoiceService.GetListInvoiced(lstID, TaiKhoan.ACCESSTOKEN, TaiKhoan.MASOTHUE, TaiKhoan.LINK + this.linkGetlist);
            if (response != null && response.success)
            {
              string dataString = response.data;
              if (!string.IsNullOrEmpty(dataString))
              {
                List<MisaInvoiceInfo> apiResponse = JsonSerializer.Deserialize<List<MisaInvoiceInfo>>(dataString);
                if (apiResponse != null && apiResponse.Count > 0)
                {
                  foreach (MisaInvoiceInfo misaInvoiceInfo in apiResponse)
                  {
                    MisaInvoiceInfo item = misaInvoiceInfo;
                    ct_HoaDon objUpdate = lstValue.FirstOrDefault<ct_HoaDon>((Func<ct_HoaDon, bool>) (e => e.ID == item.RefID));
                    if (objUpdate != null)
                    {
                      objUpdate.MATRACUU_MISA = item.TransactionID;
                      objUpdate.MACQT = item.InvoiceCode;
                      objUpdate.SOHOADON = item.InvNo;
                      objUpdate.KYHIEUHOADON = item.InvSeries;
                      this._context.ct_HoaDon.Update(objUpdate);
                    }
                    objUpdate = (ct_HoaDon) null;
                  }
                  int num = await this._context.SaveChangesAsync(stoppingToken);
                }
                apiResponse = (List<MisaInvoiceInfo>) null;
              }
              dataString = (string) null;
            }
            lstID = (List<string>) null;
            response = (MisaApiResponseInvoiced) null;
          }
          lstValue = (List<ct_HoaDon>) null;
        }
        TaiKhoan = (dm_TaiKhoan_Misa) null;
      }
      catch (OperationCanceledException ex) when (stoppingToken.IsCancellationRequested)
      {
        _invoiceService = (InvoiceService) null;
        return;
      }
      catch (Exception ex)
      {
        Console.WriteLine("FiveMinuteService error: " + ex.Message);
      }
      try
      {
        await Task.Delay(TimeSpan.FromMinutes((double) this.timerInterval), stoppingToken);
      }
      catch (OperationCanceledException ex)
      {
        _invoiceService = (InvoiceService) null;
        return;
      }
    }
    _invoiceService = (InvoiceService) null;
  }
}
