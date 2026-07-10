namespace CampusERP.Contracts.Responses;

public class StudentImportCredential
{
    public string AdmissionNumber { get; set; } = string.Empty;

    public string StudentName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string TemporaryPassword { get; set; } = string.Empty;
}