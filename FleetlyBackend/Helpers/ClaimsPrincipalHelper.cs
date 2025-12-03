using System.Security.Claims;

namespace FleetlyBackend.Helpers
{
    public static class ClaimsPrincipalHelper
    {
        /// <summary>
        /// Próbuje odczytać Id użytkownika z ClaimTypes.NameIdentifier.
        /// Zwraca true gdy udało się sparsować id (int). W przeciwnym razie zwraca false i daje komunikat błędu.
        /// </summary>
        public static bool TryGetUserId(this ClaimsPrincipal principal, out int userId, out string? error)
        {
            userId = default;
            error = null;

            var idValue = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(idValue))
            {
                error = "Brak ID użytkownika w tokenie.";
                return false;
            }

            if (!int.TryParse(idValue, out userId))
            {
                error = "Nieprawidłowe ID użytkownika.";
                return false;
            }

            return true;
        }
    }
}
