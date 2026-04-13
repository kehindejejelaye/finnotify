namespace Finnotify.Application;

public record LoginResponse
{
    public string AccessToken { get; set; } = default!;
    public int ExpiresIn { get; set; }
}