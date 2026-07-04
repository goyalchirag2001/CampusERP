using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using Microsoft.AspNetCore.Http;

public interface IProfileService
{
    Task<ProfileResponse> GetMyProfileAsync();

    Task<ProfileResponse> UpdateAsync(UpdateProfileRequest request);

    Task<string> UploadPhotoAsync(IFormFile file);

    Task RemovePhotoAsync();

    Task ChangePasswordAsync(ChangePasswordRequest request);
}