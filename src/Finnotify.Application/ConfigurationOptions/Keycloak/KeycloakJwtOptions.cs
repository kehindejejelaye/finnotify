namespace Finnotify.Application.ConfigurationOptions;

public class KeycloakJwtOptions
{
    public const string SECTION = "KeycloakJwtOptions"; // we can still bind from the same section
    public string Authority { get; set; } = default!;
    // Multiple Keycloak realms or IdPs
    public string[] ValidIssuers { get; set; } = Array.Empty<string>();

    // Multiple clients you accept tokens from
    public string[] ValidAudiences { get; set; } = Array.Empty<string>();
}