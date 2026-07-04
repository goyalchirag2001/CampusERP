using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [Authorize(Policy = PermissionConstants.DepartmentCreate)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateDepartmentRequest request)
    {
        return Ok(await _departmentService.CreateAsync(request));
    }

    [Authorize(Policy = PermissionConstants.DepartmentView)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _departmentService.GetAllAsync());
    }

    [Authorize(Policy = PermissionConstants.DepartmentView)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _departmentService.GetByIdAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [Authorize(Policy = PermissionConstants.DepartmentEdit)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateDepartmentRequest request)
    {
        return Ok(await _departmentService.UpdateAsync(id, request));
    }

    [Authorize(Policy = PermissionConstants.DepartmentActivate)]
    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _departmentService.ActivateAsync(id);

        return NoContent();
    }

    [Authorize(Policy = PermissionConstants.DepartmentDeactivate)]
    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await _departmentService.DeactivateAsync(id);

        return NoContent();
    }

    [Authorize(Policy = PermissionConstants.DepartmentView)]
    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup()
    {
        return Ok(await _departmentService.GetLookupAsync());
    }

    [HttpGet("lookup-campus")]
    [Authorize(Policy = PermissionConstants.DepartmentView)]
    public async Task<IActionResult> LookupWithCampus()
    {
        return Ok(await _departmentService.GetLookupWithCampusAsync());
    }
}