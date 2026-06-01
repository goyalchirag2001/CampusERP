namespace CampusERP.Application.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }

    Guid? InstitutionId { get; }

    string? Email { get; }
}