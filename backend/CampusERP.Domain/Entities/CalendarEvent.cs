using System.ComponentModel.DataAnnotations;
using CampusERP.Domain.Common;
using CampusERP.Shared.Enums;

namespace CampusERP.Domain.Entities;

public class CalendarEvent : BaseEntity, ITenantEntity
{
    #region Foreign Keys

    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public Guid AcademicSessionId { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? CourseId { get; set; }

    public Guid? SemesterId { get; set; }

    public Guid? SectionId { get; set; }

    public Guid? TeacherId { get; set; }

    public Guid? RoomId { get; set; }

    #endregion

    #region Basic Information

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public EventType EventType { get; set; }

    #endregion

    #region Schedule

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public bool IsFullDay { get; set; }

    #endregion

    #region Recurrence

    public bool IsRecurring { get; set; }

    [MaxLength(500)]
    public string? RecurrenceRule { get; set; }

    #endregion

    #region Timetable

    public bool AffectsTimetable { get; set; } = true;

    public int Priority { get; set; }

    #endregion

    #region Appearance

    [MaxLength(20)]
    public string? Color { get; set; }

    #endregion

    #region Status

    public bool IsActive { get; set; } = true;

    #endregion

    #region Navigation Properties

    public Institution Institution { get; set; } = null!;

    public Campus Campus { get; set; } = null!;

    public AcademicSession AcademicSession { get; set; } = null!;

    public Department? Department { get; set; }

    public Course? Course { get; set; }

    public Semester? Semester { get; set; }

    public Section? Section { get; set; }

    public Teacher? Teacher { get; set; }

    public Room? Room { get; set; }

    public ICollection<LectureOverride> LectureOverrides { get; set; } = new List<LectureOverride>();

    #endregion
}