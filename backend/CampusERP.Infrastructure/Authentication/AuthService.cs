using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Authentication;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;

    public AuthService(ApplicationDbContext dbContext,IPasswordService passwordService,IJwtService jwtService)
    {
        _dbContext = dbContext;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(x =>
                    x.Email == request.Email);

        if (existingUser is not null)
        {
            throw new Exception("Email already exists.");
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

        var accessToken = _jwtService.GenerateAccessToken(user,roles);

        var refreshToken = _jwtService.GenerateRefreshToken();

        return new LoginResponse
        {
            UserId = user.Id,

            FirstName = user.FirstName,

            LastName = user.LastName,

            Email = user.Email,

            AccessToken = accessToken,

            RefreshToken = refreshToken,

            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _dbContext.Users
                .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync(x =>
                    x.Email == request.Email);

        if (user is null)
        {
            throw new Exception("Invalid email or password.");
        }

        var isValid = _passwordService.VerifyPassword(
                request.Password,
                user.PasswordHash);

        if (!isValid)
        {
            throw new Exception("Invalid email or password.");
        }

        var roles = user.UserRoles.Select(x => x.Role.Name).ToList();

        var accessToken = _jwtService.GenerateAccessToken(user, roles);

        var refreshTokenValue = _jwtService.GenerateRefreshToken();

        var refreshToken =
            new RefreshToken
            {
                Id = Guid.NewGuid(),

                UserId = user.Id,

                Token = refreshTokenValue,

                ExpiresAt =
                    DateTime.UtcNow.AddDays(30)
            };

        _dbContext.RefreshTokens.Add(refreshToken);

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

    public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        throw new NotImplementedException();
    }
}