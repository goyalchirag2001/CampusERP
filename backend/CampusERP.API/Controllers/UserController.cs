using CampusERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;

    public UserController(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            UserId = _currentUserService.UserId,
            Email = _currentUserService.Email,
            InstitutionId = _currentUserService.InstitutionId,
            CampusId = _currentUserService.CampusId
        });
    }
}