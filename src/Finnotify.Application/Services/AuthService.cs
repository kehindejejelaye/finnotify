using Finnotify.Application.Interfaces;

namespace Finnotify.Application.Services;

public class AuthService : IAuthService
{
    private readonly IKeycloakClient _keycloakClient;

    public AuthService(IKeycloakClient keycloakClient)
    {
        _keycloakClient = keycloakClient;
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        try
        {
            var tokenResponse = await _keycloakClient.GetUserTokenAsync(
                request.Email, request.Password, ct);

            var response = new LoginResponse
            {
                AccessToken = tokenResponse.AccessToken,
                ExpiresIn = tokenResponse.ExpiresIn
            };

            return Result<LoginResponse>.Success(response);
        }
        catch
        {
            return Result<LoginResponse>.Failure("Invalid username or password");
        }
    }
}