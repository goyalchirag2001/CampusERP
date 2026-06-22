using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    private readonly ICurrentUserService _currentUserService;

    public UserController(IUserService userService, ICurrentUserService currentUserService)
    {
        _userService = userService;

        _currentUserService = currentUserService;
    }

    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(
            new
            {
                UserId = _currentUserService.UserId,

                Email = _currentUserService.Email,

                InstitutionId = _currentUserService.InstitutionId,

                CampusId = _currentUserService.CampusId
            });
    }

    [Authorize(Policy = PermissionConstants.UserView)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _userService.GetAllAsync();

        return Ok(result);
    }

    [Authorize(Policy = PermissionConstants.UserView)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _userService.GetByIdAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [Authorize(Policy = PermissionConstants.UserCreate)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateUserRequest request)
    {
        var result = await _userService.CreateAsync(request);

        return Ok(result);
    }

    [Authorize(Policy = PermissionConstants.UserEdit)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateUserRequest request)
    {
        var result =
            await _userService.UpdateAsync(
                id,
                request);

        return Ok(result);
    }

    [Authorize(Policy = PermissionConstants.UserActivate)]
    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _userService.ActivateAsync(id);

        return NoContent();
    }

    [Authorize(Policy = PermissionConstants.UserDeactivate)]
    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await _userService.DeactivateAsync(id);

        return NoContent();
    }

    [Authorize(Policy = PermissionConstants.UserEdit)]
    [HttpPut("{id:guid}/reset-password")]
    public async Task<IActionResult> ResetPassword(Guid id, ResetPasswordRequest request)
    {
        await _userService.ResetPasswordAsync(id, request.NewPassword);

        return NoContent();
    }
}