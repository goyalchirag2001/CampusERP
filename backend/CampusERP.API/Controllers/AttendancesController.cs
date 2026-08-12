using CampusERP.Application.Common.Models;
using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

/// <summary>
/// Manages attendance sessions and attendance records.
/// </summary>
[Authorize]
public class AttendancesController : BaseApiController
{
    private readonly IAttendanceService _attendanceService;

    public AttendancesController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    #region Queries

    /// <summary>
    /// Returns an attendance session by Id.
    /// </summary>
    [Authorize(Policy = PermissionConstants.AttendanceView)]
    [HttpGet("sessions/{id:guid}")]
    [ProducesResponseType(
        typeof(ApiResponse<AttendanceSessionResponse>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AttendanceSessionResponse>>> GetSession(Guid id)
    {
        var response = await _attendanceService.GetSessionByIdAsync(id);

        return Success(response);
    }

    /// <summary>
    /// Returns attendance sessions for the authenticated teacher.
    /// </summary>
    [Authorize(Policy = PermissionConstants.AttendanceView)]
    [HttpGet("sessions/teacher")]
    [ProducesResponseType(typeof(ApiResponse<List<AttendanceSessionResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<AttendanceSessionResponse>>>> GetTeacherSessions([FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
    {
        var response = await _attendanceService.GetTeacherSessionsAsync(startDate, endDate);

        return Success(response);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Creates an attendance session for a timetable lecture occurrence.
    /// </summary>
    [Authorize(Policy = PermissionConstants.AttendanceManage)]
    [HttpPost("sessions")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceSessionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AttendanceSessionResponse>>> CreateSession([FromBody] CreateAttendanceSessionRequest request)
    {
        var response = await _attendanceService.CreateSessionAsync(request);

        return Success(response, "Attendance session created successfully.");
    }

    #endregion

    [Authorize(Policy = PermissionConstants.AttendanceManage)]
    [HttpPut("records")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceSessionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AttendanceSessionResponse>>> MarkAttendance([FromBody] MarkAttendanceRequest request)
    {
        var response = await _attendanceService.MarkAttendanceAsync(request);

        return Success(response, "Attendance marked successfully.");
    }

    [Authorize(Policy = PermissionConstants.AttendanceManage)]
    [HttpPut("records/bulk")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceSessionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AttendanceSessionResponse>>> MarkAttendanceBulk([FromBody] MarkAttendanceBulkRequest request)
    {
        var response = await _attendanceService.MarkAttendanceBulkAsync(request);

        return Success(response, "Attendance marked successfully.");
    }

    [Authorize(Policy = PermissionConstants.AttendanceManage)]
    [HttpPost("sessions/complete")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceSessionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AttendanceSessionResponse>>> CompleteSession([FromBody] CompleteAttendanceSessionRequest request)
    {
        var response = await _attendanceService.CompleteSessionAsync(request);

        return Success(response, "Attendance session completed successfully.");
    }

    [Authorize(Policy = PermissionConstants.AttendanceManage)]
    [HttpPost("sessions/{id:guid}/lock")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceSessionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AttendanceSessionResponse>>> LockSession(Guid id)
    {
        var response = await _attendanceService.LockSessionAsync(id);

        return Success(response, "Attendance session locked successfully.");
    }

    [Authorize(Policy = PermissionConstants.AttendanceManage)]
    [HttpPost("qr/start")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceQrSessionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AttendanceQrSessionResponse>>> StartQr([FromBody] StartAttendanceQrRequest request)
    {
        var response = await _attendanceService.StartQrAttendanceAsync(request);

        return Success(response, "QR attendance started successfully.");
    }

    [Authorize(Policy = PermissionConstants.AttendanceManage)]
    [HttpGet("sessions/{id:guid}/qr")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceQrSessionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AttendanceQrSessionResponse>>> GetActiveQr(Guid id)
    {
        var response = await _attendanceService.GetActiveQrSessionAsync(id);

        return Success(response);
    }

    [Authorize(Policy = PermissionConstants.AttendanceStudentMark)]
    [HttpPost("qr/scan")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceQrScanResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AttendanceQrScanResponse>>> ScanQr([FromBody] ScanAttendanceQrRequest request)
    {
        var response = await _attendanceService.ScanAttendanceQrAsync(request);

        return Success(response, "Attendance marked successfully.");
    }

    [Authorize(Policy = PermissionConstants.AttendanceManage)]
    [HttpPost("sessions/{id:guid}/qr/close")]
    [ProducesResponseType(typeof(ApiResponse<AttendanceQrSessionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AttendanceQrSessionResponse>>> CloseQr(Guid id)
    {
        var response = await _attendanceService.CloseQrAttendanceAsync(id);

        return Success(response, "QR attendance closed successfully.");
    }
}