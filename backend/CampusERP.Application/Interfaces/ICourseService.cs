using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface ICourseService
{
    Task<CourseResponse> CreateAsync(CreateCourseRequest request);

    Task<List<CourseResponse>> GetAllAsync();

    Task<CourseResponse?> GetByIdAsync(Guid id);

    Task<CourseResponse> UpdateAsync(Guid id, UpdateCourseRequest request);

    Task ActivateAsync(Guid id);

    Task DeactivateAsync(Guid id);

    Task<List<LookupResponse>> GetLookupAsync();
}