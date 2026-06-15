namespace CampusERP.Contracts.Requests;

public class AssignSubjectToSemesterRequest
{
    public Guid SemesterId { get; set; }

    public Guid SubjectId { get; set; }
}