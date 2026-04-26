using Quanta.Forms.Models;

namespace Quanta.Forms.Abstractions;

public interface INotificationSender
{
    Task SendAsync(FormSubmission submission, DateTimeOffset receivedAt, CancellationToken ct);
}
