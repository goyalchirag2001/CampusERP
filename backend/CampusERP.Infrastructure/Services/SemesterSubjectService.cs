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
        var semester = await ApplySemesterScope(
                _dbContext.Semesters.Where(x => x.Id == request.SemesterId))
            .FirstOrDefaultAsync();

        if (semester is null)
        {
            throw new Exception("Semester not found.");
        }

        var subject = await ApplySubjectScope(
                _dbContext.Subjects.Where(x => x.Id == request.SubjectId))
            .FirstOrDefaultAsync();

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

        var nextDisplayOrder = await ApplySemesterSubjectScope(
                    _dbContext.SemesterSubjects
                        .Where(x => x.SemesterId == request.SemesterId))
                .Select(x => (int?)x.DisplayOrder)
                .MaxAsync() ?? 0;

        nextDisplayOrder++;

        var existing = await _dbContext.SemesterSubjects
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x =>
                    x.SemesterId == request.SemesterId &&
                    x.SubjectId == request.SubjectId);

        SemesterSubject semesterSubject;

        if (existing != null)
        {
            if (!existing.IsDeleted)
            {
                throw new Exception("Subject already assigned.");
            }

            existing.IsDeleted = false;
            existing.DisplayOrder = nextDisplayOrder;

            semesterSubject = existing;
        }
        else
        {
            semesterSubject = new SemesterSubject
            {
                Id = Guid.NewGuid(),

                SemesterId = request.SemesterId,

                SubjectId = request.SubjectId,

                DisplayOrder = nextDisplayOrder
            };

            _dbContext.SemesterSubjects.Add(semesterSubject);
        }

        await _dbContext.SaveChangesAsync();

        return new SemesterSubjectResponse
        {
            Id = semesterSubject.Id,

            SemesterId = semester.Id,

            SubjectId = subject.Id,

            DisplayOrder = semesterSubject.DisplayOrder,

            SemesterName = semester.Name,

            SubjectCode = subject.Code,

            SubjectName = subject.Name
        };
    }

    public async Task<List<SemesterSubjectResponse>> GetBySemesterAsync(Guid semesterId)
    {
        return await ApplySemesterSubjectScope(_dbContext.SemesterSubjects.Where(x => x.SemesterId == semesterId))
            .OrderBy(x => x.DisplayOrder)
            .Select(x =>
                new SemesterSubjectResponse
                {
                    Id = x.Id,

                    SemesterId = x.SemesterId,

                    SubjectId = x.SubjectId,

                    DisplayOrder = x.DisplayOrder,

                    SemesterName = x.Semester.Name,

                    SubjectCode = x.Subject.Code,

                    SubjectName = x.Subject.Name
                })
            .ToListAsync();
    }

    public async Task<List<CourseSemesterSubjectResponse>> GetByCourseAsync(Guid courseId)
    {
        var semesters = await ApplySemesterScope(_dbContext.Semesters
                        .Where(x => x.CourseId == courseId))
                .OrderBy(x => x.SequenceNumber)
                .ToListAsync();

        var semesterIds =
            semesters
                .Select(x => x.Id)
                .ToList();

        var semesterSubjects =
            await ApplySemesterSubjectScope(
                    _dbContext.SemesterSubjects
                        .Where(x => semesterIds.Contains(x.SemesterId)))
                .OrderBy(x => x.DisplayOrder)
                .Select(x =>
                    new SemesterSubjectResponse
                    {
                        Id = x.Id,

                        SemesterId = x.SemesterId,

                        SubjectId = x.SubjectId,

                        DisplayOrder = x.DisplayOrder,

                        SemesterName = x.Semester.Name,

                        SubjectCode = x.Subject.Code,

                        SubjectName = x.Subject.Name
                    })
                .ToListAsync();

        return semesters
            .Select(semester =>
                new CourseSemesterSubjectResponse
                {
                    SemesterId = semester.Id,

                    SemesterName = semester.Name,

                    SequenceNumber = semester.SequenceNumber,

                    Subjects =
                        semesterSubjects
                            .Where(x => x.SemesterId == semester.Id)
                            .OrderBy(x => x.DisplayOrder)
                            .ToList()
                })
            .ToList();
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

        var semesterId = semesterSubject.SemesterId;

        _dbContext.SemesterSubjects.Remove(semesterSubject);

        await _dbContext.SaveChangesAsync();

        var remaining =
            await ApplySemesterSubjectScope(
                    _dbContext.SemesterSubjects
                        .Where(x => x.SemesterId == semesterId))
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();

        for (int i = 0; i < remaining.Count; i++)
        {
            remaining[i].DisplayOrder = i + 1;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task MoveUpAsync(Guid id)
    {
        var item =
            await ApplySemesterSubjectScope(
                    _dbContext.SemesterSubjects
                        .Where(x => x.Id == id))
                .FirstOrDefaultAsync();

        if (item is null)
        {
            throw new Exception("Semester subject not found.");
        }

        var previous =
            await ApplySemesterSubjectScope(
                    _dbContext.SemesterSubjects
                        .Where(x =>
                            x.SemesterId == item.SemesterId &&
                            x.DisplayOrder == item.DisplayOrder - 1))
                .FirstOrDefaultAsync();

        if (previous is null)
        {
            return;
        }

        (item.DisplayOrder, previous.DisplayOrder) =
            (previous.DisplayOrder, item.DisplayOrder);

        await _dbContext.SaveChangesAsync();
    }

    public async Task MoveDownAsync(Guid id)
    {
        var item =
            await ApplySemesterSubjectScope(
                    _dbContext.SemesterSubjects
                        .Where(x => x.Id == id))
                .FirstOrDefaultAsync();

        if (item is null)
        {
            throw new Exception("Semester subject not found.");
        }

        var next =
            await ApplySemesterSubjectScope(
                    _dbContext.SemesterSubjects
                        .Where(x =>
                            x.SemesterId == item.SemesterId &&
                            x.DisplayOrder == item.DisplayOrder + 1))
                .FirstOrDefaultAsync();

        if (next is null)
        {
            return;
        }

        (item.DisplayOrder, next.DisplayOrder) =
            (next.DisplayOrder, item.DisplayOrder);

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