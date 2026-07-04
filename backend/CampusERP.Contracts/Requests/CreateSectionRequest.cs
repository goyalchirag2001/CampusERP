namespace CampusERP.Contracts.Requests;

public class CreateSectionRequest
{
    public Guid SemesterId { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Capacity { get; set; }
}