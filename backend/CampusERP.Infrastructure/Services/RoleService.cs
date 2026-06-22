using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class RoleService : IRoleService
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IDataAccessScope _scope;

    public RoleService(ApplicationDbContext dbContext, IDataAccessScope scope)
    {
        _dbContext = dbContext;
        _scope = scope;
    }
    private static void ValidateSystemRole(Role role)
    {
        if (role.IsSystemRole)
        {
            throw new Exception("System roles cannot be modified.");
        }
    }

    public async Task<RoleResponse> CreateAsync(CreateRoleRequest request)
    {
        var exists = await _dbContext.Roles.AnyAsync(x => x.Name == request.Name);

        if (exists)
        {
            throw new Exception("Role already exists.");
        }

        var permissionIds =
            request.PermissionIds
                .Distinct()
                .ToList();

        var permissionCount =
            await _dbContext.Permissions
                .CountAsync(x =>
                    permissionIds.Contains(x.Id));

        if (permissionCount != permissionIds.Count)
        {
            throw new Exception("One or more permissions are invalid.");
        }

        var role = new Role
        {
            Id = Guid.NewGuid(),

            Name = request.Name,

            Description = request.Description,

            IsSystemRole = false,

            IsActive = true
        };

        _dbContext.Roles.Add(role);

        foreach (var permissionId in permissionIds)
        {
            _dbContext.RolePermissions.Add(
                new RolePermission
                {
                    Id = Guid.NewGuid(),

                    RoleId = role.Id,

                    PermissionId = permissionId
                });
        }

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(role.Id) ?? throw new Exception();
    }

    public async Task<List<RoleResponse>> GetAllAsync()
    {
        return await _dbContext.Roles
            .Include(x => x.RolePermissions)
                .ThenInclude(x => x.Permission)
            .Select(x => new RoleResponse
            {
                Id = x.Id,

                Name = x.Name,

                Description = x.Description,

                IsSystemRole = x.IsSystemRole,

                PermissionCount = x.RolePermissions.Count,

                PermissionIds = x.RolePermissions.Select(p => p.PermissionId).ToList(),

                IsActive = x.IsActive,

                Permissions = x.RolePermissions
                                .Select(p => new PermissionResponse
                                {
                                    Id = p.Permission.Id,

                                    Code = p.Permission.Code,

                                    Name = p.Permission.Name,

                                    Module = p.Permission.Module
                                })
                                .ToList()}).ToListAsync();

    }

    public async Task<RoleResponse?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Roles
            .Where(x => x.Id == id)
            .Select(x => new RoleResponse
            {
                Id = x.Id,

                Name = x.Name,

                Description = x.Description,

                IsSystemRole = x.IsSystemRole,

                PermissionCount = x.RolePermissions.Count,

                PermissionIds = x.RolePermissions
                    .Select(p => p.PermissionId)
                    .ToList(),

                Permissions = x.RolePermissions
                    .Select(p => new PermissionResponse
                    {
                        Id = p.Permission.Id,

                        Code = p.Permission.Code,

                        Name = p.Permission.Name,

                        Module = p.Permission.Module
                    })
                    .ToList(),

                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task<RoleResponse> UpdateAsync(Guid id, UpdateRoleRequest request)
    {
        var role = await _dbContext.Roles.Include(x => x.RolePermissions).FirstOrDefaultAsync(x => x.Id == id);

        if (role is null)
        {
            throw new Exception("Role not found.");
        }

        ValidateSystemRole(role);

        var roleNameExists =
            await _dbContext.Roles
                .AnyAsync(x =>
                    x.Id != id &&
                    x.Name == request.Name);

        if (roleNameExists)
        {
            throw new Exception("Role name already exists.");
        }

        var permissionIds =
            request.PermissionIds
                .Distinct()
                .ToList();

        var permissionCount =
            await _dbContext.Permissions
                .CountAsync(x =>
                    permissionIds.Contains(x.Id));

        if (permissionCount != permissionIds.Count)
        {
            throw new Exception("One or more permissions are invalid.");
        }

        role.Name = request.Name;

        role.Description = request.Description;

        _dbContext.RolePermissions.RemoveRange(role.RolePermissions);

        await _dbContext.SaveChangesAsync();

        foreach (var permissionId in permissionIds)
        {
            _dbContext.RolePermissions.Add(
                new RolePermission
                {
                    Id = Guid.NewGuid(),

                    RoleId = role.Id,

                    PermissionId = permissionId
                });
        }

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id)
               ?? throw new Exception();
    }

    public async Task<List<LookupResponse>> GetLookupAsync()
    {
        var query = _dbContext.Roles.Where(x => x.IsActive);

        if (_scope.IsPlatformAdmin())
        {
            query = query.Where(x => x.Id != SeedData.SuperAdminRoleId);
        }

        if (_scope.IsInstitutionAdmin())
        {
            query =
                query.Where(x =>
                    x.Id != SeedData.SuperAdminRoleId &&
                    x.Id != SeedData.PlatformAdminRoleId &&
                    x.Id != SeedData.InstitutionAdminRoleId);
        }

        if (_scope.IsCampusAdmin())
        {
            query =
                query.Where(x =>
                    x.Id != SeedData.SuperAdminRoleId &&
                    x.Id != SeedData.PlatformAdminRoleId &&
                    x.Id != SeedData.InstitutionAdminRoleId &&
                    x.Id != SeedData.CampusAdminRoleId);
        }

        return await query
            .OrderBy(x => x.Name)
            .Select(x =>
                new LookupResponse
                {
                    Id = x.Id,
                    Name = x.Name
                })
            .ToListAsync();
    }

    public async Task ActivateAsync(Guid id)
    {
        var role =
            await _dbContext.Roles
                .FirstOrDefaultAsync(x => x.Id == id);

        if (role is null)
        {
            throw new Exception("Role not found.");
        }

        ValidateSystemRole(role);

        role.IsActive = true;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeactivateAsync(Guid id)
    {
        var role =
            await _dbContext.Roles
                .FirstOrDefaultAsync(x => x.Id == id);

        if (role is null)
        {
            throw new Exception("Role not found.");
        }

        ValidateSystemRole(role);

        var assignedToUsers =
            await _dbContext.UserRoles
                .AnyAsync(x => x.RoleId == id);

        if (assignedToUsers)
        {
            throw new Exception("Role is assigned to users and cannot be deactivated.");
        }

        role.IsActive = false;

        await _dbContext.SaveChangesAsync();
    }
}