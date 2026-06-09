using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface IDepartmentService
{
    Task<DepartmentResponse> CreateAsync(CreateDepartmentRequest request);

    Task<List<DepartmentResponse>> GetAllAsync();

    Task<DepartmentResponse?> GetByIdAsync(Guid id);
}