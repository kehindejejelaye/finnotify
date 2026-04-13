using System.Net.Http.Headers;
using System.Net.Http.Json;
using Finnotify.Application;
using Finnotify.Application.ConfigurationOptions;
using Finnotify.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

    /* =======================
       CLIENT CREDENTIALS FLOW
       ======================= */

    public async Task<string> GetClientTokenAsync(
        KeycloakRealm realm,
        CancellationToken ct = default)
    {
        var realmName = ResolveRealm(realm);
        var (clientId, clientSecret) = ResolveBackendClient(realm);

        var url = $"/realms/{realmName}/protocol/openid-connect/token";

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret!,
            ["grant_type"] = "client_credentials"
        });

        _logger.LogInformation(
            "Requesting client token for {Realm} using {ClientId}",
            realmName, clientId);

        var response = await _httpClient.PostAsync(url, content, ct);
        var token = await HandleResponse<KeycloakTokenResponse>(response, url);

        return token!.AccessToken;
    }

    /* =======================
       USER LOGIN FLOW
       ======================= */

    public async Task<KeycloakTokenResponse> GetUserTokenAsync(
        KeycloakRealm realm,
        string username,
        string password,
        CancellationToken ct = default)
    {
        var realmName = ResolveRealm(realm);
        var clientId = ResolveFrontendClient(realm);

        var url = $"/realms/{realmName}/protocol/openid-connect/token";

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = clientId,
            ["grant_type"] = "password",
            ["username"] = username,
            ["password"] = password
        });

        _logger.LogInformation(
            "Requesting user token for {Username} in realm {Realm}",
            username, realmName);

        var response = await _httpClient.PostAsync(url, content, ct);
        var token = await HandleResponse<KeycloakTokenResponse>(response, url);

        return token!;
    }

    /* =======================
       TENANT PROVISIONING
       ======================= */

    public async Task CreateUserInAppRealmAsync(
        string accessToken,
        KeycloakUserCreateRequest user,
        CancellationToken ct = default)
    {
        var url = $"/admin/realms/{_options.AppRealm}/users";

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

        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(payload)
        };

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        _logger.LogInformation(
            "Creating user {Username} in app realm",
            user.Email);

        using var response = await _httpClient.SendAsync(request, ct);
        await HandleResponse(response, url);
    }

    /* =======================
       RESOLUTION HELPERS
       ======================= */

    private string ResolveRealm(KeycloakRealm realm) =>
        realm switch
        {
            KeycloakRealm.Platform => _options.PlatformRealm,
            KeycloakRealm.App => _options.AppRealm,
            _ => throw new ArgumentOutOfRangeException(nameof(realm))
        };

    private (string ClientId, string ClientSecret) ResolveBackendClient(
        KeycloakRealm realm) =>
        realm switch
        {
            KeycloakRealm.Platform => (
                _options.PlatformClientId,
                _options.PlatformClientSecret
            ),
            KeycloakRealm.App => (
                _options.AppClientId,
                _options.AppClientSecret
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(realm))
        };

    private string ResolveFrontendClient(KeycloakRealm realm) =>
        realm switch
        {
            KeycloakRealm.Platform => "platform-admin-frontend",
            KeycloakRealm.App => "tenant-frontend",
            _ => throw new ArgumentOutOfRangeException(nameof(realm))
        };
}