using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface ISubjectService
{
    Task<SubjectResponse> CreateAsync(CreateSubjectRequest request);

    Task<List<SubjectResponse>> GetAllAsync();

    Task<SubjectResponse?> GetByIdAsync(Guid id);
}