namespace Quanta.Forms.Http;

public class CorsOptions
{
    public const string SectionName = "Cors";

    public string AllowedOrigin { get; set; } = "*";
}
