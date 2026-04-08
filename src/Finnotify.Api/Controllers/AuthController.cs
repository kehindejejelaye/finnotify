using Finnotify.Application;
using Microsoft.AspNetCore.Mvc;


namespace Finnotify.Api.Controllers;

[ApiController]
[Route("/api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("platform/login")]
    public async Task<IActionResult> PlatformLogin([FromBody] LoginRequest request, CancellationToken ct)
    {
        return await Login(KeycloakRealm.Platform, request, ct);
    }

    [HttpPost("app/login")]
    public async Task<IActionResult> AppLogin([FromBody] LoginRequest request, CancellationToken ct)
    {
        return await Login(KeycloakRealm.App, request, ct);
    }

    private async Task<IActionResult> Login(KeycloakRealm realm, LoginRequest request, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(realm, request, ct);
        return result.IsSuccess ? Ok(result) : Unauthorized(result);
    }
}