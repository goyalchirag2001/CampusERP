using CampusERP.Application.Common.Models;
using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

/// <summary>
/// Provides calendar occurrences for authenticated teachers and students.
/// </summary>
[Authorize]
public class TimetableCalendarController : BaseApiController
{
    private readonly ITimetableCalendarService _timetableCalendarService;

    public TimetableCalendarController(ITimetableCalendarService timetableCalendarService)
    {
        _timetableCalendarService = timetableCalendarService;
    }

    // =========================================================
    // Teacher Calendar
    // =========================================================

    /// <summary>
    /// Returns the calendar for the authenticated teacher.
    ///
    /// The teacher is resolved from the authenticated user.
    /// The client must not provide a teacher ID.
    /// </summary>
    [HttpGet("teacher")]
    [Authorize(Policy = PermissionConstants.TeacherCalendarView)]
    [ProducesResponseType(typeof(ApiResponse<List<TimetableCalendarEventResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<TimetableCalendarEventResponse>>>> GetTeacherCalendar([FromQuery] TimetableCalendarRequest request)
    {
        var response = await _timetableCalendarService.GetTeacherCalendarAsync(request);

        return Success(response);
    }

    // =========================================================
    // Student Calendar
    // =========================================================

    /// <summary>
    /// Returns the calendar for the authenticated student.
    ///
    /// The student is resolved from the authenticated user.
    /// The client must not provide a student ID.
    /// </summary>
    [HttpGet("student")]
    [Authorize(Policy = PermissionConstants.StudentCalendarView)]
    [ProducesResponseType(typeof(ApiResponse<List<TimetableCalendarEventResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<TimetableCalendarEventResponse>>>> GetStudentCalendar([FromQuery] TimetableCalendarRequest request)
    {
        var response = await _timetableCalendarService.GetStudentCalendarAsync(request);

        return Success(response);
    }
}