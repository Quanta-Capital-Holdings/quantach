using System.Net;
using System.Text.Json;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Quanta.Forms;

public class SubmitFormFunction
{
    private readonly ILogger<SubmitFormFunction> _logger;

    public SubmitFormFunction(ILogger<SubmitFormFunction> logger)
    {
        _logger = logger;
    }

    [Function("SubmitForm")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "submit")] HttpRequestData req)
    {
        // ── CORS preflight ─────────────────────────────────────
        var origin = req.Headers.TryGetValues("Origin", out var origins)
            ? origins.FirstOrDefault() ?? "*"
            : "*";

        if (req.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
        {
            var preflight = req.CreateResponse(HttpStatusCode.NoContent);
            AddCorsHeaders(preflight, origin);
            return preflight;
        }

        // ── Parse & validate body ──────────────────────────────
        FormSubmission? submission;
        try
        {
            submission = await JsonSerializer.DeserializeAsync<FormSubmission>(
                req.Body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed to parse form body: {Message}", ex.Message);
            return ErrorResponse(req, origin, HttpStatusCode.BadRequest, "Invalid request body.");
        }

        if (submission is null)
            return ErrorResponse(req, origin, HttpStatusCode.BadRequest, "Empty submission.");

        // Required fields
        if (string.IsNullOrWhiteSpace(submission.FirstName) ||
            string.IsNullOrWhiteSpace(submission.LastName) ||
            string.IsNullOrWhiteSpace(submission.Email) ||
            string.IsNullOrWhiteSpace(submission.Phone))
        {
            return ErrorResponse(req, origin, HttpStatusCode.UnprocessableEntity,
                "First name, last name, email, and phone are required.");
        }

        // Basic email format check
        if (!submission.Email.Contains('@') || !submission.Email.Contains('.'))
            return ErrorResponse(req, origin, HttpStatusCode.UnprocessableEntity, "Invalid email address.");

        // ── Save to Azure Blob Storage ─────────────────────────
        var timestamp = DateTimeOffset.UtcNow;
        var blobName = $"submissions/{timestamp:yyyy-MM-dd}/{timestamp:HH-mm-ss-fff}_{Sanitize(submission.LastName)}.json";

        var payload = new
        {
            submission.FirstName,
            submission.LastName,
            submission.Email,
            submission.Phone,
            submission.Company,
            submission.Industry,
            submission.Message,
            SubmittedAt = timestamp.ToString("o"),
            SourceIp = req.Headers.TryGetValues("X-Forwarded-For", out var ips)
                ? ips.FirstOrDefault()
                : "unknown"
        };

        try
        {
            var connString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                ?? throw new InvalidOperationException("AzureWebJobsStorage not configured.");
            var containerName = Environment.GetEnvironmentVariable("BlobContainerName") ?? "form-submissions";

            var blobServiceClient = new BlobServiceClient(connString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(blobName);
            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            await blobClient.UploadAsync(stream, overwrite: true);

            _logger.LogInformation("Saved submission to blob: {BlobName}", blobName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save submission to blob storage.");
            return ErrorResponse(req, origin, HttpStatusCode.InternalServerError,
                "We couldn't save your submission. Please try again or email us directly.");
        }

        // ── Send email alert via SendGrid ──────────────────────
        var sendGridKey = Environment.GetEnvironmentVariable("SendGridApiKey");
        var alertTo = Environment.GetEnvironmentVariable("AlertEmailTo") ?? "info@quantach.com";
        var alertFrom = Environment.GetEnvironmentVariable("AlertEmailFrom") ?? "noreply@quantach.com";

        if (!string.IsNullOrWhiteSpace(sendGridKey))
        {
            try
            {
                var sgClient = new SendGridClient(sendGridKey);
                var msg = new SendGridMessage
                {
                    From = new EmailAddress(alertFrom, "Quanta Capital Holdings"),
                    Subject = $"New enquiry from {submission.FirstName} {submission.LastName}",
                    HtmlContent = BuildEmailHtml(submission, timestamp)
                };
                msg.AddTo(new EmailAddress(alertTo));
                var sgResponse = await sgClient.SendEmailAsync(msg);

                if ((int)sgResponse.StatusCode >= 400)
                    _logger.LogWarning("SendGrid returned {Status}", sgResponse.StatusCode);
                else
                    _logger.LogInformation("Alert email sent to {AlertTo}", alertTo);
            }
            catch (Exception ex)
            {
                // Non-fatal — submission is already saved
                _logger.LogError(ex, "Failed to send SendGrid alert (submission was saved).");
            }
        }
        else
        {
            _logger.LogWarning("SendGridApiKey not set — skipping email alert.");
        }

        // ── Success ────────────────────────────────────────────
        var response = req.CreateResponse(HttpStatusCode.OK);
        AddCorsHeaders(response, origin);
        response.Headers.Add("Content-Type", "application/json");
        await response.WriteStringAsync(JsonSerializer.Serialize(new { success = true, message = "Thank you — we'll be in touch within 24 hours." }));
        return response;
    }

    // ── Helpers ───────────────────────────────────────────────

    private static HttpResponseData ErrorResponse(HttpRequestData req, string origin, HttpStatusCode status, string message)
    {
        var res = req.CreateResponse(status);
        AddCorsHeaders(res, origin);
        res.Headers.Add("Content-Type", "application/json");
        res.WriteString(JsonSerializer.Serialize(new { success = false, message }));
        return res;
    }

    private static void AddCorsHeaders(HttpResponseData response, string origin)
    {
        // Allow only your GitHub Pages domain in production — set ALLOWED_ORIGIN env var
        var allowed = Environment.GetEnvironmentVariable("AllowedOrigin") ?? "*";
        response.Headers.Add("Access-Control-Allow-Origin", allowed);
        response.Headers.Add("Access-Control-Allow-Methods", "POST, OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
    }

    private static string Sanitize(string input) =>
        new string(input.Where(c => char.IsLetterOrDigit(c) || c == '-').Take(30).ToArray());

    private static string BuildEmailHtml(FormSubmission s, DateTimeOffset ts) => $"""
        <!DOCTYPE html>
        <html>
        <body style="font-family:'Helvetica Neue',Arial,sans-serif;max-width:560px;margin:0 auto;color:#1A1612;">
          <div style="background:#0B1F3A;padding:24px 32px;">
            <span style="font-size:20px;font-weight:700;letter-spacing:0.1em;color:#fff;">QUANTA</span>
            <div style="height:2px;width:80px;background:#C4922A;margin:4px 0 2px;"></div>
            <span style="font-size:9px;letter-spacing:0.4em;color:rgba(255,255,255,0.4);text-transform:uppercase;">Capital Holdings</span>
          </div>
          <div style="padding:28px 32px;background:#F7F5F0;border:1px solid #E8E4DC;">
            <p style="font-size:13px;color:#8A8478;margin:0 0 20px;">New enquiry received · {ts:ddd, MMM d yyyy 'at' h:mm tt} UTC</p>
            <table style="width:100%;border-collapse:collapse;">
              <tr><td style="padding:10px 0;border-bottom:1px solid #E8E4DC;font-size:12px;font-weight:600;color:#8A8478;width:140px;">Name</td>
                  <td style="padding:10px 0;border-bottom:1px solid #E8E4DC;font-size:14px;">{s.FirstName} {s.LastName}</td></tr>
              <tr><td style="padding:10px 0;border-bottom:1px solid #E8E4DC;font-size:12px;font-weight:600;color:#8A8478;">Email</td>
                  <td style="padding:10px 0;border-bottom:1px solid #E8E4DC;font-size:14px;"><a href="mailto:{s.Email}" style="color:#0B1F3A;">{s.Email}</a></td></tr>
              <tr><td style="padding:10px 0;border-bottom:1px solid #E8E4DC;font-size:12px;font-weight:600;color:#8A8478;">Phone</td>
                  <td style="padding:10px 0;border-bottom:1px solid #E8E4DC;font-size:14px;"><a href="tel:{s.Phone}" style="color:#0B1F3A;">{s.Phone}</a></td></tr>
              {(string.IsNullOrWhiteSpace(s.Company) ? "" : $"<tr><td style=\"padding:10px 0;border-bottom:1px solid #E8E4DC;font-size:12px;font-weight:600;color:#8A8478;\">Company</td><td style=\"padding:10px 0;border-bottom:1px solid #E8E4DC;font-size:14px;\">{s.Company}</td></tr>")}
              {(string.IsNullOrWhiteSpace(s.Industry) ? "" : $"<tr><td style=\"padding:10px 0;border-bottom:1px solid #E8E4DC;font-size:12px;font-weight:600;color:#8A8478;\">Industry</td><td style=\"padding:10px 0;border-bottom:1px solid #E8E4DC;font-size:14px;\">{s.Industry}</td></tr>")}
            </table>
            {(string.IsNullOrWhiteSpace(s.Message) ? "" : $"<div style=\"margin-top:20px;padding:16px;background:white;border:1px solid #E8E4DC;border-radius:4px;\"><p style=\"font-size:12px;font-weight:600;color:#8A8478;margin:0 0 8px;letter-spacing:0.06em;text-transform:uppercase;\">Message</p><p style=\"font-size:14px;line-height:1.7;margin:0;\">{s.Message}</p></div>")}
            <div style="margin-top:24px;text-align:center;">
              <a href="mailto:{s.Email}?subject=Re: Your enquiry to Quanta Capital Holdings" style="display:inline-block;background:#C4922A;color:white;padding:12px 28px;border-radius:4px;font-size:14px;font-weight:600;text-decoration:none;">Reply to {s.FirstName}</a>
            </div>
          </div>
          <div style="padding:16px 32px;background:#071529;text-align:center;">
            <p style="font-size:11px;color:rgba(255,255,255,0.3);margin:0;">Quanta Capital Holdings Inc. · Toronto, Ontario, Canada</p>
          </div>
        </body>
        </html>
        """;
}
