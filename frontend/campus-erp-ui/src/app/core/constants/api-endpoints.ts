export const ApiEndpoints = {
  //#region Authentication

  Auth: 'auth',

  //#endregion

  //#region Academic

  AcademicConfiguration: 'academicconfiguration',

  AcademicSessions: 'academicsessions',

  CalendarEvents: 'calendarevents',

  AttendanceCorrectionRequests: 'attendancecorrectionrequests',

  TeacherAssignments: 'teacherassignments',

  //#endregion

  //#region Master Data

  Students: 'students',

  Teachers: 'teachers',

  Subjects: 'subjects',

  Departments: 'departments',

  Courses: 'courses',

  Sections: 'sections',

  Campuses: 'campuses',

  Institutions: 'institutions',

  Users: 'users',

  Roles: 'roles',

  //#endregion
} as const;
