using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface ICalendarEventService
{
    Task<List<CalendarEventResponse>> GetAllAsync();

    Task<CalendarEventResponse?> GetByIdAsync(Guid id);

    Task<CalendarEventResponse> CreateAsync(CreateCalendarEventRequest request);

    Task<CalendarEventResponse> UpdateAsync(Guid id, UpdateCalendarEventRequest request);

    Task ActivateAsync(Guid id);

    Task DeactivateAsync(Guid id);
}