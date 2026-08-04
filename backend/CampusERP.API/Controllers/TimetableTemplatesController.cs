using CampusERP.Application.Common.Models;
using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

/// <summary>
/// Manages timetable templates.
/// </summary>
[Authorize]
public class TimetableTemplatesController : BaseApiController
{
    private readonly ITimetableTemplateService _timetableTemplateService;

    public TimetableTemplatesController(ITimetableTemplateService timetableTemplateService)
    {
        _timetableTemplateService = timetableTemplateService;
    }

    #region Queries

    /// <summary>
    /// Returns all timetable templates.
    /// </summary>
    [Authorize(Policy = PermissionConstants.TimetableTemplateView)]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<TimetableTemplateResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<TimetableTemplateResponse>>>> GetAll()
    {
        var response = await _timetableTemplateService.GetAllAsync();

        return Success(response);
    }

    /// <summary>
    /// Returns timetable template by Id.
    /// </summary>
    [Authorize(Policy = PermissionConstants.TimetableTemplateView)]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<TimetableTemplateResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<TimetableTemplateResponse>>> GetById(Guid id)
    {
        var response = await _timetableTemplateService.GetByIdAsync(id);

        return Success(response);
    }

    /// <summary>
    /// Returns timetable for a teacher.
    /// </summary>
    [Authorize(Policy = PermissionConstants.TimetableTemplateView)]
    [HttpGet("teacher/{teacherId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<TimetableTemplateResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<TimetableTemplateResponse>>>> GetByTeacher(Guid teacherId)
    {
        var response = await _timetableTemplateService.GetByTeacherAsync(teacherId);

        return Success(response);
    }

    /// <summary>
    /// Returns timetable for a section.
    /// </summary>
    [Authorize(Policy = PermissionConstants.TimetableTemplateView)]
    [HttpGet("section/{sectionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<TimetableTemplateResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<TimetableTemplateResponse>>>> GetBySection(Guid sectionId)
    {
        var response = await _timetableTemplateService.GetBySectionAsync(sectionId);

        return Success(response);
    }

    /// <summary>
    /// Returns timetable templates for an academic session.
    /// </summary>
    [Authorize(Policy = PermissionConstants.TimetableTemplateView)]
    [HttpGet("academic-session/{academicSessionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<TimetableTemplateResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<TimetableTemplateResponse>>>> GetByAcademicSession(Guid academicSessionId)
    {
        var response = await _timetableTemplateService.GetByAcademicSessionAsync(academicSessionId);

        return Success(response);
    }

    /// <summary>
    /// Returns weekly timetable for a section.
    /// </summary>
    [Authorize(Policy = PermissionConstants.TimetableTemplateView)]
    [HttpGet("weekly")]
    [ProducesResponseType(typeof(ApiResponse<List<TimetableTemplateResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<TimetableTemplateResponse>>>> GetWeeklyTimetable(
        [FromQuery] Guid sectionId,
        [FromQuery] Guid academicSessionId)
    {
        var response = await _timetableTemplateService.GetWeeklyTimetableAsync(sectionId, academicSessionId);

        return Success(response);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates a timetable template.
    /// </summary>
    [Authorize(Policy = PermissionConstants.TimetableTemplateManage)]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TimetableTemplateResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<TimetableTemplateResponse>>> Create(
        [FromBody] CreateTimetableTemplateRequest request)
    {
        var response = await _timetableTemplateService.CreateAsync(request);

        return Success(response, "Timetable template created successfully.");
    }

    /// <summary>
    /// Updates a timetable template.
    /// </summary>
    [Authorize(Policy = PermissionConstants.TimetableTemplateManage)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<TimetableTemplateResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<TimetableTemplateResponse>>> Update(
        Guid id,
        [FromBody] UpdateTimetableTemplateRequest request)
    {
        var response = await _timetableTemplateService.UpdateAsync(id, request);

        return Success(response, "Timetable template updated successfully.");
    }

    /// <summary>
    /// Activates a timetable template.
    /// </summary>
    [Authorize(Policy = PermissionConstants.TimetableTemplateManage)]
    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> Activate(Guid id)
    {
        await _timetableTemplateService.ActivateAsync(id);

        return Success(
            new
            {
                Activated = true
            },
            "Timetable template activated successfully.");
    }

    /// <summary>
    /// Deactivates a timetable template.
    /// </summary>
    [Authorize(Policy = PermissionConstants.TimetableTemplateManage)]
    [HttpPost("{id:guid}/deactivate")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> Deactivate(Guid id)
    {
        await _timetableTemplateService.DeactivateAsync(id);

        return Success(
            new
            {
                Deactivated = true
            },
            "Timetable template deactivated successfully.");
    }

    /// <summary>
    /// Deletes a timetable template.
    /// </summary>
    [Authorize(Policy = PermissionConstants.TimetableTemplateManage)]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        await _timetableTemplateService.DeleteAsync(id);

        return Success(true, "Timetable template deleted successfully.");
    }

    #endregion
}