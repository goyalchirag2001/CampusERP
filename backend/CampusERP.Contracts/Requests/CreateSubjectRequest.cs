using CampusERP.Contracts.Enums;

namespace CampusERP.Contracts.Requests;

public class CreateSubjectRequest
{
    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int Credits { get; set; }

    public SubjectTypeDto SubjectType { get; set; }
}