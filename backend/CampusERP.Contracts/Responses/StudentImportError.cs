namespace CampusERP.Contracts.Responses;

public class StudentImportError
{
    public int RowNumber { get; set; }

    public string Column { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}