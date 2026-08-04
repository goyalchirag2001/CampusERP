using CampusERP.Shared.Enums;

namespace CampusERP.Contracts.Responses;

public class CalendarEventResponse
{
    public Guid Id { get; set; }

    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public string CampusName { get; set; } = string.Empty;

    public Guid? DepartmentId { get; set; }

    public string? DepartmentName { get; set; }

    public Guid? CourseId { get; set; }

    public string? CourseName { get; set; }

    public Guid? SemesterId { get; set; }

    public string? SemesterName { get; set; }

    public Guid? SectionId { get; set; }

    public string? SectionName { get; set; }

    public Guid? TeacherId { get; set; }

    public string? TeacherName { get; set; }

    public Guid? RoomId { get; set; }

    public string? RoomName { get; set; }

    public Guid AcademicSessionId { get; set; }

    public string AcademicSessionName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public EventType EventType { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public bool IsFullDay { get; set; }

    public bool IsRecurring { get; set; }

    public string? RecurrenceRule { get; set; }

    public int Priority { get; set; }

    public bool AffectsTimetable { get; set; }

    public bool IsActive { get; set; }
}