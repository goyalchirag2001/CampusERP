using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SemesterSubjectController : ControllerBase
{
    private readonly ISemesterSubjectService _semesterSubjectService;

    public SemesterSubjectController(ISemesterSubjectService semesterSubjectService)
    {
        _semesterSubjectService = semesterSubjectService;
    }

    [Authorize(Policy = PermissionConstants.SemesterSubjectAssign)]
    [HttpPost]
    public async Task<IActionResult> Assign(AssignSubjectToSemesterRequest request)
    {
        return Ok(await _semesterSubjectService.AssignAsync(request));
    }

    [Authorize(Policy = PermissionConstants.SemesterSubjectView)]
    [HttpGet("semester/{semesterId:guid}")]
    public async Task<IActionResult> GetBySemester(Guid semesterId)
    {
        return Ok(await _semesterSubjectService.GetBySemesterAsync(semesterId));
    }

    [Authorize(Policy = PermissionConstants.SemesterSubjectView)]
    [HttpGet("course/{courseId:guid}")]
    public async Task<IActionResult> GetByCourse(Guid courseId)
    {
        return Ok(await _semesterSubjectService.GetByCourseAsync(courseId));
    }

    [Authorize(Policy = PermissionConstants.SemesterSubjectView)]
    [HttpGet("lookup/section/{sectionId:guid}")]
    public async Task<IActionResult> Lookup(Guid sectionId)
    {
        return Ok(await _semesterSubjectService.GetLookupBySectionAsync(sectionId));
    }

    [Authorize(Policy = PermissionConstants.SemesterSubjectRemove)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remove(Guid id)
    {
        await _semesterSubjectService.RemoveAsync(id);

        return NoContent();
    }

    [Authorize(Policy = PermissionConstants.SemesterSubjectAssign)]
    [HttpPut("{id:guid}/move-up")]
    public async Task<IActionResult> MoveUp(Guid id)
    {
        await _semesterSubjectService.MoveUpAsync(id);

        return NoContent();
    }

    [Authorize(Policy = PermissionConstants.SemesterSubjectAssign)]
    [HttpPut("{id:guid}/move-down")]
    public async Task<IActionResult> MoveDown(Guid id)
    {
        await _semesterSubjectService.MoveDownAsync(id);

        return NoContent();
    }
}