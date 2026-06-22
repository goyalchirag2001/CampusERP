using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using CampusERP.Infrastructure.Identity;
using CampusERP.Shared.Constants;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class CampusService : ICampusService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IDataAccessScope _scope;

    public CampusService(ApplicationDbContext dbContext, IDataAccessScope scope)
    {
        _dbContext = dbContext;
        _scope = scope;
    }

    public async Task<CampusResponse> CreateAsync (CreateCampusRequest request)
    {
        Guid institutionId;

        if (_scope.IsSuperAdmin() || _scope.IsPlatformAdmin())
        {
            institutionId = request.InstitutionId;
        }
        else
        {
            institutionId = _scope.InstitutionId();
        }

        var institutionExists = await _dbContext.Institutions.AnyAsync(x => x.Id == institutionId);

        if (!institutionExists)
        {
            throw new Exception("Institution not found.");
        }

        var campusCodeExists = await _dbContext.Campuses
                                .AnyAsync(x =>
                                    x.InstitutionId == institutionId &&
                                    x.Code == request.Code);

        if (campusCodeExists)
        {
            throw new Exception("Campus code already exists.");
        }

        var campus = new Campus
        {
            Id = Guid.NewGuid(),

            InstitutionId = institutionId,

            Name = request.Name,

            Code = request.Code,

            Email = request.Email,

            Phone = request.Phone,

            Address = request.Address,

            CampusHeadName = request.CampusHeadName,

            IsActive = true
        };

        _dbContext.Campuses.Add(campus);

        await _dbContext.SaveChangesAsync();

        return new CampusResponse
        {
            Id = campus.Id,

            InstitutionId = campus.InstitutionId,

            Name = campus.Name,

            Code = campus.Code,

            Email = campus.Email,

            Phone = campus.Phone,

            Address = campus.Address,

            CampusHeadName = campus.CampusHeadName,

            IsActive = campus.IsActive
        };
    }

    public async Task<List<CampusResponse>> GetAllAsync()
    {
        var query = ApplyScope(_dbContext.Campuses.Include(x => x.Institution));

        return await query
            .Select(x => new CampusResponse
            {
                Id = x.Id,

                InstitutionId = x.InstitutionId,

                InstitutionName = x.Institution.Name,

                Name = x.Name,

                Code = x.Code,

                Email = x.Email,

                Phone = x.Phone,

                Address = x.Address,

                CampusHeadName = x.CampusHeadName,

                DepartmentCount = _dbContext.Departments.Count(d => d.CampusId == x.Id),

                TeacherCount = _dbContext.Teachers.Count(t => t.CampusId == x.Id),

                StudentCount = _dbContext.Students.Count(s => s.CampusId == x.Id),

                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public async Task<CampusResponse?> GetByIdAsync(Guid id)
    {
        return await ApplyScope(_dbContext.Campuses)
            .Where(x => x.Id == id)
            .Select(x => new CampusResponse
            {
                Id = x.Id,

                InstitutionId = x.InstitutionId,

                InstitutionName = x.Institution.Name,

                Name = x.Name,

                Code = x.Code,

                Email = x.Email,

                Phone = x.Phone,

                Address = x.Address,

                CampusHeadName = x.CampusHeadName,

                DepartmentCount =
                    _dbContext.Departments.Count(d =>
                        d.CampusId == x.Id),

                TeacherCount =
                    _dbContext.Teachers.Count(t =>
                        t.CampusId == x.Id),

                StudentCount =
                    _dbContext.Students.Count(s =>
                        s.CampusId == x.Id),

                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CampusResponse> UpdateAsync(Guid id, UpdateCampusRequest request)
    {
        var campus = await ApplyScope(_dbContext.Campuses).FirstOrDefaultAsync(x => x.Id == id);

        if (campus is null)
        {
            throw new Exception("Campus not found.");
        }

        var codeExists = await _dbContext.Campuses.AnyAsync(x =>
            x.Id != id &&
            x.InstitutionId == campus.InstitutionId &&
            x.Code == request.Code);

        if (codeExists)
        {
            throw new Exception("Campus code already exists.");
        }

        campus.Name = request.Name;

        campus.Code = request.Code;

        campus.Email = request.Email;

        campus.Phone = request.Phone;

        campus.Address = request.Address;

        campus.CampusHeadName = request.CampusHeadName;

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id) ?? throw new Exception();
    }

    public async Task ActivateAsync(Guid id)
    {
        var campus = await ApplyScope(_dbContext.Campuses).FirstOrDefaultAsync(x => x.Id == id);

        if (campus is null)
        {
            throw new Exception("Campus not found.");
        }

        campus.IsActive = true;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeactivateAsync(Guid id)
    {
        var campus = await ApplyScope(_dbContext.Campuses).FirstOrDefaultAsync(x => x.Id == id);

        if (campus is null)
        {
            throw new Exception("Campus not found.");
        }

        campus.IsActive = false;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<LookupResponse>> GetLookupAsync()
    {
        return await ApplyScope(_dbContext.Campuses)
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x =>
                new LookupResponse
                {
                    Id = x.Id,

                    Name = x.Name
                })
            .ToListAsync();
    }

    private IQueryable<Campus> ApplyScope(IQueryable<Campus> query)
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
                    x.Id ==
                    _scope.CampusId());
        }

        return query;
    }
}