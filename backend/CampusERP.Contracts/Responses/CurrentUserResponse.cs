namespace CampusERP.Contracts.Responses;

public class CurrentUserResponse
{
    public Guid UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public string? InstitutionSlug { get; set; }

    public List<string> Roles { get; set; } = [];
}