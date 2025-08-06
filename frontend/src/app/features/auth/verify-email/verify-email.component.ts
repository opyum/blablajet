import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router, ActivatedRoute } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '@core/services/auth.service';
import { NotificationService } from '@core/services/notification.service';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    TranslocoModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  template: `
    <div class="text-center">
      @if (isVerifying) {
        <!-- Verifying -->
        <mat-spinner class="mx-auto mb-4"></mat-spinner>
        <h1 class="text-2xl font-heading font-bold text-primary-navy mb-2">
          Vérification en cours...
        </h1>
        <p class="text-gray-600">
          Veuillez patienter pendant que nous vérifions votre email.
        </p>
      } @else if (verificationSuccess) {
        <!-- Success -->
        <mat-icon class="text-6xl text-accent-green mb-4">check_circle</mat-icon>
        <h1 class="text-3xl font-heading font-bold text-primary-navy mb-2">
          Email vérifié !
        </h1>
        <p class="text-gray-600 mb-6">
          Votre adresse email a été vérifiée avec succès. 
          Vous pouvez maintenant vous connecter à votre compte.
        </p>
        <a 
          routerLink="/auth/login" 
          mat-raised-button 
          class="btn-premium">
          Se connecter
        </a>
      } @else {
        <!-- Error -->
        <mat-icon class="text-6xl text-accent-red mb-4">error_outline</mat-icon>
        <h1 class="text-3xl font-heading font-bold text-primary-navy mb-2">
          Erreur de vérification
        </h1>
        <p class="text-gray-600 mb-6">
          {{ errorMessage }}
        </p>
        <div class="flex flex-col sm:flex-row gap-4 justify-center">
          <button 
            mat-raised-button 
            class="btn-premium"
            (click)="resendVerification()"
            [disabled]="isResending">
            @if (isResending) {
              <ng-container>
                <mat-icon class="animate-spin mr-2">refresh</mat-icon>
                Envoi en cours...
              </ng-container>
            } @else {
              Renvoyer l'email
            }
          </button>
          <a 
            routerLink="/auth/login" 
            mat-stroked-button>
            Retour à la connexion
          </a>
        </div>
      }
    </div>
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
export class VerifyEmailComponent implements OnInit {
  private authService = inject(AuthService);
  private notificationService = inject(NotificationService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  
  isVerifying = true;
  verificationSuccess = false;
  isResending = false;
  errorMessage = 'Le lien de vérification est invalide ou a expiré.';
  token = '';

  ngOnInit(): void {
    // Get token from query params
    this.token = this.route.snapshot.queryParams['token'];
    
    if (!this.token) {
      this.isVerifying = false;
      this.verificationSuccess = false;
      this.errorMessage = 'Aucun token de vérification fourni.';
      return;
    }
    
    this.verifyEmail();
  }

  private verifyEmail(): void {
    this.authService.verifyEmail(this.token).subscribe({
      next: () => {
        this.isVerifying = false;
        this.verificationSuccess = true;
        this.notificationService.success('Email vérifié avec succès !');
      },
      error: (error) => {
        console.error('Email verification error:', error);
        this.isVerifying = false;
        this.verificationSuccess = false;
        
        if (error.status === 400) {
          this.errorMessage = 'Le lien de vérification est invalide ou a expiré.';
        } else if (error.status === 404) {
          this.errorMessage = 'Token de vérification non trouvé.';
        } else {
          this.errorMessage = 'Une erreur est survenue lors de la vérification.';
        }
      }
    });
  }

  resendVerification(): void {
    if (!this.isResending) {
      this.isResending = true;
      
      // TODO: Implement resend verification email
      // This would typically require the user's email
      setTimeout(() => {
        this.notificationService.info('Fonctionnalité bientôt disponible. Veuillez vous reconnecter pour recevoir un nouveau lien.');
        this.isResending = false;
        this.router.navigate(['/auth/login']);
      }, 1500);
    }
  }
}