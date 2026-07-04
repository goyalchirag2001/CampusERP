export interface Teacher {
  id: string;

  userId: string;

  institutionId: string;

  campusId: string;

  departmentId: string;

  campusName: string;

  departmentName: string;

  employeeCode: string;

  designation: string;

  firstName: string;

  lastName: string;

  email: string;

  phoneNumber?: string;

  isActive: boolean;

  temporaryPassword?: string;
}
