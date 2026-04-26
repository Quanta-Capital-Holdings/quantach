using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quanta.Forms.Abstractions;
using Quanta.Forms.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Quanta.Forms.Notifications;

public class SendGridNotificationSender : INotificationSender
{
    private readonly SendGridOptions _options;
    private readonly ILogger<SendGridNotificationSender> _logger;

    public SendGridNotificationSender(IOptions<SendGridOptions> options, ILogger<SendGridNotificationSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendAsync(FormSubmission submission, DateTimeOffset receivedAt, CancellationToken ct)
    {
        var subject = $"New enquiry from {submission.FirstName} {submission.LastName} [{submission.FormId}]";
        var client = new SendGridClient(_options.ApiKey);
        var msg = new SendGridMessage
        {
            From = new EmailAddress(_options.AlertEmailFrom, "Quanta Capital Holdings"),
            Subject = subject,
            HtmlContent = EmailTemplate.BuildHtml(submission, receivedAt)
        };
        msg.AddTo(new EmailAddress(_options.AlertEmailTo));

        var response = await client.SendEmailAsync(msg, ct);

        if ((int)response.StatusCode >= 400)
            _logger.LogWarning("SendGrid returned {Status}", response.StatusCode);
        else
            _logger.LogInformation("Alert email sent to {AlertTo}", _options.AlertEmailTo);
    }
}
