using CampusERP.Application.Authorization;
using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    [Authorize(Policy = PermissionConstants.RoomView)]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _roomService.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = PermissionConstants.RoomView)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var room = await _roomService.GetByIdAsync(id);

        if (room is null)
        {
            return NotFound();
        }

        return Ok(room);
    }

    [HttpGet("lookup")]
    [Authorize(Policy = PermissionConstants.RoomView)]
    public async Task<IActionResult> GetLookup()
    {
        return Ok(await _roomService.GetLookupAsync());
    }

    [HttpPost]
    [Authorize(Policy = PermissionConstants.RoomCreate)]
    public async Task<IActionResult> Create(CreateRoomRequest request)
    {
        return Ok(await _roomService.CreateAsync(request));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PermissionConstants.RoomEdit)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateRoomRequest request)
    {
        return Ok(await _roomService.UpdateAsync(id, request));
    }

    [HttpPut("{id:guid}/activate")]
    [Authorize(Policy = PermissionConstants.RoomEdit)]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _roomService.ActivateAsync(id);

        return NoContent();
    }

    [HttpPut("{id:guid}/deactivate")]
    [Authorize(Policy = PermissionConstants.RoomEdit)]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await _roomService.DeactivateAsync(id);

        return NoContent();
    }
}