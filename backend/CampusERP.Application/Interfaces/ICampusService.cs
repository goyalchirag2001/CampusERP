using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface ICampusService
{
    Task<CampusResponse> CreateAsync(CreateCampusRequest request);

    Task<List<CampusResponse>> GetAllAsync();

    Task<CampusResponse?> GetByIdAsync(Guid id);

    Task<CampusResponse> UpdateAsync(Guid id, UpdateCampusRequest request);

    Task ActivateAsync(Guid id);

    Task DeactivateAsync(Guid id);

    Task<List<LookupResponse>> GetLookupAsync();
}