export interface CreateUserRequest {
  institutionId: string;

  campusId: string;

  firstName: string;

  lastName: string;

  email: string;

  phoneNumber?: string;

  roleIds: string[];
}
