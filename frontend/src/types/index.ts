// Enums
export enum UserRole {
  Customer = 1,
  Company = 2,
  Admin = 3
}

export enum FlightStatus {
  Available = 1,
  Reserved = 2,
  Confirmed = 3,
  InProgress = 4,
  Completed = 5,
  Cancelled = 6
}

export enum BookingStatus {
  Pending = 1,
  Confirmed = 2,
  PaymentPending = 3,
  PaymentConfirmed = 4,
  Cancelled = 5,
  Refunded = 6,
  Completed = 7
}

export enum AircraftType {
  Turboprop = 1,
  LightJet = 2,
  MidJet = 3,
  HeavyJet = 4,
  VeryLightJet = 5,
  Helicopter = 6
}

// User Types
export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  dateOfBirth?: string;
  role: UserRole;
  isActive: boolean;
  isEmailVerified: boolean;
  companyId?: string;
  companyName?: string;
  fullName: string;
  createdAt: string;
}

export interface CreateUser {
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  dateOfBirth?: string;
  role?: UserRole;
  companyId?: string;
}

export interface RegisterUser {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  dateOfBirth?: string;
  role?: UserRole;
  companyId?: string;
}

export interface Login {
  email: string;
  password: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  user: User;
  expiresIn: number;
}

// Airport Types
export interface Airport {
  id: string;
  name: string;
  iataCode: string;
  icaoCode: string;
  city: string;
  country: string;
  latitude: number;
  longitude: number;
  isActive: boolean;
}

// Aircraft Types
export interface Aircraft {
  id: string;
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
  companyName: string;
}

export interface CreateAircraft {
  model: string;
  manufacturer: string;
  registration: string;
  capacity: number;
  type: AircraftType;
  yearManufactured: number;
  description: string;
  cruiseSpeed?: number;
  range?: number;
  amenities: string[];
  photoUrls: string[];
  companyId: string;
}

// Company Types
export interface Company {
  id: string;
  name: string;
  description: string;
  email: string;
  phoneNumber: string;
  address: string;
  city: string;
  country: string;
  website?: string;
  logoUrl?: string;
  isVerified: boolean;
  isActive: boolean;
  createdAt: string;
}

export interface CreateCompany {
  name: string;
  description: string;
  email: string;
  phoneNumber: string;
  address: string;
  city: string;
  country: string;
  website?: string;
  logoUrl?: string;
}

// Flight Types
export interface Flight {
  id: string;
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
  departureAirport: Airport;
  arrivalAirport: Airport;
  aircraft: Aircraft;
  company: Company;
  bookedSeats: number;
  occupancyRate: number;
  duration: string;
  isFullyBooked: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateFlight {
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

export interface FlightSearch {
  departureAirportCode?: string;
  arrivalAirportCode?: string;
  departureDate?: string;
  departureDateFrom?: string;
  departureDateTo?: string;
  passengerCount?: number;
  aircraftType?: AircraftType;
  maxPrice?: number;
  minPrice?: number;
  services?: string[];
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface FlightSearchResult {
  flights: Flight[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

// Passenger Types
export interface Passenger {
  id: string;
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  passportNumber?: string;
  nationality?: string;
  specialRequests?: string;
  bookingId: string;
  fullName: string;
  age: number;
}

export interface CreatePassenger {
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  passportNumber?: string;
  nationality?: string;
  specialRequests?: string;
}

// Booking Service Types
export interface BookingService {
  id: string;
  serviceType: string;
  name: string;
  description: string;
  price: number;
  quantity: number;
  bookingId: string;
  totalPrice: number;
}

export interface CreateBookingService {
  serviceType: string;
  name: string;
  description: string;
  price: number;
  quantity?: number;
}

// Booking Types
export interface Booking {
  id: string;
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
  additionalServices: BookingService[];
  totalAmount: number;
  canBeCancelled: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateBooking {
  flightId: string;
  passengerCount: number;
  totalPrice: number;
  specialRequests?: string;
  passengers: CreatePassenger[];
  additionalServices: CreateBookingService[];
}

export interface UpdateBookingStatus {
  status: BookingStatus;
  cancellationReason?: string;
  reason?: string;
}

// API Response Types
export interface ApiResponse<T> {
  data: T;
  message?: string;
}

export interface PaginatedResponse<T> {
  data: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface ApiError {
  message: string;
  details?: string;
}

// Form Types
export interface SearchFormData {
  departureAirport: string;
  arrivalAirport: string;
  departureDate: Date;
  passengerCount: number;
  aircraftType?: AircraftType;
  maxPrice?: number;
}

export interface BookingFormData {
  passengers: CreatePassenger[];
  specialRequests?: string;
  additionalServices: CreateBookingService[];
}