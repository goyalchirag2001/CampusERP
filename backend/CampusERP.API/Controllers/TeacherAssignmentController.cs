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
public class TeacherAssignmentController : ControllerBase
{
    private readonly ITeacherAssignmentService _teacherAssignmentService;

    public TeacherAssignmentController(ITeacherAssignmentService teacherAssignmentService)
    {
        _teacherAssignmentService = teacherAssignmentService;
    }

    [HttpPost]
    [Authorize(Policy = PermissionConstants.TeacherAssignmentCreate)]
    public async Task<IActionResult> Assign(AssignTeacherRequest request)
    {
        return Ok(await _teacherAssignmentService.AssignAsync(request));
    }

    [HttpGet("teacher/{teacherId:guid}")]
    [Authorize(Policy = PermissionConstants.TeacherAssignmentView)]
    public async Task<IActionResult> GetByTeacher(Guid teacherId)
    {
        return Ok(await _teacherAssignmentService.GetByTeacherAsync(teacherId));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PermissionConstants.TeacherAssignmentDelete)]
    public async Task<IActionResult> Remove(Guid id)
    {
        await _teacherAssignmentService.RemoveAsync(id);

        return NoContent();
    }
}