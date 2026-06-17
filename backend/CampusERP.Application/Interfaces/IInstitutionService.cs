using CampusERP.Application.DTOs.Institutions;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface IInstitutionService
{
    Task<InstitutionResponse> CreateAsync(CreateInstitutionRequest request);

    Task<List<InstitutionResponse>> GetAllAsync();

    Task<InstitutionResponse?> GetByIdAsync(Guid id);

    Task<InstitutionBrandingResponse?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<InstitutionResponse> UpdateAsync(Guid id, UpdateInstitutionRequest request);

    Task DeleteAsync(Guid id);

    Task ActivateAsync(Guid id);

    Task DeactivateAsync(Guid id);
}