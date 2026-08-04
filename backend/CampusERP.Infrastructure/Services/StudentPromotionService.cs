using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class StudentPromotionService : IStudentPromotionService
{
    private readonly ApplicationDbContext _dbContext;

    public StudentPromotionService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<PromotionStudentResponse>> LoadStudentsAsync(LoadPromotionStudentsRequest request)
    {
        var currentSession = await _dbContext.AcademicSessions
            .FirstOrDefaultAsync(x =>
                x.IsCurrent &&
                x.IsActive);

        if (currentSession is null)
        {
            throw new Exception("Current academic session not found.");
        }

        var nextSession = await _dbContext.AcademicSessions
            .Where(x =>
                x.InstitutionId == currentSession.InstitutionId &&
                x.CampusId == currentSession.CampusId &&
                x.StartDate > currentSession.StartDate &&
                x.IsActive)
            .OrderBy(x => x.StartDate)
            .FirstOrDefaultAsync();

        if (nextSession is null)
        {
            throw new Exception("Next academic session has not been created.");
        }

        var currentSemester = await _dbContext.Semesters
            .FirstAsync(x => x.Id == request.SemesterId);

        var nextSemester = await _dbContext.Semesters
            .Where(x =>
                x.CourseId == currentSemester.CourseId &&
                x.SequenceNumber == currentSemester.SequenceNumber + 1)
            .FirstOrDefaultAsync();

        var students = await _dbContext.StudentEnrollments
            .Include(x => x.Student)
                .ThenInclude(x => x.User)
            .Include(x => x.Section)
            .Where(x =>
                x.IsCurrent &&
                x.AcademicSessionId == currentSession.Id &&
                x.DepartmentId == request.DepartmentId &&
                x.CourseId == request.CourseId &&
                x.SemesterId == request.SemesterId &&
                x.EnrollmentStatus == Shared.Enums.EnrollmentStatus.Active)
            .ToListAsync();

        var response = new List<PromotionStudentResponse>();

        foreach (var enrollment in students)
        {
            Guid nextSectionId = Guid.Empty;

            if (nextSemester is not null)
            {
                var section = await _dbContext.Sections
                    .Where(x =>
                        x.SemesterId == nextSemester.Id &&
                        x.Name == enrollment.Section.Name &&
                        x.IsActive)
                    .FirstOrDefaultAsync();

                nextSectionId = section?.Id ?? Guid.Empty;
            }

            response.Add(new PromotionStudentResponse
            {
                StudentId = enrollment.StudentId,

                AdmissionNumber = enrollment.Student.AdmissionNumber,

                RollNumber = enrollment.Student.RollNumber,

                StudentName =
                    $"{enrollment.Student.User.FirstName} {enrollment.Student.User.LastName}",

                CurrentSectionId = enrollment.SectionId,

                CurrentSectionName = enrollment.Section.Name,

                NextSemesterId = nextSemester?.Id ?? Guid.Empty,

                NextSemesterName = nextSemester?.Name ?? "Graduation",

                NextSectionId = nextSectionId,

                CurrentEnrollmentId = enrollment.Id,

                IsGraduating = nextSemester == null
            });
        }

        return response
            .OrderBy(x => x.RollNumber)
            .ToList();
    }

    public async Task PromoteAsync(PromoteStudentsRequest request)
    {
        if (request.Students.Count == 0)
        {
            return;
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            foreach (var item in request.Students)
            {
                var currentEnrollment =  await _dbContext.StudentEnrollments
                        .Include(x => x.AcademicSession)
                        .FirstOrDefaultAsync(x =>
                            x.Id == item.CurrentEnrollmentId &&
                            x.StudentId == item.StudentId &&
                            x.IsCurrent);

                if (currentEnrollment is null)
                {
                    throw new Exception("Student enrollment not found.");
                }

                var currentSemester = await _dbContext.Semesters
                        .FirstAsync(x =>
                            x.Id == currentEnrollment.SemesterId);

                var nextSemester = await _dbContext.Semesters
                        .FirstOrDefaultAsync(x =>
                            x.CourseId == currentEnrollment.CourseId &&
                            x.SequenceNumber ==
                            currentSemester.SequenceNumber + 1);

                currentEnrollment.IsCurrent = false;

                currentEnrollment.PromotionStatus = Shared.Enums.PromotionStatus.Promoted;

                if (nextSemester == null)
                {
                    currentEnrollment.EnrollmentStatus = Shared.Enums.EnrollmentStatus.Graduated;

                    continue;
                }

                var nextSession = await _dbContext.AcademicSessions
                        .Where(x =>
                            x.InstitutionId ==
                            currentEnrollment.InstitutionId &&
                            x.CampusId ==
                            currentEnrollment.CampusId &&
                            x.StartDate >
                            currentEnrollment.AcademicSession.StartDate &&
                            x.IsActive)
                        .OrderBy(x => x.StartDate)
                        .FirstOrDefaultAsync();

                if (nextSession == null)
                {
                    throw new Exception("Next academic session not found.");
                }

                var newEnrollment = new StudentEnrollment
                                    {
                                        Id = Guid.NewGuid(),

                                        StudentId = currentEnrollment.StudentId,

                                        InstitutionId = currentEnrollment.InstitutionId,

                                        CampusId = currentEnrollment.CampusId,

                                        DepartmentId = currentEnrollment.DepartmentId,

                                        CourseId = currentEnrollment.CourseId,

                                        SemesterId = nextSemester.Id,

                                        SectionId = item.NextSectionId,

                                        AcademicSessionId = nextSession.Id,

                                        EnrollmentStatus = Shared.Enums.EnrollmentStatus.Active,

                                        PromotionStatus = Shared.Enums.PromotionStatus.Promoted,

                                        IsCurrent = true
                                    };

                _dbContext.StudentEnrollments.Add(newEnrollment);
            }

            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();

            throw;
        }
    }
}