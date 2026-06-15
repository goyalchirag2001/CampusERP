using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface ITeacherAssignmentService
{
    Task<TeacherAssignmentResponse> AssignAsync(AssignTeacherRequest request);

    Task<List<TeacherAssignmentResponse>> GetByTeacherAsync(Guid teacherId);
}