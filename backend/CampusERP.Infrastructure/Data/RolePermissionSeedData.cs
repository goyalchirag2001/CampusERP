using CampusERP.Domain.Entities;

namespace CampusERP.Infrastructure.Data;

public static class RolePermissionSeedData
{
    public static List<RolePermission> GetRolePermissions()
    {
        var permissions = new List<RolePermission>();

        // Super Admin

        foreach (var permission in PermissionSeedData.Permissions)
        {
            permissions.Add(
                new RolePermission
                {
                    Id = Guid.NewGuid(),

                    RoleId = SeedData.SuperAdminRoleId,

                    PermissionId = permission.Id
                });
        }

        // Platform Admin

        permissions.AddRange(
        [
            Create(SeedData.PlatformAdminRoleId, PermissionSeedData.AdminDasboardViewPermissionId),

            Create(SeedData.PlatformAdminRoleId, PermissionSeedData.InstitutionViewPermissionId),

            Create(SeedData.PlatformAdminRoleId, PermissionSeedData.InstitutionCreatePermissionId),

            Create(SeedData.PlatformAdminRoleId, PermissionSeedData.InstitutionEditPermissionId),

            Create(SeedData.PlatformAdminRoleId, PermissionSeedData.InstitutionActivatePermissionId),
            
            Create(SeedData.PlatformAdminRoleId, PermissionSeedData.InstitutionDeactivatePermissionId),

            Create(SeedData.PlatformAdminRoleId, PermissionSeedData.CampusViewPermissionId),

            Create(SeedData.PlatformAdminRoleId, PermissionSeedData.CampusCreatePermissionId),

            Create(SeedData.PlatformAdminRoleId, PermissionSeedData.CampusEditPermissionId),

            Create(SeedData.PlatformAdminRoleId, PermissionSeedData.CampusActivatePermissionId),
            
            Create(SeedData.PlatformAdminRoleId, PermissionSeedData.CampusDeactivatePermissionId),

            Create(SeedData.PlatformAdminRoleId, PermissionSeedData.UserViewPermissionId),

            Create(SeedData.PlatformAdminRoleId, PermissionSeedData.UserCreatePermissionId),

            Create(SeedData.PlatformAdminRoleId, PermissionSeedData.UserEditPermissionId),

            Create(SeedData.PlatformAdminRoleId, PermissionSeedData.UserActivatePermissionId),
            
            Create(SeedData.PlatformAdminRoleId, PermissionSeedData.UserDeactivatePermissionId),

            Create(SeedData.PlatformAdminRoleId, PermissionSeedData.RoleViewPermissionId),

            Create(SeedData.PlatformAdminRoleId, PermissionSeedData.RoleCreatePermissionId),

            Create(SeedData.PlatformAdminRoleId, PermissionSeedData.RoleEditPermissionId),

            Create(SeedData.PlatformAdminRoleId, PermissionSeedData.RoleActivatePermissionId),
            
            Create(SeedData.PlatformAdminRoleId, PermissionSeedData.RoleDeactivatePermissionId),
        ]);

        // Institution Admin

        permissions.AddRange(
        [
            Create(SeedData.InstitutionAdminRoleId, PermissionSeedData.CampusViewPermissionId),

            Create(SeedData.InstitutionAdminRoleId, PermissionSeedData.CampusCreatePermissionId),

            Create(SeedData.InstitutionAdminRoleId, PermissionSeedData.CampusEditPermissionId),

            Create(SeedData.InstitutionAdminRoleId, PermissionSeedData.DepartmentViewPermissionId),

            Create(SeedData.InstitutionAdminRoleId, PermissionSeedData.DepartmentCreatePermissionId),

            Create(SeedData.InstitutionAdminRoleId, PermissionSeedData.DepartmentEditPermissionId),

            Create(SeedData.InstitutionAdminRoleId, PermissionSeedData.UserViewPermissionId),

            Create(SeedData.InstitutionAdminRoleId, PermissionSeedData.UserCreatePermissionId),

            Create(SeedData.InstitutionAdminRoleId, PermissionSeedData.UserEditPermissionId),
        ]);

        return permissions;
    }

    private static RolePermission Create(
        Guid roleId,
        Guid permissionId)
    {
        return new RolePermission
        {
            Id = Guid.NewGuid(),

            RoleId = roleId,

            PermissionId = permissionId
        };
    }
}