using Azure;
using CampusERP.Application.Common.Models;
using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AcademicConfigurationController : ControllerBase
{
    private readonly IAcademicConfigurationService _service;

    public AcademicConfigurationController(IAcademicConfigurationService service)
    {
        _service = service;
    }

    [Authorize(Policy = PermissionConstants.AcademicSettingsView)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<AcademicConfigurationResponse>>> Get()
    {
        var response = await _service.GetAsync();

        return Ok(ApiResponse<AcademicConfigurationResponse>.SuccessResponse(response));
    }

    [Authorize(Policy = PermissionConstants.AcademicSettingsEdit)]
    [HttpPut]
    public async Task<ActionResult<ApiResponse<AcademicConfigurationResponse>>> Update([FromBody] UpdateAcademicConfigurationRequest request)
    {
        var response = await _service.UpdateAsync(request);

        return Ok(ApiResponse<AcademicConfigurationResponse>.SuccessResponse(response, "Academic configuration updated successfully."));
    }
}