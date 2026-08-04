using CampusERP.Application.Common.Models;
using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

/// <summary>
/// Manages attendance correction requests.
/// </summary>
[Authorize]
public class AttendanceCorrectionRequestsController : BaseApiController
{
    private readonly IAttendanceCorrectionRequestService _service;

    public AttendanceCorrectionRequestsController(
        IAttendanceCorrectionRequestService service)
    {
        _service = service;
    }

    #region Queries

    /// <summary>
    /// Returns all attendance correction requests.
    /// </summary>
    [Authorize(Policy = PermissionConstants.AttendanceCorrectionView)]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<AttendanceCorrectionRequestResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<AttendanceCorrectionRequestResponse>>>> GetAll()
    {
        var response = await _service.GetAllAsync();

        return Success(response);
    }

    /// <summary>
    /// Returns an attendance correction request by Id.
    /// </summary>
    [Authorize(Policy = PermissionConstants.AttendanceCorrectionView)]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceCorrectionRequestResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AttendanceCorrectionRequestResponse>>> GetById(Guid id)
    {
        var response = await _service.GetByIdAsync(id);

        return Success(response);
    }

    /// <summary>
    /// Returns all pending attendance correction requests.
    /// </summary>
    [Authorize(Policy = PermissionConstants.AttendanceCorrectionApprove)]
    [HttpGet("pending")]
    [ProducesResponseType(typeof(ApiResponse<List<AttendanceCorrectionRequestResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<AttendanceCorrectionRequestResponse>>>> GetPending()
    {
        var response = await _service.GetPendingAsync();

        return Success(response);
    }

    /// <summary>
    /// Returns attendance correction requests submitted by a student.
    /// </summary>
    [Authorize(Policy = PermissionConstants.AttendanceCorrectionView)]
    [HttpGet("student/{studentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<AttendanceCorrectionRequestResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<AttendanceCorrectionRequestResponse>>>> GetByStudent(Guid studentId)
    {
        var response = await _service.GetByStudentAsync(studentId);

        return Success(response);
    }

    /// <summary>
    /// Returns correction requests for an attendance record.
    /// </summary>
    [Authorize(Policy = PermissionConstants.AttendanceCorrectionView)]
    [HttpGet("attendance/{attendanceRecordId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<AttendanceCorrectionRequestResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<AttendanceCorrectionRequestResponse>>>> GetByAttendanceRecord(Guid attendanceRecordId)
    {
        var response = await _service.GetByAttendanceRecordAsync(attendanceRecordId);

        return Success(response);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates a new attendance correction request.
    /// </summary>
    [Authorize(Policy = PermissionConstants.AttendanceCorrectionCreate)]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AttendanceCorrectionRequestResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AttendanceCorrectionRequestResponse>>> Create(
        [FromBody] CreateAttendanceCorrectionRequest request)
    {
        var response = await _service.CreateAsync(request);

        return Success(response, "Attendance correction request submitted successfully.");
    }

    /// <summary>
    /// Approves an attendance correction request.
    /// </summary>
    [Authorize(Policy = PermissionConstants.AttendanceCorrectionApprove)]
    [HttpPost("{id:guid}/approve")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceCorrectionRequestResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AttendanceCorrectionRequestResponse>>> Approve(
        Guid id,
        [FromBody] ApproveAttendanceCorrectionRequest request)
    {
        var response = await _service.ApproveAsync(id, request);

        return Success(response, "Attendance correction request approved successfully.");
    }

    /// <summary>
    /// Rejects an attendance correction request.
    /// </summary>
    [Authorize(Policy = PermissionConstants.AttendanceCorrectionApprove)]
    [HttpPost("{id:guid}/reject")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceCorrectionRequestResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AttendanceCorrectionRequestResponse>>> Reject(
        Guid id,
        [FromBody] RejectAttendanceCorrectionRequest request)
    {
        var response = await _service.RejectAsync(id, request);

        return Success(response, "Attendance correction request rejected successfully.");
    }

    /// <summary>
    /// Cancels an attendance correction request.
    /// </summary>
    [Authorize(Policy = PermissionConstants.AttendanceCorrectionCreate)]
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<bool>>> Cancel(Guid id)
    {
        await _service.CancelAsync(id);

        return Success(true, "Attendance correction request cancelled successfully.");
    }

    #endregion
}