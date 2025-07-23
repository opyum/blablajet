# EmptyLegs Frontend

A modern React/Next.js application for the EmptyLegs private jet empty leg booking platform.

## 🚀 Getting Started

### Prerequisites

- Node.js 18+ 
- npm or yarn
- EmptyLegs backend API running on `http://localhost:5000`

### Installation

1. **Clone and navigate to the frontend directory:**
```bash
cd frontend
```

2. **Install dependencies:**
```bash
npm install
```

3. **Create environment variables:**
```bash
cp .env.local.example .env.local
```

Edit `.env.local` with your configuration:
```env
NEXT_PUBLIC_API_URL=http://localhost:5000/api/v1
NEXT_PUBLIC_STRIPE_PUBLISHABLE_KEY=pk_test_your_stripe_key_here
```

4. **Start the development server:**
```bash
npm run dev
```

The application will be available at `http://localhost:3000`

## 🏗️ Built With

- **Next.js 14** - React framework with App Router
- **TypeScript** - Type safety and better developer experience
- **Tailwind CSS** - Utility-first CSS framework
- **shadcn/ui** - Beautiful and accessible UI components
- **Zustand** - State management
- **TanStack Query** - Data fetching and caching
- **React Hook Form** - Form handling with validation
- **Zod** - Schema validation
- **Lucide React** - Beautiful icons

## 📁 Project Structure

```
frontend/
├── src/
│   ├── app/                    # Next.js App Router pages
│   │   ├── auth/              # Authentication pages
│   │   │   ├── login/         # Login page
│   │   │   └── register/      # Registration page
│   │   ├── dashboard/         # User dashboard
│   │   ├── flights/           # Flight search and details
│   │   │   └── search/        # Flight search results
│   │   ├── layout.tsx         # Root layout with providers
│   │   ├── page.tsx           # Homepage
│   │   └── globals.css        # Global styles and Tailwind
│   ├── components/            # Reusable components
│   │   ├── ui/               # shadcn/ui components
│   │   ├── forms/            # Form components
│   │   │   ├── search-flights-form.tsx
│   │   │   └── airport-autocomplete.tsx
│   │   └── layout/           # Layout components
│   │       └── header.tsx    # Navigation header
│   ├── lib/                  # Utility functions
│   │   ├── api.ts           # Axios configuration
│   │   └── utils.ts         # Common utilities
│   ├── providers/            # Context providers
│   │   ├── auth-provider.tsx
│   │   ├── query-provider.tsx
│   │   └── theme-provider.tsx
│   ├── services/             # API services
│   │   ├── auth.service.ts
│   │   ├── flights.service.ts
│   │   ├── bookings.service.ts
│   │   └── airports.service.ts
│   ├── store/                # Zustand stores
│   │   └── auth.store.ts
│   └── types/                # TypeScript definitions
│       └── index.ts
├── public/                   # Static assets
└── package.json
```

## 🎯 Key Features

### ✅ Authentication System
- User registration with role selection (Customer/Company)
- Secure login with JWT tokens
- Automatic token refresh
- Role-based navigation and route protection
- Password validation and security

### ✅ Flight Search
- Real-time airport autocomplete
- Advanced search filters (date, passengers, aircraft type, price)
- Responsive search results with sorting
- Flight details with pricing and amenities
- Professional flight cards design

### ✅ User Interface
- Modern, clean design with shadcn/ui components
- Fully responsive layout (mobile, tablet, desktop)
- Dark mode support
- Accessible components following WCAG guidelines
- Loading states and error handling
- Toast notifications

### ✅ State Management
- Global authentication state with Zustand
- Persistent user sessions across browser refreshes
- API data caching with TanStack Query
- Form state management with React Hook Form

## 🔧 Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run start` - Start production server
- `npm run lint` - Run ESLint
- `npm run type-check` - Run TypeScript type checking

## 🌍 API Integration

The frontend integrates with the EmptyLegs backend API:

### Authentication
- POST `/auth/login` - User login
- POST `/auth/register` - User registration
- POST `/auth/refresh` - Token refresh
- GET `/auth/me` - Get current user

### Flights
- GET `/flights/search` - Search flights with filters
- GET `/flights/{id}` - Get flight details

### Airports
- GET `/airports/search` - Airport autocomplete
- GET `/airports` - Get airports list

### Error Handling
- Automatic token refresh on 401 errors
- User-friendly error messages
- Network error handling with retry logic

## 🎨 UI Components

### Key Components

1. **SearchFlightsForm** - Main flight search interface
   - Airport autocomplete with real-time search
   - Date picker with validation
   - Passenger and filter selections

2. **AirportAutocomplete** - Smart airport selection
   - Real-time search with debouncing
   - Popular airports display
   - Formatted display (IATA - Name, City)

3. **Header** - Navigation with role-based menus
   - User authentication status
   - Role-specific navigation items
   - Mobile responsive menu

4. **FlightCard** - Flight result display
   - Route visualization
   - Price formatting with discounts
   - Aircraft and company information
   - Amenities display

### Design System

- **Colors**: Professional blue primary palette
- **Typography**: Inter font family
- **Spacing**: Consistent 4px grid system
- **Components**: shadcn/ui for accessibility and consistency

## 🔐 Authentication Flow

1. **Registration**: Users choose account type (Customer/Company) and provide details
2. **Login**: Secure authentication with credential validation
3. **Token Management**: JWT tokens with automatic refresh
4. **Route Protection**: Authenticated routes redirect to login if not authorized
5. **Role-based Access**: Different dashboards for different user roles

## 📱 Responsive Design

- **Mobile-first** approach with Tailwind CSS
- **Breakpoints**: sm (640px), md (768px), lg (1024px), xl (1280px)
- **Touch-friendly** interface for mobile devices
- **Adaptive layouts** that work on all screen sizes

## 🚀 Deployment

### Build for Production

```bash
npm run build
```

### Environment Variables

Ensure these environment variables are set for production:

```env
NEXT_PUBLIC_API_URL=https://api.emptylegs.com/api/v1
NEXT_PUBLIC_STRIPE_PUBLISHABLE_KEY=pk_live_your_stripe_key_here
```

### Deployment Platforms

The application can be deployed to:
- **Vercel** (recommended for Next.js)
- **Netlify**
- **AWS Amplify**
- **Google Cloud Platform**
- **Traditional VPS with Docker**

## 🔮 Future Features

- Flight booking flow with payment integration
- User profile management
- Booking history and management
- Company dashboard for operators
- Admin dashboard for platform management
- Real-time notifications
- Mobile app (React Native)
- Progressive Web App (PWA)

## 🐛 Troubleshooting

### Common Issues

1. **Build errors**: Ensure all dependencies are installed with `npm install`
2. **API connection**: Verify backend is running on the correct port
3. **Environment variables**: Check `.env.local` file exists and has correct values
4. **TypeScript errors**: Run `npm run type-check` for detailed error information

### Debug Mode

Start the development server with debug information:
```bash
DEBUG=* npm run dev
```

## 📄 License

This project is part of the EmptyLegs platform. All rights reserved.

## 👥 Contributing

This is a private project. For questions or issues, please contact the development team.

---

Built with ❤️ using React, Next.js, and TypeScript