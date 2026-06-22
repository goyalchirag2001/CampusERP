namespace CampusERP.Application.Interfaces;

public interface IDataAccessScope
{
    bool IsSuperAdmin();

    bool IsPlatformAdmin();

    bool IsInstitutionAdmin();

    bool IsCampusAdmin();

    Guid UserId();

    Guid InstitutionId();

    Guid CampusId();
}