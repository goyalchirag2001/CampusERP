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
public class TeacherAssignmentsController : ControllerBase
{
    private readonly ITeacherAssignmentService _teacherAssignmentService;

    public TeacherAssignmentsController(ITeacherAssignmentService teacherAssignmentService)
    {
        _teacherAssignmentService = teacherAssignmentService;
    }

    [HttpPost]
    [Authorize(Policy = PermissionConstants.TeacherAssignmentCreate)]
    public async Task<IActionResult> Create(CreateTeacherAssignmentRequest request)
    {
        return Ok(await _teacherAssignmentService.CreateAsync(request));
    }

    [HttpGet]
    [Authorize(Policy = PermissionConstants.TeacherAssignmentView)]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _teacherAssignmentService.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = PermissionConstants.TeacherAssignmentView)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _teacherAssignmentService.GetByIdAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PermissionConstants.TeacherAssignmentEdit)]
    public async Task<IActionResult> Update(Guid id, UpdateTeacherAssignmentRequest request)
    {
        return Ok(await _teacherAssignmentService.UpdateAsync(id, request));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PermissionConstants.TeacherAssignmentDelete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _teacherAssignmentService.DeleteAsync(id);

        return NoContent();
    }
}