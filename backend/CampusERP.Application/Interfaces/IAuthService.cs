using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> RegisterAsync(RegisterRequest request);

    Task<LoginResponse> LoginAsync(LoginRequest request);

    Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request);
}