export interface AcademicConfiguration {
  // Basic

  id: string;

  institutionId: string;

  campusId: string | null;

  // Academic Structure

  academicTermType: number;

  academicTermTypeName: string;

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