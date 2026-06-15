using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class TeacherAssignmentService: ITeacherAssignmentService
{
    private readonly ApplicationDbContext _dbContext;

    public TeacherAssignmentService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TeacherAssignmentResponse> AssignAsync(AssignTeacherRequest request)
    {
        var teacher = await _dbContext.Teachers.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == request.TeacherId);

        if (teacher is null)
        {
            throw new Exception("Teacher not found.");
        }

        var semesterSubject = await _dbContext.SemesterSubjects
                                .Include(x => x.Semester)
                                .Include(x => x.Subject)
                                .FirstOrDefaultAsync(x => x.Id == request.SemesterSubjectId);

        if (semesterSubject is null)
        {
            throw new Exception("Semester subject not found.");
        }

        if (teacher.CampusId != semesterSubject.Semester.CampusId)
        {
            throw new Exception("Campus mismatch.");
        }

        if (teacher.InstitutionId != semesterSubject.Semester.InstitutionId)
        {
            throw new Exception("Institution mismatch.");
        }

        var exists = await _dbContext
                        .TeacherAssignments
                        .AnyAsync(x =>
                            x.TeacherId == request.TeacherId &&
                            x.SemesterSubjectId == request.SemesterSubjectId);

        if (exists)
        {
            throw new Exception("Teacher already assigned.");
        }

        var assignment = new TeacherAssignment
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

            TeacherId = teacher.Id,

            SemesterSubjectId = semesterSubject.Id,

            TeacherName = $"{teacher.User.FirstName} {teacher.User.LastName}",

            SubjectName = semesterSubject.Subject.Name,

            SemesterName = semesterSubject.Semester.Name
        };
    }

    public async Task<List<TeacherAssignmentResponse>> GetByTeacherAsync(Guid teacherId)
    {
        return await _dbContext
            .TeacherAssignments
            .Where(x =>
                x.TeacherId == teacherId)
            .Select(x =>
                new TeacherAssignmentResponse
                {
                    Id = x.Id,

                    TeacherId = x.TeacherId,

                    SemesterSubjectId = x.SemesterSubjectId,

                    TeacherName = x.Teacher.User.FirstName + " " + x.Teacher.User.LastName,

                    SubjectName = x.SemesterSubject.Subject.Name,

                    SemesterName = x.SemesterSubject.Semester.Name
                })
            .ToListAsync();
    }
}