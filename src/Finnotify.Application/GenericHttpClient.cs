using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace Finnotify.Application; 

public abstract class GenericHttpClient<TClient>
{
    protected readonly HttpClient _httpClient;
    protected readonly ILogger<TClient> _logger;

    protected GenericHttpClient(HttpClient httpClient, 
        ILogger<TClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    protected async Task<TResponse?> GetAsync<TResponse>(string url, CancellationToken ct = default)
    {
        _logger.LogInformation("GET Request to {Url}", url);
        var response = await _httpClient.GetAsync(url, ct);
        return await HandleResponse<TResponse>(response, url);
    }

    protected async Task<TResponse?> PostAsync<TRequest, TResponse>(
        string url,
        TRequest payload,
        CancellationToken ct = default)
    {
        _logger.LogInformation("POST Request to {Url} with payload {@Payload}", url, payload);

        var response = await _httpClient.PostAsJsonAsync(url, payload, ct);

        return await HandleResponse<TResponse>(response, url);
    }

    protected async Task PostAsync<TRequest>(
        string url,
        TRequest payload,
        CancellationToken ct = default)
    {
        _logger.LogInformation("POST Request to {Url} with payload {@Payload}", url, payload);

        var response = await _httpClient.PostAsJsonAsync(url, payload, ct);

        await HandleResponse(response, url);
    }

    protected async Task HandleResponse(HttpResponseMessage response, string url)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogError("HTTP call to {Url} failed with status {StatusCode}. Response: {Response}",
                url, response.StatusCode, content);

            throw new HttpRequestException($"Request to {url} failed with status {response.StatusCode}");
        }
    }

    protected async Task<TResponse?> HandleResponse<TResponse>(HttpResponseMessage response, string url)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogError("HTTP call to {Url} failed with status {StatusCode}. Response: {Response}",
                url, response.StatusCode, content);

            throw new HttpRequestException($"Request to {url} failed with status {response.StatusCode}");
        }
        var result = await response.Content.ReadFromJsonAsync<TResponse>();
    
        // The @ symbol tells Serilog to destructure the object instead of calling .ToString()
        _logger.LogInformation("Response from {Url}: {@Result}", url, result);

        return result;
    }
}