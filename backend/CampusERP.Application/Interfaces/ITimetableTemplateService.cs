using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface ITimetableTemplateService
{
    #region Queries

    Task<List<TimetableTemplateResponse>> GetAllAsync();

    Task<TimetableTemplateResponse> GetByIdAsync(Guid id);

    Task<List<TimetableTemplateResponse>> GetByTeacherAsync(Guid teacherId);

    Task<List<TimetableTemplateResponse>> GetBySectionAsync(Guid sectionId);

    Task<List<TimetableTemplateResponse>> GetByAcademicSessionAsync(Guid academicSessionId);

    Task<List<TimetableTemplateResponse>> GetWeeklyTimetableAsync(Guid sectionId, Guid academicSessionId);

    #endregion

    #region Commands

    Task<TimetableTemplateResponse> CreateAsync(CreateTimetableTemplateRequest request);

    Task<TimetableTemplateResponse> UpdateAsync(Guid id, UpdateTimetableTemplateRequest request);

    Task ActivateAsync(Guid id);

    Task DeactivateAsync(Guid id);

    Task DeleteAsync(Guid id);

    #endregion
}