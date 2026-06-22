export interface Campus {
  id: string;

  institutionId: string;

  institutionName: string;

  name: string;

  code: string;

  email?: string;

  phone?: string;

  address?: string;

  campusHeadName?: string;

  departmentCount: number;

  teacherCount: number;

  studentCount: number;

  isActive: boolean;
}
