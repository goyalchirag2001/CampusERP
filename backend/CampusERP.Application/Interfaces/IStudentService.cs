using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface IStudentService
{
    Task<StudentResponse> CreateAsync(CreateStudentRequest request);

    Task<List<StudentResponse>> GetAllAsync();

    Task<StudentResponse?> GetByIdAsync(Guid id);

    Task<StudentResponse> UpdateAsync(Guid id, UpdateStudentRequest request);

    Task ActivateAsync(Guid id);

    Task DeactivateAsync(Guid id);

    Task<List<LookupResponse>> GetLookupAsync();
}