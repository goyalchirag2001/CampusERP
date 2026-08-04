using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface IAttendanceCorrectionRequestService
{
    #region Queries

    Task<List<AttendanceCorrectionRequestResponse>> GetAllAsync();

    Task<AttendanceCorrectionRequestResponse?> GetByIdAsync(Guid id);

    Task<List<AttendanceCorrectionRequestResponse>> GetPendingAsync();

    Task<List<AttendanceCorrectionRequestResponse>> GetByStudentAsync(Guid studentId);

    Task<List<AttendanceCorrectionRequestResponse>> GetByAttendanceRecordAsync(Guid attendanceRecordId);

    #endregion

    #region Commands

    Task<AttendanceCorrectionRequestResponse> CreateAsync(CreateAttendanceCorrectionRequest request);

    Task<AttendanceCorrectionRequestResponse> ApproveAsync(Guid id, ApproveAttendanceCorrectionRequest request);

    Task<AttendanceCorrectionRequestResponse> RejectAsync(Guid id, RejectAttendanceCorrectionRequest request);

    Task CancelAsync(Guid id);

    #endregion
}