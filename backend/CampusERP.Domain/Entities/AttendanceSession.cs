using System.ComponentModel.DataAnnotations;
using CampusERP.Domain.Common;
using CampusERP.Shared.Enums;

namespace CampusERP.Domain.Entities;

public class AttendanceSession : BaseEntity, ITenantEntity
{
    #region Tenant

    public Guid InstitutionId { get; set; } 

    public Guid CampusId { get; set; }

    public Guid AcademicSessionId { get; set; }

    #endregion

    #region Source Information

    /// <summary>
    /// Teacher assignment from which this attendance was generated.
    /// Null when attendance is created manually.
    /// </summary>
    public Guid? TeacherAssignmentId { get; set; }

    /// <summary>
    /// Timetable lecture that generated this attendance.
    /// Null for manual attendance.
    /// </summary>
    public Guid? TimetableTemplateId { get; set; }

    /// <summary>
    /// Lecture override responsible for this attendance.
    /// Null when no override exists.
    /// </summary>
    public Guid? LectureOverrideId { get; set; }

    #endregion

    #region Academic Information

    public Guid SubjectId { get; set; }

    public Guid SemesterSubjectId { get; set; }

    public Guid TeacherId { get; set; }

    public Guid SectionId { get; set; }

    public Guid? RoomId { get; set; }

    public LectureType LectureType { get; set; }

    #endregion

    #region Schedule

    public DateOnly AttendanceDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    #endregion

    #region Attendance

    /// <summary>
    /// Indicates whether attendance has actually been marked.
    /// Session may exist but attendance may still be pending.
    /// </summary>
    public bool IsAttendanceMarked { get; set; }

    public AttendanceSessionStatus Status { get; set; }

    public AttendanceSource Source { get; set; }

    #endregion

    #region Locking

    /// <summary>
    /// Locked attendance cannot be modified.
    /// </summary>
    public bool IsLocked { get; set; }

    public Guid? LockedByUserId { get; set; }

    public DateTime? LockedOn { get; set; }

    #endregion

    #region Remarks

    [MaxLength(1000)]
    public string? Remarks { get; set; }

    #endregion

    #region Navigation

    public Institution Institution { get; set; } = null!;

    public Campus Campus { get; set; } = null!;

    public AcademicSession AcademicSession { get; set; } = null!;

    public TeacherAssignment? TeacherAssignment { get; set; }

    public TimetableTemplate? TimetableTemplate { get; set; }

    public LectureOverride? LectureOverride { get; set; }

    public Subject Subject { get; set; } = null!;

    public SemesterSubject SemesterSubject { get; set; } = null!;

    public Teacher Teacher { get; set; } = null!;

    public Section Section { get; set; } = null!;

    public Room? Room { get; set; } = null!;

    public User? LockedByUser { get; set; }

    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();

    public ICollection<AttendanceQrSession> QrSessions { get; set; } = new List<AttendanceQrSession>();

    #endregion
}