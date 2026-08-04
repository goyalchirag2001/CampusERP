using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/student-promotion")]
[Authorize]
public class StudentPromotionController : ControllerBase
{
    private readonly IStudentPromotionService _service;

    public StudentPromotionController(IStudentPromotionService service)
    {
        _service = service;
    }

    [Authorize(Policy = PermissionConstants.StudentEdit)]
    [HttpPost("students")]
    public async Task<IActionResult> LoadStudents(LoadPromotionStudentsRequest request)
    {
        return Ok(await _service.LoadStudentsAsync(request));
    }

    [Authorize(Policy = PermissionConstants.StudentEdit)]
    [HttpPost]
    public async Task<IActionResult> Promote(PromoteStudentsRequest request)
    {
        await _service.PromoteAsync(request);

        return NoContent();
    }
}