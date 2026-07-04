public class CreateAcademicSessionRequest
{
    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool IsCurrent { get; set; }
}