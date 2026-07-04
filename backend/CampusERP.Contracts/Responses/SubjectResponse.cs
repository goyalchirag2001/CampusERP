using CampusERP.Contracts.Enums;

namespace CampusERP.Contracts.Responses;

public class SubjectResponse
{
    public Guid Id { get; set; }

    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public string CampusName { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int Credits { get; set; }

    public bool IsActive { get; set; }

    public SubjectTypeDto SubjectType { get; set; }
}