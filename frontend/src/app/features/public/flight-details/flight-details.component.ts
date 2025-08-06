import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslocoModule } from '@ngneat/transloco';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { FlightService } from '@core/services/flight.service';
import { AuthService } from '@core/services/auth.service';
import { Flight } from '@core/models';

@Component({
  selector: 'app-flight-details',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    TranslocoModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatTabsModule,
    MatChipsModule,
    MatDividerModule,
    MatProgressSpinnerModule
  ],
  template: `
    <div class="min-h-screen bg-gray-50">
      @if (isLoading) {
        <div class="flex justify-center items-center h-96">
          <mat-spinner></mat-spinner>
        </div>
      } @else if (flight) {
        <!-- Hero Section with Image -->
        <section class="relative h-96">
          <img 
            [src]="flight.images[0] || '/assets/images/default-jet.jpg'" 
            [alt]="flight.aircraftId"
            class="w-full h-full object-cover"
          >
          <div class="absolute inset-0 bg-gradient-to-t from-black/70 to-transparent"></div>
          
          <div class="absolute bottom-0 left-0 right-0 text-white p-8">
            <div class="container mx-auto">
              <div class="flex items-center gap-2 mb-4">
                <button mat-icon-button routerLink="/flights" class="bg-white/20 backdrop-blur-sm">
                  <mat-icon>arrow_back</mat-icon>
                </button>
                <span class="text-sm opacity-80">{{ 'common.back' | transloco }}</span>
              </div>
              
              <h1 class="text-4xl font-heading font-bold mb-2">
                {{ flight.departureCity }} → {{ flight.arrivalCity }}
              </h1>
              <p class="text-xl opacity-90">
                {{ flight.departureAirportName }} ({{ flight.departureAirportCode }}) - 
                {{ flight.arrivalAirportName }} ({{ flight.arrivalAirportCode }})
              </p>
            </div>
          </div>
        </section>

        <!-- Main Content -->
        <section class="py-8">
          <div class="container mx-auto px-4">
            <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
              <!-- Flight Info -->
              <div class="lg:col-span-2">
                <mat-card class="mb-6">
                  <mat-card-content>
                    <!-- Flight Schedule -->
                    <div class="grid grid-cols-2 gap-8 mb-8">
                      <div>
                        <p class="text-sm text-gray-500 mb-1">{{ 'flight.details.departure' | transloco }}</p>
                        <p class="text-2xl font-bold">{{ flight.departureTime | date:'HH:mm' }}</p>
                        <p class="text-gray-600">{{ flight.departureTime | date:'dd MMM yyyy' }}</p>
                        <p class="text-sm text-gray-500 mt-2">{{ flight.departureAirportName }}</p>
                      </div>
                      <div class="text-right">
                        <p class="text-sm text-gray-500 mb-1">{{ 'flight.details.arrival' | transloco }}</p>
                        <p class="text-2xl font-bold">{{ flight.arrivalTime | date:'HH:mm' }}</p>
                        <p class="text-gray-600">{{ flight.arrivalTime | date:'dd MMM yyyy' }}</p>
                        <p class="text-sm text-gray-500 mt-2">{{ flight.arrivalAirportName }}</p>
                      </div>
                    </div>

                    <div class="flex justify-center mb-8">
                      <div class="flex items-center gap-4">
                        <mat-icon class="text-gray-400">flight</mat-icon>
                        <div class="text-center">
                          <p class="text-sm text-gray-500">{{ 'flight.details.duration' | transloco }}</p>
                          <p class="font-semibold">{{ formatDuration(flight.duration) }}</p>
                        </div>
                        <mat-divider [vertical]="true" class="h-12"></mat-divider>
                        <div class="text-center">
                          <p class="text-sm text-gray-500">{{ 'flight.details.distance' | transloco }}</p>
                          <p class="font-semibold">{{ flight.distance }} km</p>
                        </div>
                      </div>
                    </div>

                    <!-- Tabs -->
                    <mat-tab-group>
                      <!-- Description Tab -->
                      <mat-tab [label]="'flight.details.description' | transloco">
                        <div class="py-4">
                          <p class="text-gray-600">{{ flight.description || 'Profitez d\'un vol exceptionnel à bord de notre jet privé.' }}</p>
                        </div>
                      </mat-tab>

                      <!-- Amenities Tab -->
                      <mat-tab [label]="'flight.details.amenities' | transloco">
                        <div class="py-4">
                          <div class="flex flex-wrap gap-2">
                            @for (amenity of flight.amenities; track amenity) {
                              <mat-chip>
                                <mat-icon class="mr-1">check_circle</mat-icon>
                                {{ amenity }}
                              </mat-chip>
                            }
                          </div>
                        </div>
                      </mat-tab>

                      <!-- Gallery Tab -->
                      <mat-tab [label]="'flight.details.gallery' | transloco">
                        <div class="py-4">
                          <div class="grid grid-cols-2 md:grid-cols-3 gap-4">
                            @for (image of flight.images; track image) {
                              <img 
                                [src]="image" 
                                alt="Aircraft image"
                                class="w-full h-48 object-cover rounded-lg cursor-pointer hover:opacity-90 transition-opacity"
                                (click)="viewImage(image)"
                              >
                            }
                          </div>
                        </div>
                      </mat-tab>

                      <!-- Terms Tab -->
                      <mat-tab [label]="'flight.details.terms' | transloco">
                        <div class="py-4">
                          <p class="text-gray-600 whitespace-pre-line">{{ flight.terms || defaultTerms }}</p>
                        </div>
                      </mat-tab>
                    </mat-tab-group>
                  </mat-card-content>
                </mat-card>
              </div>

              <!-- Booking Card -->
              <div class="lg:col-span-1">
                <mat-card class="sticky top-24">
                  <mat-card-content>
                    <!-- Price -->
                    <div class="text-center mb-6">
                      <p class="text-sm text-gray-500 line-through">
                        {{ flight.originalPrice | currency:'EUR' }}
                      </p>
                      <p class="text-4xl font-bold text-primary-gold">
                        {{ flight.price | currency:'EUR' }}
                      </p>
                      <mat-chip class="mt-2 bg-accent-green text-white">
                        {{ 'common.save' | transloco }} {{ flight.discount }}%
                      </mat-chip>
                    </div>

                    <!-- Flight Info Summary -->
                    <div class="space-y-3 mb-6">
                      <div class="flex justify-between">
                        <span class="text-gray-600">{{ 'flight.details.aircraft' | transloco }}</span>
                        <span class="font-semibold">{{ flight.aircraftId }}</span>
                      </div>
                      <div class="flex justify-between">
                        <span class="text-gray-600">{{ 'flight.details.seats' | transloco }}</span>
                        <span class="font-semibold">{{ flight.availableSeats }} / {{ flight.totalSeats }}</span>
                      </div>
                      <div class="flex justify-between">
                        <span class="text-gray-600">{{ 'flight.search.date' | transloco }}</span>
                        <span class="font-semibold">{{ flight.departureTime | date:'dd MMM' }}</span>
                      </div>
                    </div>

                    <mat-divider class="my-6"></mat-divider>

                    <!-- CTA -->
                    @if (authService.isAuthenticated()) {
                      <button 
                        mat-raised-button 
                        class="w-full btn-premium"
                        (click)="bookFlight()">
                        {{ 'flight.details.bookNow' | transloco }}
                      </button>
                    } @else {
                      <p class="text-sm text-gray-600 mb-4 text-center">
                        Connectez-vous pour réserver ce vol
                      </p>
                      <button 
                        mat-raised-button 
                        class="w-full btn-premium"
                        routerLink="/auth/login"
                        [queryParams]="{returnUrl: router.url}">
                        {{ 'navigation.login' | transloco }}
                      </button>
                    }

                    <!-- Availability -->
                    <p class="text-center mt-4 text-sm text-gray-600">
                      <mat-icon class="text-accent-green align-middle">check_circle</mat-icon>
                      {{ 'flight.details.availability' | transloco }}: {{ flight.availableSeats }} {{ 'flight.details.seats' | transloco }}
                    </p>
                  </mat-card-content>
                </mat-card>
              </div>
            </div>
          </div>
        </section>
      } @else {
        <div class="container mx-auto px-4 py-16 text-center">
          <mat-icon class="text-6xl text-gray-300 mb-4">error_outline</mat-icon>
          <p class="text-xl text-gray-600">Vol non trouvé</p>
          <button mat-raised-button routerLink="/flights" class="mt-4">
            Retour aux vols
          </button>
        </div>
      }
    </div>
  `,
  styles: []
})
export class FlightDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  router = inject(Router);
  private flightService = inject(FlightService);
  authService = inject(AuthService);
  
  flight: Flight | null = null;
  isLoading = true;
  
  defaultTerms = `Conditions générales de réservation:
- La réservation est définitive après paiement complet
- Annulation possible jusqu'à 48h avant le départ
- Bagages inclus selon les spécifications de l'appareil
- Catering disponible sur demande
- Animaux acceptés sous conditions`;

  ngOnInit(): void {
    const flightId = this.route.snapshot.paramMap.get('id');
    if (flightId) {
      this.loadFlight(flightId);
    }
  }

  private loadFlight(id: string): void {
    this.flightService.getFlightById(id).subscribe({
      next: (flight) => {
        this.flight = flight;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading flight:', error);
        this.isLoading = false;
      }
    });
  }

  formatDuration(minutes: number): string {
    const hours = Math.floor(minutes / 60);
    const mins = minutes % 60;
    return `${hours}h${mins > 0 ? mins : ''}`;
  }

  viewImage(imageUrl: string): void {
    // TODO: Implement image viewer modal
    window.open(imageUrl, '_blank');
  }

  bookFlight(): void {
    if (this.flight) {
      this.router.navigate(['/booking'], { 
        queryParams: { flightId: this.flight.id } 
      });
    }
  }
}