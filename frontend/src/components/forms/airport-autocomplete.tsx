'use client'

import { useState, useEffect, useMemo } from 'react'
import { useQuery } from '@tanstack/react-query'
import { Check, ChevronsUpDown, MapPin } from 'lucide-react'

import { cn } from '@/lib/utils'
import { Button } from '@/components/ui/button'
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
} from '@/components/ui/command'
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from '@/components/ui/popover'
import { AirportsService } from '@/services/airports.service'
import { Airport } from '@/types'

interface AirportAutocompleteProps {
  value: string
  onValueChange: (value: string) => void
  placeholder?: string
  disabled?: boolean
}

export function AirportAutocomplete({
  value,
  onValueChange,
  placeholder = 'Search airports...',
  disabled = false,
}: AirportAutocompleteProps) {
  const [open, setOpen] = useState(false)
  const [search, setSearch] = useState('')

  // Fetch airports when search term changes
  const { data: airports, isLoading } = useQuery({
    queryKey: ['airports-search', search],
    queryFn: () => AirportsService.searchForAutocomplete(search),
    enabled: search.length >= 2,
    staleTime: 5 * 60 * 1000, // 5 minutes
  })

  // Popular airports for initial display
  const { data: popularAirports } = useQuery({
    queryKey: ['popular-airports'],
    queryFn: () => AirportsService.getAirports(1, 20, { isActive: true }),
    staleTime: 30 * 60 * 1000, // 30 minutes
  })

  const displayAirports = useMemo(() => {
    if (search.length >= 2 && airports) {
      return airports
    }
    if (popularAirports?.data) {
      return popularAirports.data.slice(0, 10) // Show top 10 popular airports
    }
    return []
  }, [search, airports, popularAirports])

  const selectedAirport = useMemo(() => {
    if (!value) return null
    return displayAirports.find(airport => 
      airport.iataCode === value || 
      AirportsService.formatAirportDisplay(airport) === value
    )
  }, [value, displayAirports])

  const handleSelect = (airport: Airport) => {
    const formattedValue = AirportsService.formatAirportDisplay(airport)
    onValueChange(formattedValue)
    setOpen(false)
    setSearch('')
  }

  const displayValue = selectedAirport 
    ? AirportsService.formatAirportDisplay(selectedAirport)
    : value || placeholder

  return (
    <Popover open={open} onOpenChange={setOpen}>
      <PopoverTrigger asChild>
        <Button
          variant="outline"
          role="combobox"
          aria-expanded={open}
          className={cn(
            'w-full justify-between',
            !value && 'text-muted-foreground'
          )}
          disabled={disabled}
        >
          <span className="truncate">
            {selectedAirport ? (
              <div className="flex items-center">
                <MapPin className="w-4 h-4 mr-2 flex-shrink-0" />
                <span className="truncate">{displayValue}</span>
              </div>
            ) : (
              displayValue
            )}
          </span>
          <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-[400px] p-0" align="start">
        <Command>
          <CommandInput
            placeholder="Search airports by city or code..."
            value={search}
            onValueChange={setSearch}
          />
          <CommandEmpty>
            {isLoading ? (
              <div className="flex items-center justify-center py-6">
                <div className="animate-spin rounded-full h-6 w-6 border-b-2 border-primary"></div>
                <span className="ml-2">Searching airports...</span>
              </div>
            ) : search.length < 2 ? (
              <div className="py-6 text-center text-sm text-muted-foreground">
                Type at least 2 characters to search
              </div>
            ) : (
              <div className="py-6 text-center text-sm text-muted-foreground">
                No airports found
              </div>
            )}
          </CommandEmpty>
          <CommandGroup>
            {displayAirports.map((airport) => (
              <CommandItem
                key={airport.id}
                value={airport.iataCode}
                onSelect={() => handleSelect(airport)}
                className="flex items-center justify-between"
              >
                <div className="flex items-center">
                  <MapPin className="w-4 h-4 mr-2 text-muted-foreground" />
                  <div>
                    <div className="font-medium">
                      {airport.iataCode} - {airport.name}
                    </div>
                    <div className="text-sm text-muted-foreground">
                      {airport.city}, {airport.country}
                    </div>
                  </div>
                </div>
                <Check
                  className={cn(
                    'ml-auto h-4 w-4',
                    selectedAirport?.id === airport.id
                      ? 'opacity-100'
                      : 'opacity-0'
                  )}
                />
              </CommandItem>
            ))}
          </CommandGroup>
        </Command>
      </PopoverContent>
    </Popover>
  )
}