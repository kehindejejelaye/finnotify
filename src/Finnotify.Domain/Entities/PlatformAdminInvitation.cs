using Finnotify.Domain.Helpers;

namespace Finnotify.Domain.Entities;

public sealed class PlatformAdminInvitation : BaseEntity
{
    
    public string Email { get; private set; } = default!;
    public string Role { get; private set; } = "platform_admin";
    public string TokenHash { get; private set; } = default!;
    public DateTime ExpiresAtUtc { get; private set; }
    public bool Used { get; private set; }

    public static PlatformAdminInvitation Create(string email, TimeSpan ttl)
    {
        var token = Crypto.GenerateSecureToken(); // 32–64 bytes
        return new PlatformAdminInvitation
        {
            Id = Guid.NewGuid(),
            Email = email,
            TokenHash = Crypto.Hash(token),
            ExpiresAtUtc = DateTime.UtcNow.Add(ttl),
            Used = false
        };
    }

    public void MarkUsed()
    {
        Used = true;
    }
}
