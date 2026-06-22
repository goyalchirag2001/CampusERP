namespace CampusERP.Contracts.Responses;

public class UserResponse
{
    public Guid Id { get; set; }

    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public string InstitutionName { get; set; } = string.Empty;

    public string CampusName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public List<Guid> RoleIds { get; set; } = [];

    public List<string> Roles { get; set; } = [];

    public string? TemporaryPassword { get; set; }

    public bool IsActive { get; set; }
}