using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quanta.Forms.Abstractions;
using Quanta.Forms.Models;

namespace Quanta.Forms.Notifications;

public class ResendNotificationSender : INotificationSender
{
    private const string ResendEndpoint = "https://api.resend.com/emails";

    private readonly HttpClient _http;
    private readonly ResendOptions _options;
    private readonly BrandOptions _brand;
    private readonly ILogger<ResendNotificationSender> _logger;

    public ResendNotificationSender(
        HttpClient http,
        IOptions<ResendOptions> options,
        IOptions<BrandOptions> brand,
        ILogger<ResendNotificationSender> logger)
    {
        _http = http;
        _options = options.Value;
        _brand = brand.Value;
        _logger = logger;
    }

    public async Task SendAsync(FormSubmission submission, DateTimeOffset receivedAt, CancellationToken ct)
    {
        var to = ResolveRecipient(submission.FormId);
        var fromHeader = string.IsNullOrWhiteSpace(_options.FromName)
            ? _options.AlertEmailFrom
            : $"{_options.FromName} <{_options.AlertEmailFrom}>";

        var payload = new
        {
            from = fromHeader,
            to = new[] { to },
            subject = $"New enquiry from {submission.FirstName} {submission.LastName} [{submission.FormId}]",
            html = EmailTemplate.BuildHtml(submission, receivedAt, _brand),
            reply_to = submission.Email,
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, ResendEndpoint)
        {
            Content = JsonContent.Create(payload),
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

        using var response = await _http.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            _logger.LogWarning("Resend returned {Status}: {Body}", response.StatusCode, body);
        }
        else
        {
            _logger.LogInformation("Alert email sent via Resend to {AlertTo} for FormId {FormId}", to, submission.FormId);
        }
    }

    private string ResolveRecipient(string formId)
    {
        if (!string.IsNullOrWhiteSpace(formId)
            && _options.RecipientsByFormId is { Count: > 0 }
            && _options.RecipientsByFormId.TryGetValue(formId, out var mapped)
            && !string.IsNullOrWhiteSpace(mapped))
        {
            return mapped;
        }
        return _options.AlertEmailTo;
    }
}
