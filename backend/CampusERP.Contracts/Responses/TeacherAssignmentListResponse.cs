namespace CampusERP.Contracts.Responses;

public class TeacherAssignmentListResponse
{
    public Guid Id { get; set; }

    public Guid TeacherId { get; set; }

    public string TeacherName { get; set; } = string.Empty;

    public Guid AcademicSessionId { get; set; }

    public string AcademicSessionName { get; set; } = string.Empty;

    public Guid SectionId { get; set; }

    public string SectionName { get; set; } = string.Empty;

    public Guid SemesterSubjectId { get; set; }

    public string SubjectName { get; set; } = string.Empty;

    public string SemesterName { get; set; } = string.Empty;

    public string CourseName { get; set; } = string.Empty;

    public string ClassDisplayName { get; set; } = string.Empty;
}