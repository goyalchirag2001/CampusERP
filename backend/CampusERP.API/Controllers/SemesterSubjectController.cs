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

    [Authorize(Policy = PermissionConstants.SemesterSubjectRemove)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remove(Guid id)
    {
        await _semesterSubjectService.RemoveAsync(id);

        return NoContent();
    }
}