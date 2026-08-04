using CampusERP.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace CampusERP.Contracts.Requests;

public class UpdateAcademicConfigurationRequest
{
    #region Academic Structure

    [Required]
    public AcademicTermType AcademicTermType { get; set; }

    [Range(1, 12)]
    public int AcademicTermsPerSession { get; set; }

    public bool AutoPromoteEnabled { get; set; }

    #endregion

    #region Attendance Rules

    [Range(0, 100)]
    public int MinimumAttendancePercentage { get; set; }

    public bool AllowAttendanceEditing { get; set; }

    [Range(0, 365)]
    public int AttendanceEditWindowDays { get; set; }

    #endregion

    #region Attendance Automation

    public bool AutoGenerateAttendanceSessions { get; set; }

    public bool AutoGenerateAttendanceRecords { get; set; }

    #endregion

    #region Attendance Lock

    [Range(0, 365)]
    public int AttendanceLockAfterDays { get; set; }

    public bool AllowTeacherAttendanceUnlock { get; set; }

    #endregion

    #region Attendance Behaviour

    [Range(0, 180)]
    public int LateThresholdMinutes { get; set; }

    public bool MedicalLeaveCountsAsPresent { get; set; }

    public bool OnDutyCountsAsPresent { get; set; }

    #endregion

    #region Student Requests

    public bool AllowStudentAttendanceCorrection { get; set; }

    #endregion
}