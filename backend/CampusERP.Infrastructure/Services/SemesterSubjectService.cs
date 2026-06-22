using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class SemesterSubjectService : ISemesterSubjectService
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IDataAccessScope _scope;

    public SemesterSubjectService(ApplicationDbContext dbContext, IDataAccessScope scope)
    {
        _dbContext = dbContext;

        _scope = scope;
    }

    public async Task<SemesterSubjectResponse> AssignAsync(AssignSubjectToSemesterRequest request)
    {
        var semester = await ApplySemesterScope(_dbContext.Semesters.Where(x => x.Id == request.SemesterId)).FirstOrDefaultAsync();

        if (semester is null)
        {
            throw new Exception("Semester not found.");
        }

        var subject = await ApplySubjectScope(_dbContext.Subjects.Where(x => x.Id == request.SubjectId)).FirstOrDefaultAsync();

        if (subject is null)
        {
            throw new Exception("Subject not found.");
        }

        if (semester.InstitutionId != subject.InstitutionId)
        {
            throw new Exception("Institution mismatch.");
        }

        if (semester.CampusId != subject.CampusId)
        {
            throw new Exception("Campus mismatch.");
        }

        var exists =
            await _dbContext.SemesterSubjects
                .AnyAsync(x =>
                    x.SemesterId == request.SemesterId &&
                    x.SubjectId == request.SubjectId);

        if (exists)
        {
            throw new Exception("Subject already assigned.");
        }

        var semesterSubject =
            new SemesterSubject
            {
                Id = Guid.NewGuid(),

                SemesterId = request.SemesterId,

                SubjectId = request.SubjectId
            };

        _dbContext.SemesterSubjects.Add(semesterSubject);

        await _dbContext.SaveChangesAsync();

        return new SemesterSubjectResponse
        {
            Id = semesterSubject.Id,

            SemesterId = semester.Id,

            SubjectId = subject.Id,

            SemesterName = semester.Name,

            SubjectCode = subject.Code,

            SubjectName = subject.Name
        };
    }

    public async Task<List<SemesterSubjectResponse>> GetBySemesterAsync(Guid semesterId)
    {
        return await ApplySemesterSubjectScope(
                _dbContext.SemesterSubjects
                    .Where(x => x.SemesterId == semesterId))
            .Select(x =>
                new SemesterSubjectResponse
                {
                    Id = x.Id,

                    SemesterId = x.SemesterId,

                    SubjectId = x.SubjectId,

                    SemesterName = x.Semester.Name,

                    SubjectCode = x.Subject.Code,

                    SubjectName = x.Subject.Name
                })
            .ToListAsync();
    }

    public async Task RemoveAsync(Guid id)
    {
        var semesterSubject =
            await ApplySemesterSubjectScope(
                    _dbContext.SemesterSubjects
                        .Where(x => x.Id == id))
                .FirstOrDefaultAsync();

        if (semesterSubject is null)
        {
            throw new Exception("Semester subject mapping not found.");
        }

        _dbContext.SemesterSubjects.Remove(semesterSubject);

        await _dbContext.SaveChangesAsync();
    }

    private IQueryable<Semester> ApplySemesterScope(IQueryable<Semester> query)
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
                    x.InstitutionId ==
                    _scope.InstitutionId());
        }

        if (_scope.IsCampusAdmin())
        {
            query =
                query.Where(x =>
                    x.CampusId ==
                    _scope.CampusId());
        }

        return query;
    }

    private IQueryable<Subject> ApplySubjectScope(IQueryable<Subject> query)
    {
        if (_scope.IsSuperAdmin() || _scope.IsPlatformAdmin())
        {
            return query;
        }

        if (_scope.IsInstitutionAdmin())
        {
            query =
                query.Where(x =>
                    x.InstitutionId ==
                    _scope.InstitutionId());
        }

        if (_scope.IsCampusAdmin())
        {
            query =
                query.Where(x =>
                    x.CampusId ==
                    _scope.CampusId());
        }

        return query;
    }

    private IQueryable<SemesterSubject> ApplySemesterSubjectScope(IQueryable<SemesterSubject> query)
    {
        if (_scope.IsSuperAdmin() || _scope.IsPlatformAdmin())
        {
            return query;
        }

        if (_scope.IsInstitutionAdmin())
        {
            query =
                query.Where(x =>
                    x.Semester.InstitutionId ==
                    _scope.InstitutionId());
        }

        if (_scope.IsCampusAdmin())
        {
            query =
                query.Where(x =>
                    x.Semester.CampusId ==
                    _scope.CampusId());
        }

        return query;
    }
}