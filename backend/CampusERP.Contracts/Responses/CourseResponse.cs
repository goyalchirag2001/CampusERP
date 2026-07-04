namespace CampusERP.Contracts.Responses;

public class CourseResponse
{
    public Guid Id { get; set; }

    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public Guid DepartmentId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string DegreeType { get; set; } = string.Empty;

    public int DurationYears { get; set; }

    public int TotalSemesters { get; set; }

    public string DepartmentName { get; set; } = string.Empty;

    public string CampusName { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public List<SemesterLookupResponse> Semesters { get; set; } = [];
}