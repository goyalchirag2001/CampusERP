using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CourseController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CourseController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [Authorize(Policy = PermissionConstants.CourseCreate)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateCourseRequest request)
    {
        return Ok(await _courseService.CreateAsync(request));
    }

    [Authorize(Policy = PermissionConstants.CourseView)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _courseService.GetAllAsync());
    }

    [Authorize(Policy = PermissionConstants.CourseView)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _courseService.GetByIdAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [Authorize(Policy = PermissionConstants.CourseEdit)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCourseRequest request)
    {
        return Ok(await _courseService.UpdateAsync(id, request));
    }

    [Authorize(Policy = PermissionConstants.CourseActivate)]
    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _courseService.ActivateAsync(id);

        return NoContent();
    }

    [Authorize(Policy = PermissionConstants.CourseDeactivate)]
    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await _courseService.DeactivateAsync(id);

        return NoContent();
    }

    [Authorize(Policy = PermissionConstants.CourseView)]
    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup()
    {
        return Ok(await _courseService.GetLookupAsync());
    }
}