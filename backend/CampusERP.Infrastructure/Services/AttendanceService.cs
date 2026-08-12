using CampusERP.Application.Interfaces;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using CampusERP.Shared.Constants;
using CampusERP.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class AttendanceService : IAttendanceService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public AttendanceService(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    // =========================================================
    // Create Attendance Session
    // =========================================================

    public async Task<AttendanceSessionResponse> CreateSessionAsync(CreateAttendanceSessionRequest request)
    {
        var institutionId = GetRequiredInstitutionId();
        var campusId = GetRequiredCampusId();

        /*
         * -----------------------------------------------------
         * Load timetable template
         * -----------------------------------------------------
         */

        var timetable = await _dbContext.TimetableTemplates
            .AsNoTracking()
            .Include(x => x.TeacherAssignment)
            .Include(x => x.SemesterSubject)
                .ThenInclude(x => x.Subject)
            .Include(x => x.Teacher)
            .Include(x => x.Section)
            .Include(x => x.Room)
            .FirstOrDefaultAsync(x =>
                x.Id == request.TimetableTemplateId &&
                x.InstitutionId == institutionId &&
                x.CampusId == campusId &&
                x.IsActive);

        if (timetable == null)
        {
            throw new InvalidOperationException("The timetable template could not be found.");
        }

        /*
         * -----------------------------------------------------
         * Validate timetable date
         * -----------------------------------------------------
         */

        if (request.AttendanceDate < timetable.ValidFrom || request.AttendanceDate > timetable.ValidTo)
        {
            throw new InvalidOperationException("The selected date is outside the timetable validity period.");
        }

        /*
         * -----------------------------------------------------
         * Validate day of week
         * -----------------------------------------------------
         */

        if (!IsMatchingDay(timetable.DayOfWeek, request.AttendanceDate))
        {
            throw new InvalidOperationException("The selected date does not match the timetable lecture day.");
        }

        /*
         * -----------------------------------------------------
         * Load approved override
         * -----------------------------------------------------
         */

        LectureOverride? lectureOverride = null;

        if (request.LectureOverrideId.HasValue)
        {
            lectureOverride = await _dbContext.LectureOverrides
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == request.LectureOverrideId.Value &&
                    x.InstitutionId == institutionId &&
                    x.CampusId == campusId &&
                    x.AcademicSessionId == timetable.AcademicSessionId &&
                    x.TimetableTemplateId == timetable.Id &&
                    x.OverrideDate == request.AttendanceDate &&
                    x.IsApproved);

            if (lectureOverride == null)
            {
                throw new InvalidOperationException("The specified lecture override could not be found.");
            }
        }
        else
        {
            /*
             * If the caller did not explicitly provide an override,
             * automatically look for the approved override for
             * this lecture occurrence.
             */
            lectureOverride = await _dbContext.LectureOverrides
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.InstitutionId == institutionId &&
                    x.CampusId == campusId &&
                    x.AcademicSessionId == timetable.AcademicSessionId &&
                    x.TimetableTemplateId == timetable.Id &&
                    x.OverrideDate == request.AttendanceDate &&
                    x.IsApproved);
        }

        /*
         * -----------------------------------------------------
         * Cancelled lecture
         * -----------------------------------------------------
         *
         * A cancelled lecture must never create an attendance
         * session.
         */

        if (lectureOverride != null && IsCancellationOverride(lectureOverride.OverrideType))
        {
            throw new InvalidOperationException("Attendance cannot be created because this lecture has been cancelled.");
        }

        /*
         * -----------------------------------------------------
         * Check whether attendance already exists
         * -----------------------------------------------------
         */

        var existingSession = await _dbContext.AttendanceSessions
            .FirstOrDefaultAsync(x =>
                x.InstitutionId == institutionId &&
                x.CampusId == campusId &&
                x.AcademicSessionId == timetable.AcademicSessionId &&
                x.AttendanceDate == request.AttendanceDate &&
                x.TimetableTemplateId == timetable.Id);

        if (existingSession != null)
        {
            return await GetSessionByIdAsync(existingSession.Id);
        }

        /*
         * -----------------------------------------------------
         * Resolve effective lecture values
         * -----------------------------------------------------
         */

        var effectiveTeacherId = lectureOverride?.NewTeacherId ?? timetable.TeacherId;

        var effectiveRoomId = lectureOverride?.NewRoomId ?? timetable.RoomId;

        var effectiveStartTime = lectureOverride?.NewStartTime ?? timetable.StartTime;

        var effectiveEndTime = lectureOverride?.NewEndTime ?? timetable.EndTime;

        var effectiveGenerateAttendance = lectureOverride == null ? timetable.GenerateAttendance : lectureOverride.GenerateAttendance;

        if (!effectiveGenerateAttendance)
        {
            throw new InvalidOperationException("Attendance generation is disabled for this lecture.");
        }

        /*
         * -----------------------------------------------------
         * Create AttendanceSession
         * -----------------------------------------------------
         */

        var attendanceSession = new AttendanceSession
        {
            InstitutionId = institutionId,
            CampusId = campusId,
            AcademicSessionId = timetable.AcademicSessionId,

            TeacherAssignmentId = timetable.TeacherAssignmentId,

            TimetableTemplateId = timetable.Id,

            LectureOverrideId = lectureOverride?.Id,

            SubjectId = timetable.SemesterSubject.SubjectId,

            SemesterSubjectId = timetable.SemesterSubjectId,

            TeacherId = effectiveTeacherId,

            SectionId = timetable.SectionId,

            RoomId = effectiveRoomId,

            LectureType = timetable.LectureType,

            AttendanceDate = request.AttendanceDate,

            StartTime = effectiveStartTime,

            EndTime = effectiveEndTime,

            IsAttendanceMarked = false,

            Status = AttendanceSessionStatus.Scheduled,

            Source = lectureOverride == null
                ? AttendanceSource.Timetable
                : AttendanceSource.CalendarOverride,

            IsLocked = false,

            Remarks = request.Remarks
        };

        _dbContext.AttendanceSessions.Add(attendanceSession);

        /*
         * -----------------------------------------------------
         * Load current students
         * -----------------------------------------------------
         */

        var students = await _dbContext.StudentEnrollments
            .AsNoTracking()
            .Where(x =>
                x.InstitutionId == institutionId &&
                x.CampusId == campusId &&
                x.AcademicSessionId == timetable.AcademicSessionId &&
                x.SectionId == timetable.SectionId &&
                x.IsCurrent)
            .Include(x => x.Student)
            .Select(x => x.Student)
            .Distinct()
            .ToListAsync();

        if (students.Count == 0)
        {
            throw new InvalidOperationException("No active students are enrolled in this section.");
        }

        /*
         * -----------------------------------------------------
         * Create AttendanceRecord for every student
         * -----------------------------------------------------
         */

        foreach (var student in students)
        {
            var record = new AttendanceRecord
            {
                InstitutionId = institutionId,

                CampusId = campusId,

                AttendanceSessionId = attendanceSession.Id,

                StudentId = student.Id,

                IsMarked = false,

                /*
                 * Status is not meaningful until IsMarked becomes true.
                 * We nevertheless initialize it to Absent as a safe
                 * database value. The UI must use IsMarked to distinguish
                 * "not yet marked" from actual absence.
                 */
                Status = AttendanceStatus.Absent,

                MarkedOn = null,

                MarkedByUserId = null,

                MarkingMethod = AttendanceMarkingMethod.System,

                Remarks = null
            };

            attendanceSession.AttendanceRecords.Add(record);
        }

        await _dbContext.SaveChangesAsync();

        return await GetSessionByIdAsync(attendanceSession.Id);
    }

    // =========================================================
    // Get Session
    // =========================================================

    public async Task<AttendanceSessionResponse> GetSessionByIdAsync(Guid id)
    {
        var institutionId = GetRequiredInstitutionId();
        var campusId = GetRequiredCampusId();

        var session = await _dbContext.AttendanceSessions
            .AsNoTracking()
            .Include(x => x.AttendanceRecords)
                .ThenInclude(x => x.Student)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.InstitutionId == institutionId &&
                x.CampusId == campusId);

        if (session == null)
        {
            throw new KeyNotFoundException("Attendance session not found.");
        }

        return MapSession(session);
    }

    // =========================================================
    // Teacher Sessions
    // =========================================================

    public async Task<List<AttendanceSessionResponse>> GetTeacherSessionsAsync(DateOnly startDate, DateOnly endDate)
    {
        var institutionId = GetRequiredInstitutionId();
        var campusId = GetRequiredCampusId();
        var userId = GetRequiredUserId();

        /*
         * We identify the teacher using the authenticated user.
         *
         * If your Teacher entity uses a different property than
         * UserId, change only this lookup.
         */

        var teacherId = await _dbContext.Teachers
            .AsNoTracking()
            .Where(x =>
                x.InstitutionId == institutionId &&
                x.CampusId == campusId &&
                x.UserId == userId)
            .Select(x => (Guid?)x.Id)
            .FirstOrDefaultAsync();

        if (!teacherId.HasValue)
        {
            throw new InvalidOperationException("The current user is not associated with a teacher.");
        }

        var sessions = await _dbContext.AttendanceSessions
            .AsNoTracking()
            .Include(x => x.AttendanceRecords)
                .ThenInclude(x => x.Student)
            .Where(x =>
                x.InstitutionId == institutionId &&
                x.CampusId == campusId &&
                x.TeacherId == teacherId.Value &&
                x.AttendanceDate >= startDate &&
                x.AttendanceDate <= endDate)
            .OrderBy(x => x.AttendanceDate)
            .ThenBy(x => x.StartTime)
            .ToListAsync();

        return sessions.Select(MapSession).ToList();
    }

    public async Task<AttendanceSessionResponse> MarkAttendanceAsync(MarkAttendanceRequest request)
    {
        var institutionId = GetRequiredInstitutionId();
        var campusId = GetRequiredCampusId();
        var userId = GetRequiredUserId();

        ValidateAttendanceStatus(request.Status);

        var record = await _dbContext.AttendanceRecords
            .Include(x => x.AttendanceSession)
            .FirstOrDefaultAsync(x =>
                x.Id == request.AttendanceRecordId &&
                x.InstitutionId == institutionId &&
                x.CampusId == campusId);

        if (record == null)
        {
            throw new KeyNotFoundException("Attendance record not found.");
        }

        var session = record.AttendanceSession;

        ValidateSessionForMarking(session);

        /*
         * Only the teacher responsible for this session may
         * manually mark the attendance.
         */
        var teacher = await _dbContext.Teachers
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == session.TeacherId &&
                x.InstitutionId == institutionId &&
                x.CampusId == campusId &&
                x.UserId == userId);

        if (teacher == null)
        {
            throw new UnauthorizedAccessException("You are not authorized to mark attendance for this session.");
        }

        record.Status = request.Status;

        record.IsMarked = true;

        record.MarkedOn = DateTime.UtcNow;

        record.MarkedByUserId = userId;

        record.MarkingMethod = AttendanceMarkingMethod.Manual;

        record.Remarks = request.Remarks;

        await UpdateSessionAttendanceStateAsync(session.Id);

        await _dbContext.SaveChangesAsync();

        return await GetSessionByIdAsync(session.Id);
    }

    public async Task<AttendanceSessionResponse> MarkAttendanceBulkAsync(MarkAttendanceBulkRequest request)
    {
        var institutionId = GetRequiredInstitutionId();
        var campusId = GetRequiredCampusId();
        var userId = GetRequiredUserId();

        if (request.Records == null || request.Records.Count == 0)
        {
            throw new InvalidOperationException("At least one attendance record is required.");
        }

        var session = await _dbContext.AttendanceSessions
            .FirstOrDefaultAsync(x =>
                x.Id == request.AttendanceSessionId &&
                x.InstitutionId == institutionId &&
                x.CampusId == campusId);

        if (session == null)
        {
            throw new KeyNotFoundException("Attendance session not found.");
        }

        ValidateSessionForMarking(session);

        /*
         * Ensure the authenticated teacher owns this session.
         */
        var teacherExists = await _dbContext.Teachers
            .AsNoTracking()
            .AnyAsync(x =>
                x.Id == session.TeacherId &&
                x.InstitutionId == institutionId &&
                x.CampusId == campusId &&
                x.UserId == userId);

        if (!teacherExists)
        {
            throw new UnauthorizedAccessException("You are not authorized to mark attendance for this session.");
        }

        /*
         * Prevent duplicate record IDs in the request.
         */
        var recordIds = request.Records
            .Select(x => x.AttendanceRecordId)
            .Distinct()
            .ToList();

        if (recordIds.Count != request.Records.Count)
        {
            throw new InvalidOperationException("Duplicate attendance records were supplied.");
        }

        /*
         * Load only records belonging to this session.
         */
        var attendanceRecords = await _dbContext.AttendanceRecords
            .Where(x =>
                x.AttendanceSessionId == session.Id &&
                x.InstitutionId == institutionId &&
                x.CampusId == campusId &&
                recordIds.Contains(x.Id))
            .ToListAsync();

        if (attendanceRecords.Count != recordIds.Count)
        {
            throw new InvalidOperationException("One or more attendance records do not belong to this session.");
        }

        var requestedRecords = request.Records
            .ToDictionary(x => x.AttendanceRecordId);

        var markedOn = DateTime.UtcNow;

        foreach (var record in attendanceRecords)
        {
            var requestItem = requestedRecords[record.Id];

            ValidateAttendanceStatus(requestItem.Status);

            record.Status = requestItem.Status;

            record.IsMarked = true;

            record.MarkedOn = markedOn;

            record.MarkedByUserId = userId;

            record.MarkingMethod = AttendanceMarkingMethod.Manual;

            record.Remarks = requestItem.Remarks;
        }

        await UpdateSessionAttendanceStateAsync(session.Id);

        await _dbContext.SaveChangesAsync();

        return await GetSessionByIdAsync(session.Id);
    }

    public async Task<AttendanceSessionResponse> CompleteSessionAsync(CompleteAttendanceSessionRequest request)
    {
        var institutionId = GetRequiredInstitutionId();
        var campusId = GetRequiredCampusId();
        var userId = GetRequiredUserId();

        var session = await _dbContext.AttendanceSessions
            .Include(x => x.AttendanceRecords)
            .FirstOrDefaultAsync(x =>
                x.Id == request.AttendanceSessionId &&
                x.InstitutionId == institutionId &&
                x.CampusId == campusId);

        if (session == null)
        {
            throw new KeyNotFoundException("Attendance session not found.");
        }

        if (session.IsLocked || session.Status == AttendanceSessionStatus.Locked)
        {
            throw new InvalidOperationException("Attendance session is already locked.");
        }

        if (session.Status == AttendanceSessionStatus.Cancelled)
        {
            throw new InvalidOperationException("Cancelled attendance session cannot be completed.");
        }

        var teacherExists = await _dbContext.Teachers
            .AsNoTracking()
            .AnyAsync(x =>
                x.Id == session.TeacherId &&
                x.InstitutionId == institutionId &&
                x.CampusId == campusId &&
                x.UserId == userId);

        if (!teacherExists)
        {
            throw new UnauthorizedAccessException("You are not authorized to complete this attendance session.");
        }

        /*
         * Every student must have a definitive attendance status
         * before the teacher can complete the session.
         */
        var unmarkedRecords = session.AttendanceRecords
            .Where(x => !x.IsMarked)
            .ToList();

        if (unmarkedRecords.Count > 0)
        {
            throw new InvalidOperationException($"Attendance cannot be completed because {unmarkedRecords.Count} student(s) are still unmarked.");
        }

        session.IsAttendanceMarked = true;

        session.Status = AttendanceSessionStatus.Completed;

        if (!string.IsNullOrWhiteSpace(request.Remarks))
        {
            session.Remarks = request.Remarks;
        }

        await _dbContext.SaveChangesAsync();

        return await GetSessionByIdAsync(session.Id);
    }

    public async Task<AttendanceSessionResponse> LockSessionAsync(Guid attendanceSessionId)
    {
        var institutionId = GetRequiredInstitutionId();
        var campusId = GetRequiredCampusId();
        var userId = GetRequiredUserId();

        var session = await _dbContext.AttendanceSessions
            .FirstOrDefaultAsync(x =>
                x.Id == attendanceSessionId &&
                x.InstitutionId == institutionId &&
                x.CampusId == campusId);

        if (session == null)
        {
            throw new KeyNotFoundException("Attendance session not found.");
        }

        if (session.IsLocked || session.Status == AttendanceSessionStatus.Locked)
        {
            return await GetSessionByIdAsync(session.Id);
        }

        if (session.Status != AttendanceSessionStatus.Completed)
        {
            throw new InvalidOperationException("Only completed attendance sessions can be locked.");
        }

        var teacherExists = await _dbContext.Teachers
            .AsNoTracking()
            .AnyAsync(x =>
                x.Id == session.TeacherId &&
                x.InstitutionId == institutionId &&
                x.CampusId == campusId &&
                x.UserId == userId);

        if (!teacherExists)
        {
            throw new UnauthorizedAccessException("You are not authorized to lock this attendance session.");
        }

        session.IsLocked = true;

        session.Status = AttendanceSessionStatus.Locked;

        session.LockedByUserId = userId;

        session.LockedOn = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return await GetSessionByIdAsync(session.Id);
    }

    public async Task<AttendanceQrSessionResponse> StartQrAttendanceAsync(StartAttendanceQrRequest request)
    {
        var institutionId = GetRequiredInstitutionId();
        var campusId = GetRequiredCampusId();
        var userId = GetRequiredUserId();

        if (request.DurationSeconds < AttendanceConstants.MinimumQrWindowSeconds || request.DurationSeconds > AttendanceConstants.MaximumQrWindowSeconds)
        {
            throw new InvalidOperationException($"QR duration must be between " +
                $"{AttendanceConstants.MinimumQrWindowSeconds} and " +
                $"{AttendanceConstants.MaximumQrWindowSeconds} seconds.");
        }

        var session = await _dbContext.AttendanceSessions
            .FirstOrDefaultAsync(x =>
                x.Id == request.AttendanceSessionId &&
                x.InstitutionId == institutionId &&
                x.CampusId == campusId);

        if (session == null)
        {
            throw new KeyNotFoundException("Attendance session not found.");
        }

        if (session.IsLocked || session.Status == AttendanceSessionStatus.Locked)
        {
            throw new InvalidOperationException("Attendance session is locked.");
        }

        if (session.Status == AttendanceSessionStatus.Completed)
        {
            throw new InvalidOperationException("Attendance session has already been completed.");
        }

        if (session.Status == AttendanceSessionStatus.Cancelled)
        {
            throw new InvalidOperationException("Attendance session is cancelled.");
        }

        var teacherExists = await _dbContext.Teachers
            .AsNoTracking()
            .AnyAsync(x =>
                x.Id == session.TeacherId &&
                x.InstitutionId == institutionId &&
                x.CampusId == campusId &&
                x.UserId == userId);

        if (!teacherExists)
        {
            throw new UnauthorizedAccessException("You are not authorized to start QR attendance for this session.");
        }

        /*
         * Close any currently active QR window.
         */
        var activeQrSessions = await _dbContext.AttendanceQrSessions
            .Where(x =>
                x.AttendanceSessionId == session.Id &&
                x.IsActive)
            .ToListAsync();

        var now = DateTime.UtcNow;

        foreach (var activeQr in activeQrSessions)
        {
            activeQr.IsActive = false;
            activeQr.ClosedOn = now;
        }

        /*
         * Session must be open while QR attendance is running.
         */
        if (session.Status == AttendanceSessionStatus.Scheduled)
        {
            session.Status = AttendanceSessionStatus.Open;
        }

        var qrSession = new AttendanceQrSession
        {
            InstitutionId = institutionId,
            CampusId = campusId,

            AttendanceSessionId = session.Id,

            Token = GenerateQrToken(),

            ValidFrom = now,

            ExpiresOn = now.AddSeconds(
                request.DurationSeconds),

            IsActive = true,

            CreatedByUserId = userId
        };

        _dbContext.AttendanceQrSessions.Add(qrSession);

        await _dbContext.SaveChangesAsync();

        return await BuildQrSessionResponseAsync(qrSession.Id);
    }

    public async Task<AttendanceQrSessionResponse> GetActiveQrSessionAsync(Guid attendanceSessionId)
    {
        var institutionId = GetRequiredInstitutionId();
        var campusId = GetRequiredCampusId();

        var qrSession = await _dbContext.AttendanceQrSessions
            .FirstOrDefaultAsync(x =>
                x.AttendanceSessionId == attendanceSessionId &&
                x.InstitutionId == institutionId &&
                x.CampusId == campusId &&
                x.IsActive &&
                x.ExpiresOn > DateTime.UtcNow);

        if (qrSession == null)
        {
            throw new KeyNotFoundException("No active QR attendance window exists.");
        }

        return await BuildQrSessionResponseAsync(qrSession.Id);
    }

    public async Task<AttendanceQrScanResponse> ScanAttendanceQrAsync(ScanAttendanceQrRequest request)
    {
        var institutionId = GetRequiredInstitutionId();
        var campusId = GetRequiredCampusId();
        var studentId = await GetCurrentStudentIdAsync();

        if (string.IsNullOrWhiteSpace(request.Token))
        {
            throw new InvalidOperationException(
                "QR token is required.");
        }

        var now = DateTime.UtcNow;

        var qrSession = await _dbContext.AttendanceQrSessions
            .Include(x => x.AttendanceSession)
            .FirstOrDefaultAsync(x =>
                x.Token == request.Token &&
                x.InstitutionId == institutionId &&
                x.CampusId == campusId &&
                x.IsActive);

        if (qrSession == null)
        {
            throw new InvalidOperationException("QR code is invalid or no longer active.");
        }

        if (now < qrSession.ValidFrom || now >= qrSession.ExpiresOn)
        {
            qrSession.IsActive = false;
            qrSession.ClosedOn = now;

            await _dbContext.SaveChangesAsync();

            throw new InvalidOperationException("QR attendance window has expired.");
        }

        var session = qrSession.AttendanceSession;

        if (session.IsLocked || session.Status == AttendanceSessionStatus.Locked)
        {
            throw new InvalidOperationException("Attendance session is locked.");
        }

        if (session.Status == AttendanceSessionStatus.Completed)
        {
            throw new InvalidOperationException("Attendance session has already been completed.");
        }

        if (session.Status == AttendanceSessionStatus.Cancelled)
        {
            throw new InvalidOperationException("Attendance session is cancelled.");
        }

        /*
         * Verify that this student belongs to the section
         * for which attendance is being taken.
         */
        var enrolled = await _dbContext.StudentEnrollments
            .AsNoTracking()
            .AnyAsync(x =>
                x.StudentId == studentId &&
                x.SectionId == session.SectionId &&
                x.AcademicSessionId == session.AcademicSessionId &&
                x.InstitutionId == institutionId &&
                x.CampusId == campusId &&
                x.IsCurrent);

        if (!enrolled)
        {
            throw new UnauthorizedAccessException("You are not enrolled in this class.");
        }

        var record = await _dbContext.AttendanceRecords
            .FirstOrDefaultAsync(x =>
                x.AttendanceSessionId == session.Id &&
                x.StudentId == studentId &&
                x.InstitutionId == institutionId &&
                x.CampusId == campusId);

        if (record == null)
        {
            throw new InvalidOperationException("Attendance record was not generated for this student.");
        }

        /*
         * Duplicate scan protection.
         */
        if (record.IsMarked)
        {
            throw new InvalidOperationException("Attendance has already been recorded for this student.");
        }

        record.IsMarked = true;

        record.Status = AttendanceStatus.Present;

        record.MarkedOn = now;

        record.MarkedByUserId = null;

        record.MarkingMethod = AttendanceMarkingMethod.QRCode;

        record.Remarks = "Attendance marked using QR code.";

        await _dbContext.SaveChangesAsync();

        return new AttendanceQrScanResponse
        {
            Success = true,

            AttendanceSessionId = session.Id,

            AttendanceRecordId = record.Id,

            Message = "Attendance marked successfully.",

            MarkedOn = now
        };
    }

    private async Task ExpireQrAttendanceAsync(AttendanceQrSession qrSession)
    {
        if (!qrSession.IsActive)
        {
            return;
        }

        var now = DateTime.UtcNow;

        if (now < qrSession.ExpiresOn)
        {
            return;
        }

        qrSession.IsActive = false;

        qrSession.ClosedOn = now;

        var records = await _dbContext.AttendanceRecords
            .Where(x =>
                x.AttendanceSessionId == qrSession.AttendanceSessionId &&
                !x.IsMarked)
            .ToListAsync();

        foreach (var record in records)
        {
            record.IsMarked = true;

            record.Status = AttendanceStatus.Absent;

            record.MarkedOn = now;

            record.MarkedByUserId = null;

            record.MarkingMethod = AttendanceMarkingMethod.System;

            record.Remarks = "Automatically marked absent after QR attendance window expired.";
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<AttendanceQrSessionResponse> CloseQrAttendanceAsync(Guid attendanceSessionId)
    {
        var institutionId = GetRequiredInstitutionId();
        var campusId = GetRequiredCampusId();
        var userId = GetRequiredUserId();

        var qrSession = await _dbContext.AttendanceQrSessions
            .FirstOrDefaultAsync(x =>
                x.AttendanceSessionId == attendanceSessionId &&
                x.InstitutionId == institutionId &&
                x.CampusId == campusId &&
                x.IsActive);

        if (qrSession == null)
        {
            throw new KeyNotFoundException("No active QR attendance window exists.");
        }

        var teacherExists = await _dbContext.Teachers
            .AsNoTracking()
            .AnyAsync(x =>
                x.Id == qrSession.AttendanceSession.TeacherId &&
                x.InstitutionId == institutionId &&
                x.CampusId == campusId &&
                x.UserId == userId);

        if (!teacherExists)
        {
            throw new UnauthorizedAccessException("You are not authorized to close this QR attendance window.");
        }

        qrSession.IsActive = false;

        qrSession.ClosedOn = DateTime.UtcNow;

        var records = await _dbContext.AttendanceRecords
            .Where(x =>
                x.AttendanceSessionId == attendanceSessionId &&
                !x.IsMarked)
            .ToListAsync();

        var now = DateTime.UtcNow;

        foreach (var record in records)
        {
            record.IsMarked = true;

            record.Status = AttendanceStatus.Absent;

            record.MarkedOn = now;

            record.MarkedByUserId = null;

            record.MarkingMethod = AttendanceMarkingMethod.System;

            record.Remarks = "Automatically marked absent when QR attendance was closed.";
        }

        await _dbContext.SaveChangesAsync();

        return await BuildQrSessionResponseAsync(qrSession.Id);
    }

    // =========================================================
    // Helpers
    // =========================================================

    private static string GenerateQrToken()
    {
        var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(48);

        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", string.Empty);
    }

    private async Task<Guid> GetCurrentStudentIdAsync()
    {
        var institutionId = GetRequiredInstitutionId();
        var campusId = GetRequiredCampusId();
        var userId = GetRequiredUserId();

        var studentId = await _dbContext.Students
            .AsNoTracking()
            .Where(x =>
                x.InstitutionId == institutionId &&
                x.CampusId == campusId &&
                x.UserId == userId)
            .Select(x => (Guid?)x.Id)
            .FirstOrDefaultAsync();

        return studentId ?? throw new UnauthorizedAccessException("The current user is not associated with a student.");
    }

    private async Task<AttendanceQrSessionResponse> BuildQrSessionResponseAsync(Guid qrSessionId)
    {
        var qrSession = await _dbContext.AttendanceQrSessions
            .AsNoTracking()
            .Include(x => x.AttendanceSession)
            .FirstOrDefaultAsync(x => x.Id == qrSessionId);

        if (qrSession == null)
        {
            throw new KeyNotFoundException(
                "QR attendance session not found.");
        }

        var records = await _dbContext.AttendanceRecords
            .AsNoTracking()
            .Where(x =>
                x.AttendanceSessionId ==
                qrSession.AttendanceSessionId)
            .Select(x => new
            {
                x.IsMarked
            })
            .ToListAsync();

        var markedCount = records.Count(x => x.IsMarked);

        return new AttendanceQrSessionResponse
        {
            Id = qrSession.Id,

            AttendanceSessionId = qrSession.AttendanceSessionId,

            Token = qrSession.Token,

            ValidFrom = qrSession.ValidFrom,

            ExpiresOn = qrSession.ExpiresOn,

            DurationSeconds = (int)(qrSession.ExpiresOn - qrSession.ValidFrom).TotalSeconds,

            IsActive = qrSession.IsActive && qrSession.ExpiresOn > DateTime.UtcNow,

            MarkedCount = markedCount,

            TotalStudentCount = records.Count,

            RemainingStudentCount = records.Count - markedCount
        };
    }

    private static void ValidateSessionForMarking(AttendanceSession session)
    {
        if (session.IsLocked || session.Status == AttendanceSessionStatus.Locked)
        {
            throw new InvalidOperationException("Attendance session is locked and can no longer be modified.");
        }

        if (session.Status == AttendanceSessionStatus.Cancelled)
        {
            throw new InvalidOperationException("Attendance cannot be marked for a cancelled session.");
        }

        if (session.Status == AttendanceSessionStatus.Completed)
        {
            throw new InvalidOperationException("Attendance session has already been completed.");
        }

        if (session.Status == AttendanceSessionStatus.Scheduled)
        {
            throw new InvalidOperationException("Attendance session must be opened before attendance can be marked.");
        }
    }

    private static void ValidateAttendanceStatus(AttendanceStatus status)
    {
        if (status is AttendanceStatus.Holiday or AttendanceStatus.Cancelled)
        {
            throw new InvalidOperationException("Holiday and Cancelled are not valid manual attendance statuses.");
        }

        if (!Enum.IsDefined(status))
        {
            throw new InvalidOperationException("Invalid attendance status.");
        }
    }

    private async Task UpdateSessionAttendanceStateAsync(Guid attendanceSessionId)
    {
        var session = await _dbContext.AttendanceSessions
            .Include(x => x.AttendanceRecords)
            .FirstAsync(x => x.Id == attendanceSessionId);

        var totalRecords = session.AttendanceRecords.Count;

        var markedRecords = session.AttendanceRecords.Count(x => x.IsMarked);

        session.IsAttendanceMarked = totalRecords > 0 && markedRecords == totalRecords;

        /*
         * Do not automatically complete the session merely because
         * some records have been marked.
         *
         * Completion is an explicit teacher action.
         */
        if (session.Status == AttendanceSessionStatus.Scheduled)
        {
            session.Status = AttendanceSessionStatus.Open;
        }
    }

    private AttendanceSessionResponse MapSession(AttendanceSession session)
    {
        var records = session.AttendanceRecords
            .OrderBy(x => x.Student.User.FirstName)
            .ThenBy(x => x.Student.User.LastName)
            .Select(x => new AttendanceRecordResponse
            {
                Id = x.Id,

                StudentId = x.StudentId,

                StudentName = BuildStudentName(x.Student),

                /*
                 * Change this property if your Student entity uses
                 * a different name for the roll number.
                 */
                RollNumber = x.Student.RollNumber,

                Status = x.Status,

                IsMarked = x.IsMarked,

                MarkedOn = x.MarkedOn,

                MarkedByUserId = x.MarkedByUserId,

                MarkingMethod = x.MarkingMethod,

                Remarks = x.Remarks
            })
            .ToList();

        return new AttendanceSessionResponse
        {
            Id = session.Id,

            AcademicSessionId = session.AcademicSessionId,

            TeacherAssignmentId = session.TeacherAssignmentId,

            TimetableTemplateId = session.TimetableTemplateId,

            LectureOverrideId = session.LectureOverrideId,

            SubjectId = session.SubjectId,

            SemesterSubjectId = session.SemesterSubjectId,

            TeacherId = session.TeacherId,

            SectionId = session.SectionId,

            RoomId = session.RoomId,

            LectureType = session.LectureType,

            AttendanceDate = session.AttendanceDate,

            StartTime = session.StartTime,

            EndTime = session.EndTime,

            IsAttendanceMarked = session.IsAttendanceMarked,

            Status = session.Status,

            Source = session.Source,

            IsLocked = session.IsLocked,

            LockedByUserId = session.LockedByUserId,

            LockedOn = session.LockedOn,

            Remarks = session.Remarks,

            TotalStudents = records.Count,

            MarkedStudents = records.Count(x => x.IsMarked),

            Records = records
        };
    }

    private static string BuildStudentName(Student student)
    {
        var parts = new[]
        {
            student.User.FirstName,
            student.User.LastName
        };

        return string.Join(" ", parts.Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    private static bool IsMatchingDay(DayOfWeekType timetableDay, DateOnly date)
    {
        var dayNumber = date.DayOfWeek switch
        {
            DayOfWeek.Monday => 1,
            DayOfWeek.Tuesday => 2,
            DayOfWeek.Wednesday => 3,
            DayOfWeek.Thursday => 4,
            DayOfWeek.Friday => 5,
            DayOfWeek.Saturday => 6,
            DayOfWeek.Sunday => 7,
            _ => 0
        };

        return (int)timetableDay == dayNumber;
    }

    private static bool IsCancellationOverride(OverrideType overrideType)
    {
        return overrideType == OverrideType.Cancelled;
    }

    private Guid GetRequiredInstitutionId()
    {
        return _currentUserService.InstitutionId ?? throw new UnauthorizedAccessException("Institution context is not available.");
    }

    private Guid GetRequiredCampusId()
    {
        return _currentUserService.CampusId ?? throw new UnauthorizedAccessException("Campus context is not available.");
    }

    private Guid GetRequiredUserId()
    {
        return _currentUserService.UserId ?? throw new UnauthorizedAccessException("User context is not available.");
    }
}