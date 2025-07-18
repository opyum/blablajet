// Enums
export enum UserRole {
  Customer = 1,
  Company = 2,
  Admin = 3,
}

export enum FlightStatus {
  Available = 1,
  Reserved = 2,
  Confirmed = 3,
  InProgress = 4,
  Completed = 5,
  Cancelled = 6,
}

export enum BookingStatus {
  Pending = 1,
  Confirmed = 2,
  PaymentPending = 3,
  PaymentConfirmed = 4,
  Cancelled = 5,
  Refunded = 6,
  Completed = 7,
}

export enum AircraftType {
  Turboprop = 1,
  LightJet = 2,
  MidJet = 3,
  HeavyJet = 4,
  VeryLightJet = 5,
}

// Base Types
export interface BaseEntity {
  id: string;
  createdAt: string;
  updatedAt: string;
  isDeleted: boolean;
  deletedAt?: string;
}

// Core Entities
export interface User extends BaseEntity {
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  dateOfBirth?: string;
  role: UserRole;
  isActive: boolean;
  isEmailVerified: boolean;
  companyId?: string;
  company?: Company;
  fullName: string;
}

export interface Company extends BaseEntity {
  name: string;
  description: string;
  license: string;
  logoUrl?: string;
  contactEmail: string;
  contactPhone: string;
  address: string;
  city: string;
  country: string;
  website?: string;
  isVerified: boolean;
  isActive: boolean;
  averageRating: number;
  totalReviews: number;
}

export interface Aircraft extends BaseEntity {
  model: string;
  manufacturer: string;
  registration: string;
  capacity: number;
  type: AircraftType;
  yearManufactured: number;
  description: string;
  cruiseSpeed?: number;
  range?: number;
  isActive: boolean;
  amenities: string[];
  photoUrls: string[];
  companyId: string;
  company: Company;
}

export interface Airport extends BaseEntity {
  iataCode: string;
  icaoCode: string;
  name: string;
  city: string;
  country: string;
  latitude: number;
  longitude: number;
  timeZone: string;
  isActive: boolean;
}

export interface Flight extends BaseEntity {
  flightNumber: string;
  departureTime: string;
  arrivalTime: string;
  basePrice: number;
  currentPrice: number;
  availableSeats: number;
  totalSeats: number;
  status: FlightStatus;
  description?: string;
  specialInstructions?: string;
  allowsAutomaticPricing: boolean;
  minimumPrice?: number;
  departureAirportId: string;
  departureAirport: Airport;
  arrivalAirportId: string;
  arrivalAirport: Airport;
  aircraftId: string;
  aircraft: Aircraft;
  companyId: string;
  company: Company;
  bookedSeats: number;
  occupancyRate: number;
  duration: string;
  isFullyBooked: boolean;
}

export interface Booking extends BaseEntity {
  bookingReference: string;
  status: BookingStatus;
  passengerCount: number;
  totalPrice: number;
  serviceFees: number;
  bookingDate: string;
  specialRequests?: string;
  cancellationReason?: string;
  cancelledAt?: string;
  flightId: string;
  flight: Flight;
  userId: string;
  user: User;
  passengers: Passenger[];
  payments: Payment[];
  additionalServices: BookingService[];
  totalAmount: number;
  canBeCancelled: boolean;
}

export interface Passenger extends BaseEntity {
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  passportNumber?: string;
  nationality?: string;
  specialRequests?: string;
  bookingId: string;
  documents: Document[];
  fullName: string;
  age: number;
}

export interface Payment extends BaseEntity {
  stripePaymentIntentId: string;
  amount: number;
  currency: string;
  status: string;
  paymentMethod: string;
  processedAt?: string;
  failureReason?: string;
  refundReason?: string;
  refundAmount?: number;
  refundedAt?: string;
  bookingId: string;
  isSuccessful: boolean;
  isRefunded: boolean;
}

export interface Document extends BaseEntity {
  type: string;
  fileName: string;
  fileUrl: string;
  contentType: string;
  fileSize: number;
  isVerified: boolean;
  verifiedAt?: string;
  verifiedBy?: string;
  passengerId: string;
}

export interface Review extends BaseEntity {
  rating: number;
  title: string;
  comment: string;
  isVerified: boolean;
  isVisible: boolean;
  userId: string;
  user: User;
  flightId?: string;
  flight?: Flight;
  companyId?: string;
  company?: Company;
}

export interface UserAlert extends BaseEntity {
  name: string;
  departureAirportCode?: string;
  arrivalAirportCode?: string;
  departureDateFrom?: string;
  departureDateTo?: string;
  minPassengers?: number;
  maxPrice?: number;
  isActive: boolean;
  emailNotifications: boolean;
  pushNotifications: boolean;
  smsNotifications: boolean;
  userId: string;
}

export interface BookingService extends BaseEntity {
  serviceType: string;
  name: string;
  description: string;
  price: number;
  quantity: number;
  bookingId: string;
  totalPrice: number;
}

// API DTOs
export interface FlightSearchRequest {
  departureAirportCode?: string;
  arrivalAirportCode?: string;
  departureDate?: string;
  passengerCount?: number;
  aircraftType?: AircraftType;
  maxPrice?: number;
  services?: string[];
  page?: number;
  pageSize?: number;
}

export interface FlightSearchResponse {
  flights: Flight[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface CreateFlightRequest {
  flightNumber: string;
  departureAirportId: string;
  arrivalAirportId: string;
  departureTime: string;
  arrivalTime?: string;
  aircraftId: string;
  basePrice: number;
  availableSeats: number;
  description?: string;
  specialInstructions?: string;
  allowsAutomaticPricing?: boolean;
  minimumPrice?: number;
}

export interface CreateBookingRequest {
  flightId: string;
  passengerCount: number;
  passengers: {
    firstName: string;
    lastName: string;
    dateOfBirth: string;
    passportNumber?: string;
    nationality?: string;
    specialRequests?: string;
  }[];
  specialRequests?: string;
  additionalServices?: {
    serviceType: string;
    name: string;
    price: number;
    quantity: number;
  }[];
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  user: User;
  expiresIn: number;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  role: UserRole;
  companyId?: string;
}

// API Response wrappers
export interface ApiResponse<T> {
  data: T;
  message?: string;
  success: boolean;
}

export interface ApiError {
  message: string;
  errors?: Record<string, string[]>;
  statusCode: number;
}

export interface PaginatedResponse<T> {
  data: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}