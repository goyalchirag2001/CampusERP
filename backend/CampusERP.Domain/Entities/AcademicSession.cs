using System.ComponentModel.DataAnnotations;
using CampusERP.Domain.Common;

namespace CampusERP.Domain.Entities;

public class AcademicSession : BaseEntity
{
    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool IsCurrent { get; set; }

    public bool IsActive { get; set; } = true;

    public Institution Institution { get; set; } = null!;

    public Campus Campus { get; set; } = null!;

    public ICollection<StudentEnrollment> StudentEnrollments { get; set; } = new List<StudentEnrollment>();
}