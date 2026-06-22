namespace CampusERP.Contracts.Requests;

public class UpdateRoleRequest
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public List<Guid> PermissionIds { get; set; } = [];
}