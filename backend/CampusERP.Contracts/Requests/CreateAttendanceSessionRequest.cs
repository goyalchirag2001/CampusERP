using System.ComponentModel.DataAnnotations;

namespace CampusERP.Contracts.Requests;

public class CreateAttendanceSessionRequest
{
    [Required]
    public Guid TimetableTemplateId { get; set; }

    [Required]
    public DateOnly AttendanceDate { get; set; }

    /// <summary>
    /// Optional approved lecture override.
    /// When supplied, the attendance session is generated
    /// from the effective overridden lecture.
    /// </summary>
    public Guid? LectureOverrideId { get; set; }

    [MaxLength(1000)]
    public string? Remarks { get; set; }
}