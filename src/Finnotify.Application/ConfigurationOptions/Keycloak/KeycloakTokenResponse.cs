namespace Finnotify.Application.ConfigurationOptions;

public class KeycloakTokenResponse
{
    public string AccessToken { get; set; } = default!;
    public int ExpiresIn { get; set; }
    public string TokenType { get; set; } = default!;
}