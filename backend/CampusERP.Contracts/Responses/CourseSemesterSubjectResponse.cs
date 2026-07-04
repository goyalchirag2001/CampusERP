namespace CampusERP.Contracts.Responses;

public class CourseSemesterSubjectResponse
{
    public Guid SemesterId { get; set; }

    public string SemesterName { get; set; } = string.Empty;

    public int SequenceNumber { get; set; }

    public List<SemesterSubjectResponse> Subjects { get; set; } = [];
}