import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { FlightService } from '@core/services/flight.service';
import { Flight } from '@core/models';
import { HeroSectionComponent } from './components/hero-section/hero-section.component';
import { FeaturedFlightsComponent } from './components/featured-flights/featured-flights.component';
import { FeaturesComponent } from './components/features/features.component';
import { HowItWorksComponent } from './components/how-it-works/how-it-works.component';
import { NewsletterComponent } from './components/newsletter/newsletter.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    TranslocoModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    HeroSectionComponent,
    FeaturedFlightsComponent,
    FeaturesComponent,
    HowItWorksComponent,
    NewsletterComponent
  ],
  template: `
    <div class="home-page">
      <!-- Hero Section -->
      <app-hero-section></app-hero-section>

      <!-- Featured Flights -->
      <section class="py-16 bg-gray-50">
        <div class="container mx-auto px-4">
          <h2 class="section-title text-center">
            {{ 'home.featuredFlights.title' | transloco }}
          </h2>
          <p class="section-subtitle text-center mb-12">
            {{ 'home.featuredFlights.subtitle' | transloco }}
          </p>
          <app-featured-flights [flights]="featuredFlights"></app-featured-flights>
        </div>
      </section>

      <!-- Features -->
      <app-features></app-features>

      <!-- How It Works -->
      <app-how-it-works></app-how-it-works>

      <!-- Newsletter -->
      <app-newsletter></app-newsletter>
    </div>
  `,
  styles: []
})
export class HomeComponent implements OnInit {
  private flightService = inject(FlightService);
  
  featuredFlights: Flight[] = [];

  ngOnInit(): void {
    this.loadFeaturedFlights();
  }

  private loadFeaturedFlights(): void {
    this.flightService.getFeaturedFlights().subscribe({
      next: (flights) => {
        this.featuredFlights = flights;
      },
      error: (error) => {
        console.error('Error loading featured flights:', error);
      }
    });
  }
}