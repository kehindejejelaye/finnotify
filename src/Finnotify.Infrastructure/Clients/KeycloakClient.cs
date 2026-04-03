using System.Net.Http.Headers;
using Finnotify.Application;
using Finnotify.Application.ConfigurationOptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Finnotify.Application.Interfaces;
using System.Net.Http.Json;

namespace Finnotify.Infrastructure.Clients;

public class KeycloakClient : GenericHttpClient<KeycloakClient>, IKeycloakClient
{
    private readonly KeycloakOptions _options;

    public KeycloakClient(
        HttpClient httpClient,
        ILogger<KeycloakClient> logger,
        IOptions<KeycloakOptions> options)
        : base(httpClient, logger)
    {
        _options = options.Value;
    }

    public async Task<string> GetClientTokenAsync(CancellationToken ct = default)
    {
        var url = $"/realms/{_options.Realm}/protocol/openid-connect/token";

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret,
            ["grant_type"] = "client_credentials"
        });

        _logger.LogInformation("Requesting Keycloak client token");

        var response = await _httpClient.PostAsync(url, content, ct);

        var tokenResponse = await HandleResponse<KeycloakTokenResponse>(response, url);

        return tokenResponse!.AccessToken;
    }

    public async Task CreateUserAsync(
        string accessToken,
        KeycloakUserCreateRequest user,
        CancellationToken ct = default)
    {
        var payload = new
        {
            username = user.Email,
            email = user.Email,
            enabled = user.Enabled,
            firstName = user.FirstName,
            lastName = user.LastName,
            credentials = new[]
            {
                new
                {
                    type = "password",
                    value = user.TemporaryPassword,
                    temporary = true
                }
            }
        };

        var url = $"/admin/realms/{_options.Realm}/users";

        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(payload)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        _logger.LogInformation("Creating user {Username} in Keycloak", user.Email);

        using var response = await _httpClient.SendAsync(request, ct);

        await HandleResponse(response, url);
    }

    public async Task<KeycloakTokenResponse> GetUserTokenAsync(string username, string password, CancellationToken ct = default)
    {
        var url = $"/realms/{_options.Realm}/protocol/openid-connect/token";

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret,
            ["grant_type"] = "password",
            ["username"] = username,
            ["password"] = password
        });

        _logger.LogInformation("Requesting Keycloak token for user {Username}", username);

        var response = await _httpClient.PostAsync(url, content, ct);

        var tokenResponse = await HandleResponse<KeycloakTokenResponse>(response, url);

        return tokenResponse!;
    }
}