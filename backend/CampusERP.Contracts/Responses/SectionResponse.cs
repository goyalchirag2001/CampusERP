public class SectionResponse
{
    public Guid Id { get; set; }

    public Guid SemesterId { get; set; }

    public Guid CampusId { get; set; }

    public string CampusName { get; set; } = string.Empty;

    public Guid CourseId { get; set; }

    public Guid DepartmentId { get; set; }

    public string DepartmentName { get; set; } = string.Empty;

    public string CourseName { get; set; } = string.Empty;

    public string SemesterName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public bool IsActive { get; set; }
}