using Finnotify.Application.ConfigurationOptions;

namespace Finnotify.Application.Interfaces;

public interface IKeycloakClient
{
    /// <summary>
    /// Requests a client credentials token from Keycloak
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Access token string</returns>
    Task<string> GetClientTokenAsync(CancellationToken ct = default);

    /// <summary>
    /// Creates a user in Keycloak
    /// </summary>
    /// <param name="accessToken">Bearer token</param>
    /// <param name="payload">User payload</param>
    /// <param name="ct">Cancellation token</param>
    Task CreateUserAsync(string accessToken, KeycloakUserCreateRequest payload, CancellationToken ct = default);

    Task<KeycloakTokenResponse> GetUserTokenAsync(string username, string password, CancellationToken ct = default);
}