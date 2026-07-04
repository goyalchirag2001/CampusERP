using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface IAcademicSessionService
{
    Task<List<AcademicSessionResponse>> GetAllAsync();

    Task<AcademicSessionResponse?> GetByIdAsync(Guid id);

    Task<List<AcademicSessionLookup>> GetLookupAsync();

    Task<AcademicSessionLookup?> GetCurrentAsync();

    Task<AcademicSessionResponse> CreateAsync(CreateAcademicSessionRequest request);

    Task<AcademicSessionResponse> UpdateAsync(Guid id, UpdateAcademicSessionRequest request);

    Task ActivateAsync(Guid id);

    Task DeactivateAsync(Guid id);

    Task SetCurrentAsync(Guid id);
}