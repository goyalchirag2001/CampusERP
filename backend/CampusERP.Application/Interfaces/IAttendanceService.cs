using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface IAttendanceService
{
    Task<AttendanceSessionResponse> CreateSessionAsync(CreateAttendanceSessionRequest request);

    Task<AttendanceSessionResponse> GetSessionByIdAsync(Guid id);

    Task<List<AttendanceSessionResponse>> GetTeacherSessionsAsync(DateOnly startDate, DateOnly endDate);

    Task<AttendanceSessionResponse> MarkAttendanceAsync(MarkAttendanceRequest request);

    Task<AttendanceSessionResponse> MarkAttendanceBulkAsync(MarkAttendanceBulkRequest request);

    Task<AttendanceSessionResponse> CompleteSessionAsync(CompleteAttendanceSessionRequest request);

    Task<AttendanceSessionResponse> LockSessionAsync(Guid attendanceSessionId);

    Task<AttendanceQrSessionResponse> StartQrAttendanceAsync(StartAttendanceQrRequest request);

    Task<AttendanceQrSessionResponse> GetActiveQrSessionAsync(Guid attendanceSessionId);

    Task<AttendanceQrScanResponse> ScanAttendanceQrAsync(ScanAttendanceQrRequest request);

    Task<AttendanceQrSessionResponse> CloseQrAttendanceAsync(Guid attendanceSessionId);

    Task ExpireQrAttendanceSessionsAsync();
}