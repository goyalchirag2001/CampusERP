namespace CampusERP.Contracts.Responses;

public class PermissionResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Module { get; set; } = string.Empty;
}