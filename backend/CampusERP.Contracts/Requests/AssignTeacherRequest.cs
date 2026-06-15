namespace CampusERP.Contracts.Requests;

public class AssignTeacherRequest
{
    public Guid TeacherId { get; set; }

    public Guid SemesterSubjectId { get; set; }
}