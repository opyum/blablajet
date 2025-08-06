import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { TranslocoModule } from '@ngneat/transloco';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { MatSliderModule } from '@angular/material/slider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { Observable, startWith, map, debounceTime, switchMap } from 'rxjs';
import { FlightService } from '@core/services/flight.service';
import { AirportService } from '@core/services/airport.service';
import { Flight, FlightSearchCriteria, Airport } from '@core/models';
import { FeaturedFlightsComponent } from '../home/components/featured-flights/featured-flights.component';

@Component({
  selector: 'app-flight-search',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslocoModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSelectModule,
    MatSliderModule,
    MatProgressSpinnerModule,
    MatAutocompleteModule,
    FeaturedFlightsComponent
  ],
  template: `
    <div class="min-h-screen bg-gray-50">
      <!-- Search Header -->
      <section class="bg-gradient-premium text-white py-12">
        <div class="container mx-auto px-4">
          <h1 class="text-3xl md:text-4xl font-heading font-bold text-center mb-8">
            {{ 'flight.search.title' | transloco }}
          </h1>
          
          <!-- Search Form -->
          <form [formGroup]="searchForm" (ngSubmit)="onSearch()" class="bg-white/10 backdrop-blur-md rounded-2xl p-6">
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
              <!-- Departure -->
              <mat-form-field appearance="outline" class="w-full">
                <mat-label>{{ 'flight.search.departure' | transloco }}</mat-label>
                <input 
                  matInput 
                  formControlName="departure"
                  [matAutocomplete]="departureAuto"
                  (input)="onAirportSearch('departure', $event)">
                <mat-icon matPrefix>flight_takeoff</mat-icon>
                <mat-autocomplete #departureAuto="matAutocomplete" [displayWith]="displayAirport">
                  @for (airport of departureAirports$ | async; track airport.id) {
                    <mat-option [value]="airport">
                      {{ airportService.formatAirportDisplay(airport) }}
                    </mat-option>
                  }
                </mat-autocomplete>
              </mat-form-field>

              <!-- Arrival -->
              <mat-form-field appearance="outline" class="w-full">
                <mat-label>{{ 'flight.search.arrival' | transloco }}</mat-label>
                <input 
                  matInput 
                  formControlName="arrival"
                  [matAutocomplete]="arrivalAuto"
                  (input)="onAirportSearch('arrival', $event)">
                <mat-icon matPrefix>flight_land</mat-icon>
                <mat-autocomplete #arrivalAuto="matAutocomplete" [displayWith]="displayAirport">
                  @for (airport of arrivalAirports$ | async; track airport.id) {
                    <mat-option [value]="airport">
                      {{ airportService.formatAirportDisplay(airport) }}
                    </mat-option>
                  }
                </mat-autocomplete>
              </mat-form-field>

              <!-- Date -->
              <mat-form-field appearance="outline" class="w-full">
                <mat-label>{{ 'flight.search.date' | transloco }}</mat-label>
                <input matInput [matDatepicker]="picker" formControlName="date">
                <mat-icon matPrefix>calendar_today</mat-icon>
                <mat-datepicker-toggle matIconSuffix [for]="picker"></mat-datepicker-toggle>
                <mat-datepicker #picker></mat-datepicker>
              </mat-form-field>

              <!-- Passengers -->
              <mat-form-field appearance="outline" class="w-full">
                <mat-label>{{ 'flight.search.passengers' | transloco }}</mat-label>
                <mat-select formControlName="passengers">
                  @for (num of [1,2,3,4,5,6,7,8]; track num) {
                    <mat-option [value]="num">{{ num }}</mat-option>
                  }
                </mat-select>
                <mat-icon matPrefix>group</mat-icon>
              </mat-form-field>
            </div>

            <div class="flex justify-center mt-6">
              <button type="submit" mat-raised-button class="btn-premium">
                <mat-icon class="mr-2">search</mat-icon>
                {{ 'flight.search.searchButton' | transloco }}
              </button>
            </div>
          </form>
        </div>
      </section>

      <!-- Filters & Results -->
      <section class="py-8">
        <div class="container mx-auto px-4">
          <div class="grid grid-cols-1 lg:grid-cols-4 gap-8">
            <!-- Filters Sidebar -->
            <div class="lg:col-span-1">
              <mat-card class="sticky top-24">
                <mat-card-header>
                  <mat-card-title>{{ 'flight.search.filters' | transloco }}</mat-card-title>
                </mat-card-header>
                <mat-card-content>
                  <!-- Price Range -->
                  <div class="mb-6">
                    <label class="block text-sm font-medium mb-2">
                      {{ 'flight.search.priceRange' | transloco }}
                    </label>
                    <div class="px-3">
                      <mat-slider min="0" max="50000" step="1000" discrete [displayWith]="formatPrice">
                        <input matSliderStartThumb formControlName="minPrice">
                        <input matSliderEndThumb formControlName="maxPrice">
                      </mat-slider>
                    </div>
                    <div class="flex justify-between text-sm text-gray-600 mt-2">
                      <span>{{ searchForm.get('minPrice')?.value | currency:'EUR' }}</span>
                      <span>{{ searchForm.get('maxPrice')?.value | currency:'EUR' }}</span>
                    </div>
                  </div>

                  <!-- Aircraft Type -->
                  <div class="mb-6">
                    <label class="block text-sm font-medium mb-2">
                      {{ 'flight.search.aircraftType' | transloco }}
                    </label>
                    <mat-select formControlName="aircraftType" multiple>
                      <mat-option value="VeryLightJet">Very Light Jet</mat-option>
                      <mat-option value="LightJet">Light Jet</mat-option>
                      <mat-option value="MidSizeJet">Mid-Size Jet</mat-option>
                      <mat-option value="HeavyJet">Heavy Jet</mat-option>
                    </mat-select>
                  </div>

                  <button mat-stroked-button class="w-full" (click)="clearFilters()">
                    {{ 'flight.search.clearFilters' | transloco }}
                  </button>
                </mat-card-content>
              </mat-card>
            </div>

            <!-- Results -->
            <div class="lg:col-span-3">
              @if (isLoading) {
                <div class="flex justify-center py-12">
                  <mat-spinner></mat-spinner>
                </div>
              } @else {
                <div class="mb-4 flex justify-between items-center">
                  <p class="text-gray-600">
                    {{ 'flight.search.results' | transloco : { count: flights.length } }}
                  </p>
                  <mat-form-field appearance="outline" class="w-48">
                    <mat-label>Trier par</mat-label>
                    <mat-select formControlName="sortBy" (selectionChange)="onSearch()">
                      <mat-option value="price">Prix</mat-option>
                      <mat-option value="duration">Durée</mat-option>
                      <mat-option value="departure">Départ</mat-option>
                    </mat-select>
                  </mat-form-field>
                </div>

                <app-featured-flights [flights]="flights"></app-featured-flights>
              }
            </div>
          </div>
        </div>
      </section>
    </div>
  `,
  styles: [`
    :host ::ng-deep .mat-mdc-form-field-wrapper {
      margin-bottom: 0;
    }
    
    :host ::ng-deep .search-form .mat-mdc-text-field-wrapper {
      background-color: rgba(255, 255, 255, 0.9);
    }
  `]
})
export class FlightSearchComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private fb = inject(FormBuilder);
  private flightService = inject(FlightService);
  airportService = inject(AirportService);
  
  flights: Flight[] = [];
  isLoading = false;
  
  departureAirports$!: Observable<Airport[]>;
  arrivalAirports$!: Observable<Airport[]>;
  
  searchForm = this.fb.group({
    departure: [''],
    arrival: [''],
    date: [new Date()],
    passengers: [1],
    minPrice: [0],
    maxPrice: [50000],
    aircraftType: [[]],
    sortBy: ['price']
  });

  ngOnInit(): void {
    // Check for query params from home page
    this.route.queryParams.subscribe(params => {
      if (params['destination']) {
        this.searchForm.patchValue({ arrival: params['destination'] });
      }
      this.onSearch();
    });
  }

  onAirportSearch(field: 'departure' | 'arrival', event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    
    if (field === 'departure') {
      this.departureAirports$ = this.airportService.searchAirports(value);
    } else {
      this.arrivalAirports$ = this.airportService.searchAirports(value);
    }
  }

  displayAirport(airport: Airport): string {
    return airport ? this.airportService.formatAirportDisplay(airport) : '';
  }

  formatPrice(value: number): string {
    return `€${value / 1000}k`;
  }

  onSearch(): void {
    this.isLoading = true;
    
    const criteria: FlightSearchCriteria = {
      departure: this.searchForm.get('departure')?.value?.code,
      arrival: this.searchForm.get('arrival')?.value?.code,
      date: this.searchForm.get('date')?.value,
      passengers: this.searchForm.get('passengers')?.value,
      minPrice: this.searchForm.get('minPrice')?.value,
      maxPrice: this.searchForm.get('maxPrice')?.value,
      aircraftType: this.searchForm.get('aircraftType')?.value,
      sortBy: this.searchForm.get('sortBy')?.value
    };

    this.flightService.searchFlights(criteria).subscribe({
      next: (result) => {
        this.flights = result.flights;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Search error:', error);
        this.isLoading = false;
      }
    });
  }

  clearFilters(): void {
    this.searchForm.patchValue({
      minPrice: 0,
      maxPrice: 50000,
      aircraftType: []
    });
    this.onSearch();
  }
}