using System.Net;
using System.Net.Http.Json;

namespace Fleetly.Shared.Client;
public class ApiClient(HttpClient http)
{
    private readonly HttpClient _http = http;

    public async Task<ApiResponse<T?>> GetAsync<T>(string url)
    {
        try
        {
            using var res = await _http.GetAsync(url);
            return await HandleResponseAsync<T?>(res);
        }
        catch (Exception ex)
        {
            return ApiResponse<T?>.ErrorResult(ex.Message, 0);
        }
    }

    public async Task<ApiResponse<TResponse?>> PostAsync<TRequest, TResponse>(string url, TRequest data)
    {
        try
        {
            using var res = await _http.PostAsJsonAsync(url, data);
            return await HandleResponseAsync<TResponse?>(res);
        }
        catch (Exception ex)
        {
            return ApiResponse<TResponse?>.ErrorResult(ex.Message, 0);
        }
    }

    public async Task<ApiResponse<bool>> PostNoResultAsync<TRequest>(string url, TRequest data)
    {
        try
        {
            using var res = await _http.PostAsJsonAsync(url, data);
            return await HandleBoolResponseAsync(res);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResult(ex.Message, 0);
        }
    }

    public async Task<ApiResponse<TResponse?>> PostMultipartAsync<TResponse>(string url, MultipartFormDataContent content)
    {
        try
        {
            using var res = await _http.PostAsync(url, content);
            return await HandleResponseAsync<TResponse?>(res);
        }
        catch (Exception ex)
        {
            return ApiResponse<TResponse?>.ErrorResult(ex.Message, 0);
        }
    }
    public async Task<ApiResponse<TResponse?>> PutAsync<TRequest, TResponse>(string url, TRequest data)
    {
        try
        {
            using var res = await _http.PutAsJsonAsync(url, data);
            return await HandleResponseAsync<TResponse?>(res);
        }
        catch (Exception ex)
        {
            return ApiResponse<TResponse?>.ErrorResult(ex.Message, 0);
        }
    }

    public async Task<ApiResponse<TResponse?>> PutMultipartAsync<TResponse>(string url, MultipartFormDataContent content)
    {
        try
        {
            using var res = await _http.PutAsync(url, content);
            return await HandleResponseAsync<TResponse?>(res);
        }
        catch (Exception ex)
        {
            return ApiResponse<TResponse?>.ErrorResult(ex.Message, 0);
        }
    }

    public async Task<ApiResponse<TResponse?>> PatchAsync<TResponse>(string url, object? data = null)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Patch, url);
            if (data != null)
            {
                request.Content = JsonContent.Create(data);
            }

            using var res = await _http.SendAsync(request);
            return await HandleResponseAsync<TResponse?>(res);
        }
        catch (Exception ex)
        {
            return ApiResponse<TResponse?>.ErrorResult(ex.Message, 0);
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(string url)
    {
        try
        {
            using var res = await _http.DeleteAsync(url);
            return await HandleBoolResponseAsync(res);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResult(ex.Message, 0);
        }
    }

    public async Task<ApiResponse<TResponse?>> DeleteAsync<TResponse>(string url)
    {
        try
        {
            using var res = await _http.DeleteAsync(url);
            return await HandleResponseAsync<TResponse?>(res);
        }
        catch (Exception ex)
        {
            return ApiResponse<TResponse?>.ErrorResult(ex.Message, 0);
        }
    }

    private static async Task<ApiResponse<T>> HandleResponseAsync<T>(HttpResponseMessage res)
    {
        var status = (int)res.StatusCode;

        if (res.StatusCode == HttpStatusCode.Unauthorized)
        {
            return ApiResponse<T>.ErrorResult("Brak autoryzacji, zaloguj się ponownie", status);
        }

        if (res.StatusCode == HttpStatusCode.NoContent)
        {
            return ApiResponse<T>.SuccessResult(default!, status);
        }

        if (res.IsSuccessStatusCode)
        {
            if (res.Content.Headers.ContentLength == 0)
                return ApiResponse<T>.SuccessResult(default!, status);

            var data = await res.Content.ReadFromJsonAsync<T>();
            return ApiResponse<T>.SuccessResult(data!, status);
        }

        var error = await SafeReadStringAsync(res);
        return ApiResponse<T>.ErrorResult(error, status);
    }

    private static async Task<ApiResponse<bool>> HandleBoolResponseAsync(HttpResponseMessage res)
    {
        var status = (int)res.StatusCode;

        if (res.StatusCode == HttpStatusCode.Unauthorized)
        {
            return ApiResponse<bool>.ErrorResult("Brak autoryzacji, zaloguj się ponownie", status);
        }

        if (res.IsSuccessStatusCode)
        {
            return ApiResponse<bool>.SuccessResult(true, status);
        }

        var error = await SafeReadStringAsync(res);
        return ApiResponse<bool>.ErrorResult(error, status);
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
            return "Wystąpił nieoczekiwany błąd sieciowy.";
        }
    }
}