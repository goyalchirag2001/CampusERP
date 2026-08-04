using System.ComponentModel.DataAnnotations;

namespace CampusERP.Contracts.Requests;

public class ApproveAttendanceCorrectionRequest
{
    [MaxLength(1000)]
    public string? ReviewRemarks { get; set; }
}