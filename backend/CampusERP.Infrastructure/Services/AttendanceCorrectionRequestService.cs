using Azure.Core;
using CampusERP.Application.Common.Exceptions;
using CampusERP.Application.Interfaces;
using CampusERP.Application.Mappings;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using CampusERP.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class AttendanceCorrectionRequestService : IAttendanceCorrectionRequestService
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IDataAccessScope _scope;

    private readonly IAcademicConfigurationService _academicConfigurationService;

    public AttendanceCorrectionRequestService(ApplicationDbContext dbContext, IDataAccessScope scope, IAcademicConfigurationService academicConfigurationService)
    {
        _dbContext = dbContext;

        _scope = scope;

        _academicConfigurationService = academicConfigurationService;
    }

    private IQueryable<AttendanceCorrectionRequest> ApplyScope(IQueryable<AttendanceCorrectionRequest> query)
    {
        if (_scope.IsSuperAdmin() || _scope.IsPlatformAdmin())
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

    private IQueryable<AttendanceCorrectionRequest> Query()
    {
        return ApplyScope(_dbContext.AttendanceCorrectionRequests)
            .Include(x => x.Student)
            .Include(x => x.AttendanceRecord)
                .ThenInclude(x => x.AttendanceSession)
            .Include(x => x.ReviewedByUser);
    }

    public async Task<List<AttendanceCorrectionRequestResponse>> GetAllAsync()
    {
        var requests = await Query()
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return AttendanceCorrectionRequestMapper.ToResponse(requests);
    }

    public async Task<AttendanceCorrectionRequestResponse?> GetByIdAsync(Guid id)
    {
        var request = await Query().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        if (request == null)
        {
            return null;
        }

        return AttendanceCorrectionRequestMapper.ToResponse(request);
    }

    public async Task<List<AttendanceCorrectionRequestResponse>> GetPendingAsync()
    {
        var requests = await Query()
            .AsNoTracking()
            .Where(x =>
                x.Status ==
                AttendanceCorrectionStatus.Pending)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return AttendanceCorrectionRequestMapper.ToResponse(requests);
    }

    public async Task<List<AttendanceCorrectionRequestResponse>> GetByStudentAsync(Guid studentId)
    {
        var requests = await Query()
            .AsNoTracking()
            .Where(x =>
                x.StudentId == studentId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return AttendanceCorrectionRequestMapper.ToResponse(requests);
    }

    public async Task<List<AttendanceCorrectionRequestResponse>> GetByAttendanceRecordAsync(Guid attendanceRecordId)
    {
        var requests = await Query()
            .AsNoTracking()
            .Where(x =>
                x.AttendanceRecordId == attendanceRecordId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return AttendanceCorrectionRequestMapper.ToResponse(requests);
    }

    public async Task<AttendanceCorrectionRequestResponse> CreateAsync(CreateAttendanceCorrectionRequest request)
    {
        var attendanceRecord = await GetAttendanceRecordAsync(request.AttendanceRecordId);

        await ValidateCreateAsync(attendanceRecord, request);

        var entity = CreateEntity(attendanceRecord, request);

        _dbContext.AttendanceCorrectionRequests.Add(entity);

        await _dbContext.SaveChangesAsync();

        entity = await Query().FirstAsync(x => x.Id == entity.Id);

        return AttendanceCorrectionRequestMapper.ToResponse(entity);
    }

    private async Task<AttendanceRecord> GetAttendanceRecordAsync(Guid id)
    {
        var attendanceRecord = await _dbContext.AttendanceRecords
            .Include(x => x.Student)
            .Include(x => x.AttendanceSession)
            .Include(x => x.CorrectionRequests)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (attendanceRecord is null)
        {
            throw new NotFoundException(ErrorCodes.AttendanceRecordNotFound, "Attendance record not found.");
        }

        return attendanceRecord;
    }

    private async Task ValidateCreateAsync(AttendanceRecord attendanceRecord, CreateAttendanceCorrectionRequest request)
    {
        if (attendanceRecord.IsDeleted)
        {
            throw new NotFoundException(ErrorCodes.AttendanceRecordNotFound, "Attendance record not found.");
        }

        if (attendanceRecord.Status == request.RequestedStatus)
        {
            throw new ValidationException(ErrorCodes.AttendanceAlreadySameStatus, "Requested attendance status must be different from the current attendance status.");
        }

        if (await HasPendingRequestAsync(attendanceRecord.Id))
        {
            throw new ConflictException(ErrorCodes.AttendanceCorrectionAlreadyExists, "A pending attendance correction request already exists.");
        }

        ValidateOwnership(attendanceRecord);

        await ValidateAttendancePolicyAsync(attendanceRecord);
    }

    private void ValidateOwnership(AttendanceRecord attendanceRecord)
    {
        if (_scope.IsSuperAdmin() ||
            _scope.IsPlatformAdmin() ||
            _scope.IsInstitutionAdmin() ||
            _scope.IsCampusAdmin())
        {
            return;
        }

        if (_scope.UserId() != attendanceRecord.Student.UserId)
        {
            throw new ForbiddenException(ErrorCodes.AttendanceRequestForbidden, "You are not allowed to submit a correction request for this attendance record.");
        }
    }

    private async Task ValidateAttendancePolicyAsync(AttendanceRecord attendanceRecord)
    {
        var configuration = await GetConfigurationAsync(
                attendanceRecord.InstitutionId,
                attendanceRecord.CampusId);

        if (!configuration.AllowStudentAttendanceCorrection)
        {
            throw new BusinessRuleException(ErrorCodes.AttendanceCorrectionDisabled, "Attendance correction requests are disabled.");
        }

        if (attendanceRecord.AttendanceSession.AttendanceDate
            < DateOnly.FromDateTime(DateTime.UtcNow)
                .AddDays(-configuration.AttendanceEditWindowDays))
        {
            throw new BusinessRuleException(ErrorCodes.AttendanceCorrectionExpired, "Attendance correction period has expired.");
        }
    }

    private async Task<bool> HasPendingRequestAsync(Guid attendanceRecordId)
    {
        return await ApplyScope(_dbContext.AttendanceCorrectionRequests)
            .AnyAsync(x =>
                x.AttendanceRecordId == attendanceRecordId &&
                x.Status == AttendanceCorrectionStatus.Pending);
    }

    private async Task<AcademicConfiguration> GetConfigurationAsync(Guid institutionId, Guid? campusId)
    {
        return await _academicConfigurationService.GetEffectiveConfigurationAsync(institutionId, campusId);
    }

    private AttendanceCorrectionRequest CreateEntity(AttendanceRecord attendanceRecord, CreateAttendanceCorrectionRequest request)
    {
        return new AttendanceCorrectionRequest
        {
            InstitutionId = attendanceRecord.InstitutionId,

            CampusId = attendanceRecord.CampusId,

            AttendanceRecordId = attendanceRecord.Id,

            StudentId = attendanceRecord.StudentId,

            Reason = request.Reason,

            Description = request.Description,

            AttachmentPath = request.AttachmentPath,

            RequestedStatus = request.RequestedStatus,

            OriginalStatus = attendanceRecord.Status,

            Status = AttendanceCorrectionStatus.Pending,

            IsProcessed = false
        };
    }

    private async Task<AttendanceCorrectionRequest> GetRequestAsync(Guid id)
    {
        var request = await Query().FirstOrDefaultAsync(x => x.Id == id);

        if (request is null)
        {
            throw new NotFoundException(ErrorCodes.AttendanceCorrectionNotFound, "Attendance correction request not found.");
        }

        return request;
    }

    private static void ValidatePending(AttendanceCorrectionRequest request)
    {
        if (request.Status != AttendanceCorrectionStatus.Pending)
        {
            throw new BusinessRuleException(ErrorCodes.AttendanceRequestAlreadyProcessed, "Only pending requests can be processed.");
        }
    }

    private static void ValidateAttendanceUnlocked(AttendanceSession session)
    {
        if (session.IsLocked)
        {
            throw new BusinessRuleException(ErrorCodes.AttendanceSessionLocked, "Attendance session is locked.");
        }
    }

    public async Task<AttendanceCorrectionRequestResponse> ApproveAsync(Guid id, ApproveAttendanceCorrectionRequest request)
    {
        var correctionRequest = await GetRequestAsync(id);

        ValidatePending(correctionRequest);

        ValidateAttendanceUnlocked(correctionRequest.AttendanceRecord.AttendanceSession);

        ApplyAttendanceCorrection(correctionRequest);

        MarkApproved(correctionRequest, request.ReviewRemarks);

        await _dbContext.SaveChangesAsync();

        return AttendanceCorrectionRequestMapper.ToResponse(correctionRequest);
    }

    public async Task<AttendanceCorrectionRequestResponse> RejectAsync(Guid id, RejectAttendanceCorrectionRequest request)
    {
        var correctionRequest = await GetRequestAsync(id);

        ValidatePending(correctionRequest);

        MarkRejected(correctionRequest, request.ReviewRemarks);

        await _dbContext.SaveChangesAsync();

        return AttendanceCorrectionRequestMapper.ToResponse(correctionRequest);
    }

    public async Task CancelAsync(Guid id)
    {
        var correctionRequest = await GetRequestAsync(id);

        ValidatePending(correctionRequest);

        ValidateCancelOwnership(correctionRequest);

        MarkCancelled(correctionRequest);

        await _dbContext.SaveChangesAsync();
    }

    private void ApplyAttendanceCorrection(AttendanceCorrectionRequest request)
    {
        request.AttendanceRecord.Status = request.RequestedStatus;

        request.AttendanceRecord.MarkedOn = DateTime.UtcNow;

        request.AttendanceUpdatedOn = DateTime.UtcNow;
    }

    private void MarkApproved(AttendanceCorrectionRequest request, string? remarks)
    {
        request.Status = AttendanceCorrectionStatus.Approved;

        request.ReviewRemarks = remarks;

        MarkReviewed(request);
    }

    private void MarkRejected(AttendanceCorrectionRequest request, string remarks)
    {
        request.Status = AttendanceCorrectionStatus.Rejected;

        request.ReviewRemarks = remarks;

        MarkReviewed(request);
    }

    private void MarkCancelled(AttendanceCorrectionRequest request)
    {
        request.Status = AttendanceCorrectionStatus.Cancelled;

        MarkReviewed(request);
    }

    private void ValidateCancelOwnership(AttendanceCorrectionRequest request)
    {
        if (_scope.IsSuperAdmin() ||
            _scope.IsPlatformAdmin() ||
            _scope.IsInstitutionAdmin() ||
            _scope.IsCampusAdmin())
        {
            return;
        }

        if (_scope.UserId() != request.Student.UserId)
        {
            throw new ForbiddenException(ErrorCodes.AttendanceRequestForbidden, "You are not allowed to cancel this attendance correction request.");
        }
    }

    private void MarkReviewed(AttendanceCorrectionRequest request)
    {
        request.ReviewedByUserId = _scope.UserId();

        request.ReviewedOn = DateTime.UtcNow;

        request.IsProcessed = true;

        request.ProcessedOn = DateTime.UtcNow;
    }
}