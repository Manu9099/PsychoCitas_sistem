using System.Text.RegularExpressions;

namespace PsychoCitas.Infrastructure.Services.Notifications;

internal static class PhoneNumberNormalizer
{
    public static string NormalizePeru(string rawPhone, string defaultCountryCode = "+51")
    {
        if (string.IsNullOrWhiteSpace(rawPhone))
            throw new ArgumentException("El teléfono es obligatorio.", nameof(rawPhone));

        var cleaned = Regex.Replace(rawPhone, @"[^\d+]", "");

        if (cleaned.StartsWith("+"))
            return cleaned;

        if (cleaned.StartsWith("00"))
            return "+" + cleaned[2..];

        if (cleaned.StartsWith("51") && cleaned.Length >= 11)
            return "+" + cleaned;

        return $"{defaultCountryCode}{cleaned}";
    }

    public static string ToWhatsAppAddress(string rawPhone, string defaultCountryCode = "+51")
    {
        var normalized = NormalizePeru(rawPhone, defaultCountryCode);
        return $"whatsapp:{normalized}";
    }
}