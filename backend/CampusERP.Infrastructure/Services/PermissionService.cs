using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Responses;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class PermissionService : IPermissionService
{
    private readonly ApplicationDbContext _dbContext;

    public PermissionService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<PermissionResponse>> GetAllAsync()
    {
        return await _dbContext.Permissions
            .OrderBy(x => x.Name)
            .Select(x => new PermissionResponse
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Module = x.Module,
            })
            .ToListAsync();
    }
}