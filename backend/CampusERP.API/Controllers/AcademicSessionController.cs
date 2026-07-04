using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CampusERP.Shared.Constants;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AcademicSessionController : ControllerBase
{
    private readonly IAcademicSessionService _academicSessionService;

    public AcademicSessionController(IAcademicSessionService academicSessionService)
    {
        _academicSessionService = academicSessionService;
    }

    [Authorize(Policy = PermissionConstants.AcademicSessionView)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _academicSessionService.GetAllAsync();

        return Ok(result);
    }

    [Authorize(Policy = PermissionConstants.AcademicSessionView)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _academicSessionService.GetByIdAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [Authorize(Policy = PermissionConstants.AcademicSessionView)]
    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup()
    {
        var result = await _academicSessionService.GetLookupAsync();

        return Ok(result);
    }

    [Authorize(Policy = PermissionConstants.AcademicSessionView)]
    [HttpGet("current")]
    public async Task<IActionResult> Current()
    {
        var result = await _academicSessionService.GetCurrentAsync();

        if (result is null)
        {
            return NotFound("No current academic session configured.");
        }

        return Ok(result);
    }

    [Authorize(Policy = PermissionConstants.AcademicSessionCreate)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateAcademicSessionRequest request)
    {
        var result = await _academicSessionService.CreateAsync(request);

        return Ok(result);
    }

    [Authorize(Policy = PermissionConstants.AcademicSessionEdit)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateAcademicSessionRequest request)
    {
        var result = await _academicSessionService.UpdateAsync(id, request);

        return Ok(result);
    }

    [Authorize(Policy = PermissionConstants.AcademicSessionEdit)]
    [HttpPut("{id:guid}/set-current")]
    public async Task<IActionResult> SetCurrent(Guid id)
    {
        await _academicSessionService.SetCurrentAsync(id);

        return NoContent();
    }

    [Authorize(Policy = PermissionConstants.AcademicSessionActivate)]
    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _academicSessionService.ActivateAsync(id);

        return NoContent();
    }

    [Authorize(Policy = PermissionConstants.AcademicSessionDeactivate)]
    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await _academicSessionService.DeactivateAsync(id);

        return NoContent();
    }
}