using System.Security.Claims;

namespace FleetlyMobile.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            if (user == null) return 0;

            var claim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null && int.TryParse(claim.Value, out int id))
            {
                return id;
            }

            return 0;
        }

        public static string GetFullName(this ClaimsPrincipal user)
            => user?.FindFirst("FullName")?.Value ?? "Użytkownik";

        public static string GetEmail(this ClaimsPrincipal user)
        {
            return user?.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        }

        public static bool IsAdmin(this ClaimsPrincipal user) => user.IsInRole("Admin");
        public static bool IsWorker(this ClaimsPrincipal user) => user.IsInRole("Worker");
        public static bool IsClient(this ClaimsPrincipal user) => user.IsInRole("Client");
    }
}