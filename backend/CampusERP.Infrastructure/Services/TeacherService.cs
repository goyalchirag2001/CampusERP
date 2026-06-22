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

    private readonly IDataAccessScope _scope;

    public TeacherService(ApplicationDbContext dbContext, IPasswordService passwordService, IDataAccessScope scope)
    {
        _dbContext = dbContext;

        _passwordService = passwordService;

        _scope = scope;
    }

    public async Task<TeacherResponse> CreateAsync(CreateTeacherRequest request)
    {
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

        var emailExists =
            await _dbContext.Users
                .AnyAsync(x =>
                    x.Email == request.Email);

        if (emailExists)
        {
            throw new Exception("Email already exists.");
        }

        var employeeCodeExists =
            await _dbContext.Teachers
                .AnyAsync(x =>
                    x.CampusId == request.CampusId &&
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

        _dbContext.UserRoles.Add(
            new UserRole
            {
                Id = Guid.NewGuid(),

                UserId = user.Id,

                RoleId = SeedData.TeacherRoleId
            });

        var teacher = new Teacher
        {
            Id = Guid.NewGuid(),

            UserId = user.Id,

            InstitutionId = request.InstitutionId,

            CampusId = request.CampusId,

            DepartmentId = request.DepartmentId,

            EmployeeCode = request.EmployeeCode,

            Designation = request.Designation,

            IsActive = true
        };

        _dbContext.Teachers.Add(teacher);

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(teacher.Id)
               ?? throw new Exception();
    }

    public async Task<List<TeacherResponse>> GetAllAsync()
    {
        return await ApplyScope(_dbContext.Teachers)
            .Include(x => x.User)
            .Select(x =>
                new TeacherResponse
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

                    PhoneNumber = x.User.PhoneNumber,

                    IsActive = x.User.IsActive
                })
            .ToListAsync();
    }

    public async Task<TeacherResponse?> GetByIdAsync(Guid id)
    {
        return await ApplyScope(_dbContext.Teachers)
            .Include(x => x.User)
            .Where(x => x.Id == id)
            .Select(x =>
                new TeacherResponse
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

                    PhoneNumber = x.User.PhoneNumber,

                    IsActive = x.User.IsActive
                })
            .FirstOrDefaultAsync();
    }

    public async Task<TeacherResponse> UpdateAsync(Guid id, UpdateTeacherRequest request)
    {
        var teacher =
            await ApplyScope(_dbContext.Teachers)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x =>
                    x.Id == id);

        if (teacher is null)
        {
            throw new Exception("Teacher not found.");
        }

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

        var emailExists =
            await _dbContext.Users
                .AnyAsync(x =>
                    x.Id != teacher.UserId &&
                    x.Email == request.Email);

        if (emailExists)
        {
            throw new Exception("Email already exists.");
        }

        var employeeCodeExists =
            await _dbContext.Teachers
                .AnyAsync(x =>
                    x.Id != id &&
                    x.EmployeeCode == request.EmployeeCode);

        if (employeeCodeExists)
        {
            throw new Exception("Employee code already exists.");
        }

        teacher.DepartmentId = request.DepartmentId;

        teacher.EmployeeCode = request.EmployeeCode;

        teacher.Designation = request.Designation;

        teacher.User.FirstName = request.FirstName;

        teacher.User.LastName = request.LastName;

        teacher.User.Email = request.Email;

        teacher.User.PhoneNumber = request.PhoneNumber;

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id)
               ?? throw new Exception();
    }

    public async Task ActivateAsync(Guid id)
    {
        var teacher =
            await ApplyScope(_dbContext.Teachers)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x =>
                    x.Id == id);

        if (teacher is null)
        {
            throw new Exception("Teacher not found.");
        }

        teacher.IsActive = true;

        teacher.User.IsActive = true;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeactivateAsync(Guid id)
    {
        var teacher =
            await ApplyScope(_dbContext.Teachers)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x =>
                    x.Id == id);

        if (teacher is null)
        {
            throw new Exception("Teacher not found.");
        }

        teacher.IsActive = false;

        teacher.User.IsActive = false;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<LookupResponse>> GetLookupAsync()
    {
        return await ApplyScope(_dbContext.Teachers)
            .Include(x => x.User)
            .Where(x => x.User.IsActive)
            .OrderBy(x => x.User.FirstName)
            .Select(x =>
                new LookupResponse
                {
                    Id = x.Id,

                    Name =
                        x.User.FirstName +
                        " " +
                        x.User.LastName
                })
            .ToListAsync();
    }

    private IQueryable<Teacher> ApplyScope(
        IQueryable<Teacher> query)
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
}