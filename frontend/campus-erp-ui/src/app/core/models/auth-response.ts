export interface AuthResponse {
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
}