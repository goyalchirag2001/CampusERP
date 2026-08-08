import { DayOfWeekType } from './day-of-week-type';
import { LectureType } from './lecture-type';

export interface TimetableTemplate {
  id: string;

  institutionId: string;

  campusId: string;
  campusName: string;

  academicSessionId: string;
  academicSessionName: string;

  teacherAssignmentId: string;

  teacherId: string;
  teacherName: string;

  sectionId: string;
  sectionName: string;

  semesterSubjectId: string;

  subjectId: string;
  subjectCode: string;
  subjectName: string;

  roomId: string | null;
  roomName: string | null;

  dayOfWeek: DayOfWeekType;

  startTime: string;

  endTime: string;

  validFrom: string;

  validTo: string;

  lectureType: LectureType;

  priority: number;

  generateAttendance: boolean;

  isOnline: boolean;

  meetingLink: string | null;

  remarks: string | null;

  displayOrder: number;

  isActive: boolean;
}
