namespace MyApiNetCore6.Models;

public class PortalReadRequest
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string Section { get; set; } = "overview";
    public string? Id { get; set; }
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public decimal? Amount { get; set; }
}

public class PortalStatementRow
{
    public DateTime Date { get; set; }
    public string? Code { get; set; }
    public string Kind { get; set; } = "";
    public decimal Increase { get; set; }
    public decimal Decrease { get; set; }
    public decimal Balance { get; set; }
}

public static class PortalStatementBuilder
{
    public static List<PortalStatementRow> Build(IEnumerable<PortalLedgerEntry> entries, decimal opening)
    {
        var balance = opening;
        return entries.OrderBy(x => x.Date).ThenBy(x => x.Kind, StringComparer.Ordinal).ThenBy(x => x.Id, StringComparer.Ordinal).Select(x =>
        {
            var increase = Convert.ToDecimal(x.Increase);
            var decrease = Convert.ToDecimal(x.Decrease);
            balance += increase - decrease;
            return new PortalStatementRow { Date = x.Date, Code = x.Code, Kind = x.Kind, Increase = increase, Decrease = decrease, Balance = balance };
        }).ToList();
    }
}

public class PortalLinkRequest
{
    public int? ValidDays { get; set; }
}

// Explicit public projections: never serialize database entities, cost prices or audit fields.
public class PortalDocument
{
    public string Id { get; set; } = "";
    public string Kind { get; set; } = "";
    public string Code { get; set; } = "";
    public DateTime Date { get; set; }
    public double Subtotal { get; set; }
    public double Discount { get; set; }
    public double Tax { get; set; }
    public double Total { get; set; }
    public string Status { get; set; } = "";
    public string? InvoiceNumber { get; set; }
    public string? InvoiceSeries { get; set; }
    public string? TaxAuthorityCode { get; set; }
    public string? LookupCode { get; set; }
}

public class PortalLine
{
    public int Position { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Unit { get; set; }
    public double Quantity { get; set; }
    public double Price { get; set; }
    public double Discount { get; set; }
    public double Tax { get; set; }
    public double Total { get; set; }
    public int Nature { get; set; }
}

public class PortalLedgerEntry
{
    public string Id { get; set; } = "";
    public string Kind { get; set; } = "";
    public string? Code { get; set; }
    public DateTime Date { get; set; }
    public double Increase { get; set; }
    public double Decrease { get; set; }
}
