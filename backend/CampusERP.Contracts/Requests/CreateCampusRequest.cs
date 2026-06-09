namespace CampusERP.Contracts.Requests;

public class CreateCampusRequest
{
    public Guid InstitutionId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string? Address { get; set; }
}