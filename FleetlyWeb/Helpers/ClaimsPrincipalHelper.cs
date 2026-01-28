using System.Security.Claims;

namespace FleetlyWeb.Helpers;

public static class ClaimsPrincipalHelper
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

    public static bool IsAdmin(this ClaimsPrincipal user)
        => user.IsInRole("Admin");

    public static bool IsWorker(this ClaimsPrincipal user)
        => user.IsInRole("Worker");

    public static bool IsClient(this ClaimsPrincipal user)
        => user.IsInRole("Client");
}
