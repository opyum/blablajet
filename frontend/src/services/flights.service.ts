import { api } from '@/lib/api';
import { 
  Flight, 
  FlightSearch, 
  FlightSearchResult, 
  CreateFlight,
  PaginatedResponse 
} from '@/types';

export class FlightsService {
  // Search flights
  static async searchFlights(searchParams: FlightSearch): Promise<FlightSearchResult> {
    const response = await api.get<FlightSearchResult>('/flights/search', {
      params: searchParams,
    });
    return response.data;
  }

  // Get flight by ID
  static async getFlightById(id: string): Promise<Flight> {
    const response = await api.get<Flight>(`/flights/${id}`);
    return response.data;
  }

  // Create new flight (Company/Admin only)
  static async createFlight(flightData: CreateFlight): Promise<Flight> {
    const response = await api.post<Flight>('/flights', flightData);
    return response.data;
  }

  // Update flight (Company/Admin only)
  static async updateFlight(id: string, flightData: Partial<CreateFlight>): Promise<Flight> {
    const response = await api.put<Flight>(`/flights/${id}`, flightData);
    return response.data;
  }

  // Delete flight (Company/Admin only)
  static async deleteFlight(id: string): Promise<void> {
    await api.delete(`/flights/${id}`);
  }

  // Get flights with pagination (for admin/company dashboard)
  static async getFlights(
    page: number = 1,
    pageSize: number = 20,
    filters?: {
      status?: string;
      companyId?: string;
      aircraftId?: string;
    }
  ): Promise<PaginatedResponse<Flight>> {
    const response = await api.get<PaginatedResponse<Flight>>('/flights', {
      params: {
        page,
        pageSize,
        ...filters,
      },
    });
    return response.data;
  }

  // Get popular destinations
  static async getPopularDestinations(): Promise<{ city: string; country: string; count: number }[]> {
    const response = await api.get('/flights/destinations/popular');
    return response.data;
  }

  // Get flight statistics (Admin only)
  static async getFlightStatistics(): Promise<{
    totalFlights: number;
    availableFlights: number;
    bookedFlights: number;
    averageOccupancy: number;
    totalRevenue: number;
  }> {
    const response = await api.get('/flights/statistics');
    return response.data;
  }
}