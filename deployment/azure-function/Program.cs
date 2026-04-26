using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quanta.Forms.Abstractions;
using Quanta.Forms.Http;
using Quanta.Forms.Notifications;
using Quanta.Forms.Storage;
using Quanta.Forms.Validation;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        var config = context.Configuration;

        // CORS
        services.Configure<CorsOptions>(opts =>
        {
            opts.AllowedOrigin = config["Cors:AllowedOrigin"]
                ?? config["AllowedOrigin"]
                ?? "*";
        });

        // Validator
        services.AddSingleton<ISubmissionValidator, SubmissionValidator>();

        // Submission store — provider-selected
        services.Configure<BlobStoreOptions>(opts =>
        {
            opts.ConnectionString = config["SubmissionStore:Blob:ConnectionString"]
                ?? config["AzureWebJobsStorage"]
                ?? "";
            opts.ContainerName = config["SubmissionStore:Blob:ContainerName"]
                ?? config["BlobContainerName"]
                ?? "form-submissions";
        });

        var provider = config["SubmissionStore:Provider"] ?? "Blob";
        switch (provider)
        {
            case "Blob":
                services.AddSingleton<ISubmissionStore, BlobSubmissionStore>();
                break;
            default:
                throw new InvalidOperationException($"Unknown SubmissionStore:Provider '{provider}'.");
        }

        // Notifications — fall back to Null sender when no API key configured
        services.Configure<SendGridOptions>(opts =>
        {
            opts.ApiKey = config["SendGrid:ApiKey"] ?? config["SendGridApiKey"] ?? "";
            opts.AlertEmailTo = config["SendGrid:AlertEmailTo"] ?? config["AlertEmailTo"] ?? "info@quantach.com";
            opts.AlertEmailFrom = config["SendGrid:AlertEmailFrom"] ?? config["AlertEmailFrom"] ?? "noreply@quantach.com";
        });

        var sendGridKey = config["SendGrid:ApiKey"] ?? config["SendGridApiKey"];
        if (!string.IsNullOrWhiteSpace(sendGridKey))
            services.AddSingleton<INotificationSender, SendGridNotificationSender>();
        else
            services.AddSingleton<INotificationSender, NullNotificationSender>();
    })
    .Build();

host.Run();
