import { EventType } from './event-type';

export interface CalendarEvent {
  id: string;

  institutionId: string;

  campusId: string;

  campusName: string;

  departmentId: string | null;

  departmentName: string | null;

  courseId: string | null;

  courseName: string | null;

  semesterId: string | null;

  semesterName: string | null;

  sectionId: string | null;

  sectionName: string | null;

  teacherId: string | null;

  teacherName: string | null;

  roomId: string | null;

  roomName: string | null;

  academicSessionId: string;

  academicSessionName: string;

  title: string;

  description: string | null;

  eventType: EventType;

  startDate: string;

  endDate: string;

  startTime: string | null;

  endTime: string | null;

  isFullDay: boolean;

  isRecurring: boolean;

  recurrenceRule: string | null;

  priority: number;

  affectsTimetable: boolean;

  isActive: boolean;
}