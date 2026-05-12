namespace Quanta.Forms.Notifications;

public class ResendOptions
{
    public const string SectionName = "Resend";

    public string ApiKey { get; set; } = "";
    public string AlertEmailFrom { get; set; } = "noreply@quantach.com";
    public string FromName { get; set; } = "Quanta Capital Holdings";
    public string AlertEmailTo { get; set; } = "info@quantach.com";

    /// <summary>
    /// Optional per-FormId recipient overrides. When a submission's FormId matches a key here,
    /// the email is sent to the mapped address instead of <see cref="AlertEmailTo"/>.
    /// Configure via "Resend:RecipientsByFormId:&lt;formId&gt;=email@example.com".
    /// </summary>
    public Dictionary<string, string> RecipientsByFormId { get; set; } = new();
}
