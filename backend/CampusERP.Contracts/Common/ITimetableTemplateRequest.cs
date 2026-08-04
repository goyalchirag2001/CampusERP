using CampusERP.Shared.Enums;

namespace CampusERP.Contracts.Common;

public interface ITimetableTemplateRequest
{
    Guid TeacherAssignmentId { get; }

    Guid AcademicSessionId { get; }

    Guid? RoomId { get; }

    DayOfWeekType DayOfWeek { get; }

    TimeOnly StartTime { get; }

    TimeOnly EndTime { get; }

    DateOnly ValidFrom { get; }

    DateOnly ValidTo { get; }

    LectureType LectureType { get; }

    int Priority { get; }

    bool GenerateAttendance { get; }

    bool IsOnline { get; }

    string? MeetingLink { get; }

    string? Remarks { get; }

    int DisplayOrder { get; }
}