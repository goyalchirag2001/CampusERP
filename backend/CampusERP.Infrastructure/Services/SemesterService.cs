using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class SemesterService : ISemesterService
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IDataAccessScope _scope;

    public SemesterService(
        ApplicationDbContext dbContext,
        IDataAccessScope scope)
    {
        _dbContext = dbContext;

        _scope = scope;
    }

    public async Task<List<SemesterResponse>> GetAllAsync()
    {
        return await ApplySemesterScope(_dbContext.Semesters)
            .OrderBy(x => x.Course.Name)
            .ThenBy(x => x.SequenceNumber)
            .Select(x => new SemesterResponse
            {
                Id = x.Id,

                InstitutionId = x.InstitutionId,

                CampusId = x.CampusId,

                CourseId = x.CourseId,

                Name = x.Name,

                SequenceNumber = x.SequenceNumber,

                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public async Task<SemesterResponse?> GetByIdAsync(Guid id)
    {
        return await ApplySemesterScope(
                _dbContext.Semesters
                    .Where(x => x.Id == id))
            .Select(x => new SemesterResponse
            {
                Id = x.Id,

                InstitutionId = x.InstitutionId,

                CampusId = x.CampusId,

                CourseId = x.CourseId,

                Name = x.Name,

                SequenceNumber = x.SequenceNumber,

                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<LookupResponse>> GetLookupByCourseAsync(Guid courseId)
    {
        return await ApplySemesterScope(
                _dbContext.Semesters
                    .Where(x =>
                        x.CourseId == courseId &&
                        x.IsActive))
            .OrderBy(x => x.SequenceNumber)
            .Select(x => new LookupResponse
            {
                Id = x.Id,

                Name = x.Name
            })
            .ToListAsync();
    }

    private IQueryable<Semester> ApplySemesterScope(IQueryable<Semester> query)
    {
        if (_scope.IsSuperAdmin() || _scope.IsPlatformAdmin())
        {
            return query;
        }

        if (_scope.IsInstitutionAdmin())
        {
            query = query.Where(x =>
                x.InstitutionId ==
                _scope.InstitutionId());
        }

        if (_scope.IsCampusAdmin())
        {
            query = query.Where(x =>
                x.CampusId ==
                _scope.CampusId());
        }

        return query;
    }
}