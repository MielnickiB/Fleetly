using System.Security.Claims;

namespace FleetlyWeb.Helpers;

public static class ClaimsPrincipalHelper
{
    public static bool IsAdmin(this ClaimsPrincipal user)
        => user.IsInRole("Admin");

    public static bool IsWorker(this ClaimsPrincipal user)
        => user.IsInRole("Worker");

    public static bool IsClient(this ClaimsPrincipal user)
        => user.IsInRole("Client");
}
