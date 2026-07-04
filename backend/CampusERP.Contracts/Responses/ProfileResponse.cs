namespace CampusERP.Contracts.Responses;

public class ProfileResponse
{
    public Guid UserId { get; set; }

    public Guid? StudentId { get; set; }

    public Guid? TeacherId { get; set; }

    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string InstitutionName { get; set; } = string.Empty;

    public string CampusName { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public string? ProfilePhotoUrl { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public DateTime? CurrentLoginAt { get; set; }

    // Student

    public string? AdmissionNumber { get; set; }

    public string? RollNumber { get; set; }

    public string? AcademicSession { get; set; }

    public string? CourseName { get; set; }

    public string? DepartmentName { get; set; }

    public string? SemesterName { get; set; }

    public string? SectionName { get; set; }

    public int? EnrollmentStatus { get; set; }

    public string? EnrollmentStatusName { get; set; }

    // Teacher

    public string? EmployeeCode { get; set; }

    public string? Designation { get; set; }

    public string AvatarInitials { get; set; } = string.Empty;
}