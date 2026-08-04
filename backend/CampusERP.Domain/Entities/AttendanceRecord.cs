using System.ComponentModel.DataAnnotations;
using CampusERP.Domain.Common;
using CampusERP.Shared.Enums;

namespace CampusERP.Domain.Entities;

public class AttendanceRecord : BaseEntity, ITenantEntity
{
    #region Tenant

    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    #endregion

    #region Foreign Keys

    public Guid AttendanceSessionId { get; set; }

    public Guid StudentId { get; set; }

    #endregion

    #region Attendance

    /// <summary>
    /// Indicates whether attendance has been marked by the teacher.
    /// </summary>
    public bool IsMarked { get; set; }

    /// <summary>
    /// Final attendance result.
    /// Meaningful only when IsMarked = true.
    /// </summary>
    public AttendanceStatus Status { get; set; }

    /// <summary>
    /// Time at which attendance was marked.
    /// </summary>
    public DateTime? MarkedOn { get; set; }

    /// <summary>
    /// User (typically teacher) who marked attendance.
    /// </summary>
    public Guid? MarkedByUserId { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }

    #endregion

    #region Navigation

    public AttendanceSession AttendanceSession { get; set; } = null!;

    public Student Student { get; set; } = null!;

    public User? MarkedByUser { get; set; }

    public ICollection<AttendanceCorrectionRequest> CorrectionRequests { get; set; } = new List<AttendanceCorrectionRequest>();

    #endregion
}