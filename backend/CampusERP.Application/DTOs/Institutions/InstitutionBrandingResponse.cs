namespace CampusERP.Application.DTOs.Institutions;

public class InstitutionBrandingResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string LoginSlug { get; set; } = string.Empty;

    public string? LogoUrl { get; set; }

    public string? PrimaryColor { get; set; }

    public string? SecondaryColor { get; set; }
}