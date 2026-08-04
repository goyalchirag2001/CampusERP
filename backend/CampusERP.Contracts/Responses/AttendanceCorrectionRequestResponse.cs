using CampusERP.Shared.Enums;

namespace CampusERP.Contracts.Responses;

public class AttendanceCorrectionRequestResponse
{
    #region Basic

    public Guid Id { get; set; }

    public Guid AttendanceRecordId { get; set; }

    public Guid StudentId { get; set; }

    #endregion

    #region Request

    public AttendanceCorrectionReason Reason { get; set; }

    public string ReasonName { get; set; } = string.Empty;

    public AttendanceStatus OriginalStatus { get; set; }

    public string OriginalStatusName { get; set; } = string.Empty;

    public AttendanceStatus RequestedStatus { get; set; }

    public string RequestedStatusName { get; set; } = string.Empty;

    public AttendanceCorrectionStatus Status { get; set; }

    public string StatusName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? AttachmentPath { get; set; }

    #endregion

    #region Review

    public Guid? ReviewedByUserId { get; set; }

    public string? ReviewedByName { get; set; }

    public DateTime? ReviewedOn { get; set; }

    public string? ReviewRemarks { get; set; }

    #endregion

    #region Processing

    public bool IsProcessed { get; set; }

    public DateTime? ProcessedOn { get; set; }

    public DateTime? AttendanceUpdatedOn { get; set; }

    #endregion

    #region Audit

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    #endregion
}