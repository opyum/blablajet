export enum UserRole {
  Customer = 'Customer',
  Company = 'Company',
  Admin = 'Admin'
}

export enum LoyaltyLevel {
  Bronze = 'Bronze',
  Silver = 'Silver',
  Gold = 'Gold',
  Platinum = 'Platinum'
}

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  role: UserRole;
  isEmailVerified: boolean;
  createdAt: Date;
  updatedAt: Date;
  lastLoginAt?: Date;
  profilePictureUrl?: string;
  preferredLanguage: string;
  notificationPreferences?: NotificationPreferences;
}

export interface Customer extends User {
  dateOfBirth?: Date;
  nationality?: string;
  passportNumber?: string;
  passportExpiry?: Date;
  loyaltyPoints: number;
  loyaltyLevel: LoyaltyLevel;
  totalFlights: number;
  totalSpent: number;
  savedAmount: number;
}

export interface CompanyUser extends User {
  companyId: string;
  position?: string;
  department?: string;
}

export interface NotificationPreferences {
  emailNotifications: boolean;
  smsNotifications: boolean;
  marketingEmails: boolean;
  bookingUpdates: boolean;
  flightAlerts: boolean;
  specialOffers: boolean;
}