using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class CampusService : ICampusService
{
    private readonly ApplicationDbContext _dbContext;

    public CampusService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CampusResponse> CreateAsync(CreateCampusRequest request)
    {
        var institutionExists = await _dbContext.Institutions
                                .AnyAsync(x =>
                                    x.Id == request.InstitutionId);

        if (!institutionExists)
        {
            throw new Exception("Institution not found.");
        }

        var campusCodeExists = await _dbContext.Campuses
                                .AnyAsync(x =>
                                    x.InstitutionId ==
                                    request.InstitutionId &&
                                    x.Code == request.Code);

        if (campusCodeExists)
        {
            throw new Exception("Campus code already exists.");
        }

        var campus = new Campus
        {
            Id = Guid.NewGuid(),

            InstitutionId = request.InstitutionId,

            Name = request.Name,

            Code = request.Code,

            Address = request.Address,

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

            Address = campus.Address,

            IsActive = campus.IsActive
        };
    }

    public async Task<List<CampusResponse>>
        GetAllAsync()
    {
        return await _dbContext.Campuses
            .Select(x => new CampusResponse
            {
                Id = x.Id,

                InstitutionId = x.InstitutionId,

                Name = x.Name,

                Code = x.Code,

                Address = x.Address,

                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public async Task<CampusResponse?>
        GetByIdAsync(Guid id)
    {
        return await _dbContext.Campuses
            .Where(x => x.Id == id)
            .Select(x => new CampusResponse
            {
                Id = x.Id,

                InstitutionId = x.InstitutionId,

                Name = x.Name,

                Code = x.Code,

                Address = x.Address,

                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync();
    }
}