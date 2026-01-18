using System.Security.Claims;

namespace FleetlyMobile.Helpers
{
    public static class ClaimsHelper
    {
        public static int GetUserId(ClaimsPrincipal user)
        {
            if (user == null) return 0;

            var claim = user.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null && int.TryParse(claim.Value, out int id) ? id : 0;
        }

        public static string GetFullName(ClaimsPrincipal user)
            => user?.FindFirst("FullName")?.Value ?? "Użytkownik";

        public static string GetEmail(ClaimsPrincipal user)
            => user?.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

        public static bool IsWorker(ClaimsPrincipal user)
            => user?.IsInRole("Worker") ?? false;
    }
}
