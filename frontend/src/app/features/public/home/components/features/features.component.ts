import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslocoModule } from '@ngneat/transloco';
import { MatIconModule } from '@angular/material/icon';

interface Feature {
  icon: string;
  titleKey: string;
  descriptionKey: string;
}

@Component({
  selector: 'app-features',
  standalone: true,
  imports: [CommonModule, TranslocoModule, MatIconModule],
  template: `
    <section class="py-16 bg-white">
      <div class="container mx-auto px-4">
        <h2 class="section-title text-center">
          {{ 'home.features.title' | transloco }}
        </h2>
        
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8 mt-12">
          @for (feature of features; track feature.icon) {
            <div class="text-center group">
              <div class="w-20 h-20 mx-auto mb-6 bg-primary-gold/10 rounded-full flex items-center justify-center group-hover:bg-primary-gold/20 transition-colors">
                <mat-icon class="text-primary-gold text-4xl">{{ feature.icon }}</mat-icon>
              </div>
              <h3 class="text-xl font-semibold text-primary-navy mb-3">
                {{ feature.titleKey | transloco }}
              </h3>
              <p class="text-gray-600">
                {{ feature.descriptionKey | transloco }}
              </p>
            </div>
          }
        </div>
      </div>
    </section>
  `,
  styles: []
})
export class FeaturesComponent {
  features: Feature[] = [
    {
      icon: 'savings',
      titleKey: 'home.features.luxury.title',
      descriptionKey: 'home.features.luxury.description'
    },
    {
      icon: 'schedule',
      titleKey: 'home.features.flexibility.title',
      descriptionKey: 'home.features.flexibility.description'
    },
    {
      icon: 'verified_user',
      titleKey: 'home.features.safety.title',
      descriptionKey: 'home.features.safety.description'
    },
    {
      icon: 'support_agent',
      titleKey: 'home.features.service.title',
      descriptionKey: 'home.features.service.description'
    }
  ];
}