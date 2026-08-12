using CampusERP.Shared.Enums;

namespace CampusERP.Contracts.Requests;

public class MarkAttendanceRequest
{
    public Guid AttendanceRecordId { get; set; }

    public AttendanceStatus Status { get; set; }

    public string? Remarks { get; set; }
}