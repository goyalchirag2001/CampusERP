using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    [HttpGet("platform")]
    [Authorize(Roles = RoleConstants.PlatformAdmin)]
    public IActionResult PlatformOnly()
    {
        return Ok(new
        {
            Message = "Platform Admin Access Granted"
        });
    }
}