namespace CampusERP.Contracts.Requests;

public class CreateTeacherRequest
{
    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public Guid DepartmentId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string EmployeeCode { get; set; } = string.Empty;

    public string Designation { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;
}