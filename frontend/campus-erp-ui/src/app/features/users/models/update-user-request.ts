export interface UpdateUserRequest {
  campusId: string;

  firstName: string;

  lastName: string;

  email: string;

  phoneNumber?: string;

  roleIds: string[];
}