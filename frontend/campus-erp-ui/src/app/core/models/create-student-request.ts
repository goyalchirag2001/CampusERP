export interface CreateStudentRequest {
  institutionId: string;
  campusId: string;
  departmentId: string;
  courseId: string;

  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;

  password: string;

  rollNumber: string;
  batch: string;
  admissionDate: string;
}