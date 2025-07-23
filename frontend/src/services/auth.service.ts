import { api, setTokens, clearTokens } from '@/lib/api';
import { Login, RegisterUser, AuthResponse, User } from '@/types';

export class AuthService {
  // Login user
  static async login(credentials: Login): Promise<AuthResponse> {
    const response = await api.post<AuthResponse>('/auth/login', credentials);
    setTokens(response.data);
    return response.data;
  }

  // Register user
  static async register(userData: RegisterUser): Promise<AuthResponse> {
    const response = await api.post<AuthResponse>('/auth/register', userData);
    setTokens(response.data);
    return response.data;
  }

  // Logout user
  static async logout(): Promise<void> {
    try {
      await api.post('/auth/revoke');
    } catch (error) {
      // Even if revoke fails, clear tokens locally
      console.error('Error revoking token:', error);
    } finally {
      clearTokens();
    }
  }

  // Get current user
  static async getCurrentUser(): Promise<User> {
    const response = await api.get<User>('/auth/me');
    return response.data;
  }

  // Change password
  static async changePassword(currentPassword: string, newPassword: string): Promise<void> {
    await api.post('/auth/change-password', {
      currentPassword,
      newPassword,
    });
  }

  // Request password reset
  static async requestPasswordReset(email: string): Promise<void> {
    await api.post('/auth/reset-password', { email });
  }

  // Confirm email
  static async confirmEmail(userId: string, token: string): Promise<void> {
    await api.post('/auth/confirm-email', { userId, token });
  }

  // Resend email confirmation
  static async resendEmailConfirmation(email: string): Promise<void> {
    await api.post('/auth/resend-confirmation', { email });
  }

  // Check if user is authenticated
  static isAuthenticated(): boolean {
    if (typeof window === 'undefined') return false;
    return !!localStorage.getItem('accessToken');
  }

  // Get stored user
  static getStoredUser(): User | null {
    if (typeof window === 'undefined') return null;
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  }
}