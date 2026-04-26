namespace Quanta.Forms.Notifications;

public class SendGridOptions
{
    public const string SectionName = "SendGrid";

    public string ApiKey { get; set; } = "";
    public string AlertEmailTo { get; set; } = "info@quantach.com";
    public string AlertEmailFrom { get; set; } = "noreply@quantach.com";
}
