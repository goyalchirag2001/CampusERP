using CampusERP.Application.Authorization;
using CampusERP.Application.Interfaces;
using CampusERP.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Authorization;

public class PermissionAuthorizationHandler: AuthorizationHandler<PermissionRequirement>
{
    private readonly ApplicationDbContext _dbContext;

    private readonly ICurrentUserService _currentUserService;

    public PermissionAuthorizationHandler(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;

        _currentUserService = currentUserService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (_currentUserService.UserId is null)
        {
            return;
        }

        var permissions =
            await _dbContext.UserRoles
                .Where(x =>
                    x.UserId ==
                    _currentUserService.UserId)
                .SelectMany(x =>
                    x.Role.RolePermissions)
                .Select(x =>
                    x.Permission.Code)
                .Distinct()
                .ToListAsync();

        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}