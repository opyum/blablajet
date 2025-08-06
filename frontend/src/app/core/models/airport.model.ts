export interface Airport {
  id: string;
  code: string; // IATA code
  icaoCode: string; // ICAO code
  name: string;
  city: string;
  country: string;
  countryCode: string;
  latitude: number;
  longitude: number;
  timezone: string;
  type: AirportType;
  isActive: boolean;
}

export enum AirportType {
  International = 'International',
  Domestic = 'Domestic',
  Regional = 'Regional',
  Private = 'Private',
  Military = 'Military'
}

export interface AirportSearchResult {
  airports: Airport[];
  query: string;
}