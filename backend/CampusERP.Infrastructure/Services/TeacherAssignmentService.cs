using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using DocumentFormat.OpenXml.Vml.Office;
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

    public async Task<List<TeacherAssignmentListResponse>> GetAllAsync()
    {
        return await BuildQuery()
            .Select(x => new TeacherAssignmentListResponse
            {
                Id = x.Id,

                TeacherId = x.TeacherId,

                TeacherName = x.Teacher.User.FirstName + " " + x.Teacher.User.LastName,

                AcademicSessionId = x.AcademicSessionId,

                AcademicSessionName = x.AcademicSession.Name,

                SectionId = x.SectionId,

                SectionName = x.Section.Name,

                SemesterSubjectId = x.SemesterSubjectId,

                SubjectName = x.SemesterSubject.Subject.Name,

                SemesterName = x.SemesterSubject.Semester.Name,

                CourseName = x.Section.Course.Name,

                ClassDisplayName = x.Section.Course.Name + " • " +
                                x.SemesterSubject.Semester.Name + " • Section " +
                                x.Section.Name
            })
            .ToListAsync();
    }

    public async Task<TeacherAssignmentResponse?> GetByIdAsync(Guid id)
    {
        var entity = await BuildQuery().FirstOrDefaultAsync(x => x.Id == id);

        return entity is null ? null: MapResponse(entity);
    }

    #region Query Builders

    private IQueryable<TeacherAssignment> BuildQuery()
    {
        return ApplyScope(_dbContext.TeacherAssignments
                    .AsNoTracking())
            .Include(x => x.Teacher)
                .ThenInclude(x => x.User)
            .Include(x => x.Section)
                .ThenInclude(x => x.Course)
            .Include(x => x.AcademicSession)
            .Include(x => x.SemesterSubject)
                .ThenInclude(x => x.Subject)
            .Include(x => x.SemesterSubject)
                .ThenInclude(x => x.Semester);
    }

    #endregion

    #region Entity Loaders

    private async Task<Teacher> GetTeacherAsync(Guid teacherId)
    {
        return await _dbContext.Teachers
                   .Include(x => x.User)
                   .FirstOrDefaultAsync(x => x.Id == teacherId)
               ?? throw new Exception("Teacher not found.");
    }

    private async Task<Section> GetSectionAsync(Guid sectionId)
    {
        return await _dbContext.Sections
                   .Include(x => x.Course)
                   .Include(x => x.Semester)
                   .FirstOrDefaultAsync(x => x.Id == sectionId)
               ?? throw new Exception("Section not found.");
    }

    private async Task<SemesterSubject> GetSemesterSubjectAsync(Guid semesterSubjectId)
    {
        return await _dbContext.SemesterSubjects
                   .Include(x => x.Subject)
                   .Include(x => x.Semester)
                   .FirstOrDefaultAsync(x => x.Id == semesterSubjectId)
               ?? throw new Exception("Semester subject not found.");
    }

    private async Task<AcademicSession> GetAcademicSessionAsync(Guid academicSessionId)
    {
        return await _dbContext.AcademicSessions
                   .FirstOrDefaultAsync(x => x.Id == academicSessionId)
               ?? throw new Exception("Academic session not found.");
    }

    #endregion

    #region Mapping

    private static TeacherAssignmentResponse MapResponse(TeacherAssignment entity)
    {
        return new TeacherAssignmentResponse
        {
            Id = entity.Id,

            TeacherId = entity.TeacherId,

            TeacherName = $"{entity.Teacher.User.FirstName} {entity.Teacher.User.LastName}",

            AcademicSessionId = entity.AcademicSessionId,

            AcademicSessionName = entity.AcademicSession.Name,

            SectionId = entity.SectionId,

            SectionName = entity.Section.Name,

            SemesterSubjectId = entity.SemesterSubjectId,

            SubjectId = entity.SemesterSubject.SubjectId,

            SubjectName = entity.SemesterSubject.Subject.Name,

            SemesterId = entity.SemesterSubject.SemesterId,

            SemesterName = entity.SemesterSubject.Semester.Name,

            CourseId = entity.Section.CourseId,

            CourseName = entity.Section.Course.Name,

            ClassDisplayName = GetClassDisplayName(entity)
        };
    }
    #endregion

    #region CRUD

    public async Task<TeacherAssignmentResponse> CreateAsync(CreateTeacherAssignmentRequest request)
    {
        await ValidateCreateScopeAsync(request.SectionId);

        var teacher = await GetTeacherAsync(request.TeacherId);

        var section = await GetSectionAsync(request.SectionId);

        var semesterSubject = await GetSemesterSubjectAsync(request.SemesterSubjectId);

        var academicSession = await GetAcademicSessionAsync(request.AcademicSessionId);

        ValidateAssignment(teacher, section, semesterSubject, academicSession);

        await CheckDuplicateAssignmentAsync(request.AcademicSessionId, request.SectionId, request.SemesterSubjectId);

        var assignment = new TeacherAssignment
        {
            Id = Guid.NewGuid(),

            TeacherId = teacher.Id,

            Teacher = teacher,

            SectionId = section.Id,

            Section = section,

            SemesterSubjectId = semesterSubject.Id,

            SemesterSubject = semesterSubject,

            AcademicSessionId = academicSession.Id,

            AcademicSession = academicSession
        };

        _dbContext.TeacherAssignments.Add(assignment);

        await _dbContext.SaveChangesAsync();

        return MapResponse(assignment);
    }

    public async Task<TeacherAssignmentResponse> UpdateAsync(Guid id, UpdateTeacherAssignmentRequest request)
    {
        var assignment = await ApplyScope(_dbContext.TeacherAssignments)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (assignment is null)
        {
            throw new Exception("Teacher assignment not found.");
        }

        await ValidateCreateScopeAsync(request.SectionId);

        var teacher = await GetTeacherAsync(request.TeacherId);

        var section = await GetSectionAsync(request.SectionId);

        var semesterSubject = await GetSemesterSubjectAsync(request.SemesterSubjectId);

        var academicSession = await GetAcademicSessionAsync(request.AcademicSessionId);

        ValidateAssignment(teacher, section, semesterSubject, academicSession);

        await CheckDuplicateAssignmentAsync(request.AcademicSessionId, request.SectionId, request.SemesterSubjectId, id);

        assignment.TeacherId = teacher.Id;
        assignment.Teacher = teacher;

        assignment.SectionId = section.Id;
        assignment.Section = section;

        assignment.SemesterSubjectId = semesterSubject.Id;
        assignment.SemesterSubject = semesterSubject;

        assignment.AcademicSessionId = academicSession.Id;
        assignment.AcademicSession = academicSession;

        await _dbContext.SaveChangesAsync();

        return MapResponse(assignment);
    }

    #endregion

    #region Delete

    public async Task DeleteAsync(Guid id)
    {
        var assignment = await ApplyScope(_dbContext.TeacherAssignments)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (assignment is null)
        {
            throw new Exception("Teacher assignment not found.");
        }

        _dbContext.TeacherAssignments.Remove(assignment);

        await _dbContext.SaveChangesAsync();
    }

    #endregion

    #region Scope

    private IQueryable<TeacherAssignment> ApplyScope(IQueryable<TeacherAssignment> query)
    {
        if (_scope.IsSuperAdmin() || _scope.IsPlatformAdmin())
        {
            return query;
        }

        if (_scope.IsInstitutionAdmin())
        {
            query = query.Where(x => x.Section.InstitutionId == _scope.InstitutionId());
        }

        if (_scope.IsCampusAdmin())
        {
            query = query.Where(x => x.Section.CampusId == _scope.CampusId());
        }

        return query;
    }

    private async Task ValidateCreateScopeAsync(Guid sectionId)
    {
        if (_scope.IsSuperAdmin() || _scope.IsPlatformAdmin())
        {
            return;
        }

        var section = await _dbContext.Sections
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == sectionId);

        if (section is null)
        {
            throw new Exception("Section not found.");
        }

        if (_scope.IsInstitutionAdmin() && section.InstitutionId != _scope.InstitutionId())
        {
            throw new Exception("Access denied.");
        }

        if (_scope.IsCampusAdmin() && section.CampusId != _scope.CampusId())
        {
            throw new Exception("Access denied.");
        }
    }

    #endregion

    #region Validation

    private static void ValidateAssignment(Teacher teacher, Section section, SemesterSubject semesterSubject, AcademicSession academicSession)
    {
        if (!teacher.IsActive)
        {
            throw new Exception("Teacher is inactive.");
        }

        if (!section.IsActive)
        {
            throw new Exception("Section is inactive.");
        }

        if (!academicSession.IsActive)
        {
            throw new Exception("Academic session is inactive.");
        }

        if (teacher.InstitutionId != section.InstitutionId)
        {
            throw new Exception("Teacher and section belong to different institutions.");
        }

        if (teacher.CampusId != section.CampusId)
        {
            throw new Exception("Teacher and section belong to different campuses.");
        }

        if (semesterSubject.SemesterId != section.SemesterId)
        {
            throw new Exception("Selected subject does not belong to the selected semester.");
        }

        if (semesterSubject.Semester.CourseId != section.CourseId)
        {
            throw new Exception("Selected subject does not belong to the selected course.");
        }
    }

    private async Task CheckDuplicateAssignmentAsync(Guid academicSessionId, Guid sectionId, Guid semesterSubjectId, Guid? currentAssignmentId = null)
    {
        var query = _dbContext.TeacherAssignments
            .Where(x =>
                x.AcademicSessionId == academicSessionId &&
                x.SectionId == sectionId &&
                x.SemesterSubjectId == semesterSubjectId);

        if (currentAssignmentId.HasValue)
        {
            query = query.Where(x => x.Id != currentAssignmentId.Value);
        }

        if (await query.AnyAsync())
        {
            throw new Exception("A teacher has already been assigned to this subject for the selected section and academic session.");
        }
    }

    #endregion

    #region Helpers

    private static string GetClassDisplayName(TeacherAssignment entity)
    {
        return
            $"{entity.Section.Course.Name} • " +
            $"{entity.SemesterSubject.Semester.Name} • " +
            $"Section {entity.Section.Name}";
    }

    #endregion
}