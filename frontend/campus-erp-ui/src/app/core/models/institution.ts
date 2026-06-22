export interface Institution {
  id: string;

  name: string;

  code: string;

  loginSlug: string;

  email?: string;

  phone?: string;

  website?: string;

  address?: string;

  logoUrl?: string;

  primaryColor?: string;

  secondaryColor?: string;

  campusCount: number;

  studentCount: number;

  teacherCount: number;

  adminFirstName: string;

  adminLastName: string;

  adminEmail: string;

  isActive: boolean;
}