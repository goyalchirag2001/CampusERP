using System.ComponentModel.DataAnnotations;
using CampusERP.Domain.Common;

namespace CampusERP.Domain.Entities;

public class Department : BaseEntity, ITenantEntity
{
    public Guid InstitutionId { get; set; }

    public Institution Institution { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
}