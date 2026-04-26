using Quanta.Forms.Models;

namespace Quanta.Forms.Abstractions;

public interface ISubmissionStore
{
    Task<string> SaveAsync(FormSubmission submission, DateTimeOffset receivedAt, CancellationToken ct);
}
