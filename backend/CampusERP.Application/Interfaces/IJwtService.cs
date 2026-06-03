using CampusERP.Domain.Entities;

namespace CampusERP.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user, IEnumerable<string> roles);

    string GenerateRefreshToken();
}