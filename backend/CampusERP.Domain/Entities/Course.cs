using System.ComponentModel.DataAnnotations;
using CampusERP.Domain.Common;

namespace CampusERP.Domain.Entities;

public class Course : BaseEntity, ITenantEntity
{
    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public Guid DepartmentId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 10)]
    public int DurationYears { get; set; }

    public Institution Institution { get; set; } = null!;

    public Campus Campus { get; set; } = null!;

    public Department Department { get; set; } = null!;

    public ICollection<Student> Students { get; set; }
        = new List<Student>();
}