using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IDataAccessScope _scope;

    private readonly IPasswordService _passwordService;

    public UserService(ApplicationDbContext dbContext, IDataAccessScope scope, IPasswordService passwordService)
    {
        _dbContext = dbContext;

        _scope = scope;

        _passwordService = passwordService;
    }

    private async Task ValidateRoleAssignmentAsync(IEnumerable<Guid> roleIds)
    {
        var currentUserRoles = await _dbContext.UserRoles.Where(x => x.UserId == _scope.UserId()).Select(x => x.RoleId).ToListAsync();

        if (currentUserRoles.Contains(SeedData.SuperAdminRoleId))
        {
            return;
        }

        if (currentUserRoles.Contains(SeedData.PlatformAdminRoleId))
        {
            if (roleIds.Contains(SeedData.SuperAdminRoleId))
            {
                throw new Exception("Platform Admin cannot assign Super Admin.");
            }

            return;
        }

        if (currentUserRoles.Contains(SeedData.InstitutionAdminRoleId))
        {
            var forbiddenRoles =
                new[]
                {
                SeedData.SuperAdminRoleId,
                SeedData.PlatformAdminRoleId,
                SeedData.InstitutionAdminRoleId
                };

            if (roleIds.Any(x =>
                    forbiddenRoles.Contains(x)))
            {
                throw new Exception("Institution Admin cannot assign this role.");
            }

            return;
        }

        if (currentUserRoles.Contains(SeedData.CampusAdminRoleId))
        {
            var forbiddenRoles =
                new[]
                {
                SeedData.SuperAdminRoleId,
                SeedData.PlatformAdminRoleId,
                SeedData.InstitutionAdminRoleId,
                SeedData.CampusAdminRoleId
                };

            if (roleIds.Any(x =>
                    forbiddenRoles.Contains(x)))
            {
                throw new Exception("Campus Admin cannot assign this role.");
            }

            return;
        }

        throw new Exception("You are not authorized to assign roles.");
    }

    private async Task ValidateTargetUserAsync(User user)
    {
        var currentUserRoles =
            await _dbContext.UserRoles
                .Where(x =>
                    x.UserId == _scope.UserId())
                .Select(x => x.RoleId)
                .ToListAsync();

        var isSuperAdmin = currentUserRoles.Contains(SeedData.SuperAdminRoleId);

        if (isSuperAdmin)
        {
            return;
        }

        var targetRoles =
            await _dbContext.UserRoles
                .Where(x =>
                    x.UserId == user.Id)
                .Select(x => x.RoleId)
                .ToListAsync();

        if (targetRoles.Contains( SeedData.SuperAdminRoleId) || targetRoles.Contains(SeedData.PlatformAdminRoleId))
        {
            throw new Exception("You are not allowed to manage this user.");
        }
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request)
    {
        var emailExists = await _dbContext.Users.AnyAsync(x => x.Email == request.Email);

        if (emailExists)
        {
            throw new Exception("Email already exists.");
        }

        Guid institutionId;

        if (_scope.IsSuperAdmin() || _scope.IsPlatformAdmin())
        {
            institutionId = request.InstitutionId;
        }
        else
        {
            institutionId = _scope.InstitutionId();
        }

        if (_scope.IsCampusAdmin())
        {
            request.CampusId = _scope.CampusId();
        }

        var campusExists = await _dbContext.Campuses.AnyAsync(x => x.Id == request.CampusId && x.InstitutionId == institutionId);

        if (!campusExists)
        {
            throw new Exception("Campus not found.");
        }

        await ValidateRoleAssignmentAsync(request.RoleIds);

        var temporaryPassword = $"Campus@{DateTime.UtcNow.Year}";

        var roleCount = await _dbContext.Roles.CountAsync(x => request.RoleIds.Contains(x.Id));

        if (roleCount != request.RoleIds.Count)
        {
            throw new Exception("One or more roles are invalid.");
        }

        var user =
            new User
            {
                Id = Guid.NewGuid(),

                InstitutionId = institutionId,

                CampusId = request.CampusId,

                FirstName = request.FirstName,

                LastName = request.LastName,

                Email = request.Email,

                PhoneNumber = request.PhoneNumber,

                PasswordHash = _passwordService.HashPassword(temporaryPassword),

                IsActive = true
            };

        _dbContext.Users.Add(user);

        foreach (var roleId in request.RoleIds)
        {
            _dbContext.UserRoles.Add(
                new UserRole
                {
                    Id = Guid.NewGuid(),

                    UserId = user.Id,

                    RoleId = roleId
                });
        }

        await _dbContext.SaveChangesAsync();

        var response = await GetByIdAsync(user.Id) ?? throw new Exception();

        response.TemporaryPassword = temporaryPassword;

        return response;
    }

    public async Task<List<UserResponse>> GetAllAsync()
    {
        return await ApplyScope(_dbContext.Users)
            .Include(x => x.Institution)
            .Include(x => x.Campus)
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .Select(x => new UserResponse
            {
                Id = x.Id,

                InstitutionId = x.InstitutionId,

                CampusId = x.CampusId,

                InstitutionName = x.Institution.Name,

                CampusName = x.Campus.Name,

                FirstName = x.FirstName,

                LastName = x.LastName,

                Email = x.Email,

                PhoneNumber = x.PhoneNumber,

                IsActive = x.IsActive,

                RoleIds =
                    x.UserRoles
                        .Select(r => r.RoleId)
                        .ToList(),

                Roles =
                    x.UserRoles
                        .Select(r => r.Role.Name)
                        .ToList()
            })
            .ToListAsync();
    }

    public async Task<UserResponse?> GetByIdAsync(Guid id)
    {
        return await ApplyScope(_dbContext.Users)
            .Where(x => x.Id == id)
            .Select(x => new UserResponse
            {
                Id = x.Id,

                InstitutionId = x.InstitutionId,

                CampusId = x.CampusId,

                InstitutionName = x.Institution.Name,

                CampusName = x.Campus.Name,

                FirstName = x.FirstName,

                LastName = x.LastName,

                Email = x.Email,

                PhoneNumber = x.PhoneNumber,

                IsActive = x.IsActive,

                RoleIds =
                    x.UserRoles
                        .Select(r => r.RoleId)
                        .ToList(),

                Roles =
                    x.UserRoles
                        .Select(r => r.Role.Name)
                        .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<UserResponse> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        var user = await _dbContext.Users.Include(x => x.UserRoles).FirstOrDefaultAsync(x => x.Id == id);

        if (user is null)
        {
            throw new Exception("User not found.");
        }

        var inScope = await ApplyScope(_dbContext.Users).AnyAsync(x => x.Id == id);

        if (!inScope)
        {
            throw new Exception("User not found.");
        }

        await ValidateTargetUserAsync(user);

        var emailExists = await _dbContext.Users.AnyAsync(x => x.Id != id && x.Email == request.Email);

        if (emailExists)
        {
            throw new Exception("Email already exists.");
        }

        if (_scope.IsCampusAdmin())
        {
            request.CampusId = _scope.CampusId();
        }

        var campusExists = await _dbContext.Campuses.AnyAsync(x => x.Id == request.CampusId && x.InstitutionId == user.InstitutionId);

        if (!campusExists)
        {
            throw new Exception("Campus not found.");
        }

        await ValidateRoleAssignmentAsync(request.RoleIds);

        // ==========================================
        // Last SuperAdmin Protection
        // ==========================================

        var currentlySuperAdmin = user.UserRoles.Any(x => x.RoleId == SeedData.SuperAdminRoleId);

        var willRemainSuperAdmin = request.RoleIds.Contains(SeedData.SuperAdminRoleId);

        if (currentlySuperAdmin && !willRemainSuperAdmin)
        {
            var totalSuperAdmins = await _dbContext.UserRoles.CountAsync(x => x.RoleId == SeedData.SuperAdminRoleId);

            if (totalSuperAdmins <= 1)
            {
                throw new Exception("At least one SuperAdmin must exist.");
            }
        }

        // ==========================================
        // Update User
        // ==========================================

        user.FirstName = request.FirstName;

        user.LastName = request.LastName;

        user.Email = request.Email;

        user.PhoneNumber = request.PhoneNumber;

        user.CampusId = request.CampusId;

        // ==========================================
        // Replace Roles
        // ==========================================

        _dbContext.UserRoles.RemoveRange(user.UserRoles);

        var roleCount = await _dbContext.Roles.CountAsync(x => request.RoleIds.Contains(x.Id));

        if (roleCount != request.RoleIds.Count)
        {
            throw new Exception("One or more roles are invalid.");
        }

        foreach (var roleId in request.RoleIds)
        {
            _dbContext.UserRoles.Add(
                new UserRole
                {
                    Id = Guid.NewGuid(),

                    UserId = user.Id,

                    RoleId = roleId
                });
        }

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id) ?? throw new Exception();
    }

    public async Task ActivateAsync(Guid id)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);

        if (user is null)
        {
            throw new Exception("User not found.");
        }

        var inScope = await ApplyScope(_dbContext.Users).AnyAsync(x => x.Id == id);

        if (!inScope)
        {
            throw new Exception("User not found.");
        }

        await ValidateTargetUserAsync(user);

        user.IsActive = true;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeactivateAsync(Guid id)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);

        if (user is null)
        {
            throw new Exception("User not found.");
        }

        if (id == _scope.UserId())
        {
            throw new Exception("You cannot deactivate yourself.");
        }

        var inScope = await ApplyScope(_dbContext.Users).AnyAsync(x => x.Id == id);

        if (!inScope)
        {
            throw new Exception("User not found.");
        }

        await ValidateTargetUserAsync(user);

        var isSuperAdmin = await _dbContext.UserRoles.AnyAsync(x => x.UserId == user.Id && x.RoleId == SeedData.SuperAdminRoleId);

        if (isSuperAdmin)
        {
            var totalSuperAdmins = await _dbContext.UserRoles.CountAsync(x => x.RoleId == SeedData.SuperAdminRoleId);

            if (totalSuperAdmins <= 1)
            {
                throw new Exception("At least one SuperAdmin must remain active.");
            }
        }

        user.IsActive = false;

        await _dbContext.SaveChangesAsync();
    }

    public async Task ResetPasswordAsync(Guid id, string newPassword)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);

        if (user is null)
        {
            throw new Exception("User not found.");
        }
        var inScope = await ApplyScope(_dbContext.Users).AnyAsync(x => x.Id == id);

        if (!inScope)
        {
            throw new Exception("User not found.");
        }

        await ValidateTargetUserAsync(user);

        user.PasswordHash = _passwordService.HashPassword(newPassword);

        await _dbContext.SaveChangesAsync();
    }

    private IQueryable<User> ApplyScope(IQueryable<User> query)
    {
        if (_scope.IsSuperAdmin() || _scope.IsPlatformAdmin())
        {
            return query;
        }

        if (_scope.IsInstitutionAdmin())
        {
            query =
                query.Where(x =>
                    x.InstitutionId ==
                    _scope.InstitutionId());
        }

        if (_scope.IsCampusAdmin())
        {
            query =
                query.Where(x =>
                    x.CampusId ==
                    _scope.CampusId());
        }

        return query;
    }
}