namespace CampusERP.Application.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }

    Guid? InstitutionId { get; }

    Guid? CampusId { get; }

    string? Email { get; }
}