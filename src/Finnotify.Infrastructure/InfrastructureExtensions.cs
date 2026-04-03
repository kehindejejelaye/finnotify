using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Finnotify.Application.ConfigurationOptions;
using Finnotify.Infrastructure.Clients;
using Finnotify.Application.Interfaces;
using Microsoft.Extensions.Options;


namespace Finnotify.Infrastructure;

public static class InfrastructureExtensions
{
    public static ServiceCollection AddInfrastructureServices(this ServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KeycloakOptions>(configuration.GetSection(KeycloakOptions.SECTION));

        services.AddHttpClient<IKeycloakClient, KeycloakClient>((serviceProvider, client) =>
        {
            var options = serviceProvider
                .GetRequiredService<IOptions<KeycloakOptions>>()
                .Value;

            client.BaseAddress = new Uri(options.BaseUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }
}