import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { AuthService } from '@core/services/auth.service';
import { LanguageSelectorComponent } from '@shared/components/language-selector/language-selector.component';

interface MenuItem {
  icon: string;
  label: string;
  route: string;
  roles?: string[];
}

@Component({
  selector: 'app-dashboard-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    TranslocoModule,
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatMenuModule,
    MatDividerModule,
    LanguageSelectorComponent
  ],
  template: `
    <mat-sidenav-container class="h-screen">
      <mat-sidenav #sidenav mode="side" [opened]="!isMobile" class="w-64 bg-primary-navy">
        <div class="p-4">
          <a routerLink="/" class="flex items-center space-x-2 mb-8">
            <span class="text-primary-gold text-xl font-heading font-bold">Empty Legs</span>
            <span class="text-primary-white text-xl font-heading">Luxury</span>
          </a>

          <mat-nav-list>
            @for (item of menuItems; track item.route) {
              @if (!item.roles || hasRole(item.roles)) {
                <a mat-list-item [routerLink]="item.route" routerLinkActive="bg-primary-gold bg-opacity-20"
                   class="text-primary-white hover:bg-primary-gold hover:bg-opacity-10 rounded-lg mb-2">
                  <mat-icon matListItemIcon class="text-primary-gold">{{ item.icon }}</mat-icon>
                  <span matListItemTitle>{{ item.label | transloco }}</span>
                </a>
              }
            }
          </mat-nav-list>
        </div>
      </mat-sidenav>

      <mat-sidenav-content>
        <!-- Toolbar -->
        <mat-toolbar class="bg-white shadow-md">
          <button mat-icon-button (click)="sidenav.toggle()" class="md:hidden">
            <mat-icon>menu</mat-icon>
          </button>

          <span class="flex-grow"></span>

          <!-- User Menu -->
          <app-language-selector></app-language-selector>
          
          <button mat-button [matMenuTriggerFor]="userMenu" class="ml-4">
            <mat-icon>account_circle</mat-icon>
            <span class="ml-2 hidden md:inline">{{ currentUser?.firstName }} {{ currentUser?.lastName }}</span>
          </button>
          
          <mat-menu #userMenu="matMenu">
            <div class="px-4 py-2 border-b">
              <p class="font-semibold">{{ currentUser?.email }}</p>
              <p class="text-sm text-gray-500">{{ getUserRoleLabel() }}</p>
            </div>
            <a mat-menu-item routerLink="/profile">
              <mat-icon>person</mat-icon>
              <span>{{ 'navigation.profile' | transloco }}</span>
            </a>
            <a mat-menu-item routerLink="/settings">
              <mat-icon>settings</mat-icon>
              <span>{{ 'navigation.settings' | transloco }}</span>
            </a>
            <mat-divider></mat-divider>
            <button mat-menu-item (click)="logout()">
              <mat-icon>logout</mat-icon>
              <span>{{ 'navigation.logout' | transloco }}</span>
            </button>
          </mat-menu>
        </mat-toolbar>

        <!-- Main Content -->
        <main class="p-6 bg-gray-50 min-h-[calc(100vh-64px)]">
          <router-outlet></router-outlet>
        </main>
      </mat-sidenav-content>
    </mat-sidenav-container>
  `,
  styles: [`
    :host ::ng-deep .mat-mdc-nav-list .mat-mdc-list-item {
      margin-bottom: 8px;
    }
  `]
})
export class DashboardLayoutComponent {
  authService = inject(AuthService);
  isMobile = false;

  menuItems: MenuItem[] = [];

  get currentUser() {
    return this.authService.getCurrentUser();
  }

  constructor() {
    this.checkScreenSize();
    window.addEventListener('resize', () => this.checkScreenSize());
    this.setMenuItems();
  }

  private checkScreenSize(): void {
    this.isMobile = window.innerWidth < 768;
  }

  private setMenuItems(): void {
    const user = this.currentUser;
    if (!user) return;

    switch (user.role) {
      case 'Customer':
        this.menuItems = [
          { icon: 'dashboard', label: 'navigation.dashboard', route: '/customer/dashboard' },
          { icon: 'flight', label: 'navigation.bookings', route: '/customer/bookings' },
          { icon: 'person', label: 'navigation.profile', route: '/customer/profile' },
          { icon: 'star', label: 'customer.loyalty.title', route: '/customer/loyalty' }
        ];
        break;
      
      case 'Company':
        this.menuItems = [
          { icon: 'dashboard', label: 'navigation.dashboard', route: '/company/dashboard' },
          { icon: 'flight', label: 'navigation.flights', route: '/company/flights' },
          { icon: 'analytics', label: 'company.analytics.title', route: '/company/analytics' },
          { icon: 'settings', label: 'navigation.settings', route: '/company/settings' }
        ];
        break;
      
      case 'Admin':
        this.menuItems = [
          { icon: 'dashboard', label: 'navigation.dashboard', route: '/admin/dashboard' },
          { icon: 'people', label: 'admin.users.title', route: '/admin/users' },
          { icon: 'business', label: 'admin.companies.title', route: '/admin/companies' },
          { icon: 'settings', label: 'admin.system.title', route: '/admin/system' }
        ];
        break;
    }
  }

  hasRole(roles: string[]): boolean {
    return roles.includes(this.currentUser?.role);
  }

  getUserRoleLabel(): string {
    const role = this.currentUser?.role;
    switch (role) {
      case 'Customer':
        return 'Client';
      case 'Company':
        return 'Entreprise';
      case 'Admin':
        return 'Administrateur';
      default:
        return '';
    }
  }

  logout(): void {
    this.authService.logout();
  }
}