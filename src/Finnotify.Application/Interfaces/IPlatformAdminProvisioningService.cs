using System;

namespace Finnotify.Application.Interfaces;

public interface IPlatformAdminProvisioningService
{    
    Task InviteAdminAsync(string email, CancellationToken ct);
}
