using CampusERP.Contracts.Common;
using CampusERP.Shared.Enums;

namespace CampusERP.Contracts.Requests;

public class UpdateTimetableTemplateRequest : ITimetableTemplateRequest
{
    public Guid TeacherAssignmentId { get; set; }

    public Guid AcademicSessionId { get; set; }

    public Guid? RoomId { get; set; }

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
}