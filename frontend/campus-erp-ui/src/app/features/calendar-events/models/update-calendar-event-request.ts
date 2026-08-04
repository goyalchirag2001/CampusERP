import { EventType } from './event-type';

export interface UpdateCalendarEventRequest {
  campusId: string | null;

  departmentId: string | null;

  courseId: string | null;

  semesterId: string | null;

  sectionId: string | null;

  teacherId: string | null;

  roomId: string | null;

  academicSessionId: string;

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
}