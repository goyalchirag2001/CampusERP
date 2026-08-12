using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface ITimetableCalendarService
{
    Task<List<TimetableCalendarEventResponse>> GetTeacherCalendarAsync(TimetableCalendarRequest request);

    Task<List<TimetableCalendarEventResponse>> GetStudentCalendarAsync(TimetableCalendarRequest request);
}