using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Enums;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Domain.Enums;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class SubjectService : ISubjectService
{
    private readonly ApplicationDbContext _dbContext;

    public SubjectService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SubjectResponse> CreateAsync(CreateSubjectRequest request)
    {
        var campusExists =
            await _dbContext.Campuses
                .AnyAsync(x =>
                    x.Id == request.CampusId &&
                    x.InstitutionId == request.InstitutionId);

        if (!campusExists)
        {
            throw new Exception("Campus not found.");
        }

        var subjectCodeExists =
            await _dbContext.Subjects
                .AnyAsync(x =>
                    x.InstitutionId == request.InstitutionId &&
                    x.CampusId == request.CampusId &&
                    x.Code == request.Code);

        if (subjectCodeExists)
        {
            throw new Exception("Subject code already exists.");
        }

        var subject = new Subject
        {
            Id = Guid.NewGuid(),

            InstitutionId = request.InstitutionId,

            CampusId = request.CampusId,

            Code = request.Code,

            Name = request.Name,

            Credits = request.Credits,

            SubjectType = (SubjectType)request.SubjectType
        };

        _dbContext.Subjects.Add(subject);

        await _dbContext.SaveChangesAsync();

        return new SubjectResponse
        {
            Id = subject.Id,

            InstitutionId = subject.InstitutionId,

            CampusId = subject.CampusId,

            Code = subject.Code,

            Name = subject.Name,

            Credits = subject.Credits,

            SubjectType = (SubjectTypeDto)subject.SubjectType
        };
    }

    public async Task<List<SubjectResponse>> GetAllAsync()
    {
        return await _dbContext.Subjects
            .Select(x =>
                new SubjectResponse
                {
                    Id = x.Id,

                    InstitutionId = x.InstitutionId,

                    CampusId = x.CampusId,

                    Code = x.Code,

                    Name = x.Name,

                    Credits = x.Credits,

                    SubjectType = (SubjectTypeDto)x.SubjectType
                })
            .ToListAsync();
    }

    public async Task<SubjectResponse?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Subjects
            .Where(x => x.Id == id)
            .Select(x =>
                new SubjectResponse
                {
                    Id = x.Id,

                    InstitutionId = x.InstitutionId,

                    CampusId = x.CampusId,

                    Code = x.Code,

                    Name = x.Name,

                    Credits = x.Credits,

                    SubjectType = (SubjectTypeDto)x.SubjectType
                })
            .FirstOrDefaultAsync();
    }
}