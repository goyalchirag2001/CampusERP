using CampusERP.Shared.Enums;

namespace CampusERP.Contracts.Responses;

public class TimetableTemplateResponse
{
    public Guid Id { get; set; }

    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public string CampusName { get; set; } = string.Empty;

    public Guid AcademicSessionId { get; set; }

    public string AcademicSessionName { get; set; } = string.Empty;

    public Guid TeacherAssignmentId { get; set; }

    public Guid TeacherId { get; set; }

    public string TeacherName { get; set; } = string.Empty;

    public Guid SectionId { get; set; }

    public string SectionName { get; set; } = string.Empty;

    public Guid SemesterSubjectId { get; set; }

    public Guid SubjectId { get; set; }

    public string SubjectCode { get; set; } = string.Empty;

    public string SubjectName { get; set; } = string.Empty;

    public Guid RoomId { get; set; }

    public string RoomName { get; set; } = string.Empty;

    public DayOfWeekType DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public DateOnly ValidFrom { get; set; }

    public DateOnly ValidTo { get; set; }

    public LectureType LectureType { get; set; }

    public int Priority { get; set; }

    public bool GenerateAttendance { get; set; }

    public bool IsOnline { get; set; }

    public string? MeetingLink { get; set; }

    public string? Remarks { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }
}