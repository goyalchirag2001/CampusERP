using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = RoleConstants.PlatformAdmin)]
public class InstitutionController : ControllerBase
{
    private readonly IInstitutionService _institutionService;

    public InstitutionController(
        IInstitutionService institutionService)
    {
        _institutionService = institutionService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateInstitutionRequest request)
    {
        var result = await _institutionService.CreateAsync(request);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _institutionService.GetAllAsync();

        return Ok(result);
    }

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

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateInstitutionRequest request)
    {
        var result = await _institutionService.UpdateAsync(id, request);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _institutionService.DeleteAsync(id);

        return NoContent();
    }

    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _institutionService.ActivateAsync(id);

        return NoContent();
    }

    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await _institutionService.DeactivateAsync(id);

        return NoContent();
    }
}