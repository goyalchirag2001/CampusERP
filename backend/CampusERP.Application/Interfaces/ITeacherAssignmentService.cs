using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface ITeacherAssignmentService
{
    Task<List<TeacherAssignmentListResponse>> GetAllAsync();

    Task<TeacherAssignmentResponse?> GetByIdAsync(Guid id);

    Task<TeacherAssignmentResponse> CreateAsync(CreateTeacherAssignmentRequest request);

    Task<TeacherAssignmentResponse> UpdateAsync(Guid id, UpdateTeacherAssignmentRequest request);

    Task DeleteAsync(Guid id);
}