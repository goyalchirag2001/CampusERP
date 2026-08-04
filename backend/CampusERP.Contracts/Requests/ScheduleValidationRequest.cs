using CampusERP.Shared.Enums;

namespace CampusERP.Contracts.Requests;

public class ScheduleValidationRequest
{
    public Guid? CalendarEventId { get; set; }

    public Guid AcademicSessionId { get; set; }

    public Guid CampusId { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? CourseId { get; set; }

    public Guid? SemesterId { get; set; }

    public Guid? SectionId { get; set; }

    public Guid? TeacherId { get; set; }

    public Guid? RoomId { get; set; }

    public string Title { get; set; } = string.Empty;

    public EventType EventType { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public bool IsFullDay { get; set; }

    public int Priority { get; set; }

    public bool AffectsTimetable { get; set; }
}