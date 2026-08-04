using CampusERP.Application.Interfaces;
using CampusERP.Shared.Enums;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class SubjectService : ISubjectService
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IDataAccessScope _scope;

    public SubjectService(ApplicationDbContext dbContext, IDataAccessScope scope)
    {
        _dbContext = dbContext;

        _scope = scope;
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

            SubjectType = (SubjectType)request.SubjectType,

            IsActive = true
        };

        _dbContext.Subjects.Add(subject);

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(subject.Id) ?? throw new Exception();
    }

    public async Task<List<SubjectResponse>> GetAllAsync()
    {
        return await ApplyScope(_dbContext.Subjects)
            .Select(x =>
                new SubjectResponse
                {
                    Id = x.Id,

                    InstitutionId = x.InstitutionId,

                    CampusId = x.CampusId,

                    CampusName = x.Campus.Name,

                    Code = x.Code,

                    Name = x.Name,

                    Credits = x.Credits,

                    SubjectType = (SubjectType)x.SubjectType,

                    IsActive = x.IsActive
                })
            .ToListAsync();
    }

    public async Task<SubjectResponse?> GetByIdAsync(Guid id)
    {
        return await ApplyScope(_dbContext.Subjects)
            .Where(x => x.Id == id)
            .Select(x =>
                new SubjectResponse
                {
                    Id = x.Id,

                    InstitutionId = x.InstitutionId,

                    CampusId = x.CampusId,

                    CampusName = x.Campus.Name,

                    Code = x.Code,

                    Name = x.Name,

                    Credits = x.Credits,

                    SubjectType = (SubjectType)x.SubjectType,

                    IsActive = x.IsActive
                })
            .FirstOrDefaultAsync();
    }

    public async Task<SubjectResponse> UpdateAsync(Guid id, UpdateSubjectRequest request)
    {
        var subject = await ApplyScope(_dbContext.Subjects).FirstOrDefaultAsync(x => x.Id == id);

        if (subject is null)
        {
            throw new Exception("Subject not found.");
        }

        var codeExists =
            await _dbContext.Subjects
                .AnyAsync(x =>
                    x.Id != id &&
                    x.InstitutionId == request.InstitutionId &&
                    x.CampusId == request.CampusId &&
                    x.Code == request.Code);

        if (codeExists)
        {
            throw new Exception("Subject code already exists.");
        }

        subject.Code = request.Code;

        subject.Name = request.Name;

        subject.Credits = request.Credits;

        subject.SubjectType =
            (SubjectType)request.SubjectType;

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id)
            ?? throw new Exception();
    }

    public async Task ActivateAsync(Guid id)
    {
        var subject = await ApplyScope(_dbContext.Subjects).FirstOrDefaultAsync(x => x.Id == id);

        if (subject is null)
        {
            throw new Exception("Subject not found.");
        }

        subject.IsActive = true;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeactivateAsync(Guid id)
    {
        var subject = await ApplyScope(_dbContext.Subjects).FirstOrDefaultAsync(x => x.Id == id);

        if (subject is null)
        {
            throw new Exception("Subject not found.");
        }

        subject.IsActive = false;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<LookupResponse>> GetLookupAsync()
    {
        return await ApplyScope(_dbContext.Subjects)
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

    private IQueryable<Subject> ApplyScope(IQueryable<Subject> query)
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
}