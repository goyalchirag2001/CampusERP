using System.ComponentModel.DataAnnotations;
using CampusERP.Domain.Common;

namespace CampusERP.Domain.Entities;

public class Student : BaseEntity, ITenantEntity
{
    public Guid UserId { get; set; }

    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public Institution Institution { get; set; } = null!;

    public Campus Campus { get; set; } = null!;

    public User User { get; set; } = null!;

    [Required]
    [MaxLength(30)]
    public string AdmissionNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string RollNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Batch { get; set; } = string.Empty;

    public DateTime AdmissionDate { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<StudentEnrollment> Enrollments { get; set; } = new List<StudentEnrollment>();

    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();

    public ICollection<AttendanceCorrectionRequest> AttendanceCorrectionRequests { get; set; } = new List<AttendanceCorrectionRequest>();
}