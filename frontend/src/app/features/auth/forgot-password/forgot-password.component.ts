import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { AuthService } from '@core/services/auth.service';
import { NotificationService } from '@core/services/notification.service';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-forgot-password',
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
    MatFormFieldModule
  ],
  template: `
    <div class="text-center mb-8">
      <mat-icon class="text-6xl text-primary-gold mb-4">lock_reset</mat-icon>
      <h1 class="text-3xl font-heading font-bold text-primary-navy mb-2">
        {{ 'auth.forgotPassword.title' | transloco }}
      </h1>
      <p class="text-gray-600">
        {{ 'auth.forgotPassword.subtitle' | transloco }}
      </p>
    </div>

    @if (!emailSent) {
      <form [formGroup]="forgotPasswordForm" (ngSubmit)="onSubmit()">
        <!-- Email -->
        <mat-form-field appearance="outline" class="w-full mb-6">
          <mat-label>{{ 'auth.forgotPassword.email' | transloco }}</mat-label>
          <input 
            matInput 
            type="email" 
            formControlName="email"
            autocomplete="email">
          <mat-icon matPrefix>email</mat-icon>
          <mat-hint>
            Entrez l'adresse email associée à votre compte
          </mat-hint>
          @if (forgotPasswordForm.get('email')?.hasError('required') && forgotPasswordForm.get('email')?.touched) {
            <mat-error>{{ 'validation.required' | transloco }}</mat-error>
          }
          @if (forgotPasswordForm.get('email')?.hasError('email') && forgotPasswordForm.get('email')?.touched) {
            <mat-error>{{ 'validation.email' | transloco }}</mat-error>
          }
        </mat-form-field>

        <!-- Submit Button -->
        <button 
          mat-raised-button 
          type="submit" 
          class="w-full btn-premium mb-4"
          [disabled]="!forgotPasswordForm.valid || isLoading">
          @if (isLoading) {
            <ng-container>
              <mat-icon class="animate-spin mr-2">refresh</mat-icon>
              Envoi en cours...
            </ng-container>
          } @else {
            <ng-container>
              <mat-icon class="mr-2">send</mat-icon>
              {{ 'auth.forgotPassword.submit' | transloco }}
            </ng-container>
          }
        </button>

        <!-- Back to Login -->
        <div class="text-center">
          <a 
            routerLink="/auth/login" 
            class="text-sm text-gray-600 hover:text-primary-gold">
            <mat-icon class="align-middle text-base">arrow_back</mat-icon>
            {{ 'auth.forgotPassword.backToLogin' | transloco }}
          </a>
        </div>
      </form>
    } @else {
      <!-- Success Message -->
      <div class="text-center">
        <mat-icon class="text-6xl text-accent-green mb-4">check_circle</mat-icon>
        <h2 class="text-2xl font-semibold text-primary-navy mb-4">
          Email envoyé !
        </h2>
        <p class="text-gray-600 mb-6">
          Nous avons envoyé un lien de réinitialisation à <strong>{{ submittedEmail }}</strong>. 
          Veuillez vérifier votre boîte de réception et suivre les instructions.
        </p>
        <p class="text-sm text-gray-500 mb-6">
          Vous n'avez pas reçu l'email ? Vérifiez votre dossier spam ou 
          <button 
            mat-button 
            color="primary" 
            (click)="resendEmail()"
            [disabled]="isLoading">
            renvoyez l'email
          </button>
        </p>
        <a 
          routerLink="/auth/login" 
          mat-raised-button 
          class="btn-secondary">
          {{ 'auth.forgotPassword.backToLogin' | transloco }}
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
export class ForgotPasswordComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private notificationService = inject(NotificationService);
  
  isLoading = false;
  emailSent = false;
  submittedEmail = '';
  
  forgotPasswordForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]]
  });

  onSubmit(): void {
    if (this.forgotPasswordForm.valid && !this.isLoading) {
      this.isLoading = true;
      this.submittedEmail = this.forgotPasswordForm.value.email!;
      
      this.authService.forgotPassword({ email: this.submittedEmail })
        .pipe(finalize(() => this.isLoading = false))
        .subscribe({
          next: () => {
            this.emailSent = true;
            this.notificationService.successTranslated('auth.forgotPassword.emailSent');
          },
          error: (error) => {
            console.error('Forgot password error:', error);
            // Show success even on error to prevent email enumeration
            this.emailSent = true;
          }
        });
    }
  }

  resendEmail(): void {
    if (!this.isLoading && this.submittedEmail) {
      this.isLoading = true;
      
      this.authService.forgotPassword({ email: this.submittedEmail })
        .pipe(finalize(() => this.isLoading = false))
        .subscribe({
          next: () => {
            this.notificationService.success('Email renvoyé avec succès');
          },
          error: (error) => {
            console.error('Resend email error:', error);
            this.notificationService.success('Email renvoyé avec succès');
          }
        });
    }
  }
}