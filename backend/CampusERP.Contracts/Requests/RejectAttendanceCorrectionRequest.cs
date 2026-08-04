using System.ComponentModel.DataAnnotations;

namespace CampusERP.Contracts.Requests;

public class RejectAttendanceCorrectionRequest
{
    [Required]
    [MaxLength(1000)]
    public string ReviewRemarks { get; set; } = string.Empty;
}