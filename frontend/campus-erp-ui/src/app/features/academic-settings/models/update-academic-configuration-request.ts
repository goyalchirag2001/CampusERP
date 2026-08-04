export interface UpdateAcademicConfigurationRequest {
  // Academic Structure

  academicTermType: number;

  academicTermsPerSession: number;

  autoPromoteEnabled: boolean;

  // Attendance Rules

  minimumAttendancePercentage: number;

  allowAttendanceEditing: boolean;

  attendanceEditWindowDays: number;

  // Attendance Automation

  autoGenerateAttendanceSessions: boolean;

  autoGenerateAttendanceRecords: boolean;

  // Attendance Lock

  attendanceLockAfterDays: number;

  allowTeacherAttendanceUnlock: boolean;

  // Attendance Behaviour

  lateThresholdMinutes: number;

  medicalLeaveCountsAsPresent: boolean;

  onDutyCountsAsPresent: boolean;

  // Student Requests

  allowStudentAttendanceCorrection: boolean;
}