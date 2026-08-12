namespace CampusERP.Contracts.Requests;

public class CompleteAttendanceSessionRequest
{
    public Guid AttendanceSessionId { get; set; }

    public string? Remarks { get; set; }
}