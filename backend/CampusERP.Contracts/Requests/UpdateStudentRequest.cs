using CampusERP.Shared.Enums;

namespace CampusERP.Contracts.Requests;

public class UpdateStudentRequest
{
    public string AdmissionNumber { get; set; } = string.Empty;

    public string RollNumber { get; set; } = string.Empty;

    public string Batch { get; set; } = string.Empty;

    public DateTime AdmissionDate { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }
}