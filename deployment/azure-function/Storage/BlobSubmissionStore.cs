using System.Text;
using System.Text.Json;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quanta.Forms.Abstractions;
using Quanta.Forms.Models;

namespace Quanta.Forms.Storage;

public class BlobSubmissionStore : ISubmissionStore
{
    private readonly BlobContainerClient _container;
    private readonly ILogger<BlobSubmissionStore> _logger;

    public BlobSubmissionStore(IOptions<BlobStoreOptions> options, ILogger<BlobSubmissionStore> logger)
    {
        var opts = options.Value;
        if (string.IsNullOrWhiteSpace(opts.ConnectionString))
            throw new InvalidOperationException("BlobStoreOptions.ConnectionString is not configured.");

        var service = new BlobServiceClient(opts.ConnectionString);
        _container = service.GetBlobContainerClient(opts.ContainerName);
        _logger = logger;
    }

    public async Task<string> SaveAsync(FormSubmission submission, DateTimeOffset receivedAt, CancellationToken ct)
    {
        await _container.CreateIfNotExistsAsync(cancellationToken: ct);

        var blobName = $"submissions/{receivedAt:yyyy-MM-dd}/{receivedAt:HH-mm-ss-fff}_{Sanitize(submission.LastName)}.json";

        var payload = new
        {
            submission.FirstName,
            submission.LastName,
            submission.Email,
            submission.Phone,
            submission.Company,
            submission.Industry,
            submission.Message,
            SubmittedAt = receivedAt.ToString("o")
        };

        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        await _container.GetBlobClient(blobName).UploadAsync(stream, overwrite: true, cancellationToken: ct);

        _logger.LogInformation("Saved submission to blob: {BlobName}", blobName);
        return blobName;
    }

    private static string Sanitize(string input) =>
        new(input.Where(c => char.IsLetterOrDigit(c) || c == '-').Take(30).ToArray());
}
