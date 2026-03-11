using API_QuanLyTHP.Controllers.Misa;
using DatabaseTHP;
using DatabaseTHP.Class.Misa;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyApiNetCore6.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MyApiNetCore6.Controllers
{
    public class FiveMinuteService : BackgroundService
    {
        private readonly dbTrangHiepPhatContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FiveMinuteService> _logger;
        private const string LinkGetList = "/getlist?InvoiceWithCode=true";
        private int _timerIntervalMinutes = 5;

        public FiveMinuteService(
            dbTrangHiepPhatContext context,
            IConfiguration configuration,
            ILogger<FiveMinuteService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // InvoiceService is created once and reused for the lifetime of this background service.
            // If InvoiceService requires disposal or has its own dependencies consider registering it in DI instead.
            var invoiceService = new InvoiceService();

            try
            {
                _logger.LogInformation("FiveMinuteService started at {Time}", DateTime.Now);

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        // Adjust interval between midnight and 5 AM
                        var now = DateTime.Now.TimeOfDay;
                        _timerIntervalMinutes = (now >= TimeSpan.FromHours(0) && now < TimeSpan.FromHours(5)) ? 40 : 5;

                        _logger.LogInformation("Run at {Time}, next delay {Minutes} minutes", DateTime.Now, _timerIntervalMinutes);

                        // Find active MISA account
                        var taiKhoan = await _context.dm_TaiKhoan_Misa
                            .Where(e => e.LOC_ID == "02" && e.ISACTIVE)
                            .FirstOrDefaultAsync(stoppingToken);

                        if (taiKhoan != null)
                        {
                            // Get invoices that have been exported but missing MISA lookup/result
                            var invoices = await _context.ct_HoaDon
                                .Where(e => e.ISXUATHOADON
                                            && string.IsNullOrWhiteSpace(e.MATRACUU_MISA)
                                            && string.IsNullOrWhiteSpace(e.ERROR))
                                .ToListAsync(stoppingToken);

                            if (invoices != null && invoices.Count > 0)
                            {
                                var ids = invoices.Select(i => i.ID).ToList();

                                var response = await invoiceService.GetListInvoiced(ids, taiKhoan.ACCESSTOKEN, taiKhoan.MASOTHUE, taiKhoan.LINK + LinkGetList);

                                if (response != null && response.success && !string.IsNullOrWhiteSpace(response.data))
                                {
                                    var options = new JsonSerializerOptions
                                    {
                                        PropertyNameCaseInsensitive = true
                                    };

                                    List<MisaInvoiceInfo>? apiResponse = null;

                                    try
                                    {
                                        apiResponse = JsonSerializer.Deserialize<List<MisaInvoiceInfo>>(response.data, options);
                                    }
                                    catch (JsonException jsonEx)
                                    {
                                        _logger.LogError(jsonEx, "Failed to deserialize MISA response");
                                    }

                                    if (apiResponse != null && apiResponse.Count > 0)
                                    {
                                        // Update matching invoices
                                        foreach (var item in apiResponse)
                                        {
                                            if (string.IsNullOrWhiteSpace(item?.RefID))
                                                continue;

                                            var invoiceToUpdate = invoices.FirstOrDefault(e => e.ID == item.RefID);
                                            if (invoiceToUpdate != null)
                                            {
                                                invoiceToUpdate.MATRACUU_MISA = item.TransactionID;
                                                invoiceToUpdate.MACQT = item.InvoiceCode;
                                                invoiceToUpdate.SOHOADON = item.InvNo;
                                                invoiceToUpdate.KYHIEUHOADON = item.InvSeries;

                                                _context.ct_HoaDon.Update(invoiceToUpdate);
                                            }
                                        }

                                        try
                                        {
                                            var saved = await _context.SaveChangesAsync(stoppingToken);
                                            _logger.LogInformation("Updated {Count} invoice records", saved);
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.LogError(ex, "Error saving updated invoices to database");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            _logger.LogDebug("No active MISA account (LOC_ID='02') found.");
                        }
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        _logger.LogInformation("FiveMinuteService cancellation requested.");
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "FiveMinuteService error");
                    }

                    try
                    {
                        await Task.Delay(TimeSpan.FromMinutes(_timerIntervalMinutes), stoppingToken);
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogInformation("FiveMinuteService delay cancelled.");
                        break;
                    }
                }
            }
            finally
            {
                // If InvoiceService implements IDisposable consider disposing it here.
                _logger.LogInformation("FiveMinuteService stopping at {Time}", DateTime.Now);
            }
        }
    }
}