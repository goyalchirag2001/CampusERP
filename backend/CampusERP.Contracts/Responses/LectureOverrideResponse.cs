using CampusERP.Shared.Enums;

namespace CampusERP.Contracts.Responses;

public class LectureOverrideResponse
{
    public Guid Id { get; set; }

    public Guid TimetableTemplateId { get; set; }

    public Guid? CalendarEventId { get; set; }

    public DateOnly OverrideDate { get; set; }

    public OverrideType OverrideType { get; set; }

    public string? Reason { get; set; }

    public bool IsApproved { get; set; }

    public bool GenerateAttendance { get; set; }

    public bool IsSystemGenerated { get; set; }
}