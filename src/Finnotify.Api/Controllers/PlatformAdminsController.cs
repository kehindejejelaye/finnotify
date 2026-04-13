using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Finnotify.Application.Common;
using Finnotify.Application.Interfaces;
using Finnotify.Application.Common.Dtos;

namespace Finnotify.Api.Controllers;

[Route("api/platform/admins")]
[ApiController]
[Authorize(Policy = Policies.PlatformSuperAdmin)]
public class PlatformAdminsController : ControllerBase
{
    
    private readonly IPlatformAdminProvisioningService _service;

    public PlatformAdminsController(
        IPlatformAdminProvisioningService service)
    {
        _service = service;
    }

    [HttpPost("invite")]
    [Authorize(Policy = "PlatformSuperAdminOnly")]
    public async Task<IActionResult> Invite(
        [FromBody] InvitePlatformAdminRequest request,
        CancellationToken ct)
    {
        await _service.InviteAdminAsync(request.Email, ct);
        return Accepted();
    }

}

