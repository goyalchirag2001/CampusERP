export interface UpdateCampusRequest {
  name: string;

  code: string;

  email?: string;

  phone?: string;

  address?: string;

  campusHeadName?: string;
}