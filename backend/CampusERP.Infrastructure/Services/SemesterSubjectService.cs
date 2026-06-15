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

    public SemesterSubjectService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SemesterSubjectResponse> AssignAsync(AssignSubjectToSemesterRequest request)
    {
        var semester = await _dbContext.Semesters.FirstOrDefaultAsync(x => x.Id == request.SemesterId);

        if (semester is null)
        {
            throw new Exception("Semester not found.");
        }

        var subject = await _dbContext.Subjects.FirstOrDefaultAsync(x => x.Id == request.SubjectId);

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
                    x.SemesterId ==
                        request.SemesterId &&
                    x.SubjectId ==
                        request.SubjectId);

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

            SubjectId =
                subject.Id,

            SemesterName = semester.Name,

            SubjectCode = subject.Code,

            SubjectName = subject.Name
        };
    }

    public async Task<List<SemesterSubjectResponse>> GetBySemesterAsync(Guid semesterId)
    {
        return await _dbContext
            .SemesterSubjects
            .Where(x =>
                x.SemesterId ==
                    semesterId)
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
}