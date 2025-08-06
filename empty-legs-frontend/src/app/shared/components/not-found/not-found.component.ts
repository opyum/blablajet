import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatButtonModule,
    MatIconModule
  ],
  template: `
    <div class="not-found">
      <div class="not-found__content">
        <mat-icon class="not-found__icon">flight_land</mat-icon>
        <h1 class="not-found__title">404</h1>
        <h2 class="not-found__subtitle">Page non trouvée</h2>
        <p class="not-found__message">
          Désolé, la page que vous recherchez n'existe pas ou a été déplacée.
        </p>
        <div class="not-found__actions">
          <button mat-raised-button color="primary" routerLink="/home">
            <mat-icon>home</mat-icon>
            Retour à l'accueil
          </button>
          <button mat-stroked-button routerLink="/search">
            <mat-icon>search</mat-icon>
            Rechercher des vols
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .not-found {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 2rem;
      background: linear-gradient(145deg, #ffffff, #f8f9fa);
    }
    
    .not-found__content {
      text-align: center;
      max-width: 600px;
    }
    
    .not-found__icon {
      font-size: 4rem;
      color: #d4af37;
      margin-bottom: 1rem;
    }
    
    .not-found__title {
      font-size: 4rem;
      font-weight: 700;
      color: #1a1a2e;
      margin-bottom: 0.5rem;
    }
    
    .not-found__subtitle {
      font-size: 1.5rem;
      color: #36454f;
      margin-bottom: 1rem;
    }
    
    .not-found__message {
      color: #6b7280;
      margin-bottom: 2rem;
      line-height: 1.6;
    }
    
    .not-found__actions {
      display: flex;
      gap: 1rem;
      justify-content: center;
      flex-wrap: wrap;
    }
  `]
})
export class NotFoundComponent {}