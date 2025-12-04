using System.Net.Http.Json;

namespace FleetlyWeb.Services
{
    public class ApiClient(HttpClient http)
    {
        private readonly HttpClient _http = http;

        public async Task<T?> GetAsync<T>(string url)
        {
            return await _http.GetFromJsonAsync<T>(url);
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
        {
            var result = await _http.PostAsJsonAsync(url, data);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<bool> PostNoResultAsync<TRequest>(string url, TRequest data)
        {
            var result = await _http.PostAsJsonAsync(url, data);
            result.EnsureSuccessStatusCode();
            return result.IsSuccessStatusCode;
        }

        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest data)
        {
            var result = await _http.PutAsJsonAsync(url, data);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<bool> DeleteAsync(string url)
        {
            var result = await _http.DeleteAsync(url);
            result.EnsureSuccessStatusCode();
            return result.IsSuccessStatusCode;
        }
    }
}
