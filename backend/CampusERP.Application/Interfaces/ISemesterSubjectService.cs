using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface ISemesterSubjectService
{
    Task<SemesterSubjectResponse> AssignAsync(AssignSubjectToSemesterRequest request);

    Task<List<SemesterSubjectResponse>> GetBySemesterAsync(Guid semesterId);
}