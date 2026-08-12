using CampusERP.Shared.Enums;

namespace CampusERP.Contracts.Requests;

public class MarkAttendanceBulkRequest
{
    public Guid AttendanceSessionId { get; set; }

    public List<AttendanceMarkItem> Records { get; set; } = [];
}

public class AttendanceMarkItem
{
    public Guid AttendanceRecordId { get; set; }

    public AttendanceStatus Status { get; set; }

    public string? Remarks { get; set; }
}