'use client'

import { useState, useCallback, useMemo } from 'react'
import { GoogleMap, useLoadScript, Marker, InfoWindow } from '@react-google-maps/api'
import { MapPin, Plane, Anchor, Car, Building, Star, Euro } from 'lucide-react'

interface MapItem {
  id: string
  type: 'flight' | 'yacht' | 'car' | 'hotel'
  title: string
  location: {
    lat: number
    lng: number
  }
  price: number
  rating?: number
  image: string
  description: string
  luxuryLevel: string
}

interface InteractiveMapProps {
  items: MapItem[]
  center?: { lat: number; lng: number }
  zoom?: number
  onItemSelect?: (item: MapItem) => void
  activeFilters?: {
    type?: string[]
    priceRange?: [number, number]
    luxuryLevel?: string[]
  }
}

const mapContainerStyle = {
  width: '100%',
  height: '600px'
}

const defaultCenter = {
  lat: 48.8566,
  lng: 2.3522 // Paris
}

const mapOptions = {
  disableDefaultUI: false,
  zoomControl: true,
  streetViewControl: false,
  mapTypeControl: false,
  fullscreenControl: true,
  styles: [
    {
      featureType: 'poi',
      elementType: 'labels',
      stylers: [{ visibility: 'off' }]
    }
  ]
}

export function InteractiveMap({ 
  items, 
  center = defaultCenter, 
  zoom = 6,
  onItemSelect,
  activeFilters 
}: InteractiveMapProps) {
  const [selectedItem, setSelectedItem] = useState<MapItem | null>(null)
  const [map, setMap] = useState<google.maps.Map | null>(null)

  const { isLoaded, loadError } = useLoadScript({
    googleMapsApiKey: process.env.NEXT_PUBLIC_GOOGLE_MAPS_API_KEY || '',
    libraries: ['places']
  })

  const onLoad = useCallback((map: google.maps.Map) => {
    setMap(map)
  }, [])

  const onUnmount = useCallback(() => {
    setMap(null)
  }, [])

  // Filter items based on active filters
  const filteredItems = useMemo(() => {
    if (!activeFilters) return items

    return items.filter(item => {
      // Filter by type
      if (activeFilters.type && activeFilters.type.length > 0) {
        if (!activeFilters.type.includes(item.type)) return false
      }

      // Filter by price range
      if (activeFilters.priceRange) {
        const [min, max] = activeFilters.priceRange
        if (item.price < min || item.price > max) return false
      }

      // Filter by luxury level
      if (activeFilters.luxuryLevel && activeFilters.luxuryLevel.length > 0) {
        if (!activeFilters.luxuryLevel.includes(item.luxuryLevel)) return false
      }

      return true
    })
  }, [items, activeFilters])

  // Get marker icon based on item type
  const getMarkerIcon = (type: string, isSelected: boolean) => {
    const baseSize = isSelected ? 40 : 32
    const colors = {
      flight: '#3B82F6', // blue
      yacht: '#06B6D4',  // cyan
      car: '#374151',    // gray
      hotel: '#F59E0B'   // amber
    }

    return {
      url: `data:image/svg+xml;charset=UTF-8,${encodeURIComponent(`
        <svg width="${baseSize}" height="${baseSize}" viewBox="0 0 32 32" xmlns="http://www.w3.org/2000/svg">
          <circle cx="16" cy="16" r="14" fill="${colors[type as keyof typeof colors]}" stroke="white" stroke-width="2"/>
          <g transform="translate(8, 8)" fill="white">
            ${type === 'flight' ? '<path d="M8 2L3 7v2l5-2v6l-2 1.5V17l2-0.5L8 17l2 0.5L12 17v-2.5L10 13V7l5 2V7l-5-5z"/>' :
              type === 'yacht' ? '<path d="M12 4V2H8l-2 8H4l-2 2v4h2v-2h12v2h2v-4l-2-2h-2L12 4zM9 4h2v4H9V4z"/>' :
              type === 'car' ? '<path d="M5 11l1.5-4.5h3l1.5 4.5v5c0 .6-.4 1-1 1H6c-.6 0-1-.4-1-1v-5zM6.5 7.5c-.3 0-.5.2-.5.5s.2.5.5.5.5-.2.5-.5-.2-.5-.5-.5zM9.5 7.5c-.3 0-.5.2-.5.5s.2.5.5.5.5-.2.5-.5-.2-.5-.5-.5z"/>' :
              '<path d="M4 2v2h8V2H4zM3 6v10h2v2h2v-2h8v2h2v-2h2V6H3zm2 8V8h10v6H5z"/>'}
          </g>
        </svg>
      `)}`,
      scaledSize: new google.maps.Size(baseSize, baseSize),
      anchor: new google.maps.Point(baseSize / 2, baseSize / 2)
    }
  }

  const handleMarkerClick = (item: MapItem) => {
    setSelectedItem(item)
    onItemSelect?.(item)
  }

  const handleInfoWindowClose = () => {
    setSelectedItem(null)
  }

  const getLuxuryStars = (level: string) => {
    const levels = {
      'standard': 1,
      'premium': 2,
      'luxury': 3,
      'ultra-luxury': 4,
      'exclusive': 5
    }
    return levels[level as keyof typeof levels] || 1
  }

  const getTypeIcon = (type: string) => {
    switch (type) {
      case 'flight': return Plane
      case 'yacht': return Anchor
      case 'car': return Car
      case 'hotel': return Building
      default: return MapPin
    }
  }

  const getTypeLabel = (type: string) => {
    switch (type) {
      case 'flight': return 'Jet Privé'
      case 'yacht': return 'Yacht'
      case 'car': return 'Voiture de Luxe'
      case 'hotel': return 'Hôtel'
      default: return 'Service'
    }
  }

  if (loadError) {
    return (
      <div className="w-full h-96 bg-gray-100 rounded-lg flex items-center justify-center">
        <div className="text-center">
          <MapPin className="w-12 h-12 text-gray-400 mx-auto mb-4" />
          <p className="text-gray-600">Erreur lors du chargement de la carte</p>
        </div>
      </div>
    )
  }

  if (!isLoaded) {
    return (
      <div className="w-full h-96 bg-gray-100 rounded-lg flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
          <p className="text-gray-600">Chargement de la carte...</p>
        </div>
      </div>
    )
  }

  return (
    <div className="w-full h-full relative">
      <GoogleMap
        mapContainerStyle={mapContainerStyle}
        center={center}
        zoom={zoom}
        options={mapOptions}
        onLoad={onLoad}
        onUnmount={onUnmount}
      >
        {filteredItems.map((item) => (
          <Marker
            key={item.id}
            position={item.location}
            icon={getMarkerIcon(item.type, selectedItem?.id === item.id)}
            onClick={() => handleMarkerClick(item)}
            animation={selectedItem?.id === item.id ? google.maps.Animation.BOUNCE : undefined}
          />
        ))}

        {selectedItem && (
          <InfoWindow
            position={selectedItem.location}
            onCloseClick={handleInfoWindowClose}
            options={{
              pixelOffset: new google.maps.Size(0, -10)
            }}
          >
            <div className="max-w-xs p-2">
              <div className="relative">
                <img
                  src={selectedItem.image}
                  alt={selectedItem.title}
                  className="w-full h-32 object-cover rounded-lg mb-2"
                />
                <div className="absolute top-2 left-2 bg-white px-2 py-1 rounded-full text-xs font-medium flex items-center gap-1">
                  {React.createElement(getTypeIcon(selectedItem.type), { className: "w-3 h-3" })}
                  {getTypeLabel(selectedItem.type)}
                </div>
              </div>
              
              <h3 className="font-semibold text-gray-900 mb-1">{selectedItem.title}</h3>
              <p className="text-sm text-gray-600 mb-2">{selectedItem.description}</p>
              
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-1">
                  {Array.from({ length: getLuxuryStars(selectedItem.luxuryLevel) }).map((_, i) => (
                    <Star key={i} className="w-3 h-3 fill-yellow-400 text-yellow-400" />
                  ))}
                  <span className="text-xs text-gray-500 ml-1 capitalize">{selectedItem.luxuryLevel}</span>
                </div>
                <div className="flex items-center gap-1 text-lg font-bold text-gray-900">
                  <Euro className="w-4 h-4" />
                  {selectedItem.price.toLocaleString()}
                </div>
              </div>

              {selectedItem.rating && (
                <div className="flex items-center gap-1 mt-1">
                  <Star className="w-3 h-3 fill-blue-500 text-blue-500" />
                  <span className="text-xs text-gray-600">{selectedItem.rating}/5</span>
                </div>
              )}

              <button 
                onClick={() => onItemSelect?.(selectedItem)}
                className="w-full mt-3 bg-blue-600 text-white px-3 py-2 rounded-lg text-sm font-medium hover:bg-blue-700 transition-colors"
              >
                Voir les détails
              </button>
            </div>
          </InfoWindow>
        )}
      </GoogleMap>

      {/* Map Controls */}
      <div className="absolute top-4 left-4 bg-white rounded-lg shadow-lg p-2">
        <div className="text-xs font-medium text-gray-700 mb-2">
          {filteredItems.length} résultat{filteredItems.length > 1 ? 's' : ''}
        </div>
        <div className="flex flex-col gap-1">
          {['flight', 'yacht', 'car', 'hotel'].map(type => {
            const count = filteredItems.filter(item => item.type === type).length
            const Icon = getTypeIcon(type)
            return count > 0 ? (
              <div key={type} className="flex items-center gap-2 text-xs">
                <Icon className="w-3 h-3" />
                <span>{getTypeLabel(type)}: {count}</span>
              </div>
            ) : null
          })}
        </div>
      </div>
    </div>
  )
}