using CampusERP.Application.Interfaces;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SemesterController : ControllerBase
{
    private readonly ISemesterService _semesterService;

    public SemesterController(ISemesterService semesterService)
    {
        _semesterService = semesterService;
    }

    [Authorize(Policy = PermissionConstants.SemesterView)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _semesterService.GetAllAsync());
    }

    [Authorize(Policy = PermissionConstants.SemesterView)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _semesterService.GetByIdAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [Authorize(Policy = PermissionConstants.SemesterView)]
    [HttpGet("lookup/{courseId:guid}")]
    public async Task<IActionResult> GetLookup(Guid courseId)
    {
        return Ok(await _semesterService.GetLookupByCourseAsync(courseId));
    }
}