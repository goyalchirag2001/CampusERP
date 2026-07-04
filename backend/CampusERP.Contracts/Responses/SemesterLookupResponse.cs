namespace CampusERP.Contracts.Responses;

public class SemesterLookupResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int SequenceNumber { get; set; }
}