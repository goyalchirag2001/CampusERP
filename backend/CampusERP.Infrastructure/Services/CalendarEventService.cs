using CampusERP.Application.Common.Exceptions;
using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Common;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using CampusERP.Infrastructure.Mappers;
using CampusERP.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CampusERP.Infrastructure.Services;

public class CalendarEventService : ICalendarEventService
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IDataAccessScope _scope;

    private readonly ISchedulingEngineService _schedulingEngine;

    public CalendarEventService(ApplicationDbContext dbContext, IDataAccessScope scope, ISchedulingEngineService schedulingEngine)
    {
        _dbContext = dbContext;
        _scope = scope;
        _schedulingEngine = schedulingEngine;
    }

    public async Task<List<CalendarEventResponse>> GetAllAsync()
    {
        return await ApplyScope(_dbContext.CalendarEvents)
            .AsNoTracking()
            .Include(x => x.Campus)
            .Include(x => x.AcademicSession)
            .Include(x => x.Department)
            .Include(x => x.Course)
            .Include(x => x.Semester)
            .Include(x => x.Section)
            .Include(x => x.Teacher)
            .ThenInclude(x => x.User)
            .Include(x => x.Room)
            .OrderByDescending(x => x.StartDate)
            .ThenBy(x => x.StartTime)
            .Select(MapToResponse())
            .ToListAsync();
    }

    public async Task<CalendarEventResponse?> GetByIdAsync(Guid id)
    {
        return await ApplyScope(_dbContext.CalendarEvents)
            .AsNoTracking()
            .Include(x => x.Campus)
            .Include(x => x.AcademicSession)
            .Include(x => x.Department)
            .Include(x => x.Course)
            .Include(x => x.Semester)
            .Include(x => x.Section)
            .Include(x => x.Teacher)
            .ThenInclude(x => x.User)
            .Include(x => x.Room)
            .Where(x => x.Id == id)
            .Select(MapToResponse())
            .FirstOrDefaultAsync();
    }

    public async Task<CalendarEventResponse> CreateAsync(CreateCalendarEventRequest request)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            await ValidateRequestAsync(request);

            var calendarEvent = BuildCalendarEvent(request);

            await ValidateScheduleAsync(calendarEvent);

            _dbContext.CalendarEvents.Add(calendarEvent);

            await _dbContext.SaveChangesAsync();

            if (calendarEvent.AffectsTimetable)
            {
                await _schedulingEngine.GenerateLectureOverridesAsync(calendarEvent.Id);
            }

            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            return await GetByIdAsync(calendarEvent.Id) ?? throw new NotFoundException(ErrorCodes.CalendarEventNotFound,"Unable to load created event.");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<CalendarEventResponse> UpdateAsync(Guid id, UpdateCalendarEventRequest request)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            await ValidateRequestAsync(request);

            var calendarEvent = await ApplyScope(_dbContext.CalendarEvents)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (calendarEvent == null)
            {
                throw new NotFoundException(ErrorCodes.CalendarEventNotFound, "Calendar event not found.");
            }

            ApplyChanges(calendarEvent, request);

            await ValidateScheduleAsync(calendarEvent);

            await _dbContext.SaveChangesAsync();

            await _schedulingEngine.RemoveLectureOverridesAsync(id);

            if (calendarEvent.AffectsTimetable)
            {
                await _schedulingEngine.GenerateLectureOverridesAsync(id);
            }

            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            return await GetByIdAsync(id) ?? throw new NotFoundException(ErrorCodes.CalendarEventNotFound, "Unable to load updated event.");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task ActivateAsync(Guid id)
    {
        var calendarEvent = await ApplyScope(_dbContext.CalendarEvents)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (calendarEvent == null)
        {
            throw new NotFoundException(ErrorCodes.CalendarEventNotFound, "Calendar event not found.");
        }

        if (calendarEvent.IsActive)
        {
            return;
        }

        calendarEvent.IsActive = true;

        await ValidateScheduleAsync(calendarEvent);

        await _dbContext.SaveChangesAsync();

        if (calendarEvent.AffectsTimetable)
        {
            await _schedulingEngine.GenerateLectureOverridesAsync(id);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeactivateAsync(Guid id)
    {
        var calendarEvent = await ApplyScope(_dbContext.CalendarEvents)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (calendarEvent == null)
        {
            throw new NotFoundException(ErrorCodes.CalendarEventNotFound, "Calendar event not found.");
        }

        if (!calendarEvent.IsActive)
        {
            return;
        }

        calendarEvent.IsActive = false;

        await _dbContext.SaveChangesAsync();

        await _schedulingEngine.RemoveLectureOverridesAsync(id);

        await _dbContext.SaveChangesAsync();
    }

    private CalendarEvent BuildCalendarEvent(CreateCalendarEventRequest request)
    {
        return new CalendarEvent
        {
            Id = Guid.NewGuid(),

            InstitutionId = _scope.InstitutionId(),

            CampusId =
                request.CampusId ??
                _scope.CampusId(),

            AcademicSessionId =
                request.AcademicSessionId,

            DepartmentId =
                request.DepartmentId,

            CourseId =
                request.CourseId,

            SemesterId =
                request.SemesterId,

            SectionId =
                request.SectionId,

            TeacherId =
                request.TeacherId,

            RoomId =
                request.RoomId,

            Title =
                request.Title.Trim(),

            Description =
                string.IsNullOrWhiteSpace(request.Description)
                    ? null
                    : request.Description.Trim(),

            EventType =
                request.EventType,

            StartDate =
                request.StartDate,

            EndDate =
                request.EndDate,

            StartTime =
                request.StartTime,

            EndTime =
                request.EndTime,

            IsFullDay =
                request.IsFullDay,

            IsRecurring =
                request.IsRecurring,

            RecurrenceRule =
                request.RecurrenceRule,

            Priority =
                request.Priority,

            AffectsTimetable =
                request.AffectsTimetable,

            Color = GetDefaultColor(request.EventType),

            IsActive = true
        };
    }

    private static void ApplyChanges(CalendarEvent entity, UpdateCalendarEventRequest request)
    {

        entity.AcademicSessionId = request.AcademicSessionId;

        entity.DepartmentId = request.DepartmentId;

        entity.CourseId = request.CourseId;

        entity.SemesterId = request.SemesterId;

        entity.SectionId = request.SectionId;

        entity.TeacherId = request.TeacherId;

        entity.RoomId = request.RoomId;

        entity.Title = request.Title.Trim();

        entity.Description =
            string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim();

        entity.EventType = request.EventType;

        entity.StartDate = request.StartDate;

        entity.EndDate = request.EndDate;

        entity.StartTime = request.StartTime;

        entity.EndTime = request.EndTime;

        entity.IsFullDay = request.IsFullDay;

        entity.IsRecurring = request.IsRecurring;

        entity.RecurrenceRule = request.RecurrenceRule;

        entity.Priority = request.Priority;

        entity.AffectsTimetable = request.AffectsTimetable;

        entity.Color = GetDefaultColor(request.EventType);
    }

    private async Task ValidateScheduleAsync(CalendarEvent calendarEvent)
    {
        var validation = await _schedulingEngine
                .ValidateCalendarEventAsync(
                    ScheduleValidationMapper.FromCalendarEvent(calendarEvent));

        if (validation.IsValid)
        {
            return;
        }

        var blockingConflicts = validation.Conflicts
                .Where(x => !x.CanOverride)
                .ToList();

        if (blockingConflicts.Count == 0)
        {
            return;
        }

        throw new BusinessRuleException(ErrorCodes.CalendarScheduleConflict,string.Join(Environment.NewLine,blockingConflicts.Select(x => x.Message)));
    }

    private static Expression<Func<CalendarEvent, CalendarEventResponse>> MapToResponse()
    {
        return x => new CalendarEventResponse
        {
            Id = x.Id,

            InstitutionId = x.InstitutionId,

            CampusId = x.CampusId,

            CampusName = x.Campus.Name,

            DepartmentId = x.DepartmentId,

            DepartmentName = x.Department != null
                ? x.Department.Name
                : null,

            CourseId = x.CourseId,

            CourseName = x.Course != null
                ? x.Course.Name
                : null,

            SemesterId = x.SemesterId,

            SemesterName = x.Semester != null
                ? x.Semester.Name
                : null,

            SectionId = x.SectionId,

            SectionName = x.Section != null
                ? x.Section.Name
                : null,

            TeacherId = x.TeacherId,

            TeacherName = x.Teacher != null ? x.Teacher.User.FirstName + " " + x.Teacher.User.LastName : null,

            RoomId = x.RoomId,

            RoomName = x.Room != null ? $"{x.Room.Building} - {x.Room.RoomNumber} ({x.Room.RoomName})" : null,

            AcademicSessionId = x.AcademicSessionId,

            AcademicSessionName = x.AcademicSession.Name,

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

            IsActive = x.IsActive,
        };
    }

    private IQueryable<CalendarEvent> ApplyScope(IQueryable<CalendarEvent> query)
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

    private async Task ValidateRequestAsync(ICalendarEventRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ValidationException(ErrorCodes.CalendarEventTitleRequired, "Event title is required.");
        }

        if (request.StartDate > request.EndDate)
        {
            throw new ValidationException(ErrorCodes.CalendarEventInvalidDateRange, "Start date cannot be after End Date.");
        }

        if (!request.IsFullDay)
        {
            if (!request.StartTime.HasValue)
                throw new ValidationException(ErrorCodes.CalendarEventStartTimeRequired, "Start time is required.");

            if (!request.EndTime.HasValue)
                throw new ValidationException(ErrorCodes.CalendarEventEndTimeRequired, "End time is required.");

            if (request.StartTime >= request.EndTime)
                throw new ValidationException(ErrorCodes.CalendarEventInvalidTimeRange, "End time must be after Start time.");
        }

        if (request.Priority < 0)
        {
            throw new ValidationException(ErrorCodes.CalendarEventInvalidPriority, "Priority cannot be negative.");
        }

        if (request.IsRecurring && string.IsNullOrWhiteSpace(request.RecurrenceRule))
        {
            throw new ValidationException(ErrorCodes.CalendarEventRecurrenceRuleRequired, "Recurrence rule is required.");
        }

        var session = await _dbContext.AcademicSessions.FirstOrDefaultAsync(x => x.Id == request.AcademicSessionId);

        if (session == null)
        {
            throw new NotFoundException(ErrorCodes.AcademicSessionNotFound, "Academic Session not found.");
        }

        if (request.StartDate < session.StartDate ||
            request.EndDate > session.EndDate)
        {
            throw new BusinessRuleException(ErrorCodes.CalendarEventOutsideAcademicSession, "Event must lie within Academic Session.");
        }

        await ValidateHierarchyAsync(request);
    }

    private async Task ValidateHierarchyAsync(ICalendarEventRequest request)
    {
        var campusId = request.CampusId ?? _scope.CampusId();

        if (!await _dbContext.Campuses.AnyAsync(x => x.Id == campusId))
            throw new NotFoundException(ErrorCodes.CampusNotFound, "Campus not found.");

        var session = await _dbContext.AcademicSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.AcademicSessionId);

        if (session == null)
            throw new NotFoundException(
                ErrorCodes.AcademicSessionNotFound,
                "Academic Session not found.");

        if (session.CampusId != campusId)
            throw new BusinessRuleException(
                ErrorCodes.InvalidAcademicSession,
                "Academic Session does not belong to the selected Campus.");

        if (request.DepartmentId.HasValue)
        {
            var department = await _dbContext.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.DepartmentId);

            if (department == null)
                throw new NotFoundException(
                    ErrorCodes.DepartmentNotFound,
                    "Department not found.");

            if (department.CampusId != campusId)
                throw new BusinessRuleException(
                    ErrorCodes.InvalidDepartment,
                    "Department does not belong to selected Campus.");
        }

        if (request.CourseId.HasValue)
        {
            var course = await _dbContext.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.CourseId);

            if (course == null)
                throw new NotFoundException(
                    ErrorCodes.CourseNotFound,
                    "Course not found.");

            if (request.DepartmentId.HasValue &&
                course.DepartmentId != request.DepartmentId)
            {
                throw new BusinessRuleException(
                    ErrorCodes.InvalidCourse,
                    "Course does not belong to selected Department.");
            }
        }

        if (request.SemesterId.HasValue)
        {
            var semester = await _dbContext.Semesters
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.SemesterId);

            if (semester == null)
                throw new NotFoundException(
                    ErrorCodes.SemesterNotFound,
                    "Semester not found.");

            if (request.CourseId.HasValue &&
                semester.CourseId != request.CourseId)
            {
                throw new BusinessRuleException(
                    ErrorCodes.InvalidSemester,
                    "Semester does not belong to selected Course.");
            }
        }

        if (request.SectionId.HasValue)
        {
            var section = await _dbContext.Sections
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.SectionId);

            if (section == null)
                throw new NotFoundException(ErrorCodes.SectionNotFound, "Section not found.");

            if (request.SemesterId.HasValue &&
                section.SemesterId != request.SemesterId)
            {
                throw new BusinessRuleException(ErrorCodes.InvalidSection, "Section does not belong to selected Semester.");
            }
        }

        if (request.TeacherId.HasValue)
        {
            var teacher = await _dbContext.Teachers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.TeacherId);

            if (teacher == null)
                throw new NotFoundException(ErrorCodes.TeacherNotFound, "Teacher not found.");

            if (teacher.CampusId != campusId)
            {
                throw new BusinessRuleException(ErrorCodes.InvalidTeacher, "Teacher does not belong to selected Campus.");
            }
        }

        if (request.RoomId.HasValue)
        {
            var room = await _dbContext.Rooms
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.RoomId);

            if (room == null)
                throw new NotFoundException(
                    ErrorCodes.RoomNotFound,
                    "Room not found.");

            if (room.CampusId != campusId)
            {
                throw new BusinessRuleException(
                    ErrorCodes.InvalidRoom,
                    "Room does not belong to selected Campus.");
            }
        }
    }

    private static string GetDefaultColor(EventType eventType)
    {
        return eventType switch
        {
            EventType.Holiday => "#ef4444",

            EventType.Examination => "#f97316",

            EventType.Workshop => "#8b5cf6",

            EventType.Seminar => "#3b82f6",

            EventType.GuestLecture => "#06b6d4",

            EventType.FacultyMeeting => "#6366f1",

            EventType.ParentTeacherMeeting => "#0ea5e9",

            EventType.ExtraClass => "#22c55e",

            EventType.SportsDay => "#84cc16",

            EventType.CulturalEvent => "#ec4899",

            EventType.Convocation => "#a855f7",

            _ => "#64748b"
        };
    }

}