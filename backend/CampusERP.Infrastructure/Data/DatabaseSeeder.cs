using CampusERP.Application.Interfaces;
using CampusERP.Domain.Entities;
using CampusERP.Shared.Constants;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext dbContext, IPasswordService passwordService)
    {
        await SeedRolesAsync(dbContext);

        await SeedPermissionsAsync(dbContext);

        await SeedRolePermissionsAsync(dbContext);

        await SeedPlatformInstitutionAsync(dbContext);

        await SeedAcademicConfigurationAsync(dbContext);

        await SeedGlobalCampusAsync(dbContext);

        await SeedSuperAdminAsync(dbContext, passwordService);
    }

    private static async Task SeedPlatformInstitutionAsync(ApplicationDbContext dbContext)
    {
        var exists =
            await dbContext.Institutions
                .AnyAsync(x =>
                    x.Id ==
                    SeedData.PlatformInstitutionId);

        if (exists)
        {
            return;
        }

        dbContext.Institutions.Add(
            new Institution
            {
                Id = SeedData.PlatformInstitutionId,

                Name = "CampusERP Platform",

                Code = "PLATFORM",

                LoginSlug = "platform",

                PrimaryColor = "#2563EB",

                SecondaryColor = "#1E293B",

                IsActive = true
            });

        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedGlobalCampusAsync(ApplicationDbContext dbContext)
    {
        var exists = await dbContext.Campuses
                .AnyAsync(x =>
                    x.Id ==
                    SeedData.GlobalCampusId);

        if (exists)
        {
            return;
        }

        dbContext.Campuses.Add(
            new Campus
            {
                Id = SeedData.GlobalCampusId,

                InstitutionId = SeedData.PlatformInstitutionId,

                Name = "Global",

                Code = "GLOBAL",

                IsActive = true
            });

        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedSuperAdminAsync(ApplicationDbContext dbContext, IPasswordService passwordService)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == "goyalchirag2001@gmail.com");

        if (user is null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),

                FirstName = "Super",

                LastName = "Admin",

                Email = "goyalchirag2001@gmail.com",

                PasswordHash = passwordService.HashPassword("Admin@123"),

                InstitutionId = SeedData.PlatformInstitutionId,

                CampusId = SeedData.GlobalCampusId,

                PhoneNumber = "+91-9811076788",

                IsActive = true
            };

            dbContext.Users.Add(user);

            await dbContext.SaveChangesAsync();
        }

        var userRoleExists =
            await dbContext.UserRoles
                .AnyAsync(x =>
                    x.UserId == user.Id &&
                    x.RoleId ==
                    SeedData.SuperAdminRoleId);

        if (!userRoleExists)
        {
            dbContext.UserRoles.Add(
                new UserRole
                {
                    Id = Guid.NewGuid(),

                    UserId = user.Id,

                    RoleId = SeedData.SuperAdminRoleId
                });

            await dbContext.SaveChangesAsync();
        }
    }

    private static async Task SeedRolesAsync(ApplicationDbContext dbContext)
    {
        var roles =
            new List<Role>
            {
            new()
            {
                Id = SeedData.SuperAdminRoleId,
                Name = RoleConstants.SuperAdmin,
                IsSystemRole = true
            },

            new()
            {
                Id = SeedData.PlatformAdminRoleId,
                Name = RoleConstants.PlatformAdmin,
                IsSystemRole = true
            },

            new()
            {
                Id = SeedData.InstitutionAdminRoleId,
                Name = RoleConstants.InstitutionAdmin,
                IsSystemRole = true
            },

            new()
            {
                Id = SeedData.CampusAdminRoleId,
                Name = RoleConstants.CampusAdmin,
                IsSystemRole = true
            },

            new()
            {
                Id = SeedData.TeacherRoleId,
                Name = RoleConstants.Teacher,
                IsSystemRole = true
            },

            new()
            {
                Id = SeedData.StudentRoleId,
                Name = RoleConstants.Student,
                IsSystemRole = true
            }
            };

        foreach (var role in roles)
        {
            var exists =
                await dbContext.Roles.AnyAsync(x =>
                    x.Id == role.Id);

            if (!exists)
            {
                dbContext.Roles.Add(role);
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedPermissionsAsync(ApplicationDbContext dbContext)
    {
        foreach (var permission in PermissionSeedData.Permissions)
        {
            var exists =
                await dbContext.Permissions.AnyAsync(x =>
                    x.Id == permission.Id);

            if (!exists)
            {
                dbContext.Permissions.Add(permission);
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedRolePermissionsAsync(ApplicationDbContext dbContext)
    {
        var permissions =
            RolePermissionSeedData.GetRolePermissions();

        foreach (var permission in permissions)
        {
            var exists =
                await dbContext.RolePermissions.AnyAsync(x =>
                    x.RoleId == permission.RoleId &&
                    x.PermissionId == permission.PermissionId);

            if (!exists)
            {
                dbContext.RolePermissions.Add(permission);
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedAcademicConfigurationAsync(ApplicationDbContext dbContext)
    {
        var exists =
            await dbContext.AcademicConfigurations
                .AnyAsync(x =>
                    x.InstitutionId == SeedData.PlatformInstitutionId &&
                    x.CampusId == null);

        if (exists)
        {
            return;
        }

        dbContext.AcademicConfigurations.Add(
            new AcademicConfiguration
            {
                Id = Guid.NewGuid(),

                InstitutionId = SeedData.PlatformInstitutionId,

                CampusId = null,

                AcademicTermsPerSession = 2,

                AutoPromoteEnabled = true,

                MinimumAttendancePercentage = 75,

                AllowAttendanceEditing = true,

                AttendanceEditWindowDays = 7
            });

        await dbContext.SaveChangesAsync();
    }
}