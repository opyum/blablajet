# Frontend Web Specifications - Empty Legs Flight Platform

## Table of Contents
1. [Architecture Overview](#architecture-overview)
2. [Common Components](#common-components)
3. [Public Pages](#public-pages)
4. [Authentication Flow](#authentication-flow)
5. [Customer Dashboard](#customer-dashboard)
6. [Company Dashboard](#company-dashboard)
7. [Admin Dashboard](#admin-dashboard)
8. [Mobile Responsiveness](#mobile-responsiveness)
9. [Performance Requirements](#performance-requirements)

## Architecture Overview

### Technology Stack
- **Framework**: Next.js 14 with App Router
- **Language**: TypeScript
- **Styling**: Tailwind CSS + shadcn/ui components
- **State Management**: Zustand + React Query/TanStack Query
- **Maps**: Google Maps JavaScript API
- **Payments**: Stripe Elements
- **Real-time**: SignalR for live updates
- **UI Components**: shadcn/ui with Radix UI primitives
- **Icons**: Lucide React icons
- **Charts**: Recharts for analytics dashboards
- **Forms**: React Hook Form with Zod validation

### Folder Structure
```
src/
├── app/
│   ├── (auth)/                 # Authentication pages
│   ├── (public)/              # Public pages
│   ├── dashboard/             # Customer dashboard
│   ├── company/               # Company dashboard
│   ├── admin/                 # Admin dashboard
│   └── api/                   # API routes
├── components/
│   ├── ui/                    # Base UI components (shadcn/ui)
│   ├── common/                # Shared components
│   ├── forms/                 # Form components
│   ├── maps/                  # Map components
│   ├── charts/                # Analytics charts
│   ├── loyalty/               # Loyalty program components
│   ├── booking/               # Booking workflow components
│   ├── disruption/            # Weather/disruption management
│   └── pricing/               # Dynamic pricing components
├── lib/
│   ├── api/                   # API client functions
│   ├── auth/                  # Authentication utilities
│   ├── stores/                # Zustand stores
│   └── utils/                 # Utility functions
└── types/                     # TypeScript type definitions
```

## shadcn/ui Components Configuration

### Installation & Setup
```bash
npx shadcn-ui@latest init
npx shadcn-ui@latest add button card input label select textarea
npx shadcn-ui@latest add dialog dropdown-menu sheet tabs
npx shadcn-ui@latest add calendar date-picker badge avatar
npx shadcn-ui@latest add table pagination toast alert
npx shadcn-ui@latest add form select combobox slider
npx shadcn-ui@latest add chart progress separator
```

### Core UI Components Used

#### Primary Components
- **Button**: CTA buttons, action buttons, navigation
- **Card**: Flight cards, dashboard widgets, info panels
- **Input/Textarea**: Form fields, search inputs
- **Select/Combobox**: Dropdowns, airport selection
- **Dialog/Sheet**: Modals, sidebars, overlays
- **Table**: Data tables, booking lists, analytics

#### Form Components
- **Form**: Built with React Hook Form + Zod validation
- **Calendar/DatePicker**: Flight date selection
- **Slider**: Price range filters, passenger count
- **Badge**: Status indicators, savings labels
- **Avatar**: User profiles, company logos

#### Feedback Components
- **Toast**: Success/error notifications
- **Alert**: Important announcements
- **Progress**: Upload progress, booking steps
- **Separator**: Visual content separation

### Custom Theme Configuration
```typescript
// tailwind.config.js
module.exports = {
  theme: {
    extend: {
      colors: {
        border: "hsl(var(--border))",
        background: "hsl(var(--background))",
        foreground: "hsl(var(--foreground))",
        primary: {
          DEFAULT: "hsl(var(--primary))",
          foreground: "hsl(var(--primary-foreground))",
        },
        // Custom colors for aviation theme
        sky: {
          50: '#f0f9ff',
          500: '#0ea5e9',
          900: '#0c4a6e',
        },
        gold: {
          50: '#fffbeb',
          500: '#f59e0b',
          900: '#78350f',
        }
      },
    },
  },
}
```

## Common Components

### Header Navigation
**Component**: `components/layout/Header.tsx`

#### Public Header (Not Authenticated)
```typescript
interface PublicHeaderProps {
  // Logo linking to homepage
  // Search bar (condensed version)
  // Navigation: How it Works, About, Contact
  // Auth buttons: Login, Sign Up
  // Language selector (EN/FR)
}
```

**Visual Elements**:
- Company logo (left side, links to `/`)
- Quick search bar (center) - minimal version
- Navigation menu (right side)
- Login/Register buttons (CTA style)
- Mobile hamburger menu for responsive design

#### Authenticated Header (Customer)
```typescript
interface CustomerHeaderProps {
  user: User;
  notificationCount: number;
}
```

**Visual Elements**:
- Logo (links to `/dashboard`)
- Search bar (expanded with recent searches dropdown)
- Notifications bell icon with badge count
- User avatar dropdown with:
  - Dashboard
  - My Bookings
  - My Alerts
  - Profile Settings
  - Logout

#### Company Header
```typescript
interface CompanyHeaderProps {
  company: Company;
  pendingReservations: number;
}
```

**Visual Elements**:
- Logo (links to `/company/dashboard`)
- Company name display
- Navigation: Dashboard, Flights, Bookings, Analytics
- Notifications with pending reservation count
- Company profile dropdown

### Footer
**Component**: `components/layout/Footer.tsx`

**Sections**:
- Company information and logo
- Quick links (Privacy, Terms, Contact, FAQ)
- Social media links
- Newsletter signup
- Contact information
- Copyright notice

### Search Components

#### Universal Search Bar
**Component**: `components/search/UniversalSearch.tsx`

```typescript
interface SearchFormData {
  departure: Airport;
  arrival: Airport;
  departureDate: Date;
  returnDate?: Date;
  passengers: number;
  aircraftType?: AircraftType[];
}
```

**Features**:
- Autocomplete for airport selection with IATA codes
- Date picker with availability calendar integration
- Passenger count selector (1-19)
- Aircraft type filter (optional)
- "Search Flights" CTA button
- Recent searches dropdown
- Popular routes suggestions

#### Search Results Filter
**Component**: `components/search/SearchFilters.tsx`

**Filter Options**:
- Price range slider
- Departure time slots (Morning, Afternoon, Evening)
- Aircraft type checkboxes
- Amenities filters (WiFi, Catering, etc.)
- Company rating filter
- Availability status

## Public Pages

### 1. Homepage (`/`)
**Route**: `/`
**Component**: `app/(public)/page.tsx`

#### Layout Structure:
```typescript
interface HomepageLayout {
  hero: HeroSection;
  searchBar: UniversalSearch;
  featuredFlights: FeaturedFlights;
  howItWorks: HowItWorksSection;
  testimonials: TestimonialsSection;
  interactiveMap: FlightMap;
  footer: Footer;
}
```

#### Hero Section
**Component**: `components/sections/Hero.tsx`

**Visual Elements**:
- Background: High-quality private jet image/video
- Overlay: Semi-transparent dark layer
- Heading: "Luxury Private Flights at Unbeatable Prices"
- Subheading: "Book empty leg flights and save up to 75% on private aviation"
- Large search form (prominent)
- Statistics: "1000+ flights available" "50+ operators" "75% savings"

#### Featured Flights Section
**Component**: `components/sections/FeaturedFlights.tsx`

**Layout**:
- Section title: "Featured Empty Leg Flights"
- Grid of 6-8 flight cards (3-4 per row on desktop)
- "View All Flights" button

**Flight Card Design**:
```typescript
interface FlightCardProps {
  flight: {
    id: string;
    route: string; // "NYC → MIA"
    date: string;
    time: string;
    aircraft: string;
    price: number;
    originalPrice: number;
    savings: string; // "Save 65%"
    image: string;
    company: string;
    availableSeats: number;
  };
}
```

**Card Elements**:
- Aircraft image (top)
- Route display with arrow
- Date and time
- Aircraft type
- Company name with rating
- Price with savings badge
- "Book Now" button
- Seats available indicator

#### How It Works Section
**Component**: `components/sections/HowItWorks.tsx`

**3-Step Process**:
1. **Search**: "Find your perfect flight"
2. **Book**: "Reserve in minutes"
3. **Fly**: "Enjoy luxury travel"

Each step has icon, title, description, and optional animation.

#### Interactive Map Section
**Component**: `components/maps/InteractiveMap.tsx`

**Features**:
- Full-width Google Maps integration
- Flight route overlays with animated paths
- Airport markers with flight counts
- Clickable routes showing flight details
- Filter controls (date range, aircraft type)
- "Explore Flights" CTA overlay

### 2. Search Results (`/search`)
**Route**: `/search?[params]`
**Component**: `app/(public)/search/page.tsx`

#### Layout Structure:
```typescript
interface SearchResultsLayout {
  searchBar: UniversalSearch; // Sticky header
  filtersPanel: SearchFilters; // Left sidebar
  resultsGrid: FlightResults; // Main content
  mapView: FlightMap; // Toggle with grid view
}
```

#### Search Bar (Sticky)
- Condensed version of homepage search
- Displays current search criteria
- "Modify Search" functionality
- Results count display

#### Filters Panel (Left Sidebar)
**Component**: `components/search/FiltersPanel.tsx`

**Filter Categories**:
- **Price Range**: Slider with min/max values
- **Departure Time**: Time slot buttons
- **Aircraft Type**: Checkbox list with icons
- **Amenities**: WiFi, Catering, Bar, etc.
- **Operator Rating**: Star rating filter
- **Airports**: Departure/arrival airport options

#### Results Grid (Main Content)
**Component**: `components/search/ResultsGrid.tsx`

**Header Options**:
- View toggle: Grid/List/Map
- Sort dropdown: Price, Time, Duration, Rating
- Results count and search time
- Save search button (authenticated users)

**Enhanced Flight Cards**:
- All basic card elements
- Hover effects with quick details
- "Save Flight" heart icon (authenticated)
- "Set Alert" bell icon
- Quick booking button

#### Map View Toggle
**Component**: `components/maps/SearchMap.tsx`

**Features**:
- Same map as homepage but filtered to search results
- Flight markers with popup details
- Route visualization
- Cluster markers for multiple flights
- Direct booking from map popups

### 3. Flight Details (`/flights/[id]`)
**Route**: `/flights/[flightId]`
**Component**: `app/(public)/flights/[id]/page.tsx`

#### Layout Structure:
```typescript
interface FlightDetailsLayout {
  breadcrumb: Breadcrumb;
  flightHeader: FlightHeader;
  imageGallery: AircraftGallery;
  flightInfo: FlightInformation;
  bookingCard: BookingCard;
  companyInfo: CompanyProfile;
  similarFlights: SimilarFlights;
}
```

#### Flight Header
**Component**: `components/flights/FlightHeader.tsx`

**Elements**:
- Route display: "New York (JFK) → Miami (MIA)"
- Date and time with timezone
- Aircraft type and registration
- Company name with logo and rating
- Share buttons (social media, email, copy link)
- Back to search results link

#### Aircraft Gallery
**Component**: `components/flights/AircraftGallery.tsx`

**Features**:
- Main image with thumbnail strip
- Interior and exterior photos
- Zoom functionality
- Fullscreen lightbox
- Image captions with details

#### Flight Information Tabs
**Component**: `components/flights/FlightInfo.tsx`

**Tab Structure**:
1. **Overview**: Basic flight details, aircraft specs, seating configuration
2. **Amenities**: WiFi, catering, bar, entertainment, etc.
3. **Company**: Operator information, safety ratings, fleet details
4. **Terms**: Booking conditions, cancellation policy, requirements

#### Booking Card (Sticky Sidebar)
**Component**: `components/flights/BookingCard.tsx`

```typescript
interface BookingCardProps {
  flight: Flight;
  onBookingClick: () => void;
  isAuthenticated: boolean;
}
```

**Elements**:
- Price display with savings badge
- Seats available counter
- Passenger selector
- "Book Now" primary button
- "Save Flight" secondary button
- "Set Price Alert" link
- Price breakdown on hover
- Login prompt for guests

### 4. About Page (`/about`)
**Route**: `/about`
**Component**: `app/(public)/about/page.tsx`

**Sections**:
- Company mission and vision
- Team profiles with photos
- Platform statistics and achievements
- Trust and safety information
- Press mentions and awards

### 5. How It Works (`/how-it-works`)
**Route**: `/how-it-works`
**Component**: `app/(public)/how-it-works/page.tsx`

**Sections**:
- Detailed explanation of empty legs concept
- Step-by-step booking process
- Safety and verification procedures
- FAQ section
- Video explanations

## Authentication Flow

### 1. Login Page (`/login`)
**Route**: `/login`
**Component**: `app/(auth)/login/page.tsx`

#### Layout Structure:
```typescript
interface LoginPageLayout {
  leftPanel: LoginForm;
  rightPanel: AuthPromoBanner;
}
```

#### Login Form
**Component**: `components/auth/LoginForm.tsx`

**Form Fields**:
- Email input with validation
- Password input with show/hide toggle
- "Remember me" checkbox
- "Forgot password?" link
- Login button (primary CTA)
- Social login options (Google, Apple)
- Register link: "Don't have an account? Sign up"

**Features**:
- Real-time validation with error messages
- Loading states during submission
- Redirect to intended page after login
- Error handling for invalid credentials

#### Promo Banner (Right Side)
**Visual Elements**:
- Background image of luxury aircraft
- Benefits list with checkmarks
- Customer testimonial quote
- "Join thousands of satisfied travelers" message

### 2. Register Page (`/register`)
**Route**: `/register`
**Component**: `app/(auth)/register/page.tsx`

#### Registration Type Selection
**Component**: `components/auth/RegistrationTypeSelector.tsx`

**Options**:
1. **Customer Account**: For booking flights
2. **Company Account**: For listing flights

#### Customer Registration Form
**Component**: `components/auth/CustomerRegistrationForm.tsx`

**Form Steps**:
1. **Basic Info**: Name, email, password, phone
2. **Preferences**: Travel preferences, communication settings
3. **Verification**: Email verification code

#### Company Registration Form
**Component**: `components/auth/CompanyRegistrationForm.tsx`

**Form Steps**:
1. **Company Info**: Name, type, contact details
2. **Documentation**: Certificates, insurance, licenses
3. **Fleet Details**: Aircraft information, capacity
4. **Verification**: Email and document verification

### 3. Forgot Password (`/forgot-password`)
**Route**: `/forgot-password`
**Component**: `app/(auth)/forgot-password/page.tsx`

**Process**:
1. Email input form
2. Confirmation message
3. Reset link sent notification
4. Password reset form (separate page)

## Customer Dashboard

### 1. Dashboard Overview (`/dashboard`)
**Route**: `/dashboard`
**Component**: `app/dashboard/page.tsx`

#### Layout Structure:
```typescript
interface CustomerDashboardLayout {
  welcomeHeader: WelcomeHeader;
  quickSearch: QuickSearchWidget;
  upcomingFlights: UpcomingFlightsWidget;
  savedFlights: SavedFlightsWidget;
  alertsWidget: AlertsWidget;
  recentActivity: ActivityWidget;
}
```

#### Welcome Header
**Component**: `components/dashboard/WelcomeHeader.tsx`

**Elements**:
- Personalized greeting: "Welcome back, [Name]"
- Quick stats: Flights booked, Money saved, Miles flown
- Profile completion progress bar
- Loyalty program status badge

#### Quick Search Widget
**Component**: `components/dashboard/QuickSearchWidget.tsx`

**Features**:
- Compact search form
- Recent searches dropdown
- Popular routes suggestions
- "Advanced Search" link

#### Upcoming Flights Widget
**Component**: `components/dashboard/UpcomingFlightsWidget.tsx`

**Features**:
- Next flight card with countdown
- Flight details and booking reference
- Check-in reminders
- "View All Bookings" link
- Quick actions: Modify, Cancel, Contact operator

#### Saved Flights Widget
**Component**: `components/dashboard/SavedFlightsWidget.tsx`

**Features**:
- Recently saved flights grid
- Price change notifications
- Remove from saved option
- "View All Saved" link

### 2. My Bookings (`/dashboard/bookings`)
**Route**: `/dashboard/bookings`
**Component**: `app/dashboard/bookings/page.tsx`

#### Booking Filters and Tabs
**Component**: `components/dashboard/BookingFilters.tsx`

**Tabs**:
- All Bookings
- Upcoming (confirmed)
- Pending (awaiting approval)
- Past
- Cancelled

**Filters**:
- Date range picker
- Status filter
- Price range
- Aircraft type

#### Booking List Item
**Component**: `components/dashboard/BookingListItem.tsx`

```typescript
interface BookingItemProps {
  booking: {
    id: string;
    flightNumber: string;
    route: string;
    date: string;
    status: BookingStatus;
    price: number;
    passengers: number;
    aircraft: string;
    company: string;
  };
}
```

**Elements**:
- Booking reference number
- Flight route and date
- Status badge with color coding
- Price and passenger count
- Aircraft type and company
- Action buttons: View Details, Modify, Cancel, Contact
- Download invoice button (for completed)

#### Booking Details Modal
**Component**: `components/dashboard/BookingDetailsModal.tsx`

**Tabs**:
1. **Flight Info**: Complete flight details
2. **Passengers**: Passenger list and documents
3. **Payment**: Invoice, payment method, receipts
4. **Communication**: Message history with operator

### 3. Saved Flights (`/dashboard/saved`)
**Route**: `/dashboard/saved`
**Component**: `app/dashboard/saved/page.tsx`

#### Features:
- Grid view of saved flights
- Price change alerts for each flight
- Bulk actions: Remove multiple, Set alerts
- Filter by route, date, price changes
- Quick booking from saved flights

### 4. Flight Alerts (`/dashboard/alerts`)
**Route**: `/dashboard/alerts`
**Component**: `app/dashboard/alerts/page.tsx`

#### Alert Creation Form
**Component**: `components/dashboard/AlertForm.tsx`

**Form Fields**:
- Departure/arrival airports
- Date range (flexible)
- Passenger count
- Maximum price
- Aircraft preferences
- Notification preferences

#### Active Alerts List
**Component**: `components/dashboard/AlertsList.tsx`

**Alert Item Elements**:
- Route and criteria summary
- Creation date and last triggered
- Match count and success rate
- Edit/delete actions
- Pause/resume toggle

### 5. Profile Settings (`/dashboard/profile`)
**Route**: `/dashboard/profile`
**Component**: `app/dashboard/profile/page.tsx`

#### Profile Tabs:
1. **Personal Info**: Name, contact, preferences
2. **Documents**: ID, passport, travel docs
3. **Payment Methods**: Saved cards, billing info
4. **Loyalty Program**: Points, tier, rewards, history
5. **Notifications**: Email, SMS, push preferences
6. **Privacy**: Data usage, visibility settings

### 6. Loyalty Program (`/dashboard/loyalty`)
**Route**: `/dashboard/loyalty`
**Component**: `app/dashboard/loyalty/page.tsx`

#### Loyalty Dashboard
**Component**: `components/loyalty/LoyaltyDashboard.tsx`

```typescript
interface LoyaltyDashboardProps {
  user: User;
  loyaltyData: {
    pointsBalance: number;
    currentTier: LoyaltyTier;
    nextTier: LoyaltyTier | null;
    pointsToNextTier: number;
    yearlyProgress: number;
    benefits: LoyaltyBenefit[];
    recentTransactions: PointsTransaction[];
    availableRewards: Reward[];
  };
}
```

**Layout Structure**:
- **Header Card**: Current points, tier badge, progress to next tier
- **Benefits Overview**: Current tier benefits with checkmarks
- **Points Activity**: Recent earning/spending history
- **Available Rewards**: Redeemable rewards grid
- **Tier Progress**: Visual progress bar to next tier

#### Loyalty Components with shadcn/ui:
```typescript
// Points Balance Card
<Card className="bg-gradient-to-r from-gold-50 to-sky-50">
  <CardHeader>
    <div className="flex items-center justify-between">
      <CardTitle>Loyalty Points</CardTitle>
      <Badge variant="outline" className="bg-gold-100">
        {currentTier.name}
      </Badge>
    </div>
  </CardHeader>
  <CardContent>
    <div className="text-3xl font-bold text-gold-600">
      {pointsBalance.toLocaleString()}
    </div>
    <Progress value={tierProgress} className="mt-4" />
    <p className="text-sm text-muted-foreground mt-2">
      {pointsToNextTier} points to {nextTier?.name}
    </p>
  </CardContent>
</Card>

// Rewards Grid
<div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
  {availableRewards.map((reward) => (
    <Card key={reward.id} className="hover:shadow-md transition-shadow">
      <CardContent className="p-4">
        <div className="flex items-center justify-between mb-2">
          <h3 className="font-semibold">{reward.title}</h3>
          <Badge>{reward.pointsCost} pts</Badge>
        </div>
        <p className="text-sm text-muted-foreground mb-4">
          {reward.description}
        </p>
        <Button 
          size="sm" 
          disabled={pointsBalance < reward.pointsCost}
          className="w-full"
        >
          Redeem
        </Button>
      </CardContent>
    </Card>
  ))}
</div>
```

### 7. Group Booking Flow (`/booking/group`)
**Route**: `/booking/group/[flightId]`
**Component**: `app/booking/group/[flightId]/page.tsx`

#### Group Booking Wizard
**Component**: `components/booking/GroupBookingWizard.tsx`

**Steps**:
1. **Group Details**: Number of passengers, group leader
2. **Passenger Information**: Individual passenger forms
3. **Payment Split**: How to divide payment
4. **Documents**: Upload for all passengers
5. **Confirmation**: Review and submit

```typescript
interface GroupBookingData {
  flight: Flight;
  groupLeader: User;
  passengers: Passenger[];
  paymentSplit: {
    type: 'equal' | 'custom' | 'single';
    payments: PaymentSplit[];
  };
  specialRequests: string;
  documents: GroupDocument[];
}
```

#### Group Booking Components:
```typescript
// Passenger Management
<Card>
  <CardHeader>
    <CardTitle>Passengers ({passengers.length})</CardTitle>
    <Button
      onClick={addPassenger}
      variant="outline"
      size="sm"
      className="ml-auto"
    >
      <Plus className="h-4 w-4 mr-2" />
      Add Passenger
    </Button>
  </CardHeader>
  <CardContent>
    {passengers.map((passenger, index) => (
      <div key={index} className="border rounded-lg p-4 mb-4">
        <div className="flex items-center justify-between mb-4">
          <h4 className="font-semibold">Passenger {index + 1}</h4>
          {index > 0 && (
            <Button
              variant="ghost"
              size="sm"
              onClick={() => removePassenger(index)}
            >
              <Trash2 className="h-4 w-4" />
            </Button>
          )}
        </div>
        <div className="grid gap-4 md:grid-cols-2">
          <div>
            <Label htmlFor={`firstName-${index}`}>First Name</Label>
            <Input
              id={`firstName-${index}`}
              value={passenger.firstName}
              onChange={(e) => updatePassenger(index, 'firstName', e.target.value)}
            />
          </div>
          <div>
            <Label htmlFor={`lastName-${index}`}>Last Name</Label>
            <Input
              id={`lastName-${index}`}
              value={passenger.lastName}
              onChange={(e) => updatePassenger(index, 'lastName', e.target.value)}
            />
          </div>
        </div>
      </div>
    ))}
  </CardContent>
</Card>

// Payment Split Configuration
<Card>
  <CardHeader>
    <CardTitle>Payment Split</CardTitle>
  </CardHeader>
  <CardContent>
    <div className="space-y-4">
      <div className="grid gap-4">
        <Label>How would you like to split the payment?</Label>
        <div className="space-y-2">
          <div className="flex items-center space-x-2">
            <input
              type="radio"
              id="equal"
              name="paymentType"
              value="equal"
              checked={paymentSplit.type === 'equal'}
              onChange={(e) => setPaymentSplit({...paymentSplit, type: 'equal'})}
            />
            <Label htmlFor="equal">Split equally</Label>
          </div>
          <div className="flex items-center space-x-2">
            <input
              type="radio"
              id="custom"
              name="paymentType"
              value="custom"
              checked={paymentSplit.type === 'custom'}
              onChange={(e) => setPaymentSplit({...paymentSplit, type: 'custom'})}
            />
            <Label htmlFor="custom">Custom split</Label>
          </div>
          <div className="flex items-center space-x-2">
            <input
              type="radio"
              id="single"
              name="paymentType"
              value="single"
              checked={paymentSplit.type === 'single'}
              onChange={(e) => setPaymentSplit({...paymentSplit, type: 'single'})}
            />
            <Label htmlFor="single">Single payment</Label>
          </div>
        </div>
      </div>
    </div>
  </CardContent>
</Card>
```

## Company Dashboard

### 1. Company Dashboard (`/company/dashboard`)
**Route**: `/company/dashboard`
**Component**: `app/company/page.tsx`

#### Layout Structure:
```typescript
interface CompanyDashboardLayout {
  metricsOverview: MetricsOverview;
  quickActions: QuickActions;
  recentBookings: RecentBookings;
  flightCalendar: FlightCalendar;
  performanceCharts: PerformanceCharts;
}
```

#### Metrics Overview
**Component**: `components/company/MetricsOverview.tsx`

**Key Metrics Cards**:
- Total Revenue (this month)
- Occupancy Rate (percentage)
- Active Listings count
- Pending Reservations count
- Customer Rating average
- Revenue growth trend

#### Quick Actions Panel
**Component**: `components/company/QuickActions.tsx`

**Action Buttons**:
- "Add New Flight" (primary CTA)
- "View Reservations"
- "Update Aircraft Info"
- "Download Reports"
- "Message Center"

#### Recent Bookings Widget
**Component**: `components/company/RecentBookings.tsx`

**Features**:
- Last 5 bookings with status
- Quick approve/decline actions
- Customer contact information
- Booking value and profit margin
- "View All Bookings" link

#### Flight Calendar
**Component**: `components/company/FlightCalendar.tsx`

**Features**:
- Monthly calendar view
- Flight markers on dates
- Color coding by status (available, booked, past)
- Click to view/edit flight details
- Drag and drop for rescheduling
- Sync with external calendars

### 2. Flight Management (`/company/flights`)
**Route**: `/company/flights`
**Component**: `app/company/flights/page.tsx`

#### Flight List View
**Component**: `components/company/FlightList.tsx`

**Toolbar**:
- "Add New Flight" button
- View toggle: List/Calendar/Map
- Status filter: All, Available, Booked, Past
- Date range filter
- Aircraft filter

#### Flight List Item
**Component**: `components/company/FlightListItem.tsx`

**Elements**:
- Flight route and date/time
- Aircraft type and registration
- Price and pricing rules
- Booking status and seat availability
- Performance metrics (views, inquiries)
- Actions: Edit, Clone, Delete, View Analytics

#### Add/Edit Flight Form
**Component**: `components/company/FlightForm.tsx`

**Form Sections**:
1. **Flight Details**: Route, date, time, aircraft
2. **Pricing**: Base price, dynamic pricing rules
3. **Amenities**: Services, equipment, catering
4. **Photos**: Aircraft interior/exterior images
5. **Booking Rules**: Advance notice, cancellation policy

### 3. Reservations Management (`/company/bookings`)
**Route**: `/company/bookings`
**Component**: `app/company/bookings/page.tsx`

#### Reservation Status Tabs:
- Pending Approval (priority)
- Confirmed
- In Progress (day of flight)
- Completed
- Cancelled/Refunded

#### Reservation Details Panel
**Component**: `components/company/ReservationDetails.tsx`

**Information Sections**:
- Customer information and contact
- Passenger list with documents
- Payment status and amount
- Flight details and special requests
- Communication history

**Action Buttons**:
- Approve/Decline (for pending)
- Send Message
- Modify Booking
- Process Refund
- Download Documents

### 4. Analytics (`/company/analytics`)
**Route**: `/company/analytics`
**Component**: `app/company/analytics/page.tsx`

#### Analytics Dashboard
**Component**: `components/company/AnalyticsDashboard.tsx`

**Chart Types**:
- Revenue trend line chart
- Occupancy rate by month
- Popular routes bar chart
- Customer demographics pie chart
- Booking lead time histogram
- Price optimization recommendations

#### Report Generation
**Component**: `components/company/ReportGenerator.tsx`

**Report Options**:
- Financial summary (monthly/quarterly)
- Flight performance report
- Customer satisfaction analysis
- Market comparison study
- Export formats: PDF, Excel, CSV

### 5. Dynamic Pricing Management (`/company/pricing`)
**Route**: `/company/pricing`
**Component**: `app/company/pricing/page.tsx`

#### Dynamic Pricing Dashboard
**Component**: `components/pricing/DynamicPricingDashboard.tsx`

```typescript
interface PricingRuleProps {
  rule: {
    id: string;
    name: string;
    flightId?: string;
    route?: string;
    timeBasedDiscounts: {
      hoursBeforeDeparture: number;
      discountPercentage: number;
    }[];
    demandMultipliers: {
      occupancyThreshold: number;
      priceMultiplier: number;
    }[];
    isActive: boolean;
    performance: {
      bookingsGenerated: number;
      revenueImpact: number;
      averageDiscount: number;
    };
  };
}
```

**Layout Structure**:
- **Pricing Rules Overview**: Active rules with performance metrics
- **Rule Creation/Editing**: Dynamic pricing configuration
- **Performance Analytics**: Charts showing pricing effectiveness
- **Manual Overrides**: Quick price adjustments
- **Historical Data**: Pricing decision history

#### Pricing Components with shadcn/ui:
```typescript
// Pricing Rule Card
<Card>
  <CardHeader>
    <div className="flex items-center justify-between">
      <CardTitle>{rule.name}</CardTitle>
      <div className="flex items-center space-x-2">
        <Badge variant={rule.isActive ? "default" : "secondary"}>
          {rule.isActive ? "Active" : "Inactive"}
        </Badge>
        <Button variant="ghost" size="sm">
          <Settings className="h-4 w-4" />
        </Button>
      </div>
    </div>
  </CardHeader>
  <CardContent>
    <div className="space-y-4">
      <div className="grid gap-4 md:grid-cols-3">
        <div>
          <p className="text-sm font-medium">Bookings Generated</p>
          <p className="text-2xl font-bold text-green-600">
            {rule.performance.bookingsGenerated}
          </p>
        </div>
        <div>
          <p className="text-sm font-medium">Revenue Impact</p>
          <p className="text-2xl font-bold text-blue-600">
            ${rule.performance.revenueImpact.toLocaleString()}
          </p>
        </div>
        <div>
          <p className="text-sm font-medium">Avg. Discount</p>
          <p className="text-2xl font-bold text-orange-600">
            {rule.performance.averageDiscount}%
          </p>
        </div>
      </div>
      
      <Separator />
      
      <div>
        <p className="text-sm font-medium mb-2">Time-based Discounts</p>
        <div className="space-y-1">
          {rule.timeBasedDiscounts.map((discount, index) => (
            <div key={index} className="flex justify-between text-sm">
              <span>{discount.hoursBeforeDeparture}h before departure</span>
              <Badge variant="outline">{discount.discountPercentage}% off</Badge>
            </div>
          ))}
        </div>
      </div>
    </div>
  </CardContent>
</Card>

// Price Override Interface
<Card>
  <CardHeader>
    <CardTitle>Manual Price Override</CardTitle>
  </CardHeader>
  <CardContent>
    <div className="space-y-4">
      <div className="grid gap-4 md:grid-cols-2">
        <div>
          <Label htmlFor="currentPrice">Current Price</Label>
          <Input
            id="currentPrice"
            value={`$${currentPrice.toLocaleString()}`}
            disabled
          />
        </div>
        <div>
          <Label htmlFor="newPrice">New Price</Label>
          <Input
            id="newPrice"
            type="number"
            value={overridePrice}
            onChange={(e) => setOverridePrice(Number(e.target.value))}
          />
        </div>
      </div>
      <div>
        <Label htmlFor="reason">Reason for Override</Label>
        <Textarea
          id="reason"
          placeholder="Explain why you're overriding the automatic pricing..."
          value={overrideReason}
          onChange={(e) => setOverrideReason(e.target.value)}
        />
      </div>
      <div className="flex items-center space-x-2">
        <input
          type="checkbox"
          id="temporary"
          checked={isTemporary}
          onChange={(e) => setIsTemporary(e.target.checked)}
        />
        <Label htmlFor="temporary">Temporary override (reverts after 24h)</Label>
      </div>
      <Button className="w-full">Apply Price Override</Button>
    </div>
  </CardContent>
</Card>
```

### 6. Company Profile (`/company/profile`)
**Route**: `/company/profile`
**Component**: `app/company/profile/page.tsx`

#### Profile Sections:
1. **Company Information**: Basic details, certifications
2. **Fleet Management**: Aircraft list, specifications
3. **Operational Details**: Bases, service areas
4. **Marketing**: Company description, photos, videos
5. **Settings**: Notification preferences, API access

## Admin Dashboard

### 1. Admin Overview (`/admin/dashboard`)
**Route**: `/admin/dashboard`
**Component**: `app/admin/page.tsx`

#### System Metrics
**Component**: `components/admin/SystemMetrics.tsx`

**Key Metrics**:
- Total platform revenue
- Active users (customers/companies)
- Total bookings and conversion rates
- Platform health indicators
- Support ticket status

#### Recent Activity Feed
**Component**: `components/admin/ActivityFeed.tsx`

**Activity Types**:
- New user registrations
- High-value bookings
- Company applications
- System alerts
- Dispute reports

### 2. User Management (`/admin/users`)
**Route**: `/admin/users`
**Component**: `app/admin/users/page.tsx`

#### User List with Filters
**Component**: `components/admin/UserManagement.tsx`

**User Types Tabs**:
- All Users
- Customers
- Companies
- Pending Approval
- Suspended

**User Actions**:
- View profile details
- Approve/reject applications
- Suspend/reactivate accounts
- Reset passwords
- Send notifications

### 3. Platform Analytics (`/admin/analytics`)
**Route**: `/admin/analytics`
**Component**: `app/admin/analytics/page.tsx`

#### Comprehensive Analytics
**Component**: `components/admin/PlatformAnalytics.tsx`

**Dashboard Sections**:
- Revenue and commission tracking
- User engagement metrics
- Popular routes and trends
- Company performance comparison
- Geographic usage patterns
- Device and browser analytics

### 4. Dispute Management (`/admin/disputes`)
**Route**: `/admin/disputes`
**Component**: `app/admin/disputes/page.tsx`

#### Dispute Queue
**Component**: `components/admin/DisputeQueue.tsx`

**Dispute Categories**:
- Payment issues
- Flight cancellations
- Service complaints
- Documentation problems
- Refund requests

**Resolution Tools**:
- Communication interface
- Refund processing
- Account actions
- Legal escalation
- Case documentation

### 5. Weather & Disruption Management (`/admin/disruptions`)
**Route**: `/admin/disruptions`
**Component**: `app/admin/disruptions/page.tsx`

#### Disruption Management Center
**Component**: `components/disruption/DisruptionCenter.tsx`

```typescript
interface DisruptionData {
  id: string;
  type: 'weather' | 'mechanical' | 'atc' | 'airport' | 'other';
  severity: 'low' | 'medium' | 'high' | 'critical';
  affectedFlights: Flight[];
  affectedRegions: string[];
  startTime: Date;
  estimatedDuration: number;
  status: 'active' | 'monitoring' | 'resolved';
  alternativeOptions: RebookingOption[];
  communicationSent: boolean;
}
```

#### Disruption Components with shadcn/ui:
```typescript
// Active Disruptions Overview
<Card className="border-l-4 border-l-red-500">
  <CardHeader>
    <div className="flex items-center justify-between">
      <CardTitle className="flex items-center">
        <AlertTriangle className="h-5 w-5 mr-2 text-red-500" />
        Weather Alert - Eastern Seaboard
      </CardTitle>
      <Badge variant="destructive">Critical</Badge>
    </div>
  </CardHeader>
  <CardContent>
    <div className="space-y-4">
      <div className="grid gap-4 md:grid-cols-3">
        <div>
          <p className="text-sm font-medium">Affected Flights</p>
          <p className="text-2xl font-bold">{affectedFlights.length}</p>
        </div>
        <div>
          <p className="text-sm font-medium">Affected Passengers</p>
          <p className="text-2xl font-bold">{totalPassengers}</p>
        </div>
        <div>
          <p className="text-sm font-medium">Estimated Duration</p>
          <p className="text-2xl font-bold">{estimatedDuration}h</p>
        </div>
      </div>
      
      <div className="flex space-x-2">
        <Button size="sm">
          <Send className="h-4 w-4 mr-2" />
          Send Notifications
        </Button>
        <Button variant="outline" size="sm">
          <RefreshCw className="h-4 w-4 mr-2" />
          Find Alternatives
        </Button>
        <Button variant="outline" size="sm">
          <FileText className="h-4 w-4 mr-2" />
          Generate Report
        </Button>
      </div>
    </div>
  </CardContent>
</Card>

// Alternative Flight Suggestions
<Card>
  <CardHeader>
    <CardTitle>Alternative Options</CardTitle>
  </CardHeader>
  <CardContent>
    <div className="space-y-4">
      {alternatives.map((alternative) => (
        <div key={alternative.id} className="border rounded-lg p-4">
          <div className="flex items-center justify-between mb-2">
            <div>
              <h4 className="font-semibold">{alternative.route}</h4>
              <p className="text-sm text-muted-foreground">
                {alternative.newDate} at {alternative.newTime}
              </p>
            </div>
            <div className="text-right">
              <p className="font-semibold">${alternative.price}</p>
              <Badge variant="outline">
                {alternative.seatsAvailable} seats
              </Badge>
            </div>
          </div>
          <div className="flex justify-between items-center">
            <p className="text-sm text-muted-foreground">
              {alternative.aircraft} • {alternative.company}
            </p>
            <Button size="sm" variant="outline">
              Offer to Customers
            </Button>
          </div>
        </div>
      ))}
    </div>
  </CardContent>
</Card>

// Automated Rebooking Interface
<Card>
  <CardHeader>
    <CardTitle>Automated Rebooking</CardTitle>
  </CardHeader>
  <CardContent>
    <div className="space-y-4">
      <div className="flex items-center space-x-2">
        <input
          type="checkbox"
          id="autoRebook"
          checked={autoRebookEnabled}
          onChange={(e) => setAutoRebookEnabled(e.target.checked)}
        />
        <Label htmlFor="autoRebook">
          Enable automatic rebooking for affected passengers
        </Label>
      </div>
      
      <div className="grid gap-4 md:grid-cols-2">
        <div>
          <Label htmlFor="maxPriceIncrease">Max Price Increase (%)</Label>
          <Slider
            id="maxPriceIncrease"
            min={0}
            max={50}
            step={5}
            value={[maxPriceIncrease]}
            onValueChange={(value) => setMaxPriceIncrease(value[0])}
          />
          <p className="text-sm text-muted-foreground mt-1">
            Current: {maxPriceIncrease}%
          </p>
        </div>
        <div>
          <Label htmlFor="maxDelayHours">Max Delay (hours)</Label>
          <Slider
            id="maxDelayHours"
            min={1}
            max={48}
            step={1}
            value={[maxDelayHours]}
            onValueChange={(value) => setMaxDelayHours(value[0])}
          />
          <p className="text-sm text-muted-foreground mt-1">
            Current: {maxDelayHours}h
          </p>
        </div>
      </div>
      
      <Alert>
        <Info className="h-4 w-4" />
        <AlertTitle>Rebooking Policy</AlertTitle>
        <AlertDescription>
          Passengers will be automatically offered alternatives within the specified 
          criteria. They can accept, decline, or request a full refund.
        </AlertDescription>
      </Alert>
      
      <Button className="w-full">
        Start Automated Rebooking Process
      </Button>
    </div>
  </CardContent>
</Card>
```

## Mobile Responsiveness

### Breakpoint Strategy
```css
/* Tailwind CSS Breakpoints */
sm: 640px   /* Small tablets */
md: 768px   /* Tablets */
lg: 1024px  /* Small laptops */
xl: 1280px  /* Large laptops */
2xl: 1536px /* Large screens */
```

### Mobile-First Components

#### Mobile Navigation
**Component**: `components/mobile/MobileNav.tsx`

**Features**:
- Hamburger menu with slide-out drawer
- Touch-friendly button sizes (44px minimum)
- Swipe gestures for navigation
- Bottom tab bar for main sections

#### Mobile Search
**Component**: `components/mobile/MobileSearch.tsx`

**Adaptations**:
- Full-screen search overlay
- Touch-optimized date pickers
- Simplified filter interface
- Voice search integration

#### Mobile Flight Cards
**Component**: `components/mobile/MobileFlightCard.tsx`

**Design Changes**:
- Stacked layout instead of grid
- Larger touch targets
- Swipe actions for save/book
- Expandable details sections

## Performance Requirements

### Loading Performance
- Initial page load: < 3 seconds
- Search results: < 1 second
- Image optimization with next/image
- Progressive loading for lists
- Skeleton loading states

### SEO Optimization
- Server-side rendering with Next.js
- Meta tags for social sharing
- Structured data for flights
- Sitemap generation
- Robot.txt optimization

### Accessibility
- WCAG 2.1 AA compliance
- Keyboard navigation support
- Screen reader compatibility
- High contrast mode
- Focus management

### Browser Support
- Chrome (last 2 versions)
- Firefox (last 2 versions)
- Safari (last 2 versions)
- Edge (last 2 versions)
- iOS Safari (last 2 versions)
- Chrome Mobile (last 2 versions)

## State Management

### Zustand Stores

#### Auth Store
```typescript
interface AuthStore {
  user: User | null;
  isAuthenticated: boolean;
  login: (credentials: LoginCredentials) => Promise<void>;
  logout: () => void;
  refreshToken: () => Promise<void>;
}
```

#### Search Store
```typescript
interface SearchStore {
  searchCriteria: SearchCriteria;
  searchResults: Flight[];
  isLoading: boolean;
  updateCriteria: (criteria: Partial<SearchCriteria>) => void;
  performSearch: () => Promise<void>;
  clearResults: () => void;
}
```

#### Booking Store
```typescript
interface BookingStore {
  currentBooking: BookingDraft | null;
  bookingStep: BookingStep;
  passengerData: Passenger[];
  paymentMethod: PaymentMethod | null;
  updateBooking: (data: Partial<BookingDraft>) => void;
  submitBooking: () => Promise<BookingResult>;
}
```

### React Query Configuration

#### Query Keys
```typescript
export const queryKeys = {
  flights: (params: SearchParams) => ['flights', params],
  flight: (id: string) => ['flight', id],
  bookings: (userId: string) => ['bookings', userId],
  alerts: (userId: string) => ['alerts', userId],
  company: (id: string) => ['company', id],
} as const;
```

#### Mutation Patterns
```typescript
// Booking mutation with optimistic updates
const bookingMutation = useMutation({
  mutationFn: createBooking,
  onMutate: async (newBooking) => {
    // Optimistic update
    await queryClient.cancelQueries({ queryKey: ['bookings'] });
    const previousBookings = queryClient.getQueryData(['bookings']);
    queryClient.setQueryData(['bookings'], (old) => [...old, newBooking]);
    return { previousBookings };
  },
  onError: (err, newBooking, context) => {
    // Rollback on error
    queryClient.setQueryData(['bookings'], context.previousBookings);
  },
  onSettled: () => {
    // Refetch to ensure consistency
    queryClient.invalidateQueries({ queryKey: ['bookings'] });
  },
});
```

## Error Handling

### Error Boundary Components
```typescript
// Global error boundary for unhandled errors
const GlobalErrorBoundary: React.FC = ({ children }) => {
  return (
    <ErrorBoundary
      FallbackComponent={ErrorFallback}
      onError={(error, errorInfo) => {
        // Log to monitoring service
        console.error('Global error:', error, errorInfo);
      }}
    >
      {children}
    </ErrorBoundary>
  );
};
```

### API Error Handling
```typescript
// Centralized error handling for API calls
const handleApiError = (error: ApiError) => {
  switch (error.status) {
    case 401:
      // Redirect to login
      router.push('/login');
      break;
    case 403:
      // Show permission denied message
      toast.error('Access denied');
      break;
    case 500:
      // Show generic error message
      toast.error('Something went wrong. Please try again.');
      break;
    default:
      toast.error(error.message || 'An error occurred');
  }
};
```

## Testing Strategy

### Component Testing
- Unit tests for all utility functions
- React Testing Library for component tests
- Mock external dependencies (APIs, maps)
- Accessibility testing with axe-core

### Integration Testing
- User flow testing with Playwright
- API integration testing
- Payment flow testing (with test mode)
- Cross-browser testing

### Performance Testing
- Lighthouse CI for performance metrics
- Bundle size monitoring
- Core Web Vitals tracking
- Load testing for search functionality

## Advanced Features Implementation

### Booking Modification Flow
**Component**: `components/booking/BookingModification.tsx`

```typescript
interface BookingModificationProps {
  booking: Booking;
  availableAlternatives: Flight[];
  onModificationComplete: (newBooking: Booking) => void;
}
```

**Modification Types**:
- **Date Change**: Select new departure date with price difference
- **Route Change**: Modify departure/arrival airports
- **Passenger Changes**: Add/remove passengers
- **Upgrade Options**: Better aircraft or additional services

**shadcn/ui Implementation**:
```typescript
<Dialog open={isModifying} onOpenChange={setIsModifying}>
  <DialogContent className="max-w-4xl">
    <DialogHeader>
      <DialogTitle>Modify Booking #{booking.reference}</DialogTitle>
    </DialogHeader>
    
    <Tabs defaultValue="date" className="w-full">
      <TabsList className="grid w-full grid-cols-4">
        <TabsTrigger value="date">Change Date</TabsTrigger>
        <TabsTrigger value="route">Change Route</TabsTrigger>
        <TabsTrigger value="passengers">Passengers</TabsTrigger>
        <TabsTrigger value="upgrade">Upgrades</TabsTrigger>
      </TabsList>
      
      <TabsContent value="date" className="space-y-4">
        <Card>
          <CardHeader>
            <CardTitle>Select New Date</CardTitle>
          </CardHeader>
          <CardContent>
            <Calendar
              mode="single"
              selected={newDate}
              onSelect={setNewDate}
              disabled={(date) => date < new Date()}
            />
            
            {priceDifference !== 0 && (
              <Alert className="mt-4">
                <Info className="h-4 w-4" />
                <AlertTitle>Price Difference</AlertTitle>
                <AlertDescription>
                  {priceDifference > 0 
                    ? `Additional payment of $${priceDifference} required`
                    : `Refund of $${Math.abs(priceDifference)} will be processed`
                  }
                </AlertDescription>
              </Alert>
            )}
          </CardContent>
        </Card>
      </TabsContent>
      
      <TabsContent value="passengers" className="space-y-4">
        <Card>
          <CardHeader>
            <CardTitle>Passenger Management</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {booking.passengers.map((passenger, index) => (
                <div key={index} className="flex items-center justify-between p-4 border rounded-lg">
                  <div>
                    <p className="font-semibold">{passenger.name}</p>
                    <p className="text-sm text-muted-foreground">{passenger.email}</p>
                  </div>
                  <Button variant="outline" size="sm">
                    <Trash2 className="h-4 w-4" />
                  </Button>
                </div>
              ))}
              
              <Button variant="outline" className="w-full">
                <Plus className="h-4 w-4 mr-2" />
                Add Passenger
              </Button>
            </div>
          </CardContent>
        </Card>
      </TabsContent>
    </Tabs>
    
    <DialogFooter>
      <Button variant="outline" onClick={() => setIsModifying(false)}>
        Cancel
      </Button>
      <Button onClick={handleModification}>
        Confirm Changes
      </Button>
    </DialogFooter>
  </DialogContent>
</Dialog>
```

### Real-time Notifications with SignalR
**Component**: `components/common/NotificationProvider.tsx`

```typescript
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

interface NotificationProviderProps {
  children: React.ReactNode;
  userId: string;
  userType: 'customer' | 'company' | 'admin';
}

const NotificationProvider: React.FC<NotificationProviderProps> = ({ 
  children, 
  userId, 
  userType 
}) => {
  const [connection, setConnection] = useState<HubConnection | null>(null);
  const { toast } = useToast();

  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
      .withUrl(`${process.env.NEXT_PUBLIC_API_URL}/notificationHub`)
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);
  }, []);

  useEffect(() => {
    if (connection) {
      connection.start()
        .then(() => {
          console.log('SignalR Connected');
          
          // Join user-specific group
          connection.invoke('JoinGroup', `${userType}_${userId}`);
          
          // Listen for different notification types
          connection.on('BookingStatusUpdate', (notification) => {
            toast({
              title: 'Booking Update',
              description: notification.message,
              variant: notification.type === 'confirmed' ? 'default' : 'destructive',
            });
          });
          
          connection.on('FlightAlert', (notification) => {
            toast({
              title: 'Flight Alert',
              description: notification.message,
              action: (
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => router.push(notification.actionUrl)}
                >
                  View Details
                </Button>
              ),
            });
          });
          
          connection.on('PriceAlert', (notification) => {
            toast({
              title: 'Price Alert',
              description: `Flight to ${notification.destination} price dropped to $${notification.newPrice}`,
              action: (
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => router.push(`/flights/${notification.flightId}`)}
                >
                  Book Now
                </Button>
              ),
            });
          });
        })
        .catch(err => console.error('SignalR Connection Error: ', err));
    }

    return () => {
      connection?.stop();
    };
  }, [connection, userId, userType]);

  return <>{children}</>;
};
```

### Enhanced Chart Components with Recharts
**Component**: `components/charts/RevenueChart.tsx`

```typescript
import { 
  LineChart, 
  Line, 
  XAxis, 
  YAxis, 
  CartesianGrid, 
  Tooltip, 
  ResponsiveContainer,
  BarChart,
  Bar,
  PieChart,
  Pie,
  Cell
} from 'recharts';

interface RevenueChartProps {
  data: RevenueData[];
  period: 'week' | 'month' | 'quarter' | 'year';
}

const RevenueChart: React.FC<RevenueChartProps> = ({ data, period }) => {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Revenue Trends</CardTitle>
        <div className="flex space-x-2">
          {['week', 'month', 'quarter', 'year'].map((p) => (
            <Button
              key={p}
              variant={period === p ? 'default' : 'outline'}
              size="sm"
              onClick={() => setPeriod(p as any)}
            >
              {p.charAt(0).toUpperCase() + p.slice(1)}
            </Button>
          ))}
        </div>
      </CardHeader>
      <CardContent>
        <ResponsiveContainer width="100%" height={300}>
          <LineChart data={data}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis dataKey="date" />
            <YAxis />
            <Tooltip 
              formatter={(value, name) => [`$${value.toLocaleString()}`, name]}
            />
            <Line 
              type="monotone" 
              dataKey="revenue" 
              stroke="hsl(var(--primary))" 
              strokeWidth={2}
            />
            <Line 
              type="monotone" 
              dataKey="bookings" 
              stroke="hsl(var(--secondary))" 
              strokeWidth={2}
            />
          </LineChart>
        </ResponsiveContainer>
      </CardContent>
    </Card>
  );
};

// Occupancy Rate Chart
const OccupancyChart: React.FC<{ data: OccupancyData[] }> = ({ data }) => {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Occupancy Rates by Route</CardTitle>
      </CardHeader>
      <CardContent>
        <ResponsiveContainer width="100%" height={300}>
          <BarChart data={data}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis dataKey="route" />
            <YAxis />
            <Tooltip formatter={(value) => [`${value}%`, 'Occupancy Rate']} />
            <Bar dataKey="occupancyRate" fill="hsl(var(--primary))" />
          </BarChart>
        </ResponsiveContainer>
      </CardContent>
    </Card>
  );
};
```

### Multi-language Support Implementation
**Component**: `components/common/LanguageProvider.tsx`

```typescript
import { createContext, useContext, useState } from 'react';

interface LanguageContextType {
  language: 'en' | 'fr';
  setLanguage: (lang: 'en' | 'fr') => void;
  t: (key: string) => string;
}

const translations = {
  en: {
    'search.departure': 'Departure',
    'search.arrival': 'Arrival',
    'search.date': 'Date',
    'search.passengers': 'Passengers',
    'booking.confirm': 'Confirm Booking',
    'flight.details': 'Flight Details',
    // ... more translations
  },
  fr: {
    'search.departure': 'Départ',
    'search.arrival': 'Arrivée',
    'search.date': 'Date',
    'search.passengers': 'Passagers',
    'booking.confirm': 'Confirmer la Réservation',
    'flight.details': 'Détails du Vol',
    // ... more translations
  }
};

const LanguageProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [language, setLanguage] = useState<'en' | 'fr'>('en');

  const t = (key: string): string => {
    return translations[language][key] || key;
  };

  return (
    <LanguageContext.Provider value={{ language, setLanguage, t }}>
      {children}
    </LanguageContext.Provider>
  );
};

// Language Selector Component
const LanguageSelector: React.FC = () => {
  const { language, setLanguage } = useLanguage();
  
  return (
    <Select value={language} onValueChange={setLanguage}>
      <SelectTrigger className="w-16">
        <SelectValue />
      </SelectTrigger>
      <SelectContent>
        <SelectItem value="en">🇺🇸 EN</SelectItem>
        <SelectItem value="fr">🇫🇷 FR</SelectItem>
      </SelectContent>
    </Select>
  );
};
```

## Final Component Libraries and Dependencies

### Package.json Dependencies
```json
{
  "dependencies": {
    "next": "14.0.0",
    "react": "18.2.0",
    "typescript": "5.0.0",
    "@radix-ui/react-*": "latest",
    "tailwindcss": "3.3.0",
    "lucide-react": "latest",
    "react-hook-form": "7.45.0",
    "zod": "3.21.0",
    "@hookform/resolvers": "3.1.0",
    "zustand": "4.4.0",
    "@tanstack/react-query": "4.29.0",
    "recharts": "2.7.0",
    "@microsoft/signalr": "7.0.0",
    "stripe": "12.0.0",
    "@stripe/stripe-js": "2.0.0",
    "react-map-gl": "7.0.0",
    "date-fns": "2.30.0",
    "class-variance-authority": "0.7.0",
    "clsx": "2.0.0",
    "tailwind-merge": "1.14.0"
  }
}
```

### Complete shadcn/ui Component Installation
```bash
# Base setup
npx shadcn-ui@latest init

# All required components
npx shadcn-ui@latest add button card input label select textarea
npx shadcn-ui@latest add dialog dropdown-menu sheet tabs
npx shadcn-ui@latest add calendar date-picker badge avatar
npx shadcn-ui@latest add table pagination toast alert
npx shadcn-ui@latest add form combobox slider progress
npx shadcn-ui@latest add separator navigation-menu
npx shadcn-ui@latest add accordion collapsible hover-card
npx shadcn-ui@latest add command popover scroll-area
npx shadcn-ui@latest add switch checkbox radio-group
```

This comprehensive frontend specification now includes all missing features identified in the diff analysis, complete shadcn/ui integration, and real-world implementation examples. The platform is ready for development with a modern, accessible, and scalable architecture. 