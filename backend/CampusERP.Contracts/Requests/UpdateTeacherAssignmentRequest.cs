namespace CampusERP.Contracts.Requests;

public class UpdateTeacherAssignmentRequest
{
    public Guid TeacherId { get; set; }

    public Guid SemesterSubjectId { get; set; }

    public Guid SectionId { get; set; }

    public Guid AcademicSessionId { get; set; }
}