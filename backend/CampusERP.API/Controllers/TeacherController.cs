using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TeacherController : ControllerBase
{
    private readonly ITeacherService _teacherService;

    public TeacherController(ITeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    [HttpPost]
    [Authorize(Policy = PermissionConstants.TeacherCreate)]
    public async Task<IActionResult> Create(CreateTeacherRequest request)
    {
        return Ok(await _teacherService.CreateAsync(request));
    }

    [HttpGet]
    [Authorize(Policy = PermissionConstants.TeacherView)]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _teacherService.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = PermissionConstants.TeacherView)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _teacherService.GetByIdAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PermissionConstants.TeacherEdit)]
    public async Task<IActionResult> Update(Guid id, UpdateTeacherRequest request)
    {
        return Ok(await _teacherService.UpdateAsync(id, request));
    }

    [HttpPut("{id:guid}/activate")]
    [Authorize(Policy = PermissionConstants.TeacherActivate)]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _teacherService.ActivateAsync(id);

        return NoContent();
    }

    [HttpPut("{id:guid}/deactivate")]
    [Authorize(Policy = PermissionConstants.TeacherDeactivate)]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await _teacherService.DeactivateAsync(id);

        return NoContent();
    }

    [HttpGet("lookup")]
    [Authorize(Policy = PermissionConstants.TeacherView)]
    public async Task<IActionResult> Lookup()
    {
        return Ok(await _teacherService.GetLookupAsync());
    }

    [HttpGet("lookup-department")]
    [Authorize(Policy = PermissionConstants.TeacherView)]
    public async Task<IActionResult> LookupWithDepartment()
    {
        return Ok(await _teacherService.GetLookupWithDepartmentAsync());
    }
}