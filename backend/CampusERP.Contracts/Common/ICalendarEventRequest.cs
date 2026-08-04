using CampusERP.Shared.Enums;

namespace CampusERP.Contracts.Common;

public interface ICalendarEventRequest
{
    Guid AcademicSessionId { get; }

    Guid? CampusId { get; }

    Guid? DepartmentId { get; }

    Guid? CourseId { get; }

    Guid? SemesterId { get; }

    Guid? SectionId { get; }

    Guid? TeacherId { get; }

    Guid? RoomId { get; }

    string Title { get; }

    string? Description { get; }

    EventType EventType { get; }

    DateOnly StartDate { get; }

    DateOnly EndDate { get; }

    TimeOnly? StartTime { get; }

    TimeOnly? EndTime { get; }

    bool IsFullDay { get; }

    bool IsRecurring { get; }

    string? RecurrenceRule { get; }

    int Priority { get; }

    bool AffectsTimetable { get; }
}