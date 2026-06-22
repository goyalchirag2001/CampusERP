using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [Authorize(Policy = PermissionConstants.RoleView)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _roleService.GetAllAsync());
    }

    [Authorize(Policy = PermissionConstants.RoleView)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _roleService.GetByIdAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [Authorize(Policy = PermissionConstants.RoleView)]
    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup()
    {
        return Ok(await _roleService.GetLookupAsync());
    }

    [Authorize(Policy = PermissionConstants.RoleCreate)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateRoleRequest request)
    {
        return Ok(await _roleService.CreateAsync(request));
    }

    [Authorize(Policy = PermissionConstants.RoleEdit)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateRoleRequest request)
    {
        return Ok(await _roleService.UpdateAsync(id, request));
    }

    [Authorize(Policy = PermissionConstants.RoleActivate)]
    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _roleService.ActivateAsync(id);

        return NoContent();
    }

    [Authorize(Policy = PermissionConstants.RoleDeactivate)]
    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await _roleService.DeactivateAsync(id);

        return NoContent();
    }
}