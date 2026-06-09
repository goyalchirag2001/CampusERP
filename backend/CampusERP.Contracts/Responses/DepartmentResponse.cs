namespace CampusERP.Contracts.Responses;

public class DepartmentResponse
{
    public Guid Id { get; set; }

    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;
}