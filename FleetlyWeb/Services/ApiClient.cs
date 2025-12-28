using System.Net.Http.Json;

namespace FleetlyWeb.Services
{
    public class ApiClient(HttpClient http)
    {
        private readonly HttpClient _http = http;

        public async Task<ApiResponse<T?>> GetAsync<T>(string url)
        {
            using var res = await _http.GetAsync(url);
            var status = (int)res.StatusCode;
            if (res.IsSuccessStatusCode)
            {
                var data = await res.Content.ReadFromJsonAsync<T?>();
                return ApiResponse<T?>.SuccessResult(data, status);
            }

            var error = await SafeReadStringAsync(res);
            return ApiResponse<T?>.ErrorResult(error, status);
        }

        public async Task<ApiResponse<TResponse?>> PostAsync<TRequest, TResponse>(string url, TRequest data)
        {
            using var res = await _http.PostAsJsonAsync(url, data);
            var status = (int)res.StatusCode;
            if (res.IsSuccessStatusCode)
            {
                var dto = await res.Content.ReadFromJsonAsync<TResponse?>();
                return ApiResponse<TResponse?>.SuccessResult(dto, status);
            }

            var error = await SafeReadStringAsync(res);
            return ApiResponse<TResponse?>.ErrorResult(error, status);
        }

        public async Task<ApiResponse<bool>> PostNoResultAsync<TRequest>(string url, TRequest data)
        {
            using var res = await _http.PostAsJsonAsync(url, data);
            var status = (int)res.StatusCode;
            if (res.IsSuccessStatusCode)
            {
                return ApiResponse<bool>.SuccessResult(true, status);
            }

            var error = await SafeReadStringAsync(res);
            return ApiResponse<bool>.ErrorResult(error, status);
        }

        public async Task<ApiResponse<TResponse?>> PutAsync<TRequest, TResponse>(string url, TRequest data)
        {
            using var res = await _http.PutAsJsonAsync(url, data);
            var status = (int)res.StatusCode;
            if (res.IsSuccessStatusCode)
            {
                var dto = await res.Content.ReadFromJsonAsync<TResponse?>();
                return ApiResponse<TResponse?>.SuccessResult(dto, status);
            }
            var error = await SafeReadStringAsync(res);
            return ApiResponse<TResponse?>.ErrorResult(error, status);
        }

        public async Task<ApiResponse<bool>> DeleteAsync(string url)
        {
            using var res = await _http.DeleteAsync(url);
            var status = (int)res.StatusCode;
            if (res.IsSuccessStatusCode)
            {
                return ApiResponse<bool>.SuccessResult(true, status);
            }

            var error = await SafeReadStringAsync(res);
            return ApiResponse<bool>.ErrorResult(error, status);
        }

        public async Task<ApiResponse<TResponse?>> DeleteAsync<TResponse>(string url)
        {
            using var res = await _http.DeleteAsync(url);
            var status = (int)res.StatusCode;

            if (res.IsSuccessStatusCode)
            {
                var dto = await res.Content.ReadFromJsonAsync<TResponse?>();
                return ApiResponse<TResponse?>.SuccessResult(dto, status);
            }

            var error = await SafeReadStringAsync(res);
            return ApiResponse<TResponse?>.ErrorResult(error, status);
        }

        private static async Task<string?> SafeReadStringAsync(HttpResponseMessage res)
        {
            try
            {
                var s = await res.Content.ReadAsStringAsync();
                return string.IsNullOrWhiteSpace(s) ? null : s;
            }
            catch
            {
                return null;
            }
        }
    }
}
