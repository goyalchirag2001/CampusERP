using CampusERP.Shared.Enums;

namespace CampusERP.Contracts.Responses;

public class AcademicConfigurationResponse
{
    #region Basic

    public Guid Id { get; set; }

    public Guid InstitutionId { get; set; }

    public Guid? CampusId { get; set; }

    #endregion

    #region Academic Structure

    public AcademicTermType AcademicTermType { get; set; }

    public string AcademicTermTypeName { get; set; } = string.Empty;

    public int AcademicTermsPerSession { get; set; }

    public bool AutoPromoteEnabled { get; set; }

    #endregion

    #region Attendance Rules

    public int MinimumAttendancePercentage { get; set; }

    public bool AllowAttendanceEditing { get; set; }

    public int AttendanceEditWindowDays { get; set; }

    #endregion

    #region Attendance Automation

    public bool AutoGenerateAttendanceSessions { get; set; }

    public bool AutoGenerateAttendanceRecords { get; set; }

    #endregion

    #region Attendance Lock

    public int AttendanceLockAfterDays { get; set; }

    public bool AllowTeacherAttendanceUnlock { get; set; }

    #endregion

    #region Attendance Behaviour

    public int LateThresholdMinutes { get; set; }

    public bool MedicalLeaveCountsAsPresent { get; set; }

    public bool OnDutyCountsAsPresent { get; set; }

    #endregion

    #region Student Requests

    public bool AllowStudentAttendanceCorrection { get; set; }

    #endregion
}