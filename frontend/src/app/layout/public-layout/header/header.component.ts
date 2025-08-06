import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { AuthService } from '@core/services/auth.service';
import { LanguageSelectorComponent } from '@shared/components/language-selector/language-selector.component';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    RouterLinkActive,
    TranslocoModule,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatDividerModule,
    LanguageSelectorComponent
  ],
  template: `
    <header class="bg-primary-navy shadow-lg sticky top-0 z-50">
      <nav class="container mx-auto px-4">
        <div class="flex items-center justify-between h-20">
          <!-- Logo -->
          <a routerLink="/" class="flex items-center space-x-2">
            <span class="text-primary-gold text-2xl font-heading font-bold">Empty Legs</span>
            <span class="text-primary-white text-2xl font-heading">Luxury</span>
          </a>

          <!-- Desktop Navigation -->
          <div class="hidden md:flex items-center space-x-8">
            <a routerLink="/" routerLinkActive="text-primary-gold" [routerLinkActiveOptions]="{exact: true}"
               class="text-primary-white hover:text-primary-gold transition-colors">
              {{ 'navigation.home' | transloco }}
            </a>
            <a routerLink="/flights" routerLinkActive="text-primary-gold"
               class="text-primary-white hover:text-primary-gold transition-colors">
              {{ 'navigation.flights' | transloco }}
            </a>
            <a routerLink="/about" routerLinkActive="text-primary-gold"
               class="text-primary-white hover:text-primary-gold transition-colors">
              {{ 'navigation.about' | transloco }}
            </a>
            <a routerLink="/contact" routerLinkActive="text-primary-gold"
               class="text-primary-white hover:text-primary-gold transition-colors">
              {{ 'navigation.contact' | transloco }}
            </a>
          </div>

          <!-- Right Section -->
          <div class="flex items-center space-x-4">
            <app-language-selector></app-language-selector>
            
            @if (authService.isAuthenticated()) {
              <button mat-button [matMenuTriggerFor]="userMenu" class="text-primary-white">
                <mat-icon>account_circle</mat-icon>
                <span class="ml-2 hidden md:inline">{{ currentUser?.firstName }}</span>
              </button>
              <mat-menu #userMenu="matMenu">
                <a mat-menu-item [routerLink]="getDashboardLink()">
                  <mat-icon>dashboard</mat-icon>
                  <span>{{ 'navigation.dashboard' | transloco }}</span>
                </a>
                <a mat-menu-item routerLink="/profile">
                  <mat-icon>person</mat-icon>
                  <span>{{ 'navigation.profile' | transloco }}</span>
                </a>
                <mat-divider></mat-divider>
                <button mat-menu-item (click)="logout()">
                  <mat-icon>logout</mat-icon>
                  <span>{{ 'navigation.logout' | transloco }}</span>
                </button>
              </mat-menu>
            } @else {
              <a routerLink="/auth/login" mat-button class="text-primary-white">
                {{ 'navigation.login' | transloco }}
              </a>
              <a routerLink="/auth/register" mat-raised-button class="btn-premium">
                {{ 'navigation.register' | transloco }}
              </a>
            }

            <!-- Mobile Menu Button -->
            <button mat-icon-button (click)="toggleMobileMenu()" class="md:hidden text-primary-white">
              <mat-icon>{{ mobileMenuOpen ? 'close' : 'menu' }}</mat-icon>
            </button>
          </div>
        </div>

        <!-- Mobile Navigation -->
        @if (mobileMenuOpen) {
          <div class="md:hidden py-4 border-t border-gray-700">
            <div class="flex flex-col space-y-2">
              <a routerLink="/" routerLinkActive="text-primary-gold" [routerLinkActiveOptions]="{exact: true}"
                 class="text-primary-white hover:text-primary-gold transition-colors py-2"
                 (click)="closeMobileMenu()">
                {{ 'navigation.home' | transloco }}
              </a>
              <a routerLink="/flights" routerLinkActive="text-primary-gold"
                 class="text-primary-white hover:text-primary-gold transition-colors py-2"
                 (click)="closeMobileMenu()">
                {{ 'navigation.flights' | transloco }}
              </a>
              <a routerLink="/about" routerLinkActive="text-primary-gold"
                 class="text-primary-white hover:text-primary-gold transition-colors py-2"
                 (click)="closeMobileMenu()">
                {{ 'navigation.about' | transloco }}
              </a>
              <a routerLink="/contact" routerLinkActive="text-primary-gold"
                 class="text-primary-white hover:text-primary-gold transition-colors py-2"
                 (click)="closeMobileMenu()">
                {{ 'navigation.contact' | transloco }}
              </a>
            </div>
          </div>
        }
      </nav>
    </header>
  `,
  styles: []
})
export class HeaderComponent {
  authService = inject(AuthService);
  mobileMenuOpen = false;

  get currentUser() {
    return this.authService.getCurrentUser();
  }

  toggleMobileMenu(): void {
    this.mobileMenuOpen = !this.mobileMenuOpen;
  }

  closeMobileMenu(): void {
    this.mobileMenuOpen = false;
  }

  getDashboardLink(): string {
    const user = this.currentUser;
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

  logout(): void {
    this.authService.logout();
  }
}