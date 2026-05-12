namespace Quanta.Forms.Notifications;

public class BrandOptions
{
    public const string SectionName = "Brand";

    public string Name { get; set; } = "QUANTA";
    public string Tagline { get; set; } = "Capital Holdings";
    public string FooterText { get; set; } = "Quanta Capital Holdings Inc. · Toronto, Ontario, Canada";
    public string ReplySubjectPrefix { get; set; } = "Re: Your enquiry to Quanta Capital Holdings";

    public string HeaderBackground { get; set; } = "#0B1F3A";
    public string FooterBackground { get; set; } = "#071529";
    public string AccentColor { get; set; } = "#C4922A";
    public string BodyBackground { get; set; } = "#F7F5F0";
    public string BorderColor { get; set; } = "#E8E4DC";
    public string TextColor { get; set; } = "#1A1612";
    public string MutedColor { get; set; } = "#8A8478";
}
