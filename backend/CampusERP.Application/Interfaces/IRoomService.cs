using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface IRoomService
{
    Task<List<RoomResponse>> GetAllAsync();

    Task<RoomResponse?> GetByIdAsync(Guid id);

    Task<RoomResponse> CreateAsync(CreateRoomRequest request);

    Task<RoomResponse> UpdateAsync(Guid id, UpdateRoomRequest request);

    Task ActivateAsync(Guid id);

    Task DeactivateAsync(Guid id);

    Task<List<LookupResponse>> GetLookupAsync();
}