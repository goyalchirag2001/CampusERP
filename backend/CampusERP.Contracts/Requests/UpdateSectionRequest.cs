namespace CampusERP.Contracts.Requests;

public class UpdateSectionRequest
{
    public string Name { get; set; } = string.Empty;

    public int Capacity { get; set; }
}