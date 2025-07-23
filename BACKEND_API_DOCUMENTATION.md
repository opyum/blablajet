# EmptyLegs Backend API Documentation

This documentation provides comprehensive information about the EmptyLegs backend API endpoints, data structures, and authentication mechanisms for frontend development.

## Base URL

**Development**: `http://localhost:5000/api/v1`  
**Production**: `https://api.emptylegs.com/api/v1`

## Authentication

The API uses JWT (JSON Web Tokens) for authentication. Include the JWT token in the Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

### Authentication Endpoints

#### POST `/auth/login`
Authenticate user and get access token
- **Public endpoint**
- **Request Body**:
```json
{
  "email": "user@example.com",
  "password": "userPassword"
}
```
- **Response** (200 OK):
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh_token_string",
  "user": {
    "id": "uuid",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "role": "Customer",
    "isActive": true,
    "isEmailVerified": true
  },
  "expiresIn": 3600
}
```

#### POST `/auth/register`
Register a new user
- **Public endpoint**
- **Request Body**:
```json
{
  "email": "user@example.com",
  "password": "userPassword",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1234567890",
  "dateOfBirth": "1990-01-01T00:00:00Z",
  "role": "Customer"
}
```
- **Response** (201 Created): Same as login response

#### POST `/auth/refresh`
Refresh access token using refresh token
- **Public endpoint**
- **Request Body**:
```json
{
  "refreshToken": "refresh_token_string"
}
```
- **Response** (200 OK): Same as login response

#### POST `/auth/revoke`
Revoke current refresh token
- **Requires Authentication**

#### POST `/auth/revoke-all`
Revoke all refresh tokens for the user
- **Requires Authentication**

#### POST `/auth/change-password`
Change user password
- **Requires Authentication**
- **Request Body**:
```json
{
  "currentPassword": "oldPassword",
  "newPassword": "newPassword"
}
```

#### POST `/auth/reset-password`
Request password reset
- **Public endpoint**
- **Request Body**:
```json
{
  "email": "user@example.com"
}
```

#### POST `/auth/confirm-email`
Confirm email address
- **Public endpoint**
- **Request Body**:
```json
{
  "userId": "uuid",
  "token": "confirmation_token"
}
```

#### POST `/auth/resend-confirmation`
Resend email confirmation
- **Public endpoint**
- **Request Body**:
```json
{
  "email": "user@example.com"
}
```

#### GET `/auth/me`
Get current user information
- **Requires Authentication**
- **Response** (200 OK): UserDto object

## Flight Management

### Flight Endpoints

#### GET `/flights/search`
Search for available flights
- **Public endpoint**
- **Query Parameters**:
  - `departureAirportCode` (string): IATA code
  - `arrivalAirportCode` (string): IATA code
  - `departureDate` (datetime): Specific departure date
  - `departureDateFrom` (datetime): Date range start
  - `departureDateTo` (datetime): Date range end
  - `passengerCount` (int): Number of passengers
  - `aircraftType` (enum): Turboprop, LightJet, MidJet, HeavyJet, VeryLightJet, Helicopter
  - `maxPrice` (decimal): Maximum price filter
  - `minPrice` (decimal): Minimum price filter
  - `page` (int): Page number (default: 1)
  - `pageSize` (int): Items per page (default: 20)
  - `sortBy` (string): price, duration, departuretime
  - `sortDescending` (bool): Sort order

- **Response** (200 OK):
```json
{
  "flights": [
    {
      "id": "uuid",
      "flightNumber": "EL001",
      "departureTime": "2024-01-15T10:00:00Z",
      "arrivalTime": "2024-01-15T12:30:00Z",
      "basePrice": 5000.00,
      "currentPrice": 4500.00,
      "availableSeats": 6,
      "totalSeats": 8,
      "status": "Available",
      "description": "Private jet flight",
      "departureAirport": {
        "id": "uuid",
        "name": "Paris Charles de Gaulle",
        "iataCode": "CDG",
        "city": "Paris",
        "country": "France"
      },
      "arrivalAirport": {
        "id": "uuid",
        "name": "London Heathrow",
        "iataCode": "LHR",
        "city": "London",
        "country": "United Kingdom"
      },
      "aircraft": {
        "id": "uuid",
        "model": "Citation CJ3+",
        "manufacturer": "Cessna",
        "type": "LightJet",
        "capacity": 8,
        "amenities": ["WiFi", "Refreshments"]
      },
      "company": {
        "id": "uuid",
        "name": "Elite Aviation",
        "isVerified": true
      },
      "duration": "02:30:00",
      "isFullyBooked": false
    }
  ],
  "totalCount": 45,
  "page": 1,
  "pageSize": 20,
  "totalPages": 3,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

#### GET `/flights/{id}`
Get flight details by ID
- **Public endpoint**
- **Response** (200 OK): FlightDto object

#### POST `/flights`
Create new flight
- **Requires Authentication** (Company/Admin role)
- **Request Body**:
```json
{
  "flightNumber": "EL001",
  "departureAirportId": "uuid",
  "arrivalAirportId": "uuid",
  "departureTime": "2024-01-15T10:00:00Z",
  "arrivalTime": "2024-01-15T12:30:00Z",
  "aircraftId": "uuid",
  "basePrice": 5000.00,
  "availableSeats": 6,
  "description": "Private jet flight",
  "specialInstructions": "VIP service",
  "allowsAutomaticPricing": true,
  "minimumPrice": 3000.00
}
```

#### PUT `/flights/{id}`
Update flight details
- **Requires Authentication** (Company/Admin role)
- **Request Body**: UpdateFlightDto (partial update)

#### DELETE `/flights/{id}`
Delete flight
- **Requires Authentication** (Company/Admin role)

## Booking Management

### Booking Endpoints

#### GET `/bookings`
Get bookings (user's own bookings or all for admin)
- **Requires Authentication**
- **Query Parameters**:
  - `page` (int): Page number (default: 1)
  - `pageSize` (int): Items per page (default: 20)
  - `status` (enum): Pending, Confirmed, PaymentPending, PaymentConfirmed, Cancelled, Refunded, Completed
  - `userId` (uuid): Filter by user (Admin only)

#### GET `/bookings/{id}`
Get booking details by ID
- **Requires Authentication** (Owner or Admin)

#### POST `/bookings`
Create new booking
- **Requires Authentication**
- **Request Body**:
```json
{
  "flightId": "uuid",
  "passengerCount": 2,
  "totalPrice": 4500.00,
  "specialRequests": "Vegetarian meals",
  "passengers": [
    {
      "firstName": "John",
      "lastName": "Doe",
      "dateOfBirth": "1990-01-01T00:00:00Z",
      "passportNumber": "A12345678",
      "nationality": "US",
      "specialRequests": "Wheelchair assistance"
    }
  ],
  "additionalServices": [
    {
      "serviceType": "Catering",
      "name": "Premium Meal",
      "description": "Gourmet dining",
      "price": 150.00,
      "quantity": 2
    }
  ]
}
```

#### PATCH `/bookings/{id}/status`
Update booking status
- **Requires Authentication** (Company/Admin role)
- **Request Body**:
```json
{
  "status": "Confirmed",
  "reason": "Payment verified"
}
```

#### POST `/bookings/{id}/cancel`
Cancel booking
- **Requires Authentication** (Owner or Admin)
- **Request Body**:
```json
{
  "cancellationReason": "Change of plans"
}
```

#### POST `/bookings/{id}/confirm`
Confirm booking
- **Requires Authentication** (Company/Admin role)

#### POST `/bookings/{id}/complete`
Mark booking as completed
- **Requires Authentication** (Company/Admin role)

#### GET `/bookings/{id}/passengers`
Get booking passengers
- **Requires Authentication** (Owner or Admin)

#### GET `/bookings/{id}/payments`
Get booking payments
- **Requires Authentication** (Owner or Admin)

#### GET `/bookings/statistics`
Get booking statistics
- **Requires Authentication** (Admin role)

## User Management

### User Endpoints

#### GET `/users`
Get all users (Admin only)
- **Requires Authentication** (Admin role)
- **Query Parameters**:
  - `page` (int): Page number
  - `pageSize` (int): Items per page
  - `role` (enum): Customer, Company, Admin

#### GET `/users/{id}`
Get user by ID
- **Requires Authentication** (Owner or Admin)

#### PUT `/users/{id}`
Update user details
- **Requires Authentication** (Owner or Admin)
- **Request Body**:
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1234567890",
  "dateOfBirth": "1990-01-01T00:00:00Z"
}
```

#### PATCH `/users/{id}/deactivate`
Deactivate user
- **Requires Authentication** (Admin role)

#### PATCH `/users/{id}/activate`
Activate user
- **Requires Authentication** (Admin role)

#### DELETE `/users/{id}`
Delete user
- **Requires Authentication** (Admin role)

#### GET `/users/{id}/bookings`
Get user's bookings
- **Requires Authentication** (Owner or Admin)

#### GET `/users/statistics`
Get user statistics
- **Requires Authentication** (Admin role)

## Aircraft Management

### Aircraft Endpoints

#### GET `/aircraft`
Get all aircraft
- **Public endpoint**
- **Query Parameters**:
  - `page` (int): Page number
  - `pageSize` (int): Items per page
  - `type` (enum): Aircraft type filter
  - `companyId` (uuid): Company filter
  - `isActive` (bool): Active status filter
  - `manufacturer` (string): Manufacturer filter

#### GET `/aircraft/{id}`
Get aircraft by ID
- **Public endpoint**

#### GET `/aircraft/search`
Search aircraft with advanced filters
- **Public endpoint**

#### POST `/aircraft`
Create new aircraft
- **Requires Authentication** (Company/Admin role)
- **Request Body**:
```json
{
  "model": "Citation CJ3+",
  "manufacturer": "Cessna",
  "registration": "N123AB",
  "capacity": 8,
  "type": "LightJet",
  "yearManufactured": 2020,
  "description": "Luxury light jet",
  "cruiseSpeed": 464.0,
  "range": 2040.0,
  "amenities": ["WiFi", "Entertainment System"],
  "photoUrls": ["url1", "url2"],
  "companyId": "uuid"
}
```

#### PUT `/aircraft/{id}`
Update aircraft details
- **Requires Authentication** (Company/Admin role)

#### PATCH `/aircraft/{id}/deactivate`
Deactivate aircraft
- **Requires Authentication** (Company/Admin role)

#### PATCH `/aircraft/{id}/activate`
Activate aircraft
- **Requires Authentication** (Company/Admin role)

#### DELETE `/aircraft/{id}`
Delete aircraft
- **Requires Authentication** (Company/Admin role)

#### GET `/aircraft/{id}/flights`
Get flights for aircraft
- **Public endpoint**

#### GET `/aircraft/types`
Get available aircraft types
- **Public endpoint**

#### GET `/aircraft/manufacturers`
Get available manufacturers
- **Public endpoint**

#### GET `/aircraft/statistics`
Get aircraft statistics
- **Requires Authentication** (Admin role)

## Airport Management

### Airport Endpoints

#### GET `/airports`
Get all airports
- **Public endpoint**
- **Query Parameters**:
  - `page` (int): Page number
  - `pageSize` (int): Items per page (default: 50)
  - `country` (string): Country filter
  - `city` (string): City filter
  - `search` (string): Search in name, city, or IATA code
  - `isActive` (bool): Active status filter

#### GET `/airports/{id}`
Get airport by ID
- **Public endpoint**

#### GET `/airports/iata/{iataCode}`
Get airport by IATA code
- **Public endpoint**

#### GET `/airports/search`
Advanced airport search
- **Public endpoint**

#### GET `/airports/nearby`
Get nearby airports
- **Public endpoint**
- **Query Parameters**:
  - `latitude` (decimal): Latitude
  - `longitude` (decimal): Longitude
  - `radiusKm` (int): Search radius in kilometers

#### POST `/airports`
Create new airport
- **Requires Authentication** (Admin role)

#### PUT `/airports/{id}`
Update airport details
- **Requires Authentication** (Admin role)

#### PATCH `/airports/{id}/deactivate`
Deactivate airport
- **Requires Authentication** (Admin role)

#### DELETE `/airports/{id}`
Delete airport
- **Requires Authentication** (Admin role)

#### GET `/airports/countries`
Get list of countries with airports
- **Public endpoint**

#### GET `/airports/countries/{country}/cities`
Get cities in a country with airports
- **Public endpoint**

## Company Management

### Company Endpoints

#### GET `/companies`
Get all companies
- **Public endpoint**
- **Query Parameters**:
  - `page` (int): Page number
  - `pageSize` (int): Items per page
  - `isVerified` (bool): Verification status filter
  - `isActive` (bool): Active status filter

#### GET `/companies/{id}`
Get company by ID
- **Public endpoint**

#### POST `/companies`
Create new company
- **Requires Authentication**

#### PUT `/companies/{id}`
Update company details
- **Requires Authentication** (Company owner or Admin)

#### PATCH `/companies/{id}/verify`
Verify company
- **Requires Authentication** (Admin role)

#### PATCH `/companies/{id}/unverify`
Unverify company
- **Requires Authentication** (Admin role)

#### PATCH `/companies/{id}/deactivate`
Deactivate company
- **Requires Authentication** (Admin role)

#### DELETE `/companies/{id}`
Delete company
- **Requires Authentication** (Admin role)

#### GET `/companies/{id}/aircraft`
Get company's aircraft
- **Public endpoint**

#### GET `/companies/{id}/flights`
Get company's flights
- **Public endpoint**

#### GET `/companies/{id}/statistics`
Get company statistics
- **Requires Authentication** (Company owner or Admin)

## Data Models

### Enums

#### UserRole
- `Customer` (1)
- `Company` (2)
- `Admin` (3)

#### FlightStatus
- `Available` (1)
- `Reserved` (2)
- `Confirmed` (3)
- `InProgress` (4)
- `Completed` (5)
- `Cancelled` (6)

#### BookingStatus
- `Pending` (1)
- `Confirmed` (2)
- `PaymentPending` (3)
- `PaymentConfirmed` (4)
- `Cancelled` (5)
- `Refunded` (6)
- `Completed` (7)

#### AircraftType
- `Turboprop` (1)
- `LightJet` (2)
- `MidJet` (3)
- `HeavyJet` (4)
- `VeryLightJet` (5)
- `Helicopter` (6)

### Error Responses

All endpoints return consistent error responses:

```json
{
  "message": "Error description",
  "details": "Additional error details (if available)"
}
```

Common HTTP status codes:
- `200 OK`: Successful request
- `201 Created`: Resource created successfully
- `400 Bad Request`: Invalid request data
- `401 Unauthorized`: Authentication required
- `403 Forbidden`: Insufficient permissions
- `404 Not Found`: Resource not found
- `409 Conflict`: Resource conflict (e.g., duplicate email)
- `500 Internal Server Error`: Server error

### Pagination

Paginated endpoints return data in this format:

```json
{
  "data": [...],
  "totalCount": 100,
  "page": 1,
  "pageSize": 20,
  "totalPages": 5,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

### Rate Limiting

The API implements rate limiting:
- **Authenticated users**: 1000 requests per hour
- **Anonymous users**: 100 requests per hour

Rate limit headers are included in responses:
- `X-RateLimit-Limit`: Total requests allowed
- `X-RateLimit-Remaining`: Remaining requests
- `X-RateLimit-Reset`: Reset time (Unix timestamp)

## Development Notes

### Authentication Flow
1. User registers/logs in via `/auth/register` or `/auth/login`
2. Store the `accessToken` and `refreshToken` in secure storage
3. Include `accessToken` in Authorization header for protected endpoints
4. Use `/auth/refresh` to get new tokens when `accessToken` expires
5. Use `/auth/revoke` or `/auth/revoke-all` to logout

### Best Practices for Frontend Integration

1. **Error Handling**: Always check HTTP status codes and handle errors appropriately
2. **Token Management**: Implement automatic token refresh logic
3. **Pagination**: Use the pagination metadata to implement infinite scroll or pagination UI
4. **Loading States**: Show loading indicators during API calls
5. **Validation**: Validate data on frontend before sending to API
6. **Caching**: Cache static data like airports and aircraft types
7. **Real-time Updates**: Consider implementing WebSocket connections for real-time booking updates

### Environment Configuration

Development environment typically runs on:
- **Backend**: `http://localhost:5000`
- **Database**: PostgreSQL on port 5432
- **Redis**: Redis cache on port 6379

The API supports CORS and includes Swagger documentation at `/swagger` when running in development mode.