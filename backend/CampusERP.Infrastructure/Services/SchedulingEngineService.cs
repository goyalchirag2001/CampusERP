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

    #region Public Validation

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

        /*
         * IMPORTANT:
         *
         * Timetable validation must NOT call:
         *
         * ValidateTeacherAsync
         * ValidateRoomAsync
         * ValidateSectionAsync
         *
         * Those methods validate a specific calendar date and loop
         * through every date in the range.
         *
         * A timetable is a recurring weekly rule, so we validate
         * the recurring rule directly.
         */

        await ValidateTimetableConflictsAsync(request, response);

        response.IsValid = response.Conflicts.Count == 0;

        return response;
    }

    #endregion

    #region Timetable Overrides

    public async Task<List<TimetableTemplate>> GetAffectedTimetableLecturesAsync(ScheduleValidationRequest request)
    {
        if (!request.AffectsTimetable)
        {
            return new List<TimetableTemplate>();
        }

        return await GetAffectedTimetableLecturesAsync(request, request.StartDate);
    }

    private async Task<List<TimetableTemplate>> GetAffectedTimetableLecturesAsync(ScheduleValidationRequest request, DateOnly date)
    {
        var lectures = await GetMatchingTimetableEntriesAsync(request, date);

        var affectedLectures = new List<TimetableTemplate>();

        foreach (var lecture in lectures)
        {
            /*
             * Full-day timetable event:
             *
             * If no teacher, room or section is specified,
             * the event affects every lecture on that date.
             *
             * If a specific teacher/room/section is supplied,
             * only matching lectures are affected.
             */
            if (request.IsFullDay)
            {
                bool hasTarget = request.TeacherId.HasValue || request.RoomId.HasValue || request.SectionId.HasValue;

                if (!hasTarget)
                {
                    affectedLectures.Add(lecture);
                    continue;
                }

                bool affectsLecture =
                    (request.TeacherId.HasValue &&
                     lecture.TeacherId == request.TeacherId.Value)
                    ||
                    (request.RoomId.HasValue &&
                     lecture.RoomId == request.RoomId.Value)
                    ||
                    (request.SectionId.HasValue &&
                     lecture.SectionId == request.SectionId.Value);

                if (affectsLecture)
                {
                    affectedLectures.Add(lecture);
                }

                continue;
            }

            /*
             * Timed timetable event.
             *
             * A timed event must have both start and end time.
             */
            if (!request.StartTime.HasValue || !request.EndTime.HasValue)
            {
                continue;
            }

            if (!IsTimeOverlapping(lecture.StartTime, lecture.EndTime, request.StartTime.Value, request.EndTime.Value))
            {
                continue;
            }

            /*
             * If the event has no specific target, it affects
             * every overlapping lecture.
             */
            bool hasTargetForTimedEvent = request.TeacherId.HasValue || request.RoomId.HasValue || request.SectionId.HasValue;

            if (!hasTargetForTimedEvent)
            {
                affectedLectures.Add(lecture);
                continue;
            }

            bool affectsTimedLecture =
                (request.TeacherId.HasValue &&
                 lecture.TeacherId == request.TeacherId.Value)
                ||
                (request.RoomId.HasValue &&
                 lecture.RoomId == request.RoomId.Value)
                ||
                (request.SectionId.HasValue &&
                 lecture.SectionId == request.SectionId.Value);

            if (affectsTimedLecture)
            {
                affectedLectures.Add(lecture);
            }
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

        var occurrences = await GetAffectedTimetableLectureOccurrencesAsync(calendarEvent);

        if (occurrences.Count == 0)
        {
            return;
        }

        var timetableIds = occurrences.Select(x => x.Lecture.Id).Distinct().ToList();

        var occurrenceDates = occurrences.Select(x => x.Date).Distinct().ToList();

        /*
         * IMPORTANT:
         *
         * LectureOverride uses:
         *
         * TimetableTemplateId + OverrideDate
         *
         * as a unique key.
         *
         * We MUST include soft-deleted rows here.
         *
         * Otherwise EF hides a previously deleted override and
         * we attempt to INSERT another row with the same unique key.
         */
        var existingOverrides = await _dbContext.LectureOverrides
            .IgnoreQueryFilters()
            .Include(x => x.CalendarEvent)
            .Where(x =>
                x.TimetableTemplateId.HasValue &&
                timetableIds.Contains(x.TimetableTemplateId.Value) &&
                occurrenceDates.Contains(x.OverrideDate))
            .ToListAsync();

        var existingOverrideMap = existingOverrides.Where(x => x.TimetableTemplateId.HasValue).ToDictionary(x => $"{x.TimetableTemplateId!.Value:N}:{x.OverrideDate:yyyy-MM-dd}");

        foreach (var occurrence in occurrences)
        {
            var lecture = occurrence.Lecture;
            var date = occurrence.Date;

            var key = $"{lecture.Id:N}:{date:yyyy-MM-dd}";

            /*
             * An override already exists for this exact
             * timetable occurrence.
             */
            if (existingOverrideMap.TryGetValue(key, out var existingOverride))
            {
                /*
                 * The row may have been soft-deleted previously.
                 *
                 * Restore it before reusing it.
                 */
                if (existingOverride.IsDeleted)
                {
                    existingOverride.IsDeleted = false;
                }

                /*
                 * If this override belongs to the same Calendar Event,
                 * update it directly.
                 */
                if (existingOverride.CalendarEventId == calendarEvent.Id)
                {
                    UpdateLectureOverride(existingOverride, calendarEvent, lecture);

                    continue;
                }

                /*
                 * If another Calendar Event owns this occurrence,
                 * compare priorities.
                 */
                var existingCalendarEvent = existingOverride.CalendarEvent;

                /*
                 * A deleted CalendarEvent relationship should not
                 * protect the occurrence.
                 *
                 * If the relationship is unavailable, the current
                 * event can take ownership.
                 */
                if (existingCalendarEvent == null)
                {
                    UpdateLectureOverride(existingOverride, calendarEvent, lecture);

                    continue;
                }

                var existingPriority = existingCalendarEvent.Priority;

                var requestedPriority = calendarEvent.Priority;

                /*
                 * Higher priority wins.
                 *
                 * Equal priority also allows the current/new event
                 * to replace the existing event.
                 */
                if (requestedPriority < existingPriority)
                {
                    continue;
                }

                UpdateLectureOverride(existingOverride, calendarEvent, lecture);

                continue;
            }

            /*
             * No active OR soft-deleted override exists.
             *
             * Create a completely new row.
             */
            var lectureOverride = CreateLectureOverride(calendarEvent, lecture, date);

            _dbContext.LectureOverrides.Add(lectureOverride);
        }

        await _dbContext.SaveChangesAsync();
    }

    private void UpdateLectureOverride(LectureOverride existingOverride, CalendarEvent calendarEvent, TimetableTemplate lecture)
    {
        existingOverride.CalendarEventId = calendarEvent.Id;

        existingOverride.TimetableTemplateId = lecture.Id;

        existingOverride.OverrideType = ResolveOverrideType(calendarEvent, lecture);

        existingOverride.Reason = calendarEvent.Title;

        existingOverride.Remarks = calendarEvent.Description;

        existingOverride.OriginalTeacherId = lecture.TeacherId;

        existingOverride.OriginalRoomId = lecture.RoomId;

        existingOverride.OriginalStartTime = lecture.StartTime;

        existingOverride.OriginalEndTime = lecture.EndTime;

        existingOverride.NewTeacherId = calendarEvent.TeacherId != lecture.TeacherId
                ? calendarEvent.TeacherId
                : null;

        existingOverride.NewRoomId = calendarEvent.RoomId != lecture.RoomId
                ? calendarEvent.RoomId
                : null;

        existingOverride.NewStartTime = !calendarEvent.IsFullDay
                ? calendarEvent.StartTime
                : null;

        existingOverride.NewEndTime = !calendarEvent.IsFullDay
                ? calendarEvent.EndTime
                : null;

        existingOverride.GenerateAttendance = calendarEvent.EventType != EventType.Holiday && calendarEvent.EventType != EventType.Maintenance;

        existingOverride.IsSystemGenerated =  true;

        existingOverride.IsApproved = true;

        existingOverride.ApprovedOn = DateTime.UtcNow;

        existingOverride.Version++;
    }

    public async Task RemoveLectureOverridesAsync(Guid calendarEventId)
    {
        var overrides = await _dbContext.LectureOverrides
                .IgnoreQueryFilters()
                .Where(x =>
                    x.CalendarEventId == calendarEventId && !x.IsDeleted)
                .ToListAsync();

        if (overrides.Count == 0)
        {
            return;
        }

        _dbContext.LectureOverrides.RemoveRange(overrides);
    }

    #endregion

    #region Availability

    public async Task<bool> IsTeacherAvailableAsync(Guid teacherId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
    {
        return !await HasTeacherConflictAsync(teacherId, date, startTime, endTime);
    }

    public async Task<bool> IsRoomAvailableAsync(Guid roomId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
    {
        return !await HasRoomConflictAsync(roomId, date, startTime, endTime);
    }

    public async Task<bool> IsSectionAvailableAsync(Guid sectionId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
    {
        return !await HasSectionConflictAsync(sectionId, date, startTime, endTime);
    }

    #endregion

    #region Basic Helpers

    private static bool IsTimeOverlapping(TimeOnly start1, TimeOnly end1, TimeOnly start2, TimeOnly end2)
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

    /*
     * Determines whether two recurring validity ranges
     * actually share at least one occurrence of the requested
     * weekday.
     *
     * Example:
     *
     * Existing:
     * Monday, Aug 3 - Aug 10
     *
     * Requested:
     * Monday, Aug 5 - Aug 20
     *
     * There is a common Monday -> true.
     *
     * This avoids looping through every date.
     */
    private static bool HasWeekdayOccurrenceOverlap(DateOnly existingStart, DateOnly existingEnd, DateOnly requestedStart, DateOnly requestedEnd, DayOfWeekType dayOfWeek)
    {
        var overlapStart = existingStart > requestedStart
                ? existingStart
                : requestedStart;

        var overlapEnd =
            existingEnd < requestedEnd
                ? existingEnd
                : requestedEnd;

        if (overlapStart > overlapEnd)
        {
            return false;
        }

        var targetDay = ToSystemDayOfWeek(dayOfWeek);

        var daysUntilTarget = ((int)targetDay -
             (int)overlapStart.DayOfWeek +
             7) % 7;

        var firstOccurrence = overlapStart.AddDays(daysUntilTarget);

        return firstOccurrence <= overlapEnd;
    }

    private static DayOfWeek ToSystemDayOfWeek(DayOfWeekType dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeekType.Monday => DayOfWeek.Monday,
            DayOfWeekType.Tuesday => DayOfWeek.Tuesday,
            DayOfWeekType.Wednesday => DayOfWeek.Wednesday,
            DayOfWeekType.Thursday => DayOfWeek.Thursday,
            DayOfWeekType.Friday => DayOfWeek.Friday,
            DayOfWeekType.Saturday => DayOfWeek.Saturday,
            DayOfWeekType.Sunday => DayOfWeek.Sunday,
            _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek))
        };
    }

    #endregion

    #region Scope

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

    #endregion

    #region Specific Date Conflict Checks

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
                x.StartTime < end &&
                start < x.EndTime);
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
                x.StartTime < end &&
                start < x.EndTime);
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
                x.StartTime < end &&
                start < x.EndTime);
    }

    #endregion

    #region Calendar Resource Validation

    /*
     * These methods are intentionally retained for calendar-event
     * validation. Calendar events operate on actual dates, unlike
     * recurring timetable templates.
     */

    private async Task ValidateTeacherAsync(ScheduleValidationRequest request, ScheduleValidationResponse response)
    {
        if (!request.TeacherId.HasValue || request.IsFullDay)
        {
            return;
        }

        for (var date = request.StartDate; date <= request.EndDate; date = date.AddDays(1))
        {
            if (GetDayOfWeekType(date) != request.DayOfWeek)
            {
                continue;
            }

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
            if (GetDayOfWeekType(date) != request.DayOfWeek)
            {
                continue;
            }

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
            if (GetDayOfWeekType(date) != request.DayOfWeek)
            {
                continue;
            }

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

    #endregion

    #region Calendar Conflict Validation

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
            if (!request.IsFullDay && !calendarEvent.IsFullDay)
            {
                if (!IsTimeOverlapping(calendarEvent.StartTime!.Value,
                        calendarEvent.EndTime!.Value,
                        request.StartTime!.Value,
                        request.EndTime!.Value))
                {
                    continue;
                }
            }

            bool hasConflict = (request.TeacherId.HasValue && calendarEvent.TeacherId == request.TeacherId.Value)
                ||
                (request.RoomId.HasValue && calendarEvent.RoomId == request.RoomId.Value)
                ||
                (request.SectionId.HasValue && calendarEvent.SectionId == request.SectionId.Value);

            if (!hasConflict)
            {
                continue;
            }

            var conflict =new ScheduleConflictResponse
                {
                    CalendarEventId = calendarEvent.Id,

                    ConflictType = "Calendar Event",

                    Message = $"Conflicts with calendar event '{calendarEvent.Title}'.",

                    TeacherId = calendarEvent.TeacherId,

                    TeacherName =  calendarEvent.Teacher != null ? calendarEvent.Teacher.User.FirstName + " " + calendarEvent.Teacher.User.LastName : null,

                    RoomId = calendarEvent.RoomId,

                    RoomName = calendarEvent.Room != null
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

    #endregion

    #region Timetable Conflict Validation

    /*
     * IMPORTANT:
     *
     * This is the timetable validation path.
     *
     * It performs ONE database query.
     *
     * It does NOT loop over every date.
     *
     * It does NOT perform separate teacher/room/section queries.
     *
     * It also excludes the current timetable during UPDATE.
     */

    private async Task ValidateTimetableConflictsAsync(ScheduleValidationRequest request, ScheduleValidationResponse response)
    {
        if (!request.AffectsTimetable)
        {
            return;
        }

        // Full-day events do not have a time range.
        // They are handled by lecture override generation
        // on every affected timetable occurrence.
        if (request.IsFullDay || !request.StartTime.HasValue || !request.EndTime.HasValue)
        {
            return;
        }

        var lectures = await GetMatchingTimetableEntriesAsync(request);

        foreach (var lecture in lectures)
        {
            if (!IsTimeOverlapping(lecture.StartTime, lecture.EndTime, request.StartTime.Value, request.EndTime.Value))
            {
                continue;
            }

            var sameTeacher = request.TeacherId.HasValue && lecture.TeacherId == request.TeacherId.Value;

            var sameRoom = request.RoomId.HasValue && lecture.RoomId == request.RoomId.Value;

            var sameSection = request.SectionId.HasValue && lecture.SectionId == request.SectionId.Value;

            if (!sameTeacher && !sameRoom && !sameSection)
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

                RoomName = lecture.Room != null ? $"{lecture.Room.Building} - {lecture.Room.RoomNumber}" : null,

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
    #endregion

    #region Timetable Query For Calendar Overrides

    /*
     * This method is intentionally kept separate from timetable
     * validation.
     *
     * Calendar override generation works against an actual
     * calendar date, so StartDate is appropriate here.
     */
    private async Task<List<TimetableTemplate>> GetMatchingTimetableEntriesAsync(ScheduleValidationRequest request, DateOnly date)
    {
        var day = GetDayOfWeekType(date);

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
                x.ValidFrom <= date &&
                x.ValidTo >= date)
            .ToListAsync();
    }

    #endregion

    #region Priority

    private bool CanOverride(ScheduleConflictResponse conflict, ScheduleValidationRequest request)
    {
        return request.Priority >= conflict.ExistingPriority;
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

    #endregion

    #region Calendar Event

    private async Task<CalendarEvent> GetCalendarEventAsync(Guid calendarEventId)
    {
        var calendarEvent = await ApplyCalendarScope(_dbContext.CalendarEvents)
                .AsNoTracking()
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .Include(x => x.Room)
                .Include(x => x.Section)
                .FirstOrDefaultAsync(
                    x => x.Id == calendarEventId);

        if (calendarEvent == null)
        {
            throw new Exception("Calendar event not found.");
        }

        return calendarEvent;
    }

    private LectureOverride CreateLectureOverride(CalendarEvent calendarEvent, TimetableTemplate lecture, DateOnly overrideDate)
    {
        return new LectureOverride
        {
            Id = Guid.NewGuid(),

            InstitutionId = lecture.InstitutionId,

            CampusId = lecture.CampusId,

            AcademicSessionId = lecture.AcademicSessionId,

            CalendarEventId = calendarEvent.Id,

            TimetableTemplateId = lecture.Id,

            OverrideDate = overrideDate,

            OverrideType = ResolveOverrideType(calendarEvent, lecture),

            Reason = calendarEvent.Title,

            Remarks = calendarEvent.Description,

            OriginalTeacherId = lecture.TeacherId,

            OriginalRoomId = lecture.RoomId,

            OriginalStartTime = lecture.StartTime,

            OriginalEndTime = lecture.EndTime,

            NewTeacherId = calendarEvent.TeacherId != lecture.TeacherId ? calendarEvent.TeacherId : null,

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

        if (!calendarEvent.IsFullDay && (calendarEvent.StartTime != lecture.StartTime || calendarEvent.EndTime != lecture.EndTime))
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

    #endregion

    #region Response

    private static ScheduleValidationResponse CreateValidationResponse(bool canAutoOverride)
    {
        return new ScheduleValidationResponse
        {
            IsValid = true,

            CanAutoOverride = canAutoOverride
        };
    }

    #endregion

    private async Task<List<(TimetableTemplate Lecture, DateOnly Date)>> GetAffectedTimetableLectureOccurrencesAsync(CalendarEvent calendarEvent)
    {
        var lectures = await ApplyScope(_dbContext.TimetableTemplates)
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .Include(x => x.Room)
                .Include(x => x.Section)
                .Include(x => x.SemesterSubject)
                    .ThenInclude(x => x.Subject)
                .Where(x =>
                    x.IsActive &&
                    x.InstitutionId == calendarEvent.InstitutionId &&
                    x.CampusId == calendarEvent.CampusId &&
                    x.AcademicSessionId == calendarEvent.AcademicSessionId &&
                    x.ValidFrom <= calendarEvent.EndDate &&
                    x.ValidTo >= calendarEvent.StartDate)
                .ToListAsync();

        var result = new List<(TimetableTemplate Lecture, DateOnly Date)>();

        for (var date = calendarEvent.StartDate; date <= calendarEvent.EndDate; date = date.AddDays(1))
        {
            var dayOfWeek = GetDayOfWeekType(date);

            foreach (var lecture in lectures)
            {
                if (lecture.DayOfWeek != dayOfWeek)
                {
                    continue;
                }

                if (date < lecture.ValidFrom || date > lecture.ValidTo)
                {
                    continue;
                }

                /*
                 * No specific teacher, room or section means
                 * the event affects the entire timetable scope.
                 *
                 * Example:
                 * Holiday
                 * Examination
                 * Campus-wide academic event
                 */
                var isTargeted = calendarEvent.TeacherId.HasValue || calendarEvent.RoomId.HasValue || calendarEvent.SectionId.HasValue;

                var affectsLecture = !isTargeted || (calendarEvent.TeacherId.HasValue && lecture.TeacherId == calendarEvent.TeacherId.Value)
                    ||
                    (calendarEvent.RoomId.HasValue && lecture.RoomId == calendarEvent.RoomId.Value)
                    ||
                    (calendarEvent.SectionId.HasValue && lecture.SectionId == calendarEvent.SectionId.Value);

                if (!affectsLecture)
                {
                    continue;
                }

                /*
                 * Full-day event:
                 * the complete lecture occurrence is affected.
                 */
                if (calendarEvent.IsFullDay)
                {
                    result.Add((lecture, date));

                    continue;
                }

                /*
                 * Timed event:
                 * only overlapping lectures are affected.
                 */
                if (!calendarEvent.StartTime.HasValue || !calendarEvent.EndTime.HasValue)
                {
                    continue;
                }

                if (!IsTimeOverlapping(lecture.StartTime, lecture.EndTime, calendarEvent.StartTime.Value, calendarEvent.EndTime.Value))
                {
                    continue;
                }

                result.Add((lecture, date));
            }
        }

        return result;
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
}