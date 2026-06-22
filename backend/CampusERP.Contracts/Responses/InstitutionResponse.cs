namespace CampusERP.Contracts.Responses;

public class InstitutionResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string LoginSlug { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Website { get; set; }

    public string? Address { get; set; }

    public string? LogoUrl { get; set; }

    public string? PrimaryColor { get; set; }

    public string? SecondaryColor { get; set; }

    public int CampusCount { get; set; }

    public int StudentCount { get; set; }

    public int TeacherCount { get; set; }

    public string AdminFirstName { get; set; } = string.Empty;

    public string AdminLastName { get; set; } = string.Empty;

    public string AdminEmail { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}