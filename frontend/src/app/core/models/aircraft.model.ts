export interface Aircraft {
  id: string;
  companyId: string;
  model: string;
  manufacturer: string;
  type: AircraftType;
  registrationNumber: string;
  capacity: number;
  range: number; // in kilometers
  cruiseSpeed: number; // in km/h
  yearOfManufacture: number;
  images: string[];
  amenities: string[];
  description?: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

export enum AircraftType {
  VeryLightJet = 'VeryLightJet',
  LightJet = 'LightJet',
  MidSizeJet = 'MidSizeJet',
  SuperMidSizeJet = 'SuperMidSizeJet',
  HeavyJet = 'HeavyJet',
  UltraLongRangeJet = 'UltraLongRangeJet',
  Turboprop = 'Turboprop',
  Helicopter = 'Helicopter'
}

export const AircraftTypeLabels: Record<AircraftType, string> = {
  [AircraftType.VeryLightJet]: 'Very Light Jet',
  [AircraftType.LightJet]: 'Light Jet',
  [AircraftType.MidSizeJet]: 'Mid-Size Jet',
  [AircraftType.SuperMidSizeJet]: 'Super Mid-Size Jet',
  [AircraftType.HeavyJet]: 'Heavy Jet',
  [AircraftType.UltraLongRangeJet]: 'Ultra Long Range Jet',
  [AircraftType.Turboprop]: 'Turboprop',
  [AircraftType.Helicopter]: 'Helicopter'
};