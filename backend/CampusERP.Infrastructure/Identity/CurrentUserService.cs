using System.Security.Claims;
using CampusERP.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CampusERP.Infrastructure.Identity;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var value = _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(value, out var id)
                ? id
                : null;
        }
    }

    public Guid? InstitutionId
    {
        get
        {
            var value = _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirst("institutionId")
                ?.Value;

            return Guid.TryParse(value, out var id)
                ? id
                : null;
        }
    }

    public Guid? CampusId
    {
        get
        {
            var value = _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirst("campusId")
                ?.Value;

            return Guid.TryParse(value, out var id)
                ? id
                : null;
        }
    }

    public string? Email =>
        _httpContextAccessor
            .HttpContext?
            .User?
            .FindFirst(ClaimTypes.Email)?.Value;
}