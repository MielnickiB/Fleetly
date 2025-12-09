using System.Security.Claims;

namespace FleetlyBackend.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static ClaimsPrincipal CurrentUser(this IHttpContextAccessor http)
    {
        var user = (http.HttpContext?.User) ?? throw new UnauthorizedAccessException("Brak użytkownika w kontekście HTTP.");
        return user;
    }

    public static int? GetUserIdOrNull(this ClaimsPrincipal user)
    {
        var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(id, out var parsed))
            return parsed;

        return null;
    }

    public static int GetUserId(this ClaimsPrincipal user)
    {
        return GetUserIdOrNull(user)
               ?? throw new UnauthorizedAccessException("Brak ID użytkownika w tokenie.");
    }

    public static string? GetUserRoleOrNull(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Role);
    }

    public static string GetUserRole(this ClaimsPrincipal user)
    {
        return GetUserRoleOrNull(user)
               ?? throw new UnauthorizedAccessException("Brak roli w tokenie.");
    }

    public static bool IsInRoleInsensitive(this ClaimsPrincipal user, string role)
    {
        var r = user.GetUserRole();
        return r.Equals(role, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsAdmin(this ClaimsPrincipal user) => user.IsInRoleInsensitive("Admin");
    public static bool IsWorker(this ClaimsPrincipal user) => user.IsInRoleInsensitive("Worker");
    public static bool IsClient(this ClaimsPrincipal user) => user.IsInRoleInsensitive("Client");
}
