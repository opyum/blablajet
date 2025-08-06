export enum FlightStatus {
  Available = 'Available',
  Booked = 'Booked',
  Cancelled = 'Cancelled',
  Completed = 'Completed',
  InProgress = 'InProgress'
}

export interface Flight {
  id: string;
  flightNumber: string;
  companyId: string;
  companyName: string;
  aircraftId: string;
  departureAirportId: string;
  departureAirportCode: string;
  departureAirportName: string;
  departureCity: string;
  departureCountry: string;
  arrivalAirportId: string;
  arrivalAirportCode: string;
  arrivalAirportName: string;
  arrivalCity: string;
  arrivalCountry: string;
  departureTime: Date;
  arrivalTime: Date;
  price: number;
  originalPrice: number;
  discount: number;
  availableSeats: number;
  totalSeats: number;
  status: FlightStatus;
  distance: number;
  duration: number;
  description?: string;
  amenities: string[];
  images: string[];
  terms?: string;
  createdAt: Date;
  updatedAt: Date;
}

export interface FlightSearchCriteria {
  departure?: string;
  arrival?: string;
  date?: Date;
  dateFlexibility?: number;
  passengers?: number;
  minPrice?: number;
  maxPrice?: number;
  aircraftType?: string[];
  sortBy?: 'price' | 'duration' | 'departure';
  sortOrder?: 'asc' | 'desc';
}

export interface FlightSearchResult {
  flights: Flight[];
  totalCount: number;
  page: number;
  pageSize: number;
}