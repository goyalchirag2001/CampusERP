using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = RoleConstants.PlatformAdmin)]
public class TeacherAssignmentController : ControllerBase
{
    private readonly ITeacherAssignmentService _teacherAssignmentService;

    public TeacherAssignmentController(ITeacherAssignmentService teacherAssignmentService)
    {
        _teacherAssignmentService = teacherAssignmentService;
    }

    [HttpPost]
    public async Task<IActionResult> Assign(AssignTeacherRequest request)
    {
        var result = await _teacherAssignmentService.AssignAsync(request);

        return Ok(result);
    }

    [HttpGet("teacher/{teacherId:guid}")]
    public async Task<IActionResult> GetByTeacher(Guid teacherId)
    {
        var result = await _teacherAssignmentService.GetByTeacherAsync(teacherId);

        return Ok(result);
    }
}