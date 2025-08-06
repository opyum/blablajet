import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink, Router, ActivatedRoute } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDividerModule } from '@angular/material/divider';
import { AuthService } from '@core/services/auth.service';
import { NotificationService } from '@core/services/notification.service';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    TranslocoModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatCheckboxModule,
    MatDividerModule
  ],
  template: `
    <div class="text-center mb-8">
      <h1 class="text-3xl font-heading font-bold text-primary-navy mb-2">
        {{ 'auth.login.title' | transloco }}
      </h1>
      <p class="text-gray-600">
        {{ 'auth.login.subtitle' | transloco }}
      </p>
    </div>

    <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
      <!-- Email -->
      <mat-form-field appearance="outline" class="w-full mb-4">
        <mat-label>{{ 'auth.login.email' | transloco }}</mat-label>
        <input 
          matInput 
          type="email" 
          formControlName="email"
          autocomplete="email">
        <mat-icon matPrefix>email</mat-icon>
        @if (loginForm.get('email')?.hasError('required') && loginForm.get('email')?.touched) {
          <mat-error>{{ 'validation.required' | transloco }}</mat-error>
        }
        @if (loginForm.get('email')?.hasError('email') && loginForm.get('email')?.touched) {
          <mat-error>{{ 'validation.email' | transloco }}</mat-error>
        }
      </mat-form-field>

      <!-- Password -->
      <mat-form-field appearance="outline" class="w-full mb-4">
        <mat-label>{{ 'auth.login.password' | transloco }}</mat-label>
        <input 
          matInput 
          [type]="showPassword ? 'text' : 'password'" 
          formControlName="password"
          autocomplete="current-password">
        <mat-icon matPrefix>lock</mat-icon>
        <button 
          mat-icon-button 
          matSuffix 
          type="button"
          (click)="showPassword = !showPassword">
          <mat-icon>{{ showPassword ? 'visibility_off' : 'visibility' }}</mat-icon>
        </button>
        @if (loginForm.get('password')?.hasError('required') && loginForm.get('password')?.touched) {
          <mat-error>{{ 'validation.required' | transloco }}</mat-error>
        }
      </mat-form-field>

      <!-- Remember Me & Forgot Password -->
      <div class="flex justify-between items-center mb-6">
        <mat-checkbox formControlName="rememberMe" color="primary">
          {{ 'auth.login.rememberMe' | transloco }}
        </mat-checkbox>
        <a 
          routerLink="/auth/forgot-password" 
          class="text-sm text-primary-gold hover:underline">
          {{ 'auth.login.forgotPassword' | transloco }}
        </a>
      </div>

      <!-- Submit Button -->
      <button 
        mat-raised-button 
        type="submit" 
        class="w-full btn-premium mb-4"
        [disabled]="!loginForm.valid || isLoading">
        @if (isLoading) {
          <ng-container>
            <mat-icon class="animate-spin mr-2">refresh</mat-icon>
            Connexion en cours...
          </ng-container>
        } @else {
          {{ 'auth.login.submit' | transloco }}
        }
      </button>

      <!-- Social Login -->
      <div class="relative my-6">
        <mat-divider></mat-divider>
        <span class="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 bg-white px-4 text-gray-500 text-sm">
          OU
        </span>
      </div>

      <div class="grid grid-cols-2 gap-4 mb-6">
        <button 
          mat-stroked-button 
          type="button"
          class="w-full"
          (click)="socialLogin('google')">
          <img src="/assets/icons/google.svg" alt="Google" class="w-5 h-5 mr-2">
          Google
        </button>
        <button 
          mat-stroked-button 
          type="button"
          class="w-full"
          (click)="socialLogin('facebook')">
          <img src="/assets/icons/facebook.svg" alt="Facebook" class="w-5 h-5 mr-2">
          Facebook
        </button>
      </div>

      <!-- Sign Up Link -->
      <p class="text-center text-sm text-gray-600">
        {{ 'auth.login.noAccount' | transloco }}
        <a 
          routerLink="/auth/register" 
          class="text-primary-gold font-semibold hover:underline">
          {{ 'auth.login.signUp' | transloco }}
        </a>
      </p>
    </form>
  `,
  styles: [`
    @keyframes spin {
      to { transform: rotate(360deg); }
    }
    
    .animate-spin {
      animation: spin 1s linear infinite;
    }
  `]
})
export class LoginComponent implements OnInit {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private notificationService = inject(NotificationService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  
  isLoading = false;
  showPassword = false;
  returnUrl = '/';
  
  loginForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
    rememberMe: [false]
  });

  ngOnInit(): void {
    // Get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  onSubmit(): void {
    if (this.loginForm.valid && !this.isLoading) {
      this.isLoading = true;
      
      this.authService.login(this.loginForm.value as any)
        .pipe(finalize(() => this.isLoading = false))
        .subscribe({
          next: () => {
            this.notificationService.successTranslated('auth.login.loginSuccess');
            // Navigation is handled by the auth service
          },
          error: (error) => {
            console.error('Login error:', error);
            this.notificationService.errorTranslated('auth.login.loginError');
          }
        });
    }
  }

  socialLogin(provider: string): void {
    // TODO: Implement social login
    this.notificationService.info(`Connexion ${provider} bientôt disponible`);
  }
}