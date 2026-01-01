using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FleetlyBackend.Services.RouteService
{
    public class RouteService(HttpClient http) : IRouteService
    {
        private readonly HttpClient _http = http;

        public async Task<int> CalculateDistanceAsync(string startAddress, string endAddress)
        {
            var startCoordsTask = GetCoordinates(startAddress);
            var endCoordsTask = GetCoordinates(endAddress);
            await Task.WhenAll(startCoordsTask, endCoordsTask);

            var (Lat, Lon) = await startCoordsTask
                ?? throw new Exception($"Nie udało się odnaleźć adresu początkowego: '{startAddress}'. Sprawdź pisownię.");
            var endCoords = await endCoordsTask
                ?? throw new Exception($"Nie udało się odnaleźć adresu końcowego: '{endAddress}'. Sprawdź pisownię.");

            var sLat = Lat.ToString(CultureInfo.InvariantCulture);
            var sLon = Lon.ToString(CultureInfo.InvariantCulture);
            var eLat = endCoords.Lat.ToString(CultureInfo.InvariantCulture);
            var eLon = endCoords.Lon.ToString(CultureInfo.InvariantCulture);

            var url = $"http://router.project-osrm.org/route/v1/driving/" +
                      $"{sLon},{sLat};" +
                      $"{eLon},{eLat}?overview=false";

            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode) return 0;

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OsrmResponse>(json);

            if (result?.Routes == null || result.Routes.Count == 0) return 0;

            return (int)Math.Ceiling(result.Routes[0].Distance / 1000.0);
        }

        private async Task<(double Lat, double Lon)?> GetCoordinates(string address)
        {
            var url = $"https://nominatim.openstreetmap.org/search?format=json&q={Uri.EscapeDataString(address)}&limit=1";

            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var results = JsonSerializer.Deserialize<List<NominatimResult>>(json);

            var best = results?.FirstOrDefault();
            if (best == null) return null;

            if (double.TryParse(best.Lat, NumberStyles.Any, CultureInfo.InvariantCulture, out var lat) &&
                double.TryParse(best.Lon, NumberStyles.Any, CultureInfo.InvariantCulture, out var lon))
            {
                return (lat, lon);
            }
            return null;
        }

        private class NominatimResult
        {
            [JsonPropertyName("lat")] public string Lat { get; set; } = "";
            [JsonPropertyName("lon")] public string Lon { get; set; } = "";
        }

        private class OsrmResponse
        {
            [JsonPropertyName("routes")] public List<OsrmRoute> Routes { get; set; } = [];
        }

        private class OsrmRoute
        {
            [JsonPropertyName("distance")] public double Distance { get; set; }
        }
    }
}