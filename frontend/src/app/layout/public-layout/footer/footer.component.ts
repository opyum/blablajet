import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslocoModule, MatIconModule],
  template: `
    <footer class="bg-primary-navy text-primary-white">
      <div class="container mx-auto px-4 py-12">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-8">
          <!-- Company Info -->
          <div>
            <h3 class="font-heading text-2xl mb-4">
              <span class="text-primary-gold">Empty Legs</span> Luxury
            </h3>
            <p class="text-gray-300 mb-4">
              {{ 'home.hero.subtitle' | transloco }}
            </p>
            <div class="flex space-x-4">
              <a href="#" class="text-gray-300 hover:text-primary-gold transition-colors">
                <mat-icon>facebook</mat-icon>
              </a>
              <a href="#" class="text-gray-300 hover:text-primary-gold transition-colors">
                <mat-icon>twitter</mat-icon>
              </a>
              <a href="#" class="text-gray-300 hover:text-primary-gold transition-colors">
                <mat-icon>instagram</mat-icon>
              </a>
              <a href="#" class="text-gray-300 hover:text-primary-gold transition-colors">
                <mat-icon>linkedin</mat-icon>
              </a>
            </div>
          </div>

          <!-- About Links -->
          <div>
            <h4 class="font-semibold text-lg mb-4">{{ 'footer.about.title' | transloco }}</h4>
            <ul class="space-y-2">
              <li>
                <a routerLink="/about" class="text-gray-300 hover:text-primary-gold transition-colors">
                  {{ 'footer.about.company' | transloco }}
                </a>
              </li>
              <li>
                <a routerLink="/team" class="text-gray-300 hover:text-primary-gold transition-colors">
                  {{ 'footer.about.team' | transloco }}
                </a>
              </li>
              <li>
                <a routerLink="/careers" class="text-gray-300 hover:text-primary-gold transition-colors">
                  {{ 'footer.about.careers' | transloco }}
                </a>
              </li>
              <li>
                <a routerLink="/press" class="text-gray-300 hover:text-primary-gold transition-colors">
                  {{ 'footer.about.press' | transloco }}
                </a>
              </li>
            </ul>
          </div>

          <!-- Services Links -->
          <div>
            <h4 class="font-semibold text-lg mb-4">{{ 'footer.services.title' | transloco }}</h4>
            <ul class="space-y-2">
              <li>
                <a routerLink="/flights" class="text-gray-300 hover:text-primary-gold transition-colors">
                  {{ 'footer.services.flights' | transloco }}
                </a>
              </li>
              <li>
                <a routerLink="/charter" class="text-gray-300 hover:text-primary-gold transition-colors">
                  {{ 'footer.services.charter' | transloco }}
                </a>
              </li>
              <li>
                <a routerLink="/consulting" class="text-gray-300 hover:text-primary-gold transition-colors">
                  {{ 'footer.services.consulting' | transloco }}
                </a>
              </li>
            </ul>
          </div>

          <!-- Support Links -->
          <div>
            <h4 class="font-semibold text-lg mb-4">{{ 'footer.support.title' | transloco }}</h4>
            <ul class="space-y-2">
              <li>
                <a routerLink="/help" class="text-gray-300 hover:text-primary-gold transition-colors">
                  {{ 'footer.support.help' | transloco }}
                </a>
              </li>
              <li>
                <a routerLink="/contact" class="text-gray-300 hover:text-primary-gold transition-colors">
                  {{ 'footer.support.contact' | transloco }}
                </a>
              </li>
              <li>
                <a routerLink="/faq" class="text-gray-300 hover:text-primary-gold transition-colors">
                  {{ 'footer.support.faq' | transloco }}
                </a>
              </li>
              <li>
                <a routerLink="/terms" class="text-gray-300 hover:text-primary-gold transition-colors">
                  {{ 'footer.support.terms' | transloco }}
                </a>
              </li>
              <li>
                <a routerLink="/privacy" class="text-gray-300 hover:text-primary-gold transition-colors">
                  {{ 'footer.support.privacy' | transloco }}
                </a>
              </li>
            </ul>
          </div>
        </div>

        <div class="border-t border-gray-700 mt-8 pt-8 text-center">
          <p class="text-gray-300">
            {{ 'footer.copyright' | transloco }}
          </p>
        </div>
      </div>
    </footer>
  `,
  styles: []
})
export class FooterComponent {}