using CampusERP.Application.Common.Models;
using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

/// <summary>
/// Manages academic calendar events.
/// </summary>
[Authorize]
public class CalendarEventsController : BaseApiController
{
    private readonly ICalendarEventService _service;

    public CalendarEventsController(ICalendarEventService service)
    {
        _service = service;
    }

    #region Queries

    /// <summary>
    /// Returns all calendar events.
    /// </summary>
    [Authorize(Policy = PermissionConstants.CalendarView)]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<CalendarEventResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<CalendarEventResponse>>>> GetAll()
    {
        var response = await _service.GetAllAsync();

        return Success(response);
    }

    /// <summary>
    /// Returns a calendar event by Id.
    /// </summary>
    [Authorize(Policy = PermissionConstants.CalendarView)]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CalendarEventResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<CalendarEventResponse>>> GetById(Guid id)
    {
        var response = await _service.GetByIdAsync(id);

        return Success(response);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates a calendar event.
    /// </summary>
    [Authorize(Policy = PermissionConstants.CalendarManage)]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CalendarEventResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<CalendarEventResponse>>> Create([FromBody] CreateCalendarEventRequest request)
    {
        var response = await _service.CreateAsync(request);

        return Success(response, "Calendar event created successfully.");
    }

    /// <summary>
    /// Updates a calendar event.
    /// </summary>
    [Authorize(Policy = PermissionConstants.CalendarManage)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CalendarEventResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<CalendarEventResponse>>> Update(Guid id, [FromBody] UpdateCalendarEventRequest request)
    {
        var response = await _service.UpdateAsync(id, request);

        return Success(response, "Calendar event updated successfully.");
    }

    /// <summary>
    /// Activates a calendar event.
    /// </summary>
    [Authorize(Policy = PermissionConstants.CalendarManage)]
    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<string>>> Activate(Guid id)
    {
        await _service.ActivateAsync(id);

        return Success(
            new
            {
                Activated = true
            },
            "Calendar event activated successfully.");
    }

    /// <summary>
    /// Deactivates a calendar event.
    /// </summary>
    [Authorize(Policy = PermissionConstants.CalendarManage)]
    [HttpPost("{id:guid}/deactivate")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<string>>> Deactivate(Guid id)
    {
        await _service.DeactivateAsync(id);

        return Success("Success", "Calendar event deactivated successfully.");
    }

    #endregion
}