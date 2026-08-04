using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface ISectionService
{
    Task<List<SectionResponse>> GetAllAsync();

    Task<SectionResponse?> GetByIdAsync(Guid id);

    Task<SectionResponse> CreateAsync(CreateSectionRequest request);

    Task<SectionResponse> UpdateAsync(Guid id, UpdateSectionRequest request);

    Task ActivateAsync(Guid id);

    Task DeactivateAsync(Guid id);

    Task<List<LookupResponse>> GetLookupBySemesterAsync(Guid semesterId);

    Task<List<LookupResponse>> GetLookupAsync();
}