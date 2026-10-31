
using System.Net.Http.Headers;
using System.Text.Json;

namespace P3_RaceDay.MVC.Services;

public class ApiService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient(string? token = null)
    {
        var client = _httpClientFactory.CreateClient("RaceDayApi");

        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    public async Task<T?> GetAsync<T>(
        string endpoint, string? token = null)
    {
        var client = CreateClient(token);

        var response = await client.GetAsync(endpoint);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(
        string endpoint,
        TRequest data,
        string? token = null)
    {
        var client = CreateClient(token);

        var response = await client.PostAsJsonAsync(endpoint, data);

        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"API Error ({(int)response.StatusCode}): {content}");
        }

        if (string.IsNullOrWhiteSpace(content))
            return default;

        return JsonSerializer.Deserialize<TResponse>(
            content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }

    public async Task<bool> PutAsync<T>(
        string endpoint, T data, string? token = null)
    {
        var client = CreateClient(token);

        var response = await client.PutAsJsonAsync(endpoint, data);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(
        string endpoint, string? token = null)
    {
        var client = CreateClient(token);

        var response = await client.DeleteAsync(endpoint);

        return response.IsSuccessStatusCode;
    }
}

