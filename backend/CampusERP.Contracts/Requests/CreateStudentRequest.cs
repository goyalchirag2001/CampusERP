using CampusERP.Contracts.Enums;

namespace CampusERP.Contracts.Requests;

public class CreateStudentRequest
{
    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid CourseId { get; set; }

    public Guid SemesterId { get; set; }

    public Guid SectionId { get; set; }

    public string AdmissionNumber { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string Password { get; set; } = string.Empty;

    public string RollNumber { get; set; } = string.Empty;

    public string Batch { get; set; } = string.Empty;

    public DateTime AdmissionDate { get; set; }
}