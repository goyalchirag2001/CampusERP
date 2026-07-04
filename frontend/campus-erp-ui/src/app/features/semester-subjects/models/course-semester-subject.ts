import { SemesterSubject } from './semester-subject';

export interface CourseSemesterSubject {
  semesterId: string;

  semesterName: string;

  sequenceNumber: number;

  subjects: SemesterSubject[];
}