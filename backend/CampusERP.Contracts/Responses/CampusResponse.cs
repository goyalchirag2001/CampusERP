namespace CampusERP.Contracts.Responses;

public class CampusResponse
{
    public Guid Id { get; set; }

    public Guid InstitutionId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string? Address { get; set; }

    public bool IsActive { get; set; }
}