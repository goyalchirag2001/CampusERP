using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface IInstitutionService
{
    Task<InstitutionResponse> CreateAsync(CreateInstitutionRequest request);

    Task<List<InstitutionResponse>> GetAllAsync();

    Task<InstitutionResponse?> GetByIdAsync(Guid id);
}