using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyApiNetCore6.Data;

[Table("web_CustomerPortalLinkAudit")]
[Microsoft.EntityFrameworkCore.Index(nameof(LocId), nameof(CustomerId), nameof(OccurredUtc))]
public class CustomerPortalLinkAudit
{
    [Key, StringLength(50)] public string Id { get; set; } = Guid.NewGuid().ToString();
    [Required, StringLength(50)] public string LocId { get; set; } = "";
    [Required, StringLength(50)] public string CustomerId { get; set; } = "";
    [Required, StringLength(50)] public string CustomerCode { get; set; } = "";
    [Required, StringLength(200)] public string CustomerName { get; set; } = "";
    [Required, StringLength(20)] public string Action { get; set; } = "";
    public DateTime OccurredUtc { get; set; }
    [Required, StringLength(50)] public string ActorUserId { get; set; } = "";
    [Required, StringLength(256)] public string ActorUsername { get; set; } = "";
    [Required, StringLength(256)] public string ActorDisplayName { get; set; } = "";
    public DateTime? ExpiresUtc { get; set; }
    public DateTime? PreviousExpiresUtc { get; set; }
}

public partial class dbTrangHiepPhatContext
{
    public DbSet<CustomerPortalLinkAudit> CustomerPortalLinkAudits { get; set; } = null!;
}
