import { Semester } from "../../semesters/models/semester";

export interface Course {
  id: string;

  institutionId: string;

  campusId: string;

  departmentId: string;

  campusName: string;

  departmentName: string;

  name: string;

  code: string;

  degreeType: string;

  durationYears: number;

  totalSemesters: number;

  isActive: boolean;

  semesters: Semester[];
}
