import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseApiService } from './base-api.service';
import { Flight, FlightSearchCriteria, FlightSearchResult } from '@core/models';

@Injectable({
  providedIn: 'root'
})
export class FlightService extends BaseApiService {
  
  searchFlights(criteria: FlightSearchCriteria): Observable<FlightSearchResult> {
    const params = this.buildParams(criteria);
    return this.get<FlightSearchResult>('/api/flights/search', params);
  }

  getFlightById(id: string): Observable<Flight> {
    return this.get<Flight>(`/api/flights/${id}`);
  }

  getFeaturedFlights(): Observable<Flight[]> {
    return this.get<Flight[]>('/api/flights/featured');
  }

  getUpcomingFlights(): Observable<Flight[]> {
    return this.get<Flight[]>('/api/flights/upcoming');
  }

  // Company specific endpoints
  getCompanyFlights(companyId: string): Observable<Flight[]> {
    return this.get<Flight[]>(`/api/companies/${companyId}/flights`);
  }

  createFlight(flight: Partial<Flight>): Observable<Flight> {
    return this.post<Flight>('/api/flights', flight);
  }

  updateFlight(id: string, flight: Partial<Flight>): Observable<Flight> {
    return this.put<Flight>(`/api/flights/${id}`, flight);
  }

  deleteFlight(id: string): Observable<void> {
    return this.delete<void>(`/api/flights/${id}`);
  }

  updateFlightStatus(id: string, status: string): Observable<Flight> {
    return this.patch<Flight>(`/api/flights/${id}/status`, { status });
  }

  uploadFlightImages(id: string, images: File[]): Observable<string[]> {
    const formData = new FormData();
    images.forEach((image, index) => {
      formData.append(`images`, image);
    });
    
    return this.post<string[]>(`/api/flights/${id}/images`, formData);
  }

  deleteFlightImage(flightId: string, imageUrl: string): Observable<void> {
    return this.delete<void>(`/api/flights/${flightId}/images`, {
      body: { imageUrl }
    });
  }
}