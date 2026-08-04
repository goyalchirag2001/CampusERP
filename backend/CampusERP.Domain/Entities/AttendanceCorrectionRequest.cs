using System.ComponentModel.DataAnnotations;
using CampusERP.Domain.Common;
using CampusERP.Shared.Enums;

namespace CampusERP.Domain.Entities;

public class AttendanceCorrectionRequest : BaseEntity, ITenantEntity
{
    #region Tenant

    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    #endregion

    #region Related Records

    public Guid AttendanceRecordId { get; set; }

    public Guid StudentId { get; set; }

    #endregion

    #region Request

    public AttendanceCorrectionReason Reason { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public AttendanceStatus RequestedStatus { get; set; }

    public AttendanceCorrectionStatus Status { get; set; } = AttendanceCorrectionStatus.Pending;

    #endregion

    #region Review

    public Guid? ReviewedByUserId { get; set; }

    public DateTime? ReviewedOn { get; set; }

    [MaxLength(1000)]
    public string? ReviewRemarks { get; set; }

    #endregion

    #region Processing

    public bool IsProcessed { get; set; }

    public DateTime? ProcessedOn { get; set; }

    public DateTime? AttendanceUpdatedOn { get; set; }

    #endregion

    #region Navigation

    public AttendanceRecord AttendanceRecord { get; set; } = null!;

    public Student Student { get; set; } = null!;

    public User? ReviewedByUser { get; set; }

    public AttendanceStatus OriginalStatus { get; set; }

    [MaxLength(500)]
    public string? AttachmentPath { get; set; }

    #endregion
}