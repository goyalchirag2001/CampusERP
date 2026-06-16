namespace CampusERP.Contracts.Responses;

public class StudentResponse
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid CourseId { get; set; }

    public string RollNumber { get; set; } = string.Empty;

    public string Batch { get; set; } = string.Empty;

    public DateTime AdmissionDate { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }
}