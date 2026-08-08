using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using CampusERP.Application.Common.Exceptions;

namespace CampusERP.Infrastructure.Authentication;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;
    private readonly ICurrentUserService _currentUserService;

    public AuthService(ApplicationDbContext dbContext,IPasswordService passwordService,IJwtService jwtService, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _passwordService = passwordService;
        _jwtService = jwtService;
        _currentUserService = currentUserService;
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

        if (existingUser is not null)
        {
            throw new ConflictException(ErrorCodes.EmailAlreadyExists, "Email already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),

            FirstName = request.FirstName,

            LastName = request.LastName,

            Email = request.Email,

            PasswordHash = _passwordService.HashPassword(request.Password),

            InstitutionId = Guid.Empty,

            CampusId = Guid.Empty,

            IsActive = true
        };

        _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync();

        var roles = new List<string>();

        var accessToken = _jwtService.GenerateAccessToken(user,roles, null);

        var refreshTokenValue = _jwtService.GenerateRefreshToken();

        _dbContext.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),

            UserId = user.Id,

            Token = refreshTokenValue,

            ExpiresAt = DateTime.UtcNow.AddDays(30)
        });

        await _dbContext.SaveChangesAsync();

        return new LoginResponse
        {
            UserId = user.Id,

            FirstName = user.FirstName,

            LastName = user.LastName,

            Email = user.Email,

            AccessToken = accessToken,

            RefreshToken = refreshTokenValue,

            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        User? user;

        if (string.IsNullOrWhiteSpace(request.InstitutionSlug))
        {
            // Platform Login

            user = await _dbContext.Users
                .Include(x => x.Institution)
                .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync(x =>
                    x.Email == request.Email &&
                    x.InstitutionId == SeedData.PlatformInstitutionId);
        }
        else
        {
            var institution = await _dbContext.Institutions
                .FirstOrDefaultAsync(x =>
                    x.LoginSlug == request.InstitutionSlug &&
                    x.IsActive);

            if (institution is null)
            {
                throw new UnauthorizedException(ErrorCodes.InstitutionNotFound, "Institution does not exist or is inactive.");
            }

            user = await _dbContext.Users
                .Include(x => x.Institution)
                .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync(x =>
                    x.Email == request.Email &&
                    x.InstitutionId == institution.Id);
        }

        if (user is null)
        {
            throw new UnauthorizedException(ErrorCodes.InvalidCredentials, "Invalid email or password.");
        }

        var isValid = _passwordService.VerifyPassword(request.Password, user.PasswordHash);

        if (!isValid)
        {
            throw new UnauthorizedException(ErrorCodes.InvalidCredentials, "Invalid email or password.");
        }

        var responseLastLoginAt = user.CurrentLoginAt;
        var responseCurrentLoginAt = DateTime.UtcNow;

        user.LastLoginAt = user.CurrentLoginAt;
        user.CurrentLoginAt = responseCurrentLoginAt;

        string? institutionSlug = null;

        if (user.InstitutionId != SeedData.PlatformInstitutionId)
        {
            institutionSlug = user.Institution.LoginSlug;
        }

        var roles = user.UserRoles
            .Select(x => x.Role.Name)
            .ToList();

        var accessToken = _jwtService.GenerateAccessToken(user, roles, institutionSlug);

        var refreshTokenValue = _jwtService.GenerateRefreshToken();

        var refreshToken =
            new RefreshToken
            {
                Id = Guid.NewGuid(),

                UserId = user.Id,

                Token = refreshTokenValue,

                ExpiresAt = DateTime.UtcNow.AddDays(30)
            };

        _dbContext.RefreshTokens.Add(refreshToken);

        await _dbContext.SaveChangesAsync();

        return new LoginResponse
        {
            UserId = user.Id,

            FirstName = user.FirstName,

            LastName = user.LastName,

            Email = user.Email,

            InstitutionSlug = institutionSlug,

            AccessToken = accessToken,

            RefreshToken = refreshTokenValue,

            ExpiresAt = DateTime.UtcNow.AddHours(1),

            LastLoginAt = responseLastLoginAt,

            CurrentLoginAt = responseCurrentLoginAt
        };
    }

    public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var refreshToken = await _dbContext.RefreshTokens
            .Include(x => x.User)
                .ThenInclude(x => x!.Institution)
            .Include(x => x.User)
                .ThenInclude(x => x!.UserRoles)
                    .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x =>
                x.Token == request.RefreshToken);

        if (refreshToken is null)
        {
            throw new UnauthorizedException(ErrorCodes.RefreshTokenInvalid, "Invalid refresh token.");
        }

        if (refreshToken.RevokedAt != null)
        {
            throw new UnauthorizedException(ErrorCodes.RefreshTokenRevoked, "Refresh token has been revoked.");
        }

        if (refreshToken.ExpiresAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedException(ErrorCodes.RefreshTokenExpired, "Refresh token has expired.");
        }

        var user = refreshToken.User!;

        string? institutionSlug = null;

        if (user.InstitutionId != SeedData.PlatformInstitutionId)
        {
            institutionSlug = user.Institution.LoginSlug;
        }

        var roles = user.UserRoles
            .Select(x => x.Role.Name)
            .ToList();

        var accessToken = _jwtService.GenerateAccessToken(user, roles, institutionSlug);

        var newRefreshTokenValue = _jwtService.GenerateRefreshToken();

        refreshToken.RevokedAt = DateTime.UtcNow;

        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),

            UserId = user.Id,

            Token = newRefreshTokenValue,

            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        _dbContext.RefreshTokens.Add(newRefreshToken);

        await _dbContext.SaveChangesAsync();

        return new LoginResponse
        {
            UserId = user.Id,

            FirstName = user.FirstName,

            LastName = user.LastName,

            Email = user.Email,

            InstitutionSlug = institutionSlug,

            AccessToken = accessToken,

            RefreshToken = newRefreshTokenValue,

            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };
    }

    public async Task<CurrentUserResponse> GetCurrentUserAsync()
    {
        var userId = _currentUserService.UserId;

        if (userId is null)
        {
            throw new UnauthorizedException(ErrorCodes.CurrentUserNotFound, "Current user could not be determined.");
        }

        var user = await _dbContext.Users
            .Include(x => x.Institution)
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == userId.Value);

        if (user is null)
        {
            throw new NotFoundException(ErrorCodes.CurrentUserNotFound, "User not found.");
        }

        string? institutionSlug = null;

        if (user.InstitutionId != SeedData.PlatformInstitutionId)
        {
            institutionSlug = user.Institution.LoginSlug;
        }

        var permissions = await _dbContext.UserRoles
                            .Where(x => x.UserId == user.Id)
                            .SelectMany(x => x.Role.RolePermissions)
                            .Select(x => x.Permission.Code)
                            .Distinct()
                            .ToListAsync();

        return new CurrentUserResponse
        {
            UserId = user.Id,

            FirstName = user.FirstName,

            LastName = user.LastName,

            Email = user.Email,

            InstitutionId = user.InstitutionId,

            CampusId = user.CampusId,

            InstitutionSlug = institutionSlug,

            Roles = user.UserRoles
                .Select(x => x.Role.Name)
                .ToList(),

            Permissions = permissions
        };
    }
}