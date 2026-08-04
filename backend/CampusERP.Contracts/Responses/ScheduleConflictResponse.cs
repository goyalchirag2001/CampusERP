namespace CampusERP.Contracts.Responses;

public class ScheduleConflictResponse
{
    public Guid? CalendarEventId { get; set; }

    public Guid? TimetableTemplateId { get; set; }

    public string ConflictType { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public Guid? TeacherId { get; set; }

    public string? TeacherName { get; set; }

    public Guid? RoomId { get; set; }

    public string? RoomName { get; set; }

    public Guid? SectionId { get; set; }

    public string? SectionName { get; set; }

    public Guid? SemesterSubjectId { get; set; }

    public string? SubjectName { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public bool CanOverride { get; set; }

    public int ExistingPriority { get; set; }

    public int RequestedPriority { get; set; }

    public string SuggestedAction { get; set; } = string.Empty;
}