namespace CampusERP.Contracts.Responses;

public class TeacherResponse
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public Guid DepartmentId { get; set; }

    public string EmployeeCode { get; set; } = string.Empty;

    public string Designation { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; }
}