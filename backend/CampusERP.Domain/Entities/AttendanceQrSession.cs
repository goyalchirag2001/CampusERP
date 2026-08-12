using System.ComponentModel.DataAnnotations;
using CampusERP.Domain.Common;

namespace CampusERP.Domain.Entities;

public class AttendanceQrSession : BaseEntity, ITenantEntity
{
    #region Tenant

    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    #endregion

    #region Attendance

    public Guid AttendanceSessionId { get; set; }

    #endregion

    #region QR Security

    /// <summary>
    /// Random opaque token used by the QR code.
    /// This is not the attendance session ID.
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// UTC time when the QR became valid.
    /// </summary>
    public DateTime ValidFrom { get; set; }

    /// <summary>
    /// UTC time after which the QR is invalid.
    /// </summary>
    public DateTime ExpiresOn { get; set; }

    #endregion

    #region Status

    public bool IsActive { get; set; }

    public DateTime? ClosedOn { get; set; }

    public Guid CreatedByUserId { get; set; }

    #endregion

    #region Navigation

    public AttendanceSession AttendanceSession { get; set; } = null!;

    public User CreatedByUser { get; set; } = null!;

    #endregion
}