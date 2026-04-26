using Microsoft.Extensions.Logging;
using Quanta.Forms.Abstractions;
using Quanta.Forms.Models;

namespace Quanta.Forms.Notifications;

public class NullNotificationSender : INotificationSender
{
    private readonly ILogger<NullNotificationSender> _logger;

    public NullNotificationSender(ILogger<NullNotificationSender> logger) => _logger = logger;

    public Task SendAsync(FormSubmission submission, DateTimeOffset receivedAt, CancellationToken ct)
    {
        _logger.LogWarning("Notification sender is disabled (no SendGrid:ApiKey configured) — skipping alert.");
        return Task.CompletedTask;
    }
}
