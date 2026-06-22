namespace CampusERP.Contracts.Responses;

public class CampusResponse
{
    public Guid Id { get; set; }

    public Guid InstitutionId { get; set; }

    public string InstitutionName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? CampusHeadName { get; set; }

    public int DepartmentCount { get; set; }

    public int TeacherCount { get; set; }

    public int StudentCount { get; set; }

    public bool IsActive { get; set; }
}