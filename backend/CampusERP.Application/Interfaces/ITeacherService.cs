using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface ITeacherService
{
    Task<TeacherResponse> CreateAsync(CreateTeacherRequest request);

    Task<List<TeacherResponse>> GetAllAsync();

    Task<TeacherResponse?> GetByIdAsync(Guid id);

    Task<TeacherResponse> UpdateAsync(Guid id, UpdateTeacherRequest request);

    Task ActivateAsync(Guid id);

    Task DeactivateAsync(Guid id);

    Task<List<LookupResponse>> GetLookupAsync();

    Task<List<TeacherLookupResponse>> GetLookupWithDepartmentAsync();
}