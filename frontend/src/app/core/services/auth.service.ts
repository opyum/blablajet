import { Injectable, inject, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, BehaviorSubject, tap, map, catchError, of } from 'rxjs';
import { BaseApiService } from './base-api.service';
import { 
  LoginRequest, 
  RegisterRequest, 
  AuthResponse, 
  ForgotPasswordRequest, 
  ResetPasswordRequest,
  ChangePasswordRequest,
  TokenRefreshRequest,
  TokenRefreshResponse
} from '@core/models';

@Injectable({
  providedIn: 'root'
})
export class AuthService extends BaseApiService {
  private router = inject(Router);
  
  private readonly TOKEN_KEY = 'auth_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';
  private readonly USER_KEY = 'current_user';
  
  private currentUserSubject = new BehaviorSubject<any>(this.getUserFromStorage());
  public currentUser$ = this.currentUserSubject.asObservable();
  
  private isAuthenticatedSignal = signal<boolean>(this.hasValidToken());
  public isAuthenticated = computed(() => this.isAuthenticatedSignal());

  constructor() {
    super();
  }

  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.post<AuthResponse>('/api/auth/login', credentials).pipe(
      tap(response => this.handleAuthResponse(response)),
      tap(() => {
        const redirectUrl = this.getRedirectUrl();
        this.router.navigate([redirectUrl]);
      })
    );
  }

  register(data: RegisterRequest): Observable<AuthResponse> {
    return this.post<AuthResponse>('/api/auth/register', data).pipe(
      tap(response => this.handleAuthResponse(response))
    );
  }

  logout(): void {
    this.clearAuthData();
    this.router.navigate(['/auth/login']);
  }

  forgotPassword(data: ForgotPasswordRequest): Observable<any> {
    return this.post('/api/auth/forgot-password', data);
  }

  resetPassword(data: ResetPasswordRequest): Observable<any> {
    return this.post('/api/auth/reset-password', data);
  }

  changePassword(data: ChangePasswordRequest): Observable<any> {
    return this.post('/api/auth/change-password', data);
  }

  refreshToken(): Observable<TokenRefreshResponse> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      return of(null as any);
    }

    return this.post<TokenRefreshResponse>('/api/auth/refresh-token', { refreshToken }).pipe(
      tap(response => {
        if (response) {
          this.setToken(response.token);
          this.setRefreshToken(response.refreshToken);
        }
      }),
      catchError(() => {
        this.logout();
        return of(null as any);
      })
    );
  }

  verifyEmail(token: string): Observable<any> {
    return this.post('/api/auth/verify-email', { token });
  }

  getCurrentUser(): any {
    return this.currentUserSubject.value;
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  isLoggedIn(): boolean {
    return this.isAuthenticatedSignal();
  }

  hasRole(role: string): boolean {
    const user = this.getCurrentUser();
    return user && user.role === role;
  }

  private handleAuthResponse(response: AuthResponse): void {
    this.setToken(response.token);
    this.setRefreshToken(response.refreshToken);
    this.setUser(response.user);
    this.isAuthenticatedSignal.set(true);
  }

  private setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  private setRefreshToken(token: string): void {
    localStorage.setItem(this.REFRESH_TOKEN_KEY, token);
  }

  private setUser(user: any): void {
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
    this.currentUserSubject.next(user);
  }

  private getUserFromStorage(): any {
    const userStr = localStorage.getItem(this.USER_KEY);
    return userStr ? JSON.parse(userStr) : null;
  }

  private clearAuthData(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.currentUserSubject.next(null);
    this.isAuthenticatedSignal.set(false);
  }

  private hasValidToken(): boolean {
    const token = this.getToken();
    if (!token) return false;
    
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expirationDate = new Date(payload.exp * 1000);
      return expirationDate > new Date();
    } catch {
      return false;
    }
  }

  private getRedirectUrl(): string {
    const user = this.getCurrentUser();
    if (!user) return '/';
    
    switch (user.role) {
      case 'Admin':
        return '/admin/dashboard';
      case 'Company':
        return '/company/dashboard';
      case 'Customer':
        return '/customer/dashboard';
      default:
        return '/';
    }
  }
}