using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface IRoleService
{
    Task<RoleResponse> CreateAsync(CreateRoleRequest request);

    Task<List<RoleResponse>> GetAllAsync();

    Task<RoleResponse?> GetByIdAsync(Guid id);

    Task<RoleResponse> UpdateAsync(Guid id, UpdateRoleRequest request);

    Task<List<LookupResponse>> GetLookupAsync();

    Task ActivateAsync(Guid id);

    Task DeactivateAsync(Guid id);
}