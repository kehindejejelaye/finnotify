using Finnotify.Domain.Entities;
using Finnotify.Application.Interfaces;

namespace Finnotify.Application.Services;

public sealed class PlatformAdminProvisioningService
    : IPlatformAdminProvisioningService
{
    private readonly IInvitationRepository _repository;
    private readonly IEmailSender _emailSender;

    public PlatformAdminProvisioningService(
        IInvitationRepository repository,
        IEmailSender emailSender)
    {
        _repository = repository;
        _emailSender = emailSender;
    }

    public async Task InviteAdminAsync(
        string email,
        CancellationToken ct)
    {
        var invitation = PlatformAdminInvitation.Create(
            email,
            TimeSpan.FromDays(3));

        await _repository.AddAsync(invitation, ct);

        var inviteUrl =
            $"https://platform.finnotify.com/accept-invite?token={invitation.Id}";

        await _emailSender.SendAsync(
            email,
            subject: "You're invited to be a Finnotify Platform Admin",
            body:
$"""
You have been invited to administer the Finnotify platform.

Accept your invitation:
{inviteUrl}

This invitation expires in 3 days.
"""
        );
    }
}
