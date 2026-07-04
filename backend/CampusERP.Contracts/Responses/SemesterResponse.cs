namespace CampusERP.Contracts.Responses;

public class SemesterResponse
{
    public Guid Id { get; set; }

    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public Guid CourseId { get; set; }

    public string Name { get; set; } = string.Empty;

    public int SequenceNumber { get; set; }

    public bool IsActive { get; set; }
}