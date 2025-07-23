'use client'

import { useState } from 'react'
import { useRouter } from 'next/navigation'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { format } from 'date-fns'
import { CalendarIcon, Plane, Users, ArrowLeftRight } from 'lucide-react'

import { Button } from '@/components/ui/button'
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form'
import { Input } from '@/components/ui/input'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from '@/components/ui/popover'
import { Calendar } from '@/components/ui/calendar'
import { cn } from '@/lib/utils'
import { AircraftType } from '@/types'
import { AirportAutocomplete } from '@/components/forms/airport-autocomplete'

const searchFormSchema = z.object({
  departureAirport: z.string().min(1, 'Departure airport is required'),
  arrivalAirport: z.string().min(1, 'Arrival airport is required'),
  departureDate: z.date({
    required_error: 'Departure date is required',
  }),
  passengerCount: z.string().min(1, 'Number of passengers is required'),
  aircraftType: z.string().optional(),
  maxPrice: z.string().optional(),
})

type SearchFormValues = z.infer<typeof searchFormSchema>

export function SearchFlightsForm() {
  const router = useRouter()
  const [isLoading, setIsLoading] = useState(false)

  const form = useForm<SearchFormValues>({
    resolver: zodResolver(searchFormSchema),
    defaultValues: {
      departureAirport: '',
      arrivalAirport: '',
      passengerCount: '1',
      aircraftType: '',
      maxPrice: '',
    },
  })

  const onSubmit = async (data: SearchFormValues) => {
    setIsLoading(true)
    
    const searchParams = new URLSearchParams()
    searchParams.set('departureAirport', data.departureAirport)
    searchParams.set('arrivalAirport', data.arrivalAirport)
    searchParams.set('departureDate', format(data.departureDate, 'yyyy-MM-dd'))
    searchParams.set('passengerCount', data.passengerCount)
    
    if (data.aircraftType) {
      searchParams.set('aircraftType', data.aircraftType)
    }
    if (data.maxPrice) {
      searchParams.set('maxPrice', data.maxPrice)
    }

    router.push(`/flights/search?${searchParams.toString()}`)
  }

  const swapAirports = () => {
    const departure = form.getValues('departureAirport')
    const arrival = form.getValues('arrivalAirport')
    
    form.setValue('departureAirport', arrival)
    form.setValue('arrivalAirport', departure)
  }

  const aircraftTypes = [
    { value: AircraftType.VeryLightJet.toString(), label: 'Very Light Jet' },
    { value: AircraftType.LightJet.toString(), label: 'Light Jet' },
    { value: AircraftType.MidJet.toString(), label: 'Mid-Size Jet' },
    { value: AircraftType.HeavyJet.toString(), label: 'Heavy Jet' },
    { value: AircraftType.Turboprop.toString(), label: 'Turboprop' },
    { value: AircraftType.Helicopter.toString(), label: 'Helicopter' },
  ]

  const passengerOptions = Array.from({ length: 12 }, (_, i) => ({
    value: (i + 1).toString(),
    label: i + 1 === 1 ? '1 passenger' : `${i + 1} passengers`,
  }))

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
        {/* Main Search Fields */}
        <div className="grid grid-cols-1 lg:grid-cols-7 gap-4 items-end">
          {/* Departure Airport */}
          <div className="lg:col-span-2">
            <FormField
              control={form.control}
              name="departureAirport"
              render={({ field }) => (
                <FormItem>
                  <FormLabel className="flex items-center">
                    <Plane className="w-4 h-4 mr-2" />
                    From
                  </FormLabel>
                  <FormControl>
                    <AirportAutocomplete
                      value={field.value}
                      onValueChange={field.onChange}
                      placeholder="Departure city or airport"
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>

          {/* Swap Button */}
          <div className="flex justify-center lg:col-span-1">
            <Button
              type="button"
              variant="outline"
              size="icon"
              onClick={swapAirports}
              className="h-10 w-10"
            >
              <ArrowLeftRight className="w-4 h-4" />
            </Button>
          </div>

          {/* Arrival Airport */}
          <div className="lg:col-span-2">
            <FormField
              control={form.control}
              name="arrivalAirport"
              render={({ field }) => (
                <FormItem>
                  <FormLabel className="flex items-center">
                    <Plane className="w-4 h-4 mr-2 rotate-90" />
                    To
                  </FormLabel>
                  <FormControl>
                    <AirportAutocomplete
                      value={field.value}
                      onValueChange={field.onChange}
                      placeholder="Destination city or airport"
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>

          {/* Departure Date */}
          <div className="lg:col-span-1">
            <FormField
              control={form.control}
              name="departureDate"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Departure</FormLabel>
                  <Popover>
                    <PopoverTrigger asChild>
                      <FormControl>
                        <Button
                          variant="outline"
                          className={cn(
                            'w-full pl-3 text-left font-normal',
                            !field.value && 'text-muted-foreground'
                          )}
                        >
                          {field.value ? (
                            format(field.value, 'MMM dd')
                          ) : (
                            <span>Pick date</span>
                          )}
                          <CalendarIcon className="ml-auto h-4 w-4 opacity-50" />
                        </Button>
                      </FormControl>
                    </PopoverTrigger>
                    <PopoverContent className="w-auto p-0" align="start">
                      <Calendar
                        mode="single"
                        selected={field.value}
                        onSelect={field.onChange}
                        disabled={(date) =>
                          date < new Date() || date < new Date('1900-01-01')
                        }
                        initialFocus
                      />
                    </PopoverContent>
                  </Popover>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>

          {/* Passengers */}
          <div className="lg:col-span-1">
            <FormField
              control={form.control}
              name="passengerCount"
              render={({ field }) => (
                <FormItem>
                  <FormLabel className="flex items-center">
                    <Users className="w-4 h-4 mr-2" />
                    Passengers
                  </FormLabel>
                  <Select onValueChange={field.onChange} defaultValue={field.value}>
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue placeholder="Select" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {passengerOptions.map((option) => (
                        <SelectItem key={option.value} value={option.value}>
                          {option.label}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>
        </div>

        {/* Advanced Filters */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          {/* Aircraft Type */}
          <FormField
            control={form.control}
            name="aircraftType"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Aircraft Type (Optional)</FormLabel>
                <Select onValueChange={field.onChange} defaultValue={field.value}>
                  <FormControl>
                    <SelectTrigger>
                      <SelectValue placeholder="Any aircraft type" />
                    </SelectTrigger>
                  </FormControl>
                  <SelectContent>
                    {aircraftTypes.map((type) => (
                      <SelectItem key={type.value} value={type.value}>
                        {type.label}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                <FormMessage />
              </FormItem>
            )}
          />

          {/* Max Price */}
          <FormField
            control={form.control}
            name="maxPrice"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Max Price (Optional)</FormLabel>
                <FormControl>
                  <Input
                    type="number"
                    placeholder="Maximum budget"
                    {...field}
                  />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          {/* Search Button */}
          <div className="flex items-end">
            <Button 
              type="submit" 
              className="w-full"
              size="lg"
              disabled={isLoading}
            >
              {isLoading ? (
                'Searching...'
              ) : (
                <>
                  Search Flights
                  <Plane className="ml-2 w-4 h-4" />
                </>
              )}
            </Button>
          </div>
        </div>
      </form>
    </Form>
  )
}