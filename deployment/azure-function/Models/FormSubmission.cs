namespace Quanta.Forms.Models;

public record FormSubmission(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string? Company,
    string? Industry,
    string? Message
);
