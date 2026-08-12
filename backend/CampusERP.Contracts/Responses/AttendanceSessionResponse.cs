using CampusERP.Shared.Enums;

namespace CampusERP.Contracts.Responses;

public class AttendanceSessionResponse
{
    public Guid Id { get; set; }

    public Guid AcademicSessionId { get; set; }

    public Guid? TeacherAssignmentId { get; set; }

    public Guid? TimetableTemplateId { get; set; }

    public Guid? LectureOverrideId { get; set; }

    public Guid SubjectId { get; set; }

    public Guid SemesterSubjectId { get; set; }

    public Guid TeacherId { get; set; }

    public Guid SectionId { get; set; }

    public Guid? RoomId { get; set; }

    public LectureType LectureType { get; set; }

    public DateOnly AttendanceDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool IsAttendanceMarked { get; set; }

    public AttendanceSessionStatus Status { get; set; }

    public AttendanceSource Source { get; set; }

    public bool IsLocked { get; set; }

    public Guid? LockedByUserId { get; set; }

    public DateTime? LockedOn { get; set; }

    public string? Remarks { get; set; }

    public int TotalStudents { get; set; }

    public int MarkedStudents { get; set; }

    public List<AttendanceRecordResponse> Records { get; set; } = [];
}