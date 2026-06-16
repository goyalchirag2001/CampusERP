using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface IStudentService
{
    Task<StudentResponse> CreateAsync(CreateStudentRequest request);

    Task<List<StudentResponse>> GetAllAsync();

    Task<StudentResponse?> GetByIdAsync(Guid id);
}