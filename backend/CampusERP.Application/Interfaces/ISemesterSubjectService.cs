using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface ISemesterSubjectService
{
    Task<SemesterSubjectResponse> AssignAsync(AssignSubjectToSemesterRequest request);

    Task<List<SemesterSubjectResponse>> GetBySemesterAsync(Guid semesterId);

    Task<List<CourseSemesterSubjectResponse>> GetByCourseAsync(Guid courseId);

    Task<List<LookupResponse>> GetLookupBySectionAsync(Guid sectionId);

    Task RemoveAsync(Guid id);

    Task MoveUpAsync(Guid id);

    Task MoveDownAsync(Guid id);
}