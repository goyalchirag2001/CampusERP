export interface User {
  id: string;

  institutionId: string;

  institutionName: string;

  campusId: string;

  campusName: string;

  firstName: string;

  lastName: string;

  email: string;

  phoneNumber?: string;

  roles: string[];

  roleIds: string[];

  temporaryPassword?: string;

  isActive: boolean;
}
