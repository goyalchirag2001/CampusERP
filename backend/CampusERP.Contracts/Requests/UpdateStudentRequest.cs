namespace CampusERP.Contracts.Requests;

public class UpdateStudentRequest
{
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