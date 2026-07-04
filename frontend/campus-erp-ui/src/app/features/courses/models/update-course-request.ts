export interface UpdateCourseRequest {
  institutionId: string;

  campusId: string;

  departmentId: string;

  name: string;

  code: string;

  degreeType: string;

  durationYears: number;

  totalSemesters: number;
}
