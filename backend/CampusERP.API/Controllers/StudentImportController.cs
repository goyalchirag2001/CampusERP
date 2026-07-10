using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/student-import")]
[Authorize]
public class StudentImportController : ControllerBase
{
    private readonly IStudentImportService _service;

    public StudentImportController(IStudentImportService service)
    {
        _service = service;
    }

    // =====================================================
    // Download Excel Template
    // =====================================================

    [Authorize(Policy = PermissionConstants.StudentCreate)]
    [HttpGet("template")]
    public async Task<IActionResult> Template()
    {
        var bytes = await _service.GenerateTemplateAsync();

        return File(bytes,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","StudentImportTemplate.xlsx");
    }

    // =====================================================
    // Validate
    // =====================================================

    [Authorize(Policy = PermissionConstants.StudentCreate)]
    [HttpPost("validate")]
    public async Task<IActionResult> Validate([FromForm] ImportStudentsRequest request)
    {
        var result = await _service.ValidateAsync(request);

        return Ok(result);
    }

    // =====================================================
    // Import
    // =====================================================

    [Authorize(Policy = PermissionConstants.StudentCreate)]
    [HttpPost("import")]
    public async Task<IActionResult> Import([FromForm] ImportStudentsRequest request)
    {
        var result = await _service.ImportAsync(request);

        return Ok(result);
    }

    // =====================================================
    // Download Credentials Excel
    // =====================================================

    [Authorize(Policy = PermissionConstants.StudentCreate)]
    [HttpPost("credentials")]
    public IActionResult Credentials([FromBody] List<StudentImportCredential> credentials)
    {
        var bytes = _service.GenerateCredentialsExcel(credentials);

        return File(bytes,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StudentCredentials.xlsx");
    }
}