export interface CreateCampusRequest {
  institutionId: string;

  name: string;

  code: string;

  email?: string;

  phone?: string;

  address?: string;

  campusHeadName?: string;
}
