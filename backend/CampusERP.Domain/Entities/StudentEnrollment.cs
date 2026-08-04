using CampusERP.Domain.Common;
using CampusERP.Shared.Enums;

namespace CampusERP.Domain.Entities;

public class StudentEnrollment : BaseEntity
{
    public Guid StudentId { get; set; }

    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid CourseId { get; set; }

    public Guid SemesterId { get; set; }

    public Guid SectionId { get; set; }

    public Guid AcademicSessionId { get; set; }

    public EnrollmentStatus EnrollmentStatus { get; set; }

    public PromotionStatus PromotionStatus { get; set; }

    public bool IsCurrent { get; set; }

    public Student Student { get; set; } = null!;

    public Department Department { get; set; } = null!;

    public Course Course { get; set; } = null!;

    public Semester Semester { get; set; } = null!;

    public Section Section { get; set; } = null!;

    public AcademicSession AcademicSession { get; set; } = null!;

    public Institution Institution { get; set; } = null!;

    public Campus Campus { get; set; } = null!;
}