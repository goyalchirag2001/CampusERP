namespace CampusERP.Contracts.Imports;

public class StudentImportRow
{
    public int RowNumber { get; set; }

    public string AdmissionNumber { get; set; } = string.Empty;

    public string RollNumber { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public DateOnly AdmissionDate { get; set; }

    public string Department { get; set; } = string.Empty;

    public string Course { get; set; } = string.Empty;

    public string Semester { get; set; } = string.Empty;

    public string Section { get; set; } = string.Empty;
}