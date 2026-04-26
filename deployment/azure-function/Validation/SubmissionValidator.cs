using Quanta.Forms.Abstractions;
using Quanta.Forms.Models;

namespace Quanta.Forms.Validation;

public class SubmissionValidator : ISubmissionValidator
{
    public ValidationResult Validate(FormSubmission s)
    {
        if (string.IsNullOrWhiteSpace(s.FormId))
            return ValidationResult.Fail("formId is required.");

        if (string.IsNullOrWhiteSpace(s.FirstName) ||
            string.IsNullOrWhiteSpace(s.LastName) ||
            string.IsNullOrWhiteSpace(s.Email) ||
            string.IsNullOrWhiteSpace(s.Phone))
        {
            return ValidationResult.Fail("First name, last name, email, and phone are required.");
        }

        if (!s.Email.Contains('@') || !s.Email.Contains('.'))
            return ValidationResult.Fail("Invalid email address.");

        return ValidationResult.Ok();
    }
}
