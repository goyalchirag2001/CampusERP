using System.ComponentModel.DataAnnotations;
using CampusERP.Domain.Common;
using CampusERP.Shared.Enums;

namespace CampusERP.Domain.Entities;

public class TimetableTemplate : BaseEntity, ITenantEntity
{
    #region Tenant

    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    #endregion

    #region Foreign Keys

    public Guid TeacherAssignmentId { get; set; }

    public Guid AcademicSessionId { get; set; }

    public Guid TeacherId { get; set; }

    public Guid SectionId { get; set; }

    public Guid SemesterSubjectId { get; set; }

    public Guid? RoomId { get; set; }

    public Room? Room { get; set; }

    #endregion

    #region Schedule

    public DayOfWeekType DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public DateOnly ValidFrom { get; set; }

    public DateOnly ValidTo { get; set; }

    #endregion

    #region Lecture Details

    public LectureType LectureType { get; set; }

    public int Priority { get; set; } = 100;

    public bool GenerateAttendance { get; set; } = true;

    public bool IsOnline { get; set; }

    [MaxLength(500)]
    public string? MeetingLink { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }

    public int DisplayOrder { get; set; }

    #endregion

    #region Status

    public bool IsActive { get; set; } = true;

    #endregion

    #region Navigation

    public Institution Institution { get; set; } = null!;

    public Campus Campus { get; set; } = null!;

    public AcademicSession AcademicSession { get; set; } = null!;

    public TeacherAssignment TeacherAssignment { get; set; } = null!;

    public Teacher Teacher { get; set; } = null!;

    public Section Section { get; set; } = null!;

    public SemesterSubject SemesterSubject { get; set; } = null!;

    public ICollection<LectureOverride> LectureOverrides { get; set; } = new List<LectureOverride>();

    #endregion
}