using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class DepartmentService
    : IDepartmentService
{
    private readonly ApplicationDbContext _dbContext;

    public DepartmentService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DepartmentResponse>
        CreateAsync(CreateDepartmentRequest request)
    {
        var campusExists =
            await _dbContext.Campuses
                .AnyAsync(x =>
                    x.Id == request.CampusId &&
                    x.InstitutionId ==
                    request.InstitutionId);

        if (!campusExists)
        {
            throw new Exception("Campus not found.");
        }

        var departmentCodeExists =
            await _dbContext.Departments
                .AnyAsync(x =>
                    x.InstitutionId ==
                    request.InstitutionId &&
                    x.CampusId ==
                    request.CampusId &&
                    x.Code ==
                    request.Code);

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

                Code = request.Code
            };

        _dbContext.Departments.Add(department);

        await _dbContext.SaveChangesAsync();

        return new DepartmentResponse
        {
            Id = department.Id,

            InstitutionId = department.InstitutionId,

            CampusId = department.CampusId,

            Name = department.Name,

            Code = department.Code
        };
    }

    public async Task<List<DepartmentResponse>>
        GetAllAsync()
    {
        return await _dbContext.Departments
            .Select(x =>
                new DepartmentResponse
                {
                    Id = x.Id,

                    InstitutionId = x.InstitutionId,

                    CampusId = x.CampusId,

                    Name = x.Name,

                    Code = x.Code
                })
            .ToListAsync();
    }

    public async Task<DepartmentResponse?>
        GetByIdAsync(Guid id)
    {
        return await _dbContext.Departments
            .Where(x => x.Id == id)
            .Select(x =>
                new DepartmentResponse
                {
                    Id = x.Id,

                    InstitutionId = x.InstitutionId,

                    CampusId = x.CampusId,

                    Name = x.Name,

                    Code = x.Code
                })
            .FirstOrDefaultAsync();
    }
}