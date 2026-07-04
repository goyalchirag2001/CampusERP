namespace CampusERP.Contracts.Responses;

public class AcademicSessionLookup
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsCurrent { get; set; }
}