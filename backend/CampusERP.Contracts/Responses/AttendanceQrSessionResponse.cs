namespace CampusERP.Contracts.Responses;

public class AttendanceQrSessionResponse
{
    public Guid Id { get; set; }

    public Guid AttendanceSessionId { get; set; }

    public string Token { get; set; } = string.Empty;

    public DateTime ValidFrom { get; set; }

    public DateTime ExpiresOn { get; set; }

    public int DurationSeconds { get; set; }

    public bool IsActive { get; set; }

    public int MarkedCount { get; set; }

    public int TotalStudentCount { get; set; }

    public int RemainingStudentCount { get; set; }
}