namespace CampusERP.Contracts.Responses;

public class DepartmentLookupResponse
{
    public Guid Id { get; set; }

    public Guid CampusId { get; set; }

    public string Name { get; set; } = string.Empty;
}