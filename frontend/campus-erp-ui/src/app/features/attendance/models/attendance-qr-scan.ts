export interface AttendanceQrScanResponse {
  success: boolean;

  attendanceSessionId: string;

  attendanceRecordId: string;

  message: string;

  markedOn: string;
}