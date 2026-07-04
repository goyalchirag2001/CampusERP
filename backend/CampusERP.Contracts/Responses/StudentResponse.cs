using CampusERP.Contracts.Enums;

namespace CampusERP.Contracts.Responses;

public class StudentResponse
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid CourseId { get; set; }

    public Guid SemesterId { get; set; }

    public Guid SectionId { get; set; }

    public Guid AcademicSessionId { get; set; }

    public string AdmissionNumber { get; set; } = string.Empty;

    public string RollNumber { get; set; } = string.Empty;

    public string Batch { get; set; } = string.Empty;

    public DateTime AdmissionDate { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; }

    public string CampusName { get; set; } = string.Empty;

    public string DepartmentName { get; set; } = string.Empty;

    public string CourseName { get; set; } = string.Empty;

    public string SemesterName { get; set; } = string.Empty;

    public string SectionName { get; set; } = string.Empty;

    public string AcademicSessionName { get; set; } = string.Empty;

    public EnrollmentStatusDto EnrollmentStatus { get; set; }

    public string EnrollmentStatusName { get; set; } = string.Empty;

    public PromotionStatusDto PromotionStatus { get; set; }

    public string PromotionStatusName { get; set; } = string.Empty;

    public string TemporaryPassword { get; set; } = string.Empty;
}