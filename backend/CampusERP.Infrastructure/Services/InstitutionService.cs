using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class InstitutionService: IInstitutionService
{
    private readonly ApplicationDbContext _dbContext;

    public InstitutionService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<InstitutionResponse> CreateAsync(CreateInstitutionRequest request)
    {
        var codeExists =
            await _dbContext.Institutions
                .AnyAsync(x =>
                    x.Code == request.Code);

        if (codeExists)
        {
            throw new Exception("Institution code already exists.");
        }

        var institution =
            new Institution
            {
                Id = Guid.NewGuid(),

                Name = request.Name,

                Code = request.Code,

                Email = request.Email,

                Phone = request.Phone,

                Website = request.Website,

                Address = request.Address,

                IsActive = true
            };

        _dbContext.Institutions.Add(institution);

        await _dbContext.SaveChangesAsync();

        return new InstitutionResponse
        {
            Id = institution.Id,

            Name = institution.Name,

            Code = institution.Code,

            Email = institution.Email,

            Phone = institution.Phone,

            Website = institution.Website,

            Address = institution.Address,

            IsActive = institution.IsActive
        };
    }

    public async Task<List<InstitutionResponse>>
        GetAllAsync()
    {
        return await _dbContext.Institutions
            .Select(x =>
                new InstitutionResponse
                {
                    Id = x.Id,

                    Name = x.Name,

                    Code = x.Code,

                    Email = x.Email,

                    Phone = x.Phone,

                    Website = x.Website,

                    Address = x.Address,

                    IsActive = x.IsActive
                })
            .ToListAsync();
    }

    public async Task<InstitutionResponse?>
        GetByIdAsync(Guid id)
    {
        return await _dbContext.Institutions
            .Where(x => x.Id == id)
            .Select(x =>
                new InstitutionResponse
                {
                    Id = x.Id,

                    Name = x.Name,

                    Code = x.Code,

                    Email = x.Email,

                    Phone = x.Phone,

                    Website = x.Website,

                    Address = x.Address,

                    IsActive = x.IsActive
                })
            .FirstOrDefaultAsync();
    }
}