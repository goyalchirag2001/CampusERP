using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface ISemesterService
{
    Task<List<SemesterResponse>> GetAllAsync();

    Task<SemesterResponse?> GetByIdAsync(Guid id);

    Task<List<LookupResponse>> GetLookupByCourseAsync(Guid courseId);
}