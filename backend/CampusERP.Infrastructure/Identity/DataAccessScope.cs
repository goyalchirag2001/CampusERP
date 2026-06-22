using CampusERP.Application.Interfaces;
using CampusERP.Shared.Constants;

namespace CampusERP.Infrastructure.Identity;

public class DataAccessScope : IDataAccessScope
{
    private readonly ICurrentUserService _currentUserService;

    public DataAccessScope(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public bool IsSuperAdmin()
    {
        return _currentUserService.Roles.Contains(RoleConstants.SuperAdmin);
    }

    public bool IsPlatformAdmin()
    {
        return _currentUserService.Roles.Contains(RoleConstants.PlatformAdmin);
    }

    public bool IsInstitutionAdmin()
    {
        return _currentUserService.Roles.Contains(RoleConstants.InstitutionAdmin);
    }

    public bool IsCampusAdmin()
    {
        return _currentUserService.Roles.Contains(RoleConstants.CampusAdmin);
    }

    public Guid UserId()
    {
        return _currentUserService.UserId ?? throw new Exception("User not found.");
    }

    public Guid InstitutionId()
    {
        return _currentUserService.InstitutionId ?? throw new Exception("Institution not found.");
    }

    public Guid CampusId()
    {
        return _currentUserService.CampusId ?? throw new Exception("Campus not found.");
    }
}