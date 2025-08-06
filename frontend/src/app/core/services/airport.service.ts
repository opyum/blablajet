import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { BaseApiService } from './base-api.service';
import { Airport, AirportSearchResult } from '@core/models';

@Injectable({
  providedIn: 'root'
})
export class AirportService extends BaseApiService {
  
  private airportsCache: Map<string, Airport[]> = new Map();

  searchAirports(query: string): Observable<Airport[]> {
    if (!query || query.length < 2) {
      return of([]);
    }

    // Check cache first
    const cacheKey = query.toLowerCase();
    if (this.airportsCache.has(cacheKey)) {
      return of(this.airportsCache.get(cacheKey)!);
    }

    return this.get<AirportSearchResult>('/api/airports/search', { q: query }).pipe(
      map(result => result.airports),
      map(airports => {
        // Cache the results
        this.airportsCache.set(cacheKey, airports);
        return airports;
      })
    );
  }

  getAirportById(id: string): Observable<Airport> {
    return this.get<Airport>(`/api/airports/${id}`);
  }

  getAirportByCode(code: string): Observable<Airport> {
    return this.get<Airport>(`/api/airports/code/${code}`);
  }

  getPopularAirports(): Observable<Airport[]> {
    return this.get<Airport[]>('/api/airports/popular');
  }

  getNearbyAirports(latitude: number, longitude: number, radius: number = 100): Observable<Airport[]> {
    const params = this.buildParams({ latitude, longitude, radius });
    return this.get<Airport[]>('/api/airports/nearby', params);
  }

  // Helper method for autocomplete
  createAirportAutocomplete(searchTerm$: Observable<string>): Observable<Airport[]> {
    return searchTerm$.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(term => this.searchAirports(term))
    );
  }

  // Format airport for display
  formatAirportDisplay(airport: Airport): string {
    return `${airport.city} (${airport.code}) - ${airport.name}`;
  }

  formatAirportShort(airport: Airport): string {
    return `${airport.city} (${airport.code})`;
  }
}