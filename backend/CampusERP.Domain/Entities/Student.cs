using System.ComponentModel.DataAnnotations;
using CampusERP.Domain.Common;

namespace CampusERP.Domain.Entities;

public class Student : BaseEntity, ITenantEntity
{
    public Guid UserId { get; set; }

    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public Institution Institution { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    public string RollNumber { get; set; } = string.Empty;

    public Guid CourseId { get; set; }

    public Guid DepartmentId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Batch { get; set; } = string.Empty;

    public DateTime AdmissionDate { get; set; }

    public User User { get; set; } = null!;

    public Campus Campus { get; set; } = null!;

    public Course Course { get; set; } = null!;

    public Department Department { get; set; } = null!;
}