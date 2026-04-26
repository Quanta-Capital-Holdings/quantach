using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quanta.Forms.Abstractions;
using Quanta.Forms.Http;
using Quanta.Forms.Models;

namespace Quanta.Forms.Functions;

public class SubmitFormFunction
{
    private readonly ISubmissionValidator _validator;
    private readonly ISubmissionStore _store;
    private readonly INotificationSender _notifier;
    private readonly CorsOptions _cors;
    private readonly ILogger<SubmitFormFunction> _logger;

    public SubmitFormFunction(
        ISubmissionValidator validator,
        ISubmissionStore store,
        INotificationSender notifier,
        IOptions<CorsOptions> cors,
        ILogger<SubmitFormFunction> logger)
    {
        _validator = validator;
        _store = store;
        _notifier = notifier;
        _cors = cors.Value;
        _logger = logger;
    }

    [Function("SubmitForm")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "submit")] HttpRequestData req,
        CancellationToken ct)
    {
        var origin = _cors.AllowedOrigin;

        if (req.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
        {
            var preflight = req.CreateResponse(HttpStatusCode.NoContent);
            CorsHelper.AddCorsHeaders(preflight, origin);
            return preflight;
        }

        FormSubmission? submission;
        try
        {
            submission = await JsonSerializer.DeserializeAsync<FormSubmission>(
                req.Body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                cancellationToken: ct
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed to parse form body: {Message}", ex.Message);
            return CorsHelper.ErrorResponse(req, origin, HttpStatusCode.BadRequest, "Invalid request body.");
        }

        if (submission is null)
            return CorsHelper.ErrorResponse(req, origin, HttpStatusCode.BadRequest, "Empty submission.");

        var validation = _validator.Validate(submission);
        if (!validation.IsValid)
            return CorsHelper.ErrorResponse(req, origin, HttpStatusCode.UnprocessableEntity, validation.Error!);

        var receivedAt = DateTimeOffset.UtcNow;

        try
        {
            await _store.SaveAsync(submission, receivedAt, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save submission.");
            return CorsHelper.ErrorResponse(req, origin, HttpStatusCode.InternalServerError,
                "We couldn't save your submission. Please try again or email us directly.");
        }

        try
        {
            await _notifier.SendAsync(submission, receivedAt, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Notification failed (submission was saved).");
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        CorsHelper.AddCorsHeaders(response, origin);
        response.Headers.Add("Content-Type", "application/json");
        await response.WriteStringAsync(JsonSerializer.Serialize(new { success = true, message = "Thank you — we'll be in touch within 24 hours." }), ct);
        return response;
    }
}
