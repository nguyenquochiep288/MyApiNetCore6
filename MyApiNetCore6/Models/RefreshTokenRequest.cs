using System.ComponentModel.DataAnnotations;

namespace MyApiNetCore6.Models;

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;

    public string? DeviceId { get; set; }
}
