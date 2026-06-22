namespace CampusERP.Contracts.Responses;

public class DashboardResponse
{
    public int InstitutionCount { get; set; }

    public int CampusCount { get; set; }

    public int UserCount { get; set; }

    public int StudentCount { get; set; }

    public int TeacherCount { get; set; }

    public int DepartmentCount { get; set; }

    public int CourseCount { get; set; }

    public int SubjectCount { get; set; }

    public bool IsPlatformDashboard { get; set; }
}