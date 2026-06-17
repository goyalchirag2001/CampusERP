export interface CurrentUser {
  userId: string;

  firstName: string;

  lastName: string;

  email: string;

  institutionId: string;

  campusId: string;

  institutionSlug: string | null;

  roles: string[];
}
