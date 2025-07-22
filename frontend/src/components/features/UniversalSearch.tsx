'use client'

import { useState } from 'react'
import { Search, MapPin, Calendar, Users, Star, Plane, Anchor, Car, Building } from 'lucide-react'

type VehicleType = 'flights' | 'yachts' | 'cars' | 'hotels'

interface SearchFilters {
  vehicleType: VehicleType
  location: string
  startDate: string
  endDate?: string
  guests: number
  luxuryLevel?: string
}

export function UniversalSearch() {
  const [activeTab, setActiveTab] = useState<VehicleType>('flights')
  const [filters, setFilters] = useState<SearchFilters>({
    vehicleType: 'flights',
    location: '',
    startDate: '',
    endDate: '',
    guests: 1,
    luxuryLevel: ''
  })

  const vehicleTypes = [
    { 
      key: 'flights' as VehicleType, 
      label: 'Jets Privés', 
      icon: Plane,
      color: 'bg-blue-500 text-white',
      placeholder: 'Paris → Nice'
    },
    { 
      key: 'yachts' as VehicleType, 
      label: 'Yachts', 
      icon: Anchor,
      color: 'bg-cyan-500 text-white',
      placeholder: 'Monaco, Cannes, Saint-Tropez'
    },
    { 
      key: 'cars' as VehicleType, 
      label: 'Voitures', 
      icon: Car,
      color: 'bg-gray-800 text-white',
      placeholder: 'Paris, Lyon, Marseille'
    },
    { 
      key: 'hotels' as VehicleType, 
      label: 'Hôtels', 
      icon: Building,
      color: 'bg-amber-500 text-white',
      placeholder: 'Paris, Londres, New York'
    }
  ]

  const luxuryLevels = [
    { value: '', label: 'Tous niveaux' },
    { value: 'standard', label: 'Standard' },
    { value: 'premium', label: 'Premium' },
    { value: 'luxury', label: 'Luxe' },
    { value: 'ultra-luxury', label: 'Ultra Luxe' },
    { value: 'exclusive', label: 'Exclusif' }
  ]

  const handleSearch = () => {
    console.log('Searching with filters:', { ...filters, vehicleType: activeTab })
    // TODO: Implement search functionality
  }

  const handleTabChange = (vehicleType: VehicleType) => {
    setActiveTab(vehicleType)
    setFilters(prev => ({ ...prev, vehicleType }))
  }

  const currentVehicle = vehicleTypes.find(v => v.key === activeTab)

  return (
    <div className="bg-white rounded-2xl shadow-2xl p-8 max-w-6xl mx-auto">
      {/* Vehicle Type Tabs */}
      <div className="flex flex-wrap gap-2 mb-8">
        {vehicleTypes.map((vehicle) => {
          const Icon = vehicle.icon
          const isActive = activeTab === vehicle.key
          
          return (
            <button
              key={vehicle.key}
              onClick={() => handleTabChange(vehicle.key)}
              className={`flex items-center gap-2 px-6 py-3 rounded-full font-medium transition-all duration-200 ${
                isActive 
                  ? vehicle.color + ' shadow-lg transform scale-105' 
                  : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              }`}
            >
              <Icon className="w-5 h-5" />
              {vehicle.label}
            </button>
          )
        })}
      </div>

      {/* Search Form */}
      <div className="grid grid-cols-1 lg:grid-cols-6 gap-6">
        {/* Location */}
        <div className="lg:col-span-2">
          <label className="block text-sm font-semibold text-gray-700 mb-2">
            <MapPin className="w-4 h-4 inline mr-1" />
            {activeTab === 'flights' ? 'Trajet' : 'Destination'}
          </label>
          <input
            type="text"
            placeholder={currentVehicle?.placeholder}
            value={filters.location}
            onChange={(e) => setFilters(prev => ({ ...prev, location: e.target.value }))}
            className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          />
        </div>

        {/* Start Date */}
        <div>
          <label className="block text-sm font-semibold text-gray-700 mb-2">
            <Calendar className="w-4 h-4 inline mr-1" />
            {activeTab === 'flights' ? 'Départ' : activeTab === 'hotels' ? 'Arrivée' : 'Début'}
          </label>
          <input
            type="date"
            value={filters.startDate}
            onChange={(e) => setFilters(prev => ({ ...prev, startDate: e.target.value }))}
            className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          />
        </div>

        {/* End Date (for multi-day rentals) */}
        {activeTab !== 'flights' && (
          <div>
            <label className="block text-sm font-semibold text-gray-700 mb-2">
              <Calendar className="w-4 h-4 inline mr-1" />
              {activeTab === 'hotels' ? 'Départ' : 'Fin'}
            </label>
            <input
              type="date"
              value={filters.endDate || ''}
              onChange={(e) => setFilters(prev => ({ ...prev, endDate: e.target.value }))}
              className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
        )}

        {/* Guests */}
        <div>
          <label className="block text-sm font-semibold text-gray-700 mb-2">
            <Users className="w-4 h-4 inline mr-1" />
            {activeTab === 'flights' ? 'Passagers' : activeTab === 'hotels' ? 'Clients' : 'Invités'}
          </label>
          <select
            value={filters.guests}
            onChange={(e) => setFilters(prev => ({ ...prev, guests: parseInt(e.target.value) }))}
            className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          >
            {Array.from({ length: 12 }, (_, i) => i + 1).map(num => (
              <option key={num} value={num}>{num}</option>
            ))}
          </select>
        </div>

        {/* Luxury Level */}
        <div>
          <label className="block text-sm font-semibold text-gray-700 mb-2">
            <Star className="w-4 h-4 inline mr-1" />
            Niveau de luxe
          </label>
          <select
            value={filters.luxuryLevel || ''}
            onChange={(e) => setFilters(prev => ({ ...prev, luxuryLevel: e.target.value }))}
            className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          >
            {luxuryLevels.map(level => (
              <option key={level.value} value={level.value}>{level.label}</option>
            ))}
          </select>
        </div>

        {/* Search Button */}
        <div className="flex items-end">
          <button
            onClick={handleSearch}
            className="w-full bg-gradient-to-r from-blue-600 to-purple-600 text-white px-6 py-3 rounded-lg font-semibold hover:from-blue-700 hover:to-purple-700 transition-all duration-200 flex items-center justify-center gap-2 shadow-lg hover:shadow-xl transform hover:scale-105"
          >
            <Search className="w-5 h-5" />
            Rechercher
          </button>
        </div>
      </div>

      {/* Quick Filters */}
      <div className="mt-6 pt-6 border-t border-gray-200">
        <div className="flex flex-wrap gap-2">
          <span className="text-sm font-medium text-gray-600 mr-4">Suggestions populaires:</span>
          {activeTab === 'flights' && (
            <>
              <button className="px-3 py-1 text-xs bg-blue-100 text-blue-700 rounded-full hover:bg-blue-200 transition-colors">
                Paris → Nice
              </button>
              <button className="px-3 py-1 text-xs bg-blue-100 text-blue-700 rounded-full hover:bg-blue-200 transition-colors">
                Londres → Monaco
              </button>
              <button className="px-3 py-1 text-xs bg-blue-100 text-blue-700 rounded-full hover:bg-blue-200 transition-colors">
                Genève → Courchevel
              </button>
            </>
          )}
          {activeTab === 'yachts' && (
            <>
              <button className="px-3 py-1 text-xs bg-cyan-100 text-cyan-700 rounded-full hover:bg-cyan-200 transition-colors">
                Côte d'Azur
              </button>
              <button className="px-3 py-1 text-xs bg-cyan-100 text-cyan-700 rounded-full hover:bg-cyan-200 transition-colors">
                Îles Grecques
              </button>
              <button className="px-3 py-1 text-xs bg-cyan-100 text-cyan-700 rounded-full hover:bg-cyan-200 transition-colors">
                Baléares
              </button>
            </>
          )}
          {activeTab === 'cars' && (
            <>
              <button className="px-3 py-1 text-xs bg-gray-100 text-gray-700 rounded-full hover:bg-gray-200 transition-colors">
                Ferrari
              </button>
              <button className="px-3 py-1 text-xs bg-gray-100 text-gray-700 rounded-full hover:bg-gray-200 transition-colors">
                Lamborghini
              </button>
              <button className="px-3 py-1 text-xs bg-gray-100 text-gray-700 rounded-full hover:bg-gray-200 transition-colors">
                Rolls-Royce
              </button>
            </>
          )}
          {activeTab === 'hotels' && (
            <>
              <button className="px-3 py-1 text-xs bg-amber-100 text-amber-700 rounded-full hover:bg-amber-200 transition-colors">
                Paris 5★
              </button>
              <button className="px-3 py-1 text-xs bg-amber-100 text-amber-700 rounded-full hover:bg-amber-200 transition-colors">
                Dubai 6★
              </button>
              <button className="px-3 py-1 text-xs bg-amber-100 text-amber-700 rounded-full hover:bg-amber-200 transition-colors">
                Maldives
              </button>
            </>
          )}
        </div>
      </div>
    </div>
  )
}