using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [Authorize(Policy = PermissionConstants.StudentCreate)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateStudentRequest request)
    {
        return Ok(await _studentService.CreateAsync(request));
    }

    [Authorize(Policy = PermissionConstants.StudentView)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _studentService.GetAllAsync());
    }

    [Authorize(Policy = PermissionConstants.StudentView)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _studentService.GetByIdAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [Authorize(Policy = PermissionConstants.StudentEdit)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateStudentRequest request)
    {
        return Ok(await _studentService.UpdateAsync(id, request));
    }

    [Authorize(Policy = PermissionConstants.StudentActivate)]
    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _studentService.ActivateAsync(id);

        return NoContent();
    }

    [Authorize(Policy = PermissionConstants.StudentDeactivate)]
    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await _studentService.DeactivateAsync(id);

        return NoContent();
    }

    [Authorize(Policy = PermissionConstants.StudentView)]
    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup()
    {
        return Ok(await _studentService.GetLookupAsync());
    }
}