using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SectionController : ControllerBase
{
    private readonly ISectionService _sectionService;

    public SectionController(ISectionService sectionService)
    {
        _sectionService = sectionService;
    }

    [Authorize(Policy = PermissionConstants.SectionView)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _sectionService.GetAllAsync());
    }

    [Authorize(Policy = PermissionConstants.SectionView)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Ok(await _sectionService.GetByIdAsync(id));
    }

    [Authorize(Policy = PermissionConstants.SectionCreate)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateSectionRequest request)
    {
        return Ok(await _sectionService.CreateAsync(request));
    }

    [Authorize(Policy = PermissionConstants.SectionEdit)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateSectionRequest request)
    {
        return Ok(await _sectionService.UpdateAsync(id, request));
    }

    [Authorize(Policy = PermissionConstants.SectionEdit)]
    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _sectionService.ActivateAsync(id);

        return NoContent();
    }

    [Authorize(Policy = PermissionConstants.SectionEdit)]
    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await _sectionService.DeactivateAsync(id);

        return NoContent();
    }

    [Authorize(Policy = PermissionConstants.SectionView)]
    [HttpGet("lookup/semester/{semesterId:guid}")]
    public async Task<IActionResult> Lookup(Guid semesterId)
    {
        return Ok(await _sectionService.GetLookupBySemesterAsync(semesterId));
    }
}