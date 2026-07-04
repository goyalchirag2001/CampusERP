using Microsoft.AspNetCore.Http;

namespace CampusERP.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveProfilePhotoAsync(IFormFile file);

    Task DeleteAsync(string? path);
}

