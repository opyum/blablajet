import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators, AbstractControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { AuthService } from '@core/services/auth.service';
import { NotificationService } from '@core/services/notification.service';
import { finalize } from 'rxjs/operators';
import { passwordMatchValidator } from '../register/register.component';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslocoModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule
  ],
  template: `
    <div class="text-center mb-8">
      <mat-icon class="text-6xl text-primary-gold mb-4">lock_open</mat-icon>
      <h1 class="text-3xl font-heading font-bold text-primary-navy mb-2">
        {{ 'auth.resetPassword.title' | transloco }}
      </h1>
      <p class="text-gray-600">
        Créez un nouveau mot de passe pour votre compte
      </p>
    </div>

    @if (isValidToken) {
      <form [formGroup]="resetPasswordForm" (ngSubmit)="onSubmit()">
        <!-- New Password -->
        <mat-form-field appearance="outline" class="w-full mb-4">
          <mat-label>{{ 'auth.resetPassword.newPassword' | transloco }}</mat-label>
          <input 
            matInput 
            [type]="showPassword ? 'text' : 'password'" 
            formControlName="newPassword"
            autocomplete="new-password">
          <mat-icon matPrefix>lock</mat-icon>
          <button 
            mat-icon-button 
            matSuffix 
            type="button"
            (click)="showPassword = !showPassword">
            <mat-icon>{{ showPassword ? 'visibility_off' : 'visibility' }}</mat-icon>
          </button>
          @if (resetPasswordForm.get('newPassword')?.hasError('required') && resetPasswordForm.get('newPassword')?.touched) {
            <mat-error>{{ 'validation.required' | transloco }}</mat-error>
          }
          @if (resetPasswordForm.get('newPassword')?.hasError('minlength') && resetPasswordForm.get('newPassword')?.touched) {
            <mat-error>{{ 'validation.minLength' | transloco : { min: 8 } }}</mat-error>
          }
        </mat-form-field>

        <!-- Confirm Password -->
        <mat-form-field appearance="outline" class="w-full mb-6">
          <mat-label>{{ 'auth.resetPassword.confirmPassword' | transloco }}</mat-label>
          <input 
            matInput 
            [type]="showPassword ? 'text' : 'password'" 
            formControlName="confirmPassword"
            autocomplete="new-password">
          <mat-icon matPrefix>lock</mat-icon>
          @if (resetPasswordForm.get('confirmPassword')?.hasError('required') && resetPasswordForm.get('confirmPassword')?.touched) {
            <mat-error>{{ 'validation.required' | transloco }}</mat-error>
          }
          @if (resetPasswordForm.hasError('passwordMismatch') && resetPasswordForm.get('confirmPassword')?.touched) {
            <mat-error>{{ 'validation.passwordMatch' | transloco }}</mat-error>
          }
        </mat-form-field>

        <!-- Password Requirements -->
        <div class="bg-gray-50 rounded-lg p-4 mb-6">
          <p class="text-sm font-semibold text-gray-700 mb-2">Le mot de passe doit contenir :</p>
          <ul class="text-sm text-gray-600 space-y-1">
            <li class="flex items-center gap-2">
              <mat-icon class="text-base" [class.text-accent-green]="passwordLength >= 8">
                {{ passwordLength >= 8 ? 'check_circle' : 'radio_button_unchecked' }}
              </mat-icon>
              Au moins 8 caractères
            </li>
          </ul>
        </div>

        <!-- Submit Button -->
        <button 
          mat-raised-button 
          type="submit" 
          class="w-full btn-premium"
          [disabled]="!resetPasswordForm.valid || isLoading">
          @if (isLoading) {
            <ng-container>
              <mat-icon class="animate-spin mr-2">refresh</mat-icon>
              Réinitialisation en cours...
            </ng-container>
          } @else {
            {{ 'auth.resetPassword.submit' | transloco }}
          }
        </button>
      </form>
    } @else {
      <!-- Invalid Token -->
      <div class="text-center">
        <mat-icon class="text-6xl text-accent-red mb-4">error_outline</mat-icon>
        <h2 class="text-2xl font-semibold text-primary-navy mb-4">
          Lien invalide ou expiré
        </h2>
        <p class="text-gray-600 mb-6">
          Ce lien de réinitialisation est invalide ou a expiré. 
          Veuillez demander un nouveau lien de réinitialisation.
        </p>
        <a 
          routerLink="/auth/forgot-password" 
          mat-raised-button 
          class="btn-premium">
          Demander un nouveau lien
        </a>
      </div>
    }
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
export class ResetPasswordComponent implements OnInit {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private notificationService = inject(NotificationService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  
  isLoading = false;
  showPassword = false;
  isValidToken = true;
  token = '';
  
  resetPasswordForm = this.fb.group({
    newPassword: ['', [Validators.required, Validators.minLength(8)]],
    confirmPassword: ['', Validators.required]
  }, { validators: passwordMatchValidator });

  get passwordLength(): number {
    return this.resetPasswordForm.get('newPassword')?.value?.length || 0;
  }

  ngOnInit(): void {
    // Get token from query params
    this.token = this.route.snapshot.queryParams['token'];
    
    if (!this.token) {
      this.isValidToken = false;
    }
    
    // TODO: Validate token with backend
  }

  onSubmit(): void {
    if (this.resetPasswordForm.valid && !this.isLoading && this.token) {
      this.isLoading = true;
      
      const resetData = {
        token: this.token,
        newPassword: this.resetPasswordForm.value.newPassword!
      };
      
      this.authService.resetPassword(resetData)
        .pipe(finalize(() => this.isLoading = false))
        .subscribe({
          next: () => {
            this.notificationService.successTranslated('auth.resetPassword.resetSuccess');
            this.router.navigate(['/auth/login']);
          },
          error: (error) => {
            console.error('Reset password error:', error);
            if (error.status === 400) {
              this.isValidToken = false;
            } else {
              this.notificationService.errorTranslated('auth.resetPassword.resetError');
            }
          }
        });
    }
  }
}