namespace CampusERP.Contracts.Requests;

public class UpdateCampusRequest
{
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? CampusHeadName { get; set; }
}