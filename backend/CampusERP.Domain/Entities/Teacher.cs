using System.ComponentModel.DataAnnotations;
using CampusERP.Domain.Common;

namespace CampusERP.Domain.Entities;

public class Teacher : BaseEntity, ITenantEntity
{
    public Guid UserId { get; set; }

    public Guid InstitutionId { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid CampusId { get; set; }

    [Required]
    [MaxLength(20)]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Designation { get; set; } = string.Empty;

    public User User { get; set; } = null!;

    public Institution Institution { get; set; } = null!;

    public Department Department { get; set; } = null!;

    public Campus Campus { get; set; } = null!;

    public bool IsActive { get; set; }

    public ICollection<TeacherAssignment> TeacherAssignments { get; set; } = new List<TeacherAssignment>();

    public ICollection<CalendarEvent> CalendarEvents { get; set; } = new List<CalendarEvent>();

    public ICollection<LectureOverride> OriginalLectureOverrides { get; set; } = new List<LectureOverride>();

    public ICollection<LectureOverride> NewLectureOverrides { get; set; } = new List<LectureOverride>();

    public ICollection<TimetableTemplate> TimetableTemplates { get; set; } = new List<TimetableTemplate>();
}