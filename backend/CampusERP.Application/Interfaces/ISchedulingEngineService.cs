using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;

namespace CampusERP.Application.Interfaces;

public interface ISchedulingEngineService
{
    Task<ScheduleValidationResponse> ValidateCalendarEventAsync(ScheduleValidationRequest request);

    Task<ScheduleValidationResponse> ValidateTimetableAsync(ScheduleValidationRequest request);

    Task<List<TimetableTemplate>> GetAffectedTimetableLecturesAsync(ScheduleValidationRequest request);

    Task GenerateLectureOverridesAsync(Guid calendarEventId);

    Task RemoveLectureOverridesAsync(Guid calendarEventId);

    Task<bool> IsTeacherAvailableAsync(Guid teacherId, DateOnly date, TimeOnly startTime, TimeOnly endTime);

    Task<bool> IsRoomAvailableAsync(Guid roomId, DateOnly date, TimeOnly startTime, TimeOnly endTime);

    Task<bool> IsSectionAvailableAsync(Guid sectionId, DateOnly date, TimeOnly startTime, TimeOnly endTime);
}