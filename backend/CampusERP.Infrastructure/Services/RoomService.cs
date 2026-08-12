using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Shared.Enums;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class RoomService : IRoomService
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IDataAccessScope _scope;

    public RoomService(ApplicationDbContext dbContext, IDataAccessScope scope)
    {
        _dbContext = dbContext;
        _scope = scope;
    }

    public async Task<List<RoomResponse>> GetAllAsync()
    {
        return await ApplyRoomScope(_dbContext.Rooms)
            .Include(x => x.Campus)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Building)
            .ThenBy(x => x.Floor)
            .ThenBy(x => x.RoomNumber)
            .Select(MapToResponse())
            .ToListAsync();
    }

    public async Task<RoomResponse?> GetByIdAsync(Guid id)
    {
        return await ApplyRoomScope(_dbContext.Rooms)
            .Include(x => x.Campus)
            .Where(x => x.Id == id)
            .Select(MapToResponse())
            .FirstOrDefaultAsync();
    }

    public async Task<RoomResponse> CreateAsync(CreateRoomRequest request)
    {
        var roomNumber = request.RoomNumber.Trim().ToUpperInvariant();

        var building = request.Building.Trim();

        var exists = await ApplyRoomScope(_dbContext.Rooms)
            .AnyAsync(x =>
                x.Building == building &&
                x.RoomNumber == roomNumber);

        if (exists)
        {
            throw new Exception("Room already exists.");
        }

        if (!Enum.TryParse<RoomType>(request.RoomType, true, out var roomType))
        {
            throw new Exception("Invalid room type.");
        }

        var institutionId = _scope.InstitutionId();

        if (institutionId == Guid.Empty)
        {
            throw new Exception("Institution scope is not available.");
        }

        var campusId = request.CampusId;

        if (campusId == Guid.Empty)
        {
            throw new Exception("Campus is required.");
        }

        if (_scope.IsCampusAdmin())
        {
            var currentCampusId = _scope.CampusId();

            if (currentCampusId == Guid.Empty)
            {
                throw new Exception("Campus scope is not available.");
            }

            if (campusId != currentCampusId)
            {
                throw new UnauthorizedAccessException("You cannot create a room outside your assigned campus.");
            }
        }

        var campusBelongsToInstitution = await _dbContext.Campuses.AnyAsync(x =>
                                            x.Id == campusId &&
                                            x.InstitutionId == institutionId);

        if (!campusBelongsToInstitution)
        {
            throw new Exception("Selected campus does not belong to your institution.");
        }

        var room = new Room
        {
            Id = Guid.NewGuid(),

            InstitutionId = institutionId,

            CampusId = campusId,

            Building = building,

            Floor = string.IsNullOrWhiteSpace(request.Floor)
                ? null
                : request.Floor.Trim(),

            RoomNumber = roomNumber,

            RoomName = request.RoomName.Trim(),

            RoomType = roomType,

            Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim(),

            LocationCode = string.IsNullOrWhiteSpace(request.LocationCode)
                ? null
                : request.LocationCode.Trim(),

            Capacity = request.Capacity,

            DisplayOrder = request.DisplayOrder,

            HasProjector = request.HasProjector,

            HasSmartBoard = request.HasSmartBoard,

            HasAirConditioning = request.HasAirConditioning,

            HasComputers = request.HasComputers,

            HasInternet = request.HasInternet,

            IsAccessible = request.IsAccessible,

            IsActive = true
        };

        _dbContext.Rooms.Add(room);

        await _dbContext.SaveChangesAsync();

        var response = await _dbContext.Rooms
            .AsNoTracking()
            .Include(x => x.Campus)
            .Where(x => x.Id == room.Id)
            .Select(MapToResponse())
            .FirstOrDefaultAsync();

        return response ?? throw new Exception("Room was created but could not be loaded.");
    }
    public async Task<RoomResponse> UpdateAsync(Guid id, UpdateRoomRequest request)
    {
        var room = await ApplyRoomScope(_dbContext.Rooms)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (room is null)
        {
            throw new Exception("Room not found.");
        }

        var roomNumber = request.RoomNumber.Trim().ToUpper();

        var building = request.Building.Trim();

        var duplicate = await ApplyRoomScope(_dbContext.Rooms)
            .AnyAsync(x =>
                x.Id != id &&
                x.Building == building &&
                x.RoomNumber == roomNumber);

        if (duplicate)
        {
            throw new Exception("Room already exists.");
        }

        if (!Enum.TryParse<RoomType>(request.RoomType, true, out var roomType))
        {
            throw new Exception("Invalid room type.");
        }

        room.Building = building;

        room.Floor = string.IsNullOrWhiteSpace(request.Floor)
            ? null
            : request.Floor.Trim();

        room.RoomNumber = roomNumber;

        room.RoomName = request.RoomName.Trim();

        room.RoomType = roomType;

        room.Description = request.Description?.Trim();

        room.LocationCode = request.LocationCode?.Trim();

        room.Capacity = request.Capacity;

        room.DisplayOrder = request.DisplayOrder;

        room.HasProjector = request.HasProjector;

        room.HasSmartBoard = request.HasSmartBoard;

        room.HasAirConditioning = request.HasAirConditioning;

        room.HasComputers = request.HasComputers;

        room.HasInternet = request.HasInternet;

        room.IsAccessible = request.IsAccessible;

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id)
            ?? throw new Exception("Room not found.");
    }

    public async Task ActivateAsync(Guid id)
    {
        var room = await ApplyRoomScope(_dbContext.Rooms)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (room is null)
        {
            throw new Exception("Room not found.");
        }

        if (room.IsActive)
        {
            return;
        }

        room.IsActive = true;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeactivateAsync(Guid id)
    {
        var room = await ApplyRoomScope(_dbContext.Rooms)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (room is null)
        {
            throw new Exception("Room not found.");
        }

        if (!room.IsActive)
        {
            return;
        }

        room.IsActive = false;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<LookupResponse>> GetLookupAsync()
    {
        return await ApplyRoomScope(_dbContext.Rooms)
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Building)
            .ThenBy(x => x.RoomNumber)
            .Select(x => new LookupResponse
            {
                Id = x.Id,

                Name = $"{x.Building} - {x.RoomNumber} ({x.RoomName})"
            })
            .ToListAsync();
    }

    private IQueryable<Room> ApplyRoomScope(IQueryable<Room> query)
    {
        if (_scope.IsSuperAdmin() || _scope.IsPlatformAdmin())
        {
            return query;
        }

        if (_scope.IsInstitutionAdmin())
        {
            query = query.Where(x =>
                x.InstitutionId == _scope.InstitutionId());
        }

        if (_scope.IsCampusAdmin())
        {
            query = query.Where(x =>
                x.CampusId == _scope.CampusId());
        }

        return query;
    }

    private static System.Linq.Expressions.Expression<Func<Room, RoomResponse>> MapToResponse()
    {
        return x => new RoomResponse
        {
            Id = x.Id,

            InstitutionId = x.InstitutionId,

            CampusId = x.CampusId,

            CampusName = x.Campus.Name,

            Building = x.Building,

            Floor = x.Floor ?? string.Empty,

            RoomNumber = x.RoomNumber,

            RoomName = x.RoomName,

            RoomType = x.RoomType.ToString(),

            Description = x.Description,

            LocationCode = x.LocationCode,

            Capacity = x.Capacity,

            DisplayOrder = x.DisplayOrder,

            HasProjector = x.HasProjector,

            HasSmartBoard = x.HasSmartBoard,

            HasAirConditioning = x.HasAirConditioning,

            HasComputers = x.HasComputers,

            HasInternet = x.HasInternet,

            IsAccessible = x.IsAccessible,

            IsActive = x.IsActive
        };
    }
}