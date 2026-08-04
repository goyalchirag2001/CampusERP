namespace CampusERP.Contracts.Requests;

public class CreateTeacherAssignmentRequest
{
    public Guid TeacherId { get; set; }

    public Guid SemesterSubjectId { get; set; }

    public Guid SectionId { get; set; }

    public Guid AcademicSessionId { get; set; }
}