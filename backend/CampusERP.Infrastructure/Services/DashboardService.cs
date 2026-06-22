using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IDataAccessScope _scope;

    public DashboardService(ApplicationDbContext dbContext, IDataAccessScope scope)
    {
        _dbContext = dbContext;

        _scope = scope;
    }

    public async Task<DashboardResponse> GetPlatformDashboardAsync()
    {
        return new DashboardResponse
        {
            IsPlatformDashboard = true,

            InstitutionCount = await _dbContext.Institutions.CountAsync(),

            CampusCount = await _dbContext.Campuses.CountAsync(),

            UserCount = await _dbContext.Users.CountAsync(),

            StudentCount = await _dbContext.Students.CountAsync(),

            TeacherCount = await _dbContext.Teachers.CountAsync(),

            DepartmentCount = await _dbContext.Departments.CountAsync(),

            CourseCount = await _dbContext.Courses.CountAsync(),

            SubjectCount = await _dbContext.Subjects.CountAsync()
        };
    }

    public async Task<DashboardResponse> GetInstitutionDashboardAsync(Guid institutionId)
    {
        if (_scope.IsCampusAdmin())
        {
            var campusId = _scope.CampusId();

            return new DashboardResponse
            {
                IsPlatformDashboard = false,

                CampusCount = 1,

                UserCount = await _dbContext.Users.CountAsync(x => x.CampusId == campusId),

                StudentCount = await _dbContext.Students.CountAsync(x => x.CampusId == campusId),

                TeacherCount = await _dbContext.Teachers.CountAsync(x => x.CampusId == campusId),

                DepartmentCount = await _dbContext.Departments.CountAsync(x => x.CampusId == campusId),

                CourseCount = await _dbContext.Courses.CountAsync(x => x.CampusId == campusId),

                SubjectCount = await _dbContext.Subjects.CountAsync(x => x.CampusId == campusId)
            };
        }

        return new DashboardResponse
        {
            IsPlatformDashboard = false,

            CampusCount = await _dbContext.Campuses.CountAsync(x => x.InstitutionId == institutionId),

            UserCount = await _dbContext.Users.CountAsync(x => x.InstitutionId == institutionId),

            StudentCount = await _dbContext.Students.CountAsync(x => x.InstitutionId == institutionId),

            TeacherCount = await _dbContext.Teachers.CountAsync(x => x.InstitutionId == institutionId),

            DepartmentCount = await _dbContext.Departments.CountAsync(x => x.InstitutionId == institutionId),

            CourseCount = await _dbContext.Courses.CountAsync(x => x.InstitutionId == institutionId),

            SubjectCount = await _dbContext.Subjects.CountAsync(x => x.InstitutionId == institutionId)
        };
    }
}