import { DayOfWeekType } from './day-of-week-type';
import { LectureType } from './lecture-type';

export interface CreateTimetableTemplateRequest {
  teacherAssignmentId: string;

  roomId: string | null;

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
}
