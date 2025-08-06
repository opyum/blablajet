import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslocoModule } from '@ngneat/transloco';
import { MatIconModule } from '@angular/material/icon';

interface Step {
  number: number;
  icon: string;
  titleKey: string;
  descriptionKey: string;
}

@Component({
  selector: 'app-how-it-works',
  standalone: true,
  imports: [CommonModule, TranslocoModule, MatIconModule],
  template: `
    <section class="py-16 bg-gray-50">
      <div class="container mx-auto px-4">
        <h2 class="section-title text-center">
          {{ 'home.howItWorks.title' | transloco }}
        </h2>
        
        <div class="grid grid-cols-1 md:grid-cols-3 gap-8 mt-12 max-w-4xl mx-auto">
          @for (step of steps; track step.number) {
            <div class="relative text-center">
              <!-- Step Number -->
              <div class="absolute -top-4 left-1/2 transform -translate-x-1/2 w-12 h-12 bg-primary-gold rounded-full flex items-center justify-center text-primary-navy font-bold text-lg z-10">
                {{ step.number }}
              </div>
              
              <!-- Card -->
              <div class="bg-white rounded-2xl p-8 pt-12 shadow-lg h-full">
                <mat-icon class="text-primary-gold text-5xl mb-4">{{ step.icon }}</mat-icon>
                <h3 class="text-xl font-semibold text-primary-navy mb-3">
                  {{ step.titleKey | transloco }}
                </h3>
                <p class="text-gray-600">
                  {{ step.descriptionKey | transloco }}
                </p>
              </div>
              
              <!-- Connector Line -->
              @if (!$last) {
                <div class="hidden md:block absolute top-1/2 left-full w-full h-0.5 bg-gray-300 transform -translate-y-1/2 z-0"></div>
              }
            </div>
          }
        </div>
      </div>
    </section>
  `,
  styles: []
})
export class HowItWorksComponent {
  steps: Step[] = [
    {
      number: 1,
      icon: 'search',
      titleKey: 'home.howItWorks.step1.title',
      descriptionKey: 'home.howItWorks.step1.description'
    },
    {
      number: 2,
      icon: 'touch_app',
      titleKey: 'home.howItWorks.step2.title',
      descriptionKey: 'home.howItWorks.step2.description'
    },
    {
      number: 3,
      icon: 'flight_takeoff',
      titleKey: 'home.howItWorks.step3.title',
      descriptionKey: 'home.howItWorks.step3.description'
    }
  ];
}