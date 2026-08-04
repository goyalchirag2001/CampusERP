namespace CampusERP.Application.Common.Exceptions;

public static class ErrorCodes
{
    #region General

    public const string Validation = "VALIDATION_ERROR";

    public const string NotFound = "NOT_FOUND";

    public const string Conflict = "CONFLICT";

    public const string Unauthorized = "UNAUTHORIZED";

    public const string Forbidden = "FORBIDDEN";

    public const string BusinessRule = "BUSINESS_RULE";

    #endregion

    #region Attendance

    public const string AttendanceRecordNotFound = "ATTENDANCE_RECORD_NOT_FOUND";

    public const string AttendanceCorrectionNotFound = "ATTENDANCE_CORRECTION_NOT_FOUND";

    public const string AttendanceCorrectionAlreadyExists = "ATTENDANCE_CORRECTION_ALREADY_EXISTS";

    public const string AttendanceCorrectionExpired = "ATTENDANCE_CORRECTION_EXPIRED";

    public const string AttendanceCorrectionDisabled = "ATTENDANCE_CORRECTION_DISABLED";

    public const string AttendanceSessionLocked = "ATTENDANCE_SESSION_LOCKED";

    public const string AttendanceAlreadySameStatus = "ATTENDANCE_ALREADY_IN_REQUESTED_STATUS";

    public const string AttendanceRequestAlreadyProcessed = "ATTENDANCE_REQUEST_ALREADY_PROCESSED";

    public const string AttendanceRequestForbidden = "ATTENDANCE_REQUEST_FORBIDDEN";

    public const string TimetableTemplateHasAttendance = "TIMETABLE_TEMPLATE_HAS_ATTENDANCE";

    public const string TimetableTemplateHasOverrides = "TIMETABLE_TEMPLATE_HAS_OVERRIDES";

    #endregion

    #region Calendar

    public const string CalendarEventNotFound = "CALENDAR_EVENT_NOT_FOUND";

    public const string CalendarEventTitleRequired = "CALENDAR_EVENT_TITLE_REQUIRED";

    public const string CalendarEventInvalidDateRange = "CALENDAR_EVENT_INVALID_DATE_RANGE";

    public const string CalendarEventStartTimeRequired = "CALENDAR_EVENT_START_TIME_REQUIRED";

    public const string CalendarEventEndTimeRequired = "CALENDAR_EVENT_END_TIME_REQUIRED";

    public const string CalendarEventInvalidTimeRange = "CALENDAR_EVENT_INVALID_TIME_RANGE";

    public const string CalendarEventInvalidPriority = "CALENDAR_EVENT_INVALID_PRIORITY";

    public const string CalendarEventRecurrenceRuleRequired = "CALENDAR_EVENT_RECURRENCE_REQUIRED";

    public const string CalendarEventOutsideAcademicSession = "CALENDAR_EVENT_OUTSIDE_ACADEMIC_SESSION";

    public const string CalendarScheduleConflict = "CALENDAR_SCHEDULE_CONFLICT";

    public const string InvalidAcademicSession = "CALENDAR_INVALID_ACADEMIC_SESSION";

    public const string InvalidDepartment = "CALENDAR_INVALID_DEPARTMENT";

    public const string InvalidCourse = "CALENDAR_INVALID_COURSE";

    public const string InvalidSemester = "CALENDAR_INVALID_SEMESTER";

    public const string InvalidSection = "CALENDAR_INVALID_SECTION";

    public const string InvalidTeacher = "CALENDAR_INVALID_TEACHER";

    public const string InvalidRoom = "CALENDAR_INVALID_ROOM";

    public const string InvalidLectureTime = "CALENDAR_INVALID_LECTURE_TIME";

    public const string InvalidLectureValidity = "CALENDAR_INVALID_LECTURE_VALIDITY";

    public const string MissingMeetingLink = "CALENDAR_MISSING_MEETING_LINK";

    public const string RoomRequired = "ROOM_REQUIRED";

    #endregion

    #region Master Data

    public const string AcademicSessionNotFound = "ACADEMIC_SESSION_NOT_FOUND";

    public const string CampusNotFound = "CAMPUS_NOT_FOUND";

    public const string DepartmentNotFound = "DEPARTMENT_NOT_FOUND";

    public const string CourseNotFound = "COURSE_NOT_FOUND";

    public const string SemesterNotFound = "SEMESTER_NOT_FOUND";

    public const string SectionNotFound = "SECTION_NOT_FOUND";
  
    public const string TeacherNotFound = "TEACHER_NOT_FOUND";

    public const string RoomNotFound = "ROOM_NOT_FOUND";

    public const string SubjectNotFound = "SUBJECT_NOT_FOUND";

    public const string SemesterSubjectNotFound = "SEMESTER_SUBJECT_NOT_FOUND";

    public const string TimetableTemplateNotFound = "TIMETABLE_TEMPLATE_NOT_FOUND";

    public const string TeacherAssignmentNotFound = "TEACHER_ASSIGNMENT_NOT_FOUND";

    public const string InvalidTeacherAssignment = "INVALID_TEACHER_ASSIGNMENT";

    #endregion

    #region Authentication

    public const string EmailAlreadyExists = "EMAIL_ALREADY_EXISTS";

    public const string InvalidCredentials = "INVALID_CREDENTIALS";

    public const string InstitutionNotFound = "INSTITUTION_NOT_FOUND";

    public const string RefreshTokenInvalid = "REFRESH_TOKEN_INVALID";

    public const string RefreshTokenExpired = "REFRESH_TOKEN_EXPIRED";

    public const string RefreshTokenRevoked = "REFRESH_TOKEN_REVOKED";

    public const string CurrentUserNotFound = "CURRENT_USER_NOT_FOUND";

    #endregion
}