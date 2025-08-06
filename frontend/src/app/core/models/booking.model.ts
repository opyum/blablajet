export enum BookingStatus {
  Pending = 'Pending',
  Confirmed = 'Confirmed',
  Cancelled = 'Cancelled',
  Completed = 'Completed',
  Refunded = 'Refunded'
}

export interface Booking {
  id: string;
  bookingReference: string;
  userId: string;
  flightId: string;
  flight?: any; // Will be populated with flight details
  status: BookingStatus;
  totalAmount: number;
  subtotal: number;
  taxes: number;
  fees: number;
  numberOfPassengers: number;
  passengers: Passenger[];
  paymentId?: string;
  paymentStatus: string;
  createdAt: Date;
  updatedAt: Date;
  cancelledAt?: Date;
  cancellationReason?: string;
}

export interface Passenger {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  dateOfBirth: Date;
  nationality: string;
  passportNumber: string;
  passportExpiry: Date;
  specialRequests?: string;
}

export interface BookingCreateRequest {
  flightId: string;
  passengers: Passenger[];
  paymentMethodId: string;
  acceptTerms: boolean;
}

export interface BookingUpdateRequest {
  passengers?: Passenger[];
  specialRequests?: string;
}