using CampusERP.Shared.Enums;

namespace CampusERP.Contracts.Responses;

public class TimetableCalendarEventResponse
{
    public Guid Id { get; set; }

    public Guid? TimetableTemplateId { get; set; }

    public Guid? CalendarEventId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public EventType? EventType { get; set; }

    public string? SubjectCode { get; set; }

    public string? SubjectName { get; set; }

    public Guid? TeacherId { get; set; }

    public string? TeacherName { get; set; }

    public Guid? SectionId { get; set; }

    public string? SectionName { get; set; }

    public Guid? RoomId { get; set; }

    public string? RoomBuilding { get; set; }

    public string? RoomFloor { get; set; }

    public string? RoomNumber { get; set; }

    public string? RoomName { get; set; }

    public LectureType? LectureType { get; set; }

    public int Priority { get; set; }

    public bool GenerateAttendance { get; set; }

    public bool IsOnline { get; set; }

    public string? MeetingLink { get; set; }

    public bool IsFullDay { get; set; }

    public string? Color { get; set; }

    public bool IsOverride { get; set; }

    public bool IsCancelled { get; set; }

    public string? OverrideReason { get; set; }
}