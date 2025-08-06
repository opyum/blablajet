export interface Company {
  id: string;
  name: string;
  legalName: string;
  registrationNumber: string;
  vatNumber?: string;
  email: string;
  phoneNumber: string;
  website?: string;
  logo?: string;
  description?: string;
  address: Address;
  bankDetails?: BankDetails;
  certifications: Certification[];
  operatingLicense: string;
  insurancePolicy: string;
  safetyRating: number;
  totalFlights: number;
  totalRevenue: number;
  commissionRate: number;
  status: CompanyStatus;
  isVerified: boolean;
  verifiedAt?: Date;
  createdAt: Date;
  updatedAt: Date;
}

export interface Address {
  street: string;
  city: string;
  state?: string;
  postalCode: string;
  country: string;
}

export interface BankDetails {
  bankName: string;
  accountName: string;
  accountNumber: string;
  iban?: string;
  swift?: string;
}

export interface Certification {
  name: string;
  issuedBy: string;
  issuedDate: Date;
  expiryDate: Date;
  documentUrl: string;
}

export enum CompanyStatus {
  Pending = 'Pending',
  Active = 'Active',
  Suspended = 'Suspended',
  Inactive = 'Inactive'
}