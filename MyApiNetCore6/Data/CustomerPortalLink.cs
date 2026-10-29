using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyApiNetCore6.Data;

[Table("web_CustomerPortalLink")]
[Microsoft.EntityFrameworkCore.Index(nameof(LocId), nameof(CustomerId), IsUnique = true)]
[Microsoft.EntityFrameworkCore.Index(nameof(TokenHash), IsUnique = true)]
public class CustomerPortalLink
{
    [Key, StringLength(50)] public string Id { get; set; } = Guid.NewGuid().ToString();
    [Required, StringLength(50)] public string LocId { get; set; } = "";
    [Required, StringLength(50)] public string CustomerId { get; set; } = "";
    [Required, StringLength(64)] public string TokenHash { get; set; } = "";
    [StringLength(512)] public string? ProtectedToken { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime? ExpiresUtc { get; set; }
    public DateTime? RevokedUtc { get; set; }
    [StringLength(256)] public string CreatedBy { get; set; } = "";
    [Timestamp] public byte[] Version { get; set; } = Array.Empty<byte>();
}

public partial class dbTrangHiepPhatContext
{
    public DbSet<CustomerPortalLink> CustomerPortalLinks { get; set; } = null!;
}
