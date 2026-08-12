namespace CampusERP.Contracts.Responses;

public class AttendanceQrScanResponse
{
    public bool Success { get; set; }

    public Guid AttendanceSessionId { get; set; }

    public Guid AttendanceRecordId { get; set; }

    public string Message { get; set; } = string.Empty;

    public DateTime MarkedOn { get; set; }
}