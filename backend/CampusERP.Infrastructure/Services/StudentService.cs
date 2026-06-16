using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class StudentService : IStudentService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordService _passwordService;

    public StudentService(ApplicationDbContext dbContext, IPasswordService passwordService)
    {
        _dbContext = dbContext;
        _passwordService = passwordService;
    }

    public async Task<StudentResponse> CreateAsync(CreateStudentRequest request)
    {
        var departmentExists = await _dbContext.Departments
            .AnyAsync(x => x.Id == request.DepartmentId &&
                           x.CampusId == request.CampusId &&
                           x.InstitutionId == request.InstitutionId);

        if (!departmentExists)
        {
            throw new Exception("Department not found.");
        }

        var course = await _dbContext.Courses
            .FirstOrDefaultAsync(x => x.Id == request.CourseId &&
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

        var emailExists = await _dbContext.Users
            .AnyAsync(x => x.CampusId == request.CampusId &&
                           x.Email == request.Email);

        if (emailExists)
        {
            throw new Exception("Email already exists.");
        }

        var rollNumberExists = await _dbContext.Students
            .AnyAsync(x => x.CampusId == request.CampusId &&
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

        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RoleId = SeedData.StudentRoleId
        };

        _dbContext.UserRoles.Add(userRole);

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
            AdmissionDate = request.AdmissionDate
        };

        _dbContext.Students.Add(student);

        await _dbContext.SaveChangesAsync();

        return new StudentResponse
        {
            Id = student.Id,
            UserId = user.Id,
            InstitutionId = student.InstitutionId,
            CampusId = student.CampusId,
            DepartmentId = student.DepartmentId,
            CourseId = student.CourseId,
            RollNumber = student.RollNumber,
            Batch = student.Batch,
            AdmissionDate = student.AdmissionDate,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };
    }

    public async Task<List<StudentResponse>> GetAllAsync()
    {
        return await _dbContext.Students
            .Include(x => x.User)
            .Select(x => new StudentResponse
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
                PhoneNumber = x.User.PhoneNumber
            })
            .ToListAsync();
    }

    public async Task<StudentResponse?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Students
            .Include(x => x.User)
            .Where(x => x.Id == id)
            .Select(x => new StudentResponse
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
                PhoneNumber = x.User.PhoneNumber
            })
            .FirstOrDefaultAsync();
    }
}