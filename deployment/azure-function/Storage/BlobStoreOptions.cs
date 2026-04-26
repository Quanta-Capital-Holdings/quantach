namespace Quanta.Forms.Storage;

public class BlobStoreOptions
{
    public const string SectionName = "SubmissionStore:Blob";

    public string ConnectionString { get; set; } = "";
    public string ContainerName { get; set; } = "form-submissions";
}
