using CampusERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/institution-discovery")]
public class InstitutionDiscoveryController : ControllerBase
{
    private readonly IInstitutionService _institutionService;

    public InstitutionDiscoveryController(
        IInstitutionService institutionService)
    {
        _institutionService = institutionService;
    }

    [AllowAnonymous]
    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        var institution = await _institutionService.GetBySlugAsync(slug, cancellationToken);

        if (institution is null)
        {
            return NotFound();
        }

        return Ok(institution);
    }
}