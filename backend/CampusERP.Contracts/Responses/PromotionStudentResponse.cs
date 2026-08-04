namespace CampusERP.Contracts.Responses;

public class PromotionStudentResponse
{
    public Guid StudentId { get; set; }

    public string AdmissionNumber { get; set; } = string.Empty;

    public string RollNumber { get; set; } = string.Empty;

    public string StudentName { get; set; } = string.Empty;

    public Guid CurrentSectionId { get; set; }

    public string CurrentSectionName { get; set; } = string.Empty;

    public Guid NextSemesterId { get; set; }

    public string NextSemesterName { get; set; } = string.Empty;

    public Guid NextSectionId { get; set; }

    public Guid CurrentEnrollmentId { get; set; }

    public bool IsGraduating { get; set; }
}