using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface IPermissionService
{
    Task<List<PermissionResponse>> GetAllAsync();
}