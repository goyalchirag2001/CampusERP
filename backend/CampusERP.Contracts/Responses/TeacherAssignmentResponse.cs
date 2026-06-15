namespace CampusERP.Contracts.Responses;

public class TeacherAssignmentResponse
{
    public Guid Id { get; set; }

    public Guid TeacherId { get; set; }

    public Guid SemesterSubjectId { get; set; }

    public string TeacherName { get; set; } = string.Empty;

    public string SubjectName { get; set; } = string.Empty;

    public string SemesterName { get; set; } = string.Empty;
}