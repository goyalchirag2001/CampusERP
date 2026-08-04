using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using CampusERP.Infrastructure.Mappers;
using CampusERP.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class SchedulingEngineService : ISchedulingEngineService
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IDataAccessScope _scope;

    public SchedulingEngineService(ApplicationDbContext dbContext, IDataAccessScope scope)
    {
        _dbContext = dbContext;
        _scope = scope;
    }

    public async Task<ScheduleValidationResponse> ValidateCalendarEventAsync(ScheduleValidationRequest request)
    {
        var response = CreateValidationResponse(true);

        await ValidateTeacherAsync(request, response);

        await ValidateRoomAsync(request, response);

        await ValidateSectionAsync(request, response);

        await ValidateCalendarConflictsAsync(request, response);

        await ValidateTimetableConflictsAsync(request, response);

        response.IsValid = response.Conflicts.Count == 0;

        return response;
    }

    public async Task<ScheduleValidationResponse> ValidateTimetableAsync(ScheduleValidationRequest request)
    {
        var response = CreateValidationResponse(false);

        await ValidateTeacherAsync(request, response);

        await ValidateRoomAsync(request, response);

        await ValidateSectionAsync(request, response);

        response.IsValid = response.Conflicts.Count == 0;

        return response;
    }

    public async Task<List<TimetableTemplate>> GetAffectedTimetableLecturesAsync(ScheduleValidationRequest request)
    {
        if (!request.AffectsTimetable)
        {
            return new List<TimetableTemplate>();
        }

        var lectures = await GetMatchingTimetableEntriesAsync(request);

        var affectedLectures = new List<TimetableTemplate>();

        foreach (var lecture in lectures)
        {
            if (!IsTimeOverlapping(
                    lecture.StartTime,
                    lecture.EndTime,
                    request.StartTime!.Value,
                    request.EndTime!.Value))
            {
                continue;
            }

            bool affectsLecture =
                (request.TeacherId.HasValue &&
                 lecture.TeacherId == request.TeacherId)

                ||

                (request.RoomId.HasValue &&
                 lecture.RoomId == request.RoomId)

                ||

                (request.SectionId.HasValue &&
                 lecture.SectionId == request.SectionId);

            if (!affectsLecture)
            {
                continue;
            }

            affectedLectures.Add(lecture);
        }

        return affectedLectures;
    }

    public async Task GenerateLectureOverridesAsync(Guid calendarEventId)
    {
        var calendarEvent = await GetCalendarEventAsync(calendarEventId);

        if (!calendarEvent.AffectsTimetable)
        {
            return;
        }

        var request =
            ScheduleValidationMapper.FromCalendarEvent(calendarEvent);

        var lectures =
            await GetAffectedTimetableLecturesAsync(request);

        if (lectures.Count == 0)
        {
            return;
        }

        var existingOverrides = await _dbContext.LectureOverrides
            .Where(x => x.CalendarEventId == calendarEventId)
            .Select(x => x.TimetableTemplateId)
            .ToHashSetAsync();

        foreach (var lecture in lectures)
        {
            if (existingOverrides.Contains(lecture.Id))
            {
                continue;
            }

            var lectureOverride = CreateLectureOverride(calendarEvent, lecture);

            _dbContext.LectureOverrides.Add(lectureOverride);
        }
    }

    public async Task RemoveLectureOverridesAsync(Guid calendarEventId)
    {
        var overrides = await _dbContext.LectureOverrides
            .Where(x => x.CalendarEventId == calendarEventId)
            .ToListAsync();

        if (overrides.Count == 0)
        {
            return;
        }

        _dbContext.LectureOverrides.RemoveRange(overrides);
    }

    public async Task<bool> IsTeacherAvailableAsync(Guid teacherId, DateOnly date,TimeOnly startTime, TimeOnly endTime)
    {
        return !await HasTeacherConflictAsync(teacherId,date,startTime,endTime);
    }

    public async Task<bool> IsRoomAvailableAsync(Guid roomId,DateOnly date,TimeOnly startTime,TimeOnly endTime)
    {
        return !await HasRoomConflictAsync(roomId,date,startTime,endTime);
    }

    public async Task<bool> IsSectionAvailableAsync(Guid sectionId,DateOnly date,TimeOnly startTime,TimeOnly endTime)
    {
        return !await HasSectionConflictAsync(sectionId,date,startTime, endTime);
    }

    private static bool IsTimeOverlapping(TimeOnly start1,TimeOnly end1, TimeOnly start2, TimeOnly end2)
    {
        return start1 < end2 && start2 < end1;
    }

    private static DayOfWeekType GetDayOfWeekType(DateOnly date)
    {
        return date.DayOfWeek switch
        {
            DayOfWeek.Monday => DayOfWeekType.Monday,
            DayOfWeek.Tuesday => DayOfWeekType.Tuesday,
            DayOfWeek.Wednesday => DayOfWeekType.Wednesday,
            DayOfWeek.Thursday => DayOfWeekType.Thursday,
            DayOfWeek.Friday => DayOfWeekType.Friday,
            DayOfWeek.Saturday => DayOfWeekType.Saturday,
            DayOfWeek.Sunday => DayOfWeekType.Sunday,
            _ => throw new Exception("Invalid day.")
        };
    }

    private IQueryable<TimetableTemplate> ApplyScope(IQueryable<TimetableTemplate> query)
    {
        if (_scope.IsSuperAdmin() ||
            _scope.IsPlatformAdmin())
        {
            return query;
        }

        if (_scope.IsInstitutionAdmin())
        {
            query = query.Where(x =>
                x.InstitutionId ==
                _scope.InstitutionId());
        }

        if (_scope.IsCampusAdmin())
        {
            query = query.Where(x =>
                x.CampusId ==
                _scope.CampusId());
        }

        return query;
    }

    private IQueryable<CalendarEvent> ApplyCalendarScope(IQueryable<CalendarEvent> query)
    {
        if (_scope.IsSuperAdmin() ||
            _scope.IsPlatformAdmin())
        {
            return query;
        }

        if (_scope.IsInstitutionAdmin())
        {
            query = query.Where(x =>
                x.InstitutionId ==
                _scope.InstitutionId());
        }

        if (_scope.IsCampusAdmin())
        {
            query = query.Where(x =>
                x.CampusId ==
                _scope.CampusId());
        }

        return query;
    }

    private async Task<bool> HasTeacherConflictAsync(Guid teacherId, DateOnly date, TimeOnly start, TimeOnly end)
    {
        var day = GetDayOfWeekType(date);

        return await ApplyScope(_dbContext.TimetableTemplates)
            .AnyAsync(x =>
                x.IsActive &&
                x.TeacherId == teacherId &&
                x.DayOfWeek == day &&
                x.ValidFrom <= date &&
                x.ValidTo >= date &&
                IsTimeOverlapping(x.StartTime, x.EndTime, start, end));
    }

    private async Task<bool> HasRoomConflictAsync(Guid roomId, DateOnly date, TimeOnly start, TimeOnly end)
    {
        var day = GetDayOfWeekType(date);

        return await ApplyScope(_dbContext.TimetableTemplates)
            .AnyAsync(x =>
                x.IsActive &&
                x.RoomId == roomId &&
                x.DayOfWeek == day &&
                x.ValidFrom <= date &&
                x.ValidTo >= date &&
                IsTimeOverlapping(x.StartTime, x.EndTime, start, end));
    }

    private async Task<bool> HasSectionConflictAsync(Guid sectionId, DateOnly date, TimeOnly start, TimeOnly end)
    {
        var day = GetDayOfWeekType(date);

        return await ApplyScope(_dbContext.TimetableTemplates)
            .AnyAsync(x =>
                x.IsActive &&
                x.SectionId == sectionId &&
                x.DayOfWeek == day &&
                x.ValidFrom <= date &&
                x.ValidTo >= date &&
                IsTimeOverlapping(x.StartTime, x.EndTime, start, end));
    }

    private async Task ValidateTeacherAsync(ScheduleValidationRequest request, ScheduleValidationResponse response)
    {
        if (!request.TeacherId.HasValue || request.IsFullDay)
        {
            return;
        }

        for (var date = request.StartDate; date <= request.EndDate; date = date.AddDays(1))
        {
            if (!await HasTeacherConflictAsync(request.TeacherId.Value, date, request.StartTime!.Value, request.EndTime!.Value))
            {
                continue;
            }

            response.Conflicts.Add(new ScheduleConflictResponse
            {
                ConflictType = "Teacher",

                TeacherId = request.TeacherId,

                Date = date,

                StartTime = request.StartTime,

                EndTime = request.EndTime,

                Message = "Teacher already has another lecture."
            });
        }
    }

    private async Task ValidateRoomAsync(ScheduleValidationRequest request, ScheduleValidationResponse response)
    {
        if (!request.RoomId.HasValue || request.IsFullDay)
        {
            return;
        }

        for (var date = request.StartDate; date <= request.EndDate; date = date.AddDays(1))
        {
            if (!await HasRoomConflictAsync(request.RoomId.Value, date, request.StartTime!.Value, request.EndTime!.Value))
            {
                continue;
            }

            response.Conflicts.Add(new ScheduleConflictResponse
            {
                ConflictType = "Room",

                RoomId = request.RoomId,

                Date = date,

                StartTime = request.StartTime,

                EndTime = request.EndTime,

                Message = "Room already occupied."
            });
        }
    }

    private async Task ValidateSectionAsync(ScheduleValidationRequest request, ScheduleValidationResponse response)
    {
        if (!request.SectionId.HasValue || request.IsFullDay)
        {
            return;
        }

        for (var date = request.StartDate; date <= request.EndDate; date = date.AddDays(1))
        {
            if (!await HasSectionConflictAsync(request.SectionId.Value, date, request.StartTime!.Value, request.EndTime!.Value))
            {
                continue;
            }

            response.Conflicts.Add(new ScheduleConflictResponse
            {
                ConflictType = "Section",

                SectionId = request.SectionId,

                Date = date,

                StartTime = request.StartTime,

                EndTime = request.EndTime,

                Message = "Section already has another lecture."
            });
        }
    }

    private async Task ValidateCalendarConflictsAsync(ScheduleValidationRequest request, ScheduleValidationResponse response)
    {
        var events = await ApplyCalendarScope(_dbContext.CalendarEvents)
            .Include(x => x.Teacher)
                .ThenInclude(x => x.User)
            .Include(x => x.Room)
            .Include(x => x.Section)
            .Where(x =>
                x.IsActive &&
                x.Id != request.CalendarEventId &&
                x.AcademicSessionId == request.AcademicSessionId &&
                x.StartDate <= request.EndDate &&
                x.EndDate >= request.StartDate)
            .ToListAsync();

        foreach (var calendarEvent in events)
        {
            if (!request.IsFullDay &&
                !calendarEvent.IsFullDay)
            {
                if (!IsTimeOverlapping(
                        calendarEvent.StartTime!.Value,
                        calendarEvent.EndTime!.Value,
                        request.StartTime!.Value,
                        request.EndTime!.Value))
                {
                    continue;
                }
            }

            bool hasConflict = (request.TeacherId.HasValue && calendarEvent.TeacherId == request.TeacherId)

                ||

                (request.RoomId.HasValue &&
                 calendarEvent.RoomId == request.RoomId)

                ||

                (request.SectionId.HasValue &&
                 calendarEvent.SectionId == request.SectionId);

            if (!hasConflict)
                continue;

            var conflict = new ScheduleConflictResponse
            {
                CalendarEventId = calendarEvent.Id,

                ConflictType = "Calendar Event",

                Message = $"Conflicts with calendar event '{calendarEvent.Title}'.",

                TeacherId = calendarEvent.TeacherId,

                TeacherName =
        calendarEvent.Teacher != null
            ? calendarEvent.Teacher.User.FirstName + " " +
              calendarEvent.Teacher.User.LastName
            : null,

                RoomId = calendarEvent.RoomId,

                RoomName =
        calendarEvent.Room != null
            ? $"{calendarEvent.Room.Building}-{calendarEvent.Room.RoomNumber}"
            : null,

                SectionId = calendarEvent.SectionId,

                SectionName = calendarEvent.Section?.Name,

                Date = calendarEvent.StartDate,

                StartTime = calendarEvent.StartTime,

                EndTime = calendarEvent.EndTime,

                ExistingPriority = calendarEvent.Priority,

                RequestedPriority = request.Priority
            };

            conflict.CanOverride = CanOverride(conflict, request);

            conflict.SuggestedAction = GetSuggestedAction(conflict, request);

            response.Conflicts.Add(conflict);
        }
    }

    private async Task<List<TimetableTemplate>> GetMatchingTimetableEntriesAsync(ScheduleValidationRequest request)
    {
        var day = GetDayOfWeekType(request.StartDate);

        return await ApplyScope(_dbContext.TimetableTemplates)
            .Include(x => x.Teacher)
                .ThenInclude(x => x.User)
            .Include(x => x.Room)
            .Include(x => x.Section)
            .Include(x => x.SemesterSubject)
                .ThenInclude(x => x.Subject)
            .Where(x =>
                x.IsActive &&
                x.AcademicSessionId == request.AcademicSessionId &&
                x.DayOfWeek == day &&
                x.ValidFrom <= request.StartDate &&
                x.ValidTo >= request.StartDate)
            .ToListAsync();
    }

    private async Task ValidateTimetableConflictsAsync(ScheduleValidationRequest request, ScheduleValidationResponse response)
    {
        if (!request.AffectsTimetable)
        {
            return;
        }

        var lectures = await GetMatchingTimetableEntriesAsync(request);

        foreach (var lecture in lectures)
        {
            if (!IsTimeOverlapping(
                    lecture.StartTime,
                    lecture.EndTime,
                    request.StartTime!.Value,
                    request.EndTime!.Value))
            {
                continue;
            }

            var sameTeacher =
                request.TeacherId.HasValue &&
                lecture.TeacherId == request.TeacherId;

            var sameRoom =
                request.RoomId.HasValue &&
                lecture.RoomId == request.RoomId;

            var sameSection =
                request.SectionId.HasValue &&
                lecture.SectionId == request.SectionId;

            if (!sameTeacher &&
                !sameRoom &&
                !sameSection)
            {
                continue;
            }

            var conflict = new ScheduleConflictResponse
            {
                TimetableTemplateId = lecture.Id,

                ConflictType = "Timetable",

                Message = "Conflicts with an existing lecture.",

                TeacherId = lecture.TeacherId,

                TeacherName = lecture.Teacher.User.FirstName + " " + lecture.Teacher.User.LastName,

                RoomId = lecture.RoomId,

                RoomName = $"{lecture.Room.Building} - {lecture.Room.RoomNumber}",

                SectionId = lecture.SectionId,

                SectionName = lecture.Section.Name,

                SemesterSubjectId = lecture.SemesterSubjectId,

                SubjectName = lecture.SemesterSubject.Subject.Name,

                Date = request.StartDate,

                StartTime = lecture.StartTime,

                EndTime = lecture.EndTime,

                ExistingPriority = lecture.Priority,

                RequestedPriority = request.Priority
            };

            conflict.CanOverride = CanOverride(conflict, request);

            conflict.SuggestedAction = GetSuggestedAction(conflict, request);

            response.Conflicts.Add(conflict);
        }
    }

    private bool CanOverride(ScheduleConflictResponse conflict, ScheduleValidationRequest request)
    {
        return request.Priority > conflict.ExistingPriority;
    }

    private string GetSuggestedAction(ScheduleConflictResponse conflict, ScheduleValidationRequest request)
    {
        if (!CanOverride(conflict, request))
        {
            return "Choose another schedule.";
        }

        return conflict.ConflictType switch
        {
            "Timetable" => "Existing lecture will be cancelled.",

            "Calendar Event" => "Existing calendar event will be overridden.",

            "Teacher" => "Assign another teacher.",

            "Room" => "Assign another room.",

            "Section" => "Choose another time.",

            _ => "Manual intervention required."
        };
    }

    private async Task<CalendarEvent> GetCalendarEventAsync(Guid calendarEventId)
    {
        var calendarEvent = await ApplyCalendarScope(_dbContext.CalendarEvents)
            .AsNoTracking()
            .Include(x => x.Teacher)
            .ThenInclude(x => x.User)
            .Include(x => x.Room)
            .Include(x => x.Section)
            .FirstOrDefaultAsync(x => x.Id == calendarEventId);

        if (calendarEvent == null)
        {
            throw new Exception("Calendar event not found.");
        }

        return calendarEvent;
    }

    private LectureOverride CreateLectureOverride(CalendarEvent calendarEvent, TimetableTemplate lecture)
    {
        return new LectureOverride
        {
            Id = Guid.NewGuid(),

            InstitutionId = lecture.InstitutionId,

            CampusId = lecture.CampusId,

            AcademicSessionId = lecture.AcademicSessionId,

            CalendarEventId = calendarEvent.Id,

            TimetableTemplateId = lecture.Id,

            OverrideDate = calendarEvent.StartDate,

            OverrideType = ResolveOverrideType(calendarEvent, lecture),

            Reason = calendarEvent.Title,

            Remarks = calendarEvent.Description,

            OriginalTeacherId = lecture.TeacherId,

            OriginalRoomId = lecture.RoomId,

            OriginalStartTime = lecture.StartTime,

            OriginalEndTime = lecture.EndTime,

            NewTeacherId = calendarEvent.TeacherId != lecture.TeacherId ? calendarEvent.TeacherId: null,

            NewRoomId = calendarEvent.RoomId != lecture.RoomId ? calendarEvent.RoomId : null,

            NewStartTime = !calendarEvent.IsFullDay ? calendarEvent.StartTime : null,

            NewEndTime = !calendarEvent.IsFullDay ? calendarEvent.EndTime : null,

            GenerateAttendance = calendarEvent.EventType != EventType.Holiday && calendarEvent.EventType != EventType.Maintenance,

            IsSystemGenerated = true,

            IsApproved = true,

            ApprovedOn = DateTime.UtcNow,

            Version = 1
        };
    }

    private OverrideType ResolveOverrideType(CalendarEvent calendarEvent, TimetableTemplate lecture)
    {
        if (calendarEvent.RoomId.HasValue && calendarEvent.RoomId != lecture.RoomId)
        {
            return OverrideType.RoomChanged;
        }

        if (calendarEvent.TeacherId.HasValue && calendarEvent.TeacherId != lecture.TeacherId)
        {
            return OverrideType.TeacherChanged;
        }

        if (!calendarEvent.IsFullDay &&
            (calendarEvent.StartTime != lecture.StartTime ||
             calendarEvent.EndTime != lecture.EndTime))
        {
            return OverrideType.TimeChanged;
        }

        return calendarEvent.EventType switch
        {
            EventType.Examination => OverrideType.Cancelled,

            EventType.Holiday => OverrideType.Cancelled,

            EventType.Maintenance => OverrideType.RoomChanged,

            EventType.GuestLecture => OverrideType.TeacherChanged,

            EventType.ExtraClass => OverrideType.Rescheduled,

            _ => OverrideType.Cancelled
        };
    }

    private static ScheduleValidationResponse CreateValidationResponse(bool canAutoOverride)
    {
        return new ScheduleValidationResponse
        {
            IsValid = true,
            CanAutoOverride = canAutoOverride
        };
    }
}