using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class AcademicSessionService : IAcademicSessionService
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IDataAccessScope _scope;

    public AcademicSessionService(ApplicationDbContext dbContext, IDataAccessScope scope)
    {
        _dbContext = dbContext;

        _scope = scope;
    }

    public async Task<AcademicSessionResponse> CreateAsync(CreateAcademicSessionRequest request)
    {
        ValidateCreateScope(request.InstitutionId, request.CampusId);

        var campusExists = await _dbContext.Campuses.AnyAsync(x => x.Id == request.CampusId && x.InstitutionId == request.InstitutionId);

        if (!campusExists)
        {
            throw new Exception("Campus not found.");
        }

        await ValidateDuplicateName(request.Name, request.InstitutionId, request.CampusId);

        ValidateDates(request.StartDate, request.EndDate);

        var session = new AcademicSession
        {
            Id = Guid.NewGuid(),

            InstitutionId = request.InstitutionId,

            CampusId = request.CampusId,

            Name = request.Name.Trim(),

            StartDate = request.StartDate,

            EndDate = request.EndDate,

            IsCurrent = request.IsCurrent,

            IsActive = true
        };

        _dbContext.AcademicSessions.Add(session);

        if (request.IsCurrent)
        {
            await SetCurrentSessionAsync(session.Id, session.InstitutionId, session.CampusId);
        }

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(session.Id) ?? throw new Exception();
    }

    public async Task<List<AcademicSessionResponse>> GetAllAsync()
    {
        return await ApplyScope(_dbContext.AcademicSessions
                    .Include(x => x.Campus))
            .OrderByDescending(x => x.IsCurrent)
            .ThenByDescending(x => x.StartDate)
            .Select(x => new AcademicSessionResponse
            {
                Id = x.Id,

                InstitutionId = x.InstitutionId,

                CampusId = x.CampusId,

                CampusName = x.Campus.Name,

                Name = x.Name,

                StartDate = x.StartDate,

                EndDate = x.EndDate,

                IsCurrent = x.IsCurrent,

                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public async Task<AcademicSessionResponse?> GetByIdAsync(Guid id)
    {
        return await ApplyScope(_dbContext.AcademicSessions
                    .Include(x => x.Campus)
                    .Where(x => x.Id == id))
            .Select(x => new AcademicSessionResponse
            {
                Id = x.Id,

                InstitutionId = x.InstitutionId,

                CampusId = x.CampusId,

                CampusName = x.Campus.Name,

                Name = x.Name,

                StartDate = x.StartDate,

                EndDate = x.EndDate,

                IsCurrent = x.IsCurrent,

                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<AcademicSessionLookup>> GetLookupAsync()
    {
        return await ApplyScope(_dbContext.AcademicSessions
                    .Where(x => x.IsActive))
            .OrderByDescending(x => x.IsCurrent)
            .ThenByDescending(x => x.StartDate)
            .Select(x => new AcademicSessionLookup
            {
                Id = x.Id,

                Name = x.Name,

                IsCurrent = x.IsCurrent
            })
            .ToListAsync();
    }

    public async Task<List<AcademicSessionLookup>> GetLookupByCampusAsync(Guid campusId)
    {
        return await ApplyScope(_dbContext.AcademicSessions
            .Where(x =>
                x.IsActive &&
                x.CampusId == campusId))
            .OrderByDescending(x => x.IsCurrent)
            .ThenByDescending(x => x.StartDate)
            .Select(x => new AcademicSessionLookup
            {
                Id = x.Id,
                Name = x.Name,
                IsCurrent = x.IsCurrent
            })
            .ToListAsync();
    }

    public async Task<AcademicSessionLookup?> GetCurrentAsync()
    {
        return await ApplyScope(_dbContext.AcademicSessions
                    .Where(x =>
                        x.IsActive &&
                        x.IsCurrent))
            .Select(x => new AcademicSessionLookup
            {
                Id = x.Id,

                Name = x.Name,

                IsCurrent = true
            })
            .FirstOrDefaultAsync();
    }

    public async Task<AcademicSessionResponse> UpdateAsync(Guid id, UpdateAcademicSessionRequest request)
    {
        var session = await ApplyScope(_dbContext.AcademicSessions.Where(x => x.Id == id)).FirstOrDefaultAsync();

        if (session is null)
        {
            throw new Exception("Academic session not found.");
        }

        ValidateDates(request.StartDate, request.EndDate);

        await ValidateDuplicateName(request.Name, session.InstitutionId, session.CampusId, id);

        session.Name = request.Name.Trim();

        session.StartDate = request.StartDate;

        session.EndDate = request.EndDate;

        session.IsCurrent = request.IsCurrent;

        if (request.IsCurrent)
        {
            await SetCurrentSessionAsync(session.Id, session.InstitutionId, session.CampusId);
        }

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id)
               ?? throw new Exception();
    }

    public async Task ActivateAsync(Guid id)
    {
        var session = await ApplyScope(_dbContext.AcademicSessions.Where(x => x.Id == id)).FirstOrDefaultAsync();

        if (session is null)
        {
            throw new Exception("Academic session not found.");
        }

        session.IsActive = true;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeactivateAsync(Guid id)
    {
        var session = await ApplyScope(_dbContext.AcademicSessions.Where(x => x.Id == id)).FirstOrDefaultAsync();

        if (session is null)
        {
            throw new Exception("Academic session not found.");
        }

        if (session.IsCurrent)
        {
            throw new Exception("Current academic session cannot be deactivated.");
        }

        session.IsActive = false;

        await _dbContext.SaveChangesAsync();
    }

    public async Task SetCurrentAsync(Guid id)
    {
        var session =
            await ApplyScope(_dbContext.AcademicSessions.Where(x => x.Id == id)).FirstOrDefaultAsync();

        if (session is null)
        {
            throw new Exception("Academic session not found.");
        }

        if (!session.IsActive)
        {
            throw new Exception(
                "Inactive academic session cannot be set as current.");
        }

        await SetCurrentSessionAsync(
            session.Id,
            session.InstitutionId,
            session.CampusId);

        await _dbContext.SaveChangesAsync();
    }

    private IQueryable<AcademicSession> ApplyScope(IQueryable<AcademicSession> query)
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

    private static void ValidateDates(DateOnly startDate, DateOnly endDate)
    {
        if (startDate >= endDate)
        {
            throw new Exception("End date must be greater than start date.");
        }
    }

    private async Task ValidateDuplicateName(string name, Guid institutionId, Guid campusId, Guid? excludeId = null)
    {
        name = name.Trim();

        var exists =
            await _dbContext.AcademicSessions
                .AnyAsync(x =>

                    x.Id != excludeId &&

                    x.InstitutionId == institutionId &&

                    x.CampusId == campusId &&

                    x.Name.ToLower() == name.ToLower());

        if (exists)
        {
            throw new Exception("Academic session already exists.");
        }
    }

    private async Task SetCurrentSessionAsync(Guid sessionId, Guid institutionId, Guid campusId)
    {
        var sessions = await _dbContext.AcademicSessions
                .Where(x =>

                    x.InstitutionId == institutionId &&

                    x.CampusId == campusId)

                .ToListAsync();

        foreach (var session in sessions)
        {
            session.IsCurrent = session.Id == sessionId;
        }
    }
}