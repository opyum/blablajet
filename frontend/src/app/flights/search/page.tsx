'use client'

import { useState, useEffect, useMemo, Suspense } from 'react'
import { useRouter, useSearchParams } from 'next/navigation'
import { useQuery } from '@tanstack/react-query'
import { format } from 'date-fns'
import { 
  Plane, 
  Clock, 
  Users, 
  Filter,
  SortAsc,
  SortDesc,
  MapPin,
  Star,
  Wifi,
  Coffee,
  Heart,
  ArrowLeft,
  Building2
} from 'lucide-react'

import { Header } from '@/components/layout/header'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { Badge } from '@/components/ui/badge'
import { FlightsService } from '@/services/flights.service'
import { Flight, FlightSearch, AircraftType } from '@/types'
import { cn } from '@/lib/utils'
import Link from 'next/link'

function FlightSearchPageContent() {
  const router = useRouter()
  const searchParams = useSearchParams()
  const [sortBy, setSortBy] = useState('departureTime')
  const [sortDescending, setSortDescending] = useState(false)
  const [selectedAircraftType, setSelectedAircraftType] = useState<string>('')

  // Extract search parameters
  const searchCriteria: FlightSearch = useMemo(() => {
    const params: FlightSearch = {
      page: 1,
      pageSize: 20,
      sortBy,
      sortDescending,
    }

    const departureAirport = searchParams.get('departureAirport')
    const arrivalAirport = searchParams.get('arrivalAirport')
    const departureDate = searchParams.get('departureDate')
    const passengerCount = searchParams.get('passengerCount')
    const aircraftType = searchParams.get('aircraftType')
    const maxPrice = searchParams.get('maxPrice')

    if (departureAirport) {
      // Extract IATA code from formatted string (e.g., "CDG - Paris Charles de Gaulle, Paris")
      const iataMatch = departureAirport.match(/^([A-Z]{3})/)
      params.departureAirportCode = iataMatch ? iataMatch[1] : departureAirport
    }

    if (arrivalAirport) {
      const iataMatch = arrivalAirport.match(/^([A-Z]{3})/)
      params.arrivalAirportCode = iataMatch ? iataMatch[1] : arrivalAirport
    }

    if (departureDate) {
      params.departureDate = departureDate
    }

    if (passengerCount) {
      params.passengerCount = parseInt(passengerCount)
    }

    if (aircraftType && aircraftType !== selectedAircraftType) {
      params.aircraftType = parseInt(aircraftType) as AircraftType
    } else if (selectedAircraftType) {
      params.aircraftType = parseInt(selectedAircraftType) as AircraftType
    }

    if (maxPrice) {
      params.maxPrice = parseFloat(maxPrice)
    }

    return params
  }, [searchParams, sortBy, sortDescending, selectedAircraftType])

  // Fetch flights
  const { data: searchResults, isLoading, error, refetch } = useQuery({
    queryKey: ['flight-search', searchCriteria],
    queryFn: () => FlightsService.searchFlights(searchCriteria),
    enabled: !!(searchCriteria.departureAirportCode && searchCriteria.arrivalAirportCode),
  })

  const formatDuration = (duration: string) => {
    // Duration is in format "HH:MM:SS"
    const [hours, minutes] = duration.split(':')
    return `${hours}h ${minutes}m`
  }

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'EUR',
    }).format(price)
  }

  const getAircraftTypeLabel = (type: AircraftType) => {
    const types = {
      [AircraftType.Turboprop]: 'Turboprop',
      [AircraftType.LightJet]: 'Light Jet',
      [AircraftType.MidJet]: 'Mid-Size Jet',
      [AircraftType.HeavyJet]: 'Heavy Jet',
      [AircraftType.VeryLightJet]: 'Very Light Jet',
      [AircraftType.Helicopter]: 'Helicopter',
    }
    return types[type] || 'Unknown'
  }

  const handleSort = (newSortBy: string) => {
    if (sortBy === newSortBy) {
      setSortDescending(!sortDescending)
    } else {
      setSortBy(newSortBy)
      setSortDescending(false)
    }
  }

  if (!searchCriteria.departureAirportCode || !searchCriteria.arrivalAirportCode) {
    return (
      <div className="flex flex-col min-h-screen">
        <Header />
        <main className="flex-1 container mx-auto px-4 py-8">
          <div className="text-center">
            <h1 className="text-2xl font-bold mb-4">Invalid Search</h1>
            <p className="text-muted-foreground mb-8">
              Please provide both departure and arrival airports to search for flights.
            </p>
            <Button asChild>
              <Link href="/">
                <ArrowLeft className="w-4 h-4 mr-2" />
                Go Back to Search
              </Link>
            </Button>
          </div>
        </main>
      </div>
    )
  }

  return (
    <div className="flex flex-col min-h-screen">
      <Header />
      
      <main className="flex-1 container mx-auto px-4 py-8">
        {/* Search Summary */}
        <div className="mb-6">
          <div className="flex items-center mb-4">
            <Button variant="ghost" size="sm" asChild>
              <Link href="/">
                <ArrowLeft className="w-4 h-4 mr-2" />
                Back to Search
              </Link>
            </Button>
          </div>
          
          <h1 className="text-2xl font-bold mb-2">
            {searchCriteria.departureAirportCode} → {searchCriteria.arrivalAirportCode}
          </h1>
          <div className="flex flex-wrap gap-4 text-sm text-muted-foreground">
            {searchCriteria.departureDate && (
              <span>
                📅 {format(new Date(searchCriteria.departureDate), 'MMM dd, yyyy')}
              </span>
            )}
            {searchCriteria.passengerCount && (
              <span>
                👥 {searchCriteria.passengerCount} passenger{searchCriteria.passengerCount !== 1 ? 's' : ''}
              </span>
            )}
          </div>
        </div>

        <div className="grid lg:grid-cols-4 gap-6">
          {/* Filters Sidebar */}
          <div className="lg:col-span-1">
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center">
                  <Filter className="w-4 h-4 mr-2" />
                  Filters & Sort
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                {/* Sort Options */}
                <div>
                  <label className="text-sm font-medium mb-2 block">Sort by</label>
                  <Select value={sortBy} onValueChange={handleSort}>
                    <SelectTrigger>
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="departureTime">
                        <div className="flex items-center">
                          <Clock className="w-4 h-4 mr-2" />
                          Departure Time
                        </div>
                      </SelectItem>
                      <SelectItem value="price">
                        <div className="flex items-center">
                          <span className="text-lg mr-2">€</span>
                          Price
                        </div>
                      </SelectItem>
                      <SelectItem value="duration">
                        <div className="flex items-center">
                          <Plane className="w-4 h-4 mr-2" />
                          Duration
                        </div>
                      </SelectItem>
                    </SelectContent>
                  </Select>
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={() => setSortDescending(!sortDescending)}
                    className="mt-2 w-full"
                  >
                    {sortDescending ? (
                      <>
                        <SortDesc className="w-4 h-4 mr-2" />
                        Descending
                      </>
                    ) : (
                      <>
                        <SortAsc className="w-4 h-4 mr-2" />
                        Ascending
                      </>
                    )}
                  </Button>
                </div>

                {/* Aircraft Type Filter */}
                <div>
                  <label className="text-sm font-medium mb-2 block">Aircraft Type</label>
                  <Select value={selectedAircraftType} onValueChange={setSelectedAircraftType}>
                    <SelectTrigger>
                      <SelectValue placeholder="Any type" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="">Any type</SelectItem>
                      <SelectItem value={AircraftType.VeryLightJet.toString()}>Very Light Jet</SelectItem>
                      <SelectItem value={AircraftType.LightJet.toString()}>Light Jet</SelectItem>
                      <SelectItem value={AircraftType.MidJet.toString()}>Mid-Size Jet</SelectItem>
                      <SelectItem value={AircraftType.HeavyJet.toString()}>Heavy Jet</SelectItem>
                      <SelectItem value={AircraftType.Turboprop.toString()}>Turboprop</SelectItem>
                      <SelectItem value={AircraftType.Helicopter.toString()}>Helicopter</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
              </CardContent>
            </Card>
          </div>

          {/* Results */}
          <div className="lg:col-span-3">
            {isLoading ? (
              <div className="text-center py-12">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary mx-auto mb-4"></div>
                <p>Searching for flights...</p>
              </div>
            ) : error ? (
              <div className="text-center py-12">
                <p className="text-destructive mb-4">Error loading flights</p>
                <Button onClick={() => refetch()}>Try Again</Button>
              </div>
            ) : searchResults?.flights.length === 0 ? (
              <div className="text-center py-12">
                <Plane className="w-12 h-12 text-muted-foreground mx-auto mb-4" />
                <h3 className="text-lg font-semibold mb-2">No flights found</h3>
                <p className="text-muted-foreground mb-4">
                  Try adjusting your search criteria or dates
                </p>
                <Button asChild>
                  <Link href="/">Modify Search</Link>
                </Button>
              </div>
            ) : (
              <>
                {/* Results Summary */}
                <div className="flex justify-between items-center mb-6">
                  <h2 className="text-lg font-semibold">
                    {searchResults?.totalCount} flights found
                  </h2>
                </div>

                {/* Flight Cards */}
                <div className="space-y-4">
                  {searchResults?.flights.map((flight) => (
                    <Card key={flight.id} className="hover:shadow-lg transition-shadow">
                      <CardContent className="p-6">
                        <div className="grid md:grid-cols-4 gap-4 items-center">
                          {/* Flight Route & Times */}
                          <div className="md:col-span-2">
                            <div className="flex items-center justify-between mb-2">
                              <div className="text-center">
                                <div className="text-lg font-bold">
                                  {format(new Date(flight.departureTime), 'HH:mm')}
                                </div>
                                <div className="text-sm text-muted-foreground">
                                  {flight.departureAirport.iataCode}
                                </div>
                                <div className="text-xs text-muted-foreground">
                                  {flight.departureAirport.city}
                                </div>
                              </div>
                              
                              <div className="flex-1 mx-4">
                                <div className="text-center text-xs text-muted-foreground mb-1">
                                  {formatDuration(flight.duration)}
                                </div>
                                <div className="relative">
                                  <div className="absolute inset-0 flex items-center">
                                    <div className="w-full border-t border-dashed"></div>
                                  </div>
                                  <div className="relative flex justify-center">
                                    <Plane className="w-4 h-4 bg-background text-muted-foreground" />
                                  </div>
                                </div>
                              </div>
                              
                              <div className="text-center">
                                <div className="text-lg font-bold">
                                  {format(new Date(flight.arrivalTime), 'HH:mm')}
                                </div>
                                <div className="text-sm text-muted-foreground">
                                  {flight.arrivalAirport.iataCode}
                                </div>
                                <div className="text-xs text-muted-foreground">
                                  {flight.arrivalAirport.city}
                                </div>
                              </div>
                            </div>

                            {/* Aircraft & Company Info */}
                            <div className="space-y-1">
                              <div className="flex items-center text-sm">
                                <Plane className="w-4 h-4 mr-2 text-muted-foreground" />
                                <span className="font-medium">{flight.aircraft.manufacturer} {flight.aircraft.model}</span>
                                <Badge variant="secondary" className="ml-2">
                                  {getAircraftTypeLabel(flight.aircraft.type)}
                                </Badge>
                              </div>
                              
                              <div className="flex items-center text-sm text-muted-foreground">
                                <Building2 className="w-4 h-4 mr-2" />
                                <span>{flight.company.name}</span>
                                {flight.company.isVerified && (
                                  <Badge variant="default" className="ml-2 text-xs">
                                    Verified
                                  </Badge>
                                )}
                              </div>

                              <div className="flex items-center text-sm text-muted-foreground">
                                <Users className="w-4 h-4 mr-2" />
                                <span>
                                  {flight.availableSeats} of {flight.totalSeats} seats available
                                </span>
                              </div>
                            </div>

                            {/* Amenities */}
                            {flight.aircraft.amenities.length > 0 && (
                              <div className="flex items-center gap-2 mt-2">
                                {flight.aircraft.amenities.slice(0, 3).map((amenity) => (
                                  <Badge key={amenity} variant="outline" className="text-xs">
                                    {amenity === 'WiFi' && <Wifi className="w-3 h-3 mr-1" />}
                                    {amenity === 'Refreshments' && <Coffee className="w-3 h-3 mr-1" />}
                                    {amenity}
                                  </Badge>
                                ))}
                                {flight.aircraft.amenities.length > 3 && (
                                  <span className="text-xs text-muted-foreground">
                                    +{flight.aircraft.amenities.length - 3} more
                                  </span>
                                )}
                              </div>
                            )}
                          </div>

                          {/* Price & Actions */}
                          <div className="text-center">
                            <div className="mb-2">
                              {flight.currentPrice < flight.basePrice && (
                                <div className="text-sm text-muted-foreground line-through">
                                  {formatPrice(flight.basePrice)}
                                </div>
                              )}
                              <div className="text-2xl font-bold text-primary">
                                {formatPrice(flight.currentPrice)}
                              </div>
                              <div className="text-xs text-muted-foreground">
                                per passenger
                              </div>
                            </div>
                          </div>

                          {/* Actions */}
                          <div className="space-y-2">
                            <Button className="w-full" asChild>
                              <Link href={`/flights/${flight.id}`}>
                                View Details
                              </Link>
                            </Button>
                            <Button variant="outline" size="sm" className="w-full">
                              <Heart className="w-4 h-4 mr-2" />
                              Save
                            </Button>
                          </div>
                        </div>
                      </CardContent>
                    </Card>
                  ))}
                </div>

                {/* Pagination would go here */}
                {searchResults && searchResults.totalPages > 1 && (
                  <div className="mt-8 text-center">
                    <p className="text-sm text-muted-foreground">
                      Showing page {searchResults.page} of {searchResults.totalPages}
                    </p>
                  </div>
                )}
              </>
            )}
          </div>
        </div>
      </main>
    </div>
  )
}

export default function FlightSearchPage() {
  return (
    <Suspense fallback={
      <div className="flex flex-col min-h-screen">
        <Header />
        <main className="flex-1 flex items-center justify-center">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
        </main>
      </div>
    }>
      <FlightSearchPageContent />
    </Suspense>
  )
}