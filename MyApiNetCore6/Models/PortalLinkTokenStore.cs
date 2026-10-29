using System.Security.Cryptography;
using System.Text;
using MyApiNetCore6.Data;

namespace MyApiNetCore6.Models;

public static class PortalLinkTokenStore
{
    private static byte[] Key(string secret)
    {
        if (string.IsNullOrWhiteSpace(secret)) throw new InvalidOperationException("Portal encryption key is unavailable.");
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        return hmac.ComputeHash(Encoding.UTF8.GetBytes("THP.CustomerPortal.TokenStorage.v1"));
    }

    public static string Protect(string token, string secret, string loc, string customerId)
    {
        var nonce = RandomNumberGenerator.GetBytes(12);
        var plaintext = Encoding.UTF8.GetBytes(token);
        var cipher = new byte[plaintext.Length]; var tag = new byte[16];
        using var aes = new AesGcm(Key(secret));
        aes.Encrypt(nonce, plaintext, cipher, tag, Encoding.UTF8.GetBytes(loc + "\0" + customerId));
        return "v1." + Convert.ToBase64String(nonce.Concat(tag).Concat(cipher).ToArray());
    }

    public static string? Reveal(CustomerPortalLink link, string secret)
    {
        if (string.IsNullOrEmpty(link.ProtectedToken) || !link.ProtectedToken.StartsWith("v1.", StringComparison.Ordinal)) return null;
        try
        {
            var bytes = Convert.FromBase64String(link.ProtectedToken[3..]);
            if (bytes.Length != 71) return null;
            var plaintext = new byte[43];
            using var aes = new AesGcm(Key(secret));
            aes.Decrypt(bytes.AsSpan(0, 12), bytes.AsSpan(28), bytes.AsSpan(12, 16), plaintext, Encoding.UTF8.GetBytes(link.LocId + "\0" + link.CustomerId));
            var actualHash = SHA256.HashData(plaintext);
            var expectedHash = Convert.FromHexString(link.TokenHash);
            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash) ? Encoding.UTF8.GetString(plaintext) : null;
        }
        catch (Exception ex) when (ex is CryptographicException || ex is FormatException || ex is ArgumentException) { return null; }
    }
}
