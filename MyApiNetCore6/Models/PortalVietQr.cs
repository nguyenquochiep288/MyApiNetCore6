using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MyApiNetCore6.Models;

public static class PortalVietQr
{
    // NAPAS account-transfer TLV, currency VND, CRC16/CCITT-FALSE.
    // Sources: NAPAS QR Format v1.5.2, VietQR.io account/BIN constraints (see docs).
    public static string Reference(string code)
    {
        var plain = code.Normalize(NormalizationForm.FormD);
        plain = string.Concat(plain.Where(x => CharUnicodeInfo.GetUnicodeCategory(x) != UnicodeCategory.NonSpacingMark)).Replace('đ', 'd').Replace('Đ', 'D');
        plain = "THP " + Regex.Replace(plain.ToUpperInvariant(), "[^A-Z0-9]", "");
        if (plain.Length > 25) plain = plain[..16] + " " + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(code)))[..8];
        return plain;
    }

    public static string? Build(string? bin, string? account, string reference, decimal? amount)
    {
        bin = bin?.Trim();
        // Bank account numbers are often entered with spaces for readability.
        // Preserve every digit (including leading zeroes), rejecting other characters.
        account = account == null ? null : string.Concat(account.Where(c => !char.IsWhiteSpace(c)));
        if (bin == null || account == null || !Regex.IsMatch(bin, "^[0-9]{6}$") || !Regex.IsMatch(account, "^[0-9]{6,19}$")) return null;
        if (amount.HasValue && (amount <= 0 || amount > 9999999999999m || decimal.Truncate(amount.Value) != amount.Value)) return null;
        if (!Regex.IsMatch(reference, "^[A-Z0-9 ]{1,25}$")) return null;
        string Tlv(string tag, string value) => tag + value.Length.ToString("D2", CultureInfo.InvariantCulture) + value;
        var beneficiary = Tlv("00", bin) + Tlv("01", account);
        var merchant = Tlv("00", "A000000727") + Tlv("01", beneficiary) + Tlv("02", "QRIBFTTA");
        var payload = Tlv("00", "01") + Tlv("01", amount.HasValue ? "12" : "11") + Tlv("38", merchant) + Tlv("53", "704");
        if (amount.HasValue) payload += Tlv("54", amount.Value.ToString("0", CultureInfo.InvariantCulture));
        payload += Tlv("58", "VN") + Tlv("62", Tlv("08", reference)) + "6304";
        ushort crc = 0xffff;
        foreach (byte value in Encoding.ASCII.GetBytes(payload))
        {
            crc ^= (ushort)(value << 8);
            for (int i = 0; i < 8; i++) crc = (ushort)((crc & 0x8000) != 0 ? (crc << 1) ^ 0x1021 : crc << 1);
        }
        return payload + crc.ToString("X4", CultureInfo.InvariantCulture);
    }
}
