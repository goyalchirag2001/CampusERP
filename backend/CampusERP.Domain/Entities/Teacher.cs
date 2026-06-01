using System.ComponentModel.DataAnnotations;
using CampusERP.Domain.Common;

namespace CampusERP.Domain.Entities;

public class Teacher : BaseEntity, ITenantEntity
{
    public Guid UserId { get; set; }

    public Guid InstitutionId { get; set; }

    public Institution Institution { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    public string EmployeeCode { get; set; } = string.Empty;

    public Guid DepartmentId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Designation { get; set; } = string.Empty;

    public User User { get; set; } = null!;

    public Department Department { get; set; } = null!;
}