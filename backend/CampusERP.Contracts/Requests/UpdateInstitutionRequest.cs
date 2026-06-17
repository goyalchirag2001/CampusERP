namespace CampusERP.Contracts.Requests;

public class UpdateInstitutionRequest
{
    public string Name { get; set; } = string.Empty;

    public string LoginSlug { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Website { get; set; }

    public string? Address { get; set; }

    public string? LogoUrl { get; set; }

    public string? PrimaryColor { get; set; }

    public string? SecondaryColor { get; set; }

    public bool IsActive { get; set; }
}