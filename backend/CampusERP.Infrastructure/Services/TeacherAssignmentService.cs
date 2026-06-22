using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class TeacherAssignmentService : ITeacherAssignmentService
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IDataAccessScope _scope;

    public TeacherAssignmentService(ApplicationDbContext dbContext, IDataAccessScope scope)
    {
        _dbContext = dbContext;

        _scope = scope;
    }

    public async Task<TeacherAssignmentResponse> AssignAsync(AssignTeacherRequest request)
    {
        var teacherQuery = _dbContext.Teachers.Include(x => x.User).Where(x => x.Id == request.TeacherId);

        if (_scope.IsInstitutionAdmin())
        {
            teacherQuery =
                teacherQuery.Where(x =>
                    x.InstitutionId ==
                    _scope.InstitutionId());
        }

        if (_scope.IsCampusAdmin())
        {
            teacherQuery =
                teacherQuery.Where(x =>
                    x.CampusId ==
                    _scope.CampusId());
        }

        var teacherEntity = await teacherQuery.FirstOrDefaultAsync();

        if (teacherEntity is null)
        {
            throw new Exception("Teacher not found.");
        }

        var semesterSubject =
            await _dbContext.SemesterSubjects
                .Include(x => x.Semester)
                .Include(x => x.Subject)
                .FirstOrDefaultAsync(x =>
                    x.Id == request.SemesterSubjectId);

        if (semesterSubject is null)
        {
            throw new Exception("Semester subject not found.");
        }

        if (teacherEntity.CampusId != semesterSubject.Semester.CampusId)
        {
            throw new Exception("Campus mismatch.");
        }

        if (teacherEntity.InstitutionId != semesterSubject.Semester.InstitutionId)
        {
            throw new Exception("Institution mismatch.");
        }

        var exists =
            await ApplyScope(_dbContext.TeacherAssignments)
                .AnyAsync(x =>
                    x.TeacherId == request.TeacherId &&
                    x.SemesterSubjectId == request.SemesterSubjectId);

        if (exists)
        {
            throw new Exception("Teacher already assigned.");
        }

        var assignment =
            new TeacherAssignment
            {
                Id = Guid.NewGuid(),

                TeacherId = request.TeacherId,

                SemesterSubjectId = request.SemesterSubjectId
            };

        _dbContext.TeacherAssignments.Add(assignment);

        await _dbContext.SaveChangesAsync();

        return new TeacherAssignmentResponse
        {
            Id = assignment.Id,

            TeacherId = teacherEntity.Id,

            SemesterSubjectId = semesterSubject.Id,

            TeacherName =
                $"{teacherEntity.User.FirstName} {teacherEntity.User.LastName}",

            SubjectName = semesterSubject.Subject.Name,

            SemesterName = semesterSubject.Semester.Name
        };
    }

    public async Task<List<TeacherAssignmentResponse>> GetByTeacherAsync(Guid teacherId)
    {
        return await ApplyScope(_dbContext.TeacherAssignments)
            .Where(x => x.TeacherId == teacherId)
            .Select(x =>
                new TeacherAssignmentResponse
                {
                    Id = x.Id,

                    TeacherId = x.TeacherId,

                    SemesterSubjectId = x.SemesterSubjectId,

                    TeacherName =
                        x.Teacher.User.FirstName +
                        " " +
                        x.Teacher.User.LastName,

                    SubjectName =
                        x.SemesterSubject.Subject.Name,

                    SemesterName =
                        x.SemesterSubject.Semester.Name
                })
            .ToListAsync();
    }

    public async Task RemoveAsync(Guid id)
    {
        var assignment =
            await ApplyScope(_dbContext.TeacherAssignments)
                .FirstOrDefaultAsync(x =>
                    x.Id == id);

        if (assignment is null)
        {
            throw new Exception("Teacher assignment not found.");
        }

        _dbContext.TeacherAssignments.Remove(assignment);

        await _dbContext.SaveChangesAsync();
    }

    private IQueryable<TeacherAssignment> ApplyScope(IQueryable<TeacherAssignment> query)
    {
        if (_scope.IsSuperAdmin() ||
            _scope.IsPlatformAdmin())
        {
            return query;
        }

        if (_scope.IsInstitutionAdmin())
        {
            query =
                query.Where(x =>
                    x.Teacher.InstitutionId ==
                    _scope.InstitutionId());
        }

        if (_scope.IsCampusAdmin())
        {
            query =
                query.Where(x =>
                    x.Teacher.CampusId ==
                    _scope.CampusId());
        }

        return query;
    }
}