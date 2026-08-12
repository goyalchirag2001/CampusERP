using CampusERP.Application.Common.Exceptions;
using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Common;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using CampusERP.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CampusERP.Infrastructure.Services;

public class TimetableTemplateService : ITimetableTemplateService
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IDataAccessScope _scope;

    private readonly ISchedulingEngineService _schedulingEngine;

    private readonly ICurrentUserService _currentUser;

    public TimetableTemplateService(ApplicationDbContext dbContext, IDataAccessScope scope, ISchedulingEngineService schedulingEngine, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;

        _scope = scope;

        _schedulingEngine = schedulingEngine;

        _currentUser = currentUser;
    }

    #region Queries

    public async Task<List<TimetableTemplateResponse>> GetAllAsync()
    {
        var result = await ApplyScope(_dbContext.TimetableTemplates)
            .AsNoTracking()
            .Select(MapResponse())
            .ToListAsync();

        return result
            .OrderBy(x => x.AcademicSessionName)
            .ThenBy(x => x.SectionName)
            .ThenBy(x => x.DayOfWeek)
            .ThenBy(x => x.DisplayOrder)
            .ThenBy(x => x.StartTime)
            .ToList();
    }

    public async Task<TimetableTemplateResponse> GetByIdAsync(Guid id)
    {
        var timetable = await BuildQuery()
            .Where(x => x.Id == id)
            .Select(MapResponse())
            .FirstOrDefaultAsync();

        if (timetable == null)
        {
            throw new NotFoundException(ErrorCodes.TimetableTemplateNotFound, "Timetable template not found.");
        }

        return timetable;
    }

    public async Task<List<TimetableTemplateResponse>> GetByTeacherAsync(Guid teacherId)
    {
        return await BuildQuery()
            .Where(x => x.TeacherId == teacherId)
            .OrderBy(x => x.DayOfWeek)
            .ThenBy(x => x.DisplayOrder)
            .ThenBy(x => x.StartTime)
            .Select(MapResponse())
            .ToListAsync();
    }

    public async Task<List<TimetableTemplateResponse>> GetBySectionAsync(Guid sectionId)
    {
        return await BuildQuery()
            .Where(x => x.SectionId == sectionId)
            .OrderBy(x => x.DayOfWeek)
            .ThenBy(x => x.DisplayOrder)
            .ThenBy(x => x.StartTime)
            .Select(MapResponse())
            .ToListAsync();
    }

    public async Task<List<TimetableTemplateResponse>> GetWeeklyTimetableAsync(Guid sectionId, Guid academicSessionId)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        return await BuildQuery()
            .Where(x =>
                x.SectionId == sectionId &&
                x.AcademicSessionId == academicSessionId &&
                x.IsActive &&
                x.ValidFrom <= today &&
                x.ValidTo >= today)
            .OrderBy(x => x.DayOfWeek)
            .ThenBy(x => x.DisplayOrder)
            .ThenBy(x => x.StartTime)
            .Select(MapResponse())
            .ToListAsync();
    }

    public async Task<List<TimetableTemplateResponse>> GetByAcademicSessionAsync(Guid academicSessionId)
    {
        return await BuildQuery()
            .Where(x => x.AcademicSessionId == academicSessionId)
            .OrderBy(x => x.Section.Name)
            .ThenBy(x => x.DayOfWeek)
            .ThenBy(x => x.DisplayOrder)
            .ThenBy(x => x.StartTime)
            .Select(MapResponse())
            .ToListAsync();
    }

    #endregion

    #region Query Builder

    private IQueryable<TimetableTemplate> BuildQuery()
    {
        return ApplyScope(_dbContext.TimetableTemplates)
            .AsNoTracking()

            .Include(x => x.Campus)

            .Include(x => x.AcademicSession)

            .Include(x => x.TeacherAssignment)

            .Include(x => x.Teacher)
                .ThenInclude(x => x.User)

            .Include(x => x.Section)

            .Include(x => x.SemesterSubject)
                .ThenInclude(x => x.Subject)

            .Include(x => x.Room);
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

    #endregion

    #region Mapping

    private static Expression<Func<TimetableTemplate, TimetableTemplateResponse>> MapResponse()
    {
        return x => new TimetableTemplateResponse
        {
            Id = x.Id,

            InstitutionId = x.InstitutionId,

            CampusId = x.CampusId,

            CampusName = x.Campus.Name,

            AcademicSessionId = x.AcademicSessionId,

            AcademicSessionName = x.AcademicSession.Name,

            TeacherAssignmentId = x.TeacherAssignmentId,

            TeacherId = x.TeacherId,

            TeacherName =
                x.Teacher.User.FirstName +
                " " +
                x.Teacher.User.LastName,

            SectionId = x.SectionId,

            SectionName = x.Section.Name,

            SemesterSubjectId = x.SemesterSubjectId,

            SubjectId = x.SemesterSubject.SubjectId,

            SubjectCode = x.SemesterSubject.Subject.Code,

            SubjectName = x.SemesterSubject.Subject.Name,

            RoomId = x.RoomId ?? Guid.Empty,

            RoomName = x.Room.Building + " - " +  x.Room.RoomNumber + " (" + x.Room.RoomName + ")",

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

            Remarks = x.Remarks,

            DisplayOrder = x.DisplayOrder,

            IsActive = x.IsActive
        };
    }

    #endregion

    #region Validation

    private async Task ValidateRequestAsync(CreateTimetableTemplateRequest request, Guid? excludeTimetableTemplateId)
    {
        if (request.StartTime >= request.EndTime)
        {
            throw new BadRequestException(ErrorCodes.InvalidLectureTime, "End time must be greater than start time.");
        }

        if (request.ValidFrom > request.ValidTo)
        {
            throw new BadRequestException(ErrorCodes.InvalidLectureValidity, "Valid From cannot be greater than Valid To.");
        }

        await ValidateTeacherAssignmentAsync(request);

        await ValidateRoomAsync(request);

        await ValidateAcademicSessionAsync(request);

        await ValidateDisplayOrderAsync(request);

        await ValidateMeetingLinkAsync(request);

        await ValidateSchedulingEngineAsync(request, excludeTimetableTemplateId);
    }

    private async Task ValidateRequestAsync(UpdateTimetableTemplateRequest request, Guid? excludeTimetableTemplateId)
    {
        if (request.StartTime >= request.EndTime)
        {
            throw new BadRequestException(ErrorCodes.InvalidLectureTime, "End time must be greater than start time.");
        }

        if (request.ValidFrom > request.ValidTo)
        {
            throw new BadRequestException(ErrorCodes.InvalidLectureValidity, "Valid From cannot be greater than Valid To.");
        }

        await ValidateTeacherAssignmentAsync(request);

        await ValidateRoomAsync(request);

        await ValidateAcademicSessionAsync(request);

        await ValidateDisplayOrderAsync(request);

        await ValidateMeetingLinkAsync(request);

        await ValidateSchedulingEngineAsync(request, excludeTimetableTemplateId);
    }
    #endregion

    private async Task ValidateTeacherAssignmentAsync(ITimetableTemplateRequest request)
    {
        var exists = await _dbContext.TeacherAssignments
            .AnyAsync(x =>
                x.Id == request.TeacherAssignmentId);

        if (!exists)
        {
            throw new NotFoundException(ErrorCodes.TeacherAssignmentNotFound, "Teacher assignment not found.");
        }
    }

    private async Task ValidateRoomAsync(ITimetableTemplateRequest request)
    {
        if (request.IsOnline)
        {
            return;
        }

        if (!request.RoomId.HasValue)
        {
            throw new BadRequestException(ErrorCodes.RoomRequired, "Room is required for offline lectures.");
        }

        var institutionId = _currentUser.InstitutionId;

        if (!institutionId.HasValue || institutionId.Value == Guid.Empty)
        {
            throw new BadRequestException(ErrorCodes.Validation, "Institution scope is not available.");
        }

        var room = await _dbContext.Rooms
            .AsNoTracking()
            .Where(x =>
                x.Id == request.RoomId.Value &&
                x.IsActive)
            .Select(x => new
            {
                x.Id,
                x.InstitutionId,
                x.CampusId
            })
            .FirstOrDefaultAsync();

        if (room == null)
        {
            throw new NotFoundException(ErrorCodes.RoomNotFound, "Room not found.");
        }

        if (room.InstitutionId != institutionId.Value)
        {
            throw new BadRequestException(ErrorCodes.Validation, "The selected room does not belong to the current institution.");
        }

        // CampusAdmin must only be able to use rooms
        // belonging to their own campus.
        if (_scope.IsCampusAdmin())
        {
            var campusId = _scope.CampusId();

            if (campusId == Guid.Empty)
            {
                throw new BadRequestException(ErrorCodes.Validation, "Campus scope is not available.");
            }

            if (room.CampusId != campusId)
            {
                throw new BadRequestException(ErrorCodes.Validation, "The selected room does not belong to your campus.");
            }
        }
    }

    private async Task ValidateAcademicSessionAsync(ITimetableTemplateRequest request)
    {
        var session = await _dbContext.AcademicSessions
            .FirstOrDefaultAsync(x =>
                x.Id == request.AcademicSessionId &&
                x.IsActive);

        if (session == null)
        {
            throw new NotFoundException(ErrorCodes.AcademicSessionNotFound, "Academic session not found.");
        }

        if (request.ValidFrom < session.StartDate ||
            request.ValidTo > session.EndDate)
        {
            throw new BadRequestException(ErrorCodes.InvalidLectureValidity, "Lecture validity must fall inside the academic session.");
        }
    }

    private static Task ValidateDisplayOrderAsync(ITimetableTemplateRequest request)
    {
        if (request.DisplayOrder <= 0)
        {
            throw new BadRequestException(ErrorCodes.InvalidLectureTime, "Display order must be greater than zero.");
        }

        return Task.CompletedTask;
    }

    private static Task ValidateMeetingLinkAsync(ITimetableTemplateRequest request)
    {
        if (!request.IsOnline)
        {
            return Task.CompletedTask;
        }

        if (string.IsNullOrWhiteSpace(request.MeetingLink))
        {
            throw new BadRequestException(ErrorCodes.MissingMeetingLink, "Meeting link is required for online lectures.");
        }

        return Task.CompletedTask;
    }

    private async Task ValidateSchedulingEngineAsync(ITimetableTemplateRequest request, Guid? excludeTimetableTemplateId)
    {
        var assignment = await _dbContext.TeacherAssignments
                .AsNoTracking()
                .FirstAsync(x =>
                    x.Id == request.TeacherAssignmentId);

        var validation = await _schedulingEngine.ValidateTimetableAsync(new ScheduleValidationRequest
                {
                    AcademicSessionId = request.AcademicSessionId,

                    TeacherId = assignment.TeacherId,

                    RoomId = request.RoomId,

                    SectionId = assignment.SectionId,

                    StartDate = request.ValidFrom,

                    EndDate = request.ValidTo,

                    StartTime = request.StartTime,

                    EndTime = request.EndTime,

                    IsFullDay = false,

                    Priority = request.Priority,

                    AffectsTimetable = true,

                    DayOfWeek = request.DayOfWeek,

                    /*
                     * NULL for create.
                     *
                     * Existing timetable ID for update.
                     */
                    ExcludeTimetableTemplateId = excludeTimetableTemplateId
                });

        if (validation.IsValid)
        {
            return;
        }

        var messages = validation.Conflicts
                .Select(x => x.Message)
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

        throw new BadRequestException(ErrorCodes.Validation, string.Join(Environment.NewLine, messages));
    }

    #region Commands

    public async Task<TimetableTemplateResponse> CreateAsync(CreateTimetableTemplateRequest request)
    {
        await ValidateRequestAsync(request, null);

        var institutionId = _currentUser.InstitutionId;

        if (!institutionId.HasValue || institutionId.Value == Guid.Empty)
        {
            throw new BadRequestException(ErrorCodes.Validation, "Institution scope is not available.");
        }

        Guid campusId;

        if (request.RoomId.HasValue)
        {
            var room = await _dbContext.Rooms
                    .AsNoTracking()
                    .Where(x =>
                        x.Id == request.RoomId.Value &&
                        x.IsActive)
                    .Select(x => new
                    {
                        x.Id,
                        x.CampusId,
                        x.InstitutionId
                    })
                    .FirstOrDefaultAsync();

            if (room == null)
            {
                throw new NotFoundException(ErrorCodes.RoomNotFound, "Room not found.");
            }

            if (room.InstitutionId != institutionId.Value)
            {
                throw new BadRequestException(ErrorCodes.Validation, "The selected room does not belong to the current institution.");
            }

            campusId = room.CampusId;
        }
        else
        {
            var currentCampusId = _currentUser.CampusId;

            if (!currentCampusId.HasValue || currentCampusId.Value == Guid.Empty)
            {
                throw new BadRequestException(ErrorCodes.Validation, "Campus scope is required when creating an online timetable.");
            }

            campusId = currentCampusId.Value;
        }

        var deletedEntity = await ApplyScope(_dbContext.TimetableTemplates
                            .IgnoreQueryFilters())
                               .FirstOrDefaultAsync(x =>
                                   x.InstitutionId == institutionId.Value &&
                                   x.AcademicSessionId == request.AcademicSessionId &&
                                   x.TeacherAssignmentId == request.TeacherAssignmentId &&
                                   x.DayOfWeek == request.DayOfWeek &&
                                   x.StartTime == request.StartTime &&
                                   x.EndTime == request.EndTime &&
                                   x.IsDeleted);

        if (deletedEntity != null)
        {
            // =====================================================
            // 5. Restore the previously deleted timetable
            // =====================================================

            deletedEntity.IsDeleted = false;
            deletedEntity.IsActive = true;

            deletedEntity.InstitutionId = institutionId.Value;
            deletedEntity.CampusId = campusId;

            await ApplyRequestAsync(deletedEntity, request);

            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(deletedEntity.Id) ?? throw new NotFoundException(ErrorCodes.TimetableTemplateNotFound, "Timetable template could not be restored.");
        }


        var entity = new TimetableTemplate
            {
                Id = Guid.NewGuid(),

                InstitutionId = institutionId.Value,

                CampusId = campusId,

                IsActive = true
            };

        await ApplyRequestAsync(entity, request);

        _dbContext.TimetableTemplates.Add(entity);

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(entity.Id) ?? throw new NotFoundException(ErrorCodes.TimetableTemplateNotFound, "Timetable template not found.");
    }
    
    #endregion

    public async Task<TimetableTemplateResponse> UpdateAsync(Guid id, UpdateTimetableTemplateRequest request)
    {

        var entity = await ApplyScope(_dbContext.TimetableTemplates).FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
        {
            throw new NotFoundException(ErrorCodes.TimetableTemplateNotFound, "Timetable template not found.");
        }

        /*
         * Pass the existing timetable ID into validation.
         *
         * The scheduling engine will exclude this record from
         * conflict detection.
         */
        await ValidateRequestAsync(request, id);

        await ApplyRequestAsync(entity, request);

        /*
         * If the room changes, the campus must follow the room.
         */
        if (request.RoomId.HasValue)
        {
            var room = await _dbContext.Rooms
                    .AsNoTracking()
                    .Where(x =>
                        x.Id == request.RoomId.Value &&
                        x.IsActive)
                    .Select(x => new
                    {
                        x.CampusId,
                        x.InstitutionId
                    })
                    .FirstOrDefaultAsync();

            if (room == null)
            {
                throw new NotFoundException(ErrorCodes.RoomNotFound, "Room not found.");
            }

            entity.CampusId = room.CampusId;
        }
        else
        {
            var currentCampusId = _currentUser.CampusId;

            if (!currentCampusId.HasValue || currentCampusId.Value == Guid.Empty)
            {
                throw new BadRequestException(ErrorCodes.Validation, "Campus scope is required for an online timetable.");
            }

            entity.CampusId = currentCampusId.Value;
        }

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(id) ?? throw new NotFoundException(ErrorCodes.TimetableTemplateNotFound, "Timetable template not found.");
    }

    private async Task ApplyRequestAsync(TimetableTemplate entity, ITimetableTemplateRequest request)
    {
        var assignment = await _dbContext.TeacherAssignments
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == request.TeacherAssignmentId);

        if (assignment == null)
        {
            throw new NotFoundException(ErrorCodes.TeacherAssignmentNotFound, "Teacher assignment not found.");
        }

        if (assignment.AcademicSessionId != request.AcademicSessionId)
        {
            throw new BadRequestException(ErrorCodes.InvalidTeacherAssignment, "The selected teacher assignment does not belong to the selected academic session.");
        }

        entity.AcademicSessionId = request.AcademicSessionId;

        entity.TeacherAssignmentId = assignment.Id;

        entity.TeacherId = assignment.TeacherId;

        entity.SectionId = assignment.SectionId;

        entity.SemesterSubjectId = assignment.SemesterSubjectId;

        entity.RoomId = request.RoomId;

        entity.DayOfWeek = request.DayOfWeek;

        entity.StartTime = request.StartTime;

        entity.EndTime = request.EndTime;

        entity.ValidFrom = request.ValidFrom;

        entity.ValidTo = request.ValidTo;

        entity.LectureType = request.LectureType;

        entity.Priority = request.Priority;

        entity.GenerateAttendance = request.GenerateAttendance;

        entity.IsOnline = request.IsOnline;

        entity.MeetingLink = request.MeetingLink;

        entity.Remarks = request.Remarks;

        entity.DisplayOrder = request.DisplayOrder;
    }

    public async Task ActivateAsync(Guid id)
    {
        var entity = await _dbContext.TimetableTemplates
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
        {
            throw new NotFoundException(ErrorCodes.TimetableTemplateNotFound, "Timetable template not found.");
        }

        if (entity.IsActive)
        {
            return;
        }

        entity.IsActive = true;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeactivateAsync(Guid id)
    {
        var entity = await _dbContext.TimetableTemplates
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
        {
            throw new NotFoundException(ErrorCodes.TimetableTemplateNotFound, "Timetable template not found.");
        }

        if (!entity.IsActive)
        {
            return;
        }

        entity.IsActive = false;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _dbContext.TimetableTemplates
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
        {
            throw new NotFoundException(ErrorCodes.TimetableTemplateNotFound, "Timetable template not found.");
        }

        var attendanceExists = await _dbContext.AttendanceSessions.AnyAsync(x => x.TimetableTemplateId == id);

        if (attendanceExists)
        {
            throw new BadRequestException(ErrorCodes.TimetableTemplateHasAttendance, "This timetable template cannot be deleted because attendance has already been generated.");
        }

        var overrideExists = await _dbContext.LectureOverrides.AnyAsync(x => x.TimetableTemplateId == id);

        if (overrideExists)
        {
            throw new BadRequestException(ErrorCodes.TimetableTemplateHasOverrides, "This timetable template cannot be deleted because lecture overrides already exist.");
        }

        _dbContext.TimetableTemplates.Remove(entity);

        await _dbContext.SaveChangesAsync();
    }
}