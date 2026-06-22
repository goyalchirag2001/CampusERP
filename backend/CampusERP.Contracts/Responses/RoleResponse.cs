namespace CampusERP.Contracts.Responses;

public class RoleResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsSystemRole { get; set; }

    public int PermissionCount { get; set; }

    public List<Guid> PermissionIds { get; set; } = [];

    public List<PermissionResponse> Permissions { get; set; } = [];

    public bool IsActive { get; set; }
}