export interface AttendanceSession {
  id: string;

  academicSessionId: string;

  teacherAssignmentId?: string | null;

  timetableTemplateId?: string | null;

  lectureOverrideId?: string | null;

  subjectId: string;

  semesterSubjectId: string;

  teacherId: string;

  sectionId: string;

  roomId?: string | null;

  lectureType: number;

  attendanceDate: string;

  startTime: string;

  endTime: string;

  isAttendanceMarked: boolean;

  status: number;

  source: number;

  isLocked: boolean;

  lockedByUserId?: string | null;

  lockedOn?: string | null;

  remarks?: string | null;

  totalStudents: number;

  markedStudents: number;

  records: AttendanceRecord[];
}

export interface AttendanceRecord {
  id: string;

  studentId: string;

  studentName: string;

  rollNumber?: string | null;

  status: number;

  isMarked: boolean;

  markedOn?: string | null;

  markedByUserId?: string | null;

  markingMethod: number;

  remarks?: string | null;
}
