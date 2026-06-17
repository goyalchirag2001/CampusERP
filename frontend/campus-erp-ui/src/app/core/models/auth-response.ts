export interface AuthResponse {
  userId: string;

  firstName: string;

  lastName: string;

  email: string;

  institutionSlug?: string | null;

  accessToken: string;

  refreshToken: string;

  expiresAt: string;
}