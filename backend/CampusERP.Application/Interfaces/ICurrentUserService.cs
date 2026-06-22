namespace CampusERP.Application.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }

    Guid? InstitutionId { get; }

    Guid? CampusId { get; }

    string? Email { get; }

    string? InstitutionSlug { get; }

    List<string> Roles { get; }
}