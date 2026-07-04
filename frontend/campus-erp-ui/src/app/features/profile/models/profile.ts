export interface Profile {
  userId: string;

  studentId?: string;

  teacherId?: string;

  institutionId: string;

  campusId: string;

  fullName: string;

  email: string;

  phoneNumber: string;

  role: string;

  institutionName: string;

  campusName: string;

  isActive: boolean;

  profilePhotoUrl?: string;

  avatarInitials: string;

  lastLoginAt?: string;

  // Student

  admissionNumber?: string;

  rollNumber?: string;

  academicSession?: string;

  departmentName?: string;

  courseName?: string;

  semesterName?: string;

  sectionName?: string;

  enrollmentStatus?: number;

  enrollmentStatusName?: string;

  // Teacher

  employeeCode?: string;

  designation?: string;
}