using System.ComponentModel.DataAnnotations;
using CampusERP.Domain.Common;

namespace CampusERP.Domain.Entities;

public class Semester : BaseEntity, ITenantEntity
{
    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public Guid CourseId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public int SequenceNumber { get; set; }

    public int YearNumber { get; set; }

    public bool IsActive { get; set; } = true;

    public Institution Institution { get; set; } = null!;

    public Campus Campus { get; set; } = null!;

    public Course Course { get; set; } = null!;

    public ICollection<SemesterSubject> SemesterSubjects { get; set; } = new List<SemesterSubject>();

    public ICollection<Section> Sections { get; set; } = new List<Section>();

    public ICollection<CalendarEvent> CalendarEvents { get; set; } = new List<CalendarEvent>();
}