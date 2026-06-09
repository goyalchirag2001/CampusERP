namespace CampusERP.Contracts.Requests;

public class CreateDepartmentRequest
{
    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;
}