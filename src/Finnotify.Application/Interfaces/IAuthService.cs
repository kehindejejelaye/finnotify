namespace Finnotify.Application;

public interface IAuthService
{
    Task<Result<LoginResponse>> LoginAsync(KeycloakRealm realm, LoginRequest request, CancellationToken ct);
}