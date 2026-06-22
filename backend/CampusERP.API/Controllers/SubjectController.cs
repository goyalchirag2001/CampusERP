using CampusERP.Application.Authorization;
using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubjectController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    [HttpPost]
    [Authorize(Policy = PermissionConstants.SubjectCreate)]
    public async Task<IActionResult> Create(CreateSubjectRequest request)
    {
        return Ok(await _subjectService.CreateAsync(request));
    }

    [HttpGet]
    [Authorize(Policy = PermissionConstants.SubjectView)]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _subjectService.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = PermissionConstants.SubjectView)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _subjectService.GetByIdAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PermissionConstants.SubjectEdit)]
    public async Task<IActionResult> Update(Guid id, UpdateSubjectRequest request)
    {
        return Ok(await _subjectService.UpdateAsync(id, request));
    }

    [HttpPut("{id:guid}/activate")]
    [Authorize(Policy = PermissionConstants.SubjectActivate)]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _subjectService.ActivateAsync(id);

        return NoContent();
    }

    [HttpPut("{id:guid}/deactivate")]
    [Authorize(Policy = PermissionConstants.SubjectDeactivate)]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await _subjectService.DeactivateAsync(id);

        return NoContent();
    }

    [HttpGet("lookup")]
    [Authorize(Policy = PermissionConstants.SubjectView)]
    public async Task<IActionResult> Lookup()
    {
        return Ok(await _subjectService.GetLookupAsync());
    }
}