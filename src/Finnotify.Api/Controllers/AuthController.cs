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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(request, ct);

        if (result.IsSuccess)
            return Ok(result);

        return Unauthorized(result);
    }
}