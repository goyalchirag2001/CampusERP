using Microsoft.AspNetCore.Http;

namespace CampusERP.Contracts.Requests;

public class ImportStudentsRequest
{
    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public IFormFile File { get; set; } = default!;
}