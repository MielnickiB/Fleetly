using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FleetlyBackend.Helpers
{
    public static class SecretKeyHelper
    {
        public static byte[] GetKeyBytes(IConfiguration configuration, string keyPath = "AppSettings:Token")
        {
            var key = configuration.GetValue<string>(keyPath)
                      ?? throw new InvalidOperationException($"Brakuje wartości konfiguracyjnej '{keyPath}'.");

            try
            {
                return Convert.FromBase64String(key);
            }
            catch (FormatException)
            {
                return Encoding.UTF8.GetBytes(key);
            }
        }

        public static SymmetricSecurityKey GetSymmetricSecurityKey(IConfiguration configuration, string keyPath = "AppSettings:Token", int minBytes = 32)
        {
            var bytes = GetKeyBytes(configuration, keyPath);
            EnsureMinimumKeyLength(bytes, minBytes);
            return new SymmetricSecurityKey(bytes);
        }

        public static void EnsureMinimumKeyLength(byte[] keyBytes, int minBytes = 32)
        {
            if (keyBytes is null) throw new ArgumentNullException(nameof(keyBytes));
            if (keyBytes.Length < minBytes)
                throw new InvalidOperationException($"Klucz JWT musi mieć przynajmniej {minBytes} bajty.");
        }
    }
}
