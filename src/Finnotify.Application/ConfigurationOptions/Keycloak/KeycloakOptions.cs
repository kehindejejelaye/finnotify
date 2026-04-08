namespace Finnotify.Application.ConfigurationOptions;

public class KeycloakOptions
{
    public const string SECTION = "Keycloak";
    public string BaseUrl { get; set; } = default!;
    public string PlatformRealm { get; set; } = default!;
    public string PlatformClientId { get; set; } = default!;
    public string PlatformClientSecret { get; set; } = default!;
    public string AppRealm { get; set; } = default!;
    public string AppClientId { get; set; } = default!;
    public string AppClientSecret { get; set; } = default!;
}
