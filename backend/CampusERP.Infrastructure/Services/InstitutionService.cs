using CampusERP.Application.DTOs.Institutions;
using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class InstitutionService : IInstitutionService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordService _passwordService;

    public InstitutionService(ApplicationDbContext dbContext, IPasswordService passwordService)
    {
        _dbContext = dbContext;
        _passwordService = passwordService;
    }

    public async Task<InstitutionResponse> CreateAsync(CreateInstitutionRequest request)
    {
        var codeExists = await _dbContext.Institutions.AnyAsync(x => x.Code == request.Code);

        if (codeExists)
        {
            throw new Exception("Institution code already exists.");
        }

        var slugExists = await _dbContext.Institutions.AnyAsync(x => x.LoginSlug == request.LoginSlug);

        if (slugExists)
        {
            throw new Exception("Login slug already exists.");
        }

        var adminEmailExists = await _dbContext.Users.AnyAsync(x => x.Email == request.AdminEmail);

        if (adminEmailExists)
        {
            throw new Exception("Admin email already exists.");
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var institution =
                new Institution
                {
                    Id = Guid.NewGuid(),

                    Name = request.Name,

                    Code = request.Code,

                    LoginSlug = request.LoginSlug,

                    Email = request.Email,

                    Phone = request.Phone,

                    Website = request.Website,

                    Address = request.Address,

                    LogoUrl = request.LogoUrl,

                    PrimaryColor = request.PrimaryColor,

                    SecondaryColor = request.SecondaryColor,

                    IsActive = true
                };

            _dbContext.Institutions.Add(institution);

            var campus =
                new Campus
                {
                    Id = Guid.NewGuid(),

                    InstitutionId = institution.Id,

                    Name = "Main Campus",

                    Code = $"{request.Code}-MAIN",

                    IsActive = true
                };

            _dbContext.Campuses.Add(campus);

            var temporaryPassword = $"Admin@{DateTime.UtcNow.Year}";

            var adminUser =
                new User
                {
                    Id = Guid.NewGuid(),

                    FirstName = request.AdminFirstName,

                    LastName = request.AdminLastName,

                    Email = request.AdminEmail,

                    PasswordHash = _passwordService.HashPassword(temporaryPassword),

                    InstitutionId = institution.Id,

                    CampusId = campus.Id,

                    IsActive = true
                };

            _dbContext.Users.Add(adminUser);

            var userRole =
                new UserRole
                {
                    Id = Guid.NewGuid(),

                    UserId = adminUser.Id,

                    RoleId = SeedData.InstitutionAdminRoleId
                };

            _dbContext.UserRoles.Add(userRole);

            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            return new InstitutionResponse
            {
                Id = institution.Id,

                Name = institution.Name,

                Code = institution.Code,

                LoginSlug = institution.LoginSlug,

                Email = institution.Email,

                Phone = institution.Phone,

                Website = institution.Website,

                Address = institution.Address,

                LogoUrl = institution.LogoUrl,

                PrimaryColor = institution.PrimaryColor,

                SecondaryColor = institution.SecondaryColor,

                IsActive = institution.IsActive
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<InstitutionResponse>> GetAllAsync()
    {
        return await _dbContext.Institutions
            .Select(x =>
                new InstitutionResponse
                {
                    Id = x.Id,

                    Name = x.Name,

                    Code = x.Code,

                    LoginSlug = x.LoginSlug,

                    Email = x.Email,

                    Phone = x.Phone,

                    Website = x.Website,

                    Address = x.Address,

                    LogoUrl = x.LogoUrl,

                    PrimaryColor = x.PrimaryColor,

                    SecondaryColor = x.SecondaryColor,

                    CampusCount = _dbContext.Campuses.Count(c => c.InstitutionId == x.Id),

                    StudentCount = _dbContext.Students.Count(s => s.InstitutionId == x.Id),

                    TeacherCount = _dbContext.Teachers.Count(t => t.InstitutionId == x.Id),

                    AdminEmail = _dbContext.Users
                            .Where(u =>
                                u.InstitutionId == x.Id)
                            .OrderBy(u => u.CreatedAt)
                            .Select(u => u.Email)
                            .FirstOrDefault() ?? "",

                    IsActive = x.IsActive
                })
            .ToListAsync();
    }

    public async Task<InstitutionResponse?> GetByIdAsync(Guid id)
    {
        var institution = await _dbContext.Institutions
                .Where(x => x.Id == id)
                .Select(x =>
                    new InstitutionResponse
                    {
                        Id = x.Id,

                        Name = x.Name,

                        Code = x.Code,

                        LoginSlug = x.LoginSlug,

                        Email = x.Email,

                        Phone = x.Phone,

                        Website = x.Website,

                        Address = x.Address,

                        LogoUrl = x.LogoUrl,

                        PrimaryColor = x.PrimaryColor,

                        SecondaryColor = x.SecondaryColor,

                        CampusCount = _dbContext.Campuses.Count(c => c.InstitutionId == x.Id),

                        StudentCount = _dbContext.Students.Count(s => s.InstitutionId == x.Id),

                        TeacherCount = _dbContext.Teachers.Count(t => t.InstitutionId == x.Id),

                        IsActive = x.IsActive
                    })
                .FirstOrDefaultAsync();

        if (institution is null)
        {
            return null;
        }

        var adminUser = await _dbContext.Users
                .Include(x => x.UserRoles)
                .FirstOrDefaultAsync(x =>
                    x.InstitutionId == institution.Id &&
                    x.UserRoles.Any(r =>
                        r.RoleId == SeedData.InstitutionAdminRoleId));

        if (adminUser is not null)
        {
            institution.AdminFirstName = adminUser.FirstName;

            institution.AdminLastName = adminUser.LastName;

            institution.AdminEmail = adminUser.Email;
        }

        return institution;
    }

    public async Task<InstitutionBrandingResponse?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Institutions
            .AsNoTracking()
            .Where(x =>
                x.LoginSlug.ToLower() == slug.ToLower() &&
                x.IsActive)
            .Select(x =>
                new InstitutionBrandingResponse
                {
                    Id = x.Id,

                    Name = x.Name,

                    LoginSlug = x.LoginSlug,

                    LogoUrl = x.LogoUrl,

                    PrimaryColor = x.PrimaryColor,

                    SecondaryColor = x.SecondaryColor
                })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<InstitutionResponse> UpdateAsync(Guid id, UpdateInstitutionRequest request)
    {
        var institution =
            await _dbContext.Institutions
                .FirstOrDefaultAsync(x =>
                    x.Id == id);

        if (institution is null)
        {
            throw new Exception("Institution not found.");
        }

        institution.Name = request.Name;

        institution.Code = request.Code;

        institution.LoginSlug = request.LoginSlug;

        institution.Email = request.Email;

        institution.Phone = request.Phone;

        institution.Website = request.Website;

        institution.Address = request.Address;

        institution.LogoUrl = request.LogoUrl;

        institution.PrimaryColor = request.PrimaryColor;

        institution.SecondaryColor = request.SecondaryColor;

        var adminUser =
            await _dbContext.Users
                .Include(x => x.UserRoles)
                .FirstOrDefaultAsync(x =>
                    x.InstitutionId == institution.Id &&
                    x.UserRoles.Any(r =>
                        r.RoleId == SeedData.InstitutionAdminRoleId));

        if (adminUser is not null)
        {
            adminUser.FirstName = request.AdminFirstName ?? adminUser.FirstName;

            adminUser.LastName = request.AdminLastName ?? adminUser.LastName;

            adminUser.Email = request.AdminEmail ?? adminUser.Email;
        }

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id) ?? throw new Exception("Institution not found.");
    }

    public async Task DeleteAsync(Guid id)
    {
        var institution =
            await _dbContext.Institutions
                .FirstOrDefaultAsync(x =>
                    x.Id == id);

        if (institution is null)
        {
            throw new Exception("Institution not found.");
        }

        _dbContext.Institutions.Remove(institution);

        await _dbContext.SaveChangesAsync();
    }

    public async Task ActivateAsync(Guid id)
    {
        var institution = await _dbContext.Institutions
            .FirstOrDefaultAsync(x => x.Id == id);

        if (institution is null)
        {
            throw new Exception("Institution not found.");
        }

        institution.IsActive = true;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeactivateAsync(Guid id)
    {
        var institution = await _dbContext.Institutions
            .FirstOrDefaultAsync(x => x.Id == id);

        if (institution is null)
        {
            throw new Exception("Institution not found.");
        }

        institution.IsActive = false;

        await _dbContext.SaveChangesAsync();
    }
}