using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class TeacherService : ITeacherService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordService _passwordService;

    public TeacherService(ApplicationDbContext dbContext,IPasswordService passwordService)
    {
        _dbContext = dbContext;
        _passwordService = passwordService;
    }

    public async Task<TeacherResponse> CreateAsync(CreateTeacherRequest request)
    {
        var departmentExists = await _dbContext.Departments
            .AnyAsync(x => x.Id == request.DepartmentId &&
                           x.CampusId == request.CampusId &&
                           x.InstitutionId == request.InstitutionId);

        if (!departmentExists)
        {
            throw new Exception("Department not found.");
        }

        var emailExists = await _dbContext.Users
            .AnyAsync(x => x.CampusId == request.CampusId &&
                           x.Email == request.Email);

        if (emailExists)
        {
            throw new Exception("Email already exists.");
        }

        var employeeCodeExists = await _dbContext.Teachers
            .AnyAsync(x => x.CampusId == request.CampusId &&
                           x.EmployeeCode == request.EmployeeCode);

        if (employeeCodeExists)
        {
            throw new Exception("Employee code already exists.");
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
            RoleId = SeedData.TeacherRoleId
        };

        _dbContext.UserRoles.Add(userRole);

        var teacher = new Teacher
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            InstitutionId = request.InstitutionId,
            CampusId = request.CampusId,
            DepartmentId = request.DepartmentId,
            EmployeeCode = request.EmployeeCode,
            Designation = request.Designation
        };

        _dbContext.Teachers.Add(teacher);

        await _dbContext.SaveChangesAsync();

        return new TeacherResponse
        {
            Id = teacher.Id,
            UserId = user.Id,
            InstitutionId = teacher.InstitutionId,
            CampusId = teacher.CampusId,
            DepartmentId = teacher.DepartmentId,
            EmployeeCode = teacher.EmployeeCode,
            Designation = teacher.Designation,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };
    }

    public async Task<List<TeacherResponse>> GetAllAsync()
    {
        return await _dbContext.Teachers
            .Include(x => x.User)
            .Select(x => new TeacherResponse
            {
                Id = x.Id,
                UserId = x.UserId,
                InstitutionId = x.InstitutionId,
                CampusId = x.CampusId,
                DepartmentId = x.DepartmentId,
                EmployeeCode = x.EmployeeCode,
                Designation = x.Designation,
                FirstName = x.User.FirstName,
                LastName = x.User.LastName,
                Email = x.User.Email,
                PhoneNumber = x.User.PhoneNumber
            })
            .ToListAsync();
    }

    public async Task<TeacherResponse?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Teachers
            .Include(x => x.User)
            .Where(x => x.Id == id)
            .Select(x => new TeacherResponse
            {
                Id = x.Id,
                UserId = x.UserId,
                InstitutionId = x.InstitutionId,
                CampusId = x.CampusId,
                DepartmentId = x.DepartmentId,
                EmployeeCode = x.EmployeeCode,
                Designation = x.Designation,
                FirstName = x.User.FirstName,
                LastName = x.User.LastName,
                Email = x.User.Email,
                PhoneNumber = x.User.PhoneNumber
            })
            .FirstOrDefaultAsync();
    }
}