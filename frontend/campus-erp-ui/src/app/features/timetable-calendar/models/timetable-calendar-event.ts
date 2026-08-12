export interface TimetableCalendarEvent {
  id: string;

  timetableTemplateId?: string | null;

  calendarEventId?: string | null;

  date: string;

  startTime?: string | null;

  endTime?: string | null;

  title: string;

  description?: string | null;

  eventType?: number | null;

  subjectCode?: string | null;

  subjectName?: string | null;

  teacherId?: string | null;

  teacherName?: string | null;

  sectionId?: string | null;

  sectionName?: string | null;

  roomId?: string | null;

  roomBuilding?: string | null;

  roomFloor?: string | null;

  roomNumber?: string | null;
  
  roomName?: string | null;

  lectureType?: number | null;

  priority: number;

  generateAttendance?: boolean;

  isOnline?: boolean;

  meetingLink?: string | null;

  isFullDay: boolean;

  color?: string | null;

  isOverride: boolean;

  isCancelled: boolean;

  overrideReason?: string | null;
}
