using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface IUserService
{
    Task<UserResponse> CreateAsync(CreateUserRequest request);

    Task<List<UserResponse>> GetAllAsync();

    Task<UserResponse?> GetByIdAsync(Guid id);

    Task<UserResponse> UpdateAsync(Guid id, UpdateUserRequest request);

    Task ActivateAsync(Guid id);

    Task DeactivateAsync(Guid id);

    Task ResetPasswordAsync(Guid id, string newPassword);
}