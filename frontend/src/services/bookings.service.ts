import { api } from '@/lib/api';
import { 
  Booking, 
  CreateBooking, 
  UpdateBookingStatus,
  PaginatedResponse,
  Passenger,
  BookingStatus
} from '@/types';

export class BookingsService {
  // Get user's bookings
  static async getBookings(
    page: number = 1,
    pageSize: number = 20,
    status?: BookingStatus,
    userId?: string
  ): Promise<PaginatedResponse<Booking>> {
    const response = await api.get<PaginatedResponse<Booking>>('/bookings', {
      params: {
        page,
        pageSize,
        status,
        userId,
      },
    });
    return response.data;
  }

  // Get booking by ID
  static async getBookingById(id: string): Promise<Booking> {
    const response = await api.get<Booking>(`/bookings/${id}`);
    return response.data;
  }

  // Create new booking
  static async createBooking(bookingData: CreateBooking): Promise<Booking> {
    const response = await api.post<Booking>('/bookings', bookingData);
    return response.data;
  }

  // Update booking status (Company/Admin only)
  static async updateBookingStatus(
    id: string, 
    statusData: UpdateBookingStatus
  ): Promise<Booking> {
    const response = await api.patch<Booking>(`/bookings/${id}/status`, statusData);
    return response.data;
  }

  // Cancel booking
  static async cancelBooking(id: string, cancellationReason: string): Promise<Booking> {
    const response = await api.post<Booking>(`/bookings/${id}/cancel`, {
      cancellationReason,
    });
    return response.data;
  }

  // Confirm booking (Company/Admin only)
  static async confirmBooking(id: string): Promise<Booking> {
    const response = await api.post<Booking>(`/bookings/${id}/confirm`);
    return response.data;
  }

  // Complete booking (Company/Admin only)
  static async completeBooking(id: string): Promise<Booking> {
    const response = await api.post<Booking>(`/bookings/${id}/complete`);
    return response.data;
  }

  // Get booking passengers
  static async getBookingPassengers(id: string): Promise<Passenger[]> {
    const response = await api.get<Passenger[]>(`/bookings/${id}/passengers`);
    return response.data;
  }

  // Get booking payments
  static async getBookingPayments(id: string): Promise<any[]> {
    const response = await api.get<any[]>(`/bookings/${id}/payments`);
    return response.data;
  }

  // Get booking statistics (Admin only)
  static async getBookingStatistics(): Promise<{
    totalBookings: number;
    confirmedBookings: number;
    pendingBookings: number;
    cancelledBookings: number;
    totalRevenue: number;
    averageBookingValue: number;
  }> {
    const response = await api.get('/bookings/statistics');
    return response.data;
  }

  // Calculate booking total
  static calculateBookingTotal(
    basePrice: number,
    passengerCount: number,
    additionalServices: { price: number; quantity: number }[] = []
  ): number {
    const flightTotal = basePrice * passengerCount;
    const servicesTotal = additionalServices.reduce(
      (sum, service) => sum + (service.price * service.quantity),
      0
    );
    const serviceFees = (flightTotal + servicesTotal) * 0.05; // 5% service fee
    
    return flightTotal + servicesTotal + serviceFees;
  }
}