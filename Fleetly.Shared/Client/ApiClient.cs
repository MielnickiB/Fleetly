using System.Net.Http.Json;

namespace Fleetly.Shared.Client;

public class ApiClient(HttpClient http)
{
    private readonly HttpClient _http = http;

    public async Task<ApiResponse<T?>> GetAsync<T>(string url)
    {
        using var res = await _http.GetAsync(url);
        var status = (int)res.StatusCode;
        if (res.StatusCode == System.Net.HttpStatusCode.NoContent)
        {
            return ApiResponse<T?>.SuccessResult(default, status);
        }
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


    public async Task<ApiResponse<TResponse?>> PatchAsync<TResponse>(string url, object? data = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Patch, url);

        if (data != null)
        {
            request.Content = JsonContent.Create(data);
        }

        using var res = await _http.SendAsync(request);
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

    public async Task<ApiResponse<TResponse?>> PostMultipartAsync<TResponse>(string url, MultipartFormDataContent content)
    {
        using var res = await _http.PostAsync(url, content);
        var status = (int)res.StatusCode;
        if (res.IsSuccessStatusCode)
        {
            var dto = await res.Content.ReadFromJsonAsync<TResponse?>();
            return ApiResponse<TResponse?>.SuccessResult(dto, status);
        }
        var error = await SafeReadStringAsync(res);
        return ApiResponse<TResponse?>.ErrorResult(error, status);
    }

    public async Task<ApiResponse<TResponse?>> PutMultipartAsync<TResponse>(string url, MultipartFormDataContent content)
    {
        using var res = await _http.PutAsync(url, content);
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
