using System.ComponentModel.DataAnnotations;
using CampusERP.Shared.Enums;

namespace CampusERP.Contracts.Requests;

public class CreateAttendanceCorrectionRequest
{
    [Required]
    public Guid AttendanceRecordId { get; set; }

    [Required]
    public AttendanceCorrectionReason Reason { get; set; }

    [Required]
    public AttendanceStatus RequestedStatus { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// File path returned after upload.
    /// Null when no attachment is required.
    /// </summary>
    [MaxLength(500)]
    public string? AttachmentPath { get; set; }
}