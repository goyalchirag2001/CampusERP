using CampusERP.Shared.Enums;

namespace CampusERP.Contracts.Responses;

public class AttendanceRecordResponse
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string? RollNumber { get; set; }

    public AttendanceStatus Status { get; set; }

    public bool IsMarked { get; set; }

    public DateTime? MarkedOn { get; set; }

    public Guid? MarkedByUserId { get; set; }

    public AttendanceMarkingMethod MarkingMethod { get; set; }

    public string? Remarks { get; set; }
}