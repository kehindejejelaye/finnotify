namespace Finnotify.Application.ConfigurationOptions;

public class KeycloakUserCreateRequest
{
    public string Email { get; set; } = default!;
    public bool Enabled { get; set; } = true;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string TemporaryPassword { get; set; } = default!;
}