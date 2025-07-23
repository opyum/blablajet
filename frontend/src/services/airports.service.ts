import { api } from '@/lib/api';
import { Airport, PaginatedResponse } from '@/types';

export class AirportsService {
  // Get all airports with filters
  static async getAirports(
    page: number = 1,
    pageSize: number = 50,
    filters?: {
      country?: string;
      city?: string;
      search?: string;
      isActive?: boolean;
    }
  ): Promise<PaginatedResponse<Airport>> {
    const response = await api.get<PaginatedResponse<Airport>>('/airports', {
      params: {
        page,
        pageSize,
        ...filters,
      },
    });
    return response.data;
  }

  // Get airport by ID
  static async getAirportById(id: string): Promise<Airport> {
    const response = await api.get<Airport>(`/airports/${id}`);
    return response.data;
  }

  // Get airport by IATA code
  static async getAirportByIata(iataCode: string): Promise<Airport> {
    const response = await api.get<Airport>(`/airports/iata/${iataCode}`);
    return response.data;
  }

  // Search airports with advanced filters
  static async searchAirports(searchTerm: string): Promise<Airport[]> {
    const response = await api.get<Airport[]>('/airports/search', {
      params: { search: searchTerm },
    });
    return response.data;
  }

  // Get nearby airports
  static async getNearbyAirports(
    latitude: number,
    longitude: number,
    radiusKm: number = 100
  ): Promise<Airport[]> {
    const response = await api.get<Airport[]>('/airports/nearby', {
      params: {
        latitude,
        longitude,
        radiusKm,
      },
    });
    return response.data;
  }

  // Get list of countries with airports
  static async getCountries(): Promise<string[]> {
    const response = await api.get<string[]>('/airports/countries');
    return response.data;
  }

  // Get cities in a country with airports
  static async getCitiesInCountry(country: string): Promise<string[]> {
    const response = await api.get<string[]>(`/airports/countries/${country}/cities`);
    return response.data;
  }

  // Create new airport (Admin only)
  static async createAirport(airportData: Omit<Airport, 'id'>): Promise<Airport> {
    const response = await api.post<Airport>('/airports', airportData);
    return response.data;
  }

  // Update airport (Admin only)
  static async updateAirport(id: string, airportData: Partial<Airport>): Promise<Airport> {
    const response = await api.put<Airport>(`/airports/${id}`, airportData);
    return response.data;
  }

  // Deactivate airport (Admin only)
  static async deactivateAirport(id: string): Promise<void> {
    await api.patch(`/airports/${id}/deactivate`);
  }

  // Delete airport (Admin only)
  static async deleteAirport(id: string): Promise<void> {
    await api.delete(`/airports/${id}`);
  }

  // Helper function to format airport display
  static formatAirportDisplay(airport: Airport): string {
    return `${airport.iataCode} - ${airport.name}, ${airport.city}`;
  }

  // Helper function to search airports for autocomplete
  static async searchForAutocomplete(searchTerm: string): Promise<Airport[]> {
    if (searchTerm.length < 2) return [];
    
    const airports = await this.searchAirports(searchTerm);
    return airports.slice(0, 10); // Limit to 10 results for autocomplete
  }
}