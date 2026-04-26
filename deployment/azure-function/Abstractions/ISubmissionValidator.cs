using Quanta.Forms.Models;

namespace Quanta.Forms.Abstractions;

public readonly record struct ValidationResult(bool IsValid, string? Error)
{
    public static ValidationResult Ok() => new(true, null);
    public static ValidationResult Fail(string error) => new(false, error);
}

public interface ISubmissionValidator
{
    ValidationResult Validate(FormSubmission submission);
}
