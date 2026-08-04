using CampusERP.Application.Interfaces;
using CampusERP.Shared.Enums;
using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusERP.Infrastructure.Services;

public class AcademicConfigurationService : IAcademicConfigurationService
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IDataAccessScope _scope;

    public AcademicConfigurationService(ApplicationDbContext dbContext, IDataAccessScope scope)
    {
        _dbContext = dbContext;

        _scope = scope;
    }

    #region Public Methods

    public async Task<AcademicConfigurationResponse> GetAsync()
    {
        var configuration = await GetOrCreateCurrentConfigurationAsync();

        return Map(configuration);
    }

    public async Task<AcademicConfigurationResponse> UpdateAsync(UpdateAcademicConfigurationRequest request)
    {
        var configuration = await GetOrCreateCurrentConfigurationAsync();

        UpdateConfiguration(configuration, request);

        await _dbContext.SaveChangesAsync();

        return Map(configuration);
    }

    public async Task<AcademicConfiguration> GetEffectiveConfigurationAsync(Guid institutionId, Guid? campusId)
    {
        var configurations = await _dbContext.AcademicConfigurations
            .Where(x =>
                x.InstitutionId == institutionId &&
                (x.CampusId == null || x.CampusId == campusId))
            .ToListAsync();

        var campusConfiguration = configurations.FirstOrDefault(x => x.CampusId == campusId);

        if (campusConfiguration != null)
        {
            return campusConfiguration;
        }

        var institutionConfiguration = configurations.FirstOrDefault(x => x.CampusId == null);

        if (institutionConfiguration == null)
        {
            throw new Exception("Academic configuration not found.");
        }

        return institutionConfiguration;
    }

    #endregion

    #region Private Methods

    private async Task<AcademicConfiguration> GetOrCreateCurrentConfigurationAsync()
    {
        var configuration = await ApplyConfigurationScope(_dbContext.AcademicConfigurations)
            .FirstOrDefaultAsync();

        if (configuration != null)
        {
            return configuration;
        }

        configuration = CreateDefaultConfiguration();

        _dbContext.AcademicConfigurations.Add(configuration);

        await _dbContext.SaveChangesAsync();

        return configuration;
    }

    private AcademicConfiguration CreateDefaultConfiguration()
    {
        return new AcademicConfiguration
        {
            InstitutionId = _scope.InstitutionId(),

            CampusId = null,

            AcademicTermType = AcademicTermType.Semester,

            AcademicTermsPerSession = 2,

            AutoPromoteEnabled = true,

            MinimumAttendancePercentage = 75,

            AllowAttendanceEditing = true,

            AttendanceEditWindowDays = 7,

            AutoGenerateAttendanceSessions = true,

            AutoGenerateAttendanceRecords = true,

            AttendanceLockAfterDays = 7,

            AllowTeacherAttendanceUnlock = false,

            LateThresholdMinutes = 10,

            MedicalLeaveCountsAsPresent = false,

            OnDutyCountsAsPresent = true,

            AllowStudentAttendanceCorrection = true,
        };
    }

    private IQueryable<AcademicConfiguration> ApplyConfigurationScope(IQueryable<AcademicConfiguration> query)
    {
        if (_scope.IsSuperAdmin() || _scope.IsPlatformAdmin())
        {
            return query;
        }

        if (_scope.IsInstitutionAdmin() || _scope.IsCampusAdmin())
        {
            return query.Where(x =>
                x.InstitutionId == _scope.InstitutionId() &&
                x.CampusId == null);
        }

        throw new Exception("Access denied.");
    }

    private static void UpdateConfiguration(AcademicConfiguration configuration, UpdateAcademicConfigurationRequest request)
    {
        Validate(request);

        configuration.AcademicTermType = (AcademicTermType)request.AcademicTermType;

        configuration.AcademicTermsPerSession = request.AcademicTermsPerSession;

        configuration.AutoPromoteEnabled = request.AutoPromoteEnabled;

        configuration.MinimumAttendancePercentage = request.MinimumAttendancePercentage;

        configuration.AllowAttendanceEditing = request.AllowAttendanceEditing;

        configuration.AttendanceEditWindowDays = request.AttendanceEditWindowDays;

        configuration.AutoGenerateAttendanceSessions = request.AutoGenerateAttendanceSessions;

        configuration.AutoGenerateAttendanceRecords = request.AutoGenerateAttendanceRecords;

        configuration.AttendanceLockAfterDays = request.AttendanceLockAfterDays;

        configuration.AllowTeacherAttendanceUnlock = request.AllowTeacherAttendanceUnlock;

        configuration.LateThresholdMinutes = request.LateThresholdMinutes;

        configuration.MedicalLeaveCountsAsPresent = request.MedicalLeaveCountsAsPresent;

        configuration.OnDutyCountsAsPresent = request.OnDutyCountsAsPresent;

        configuration.AllowStudentAttendanceCorrection = request.AllowStudentAttendanceCorrection;
    }

    private static void Validate(UpdateAcademicConfigurationRequest request)
    {
        if (request.AcademicTermsPerSession < 1)
        {
            throw new Exception("Academic terms per session must be greater than zero.");
        }

        if (request.MinimumAttendancePercentage < 0 || request.MinimumAttendancePercentage > 100)
        {
            throw new Exception("Minimum attendance percentage must be between 0 and 100.");
        }

        if (request.AttendanceEditWindowDays < 0)
        {
            throw new Exception("Attendance edit window cannot be negative.");
        }

        if (request.AttendanceLockAfterDays < 0)
        {
            throw new Exception("Attendance lock after days cannot be negative.");
        }

        if (request.LateThresholdMinutes < 0)
        {
            throw new Exception("Late threshold minutes cannot be negative.");
        }

        switch (request.AcademicTermType)
        {
            case AcademicTermType.Annual:

                if (request.AcademicTermsPerSession != 1)
                {
                    throw new Exception("Annual academic structure must contain exactly one academic term.");
                }

                break;

            case AcademicTermType.Semester:

                if (request.AcademicTermsPerSession != 2)
                {
                    throw new Exception("Semester academic structure must contain exactly two academic terms.");
                }

                break;

            case AcademicTermType.Trimester:

                if (request.AcademicTermsPerSession != 3)
                {
                    throw new Exception("Trimester academic structure must contain exactly three academic terms.");
                }

                break;

            case AcademicTermType.Quarter:

                if (request.AcademicTermsPerSession != 4)
                {
                    throw new Exception("Quarter academic structure must contain exactly four academic terms.");
                }

                break;

            case AcademicTermType.Custom:

                break;

            default:

                throw new Exception("Invalid academic term type.");
        }
    }

    private static AcademicConfigurationResponse Map(AcademicConfiguration configuration)
    {
        return new AcademicConfigurationResponse
        {
            Id = configuration.Id,

            InstitutionId = configuration.InstitutionId,

            CampusId = configuration.CampusId,

            AcademicTermType = configuration.AcademicTermType,

            AcademicTermTypeName = configuration.AcademicTermType.ToString(),

            AcademicTermsPerSession = configuration.AcademicTermsPerSession,

            AutoPromoteEnabled = configuration.AutoPromoteEnabled,

            MinimumAttendancePercentage = configuration.MinimumAttendancePercentage,

            AllowAttendanceEditing = configuration.AllowAttendanceEditing,

            AttendanceEditWindowDays = configuration.AttendanceEditWindowDays,

            AutoGenerateAttendanceSessions = configuration.AutoGenerateAttendanceSessions,

            AutoGenerateAttendanceRecords = configuration.AutoGenerateAttendanceRecords,

            AttendanceLockAfterDays = configuration.AttendanceLockAfterDays,

            AllowTeacherAttendanceUnlock = configuration.AllowTeacherAttendanceUnlock,

            LateThresholdMinutes = configuration.LateThresholdMinutes,

            MedicalLeaveCountsAsPresent = configuration.MedicalLeaveCountsAsPresent,

            OnDutyCountsAsPresent = configuration.OnDutyCountsAsPresent,

            AllowStudentAttendanceCorrection = configuration.AllowStudentAttendanceCorrection,
        };
    }

    #endregion
}