using CampusERP.Application.Interfaces;
using CampusERP.Domain.Entities;
using CampusERP.Shared.Constants;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext dbContext,
        IPasswordService passwordService)
    {
        await SeedPlatformInstitutionAsync(
            dbContext);

        await SeedGlobalCampusAsync(
            dbContext);

        await SeedPlatformAdminAsync(
            dbContext,
            passwordService);
    }

    private static async Task SeedPlatformInstitutionAsync(
        ApplicationDbContext dbContext)
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
                Id =
                    SeedData.PlatformInstitutionId,

                Name =
                    "CampusERP Platform",

                Code =
                    "PLATFORM",

                IsActive = true
            });

        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedGlobalCampusAsync(
        ApplicationDbContext dbContext)
    {
        var exists =
            await dbContext.Campuses
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
                Id =
                    SeedData.GlobalCampusId,

                InstitutionId =
                    SeedData.PlatformInstitutionId,

                Name = "Global",

                Code = "GLOBAL",

                IsActive = true
            });

        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedPlatformAdminAsync(
    ApplicationDbContext dbContext,
    IPasswordService passwordService)
    {
        var user =
            await dbContext.Users
                .FirstOrDefaultAsync(x =>
                    x.Email ==
                    "admin@campuserp.local");

        if (user is null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),

                FirstName = "Platform",

                LastName = "Admin",

                Email =
                    "admin@campuserp.local",

                PasswordHash =
                    passwordService
                        .HashPassword(
                            "Admin@123"),

                InstitutionId =
                    SeedData.PlatformInstitutionId,

                CampusId =
                    SeedData.GlobalCampusId,

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
                    SeedData.PlatformAdminRoleId);

        if (!userRoleExists)
        {
            dbContext.UserRoles.Add(
                new UserRole
                {
                    Id = Guid.NewGuid(),

                    UserId = user.Id,

                    RoleId =
                        SeedData.PlatformAdminRoleId
                });

            await dbContext.SaveChangesAsync();
        }
    }
}