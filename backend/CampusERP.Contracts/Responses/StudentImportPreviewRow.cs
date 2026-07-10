namespace CampusERP.Contracts.Responses;

public class StudentImportPreviewRow
{
    public int RowNumber { get; set; }

    public string AdmissionNumber { get; set; } = string.Empty;

    public string StudentName { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public string Course { get; set; } = string.Empty;

    public string Semester { get; set; } = string.Empty;

    public string Section { get; set; } = string.Empty;

    public bool IsValid { get; set; }
}