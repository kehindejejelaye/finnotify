using Microsoft.Extensions.DependencyInjection;
using Finnotify.Application.Services;

namespace Finnotify.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}