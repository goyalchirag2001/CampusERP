namespace CampusERP.Contracts.Responses;

public class TeacherLookupResponse
{
    public Guid Id { get; set; }

    public Guid DepartmentId { get; set; }

    public string Name { get; set; } = string.Empty;
}