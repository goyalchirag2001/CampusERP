using CampusERP.Application.Interfaces;
using CampusERP.Infrastructure.Data;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    private readonly ICurrentUserService _currentUserService;

    public DashboardController(IDashboardService dashboardService, ICurrentUserService currentUserService)
    {
        _dashboardService = dashboardService;

        _currentUserService = currentUserService;
    }

    [Authorize(Policy = PermissionConstants.AdminDashboardView)]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (_currentUserService.InstitutionId == SeedData.PlatformInstitutionId)
        {
            var platformDashboard = await _dashboardService.GetPlatformDashboardAsync();

            return Ok(platformDashboard);
        }

        var institutionDashboard = await _dashboardService.GetInstitutionDashboardAsync(_currentUserService.InstitutionId!.Value);

        return Ok(institutionDashboard);
    }
}