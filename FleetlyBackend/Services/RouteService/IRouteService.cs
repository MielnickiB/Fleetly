namespace FleetlyBackend.Services.RouteService
{
    public interface IRouteService
    {
        Task<int> CalculateDistanceAsync(string startAddress, string endAddress);
    }
}
