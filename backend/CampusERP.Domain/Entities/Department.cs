using System.ComponentModel.DataAnnotations;
using CampusERP.Domain.Common;

namespace CampusERP.Domain.Entities;

public class Department : BaseEntity, ITenantEntity
{
    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    public Institution Institution { get; set; } = null!;

    public Campus Campus { get; set; } = null!;

    public ICollection<Course> Courses { get; set; } = new List<Course>();

    public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();

    public ICollection<Student> Students { get; set; } = new List<Student>();

    public ICollection<CalendarEvent> CalendarEvents { get; set; } = new List<CalendarEvent>();

    public bool IsActive { get; set; } = true;
}