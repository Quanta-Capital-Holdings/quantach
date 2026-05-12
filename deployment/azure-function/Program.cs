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

        var storeProvider = config["SubmissionStore:Provider"] ?? "Blob";
        switch (storeProvider)
        {
            case "Blob":
                services.AddSingleton<ISubmissionStore, BlobSubmissionStore>();
                break;
            default:
                throw new InvalidOperationException($"Unknown SubmissionStore:Provider '{storeProvider}'.");
        }

        // Brand (email template parameters — per-deployment, site-agnostic)
        services.Configure<BrandOptions>(config.GetSection(BrandOptions.SectionName));

        // SendGrid options (kept for backwards compat)
        services.Configure<SendGridOptions>(opts =>
        {
            opts.ApiKey = config["SendGrid:ApiKey"] ?? config["SendGridApiKey"] ?? "";
            opts.AlertEmailTo = config["SendGrid:AlertEmailTo"] ?? config["AlertEmailTo"] ?? "info@quantach.com";
            opts.AlertEmailFrom = config["SendGrid:AlertEmailFrom"] ?? config["AlertEmailFrom"] ?? "noreply@quantach.com";
        });

        // Resend options (binds Resend:RecipientsByFormId dictionary too)
        services.Configure<ResendOptions>(config.GetSection(ResendOptions.SectionName));

        // Notification provider selection.
        // Explicit "Notifications:Provider" wins. Otherwise auto-detect by which API key is set.
        var notifProvider = config["Notifications:Provider"];
        if (string.IsNullOrWhiteSpace(notifProvider))
        {
            if (!string.IsNullOrWhiteSpace(config["Resend:ApiKey"]))
                notifProvider = "Resend";
            else if (!string.IsNullOrWhiteSpace(config["SendGrid:ApiKey"]) || !string.IsNullOrWhiteSpace(config["SendGridApiKey"]))
                notifProvider = "SendGrid";
            else
                notifProvider = "None";
        }

        switch (notifProvider)
        {
            case "Resend":
                services.AddHttpClient<ResendNotificationSender>();
                services.AddTransient<INotificationSender>(sp => sp.GetRequiredService<ResendNotificationSender>());
                break;
            case "SendGrid":
                services.AddSingleton<INotificationSender, SendGridNotificationSender>();
                break;
            case "None":
                services.AddSingleton<INotificationSender, NullNotificationSender>();
                break;
            default:
                throw new InvalidOperationException($"Unknown Notifications:Provider '{notifProvider}'.");
        }
    })
    .Build();

host.Run();
