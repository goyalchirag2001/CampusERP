using CampusERP.Shared.Enums;

namespace CampusERP.Contracts.Requests;

public class UpdateSubjectRequest
{
    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int Credits { get; set; }

    public SubjectType SubjectType { get; set; }
}
