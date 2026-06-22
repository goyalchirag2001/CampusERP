namespace CampusERP.Contracts.Requests;

public class UpdateUserRequest
{
    public Guid CampusId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public List<Guid> RoleIds { get; set; } = [];
}