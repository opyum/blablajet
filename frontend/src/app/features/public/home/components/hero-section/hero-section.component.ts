import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-hero-section',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    TranslocoModule,
    MatButtonModule,
    MatIconModule,
    ReactiveFormsModule
  ],
  template: `
    <section class="hero-section relative min-h-[600px] md:min-h-[700px] flex items-center">
      <!-- Background Image/Video -->
      <div class="absolute inset-0 z-0">
        <img 
          src="/assets/images/hero-jet.jpg" 
          alt="Private Jet" 
          class="w-full h-full object-cover"
        >
        <div class="absolute inset-0 bg-gradient-to-b from-primary-navy/80 to-primary-navy/60"></div>
      </div>

      <!-- Content -->
      <div class="container mx-auto px-4 relative z-10">
        <div class="max-w-3xl mx-auto text-center text-white">
          <h1 class="text-4xl md:text-5xl lg:text-6xl font-heading font-bold mb-6 animate-fade-in">
            {{ 'home.hero.title' | transloco }}
          </h1>
          <p class="text-xl md:text-2xl mb-8 opacity-90 animate-slide-up">
            {{ 'home.hero.subtitle' | transloco }}
          </p>

          <!-- Quick Search Form -->
          <form [formGroup]="searchForm" (ngSubmit)="onSearch()" 
                class="bg-white/10 backdrop-blur-md rounded-2xl p-6 md:p-8 animate-scale-in">
            <div class="flex flex-col md:flex-row gap-4">
              <div class="flex-1">
                <input 
                  type="text" 
                  formControlName="destination"
                  [placeholder]="'home.hero.searchPlaceholder' | transloco"
                  class="w-full px-4 py-3 rounded-lg bg-white/90 text-primary-navy placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-primary-gold"
                >
              </div>
              <button 
                type="submit" 
                mat-raised-button 
                class="btn-premium whitespace-nowrap"
                [disabled]="!searchForm.valid">
                <mat-icon class="mr-2">search</mat-icon>
                {{ 'home.hero.cta' | transloco }}
              </button>
            </div>
          </form>

          <!-- Quick Stats -->
          <div class="grid grid-cols-3 gap-8 mt-12">
            <div class="text-center animate-fade-in" style="animation-delay: 0.2s">
              <div class="text-3xl md:text-4xl font-bold text-primary-gold">500+</div>
              <div class="text-sm md:text-base opacity-80">Vols disponibles</div>
            </div>
            <div class="text-center animate-fade-in" style="animation-delay: 0.4s">
              <div class="text-3xl md:text-4xl font-bold text-primary-gold">75%</div>
              <div class="text-sm md:text-base opacity-80">D'économies</div>
            </div>
            <div class="text-center animate-fade-in" style="animation-delay: 0.6s">
              <div class="text-3xl md:text-4xl font-bold text-primary-gold">24/7</div>
              <div class="text-sm md:text-base opacity-80">Support client</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Scroll Indicator -->
      <div class="absolute bottom-8 left-1/2 transform -translate-x-1/2 animate-bounce">
        <mat-icon class="text-white text-3xl">expand_more</mat-icon>
      </div>
    </section>
  `,
  styles: [`
    .hero-section {
      background-attachment: fixed;
    }

    @keyframes fadeIn {
      from { opacity: 0; }
      to { opacity: 1; }
    }

    @keyframes slideUp {
      from { 
        opacity: 0;
        transform: translateY(20px);
      }
      to { 
        opacity: 1;
        transform: translateY(0);
      }
    }

    @keyframes scaleIn {
      from { 
        opacity: 0;
        transform: scale(0.95);
      }
      to { 
        opacity: 1;
        transform: scale(1);
      }
    }

    .animate-fade-in {
      animation: fadeIn 1s ease-out forwards;
    }

    .animate-slide-up {
      animation: slideUp 0.8s ease-out forwards;
      animation-delay: 0.2s;
      opacity: 0;
    }

    .animate-scale-in {
      animation: scaleIn 0.6s ease-out forwards;
      animation-delay: 0.4s;
      opacity: 0;
    }
  `]
})
export class HeroSectionComponent {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  
  searchForm = this.fb.group({
    destination: ['', Validators.required]
  });

  onSearch(): void {
    if (this.searchForm.valid) {
      const destination = this.searchForm.get('destination')?.value;
      this.router.navigate(['/flights'], { 
        queryParams: { destination } 
      });
    }
  }
}