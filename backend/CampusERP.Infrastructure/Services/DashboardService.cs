using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Responses;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _dbContext;

    public DashboardService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardResponse> GetPlatformDashboardAsync()
    {
        return new DashboardResponse
        {
            InstitutionCount = await _dbContext.Institutions.CountAsync(),

            CampusCount = await _dbContext.Campuses.CountAsync(),

            UserCount = await _dbContext.Users.CountAsync(),

            StudentCount = await _dbContext.Students.CountAsync(),

            TeacherCount = await _dbContext.Teachers.CountAsync()
        };
    }
}