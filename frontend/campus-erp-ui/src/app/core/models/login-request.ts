export interface LoginRequest {
  email: string;

  password: string;

  institutionSlug?: string | null;
}