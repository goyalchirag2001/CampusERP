using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface ISubjectService
{
    Task<SubjectResponse> CreateAsync(CreateSubjectRequest request);

    Task<List<SubjectResponse>> GetAllAsync();

    Task<SubjectResponse?> GetByIdAsync(Guid id);

    Task<SubjectResponse> UpdateAsync(Guid id, UpdateSubjectRequest request);

    Task ActivateAsync(Guid id);

    Task DeactivateAsync(Guid id);

    Task<List<LookupResponse>> GetLookupAsync();
}