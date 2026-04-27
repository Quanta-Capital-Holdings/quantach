namespace Quanta.Forms.Models;

public record FormSubmission(
    string FormId,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string? Company,
    string? Industry,
    string? Message
);
