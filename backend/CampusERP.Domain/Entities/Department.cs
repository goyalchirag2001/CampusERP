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

    public Institution Institution { get; set; } = null!;

    public Campus Campus { get; set; } = null!;

    public ICollection<Course> Courses { get; set; }
        = new List<Course>();

    public ICollection<Teacher> Teachers { get; set; }
        = new List<Teacher>();
}