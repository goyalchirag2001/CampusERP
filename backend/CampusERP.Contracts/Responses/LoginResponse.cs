namespace CampusERP.Contracts.Responses;

public class LoginResponse
{
    public Guid UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? InstitutionSlug { get; set; }

    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public DateTime? CurrentLoginAt { get; set; }
}