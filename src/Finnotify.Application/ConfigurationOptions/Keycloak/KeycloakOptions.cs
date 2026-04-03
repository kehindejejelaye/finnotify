namespace Finnotify.Application.ConfigurationOptions;

public class KeycloakOptions
{
    public const string SECTION = "Keycloak";
    public string BaseUrl { get; set; } = default!;
    public string Realm { get; set; } = default!;
    public string ClientId { get; set; } = default!;
    public string ClientSecret { get; set; } = default!;
}