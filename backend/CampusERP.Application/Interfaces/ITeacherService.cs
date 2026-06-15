using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface ITeacherService
{
    Task<TeacherResponse> CreateAsync(CreateTeacherRequest request);

    Task<List<TeacherResponse>> GetAllAsync();

    Task<TeacherResponse?> GetByIdAsync(Guid id);
}