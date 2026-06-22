using System.ComponentModel.DataAnnotations;
using CampusERP.Domain.Common;
using CampusERP.Domain.Enums;

namespace CampusERP.Domain.Entities;

public class Subject : BaseEntity, ITenantEntity
{
    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    [Required]
    [MaxLength(30)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public int Credits { get; set; }

    public SubjectType SubjectType { get; set; }

    public Institution Institution { get; set; } = null!;

    public Campus Campus { get; set; } = null!;

    public bool IsActive { get; set; }

    public ICollection<SemesterSubject> SemesterSubjects { get; set; } = new List<SemesterSubject>();
}