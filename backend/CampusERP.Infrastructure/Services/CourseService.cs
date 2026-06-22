using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class CourseService : ICourseService
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IDataAccessScope _scope;

    public CourseService(ApplicationDbContext dbContext, IDataAccessScope scope)
    {
        _dbContext = dbContext;

        _scope = scope;
    }

    public async Task<CourseResponse> CreateAsync(CreateCourseRequest request)
    {
        ValidateCreateScope(request.InstitutionId, request.CampusId);

        var departmentExists =
            await _dbContext.Departments
                .AnyAsync(x =>
                    x.Id == request.DepartmentId &&
                    x.CampusId == request.CampusId &&
                    x.InstitutionId == request.InstitutionId);

        if (!departmentExists)
        {
            throw new Exception("Department not found.");
        }

        var courseCodeExists =
            await _dbContext.Courses
                .AnyAsync(x =>
                    x.InstitutionId == request.InstitutionId &&
                    x.CampusId == request.CampusId &&
                    x.Code == request.Code);

        if (courseCodeExists)
        {
            throw new Exception("Course code already exists.");
        }

        if (request.DurationYears <= 0)
        {
            throw new Exception("Invalid duration.");
        }

        if (request.TotalSemesters <= 0)
        {
            throw new Exception("Invalid semester count.");
        }

        var course = new Course
        {
            Id = Guid.NewGuid(),

            InstitutionId = request.InstitutionId,

            CampusId = request.CampusId,

            DepartmentId = request.DepartmentId,

            Name = request.Name,

            Code = request.Code,

            DegreeType = request.DegreeType,

            DurationYears = request.DurationYears,

            TotalSemesters = request.TotalSemesters,

            IsActive = true
        };

        _dbContext.Courses.Add(course);

        await _dbContext.SaveChangesAsync();

        var semesters = new List<Semester>();

        for (int i = 1; i <= course.TotalSemesters; i++)
        {
            semesters.Add(new Semester
            {
                Id = Guid.NewGuid(),

                InstitutionId = course.InstitutionId,

                CampusId = course.CampusId,

                CourseId = course.Id,

                Name = $"Semester {i}",

                SequenceNumber = i,

                IsActive = course.IsActive
            });
        }

        _dbContext.Semesters.AddRange(semesters);

        await _dbContext.SaveChangesAsync();

        return new CourseResponse
        {
            Id = course.Id,

            InstitutionId = course.InstitutionId,

            CampusId = course.CampusId,

            DepartmentId = course.DepartmentId,

            Name = course.Name,

            Code = course.Code,

            DegreeType = course.DegreeType,

            DurationYears = course.DurationYears,

            TotalSemesters = course.TotalSemesters,

            IsActive = course.IsActive
        };
    }

    public async Task<List<CourseResponse>> GetAllAsync()
    {
        var query = ApplyCourseScope(_dbContext.Courses.AsQueryable());

        return await query
            .Select(x =>
                new CourseResponse
                {
                    Id = x.Id,

                    InstitutionId = x.InstitutionId,

                    CampusId = x.CampusId,

                    DepartmentId = x.DepartmentId,

                    Name = x.Name,

                    Code = x.Code,

                    DegreeType = x.DegreeType,

                    DurationYears = x.DurationYears,

                    TotalSemesters = x.TotalSemesters,

                    IsActive = x.IsActive
                })
            .ToListAsync();
    }

    public async Task<CourseResponse?> GetByIdAsync(Guid id)
    {
        return await ApplyCourseScope(_dbContext.Courses.Where(x => x.Id == id))
        .Select(x =>
            new CourseResponse
            {
                    Id = x.Id,

                    InstitutionId = x.InstitutionId,

                    CampusId = x.CampusId,

                    DepartmentId = x.DepartmentId,

                    Name = x.Name,

                    Code = x.Code,

                    DegreeType = x.DegreeType,

                    DurationYears = x.DurationYears,

                    TotalSemesters = x.TotalSemesters,

                    IsActive = x.IsActive
                })
            .FirstOrDefaultAsync();
    }

    public async Task<CourseResponse> UpdateAsync(Guid id, UpdateCourseRequest request)
    {
        var course = await ApplyCourseScope(_dbContext.Courses.Where(x => x.Id == id)).FirstOrDefaultAsync();

        if (course is null)
        {
            throw new Exception("Course not found.");
        }

        var departmentExists =
            await _dbContext.Departments
                .AnyAsync(x =>
                    x.Id == request.DepartmentId &&
                    x.CampusId == request.CampusId &&
                    x.InstitutionId == request.InstitutionId);

        if (!departmentExists)
        {
            throw new Exception("Department not found.");
        }

        var codeExists =
            await _dbContext.Courses
                .AnyAsync(x =>
                    x.Id != id &&
                    x.CampusId == request.CampusId &&
                    x.Code == request.Code);

        if (codeExists)
        {
            throw new Exception("Course code already exists.");
        }

        course.Name = request.Name;

        course.Code = request.Code;

        course.DegreeType = request.DegreeType;

        course.DurationYears = request.DurationYears;

        course.DepartmentId = request.DepartmentId;

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id) ?? throw new Exception();
    }

    public async Task ActivateAsync(Guid id)
    {
        var course = await ApplyCourseScope(_dbContext.Courses.Where(x => x.Id == id)).FirstOrDefaultAsync();

        if (course is null)
        {
            throw new Exception("Course not found.");
        }

        course.IsActive = true;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeactivateAsync(Guid id)
    {
        var course = await ApplyCourseScope(_dbContext.Courses.Where(x => x.Id == id)).FirstOrDefaultAsync();

        if (course is null)
        {
            throw new Exception("Course not found.");
        }

        course.IsActive = false;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<LookupResponse>> GetLookupAsync()
    {
        return await ApplyCourseScope(_dbContext.Courses.Where(x => x.IsActive))
            .OrderBy(x => x.Name)
            .Select(x =>
                new LookupResponse
                {
                    Id = x.Id,

                    Name = x.Name
                })
            .ToListAsync();
    }

    private IQueryable<Course> ApplyCourseScope(IQueryable<Course> query)
    {
        if (_scope.IsSuperAdmin() || _scope.IsPlatformAdmin())
        {
            return query;
        }

        if (_scope.IsInstitutionAdmin())
        {
            query = query.Where(x => x.InstitutionId == _scope.InstitutionId());
        }

        if (_scope.IsCampusAdmin())
        {
            query = query.Where(x => x.CampusId == _scope.CampusId());
        }

        return query;
    }

    private void ValidateCreateScope(Guid institutionId, Guid campusId)
    {
        if (_scope.IsInstitutionAdmin())
        {
            if (institutionId != _scope.InstitutionId())
            {
                throw new Exception("Access denied.");
            }
        }

        if (_scope.IsCampusAdmin())
        {
            if (campusId != _scope.CampusId())
            {
                throw new Exception("Access denied.");
            }
        }
    }
}