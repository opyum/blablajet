import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseApiService } from './base-api.service';
import { Booking, BookingCreateRequest, BookingUpdateRequest, BookingStatus } from '@core/models';

@Injectable({
  providedIn: 'root'
})
export class BookingService extends BaseApiService {
  
  // Customer endpoints
  getMyBookings(status?: BookingStatus): Observable<Booking[]> {
    const params = status ? this.buildParams({ status }) : undefined;
    return this.get<Booking[]>('/api/bookings/my', params);
  }

  getBookingById(id: string): Observable<Booking> {
    return this.get<Booking>(`/api/bookings/${id}`);
  }

  getBookingByReference(reference: string): Observable<Booking> {
    return this.get<Booking>(`/api/bookings/reference/${reference}`);
  }

  createBooking(booking: BookingCreateRequest): Observable<Booking> {
    return this.post<Booking>('/api/bookings', booking);
  }

  updateBooking(id: string, update: BookingUpdateRequest): Observable<Booking> {
    return this.patch<Booking>(`/api/bookings/${id}`, update);
  }

  cancelBooking(id: string, reason?: string): Observable<Booking> {
    return this.post<Booking>(`/api/bookings/${id}/cancel`, { reason });
  }

  // Company endpoints
  getCompanyBookings(companyId: string, status?: BookingStatus): Observable<Booking[]> {
    const params = status ? this.buildParams({ status }) : undefined;
    return this.get<Booking[]>(`/api/companies/${companyId}/bookings`, params);
  }

  // Admin endpoints
  getAllBookings(filters?: any): Observable<Booking[]> {
    const params = filters ? this.buildParams(filters) : undefined;
    return this.get<Booking[]>('/api/admin/bookings', params);
  }

  // Payment related
  processPayment(bookingId: string, paymentMethodId: string): Observable<any> {
    return this.post(`/api/bookings/${bookingId}/payment`, { paymentMethodId });
  }

  refundBooking(bookingId: string, amount?: number): Observable<any> {
    return this.post(`/api/bookings/${bookingId}/refund`, { amount });
  }

  // Documents
  downloadTicket(bookingId: string): Observable<Blob> {
    return this.get(`/api/bookings/${bookingId}/ticket`, {
      responseType: 'blob'
    });
  }

  downloadInvoice(bookingId: string): Observable<Blob> {
    return this.get(`/api/bookings/${bookingId}/invoice`, {
      responseType: 'blob'
    });
  }

  sendTicketByEmail(bookingId: string, email?: string): Observable<any> {
    return this.post(`/api/bookings/${bookingId}/send-ticket`, { email });
  }
}