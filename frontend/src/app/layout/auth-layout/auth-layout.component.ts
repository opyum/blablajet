import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { LanguageSelectorComponent } from '@shared/components/language-selector/language-selector.component';

@Component({
  selector: 'app-auth-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, TranslocoModule, LanguageSelectorComponent],
  template: `
    <div class="min-h-screen bg-gradient-premium flex flex-col">
      <!-- Header -->
      <header class="p-6">
        <div class="container mx-auto flex justify-between items-center">
          <a routerLink="/" class="flex items-center space-x-2">
            <span class="text-primary-gold text-3xl font-heading font-bold">Empty Legs</span>
            <span class="text-primary-white text-3xl font-heading">Luxury</span>
          </a>
          <app-language-selector></app-language-selector>
        </div>
      </header>

      <!-- Main Content -->
      <main class="flex-grow flex items-center justify-center p-6">
        <div class="w-full max-w-md">
          <div class="bg-white rounded-2xl shadow-2xl p-8">
            <router-outlet></router-outlet>
          </div>
        </div>
      </main>

      <!-- Footer -->
      <footer class="p-6 text-center text-primary-white">
        <p class="text-sm opacity-75">
          {{ 'footer.copyright' | transloco }}
        </p>
      </footer>
    </div>
  `,
  styles: []
})
export class AuthLayoutComponent {}