namespace CampusERP.Contracts.Requests;

public class StartAttendanceQrRequest
{
    public Guid AttendanceSessionId { get; set; }

    /// <summary>
    /// Requested QR validity duration in seconds.
    /// </summary>
    public int DurationSeconds { get; set; }
}