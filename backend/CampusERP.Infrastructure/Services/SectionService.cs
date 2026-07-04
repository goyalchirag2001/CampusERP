using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class SectionService : ISectionService
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IDataAccessScope _scope;

    public SectionService(ApplicationDbContext dbContext, IDataAccessScope scope)
    {
        _dbContext = dbContext;

        _scope = scope;
    }

    public async Task<List<SectionResponse>> GetAllAsync()
    {
        return await ApplySectionScope(_dbContext.Sections)
            .OrderBy(x => x.Course.Name)
            .ThenBy(x => x.Semester.SequenceNumber)
            .ThenBy(x => x.Name)
            .Select(x => new SectionResponse
            {
                Id = x.Id,

                SemesterId = x.SemesterId,

                CourseId = x.CourseId,

                DepartmentId = x.DepartmentId,

                DepartmentName = x.Department.Name,

                CourseName = x.Course.Name,

                SemesterName = x.Semester.Name,

                Name = x.Name,

                Capacity = x.Capacity,

                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public async Task<SectionResponse?> GetByIdAsync(Guid id)
    {
        return await ApplySectionScope(_dbContext.Sections.Where(x => x.Id == id))
            .Select(x => new SectionResponse
            {
                Id = x.Id,

                SemesterId = x.SemesterId,

                CourseId = x.CourseId,

                DepartmentId = x.DepartmentId,

                DepartmentName = x.Department.Name,

                CourseName = x.Course.Name,

                SemesterName = x.Semester.Name,

                Name = x.Name,

                Capacity = x.Capacity,

                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task<SectionResponse> CreateAsync(CreateSectionRequest request)
    {
        var semester = await ApplySemesterScope(_dbContext.Semesters.Where(x => x.Id == request.SemesterId))
                .Include(x => x.Course)
                .FirstOrDefaultAsync();

        if (semester is null)
        {
            throw new Exception("Semester not found.");
        }

        var sectionName = request.Name.Trim().ToUpper();

        var exists = await _dbContext.Sections
                .AnyAsync(x =>
                    x.SemesterId == request.SemesterId &&
                    x.Name == sectionName);

        if (exists)
        {
            throw new Exception("Section already exists.");
        }

        var section = new Section
        {
            Id = Guid.NewGuid(),

            InstitutionId = semester.InstitutionId,

            CampusId = semester.CampusId,

            DepartmentId = semester.Course.DepartmentId,

            CourseId = semester.CourseId,

            SemesterId = semester.Id,

            Name = sectionName,

            Capacity = request.Capacity,

            IsActive = true
        };

        _dbContext.Sections.Add(section);

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(section.Id) ?? throw new Exception("Section not found.");
    }

    public async Task<SectionResponse> UpdateAsync(Guid id, UpdateSectionRequest request)
    {
        var section = await ApplySectionScope(_dbContext.Sections
                        .Where(x => x.Id == id))
                .FirstOrDefaultAsync();

        if (section is null)
        {
            throw new Exception("Section not found.");
        }

        var duplicate = await _dbContext.Sections.AnyAsync(x =>
                x.Id != id &&
                x.SemesterId == section.SemesterId &&
                x.Name == request.Name);

        if (duplicate)
        {
            throw new Exception("Section already exists.");
        }

        section.Name = request.Name.Trim().ToUpper();

        section.Capacity = request.Capacity;

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id) ?? throw new Exception("Section not found.");
    }

    public async Task ActivateAsync(Guid id)
    {
        var section = await ApplySectionScope(_dbContext.Sections.Where(x => x.Id == id))
                .FirstOrDefaultAsync();

        if (section is null)
        {
            throw new Exception("Section not found.");
        }

        if (section.IsActive)
        {
            return;
        }

        section.IsActive = true;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeactivateAsync(Guid id)
    {
        var section = await ApplySectionScope(_dbContext.Sections.Where(x => x.Id == id))
                .FirstOrDefaultAsync();

        if (section is null)
        {
            throw new Exception("Section not found.");
        }

        if (!section.IsActive)
        {
            return;
        }

        section.IsActive = false;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<LookupResponse>> GetLookupBySemesterAsync(Guid semesterId)
    {
        return await ApplySectionScope(_dbContext.Sections
                    .Where(x =>
                        x.SemesterId == semesterId &&
                        x.IsActive))
            .OrderBy(x => x.Name)
            .Select(x => new LookupResponse
            {
                Id = x.Id,

                Name = $"Section {x.Name}"
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

    private IQueryable<Section> ApplySectionScope(IQueryable<Section> query)
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