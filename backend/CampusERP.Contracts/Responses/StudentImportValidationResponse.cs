namespace CampusERP.Contracts.Responses;

public class StudentImportValidationResponse
{
    public int TotalRows { get; set; }

    public int ValidRows { get; set; }

    public int InvalidRows { get; set; }

    public bool CanImport => InvalidRows == 0;

    public List<StudentImportPreviewRow> Preview { get; set; } = [];

    public List<StudentImportError> Errors { get; set; } = [];

    public List<StudentImportCredential> Credentials { get; set; } = [];
}