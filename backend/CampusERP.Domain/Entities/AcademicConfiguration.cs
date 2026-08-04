using CampusERP.Domain.Common;
using CampusERP.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace CampusERP.Domain.Entities;

public class AcademicConfiguration : BaseEntity
{
    #region Tenant

    public Guid InstitutionId { get; set; }

    /// <summary>
    /// Null = Institution level configuration.
    /// Value = Campus specific override.
    /// </summary>
    public Guid? CampusId { get; set; }

    #endregion

    #region Academic Structure

    /// <summary>
    /// Semester / Trimester / Quarter / Annual / Custom
    /// </summary>
    public AcademicTermType AcademicTermType { get; set; } = AcademicTermType.Semester;

    /// <summary>
    /// Number of academic terms in one academic session.
    /// Semester = 2
    /// Trimester = 3
    /// Quarter = 4
    /// Annual = 1
    /// Custom = User Defined
    /// </summary>
    [Range(1, 12)]
    public int AcademicTermsPerSession { get; set; } = 2;

    public bool AutoPromoteEnabled { get; set; } = true;

    #endregion

    #region Attendance Rules

    [Range(0, 100)]
    public int MinimumAttendancePercentage { get; set; } = 75;

    public bool AllowAttendanceEditing { get; set; } = true;

    [Range(0, 365)]
    public int AttendanceEditWindowDays { get; set; } = 7;

    #endregion

    #region Attendance Automation

    public bool AutoGenerateAttendanceSessions { get; set; } = true;

    public bool AutoGenerateAttendanceRecords { get; set; } = true;

    #endregion

    #region Attendance Lock

    [Range(0, 365)]
    public int AttendanceLockAfterDays { get; set; } = 7;

    public bool AllowTeacherAttendanceUnlock { get; set; }

    #endregion

    #region Attendance Behaviour

    [Range(0, 180)]
    public int LateThresholdMinutes { get; set; } = 10;

    public bool MedicalLeaveCountsAsPresent { get; set; }

    public bool OnDutyCountsAsPresent { get; set; } = true;

    #endregion

    #region Student Requests

    public bool AllowStudentAttendanceCorrection { get; set; } = true;

    #endregion

    #region Navigation

    public Institution Institution { get; set; } = null!;

    public Campus? Campus { get; set; }

    #endregion
}