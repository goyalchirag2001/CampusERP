using System.ComponentModel.DataAnnotations;
using CampusERP.Domain.Common;

namespace CampusERP.Domain.Entities;

public class Section : BaseEntity, ITenantEntity
{
    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid CourseId { get; set; }

    public Guid SemesterId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Name { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public bool IsActive { get; set; } = true;

    public Institution Institution { get; set; } = null!;

    public Campus Campus { get; set; } = null!;

    public Department Department { get; set; } = null!;

    public Course Course { get; set; } = null!;

    public Semester Semester { get; set; } = null!;

    public ICollection<StudentEnrollment> StudentEnrollments { get; set; } = new List<StudentEnrollment>();

    public ICollection<TeacherAssignment> TeacherAssignments { get; set; } = new List<TeacherAssignment>();

    public ICollection<CalendarEvent> CalendarEvents { get; set; } = new List<CalendarEvent>();

    public ICollection<TimetableTemplate> TimetableTemplates { get; set; } = new List<TimetableTemplate>();
}