export interface AttendanceQrSession {
  id: string;

  attendanceSessionId: string;

  token: string;

  validFrom: string;

  expiresOn: string;

  durationSeconds: number;

  isActive: boolean;

  markedCount: number;

  totalStudentCount: number;

  remainingStudentCount: number;
}