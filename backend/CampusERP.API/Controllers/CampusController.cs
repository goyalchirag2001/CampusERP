using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CampusController : ControllerBase
{
    private readonly ICampusService _campusService;

    public CampusController(ICampusService campusService)
    {
        _campusService = campusService;
    }

    [Authorize(Policy = PermissionConstants.CampusCreate)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateCampusRequest request)
    {
        var result = await _campusService.CreateAsync(request);

        return Ok(result);
    }

    [Authorize(Policy = PermissionConstants.CampusView)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _campusService.GetAllAsync();

        return Ok(result);
    }

    [Authorize(Policy = PermissionConstants.CampusView)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _campusService.GetByIdAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [Authorize(Policy = PermissionConstants.CampusEdit)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCampusRequest request)
    {
        var result = await _campusService.UpdateAsync(id, request);

        return Ok(result);
    }

    [Authorize(Policy = PermissionConstants.CampusActivate)]
    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _campusService.ActivateAsync(id);

        return NoContent();
    }

    [Authorize(Policy = PermissionConstants.CampusDeactivate)]
    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(
        Guid id)
    {
        await _campusService.DeactivateAsync(id);

        return NoContent();
    }

    [Authorize(Policy = PermissionConstants.CampusView)]
    [HttpGet("lookup")]
    public async Task<IActionResult> GetLookup()
    {
        var result = await _campusService.GetLookupAsync();

        return Ok(result);
    }
}