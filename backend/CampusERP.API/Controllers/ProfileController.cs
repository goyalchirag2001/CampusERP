using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyProfile()
    {
        return Ok(await _profileService.GetMyProfileAsync());
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateProfileRequest request)
    {
        return Ok(await _profileService.UpdateAsync(request));
    }

    [HttpPost("photo")]
    public async Task<IActionResult> UploadPhoto(IFormFile file)
    {
        return Ok(new
        {
            profilePhotoUrl = await _profileService.UploadPhotoAsync(file)
        });
    }

    [HttpDelete("photo")]
    public async Task<IActionResult> RemovePhoto()
    {
        await _profileService.RemovePhotoAsync();

        return NoContent();
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        await _profileService.ChangePasswordAsync(request);

        return NoContent();
    }
}