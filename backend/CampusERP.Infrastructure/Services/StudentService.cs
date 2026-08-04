using CampusERP.Application.Interfaces;
using CampusERP.Shared.Enums;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using CampusERP.Shared.Constants;
using CampusERP.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class StudentService : IStudentService
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IPasswordService _passwordService;

    private readonly IDataAccessScope _scope;

    public StudentService(ApplicationDbContext dbContext, IPasswordService passwordService, IDataAccessScope scope)
    {
        _dbContext = dbContext;

        _passwordService = passwordService;

        _scope = scope;
    }

    public async Task<StudentResponse> CreateAsync(CreateStudentRequest request)
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

        var course =
            await _dbContext.Courses
                .FirstOrDefaultAsync(x =>
                    x.Id == request.CourseId &&
                    x.CampusId == request.CampusId &&
                    x.InstitutionId == request.InstitutionId);

        if (course is null)
        {
            throw new Exception("Course not found.");
        }

        if (course.DepartmentId != request.DepartmentId)
        {
            throw new Exception("Course does not belong to the selected department.");
        }

        var semester =
            await _dbContext.Semesters
                .FirstOrDefaultAsync(x =>
                    x.Id == request.SemesterId &&
                    x.CourseId == request.CourseId &&
                    x.CampusId == request.CampusId &&
                    x.InstitutionId == request.InstitutionId);

        if (semester is null)
        {
            throw new Exception("Semester not found.");
        }

        var section =
            await _dbContext.Sections
                .FirstOrDefaultAsync(x =>
                    x.Id == request.SectionId &&
                    x.SemesterId == request.SemesterId &&
                    x.CampusId == request.CampusId &&
                    x.InstitutionId == request.InstitutionId);

        if (section is null)
        {
            throw new Exception("Section not found.");
        }

        var academicSession =
            await _dbContext.AcademicSessions
                .FirstOrDefaultAsync(x =>
                    x.InstitutionId == request.InstitutionId &&
                    x.CampusId == request.CampusId &&
                    x.IsCurrent &&
                    x.IsActive);

        if (academicSession is null)
        {
            throw new Exception("No current academic session has been configured.");
        }

        var emailExists =
            await _dbContext.Users
                .AnyAsync(x =>
                    x.CampusId == request.CampusId &&
                    x.Email == request.Email);

        if (emailExists)
        {
            throw new Exception("Email already exists.");
        }

        var rollNumberExists =
            await _dbContext.Students
                .AnyAsync(x =>
                    x.CampusId == request.CampusId &&
                    x.RollNumber == request.RollNumber);

        if (rollNumberExists)
        {
            throw new Exception("Roll number already exists.");
        }

        var admissionNumberExists =
            await _dbContext.Students
                .AnyAsync(x =>
                    x.CampusId == request.CampusId &&
                    x.AdmissionNumber == request.AdmissionNumber);

        if (admissionNumberExists)
        {
            throw new Exception("Admission number already exists.");
        }

        var password = PasswordGenerator.Generate();

        var user = new User
        {
            Id = Guid.NewGuid(),

            FirstName = request.FirstName,

            LastName = request.LastName,

            Email = request.Email,

            PhoneNumber = request.PhoneNumber,

            PasswordHash = _passwordService.HashPassword(password),

            InstitutionId = request.InstitutionId,

            CampusId = request.CampusId,

            IsActive = true
        };

        _dbContext.Users.Add(user);

        _dbContext.UserRoles.Add(new UserRole
        {
            Id = Guid.NewGuid(),

            UserId = user.Id,

            RoleId = SeedData.StudentRoleId
        });

        var student = new Student
        {
            Id = Guid.NewGuid(),

            UserId = user.Id,

            InstitutionId = request.InstitutionId,

            CampusId = request.CampusId,

            AdmissionNumber = request.AdmissionNumber,

            RollNumber = request.RollNumber,

            Batch = request.Batch,

            AdmissionDate = request.AdmissionDate,

            IsActive = true
        };

        _dbContext.Students.Add(student);

        var enrollment = new StudentEnrollment
        {
            Id = Guid.NewGuid(),

            StudentId = student.Id,

            InstitutionId = request.InstitutionId,

            CampusId = request.CampusId,

            DepartmentId = request.DepartmentId,

            CourseId = request.CourseId,

            SemesterId = request.SemesterId,

            SectionId = request.SectionId,

            AcademicSessionId = academicSession.Id,

            EnrollmentStatus = EnrollmentStatus.Active,

            PromotionStatus = semester.SequenceNumber == 1
                ? PromotionStatus.NewAdmission
                : PromotionStatus.LateralEntry,

            IsCurrent = true
        };

        _dbContext.StudentEnrollments.Add(enrollment);

        await _dbContext.SaveChangesAsync();

        var response = await GetByIdAsync(student.Id)
                       ?? throw new Exception();

        response.TemporaryPassword = password;

        return response;
    }

    public async Task<List<StudentResponse>> GetAllAsync()
    {
        var students =
            await ApplyStudentScope(
                    _dbContext.Students
                        .Include(x => x.User)
                        .Include(x => x.Campus)
                        .Include(x => x.Enrollments.Where(e => e.IsCurrent))
                            .ThenInclude(e => e.Department)
                        .Include(x => x.Enrollments.Where(e => e.IsCurrent))
                            .ThenInclude(e => e.Course)
                        .Include(x => x.Enrollments.Where(e => e.IsCurrent))
                            .ThenInclude(e => e.Semester)
                        .Include(x => x.Enrollments.Where(e => e.IsCurrent))
                            .ThenInclude(e => e.Section)
                        .Include(x => x.Enrollments.Where(e => e.IsCurrent))
                            .ThenInclude(e => e.AcademicSession))
                .ToListAsync();

        return students.Select(MapStudent).ToList();
    }

    public async Task<StudentResponse?> GetByIdAsync(Guid id)
    {
        var student =
            await ApplyStudentScope(
                    _dbContext.Students
                        .Include(x => x.User)
                        .Include(x => x.Campus)
                        .Include(x => x.Enrollments.Where(e => e.IsCurrent))
                            .ThenInclude(e => e.Department)
                        .Include(x => x.Enrollments.Where(e => e.IsCurrent))
                            .ThenInclude(e => e.Course)
                        .Include(x => x.Enrollments.Where(e => e.IsCurrent))
                            .ThenInclude(e => e.Semester)
                        .Include(x => x.Enrollments.Where(e => e.IsCurrent))
                            .ThenInclude(e => e.Section)
                        .Include(x => x.Enrollments.Where(e => e.IsCurrent))
                            .ThenInclude(e => e.AcademicSession))
                .FirstOrDefaultAsync(x => x.Id == id);

        if (student is null)
        {
            return null;
        }

        return MapStudent(student);
    }

    public async Task<StudentResponse> UpdateAsync(Guid id, UpdateStudentRequest request)
    {
        var student = await ApplyStudentScope(
                _dbContext.Students
                    .Include(x => x.User))
            .FirstOrDefaultAsync(x => x.Id == id);

        if (student is null)
        {
            throw new Exception("Student not found.");
        }

        var emailExists =
            await _dbContext.Users
                .AnyAsync(x =>
                    x.Id != student.UserId &&
                    x.Email == request.Email);

        if (emailExists)
        {
            throw new Exception("Email already exists.");
        }

        var rollExists =
            await _dbContext.Students
                .AnyAsync(x =>
                    x.Id != student.Id &&
                    x.CampusId == student.CampusId &&
                    x.RollNumber == request.RollNumber);

        if (rollExists)
        {
            throw new Exception("Roll number already exists.");
        }

        var admissionExists =
            await _dbContext.Students
                .AnyAsync(x =>
                    x.Id != student.Id &&
                    x.CampusId == student.CampusId &&
                    x.AdmissionNumber == request.AdmissionNumber);

        if (admissionExists)
        {
            throw new Exception("Admission number already exists.");
        }

        student.AdmissionNumber = request.AdmissionNumber.Trim();

        student.RollNumber = request.RollNumber.Trim();

        student.Batch = request.Batch.Trim();

        student.AdmissionDate = request.AdmissionDate;

        student.User.FirstName = request.FirstName.Trim();

        student.User.LastName = request.LastName.Trim();

        student.User.Email = request.Email.Trim();

        student.User.PhoneNumber = request.PhoneNumber?.Trim();

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(student.Id)
               ?? throw new Exception();
    }

    public async Task ActivateAsync(Guid id)
    {
        var student = await ApplyStudentScope(_dbContext.Students.Include(x => x.User).Where(x => x.Id == id)).FirstOrDefaultAsync();

        if (student is null)
        {
            throw new Exception("Student not found.");
        }

        student.User.IsActive = true;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeactivateAsync(Guid id)
    {
        var student = await ApplyStudentScope(_dbContext.Students.Include(x => x.User).Where(x => x.Id == id)).FirstOrDefaultAsync();

        if (student is null)
        {
            throw new Exception("Student not found.");
        }

        student.User.IsActive = false;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<LookupResponse>> GetLookupAsync()
    {
        var query = ApplyStudentScope(_dbContext.Students.Include(x => x.User).Where(x => x.User.IsActive));

        return await query.OrderBy(x => x.User.FirstName).Select(x =>
                new LookupResponse
                {
                    Id = x.Id,

                    Name = $"{x.User.FirstName} {x.User.LastName}"
                })
            .ToListAsync();
    }

    private IQueryable<Student> ApplyStudentScope(IQueryable<Student> query)
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

    private static StudentResponse MapStudent(Student student)
    {
        var enrollment = student.Enrollments.Single(e => e.IsCurrent);

        return new StudentResponse
        {
            Id = student.Id,

            UserId = student.UserId,

            InstitutionId = student.InstitutionId,

            CampusId = student.CampusId,

            DepartmentId = enrollment.DepartmentId,

            CourseId = enrollment.CourseId,

            SemesterId = enrollment.SemesterId,

            SectionId = enrollment.SectionId,

            AcademicSessionId = enrollment.AcademicSessionId,

            AdmissionNumber = student.AdmissionNumber,

            RollNumber = student.RollNumber,

            Batch = student.Batch,

            AdmissionDate = student.AdmissionDate,

            FirstName = student.User.FirstName,

            LastName = student.User.LastName,

            Email = student.User.Email,

            PhoneNumber = student.User.PhoneNumber,

            IsActive = student.User.IsActive,

            CampusName = student.Campus.Name,

            DepartmentName = enrollment.Department.Name,

            CourseName = enrollment.Course.Name,

            SemesterName = enrollment.Semester.Name,

            SectionName = enrollment.Section == null ? null: $"Section {enrollment.Section.Name}",

            AcademicSessionName = enrollment.AcademicSession.Name,

            EnrollmentStatus = (EnrollmentStatus)enrollment.EnrollmentStatus,

            EnrollmentStatusName = enrollment.EnrollmentStatus.ToString(),

            PromotionStatus = (PromotionStatus)enrollment.PromotionStatus,

            PromotionStatusName = enrollment.PromotionStatus.ToString()
        };
    }
}