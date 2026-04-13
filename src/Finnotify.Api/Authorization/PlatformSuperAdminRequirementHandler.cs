using System;
using Microsoft.AspNetCore.Authorization;
using Finnotify.Application.Common;

namespace Finnotify.Api.Authorization;

public class PlatformSuperAdminRequirementHandler 
    : AuthorizationHandler<PlatformSuperAdminRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        PlatformSuperAdminRequirement requirement)
    {
        if (context.User.IsInRole(Roles.PlatformSuperAdmin))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
