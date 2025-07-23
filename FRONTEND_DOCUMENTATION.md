# EmptyLegs Frontend - React/Next.js Application

This documentation provides a comprehensive overview of the EmptyLegs frontend application built with React, Next.js 14, TypeScript, and shadcn/ui components.

## 🏗️ Architecture Overview

### Technology Stack

- **Framework**: Next.js 14 with App Router
- **Language**: TypeScript
- **Styling**: Tailwind CSS
- **UI Components**: shadcn/ui (built on Radix UI)
- **State Management**: Zustand
- **Data Fetching**: TanStack Query (React Query)
- **Form Handling**: React Hook Form with Zod validation
- **Icons**: Lucide React
- **Date Handling**: date-fns

### Project Structure

```
frontend/
├── src/
│   ├── app/                    # Next.js App Router pages
│   │   ├── auth/              # Authentication pages
│   │   ├── dashboard/         # User dashboard
│   │   ├── flights/           # Flight search and details
│   │   ├── layout.tsx         # Root layout
│   │   ├── page.tsx           # Homepage
│   │   └── globals.css        # Global styles
│   ├── components/            # Reusable components
│   │   ├── ui/               # shadcn/ui components
│   │   ├── forms/            # Form components
│   │   └── layout/           # Layout components
│   ├── lib/                  # Utility functions
│   ├── providers/            # Context providers
│   ├── services/             # API services
│   ├── store/                # Zustand stores
│   └── types/                # TypeScript type definitions
├── public/                   # Static assets
└── package.json
```

## 🔐 Authentication System

### Auth Store (Zustand)

Located in `src/store/auth.store.ts`, handles:
- User login/logout
- Registration
- Token management (with automatic refresh)
- User state persistence

### Authentication Flow

1. **Login/Register**: Users authenticate via `/auth/login` or `/auth/register`
2. **Token Storage**: JWT tokens stored in localStorage with automatic refresh
3. **Route Protection**: Pages check authentication status and redirect as needed
4. **Role-based Navigation**: Different dashboards for Customer, Company, and Admin roles

### Protected Routes

- `/dashboard/*` - Customer dashboard (requires Customer role)
- `/company/*` - Company dashboard (requires Company role)  
- `/admin/*` - Admin dashboard (requires Admin role)

## 🛫 Flight Search System

### Search Form Component

**Location**: `src/components/forms/search-flights-form.tsx`

**Features**:
- Airport autocomplete with API integration
- Date picker with validation
- Passenger count selection
- Aircraft type filtering
- Price range filtering
- Form validation with Zod schema

### Airport Autocomplete

**Location**: `src/components/forms/airport-autocomplete.tsx`

**Features**:
- Real-time search with debouncing
- Popular airports display
- IATA code extraction
- Formatted display (IATA - Name, City)

### Search Results

**Location**: `src/app/flights/search/page.tsx`

**Features**:
- Real-time flight search with API integration
- Sorting (price, time, duration)
- Filtering by aircraft type
- Responsive flight cards
- Price formatting and discount display
- Amenities and company verification badges

## 🏠 Homepage

**Location**: `src/app/page.tsx`

**Sections**:
- Hero section with search form
- Features showcase
- How it works steps
- Customer testimonials
- Call-to-action sections
- Footer with navigation links

## 📊 Dashboard System

### Customer Dashboard

**Location**: `src/app/dashboard/page.tsx`

**Features**:
- Welcome message with user name
- Quick action cards (Search, Bookings, Profile)
- Recent activity timeline
- Account status display
- Member information

### Role-based Redirection

The main dashboard automatically redirects users based on their role:
- `Customer` → `/dashboard` (customer dashboard)
- `Company` → `/company/dashboard` (company dashboard)
- `Admin` → `/admin/dashboard` (admin dashboard)

## 🔧 Services & API Integration

### API Configuration

**Location**: `src/lib/api.ts`

**Features**:
- Axios instance with base configuration
- Automatic token injection
- Token refresh interceptor
- Error handling with automatic logout on 401

### Service Classes

All API interactions are organized into service classes:

- **AuthService** (`src/services/auth.service.ts`)
  - Login, register, logout
  - Password management
  - Email verification

- **FlightsService** (`src/services/flights.service.ts`)
  - Flight search and filtering
  - Flight details retrieval
  - Flight management (for companies)

- **BookingsService** (`src/services/bookings.service.ts`)
  - Booking creation and management
  - Booking status updates
  - Payment handling

- **AirportsService** (`src/services/airports.service.ts`)
  - Airport search and autocomplete
  - Location-based queries

### Data Fetching with React Query

All API calls use TanStack Query for:
- Automatic caching
- Background refetching
- Loading and error states
- Optimistic updates

## 🎨 UI Components & Design System

### shadcn/ui Components

The application uses shadcn/ui components for consistent design:

- **Forms**: Input, Select, Calendar, Command
- **Layout**: Card, Dialog, Popover, Toast
- **Navigation**: Button, DropdownMenu, Badge
- **Feedback**: Alert, Skeleton, Progress

### Custom Components

- **SearchFlightsForm**: Main flight search interface
- **AirportAutocomplete**: Smart airport selection
- **Header**: Navigation with role-based menus
- **FlightCard**: Flight result display

### Styling System

- **Base Styles**: Tailwind CSS with custom color palette
- **Component Variants**: Using class-variance-authority (cva)
- **Responsive Design**: Mobile-first approach
- **Dark Mode**: Supported via next-themes
- **Animations**: Custom CSS animations and Tailwind utilities

## 📱 Responsive Design

### Breakpoints

- **Mobile**: < 640px (sm)
- **Tablet**: 640px - 1024px (md, lg)
- **Desktop**: > 1024px (xl, 2xl)

### Mobile Features

- Collapsible navigation menu
- Touch-friendly form controls
- Responsive grid layouts
- Optimized search interface

## 🔧 Development Features

### Type Safety

- Comprehensive TypeScript types in `src/types/index.ts`
- API response types matching backend DTOs
- Form validation schemas with Zod
- Strict type checking enabled

### Developer Experience

- Hot reloading with Next.js
- TypeScript error checking
- Tailwind CSS IntelliSense
- Component auto-completion
- ESLint configuration

### Environment Configuration

```env
NEXT_PUBLIC_API_URL=http://localhost:5000/api/v1
NEXT_PUBLIC_STRIPE_PUBLISHABLE_KEY=pk_test_your_stripe_key_here
```

## 🚀 Getting Started

### Prerequisites

- Node.js 18+
- npm or yarn
- Backend API running on port 5000

### Installation

```bash
cd frontend
npm install
```

### Development

```bash
npm run dev
```

The application will be available at `http://localhost:3000`

### Building for Production

```bash
npm run build
npm start
```

## 🔍 Key Features Implemented

### ✅ Authentication
- [x] User registration with role selection
- [x] Login with credential validation
- [x] Automatic token refresh
- [x] Role-based routing
- [x] Protected routes

### ✅ Flight Search
- [x] Airport autocomplete
- [x] Advanced search filters
- [x] Real-time search results
- [x] Sorting and filtering
- [x] Responsive flight cards

### ✅ User Interface
- [x] Modern, clean design
- [x] Responsive layout
- [x] Dark mode support
- [x] Accessible components
- [x] Loading states and error handling

### ✅ State Management
- [x] Global auth state with Zustand
- [x] Persistent user sessions
- [x] API state with React Query
- [x] Form state with React Hook Form

## 🎯 User Flows

### 1. New User Registration

1. Visit homepage → Click "Sign Up"
2. Choose account type (Customer/Company)
3. Fill registration form with validation
4. Automatic login after successful registration
5. Redirect to appropriate dashboard

### 2. Flight Search

1. Use search form on homepage or dedicated search page
2. Enter departure/arrival airports with autocomplete
3. Select date and passenger count
4. Apply filters (aircraft type, price range)
5. View results with sorting options
6. Click flight for detailed view

### 3. Customer Journey

1. Register as Customer
2. Access customer dashboard
3. Search for flights
4. View flight details
5. Create booking (future implementation)
6. Manage bookings in dashboard

## 🔮 Future Enhancements

### Planned Features

- [ ] Flight booking flow with payment integration
- [ ] User profile management
- [ ] Booking history and management
- [ ] Company dashboard for flight management
- [ ] Admin dashboard for system management
- [ ] Real-time notifications
- [ ] Mobile app (React Native)
- [ ] Social login (Google, Apple)
- [ ] Multi-language support
- [ ] Advanced filters and search
- [ ] Map integration for flight routes
- [ ] Price alerts and notifications

### Technical Improvements

- [ ] Server-side rendering optimization
- [ ] Progressive Web App (PWA)
- [ ] Enhanced error boundaries
- [ ] Performance monitoring
- [ ] Unit and integration tests
- [ ] E2E testing with Playwright
- [ ] Bundle optimization
- [ ] SEO improvements

## 📞 API Integration

### Base URL

Development: `http://localhost:5000/api/v1`

### Authentication Headers

```javascript
headers: {
  'Authorization': 'Bearer <jwt-token>',
  'Content-Type': 'application/json'
}
```

### Error Handling

The application handles API errors gracefully:

- **401 Unauthorized**: Automatic token refresh or redirect to login
- **400 Bad Request**: Form validation errors displayed
- **500 Server Error**: User-friendly error messages
- **Network Errors**: Retry mechanisms and offline indicators

## 🎨 Design System

### Color Palette

- **Primary**: Blue shades for main actions and branding
- **Secondary**: Gray tones for secondary elements
- **Success**: Green for confirmations and success states
- **Destructive**: Red for errors and destructive actions
- **Muted**: Light gray for backgrounds and disabled states

### Typography

- **Headings**: Font weights 600-700
- **Body**: Font weight 400
- **Small text**: Font size 14px for metadata
- **Font Family**: Inter (via Next.js fonts)

### Spacing

- **Consistent spacing**: 4px base unit (Tailwind spacing scale)
- **Component padding**: 16px-24px standard
- **Section gaps**: 32px-48px between major sections

## 🔧 Component Architecture

### Composition Pattern

Components are built using composition:

```tsx
<Card>
  <CardHeader>
    <CardTitle>Flight Details</CardTitle>
  </CardHeader>
  <CardContent>
    {/* Content */}
  </CardContent>
</Card>
```

### Props Pattern

Clear, typed props interfaces:

```tsx
interface FlightCardProps {
  flight: Flight
  onBook: (flightId: string) => void
  showPrice?: boolean
}
```

### Hooks Usage

Custom hooks for repeated logic:
- `useAuthStore()` - Authentication state
- `useToast()` - Toast notifications
- `useQuery()` - Data fetching

This frontend application provides a solid foundation for the EmptyLegs platform with modern development practices, excellent user experience, and seamless API integration.