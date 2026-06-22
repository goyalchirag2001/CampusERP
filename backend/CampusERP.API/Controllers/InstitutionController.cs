using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InstitutionController : ControllerBase
{
    private readonly IInstitutionService _institutionService;

    public InstitutionController(IInstitutionService institutionService)
    {
        _institutionService = institutionService;
    }

    [Authorize(Policy = PermissionConstants.InstitutionCreate)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateInstitutionRequest request)
    {
        return Ok(await _institutionService.CreateAsync(request));
    }

    [Authorize(Policy = PermissionConstants.InstitutionView)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _institutionService.GetAllAsync());
    }

    [Authorize(Policy = PermissionConstants.InstitutionView)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _institutionService.GetByIdAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [Authorize(Policy = PermissionConstants.InstitutionEdit)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateInstitutionRequest request)
    {
        return Ok(await _institutionService.UpdateAsync(id, request));
    }

    [Authorize(Policy = PermissionConstants.InstitutionActivate)]
    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _institutionService.ActivateAsync(id);

        return NoContent();
    }

    [Authorize(Policy = PermissionConstants.InstitutionDeactivate)]
    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await _institutionService.DeactivateAsync(id);

        return NoContent();
    }
}