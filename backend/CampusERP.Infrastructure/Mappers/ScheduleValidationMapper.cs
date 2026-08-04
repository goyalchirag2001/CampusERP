using CampusERP.Contracts.Requests;
using CampusERP.Domain.Entities;

namespace CampusERP.Infrastructure.Mappers;

public static class ScheduleValidationMapper
{
    public static ScheduleValidationRequest FromCalendarEvent(CalendarEvent calendarEvent)
    {
        return new ScheduleValidationRequest
        {
            CalendarEventId = calendarEvent.Id,

            AcademicSessionId = calendarEvent.AcademicSessionId,

            CampusId = calendarEvent.CampusId,

            DepartmentId = calendarEvent.DepartmentId,

            CourseId = calendarEvent.CourseId,

            SemesterId = calendarEvent.SemesterId,

            SectionId = calendarEvent.SectionId,

            TeacherId = calendarEvent.TeacherId,

            RoomId = calendarEvent.RoomId,

            Title = calendarEvent.Title,

            EventType = calendarEvent.EventType,

            StartDate = calendarEvent.StartDate,

            EndDate = calendarEvent.EndDate,

            StartTime = calendarEvent.StartTime,

            EndTime = calendarEvent.EndTime,

            IsFullDay = calendarEvent.IsFullDay,

            Priority = calendarEvent.Priority,

            AffectsTimetable = calendarEvent.AffectsTimetable
        };
    }
}