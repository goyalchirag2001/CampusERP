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

    public CourseService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CourseResponse> CreateAsync(CreateCourseRequest request)
    {
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

            TotalSemesters = request.TotalSemesters
        };

        _dbContext.Courses.Add(course);

        await _dbContext.SaveChangesAsync();

        var semesters = new List<Semester>();

        for (int i = 1; i <= course.TotalSemesters; i++)
        {
            semesters.Add(new Semester
            {
                Id = Guid.NewGuid(),

                InstitutionId =
                    course.InstitutionId,

                CampusId =
                    course.CampusId,

                CourseId =
                    course.Id,

                Name =
                    $"Semester {i}",

                SequenceNumber =
                    i,

                IsActive = true
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

            DurationYears =
                course.DurationYears,

            TotalSemesters = course.TotalSemesters
        };
    }

    public async Task<List<CourseResponse>>
        GetAllAsync()
    {
        return await _dbContext.Courses
            .Select(x =>
                new CourseResponse
                {
                    Id = x.Id,

                    InstitutionId =
                        x.InstitutionId,

                    CampusId =
                        x.CampusId,

                    DepartmentId =
                        x.DepartmentId,

                    Name =
                        x.Name,

                    Code =
                        x.Code,

                    DegreeType =
                        x.DegreeType,

                    DurationYears =
                        x.DurationYears,

                    TotalSemesters =
                        x.TotalSemesters
                })
            .ToListAsync();
    }

    public async Task<CourseResponse?>
        GetByIdAsync(Guid id)
    {
        return await _dbContext.Courses
            .Where(x => x.Id == id)
            .Select(x =>
                new CourseResponse
                {
                    Id = x.Id,

                    InstitutionId =
                        x.InstitutionId,

                    CampusId =
                        x.CampusId,

                    DepartmentId =
                        x.DepartmentId,

                    Name =
                        x.Name,

                    Code =
                        x.Code,

                    DegreeType =
                        x.DegreeType,

                    DurationYears =
                        x.DurationYears,

                    TotalSemesters =
                        x.TotalSemesters
                })
            .FirstOrDefaultAsync();
    }
}