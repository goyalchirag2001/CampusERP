using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface IStudentImportService
{
    Task<byte[]> GenerateTemplateAsync();

    Task<StudentImportValidationResponse> ValidateAsync(ImportStudentsRequest request);

    Task<StudentImportValidationResponse> ImportAsync(ImportStudentsRequest request);

    byte[] GenerateCredentialsExcel(IEnumerable<StudentImportCredential> credentials);
}