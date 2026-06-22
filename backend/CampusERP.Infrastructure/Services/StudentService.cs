using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using CampusERP.Shared.Constants;
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

        var user = new User
        {
            Id = Guid.NewGuid(),

            FirstName = request.FirstName,

            LastName = request.LastName,

            Email = request.Email,

            PhoneNumber = request.PhoneNumber,

            PasswordHash = _passwordService.HashPassword(request.Password),

            InstitutionId = request.InstitutionId,

            CampusId = request.CampusId,

            IsActive = true
        };

        _dbContext.Users.Add(user);

        _dbContext.UserRoles.Add(
            new UserRole
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

            DepartmentId = request.DepartmentId,

            CourseId = request.CourseId,

            RollNumber = request.RollNumber,

            Batch = request.Batch,

            AdmissionDate = request.AdmissionDate,

            IsActive = true
        };

        _dbContext.Students.Add(student);

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(student.Id) ?? throw new Exception();
    }

    public async Task<List<StudentResponse>> GetAllAsync()
    {
        var query = ApplyStudentScope(_dbContext.Students.Include(x => x.User).AsQueryable());

        return await query
            .Select(x =>
                new StudentResponse
                {
                    Id = x.Id,

                    UserId = x.UserId,

                    InstitutionId = x.InstitutionId,

                    CampusId = x.CampusId,

                    DepartmentId = x.DepartmentId,

                    CourseId = x.CourseId,

                    RollNumber = x.RollNumber,

                    Batch = x.Batch,

                    AdmissionDate = x.AdmissionDate,

                    FirstName = x.User.FirstName,

                    LastName = x.User.LastName,

                    Email = x.User.Email,

                    PhoneNumber = x.User.PhoneNumber,

                    IsActive = x.User.IsActive
                })
            .ToListAsync();
    }

    public async Task<StudentResponse?> GetByIdAsync(Guid id)
    {
        var query = ApplyStudentScope(_dbContext.Students.Include(x => x.User).Where(x => x.Id == id));

        return await query
            .Select(x =>
                new StudentResponse
                {
                    Id = x.Id,

                    UserId = x.UserId,

                    InstitutionId = x.InstitutionId,

                    CampusId = x.CampusId,

                    DepartmentId = x.DepartmentId,

                    CourseId = x.CourseId,

                    RollNumber = x.RollNumber,

                    Batch = x.Batch,

                    AdmissionDate = x.AdmissionDate,

                    FirstName = x.User.FirstName,

                    LastName = x.User.LastName,

                    Email = x.User.Email,

                    PhoneNumber = x.User.PhoneNumber,

                    IsActive = x.User.IsActive
                })
            .FirstOrDefaultAsync();
    }

    public async Task<StudentResponse> UpdateAsync(Guid id, UpdateStudentRequest request)
    {
        var student = await ApplyStudentScope(_dbContext.Students.Include(x => x.User).Where(x => x.Id == id)).FirstOrDefaultAsync();

        if (student is null)
        {
            throw new Exception("Student not found.");
        }

        var departmentExists =
            await _dbContext.Departments
                .AnyAsync(x =>
                    x.Id == request.DepartmentId &&
                    x.InstitutionId == student.InstitutionId);

        if (!departmentExists)
        {
            throw new Exception("Department not found.");
        }

        var course =
            await _dbContext.Courses
                .FirstOrDefaultAsync(x =>
                    x.Id == request.CourseId);

        if (course is null)
        {
            throw new Exception("Course not found.");
        }

        if (course.InstitutionId != student.InstitutionId)
        {
            throw new Exception("Invalid course.");
        }

        if (course.DepartmentId != request.DepartmentId)
        {
            throw new Exception("Course does not belong to department.");
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
                    x.Id != id &&
                    x.RollNumber == request.RollNumber);

        if (rollExists)
        {
            throw new Exception("Roll number already exists.");
        }

        student.RollNumber = request.RollNumber;

        student.Batch = request.Batch;

        student.AdmissionDate = request.AdmissionDate;

        student.DepartmentId = request.DepartmentId;

        student.CourseId = request.CourseId;

        student.User.FirstName = request.FirstName;

        student.User.LastName = request.LastName;

        student.User.Email = request.Email;

        student.User.PhoneNumber = request.PhoneNumber;

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id) ?? throw new Exception();
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
}