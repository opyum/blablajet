import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { Flight } from '@core/models';

@Component({
  selector: 'app-featured-flights',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    TranslocoModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule
  ],
  template: `
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      @for (flight of flights; track flight.id) {
        <mat-card class="luxury-card hover:scale-105 transition-transform duration-300">
          <!-- Image -->
          <div class="relative h-48 -mx-8 -mt-8 mb-4">
            <img 
              [src]="flight.images[0] || '/assets/images/default-jet.jpg'" 
              [alt]="flight.aircraftId"
              class="w-full h-full object-cover rounded-t-2xl"
            >
            <div class="absolute top-4 right-4">
              <mat-chip class="bg-primary-gold text-primary-navy font-semibold">
                -{{ flight.discount }}%
              </mat-chip>
            </div>
          </div>

          <!-- Content -->
          <mat-card-content>
            <div class="flex justify-between items-start mb-4">
              <div>
                <h3 class="text-lg font-semibold text-primary-navy">
                  {{ flight.departureCity }} → {{ flight.arrivalCity }}
                </h3>
                <p class="text-sm text-gray-500">
                  {{ flight.departureAirportCode }} - {{ flight.arrivalAirportCode }}
                </p>
              </div>
              <div class="text-right">
                <p class="text-2xl font-bold text-primary-gold">
                  {{ flight.price | currency:'EUR':'symbol':'1.0-0' }}
                </p>
                <p class="text-sm text-gray-500 line-through">
                  {{ flight.originalPrice | currency:'EUR':'symbol':'1.0-0' }}
                </p>
              </div>
            </div>

            <div class="grid grid-cols-2 gap-4 mb-4 text-sm">
              <div class="flex items-center gap-2">
                <mat-icon class="text-gray-400">calendar_today</mat-icon>
                <span>{{ flight.departureTime | date:'dd MMM' }}</span>
              </div>
              <div class="flex items-center gap-2">
                <mat-icon class="text-gray-400">schedule</mat-icon>
                <span>{{ flight.departureTime | date:'HH:mm' }}</span>
              </div>
              <div class="flex items-center gap-2">
                <mat-icon class="text-gray-400">flight</mat-icon>
                <span>{{ formatDuration(flight.duration) }}</span>
              </div>
              <div class="flex items-center gap-2">
                <mat-icon class="text-gray-400">airline_seat_recline_extra</mat-icon>
                <span>{{ flight.availableSeats }} {{ 'flight.details.seats' | transloco }}</span>
              </div>
            </div>

            <!-- Amenities -->
            <div class="flex flex-wrap gap-2 mb-4">
              @for (amenity of flight.amenities.slice(0, 3); track amenity) {
                <mat-chip class="text-xs">{{ amenity }}</mat-chip>
              }
              @if (flight.amenities.length > 3) {
                <mat-chip class="text-xs">+{{ flight.amenities.length - 3 }}</mat-chip>
              }
            </div>
          </mat-card-content>

          <mat-card-actions>
            <a [routerLink]="['/flights', flight.id]" mat-raised-button class="w-full btn-premium">
              {{ 'flight.details.bookNow' | transloco }}
            </a>
          </mat-card-actions>
        </mat-card>
      }
    </div>

    @if (flights.length === 0) {
      <div class="text-center py-12">
        <mat-icon class="text-6xl text-gray-300 mb-4">flight_takeoff</mat-icon>
        <p class="text-gray-500">{{ 'flight.search.noResults' | transloco }}</p>
      </div>
    }
  `,
  styles: []
})
export class FeaturedFlightsComponent {
  @Input() flights: Flight[] = [];

  formatDuration(minutes: number): string {
    const hours = Math.floor(minutes / 60);
    const mins = minutes % 60;
    return `${hours}h${mins > 0 ? mins : ''}`;
  }
}