using CampusERP.Application.Common.Exceptions;
using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using CampusERP.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

/// <summary>
/// Builds the unified calendar for teachers and students.
///
/// This service does not modify timetable templates or CalendarEvents.
/// It is a read-only occurrence projection layer.
/// </summary>
public class TimetableCalendarService : ITimetableCalendarService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IDataAccessScope _scope;
    private readonly ICurrentUserService _currentUser;

    public TimetableCalendarService(ApplicationDbContext dbContext, IDataAccessScope scope, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _scope = scope;
        _currentUser = currentUser;
    }

    #region Teacher Calendar

    public async Task<List<TimetableCalendarEventResponse>> GetTeacherCalendarAsync(TimetableCalendarRequest request)
    {
        ValidateRequest(request);

        var userId = _currentUser.UserId;

        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("Authenticated user could not be resolved.");
        }

        var teacher = await _dbContext.Teachers.AsNoTracking()
            .Where(x =>
                x.UserId == userId.Value &&
                x.IsActive)
            .Select(x => new
            {
                x.Id,
                x.InstitutionId,
                x.CampusId
            })
            .FirstOrDefaultAsync();

        if (teacher is null)
        {
            throw new NotFoundException(ErrorCodes.TeacherNotFound, "Teacher not found.");
        }

        ValidateCampusScope(teacher.CampusId);

        var sessionId = await ResolveAcademicSessionAsync(request.AcademicSessionId, teacher.CampusId, request.StartDate, request.EndDate);

        var timetableEvents = await GetTeacherTimetableEventsAsync(teacher.Id, teacher.InstitutionId, teacher.CampusId, sessionId, request.StartDate, request.EndDate);

        var lectureOverrides = await GetLectureOverridesAsync(teacher.InstitutionId, teacher.CampusId, sessionId, request.StartDate, request.EndDate);

        var calendarEvents = await GetCalendarEventsForTeacherAsync(teacher.Id, teacher.InstitutionId, teacher.CampusId, sessionId, request.StartDate, request.EndDate);

        timetableEvents = await ApplyLectureOverrides(timetableEvents, lectureOverrides);

        return MergeAndOrder(timetableEvents, calendarEvents);
    }

    #endregion

    #region Student Calendar

    public async Task<List<TimetableCalendarEventResponse>> GetStudentCalendarAsync(TimetableCalendarRequest request)
    {
        ValidateRequest(request);

        var userId = _currentUser.UserId;

        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("Authenticated user could not be resolved.");
        }

        var student = await _dbContext.Students.AsNoTracking()
            .Where(x =>
                x.UserId == userId.Value &&
                x.IsActive)
            .Select(x => new
            {
                x.Id,
                x.InstitutionId,
                x.CampusId,

                CurrentEnrollment = x.Enrollments
                    .Where(e => e.IsCurrent)
                    .Select(e => new
                    {
                        e.AcademicSessionId,
                        e.SectionId,
                        e.DepartmentId,
                        e.CourseId,
                        e.SemesterId
                    })
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();

        if (student is null)
        {
            throw new NotFoundException(ErrorCodes.StudentNotFound, "Student not found.");
        }

        if (student.CurrentEnrollment is null)
        {
            return [];
        }

        ValidateCampusScope(student.CampusId);

        var requestedSessionId = request.AcademicSessionId ?? student.CurrentEnrollment.AcademicSessionId;

        if (requestedSessionId != student.CurrentEnrollment.AcademicSessionId)
        {
            throw new BusinessRuleException(ErrorCodes.BusinessRule, "The requested academic session is not the student's current academic session.");
        }

        var timetableEvents = await GetStudentTimetableEventsAsync(student.CurrentEnrollment.SectionId, student.InstitutionId, student.CampusId, requestedSessionId, request.StartDate, request.EndDate);

        var lectureOverrides = await GetLectureOverridesAsync(student.InstitutionId, student.CampusId, requestedSessionId, request.StartDate, request.EndDate);

        var calendarEvents = await GetCalendarEventsForStudentAsync(student.CurrentEnrollment.SectionId, student.CurrentEnrollment.DepartmentId, student.CurrentEnrollment.CourseId, student.CurrentEnrollment.SemesterId, student.InstitutionId, student.CampusId, requestedSessionId, request.StartDate, request.EndDate);

        timetableEvents = await ApplyLectureOverrides(timetableEvents, lectureOverrides);

        return MergeAndOrder(timetableEvents, calendarEvents);
    }

    #endregion

    #region Timetable Queries

    private async Task<List<TimetableCalendarEventResponse>> GetTeacherTimetableEventsAsync(Guid teacherId, Guid institutionId, Guid campusId, Guid academicSessionId, DateOnly startDate, DateOnly endDate)
    {
        var templates = await _dbContext.TimetableTemplates
            .AsNoTracking()
            .Where(x =>
                x.TeacherId == teacherId &&
                x.InstitutionId == institutionId &&
                x.CampusId == campusId &&
                x.AcademicSessionId == academicSessionId &&
                x.IsActive &&

                // Template must overlap requested range.
                x.ValidFrom <= endDate &&
                x.ValidTo >= startDate)
            .Select(x => new TimetableTemplateCalendarProjection
            {
                Id = x.Id,

                InstitutionId = x.InstitutionId,

                CampusId = x.CampusId,

                AcademicSessionId = x.AcademicSessionId,

                TeacherId = x.TeacherId,

                TeacherName = x.Teacher.User.FirstName + " " + x.Teacher.User.LastName,

                SectionId = x.SectionId,

                SectionName = x.Section.Name,

                SubjectCode = x.SemesterSubject.Subject.Code,

                SubjectName = x.SemesterSubject.Subject.Name,

                RoomId = x.RoomId,

                RoomBuilding = x.Room != null ? x.Room.Building : null,

                RoomFloor = x.Room != null ? x.Room.Floor : null,

                RoomNumber = x.Room != null ? x.Room.RoomNumber : null,

                RoomName = x.Room != null ? x.Room.RoomName : null,

                DayOfWeek = x.DayOfWeek,

                StartTime = x.StartTime,

                EndTime = x.EndTime,

                ValidFrom = x.ValidFrom,

                ValidTo = x.ValidTo,

                LectureType = x.LectureType,

                Priority = x.Priority,

                GenerateAttendance = x.GenerateAttendance,

                IsOnline = x.IsOnline,

                MeetingLink = x.MeetingLink,

                Remarks = x.Remarks
            })
            .ToListAsync();

        return GenerateTimetableOccurrences(templates, startDate, endDate);
    }

    private async Task<List<TimetableCalendarEventResponse>> GetStudentTimetableEventsAsync(Guid sectionId, Guid institutionId, Guid campusId, Guid academicSessionId, DateOnly startDate, DateOnly endDate)
    {
        var templates = await _dbContext.TimetableTemplates
            .AsNoTracking()
            .Where(x =>
                x.SectionId == sectionId &&
                x.InstitutionId == institutionId &&
                x.CampusId == campusId &&
                x.AcademicSessionId == academicSessionId &&
                x.IsActive &&

                x.ValidFrom <= endDate &&
                x.ValidTo >= startDate)
            .Select(x => new TimetableTemplateCalendarProjection
            {
                Id = x.Id,

                InstitutionId = x.InstitutionId,

                CampusId = x.CampusId,

                AcademicSessionId = x.AcademicSessionId,

                TeacherId = x.TeacherId,

                TeacherName = x.Teacher.User.FirstName + " " + x.Teacher.User.LastName,

                SectionId = x.SectionId,

                SectionName = x.Section.Name,

                SubjectCode = x.SemesterSubject.Subject.Code,

                SubjectName = x.SemesterSubject.Subject.Name,

                RoomId = x.RoomId,

                RoomBuilding = x.Room != null ? x.Room.Building : null,

                RoomFloor = x.Room != null ? x.Room.Floor : null,

                RoomNumber = x.Room != null ? x.Room.RoomNumber : null,

                RoomName = x.Room != null ? x.Room.RoomName : null,

                DayOfWeek = x.DayOfWeek,

                StartTime = x.StartTime,

                EndTime = x.EndTime,

                ValidFrom = x.ValidFrom,

                ValidTo = x.ValidTo,

                LectureType = x.LectureType,

                Priority = x.Priority,

                GenerateAttendance = x.GenerateAttendance,

                IsOnline = x.IsOnline,

                MeetingLink = x.MeetingLink,

                Remarks = x.Remarks
            })
            .ToListAsync();

        return GenerateTimetableOccurrences(templates, startDate, endDate);
    }

    #endregion

    #region Calendar Events

    private async Task<List<TimetableCalendarEventResponse>> GetCalendarEventsForTeacherAsync(Guid teacherId, Guid institutionId, Guid campusId, Guid academicSessionId, DateOnly startDate, DateOnly endDate)
    {
        var events = await BuildCalendarEventQuery(institutionId, campusId, academicSessionId, startDate, endDate)
            .Where(x =>
                x.TeacherId == teacherId ||
                (
                    x.TeacherId == null &&
                    x.SectionId == null &&
                    x.CourseId == null &&
                    x.DepartmentId == null
                ))
            .ToListAsync();

        return ExpandCalendarEvents(events, startDate, endDate);
    }

    private async Task<List<TimetableCalendarEventResponse>> GetCalendarEventsForStudentAsync(Guid sectionId, Guid departmentId, Guid courseId, Guid semesterId, Guid institutionId, Guid campusId, Guid academicSessionId, DateOnly startDate, DateOnly endDate)
    {
        var events = await BuildCalendarEventQuery(institutionId, campusId, academicSessionId, startDate, endDate).Where(x =>
           x.SectionId == sectionId ||
           (x.SectionId == null &&
               (
                   x.SemesterId == semesterId ||
                   x.CourseId == courseId ||
                   x.DepartmentId == departmentId ||
                   (
                       x.SemesterId == null &&
                       x.CourseId == null &&
                       x.DepartmentId == null
                   )
               )
           ))
       .ToListAsync();

        return ExpandCalendarEvents(events, startDate, endDate);
    }

    private IQueryable<CalendarEventCalendarProjection> BuildCalendarEventQuery(Guid institutionId, Guid campusId, Guid academicSessionId, DateOnly startDate, DateOnly endDate)
    {
        return _dbContext.CalendarEvents
            .AsNoTracking()
            .Where(x =>
                x.InstitutionId == institutionId &&
                x.CampusId == campusId &&
                x.AcademicSessionId == academicSessionId &&
                x.IsActive &&

                // There is at least some overlap with the
                // requested calendar range.
                x.StartDate <= endDate &&
                x.EndDate >= startDate)
            .Select(x => new CalendarEventCalendarProjection
            {
                Id = x.Id,

                Title = x.Title,

                Description = x.Description,

                EventType = x.EventType,

                StartDate = x.StartDate,

                EndDate = x.EndDate,

                StartTime = x.StartTime,

                EndTime = x.EndTime,

                IsFullDay = x.IsFullDay,

                IsRecurring = x.IsRecurring,

                RecurrenceRule = x.RecurrenceRule,

                Priority = x.Priority,

                AffectsTimetable = x.AffectsTimetable,

                Color = x.Color,

                TeacherId = x.TeacherId,

                SectionId = x.SectionId,

                DepartmentId = x.DepartmentId,

                CourseId = x.CourseId,

                SemesterId = x.SemesterId,

                RoomId = x.RoomId,

                RoomBuilding = x.Room != null ? x.Room.Building : null,

                RoomFloor = x.Room != null ? x.Room.Floor : null,

                RoomNumber = x.Room != null ? x.Room.RoomNumber : null,

                RoomName = x.Room != null ? x.Room.RoomName : null,
            });
    }

    #endregion

    #region Occurrence Generation

    private static List<TimetableCalendarEventResponse> GenerateTimetableOccurrences(List<TimetableTemplateCalendarProjection> templates, DateOnly startDate, DateOnly endDate)
    {
        var result = new List<TimetableCalendarEventResponse>();

        foreach (var template in templates)
        {
            var effectiveStart = Max(startDate, template.ValidFrom);

            var effectiveEnd = Min(endDate, template.ValidTo);

            if (effectiveStart > effectiveEnd)
            {
                continue;
            }

            var current = effectiveStart;

            while (current <= effectiveEnd)
            {
                if (MatchesDay(current, template.DayOfWeek))
                {
                    result.Add(
                        new TimetableCalendarEventResponse
                        {
                            Id = CreateOccurrenceId(template.Id, current),

                            TimetableTemplateId = template.Id,

                            Date = current,

                            StartTime = template.StartTime,

                            EndTime = template.EndTime,

                            Title = string.IsNullOrWhiteSpace(
                                    template.SubjectCode)
                                    ? template.SubjectName ??
                                      "Lecture"
                                    : template.SubjectCode +
                                      " · " +
                                      template.SubjectName,

                            SubjectCode = template.SubjectCode,

                            SubjectName = template.SubjectName,

                            TeacherId = template.TeacherId,

                            TeacherName = template.TeacherName,

                            SectionId = template.SectionId,

                            SectionName = template.SectionName,

                            RoomId = template.RoomId,

                            RoomBuilding = template.IsOnline ? null: template.RoomBuilding,

                            RoomFloor = template.IsOnline ? null: template.RoomFloor,

                            RoomNumber = template.IsOnline ? null: template.RoomNumber,

                            RoomName = template.IsOnline ? null: template.RoomName,

                            LectureType = template.LectureType,

                            Priority = template.Priority,

                            GenerateAttendance = template.GenerateAttendance,

                            IsOnline = template.IsOnline,

                            MeetingLink = template.MeetingLink,

                            IsFullDay = false,

                            Color = GetTimetableColor(template.LectureType),

                            IsOverride = false,

                            IsCancelled = false
                        });
                }

                current = current.AddDays(1);
            }
        }

        return result;
    }

    private static List<TimetableCalendarEventResponse> ExpandCalendarEvents(List<CalendarEventCalendarProjection> events, DateOnly startDate, DateOnly endDate)
    {
        var result = new List<TimetableCalendarEventResponse>();

        foreach (var calendarEvent in events)
        {
            var effectiveStart = Max(startDate, calendarEvent.StartDate);

            var effectiveEnd = Min(endDate, calendarEvent.EndDate);

            if (effectiveStart > effectiveEnd)
            {
                continue;
            }

            var current = effectiveStart;

            while (current <= effectiveEnd)
            {
                if (!calendarEvent.IsRecurring || MatchesRecurrence(calendarEvent, current))
                {
                    AddCalendarOccurrence(result, calendarEvent, current);
                }

                current = current.AddDays(1);
            }
        }

        return result;
    }

    private static void AddCalendarOccurrence(List<TimetableCalendarEventResponse> result, CalendarEventCalendarProjection calendarEvent, DateOnly date)
    {
        result.Add(new TimetableCalendarEventResponse
            {
                Id = CreateCalendarOccurrenceId(calendarEvent.Id, date),

                CalendarEventId = calendarEvent.Id,

                Date = date,

                StartTime = calendarEvent.IsFullDay
                        ? null
                        : calendarEvent.StartTime,

                EndTime = calendarEvent.IsFullDay
                        ? null
                        : calendarEvent.EndTime,

                Title = calendarEvent.Title,

                Description = calendarEvent.Description,

                EventType = calendarEvent.EventType,

                Priority = calendarEvent.Priority,

                IsFullDay = calendarEvent.IsFullDay,

                Color = calendarEvent.Color,

                TeacherId = calendarEvent.TeacherId,

                SectionId = calendarEvent.SectionId,

                RoomId = calendarEvent.RoomId,

                RoomBuilding = calendarEvent.RoomBuilding,

                RoomFloor = calendarEvent.RoomFloor,

                RoomNumber = calendarEvent.RoomNumber,

                RoomName = calendarEvent.RoomName,

                IsOverride = calendarEvent.AffectsTimetable,

                IsCancelled = false
            });
    }

    #endregion

    #region Recurrence

    private static bool MatchesRecurrence(CalendarEventCalendarProjection calendarEvent, DateOnly date)
    {
        if (string.IsNullOrWhiteSpace(calendarEvent.RecurrenceRule))
        {
            return false;
        }

        /*
         * Phase 8.4.2 intentionally supports the
         * most important academic-calendar recurrence:
         *
         * WEEKLY
         *
         * We keep recurrence parsing isolated so that
         * RRULE support can be expanded later without
         * touching the calendar aggregation logic.
         */

        var rule = calendarEvent.RecurrenceRule.Trim().ToUpperInvariant();

        if (rule.Contains("FREQ=WEEKLY"))
        {
            return date.DayOfWeek == calendarEvent.StartDate.DayOfWeek;
        }

        if (rule.Contains("FREQ=DAILY"))
        {
            return true;
        }

        if (rule.Contains("FREQ=MONTHLY"))
        {
            return date.Day == calendarEvent.StartDate.Day;
        }

        return false;
    }

    #endregion

    #region Merge

    private static List<TimetableCalendarEventResponse> MergeAndOrder(List<TimetableCalendarEventResponse> timetableEvents, List<TimetableCalendarEventResponse> calendarEvents)
    {
        return timetableEvents
            .Concat(calendarEvents)
            .OrderBy(x => x.Date)
            .ThenBy(x =>
                x.IsFullDay
                    ? TimeOnly.MinValue
                    : x.StartTime ?? TimeOnly.MinValue)
            .ThenByDescending(x => x.Priority)
            .ThenBy(x => x.Title)
            .ToList();
    }

    #endregion

    #region Session

    private async Task<Guid> ResolveAcademicSessionAsync(Guid? requestedSessionId, Guid campusId, DateOnly startDate, DateOnly endDate)
    {
        if (requestedSessionId.HasValue)
        {
            var exists = await _dbContext.AcademicSessions
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.Id == requestedSessionId.Value &&
                        x.CampusId == campusId);

            if (!exists)
            {
                throw new NotFoundException(ErrorCodes.AcademicSessionNotFound, "Academic session not found.");
            }

            return requestedSessionId.Value;
        }

        var session = await _dbContext.AcademicSessions
                .AsNoTracking()
                .Where(x =>
                    x.CampusId == campusId &&

                    x.StartDate <= endDate &&
                    x.EndDate >= startDate)
                .OrderByDescending(x => x.IsActive)
                .ThenByDescending(x => x.StartDate)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

        if (session == Guid.Empty)
        {
            throw new NotFoundException(ErrorCodes.AcademicSessionNotFound, "No academic session was found for the requested calendar range.");
        }

        return session;
    }

    #endregion

    #region Validation

    private static void ValidateRequest(TimetableCalendarRequest request)
    {
        if (request.StartDate > request.EndDate)
        {
            throw new BusinessRuleException(ErrorCodes.CalendarEventInvalidDateRange, "Calendar start date cannot be after end date.");
        }

        var range = request.EndDate.DayNumber - request.StartDate.DayNumber + 1;

        /*
         * Protect the API from somebody accidentally
         * requesting years of occurrences.
         *
         * Monthly and weekly calendar views never need
         * more than 62 days in one request.
         */
        if (range > 62)
        {
            throw new BusinessRuleException(ErrorCodes.CalendarEventInvalidTimeRange, "Calendar range cannot exceed 62 days.");
        }
    }

    private void ValidateCampusScope(Guid campusId)
    {
        if (_scope.IsCampusAdmin() && campusId != _scope.CampusId())
        {
            throw new UnauthorizedAccessException("Access denied.");
        }

        if (_scope.IsInstitutionAdmin() && campusId != _scope.CampusId())
        {
            /*
             * Institution administrators may span campuses.
             *
             * Therefore no campus equality check is required
             * here for institution admins.
             */
        }
    }

    #endregion

    #region Helpers

    private static bool MatchesDay(DateOnly date, DayOfWeekType dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeekType.Monday => date.DayOfWeek == DayOfWeek.Monday,

            DayOfWeekType.Tuesday => date.DayOfWeek == DayOfWeek.Tuesday,

            DayOfWeekType.Wednesday => date.DayOfWeek == DayOfWeek.Wednesday,

            DayOfWeekType.Thursday => date.DayOfWeek == DayOfWeek.Thursday,

            DayOfWeekType.Friday => date.DayOfWeek == DayOfWeek.Friday,

            DayOfWeekType.Saturday => date.DayOfWeek == DayOfWeek.Saturday,

            DayOfWeekType.Sunday => date.DayOfWeek == DayOfWeek.Sunday,

            _ => false
        };
    }

    private static DateOnly Max(DateOnly left, DateOnly right)
    {
        return left > right ? left : right;
    }

    private static DateOnly Min(DateOnly left, DateOnly right)
    {
        return left < right ? left : right;
    }

    private static string GetTimetableColor(LectureType lectureType)
    {
        return lectureType switch
        {
            LectureType.Practical => "#8b5cf6",

            LectureType.Laboratory => "#7c3aed",

            LectureType.Tutorial => "#06b6d4",

            LectureType.Seminar => "#3b82f6",

            LectureType.Workshop => "#f97316",

            LectureType.Project => "#14b8a6",

            LectureType.Viva => "#ec4899",

            LectureType.ExtraClass => "#22c55e",

            LectureType.Revision => "#eab308",

            LectureType.GuestLecture => "#6366f1",

            _ => "#64748b"
        };
    }

    private static Guid CreateOccurrenceId(Guid templateId, DateOnly date)
    {
        return CreateDeterministicGuid($"{templateId:N}:{date:yyyy-MM-dd}");
    }

    private static Guid CreateCalendarOccurrenceId(Guid calendarEventId, DateOnly date)
    {
        return CreateDeterministicGuid($"{calendarEventId:N}:{date:yyyy-MM-dd}");
    }

    private static Guid CreateDeterministicGuid(string value)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();

        var hash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(value));

        var bytes = new byte[16];

        Array.Copy(hash, bytes, 16);

        return new Guid(bytes);
    }

    #endregion

    #region Projection Models

    private sealed class TimetableTemplateCalendarProjection
    {
        public Guid Id { get; set; }

        public Guid InstitutionId { get; set; }

        public Guid CampusId { get; set; }

        public Guid AcademicSessionId { get; set; }

        public Guid TeacherId { get; set; }

        public string TeacherName { get; set; } = string.Empty;

        public Guid SectionId { get; set; }

        public string SectionName { get; set; } = string.Empty;

        public string? SubjectCode { get; set; }

        public string? SubjectName { get; set; }

        public Guid? RoomId { get; set; }

        public string? RoomBuilding { get; set; }

        public string? RoomFloor { get; set; }

        public string? RoomNumber { get; set; }

        public string? RoomName { get; set; }

        public DayOfWeekType DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public DateOnly ValidFrom { get; set; }

        public DateOnly ValidTo { get; set; }

        public LectureType LectureType { get; set; }

        public int Priority { get; set; }

        public bool GenerateAttendance { get; set; }

        public bool IsOnline { get; set; }

        public string? MeetingLink { get; set; }

        public string? Remarks { get; set; }
    }

    private sealed class CalendarEventCalendarProjection
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public EventType EventType { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        public bool IsFullDay { get; set; }

        public bool IsRecurring { get; set; }

        public string? RecurrenceRule { get; set; }

        public int Priority { get; set; }

        public bool AffectsTimetable { get; set; }

        public string? Color { get; set; }

        public Guid? TeacherId { get; set; }

        public Guid? SectionId { get; set; }

        public Guid? DepartmentId { get; set; }

        public Guid? CourseId { get; set; }

        public Guid? SemesterId { get; set; }

        public Guid? RoomId { get; set; }

        public string? RoomBuilding { get; set; }

        public string? RoomFloor { get; set; }

        public string? RoomNumber { get; set; }

        public string? RoomName { get; set; }
    }

    #endregion

    #region Lecture Overrides

    private async Task<List<LectureOverrideCalendarProjection>> GetLectureOverridesAsync(Guid institutionId, Guid campusId, Guid academicSessionId, DateOnly startDate, DateOnly endDate)
    {
        return await _dbContext.LectureOverrides
            .AsNoTracking()
            .Where(x =>
                x.InstitutionId == institutionId &&
                x.CampusId == campusId &&
                x.AcademicSessionId == academicSessionId &&
                x.OverrideDate >= startDate &&
                x.OverrideDate <= endDate &&
                x.IsApproved)
            .Select(x => new LectureOverrideCalendarProjection
            {
                Id = x.Id,

                TimetableTemplateId = x.TimetableTemplateId,

                CalendarEventId = x.CalendarEventId,

                OverrideDate = x.OverrideDate,

                OverrideType = x.OverrideType,

                Reason = x.Reason,

                Remarks = x.Remarks,

                OriginalTeacherId = x.OriginalTeacherId,

                OriginalRoomId = x.OriginalRoomId,

                OriginalStartTime = x.OriginalStartTime,

                OriginalEndTime = x.OriginalEndTime,

                NewTeacherId = x.NewTeacherId,

                NewRoomId = x.NewRoomId,

                NewStartTime = x.NewStartTime,

                NewEndTime = x.NewEndTime,

                GenerateAttendance = x.GenerateAttendance
            })
            .ToListAsync();
    }

    #endregion

    private async Task<List<TimetableCalendarEventResponse>> ApplyLectureOverrides(List<TimetableCalendarEventResponse> timetableEvents, List<LectureOverrideCalendarProjection> overrides)
    {
        if (overrides.Count == 0)
        {
            return timetableEvents;
        }

        var teacherNames = await GetTeacherNamesAsync(overrides.Where(x => x.NewTeacherId.HasValue).Select(x => x.NewTeacherId!.Value));

        var roomDetails = await GetRoomDetailsAsync(overrides.Where(x => x.NewRoomId.HasValue).Select(x => x.NewRoomId));

        var result = new List<TimetableCalendarEventResponse>(timetableEvents);

        foreach (var overrideItem in overrides)
        {
            var affectedEvents = result
                .Where(x =>
                    x.TimetableTemplateId == overrideItem.TimetableTemplateId &&

                    x.Date == overrideItem.OverrideDate)
                .ToList();

            foreach (var calendarEvent in affectedEvents)
            {
                switch (overrideItem.OverrideType)
                {
                    case OverrideType.Cancelled:

                        calendarEvent.IsCancelled = true;

                        calendarEvent.IsOverride = true;

                        calendarEvent.OverrideReason = overrideItem.Reason;

                        break;

                    case OverrideType.TimeChanged:

                        if (overrideItem.NewStartTime.HasValue)
                        {
                            calendarEvent.StartTime = overrideItem.NewStartTime.Value;
                        }

                        if (overrideItem.NewEndTime.HasValue)
                        {
                            calendarEvent.EndTime = overrideItem.NewEndTime.Value;
                        }

                        calendarEvent.IsOverride = true;

                        calendarEvent.OverrideReason = overrideItem.Reason;

                        break;

                    case OverrideType.TeacherChanged:

                        if (overrideItem.NewTeacherId.HasValue)
                        {
                            calendarEvent.TeacherId = overrideItem.NewTeacherId.Value;

                            if (teacherNames.TryGetValue(overrideItem.NewTeacherId.Value, out var newTeacherName))
                            {
                                calendarEvent.TeacherName = newTeacherName;
                            }
                        }

                        calendarEvent.IsOverride = true;

                        calendarEvent.OverrideReason = overrideItem.Reason;

                        break;

                    case OverrideType.RoomChanged:

                        if (overrideItem.NewRoomId.HasValue)
                        {
                            var newRoomId = overrideItem.NewRoomId.Value;

                            calendarEvent.RoomId = newRoomId;

                            if (roomDetails.TryGetValue(newRoomId, out var newRoom))
                            {
                                calendarEvent.RoomBuilding = newRoom.Building;

                                calendarEvent.RoomFloor = newRoom.Floor;

                                calendarEvent.RoomNumber = newRoom.RoomNumber;

                                calendarEvent.RoomName = newRoom.RoomName;
                            }
                        }

                        calendarEvent.IsOverride = true;

                        calendarEvent.OverrideReason = overrideItem.Reason;

                        break;

                    case OverrideType.Rescheduled:

                        if (overrideItem.NewStartTime.HasValue)
                        {
                            calendarEvent.StartTime = overrideItem.NewStartTime.Value;
                        }

                        if (overrideItem.NewEndTime.HasValue)
                        {
                            calendarEvent.EndTime = overrideItem.NewEndTime.Value;
                        }

                        if (overrideItem.NewTeacherId.HasValue)
                        {
                            calendarEvent.TeacherId = overrideItem.NewTeacherId.Value;

                            if (teacherNames.TryGetValue(overrideItem.NewTeacherId.Value, out var rescheduledTeacherName))
                            {
                                calendarEvent.TeacherName = rescheduledTeacherName;
                            }
                        }

                        if (overrideItem.NewRoomId.HasValue)
                        {
                            var newRoomId = overrideItem.NewRoomId.Value;

                            calendarEvent.RoomId = newRoomId;

                            if (roomDetails.TryGetValue(newRoomId, out var newRoom))
                            {
                                calendarEvent.RoomBuilding = newRoom.Building;

                                calendarEvent.RoomFloor = newRoom.Floor;

                                calendarEvent.RoomNumber = newRoom.RoomNumber;

                                calendarEvent.RoomName = newRoom.RoomName;
                            }
                        }

                        calendarEvent.IsOverride = true;

                        calendarEvent.OverrideReason = overrideItem.Reason;

                        break;
                }
            }
        }

        return result;
    }

    private async Task<Dictionary<Guid, string>> GetTeacherNamesAsync(IEnumerable<Guid> teacherIds)
    {
        var ids = teacherIds.Distinct().ToList();

        if (ids.Count == 0)
        {
            return new Dictionary<Guid, string>();
        }

        return await _dbContext.Teachers
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .Select(x => new
            {
                x.Id,

                Name = x.User.FirstName + " " + x.User.LastName
            })
            .ToDictionaryAsync(x => x.Id, x => x.Name);
    }

    private async Task<Dictionary<Guid, RoomCalendarDetails>> GetRoomDetailsAsync(IEnumerable<Guid?> roomIds)
    {
        var ids = roomIds.Where(x => x.HasValue).Select(x => x.Value).Distinct().ToList();

        if (ids.Count == 0)
        {
            return new Dictionary<Guid, RoomCalendarDetails>();
        }

        return await _dbContext.Rooms
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .Select(x => new RoomCalendarDetails
            {
                Id = x.Id,

                Building = x.Building,

                Floor = x.Floor,

                RoomNumber = x.RoomNumber,

                RoomName = x.RoomName
            })
            .ToDictionaryAsync(x => x.Id);
    }

    private sealed class RoomCalendarDetails
    {
        public Guid Id { get; set; }

        public string? Building { get; set; }

        public string? Floor { get; set; }

        public string? RoomNumber { get; set; }

        public string? RoomName { get; set; }
    }

    private sealed class LectureOverrideCalendarProjection
    {
        public Guid Id { get; set; }

        public Guid? TimetableTemplateId { get; set; }

        public Guid? CalendarEventId { get; set; }

        public DateOnly OverrideDate { get; set; }

        public OverrideType OverrideType { get; set; }

        public string? Reason { get; set; }

        public string? Remarks { get; set; }

        public Guid? OriginalTeacherId { get; set; }

        public Guid? OriginalRoomId { get; set; }

        public TimeOnly? OriginalStartTime { get; set; }

        public TimeOnly? OriginalEndTime { get; set; }

        public Guid? NewTeacherId { get; set; }

        public Guid? NewRoomId { get; set; }

        public TimeOnly? NewStartTime { get; set; }

        public TimeOnly? NewEndTime { get; set; }

        public bool GenerateAttendance { get; set; }
    }
}