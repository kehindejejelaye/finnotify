using Microsoft.AspNetCore.Authorization;
using Finnotify.Application.Common;

namespace Finnotify.Api.Authorization;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddPlatformAuthorization(this IServiceCollection services)
    {
        services.RegisterHandlers()
                .RegisterPolicies();
        return services;
    }

    public static IServiceCollection RegisterHandlers(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, PlatformSuperAdminRequirementHandler>();

        return services;
    }

    public static IServiceCollection RegisterPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
           options.AddPolicy(Policies.PlatformSuperAdmin, p =>
           {
               p.RequireAuthenticatedUser();
               p.AddRequirements(new PlatformSuperAdminRequirement());
           }); 
        });

        return services;
    }
}
