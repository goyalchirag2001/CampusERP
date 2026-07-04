using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class DepartmentService: IDepartmentService
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IDataAccessScope _scope;

    public DepartmentService(ApplicationDbContext dbContext, IDataAccessScope scope)
    {
        _dbContext = dbContext;

        _scope = scope;
    }

    public async Task<DepartmentResponse> CreateAsync(CreateDepartmentRequest request)
    {
        ValidateCreateScope(request.InstitutionId, request.CampusId);

        var campusExists = await _dbContext.Campuses.AnyAsync(x => x.Id == request.CampusId && x.InstitutionId == request.InstitutionId);

        if (!campusExists)
        {
            throw new Exception("Campus not found.");
        }

        var departmentCodeExists = await _dbContext.Departments.AnyAsync(x => x.InstitutionId == request.InstitutionId && x.CampusId == request.CampusId && x.Code == request.Code);

        if (departmentCodeExists)
        {
            throw new Exception("Department code already exists.");
        }

        var department =
            new Department
            {
                Id = Guid.NewGuid(),

                InstitutionId = request.InstitutionId,

                CampusId = request.CampusId,

                Name = request.Name,

                Code = request.Code,

                IsActive = true
            };

        _dbContext.Departments.Add(department);

        await _dbContext.SaveChangesAsync();

        return new DepartmentResponse
        {
            Id = department.Id,

            InstitutionId = department.InstitutionId,

            CampusId = department.CampusId,

            Name = department.Name,

            Code = department.Code,

            IsActive = department.IsActive,
        };
    }

    public async Task<List<DepartmentResponse>> GetAllAsync()
    {
        return await ApplyDepartmentScope(_dbContext.Departments)
            .Select(x =>
                new DepartmentResponse
                {
                    Id = x.Id,

                    InstitutionId = x.InstitutionId,

                    CampusId = x.CampusId,

                    Name = x.Name,

                    Code = x.Code,

                    IsActive = x.IsActive,
                })
            .ToListAsync();
    }

    public async Task<DepartmentResponse?> GetByIdAsync(Guid id)
    {
        return await ApplyDepartmentScope(_dbContext.Departments
            .Where(x => x.Id == id))
            .Select(x =>
                new DepartmentResponse
                {
                    Id = x.Id,

                    InstitutionId = x.InstitutionId,

                    CampusId = x.CampusId,

                    Name = x.Name,

                    Code = x.Code,

                    IsActive = x.IsActive,
                })
            .FirstOrDefaultAsync();
    }

    public async Task<DepartmentResponse> UpdateAsync(Guid id, UpdateDepartmentRequest request)
    {
        var department = await ApplyDepartmentScope(_dbContext.Departments.Where(x => x.Id == id)).FirstOrDefaultAsync();

        if (department is null)
        {
            throw new Exception("Department not found.");
        }

        ValidateCreateScope(request.InstitutionId, request.CampusId);

        var campusExists =
            await _dbContext.Campuses
                .AnyAsync(x =>
                    x.Id == request.CampusId &&
                    x.InstitutionId == request.InstitutionId);

        if (!campusExists)
        {
            throw new Exception("Campus not found.");
        }

        var codeExists =
            await _dbContext.Departments
                .AnyAsync(x =>
                    x.Id != id &&
                    x.InstitutionId == request.InstitutionId &&
                    x.CampusId == request.CampusId &&
                    x.Code == request.Code);

        if (codeExists)
        {
            throw new Exception("Department code already exists.");
        }

        department.Name = request.Name;

        department.Code = request.Code;

        department.CampusId = request.CampusId;

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id) ?? throw new Exception();
    }

    public async Task ActivateAsync(Guid id)
    {
        var department = await ApplyDepartmentScope(_dbContext.Departments.Where(x => x.Id == id)).FirstOrDefaultAsync();

        if (department is null)
        {
            throw new Exception("Department not found.");
        }

        department.IsActive = true;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeactivateAsync(Guid id)
    {
        var department = await ApplyDepartmentScope(_dbContext.Departments.Where(x => x.Id == id)).FirstOrDefaultAsync();

        if (department is null)
        {
            throw new Exception("Department not found.");
        }

        department.IsActive = false;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<LookupResponse>> GetLookupAsync()
    {
        return await ApplyDepartmentScope(_dbContext.Departments
            .Where(x => x.IsActive))
            .OrderBy(x => x.Name)
            .Select(x =>
                new LookupResponse
                {
                    Id = x.Id,

                    Name = x.Name
                })
            .ToListAsync();
    }

    private IQueryable<Department> ApplyDepartmentScope(IQueryable<Department> query)
    {
        if (_scope.IsSuperAdmin() || _scope.IsPlatformAdmin())
        {
            return query;
        }

        if (_scope.IsInstitutionAdmin())
        {
            query = query.Where(x => x.InstitutionId == _scope.InstitutionId());
        }

        if (_scope.IsCampusAdmin())
        {
            query = query.Where(x => x.CampusId == _scope.CampusId());
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

    public async Task<List<DepartmentLookupResponse>> GetLookupWithCampusAsync()
    {
        return await ApplyDepartmentScope(
                _dbContext.Departments.Where(x => x.IsActive))
            .OrderBy(x => x.Name)
            .Select(x => new DepartmentLookupResponse
            {
                Id = x.Id,

                CampusId = x.CampusId,

                Name = x.Name
            })
            .ToListAsync();
    }
}