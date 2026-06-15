using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = RoleConstants.PlatformAdmin)]
public class SemesterSubjectController : ControllerBase
{
    private readonly ISemesterSubjectService _semesterSubjectService;

    public SemesterSubjectController(ISemesterSubjectService semesterSubjectService)
    {
        _semesterSubjectService = semesterSubjectService;
    }

    [HttpPost]
    public async Task<IActionResult> Assign(AssignSubjectToSemesterRequest request)
    {
        var result = await _semesterSubjectService.AssignAsync(request);

        return Ok(result);
    }

    [HttpGet("semester/{semesterId:guid}")]
    public async Task<IActionResult>GetBySemester(Guid semesterId)
    {
        var result = await _semesterSubjectService.GetBySemesterAsync(semesterId);

        return Ok(result);
    }
}