using System.Globalization;

namespace FleetlyBackend.Helpers
{
    public static class TimeHelper
    {
        // Parsuje "HH:mm" lub "H:mm" i zwraca TimeSpan, waliduje zakres < 24h
        public static bool TryParseHourMinute(string? s, out TimeSpan result)
        {
            result = default;
            if (string.IsNullOrWhiteSpace(s)) return false;

            s = s.Trim();

            // akceptujemy "h:mm" i "hh:mm"
            var formats = new[] { "h\\:mm", "hh\\:mm" };

            if (TimeSpan.TryParseExact(s, formats, CultureInfo.InvariantCulture, out result))
            {
                return result.TotalHours >= 0 && result.TotalHours < 24;
            }

            // fallback: próba ogólnego parsowania (np. "16:30" lub "16:30:00")
            if (TimeSpan.TryParse(s, CultureInfo.InvariantCulture, out result))
            {
                return result.TotalHours >= 0 && result.TotalHours < 24;
            }

            return false;
        }
    }
}